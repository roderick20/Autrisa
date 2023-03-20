using System;
using System.Collections.Generic;

namespace Autrisa.Models
{
    public partial class Account
    {
        public Account()
        {
            Operations = new HashSet<Operation>();
        }

        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        public string Name { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        /// <summary>
        /// 0: Sol, 1: Dollar
        /// </summary>
        public int Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal? PreviousRemaining { get; set; }
        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual ICollection<Investment> Investments { get; set; }
        public virtual ICollection<Lending> Lendings { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
    }
}
