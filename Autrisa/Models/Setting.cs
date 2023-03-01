using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Autrisa.Models
{
    public partial class Setting
    {
        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Title { get; set; }

        public string Key { get; set; }

        [Display(Name = "Valor")]
        [Required(ErrorMessage = "Campo es obligatorio")]
        public string Value { get; set; }

        public int Type { get; set; }

        //--------------------Auth--------------------
        [Display(Name = "Creado")]
        public DateTime Created { get; set; }

        [Display(Name = "Autor")]
        public int Author { get; set; }

        [Display(Name = "Modificado")]
        public DateTime? Modified { get; set; }

        [Display(Name = "Editor")]
        public int? Editor { get; set; }

        [NotMapped]
        public string AuthorName { get; set; }

        [NotMapped]
        public string? EditorName { get; set; }
        //--------------------Auth--------------------
    }
}