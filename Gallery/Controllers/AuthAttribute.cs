using Gallery.Data;
using Gallery.Database;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;

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
            if (!filterContext.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                filterContext.Result = controller.CustomStatus(401, "Authorization header is missing. ( G-1 )");
                return;
            }

            string AuthKey = filterContext.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(AuthKey))
            {
                filterContext.Result = controller.CustomStatus(401, "Authorization header is empty. ( G-1 )");
                return;
            }

            if (!DB.Keys.TryGetValue(AuthKey, out ApiUser User))
            {
                filterContext.Result = controller.CustomStatus(401, $"Your token is invalid, for support go to {Config.Discord} ( G-2 )");
                return;
            }

            if (User.Disabled)
            {
                filterContext.Result = controller.CustomStatus(401, "Your token is disabled, for support go to " + Config.Discord + " ( G-3 )");
                return;
            }
            if (AuthKey.StartsWith("FP-Public"))
                controller.IsPublicUse = true;



            controller.User = User;
            if (Type == RoleType.All)
                return;
            switch (Type)
            {
                case RoleType.Owner:
                    if (!User.IsOwner())
                    {
                        filterContext.Result = controller.CustomStatus(401, "This endpoint is for Builderb only ;) ( G-4 )");
                        return;
                    }
                    break;
            }
        }
    }
}
