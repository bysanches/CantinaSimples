using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaSimples.Web.Models.ViewModels
{
    public class MovimentoEstoqueViewModel
    {
        public int Id { get; set; }
        public ProdutoViewModel Produto { get; set; }
        public int Quantidade { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; }

        public class ProdutoViewModel
        {
            public int Id { get; set; }
            public string Nome { get; set; }
        }
    }
}
