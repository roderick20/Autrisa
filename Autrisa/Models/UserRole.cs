
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autrisa.Models
{
    public partial class UserRole
    {
        public UserRole()
        {
            
        }


        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        
        [Display(Name = "UserId")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public int UserId { get; set; }
        
        [Display(Name = "RoleId")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public int RoleId { get; set; }
               

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

        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
        
    }
}
