using Gallery;
using Gallery.Data;
using Gallery.Database;
using Gallery.DiscordAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discord.OAuth2
{
    internal class DiscordHandler : OAuthHandler<DiscordOptions>
    {
        public DiscordHandler(IOptionsMonitor<DiscordOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await Backchannel.SendAsync(request, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to retrieve Discord user information ({response.StatusCode}).");
            string UserJson = await response.Content.ReadAsStringAsync();
            UserInfoJson User = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoJson>(UserJson);
            RethinkUser DBUser = await DB.User.Get(User.id, $"{User.username}#{User.discriminator}", true);

            using var payload = JsonDocument.Parse(UserJson);
            OAuthCreatingTicketContext context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);
            context.RunClaimActions();
            if (DBUser.Blacklisted)
            {
                context.Properties.RedirectUri = "/logout?banned";
                return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
            }
            identity.AddClaim(new Claim("user", UserJson));
            if (User.id == Config.OwnerID.ToString())
                identity.AddClaim(new Claim(ClaimTypes.Role, "owner"));
            else if (DBUser.Admin)
                identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
            else if (DBUser.Donator)
                identity.AddClaim(new Claim(ClaimTypes.Role, "donator"));
            else
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
           
            if (!DB.GalleryUsers.ContainsKey(User.id))
            {
                GUser GUser = new GUser
                {
                    id = User.id
                };
                if (DBUser.Donator)
                    GUser.upload.imageLimit = 50;
                DB.GalleryUsers.Add(User.id, GUser);
                DB.R.Db("Gallery").Table("Users").Insert(GUser).RunNoReply(DB.Con);
            }
                
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }
    }
}
