using System;
using System.Collections.Generic;

namespace Autrisa.Models
{
    public partial class Setting
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        public string Title { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public int Type { get; set; }
        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }
    }
}
