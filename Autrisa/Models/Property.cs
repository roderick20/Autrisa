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

        [Display(Name = "Rentabilidad")]
        public decimal Amount { get; set; }

        [Display(Name = "Moneda")]
        public int Currency { get; set; }

        [Display(Name = "Cuenta")]
        public int AccountId { get; set; }

        [Display(Name = "Monto en Soles")]
        public decimal? SolesAmount { get; set; }

        [Display(Name = "Monto en Dólares")]
        public decimal? DollarsAmount { get; set; }

        [Display(Name = "Ingreso")]
        public decimal? Income{ get; set; }

        [Display(Name = "Salida")]
        public decimal? Outcome{ get; set; }


        public int Author { get; set; }
        public DateTime Created { get; set; }
        public int? Editor { get; set; }
        public DateTime? Modified { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<PropertiesOperation> PropertiesOperations { get; set; }
    }
}
