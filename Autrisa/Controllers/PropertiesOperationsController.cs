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

namespace Autrisa.Controllers
{
    public class PropertiesOperationsController : Controller
    {
        private readonly EFContext _context;

        public PropertiesOperationsController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int PropertyId)
        {
            var operations = await _context.PropertiesOperations
                .Include(m => m.Property)
                .ToListAsync();
            ViewBag.PropertyId = PropertyId;
            return View(operations);
        }

        ////public async Task<IActionResult> Details(Guid UniqueId)
        ////{
        ////    var operation = await _context.InvestmentsOperations
        ////        .Include(m => m.Account)
        ////        .FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
        ////    if (operation == null)
        ////    {
        ////        return NotFound();
        ////    }
        ////    return View(operation);
        ////}

        public IActionResult Create(int PropertyId)
        {
            ViewBag.PropertyId = PropertyId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertiesOperation propertyoperation, int PropertyId)
        {
            try
            {
                var accountEdit = await _context.Properties.FirstOrDefaultAsync(m => m.Id == PropertyId);

                propertyoperation.UniqueId = Guid.NewGuid();
                propertyoperation.Created = DateTime.Now;
                propertyoperation.Author = (int)HttpContext.Session.GetInt32("UserId");

                var montoInicial = accountEdit.Amount;
                if (propertyoperation.Type == 0)
                {
                    accountEdit.Amount = montoInicial - propertyoperation.Amount;
                }
                else if (propertyoperation.Type == 1)
                {
                    accountEdit.Amount = montoInicial + propertyoperation.Amount;
                }

                _context.Update(accountEdit);
                _context.Add(propertyoperation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Agregado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            //ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", propertyoperation.AccountId);
            return View();
        }

        public async Task<IActionResult> Edit(Guid UniqueId)
        {
            var operation = await _context.PropertiesOperations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (operation == null)
            {
                return NotFound();
            }
            //ViewBag.AccountId = new SelectList(_context.Accounts.Where(m => m.AccountType == "Predios"), "Id", "Name", operation.AccountId);
            return View(operation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PropertiesOperation propertyoperation)
        {
            try
            {
                var operationEdit = await _context.PropertiesOperations.FirstOrDefaultAsync(m => m.UniqueId == propertyoperation.UniqueId);

                if (operationEdit.Type != propertyoperation.Type && operationEdit.Type == 0)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.PropertyId);
                    repay.Amount = operationEdit.Amount + repay.Amount;
                    operationEdit.Type = propertyoperation.Type;
                    _context.Update(repay);
                }

                else if (operationEdit.Type != propertyoperation.Type && operationEdit.Type == 1)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.PropertyId);
                    repay.Amount = repay.Amount - operationEdit.Amount;
                    operationEdit.PropertyId = propertyoperation.PropertyId;
                    _context.Update(repay);
                }
                //________________________________________________________________________________________________________
                if (propertyoperation.Type == 0 && operationEdit.Type == 1)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == propertyoperation.PropertyId);
                    repay.Amount = repay.Amount - propertyoperation.Amount;
                    operationEdit.Type = propertyoperation.Type;
                }
                else if (propertyoperation.Type == 1 && operationEdit.Type == 0)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == propertyoperation.PropertyId);
                    repay.Amount = repay.Amount + propertyoperation.Amount;
                    operationEdit.Type = propertyoperation.Type;
                }
                else
                {
                    operationEdit.Type = propertyoperation.Type;
                }
                //________________________________________________________________________________________________________
                if (operationEdit.PropertyId != propertyoperation.PropertyId && operationEdit.Type == 0)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.PropertyId);
                    repay.Amount = repay.Amount + operationEdit.Amount;
                    operationEdit.PropertyId = propertyoperation.PropertyId;
                    _context.Update(repay);
                }

                else if (operationEdit.PropertyId != propertyoperation.PropertyId && operationEdit.Type == 1)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.PropertyId);
                    repay.Amount = repay.Amount - operationEdit.Amount;
                    operationEdit.PropertyId = propertyoperation.PropertyId;
                    _context.Update(repay);
                }
                else
                {
                    operationEdit.PropertyId = propertyoperation.PropertyId;
                }
                //__________________________________________________________________________________________________________
                if (propertyoperation.Type == 0)
                {
                    var pay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == propertyoperation.PropertyId);
                    pay.Amount = pay.Amount - propertyoperation.Amount;
                    _context.Update(pay);
                }
                else if (propertyoperation.Type == 1)
                {
                    var pay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == propertyoperation.PropertyId);
                    pay.Amount = pay.Amount + propertyoperation.Amount;
                    _context.Update(pay);
                }

                operationEdit.Modality = propertyoperation.Modality;
                operationEdit.Description = propertyoperation.Description;

                operationEdit.Modified = DateTime.Now;
                operationEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
                _context.Update(operationEdit);
                _context.Update(propertyoperation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Editado exitosamente";
                return RedirectToAction(nameof(Index), "Lendings");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            //ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", lendingoperation.AccountId);
            return View(propertyoperation);
        }

        public async Task<IActionResult> Delete(Guid UniqueId)
        {
            var operation = await _context.PropertiesOperations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
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
                var operation = await _context.PropertiesOperations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                if (operation != null)
                {
                    _context.PropertiesOperations.Remove(operation);
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

        public async Task<IActionResult> Reportes(int PropertyId)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.AddWorksheet();
            int cont = 2;

            var operation = _context.PropertiesOperations.Include(m => m.Property)
            .ThenInclude(m => m.Account).Where(m => m.PropertyId == PropertyId).ToList();

            ws.Cell("C3").Value = "Reporte de Predios";
            ws.Range("C3:D3").Row(1).Merge();
            ws.Cell("G1").Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy");

            cont = cont + 2;
            ws.Cell("A" + cont).Value = "Propiedad";
            cont = cont + 2; ;

            ws.Range("A" + cont, "G" + cont).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont, "G" + cont).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont, "G" + cont).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont, "G" + cont).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont).Value = "Fecha";
            ws.Cell("B" + cont).Value = "Tipo";
            ws.Cell("C" + cont).Value = "Modalidad";
            ws.Cell("D" + cont).Value = "Receptor";
            ws.Cell("E" + cont).Value = "Moneda";
            ws.Cell("F" + cont).Value = "Monto";
            ws.Cell("G" + cont).Value = "Descripción";

            cont = 4;
            var iteration = 0;
            foreach (var item in operation)
            {
                ws.Cell("B" + cont).Value = item.Property.Address;
                ws.Range("B4:C5").Row(1).Merge();

                cont = 7 + iteration;

                ws.Cell("A" + cont).Value = item.OperationDate;
                if (item.Type == 0)
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
                else if (item.Modality == 1)
                {
                    ws.Cell("C" + cont).Value = "Cheque";
                }
                else
                {
                    ws.Cell("C" + cont).Value = "Efectivo";
                }

                ws.Cell("D" + cont).Value = item.Receptor;

                if (item.Property.Currency == 0)
                {
                    ws.Cell("E" + cont).Value = "Soles";
                }
                else
                {
                    ws.Cell("E" + cont).Value = "Dólares";
                }

                ws.Cell("F" + cont).Value = item.Amount;
                ws.Cell("G" + cont).Value = item.Description;
                cont++;
                iteration++;
            }

            ws.Columns("A", "T").AdjustToContents();
            return wb.Deliver("Reporte_Predios", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}