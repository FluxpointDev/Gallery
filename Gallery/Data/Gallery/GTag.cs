using Gallery.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GTag
    {
        public int id;
        public string link;
        public string name;
        public bool isNsfw = false;
        public bool isCharacter = false;
        public string description = "";

        public void Add()
        {
            if (!Config.DevMode)
                DB.R.Db("Gallery").Table("Tags").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            if (!Config.DevMode)
                DB.R.Db("Gallery").Table("Tags").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
}
