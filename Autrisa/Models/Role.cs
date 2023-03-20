using System;
using System.Collections.Generic;

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
        public string Name { get; set; } = null!;
        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual ICollection<RoleResource> RoleResources { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
