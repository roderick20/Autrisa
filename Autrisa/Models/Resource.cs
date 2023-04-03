using System;
using System.Collections.Generic;

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
        public string Title { get; set; } = null!;
        public string? Area { get; set; }
        public string Controller { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string? Attributes { get; set; }
        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual ICollection<RoleResource> RoleResources { get; set; }
    }
}
