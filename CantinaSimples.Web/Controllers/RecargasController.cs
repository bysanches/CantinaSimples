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

namespace CantinaSimples.Web.Controllers
{
    [Authorize(Roles="Administrador,Gerente")]
    public class RecargasController : BaseApiController
    {
        private CantinaContext db = new CantinaContext();

        // GET: api/Recargas
        public async Task<IEnumerable<Recarga>> GetRecargas(int? ClienteId = null)
        {
            IQueryable<Recarga> recargas = db.Recargas;

            if (ClienteId.HasValue)
            {
                recargas = recargas.Where(r => r.Cliente.Id == ClienteId);
            }

            return await recargas.ToArrayAsync<Recarga>();
        }

        // GET: api/Recargas/5
        [ResponseType(typeof(Recarga))]
        public async Task<IHttpActionResult> GetRecarga(int id)
        {
            Recarga recarga = await db.Recargas.FindAsync(id);
            if (recarga == null)
            {
                return NotFound();
            }

            return Ok(recarga);
        }

        // POST: api/Recargas
        [ResponseType(typeof(Recarga))]
        public async Task<IHttpActionResult> PostRecarga(RecargaBindingModel recargaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            Usuario user = await db.Users.FirstAsync(u => u.Email == User.Identity.Name); 
            Cliente cliente = await db.Clientes.FindAsync(recargaModel.IdCliente);
            decimal saldoAtual = cliente.Saldo;

            Recarga recarga = new Recarga()
            {
                Cliente = cliente,
                Data = DateTime.Now,
                Usuario = user,
                Valor = recargaModel.Valor
            };

            cliente.Saldo = saldoAtual + recargaModel.Valor;
            db.Recargas.Add(recarga);
            await db.SaveChangesAsync();            

            return CreatedAtRoute("DefaultApi", new { id = recarga.Id }, recarga);
        }

        // DELETE: api/Recargas/5
        [ResponseType(typeof(Recarga))]
        public async Task<IHttpActionResult> DeleteRecarga(int id)
        {
            Recarga recarga = await db.Recargas.FindAsync(id);
            if (recarga == null)
            {
                return NotFound();
            }

            db.Recargas.Remove(recarga);
            await db.SaveChangesAsync();

            return Ok(recarga);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RecargaExists(int id)
        {
            return db.Recargas.Count(e => e.Id == id) > 0;
        }
    }
}