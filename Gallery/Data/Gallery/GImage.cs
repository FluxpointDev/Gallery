﻿using Gallery.Database;
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
            public bool allowDiscord = true;
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

        public void Add()
        {
            DB.R.Db("Gallery").Table("Images").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            DB.R.Db("Gallery").Table("Images").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
    public enum imageType
    {
        Full, Medium, Thumbnail
    }
}
