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
        public static Dictionary<int, GAlbum> Albums = new Dictionary<int, GAlbum>();
        public static Dictionary<string, GImage> Images = new Dictionary<string, GImage>();
        public static Dictionary<int, GTag> Tags = new Dictionary<int, GTag>();

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
            Cursor<GUser> users = R.Db("Gallery").Table("Users").RunCursor<GUser>(Con);
            GalleryUsers = users.ToDictionary(x => x.id, x => x);

            Cursor<GAlbum> albums = R.Db("Gallery").Table("Albums").RunCursor<GAlbum>(Con);
            Albums = albums.ToDictionary(x => x.id, x => x);

            Cursor<GImage> images = R.Db("Gallery").Table("Images").RunCursor<GImage>(Con);
            Images = images.ToDictionary(x => x.id, x => x);

            Cursor<GTag> tags = R.Db("Gallery").Table("Tags").RunCursor<GTag>(Con);
            Tags = tags.ToDictionary(x => x.id, x => x);

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
