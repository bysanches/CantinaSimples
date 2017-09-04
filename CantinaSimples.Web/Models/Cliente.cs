using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaSimples.Web.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        
        [Required]
        public string Nome { get; set; }
        public DateTime? Nascimento { get; set; }
        public string Documento { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        public string Telefone { get; set; }
        public string NomeResponsavel { get; set; }

        [StringLength(20)]
        public string TelefoneResponsavel { get; set; }

        [EmailAddress]
        public string EmailResponsavel { get; set; }

        public virtual Usuario Responsavel { get; set; }

        public virtual ICollection<Produto> Restricoes { get; set; }

        public virtual ICollection<Recarga> Recargas { get; set; }

        public virtual ICollection<Venda> Vendas { get; set; }
                
        public decimal Saldo { get; set; }

        public Cliente()
        {
            Restricoes = new HashSet<Produto>();
            Saldo = 0;
        }
    }
}
