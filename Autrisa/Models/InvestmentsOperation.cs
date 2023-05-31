using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class InvestmentsOperation
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }

        [Display(Name = "Tipo")]
        public int Type { get; set; }

        [Display(Name = "Modalidad")]
        public int Modality { get; set; }

        [Display(Name = "Inversión")]
        public int OperationId { get; set; }

        [Display(Name = "Fecha de operación")]
        public DateTime OperationDate { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;

        [Display(Name = "Monto")]
        public decimal Amount { get; set; }
        
        [Display(Name = "Monto")]
        public decimal? FinalAmount { get; set; }
        
        
        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual Operation Operation { get; set; } = null!;
    }
}
