using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CantinaSimples.Web.Models.BindingModels
{
    public class ClienteBindingModel
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

        public Guid? IdResponsavel { get; set; }
    }
}