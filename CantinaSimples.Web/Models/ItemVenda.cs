using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaSimples.Web.Models
{
    public class ItemVenda
    {
        public int Id { get; set; }

        [Required]
        public virtual Produto Produto { get; set; }

        [Min(1)]
        public int Quantidade { get; set; }

        [Min(0)]
        public decimal Preco { get; set; }

        /// <summary>
        /// Movimento de estoque gerado por esse item da venda.
        /// </summary>
        public virtual MovimentoEstoque MovimentoEstoque { get; set; }

        public decimal Subtotal
        {
            get
            {
                return Quantidade * Preco;
            }
        }
    }
}
