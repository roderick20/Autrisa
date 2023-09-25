using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class Operation
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        /// <summary>
        /// 0: Income, 1: Outcome, 2: Remaining
        /// </summary>
        
        [Display(Name = "Tipo")]
        public int Type { get; set; }
        /// <summary>
        /// 0: Check, 1: Transfer, 2...
        /// </summary>
        
        [Display(Name = "Modalidad")]
        public int Modality { get; set; }
        
        [Display(Name = "Número")]
        public string? Number { get; set; }
        
        [Display(Name = "Cuenta")]
        public int? AccountId { get; set; }

        [Display(Name = "Fecha de operación")]
        public DateTime OperationDate { get; set; }
        
        [Display(Name = "Concepto")]
        public string? Concept { get; set; }
        
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Display(Name = "Ingreso")]
        public decimal? Income { get; set; }
        
        [Display(Name = "Salida")]
        public decimal? Outcome { get; set; }
        
        [Display(Name = "Año")]
        public int Year { get; set; }
        
        [Display(Name = "Mes")]
        public int Month { get; set; }

        [Display(Name = "Clase de operación")]
        public int? OperationType { get; set; }

        [Display(Name = "Cliente")]
        public string? Customer { get; set; }

        [Display(Name = "Saldo inicial")]
        public decimal? InitialBalance { get; set; }

        [Display(Name = "Saldo actual")]
        public decimal? ActualBalance { get; set; }

        [Display(Name = "Saldo final")]
        public decimal? FinalBalance { get; set; }

        public int? FatherOperation { get; set; }

        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
