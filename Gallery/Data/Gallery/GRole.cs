using Gallery.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GRole
    {
        public int id;
        public string name;
        public string description = "";
        public bool isAdmin = false;
        public bool isMod = false;
        public bool isApprover = false;

        public void Add()
        {
            if (!Config.DevMode)
                DB.R.Db("Gallery").Table("Roles").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            if (!Config.DevMode)
                DB.R.Db("Gallery").Table("Roles").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
}
