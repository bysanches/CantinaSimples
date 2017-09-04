using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CantinaSimples.Web.Models
{
    public class Recarga
    {
        public int Id { get; set; }

        public decimal Valor { get; set; }

        public DateTime Data { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual Cliente Cliente { get; set; }
    }
}