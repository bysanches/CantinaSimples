namespace CantinaSimples.Web.Migrations
{
    using CantinaSimples.Web.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Diagnostics;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CantinaSimples.Web.Models.CantinaContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CantinaSimples.Web.Models.CantinaContext context)
        {
            var coxinha = new Produto { Nome = "Coxinha", Descricao = "Coxinha de massa de batata com recheio de frango", Preco = 4.5M /* quem entendeu me add */ };
            var cocacola = new Produto { Nome = "Coca-cola lata", Descricao = "Coca-cola de lata 360ml", Preco = 4M };
            var twix = new Produto { Nome = "Twix", Descricao = "Biscoito, caramelo e chocolate", Preco = 1.5M };
            var halls = new Produto { Nome = "Halls", Descricao = "Bala dura", Preco = 2M };

            context.Produtos.AddOrUpdate(p => p.Nome,
                coxinha,
                cocacola,
                twix,
                halls
            );

            context.MovimentosEstoque.AddOrUpdate(m => m.Data,
                new MovimentoEstoque { Produto = coxinha, Quantidade = 10, Data = new DateTime(2015, 10, 27, 09, 15, 22) },
                new MovimentoEstoque { Produto = coxinha, Quantidade = -1, Data = new DateTime(2015, 10, 27, 09, 17, 35) },
                new MovimentoEstoque { Produto = coxinha, Quantidade = -1, Data = new DateTime(2015, 10, 27, 09, 18, 02) },
                new MovimentoEstoque { Produto = coxinha, Quantidade = -1, Data = new DateTime(2015, 10, 27, 09, 19, 20) }
            );
            coxinha.Saldo = coxinha.CalcularSaldo();

            context.Clientes.AddOrUpdate(c => c.Documento,
                new Cliente { Nome = "Bruno", Nascimento = new DateTime(1991, 10, 15), Documento = "397.292.257-45" },
                new Cliente { Nome = "Dionisio", Nascimento = new DateTime(1992, 06, 13), Documento = "525.135.464-90" },
                new Cliente { Nome = "Rafael", Nascimento = new DateTime(1992, 01, 01), Documento = "674.231.384-60" }
            );

            var manager = new UserManager<Usuario>(new UserStore<Usuario>(context));

            var user = new Usuario()
            {
                UserName = "admin@cantinasimples.com.br",
                Email = "admin@cantinasimples.com.br",
                EmailConfirmed = true,
                FirstName = "Administrador",
                LastName = "Cantina"
            };

            manager.Create(user, "C@ntina13");

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (roleManager.Roles.Count() == 0)
            {
                roleManager.Create(new IdentityRole { Name = "Administrador" });
                roleManager.Create(new IdentityRole { Name = "Gerente" });
                roleManager.Create(new IdentityRole { Name = "Atendente" });
                roleManager.Create(new IdentityRole { Name = "Responsavel" });
            }

            var adminUser = manager.FindByEmail("admin@cantinasimples.com.br");

            manager.AddToRoles(adminUser.Id, new string[] { "Administrador" });
        }
    }
}
