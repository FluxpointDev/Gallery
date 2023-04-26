using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gallery.Data
{
    public static class Config
    {
        public const string Website = "https://fluxpoint.dev";
        public const string Docs = "https://docs.fluxpoint.dev/api";
        public const string Twitter = "https://twitter.com/FluxpointDev";
        public const string Reddit = "https://www.reddit.com/r/FluxpointDev/";
        public const string Email = "support@fluxpoint.dev";
        public const string Discord = "https://discord.gg/fluxpoint";
        public const string Patreon = "https://patreon.com/fluxpointdev";
        public const ulong OwnerID = 190590364871032834;

        public static string ConfigPath { get; private set; }
        public static string GlobalPath = "";
        private static string ConfigFile => Path.Combine(ConfigPath, "GalleryConfig.json");
        public static Dictionary<string, string> Tokens { get; } = new Dictionary<string, string>();
        public static bool DevMode = false;
        public static void SetConfig()
        {
            if (DevMode)
            {
                ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DiscordBots/Gallery/");
                GlobalPath = "C:/Global/Website/";
            }
            else
            {
                ConfigPath = "/root/config/";
                GlobalPath = "/home/website/gallery/";
            }
            Console.Title = DevMode ? $"[DEV] Gallery" : "Gallery";

            if (!Directory.Exists(ConfigPath))
            {
                Directory.CreateDirectory(ConfigPath);
                return;
            }

            JObject json = null;
            if (!File.Exists(ConfigFile))
            {
                File.WriteAllText(ConfigFile, "");
                return;
            }
            else
            {
                try
                {
                    using var reader = new StreamReader(ConfigFile);
                    var configContents = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(configContents)) return;
                    json = JObject.Parse(configContents);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }

            foreach (JProperty jp in json.Properties())
            {
                string root = jp.Name.ToLower();
                if (jp.Values().Count() == 1)
                {
                    Tokens.Add(root, jp.Value.ToString());
                }
                else
                {
                    foreach (JProperty p in jp.Value)
                    {
                        Tokens.Add($"{root}.{p.Name.ToLower()}", p.Value.ToString());
                    }
                }

            }
        }
    }
}
