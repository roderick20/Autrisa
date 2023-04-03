using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Autrisa.Helpers;
using Autrisa.Models;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Autrisa
{

    public class AuthenticationFilter : IActionFilter
    {
        private readonly EFContext _context;
        public AuthenticationFilter(EFContext context)
        {
            _context = context;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            String path = context.HttpContext.Request.Path;
            List<String> path_anonymus = new List<string>();
            path_anonymus.Add("/Auths/Login");
            
            if (String.IsNullOrEmpty(context.HttpContext.Session.GetString("Anonymus")))
            {
                context.HttpContext.Session.SetString("Anonymus", "anonymus");
                context.HttpContext.Session.SetSettings(_context.Settings.ToDictionary(m => m.Key, m =>m.Value));
            }
            
            #if (DEBUG)
            context.HttpContext.Session.SetInt32("UserId", 1);
            context.HttpContext.Session.SetString("UserName", "Admin");
            context.HttpContext.Session.SetString("UserRole", "admin");
#endif

            String controllerName = context.RouteData.Values["controller"].ToString();
            String actionName = context.RouteData.Values["action"].ToString();
            String areaName = context.RouteData.Values["area"] != null ? context.RouteData.Values["area"].ToString() : "";

            String anonymus = path_anonymus.Where(m => m.Equals("/" + controllerName +"/"+ actionName)).FirstOrDefault();

            if (anonymus == null && String.IsNullOrEmpty(context.HttpContext.Session.GetString("UserId")))
            {

                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        area = "",
                        controller = "Auths",
                        action = "Login"
                    }));                
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}