using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CantinaSimples.Web.Models.BindingModels
{
    public class RecargaBindingModel
    {
        [Required]
        public int IdCliente { get; set; }

        [Required]
        public decimal Valor { get; set; }
    }
}