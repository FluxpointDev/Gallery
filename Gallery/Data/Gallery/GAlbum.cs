using Gallery.Database;
using System.Collections.Generic;
using System.Linq;

namespace Gallery.Data
{
    public class GAlbum
    {
        public int id;
        public string name = "";
        public bool isPublic = false;
        public bool isNsfw = false;
        public string owner = "";
        public string thumbnailImage = "";
        public bool isSubAlbum = false;
        public int subAlbum = 0;
        public Dictionary<int, GAccess> roleAccess = new Dictionary<int, GAccess>();

        public string GetThumbnailImage()
        {
            if (thumbnailImage == "")
                return "";
            if (DB.Images.TryGetValue(thumbnailImage, out GImage img))
                return img.GetImage(imageType.Thumbnail);
            return "";
        }

        public string GetComboxName()
        {
            if (name == "Nsfw")
                return "Nsfw";
            if (isNsfw)
                return name + " (Nsfw)";
            if (!isPublic)
                return name + " (Private)";
            return name;
        }

        public void Add()
        {
            if (!Config.DevMode)
                DB.R.Db(Program.DatabaseName).Table("Albums").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            if (!Config.DevMode)
                DB.R.Db(Program.DatabaseName).Table("Albums").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
}
