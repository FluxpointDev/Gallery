using Gallery.Data;
using Gallery.Database;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gallery.Controllers
{
    public class ApiImage : Response
    {
        public ApiImage(GImage img, bool isNsfw) : base(200, isNsfw ? "This image is NSFW!" : "")
        {
            if (img == null)
                return;
            id = img.id;
            file = img.GetImage(imageType.Full);
        }
        public string id = "";
        public string file = "";
    }
    public class ApiImages : Response
    {
        public ApiImages(HashSet<GImage> imges, bool isNsfw) : base(200, isNsfw ? "This image is NSFW!" : "")
        {
            if (imges == null)
                return;
            ids = imges.Select(x => x.id).ToArray();
            files = imges.Select(x => x.GetImage(imageType.Full)).ToArray();
        }
        public string[] ids;
        public string[] files;
    }

    [Route("/api/[controller]"), ApiController, IsAuthenticated]
    public class APIController : MyController
    {
        [HttpGet("/api")]
        public ActionResult Index()
        {
            if (!Request.Headers.ContainsKey("Authorization") || !DB.Keys.TryGetValue(Request.Headers["Authorization"][0], out ApiUser User))
                return Ok(new HomeResponse("Hello user, to get access to this API visit our website."));
            else
                return Ok(new HomeResponse($"Hello API user - {User.ID}"));
        }

        [Route("/api/{test1?}/{test2?}/{test3?}")]
        public ActionResult Fallback()
        {
            return NotFound($"This endpoint does not exist, read the docs {Config.Docs} ( G-7 )");
        }

        [Route("/http_not_allowed")]
        public ActionResult HttpNotAllowed()
        {
            return this.StatusCode(405, "You need to use https to use the API otherwise body content gets stripped from cloudflare redirect. ( G-6 )");
        }

        [Route("/api/gen/{image?}"), HttpGet, HttpPost]
        public IActionResult HintGen()
        {
            return BadRequest("You need to use api.fluxpoint.dev/gen/ instead. ( G-5 )");
        }

        [HttpGet("/api/test/error")]
        public IActionResult TestError()
        {
            throw new Exception("Test");
            return Ok("Test");
        }

        [HttpGet("/api/album/{id}")]
        public IActionResult GetAlbum(int id)
        {
            if (!DB.Keys.TryGetValue(Request.Headers["Authorization"][0], out ApiUser User))
            {
                return Unauthorized();

            }
            if (!DB.Albums.TryGetValue(id, out GAlbum album))
                return BadRequest("Unknown album. ( GL-1 )");

            if (!album.isPublic && !album.HasAccess(User.ID, true))
                return BadRequest("This album is private/restricted. ( GL-3 )");

            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == id);
            int ID = Program.RNGBetween(0, List.Count() - 1);
            return Ok(new ApiImage(List.ElementAt(ID), album.isNsfw));
        }

        [HttpGet("/api/tag/{id}")]
        public IActionResult GetTag(int id, string type = "all", string nsfw = "")
        {
            if (!DB.Tags.TryGetValue(id, out GTag tag))
                return BadRequest("Unknown tag. ( GL-2 )");

            HashSet<int> Albums = nsfw == "true" ? DB.Albums.Values.Where(x => x.isPublic && x.isNsfw).Select(x => x.id).ToHashSet() : DB.Albums.Values.Where(x => x.isPublic && !x.isNsfw).Select(x => x.id).ToHashSet();
            GImage[] List = DB.Images.Values.Where(x => Albums.Contains(x.album) && x.tags.Contains(id) && (type == "all" || x.IsImageType(type == "gif"))).ToArray();
            if (!List.Any())
                return BadRequest("This tag has no images. ( GL-5 )");
            int ID = Program.RNGBetween(0, List.Length - 1);
            return Ok(new ApiImage(List[ID], tag.isNsfw));
        }

        [HttpGet("/api/sfw/img/{type}")]
        public IActionResult GetSfw(string type, [FromQuery] int multi = 1)
        {
            if (!DB.EndpointCache.TryGetValue("/sfw/img/" + type, out int EndpintID))
            {
                return BadRequest("Invalid image category. ( GL-6)");
            }
            DB.Endpoints.TryGetValue(EndpintID, out Endpoint EN);
            if (EN == null)
                return BadRequest("API error, invalid image endpoint. ( Gl-7 )");
            if (EN.isNsfw)
                return BadRequest("Api error, invalid endpoint type. ( GL-7 )");

            IEnumerable<GImage> List = DB.Images.Values.Where(x => EN.albums.ContainsKey(x.album) || (x.tags.Any(x => EN.tags.ContainsKey(x)) && !x.IsNsfw()));
            if (!List.Any())
                return BadRequest("This tag/category has no images. ( GL-5 )");

            if (multi == 1)
            {
                int ID = Program.RNGBetween(0, List.Count() - 1);
                GImage Image = List.ElementAt(ID);
                if (Image.IsNsfw())
                    return BadRequest("Api error, invalid image type. ( GL-8 )");
                return Ok(new ApiImage(Image, false));
            }
            else
            {
                if (IsPublicUse)
                    return Unauthorized("You are not allowed to use this with the public token");

                DB.Keys.TryGetValue(Request.Headers["Authorization"][0], out ApiUser User);
                if (!(User.ID == "190590364871032834" || User.ID == "729873770990534766"))
                    return Unauthorized("You do not have access to multi image selection.");

                if (multi < 1)
                    return BadRequest("Multi parameter can't be less than 1");
                if (multi > 10)
                    return BadRequest("Multi parameter can't be more than 10");

                int MaxCount = List.Count();
                int MultiCount = multi;
                if (MaxCount < MultiCount)
                    MultiCount = MaxCount;

                MultiCount += 1;

                HashSet<GImage> Images = new HashSet<GImage>();
                while (Images.Count != multi)
                {
                    if (Images.Count == MaxCount)
                        break;
                    int ID = Program.RNGBetween(0, MaxCount - 1);
                    GImage Img = List.ElementAt(ID);
                    if (Images.Contains(Img))
                    {
                        Img = List.ElementAtOrDefault(ID);
                    }
                        
                    if (Img == null || Img.IsNsfw())
                    {
                        continue;
                    }
                    Images.Add(Img);
                }
                return Ok(new ApiImages(Images, false));
            }

        }

        [HttpGet("/api/sfw/gif/{type}")]
        public IActionResult GetSfwGif(string type, [FromQuery] int multi = 1)
        {
            if (!DB.EndpointCache.TryGetValue("/sfw/gif/" + type, out int EndpintID))
                return BadRequest("Invalid image category. ( GL-6 )");

            DB.Endpoints.TryGetValue(EndpintID, out Endpoint EN);
            if (EN == null)
                return BadRequest("API error, invalid image endpoint. ( GL-7 )");
            if (EN.isNsfw)
                return BadRequest("Api error, invalid endpoint type. ( GL-7 )");

            IEnumerable<GImage> List = DB.Images.Values.Where(x => EN.albums.ContainsKey(x.album) || (x.tags.Any(x => EN.tags.ContainsKey(x)) && !x.IsNsfw()));
            if (!List.Any())
                return BadRequest("This tag/category has no images. ( GL-5 )");

            if (multi == 1)
            {
                int ID = Program.RNGBetween(0, List.Count() - 1);
                GImage Image = List.ElementAt(ID);
                if (Image.IsNsfw())
                    return BadRequest("Api error, invalid image type. ( GL-8 )");
                return Ok(new ApiImage(Image, false));
            }
            else
            {
                if (IsPublicUse)
                    return Unauthorized("You are not allowed to use this with the public token");

                DB.Keys.TryGetValue(Request.Headers["Authorization"][0], out ApiUser User);
                if (!(User.ID == "190590364871032834" || User.ID == "729873770990534766"))
                    return Unauthorized("You do not have access to multi image selection.");

                if (multi < 1)
                    return BadRequest("Multi parameter can't be less than 1");
                if (multi > 10)
                    return BadRequest("Multi parameter can't be more than 10");

                int MaxCount = List.Count();
                int MultiCount = multi;
                if (MaxCount < MultiCount)
                    MultiCount = MaxCount;

                MultiCount += 1;

                HashSet<GImage> Images = new HashSet<GImage>();
                while (Images.Count != multi)
                {
                    if (Images.Count == MaxCount)
                        break;
                    int ID = Program.RNGBetween(0, MaxCount - 1);
                    GImage Img = List.ElementAt(ID);
                    if (Images.Contains(Img))
                    {
                        Img = List.ElementAtOrDefault(ID);
                    }

                    if (Img == null || Img.IsNsfw())
                    {
                        continue;
                    }
                    Images.Add(Img);
                }
                return Ok(new ApiImages(Images, false));
            }
        }

        [HttpGet("/api/nsfw/img/{type}")]
        public IActionResult GetNsfw(string type, [FromQuery] int multi = 1)
        {
            if (!DB.EndpointCache.TryGetValue("/nsfw/img/" + type, out int EndpintID))
                return BadRequest("Invalid image category. ( GL-6 )");

            DB.Endpoints.TryGetValue(EndpintID, out Endpoint EN);
            if (EN == null)
                return BadRequest("API error, invalid image endpoint. ( GL-7 )");
            if (!EN.isNsfw)
                return BadRequest("Api error, invalid endpoint type. ( GL-7 )");

            IEnumerable<GImage> List = DB.Images.Values.Where(x => EN.albums.ContainsKey(x.album) || x.tags.Any(x => EN.tags.ContainsKey(x)));
            if (!List.Any())
                return BadRequest("This tag/category has no images. ( GL-5 )");

            if (multi == 1)
            {
                int ID = Program.RNGBetween(0, List.Count() - 1);
                GImage Image = List.ElementAt(ID);
                return Ok(new ApiImage(Image, true));
            }
            else
            {
                if (IsPublicUse)
                    return Unauthorized("You are not allowed to use this with the public token");

                DB.Keys.TryGetValue(Request.Headers["Authorization"][0], out ApiUser User);
                if (!(User.ID == "190590364871032834" || User.ID == "729873770990534766"))
                    return Unauthorized("You do not have access to multi image selection.");
                if (multi < 1)
                    return BadRequest("Multi parameter can't be less than 1");
                if (multi > 10)
                    return BadRequest("Multi parameter can't be more than 10");

                int MaxCount = List.Count();
                int MultiCount = multi;
                if (MaxCount < MultiCount)
                    MultiCount = MaxCount;

                MultiCount += 1;

                HashSet<GImage> Images = new HashSet<GImage>();
                while (Images.Count != multi)
                {
                    if (Images.Count == MaxCount)
                        break;
                    int ID = Program.RNGBetween(0, MaxCount - 1);
                    GImage Img = List.ElementAt(ID);
                    if (Images.Contains(Img))
                    {
                        Img = List.ElementAtOrDefault(ID);
                    }

                    if (Img == null)
                    {
                        continue;
                    }
                    Images.Add(Img);
                }
                return Ok(new ApiImages(Images, true));
            }
        }

        [HttpGet("/api/nsfw/gif/{type}")]
        public IActionResult GetNsfwGif(string type, [FromQuery] int multi = 1)
        {
            if (!DB.EndpointCache.TryGetValue("/nsfw/gif/" + type, out int EndpintID))
                return BadRequest("Invalid image category. ( GL-6 )");
            DB.Endpoints.TryGetValue(EndpintID, out Endpoint EN);
            if (EN == null)
                return BadRequest("API error, invalid image endpoint. ( GL-7 )");
            if (!EN.isNsfw)
                return BadRequest("Api error, invalid endpoint type. ( GL-7 )");

            IEnumerable<GImage> List = DB.Images.Values.Where(x => EN.albums.ContainsKey(x.album) || x.tags.Any(x => EN.tags.ContainsKey(x)));
            if (!List.Any())
                return BadRequest("This tag/category has no images. ( GL-5 )");

            if (multi == 1)
            {
                int ID = Program.RNGBetween(0, List.Count() - 1);
                GImage Image = List.ElementAt(ID);
                return Ok(new ApiImage(Image, true));
            }
            else
            {
                if (IsPublicUse)
                    return Unauthorized("You are not allowed to use this with the public token");

                DB.Keys.TryGetValue(Request.Headers["Authorization"][0], out ApiUser User);
                if (!(User.ID == "190590364871032834" || User.ID == "729873770990534766"))
                    return Unauthorized("You do not have access to multi image selection.");

                if (multi < 1)
                    return BadRequest("Multi parameter can't be less than 1");
                if (multi > 10)
                    return BadRequest("Multi parameter can't be more than 10");

                int MaxCount = List.Count();
                int MultiCount = multi;
                if (MaxCount < MultiCount)
                    MultiCount = MaxCount;

                MultiCount += 1;

                HashSet<GImage> Images = new HashSet<GImage>();
                while (Images.Count != multi)
                {
                    if (Images.Count == MaxCount)
                        break;
                    int ID = Program.RNGBetween(0, MaxCount - 1);
                    GImage Img = List.ElementAt(ID);
                    if (Images.Contains(Img))
                    {
                        Img = List.ElementAtOrDefault(ID);
                    }

                    if (Img == null)
                    {
                        continue;
                    }
                    Images.Add(Img);
                    
                }
                return Ok(new ApiImages(Images, true));
            }
        }




        [HttpGet("/api/nsfw/waifulewd")]
        public IActionResult GetNsfwWaifuLewd()
        {
            if (DB.WaifuLewdCount == 0)
                return BadRequest("This tag/category has no images. ( GL-5 )");

            GImage Img = DB.WaifuLewds.ElementAt(Program.RNGBetween(0, DB.WaifuLewdCount - 1));
            return Ok(new ApiImage(Img, true));
        }


        [HttpPost("/api/upload/revolt")]
        public async Task<IActionResult> UploadRevoltImage(string type, string folder)
        {
            using var buffer = new System.IO.MemoryStream();
            await this.Request.Body.CopyToAsync(buffer, this.Request.HttpContext.RequestAborted);
            var imageBytes = buffer.ToArray();
            string FileName = Program.Letters[Program.RNGBetween(0, Program.Letters.Length - 1)].ToString() + Program.Letters[Program.RNGBetween(0, Program.Letters.Length - 1)].ToString() + Program.Gen.CreateId().ToString() + "." + type;
            string FilePath = "/home/website/revolt/" + folder + "/" + FileName;
            System.IO.File.WriteAllBytes(FilePath, imageBytes); ;
            return Ok(new Response(200, "https://revoltimg.fluxpoint.dev/" + folder + "/" + FileName));
        }

        [HttpGet("/api/upload/url")]
        public async Task<IActionResult> UploadUrl(string url, string album = "19")
        {
            if (IsPublicUse)
                return Unauthorized("You are not allowed to use this with the public token");

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


            string Hash = Program.getMd5Hash(Bytes);
            if (album == "19")
            {
                if (DB.Images.Values.Any(x => x.album == 19 && x.file.hash == Hash))
                    return CustomStatus(409, "Already exist.");
            }
            else
            {
                if (DB.HashSet.ContainsKey(Hash))
                    return CustomStatus(409, "Already exist.");
            }
            string ID = Program.Gen.CreateId().ToString();
            bool GifConvert = false;
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
                case "gif":
                    Format = MagickFormat.Gif;
                    if (album == "19")
                    {
                        GifConvert = true;
                        FormatName = "png";
                    }
                    else
                    {
                        using (FileStream files = new FileStream(Config.GlobalPath + "img/" + ID + ".gif", FileMode.Create, FileAccess.Write))
                        {
                            files.Write(Bytes);
                        }
                    }
                    break;
            }

            try
            {
                DB.HashSet.Add(Hash, ID);
            }
            catch { }
            int Width = 0;
            int Height = 0;
            using (MagickImage image = new MagickImage(new MemoryStream(Bytes), new MagickReadSettings { ColorType = ColorType.Optimize, Format = Format, FrameIndex = 0, FrameCount = 1 }))
            {
                image.Strip();
                Width = image.Width;
                Height = image.Height;
                if (GifConvert || Format != MagickFormat.Gif)
                    image.Write(Config.GlobalPath + "img/" + ID + "." + FormatName, Format);

                image.Resize(792, 594);
                image.Write(Config.GlobalPath + "med/" + ID + ".webp", Format);
                //image.Resize(320, 320);
                //image.Write(Config.GlobalPath + "thm/" + ID + "." + FormatName, Format);
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
                    height = Height,
                    width = Width,
                    size = Bytes.Length,
                    type = FormatName
                },
                album = albumid
            };
            DB.Images.Add(ID, img);
            img.Add();
            return Ok();
        }
    }
}
