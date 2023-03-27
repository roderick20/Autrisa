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
using System.Drawing;
using System.Globalization;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace Autrisa.Controllers
{
    public class LendingOperationsController : Controller
    {
        private readonly EFContext _context;

        public LendingOperationsController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var operations = await _context.LendingOperations
                .Include(m => m.Lending)
                .Where(m => m.LendingId == Id)
                .ToListAsync();
            ViewBag.LendingId = Id;
            return View(operations);
        }

        public IActionResult Create(int LendingId)
        {
            ViewBag.LendingId = LendingId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LendingOperation lendingoperation, int LendingId, string OperationDate,
            string Created)
        {
            try
            {
                var accountEdit = await _context.Lendings.FirstOrDefaultAsync(m => m.Id == LendingId);

                lendingoperation.OperationDate = DateTime.ParseExact(OperationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                lendingoperation.UniqueId = Guid.NewGuid();
                //lendingoperation.Created = DateTime.Now;
                lendingoperation.Author = (int)HttpContext.Session.GetInt32("UserId");
                
                if (lendingoperation.Type == 0)
                {
                    if(accountEdit.Currency == 0)
                    {
                        var montoInicial = accountEdit.SolesAmount;
                        accountEdit.SolesAmount = montoInicial - lendingoperation.Amount;
                    }
                    else
                    {
                        var montoInicial = accountEdit.DollarsAmount;
                        accountEdit.DollarsAmount = montoInicial - lendingoperation.Amount;
                    }
                    
                }
                else if (lendingoperation.Type == 1)
                {
                    if (accountEdit.Currency == 0)
                    {
                        var montoInicial = accountEdit.SolesAmount;
                        accountEdit.SolesAmount = montoInicial + lendingoperation.Amount;
                    }
                    else
                    {
                        var montoInicial = accountEdit.DollarsAmount;
                        accountEdit.DollarsAmount = montoInicial + lendingoperation.Amount;
                    }
                }
                lendingoperation.Created = DateTime.Now;
                _context.Update(accountEdit);
                _context.Add(lendingoperation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Agregado exitosamente";
                return RedirectToAction(nameof(Index), "LendingOperations", new { Id = LendingId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            //  ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", lendingoperation.AccountId);

            return View();
        }

        public async Task<IActionResult> Edit(Guid UniqueId)
        {
            var operation = await _context.LendingOperations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (operation == null)
            {
                return NotFound();
            }
            //ViewBag.AccountId = new SelectList(_context.Accounts.Where(m => m.AccountType == "Préstamo"), "Id", "Name", operation.AccountId);
            return View(operation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LendingOperation lendingoperation, string Created, string Modified)
        {
            try
            {
                var operationEdit = await _context.LendingOperations.FirstOrDefaultAsync(m => m.UniqueId == lendingoperation.UniqueId);

                if (operationEdit.Type != lendingoperation.Type && operationEdit.Type == 0)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.LendingId);
                    repay.SolesAmount = operationEdit.Amount + repay.SolesAmount;
                    operationEdit.Type = lendingoperation.Type;
                    _context.Update(repay);
                }

                else if (operationEdit.Type != lendingoperation.Type && operationEdit.Type == 1)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.LendingId);
                    repay.DollarsAmount = repay.DollarsAmount - operationEdit.Amount;
                    operationEdit.LendingId = lendingoperation.LendingId;
                    _context.Update(repay);
                }
                //________________________________________________________________________________________________________
                if (lendingoperation.Type == 0 && operationEdit.Type == 1)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == lendingoperation.LendingId);
                    repay.SolesAmount = repay.SolesAmount - lendingoperation.Amount;
                    operationEdit.Type = lendingoperation.Type;
                }
                else if (lendingoperation.Type == 1 && operationEdit.Type == 0)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == lendingoperation.LendingId);
                    repay.DollarsAmount = repay.DollarsAmount + lendingoperation.Amount;
                    operationEdit.Type = lendingoperation.Type;
                }
                else
                {
                    operationEdit.Type = lendingoperation.Type;
                }
                //________________________________________________________________________________________________________
                if (operationEdit.LendingId != lendingoperation.LendingId && operationEdit.Type == 0)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.LendingId);
                    repay.SolesAmount = repay.SolesAmount + operationEdit.Amount;
                    operationEdit.LendingId = lendingoperation.LendingId;
                    _context.Update(repay);
                }

                else if (operationEdit.LendingId != lendingoperation.LendingId && operationEdit.Type == 1)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.LendingId);
                    repay.DollarsAmount = repay.DollarsAmount - operationEdit.Amount;
                    operationEdit.LendingId = lendingoperation.LendingId;
                    _context.Update(repay);
                }
                else
                {
                    operationEdit.LendingId = lendingoperation.LendingId;
                }
                //__________________________________________________________________________________________________________
                if (lendingoperation.Type == 0)
                {
                    var pay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == lendingoperation.LendingId);
                    pay.SolesAmount = pay.SolesAmount - lendingoperation.Amount;
                    _context.Update(pay);
                }
                else if (lendingoperation.Type == 1)
                {
                    var pay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == lendingoperation.LendingId);
                    pay.DollarsAmount = pay.DollarsAmount + lendingoperation.Amount;
                    _context.Update(pay);
                }

                operationEdit.Modality = lendingoperation.Modality;
                operationEdit.Description = lendingoperation.Description;

                //lendingoperation.Created = operationEdit.Created;
                lendingoperation.Created = DateTime.ParseExact(Created, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //operationEdit.Modified = DateTime.Now;
                operationEdit.Modified = DateTime.ParseExact(Modified, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                operationEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
                //lendingoperation.Modified = DateTime.Now;
                lendingoperation.Modified = DateTime.ParseExact(Modified, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                lendingoperation.Editor = (int)HttpContext.Session.GetInt32("UserId");

                _context.Update(operationEdit);
                _context.Update(lendingoperation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Editado exitosamente";
                return RedirectToAction(nameof(Index), "Lendings");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            //ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", lendingoperation.AccountId);
            return View(lendingoperation);
        }

        public async Task<IActionResult> Delete(Guid UniqueId)
        {
            var operation = await _context.LendingOperations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (operation == null)
            {
                return NotFound();
            }

            return View(operation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid UniqueId)
        {
            try
            {
                var operation = await _context.LendingOperations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                if (operation != null)
                {
                    _context.LendingOperations.Remove(operation);
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

        public async Task<IActionResult> Reportes(int LendingId)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.AddWorksheet();
            int cont = 2;

            var operation = _context.LendingOperations.Include(m => m.Lending)
            .ThenInclude(m => m.Account).Where(m => m.LendingId == LendingId).ToList();

            ws.Cell("C3").Value = "Reporte de Préstamo";
            ws.Range("C3:D3").Row(1).Merge();
            ws.Cell("F1").Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy");

            cont = cont + 2;
            ws.Cell("A" + cont).Value = "Cliente";
            cont = cont + 2; ;

            ws.Range("A" + cont, "F" + cont).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont, "F" + cont).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont, "F" + cont).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont, "F" + cont).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont).Value = "Fecha";
            ws.Cell("B" + cont).Value = "Tipo";
            ws.Cell("C" + cont).Value = "Modalidad";
            ws.Cell("D" + cont).Value = "Moneda";
            ws.Cell("E" + cont).Value = "Monto";
            ws.Cell("F" + cont).Value = "Descripción";

            cont = 4;
            var iteration = 0;
            foreach (var item in operation)
            {
                ws.Cell("B" + cont).Value = item.Lending.Customer;
                ws.Range("B4:C4").Row(1).Merge();

                cont = 7 + iteration;

                ws.Cell("A" + cont).Value = item.OperationDate;
                if(item.Type == 0)
                {
                    ws.Cell("B" + cont).Value = "Salida de dinero";
                }
                else
                {
                    ws.Cell("B" + cont).Value = "Ingreso de dinero";
                }

                if (item.Modality == 0)
                {
                    ws.Cell("C" + cont).Value = "Transferencia";
                }
                else if(item.Modality == 1)
                {
                    ws.Cell("C" + cont).Value = "Cheque";
                }
                else
                {
                    ws.Cell("C" + cont).Value = "Efectivo";
                }

                if (item.Lending.Currency == 0)
                {
                    ws.Cell("D" + cont).Value = "Soles";
                }
                else
                {
                    ws.Cell("D" + cont).Value = "Dólares";
                }

                ws.Cell("E" + cont).Value = item.Amount;
                ws.Cell("F" + cont).Value = item.Description;
                cont++;
                iteration++;
            }

            ws.Columns("A", "T").AdjustToContents();
            return wb.Deliver("Reporte_Préstamo", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}