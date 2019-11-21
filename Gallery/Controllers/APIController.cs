using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gallery.Data;
using Gallery.Database;
using Microsoft.Extensions.Primitives;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gallery.Controllers
{
    public class ApiError
    {
        public string message = "";
    }

    public class ApiImage
    {
        public string file = "";
    }

    [Route("api/[controller]")]
    public class APIController : Controller
    {
        // GET api/<controller>/5
        [HttpGet("/api/album/{id}")]
        public ContentResult GetAlbum(int id)
        {
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues Auth) || Auth[0] == "")
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ApiError { message = "No auth header" }, Newtonsoft.Json.Formatting.Indented));
            }
            if (!DB.Keys.TryGetValue(Auth[0], out string st))
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ApiError { message = "Your api token is invalid" }, Newtonsoft.Json.Formatting.Indented));
            }

            if (!DB.Albums.TryGetValue(id, out GAlbum album))
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ApiError { message = "Unknown album" }, Newtonsoft.Json.Formatting.Indented));
            
            if (!album.isPublic)
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ApiError { message = "This album is private" }, Newtonsoft.Json.Formatting.Indented));


            string File = "";
            int ID = Program.rng.Next(0, DB.Images.Values.Count(x => x.album == id));
            File = DB.Images.Values.Where(x => x.album == id).ElementAt(ID).GetImage(imageType.Full);
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ApiImage { file = File }, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
