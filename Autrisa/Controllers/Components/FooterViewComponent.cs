using Microsoft.AspNetCore.Mvc;

namespace Autrisa.Controllers
{
    public class FooterViewComponent : ViewComponent
    {
        public FooterViewComponent()
        {      
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}