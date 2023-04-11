using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Autrisa.Models;
using System.Text.RegularExpressions;
using Autrisa.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace Autrisa.Controllers
{
    public class BanksController : Controller
    {
        private readonly EFContext _context;

        public BanksController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var banks = await _context.Banks.ToListAsync();
            return View(banks);
        }

        public async Task<IActionResult> Details(Guid UniqueId)
        {
            var account = await _context.Banks
                .Select(m => new Bank
                {
                    Id = m.Id,
                    UniqueId = m.UniqueId,
                    Name = m.Name,
                    Created = m.Created,
                    Author = m.Author,
                    Modified = m.Modified,
                    Editor = m.Editor,
                })
                .FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bank bank)
        {
            try
            {
                bank.UniqueId = Guid.NewGuid();
                bank.Created = DateTime.Now;
                bank.Author = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(bank);
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
            var account = await _context.Banks.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Bank bank, string Modified)
        {
            try
            {
                var bankEdit = await _context.Banks.FirstOrDefaultAsync(m => m.UniqueId == bank.UniqueId);

                bankEdit.Name = bank.Name;
                bankEdit.Modified = DateTime.ParseExact(Modified, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                bankEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
                _context.Update(bankEdit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Editado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            return View(bank);
        }

        public async Task<IActionResult> Delete(Guid UniqueId)
        {
            var bank = await _context.Banks.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (bank == null)
            {
                return NotFound();
            }

            return View(bank);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid UniqueId)
        {
            try
            {
                var bank = await _context.Banks.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                if (bank != null)
                {
                    _context.Banks.Remove(bank);
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