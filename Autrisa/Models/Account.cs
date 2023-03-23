using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class Account
    {
        public Account()
        {
            Investments = new HashSet<Investment>();
            Lendings = new HashSet<Lending>();
            Operations = new HashSet<Operation>();
            Properties = new HashSet<Property>();
        }
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "-")]
        public Guid UniqueId { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;

        [Display(Name = "Tipo de cuenta")]
        public string AccountType { get; set; } = null!;

        [Display(Name = "Número de cuenta")]
        public string AccountNumber { get; set; } = null!;
        
        [Display(Name = "Moneda")]
        public int Currency { get; set; }
        
        [Display(Name = "Monto")]
        public decimal Amount { get; set; }
        
        [Display(Name = "Monto previo")]
        public decimal? PreviousRemaining { get; set; }
        
        [Display(Name = "Creado")]
        public DateTime Created { get; set; }
        
        [Display(Name = "Autor")]
        public int Author { get; set; }
        
        [Display(Name = "Modificado")]
        public DateTime? Modified { get; set; }
        
        [Display(Name = "Editor")]
        public int? Editor { get; set; }

        public virtual ICollection<Investment> Investments { get; set; }
        public virtual ICollection<Lending> Lendings { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
    }
}
