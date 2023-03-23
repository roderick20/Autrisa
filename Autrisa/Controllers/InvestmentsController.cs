using Autrisa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Autrisa.Controllers
{
    public class InvestmentsController : Controller
    {
        private readonly EFContext _context;

        public InvestmentsController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var investment = await _context.Investments
                .Include(m => m.Account)
                .ToListAsync();
            return View(investment);
        }

        public async Task<IActionResult> Details(Guid UniqueId)
        {
            var investment = await _context.Investments
                .Include(m => m.Account)
                .FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (investment == null)
            {
                return NotFound();
            }

            return View(investment);
        }

        public IActionResult NewAccount()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.AccountId = new SelectList(_context.Accounts.Where(m => m.AccountType == "Inversión"), "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Investment investment)
        {
            try
            {
                var account = await _context.Accounts.Where(x => x.Id == investment.AccountId).FirstOrDefaultAsync();
                investment.Currency = account.Currency;
                investment.OperationAmount = investment.Amount;
                investment.UniqueId = Guid.NewGuid();
                investment.Created = DateTime.Now;
                investment.Author = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(investment);
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
            var investment= await _context.Investments.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (investment == null)
            {
                return NotFound();
            }
            ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "Name", investment.AccountId);
            return View(investment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Investment investment)
        {
            try
            {
                var investmentEdit = await _context.Investments.FirstOrDefaultAsync(m => m.UniqueId == investment.UniqueId);

                investmentEdit.Customer = investment.Customer;
                investmentEdit.Amount = investment.Amount;
                investmentEdit.OperationType = investment.OperationType;
                investmentEdit.OperationDate = investment.OperationDate;
                investmentEdit.OperationAmount = investment.OperationAmount;
                investmentEdit.Description = investment.Description;
                investmentEdit.AccountId = investmentEdit.AccountId;
                investmentEdit.Modified = DateTime.Now;
                investmentEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
                _context.Update(investmentEdit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Editado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", investment.AccountId);
            return View(investment);
        }

        public async Task<IActionResult> Delete(Guid UniqueId)
        {
            var investment = await _context.Investments.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (investment == null)
            {
                return NotFound();
            }

            return View(investment);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid UniqueId)
        {
            try
            {
                var investments= await _context.Investments.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                if (investments != null)
                {
                    _context.Investments.Remove(investments);
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
