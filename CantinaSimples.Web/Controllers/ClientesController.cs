using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CantinaSimples.Web.Models;
using CantinaSimples.Web.Models.BindingModels;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CantinaSimples.Web.Controllers
{
    public class ClientesController : BaseApiController
    {
        private CantinaContext db;

        public ClientesController()
        {
            db = new CantinaContext();
        }

        public ClientesController(CantinaContext context)
        {
            db = context;
        }

        // GET: api/Clientes
        [Authorize]
        [ResponseType(typeof(Cliente))]
        public async Task<IEnumerable<Cliente>> GetClientesAsync(string nome = null, string email = null, string documento = null)
        {
            IQueryable<Cliente> clientes = db.Clientes;

            if (!String.IsNullOrEmpty(nome))
            {
                clientes = clientes.Where(c => c.Nome.ToUpper().Contains(nome.ToUpper()));
            }

            if (!String.IsNullOrEmpty(email))
            {
                clientes = clientes.Where(c => c.Email == email);
            }

            if (!String.IsNullOrEmpty(documento))
            {
                clientes = clientes.Where(c => c.Documento == documento);
            }

            Usuario user = await this.AppUserManager.FindByEmailAsync(User.Identity.Name);
            IdentityRole role = await this.AppRoleManager.FindByIdAsync(user.Roles.FirstOrDefault().RoleId);
            if (role.Name == "Responsavel")
            {
                clientes = clientes.Where(c => c.Responsavel.Id == user.Id);
            }

            return await clientes.ToArrayAsync<Cliente>();
        }

        [Authorize(Roles = "Administrador,Gerente,Responsavel")]
        [Route("api/clientes/{id}/restricoes")]
        public async Task<IHttpActionResult> GetRestricoes(int id)
        {
            Cliente cliente = await db.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            var ids = cliente.Restricoes.Select(p => p.Id);

            return Ok(ids);
        }

        [Authorize(Roles = "Administrador,Gerente,Responsavel")]
        [Route("api/clientes/{id}/restricoes/{idProduto}")]
        public async Task<IHttpActionResult> PostRestricao(int id, int idProduto)
        {
            Cliente cliente = await db.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            Produto produto = await db.Produtos.FindAsync(idProduto);
            if (produto == null)
            {
                return NotFound();
            }

            Usuario user = await this.AppUserManager.FindByEmailAsync(User.Identity.Name);
            IdentityRole role = await this.AppRoleManager.FindByIdAsync(user.Roles.FirstOrDefault().RoleId);
            if (role.Name == "Responsavel" && cliente.Responsavel.Id != user.Id)
            {
                return Unauthorized();
            }

            cliente.Restricoes.Add(produto);
            await db.SaveChangesAsync();

            return Created(String.Format("/api/clientes/{0}/restricoes", id), cliente.Restricoes.Select(p => p.Id));
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [Route("api/clientes/{id}/restricoes/{idProduto}")]
        public async Task<IHttpActionResult> DeleteRestricao(int id, int idProduto)
        {
            Cliente cliente = await db.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            Produto produto = await db.Produtos.FindAsync(idProduto);
            if (produto == null)
            {
                return NotFound();
            }

            cliente.Restricoes.Remove(produto);
            await db.SaveChangesAsync();

            return Ok();
        }

        // GET: api/Clientes/5
        [Authorize(Roles = "Administrador,Gerente")]
        [ResponseType(typeof(Cliente))]
        public async Task<IHttpActionResult> GetCliente(int id)
        {
            Cliente cliente = await db.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        // PUT: api/Clientes/5
        [Authorize(Roles = "Administrador,Gerente")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCliente(int id, ClienteBindingModel clienteModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != clienteModel.Id)
            {
                return BadRequest();
            }

            try
            {
                Cliente cliente = db.Clientes.Find(clienteModel.Id);

                cliente.Documento = clienteModel.Documento;
                cliente.Email = clienteModel.Email;
                cliente.EmailResponsavel = clienteModel.EmailResponsavel;
                cliente.Nascimento = clienteModel.Nascimento;
                cliente.Nome = clienteModel.Nome;
                cliente.NomeResponsavel = clienteModel.NomeResponsavel;
                cliente.Telefone = clienteModel.Telefone;
                cliente.TelefoneResponsavel = clienteModel.TelefoneResponsavel;

                if (clienteModel.IdResponsavel.HasValue)
                {
                    Usuario user = await db.Users.FirstAsync(u => u.Id == clienteModel.IdResponsavel.Value.ToString());

                    if (user != null)
                    {
                        cliente.Responsavel = user;
                    }
                }

                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!ClienteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Clientes
        [Authorize(Roles = "Administrador,Gerente")]
        [ResponseType(typeof(Cliente))]
        public async Task<IHttpActionResult> PostCliente(ClienteBindingModel clienteModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Cliente cliente = new Cliente()
            {
                Documento = clienteModel.Documento,
                Email = clienteModel.Email,
                EmailResponsavel = clienteModel.EmailResponsavel,
                Nascimento = clienteModel.Nascimento,
                Nome = clienteModel.Nome,
                NomeResponsavel = clienteModel.NomeResponsavel,
                Telefone = clienteModel.Telefone,
                TelefoneResponsavel = clienteModel.TelefoneResponsavel
            };

            if (clienteModel.IdResponsavel.HasValue)
            {
                Usuario user = await db.Users.FirstAsync(u => u.Id == clienteModel.IdResponsavel.Value.ToString());

                if (user != null)
                {
                    cliente.Responsavel = user;
                }
            }

            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = cliente.Id }, cliente);
        }

        // DELETE: api/Clientes/5
        [Authorize(Roles = "Administrador,Gerente")]
        [ResponseType(typeof(Cliente))]
        public async Task<IHttpActionResult> DeleteCliente(int id)
        {
            Cliente cliente = await db.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            try
            {
                db.Clientes.Remove(cliente);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Cliente não pode ser excluído pois já há vendas para esse cliente."));
            }

            return Ok(cliente);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [Route("api/clientes/recalcularSaldo/{idCliente}")]
        public async Task<IHttpActionResult> RecalcularSaldo(int IdCliente)
        {
            Cliente cliente = await db.Clientes.FindAsync(IdCliente);

            if (cliente == null)
            {
                throw new Exception("Cliente não encontrado.");
            }

            decimal saldo = 0;

            saldo -= cliente.Vendas.Where(v => v.FormaPagamento == FormaPagamento.PrePago).Sum(v => v.Total);
            saldo += cliente.Recargas.Sum(r => r.Valor);

            cliente.Saldo = saldo;

            await db.SaveChangesAsync();
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClienteExists(int id)
        {
            return db.Clientes.Count(e => e.Id == id) > 0;
        }
    }
}