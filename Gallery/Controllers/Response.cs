using Gallery.Data;

namespace Gallery.Controllers
{
    public class HomeResponse : Response
    {
        public HomeResponse(string msg) : base(200, msg) { }
        public string website = Config.Website;
        public string discord = Config.Discord;
        public string docs = Config.Docs;
        public string twitter = Config.Twitter;
        public string reddit = Config.Reddit;
        public string email = Config.Email;
        public string donate = Config.Patreon;
    }
    public class Response
    {
        public Response(int cd, string msg)
        {
            code = cd;
            message = msg;
            if (code != 200 && code != 418)
                status = "error";
        }

        public string status = "ok";
        public int code = 200;
        public string message = "";
    }

    public class ErrorResponse : Response
    {
        public ErrorResponse(ErrorType type, string msg) : base(500, msg)
        {
            switch (type)
            {
                case ErrorType.UnAuthorized:
                    code = 401;
                    break;
                case ErrorType.BadRequest:
                    code = 400;
                    break;
                case ErrorType.Forbidden:
                    code = 403;
                    break;
            }
        }
    }
    public enum ErrorType
    {
        InternalServerError, UnAuthorized, BadRequest, Forbidden
    }
}
