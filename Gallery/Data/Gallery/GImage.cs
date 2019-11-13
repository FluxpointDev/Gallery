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
        public string author;
        public List<int> tags = new List<int>();
        public List<int> albums = new List<int>();
        public DateTime date = DateTime.UtcNow;
        public string hash;
        public int width;
        public int height;
        public long filesize;
        public Dictionary<string, GComment> comments = new Dictionary<string, GComment>();
        public bool hidden = false;
        public bool deleted = false;
        public bool allowDiscord = true;
        public void Add()
        {
            DB.R.Db("Gallery").Table("Images").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            DB.R.Db("Gallery").Table("Images").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
}
