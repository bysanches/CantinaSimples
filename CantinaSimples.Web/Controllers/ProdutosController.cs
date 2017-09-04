using CantinaSimples.Web.Models;
using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace CantinaSimples.Web.Controllers
{
    [RoutePrefix("api/produtos")]
    public class ProdutosController : ApiController
    {
        private CantinaContext Context;

        public ProdutosController()
        {
            Context = CantinaContext.Create();
        }

        // GET: api/produtos
        [Authorize]
        public async Task<IEnumerable<ProdutoViewModel>> Get(string nome = null)
        {
            IQueryable<Produto> produtos = Context.Produtos;

            if (!string.IsNullOrEmpty(nome))
            {
                produtos = produtos.Where(p => p.Nome.ToUpper().Contains(nome.ToUpper()));
            }

            return await produtos.Select(p => new ProdutoViewModel
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.Preco,
                CaminhoImagem = p.CaminhoImagem,
                Saldo = p.Saldo
            }).ToListAsync();
        }

        // GET: api/produtos/5
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IHttpActionResult> Get(int id)
        {
            Produto produto = await Context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return Ok(ProdutoViewModel.FromProduto(produto));
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IHttpActionResult> Post(ProdutoBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Produto produto = new Produto();
            model.MapToProduto(produto);
            Context.Produtos.Add(produto);

            await Context.SaveChangesAsync();

            return Created("/api/produtos/" + produto.Id, produto);
        }

        //[HttpPost]
        //[Route("{id}/imagem")]
        //[Authorize(Roles = "Administrador,Gerente")]
        //public async Task<IHttpActionResult> PostImagem(int id)
        //{
        //    var produto = await Context.Produtos.FindAsync(id);

        //    if (produto == null)
        //    {
        //        return NotFound();
        //    }

        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    string filePath = HttpContext.Current.Server.MapPath("~/App_Data/produtos");
        //    var provider = new MultipartFormDataStreamProvider(filePath);

        //    var allow = new List<string>
        //    {
        //        ".jpg",
        //        ".png",
        //        ".bmp",
        //        ".gif"
        //    };

        //    try
        //    {
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        MultipartFileData file = provider.FileData.First();
        //        string originalName = file.Headers.ContentDisposition.FileName.Replace("\"", "");
        //        string ext = Path.GetExtension(originalName);
        //        string saveName = produto.Id + ext;

        //        if (!allow.Contains(ext))
        //        {
        //            File.Delete(file.LocalFileName);
        //            return InternalServerError(new HttpResponseException(HttpStatusCode.UnsupportedMediaType));
        //        }

        //        string fullSavePath = Path.Combine(filePath, saveName);
        //        if (File.Exists(fullSavePath))
        //        {
        //            File.Delete(fullSavePath);
        //        }

        //        File.Move(file.LocalFileName, fullSavePath);

        //        produto.CaminhoImagem = saveName;
        //        await Context.SaveChangesAsync();

        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        return InternalServerError(e);
        //    }
        //}

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpPut]
        [ResponseType(typeof(void))]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, ProdutoBindingModel model)
        {
            var produto = await Context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.MapToProduto(produto);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var produto = await Context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            try
            {
                Context.Entry<Produto>(produto).State = EntityState.Deleted;
                await Context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Produto não pode ser excluído pois já há vendas para este produto."));
            }

            return Ok();
        }

        [Route("{id}/imagem")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IHttpActionResult> DeleteImagem(int id)
        {
            var produto = await Context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            string path = HttpContext.Current.Server.MapPath("~/App_Data/produtos");
            path = Path.Combine(path, produto.CaminhoImagem);

            File.Delete(path);
            produto.CaminhoImagem = null;
            await Context.SaveChangesAsync();

            return Ok();
        }

    }

    public class ProdutoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public string CaminhoImagem { get; set; }
        public int Saldo { get; set; }

        public static ProdutoViewModel FromProduto(Produto produto)
        {
            var viewModel = new ProdutoViewModel();
            viewModel.Id = produto.Id;
            viewModel.Nome = produto.Nome;
            viewModel.Descricao = produto.Descricao;
            viewModel.Preco = produto.Preco;
            viewModel.CaminhoImagem = produto.CaminhoImagem;
            viewModel.Saldo = produto.Saldo;
            return viewModel;
        }
    }

    public class ProdutoBindingModel
    {
        [Required]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [Min(0)]
        public decimal Preco { get; set; }

        public void MapToProduto(Produto produto)
        {
            produto.Nome = Nome;
            produto.Descricao = Descricao;
            produto.Preco = Preco;
        }
    }
}
