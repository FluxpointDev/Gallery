using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Shared
{
    public class Meta
    {
        public static Dictionary<string, Meta> List = new Dictionary<string, Meta>
        {
            { "/", new Meta { } }
        };
        public string Name = "";
        public string Desc = "Gallery for cute anime images, nekos and misc content with an api. https://fluxpoint.dev/api";
        public string Image = "https://gallery.fluxpoint.dev/icon.jpg";
        public string Path = "";

        public string GetName()
        {
            if (Name == "")
                return "Fluxpoint Gallery";
            return Name;
        }

        public string GetUrl(string path)
        {
            return "https://fluxpoint.dev" + path;
        }
    }
}
