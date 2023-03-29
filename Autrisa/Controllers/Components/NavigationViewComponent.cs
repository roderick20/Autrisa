using Microsoft.AspNetCore.Mvc;


namespace Autrisa.Controllers
{
    public class NavigationViewComponent : ViewComponent
    {

        public NavigationViewComponent()
        {         
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            return View();
        }
    }
}