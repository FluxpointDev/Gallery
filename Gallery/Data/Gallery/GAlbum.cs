using Gallery.Database;
using Gallery.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Gallery.Data
{
    public class GAlbum
    {
        public int id;
        public string name = "";
        public bool isPublic = false;
        public bool LoginOnly = false;
        public bool isNsfw = false;
        public string owner = "";
        public Dictionary<string, GalleryAccess> access = new Dictionary<string, GalleryAccess>();
        public string thumbnailImage = "";
        public bool isSubAlbum = false;
        public bool isAPIOnly = false;
        public int subAlbum = 0;

        public string GetThumbnailImage()
        {
            if (thumbnailImage == "")
                return "";
            if (DB.Images.TryGetValue(thumbnailImage, out GImage img))
                return img.GetImage(imageType.Thumbnail);
            return "";
        }

        public bool HasAccess(Session session)
        {
            if (session.State.User.GetId() == "190590364871032834")
                return true;
            if (isNsfw)
            {
                if (session.Nsfw)
                    return true;
                return false;
            }
            if (LoginOnly && !session.State.User.Identity.IsAuthenticated)
                return false;
            return HasAccess(session.State.User.GetId());
        }

        public bool HasAccess(string id, bool api = false)
        {
            if (id == "190590364871032834")
                return true;
            if (isPublic)
                return true;
            if (id == owner)
                return true;
            if (access.TryGetValue(id, out GalleryAccess ga))
            {
                if (api && ga == GalleryAccess.Read)
                    return false;
                return true;
            }
            return false;
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

        public enum GalleryAccess
        {
            Read, ReadAPI, Upload, Manage
        }
    }
}
