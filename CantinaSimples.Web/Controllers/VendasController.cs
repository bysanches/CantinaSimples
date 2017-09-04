using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CantinaSimples.Web.Models;
using CantinaSimples.Web.Models.BindingModels;
using CantinaSimples.Web.Services;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CantinaSimples.Web.Controllers
{
    [Authorize]
    public class VendasController : BaseApiController
    {
        private CantinaContext db = new CantinaContext();
        private EstoqueService estoqueService;

        public VendasController()
        {
            estoqueService = new EstoqueService(db);
        }

        [Authorize(Roles = "Administrador,Gerente,Responsavel")]
        // GET: api/Vendas
        public async Task<IEnumerable<Venda>> GetVendas(string nomeCliente = null, DateTime? dataDe = null, DateTime? dataAte = null)
        {
            IQueryable<Venda> vendas = db.Vendas;

            if (!string.IsNullOrEmpty(nomeCliente))
            {
                vendas = vendas.Where(v => v.Cliente.Nome.ToUpper().Contains(nomeCliente.ToUpper()));
            }

            if (dataDe.HasValue)
            {
                vendas = vendas.Where(v => DbFunctions.TruncateTime(v.Data) >= DbFunctions.TruncateTime(dataDe));
            }

            if (dataAte.HasValue)
            {
                vendas = vendas.Where(v => DbFunctions.TruncateTime(v.Data) <= DbFunctions.TruncateTime(dataAte));
            }

            Usuario user = await this.AppUserManager.FindByEmailAsync(User.Identity.Name);
            IdentityRole role = await this.AppRoleManager.FindByIdAsync(user.Roles.FirstOrDefault().RoleId);
            if (role.Name == "Responsavel")
            {
                vendas = vendas.Where(v => v.Cliente.Responsavel.Id == user.Id);
            }

            return await vendas.ToArrayAsync<Venda>();
        }

        [Authorize(Roles = "Administrador,Gerente")]
        // GET: api/Vendas/5
        [ResponseType(typeof(Venda))]
        public async Task<IHttpActionResult> GetVenda(int id)
        {
            Venda venda = await db.Vendas.FindAsync(id);
            if (venda == null)
            {
                return NotFound();
            }

            return Ok(venda);
        }

        // POST: api/Vendas
        [Authorize(Roles = "Administrador,Gerente,Atendente")]
        [ResponseType(typeof(Venda))]
        public async Task<IHttpActionResult> PostVenda(VendaBindingModel vendaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Usuario user = await db.Users.FirstAsync(u => u.Email == User.Identity.Name);

            Venda venda = new Venda()
            {
                Atendente = user,
                Data = DateTime.Now,
                FormaPagamento = vendaModel.FormaPagamento
            };

            if (vendaModel.IdCliente.HasValue)
            {
                Cliente cliente = await db.Clientes.FindAsync(vendaModel.IdCliente);

                if (cliente != null)
                    venda.Cliente = cliente;
            }
            foreach (var itemModel in vendaModel.Itens)
            {
                Produto produto = await db.Produtos.FindAsync(itemModel.IdProduto);

                if (venda.Cliente != null)
                {
                    if (venda.Cliente.Restricoes.Count > 0 && venda.Cliente.Restricoes.Select(p => p.Id).Contains(itemModel.IdProduto))
                    {
                        throw new Exception("Cliente não pode consumir o produto " + produto.Nome);
                    }
                }

                ItemVenda item = new ItemVenda()
                {
                    Preco = itemModel.Preco,
                    Produto = produto,
                    Quantidade = itemModel.Quantidade
                };

                venda.Itens.Add(item);
                estoqueService.CriarMovimento(item);
            }

            if (venda.Cliente != null && vendaModel.FormaPagamento == FormaPagamento.PrePago)
            {
                venda.Cliente.Saldo = venda.Cliente.Saldo - venda.Total;
            }

            db.Vendas.Add(venda);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = venda.Id }, venda);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        // DELETE: api/Vendas/5
        [ResponseType(typeof(Venda))]
        public async Task<IHttpActionResult> DeleteVenda(int id)
        {
            Venda venda = await db.Vendas.FindAsync(id);
            if (venda == null)
            {
                return NotFound();
            }

            foreach (var item in venda.Itens)
            {
                if (item.MovimentoEstoque != null)
                {
                    estoqueService.RemoverMovimento(item.MovimentoEstoque);
                }
            }
            db.Vendas.Remove(venda);
            await db.SaveChangesAsync();

            return Ok(venda);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VendaExists(int id)
        {
            return db.Vendas.Count(e => e.Id == id) > 0;
        }
    }
}