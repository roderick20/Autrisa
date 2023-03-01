
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autrisa.Models
{
    public partial class Operation
    {
        public Operation()
        {
            
        }


        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        
        [Display(Name = "-")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public int Type { get; set; }
        
        [Display(Name = "-")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public int Modality { get; set; }
        
        [Display(Name = "-")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public int Number { get; set; }
        
        [Display(Name = "-")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public int AccountId { get; set; }
        
        [Display(Name = "-")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public DateTime OperationDate { get; set; }
        
        [Display(Name = "-")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Concept { get; set; }
        
        [Display(Name = "-")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Description { get; set; }
        
        [Display(Name = "-")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public decimal Income { get; set; }
        
        [Display(Name = "-")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public decimal Outcome { get; set; }
        
        [Display(Name = "-")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public int Year { get; set; }
        
        [Display(Name = "-")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public int Month { get; set; }
               

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

        public virtual Account Account { get; set; }
        
    }
}
