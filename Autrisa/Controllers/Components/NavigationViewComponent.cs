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
            return View();
        }
    }
}