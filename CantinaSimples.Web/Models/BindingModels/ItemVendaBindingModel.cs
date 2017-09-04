using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CantinaSimples.Web.Models.BindingModels
{
    public class ItemVendaBindingModel
    {
        [Required]
        public int IdProduto { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public decimal Preco { get; set; }
    }
}