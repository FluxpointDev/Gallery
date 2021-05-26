using Gallery.Database;

namespace Gallery.Data
{
    public class GTag
    {
        public int id;
        public string name;
        public bool isNsfw = false;
        public string description = "";

        public void Add()
        {
            if (!Config.DevMode)
                DB.R.Db(Program.DatabaseName).Table("Tags").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            if (!Config.DevMode)
                DB.R.Db(Program.DatabaseName).Table("Tags").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
}
