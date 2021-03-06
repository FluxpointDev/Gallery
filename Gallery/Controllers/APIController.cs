﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Gallery.Data;
using Gallery.Database;
using System.Threading.Tasks;
using System;
using ImageMagick;
using System.IO;
using System.Drawing;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gallery.Controllers
{
    public class ApiImage : Response
    {
        public ApiImage() : base(200, "")
        {

        }

        public string file = "";
    }

    [Route("/api/[controller]"), ApiController, IsAuthenticated]
    public class APIController : MyController
    {
        [HttpGet("/api")]
        public ActionResult Index()
        {

            if (!Request.Headers.ContainsKey("Authorization") || !DB.Keys.TryGetValue(Request.Headers["Authorization"][0], out ApiUser User))
                return Ok(new HomeResponse("Hello user"));
            else
                return Ok(new HomeResponse($"Hello {User.Name} - {User.ID}"));
        }

        [HttpGet("/api/list")]
        public IActionResult ListAlbums()
        {
            return Ok();
        }

        [HttpGet("/api/album/{id}")]
        public IActionResult GetAlbum(int id)
        {
            if (!DB.Keys.TryGetValue(Request.Headers["Authorization"][0], out ApiUser User))
            {
                return Unauthorized();
                
            }
            if (!DB.Albums.TryGetValue(id, out GAlbum album))
                return BadRequest("Unknown album");

            if (!album.isPublic && !album.HasAccess(User.ID, true))
                return BadRequest("This album is private");

            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == id);
            int ID = Program.RNGBetween(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/tag/{id}")]
        public IActionResult GetTag(int id)
        {
            if (!DB.Tags.TryGetValue(id, out GTag tag))
                return BadRequest("Unknown Tag");

            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.tags.Contains(id) && x.file.type != "gif");
            int ID = Program.RNGBetween(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/sfw/gif/{type}")]
        public IActionResult GetAction(string type)
        {
            int Album = 0;
            switch (type.ToLower())
            {
                case "hug":
                    Album = 21;
                    break;
                case "pat":
                    Album = 22;
                    break;
                case "feed":
                    Album = 23;
                    break;
                case "bite":
                    Album = 24;
                    break;
                case "highfive":
                    Album = 25;
                    break;
                case "kiss":
                    Album = 26;
                    break;
                case "lick":
                    Album = 27;
                    break;
                case "poke":
                    Album = 28;
                    break;
                case "punch":
                    Album = 29;
                    break;
                case "slap":
                    Album = 30;
                    break;
                case "tickle":
                    Album = 31;
                    break;
                case "wasted":
                    Album = 32;
                    break;
                case "wink":
                    Album = 33;
                    break;
                case "wag":
                    Album = 34;
                    break;
                case "fluff":
                    Album = 35;
                    break;
                case "baka":
                    Album = 36;
                    break;
                case "smug":
                    Album = 37;
                    break;
            }
            if (Album != 0)
            {
                IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == Album);
                int ID = Program.RNGBetween(0, List.Count() - 1);
                string File = List.ElementAt(ID).GetImage(imageType.Full);
                return Ok(new ApiImage { file = File });
            }
            return BadRequest("Invalid action type.");
        }

        [HttpGet("/api/sfw/anime")]
        public IActionResult GetSfwAnime()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 5);
            int ID = Program.RNGBetween(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/sfw/wallpaper")]
        public IActionResult GetSfwWallpaper()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 6);
            int ID = Program.RNGBetween(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/sfw/azurlane")]
        public IActionResult GetSfwAzurlane()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 7);
            int ID = Program.RNGBetween(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/sfw/nekopara")]
        public IActionResult GetSfwNekopara()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 8);
            int ID = Program.RNGBetween(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/nsfw/azurlane")]
        public IActionResult GetNsfwAzurlane()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 11);
            int ID = Program.RNGBetween(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/nsfw/nekopara")]
        public IActionResult GetNsfwNekopara()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 10);
            int ID = Program.RNGBetween(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/nsfw/lewd")]
        public IActionResult GetNsfwLewd()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 12);
            int ID = Program.RNGBetween(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/upload/url")]
        public async Task<IActionResult> UploadUrl(string url, string album = "19")
        {
            if (!DB.Keys.TryGetValue(Request.Headers["Authorization"][0], out ApiUser User))
                return Unauthorized();
            if (!int.TryParse(album, out int albumid) || !DB.Albums.TryGetValue(albumid, out GAlbum GA))
                return BadRequest("Invalid album id");
            if (User.ID != "190590364871032834")
            {
                if (GA.owner == User.ID)
                { }
                else if (GA.access.TryGetValue(User.ID, out GAlbum.GalleryAccess access))
                {
                    if (access == GAlbum.GalleryAccess.Read || access == GAlbum.GalleryAccess.ReadAPI)
                        return Unauthorized();
                }
                else
                    return Unauthorized();
            }

            if (string.IsNullOrEmpty(url) || !url.StartsWith("https://"))
                return BadRequest();
            byte[] Bytes = null;
            try
            {
                Bytes = await Program.Http.GetByteArrayAsync(url);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex.Message);
            }
            if (Bytes.Length == 0)
                return BadRequest("Invalid image");
            string ImageName = url.Split('/').Last().Split('.').First();
            MagickFormat Format = MagickFormat.A;
            string FormatName = url.Split('.').Last().Split('?').First().ToLower();
            switch (FormatName)
            {
                case "png":
                    Format = MagickFormat.Png;
                    break;
                case "jpg":
                case "jpeg":
                    Format = MagickFormat.Jpg;
                    break;
                case "webp":
                    Format = MagickFormat.WebP;
                    break;
            }

            Size Size = GetSize(Bytes);
                string Hash = Program.getMd5Hash(Bytes);
            if (DB.HashSet.ContainsKey(Hash))
                return CustomStatus(409, "Already exist.");

            string ID = Program.Gen.CreateId().ToString();
            DB.HashSet.Add(Hash, ID);
           
            using (MagickImage image = new MagickImage(new MemoryStream(Bytes), new MagickReadSettings { ColorType = ColorType.Optimize, Format = Format, FrameIndex = 0, FrameCount = 1 }))
            {
                image.Strip();
                if (Format != MagickFormat.Gif)
                    image.Write(Config.GlobalPath + "img/" + ID + "." + FormatName, Format);

                image.Resize(792, 594);
                image.Write(Config.GlobalPath + "med/" + ID + "." + FormatName, Format);
                image.Resize(320, 320);
                image.Write(Config.GlobalPath + "thm/" + ID + "." + FormatName, Format);
            }
            GImage img = new GImage
            {
                id = ID,
                name = ImageName,
                author = "190590364871032834",
                date = DateTime.UtcNow,
                file = new GImage.FileInfo
                {
                    hash = Hash,
                    height = Size.Height,
                    width = Size.Width,
                    size = Bytes.Length,
                    type = FormatName
                },
                album = albumid
            };
            DB.Images.Add(ID, img);
            img.Add();
            return Ok();
        }

        public Size GetSize(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var image = System.Drawing.Image.FromStream(stream);

                return image.Size;
            }
        }
    }
}
