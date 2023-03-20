using System;
using System.Collections.Generic;

namespace Autrisa.Models
{
    public partial class Lending
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        public DateTime LendDate { get; set; }
        public string Customer { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public int MovementType { get; set; }
        public int Currency { get; set; }
        public int AccountId { get; set; }
        public int Author { get; set; }
        public DateTime Created { get; set; }
        public int? Editor { get; set; }
        public DateTime? Modified { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
