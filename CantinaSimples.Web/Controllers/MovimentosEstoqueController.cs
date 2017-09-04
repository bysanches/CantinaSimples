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
using CantinaSimples.Web.Services;
using CantinaSimples.Web.Models.ViewModels;

namespace CantinaSimples.Web.Controllers
{
    [Authorize(Roles = "Administrador,Gerente")]
    public class MovimentosEstoqueController : ApiController
    {
        private CantinaContext db;
        private EstoqueService service;

        public MovimentosEstoqueController()
        {
            db = new CantinaContext();
            service = new EstoqueService(db);
        }

        public MovimentosEstoqueController(CantinaContext context)
        {
            db = context;
            service = new EstoqueService(db);
        }

        // GET: api/MovimentosEstoque
        public async Task<IEnumerable<MovimentoEstoqueViewModel>> GetMovimentosEstoque()
        {
            return await db.MovimentosEstoque.OrderByDescending(m => m.Data)
                .Select(m =>
                    new MovimentoEstoqueViewModel
                    {
                        Id = m.Id,
                        Produto = new MovimentoEstoqueViewModel.ProdutoViewModel
                        {
                            Id = m.Produto.Id,
                            Nome = m.Produto.Nome
                        },
                        Quantidade = m.Quantidade,
                        Data = m.Data,
                        Observacao = m.Observacao
                    }).ToListAsync();
        }

        [Route("api/movimentosestoque/paged")]
        public async Task<object> GetPagedMovimentosEstoque(int page = 1, int pageSize = 10)
        {
            var movimentos = db.MovimentosEstoque.OrderByDescending(m => m.Data);
            var paged = await movimentos.Skip(pageSize * (page - 1)).Take(pageSize).Select(m =>
                    new MovimentoEstoqueViewModel
                    {
                        Id = m.Id,
                        Produto = new MovimentoEstoqueViewModel.ProdutoViewModel
                        {
                            Id = m.Produto.Id,
                            Nome = m.Produto.Nome
                        },
                        Quantidade = m.Quantidade,
                        Data = m.Data,
                        Observacao = m.Observacao
                    }).ToListAsync();
            var totalCount = await movimentos.CountAsync();

            return new
            {
                data = paged,
                totalCount
            };
        }

        // GET: api/MovimentosEstoque/5
        [ResponseType(typeof(MovimentoEstoque))]
        public async Task<IHttpActionResult> GetMovimentoEstoque(int id)
        {
            MovimentoEstoque movimentoEstoque = await db.MovimentosEstoque.FindAsync(id);
            if (movimentoEstoque == null)
            {
                return NotFound();
            }

            return Ok(movimentoEstoque);
        }

        // PUT: api/MovimentosEstoque/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMovimentoEstoque(int id, MovimentoEstoqueBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != model.Id)
            {
                return BadRequest();
            }

            var movimento = await db.MovimentosEstoque.FindAsync(id);
            var produto = await db.Produtos.FindAsync(model.IdProduto);

            if (movimento == null)
            {
                return NotFound();
            }
            if (produto == null)
            {
                ThrowProdutoNotFound(model.IdProduto.Value);
            }

            int quantidadeAnterior = movimento.Quantidade;

            movimento.Produto = produto;
            movimento.Quantidade = model.Quantidade;
            movimento.Data = model.Data.GetValueOrDefault(movimento.Data);
            movimento.Observacao = model.Observacao;

            service.EditarMovimento(movimento, quantidadeAnterior);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovimentoEstoqueExists(id))
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

        // POST: api/MovimentosEstoque
        [ResponseType(typeof(MovimentoEstoque))]
        public async Task<IHttpActionResult> PostMovimentoEstoque(MovimentoEstoqueBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var produto = await db.Produtos.FindAsync(model.IdProduto);

            if (produto == null)
            {
                ThrowProdutoNotFound(model.IdProduto.Value);
            }

            var movimento = new MovimentoEstoque
            {
                Produto = produto,
                Quantidade = model.Quantidade,
                Data = model.Data.GetValueOrDefault(DateTime.Now),
                Observacao = model.Observacao
            };

            service.CriarMovimento(movimento);

            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { movimento.Id }, movimento);
        }

        // DELETE: api/MovimentosEstoque/5
        [ResponseType(typeof(MovimentoEstoque))]
        public async Task<IHttpActionResult> DeleteMovimentoEstoque(int id)
        {
            MovimentoEstoque movimentoEstoque = await db.MovimentosEstoque.FindAsync(id);
            if (movimentoEstoque == null)
            {
                return NotFound();
            }

            try
            {
                service.RemoverMovimento(movimentoEstoque);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Esse movimento foi gerado por uma venda e não pode ser excluído."));
            }

            return Ok(movimentoEstoque);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MovimentoEstoqueExists(int id)
        {
            return db.MovimentosEstoque.Count(e => e.Id == id) > 0;
        }

        private void ThrowProdutoNotFound(int id)
        {
            string message = String.Format("Produto não encontrado com id = {0}", id);
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, message));
        }
    }
}