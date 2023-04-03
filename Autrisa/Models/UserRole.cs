using System;
using System.Collections.Generic;

namespace Autrisa.Models
{
    public partial class UserRole
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
