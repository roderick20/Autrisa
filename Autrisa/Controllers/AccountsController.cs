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
            var accounts = await _context.Accounts
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
                    AuthorName = _context.Users.FirstOrDefault(a => a.Id == m.Author).Name,
                    EditorName = _context.Users.FirstOrDefault(e => e.Id == m.Editor).Name,
                })
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
                AuthorName = _context.Users.FirstOrDefault(a => a.Id == m.Author).Name,
                EditorName = _context.Users.FirstOrDefault(e => e.Id == m.Editor).Name,
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
        public async Task<IActionResult> Create(Account account)
        {
            try
            {
                account.UniqueId = Guid.NewGuid();
                account.Created = DateTime.Now;
                account.Author = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(account);
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
            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Account account)
        {
            try
            {
                var accountEdit = await _context.Accounts.FirstOrDefaultAsync(m => m.UniqueId == account.UniqueId);

                accountEdit.Name = account.Name; 
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(id);
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