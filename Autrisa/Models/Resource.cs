
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autrisa.Models
{
    public partial class Resource
    {
        public Resource()
        {
            RoleResources = new HashSet<RoleResource>();
        
        }


        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        
        [Display(Name = "Title")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Title { get; set; }
        
        [Display(Name = "Area")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Area { get; set; }
        
        [Display(Name = "Controller")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Controller { get; set; }
        
        [Display(Name = "Action")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Action { get; set; }
        
        [Display(Name = "Attributes")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Attributes { get; set; }
               

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

        public virtual ICollection<RoleResource> RoleResources { get; set; }
        
    }
}
