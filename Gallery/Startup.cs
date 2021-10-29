using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gallery.Data;
using Blazor.FileReader;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Gallery.Database;
using EmbeddedBlazorContent;
using Microsoft.AspNetCore.ResponseCompression;
using BlazorContextMenu;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MatBlazor;
using ImageMagick;
using System.IO;
using RethinkDb.Driver.Model;

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
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddMatBlazor();
            services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.BottomRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = false;
                config.MaximumOpacity = 100;
            });
            services.AddSingleton<RethinkUpdaterService>();
            services.AddSingleton<System.Net.Http.HttpClient>();
            services.AddServerSideBlazor(configure =>
            {
                configure.DetailedErrors = true;
            }).AddHubOptions(o =>
            {
                o.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
            });

            services.AddFileReaderService(options => options.InitializeOnFirstCall = true);
            services.AddBlazoredModal();
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
               options.Events.OnRedirectToAuthorizationEndpoint = context =>
               {
                   context.Request.QueryString.Add("prompt", "none");
                   context.HttpContext.Response.Redirect(context.RedirectUri);
                   return Task.FromResult(0);
               };
               options.Events.OnAccessDenied = context =>
               {
                   Console.WriteLine("ACCESS DENIED\n\n\n");
                   context.Response.Redirect(context.ReturnUrl);
                   context.SkipHandler();
                   return Task.FromResult(0);
               };
               options.Events.OnRemoteFailure = context =>
               {
                   Console.WriteLine("ERROR OCCURED\n\n\n");
                   if (context.Failure.Message == "Correlation failed")
                       context.Response.Redirect(context.Properties.RedirectUri);
                   else
                       context.Response.Redirect($"/error");
                   context.SkipHandler();
                   return Task.FromResult(0);
               };
               options.CorrelationCookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
           });
            services.AddBlazorContextMenu(options =>
            {
                options.ConfigureTemplate("dark", template =>
                {
                    template.MenuCssClass = "dark-menu";
                    template.MenuItemCssClass = "dark-menu-item";
                    template.MenuItemDisabledCssClass = "dark-menu-item--disabled";
                    template.MenuItemWithSubMenuCssClass = "dark-menu-item--with-submenu";
                    template.Animation = Animation.FadeIn;
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine(env.IsDevelopment());
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
            _ = Task.Run(() =>
            {
                foreach(var i in System.IO.Directory.GetFiles(Config.GlobalPath + "med/"))
                {
                    if (!i.EndsWith(".webp"))
                    {
                        Console.WriteLine("Converted: " + i);
                        string Split = i.Split('/').Last();
                        MagickFormat Format = MagickFormat.Png;
                        if (Split.EndsWith(".jpg") || Split.EndsWith(".jpeg"))
                            Format = MagickFormat.Jpg;
                        using (MagickImage image = new MagickImage(new MemoryStream(File.ReadAllBytes(i)), new MagickReadSettings { ColorType = ColorType.Optimize, Format = Format }))
                        {
                            image.Strip();
                            image.Resize(792, 594);
                            image.Write(Config.GlobalPath + "med/" + Split.Split('.').First() + ".webp", MagickFormat.WebP);
                        }
                        System.IO.File.Delete(i);
                    }
                }
            });
            app.UseResponseCompression();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseEmbeddedBlazorContent(typeof(MatBlazor.BaseMatComponent).Assembly);
            app.UseStaticFiles();
            app.UseAuthentication();
            
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
