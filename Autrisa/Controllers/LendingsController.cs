using Autrisa.Models;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Autrisa.Controllers
{
    public class LendingsController : Controller
    {
        private readonly EFContext _context;

        public LendingsController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lendings = await _context.Accounts
                .Where(m => m.OperationType == 1)
                .ToListAsync();
            return View(lendings);
        }

        public async Task<IActionResult> Details(Guid UniqueId)
        {
            var operation = await _context.Lendings
                .Include(m => m.Account)
                .FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (operation == null)
            {
                return NotFound();
            }
            return View(operation);
        }

        public IActionResult NewAccount()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.AccountId = new SelectList(_context.Accounts.Where(m => m.AccountType == "Préstamo"), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lending lending, string Created)
        {
            try
            {
                var account = await _context.Accounts.Where(x => x.Id == lending.AccountId).FirstOrDefaultAsync();

                if (account.Currency == 0)
                {
                    lending.SolesAmount = lending.Amount;
                    lending.DollarsAmount = 0;
                }
                else
                {
                    lending.DollarsAmount = lending.Amount;
                    lending.SolesAmount = 0;
                }

                lending.Currency = account.Currency;
                lending.UniqueId = Guid.NewGuid();
                lending.Created = DateTime.ParseExact(Created, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //lending.Created = DateTime.Now;
                lending.Author = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(lending);
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
            var lending = await _context.Lendings.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (lending == null)
            {
                return NotFound();
            }
            ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "Name", lending.AccountId);
            return View(lending);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Lending lending, string Modified)
        {
            try
            {
                var lendingEdit = await _context.Lendings.FirstOrDefaultAsync(m => m.UniqueId == lending.UniqueId);
                var account = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == lending.AccountId);

                if (lendingEdit.Currency == 0)
                {
                    lendingEdit.SolesAmount = lendingEdit.Amount;
                }
                else
                {
                    lendingEdit.DollarsAmount = lendingEdit.Amount;
                }

                lendingEdit.AccountId = lending.AccountId;
                lendingEdit.Currency = account.Currency;
                lendingEdit.Amount = lending.Amount;
                lendingEdit.Customer = lending.Customer;
                lendingEdit.LendDate = lending.LendDate;
                lendingEdit.Description = lending.Description;
                lendingEdit.Modified = DateTime.ParseExact(Modified, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //lendingEdit.Modified = DateTime.Now;
                lendingEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
                _context.Update(lendingEdit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Editado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", lending.AccountId);
            return View(lending);
        }

        public async Task<IActionResult> Delete(Guid UniqueId)
        {
            var lending = await _context.Lendings.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
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
                var lending = await _context.Lendings.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                if (lending != null)
                {
                    _context.Lendings.Remove(lending);
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
