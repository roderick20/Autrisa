using System;
using System.Collections.Generic;

namespace Autrisa.Models
{
    public partial class Property
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        public string Address { get; set; } = null!;
        public string Number { get; set; } = null!;
        public decimal Participation { get; set; }
        public string Description { get; set; } = null!;
        public string Receptor { get; set; } = null!;
        public decimal Amount { get; set; }
        public int Currency { get; set; }
        public decimal AccountId { get; set; }
        public int Author { get; set; }
        public DateTime Created { get; set; }
        public int? Editor { get; set; }
        public DateTime? Modified { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
