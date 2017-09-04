using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CantinaSimples.Web.Models.BindingModels
{
    public class VendaBindingModel
    {
        [Required]
        public FormaPagamento FormaPagamento { get; set; }

        public int? IdCliente { get; set; }

        public ICollection<ItemVendaBindingModel> Itens { get; set; }
    }
}