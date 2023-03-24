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
                    //AuthorName = _context.Users.FirstOrDefault(a => a.Id == m.Author).Name,
                    //EditorName = _context.Users.FirstOrDefault(e => e.Id == m.Editor).Name,
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
        public async Task<IActionResult> Create(Operation operation, int montoTransaccion, string Created)
        {
            try
            {
                var accountEdit = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == operation.AccountId);

                operation.UniqueId = Guid.NewGuid();
                //operation.Created = DateTime.Now;
                operation.Created = DateTime.ParseExact(Created, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                operation.Author = (int)HttpContext.Session.GetInt32("UserId");
                DateTime selectedDate = operation.OperationDate;
                operation.Year = selectedDate.Year;
                operation.Month = selectedDate.Month;

                var montoInicial = accountEdit.Amount;
                if (operation.Type == 0)
                {
                    accountEdit.Amount = montoInicial + montoTransaccion;
                    operation.Income = montoTransaccion;
                    operation.Outcome = 0;
                }
                else if (operation.Type == 1)
                {
                    accountEdit.Amount = montoInicial - montoTransaccion;
                    operation.Outcome = montoTransaccion;
                    operation.Income = 0;
                }

                _context.Update(accountEdit);
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
        public async Task<IActionResult> Edit(Operation operation, string OperationDate, string Modified)
        {
            try
            {
                var operationEdit = await _context.Operations.FirstOrDefaultAsync(m => m.UniqueId == operation.UniqueId);

                operationEdit.Type = operation.Type;
                operationEdit.Modality = operation.Modality;
                operationEdit.Number = operation.Number;
                operationEdit.AccountId = operation.AccountId;
                operationEdit.OperationDate = DateTime.ParseExact(OperationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //operationEdit.OperationDate = operation.OperationDate;
                operationEdit.Concept = operation.Concept;
                operationEdit.Description = operation.Description;
                operationEdit.Income = operation.Income;
                operationEdit.Outcome = operation.Outcome;
                operationEdit.Year = operation.Year;
                operationEdit.Month = operation.Month;
                operationEdit.Modified = DateTime.ParseExact(Modified, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //operationEdit.Modified = DateTime.Now;
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
        public async Task<IActionResult> DeleteConfirmed(Guid UniqueId)
        {
            try
            {
                var operation = await _context.Operations.FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
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

        public async Task<IActionResult> MonthClosure()
        {
            var years = await _context.Operations.Select(m => m.Year).Distinct().ToListAsync();
            ViewBag.YearList = years;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MonthClosure(int Month, int Year)
        {
            try
            {
                var accountData = await _context.Accounts.Where(m => m.AccountType != "Inversión"
                && m.AccountType != "Préstamo" && m.AccountType != "Predios").ToListAsync();
                decimal? accountMoney = 0;
                decimal? accountIncome = 0;
                decimal? accountOutcome = 0;
                decimal? totalIncome = 0;
                decimal? totalOutcome = 0;

                foreach (var account in accountData)
                {
                    accountMoney = account.PreviousRemaining;
                    var operationsData = await _context.Operations.Where(m => m.Year == Year && m.Month == Month 
                    && m.AccountId == account.Id && m.Type != 2).ToListAsync();

                    foreach (var operation in operationsData)
                    {
                        accountIncome = operation.Income;
                        accountOutcome = operation.Outcome;
                        totalIncome = totalIncome + accountIncome;
                        totalOutcome = totalOutcome + accountOutcome;
                    }

                    Operation closhure = new Operation();
                    closhure.UniqueId = Guid.NewGuid();
                    closhure.Created = DateTime.Now;
                    closhure.Author = (int)HttpContext.Session.GetInt32("UserId");
                    DateTime selectedDate = closhure.Created;
                    closhure.Year = selectedDate.Year;
                    closhure.Month = selectedDate.Month;
                    closhure.Income = totalIncome;
                    closhure.Outcome = totalOutcome;
                    closhure.Concept = "Cierre de mes";
                    closhure.Description = "Cierre de mes";
                    closhure.Modality = 100;
                    closhure.Type = 2;
                    closhure.Number = 0;
                    closhure.OperationDate = DateTime.Now;
                    closhure.AccountId = account.Id;
                    _context.Add(closhure);

                    if (accountMoney + totalIncome - totalOutcome == account.Amount)
                    {
                        account.PreviousRemaining = account.Amount;
                        account.Modified = DateTime.Now;
                        account.Editor = (int)HttpContext.Session.GetInt32("UserId");
                        _context.Update(account);
                    }
                    accountMoney = 0;
                    accountIncome = 0;
                    accountOutcome = 0;
                    totalIncome = 0;
                    totalOutcome = 0;
                }

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

        public async Task<IActionResult> Reportes()
        {
            var years = await _context.Operations.Select(m => m.Year).Distinct().ToListAsync();
            ViewBag.YearList = years;

            var accounts = await _context.Accounts.Where(m => m.AccountType == "Corriente" || m.AccountType == "Ahorros")
            .Select(m => m.Name).Distinct().ToListAsync();
            ViewBag.AccountId = accounts;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reportes(String fechaInicio, String fechaFin, string AccountId, int Modality)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            string[] formats = { "dd/MM/yyyy" };
            var fechaInicioDT = DateTime.ParseExact(fechaInicio, formats, new CultureInfo("en-US"), DateTimeStyles.None);
            var fechaFinDT = DateTime.ParseExact(fechaFin, formats, new CultureInfo("en-US"), DateTimeStyles.None);
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.AddWorksheet();
            int cont = 2;

            ws.Cell("A" + cont).Value = "Reporte de Movimientos";

            cont = cont + 2;

            ws.Range("A" + cont, "N" + cont).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont, "N" + cont).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont, "N" + cont).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont, "N" + cont).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont).Value = "Banco";
            ws.Cell("B" + cont).Value = "Número de cuenta";
            ws.Cell("C" + cont).Value = "Tipo de cuenta";
            ws.Cell("D" + cont).Value = "Moneda";
            ws.Cell("E" + cont).Value = "Saldo mes anterior";
            ws.Cell("F" + cont).Value = "Movimiento";
            ws.Cell("G" + cont).Value = "Concepto";
            ws.Cell("H" + cont).Value = "Descripción";
            ws.Cell("I" + cont).Value = "Monto";
            ws.Cell("J" + cont).Value = "Modalidad";
            ws.Cell("K" + cont).Value = "Número";
            ws.Cell("L" + cont).Value = "Fecha";
            //ws.Cell("M" + cont).Value = "Monto";
            cont++;

            if (AccountId == "none")
            {
                if (Modality == 1000)
                {
                    var account = _context.Accounts.ToList();
                    foreach (var item in account)
                    {
                        var operation = _context.Operations.Include(m => m.Account).Where(m => m.AccountId == item.Id).ToList();

                        foreach (var obj in operation)
                        {
                            ws.Cell("A" + cont).Value = item.Name;
                            ws.Cell("B" + cont).Value = item.AccountNumber;
                            ws.Cell("C" + cont).Value = item.AccountType;

                            if (item.Currency == 0)
                            {
                                ws.Cell("D" + cont).Value = "Soles";
                            }
                            else
                            {
                                ws.Cell("D" + cont).Value = "Dólares";
                            }

                            ws.Cell("E" + cont).Value = item.PreviousRemaining;

                            if (obj.Type == 0)
                            {
                                ws.Cell("F" + cont).Value = "Ingreso";
                                ws.Cell("I" + cont).Value = obj.Income;
                            }
                            else if (obj.Type == 1)
                            {
                                ws.Cell("F" + cont).Value = "Egreso";
                                ws.Cell("I" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("F" + cont).Value = "Cierre de mes";
                                ws.Cell("I" + cont).Value = obj.Income - obj.Outcome;
                            }

                            ws.Cell("G" + cont).Value = obj.Concept;
                            ws.Cell("H" + cont).Value = obj.Description;

                            if (obj.Modality == 0)
                            {
                                ws.Cell("J" + cont).Value = "Cheque";

                            }
                            else if (obj.Modality == 1)
                            {
                                ws.Cell("J" + cont).Value = "Transferencia";
                            }
                            else if (obj.Modality == 100)
                            {
                                ws.Cell("J" + cont).Value = "Cierre de mes";
                            }
                            ws.Cell("K" + cont).Value = obj.Number;
                            ws.Cell("L" + cont).Value = obj.OperationDate;
                            cont++;
                        }
                    }

                }
                else
                {
                    var account = _context.Accounts.ToList();
                    foreach (var item in account)
                    {
                        var operation = _context.Operations.Include(m => m.Account).Where(m => m.AccountId == item.Id && m.Modality == Modality).ToList();

                        foreach (var obj in operation)
                        {
                            ws.Cell("A" + cont).Value = item.Name;
                            ws.Cell("B" + cont).Value = item.AccountNumber;
                            ws.Cell("C" + cont).Value = item.AccountType;

                            if (item.Currency == 0)
                            {
                                ws.Cell("D" + cont).Value = "Soles";
                            }
                            else
                            {
                                ws.Cell("D" + cont).Value = "Dólares";
                            }

                            ws.Cell("E" + cont).Value = item.PreviousRemaining;

                            if (obj.Type == 0)
                            {
                                ws.Cell("F" + cont).Value = "Ingreso";
                                ws.Cell("I" + cont).Value = obj.Income;
                            }
                            else if (obj.Type == 1)
                            {
                                ws.Cell("F" + cont).Value = "Egreso";
                                ws.Cell("I" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("F" + cont).Value = "Cierre de mes";
                                ws.Cell("I" + cont).Value = obj.Income - obj.Outcome;
                            }

                            ws.Cell("G" + cont).Value = obj.Concept;
                            ws.Cell("H" + cont).Value = obj.Description;

                            if (Modality == 0)
                            {
                                ws.Cell("J" + cont).Value = "Cheque";

                            }
                            else if (Modality == 1)
                            {
                                ws.Cell("J" + cont).Value = "Transferencia";
                            }
                            else if (Modality == 100)
                            {
                                ws.Cell("J" + cont).Value = "Cierre de mes";
                            }
                            ws.Cell("K" + cont).Value = obj.Number;
                            ws.Cell("L" + cont).Value = obj.OperationDate;
                            cont++;
                        }
                    }
                }
            }

            else
            {
                if (Modality == 1000)
                {
                    var account = _context.Accounts.Where(m => m.Name == AccountId).ToList();
                    foreach (var item in account)
                    {
                        var operation = _context.Operations.Include(m => m.Account).Where(m => m.AccountId == item.Id).ToList();

                        foreach (var obj in operation)
                        {
                            ws.Cell("A" + cont).Value = item.Name;
                            ws.Cell("B" + cont).Value = item.AccountNumber;
                            ws.Cell("C" + cont).Value = item.AccountType;

                            if (item.Currency == 0)
                            {
                                ws.Cell("D" + cont).Value = "Soles";
                            }
                            else
                            {
                                ws.Cell("D" + cont).Value = "Dólares";
                            }

                            ws.Cell("E" + cont).Value = item.PreviousRemaining;

                            if (obj.Type == 0)
                            {
                                ws.Cell("F" + cont).Value = "Ingreso";
                                ws.Cell("I" + cont).Value = obj.Income;
                            }
                            else if (obj.Type == 1)
                            {
                                ws.Cell("F" + cont).Value = "Egreso";
                                ws.Cell("I" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("F" + cont).Value = "Cierre de mes";
                                ws.Cell("I" + cont).Value = obj.Income - obj.Outcome;
                            }

                            ws.Cell("G" + cont).Value = obj.Concept;
                            ws.Cell("H" + cont).Value = obj.Description;

                            if (obj.Modality == 0)
                            {
                                ws.Cell("J" + cont).Value = "Cheque";

                            }
                            else if (obj.Modality == 1)
                            {
                                ws.Cell("J" + cont).Value = "Transferencia";
                            }
                            else if (obj.Modality == 100)
                            {
                                ws.Cell("J" + cont).Value = "Cierre de mes";
                            }
                            ws.Cell("K" + cont).Value = obj.Number;
                            ws.Cell("L" + cont).Value = obj.OperationDate;
                            cont++;
                        }
                    }

                }
                else
                {
                    var account = _context.Accounts.Where(m => m.Name == AccountId).ToList();
                    foreach (var item in account)
                    {
                        var operation = _context.Operations.Include(m => m.Account).Where(m => m.AccountId == item.Id && m.Modality == Modality).ToList();

                        foreach (var obj in operation)
                        {
                            ws.Cell("A" + cont).Value = item.Name;
                            ws.Cell("B" + cont).Value = item.AccountNumber;
                            ws.Cell("C" + cont).Value = item.AccountType;

                            if (item.Currency == 0)
                            {
                                ws.Cell("D" + cont).Value = "Soles";
                            }
                            else
                            {
                                ws.Cell("D" + cont).Value = "Dólares";
                            }

                            ws.Cell("E" + cont).Value = item.PreviousRemaining;

                            if (obj.Type == 0)
                            {
                                ws.Cell("F" + cont).Value = "Ingreso";
                                ws.Cell("I" + cont).Value = obj.Income;
                            }
                            else if (obj.Type == 1)
                            {
                                ws.Cell("F" + cont).Value = "Egreso";
                                ws.Cell("I" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("F" + cont).Value = "Cierre de mes";
                                ws.Cell("I" + cont).Value = obj.Income - obj.Outcome;
                            }

                            ws.Cell("G" + cont).Value = obj.Concept;
                            ws.Cell("H" + cont).Value = obj.Description;

                            if (Modality == 0)
                            {
                                ws.Cell("J" + cont).Value = "Cheque";

                            }
                            else if (Modality == 1)
                            {
                                ws.Cell("J" + cont).Value = "Transferencia";
                            }
                            else if (Modality == 100)
                            {
                                ws.Cell("J" + cont).Value = "Cierre de mes";
                            }
                            ws.Cell("K" + cont).Value = obj.Number;
                            ws.Cell("L" + cont).Value = obj.OperationDate;
                            cont++;
                        }
                    }
                }
            }

            ws.Columns("A", "T").AdjustToContents();
            return wb.Deliver("Estado_cuenta.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

    }
}