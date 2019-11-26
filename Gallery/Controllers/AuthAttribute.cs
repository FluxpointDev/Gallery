using Gallery.Data;
using Gallery.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Controllers
{
    public enum RoleType
    {
        All, Donator, Owner
    }

    public class IsDonatorAttribute : IsAuthenticatedAttribute
    {
        public IsDonatorAttribute()
        {
            Type = RoleType.Donator;
        }
    }
    public class IsOwner : IsAuthenticatedAttribute
    {
        public IsOwner()
        {
            Type = RoleType.Owner;
        }
    }

    public class IsAuthenticatedAttribute : ActionFilterAttribute
    {
        public RoleType Type { get; internal set; } = RoleType.All;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MyController controller = filterContext.Controller as MyController;
            if (!filterContext.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues Auth) || Auth[0] == "")
            {
                filterContext.Result = controller.CustomStatus(401, "Authorization header not set");
                return;
            }
            if (!DB.Keys.TryGetValue(Auth[0], out ApiUser User))
            {
                filterContext.Result = controller.CustomStatus(401, $"Your token is invalid, for support go to {Config.Website} or email {Config.Email}");
                return;
            }

            if (User.Disabled)
            {
                filterContext.Result = controller.CustomStatus(401, "Your token is disabled, for support go to " + Config.Website);
                return;
            }
            if (User.Expire.HasValue && DateTime.UtcNow > User.Expire.Value)
            {
                filterContext.Result = controller.CustomStatus(401, "Your token has expired, for support go to " + Config.Website);
                return;
            }
            controller.User = User;
            if (Type == RoleType.All)
                return;
            switch (Type)
            {
                case RoleType.Donator:
                    if (User.Donator)
                    {

                    }
                    else
                    {
                        filterContext.Result = controller.CustomStatus(401, "This endpoint is for donators only, for support go to " + Config.Website);
                        return;
                    }
                    break;
                case RoleType.Owner:
                    if (!User.Owner)
                    {
                        filterContext.Result = controller.CustomStatus(401, "This endpoint is for Builderb only ;)");
                        return;
                    }
                    break;
            }
        }
    }
}
