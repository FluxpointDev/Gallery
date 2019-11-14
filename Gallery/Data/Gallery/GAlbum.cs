using Gallery.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public void Add()
        {
            DB.R.Db("Gallery").Table("Albums").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            DB.R.Db("Gallery").Table("Albums").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
}
