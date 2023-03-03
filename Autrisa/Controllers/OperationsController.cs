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
        public async Task<IActionResult> Create(Operation operation, int montoTransaccion)
        {
            try
            {
                var accountEdit = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == operation.AccountId);

                operation.UniqueId = Guid.NewGuid();
                operation.Created = DateTime.Now;
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
                var accountData = await _context.Accounts.ToListAsync();
                decimal? accountMoney = 0;
                decimal? accountIncome = 0;
                decimal? accountOutcome = 0;
                decimal? totalIncome = 0;
                decimal? totalOutcome = 0;

                foreach (var account in accountData)
                {
                    accountMoney = account.PreviousRemaining;
                    var operationsData = await _context.Operations.Where(m => m.Year == Year && m.Month == Month && m.AccountId == account.Id).ToListAsync();

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

            var accounts = await _context.Accounts.Select(m => m.Name).Distinct().ToListAsync();
            ViewBag.YearList = years;
            ViewBag.AccountId = accounts;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reportes(String fechaInicio, String fechaFin, int AccountId, int Modality)
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
            ws.Cell("B" + cont).Value = "N�mero de cuenta";
            ws.Cell("C" + cont).Value = "Tipo de cuenta";
            ws.Cell("D" + cont).Value = "Moneda";
            ws.Cell("E" + cont).Value = "Saldo mes anterior";
            ws.Cell("F" + cont).Value = "Movimiento";
            ws.Cell("G" + cont).Value = "Concepto";
            ws.Cell("H" + cont).Value = "Descripci�n";
            ws.Cell("I" + cont).Value = "Monto";
            ws.Cell("J" + cont).Value = "Modalidad";
            ws.Cell("K" + cont).Value = "N�mero";
            ws.Cell("L" + cont).Value = "Fecha";
            //ws.Cell("M" + cont).Value = "Monto";
            cont++;

            if (AccountId == 1000)
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
                            
                            if(item.Currency == 0)
                            {
                                ws.Cell("D" + cont).Value = "Soles";
                            }
                            else
                            {
                                ws.Cell("D" + cont).Value = "D�lares";
                            }

                            ws.Cell("E" + cont).Value = item.PreviousRemaining;

                            if(obj.Type == 0)
                            {
                                ws.Cell("F" + cont).Value = "Ingreso";
                                ws.Cell("I" + cont).Value = obj.Income;
                            }
                            else if(obj.Type == 1)
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
                            
                            if(obj.Modality == 0)
                            {
                                ws.Cell("J" + cont).Value = "Cheque";

                            }
                            else if (obj.Modality == 1)
                            {
                                ws.Cell("J" + cont).Value = "Transferencia";
                            }
                            else if(obj.Modality == 100)
                            {
                                ws.Cell("J" + cont).Value = "Cierre de mes";
                            }
                            ws.Cell("K" + cont).Value = obj.Number;
                            ws.Cell("L" + cont).Value = obj.OperationDate;
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
                                ws.Cell("D" + cont).Value = "D�lares";
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
                        }
                    }
            }







            //foreach (var item in list)
            //{
            //    if (fechaInicioDT <= item.Created && item.Created <= fechaFinDT)
            //    {
            //        if (item.Header.CreditTypeId != 11)
            //        {
            //            ws.Cell("A" + cont).Value = item.Header.Customers.Name;
            //            ws.Cell("B" + cont).Value = item.Installment;
            //            ws.Cell("C" + cont).Value = item.PaidCapital;
            //            ws.Cell("D" + cont).Value = item.PaidInterest;
            //            ws.Cell("E" + cont).Value = item.InstallmentPaid;
            //            ws.Cell("F" + cont).Value = item.TotalDebt;
            //            ws.Cell("G" + cont).Value = item.InstallmentDate;
            //            ws.Cell("H" + cont).Value = item.InterestAmount;
            //            if (item.PaymentStatus == true)
            //            {
            //                if (item.PaymentModality == 1)
            //                {
            //                    ws.Cell("I" + cont).Value = "Dep�sito";
            //                }
            //                else if (item.PaymentModality == 0)
            //                {
            //                    ws.Cell("H" + cont).Value = "Efectivo";
            //                }
            //                else
            //                {
            //                    ws.Cell("H" + cont).Value = "Reprogramado";
            //                }
            //            }
            //            else
            //            {
            //                ws.Cell("I" + cont).Value = "A�n no pagado";
            //            }


            //            if (item.Header.CreditTypeId == 1)
            //            {
            //                ws.Cell("J" + cont).Value = "Diario";
            //            }
            //            else if (item.Header.CreditTypeId == 2)
            //            {
            //                ws.Cell("J" + cont).Value = "Semanal";
            //            }
            //            else if (item.Header.CreditTypeId == 3)
            //            {
            //                ws.Cell("J" + cont).Value = "Mensual";
            //            }
            //            else if (item.Header.CreditTypeId == 4)
            //            {
            //                ws.Cell("J" + cont).Value = "Paralelo";
            //            }

            //            ws.Cell("K" + cont).Value = item.CapitalInstallment;

            //            if (item.PaymentDate == null)
            //            {
            //                ws.Cell("L" + cont).Value = "A�n no pagado";
            //            }
            //            else
            //            {
            //                ws.Cell("L" + cont).Value = item.PaymentDate;
            //            }


            //            if (item.PaymentStatus == true)
            //            {
            //                ws.Cell("M" + cont).Value = "Pagado";

            //                if (item.PaymentDate - item.InstallmentDate <= new TimeSpan(23, 59, 59))
            //                {
            //                    ws.Cell("N" + cont).Value = "A tiempo";
            //                }

            //                else if (item.PaymentDate - item.InstallmentDate > new TimeSpan(23, 59, 59))
            //                {
            //                    ws.Cell("N" + cont).Value = "Demorado";
            //                    demoras++;
            //                }

            //            }
            //            else
            //            {
            //                ws.Cell("M" + cont).Value = "No pagado";

            //                if (item.InstallmentDate - DateTime.Now <= new TimeSpan(0, 0, 0))
            //                {
            //                    ws.Cell("N" + cont).Value = "En mora";
            //                    mora++;
            //                }

            //                else if (((item.InstallmentDate - DateTime.Now) > new TimeSpan(0, 0, 0)) && ((item.InstallmentDate - DateTime.Now) <= new TimeSpan(23, 59, 59)))
            //                {
            //                    ws.Cell("N" + cont).Value = "Debe pagar";
            //                }

            //                if (item.InstallmentDate - DateTime.Now >= new TimeSpan(23, 59, 59))
            //                {
            //                    ws.Cell("N" + cont).Value = "Cuota a�n no vencida";
            //                }
            //            }
            //            cont++;
            //        }
            //    }
            //}

            //cont = cont + 2;

            //ws.Cell("A" + cont).Value = "Ahorros";
            //cont++;
            //ws.Range("A" + cont, "G" + cont).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            //ws.Range("A" + cont, "G" + cont).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            //ws.Range("A" + cont, "G" + cont).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            //ws.Range("A" + cont, "G" + cont).Style.Font.SetFontColor(XLColor.White);

            //ws.Cell("A" + cont).Value = "Cliente";
            //ws.Cell("B" + cont).Value = "Movimiento";
            //ws.Cell("C" + cont).Value = "Modalidad";
            //ws.Cell("D" + cont).Value = "Monto de movimiento";
            //ws.Cell("E" + cont).Value = "Fecha de movimiento";
            //ws.Cell("F" + cont).Value = "Estado pre-movimiento";
            //ws.Cell("G" + cont).Value = "Estado post-movimiento";
            //cont++;

            //foreach (var item in list)
            //{
            //    if (fechaInicioDT <= item.Created && item.Created <= fechaFinDT)
            //    {
            //        if (item.Header.CreditTypeId == 11)
            //        {
            //            ws.Cell("A" + cont).Value = item.Header.Customers.Name;

            //            if (item.OperationType == 0)
            //            {
            //                ws.Cell("B" + cont).Value = "Efectivo";
            //            }
            //            else
            //            {
            //                ws.Cell("B" + cont).Value = "Dep�sito";
            //            }

            //            if (item.PaymentModality == 0)
            //            {
            //                ws.Cell("C" + cont).Value = "Efectivo";
            //            }
            //            else
            //            {
            //                ws.Cell("C" + cont).Value = "Dep�sito";
            //            }

            //            if (item.OperationType == 1)
            //            {
            //                ws.Cell("D" + cont).Value = item.DepositAmount;
            //            }
            //            else
            //            {
            //                ws.Cell("D" + cont).Value = item.WithdrawalAmount;
            //            }

            //            ws.Cell("E" + cont).Value = item.Created;
            //            ws.Cell("F" + cont).Value = item.Balance;
            //            ws.Cell("G" + cont).Value = item.RemainingCapital;
            //            cont++;
            //        }
            //    }
            //}



            ws.Columns("A", "T").AdjustToContents();
            return wb.Deliver("Estado_cuenta.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

    }
}