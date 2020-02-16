using Gallery.Data;
using Gallery.Database;
using Gallery.DiscordAuth;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gallery
{
    public static class AuthenticateStateExtensions
    {
        public static string GetId(this ClaimsPrincipal pr)
        {
            if (!pr.Identity.IsAuthenticated)
                return "0";
            return pr.Claims.ElementAt(0).Value;
        }

        public static UserInfoJson GetUser(this ClaimsPrincipal pr)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoJson>(pr.Claims.ElementAt(4).Value);
        }
        public static async Task<RethinkUser> GetData(this ClaimsPrincipal pr)
        {
            return await DB.User.Get(pr.GetId(), pr.GetFullName(), true);
        }

        public static async Task<GUser> GetGalleryUser(this ClaimsPrincipal pr)
        {
            if (DB.GalleryUsers.TryGetValue(pr.GetId(), out GUser User))
                return User;
            else
            {
                User = await DB.R.Db("Gallery").Table("Users").Get(pr.GetId()).RunAtomAsync<GUser>(DB.Con);
                DB.GalleryUsers.Add(pr.GetId(), User);
                return User;
            }
        }

        public static string GetName(this ClaimsPrincipal pr)
        {
            return pr.Claims.ElementAt(1).Value;
        }

        public static string GetFullName(this ClaimsPrincipal pr)
        {
            return $"{GetName(pr)}#{GetDiscrim(pr)}";
        }

        public static string GetDiscrim(this ClaimsPrincipal pr)
        {
            return pr.Claims.ElementAt(2).Value;
        }

        public static string GetRole(this ClaimsPrincipal pr)
        {
            return pr.Claims.ElementAt(5).Value;
        }

        public static string GetAvatarUrl(this ClaimsPrincipal pr, int size)
        {
            if (size == 0)
                return "https://cdn.discordapp.com/avatars/" + pr.Claims.ElementAt(0).Value + "/" + pr.Claims.ElementAt(3).Value + ".png";
            else
                return "https://cdn.discordapp.com/avatars/" + pr.Claims.ElementAt(0).Value + "/" + pr.Claims.ElementAt(3).Value + ".png?size=" + size.ToString();
        }
    }
}
