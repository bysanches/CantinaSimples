using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;

namespace CantinaSimples.Web.Models
{
    public class CantinaContext : IdentityDbContext<Usuario>
    {
        public CantinaContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static CantinaContext Create()
        {
            return new CantinaContext();
        }

        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Produto> Produtos { get; set; }
        public virtual DbSet<Venda> Vendas { get; set; }
        public virtual DbSet<Recarga> Recargas { get; set; }
        public virtual DbSet<MovimentoEstoque> MovimentosEstoque { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemVenda>()
                .HasRequired(x => x.Produto)
                .WithMany(x => x.ItensVendas)
                .WillCascadeOnDelete(false);
        }

    }
}