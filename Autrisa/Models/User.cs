using System;
using System.Collections.Generic;

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
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime LastAccess { get; set; }
        public bool Enabled { get; set; }
        public DateTime Created { get; set; }
        public int? Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
