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
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Autrisa.Controllers
{
    public class productoCliente
    {
        public int value { get; set; }
        public string text { get; set; } = null!;
        public int? type { get; set; }
    }

    public class CustomerTotal
    {
        public string Customer { get; set; }
        public string Currency { get; set; }
        public string AccountName { get; set; }
        public decimal TotalSoles { get; set; }
        public decimal TotalDolares { get; set; }
    }

    class OperationComparer : IEqualityComparer<Operation>
    {
        public bool Equals(Operation x, Operation y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x is null || y is null)
                return false;

            return x.Customer == y.Customer && x.Account.Currency == y.Account.Currency;
        }

        public int GetHashCode(Operation obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            return obj.Customer.GetHashCode();
        }
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
            var operations = await _context.Operations.Where(m => m.Type == 0 || m.Type == 1)
                .Include(m => m.Account)
                //.ThenInclude(m => m.AccountDetails)
                .ToListAsync();

            ///ViewBag.operations = operations;
            return View(operations);
        }

        public async Task<IActionResult> Lendings()//Prestamos
        {
            List<CustomerTotal> list = new List<CustomerTotal>();
            var operations = await _context.Operations
                .Include(m => m.Account)
                .ThenInclude(m => m.AccountDetails)
                .Where(m => m.OperationType == 2 && m.FatherOperation == 1)
                .ToListAsync();

            var operationDistintas = operations.Distinct(new OperationComparer()).ToList();

            foreach (var operation in operationDistintas)
            {
                decimal total = (decimal)(operations.Where(m => m.Customer == operation.Customer && m.Account.Currency == operation.Account.Currency).Sum(m => m.Income) - operations.Where(m => m.Customer == operation.Customer && m.Account.Currency == operation.Account.Currency).Sum(m => m.Outcome));

                total = Math.Abs(total);

                list.Add(new CustomerTotal()
                {
                    Customer = operation.Customer,
                    AccountName = operation.Account.Name,
                    Currency = operation.Account.Currency == 0 ? "Soles" : "Dolares",
                    TotalSoles = operation.Account.Currency == 0 ? total : 0,
                    TotalDolares = operation.Account.Currency == 1 ? total : 0,
                });
            }
            ViewBag.operations = list;
            return View();
        }

        public async Task<IActionResult> Investments()
        {
            List<CustomerTotal> list = new List<CustomerTotal>();
            var operations = await _context.Operations
                .Include(m => m.Account)
                .ThenInclude(m => m.AccountDetails)
                .Where(m => m.OperationType == 3 && m.FatherOperation == 1)
                .ToListAsync();

            var operationDistintas = operations.Distinct(new OperationComparer()).ToList();

            foreach (var operation in operationDistintas)
            {
                decimal total = (decimal)(operations.Where(m => m.Customer == operation.Customer && m.Account.Currency == operation.Account.Currency).Sum(m => m.Income) - operations.Where(m => m.Customer == operation.Customer && m.Account.Currency == operation.Account.Currency).Sum(m => m.Outcome));

                total = Math.Abs(total);

                list.Add(new CustomerTotal()
                {
                    Customer = operation.Customer,
                    AccountName = operation.Account.Name,
                    Currency = operation.Account.Currency == 0 ? "Soles" : "Dolares",
                    TotalSoles = operation.Account.Currency == 0 ? total : 0,
                    TotalDolares = operation.Account.Currency == 1 ? total : 0,
                });
            }
            ViewBag.operations = list;
            return View();
        }

        public async Task<IActionResult> Propertys()
        {
            List<CustomerTotal> list = new List<CustomerTotal>();
            var operations = await _context.Operations
                .Include(m => m.Account)
                .ThenInclude(m => m.AccountDetails)
                .Where(m => m.OperationType == 4 && m.FatherOperation == 1)
                .ToListAsync();

            var operationDistintas = operations.Distinct(new OperationComparer()).ToList();

            foreach (var operation in operationDistintas)
            {
                decimal total = (decimal)(operations.Where(m => m.Customer == operation.Customer && m.Account.Currency == operation.Account.Currency).Sum(m => m.Income) - operations.Where(m => m.Customer == operation.Customer && m.Account.Currency == operation.Account.Currency).Sum(m => m.Outcome));

                total = Math.Abs(total);

                list.Add(new CustomerTotal()
                {
                    Customer = operation.Customer,
                    AccountName = operation.Account.Name,
                    Currency = operation.Account.Currency == 0 ? "Soles" : "Dolares",
                    TotalSoles = operation.Account.Currency == 0 ? total : 0,
                    TotalDolares = operation.Account.Currency == 1 ? total : 0,
                });
            }
            ViewBag.operations = list;
            return View();
        }

        public async Task<IActionResult> LendingDetails(String Customer, String Currency)//Prestamos
        {
            int CurrencyInt = Currency == "Soles" ? 0 : 1;
            var operations = await _context.Operations
                .Include(m => m.Account)
                .ThenInclude(m => m.AccountDetails)
                .Where(m => m.OperationType == 2 && m.FatherOperation == 1 && m.Customer == Customer && m.Account.Currency == CurrencyInt)
                .ToListAsync();

            ViewBag.operations = operations;
            return View(operations);
        }

        public async Task<IActionResult> InvestmentDetails(String Customer, String Currency)
        {
            int CurrencyInt = Currency == "Soles" ? 0 : 1;
            var operations = await _context.Operations
                .Include(m => m.Account)
                .ThenInclude(m => m.AccountDetails)
                .Where(m => m.OperationType == 3 && m.FatherOperation == 1 && m.Customer == Customer && m.Account.Currency == CurrencyInt)
                .ToListAsync();

            ViewBag.operations = operations;
            return View(operations);
        }

        public async Task<IActionResult> PropertyDetails(String Customer, String Currency)
        {
            int CurrencyInt = Currency == "Soles" ? 0 : 1;
            var operations = await _context.Operations
                .Include(m => m.Account)
                .ThenInclude(m => m.AccountDetails)
                .Where(m => m.OperationType == 4 && m.FatherOperation == 1 && m.Customer == Customer && m.Account.Currency == CurrencyInt)
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
            var accounts = await _context.Accounts.Where(m => m.Visible == 0).ToListAsync();
            var clients = await _context.Clients.Select(m => m.Name).ToListAsync();
            var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.ClientsJson = clientsJson;
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name");
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            ViewBag.LendingId = LendingId;
            ViewBag.InvestmentId = InvestmentId;
            ViewBag.PropertyId = PropertyId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Operation operation, decimal montoTransaccion, string Created, int AccountOper, int OperationType,
            string customer, string operDate, int LendingId, int InvestmentId, int PropertyId, string Receptor)
        {
            try
            {
                //var check = await _context.Operations.Include(m => m.Account)
                //    .Where(m => m.Account.Id == operation.AccountId && m.Number == operation.Number)
                //    .FirstOrDefaultAsync();

                var existentClient = await _context.Clients.Where(m => m.Name == customer).FirstOrDefaultAsync();

                //if (check == null)
                //{
                if (existentClient == null)
                {
                    Client client = new()
                    {
                        Name = customer
                    };
                    _context.Add(client);
                    await _context.SaveChangesAsync();
                }

                //AccountDetail accdetail = new AccountDetail();

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

                //var montoInicial = accountEdit.Amount;
                //if (OperationType != 1)
                //{
                //    if (operation.Type == 0)
                //    {
                //        if (montoTransaccion != operation.InitialBalance && operation.InitialBalance != null)
                //        {
                //            if (operation.InitialBalance < montoTransaccion)
                //            {
                //                //accountEdit.Amount = montoInicial + montoTransaccion;
                //                operation.Income = montoTransaccion;
                //                operation.Outcome = 0;
                //            }
                //            else
                //            {
                //                //accountEdit.Amount = montoInicial + (decimal)operation.InitialBalance;
                //                operation.Income = montoTransaccion;
                //                operation.Outcome = 0;
                //            }

                //        }
                //        else
                //        {
                //            //accountEdit.Amount = montoInicial + montoTransaccion;
                //            operation.Income = montoTransaccion;
                //            operation.Outcome = 0;
                //            operation.InitialBalance = montoTransaccion;
                //        }
                //    }
                //    else if (operation.Type == 1)
                //    {
                //        if (montoTransaccion != operation.InitialBalance && operation.InitialBalance != null)
                //        {
                //            //accountEdit.Amount = montoInicial - (decimal)operation.InitialBalance;
                //            operation.Outcome = montoTransaccion;
                //            operation.Income = 0;
                //        }
                //        else
                //        {
                //            //accountEdit.Amount = montoInicial - montoTransaccion;
                //            operation.Outcome = montoTransaccion;
                //            operation.Income = 0;
                //            operation.InitialBalance = -montoTransaccion;
                //        }
                //    }
                //}
                //else
                //{
                if (operation.Type == 0)
                {
                    //accountEdit.Amount = montoInicial + montoTransaccion;
                    operation.Income = montoTransaccion;
                    operation.Outcome = 0;
                }
                else if (operation.Type == 1 || operation.Type == 2)
                {
                    //accountEdit.Amount = montoInicial - montoTransaccion;
                    operation.Outcome = montoTransaccion;
                    operation.Income = 0;
                }
                //}

                //if (AccountOper == 0)
                //{
                //    accdetail.AccountId = operation.AccountId;
                //}
                //else
                //{
                //    accdetail.AccountId = AccountOper;
                //}
                //accdetail.UniqueId = Guid.NewGuid();
                //accdetail.Description = operation.Description;
                //accdetail.Concept = operation.Concept;
                //accdetail.Author = operation.Author;
                //accdetail.Created = DateTime.Now;
                //if (accountEdit.Currency == 0)
                //{
                //    accdetail.SolesAmount = montoTransaccion;
                //}
                //else
                //{
                //    accdetail.DollarsAmount = montoTransaccion;
                //}
                //accdetail.InitialAmount = accountEdit.Amount;
                //accdetail.Customer = customer;
                //accdetail.OperationType = OperationType;
                //accdetail.OperationDate = DateTime.Now;
                //_context.Update(accountEdit);

                if (OperationType != 1)
                {
                    operation.FatherOperation = 1;
                }

                _context.Add(operation);
                await _context.SaveChangesAsync();

                //if (OperationType == 2)
                //{
                //    LendingOperation lendingOp = new();
                //    lendingOp.UniqueId = Guid.NewGuid();
                //    lendingOp.Type = operation.Type;
                //    lendingOp.Modality = operation.Modality;
                //    lendingOp.OperationId = operation.Id;
                //    lendingOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    lendingOp.Description = operation.Description;
                //    lendingOp.Amount = montoTransaccion;
                //    lendingOp.Created = DateTime.Now;
                //    lendingOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                //    lendingOp.FinalAmount = operation.InitialBalance;
                //    _context.Add(lendingOp);
                //}
                //else if (OperationType == 3)
                //{
                //    InvestmentsOperation investmentOp = new();
                //    investmentOp.UniqueId = Guid.NewGuid();
                //    investmentOp.Type = operation.Type;
                //    investmentOp.Modality = operation.Modality;
                //    investmentOp.OperationId = operation.Id;
                //    investmentOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    investmentOp.Description = operation.Description;
                //    investmentOp.Amount = montoTransaccion;
                //    investmentOp.Created = DateTime.Now;
                //    investmentOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                //    investmentOp.FinalAmount = operation.InitialBalance;
                //    _context.Add(investmentOp);
                //}
                //else if (OperationType == 4)
                //{
                //    PropertiesOperation propertiesOp = new();
                //    propertiesOp.UniqueId = Guid.NewGuid();
                //    propertiesOp.Type = operation.Type;
                //    propertiesOp.Modality = operation.Modality;
                //    propertiesOp.OperationId = operation.Id;
                //    propertiesOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    propertiesOp.Description = operation.Description;
                //    propertiesOp.Amount = montoTransaccion;
                //    propertiesOp.Created = DateTime.Now;
                //    propertiesOp.FinalAmount = operation.InitialBalance;
                //    if (Receptor != null)
                //    {
                //        propertiesOp.Receptor = Receptor;
                //    }
                //    else
                //    {
                //        propertiesOp.Receptor = "-";
                //    }
                //    propertiesOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                //    _context.Add(propertiesOp);
                //}
                //accdetail.OperationId = operation.Id;
                //_context.Add(accdetail);
                //await _context.SaveChangesAsync();
                TempData["Success"] = "Agregado exitosamente";
                return RedirectToAction(nameof(Index));
                //}
                //ViewBag.Check = "Documento ya existente";

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                TempData["Check"] = "Documento ya existente";
            }

            var accounts = await _context.Accounts.ToListAsync();
            var clients = await _context.Clients.Select(m => m.Name).ToListAsync();
            var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.ClientsJson = clientsJson;
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name");
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            ViewBag.LendingId = LendingId;
            ViewBag.InvestmentId = InvestmentId;
            ViewBag.PropertyId = PropertyId;
            return View(operation);
        }

        //, , 

        public async Task<IActionResult> CreateProperty(int LendingId, int InvestmentId, int PropertyId)
        {
            var accounts = await _context.Accounts.ToListAsync();
            var clients = await _context.Clients.Select(m => m.Name).ToListAsync();
            var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.ClientsJson = clientsJson;
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name");
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            ViewBag.LendingId = LendingId;
            ViewBag.InvestmentId = InvestmentId;
            ViewBag.PropertyId = PropertyId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProperty(Operation operation, decimal montoTransaccion, string Created, int AccountOper, int OperationType,
            string customer, string operDate, int LendingId, int InvestmentId, int PropertyId, string Receptor)
        {
            try
            {
                var check = await _context.Operations.Include(m => m.Account)
                    .Where(m => m.Account.Id == operation.AccountId && m.Number == operation.Number)
                    .FirstOrDefaultAsync();

                var existentClient = await _context.Clients.Where(m => m.Name == customer).FirstOrDefaultAsync();

                if (check == null)
                {
                    if (existentClient == null)
                    {
                        Client client = new()
                        {
                            Name = customer
                        };
                        _context.Add(client);
                        await _context.SaveChangesAsync();
                    }

                    //AccountDetail accdetail = new AccountDetail();

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

                    //var montoInicial = accountEdit.Amount;
                    //if (OperationType != 1)
                    //{
                    //    if (operation.Type == 0)
                    //    {
                    //        if (montoTransaccion != operation.InitialBalance && operation.InitialBalance != null)
                    //        {
                    //            if (operation.InitialBalance < montoTransaccion)
                    //            {
                    //                //accountEdit.Amount = montoInicial + montoTransaccion;
                    //                operation.Income = montoTransaccion;
                    //                operation.Outcome = 0;
                    //            }
                    //            else
                    //            {
                    //                //accountEdit.Amount = montoInicial + (decimal)operation.InitialBalance;
                    //                operation.Income = montoTransaccion;
                    //                operation.Outcome = 0;
                    //            }

                    //        }
                    //        else
                    //        {
                    //            //accountEdit.Amount = montoInicial + montoTransaccion;
                    //            operation.Income = montoTransaccion;
                    //            operation.Outcome = 0;
                    //            operation.InitialBalance = montoTransaccion;
                    //        }
                    //    }
                    //    else if (operation.Type == 1)
                    //    {
                    //        if (montoTransaccion != operation.InitialBalance && operation.InitialBalance != null)
                    //        {
                    //            //accountEdit.Amount = montoInicial - (decimal)operation.InitialBalance;
                    //            operation.Outcome = montoTransaccion;
                    //            operation.Income = 0;
                    //        }
                    //        else
                    //        {
                    //            //accountEdit.Amount = montoInicial - montoTransaccion;
                    //            operation.Outcome = montoTransaccion;
                    //            operation.Income = 0;
                    //            operation.InitialBalance = -montoTransaccion;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    if (operation.Type == 0)
                    {
                        //accountEdit.Amount = montoInicial + montoTransaccion;
                        operation.Income = montoTransaccion;
                        operation.Outcome = 0;
                    }
                    else if (operation.Type == 1 || operation.Type == 2)
                    {
                        //accountEdit.Amount = montoInicial - montoTransaccion;
                        operation.Outcome = montoTransaccion;
                        operation.Income = 0;
                    }
                    //}

                    //if (AccountOper == 0)
                    //{
                    //    accdetail.AccountId = operation.AccountId;
                    //}
                    //else
                    //{
                    //    accdetail.AccountId = AccountOper;
                    //}
                    //accdetail.UniqueId = Guid.NewGuid();
                    //accdetail.Description = operation.Description;
                    //accdetail.Concept = operation.Concept;
                    //accdetail.Author = operation.Author;
                    //accdetail.Created = DateTime.Now;
                    //if (accountEdit.Currency == 0)
                    //{
                    //    accdetail.SolesAmount = montoTransaccion;
                    //}
                    //else
                    //{
                    //    accdetail.DollarsAmount = montoTransaccion;
                    //}
                    //accdetail.InitialAmount = accountEdit.Amount;
                    //accdetail.Customer = customer;
                    //accdetail.OperationType = OperationType;
                    //accdetail.OperationDate = DateTime.Now;
                    //_context.Update(accountEdit);

                    if (OperationType != 1)
                    {
                        operation.FatherOperation = 1;
                    }

                    _context.Add(operation);
                    await _context.SaveChangesAsync();

                    //if (OperationType == 2)
                    //{
                    //    LendingOperation lendingOp = new();
                    //    lendingOp.UniqueId = Guid.NewGuid();
                    //    lendingOp.Type = operation.Type;
                    //    lendingOp.Modality = operation.Modality;
                    //    lendingOp.OperationId = operation.Id;
                    //    lendingOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //    lendingOp.Description = operation.Description;
                    //    lendingOp.Amount = montoTransaccion;
                    //    lendingOp.Created = DateTime.Now;
                    //    lendingOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                    //    lendingOp.FinalAmount = operation.InitialBalance;
                    //    _context.Add(lendingOp);
                    //}
                    //else if (OperationType == 3)
                    //{
                    //    InvestmentsOperation investmentOp = new();
                    //    investmentOp.UniqueId = Guid.NewGuid();
                    //    investmentOp.Type = operation.Type;
                    //    investmentOp.Modality = operation.Modality;
                    //    investmentOp.OperationId = operation.Id;
                    //    investmentOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //    investmentOp.Description = operation.Description;
                    //    investmentOp.Amount = montoTransaccion;
                    //    investmentOp.Created = DateTime.Now;
                    //    investmentOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                    //    investmentOp.FinalAmount = operation.InitialBalance;
                    //    _context.Add(investmentOp);
                    //}
                    //else if (OperationType == 4)
                    //{
                    //    PropertiesOperation propertiesOp = new();
                    //    propertiesOp.UniqueId = Guid.NewGuid();
                    //    propertiesOp.Type = operation.Type;
                    //    propertiesOp.Modality = operation.Modality;
                    //    propertiesOp.OperationId = operation.Id;
                    //    propertiesOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //    propertiesOp.Description = operation.Description;
                    //    propertiesOp.Amount = montoTransaccion;
                    //    propertiesOp.Created = DateTime.Now;
                    //    propertiesOp.FinalAmount = operation.InitialBalance;
                    //    if (Receptor != null)
                    //    {
                    //        propertiesOp.Receptor = Receptor;
                    //    }
                    //    else
                    //    {
                    //        propertiesOp.Receptor = "-";
                    //    }
                    //    propertiesOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                    //    _context.Add(propertiesOp);
                    //}
                    //accdetail.OperationId = operation.Id;
                    //_context.Add(accdetail);
                    //await _context.SaveChangesAsync();
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

            var accounts = await _context.Accounts.ToListAsync();
            var clients = await _context.Clients.Select(m => m.Name).ToListAsync();
            var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.ClientsJson = clientsJson;
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name");
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            ViewBag.LendingId = LendingId;
            ViewBag.InvestmentId = InvestmentId;
            ViewBag.PropertyId = PropertyId;
            return View(operation);
        }


        public async Task<IActionResult> CreateInvestment(int LendingId, int InvestmentId, int PropertyId)
        {
            var accounts = await _context.Accounts.ToListAsync();
            var clients = await _context.Clients.Select(m => m.Name).ToListAsync();
            var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.ClientsJson = clientsJson;
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name");
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            ViewBag.LendingId = LendingId;
            ViewBag.InvestmentId = InvestmentId;
            ViewBag.PropertyId = PropertyId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvestment(Operation operation, decimal montoTransaccion, string Created, int AccountOper, int OperationType,
            string customer, string operDate, int LendingId, int InvestmentId, int PropertyId, string Receptor)
        {
            try
            {
                var check = await _context.Operations.Include(m => m.Account)
                    .Where(m => m.Account.Id == operation.AccountId && m.Number == operation.Number)
                    .FirstOrDefaultAsync();

                var existentClient = await _context.Clients.Where(m => m.Name == customer).FirstOrDefaultAsync();

                if (check == null)
                {
                    if (existentClient == null)
                    {
                        Client client = new()
                        {
                            Name = customer
                        };
                        _context.Add(client);
                        await _context.SaveChangesAsync();
                    }

                    //AccountDetail accdetail = new AccountDetail();

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

                    //var montoInicial = accountEdit.Amount;
                    //if (OperationType != 1)
                    //{
                    //    if (operation.Type == 0)
                    //    {
                    //        if (montoTransaccion != operation.InitialBalance && operation.InitialBalance != null)
                    //        {
                    //            if (operation.InitialBalance < montoTransaccion)
                    //            {
                    //                //accountEdit.Amount = montoInicial + montoTransaccion;
                    //                operation.Income = montoTransaccion;
                    //                operation.Outcome = 0;
                    //            }
                    //            else
                    //            {
                    //                //accountEdit.Amount = montoInicial + (decimal)operation.InitialBalance;
                    //                operation.Income = montoTransaccion;
                    //                operation.Outcome = 0;
                    //            }

                    //        }
                    //        else
                    //        {
                    //            //accountEdit.Amount = montoInicial + montoTransaccion;
                    //            operation.Income = montoTransaccion;
                    //            operation.Outcome = 0;
                    //            operation.InitialBalance = montoTransaccion;
                    //        }
                    //    }
                    //    else if (operation.Type == 1)
                    //    {
                    //        if (montoTransaccion != operation.InitialBalance && operation.InitialBalance != null)
                    //        {
                    //            //accountEdit.Amount = montoInicial - (decimal)operation.InitialBalance;
                    //            operation.Outcome = montoTransaccion;
                    //            operation.Income = 0;
                    //        }
                    //        else
                    //        {
                    //            //accountEdit.Amount = montoInicial - montoTransaccion;
                    //            operation.Outcome = montoTransaccion;
                    //            operation.Income = 0;
                    //            operation.InitialBalance = -montoTransaccion;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    if (operation.Type == 0)
                    {
                        //accountEdit.Amount = montoInicial + montoTransaccion;
                        operation.Income = montoTransaccion;
                        operation.Outcome = 0;
                    }
                    else if (operation.Type == 1 || operation.Type == 2)
                    {
                        //accountEdit.Amount = montoInicial - montoTransaccion;
                        operation.Outcome = montoTransaccion;
                        operation.Income = 0;
                    }
                    //}

                    //if (AccountOper == 0)
                    //{
                    //    accdetail.AccountId = operation.AccountId;
                    //}
                    //else
                    //{
                    //    accdetail.AccountId = AccountOper;
                    //}
                    //accdetail.UniqueId = Guid.NewGuid();
                    //accdetail.Description = operation.Description;
                    //accdetail.Concept = operation.Concept;
                    //accdetail.Author = operation.Author;
                    //accdetail.Created = DateTime.Now;
                    //if (accountEdit.Currency == 0)
                    //{
                    //    accdetail.SolesAmount = montoTransaccion;
                    //}
                    //else
                    //{
                    //    accdetail.DollarsAmount = montoTransaccion;
                    //}
                    //accdetail.InitialAmount = accountEdit.Amount;
                    //accdetail.Customer = customer;
                    //accdetail.OperationType = OperationType;
                    //accdetail.OperationDate = DateTime.Now;
                    //_context.Update(accountEdit);

                    if (OperationType != 1)
                    {
                        operation.FatherOperation = 1;
                    }

                    _context.Add(operation);
                    await _context.SaveChangesAsync();

                    //if (OperationType == 2)
                    //{
                    //    LendingOperation lendingOp = new();
                    //    lendingOp.UniqueId = Guid.NewGuid();
                    //    lendingOp.Type = operation.Type;
                    //    lendingOp.Modality = operation.Modality;
                    //    lendingOp.OperationId = operation.Id;
                    //    lendingOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //    lendingOp.Description = operation.Description;
                    //    lendingOp.Amount = montoTransaccion;
                    //    lendingOp.Created = DateTime.Now;
                    //    lendingOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                    //    lendingOp.FinalAmount = operation.InitialBalance;
                    //    _context.Add(lendingOp);
                    //}
                    //else if (OperationType == 3)
                    //{
                    //    InvestmentsOperation investmentOp = new();
                    //    investmentOp.UniqueId = Guid.NewGuid();
                    //    investmentOp.Type = operation.Type;
                    //    investmentOp.Modality = operation.Modality;
                    //    investmentOp.OperationId = operation.Id;
                    //    investmentOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //    investmentOp.Description = operation.Description;
                    //    investmentOp.Amount = montoTransaccion;
                    //    investmentOp.Created = DateTime.Now;
                    //    investmentOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                    //    investmentOp.FinalAmount = operation.InitialBalance;
                    //    _context.Add(investmentOp);
                    //}
                    //else if (OperationType == 4)
                    //{
                    //    PropertiesOperation propertiesOp = new();
                    //    propertiesOp.UniqueId = Guid.NewGuid();
                    //    propertiesOp.Type = operation.Type;
                    //    propertiesOp.Modality = operation.Modality;
                    //    propertiesOp.OperationId = operation.Id;
                    //    propertiesOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //    propertiesOp.Description = operation.Description;
                    //    propertiesOp.Amount = montoTransaccion;
                    //    propertiesOp.Created = DateTime.Now;
                    //    propertiesOp.FinalAmount = operation.InitialBalance;
                    //    if (Receptor != null)
                    //    {
                    //        propertiesOp.Receptor = Receptor;
                    //    }
                    //    else
                    //    {
                    //        propertiesOp.Receptor = "-";
                    //    }
                    //    propertiesOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                    //    _context.Add(propertiesOp);
                    //}
                    //accdetail.OperationId = operation.Id;
                    //_context.Add(accdetail);
                    //await _context.SaveChangesAsync();
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

            var accounts = await _context.Accounts.ToListAsync();
            var clients = await _context.Clients.Select(m => m.Name).ToListAsync();
            var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.ClientsJson = clientsJson;
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name");
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            ViewBag.LendingId = LendingId;
            ViewBag.InvestmentId = InvestmentId;
            ViewBag.PropertyId = PropertyId;
            return View(operation);
        }


        public async Task<IActionResult> CreateLending(int LendingId, int InvestmentId, int PropertyId)
        {
            var accounts = await _context.Accounts.Where(m => m.Visible == 1).ToListAsync();
            var clients = await _context.Clients.Select(m => m.Name).ToListAsync();
            var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.ClientsJson = clientsJson;
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name");
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            ViewBag.LendingId = LendingId;
            ViewBag.InvestmentId = InvestmentId;
            ViewBag.PropertyId = PropertyId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLending(Operation operation, decimal montoTransaccion, string Created, int AccountOper, int OperationType,
            string customer, string operDate, int LendingId, int InvestmentId, int PropertyId, string Receptor)
        {
            try
            {
                //var check = await _context.Operations.Include(m => m.Account)
                //    .Where(m => m.Account.Id == operation.AccountId && m.Number == operation.Number)
                //    .FirstOrDefaultAsync();
                var existentClient = await _context.Clients.Where(m => m.Name == customer).FirstOrDefaultAsync();



                //if (check == null)
                //{
                if (existentClient == null)
                {
                    Client client = new()
                    {
                        Name = customer
                    };
                    _context.Add(client);
                    await _context.SaveChangesAsync();
                }

                //AccountDetail accdetail = new AccountDetail();

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

                //var montoInicial = accountEdit.Amount;
                //if (OperationType != 1)
                //{
                //    if (operation.Type == 0)
                //    {
                //        if (montoTransaccion != operation.InitialBalance && operation.InitialBalance != null)
                //        {
                //            if (operation.InitialBalance < montoTransaccion)
                //            {
                //                //accountEdit.Amount = montoInicial + montoTransaccion;
                //                operation.Income = montoTransaccion;
                //                operation.Outcome = 0;
                //            }
                //            else
                //            {
                //                //accountEdit.Amount = montoInicial + (decimal)operation.InitialBalance;
                //                operation.Income = montoTransaccion;
                //                operation.Outcome = 0;
                //            }

                //        }
                //        else
                //        {
                //            //accountEdit.Amount = montoInicial + montoTransaccion;
                //            operation.Income = montoTransaccion;
                //            operation.Outcome = 0;
                //            operation.InitialBalance = montoTransaccion;
                //        }
                //    }
                //    else if (operation.Type == 1)
                //    {
                //        if (montoTransaccion != operation.InitialBalance && operation.InitialBalance != null)
                //        {
                //            //accountEdit.Amount = montoInicial - (decimal)operation.InitialBalance;
                //            operation.Outcome = montoTransaccion;
                //            operation.Income = 0;
                //        }
                //        else
                //        {
                //            //accountEdit.Amount = montoInicial - montoTransaccion;
                //            operation.Outcome = montoTransaccion;
                //            operation.Income = 0;
                //            operation.InitialBalance = -montoTransaccion;
                //        }
                //    }
                //}
                //else
                //{
                if (operation.Type == 0)
                {
                    //accountEdit.Amount = montoInicial + montoTransaccion;
                    operation.Income = montoTransaccion;
                    operation.Outcome = 0;
                }
                else if (operation.Type == 1 || operation.Type == 2)
                {
                    //accountEdit.Amount = montoInicial - montoTransaccion;
                    operation.Outcome = montoTransaccion;
                    operation.Income = 0;
                }
                //}

                //if (AccountOper == 0)
                //{
                //    accdetail.AccountId = operation.AccountId;
                //}
                //else
                //{
                //    accdetail.AccountId = AccountOper;
                //}
                //accdetail.UniqueId = Guid.NewGuid();
                //accdetail.Description = operation.Description;
                //accdetail.Concept = operation.Concept;
                //accdetail.Author = operation.Author;
                //accdetail.Created = DateTime.Now;
                //if (accountEdit.Currency == 0)
                //{
                //    accdetail.SolesAmount = montoTransaccion;
                //}
                //else
                //{
                //    accdetail.DollarsAmount = montoTransaccion;
                //}
                //accdetail.InitialAmount = accountEdit.Amount;
                //accdetail.Customer = customer;
                //accdetail.OperationType = OperationType;
                //accdetail.OperationDate = DateTime.Now;
                //_context.Update(accountEdit);

                if (OperationType != 1)
                {
                    operation.FatherOperation = 1;
                }

                _context.Add(operation);
                await _context.SaveChangesAsync();

                //if (OperationType == 2)
                //{
                //    LendingOperation lendingOp = new();
                //    lendingOp.UniqueId = Guid.NewGuid();
                //    lendingOp.Type = operation.Type;
                //    lendingOp.Modality = operation.Modality;
                //    lendingOp.OperationId = operation.Id;
                //    lendingOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    lendingOp.Description = operation.Description;
                //    lendingOp.Amount = montoTransaccion;
                //    lendingOp.Created = DateTime.Now;
                //    lendingOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                //    lendingOp.FinalAmount = operation.InitialBalance;
                //    _context.Add(lendingOp);
                //}
                //else if (OperationType == 3)
                //{
                //    InvestmentsOperation investmentOp = new();
                //    investmentOp.UniqueId = Guid.NewGuid();
                //    investmentOp.Type = operation.Type;
                //    investmentOp.Modality = operation.Modality;
                //    investmentOp.OperationId = operation.Id;
                //    investmentOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    investmentOp.Description = operation.Description;
                //    investmentOp.Amount = montoTransaccion;
                //    investmentOp.Created = DateTime.Now;
                //    investmentOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                //    investmentOp.FinalAmount = operation.InitialBalance;
                //    _context.Add(investmentOp);
                //}
                //else if (OperationType == 4)
                //{
                //    PropertiesOperation propertiesOp = new();
                //    propertiesOp.UniqueId = Guid.NewGuid();
                //    propertiesOp.Type = operation.Type;
                //    propertiesOp.Modality = operation.Modality;
                //    propertiesOp.OperationId = operation.Id;
                //    propertiesOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    propertiesOp.Description = operation.Description;
                //    propertiesOp.Amount = montoTransaccion;
                //    propertiesOp.Created = DateTime.Now;
                //    propertiesOp.FinalAmount = operation.InitialBalance;
                //    if (Receptor != null)
                //    {
                //        propertiesOp.Receptor = Receptor;
                //    }
                //    else
                //    {
                //        propertiesOp.Receptor = "-";
                //    }
                //    propertiesOp.Author = (int)HttpContext.Session.GetInt32("UserId");
                //    _context.Add(propertiesOp);
                //}
                //accdetail.OperationId = operation.Id;
                //_context.Add(accdetail);
                //await _context.SaveChangesAsync();
                TempData["Success"] = "Agregado exitosamente";
                return RedirectToAction(nameof(Index));
                //}
                //ViewBag.Check = "Documento ya existente";

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                TempData["Check"] = "Documento ya existente";
            }

            var accounts = await _context.Accounts.ToListAsync();
            var clients = await _context.Clients.Select(m => m.Name).ToListAsync();
            var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.ClientsJson = clientsJson;
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name");
            ViewBag.BankId = new SelectList(_context.Banks, "Id", "Name");
            ViewBag.LendingId = LendingId;
            ViewBag.InvestmentId = InvestmentId;
            ViewBag.PropertyId = PropertyId;
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
                else if (PropertyId != 0)
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
                        accdetail.AccountId = (int)operation.AccountId;
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
                        var father = await _context.Operations.FirstOrDefaultAsync(m => m.Id == LendingId);

                        LendingOperation lendingOp = new();
                        lendingOp.UniqueId = Guid.NewGuid();
                        lendingOp.Amount = montoTransaccion;
                        lendingOp.Type = operation.Type;
                        lendingOp.Modality = operation.Modality;
                        lendingOp.OperationId = LendingId;
                        lendingOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        lendingOp.Description = operation.Description;
                        lendingOp.Created = DateTime.Now;
                        if (lendingOp.Type == 0)
                        {
                            if (father.ActualBalance != null)
                            {
                                lendingOp.FinalAmount = father.ActualBalance + montoTransaccion;
                            }
                            else
                            {
                                lendingOp.FinalAmount = father.InitialBalance + montoTransaccion;
                            }
                        }
                        else
                        {
                            if (father.ActualBalance != null)
                            {
                                lendingOp.FinalAmount = father.ActualBalance - montoTransaccion;
                            }
                            else
                            {
                                lendingOp.FinalAmount = father.InitialBalance - montoTransaccion;
                            }
                        }
                        operation.Author = (int)HttpContext.Session.GetInt32("UserId");
                        _context.Add(lendingOp);
                    }
                    else if (OperationType == 3)
                    {
                        var father = await _context.Operations.FirstOrDefaultAsync(m => m.Id == InvestmentId);

                        InvestmentsOperation investmentOp = new();
                        investmentOp.UniqueId = Guid.NewGuid();
                        investmentOp.Amount = montoTransaccion;
                        investmentOp.Type = operation.Type;
                        investmentOp.Modality = operation.Modality;
                        investmentOp.OperationId = InvestmentId;
                        investmentOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        investmentOp.Description = operation.Description;
                        investmentOp.Created = DateTime.Now;
                        if (investmentOp.Type == 0)
                        {
                            if (father.ActualBalance != null)
                            {
                                investmentOp.FinalAmount = father.ActualBalance + montoTransaccion;
                            }
                            else
                            {
                                investmentOp.FinalAmount = father.InitialBalance + montoTransaccion;
                            }
                        }
                        else
                        {
                            if (father.ActualBalance != null)
                            {
                                investmentOp.FinalAmount = father.ActualBalance - montoTransaccion;
                            }
                            else
                            {
                                investmentOp.FinalAmount = father.InitialBalance - montoTransaccion;
                            }
                        }
                        operation.Author = (int)HttpContext.Session.GetInt32("UserId");
                        _context.Add(investmentOp);
                    }
                    else if (OperationType == 4)
                    {
                        var father = await _context.Operations.FirstOrDefaultAsync(m => m.Id == PropertyId);

                        PropertiesOperation propertiesOp = new();
                        propertiesOp.UniqueId = Guid.NewGuid();
                        propertiesOp.Amount = montoTransaccion;
                        propertiesOp.Type = operation.Type;
                        propertiesOp.Modality = operation.Modality;
                        propertiesOp.OperationId = PropertyId;
                        propertiesOp.OperationDate = DateTime.ParseExact(operDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        propertiesOp.Description = operation.Description;
                        propertiesOp.Created = DateTime.Now;
                        if (propertiesOp.Type == 0)
                        {
                            if (father.ActualBalance != null)
                            {
                                propertiesOp.FinalAmount = father.ActualBalance + montoTransaccion;
                            }
                            else
                            {
                                propertiesOp.FinalAmount = father.InitialBalance + montoTransaccion;
                            }
                        }
                        else
                        {
                            if (father.ActualBalance != null)
                            {
                                propertiesOp.FinalAmount = father.ActualBalance - montoTransaccion;
                            }
                            else
                            {
                                propertiesOp.FinalAmount = father.InitialBalance - montoTransaccion;
                            }
                        }
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
            var clients = await _context.Clients.ToListAsync();
            var clientsJson = JsonConvert.SerializeObject(clients);
            ViewBag.ClientsJson = clientsJson;
            ViewBag.AccountId = new SelectList(_context.Accounts, "Id", "", operation.AccountId);
            return View(operation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Operation operation, string Modified, string operDate, string Receptor, string customer)
        {
            try
            {
                var operationEdit = await _context.Operations.FirstOrDefaultAsync(m => m.UniqueId == operation.UniqueId);
                var accountEdit = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == operationEdit.AccountId);
                //var accdetailEdit = await _context.AccountDetails.FirstOrDefaultAsync(m => m.OperationId == operationEdit.Id);
                var specialOpL = await _context.LendingOperations.FirstOrDefaultAsync(m => m.OperationId == operationEdit.Id);
                var specialOpI = await _context.InvestmentsOperations.FirstOrDefaultAsync(m => m.OperationId == operationEdit.Id);
                var specialOpP = await _context.PropertiesOperations.FirstOrDefaultAsync(m => m.OperationId == operationEdit.Id);
                var check = 0;

                // Modificar tanto Operations, como Account y AccountDetail
                //--------------------------------------------------------------------------------------------------------------------
                // Movimiento errado, mismo monto (Ingreso/Salida)

                //operations, type: 0:In, 1:Out

                // Era abono, pero ahora es cargo
                if (operationEdit.Type == 0 && operation.Type == 1 && operationEdit.Income == operation.Income && check == 0)
                {
                    accountEdit.Amount = accountEdit.Amount - (2 * (decimal)operation.Income);
                    operationEdit.Outcome = operation.Income;
                    operationEdit.Income = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Outcome;
                    if (specialOpL != null && specialOpI == null && specialOpP == null)
                    {
                        specialOpL.Amount = (decimal)operationEdit.Outcome;
                        specialOpL.FinalAmount = specialOpL.Amount - (2 * (decimal)(operation.Income));
                    }
                    else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    {
                        specialOpI.Amount = (decimal)operationEdit.Outcome;
                        specialOpI.FinalAmount = specialOpL.Amount - (2 * (decimal)(operation.Income));
                    }
                    else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    {
                        specialOpP.Amount = (decimal)operationEdit.Outcome;
                        specialOpP.FinalAmount = specialOpL.Amount - (2 * (decimal)(operation.Income));
                    }
                    check++;
                }
                //Era cargo, pero ahora es abono
                else if (operationEdit.Type == 1 && operation.Type == 0 && operationEdit.Outcome == operation.Outcome && check == 0)
                {
                    accountEdit.Amount = accountEdit.Amount + (2 * (decimal)operation.Outcome);
                    operationEdit.Income = operation.Outcome;
                    operationEdit.Outcome = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Income;
                    if (specialOpL != null && specialOpI == null && specialOpP == null)
                    {
                        specialOpL.FinalAmount = specialOpL.Amount + (2 * (decimal)(operation.Outcome));
                        specialOpL.Amount = (decimal)operationEdit.Income;
                    }
                    else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    {
                        specialOpI.FinalAmount = specialOpL.Amount + (2 * (decimal)(operation.Outcome));
                        specialOpI.Amount = (decimal)operationEdit.Income;
                    }
                    else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    {
                        specialOpP.FinalAmount = specialOpL.Amount + (2 * (decimal)(operation.Outcome));
                        specialOpP.Amount = (decimal)operationEdit.Income;
                    }
                    check++;
                }

                // Movimiento errado, diferente monto (Ingreso, Salida)

                //Era abono, ahora es cargo
                if (operationEdit.Type == 0 && operation.Type == 1 && operationEdit.Income != operation.Income && check == 0)// && operation.Outcome != null)
                {
                    if (operationEdit.Income != null)
                    {
                        accountEdit.Amount = accountEdit.Amount + (decimal)operationEdit.Income - (decimal)operation.Income;

                        if (specialOpL != null && specialOpI == null && specialOpP == null)
                        {
                            specialOpL.FinalAmount = specialOpL.Amount + (decimal)operationEdit.Income - (decimal)operation.Income;
                            specialOpL.Amount = (decimal)operationEdit.Outcome;
                        }
                        else if (specialOpL == null && specialOpI != null && specialOpP == null)
                        {
                            specialOpI.FinalAmount = specialOpI.Amount + (decimal)operationEdit.Income - (decimal)operation.Income;
                            specialOpI.Amount = (decimal)operationEdit.Outcome;
                        }
                        else if (specialOpL == null && specialOpI == null && specialOpP != null)
                        {
                            specialOpP.FinalAmount = specialOpP.Amount + (decimal)operationEdit.Income - (decimal)operation.Income;
                            specialOpP.Amount = (decimal)operationEdit.Outcome;
                        }
                    }
                    else
                    {
                        accountEdit.Amount = accountEdit.Amount - (decimal)operation.Income;

                        if (specialOpL != null && specialOpI == null && specialOpP == null)
                        {
                            specialOpL.FinalAmount = specialOpL.Amount - (decimal)operation.Income;
                            specialOpL.Amount = (decimal)operationEdit.Outcome;
                        }
                        else if (specialOpL == null && specialOpI != null && specialOpP == null)
                        {
                            specialOpI.FinalAmount = specialOpI.Amount - (decimal)operation.Income;
                            specialOpI.Amount = (decimal)operationEdit.Outcome;
                        }
                        else if (specialOpL == null && specialOpI == null && specialOpP != null)
                        {
                            specialOpP.FinalAmount = specialOpP.Amount - (decimal)operation.Income;
                            specialOpP.Amount = (decimal)operationEdit.Outcome;
                        }
                    }

                    operationEdit.Outcome = operation.Income;
                    operationEdit.Income = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Outcome;
                    check++;
                }
                //Era cargo, pero ahora es abono
                else if (operationEdit.Type == 1 && operation.Type == 0 && operationEdit.Income != operation.Outcome && check == 0)// && operation.Income != null)
                {
                    if (operationEdit.Outcome != null)
                    {
                        accountEdit.Amount = accountEdit.Amount - (decimal)operationEdit.Outcome + (decimal)operation.Outcome;

                        if (specialOpL != null && specialOpI == null && specialOpP == null)
                        {
                            specialOpL.FinalAmount = specialOpL.Amount - (decimal)operationEdit.Outcome + (decimal)operation.Outcome;
                            specialOpL.Amount = (decimal)operationEdit.Income;
                        }
                        else if (specialOpL == null && specialOpI != null && specialOpP == null)
                        {
                            specialOpI.FinalAmount = specialOpI.Amount - (decimal)operationEdit.Outcome + (decimal)operation.Outcome;
                            specialOpI.Amount = (decimal)operationEdit.Income;
                        }
                        else if (specialOpL == null && specialOpI == null && specialOpP != null)
                        {
                            specialOpP.FinalAmount = specialOpP.Amount - (decimal)operationEdit.Outcome + (decimal)operation.Outcome;
                            specialOpP.Amount = (decimal)operationEdit.Income;
                        }
                    }
                    else
                    {
                        accountEdit.Amount = accountEdit.Amount + (decimal)operation.Outcome;

                        if (specialOpL != null && specialOpI == null && specialOpP == null)
                        {
                            specialOpL.FinalAmount = specialOpL.Amount + (decimal)operation.Outcome;
                            specialOpL.Amount = (decimal)operationEdit.Outcome;
                        }
                        else if (specialOpL == null && specialOpI != null && specialOpP == null)
                        {
                            specialOpI.FinalAmount = specialOpI.Amount + (decimal)operation.Outcome;
                            specialOpI.Amount = (decimal)operationEdit.Outcome;
                        }
                        else if (specialOpL == null && specialOpI == null && specialOpP != null)
                        {
                            specialOpP.FinalAmount = specialOpP.Amount + (decimal)operation.Outcome;
                            specialOpP.Amount = (decimal)operationEdit.Outcome;
                        }
                    }
                    operationEdit.Income = operation.Outcome;
                    operationEdit.Outcome = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Income;
                    //if (specialOpL != null && specialOpI == null && specialOpP == null)
                    //{
                    //    specialOpL.Amount = (decimal)operationEdit.Income;
                    //}
                    //else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    //{
                    //    specialOpI.Amount = (decimal)operationEdit.Income;
                    //}
                    //else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    //{
                    //    specialOpP.Amount = (decimal)operationEdit.Income;
                    //}
                    check++;
                }

                // Movimiento correcto, diferente monto
                if (operationEdit.Type == operation.Type && operation.Type == 1 && operationEdit.Outcome != operation.Outcome && check == 0)
                {
                    if (operationEdit.Outcome != null)
                    {
                        accountEdit.Amount = accountEdit.Amount + (decimal)operationEdit.Outcome - (decimal)operation.Outcome;
                    }
                    else
                    {
                        accountEdit.Amount = accountEdit.Amount - (decimal)operation.Outcome;
                    }
                    operationEdit.Outcome = operation.Outcome;
                    operationEdit.Income = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Outcome;
                    if (specialOpL != null && specialOpI == null && specialOpP == null)
                    {
                        specialOpL.FinalAmount = specialOpL.Amount + (decimal)operationEdit.Outcome - (decimal)operation.Outcome;
                        specialOpL.Amount = (decimal)operationEdit.Outcome;
                    }
                    else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    {
                        specialOpI.FinalAmount = specialOpL.Amount + (decimal)operationEdit.Outcome - (decimal)operation.Outcome;
                        specialOpI.Amount = (decimal)operationEdit.Outcome;
                    }
                    else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    {
                        specialOpP.FinalAmount = specialOpL.Amount + (decimal)operationEdit.Outcome - (decimal)operation.Outcome;
                        specialOpP.Amount = (decimal)operationEdit.Outcome;
                    }
                    check++;
                }
                else if (operationEdit.Type == operation.Type && operation.Type == 0 && operationEdit.Income != operation.Income && check == 0)
                {
                    if (operationEdit.Income != null)
                    {
                        accountEdit.Amount = accountEdit.Amount - (decimal)operationEdit.Income + (decimal)operation.Income;
                    }
                    else
                    {
                        accountEdit.Amount = accountEdit.Amount + (decimal)operation.Income;
                    }

                    operationEdit.Income = operation.Income;
                    operationEdit.Outcome = 0;
                    operationEdit.InitialBalance = (decimal)operationEdit.Income;
                    if (specialOpL != null && specialOpI == null && specialOpP == null)
                    {
                        specialOpL.FinalAmount = specialOpL.Amount - (decimal)operationEdit.Income + (decimal)operation.Income;
                        specialOpL.Amount = (decimal)operationEdit.Income;
                    }
                    else if (specialOpL == null && specialOpI != null && specialOpP == null)
                    {
                        specialOpI.FinalAmount = specialOpL.Amount - (decimal)operationEdit.Income + (decimal)operation.Income;
                        specialOpI.Amount = (decimal)operationEdit.Income;
                    }
                    else if (specialOpL == null && specialOpI == null && specialOpP != null)
                    {
                        specialOpP.FinalAmount = specialOpL.Amount - (decimal)operationEdit.Income + (decimal)operation.Income;
                        specialOpP.Amount = (decimal)operationEdit.Income;
                    }
                    check++;
                }

                // Movimiento correcto, monto correcto, otro cambio

                operationEdit.Customer = customer;
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
                //_context.Update(accdetailEdit);
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
            var specialopL = await _context.LendingOperations.Include(m => m.Operation).FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            var specialopI = await _context.InvestmentsOperations.Include(m => m.Operation).FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
            var specialopP = await _context.PropertiesOperations.Include(m => m.Operation).FirstOrDefaultAsync(m => m.UniqueId == UniqueId);


            if (operation == null && specialopL == null && specialopI == null && specialopP == null)
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
                var specialopL = await _context.LendingOperations.Include(m => m.Operation).FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                var specialopI = await _context.InvestmentsOperations.Include(m => m.Operation).FirstOrDefaultAsync(m => m.UniqueId == UniqueId);
                var specialopP = await _context.PropertiesOperations.Include(m => m.Operation).FirstOrDefaultAsync(m => m.UniqueId == UniqueId);

                if (operation != null)
                {
                    var account = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == operation.AccountId);
                    if (account != null && operation != null)
                    {
                        if (operation.Type == 0)
                        {
                            if (operation.ActualBalance != null)
                            {
                                account.Amount = account.Amount - (decimal)operation.ActualBalance;
                            }
                            else
                            {
                                account.Amount = account.Amount - (decimal)operation.Income;
                            }
                        }
                        else
                        {
                            if (operation.ActualBalance != null)
                            {
                                account.Amount = account.Amount + (decimal)operation.ActualBalance;
                            }
                            else
                            {
                                account.Amount = account.Amount + (decimal)operation.Outcome;
                            }
                        }
                        _context.Update(account);
                    }
                    _context.Operations.Remove(operation);
                }

                if (specialopL != null)
                {
                    var accountL = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == specialopL.Operation.AccountId);
                    if (accountL != null && specialopL != null)
                    {
                        if (specialopL.Type == 0)
                        {
                            if (specialopL.FinalAmount != null)
                            {
                                accountL.Amount = accountL.Amount - (decimal)specialopL.FinalAmount;
                            }
                            else
                            {
                                accountL.Amount = accountL.Amount - (decimal)specialopL.Amount;
                            }
                        }
                        else
                        {
                            if (operation.ActualBalance != null)
                            {
                                accountL.Amount = accountL.Amount + (decimal)specialopL.FinalAmount;
                            }
                            else
                            {
                                accountL.Amount = accountL.Amount + (decimal)specialopL.Amount;
                            }
                        }
                        _context.Update(accountL);
                    }
                    _context.LendingOperations.Remove(specialopL);
                }

                if (specialopI != null)
                {
                    var accountI = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == specialopI.Operation.AccountId); ;
                    if (accountI != null && specialopI != null)
                    {
                        if (specialopI.Type == 0)
                        {
                            if (specialopI.FinalAmount != null)
                            {
                                accountI.Amount = accountI.Amount - (decimal)specialopI.FinalAmount;
                            }
                            else
                            {
                                accountI.Amount = accountI.Amount - (decimal)specialopI.Amount;
                            }
                        }
                        else
                        {
                            if (operation.ActualBalance != null)
                            {
                                accountI.Amount = accountI.Amount + (decimal)specialopI.FinalAmount;
                            }
                            else
                            {
                                accountI.Amount = accountI.Amount + (decimal)specialopI.Amount;
                            }
                        }
                        _context.Update(accountI);
                    }
                    _context.InvestmentsOperations.Remove(specialopI);
                }

                if (specialopP != null)
                {
                    var accountP = await _context.Accounts.FirstOrDefaultAsync(m => m.Id == specialopP.Operation.AccountId);
                    if (accountP != null && specialopP != null)
                    {
                        if (specialopL.Type == 0)
                        {
                            if (specialopP.FinalAmount != null)
                            {
                                accountP.Amount = accountP.Amount - (decimal)specialopP.FinalAmount;
                            }
                            else
                            {
                                accountP.Amount = accountP.Amount - (decimal)specialopP.Amount;
                            }
                        }
                        else
                        {
                            if (operation.ActualBalance != null)
                            {
                                accountP.Amount = accountP.Amount + (decimal)specialopP.FinalAmount;
                            }
                            else
                            {
                                accountP.Amount = accountP.Amount + (decimal)specialopP.Amount;
                            }
                        }
                        _context.Update(accountP);
                    }
                    _context.PropertiesOperations.Remove(specialopP);
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
            //try
            //{
            //    //var accountData = await _context.Accounts.Where(m => m.AccountType != "Inversión"
            //    //&& m.AccountType != "Préstamo" && m.AccountType != "Predios").ToListAsync();
            //    var accountData = await _context.Accounts.ToListAsync();
            //    decimal? accountMoney = 0;
            //    decimal? accountIncome = 0;
            //    decimal? accountOutcome = 0;
            //    decimal? totalIncome = 0;
            //    decimal? totalOutcome = 0;

            //    foreach (var account in accountData)
            //    {
            //        accountMoney = account.PreviousRemaining;
            //        var operationsData = await _context.Operations.Where(m => m.Year == Year && m.Month == Month
            //        && m.AccountId == account.Id && m.Type != 2).ToListAsync();

            //        foreach (var operation in operationsData)
            //        {
            //            accountIncome = operation.Income;
            //            accountOutcome = operation.Outcome;
            //            totalIncome = totalIncome + accountIncome;
            //            totalOutcome = totalOutcome + accountOutcome;
            //        }

            //        var closhure = await _context.Operations.Where(m => m.Year == Year && m.Month == Month
            //       && m.AccountId == account.Id && m.Type == 2).FirstOrDefaultAsync();

            //        if (closhure == null)
            //        {
            //            closhure = new Operation();
            //            closhure.UniqueId = Guid.NewGuid();
            //            closhure.Created = DateTime.Now;
            //            closhure.Author = (int)HttpContext.Session.GetInt32("UserId");
            //            DateTime selectedDate = closhure.Created;
            //            closhure.Year = Year;
            //            closhure.Month = Month;
            //            closhure.Income = totalIncome;
            //            closhure.Outcome = totalOutcome;
            //            closhure.Concept = "Cierre de mes";
            //            closhure.Description = "Cierre de mes";
            //            closhure.Modality = 100;
            //            closhure.Type = 2;
            //            closhure.Number = "0";
            //            closhure.OperationDate = DateTime.Now;
            //            closhure.AccountId = account.Id;
            //            closhure.OperationType = 5;
            //            _context.Add(closhure);
            //        }
            //        else
            //        {

            //            closhure.Modified = DateTime.Now;
            //            closhure.Editor = (int)HttpContext.Session.GetInt32("UserId");
            //            DateTime selectedDate = closhure.Created;
            //            closhure.Year = Year;
            //            closhure.Month = Month;
            //            closhure.Income = totalIncome;
            //            closhure.Outcome = totalOutcome;
            //            closhure.Concept = "Cierre de mes";
            //            closhure.Description = "Cierre de mes";
            //            closhure.Modality = 100;
            //            closhure.Type = 2;
            //            closhure.Number = "0";
            //            closhure.OperationDate = DateTime.Now;
            //            closhure.AccountId = account.Id;
            //            closhure.OperationType = 5;
            //            _context.Update(closhure);
            //        }



            //        if (accountMoney + totalIncome - totalOutcome == account.Amount)
            //        {
            //            account.PreviousRemaining = account.Amount;
            //            account.Modified = DateTime.Now;
            //            account.Editor = (int)HttpContext.Session.GetInt32("UserId");
            //            _context.Update(account);
            //        }
            //        accountMoney = 0;
            //        accountIncome = 0;
            //        accountOutcome = 0;
            //        totalIncome = 0;
            //        totalOutcome = 0;
            //    }

            //    await _context.SaveChangesAsync();
            //    TempData["Success"] = "Agregado exitosamente";
            //    return RedirectToAction(nameof(Index));

            //}
            //catch (Exception ex)
            //{
            //    TempData["Error"] = "Error: " + ex.Message;
            //}

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

        class Rpta
        {
            public string Name { get; set; }
            public int Moneda { get; set; }
            public decimal SaldoInicial { get; set; }
            public decimal Suma { get; set; }
            public decimal Suma2 { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reportes(String fechaInicio, String fechaFin, string AccountId, int Modality)
        {

            var fechaInicioDT = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);
            var fechaFinDT = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.AddWorksheet();
            int cont = 2;

            ws.Cell("A" + cont).Value = "Reporte de Movimientos";

            cont = cont + 2;

            ws.Range("A" + cont, "O" + cont).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont, "O" + cont).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont, "O" + cont).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont, "O" + cont).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont).Value = "Banco";
            ws.Cell("A" + cont).Value = "Banco";
            ws.Cell("B" + cont).Value = "Número de cuenta";
            ws.Cell("C" + cont).Value = "Tipo de cuenta";
            ws.Cell("D" + cont).Value = "Moneda";
            ws.Cell("E" + cont).Value = "Cliente";
            ws.Cell("F" + cont).Value = "Movimiento";
            ws.Cell("G" + cont).Value = "Concepto";
            ws.Cell("H" + cont).Value = "Descripción";
            //ws.Cell("H" + cont).Value = "Monto";
            ws.Cell("I" + cont).Value = "Modalidad";
            ws.Cell("J" + cont).Value = "Número";
            ws.Cell("K" + cont).Value = "Fecha";
            //ws.Cell("M" + cont).Value = "Monto";
            ws.Cell("L" + cont).Value = "Ingreso (S/)";
            ws.Cell("M" + cont).Value = "Salida (S/)";
            ws.Cell("N" + cont).Value = "Ingreso (USD)";
            ws.Cell("O" + cont).Value = "Salida (USD)";


            cont++;
            List<Account> accounts = new List<Account>();
            List<Rpta> rptas = new List<Rpta>();


            if (AccountId == "none")
            {
                accounts = _context.Accounts.Where(m => m.Id != 49 && m.Id != 50).ToList();
            }
            else
            {
                accounts = _context.Accounts.Where(m => m.Name.Equals(AccountId)).ToList();
            }

            decimal sumaL = 0;
            decimal sumaM = 0;
            decimal sumaN = 0;
            decimal sumaO = 0;

            foreach (var item in accounts)
            {
                var operation = _context.Operations.Include(m => m.Account).Where(m => m.AccountId == item.Id && m.Type < 2).ToList();
                decimal suma = 0;



                decimal suma2 = 0;
                foreach (var obj in operation)
                {
                    if (obj.OperationDate >= fechaInicioDT && obj.OperationDate <= fechaFinDT)
                    {
                        if (obj.Type == 2)
                        {
                            continue;
                        }
                        ws.Cell("A" + cont).Value = item.Name;
                        ws.Cell("A" + cont).Value = item.Name;
                        ws.Cell("B" + cont).Value = item.AccountNumber;
                        ws.Cell("C" + cont).Value = item.AccountType;

                        ws.Cell("D" + cont).Value = item.Currency == 0 ? "Soles" : "Dólares";
                        ws.Cell("E" + cont).Value = obj.Customer;

                        ws.Cell("F" + cont).Value = obj.Type == 0 ? "Ingreso" : "Egreso";

                        //if ()
                        //{
                        //    ws.Cell("F" + cont).Value = "Ingreso";
                        //}
                        //else if (obj.Type == 1)
                        //{
                        //    ws.Cell("F" + cont).Value = "Egreso";
                        //}
                        //else if (obj.Type == 100)
                        //{
                        //    ws.Cell("F" + cont).Value = "Cierre de mes";
                        //}
                        //else if (obj.Type == 200)
                        //{
                        //    ws.Cell("F" + cont).Value = "Apertura de cuenta";
                        //}

                        ws.Cell("G" + cont).Value = obj.Concept;
                        ws.Cell("H" + cont).Value = obj.Description;

                        if (obj.Modality == 0)
                        {
                            ws.Cell("I" + cont).Value = "Cheque";

                        }
                        else if (obj.Modality == 1)
                        {
                            ws.Cell("I" + cont).Value = "Transferencia";
                        }
                        else if (obj.Modality == 100)
                        {
                            ws.Cell("I" + cont).Value = "Cierre de mes";
                        }
                        else if (obj.Modality == 200)
                        {
                            ws.Cell("I" + cont).Value = "Apertura de cuenta";
                        }
                        ws.Cell("J" + cont).Value = obj.Number;
                        ws.Cell("K" + cont).Value = obj.OperationDate;

                        if (obj.Account.Currency == 0 && obj.Type == 0)
                        {

                            ws.Cell("L" + cont).Value = obj.Income;
                            ws.Cell("L" + cont).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell("L" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;



                            suma = suma + Convert.ToDecimal(obj.Income);
                            sumaL = sumaL + Convert.ToDecimal(obj.Income);
                            //_ = (obj.OperationDate > fechaInicioDT) ? (suma2 = suma2 + Convert.ToDecimal(obj.Outcome)) : 0;
                        }
                        else
                        {
                            ws.Cell("L" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 0 && obj.Type == 1)
                        {

                            ws.Cell("M" + cont).Value = obj.Outcome;
                            ws.Cell("M" + cont).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell("M" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


                            suma = suma - Convert.ToDecimal(obj.Outcome);
                            sumaM = sumaM + Convert.ToDecimal(obj.Outcome);
                            //_ = (fechaInicioDT > obj.OperationDate) ? (suma2 = suma2 - Convert.ToDecimal(obj.Outcome)) : 0;
                        }
                        else
                        {
                            ws.Cell("M" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 1 && obj.Type == 0)
                        {

                            ws.Cell("N" + cont).Value = obj.Income;
                            ws.Cell("N" + cont).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell("N" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


                            suma = suma + Convert.ToDecimal(obj.Income);
                            sumaN = sumaN + Convert.ToDecimal(obj.Income);
                            //_ = (fechaInicioDT > obj.OperationDate) ? (suma2 = suma2 + Convert.ToDecimal(obj.Outcome)) : 0;
                        }
                        else
                        {
                            ws.Cell("N" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 1 && obj.Type == 1)
                        {

                            ws.Cell("O" + cont).Value = obj.Outcome;
                            ws.Cell("O" + cont).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell("O" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


                            suma = suma - Convert.ToDecimal(obj.Outcome);
                            sumaO = sumaO + Convert.ToDecimal(obj.Outcome);
                            // _ = (fechaInicioDT > obj.OperationDate) ? (suma2 = suma2 - Convert.ToDecimal(obj.Outcome)) : 0;
                        }
                        else
                        {
                            ws.Cell("O" + cont).Value = "-";
                        }
                        cont++;
                    }
                    else
                    {
                        if (obj.OperationDate <= fechaInicioDT)
                        {
                            if (obj.Account.Currency == 0 && obj.Type == 0)
                            {
                                suma2 = suma2 + Convert.ToDecimal(obj.Income);
                            }
                            if (obj.Account.Currency == 0 && obj.Type == 1)
                            {
                                suma2 = suma2 - Convert.ToDecimal(obj.Outcome);
                            }
                            if (obj.Account.Currency == 1 && obj.Type == 0)
                            {
                                suma2 = suma2 + Convert.ToDecimal(obj.Income);
                            }
                            if (obj.Account.Currency == 1 && obj.Type == 1)
                            {
                                suma2 = suma2 - Convert.ToDecimal(obj.Outcome);
                            }
                        }

                    }


                }

                rptas.Add(new Rpta()
                {
                    Name = item.Name,
                    Moneda = item.Currency,
                    Suma2 = item.Amount + suma2,
                    Suma = item.Amount + suma2 + suma,

                }); ;
            }
            cont = cont + 1;

            ws.Cell("L" + cont).Value = sumaL;
            ws.Cell("L" + cont).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell("L" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("L" + cont).Style.Font.Bold = true;

            ws.Cell("M" + cont).Value = sumaM;
            ws.Cell("M" + cont).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell("M" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("M" + cont).Style.Font.Bold = true;

            ws.Cell("N" + cont).Value = sumaN;
            ws.Cell("N" + cont).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell("N" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("N" + cont).Style.Font.Bold = true;

            ws.Cell("O" + cont).Value = sumaO;
            ws.Cell("O" + cont).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell("O" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("O" + cont).Style.Font.Bold = true;




            cont = cont + 3;
            var cont_a = cont - 1;

            ws.Range("A" + cont_a, "C" + cont_a).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont_a, "C" + cont_a).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont_a, "C" + cont_a).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont_a, "C" + cont_a).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont_a).Value = "Cuenta";
            ws.Cell("B" + cont_a).Value = "Soles";
            ws.Cell("C" + cont_a).Value = "Dolares";
            cont = cont + 1;
            var Operations = await _context.Operations.Where(m => m.Type < 2).ToListAsync();
            var accounts2 = await _context.Accounts.Where(m => m.Visible == null || m.Visible == 0).ToListAsync();
            decimal? sumsol = 0;
            decimal? sumdlls = 0;

            foreach (var item in accounts2)
            {

                var Income = Operations.Where(m => m.AccountId == item.Id).Select(m => m.Income).Sum();
                var Outcome = Operations.Where(m => m.AccountId == item.Id).Select(m => m.Outcome).Sum();
                var Amount = item.PreviousRemaining + Income - Outcome;

                if (item.Currency == 0)
                {
                    sumsol = sumsol + Amount;
                }
                else
                {
                    sumdlls = sumdlls + Amount;
                }

                ws.Cell("A" + cont).Value = item.Name;

                ws.Cell("B" + cont).Value = item.Currency == 0 ? Amount : 0;
                ws.Cell("B" + cont).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("C" + cont).Value = item.Currency == 1 ? Amount : 0; ;
                ws.Cell("C" + cont).Style.NumberFormat.Format = "#,##0.00";

                cont = cont + 1;


            }

            
            cont = cont + 1;

            ws.Cell("B" + cont).Value = sumsol;
            ws.Cell("B" + cont).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell("B" + cont).Style.Font.Bold = true;

            ws.Cell("C" + cont).Value = sumdlls;
            ws.Cell("C" + cont).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell("C" + cont).Style.Font.Bold = true;


            ws.Columns("A", "T").AdjustToContents();
            return wb.Deliver("Reporte.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<IActionResult> ReportesPrestamos()
        {
            ViewBag.YearList = await _context.Operations.Select(m => m.Year).Distinct().ToListAsync();

            ViewBag.Customers = await _context.Operations
                            .Where(m => m.OperationType == 2 && m.FatherOperation == 1)
                            .Select(m => m.Customer).Distinct().ToListAsync();

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportesPrestamos(String fechaInicio, String fechaFin, string AccountId, int Modality)
        {
            var fechaInicioDT = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);
            var fechaFinDT = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.AddWorksheet();
            int cont = 2;

            ws.Cell("A" + cont).Value = "Reporte de Prestamos";

            cont = cont + 2;

            ws.Range("A" + cont, "O" + cont).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont, "O" + cont).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont, "O" + cont).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont, "O" + cont).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont).Value = "Banco";
            ws.Cell("A" + cont).Value = "Banco";
            ws.Cell("B" + cont).Value = "Número de cuenta";
            ws.Cell("C" + cont).Value = "Tipo de cuenta";
            ws.Cell("D" + cont).Value = "Moneda";
            ws.Cell("E" + cont).Value = "Cliente";
            ws.Cell("F" + cont).Value = "Movimiento";
            ws.Cell("G" + cont).Value = "Concepto";
            ws.Cell("H" + cont).Value = "Descripción";
            ws.Cell("I" + cont).Value = "Modalidad";
            ws.Cell("J" + cont).Value = "Número";
            ws.Cell("K" + cont).Value = "Fecha";
            ws.Cell("L" + cont).Value = "Ingreso (S/)";
            ws.Cell("M" + cont).Value = "Salida (S/)";
            ws.Cell("N" + cont).Value = "Ingreso (USD)";
            ws.Cell("O" + cont).Value = "Salida (USD)";


            cont++;
            List<string> customers = new List<string>();
            Dictionary<string, decimal> myDictionary = new Dictionary<string, decimal>();
            if (AccountId == "none")
            {
                customers = await _context.Operations
                        .Where(m => m.OperationType == 2 && m.FatherOperation == 1)
                        .Select(m => m.Customer).Distinct().ToListAsync();
            }
            else
            {
                customers.Add(AccountId);
            }

            foreach (var item in customers)
            {
                var operation = _context.Operations.Include(m => m.Account).Where(m => m.Customer == item)
                    .Where(m => m.OperationType == 2 && m.FatherOperation == 1)
                    .ToList();

                decimal suma = 0;
                foreach (var obj in operation)
                {
                    if (obj.OperationDate >= fechaInicioDT && obj.OperationDate <= fechaFinDT)
                    {
                        ws.Cell("A" + cont).Value = obj.Account.Name;
                        ws.Cell("A" + cont).Value = obj.Account.Name;
                        ws.Cell("B" + cont).Value = obj.Account.AccountNumber;
                        ws.Cell("C" + cont).Value = obj.Account.AccountType;

                        if (obj.Account.Currency == 0)
                        {
                            ws.Cell("D" + cont).Value = "Soles";
                        }
                        else
                        {
                            ws.Cell("D" + cont).Value = "Dólares";
                        }

                        ws.Cell("E" + cont).Value = obj.Customer;

                        if (obj.Type == 0)
                        {
                            ws.Cell("F" + cont).Value = "Ingreso";
                        }
                        else if (obj.Type == 1)
                        {
                            ws.Cell("F" + cont).Value = "Egreso";
                        }
                        else if (obj.Type == 100)
                        {
                            ws.Cell("F" + cont).Value = "Cierre de mes";
                        }
                        else if (obj.Type == 200)
                        {
                            ws.Cell("F" + cont).Value = "Apertura de cuenta";
                        }

                        ws.Cell("G" + cont).Value = obj.Concept;
                        ws.Cell("H" + cont).Value = obj.Description;

                        if (obj.Modality == 0)
                        {
                            ws.Cell("I" + cont).Value = "Cheque";
                        }
                        else if (obj.Modality == 1)
                        {
                            ws.Cell("I" + cont).Value = "Transferencia";
                        }
                        else if (obj.Modality == 200)
                        {
                            ws.Cell("I" + cont).Value = "Apertura de cuenta";
                        }
                        ws.Cell("J" + cont).Value = obj.Number;
                        ws.Cell("K" + cont).Value = obj.OperationDate;

                        if (obj.Account.Currency == 0 && obj.Type == 0)
                        {
                            ws.Cell("L" + cont).Value = obj.Income;
                            suma = suma + Convert.ToDecimal(obj.Income);
                        }
                        else
                        {
                            ws.Cell("L" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 0 && (obj.Type == 1 || obj.Type == 2))
                        {
                            ws.Cell("M" + cont).Value = obj.Outcome;
                            suma = suma - Convert.ToDecimal(obj.Outcome);
                        }
                        else
                        {
                            ws.Cell("M" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 1 && obj.Type == 0)
                        {
                            ws.Cell("N" + cont).Value = obj.Income;
                            suma = suma + Convert.ToDecimal(obj.Income);
                        }
                        else
                        {
                            ws.Cell("N" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 1 && (obj.Type == 1 || obj.Type == 2))
                        {
                            ws.Cell("O" + cont).Value = obj.Outcome;
                            suma = suma - Convert.ToDecimal(obj.Outcome);
                        }
                        else
                        {
                            ws.Cell("O" + cont).Value = "-";
                        }
                        cont++;
                    }
                    else
                    {
                        if (obj.OperationDate <= fechaInicioDT)
                        {
                            if (obj.Account.Currency == 0 && obj.Type == 0)
                            {
                                suma = suma + Convert.ToDecimal(obj.Income);
                            }
                            if (obj.Account.Currency == 0 && (obj.Type == 1 || obj.Type == 2))
                            {
                                suma = suma - Convert.ToDecimal(obj.Outcome);
                            }
                            if (obj.Account.Currency == 1 && obj.Type == 0)
                            {
                                suma = suma + Convert.ToDecimal(obj.Income);
                            }
                            if (obj.Account.Currency == 1 && (obj.Type == 1 || obj.Type == 2))
                            {
                                suma = suma - Convert.ToDecimal(obj.Outcome);
                            }
                        }
                    }
                }
                myDictionary.Add(item, suma);
            }

            cont = cont + 3;
            var cont_a = cont - 1;

            ws.Range("A" + cont_a, "B" + cont_a).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont_a, "B" + cont_a).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont_a, "B" + cont_a).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont_a, "B" + cont_a).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont_a).Value = "Cuenta";
            ws.Cell("B" + cont_a).Value = "Saldo Actual";

            foreach (KeyValuePair<string, decimal> kvp in myDictionary)
            {
                ws.Cell("A" + cont).Value = kvp.Key;
                ws.Cell("B" + cont).Value = kvp.Value;
                cont++;
            }

            ws.Columns("A", "T").AdjustToContents();
            return wb.Deliver("Reporte.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }


        public async Task<IActionResult> ReportesPrestamosTotales()
        {

            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.AddWorksheet();
            int cont = 2;

            ws.Cell("A" + cont).Value = "Reporte de Prestamos";

            cont = cont + 2;

            ws.Range("A" + cont, "D" + cont).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont, "D" + cont).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont, "D" + cont).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont, "D" + cont).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont).Value = "Cliente";
            ws.Cell("B" + cont).Value = "Moneda";
            ws.Cell("C" + cont).Value = "S/";
            ws.Cell("D" + cont).Value = "$";

            List<CustomerTotal> list = new List<CustomerTotal>();
            var operations = await _context.Operations
                .Include(m => m.Account)
                .ThenInclude(m => m.AccountDetails)
                .Where(m => m.OperationType == 2 && m.FatherOperation == 1)
                .ToListAsync();

            var operationDistintas = operations.Distinct(new OperationComparer()).ToList();


            decimal TotalSoles = 0;
            decimal TotalDolares = 0;


            foreach (var operation in operationDistintas)
            {
                decimal total = (decimal)(operations.Where(m => m.Customer == operation.Customer && m.Account.Currency == operation.Account.Currency).Sum(m => m.Income) - operations.Where(m => m.Customer == operation.Customer && m.Account.Currency == operation.Account.Currency).Sum(m => m.Outcome));

                total = Math.Abs(total);

                //list.Add(new CustomerTotal()
                //{
                //    Customer = operation.Customer,
                //    AccountName = operation.Account.Name,
                //    Currency = operation.Account.Currency == 0 ? "Soles" : "Dolares",
                //    TotalSoles = operation.Account.Currency == 0 ? total : 0,
                //    TotalDolares = operation.Account.Currency == 1 ? total : 0,
                //});

                cont++;

                var ValSoles = operation.Account.Currency == 0 ? total : 0;
                var ValDolares = operation.Account.Currency == 1 ? total : 0;

                TotalSoles = TotalSoles + ValSoles;
                TotalDolares = TotalDolares + ValSoles;


                ws.Cell("A" + cont).Value = operation.Customer;
                ws.Cell("B" + cont).Value = operation.Account.Currency == 0 ? "Soles" : "Dolares";

                //var cellB2 = worksheet.Cell("B2");
                //cellB2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                //cellB2.Style.Font.Bold = true;

                ws.Cell("C" + cont).Value = string.Format("{0:#,##0.00}", ValSoles);
                ws.Cell("C" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell("D" + cont).Value = string.Format("{0:#,##0.00}", ValDolares);
                ws.Cell("D" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            }

            ws.Cell("A" + cont).Value = "";
            ws.Cell("B" + cont).Value = "Total";
            ws.Cell("B" + cont).Style.Font.Bold = true;


            ws.Cell("C" + cont).Value = string.Format("{0:#,##0.00}", TotalSoles);
            ws.Cell("C" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("C" + cont).Style.Font.Bold = true;

            ws.Cell("D" + cont).Value = string.Format("{0:#,##0.00}", TotalDolares);
            ws.Cell("D" + cont).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("D" + cont).Style.Font.Bold = true;








            ws.Columns("A", "D").AdjustToContents();
            return wb.Deliver("Reporte.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<IActionResult> ReportesInversiones()
        {
            ViewBag.YearList = await _context.Operations.Select(m => m.Year).Distinct().ToListAsync();

            ViewBag.Customers = await _context.Operations
                            .Where(m => m.OperationType == 3 && m.FatherOperation == 1)
                            .Select(m => m.Customer).Distinct().ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportesInversiones(String fechaInicio, String fechaFin, string AccountId, int Modality)
        {
            var fechaInicioDT = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);
            var fechaFinDT = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.AddWorksheet();
            int cont = 2;

            ws.Cell("A" + cont).Value = "Reporte de Inversiones";

            cont = cont + 2;

            ws.Range("A" + cont, "O" + cont).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont, "O" + cont).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont, "O" + cont).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont, "O" + cont).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont).Value = "Banco";
            ws.Cell("A" + cont).Value = "Banco";
            ws.Cell("B" + cont).Value = "Número de cuenta";
            ws.Cell("C" + cont).Value = "Tipo de cuenta";
            ws.Cell("D" + cont).Value = "Moneda";
            ws.Cell("E" + cont).Value = "Cliente";
            ws.Cell("F" + cont).Value = "Movimiento";
            ws.Cell("G" + cont).Value = "Concepto";
            ws.Cell("H" + cont).Value = "Descripción";
            ws.Cell("I" + cont).Value = "Modalidad";
            ws.Cell("J" + cont).Value = "Número";
            ws.Cell("K" + cont).Value = "Fecha";
            ws.Cell("L" + cont).Value = "Ingreso (S/)";
            ws.Cell("M" + cont).Value = "Salida (S/)";
            ws.Cell("N" + cont).Value = "Ingreso (USD)";
            ws.Cell("O" + cont).Value = "Salida (USD)";


            cont++;
            List<string> customers = new List<string>();
            Dictionary<string, decimal> myDictionary = new Dictionary<string, decimal>();
            if (AccountId == "none")
            {
                customers = await _context.Operations
                        .Where(m => m.OperationType == 3 && m.FatherOperation == 1)
                        .Select(m => m.Customer).Distinct().ToListAsync();
            }
            else
            {
                customers.Add(AccountId);
            }

            foreach (var item in customers)
            {
                var operation = _context.Operations.Include(m => m.Account).Where(m => m.Customer == item)
                    .Where(m => m.OperationType == 2 && m.FatherOperation == 1)
                    .ToList();

                decimal suma = 0;
                foreach (var obj in operation)
                {
                    if (obj.OperationDate >= fechaInicioDT && obj.OperationDate <= fechaFinDT)
                    {
                        ws.Cell("A" + cont).Value = obj.Account.Name;
                        ws.Cell("A" + cont).Value = obj.Account.Name;
                        ws.Cell("B" + cont).Value = obj.Account.AccountNumber;
                        ws.Cell("C" + cont).Value = obj.Account.AccountType;

                        if (obj.Account.Currency == 0)
                        {
                            ws.Cell("D" + cont).Value = "Soles";
                        }
                        else
                        {
                            ws.Cell("D" + cont).Value = "Dólares";
                        }

                        ws.Cell("E" + cont).Value = obj.Customer;

                        if (obj.Type == 0)
                        {
                            ws.Cell("F" + cont).Value = "Ingreso";
                        }
                        else if (obj.Type == 1)
                        {
                            ws.Cell("F" + cont).Value = "Egreso";
                        }
                        else if (obj.Type == 100)
                        {
                            ws.Cell("F" + cont).Value = "Cierre de mes";
                        }
                        else if (obj.Type == 200)
                        {
                            ws.Cell("F" + cont).Value = "Apertura de cuenta";
                        }

                        ws.Cell("G" + cont).Value = obj.Concept;
                        ws.Cell("H" + cont).Value = obj.Description;

                        if (obj.Modality == 0)
                        {
                            ws.Cell("I" + cont).Value = "Cheque";
                        }
                        else if (obj.Modality == 1)
                        {
                            ws.Cell("I" + cont).Value = "Transferencia";
                        }
                        else if (obj.Modality == 200)
                        {
                            ws.Cell("I" + cont).Value = "Apertura de cuenta";
                        }
                        ws.Cell("J" + cont).Value = obj.Number;
                        ws.Cell("K" + cont).Value = obj.OperationDate;

                        if (obj.Account.Currency == 0 && obj.Type == 0)
                        {
                            ws.Cell("L" + cont).Value = obj.Income;
                            suma = suma + Convert.ToDecimal(obj.Income);
                        }
                        else
                        {
                            ws.Cell("L" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 0 && (obj.Type == 1 || obj.Type == 2))
                        {
                            ws.Cell("M" + cont).Value = obj.Outcome;
                            suma = suma - Convert.ToDecimal(obj.Outcome);
                        }
                        else
                        {
                            ws.Cell("M" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 1 && obj.Type == 0)
                        {
                            ws.Cell("N" + cont).Value = obj.Income;
                            suma = suma + Convert.ToDecimal(obj.Income);
                        }
                        else
                        {
                            ws.Cell("N" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 1 && (obj.Type == 1 || obj.Type == 2))
                        {
                            ws.Cell("O" + cont).Value = obj.Outcome;
                            suma = suma - Convert.ToDecimal(obj.Outcome);
                        }
                        else
                        {
                            ws.Cell("O" + cont).Value = "-";
                        }
                        cont++;
                    }
                    else
                    {
                        if (obj.OperationDate <= fechaInicioDT)
                        {
                            if (obj.Account.Currency == 0 && obj.Type == 0)
                            {
                                suma = suma + Convert.ToDecimal(obj.Income);
                            }
                            if (obj.Account.Currency == 0 && (obj.Type == 1 || obj.Type == 2))
                            {
                                suma = suma - Convert.ToDecimal(obj.Outcome);
                            }
                            if (obj.Account.Currency == 1 && obj.Type == 0)
                            {
                                suma = suma + Convert.ToDecimal(obj.Income);
                            }
                            if (obj.Account.Currency == 1 && (obj.Type == 1 || obj.Type == 2))
                            {
                                suma = suma - Convert.ToDecimal(obj.Outcome);
                            }
                        }
                    }
                }
                myDictionary.Add(item, suma);
            }

            cont = cont + 3;
            var cont_a = cont - 1;

            ws.Range("A" + cont_a, "B" + cont_a).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont_a, "B" + cont_a).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont_a, "B" + cont_a).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont_a, "B" + cont_a).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont_a).Value = "Cuenta";
            ws.Cell("B" + cont_a).Value = "Saldo Actual";

            foreach (KeyValuePair<string, decimal> kvp in myDictionary)
            {
                ws.Cell("A" + cont).Value = kvp.Key;
                ws.Cell("B" + cont).Value = kvp.Value;
                cont++;
            }

            ws.Columns("A", "T").AdjustToContents();
            return wb.Deliver("Reporte.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        }

        public async Task<IActionResult> ReportesPredios()
        {
            ViewBag.YearList = await _context.Operations.Select(m => m.Year).Distinct().ToListAsync();

            ViewBag.Customers = await _context.Operations
                .Where(m => m.OperationType == 4 && m.FatherOperation == 1)
                .Select(m => m.Customer).Distinct().ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportesPredios(String fechaInicio, String fechaFin, string AccountId, int Modality)
        {
            var fechaInicioDT = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);
            var fechaFinDT = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.AddWorksheet();
            int cont = 2;

            ws.Cell("A" + cont).Value = "Reporte de Predios";

            cont = cont + 2;

            ws.Range("A" + cont, "O" + cont).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont, "O" + cont).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont, "O" + cont).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont, "O" + cont).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont).Value = "Banco";
            ws.Cell("A" + cont).Value = "Banco";
            ws.Cell("B" + cont).Value = "Número de cuenta";
            ws.Cell("C" + cont).Value = "Tipo de cuenta";
            ws.Cell("D" + cont).Value = "Moneda";
            ws.Cell("E" + cont).Value = "Cliente";
            ws.Cell("F" + cont).Value = "Movimiento";
            ws.Cell("G" + cont).Value = "Concepto";
            ws.Cell("H" + cont).Value = "Descripción";
            ws.Cell("I" + cont).Value = "Modalidad";
            ws.Cell("J" + cont).Value = "Número";
            ws.Cell("K" + cont).Value = "Fecha";
            ws.Cell("L" + cont).Value = "Ingreso (S/)";
            ws.Cell("M" + cont).Value = "Salida (S/)";
            ws.Cell("N" + cont).Value = "Ingreso (USD)";
            ws.Cell("O" + cont).Value = "Salida (USD)";


            cont++;
            List<string> customers = new List<string>();
            Dictionary<string, decimal> myDictionary = new Dictionary<string, decimal>();
            if (AccountId == "none")
            {
                customers = await _context.Operations
                        .Where(m => m.OperationType == 4 && m.FatherOperation == 1)
                        .Select(m => m.Customer).Distinct().ToListAsync();
            }
            else
            {
                customers.Add(AccountId);
            }

            foreach (var item in customers)
            {
                var operation = _context.Operations.Include(m => m.Account).Where(m => m.Customer == item)
                    .Where(m => m.OperationType == 4 && m.FatherOperation == 1)
                    .ToList();

                decimal suma = 0;
                foreach (var obj in operation)
                {
                    if (obj.OperationDate >= fechaInicioDT && obj.OperationDate <= fechaFinDT)
                    {
                        ws.Cell("A" + cont).Value = obj.Account.Name;
                        ws.Cell("A" + cont).Value = obj.Account.Name;
                        ws.Cell("B" + cont).Value = obj.Account.AccountNumber;
                        ws.Cell("C" + cont).Value = obj.Account.AccountType;

                        if (obj.Account.Currency == 0)
                        {
                            ws.Cell("D" + cont).Value = "Soles";
                        }
                        else
                        {
                            ws.Cell("D" + cont).Value = "Dólares";
                        }

                        ws.Cell("E" + cont).Value = obj.Customer;

                        if (obj.Type == 0)
                        {
                            ws.Cell("F" + cont).Value = "Ingreso";
                        }
                        else if (obj.Type == 1)
                        {
                            ws.Cell("F" + cont).Value = "Egreso";
                        }
                        else if (obj.Type == 100)
                        {
                            ws.Cell("F" + cont).Value = "Cierre de mes";
                        }
                        else if (obj.Type == 200)
                        {
                            ws.Cell("F" + cont).Value = "Apertura de cuenta";
                        }

                        ws.Cell("G" + cont).Value = obj.Concept;
                        ws.Cell("H" + cont).Value = obj.Description;

                        if (obj.Modality == 0)
                        {
                            ws.Cell("I" + cont).Value = "Cheque";
                        }
                        else if (obj.Modality == 1)
                        {
                            ws.Cell("I" + cont).Value = "Transferencia";
                        }
                        else if (obj.Modality == 200)
                        {
                            ws.Cell("I" + cont).Value = "Apertura de cuenta";
                        }
                        ws.Cell("J" + cont).Value = obj.Number;
                        ws.Cell("K" + cont).Value = obj.OperationDate;

                        if (obj.Account.Currency == 0 && obj.Type == 0)
                        {
                            ws.Cell("L" + cont).Value = obj.Income;
                            suma = suma + Convert.ToDecimal(obj.Income);
                        }
                        else
                        {
                            ws.Cell("L" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 0 && (obj.Type == 1 || obj.Type == 2))
                        {
                            ws.Cell("M" + cont).Value = obj.Outcome;
                            suma = suma - Convert.ToDecimal(obj.Outcome);
                        }
                        else
                        {
                            ws.Cell("M" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 1 && obj.Type == 0)
                        {
                            ws.Cell("N" + cont).Value = obj.Income;
                            suma = suma + Convert.ToDecimal(obj.Income);
                        }
                        else
                        {
                            ws.Cell("N" + cont).Value = "-";
                        }

                        if (obj.Account.Currency == 1 && (obj.Type == 1 || obj.Type == 2))
                        {
                            ws.Cell("O" + cont).Value = obj.Outcome;
                            suma = suma - Convert.ToDecimal(obj.Outcome);
                        }
                        else
                        {
                            ws.Cell("O" + cont).Value = "-";
                        }
                        cont++;
                    }
                    else
                    {
                        if (obj.OperationDate <= fechaInicioDT)
                        {
                            if (obj.Account.Currency == 0 && obj.Type == 0)
                            {
                                suma = suma + Convert.ToDecimal(obj.Income);
                            }
                            if (obj.Account.Currency == 0 && (obj.Type == 1 || obj.Type == 2))
                            {
                                suma = suma - Convert.ToDecimal(obj.Outcome);
                            }
                            if (obj.Account.Currency == 1 && obj.Type == 0)
                            {
                                suma = suma + Convert.ToDecimal(obj.Income);
                            }
                            if (obj.Account.Currency == 1 && (obj.Type == 1 || obj.Type == 2))
                            {
                                suma = suma - Convert.ToDecimal(obj.Outcome);
                            }
                        }
                    }
                }
                myDictionary.Add(item, suma);
            }

            cont = cont + 3;
            var cont_a = cont - 1;

            ws.Range("A" + cont_a, "B" + cont_a).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189));
            ws.Range("A" + cont_a, "B" + cont_a).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            ws.Range("A" + cont_a, "B" + cont_a).Style.Border.SetOutsideBorderColor(XLColor.FromArgb(149, 179, 215));
            ws.Range("A" + cont_a, "B" + cont_a).Style.Font.SetFontColor(XLColor.White);

            ws.Cell("A" + cont_a).Value = "Cuenta";
            ws.Cell("B" + cont_a).Value = "Saldo Actual";

            foreach (KeyValuePair<string, decimal> kvp in myDictionary)
            {
                ws.Cell("A" + cont).Value = kvp.Key;
                ws.Cell("B" + cont).Value = kvp.Value;
                cont++;
            }

            ws.Columns("A", "T").AdjustToContents();
            return wb.Deliver("Reporte.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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