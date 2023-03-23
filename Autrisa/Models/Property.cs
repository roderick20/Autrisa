using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class Property
    {
        public Property()
        {
            PropertiesOperations = new HashSet<PropertiesOperation>();
        }

        public int Id { get; set; }
        public Guid UniqueId { get; set; }

        [Display(Name = "Dirección")]
        public string Address { get; set; } = null!;

        [Display(Name = "Número")]
        public string Number { get; set; } = null!;

        [Display(Name = "Participación")]
        public decimal Participation { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;

        [Display(Name = "Receptor")]
        public string Receptor { get; set; } = null!;

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Moneda")]
        public int Currency { get; set; }

        [Display(Name = "ID de la cuenta")]
        public int AccountId { get; set; }

        [Display(Name = "Autor")]
        public int Author { get; set; }

        [Display(Name = "Creado")]
        public DateTime Created { get; set; }

        [Display(Name = "Editor")]
        public int? Editor { get; set; }

        [Display(Name = "Modificado")]
        public DateTime? Modified { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<PropertiesOperation> PropertiesOperations { get; set; }
    }
}
