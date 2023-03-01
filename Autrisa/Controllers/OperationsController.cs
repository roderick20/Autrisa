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
    public class OperationsController : Controller
    {
        private readonly EFContext _context;

        public OperationsController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var operations = await _context.Operations
                .Include(m => m.Account)
                .Select(m => new Operation
                {
                    Id = m.Id,                                    
                    UniqueId = m.UniqueId,                                    
                    Type = m.Type,                                    
                    Modality = m.Modality,                                    
                    Number = m.Number,                                    
                    AccountId = m.AccountId,                                    
                    OperationDate = m.OperationDate,                                    
                    Concept = m.Concept,                                    
                    Description = m.Description,                                    
                    Income = m.Income,                                    
                    Outcome = m.Outcome,                                    
                    Year = m.Year,                                    
                    Month = m.Month,                                    
                    Created = m.Created,                                    
                    Author = m.Author,                                    
                    Modified = m.Modified,                                    
                    Editor = m.Editor,                                    
                    AuthorName = _context.Users.FirstOrDefault(a => a.Id == m.Author).Name,
                    EditorName = _context.Users.FirstOrDefault(e => e.Id == m.Editor).Name,
                })
                .ToListAsync();
            return View(operations);
        }
        
        public async Task<IActionResult> Details(Guid UniqueId)
        {
            var operation = await _context.Operations
                .Include(m => m.Account)
                .Select(m => new Operation
                {
                    Id = m.Id,                                    
                    UniqueId = m.UniqueId,                                    
                    Type = m.Type,                                    
                    Modality = m.Modality,                                    
                    Number = m.Number,                                    
                    AccountId = m.AccountId,                                    
                    OperationDate = m.OperationDate,                                    
                    Concept = m.Concept,                                    
                    Description = m.Description,                                    
                    Income = m.Income,                                    
                    Outcome = m.Outcome,                                    
                    Year = m.Year,                                    
                    Month = m.Month,                                    
                    Created = m.Created,                                    
                    Author = m.Author,                                    
                    Modified = m.Modified,                                    
                    Editor = m.Editor,                                    
                AuthorName = _context.Users.FirstOrDefault(a => a.Id == m.Author).Name,
                EditorName = _context.Users.FirstOrDefault(e => e.Id == m.Editor).Name,
                })
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
        public async Task<IActionResult> Create(Operation operation)
        {
            try
            {
                operation.UniqueId = Guid.NewGuid();
                operation.Created = DateTime.Now;
                operation.Author = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(operation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Agregado exitosamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
                ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", operation.AccountId);
           
            return View();
        }
        
        public async Task<IActionResult> Edit(Guid UniqueId)
        {
            var operation = await _context.Operations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (operation == null)
            {
                return NotFound();
            }
                ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", operation.AccountId);
            return View(operation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Operation operation)
        {
            try
            {
                var operationEdit = await _context.Operations.FirstOrDefaultAsync(m => m.UniqueId == operation.UniqueId);

                operationEdit.Type = operation.Type; 
                operationEdit.Modality = operation.Modality; 
                operationEdit.Number = operation.Number; 
                operationEdit.AccountId = operation.AccountId; 
                operationEdit.OperationDate = operation.OperationDate; 
                operationEdit.Concept = operation.Concept; 
                operationEdit.Description = operation.Description; 
                operationEdit.Income = operation.Income; 
                operationEdit.Outcome = operation.Outcome; 
                operationEdit.Year = operation.Year; 
                operationEdit.Month = operation.Month; 
                operationEdit.Modified = DateTime.Now;
                operationEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
                _context.Update(operationEdit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Editado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;    
            }
                ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", operation.AccountId);
            return View(operation);
        }
        
        public async Task<IActionResult> Delete(Guid UniqueId)
        {
            var operation = await _context.Operations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (operation == null)
            {
                return NotFound();
            }

            return View(operation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var operation = await _context.Operations.FindAsync(id);
                if (operation != null)
                {
                    _context.Operations.Remove(operation);
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