using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class Account
    {
        public Account()
        {
            //Investments = new HashSet<Investment>();
            //Lendings = new HashSet<Lending>();
            Operations = new HashSet<Operation>();
            //Properties = new HashSet<Property>();
        }

        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;

        [Display(Name = "Tipo de cuenta")]
        public string AccountType { get; set; } = null!;

        [Display(Name = "Número de cuenta")]
        public string AccountNumber { get; set; } = null!;
        /// <summary>
        /// 0: Sol, 1: Dollar
        /// </summary>

        [Display(Name = "Moneda")]
        public int Currency { get; set; }
        
        [Display(Name = "Tipo de movimientos")]
        public int OperationType { get; set; }
        /// <summary>
        /// 0: Normal operation, 1: Lending, 2: Investment, 3: Property
        /// </summary>

        [Display(Name = "Banco")]
        public int BankId { get; set; }

        [Display(Name = "Monto")]
        public decimal Amount { get; set; }

        [Display(Name = "Monto previo")]
        public decimal? PreviousRemaining { get; set; }

        public DateTime Created { get; set; }
        public int Author { get; set; }
        public DateTime? Modified { get; set; }
        public int? Editor { get; set; }
        public int? Visible { get; set; }

        public DateTime DateAccount { get; set; }
        

        //public virtual ICollection<Investment> Investments { get; set; }
        //public virtual ICollection<Lending> Lendings { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        //public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<AccountDetail> AccountDetails { get; set; }
        public virtual ICollection<Bank> Banks { get; set; }
    }
}
