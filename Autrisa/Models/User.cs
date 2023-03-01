
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autrisa.Models
{
    public partial class User
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
        
        }


        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Name { get; set; }
        
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Email { get; set; }
        
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Password { get; set; }
        
        [Display(Name = "LastAccess")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public DateTime LastAccess { get; set; }
        
        [Display(Name = "Enabled")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public bool Enabled { get; set; }
               

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

        [NotMapped]
        public string? Roles { get; set; }

        //--------------------Auth--------------------

        public virtual ICollection<UserRole> UserRoles { get; set; }
        
    }
}
