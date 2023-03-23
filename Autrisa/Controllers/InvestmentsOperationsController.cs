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
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Autrisa.Controllers
{
    public class InvestmentsOperationsController : Controller
    {
        private readonly EFContext _context;

        public InvestmentsOperationsController(EFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var operations = await _context.InvestmentsOperations
                .Include(m => m.Investment)
                .Where(m => m.InvestmentId == Id)
                .ToListAsync();
            ViewBag.InvestmentId = Id;
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

        public IActionResult Create(int InvestmentId)
        {
            ViewBag.InvestmentId = InvestmentId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvestmentsOperation investmentsoperation, int InvestmentId)
        {
            try
            {
                var accountEdit = await _context.Investments.FirstOrDefaultAsync(m => m.Id == InvestmentId);

                investmentsoperation.UniqueId = Guid.NewGuid();
                investmentsoperation.Created = DateTime.Now;
                investmentsoperation.Author = (int)HttpContext.Session.GetInt32("UserId");

                var montoInicial = accountEdit.Amount;
                if (investmentsoperation.Type == 0)
                {
                    accountEdit.Amount = montoInicial - investmentsoperation.Amount;
                }
                else if (investmentsoperation.Type == 1)
                {
                    accountEdit.Amount = montoInicial + investmentsoperation.Amount;
                }

                _context.Update(accountEdit);
                _context.Add(investmentsoperation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Agregado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            //ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", investmentsoperation.AccountId);

            return View();
        }

        public async Task<IActionResult> Edit(Guid UniqueId)
        {
            var operation = await _context.InvestmentsOperations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            if (operation == null)
            {
                return NotFound();
            }
            return View(operation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InvestmentsOperation investmentsoperation)
        {
            try
            {
                var operationEdit = await _context.InvestmentsOperations.FirstOrDefaultAsync(m => m.UniqueId == investmentsoperation.UniqueId);

                if (operationEdit.Type != investmentsoperation.Type && operationEdit.Type == 0)
                {
                    var repay = await _context.Investments.FirstOrDefaultAsync(m => m.Id == operationEdit.InvestmentId);
                    repay.Amount = operationEdit.Amount + repay.Amount;
                    operationEdit.Type = investmentsoperation.Type;
                    _context.Update(repay);
                }

                else if (operationEdit.Type != investmentsoperation.Type && operationEdit.Type == 1)
                {
                    var repay = await _context.Investments.FirstOrDefaultAsync(m => m.Id == operationEdit.InvestmentId);
                    repay.Amount = repay.Amount - operationEdit.Amount;
                    operationEdit.InvestmentId = investmentsoperation.InvestmentId;
                    _context.Update(repay);
                }
                //________________________________________________________________________________________________________
                if (investmentsoperation.Type == 0 && operationEdit.Type == 1)
                {
                    var repay = await _context.Investments.FirstOrDefaultAsync(m => m.Id == investmentsoperation.InvestmentId);
                    repay.Amount = repay.Amount - investmentsoperation.Amount;
                    operationEdit.Type = investmentsoperation.Type;
                }
                else if (investmentsoperation.Type == 1 && operationEdit.Type == 0)
                {
                    var repay = await _context.Investments.FirstOrDefaultAsync(m => m.Id == investmentsoperation.InvestmentId);
                    repay.Amount = repay.Amount + investmentsoperation.Amount;
                    operationEdit.Type = investmentsoperation.Type;
                }
                else
                {
                    operationEdit.Type = investmentsoperation.Type;
                }
                //________________________________________________________________________________________________________
                if (operationEdit.InvestmentId != investmentsoperation.InvestmentId && operationEdit.Type == 0)
                {
                    var repay = await _context.Investments.FirstOrDefaultAsync(m => m.Id == operationEdit.InvestmentId);
                    repay.Amount = repay.Amount + operationEdit.Amount;
                    operationEdit.InvestmentId = investmentsoperation.InvestmentId;
                    _context.Update(repay);
                }

                else if (operationEdit.InvestmentId != investmentsoperation.InvestmentId && operationEdit.Type == 1)
                {
                    var repay = await _context.Investments.FirstOrDefaultAsync(m => m.Id == operationEdit.InvestmentId);
                    repay.Amount = repay.Amount - operationEdit.Amount;
                    operationEdit.InvestmentId = investmentsoperation.InvestmentId;
                    _context.Update(repay);
                }
                else
                {
                    operationEdit.InvestmentId = investmentsoperation.InvestmentId;
                }
                //__________________________________________________________________________________________________________
                if (investmentsoperation.Type == 0)
                {
                    var pay = await _context.Investments.FirstOrDefaultAsync(m => m.Id == investmentsoperation.InvestmentId);
                    pay.Amount = pay.Amount - investmentsoperation.Amount;
                    _context.Update(pay);
                }
                else if (investmentsoperation.Type == 1)
                {
                    var pay = await _context.Investments.FirstOrDefaultAsync(m => m.Id == investmentsoperation.InvestmentId);
                    pay.Amount = pay.Amount + investmentsoperation.Amount;
                    _context.Update(pay);
                }

                operationEdit.Modality = investmentsoperation.Modality;
                operationEdit.Description = investmentsoperation.Description;
                investmentsoperation.Created = operationEdit.Created;
                operationEdit.Modified = DateTime.Now;
                operationEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
                investmentsoperation.Modified = DateTime.Now;
                investmentsoperation.Editor = (int)HttpContext.Session.GetInt32("UserId");
                _context.Update(operationEdit);
                _context.Update(investmentsoperation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Editado exitosamente";
                return RedirectToAction(nameof(Index),"Investments");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            //ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", investmentsoperation.AccountId);
            return View(investmentsoperation);
        }

        public async Task<IActionResult> Delete(Guid UniqueId)
        {
            var operation = await _context.InvestmentsOperations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
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
                var operation = await _context.InvestmentsOperations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                if (operation != null)
                {
                    _context.InvestmentsOperations.Remove(operation);
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

        public async Task<IActionResult> Reportes(int InvestmentId)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.AddWorksheet();
            int cont = 2;

            var operation = _context.InvestmentsOperations.Include(m => m.Investment)
            .ThenInclude(m => m.Account).Where(m => m.InvestmentId == InvestmentId).ToList();

            ws.Cell("C3").Value = "Reporte de Inversiones";
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
                ws.Cell("B" + cont).Value = item.Investment.Customer;
                ws.Range("B4:C4").Row(1).Merge();

                cont = 7 + iteration;

                ws.Cell("A" + cont).Value = item.OperationDate;
                if (item.Type == 0)
                {
                    ws.Cell("B" + cont).Value = "Salida";
                }
                else if(item.Type == 1)
                {
                    ws.Cell("B" + cont).Value = "Aumento de capital";
                }
                else
                {
                    ws.Cell("B" + cont).Value = "Ingreso";
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

                if (item.Investment.Currency == 0)
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
            return wb.Deliver("Reporte_Inversiones", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
