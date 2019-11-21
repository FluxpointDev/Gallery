using Gallery.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GImage
    {
        public string id;
        public string name;
        public string link = "";
        public string source = "";
        public string author = "";
        public List<int> tags = new List<int>();
        public int album = 0;
        public DateTime date = DateTime.UtcNow;
        public FileInfo file = new FileInfo();
        public class FileInfo
        {
            public string type;
            public string hash;
            public int width;
            public int height;
            public long size;
        }
       
        public Dictionary<string, GComment> comments = new Dictionary<string, GComment>();

        public Options options = new Options();
        public class Options
        {
            public bool isHidden = false;
            public bool isDeleted = false;
            public bool allowApi = true;
        }

        public string GetImage(imageType type)
        {
            switch (type)
            {
                case imageType.Full:
                    return "https://img.fluxpoint.dev/" + id + "." + file.type;
                case imageType.Medium:
                    return "https://img.fluxpoint.dev/med/" + id + "." + file.type;
                case imageType.Thumbnail:
                    return "https://img.fluxpoint.dev/thm/" + id + "." + file.type; 
            }
            return "";
        }

        public string GetFile(imageType type)
        {
            switch (type)
            {
                case imageType.Full:
                    return Config.GlobalPath + "img/" + id + "." + file.type;
                case imageType.Medium:
                    return Config.GlobalPath + "med/" + id + "." + file.type;
                case imageType.Thumbnail:
                    return Config.GlobalPath + "thm/" + id + "." + file.type;
            }
            return "";
        }

        public void Add()
        {
            if (!Config.DevMode)
                DB.R.Db("Gallery").Table("Images").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            if (!Config.DevMode)
                DB.R.Db("Gallery").Table("Images").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
    public enum imageType
    {
        Full, Medium, Thumbnail
    }
}
