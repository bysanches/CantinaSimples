using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaSimples.Web.Models
{
    [Table("MovimentosEstoque")]
    public class MovimentoEstoque : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public virtual Produto Produto { get; set; }

        public int Quantidade { get; set; }

        public DateTime Data { get; set; }

        public string Observacao { get; set; }

        public MovimentoEstoque()
        {
            Data = DateTime.Now;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Quantidade == 0)
            {
                yield return new ValidationResult("A quantidade não pode ser zero.", new[] { "Quantidade" });
            }
        }
    }
}
