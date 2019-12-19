using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Gallery.Data;
using Gallery.Database;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gallery.Controllers
{
    public class ApiImage
    {
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

        // GET api/<controller>/5
        [HttpGet("/api/album/{id}")]
        public IActionResult GetAlbum(int id)
        {
            if (!DB.Albums.TryGetValue(id, out GAlbum album))
                return BadRequest("Unknown album");

            if (!album.isPublic)
                return BadRequest("This album is private");

            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == id);
            int ID = Program.rng.Next(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/sfw/anime")]
        public IActionResult GetSfwAnime()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 5);
            int ID = Program.rng.Next(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/sfw/wallpaper")]
        public IActionResult GetSfwWallpaper()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 6);
            int ID = Program.rng.Next(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/sfw/azurlane")]
        public IActionResult GetSfwAzurlane()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 7);
            int ID = Program.rng.Next(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/sfw/nekopara")]
        public IActionResult GetSfwNekopara()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 8);
            int ID = Program.rng.Next(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/nsfw/azurlane")]
        public IActionResult GetNsfwAzurlane()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 11);
            int ID = Program.rng.Next(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/nsfw/nekopara")]
        public IActionResult GetNsfwNekopara()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 10);
            int ID = Program.rng.Next(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }

        [HttpGet("/api/nsfw/lewd")]
        public IActionResult GetNsfwLewd()
        {
            IEnumerable<GImage> List = DB.Images.Values.Where(x => x.album == 12);
            int ID = Program.rng.Next(0, List.Count() - 1);
            string File = List.ElementAt(ID).GetImage(imageType.Full);
            return Ok(new ApiImage { file = File });
        }
    }
}
