using System;
using System.Collections.Generic;

namespace Autrisa.Models
{
    public partial class Operation
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        /// <summary>
        /// 0: Income, 1: Outcome, 2: Remaining
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 0: Check, 1: Transfer, 2...
        /// </summary>
        public int Modality { get; set; }
        public int Number { get; set; }
        public int AccountId { get; set; }
        public DateTime OperationDate { get; set; }
        public string? Concept { get; set; }
        public string? Description { get; set; }
        public decimal? Income { get; set; }
        public decimal? Outcome { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
