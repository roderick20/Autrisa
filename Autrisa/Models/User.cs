using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;

        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Display(Name = "Contraseña")]
        public string Password { get; set; } = null!;

        [Display(Name = "Último acceso")]
        public DateTime LastAccess { get; set; }

        [Display(Name = "Habilitado")]
        public bool Enabled { get; set; }
        
        public string? Role { get; set; }
        
        public DateTime Created { get; set; }
        public int? Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
