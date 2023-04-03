using Microsoft.AspNetCore.Mvc;

namespace Autrisa.Controllers
{
    public class TopBarViewComponent : ViewComponent
    {
        public TopBarViewComponent()
        {         
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }
    }
}