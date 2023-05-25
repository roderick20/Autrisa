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
using DocumentFormat.OpenXml.Presentation;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.CodeAnalysis;

namespace Autrisa.Controllers
{
    public class productoCliente
    {
        public int value { get; set; }
        public string text { get; set; } = null!;
        public int? type { get; set; }
    }

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
                .ThenInclude(m => m.AccountDetails)
                .ToListAsync();

            ViewBag.operations = operations;
            return View(operations);
        }

        public async Task<IActionResult> LendingDetails()
        {
            var operations = await _context.Operations
                .Include(m => m.Account)
                .ThenInclude(m => m.AccountDetails)
                .Where(m => m.OperationType == 2 && m.FatherOperation == 1)
                .ToListAsync();

            ViewBag.operations = operations;
            return View(operations);
        }

        public async Task<IActionResult> InvestmentDetails()
        {
            var operations = await _context.Operations
                .Include(m => m.Account)
                .ThenInclude(m => m.AccountDetails)
                .Where(m => m.OperationType == 3 && m.FatherOperation == 1)
                .ToListAsync();

            ViewBag.operations = operations;
            return View(operations);
        }

        public async Task<IActionResult> PropertyDetails()
        {
            var operations = await _context.Operations
                .Include(m => m.Account)
                .ThenInclude(m => m.AccountDetails)
                .Where(m => m.OperationType == 4 && m.FatherOperation == 1)
                .ToListAsync();

            ViewBag.operations = operations;
            return View(operations);
        }

        public async Task<IActionResult> LendingOperations(int Id)
        {
            var lendingop = await _context.LendingOperations
                .Include(m => m.Operation)
                .ThenInclude(m => m.Account)
                .Where(m => m.OperationId == Id)
                .ToListAsync();
            if (lendingop == null)
            {
                return NotFound();
            }

            ViewBag.LendingId = Id;
            return View(lendingop);
        }

        public async Task<IActionResult> InvestmentOperations(int Id)
        {
            var lendingop = await _context.InvestmentsOperations
                .Include(m => m.Operation)
                .ThenInclude(m => m.Account)
                .Where(m => m.OperationId == Id)
                .ToListAsync();
            if (lendingop == null)
            {
                return NotFound();
            }
            ViewBag.InvestmentId = Id;
            return View(lendingop);
        }

        public async Task<IActionResult> PropertyOperations(int Id)
        {
            var lendingop = await _context.PropertiesOperations
                .Include(m => m.Operation)
                .ThenInclude(m => m.Account)
                .Where(m => m.OperationId == Id)
                .ToListAsync();
            if (lendingop == null)
            {
                return NotFound();
            }
            ViewBag.PropertyId = Id;
            return View(lendingop);
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

        public async Task<IActionResult> Create(int LendingId, int InvestmentId, int PropertyId)
        {
            var accounts = await _context.Accounts.ToListAsync();
            var clients = await _context.Clients.ToListAsync();
            var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name");
            ViewBag.ClientsJson = clientsJson;
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            ViewBag.LendingId = LendingId;
            ViewBag.InvestmentId = InvestmentId;
            ViewBag.PropertyId = PropertyId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Operation operation, int montoTransaccion, string Created, int AccountOper, int OperationType,
            string customer, string operDate, int LendingId, int InvestmentId, int PropertyId, string Receptor)
        {
            try
            {
                var accountEdit = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == operation.AccountId);
                var check = await _context.Operations.Include(m => m.Account).Where(m => m.Account.BankId == accountEdit.BankId &&
                m.Number == operation.Number).FirstOrDefaultAsync();
                var existentClient = await _context.Clients.Where(m => m.Name == customer).FirstOrDefaultAsync();

                if (check == null)
                {
                    if (existentClient == null)
                    {
                        Client client = new()
                        {
                            Name = customer
                        };
                    }

                    AccountDetail accdetail = new AccountDetail();

                    operation.UniqueId = Guid.NewGuid();
                    operation.Created = DateTime.Now;
                    operation.Author = (int)HttpContext.Session.GetInt32("UserId");
                    DateTime selectedDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    operation.Year = selectedDate.Year;
                    operation.Month = selectedDate.Month;
                    operation.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    operation.OperationType = OperationType;

                    if (operation.Number == null)
                    {
                        operation.Number = "0";
                    }

                    //if (operation.InitialBalance != null)
                    //{
                    //    operation.InitialBalance = operation.InitialBalance;
                    //}
                    //else
                    //{
                    //    if (operation.Type == 0)
                    //    {
                    //        operation.InitialBalance = montoTransaccion;
                    //    }
                    //    else
                    //    {
                    //        operation.InitialBalance = -montoTransaccion;
                    //    }
                    //}

                    var montoInicial = accountEdit.Amount;
                    if (OperationType != 1)
                    {
                        if (operation.Type == 0)
                        {
                            if (montoTransaccion != operation.InitialBalance && operation.InitialBalance != null)
                            {
                                accountEdit.Amount = montoInicial + (decimal)operation.InitialBalance;
                                operation.Income = montoTransaccion;
                                operation.Outcome = 0;
                            }
                            else
                            {
                                accountEdit.Amount = montoInicial + montoTransaccion;
                                operation.Income = montoTransaccion;
                                operation.Outcome = 0;
                            }
                        }
                        else if (operation.Type == 1)
                        {
                            if (montoTransaccion != operation.InitialBalance && operation.InitialBalance != null)
                            {
                                accountEdit.Amount = montoInicial - (decimal)operation.InitialBalance;
                                operation.Outcome = montoTransaccion;
                                operation.Income = 0;
                            }
                            else
                            {
                                accountEdit.Amount = montoInicial - montoTransaccion;
                                operation.Outcome = montoTransaccion;
                                operation.Income = 0;
                            }
                        }
                    }
                    else
                    {
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
                    }

                    if (AccountOper == 0)
                    {
                        accdetail.AccountId = operation.AccountId;
                    }
                    else
                    {
                        accdetail.AccountId = AccountOper;
                    }
                    accdetail.UniqueId = Guid.NewGuid();
                    accdetail.Description = operation.Description;
                    accdetail.Concept = operation.Concept;
                    accdetail.Author = operation.Author;
                    accdetail.Created = DateTime.Now;
                    if (accountEdit.Currency == 0)
                    {
                        accdetail.SolesAmount = montoTransaccion;
                    }
                    else
                    {
                        accdetail.DollarsAmount = montoTransaccion;
                    }

                    accdetail.InitialAmount = accountEdit.Amount;
                    accdetail.Customer = customer;
                    accdetail.OperationType = OperationType;
                    accdetail.OperationDate = DateTime.Now;
                    _context.Update(accountEdit);

                    if (OperationType != 1)
                    {
                        operation.FatherOperation = 1;
                    }

                    _context.Add(operation);
                    await _context.SaveChangesAsync();

                    if (OperationType == 2)
                    {
                        LendingOperation lendingOp = new();
                        lendingOp.UniqueId = Guid.NewGuid();
                        lendingOp.Type = operation.Type;
                        lendingOp.Modality = operation.Modality;
                        lendingOp.OperationId = operation.Id;
                        lendingOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        lendingOp.Description = operation.Description;
                        lendingOp.Amount = montoTransaccion;
                        lendingOp.Created = DateTime.Now;
                        lendingOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                        _context.Add(lendingOp);
                    }
                    else if (OperationType == 3)
                    {
                        InvestmentsOperation investmentOp = new();
                        investmentOp.UniqueId = Guid.NewGuid();
                        investmentOp.Type = operation.Type;
                        investmentOp.Modality = operation.Modality;
                        investmentOp.OperationId = operation.Id;
                        investmentOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        investmentOp.Description = operation.Description;
                        investmentOp.Amount = montoTransaccion;
                        investmentOp.Created = DateTime.Now;
                        investmentOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                        _context.Add(investmentOp);
                    }
                    else if (OperationType == 4)
                    {
                        PropertiesOperation propertiesOp = new();
                        propertiesOp.UniqueId = Guid.NewGuid();
                        propertiesOp.Type = operation.Type;
                        propertiesOp.Modality = operation.Modality;
                        propertiesOp.OperationId = operation.Id;
                        propertiesOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        propertiesOp.Description = operation.Description;
                        propertiesOp.Amount = montoTransaccion;
                        propertiesOp.Created = DateTime.Now;
                        if (Receptor != null)
                        {
                            propertiesOp.Receptor = Receptor;
                        }
                        else
                        {
                            propertiesOp.Receptor = "-";
                        }
                        propertiesOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                        _context.Add(propertiesOp);
                    }
                    accdetail.OperationId = operation.Id;
                    _context.Add(accdetail);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Agregado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Check = "Documento ya existente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                TempData["Check"] = "Documento ya existente";
            }
            ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "Name", operation.AccountId);
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            return View(operation);
        }

        public async Task<IActionResult> CreateDetail(int LendingId, int InvestmentId, int PropertyId)
        {
            var operation = await _context.Operations.Include(m => m.Account)
                .FirstOrDefaultAsync(m => m.Id == LendingId || m.Id == InvestmentId || m.Id == PropertyId);
            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == operation.AccountId);
            //var accdetail = await _context.AccountDetails.FirstOrDefaultAsync(m => m.AccountId == account.Id);
            //var clients = await _context.Clients.ToListAsync();
            //var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.Operation = operation;
            ViewBag.AccountId = account;
            //ViewBag.AccountDetail = accdetail;
            //ViewBag.ClientsJson = clientsJson;
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            ViewBag.LendingId = LendingId;
            ViewBag.InvestmentId = InvestmentId;
            ViewBag.PropertyId = PropertyId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDetail(Operation operation, int montoTransaccion, string Created, int AccountOper, int OperationType,
            string customer, string operDate, int LendingId, int InvestmentId, int PropertyId, string Receptor)
        {
            try
            {
                var accountEdit = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == operation.AccountId);
                var check = await _context.Operations.Include(m => m.Account).Where(m => m.Account.BankId == accountEdit.BankId &&
                m.Number == operation.Number).FirstOrDefaultAsync();
                var existentClient = await _context.Clients.Where(m => m.Name == customer).FirstOrDefaultAsync();
                var specialOp = 0;
                if (LendingId != 0)
                {
                    specialOp = 1;
                }
                else if (InvestmentId != 0)
                {
                    specialOp = 2;
                }
                else if (InvestmentId != 0)
                {
                    specialOp = 3;
                }

                if (check == null)
                {
                    if (existentClient == null)
                    {
                        Client client = new()
                        {
                            Name = customer
                        };
                    }

                    AccountDetail accdetail = new AccountDetail();

                    //string operdate = Convert.ToString(operation.OperationDate);
                    operation.UniqueId = Guid.NewGuid();
                    //operation.Created = DateTime.Now;
                    operation.Created = DateTime.Now;
                    operation.Author = (int)HttpContext.Session.GetInt32("UserId");
                    DateTime selectedDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    operation.Year = selectedDate.Year;
                    operation.Month = selectedDate.Month;
                    operation.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    operation.OperationType = OperationType;
                    if (operation.Number == null)
                    {
                        operation.Number = "0";
                    }
                    if (specialOp == 1)
                    {
                        var oper = await _context.Operations.FirstOrDefaultAsync(m => m.Id == LendingId);
                        if (operation.Type == 0)
                        {
                            if (oper.InitialBalance == null)
                            {
                                oper.ActualBalance = montoTransaccion;
                            }
                            else
                            {
                                oper.ActualBalance = oper.InitialBalance + montoTransaccion;
                            }
                        }
                        else
                        {
                            if (oper.InitialBalance == null)
                            {
                                oper.ActualBalance = 0 - montoTransaccion;
                            }
                            else
                            {
                                oper.ActualBalance = oper.InitialBalance - montoTransaccion;
                            }
                        }
                        _context.Update(oper);
                    }

                    if (specialOp == 2)
                    {
                        var oper = await _context.Operations.FirstOrDefaultAsync(m => m.Id == InvestmentId);
                        if (operation.Type == 0)
                        {
                            if (oper.InitialBalance == null)
                            {
                                oper.ActualBalance = montoTransaccion;
                            }
                            else
                            {
                                oper.ActualBalance = oper.InitialBalance + montoTransaccion;
                            }
                        }
                        else
                        {
                            if (oper.InitialBalance == null)
                            {
                                oper.ActualBalance = 0 - montoTransaccion;
                            }
                            else
                            {
                                oper.ActualBalance = oper.InitialBalance - montoTransaccion;
                            }
                        }
                        _context.Update(oper);
                    }

                    if (specialOp == 3)
                    {
                        var oper = await _context.Operations.FirstOrDefaultAsync(m => m.Id == PropertyId);
                        if (operation.Type == 0)
                        {
                            if (oper.InitialBalance == null)
                            {
                                oper.ActualBalance = montoTransaccion;
                            }
                            else
                            {
                                oper.ActualBalance = oper.InitialBalance + montoTransaccion;
                            }
                        }
                        else
                        {
                            if (oper.InitialBalance == null)
                            {
                                oper.ActualBalance = 0 - montoTransaccion;
                            }
                            else
                            {
                                oper.ActualBalance = oper.InitialBalance - montoTransaccion;
                            }
                        }
                        _context.Update(oper);
                    }

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

                    if (AccountOper == 0)
                    {
                        accdetail.AccountId = operation.AccountId;
                    }
                    else
                    {
                        accdetail.AccountId = AccountOper;
                    }
                    accdetail.UniqueId = Guid.NewGuid();
                    accdetail.Description = operation.Description;
                    accdetail.Concept = operation.Concept;
                    accdetail.Author = operation.Author;
                    accdetail.Created = DateTime.Now;
                    if (accountEdit.Currency == 0)
                    {
                        accdetail.SolesAmount = montoTransaccion;
                    }
                    else
                    {
                        accdetail.DollarsAmount = montoTransaccion;
                    }
                    accdetail.InitialAmount = accountEdit.Amount;
                    accdetail.Customer = customer;
                    accdetail.OperationType = OperationType;
                    accdetail.OperationDate = DateTime.Now;


                    _context.Update(accountEdit);
                    _context.Add(operation);
                    await _context.SaveChangesAsync();

                    accdetail.OperationId = operation.Id;
                    _context.Add(accdetail);

                    if (OperationType == 2)
                    {
                        LendingOperation lendingOp = new();
                        lendingOp.UniqueId = Guid.NewGuid();
                        lendingOp.Amount = montoTransaccion;
                        lendingOp.Type = operation.Type;
                        lendingOp.Modality = operation.Modality;
                        lendingOp.OperationId = LendingId;
                        lendingOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        lendingOp.Description = operation.Description;
                        lendingOp.Created = DateTime.Now;
                        operation.Author = (int)HttpContext.Session.GetInt32("UserId");
                        _context.Add(lendingOp);
                    }
                    else if (OperationType == 3)
                    {
                        InvestmentsOperation investmentOp = new();
                        investmentOp.UniqueId = Guid.NewGuid();
                        investmentOp.Amount = montoTransaccion;
                        investmentOp.Type = operation.Type;
                        investmentOp.Modality = operation.Modality;
                        investmentOp.OperationId = InvestmentId;
                        investmentOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        investmentOp.Description = operation.Description;
                        investmentOp.Created = DateTime.Now;
                        operation.Author = (int)HttpContext.Session.GetInt32("UserId");
                        _context.Add(investmentOp);
                    }
                    else if (OperationType == 4)
                    {
                        PropertiesOperation propertiesOp = new();
                        propertiesOp.UniqueId = Guid.NewGuid();
                        propertiesOp.Amount = montoTransaccion;
                        propertiesOp.Type = operation.Type;
                        propertiesOp.Modality = operation.Modality;
                        propertiesOp.OperationId = PropertyId;
                        propertiesOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        propertiesOp.Description = operation.Description;
                        propertiesOp.Created = DateTime.Now;
                        if (Receptor != null)
                        {
                            propertiesOp.Receptor = Receptor;
                        }
                        else
                        {
                            propertiesOp.Receptor = "-";
                        }
                        operation.Author = (int)HttpContext.Session.GetInt32("UserId");
                        _context.Add(propertiesOp);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Agregado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Check = "Documento ya existente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                TempData["Check"] = "Documento ya existente";
            }
            ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "Name", operation.AccountId);
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            return View(operation);
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
        public async Task<IActionResult> Edit(Operation operation, string Modified, string operDate, string Receptor)
        {
            try
            {
                var operationEdit = await _context.Operations.FirstOrDefaultAsync(m => m.UniqueId == operation.UniqueId);
                var accountEdit = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == operationEdit.AccountId);
                var accdetailEdit = await _context.AccountDetails.FirstOrDefaultAsync(m => m.OperationId == operationEdit.Id);
                var specialOpL = await _context.LendingOperations.FirstOrDefaultAsync(m => m.OperationId == operationEdit.Id);
                var specialOpI = await _context.InvestmentsOperations.FirstOrDefaultAsync(m => m.OperationId == operationEdit.Id);
                var specialOpP = await _context.PropertiesOperations.FirstOrDefaultAsync(m => m.OperationId == operationEdit.Id);
                var check = 0;

                // Modificar tanto Operations, como Account y AccountDetail
                //--------------------------------------------------------------------------------------------------------------------
                // Movimiento errado, mismo monto (Ingreso/Salida)

                //operations, type: 0:In, 1:Out

                if (operationEdit.Type == 0 && operation.Type == 1 && operationEdit.Income == operation.Income && check == 0)
                {
                    accountEdit.Amount = accountEdit.Amount - (2 * (decimal)operation.Income);
                    operationEdit.Outcome = operation.Income;
                    operationEdit.Income = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Outcome;
                    if (specialOpL != null && specialOpI == null && specialOpP == null)
                    {
                        specialOpL.Amount = (decimal)operationEdit.Outcome;
                    }
                    else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    {
                        specialOpI.Amount = (decimal)operationEdit.Outcome;
                    }
                    else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    {
                        specialOpP.Amount = (decimal)operationEdit.Outcome;
                    }
                    check++;
                }
                else if (operationEdit.Type == 1 && operation.Type == 0 && operationEdit.Outcome == operation.Outcome && check == 0)
                {
                    accountEdit.Amount = accountEdit.Amount + (2 * (decimal)operation.Outcome);
                    operationEdit.Income = operation.Outcome;
                    operationEdit.Outcome = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Income;
                    if (specialOpL != null && specialOpI == null && specialOpP == null)
                    {
                        specialOpL.Amount = (decimal)operationEdit.Income;
                    }
                    else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    {
                        specialOpI.Amount = (decimal)operationEdit.Income;
                    }
                    else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    {
                        specialOpP.Amount = (decimal)operationEdit.Income;
                    }
                    check++;
                }

                // Movimiento errado, diferente monto (Ingreso, Salida)

                if (operationEdit.Type == 0 && operation.Type == 1 && operationEdit.Income != operation.Income && check == 0)// && operation.Outcome != null)
                {
                    accountEdit.Amount = accountEdit.Amount + (decimal)operationEdit.Income - (decimal)operation.Income;
                    operationEdit.Outcome = operation.Income;
                    operationEdit.Income = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Outcome;
                    if (specialOpL != null && specialOpI == null && specialOpP == null)
                    {
                        specialOpL.Amount = (decimal)operationEdit.Outcome;
                    }
                    else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    {
                        specialOpI.Amount = (decimal)operationEdit.Outcome;
                    }
                    else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    {
                        specialOpP.Amount = (decimal)operationEdit.Outcome;
                    }
                    check++;
                }
                else if (operationEdit.Type == 1 && operation.Type == 0 && operationEdit.Income != operation.Outcome && check == 0)// && operation.Income != null)
                {
                    accountEdit.Amount = accountEdit.Amount - (decimal)operationEdit.Outcome + (decimal)operation.Outcome;
                    operationEdit.Income = operation.Outcome;
                    operationEdit.Outcome = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Income;
                    if (specialOpL != null && specialOpI == null && specialOpP == null)
                    {
                        specialOpL.Amount = (decimal)operationEdit.Income;
                    }
                    else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    {
                        specialOpI.Amount = (decimal)operationEdit.Income;
                    }
                    else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    {
                        specialOpP.Amount = (decimal)operationEdit.Income;
                    }
                    check++;
                }

                // Movimiento correcto, diferente monto

                if (operationEdit.Type == operation.Type && operation.Type == 1 && operationEdit.Outcome != operation.Outcome && check == 0)
                {
                    accountEdit.Amount = accountEdit.Amount + (decimal)operationEdit.Outcome - (decimal)operation.Outcome;
                    operationEdit.Outcome = operation.Outcome;
                    operationEdit.Income = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Outcome;
                    if (specialOpL != null && specialOpI == null && specialOpP == null)
                    {
                        specialOpL.Amount = (decimal)operationEdit.Outcome;
                    }
                    else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    {
                        specialOpI.Amount = (decimal)operationEdit.Outcome;
                    }
                    else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    {
                        specialOpP.Amount = (decimal)operationEdit.Outcome;
                    }
                    check++;
                }
                else if (operationEdit.Type == operation.Type && operation.Type == 0 && operationEdit.Income != operation.Income && check == 0)
                {
                    accountEdit.Amount = accountEdit.Amount - (decimal)operationEdit.Income + (decimal)operation.Income;
                    operationEdit.Income = operation.Income;
                    operationEdit.Outcome = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Income;
                    if (specialOpL != null && specialOpI == null && specialOpP == null)
                    {
                        specialOpL.Amount = (decimal)operationEdit.Income;
                    }
                    else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    {
                        specialOpI.Amount = (decimal)operationEdit.Income;
                    }
                    else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    {
                        specialOpP.Amount = (decimal)operationEdit.Income;
                    }
                    check++;
                }

                // Movimiento correcto, monto correcto, otro cambio

                operationEdit.Type = operation.Type;
                operationEdit.Modality = operation.Modality;
                operationEdit.Number = operation.Number;
                operationEdit.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //operationEdit.OperationDate = operation.OperationDate;
                operationEdit.Concept = operation.Concept;
                operationEdit.Description = operation.Description;
                DateTime selectedDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                operationEdit.Year = selectedDate.Year;
                operationEdit.Month = selectedDate.Month;
                //operationEdit.Modified = DateTime.ParseExact(Modified, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                operationEdit.Modified = DateTime.Now;
                operationEdit.Editor = (int)HttpContext.Session.GetInt32("UserId");

                if (specialOpL != null && specialOpI == null && specialOpP == null)
                {
                    specialOpL.Type = operation.Type;
                    specialOpL.Modality = operation.Modality;
                    specialOpL.Description = operation.Description;
                    specialOpL.Editor = (int)HttpContext.Session.GetInt32("UserId");
                    specialOpL.Modified = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else if (specialOpL == null && specialOpI != null && specialOpP == null)
                {
                    specialOpI.Type = operation.Type;
                    specialOpI.Modality = operation.Modality;
                    specialOpI.Description = operation.Description;
                    specialOpI.Editor = (int)HttpContext.Session.GetInt32("UserId");
                    specialOpI.Modified = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else if (specialOpL == null && specialOpI == null && specialOpP != null)
                {
                    specialOpP.Type = operation.Type;
                    specialOpP.Modality = operation.Modality;
                    specialOpP.Description = operation.Description;
                    if (Receptor != null)
                    {
                        specialOpP.Receptor = Receptor;
                    }
                    specialOpP.Editor = (int)HttpContext.Session.GetInt32("UserId");
                    specialOpP.Modified = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                if (check == 0)
                {
                    operationEdit.Income = operation.Income;
                    operationEdit.Outcome = operation.Outcome;
                }

                _context.Update(operationEdit);
                _context.Update(accdetailEdit);
                _context.Update(accountEdit);
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
                var account = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == operation.AccountId);

                if (operation != null)
                {
                    _context.Operations.Remove(operation);
                }

                if (account != null)
                {
                    if (operation.Income != 0)
                    {
                        account.Amount = account.Amount - (decimal)operation.ActualBalance;
                    }
                    else
                    {
                        account.Amount = account.Amount + (decimal)operation.ActualBalance;
                    }
                }

                _context.Update(account);
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
                //var accountData = await _context.Accounts.Where(m => m.AccountType != "Inversión"
                //&& m.AccountType != "Préstamo" && m.AccountType != "Predios").ToListAsync();
                var accountData = await _context.Accounts.ToListAsync();
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
                    closhure.Number = "0";
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

            var accounts = await _context.Accounts.Where(m => m.AccountType == "Corriente" || m.AccountType == "Ahorros" || m.AccountType == "Maestra")
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
            //ws.Cell("E" + cont).Value = "Saldo mes anterior";
            ws.Cell("E" + cont).Value = "Movimiento";
            ws.Cell("F" + cont).Value = "Concepto";
            ws.Cell("G" + cont).Value = "Descripción";
            //ws.Cell("H" + cont).Value = "Monto";
            ws.Cell("H" + cont).Value = "Modalidad";
            ws.Cell("I" + cont).Value = "Número";
            ws.Cell("J" + cont).Value = "Fecha";
            //ws.Cell("M" + cont).Value = "Monto";
            ws.Cell("K" + cont).Value = "Ingreso (S/)";
            ws.Cell("L" + cont).Value = "Salida (S/)";
            ws.Cell("M" + cont).Value = "Ingreso (USD)";
            ws.Cell("N" + cont).Value = "Salida (USD)";


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

                            //ws.Cell("E" + cont).Value = item.PreviousRemaining;

                            if (obj.Type == 0)
                            {
                                ws.Cell("E" + cont).Value = "Ingreso";
                                //ws.Cell("H" + cont).Value = obj.Income;
                            }
                            else if (obj.Type == 1)
                            {
                                ws.Cell("E" + cont).Value = "Egreso";
                                //ws.Cell("H" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("E" + cont).Value = "Cierre de mes";
                                //ws.Cell("H" + cont).Value = obj.Income - obj.Outcome;
                            }

                            ws.Cell("F" + cont).Value = obj.Concept;
                            ws.Cell("G" + cont).Value = obj.Description;

                            if (obj.Modality == 0)
                            {
                                ws.Cell("H" + cont).Value = "Cheque";

                            }
                            else if (obj.Modality == 1)
                            {
                                ws.Cell("H" + cont).Value = "Transferencia";
                            }
                            else if (obj.Modality == 100)
                            {
                                ws.Cell("H" + cont).Value = "Cierre de mes";
                            }
                            ws.Cell("I" + cont).Value = obj.Number;
                            ws.Cell("J" + cont).Value = obj.OperationDate;
                            if (item.Currency == 0 && obj.Type == 0)
                            {
                                ws.Cell("K" + cont).Value = obj.Income;
                            }
                            else
                            {
                                ws.Cell("K" + cont).Value = "-";
                            }

                            if (item.Currency == 0 && obj.Type == 1)
                            {
                                ws.Cell("L" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("L" + cont).Value = "-";
                            }

                            if (item.Currency == 1 && obj.Type == 0)
                            {
                                ws.Cell("M" + cont).Value = obj.Income;
                            }
                            else
                            {
                                ws.Cell("M" + cont).Value = "-";
                            }

                            if (item.Currency == 0 && obj.Type == 0)
                            {
                                ws.Cell("N" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("N" + cont).Value = "-";
                            }
                            cont++;
                        }
                    }

                    cont = cont + 3;
                    var cont_a = cont - 1;

                    ws.Range("A" + cont_a, "C" + cont_a).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Font.SetFontColor(XLColor.White);
                    foreach (var item in account)
                    {
                        ws.Cell("A" + cont_a).Value = "Cuenta";
                        ws.Cell("B" + cont_a).Value = "Saldo Previo";
                        ws.Cell("C" + cont_a).Value = "Saldo Actual";

                        ws.Cell("A" + cont).Value = item.Name;
                        ws.Cell("B" + cont).Value = item.PreviousRemaining;
                        ws.Cell("C" + cont).Value = item.Amount;
                        cont++;
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

                            //ws.Cell("E" + cont).Value = item.PreviousRemaining;

                            if (obj.Type == 0)
                            {
                                ws.Cell("E" + cont).Value = "Ingreso";
                                //ws.Cell("H" + cont).Value = obj.Income;
                            }
                            else if (obj.Type == 1)
                            {
                                ws.Cell("E" + cont).Value = "Egreso";
                                //ws.Cell("H" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("E" + cont).Value = "Cierre de mes";
                                //ws.Cell("H" + cont).Value = obj.Income - obj.Outcome;
                            }

                            ws.Cell("F" + cont).Value = obj.Concept;
                            ws.Cell("G" + cont).Value = obj.Description;

                            if (Modality == 0)
                            {
                                ws.Cell("H" + cont).Value = "Cheque";

                            }
                            else if (Modality == 1)
                            {
                                ws.Cell("H" + cont).Value = "Transferencia";
                            }
                            else if (Modality == 100)
                            {
                                ws.Cell("H" + cont).Value = "Cierre de mes";
                            }
                            ws.Cell("I" + cont).Value = obj.Number;
                            ws.Cell("J" + cont).Value = obj.OperationDate;
                            if (item.Currency == 0 && obj.Type == 0)
                            {
                                ws.Cell("K" + cont).Value = obj.Income;
                            }
                            else
                            {
                                ws.Cell("K" + cont).Value = "-";
                            }

                            if (item.Currency == 0 && obj.Type == 1)
                            {
                                ws.Cell("L" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("L" + cont).Value = "-";
                            }

                            if (item.Currency == 1 && obj.Type == 0)
                            {
                                ws.Cell("M" + cont).Value = obj.Income;
                            }
                            else
                            {
                                ws.Cell("M" + cont).Value = "-";
                            }

                            if (item.Currency == 1 && obj.Type == 1)
                            {
                                ws.Cell("N" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("N" + cont).Value = "-";
                            }
                            cont++;
                        }
                    }

                    cont = cont + 3;
                    var cont_a = cont - 1;

                    ws.Range("A" + cont_a, "C" + cont_a).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Font.SetFontColor(XLColor.White);
                    foreach (var item in account)
                    {
                        ws.Cell("A" + cont_a).Value = "Cuenta";
                        ws.Cell("B" + cont_a).Value = "Saldo Previo";
                        ws.Cell("C" + cont_a).Value = "Saldo Actual";

                        ws.Cell("A" + cont).Value = item.Name;
                        ws.Cell("B" + cont).Value = item.PreviousRemaining;
                        ws.Cell("C" + cont).Value = item.Amount;
                        cont++;
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

                            //ws.Cell("E" + cont).Value = item.PreviousRemaining;

                            if (obj.Type == 0)
                            {
                                ws.Cell("E" + cont).Value = "Ingreso";
                                //ws.Cell("H" + cont).Value = obj.Income;
                            }
                            else if (obj.Type == 1)
                            {
                                ws.Cell("E" + cont).Value = "Egreso";
                                //ws.Cell("H" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("E" + cont).Value = "Cierre de mes";
                                //ws.Cell("H" + cont).Value = obj.Income - obj.Outcome;
                            }

                            ws.Cell("F" + cont).Value = obj.Concept;
                            ws.Cell("G" + cont).Value = obj.Description;

                            if (obj.Modality == 0)
                            {
                                ws.Cell("H" + cont).Value = "Cheque";

                            }
                            else if (obj.Modality == 1)
                            {
                                ws.Cell("H" + cont).Value = "Transferencia";
                            }
                            else if (obj.Modality == 100)
                            {
                                ws.Cell("H" + cont).Value = "Cierre de mes";
                            }
                            ws.Cell("I" + cont).Value = obj.Number;
                            ws.Cell("J" + cont).Value = obj.OperationDate;
                            if (item.Currency == 0 && obj.Type == 0)
                            {
                                ws.Cell("K" + cont).Value = obj.Income;
                            }
                            else
                            {
                                ws.Cell("K" + cont).Value = "-";
                            }

                            if (item.Currency == 0 && obj.Type == 1)
                            {
                                ws.Cell("L" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("L" + cont).Value = "-";
                            }

                            if (item.Currency == 1 && obj.Type == 0)
                            {
                                ws.Cell("M" + cont).Value = obj.Income;
                            }
                            else
                            {
                                ws.Cell("M" + cont).Value = "-";
                            }

                            if (item.Currency == 1 && obj.Type == 1)
                            {
                                ws.Cell("N" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("N" + cont).Value = "-";
                            }
                            cont++;
                        }
                    }

                    cont = cont + 3;
                    var cont_a = cont - 1;

                    ws.Range("A" + cont_a, "C" + cont_a).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Font.SetFontColor(XLColor.White);
                    foreach (var item in account)
                    {
                        ws.Cell("A" + cont_a).Value = "Cuenta";
                        ws.Cell("B" + cont_a).Value = "Saldo Previo";
                        ws.Cell("C" + cont_a).Value = "Saldo Actual";

                        ws.Cell("A" + cont).Value = item.Name;
                        ws.Cell("B" + cont).Value = item.PreviousRemaining;
                        ws.Cell("C" + cont).Value = item.Amount;
                        cont++;
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

                            //ws.Cell("E" + cont).Value = item.PreviousRemaining;

                            if (obj.Type == 0)
                            {
                                ws.Cell("E" + cont).Value = "Ingreso";
                                //ws.Cell("H" + cont).Value = obj.Income;
                            }
                            else if (obj.Type == 1)
                            {
                                ws.Cell("E" + cont).Value = "Egreso";
                                //ws.Cell("H" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("E" + cont).Value = "Cierre de mes";
                                //ws.Cell("H" + cont).Value = obj.Income - obj.Outcome;
                            }

                            ws.Cell("F" + cont).Value = obj.Concept;
                            ws.Cell("G" + cont).Value = obj.Description;

                            if (Modality == 0)
                            {
                                ws.Cell("H" + cont).Value = "Cheque";

                            }
                            else if (Modality == 1)
                            {
                                ws.Cell("H" + cont).Value = "Transferencia";
                            }
                            else if (Modality == 100)
                            {
                                ws.Cell("H" + cont).Value = "Cierre de mes";
                            }
                            ws.Cell("I" + cont).Value = obj.Number;
                            ws.Cell("J" + cont).Value = obj.OperationDate;
                            if (item.Currency == 0 && obj.Type == 0)
                            {
                                ws.Cell("K" + cont).Value = obj.Income;
                            }
                            else
                            {
                                ws.Cell("K" + cont).Value = "-";
                            }

                            if (item.Currency == 0 && obj.Type == 1)
                            {
                                ws.Cell("L" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("L" + cont).Value = "-";
                            }

                            if (item.Currency == 1 && obj.Type == 0)
                            {
                                ws.Cell("M" + cont).Value = obj.Income;
                            }
                            else
                            {
                                ws.Cell("M" + cont).Value = "-";
                            }

                            if (item.Currency == 1 && obj.Type == 1)
                            {
                                ws.Cell("N" + cont).Value = obj.Outcome;
                            }
                            else
                            {
                                ws.Cell("N" + cont).Value = "-";
                            }
                            cont++;
                        }
                    }

                    cont = cont + 3;
                    var cont_a = cont - 1;

                    ws.Range("A" + cont_a, "C" + cont_a).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
                    ws.Range("A" + cont_a, "C" + cont_a).Style.Font.SetFontColor(XLColor.White);
                    foreach (var item in account)
                    {
                        ws.Cell("A" + cont_a).Value = "Cuenta";
                        ws.Cell("B" + cont_a).Value = "Saldo Previo";
                        ws.Cell("C" + cont_a).Value = "Saldo Actual";

                        ws.Cell("A" + cont).Value = item.Name;
                        ws.Cell("B" + cont).Value = item.PreviousRemaining;
                        ws.Cell("C" + cont).Value = item.Amount;
                        cont++;
                    }
                }
            }

            ws.Columns("A", "T").AdjustToContents();
            return wb.Deliver("Estado_cuenta.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpPost]
        public ActionResult Elementos(int id)
        {
            var elementos = _context.Accounts.Where(m => m.BankId == id)
                    .Select(m => new productoCliente
                    {
                        value = m.Id,
                        type = m.OperationType,
                        text = m.AccountNumber,
                    })
                    .GroupBy(p => p.type)
                    .Select(g => g.First())
                    .ToList();
            return Json(elementos);
        }

        [HttpPost]
        public ActionResult CuentaOperacion(int id, int bank)
        {
            var elementos = _context.Accounts.Where(m => m.BankId == bank && m.OperationType == id).Distinct()
                    .Select(m => new productoCliente
                    {
                        value = m.Id,
                        type = m.OperationType,
                        text = m.AccountNumber,
                    }).Distinct().ToList();
            return Json(elementos);
        }
    }
}