using Gallery.Database;
using System;
using System.Collections.Generic;

namespace Gallery.Data
{
    public class GImage
    {
        public string id;
        public string name;
        public string link = "";
        public string source = "";
        public string author = "";
        public HashSet<int> tags = new HashSet<int>();
        public int album = 0;
        public DateTime date = DateTime.UtcNow;
        public FileInfo file = new FileInfo();

        public bool IsNsfw()
        {
            if (DB.Albums.TryGetValue(album, out GAlbum AB))
                return AB.isNsfw;
                return false;
        }
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

        public HashSet<GTag> GetTags()
        {
            HashSet<GTag> Tags = new HashSet<GTag>();
            foreach(int t in tags)
            {
                if (DB.Tags.TryGetValue(t, out GTag tag))
                    Tags.Add(tag);
            }
            return Tags;
        }

        public string GetImage(imageType type)
        {
            switch (type)
            {
                case imageType.Full:
                    return "https://img.fluxpoint.dev/" + id + "." + file.type;
                case imageType.Medium:
                    return "https://img.fluxpoint.dev/med/" + id + ".webp";
                case imageType.Thumbnail:
                    return "https://img.fluxpoint.dev/med/" + id + ".webp";
                case imageType.Download:
                    return "https://img.fluxpoint.dev/dl/" + id + "." + file.type;
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
                    return Config.GlobalPath + "med/" + id + ".webp";
                case imageType.Thumbnail:
                    return Config.GlobalPath + "med/" + id + ".webp";
            }
            return "";
        }

        public bool IsImageType(bool gif)
        {
            if (gif)
                return file.type == "gif";
            return file.type != "gif";
        }

        public void Add()
        {
            if (!Config.DevMode)
                DB.R.Db(Program.DatabaseName).Table("Images").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            if (!Config.DevMode)
                DB.R.Db(Program.DatabaseName).Table("Images").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
    public enum imageType
    {
        Full, Medium, Thumbnail, Download
    }
}
