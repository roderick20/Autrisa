
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autrisa.Models
{
    public partial class Account
    {
        public Account()
        {
            Operations = new HashSet<Operation>();
        
        }


        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Name { get; set; }
        
        [Display(Name = "Tipo de cuenta")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string AccountType { get; set; }
        
        [Display(Name = "Número de cuenta")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string AccountNumber { get; set; }
        
        [Display(Name = "Moneda")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public int Currency { get; set; }
        
        [Display(Name = "Monto")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public decimal Amount { get; set; }

        [Display(Name = "Saldo del mes anterior")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public decimal? PreviousRemaining { get; set; }


        //------------Auth--------------------
        [Display(Name = "Creado")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Created { get; set; }

        [Display(Name = "Autor")]
        public int Author { get; set; }

        [Display(Name = "Modificado")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Modified { get; set; }

        [Display(Name = "Editor")]
        public int? Editor { get; set; }

        [Display(Name = "Autor")]
        [NotMapped]
        public string AuthorName { get; set; }

        [Display(Name = "Editor")]
        [NotMapped]
        public string? EditorName { get; set; }
        //--------------------Auth--------------------

        public virtual ICollection<Operation> Operations { get; set; }
        
    }
}
