using Gallery.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GUser
    {
        public string id;
        public string name;
        public List<int> roles = new List<int>();
        public UploadSet upload = new UploadSet();
        public class UploadSet
        {
            public int imageLimit = 0;
            // In mb
            public int sizeLimit = 200;
        }

        public void Add()
        {
            if (!Config.DevMode)
                DB.R.Db("Gallery").Table("Users").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            if (!Config.DevMode)
                DB.R.Db("Gallery").Table("Users").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
}
