using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class Investment
    {
        public Investment()
        {
            InvestmentsOperations = new HashSet<InvestmentsOperation>();
        }

        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "-")]
        public Guid UniqueId { get; set; }
        
        [Display(Name = "Fecha de operación")]
        public DateTime OperationDate { get; set; }
        
        [Display(Name = "Tipo de operación")]
        public int OperationType { get; set; }
        
        [Display(Name = "Monto")]
        public decimal Amount { get; set; }
        
        [Display(Name = "Cliente")]
        public string Customer { get; set; } = null!;

        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;
        
        [Display(Name = "Monto de Operación")]
        public decimal OperationAmount { get; set; }
        
        [Display(Name = "ID de la cuenta")]
        public int AccountId { get; set; }
        
        [Display(Name = "Moneda")]
        public int Currency { get; set; }
        
        [Display(Name = "Autor")]
        public int Author { get; set; }
        
        [Display(Name = "Creado")]
        public DateTime Created { get; set; }
        
        [Display(Name = "Editor")]
        public int? Editor { get; set; }
        
        [Display(Name = "Modificado")]
        public DateTime? Modified { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<InvestmentsOperation> InvestmentsOperations { get; set; }
    }
}
