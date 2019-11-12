using Gallery.Data;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Gallery.Database
{
    public static class DB
    {
        public static RethinkDB R = RethinkDB.R;
        public static Connection Con;
        public static bool Connected = false;
        public static Dictionary<string, RethinkUser> Users = new Dictionary<string, RethinkUser>();
        public static Dictionary<string, GUser> GalleryUsers = new Dictionary<string, GUser>();

        public static Task Start()
        {
            Console.WriteLine("Connecting to DB");
            Con = R.Connection()
            .Hostname(Config.Tokens["rethink.ip"])
            .Port(int.Parse(Config.Tokens["rethink.port"]))
            .AuthKey(Config.Tokens["rethink.auth"])
            .Db("Global")
            .Timeout(60)
            .Connect();
            Con.CheckOpen();
            //Cursor<RethinkBot> botlist = R.Db("Global").Table("Bots").RunCursor<RethinkBot>(Con);
            //Bots = botlist.ToDictionary(x => x.ID, x => x);
            Connected = true;
            return Task.CompletedTask;
        }
        public static class User
        {
            private static RethinkUser New(string id, string name)
            {
                RethinkUser User = new RethinkUser(id, name);
                R.Table("Users").Insert(User).RunNoReply(Con);
                return User;
            }

            public static async Task<RethinkUser> Get(string id, string name, bool gen)
            {
                if (Users.TryGetValue(id, out RethinkUser U))
                    return U;
                RethinkUser User = await R.Db("Global").Table("Users").Get(id).RunAtomAsync<RethinkUser>(Con);
                if (!gen && User == null)
                    return null;

                if (User == null)
                    User =  New(id, name);
                Users.Add(id, User);
                if (User.Name != name)
                    User.Update();
                return User;
            }
        }
    }
}
