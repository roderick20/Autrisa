﻿using System;
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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DocumentFormat.OpenXml.Office.CustomDocumentInformationPanel;

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
                .Where(m => m.PropertyId == PropertyId)
                .ToListAsync();
            ViewBag.PropertyId = PropertyId;
            return View(operations);
        }


        public IActionResult Create(int PropertyId)
        {
            ViewBag.PropertyId = PropertyId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertiesOperation propertyoperation, int PropertyId, string OperationDate)
        {
            try
            {
                var accountEdit = await _context.Properties.FirstOrDefaultAsync(m => m.Id == PropertyId);

                propertyoperation.OperationDate = DateTime.ParseExact(OperationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                propertyoperation.UniqueId = Guid.NewGuid();
                propertyoperation.Created = DateTime.Now;
                propertyoperation.Author = (int)HttpContext.Session.GetInt32("UserId");

                if (accountEdit.Currency == 0)
                {
                    var montoInicial = accountEdit.SolesAmount;
                    accountEdit.SolesAmount = montoInicial - propertyoperation.Amount;
                }
                else if (accountEdit.Currency == 1)
                {
                    var montoInicial = accountEdit.DollarsAmount;
                    accountEdit.DollarsAmount = montoInicial + propertyoperation.Amount;
                }

                if (propertyoperation.Type == 0)
                {
                    if (accountEdit.Currency == 0)
                    {
                        var montoInicial = accountEdit.OutcomeSoles;
                        accountEdit.OutcomeSoles = montoInicial + propertyoperation.Amount;
                    }
                    else
                    {
                        var montoInicial = accountEdit.OutcomeDollars;
                        accountEdit.OutcomeDollars = montoInicial + propertyoperation.Amount;
                    }
                    
                }
                else if (propertyoperation.Type == 1)
                {
                    if(accountEdit.Currency == 0)
                    {
                        var montoInicial = accountEdit.IncomeSoles;
                        accountEdit.IncomeSoles = montoInicial + propertyoperation.Amount;
                    }
                    else
                    {
                        var montoInicial = accountEdit.IncomeDollars;
                        accountEdit.IncomeDollars = montoInicial + propertyoperation.Amount;
                    }
                }

                _context.Update(accountEdit);
                _context.Add(propertyoperation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Agregado exitosamente";
                return RedirectToAction(nameof(Index), "PropertiesOperations", new { PropertyId = PropertyId });
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
        public async Task<IActionResult> Edit(PropertiesOperation propertyoperation, string Created)
        {
            try
            {
                var operationEdit = await _context.PropertiesOperations.FirstOrDefaultAsync(m => m.UniqueId == propertyoperation.UniqueId);

                if (operationEdit.Type != propertyoperation.Type && operationEdit.Type == 0)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.PropertyId);
                    repay.SolesAmount = operationEdit.Amount + repay.SolesAmount;
                    if (repay.Currency == 0)
                    {
                        //salida a ingreso
                        repay.OutcomeSoles = repay.OutcomeSoles - repay.OutcomeSoles;
                    }
                    else
                    {
                        repay.OutcomeDollars = repay.OutcomeDollars - repay.OutcomeDollars;
                    }
                    operationEdit.Type = propertyoperation.Type;
                    _context.Update(repay);
                }

                else if (operationEdit.Type != propertyoperation.Type && operationEdit.Type == 1)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.PropertyId);
                    repay.DollarsAmount = repay.DollarsAmount - operationEdit.Amount;
                    if (repay.Currency == 0)
                    {
                        //salida a ingreso
                        repay.IncomeSoles = repay.IncomeSoles - repay.IncomeSoles;
                    }
                    else
                    {
                        repay.IncomeDollars = repay.IncomeDollars - repay.IncomeDollars;
                    }
                    operationEdit.PropertyId = propertyoperation.PropertyId;
                    _context.Update(repay);
                }
                //________________________________________________________________________________________________________
                if (propertyoperation.Type == 0 && operationEdit.Type == 1)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == propertyoperation.PropertyId);
                    if (repay.Currency == 0)
                    {
                        repay.SolesAmount = repay.SolesAmount - propertyoperation.Amount;
                        //repay.IncomeSoles = repay.IncomeSoles - operationEdit.Amount;
                    }
                    else
                    {
                        repay.DollarsAmount = repay.DollarsAmount - propertyoperation.Amount;
                        //repay.IncomeDollars = repay.IncomeDollars - operationEdit.Amount;
                    }
                    operationEdit.Type = propertyoperation.Type;
                }
                else if (propertyoperation.Type == 1 && operationEdit.Type == 0)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == propertyoperation.PropertyId);
                    if (repay.Currency == 0)
                    {
                        repay.SolesAmount = repay.SolesAmount - propertyoperation.Amount;
                        repay.OutcomeSoles = repay.OutcomeSoles - operationEdit.Amount;
                    }
                    else
                    {
                        repay.DollarsAmount = repay.DollarsAmount - propertyoperation.Amount;
                        repay.OutcomeDollars = repay.OutcomeDollars - operationEdit.Amount;
                    }
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
                    repay.SolesAmount = repay.SolesAmount + operationEdit.Amount;
                    operationEdit.PropertyId = propertyoperation.PropertyId;
                    _context.Update(repay);
                }

                else if (operationEdit.PropertyId != propertyoperation.PropertyId && operationEdit.Type == 1)
                {
                    var repay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.PropertyId);
                    repay.DollarsAmount = repay.DollarsAmount - operationEdit.Amount;
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
                    var pay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == operationEdit.PropertyId);
                    pay.SolesAmount = pay.SolesAmount - operationEdit.Amount;
                    operationEdit.Amount = propertyoperation.Amount;
                    if (pay.Currency == 0)
                    {
                        pay.OutcomeSoles = pay.OutcomeSoles + propertyoperation.Amount;
                    }
                    else
                    {
                        pay.OutcomeDollars = pay.OutcomeDollars + propertyoperation.Amount;
                    }
                    _context.Update(pay);
                }
                else if (propertyoperation.Type == 1)
                {
                    var pay = await _context.Properties.FirstOrDefaultAsync(m => m.Id == propertyoperation.PropertyId);
                    pay.DollarsAmount = pay.DollarsAmount - operationEdit.Amount;
                    pay.DollarsAmount = operationEdit.Amount;
                    if (pay.Currency == 0)
                    {
                        pay.IncomeSoles = pay.IncomeSoles + propertyoperation.Amount;
                    }
                    else
                    {
                        pay.IncomeDollars = pay.IncomeDollars + propertyoperation.Amount;
                    }
                    _context.Update(pay);
                }
                operationEdit.Amount = propertyoperation.Amount;
                string Modified = DateTime.Now.ToString("dd/MM/yyyy");
                operationEdit.Modality = propertyoperation.Modality;
                operationEdit.Description = propertyoperation.Description;
                operationEdit.Created = DateTime.ParseExact(Created, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                operationEdit.Modified = DateTime.ParseExact(Modified, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                operationEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");
                _context.Update(operationEdit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Editado exitosamente";
                return RedirectToAction(nameof(Index), "Properties");
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
                var data = operation.Id;
                if (operation != null)
                {
                    _context.PropertiesOperations.Remove(operation);
                }
                await _context.SaveChangesAsync();
                TempData["Success"] = "Eliminado exitosamente";
                return RedirectToAction(nameof(Index), "PropertiesOperations", new { PropertyId = data });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), "PropertiesOperations");
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