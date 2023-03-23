﻿using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autrisa.Models
{
    public partial class Operation
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        /// <summary>
        /// 0: Income, 1: Outcome, 2: Remaining
        /// </summary>
        [Display(Name = "Tipo")]
        public int Type { get; set; }
        /// <summary>
        /// 0: Check, 1: Transfer, 2...
        /// </summary>
        [Display(Name = "Modalidad")]
        public int Modality { get; set; }
        
        [Display(Name = "Número")]
        public int Number { get; set; }
        
        [Display(Name = "ID de la cuenta")]
        public int AccountId { get; set; }

        [Display(Name = "Fecha de operación")]
        public DateTime OperationDate { get; set; }
        
        [Display(Name = "Concepto")]
        public string? Concept { get; set; }
        
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Display(Name = "Ingreso")]
        public decimal? Income { get; set; }
        
        [Display(Name = "Salida")]
        public decimal? Outcome { get; set; }
        
        [Display(Name = "Año")]
        public int Year { get; set; }
        
        [Display(Name = "Mes")]
        public int Month { get; set; }
        
        [Display(Name = "Creado")]
        public DateTime Created { get; set; }
        
        [Display(Name = "Autor")]
        public int Author { get; set; }
        
        [Display(Name = "Modificado")]
        public DateTime? Modified { get; set; }
        
        [Display(Name = "Editor")]
        public int? Editor { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
