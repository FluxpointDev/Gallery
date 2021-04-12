using Gallery.Database;

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
                DB.R.Db(Program.DatabaseName).Table("Roles").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            if (!Config.DevMode)
                DB.R.Db(Program.DatabaseName).Table("Roles").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
}
