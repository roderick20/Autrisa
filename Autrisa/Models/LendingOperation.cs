using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class LendingOperation
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }

        [Display(Name = "Tipo")]
        public int Type { get; set; }

        [Display(Name = "Modalidad")]
        public int Modality { get; set; }
        
        [Display(Name = "ID del préstamo")]
        public int LendingId { get; set; }

        [Display(Name = "Fecha de operación")]
        public DateTime OperationDate { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;
        
        [Display(Name = "Monto")]
        public decimal Amount { get; set; }
        
        [Display(Name = "Creado")]
        public DateTime Created { get; set; }
        
        [Display(Name = "Autor")]
        public int Author { get; set; }
        
        [Display(Name = "Modificado")]
        public DateTime? Modified { get; set; }
        
        [Display(Name = "Editor")]
        public int? Editor { get; set; }

        public virtual Lending Lending { get; set; } = null!;
    }
}
