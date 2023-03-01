
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autrisa.Models
{
    public partial class Role
    {
        public Role()
        {
            RoleResources = new HashSet<RoleResource>();
        UserRoles = new HashSet<UserRole>();
        
        }


        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Name { get; set; }
               

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
        public virtual ICollection<UserRole> UserRoles { get; set; }
        
    }
}
