using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gallery.Data;
using System.Drawing;
using Blazor.FileReader;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Gallery.Database;

namespace Gallery
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddFileReaderService(options => options.InitializeOnFirstCall = true);
            services.AddSingleton<RethinkUpdaterService>();
            services.AddServerSideBlazor().AddHubOptions(o =>
            {
                o.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
            });
            services.AddServerSideBlazor();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })

           .AddCookie(options =>
           {
               options.Cookie.Name = "session";
               options.ExpireTimeSpan = new TimeSpan(3, 0, 0, 0);
               options.LoginPath = "/login";
               options.LogoutPath = "/logout";
           })

           .AddDiscord(options =>
           {
               options.ClientId = Config.Tokens["discord.id"];
               options.ClientSecret = Config.Tokens["discord.secret"];
               options.Scope.Add("identify");
           });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            if (Config.Tokens.ContainsKey("rethink.ip"))
            {
                DB.Start();
                RethinkUpdaterService RU = app.ApplicationServices.GetService<RethinkUpdaterService>();
                RU.Start();
            }

            Image bm = Image.FromFile("c:/Global/Image.png");
            string Hash = Program.getMd5Hash(Program.imageToByteArray(bm));
            Console.WriteLine("Hash " + Hash);
            Console.WriteLine("ID " + Program.Gen.CreateId());
            Console.WriteLine("ID " + Program.Gen.CreateId());

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
