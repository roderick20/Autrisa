using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class AccountDetail
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }

        [Display(Name = "Cliente")]
        public string Customer { get; set; } = null!;

        [Display(Name = "Monto inicial")]
        public decimal InitialAmount { get; set; }

        [Display(Name = "Fecha de operación")]
        public DateTime OperationDate { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;

        [Display(Name = "Concepto")]
        public string Concept { get; set; } = null!;

        [Display(Name = "Monto en soles")]
        public decimal? SolesAmount { get; set; } = null!;
        
        [Display(Name = "Monto en dólares")]
        public decimal? DollarsAmount { get; set; } = null!;

        [Display(Name = "Cuenta")]
        public int AccountId { get; set; }
        
        [Display(Name = "Tipo de operación")]
        public int? OperationType { get; set; }

        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
