using Autrisa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Autrisa.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly EFContext _context;
    
        public PropertiesController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var properties = await _context.Accounts
                .Where(m => m.OperationType == 3)
                .ToListAsync();
            return View(properties);
        }

        public async Task<IActionResult> Details(Guid UniqueId)
        {
            var properties = await _context.Properties
                .Include(m => m.Account)
                .FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (properties == null)
            {
                return NotFound();
            }
            return View(properties);
        }

        public IActionResult NewAccount()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.AccountId = new SelectList(_context.Accounts.Where(m => m.AccountType == "Predios"), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property, string Created)
        {
            try
            {
                var account = await _context.Accounts.Where(x => x.Id == property.AccountId).FirstOrDefaultAsync();
                property.Currency = account.Currency;
                if(property.Currency == 0)
                {
                    property.SolesAmount = property.Amount;
                }
                else
                {
                    property.DollarsAmount = property.Amount;
                }
                property.IncomeSoles = 0;
                property.OutcomeSoles = 0;
                property.IncomeDollars = 0;
                property.OutcomeDollars = 0;
                property.Amount = 0;
                property.UniqueId = Guid.NewGuid();
                //property.Created = DateTime.Now;
                property.Created = DateTime.ParseExact(Created, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                property.Author = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(property);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Agregado exitosamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return View();
        }

        public async Task<IActionResult> Edit(Guid UniqueId)
        {
            var property = await _context.Properties.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (property == null)
            {
                return NotFound();
            }
            ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "Name", property.AccountId);
            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Property property, string Modified)
        {
            try
            {
                var propertyEdit = await _context.Properties.FirstOrDefaultAsync(m => m.UniqueId == property.UniqueId);
                var account = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == property.AccountId);

                if (propertyEdit.Currency == 0)
                {
                    propertyEdit.SolesAmount = propertyEdit.Amount;
                }
                else
                {
                    propertyEdit.DollarsAmount = propertyEdit.Amount;
                }

                propertyEdit.Address = property.Address;
                propertyEdit.Amount = property.Amount;
                propertyEdit.Number = property.Number;
                propertyEdit.Participation = property.Participation;
                propertyEdit.Description = property.Description;
                propertyEdit.Receptor = property.Receptor;
                propertyEdit.Amount = property.Amount;
                propertyEdit.Currency = account.Currency;
                //propertyEdit.Modified = DateTime.Now;
                propertyEdit.Modified= DateTime.ParseExact(Modified, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                propertyEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
                _context.Update(propertyEdit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Editado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", property.AccountId);
            return View(property);
        }

        public async Task<IActionResult> Delete(Guid UniqueId)
        {
            var lending = await _context.Properties.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (lending == null)
            {
                return NotFound();
            }

            return View(lending);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid UniqueId)
        {
            try
            {
                var properties = await _context.Properties.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                if (properties != null)
                {
                    _context.Properties.Remove(properties);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Eliminado exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
