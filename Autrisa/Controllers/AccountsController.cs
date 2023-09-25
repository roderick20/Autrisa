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
    public class AccountsController : Controller
    {
        private readonly EFContext _context;

        public AccountsController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Operations = await _context.Operations.Where(m => m.Type < 2).ToListAsync();
            var accounts = await _context.Accounts.Where(m => m.Visible == null || m.Visible == 0).ToListAsync();
            return View(accounts);
        }

        public async Task<IActionResult> IndexLendings()
        {
            var accounts = await _context.Accounts
                .Where(m => m.OperationType == 1)
                .ToListAsync();
            return View(accounts);
        }

        public async Task<IActionResult> IndexInvestments()
        {
            var accounts = await _context.Accounts
                .Where(m => m.OperationType == 2)
                .ToListAsync();
            return View(accounts);
        }

        public async Task<IActionResult> IndexProperties()
        {
            var accounts = await _context.Accounts
                .Where(m => m.OperationType == 3)
                .ToListAsync();
            return View(accounts);
        }

        public async Task<IActionResult> Details(Guid UniqueId)
        {
            var account = await _context.Accounts
                .Select(m => new Account
                {
                    Id = m.Id,                                    
                    UniqueId = m.UniqueId,                                    
                    Name = m.Name,                                    
                    AccountType = m.AccountType,                                    
                    AccountNumber = m.AccountNumber,                                    
                    Currency = m.Currency,                                    
                    Amount = m.Amount,                                    
                    Created = m.Created,                                    
                    Author = m.Author,                                    
                    Modified = m.Modified,                                    
                    Editor = m.Editor,                                    
                //AuthorName = _context.Users.FirstOrDefault(a => a.Id == m.Author).Name,
                //EditorName = _context.Users.FirstOrDefault(e => e.Id == m.Editor).Name,
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
            ViewBag.bankId = new SelectList(_context.Banks, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account, string DateAccountStr)//, int BankId)
        {
            try
            {
                account.UniqueId = Guid.NewGuid();
                account.PreviousRemaining = account.Amount;
                account.Created = DateTime.Now;
                account.DateAccount = DateTime.ParseExact(DateAccountStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                account.Author = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(account);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            ViewBag.bankId = new SelectList(_context.Banks, "Id", "Name");
            return View();
        }
        
        public async Task<IActionResult> Edit(Guid UniqueId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (account == null)
            {
                return NotFound();
            }
            ViewBag.bankId = new SelectList(_context.Banks, "Id", "Name", account.BankId);
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Account account, string Modified,string DateAccountStr)
        {
            try
            {
                var accountEdit = await _context.Accounts.FirstOrDefaultAsync(m => m.UniqueId == account.UniqueId);
                accountEdit.Name = account.Name;
                accountEdit.BankId = account.BankId;
                accountEdit.DateAccount = DateTime.ParseExact(DateAccountStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                accountEdit.AccountType = account.AccountType; 
                accountEdit.AccountNumber = account.AccountNumber; 
                accountEdit.Currency = account.Currency; 
                accountEdit.Amount = account.Amount;
                accountEdit.Modified = DateTime.Now;                
                accountEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
                _context.Update(accountEdit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Editado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;    
            }
            ViewBag.bankId = new SelectList(_context.Banks, "Id", "Name", account.BankId);
            return View(account);
        }
        
        public async Task<IActionResult> Delete(Guid UniqueId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid UniqueId)
        {
            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                if (account != null)
                {
                    _context.Accounts.Remove(account);
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