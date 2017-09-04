using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using CantinaSimples.Web.Models;
using System.Linq;
using System.Collections.Generic;
using Moq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CantinaSimples.Web.Tests.Provider;
using CantinaSimples.Web.Controllers;

namespace CantinaSimples.Web.Tests.Controllers
{
    [TestClass]
    public class ClientesControllerTest
    {
        private IQueryable<Cliente> CreateTestData()
        {
            return new List<Cliente>
            {
                new Cliente { Id = 1, Nome = "Ted", Nascimento = new DateTime(1984, 1, 1), Documento = "626.137.562-33", Email = "ted@builtcode.com.br", Telefone = "(21) 9666-4018", NomeResponsavel = "Sra. Mosby", TelefoneResponsavel = "(81) 8433-3444", EmailResponsavel = "sramosby@builtcode.com.br" },
                new Cliente { Id = 2, Nome = "Marshall", Nascimento = new DateTime(1985, 2, 2), Documento = "573.743.854-63", Email = "marshall@builtcode.com.br", Telefone = "(11) 5282-6253", NomeResponsavel = "Judy", TelefoneResponsavel = "(11) 5282-6253", EmailResponsavel = "judy@builtcode.com.br" },
                new Cliente { Id = 3, Nome = "Lilly", Nascimento = new DateTime(1986, 3, 3), Documento = "495.427.518-00", Email = "lilly@builtcode.com.br", Telefone = "(27) 8748-8642", NomeResponsavel = "Mickey", TelefoneResponsavel = "(27) 8748-8642", EmailResponsavel = "mickey@builtcode.com.br" },
                new Cliente { Id = 4, Nome = "Barney", Nascimento = new DateTime(1987, 4, 4), Documento = "553.020.499-61", Email = "barney@builtcode.com.br", Telefone = "(11) 3041-2339", NomeResponsavel = "Loretta", TelefoneResponsavel = "(11) 3041-2339", EmailResponsavel = "loretta@builtcode.com.br" },
                new Cliente { Id = 5, Nome = "Robin", Nascimento = new DateTime(1983, 5, 6), Documento = "626.425.150-01", Email = "robin@builtcode.com.br", Telefone = "(81) 8433-3444", NomeResponsavel = "Robin Senior", TelefoneResponsavel = "(81) 8433-3444", EmailResponsavel = "robin.senior@builtcode.com.br" }
            }.AsQueryable();
        }

        [TestMethod]
        public async Task GetClientesAsync()
        {
            var data = CreateTestData();

            var mockSet = new Mock<DbSet<Cliente>>();
            mockSet.As<IDbAsyncEnumerable<Cliente>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Cliente>(data.GetEnumerator()));

            mockSet.As<IQueryable<Cliente>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Cliente>(data.Provider));

            mockSet.As<IQueryable<Cliente>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Cliente>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Cliente>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<CantinaContext>();
            mockContext.Setup(c => c.Clientes).Returns(mockSet.Object);

            var controller = new ClientesController(mockContext.Object);
            var clientes = await controller.GetClientesAsync();

            Assert.AreEqual(5, clientes.Count());
            Assert.AreEqual("Ted", clientes.ElementAt(0).Nome);
            Assert.AreEqual("Marshall", clientes.ElementAt(1).Nome);
            Assert.AreEqual("Lilly", clientes.ElementAt(2).Nome);
            Assert.AreEqual("Barney", clientes.ElementAt(3).Nome);
            Assert.AreEqual("Robin", clientes.ElementAt(4).Nome);

            clientes = await controller.GetClientesAsync("ted");
            Assert.AreEqual(1, clientes.Count());
            Assert.AreEqual("Ted", clientes.ElementAt(0).Nome);

            clientes = await controller.GetClientesAsync(email: "barney@builtcode.com.br");
            Assert.AreEqual(1, clientes.Count());
            Assert.AreEqual("Barney", clientes.ElementAt(0).Nome);

            clientes = await controller.GetClientesAsync(documento: "495.427.518-00");
            Assert.AreEqual(1, clientes.Count());
            Assert.AreEqual("Lilly", clientes.ElementAt(0).Nome);
        }
    }
}
