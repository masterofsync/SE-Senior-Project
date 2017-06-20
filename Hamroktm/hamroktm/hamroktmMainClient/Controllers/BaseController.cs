using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace hamroktmMainClient.Controllers
{
    public abstract class BaseController : Controller
    {
        public string GetToken()
        {
            HttpContext context = System.Web.HttpContext.Current;

            var httpCookie = context.Request.Cookies["ebsCookie"];
            if (httpCookie != null && httpCookie["AccessToken"] != null)
            {
                var cookie = httpCookie["AccessToken"];
                if (cookie != null) return cookie;
            }
            return null;
        }

        public string GetRole()
        {
            HttpContext context = System.Web.HttpContext.Current;

            var httpCookie = context.Request.Cookies["ebsCookie"];
            if (httpCookie != null && httpCookie["Role"] != null)
            {
                var cookie = httpCookie["Role"];
                if (cookie != null) return cookie;
            }
            return null;
        }

        public string GetUserName()
        {
            HttpContext context = System.Web.HttpContext.Current;

            var httpCookie = context.Request.Cookies["ebsCookie"];
            if (httpCookie != null && httpCookie["Username"] != null)
            {
                var cookie = httpCookie["Username"];
                if (cookie != null) return cookie;
            }
            return null;
        }
        public static string Token;
        public static string Role;
        public static string Name;

        public BaseController()
        {
            Token = GetToken();
            Role = GetRole();
            Name = GetUserName();
        }

        //public class CustomAuthorize : AuthorizeAttribute
        //{
        //    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //    {
        //        if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
        //        {
        //            filterContext.Result = new HttpUnauthorizedResult();
        //        }
        //        else
        //        {
        //            filterContext.Result = new RedirectToRouteResult(new
        //                RouteValueDictionary(new { controller = "Home", action = "Unauthorized" }));
        //        }
        //    }
        //}



        // with this you should be able to check if there is a token and configure custom login.
        public class TokenAuthorize : ActionFilterAttribute
        {

            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false)
     && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false))
                {
                    if (Token == null)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                           new RouteValueDictionary 
                                   {
                                       { "action", "Login" },
                                       { "controller", "Account" }
                                   });
                    }
                    base.OnActionExecuting(filterContext);
                }
            }
        }

        public class TokenAdminAuthorize : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false)
                    &&
                    !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute),
                        false))
                {
                    if (Token == null)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                           new RouteValueDictionary 
                                   {
                                       { "action", "Login" },
                                       { "controller", "Account" }
                                   });
                        base.OnActionExecuting(filterContext);
                        return;
                    }
                    if (Role != "Admin")
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                                {"action", "Unauthorized"},
                                {"controller", "Home"}
                            });
                        base.OnActionExecuting(filterContext);
                        
                    }

                }
            }
        }

        //Authentication with token
        // public  class CustomAuthenticate : AuthorizeAttribute
        //{

        //    protected override void HandleUnauthenticatedRequest(AuthorizationContext filterContext)
        //    {
        //        if (Gettoke)
        //        {
        //            filterContext.Result = new HttpUnauthorizedResult();
        //        }
        //        else
        //        {
        //            filterContext.Result = new RedirectToRouteResult(new
        //                RouteValueDictionary(new { controller = "Home", action = "Unauthorized" }));
        //        }
        //    }
        //}
    }
}