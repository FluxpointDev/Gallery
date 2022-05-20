using Gallery.Database;
using System.Collections.Generic;

namespace Gallery.Data
{
    public class Endpoint
    {
        public int id;
        public bool isDisabled;
        public bool isNsfw;
        public bool isGifs;
        public string path;
        public Dictionary<int, char> albums = new Dictionary<int, char>();
        public Dictionary<int, char> tags = new Dictionary<int, char>();

        public string GetPath()
        {
            string Type = isGifs ? "gif/" : "img/";
            return isNsfw ? "/nsfw/" + Type + path : "/sfw/" + Type + path;
        }

        public void Update()
        {
            if (!Config.DevMode)
                DB.R.Db(Program.DatabaseName).Table("Endpoints").Get(id).Update(this).RunNoReply(DB.Con);

        }
    }
}
