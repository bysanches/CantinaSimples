using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaSimples.Web.Models
{
    public class Venda
    {
        public Venda()
        {
            this.Itens = new List<ItemVenda>();
        }

        public int Id { get; set; }
        public DateTime Data { get; set; }
        public FormaPagamento FormaPagamento { get; set; }

        public virtual ICollection<ItemVenda> Itens { get; set; }
        
        public virtual Cliente Cliente { get; set; }

        [Required]
        public virtual Usuario Atendente { get; set; }

        public decimal Total
        {
            get
            {
                return Itens.Sum(item => item.Subtotal);
            }
        }
    }
}
