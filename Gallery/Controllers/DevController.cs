using Gallery.Data;
using Gallery.Database;
using Gallery.DiscordAuth;
using Gallery.Shared;
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

    [Route("/api/dev/[controller]"), ApiController, IsOwner]
    public class DevController : MyController
    {
        [HttpGet("/api/dev/user/{userId}/{actionType}")]
        public async Task<IActionResult> User([FromRoute] string userId, [FromRoute] string actionType)
        {
            ApiUser User = DB.R.Db("API").Table("Users").Get(userId).RunAtom<ApiUser>(DB.Con);
            if (User == null)
                return BadRequest("This user does not have API access!");

            switch (actionType)
            {
                case "updateToken":
                    {
                        string body;
                        try
                        {
                            using (StreamReader reader = new StreamReader(Request.Body))
                            {
                                body = await reader.ReadToEndAsync();
                            }
                        }
                        catch
                        {
                            return BadRequest("Request body is not have string");
                        }

                        if (string.IsNullOrEmpty(body))
                            return BadRequest("Request body string is empty");

                        DB.Keys.Remove(User.GetRealToken());
                        User.Token = APICrypt.EncryptString(body);
                        DB.Keys.Add(body, User);
                    }
                    break;
                case "updatePublicToken":
                    {
                        string body;
                        try
                        {
                            using (StreamReader reader = new StreamReader(Request.Body))
                            {
                                body = await reader.ReadToEndAsync();
                            }
                        }
                        catch
                        {
                            return BadRequest("Request body is not have string");
                        }

                        if (string.IsNullOrEmpty(body))
                            return BadRequest("Request body string is empty");

                        User.PublicToken = APICrypt.EncryptString(body);
                        if (!string.IsNullOrEmpty(User.PublicToken))
                            DB.Keys.Remove(User.GetRealPublicToken());
                        DB.Keys.Add(body, User);
                    }
                    break;
            }

            return Ok();
        }

    }
}
