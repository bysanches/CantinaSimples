using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaSimples.Web.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }
        public string Descricao { get; set; }

        [Min(0)]
        public decimal Preco { get; set; }

        [DataType(DataType.ImageUrl)]
        public string CaminhoImagem { get; set; }
        public virtual ICollection<Cliente> Restricoes { get; set; }
        public virtual ICollection<ItemVenda> ItensVendas { get; set; }
        public virtual ICollection<MovimentoEstoque> MovimentosEstoque { get; set; }
        public int Saldo { get; set; }

        public int CalcularSaldo()
        {
            return MovimentosEstoque.Sum(m => m.Quantidade);
        }
    }
}
