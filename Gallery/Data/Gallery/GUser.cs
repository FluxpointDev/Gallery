using Gallery.Database;
using System.Collections.Generic;

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
            public bool enabled = false;
            public int imageLimit = 0;
            // In mb
            public int sizeLimit = 200;
        }

        public void Add()
        {
            if (!Config.DevMode)
                DB.R.Db(Program.DatabaseName).Table("Users").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            if (!Config.DevMode)
                DB.R.Db(Program.DatabaseName).Table("Users").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
}
