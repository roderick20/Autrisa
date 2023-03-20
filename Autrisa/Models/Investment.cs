using System;
using System.Collections.Generic;

namespace Autrisa.Models
{
    public partial class Investment
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        public DateTime OperationDate { get; set; }
        public int OperationType { get; set; }
        public decimal Amount { get; set; }
        public string Customer { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal OperationAmount { get; set; }
        public int AccountId { get; set; }
        public int Currency { get; set; }
        public int Author { get; set; }
        public DateTime Created { get; set; }
        public int? Editor { get; set; }
        public DateTime? Modified { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
