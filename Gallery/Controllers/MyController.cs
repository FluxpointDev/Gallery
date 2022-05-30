using Gallery.Database;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Gallery.Controllers
{
    public class MyController : ControllerBase
    {
        public new ApiUser User = null;
        public MyController()
        {

        }

        public new ContentResult Ok()
        {
            return Ok(new Response(200, ""));
        }

        public ContentResult Ok(string msg, string optional = "")
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            if (msg[0] == '{')
            {
                JObject JObject = JObject.Parse(msg);
                if (JObject.ContainsKey("status"))
                    JObject["status"] = "ok";
                else
                    JObject.Add("status", "ok");
                JObject.Add("code", 200);
                if (!JObject.ContainsKey("message"))
                    JObject.Add("message", optional);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JObject, Newtonsoft.Json.Formatting.Indented));
            }
            if (msg[0] == '[')
            {
                JObject JObject = new JObject
                {
                    { "status", "ok" },
                    { "code", 200 },
                    { "message", optional },
                    { "list",  JArray.Parse(msg) }
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JObject, Newtonsoft.Json.Formatting.Indented));
            }
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new Response(200, msg), Newtonsoft.Json.Formatting.Indented));
        }

        public ContentResult Ok(object obj, string msg = "")
        {
            return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(obj), msg);
        }

        public ContentResult Ok(JObject json, string msg = "")
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            JObject JObject = JObject.Parse(msg);
            JObject.Add("status", "ok");
            JObject.Add("code", 200);
            JObject.Add("message", msg);
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JObject, Newtonsoft.Json.Formatting.Indented));
        }

        public ContentResult Ok(JArray json, string msg = "")
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            JObject JObject = new JObject
                {
                    { "status", "ok" },
                    { "code", 200 },
                    { "message", msg },
                    { "list",  JArray.Parse(msg) }
                };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JObject, Newtonsoft.Json.Formatting.Indented));
        }

        public ContentResult Ok(Response msg)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented));
        }

        public ContentResult BadRequest(string error)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 400;
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ErrorResponse(ErrorType.BadRequest, error), Newtonsoft.Json.Formatting.Indented));
        }

        public ContentResult Unauthorized(string error)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 401;
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ErrorResponse(ErrorType.UnAuthorized, error), Newtonsoft.Json.Formatting.Indented));
        }

        public ContentResult InternalServerError(string error)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 500;
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ErrorResponse(ErrorType.InternalServerError, error), Newtonsoft.Json.Formatting.Indented));
        }

        public ContentResult Forbidden(string error)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 403;
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ErrorResponse(ErrorType.Forbidden, error), Newtonsoft.Json.Formatting.Indented));
        }

        public ContentResult NotFound(string error)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 404;
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ErrorResponse(ErrorType.NotFound, error), Newtonsoft.Json.Formatting.Indented));
        }

        public ContentResult CustomStatus(int code, string msg)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = code;
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new Response(code, msg), Newtonsoft.Json.Formatting.Indented));
        }
    }
}
