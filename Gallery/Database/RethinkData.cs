using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gallery.Database
{
    public class RethinkUser
    {
        public RethinkUser(string id = "", string name = "")
        {
            if (id != "")
            {
                ID = id;
                Name = name;
            }
        }
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("_Name")]
        public string Name = "";
        [JsonProperty("_Note")]
        public string Note = "";
        public bool Admin = false;
        public bool Donator = false;
        public bool Special = false;
        public bool Blacklisted = false;
        public bool Translator = false;
        public List<string> Upvotes = new List<string>();

        public UserProfile Profile = new UserProfile();
        public void Update()
        {
            DB.R.Table("Users").Get(ID).Update(this).RunNoReply(DB.Con);
        }
    }
    public class UserProfile
    {
        public string Description = "";
        public string Color = "#26262b";
        public string Nickname = "";
    }

    public class RethinkUserBlacklist
    {
        public void Set(ulong id, string reason = "")
        {
            ID = id.ToString();
            Reason = reason;
        }

        [JsonProperty("id")]
        public string ID;

        public string Name = "";
        public string Reason = "";

        public void Add()
        {
            DB.R.Table("User_Blacklist").Add(this).RunNoReply(DB.Con);
        }

        public void Delete()
        {
            RethinkUser User = DB.R.Table("Users").Get(ID).RunAtom<RethinkUser>(DB.Con);
            User.Blacklisted = false;
            try
            {
                DB.R.Table("User_Blacklist").Get(ID).Delete().Run(DB.Con);
            }
            catch { }
        }
    }
}
