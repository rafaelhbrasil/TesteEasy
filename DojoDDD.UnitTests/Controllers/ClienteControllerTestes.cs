using DojoDDD.Api.Controllers;
using DojoDDD.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DojoDDD.UnitTests.Controllers
{
    public class ClienteControllerTestes
    {
        Mock<IClienteRepositorio> _clienteRepoMock;
        Mock<ICustomLogger> _loggerMock;
        ClienteController _clienteController;

        /// <summary>
        /// Inicializa uma instância da classe de testes. Cada UnitTest usa uma instância independente. Por isso é tranquilo esse tipo de reuso com xUnit.
        /// </summary>
        public ClienteControllerTestes()
        {
            _clienteRepoMock = new Mock<IClienteRepositorio>();
            _loggerMock = new Mock<ICustomLogger>();
            _clienteController = new ClienteController(_clienteRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Get_ExistemClientes_RetornaClientes()
        {
            var cliente1 = CriarCliente();
            var cliente2 = CriarCliente();
            _clienteRepoMock.Setup(m => m.ConsultarTodosCliente()).ReturnsAsync(new List<Cliente> { cliente1, cliente2 });
            var clientesResult = await _clienteController.Get();

            var resultadoRequest = (IEnumerable<Cliente>)((OkObjectResult)clientesResult.Result).Value;
            Assert.Equal(2, resultadoRequest.Count());
            Assert.Equal(cliente1, resultadoRequest.First());
            Assert.Equal(cliente2, resultadoRequest.Last());
        }

        [Fact]
        public async Task Get_NaoExistemClientes_RetornaListaVazia()
        {
            var cliente1 = CriarCliente();
            var cliente2 = CriarCliente();
            _clienteRepoMock.Setup(m => m.ConsultarTodosCliente()).ReturnsAsync(new List<Cliente>());
            var clientesResult = await _clienteController.Get();

            var resultadoRequest = (IEnumerable<Cliente>)((OkObjectResult)clientesResult.Result).Value;
            Assert.Empty(resultadoRequest);
        }

        [Fact]
        public async Task Get_Erro_LogaESobeExcecao()
        {
            _clienteRepoMock.Setup(m => m.ConsultarTodosCliente()).ThrowsAsync(new IOException());

            await Assert.ThrowsAsync<IOException>(async () =>
            {
                var clientesResult = await _clienteController.Get();
            });
            _loggerMock.Verify(mocks => mocks.Log(It.IsAny<string>(), EventLevel.Error));
        }

        private static Cliente CriarCliente()
        {
            var id = Guid.NewGuid().ToString();
            var numero = new Random().Next(20, 60);
            return new Cliente
            {
                Id = id,
                Nome = $"Cliente {id}",
                Idade = numero,
                Saldo = numero + 100,
                Endereco = $"Rua {numero} número {numero}, Bairro XXX",
            };
        }


        [Fact]
        public async Task GetById_EncontrouCliente_RetornaCliente()
        {
            var cliente1 = CriarCliente();
            _clienteRepoMock.Setup(m => m.ConsultarPorId(cliente1.Id)).ReturnsAsync(cliente1);
            var clientesResult = await _clienteController.GetById(cliente1.Id);

            var resultadoRequest = (Cliente)((OkObjectResult)clientesResult.Result).Value;
            Assert.Equal(cliente1, resultadoRequest);
            Assert.Equal(cliente1.Id, resultadoRequest.Id);
        }

        [Fact]
        public async Task GetById_NaoEncontrouCliente_RetornaNoContent404()
        {
            _clienteRepoMock.Setup(m => m.ConsultarPorId("xxx")).ReturnsAsync(null as Cliente);
            var clientesResult = await _clienteController.GetById("xxx");
            Assert.IsType<NotFoundResult>(clientesResult.Result);
        }

        [Fact]
        public async Task GetById_Erro_LogaESobeExcecao()
        {
            _clienteRepoMock.Setup(m => m.ConsultarPorId("xxx2")).ThrowsAsync(new IOException());

            await Assert.ThrowsAsync<IOException>(async () =>
            {
                var clientesResult = await _clienteController.GetById("xxx2");
            });
            _loggerMock.Verify(mocks => mocks.Log(It.IsAny<string>(), EventLevel.Error));
        }
    }
}
