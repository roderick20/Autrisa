using System;
using System.Collections.Generic;

namespace Autrisa.Models
{
    public partial class RoleResource
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        public int RoleId { get; set; }
        public int ResourceId { get; set; }
        public string Title { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string Controller { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string Attributes { get; set; } = null!;
        public bool Anonymus { get; set; }
        public int? ParentId { get; set; }
        public string? Icon { get; set; }
        public int Orden { get; set; }
        public int MenuAction { get; set; }
        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual Resource Resource { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
