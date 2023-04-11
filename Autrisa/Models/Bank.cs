using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class Bank
    {
        public Bank()
        {
            Accounts = new HashSet<Account>();
        }

        public int Id { get; set; }
        public Guid UniqueId { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        public int Author { get; set; }
        public DateTime Created { get; set; }
        public int? Editor { get; set; }
        public DateTime? Modified { get; set; }

        //public virtual Account Account { get; set; } = null!;
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
