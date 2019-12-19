using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Gallery.Data;
using Gallery.Extensions;
using IdGen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Gallery
{
    public class Program
    {
        public static Random rng = new Random(Convert.ToInt32(DateTime.Now.Ticks & 0x000000007FFFFFFF));

        public static void Main(string[] args)
        {

#if DEBUG
            DisableConsoleQuickEdit.Go();
            Config.DevMode = true;
#endif
            Config.SetConfig();
            CreateHostBuilder(args).Build().Run();
        }

        public static IdGenerator Gen = new IdGenerator(0, new DateTime(2019, 11, 14, 9, 0, 0, DateTimeKind.Utc));

        public static string getMd5Hash(byte[] buffer)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(buffer);

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static byte[] imageToByteArray(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Bmp);
            return ms.ToArray();
        }

        public static HttpClient Http = new HttpClient();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
