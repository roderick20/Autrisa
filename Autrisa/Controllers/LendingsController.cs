using Autrisa.Models;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            var lendings = await _context.Lendings
                .Include(m => m.Account)
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

        public IActionResult Create()
        {
            ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lending lending)
        {
            try
            {
                var account = await _context.Accounts.Where(x => x.Id == lending.AccountId).FirstOrDefaultAsync();
                lending.Currency = account.Currency;
                lending.UniqueId = Guid.NewGuid();
                lending.Created = DateTime.Now;
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

        //public async Task<IActionResult> Edit(Guid UniqueId)
        //{
        //    var operation = await _context.Operations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
        //    if (operation == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", operation.AccountId);
        //    return View(operation);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Operation operation)
        //{
        //    try
        //    {
        //        var operationEdit = await _context.Operations.FirstOrDefaultAsync(m => m.UniqueId == operation.UniqueId);

        //        operationEdit.Type = operation.Type;
        //        operationEdit.Modality = operation.Modality;
        //        operationEdit.Number = operation.Number;
        //        operationEdit.AccountId = operation.AccountId;
        //        operationEdit.OperationDate = operation.OperationDate;
        //        operationEdit.Concept = operation.Concept;
        //        operationEdit.Description = operation.Description;
        //        operationEdit.Income = operation.Income;
        //        operationEdit.Outcome = operation.Outcome;
        //        operationEdit.Year = operation.Year;
        //        operationEdit.Month = operation.Month;
        //        operationEdit.Modified = DateTime.Now;
        //        operationEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
        //        _context.Update(operationEdit);
        //        await _context.SaveChangesAsync();
        //        TempData["Success"] = "Editado exitosamente";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = "Error: " + ex.Message;
        //    }
        //    ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", operation.AccountId);
        //    return View(operation);
        //}

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
