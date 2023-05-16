using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class Client
    {
        public Client()
        {
            
        }

        public int Id { get; set; }
        public Guid UniqueId { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

    }
}
