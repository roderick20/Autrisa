using Autrisa.Helper;
using Autrisa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Autrisa.Controllers
{
    public class AuthsController : Controller
    {
        private readonly EFContext _context;

        public AuthsController(EFContext context)
        {
            _context = context;
        }


        public IActionResult Index() => View();
        
        [Route("Login")]
        public IActionResult Login() => View();
        
        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Usuario y contraseña son obligatorios";
                return View();
            }

            password = PasswordEncrypt.Encrypt(password);
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == email && m.Password == password);
            if (user != null)// && user.Role == 1)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);//s + " " + user.Surnames);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Usuario o contraseña invalidos";
            }

            return View();
        }


        [Route("UpdatePassword")]
        public IActionResult UpdatePassword() => View();

        [Route("UpdatePassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(String password)
        {
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Contraseña es obligarotio");
                return View();
            }

            //String ErrorMessage = "";
            //if (!PasswordTools.Validate(password, out ErrorMessage))
            //{
            //    ModelState.AddModelError("", ErrorMessage);
            //    return View();
            //}

            int UserId = (int)HttpContext.Session.GetInt32("UserId");

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == UserId);

            try
            {
                user.Password = PasswordEncrypt.Encrypt(password);
                _context.Update(user);
                await _context.SaveChangesAsync();
                ViewBag.Message = "Contraseña actualizada";
                return View();
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        [Route("/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Login");
        }        
    }
}