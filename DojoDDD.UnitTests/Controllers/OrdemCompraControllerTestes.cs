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
    public class OrdemCompraControllerTestes
    {
        Mock<ICustomLogger> _loggerMock;
        Mock<IOrdemCompraRepositorio> _ordemCompraRepoMock;
        Mock<IOrdemCompraServico> _ordemCompraServicoMock;
        OrdemCompraController _ordemCompraController;

        /// <summary>
        /// Inicializa uma instância da classe de testes. Cada UnitTest usa uma instância independente. Por isso é tranquilo esse tipo de reuso com xUnit.
        /// </summary>
        public OrdemCompraControllerTestes()
        {
            _loggerMock = new Mock<ICustomLogger>();
            _ordemCompraRepoMock = new Mock<IOrdemCompraRepositorio>();
            _ordemCompraServicoMock = new Mock<IOrdemCompraServico>();
            _ordemCompraController = new OrdemCompraController(_ordemCompraServicoMock.Object, _ordemCompraRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Post_Sucesso_RetornaIdNovoPedido()
        {
            var ordemCompra = CriarOrdemCompra();
            _ordemCompraServicoMock.Setup(m => m.RegistrarOrdemCompra(ordemCompra.ClienteId, ordemCompra.ProdutoId, ordemCompra.QuantidadeSolicitada))
                                   .ReturnsAsync(ordemCompra.Id);
            var ordemCompraResult = await _ordemCompraController.Post(ordemCompra);

            var resultadoRequest = (string)((CreatedResult)ordemCompraResult.Result).Value;
            Assert.Equal(ordemCompra.Id, resultadoRequest);
        }

        [Fact]
        public async Task Post_CamposInvalidos_RetornaBadRequest400()
        {
            var ordemCompra = CriarOrdemCompra();
            _ordemCompraServicoMock.Setup(m => m.RegistrarOrdemCompra(ordemCompra.ClienteId, ordemCompra.ProdutoId, ordemCompra.QuantidadeSolicitada))
                                   .ThrowsAsync(new InvalidOperationException("Erro ORDEMCOMPRA"));
            var ordemCompraResult = await _ordemCompraController.Post(ordemCompra);

            var resultadoRequest = (string)((BadRequestObjectResult)ordemCompraResult.Result).Value;
            Assert.Equal("Erro ORDEMCOMPRA", resultadoRequest);
            _loggerMock.Verify(mocks => mocks.Log(It.IsAny<string>(), EventLevel.Warning));
        }

        [Fact]
        public async Task Post_Erro_LogaESobeExcecao()
        {
            var ordemCompra = CriarOrdemCompra();
            _ordemCompraServicoMock.Setup(m => m.RegistrarOrdemCompra(ordemCompra.ClienteId, ordemCompra.ProdutoId, ordemCompra.QuantidadeSolicitada))
                                   .ThrowsAsync(new IOException());

            await Assert.ThrowsAsync<IOException>(async () =>
            {
                var produtosResult = await _ordemCompraController.Post(ordemCompra);
            });
            _loggerMock.Verify(mocks => mocks.Log(It.IsAny<string>(), EventLevel.Error));
        }

        private static OrdemCompra CriarOrdemCompra()
        {
            var id = Guid.NewGuid().ToString();
            var numero = new Random().Next(1, 1000);

            var precoUnitario = numero * 100;
            var quantidade = (numero + 10)/10;
            return new OrdemCompra
            {
                Id = id,
                ClienteId = id+1,
                ProdutoId = numero,
                DataOperacao = DateTime.Now,
                PrecoUnitario = precoUnitario,
                Status = OrdemCompraStatus.EmAnalise,
                QuantidadeSolicitada = quantidade,
                ValorOperacao = precoUnitario * quantidade,
            };
        }


        [Fact]
        public async Task ConsultarPorId_EncontrouProduto_RetornaProduto()
        {
            var ordemCompra = CriarOrdemCompra();
            _ordemCompraRepoMock.Setup(m => m.ConsultarPorId(ordemCompra.Id)).ReturnsAsync(ordemCompra);
            var ordemCompraResult = await _ordemCompraController.ConsultarPorId(ordemCompra.Id);

            var resultadoRequest = (OrdemCompra)((OkObjectResult)ordemCompraResult.Result).Value;
            Assert.Equal(ordemCompra, resultadoRequest);
            Assert.Equal(ordemCompra.Id, resultadoRequest.Id);
        }

        [Fact]
        public async Task ConsultarPorId_NaoEncontrouProduto_RetornaNoContent404()
        {
            _ordemCompraRepoMock.Setup(m => m.ConsultarPorId("101")).ReturnsAsync(null as OrdemCompra);
            var produtosResult = await _ordemCompraController.ConsultarPorId("101");
            Assert.IsType<NotFoundResult>(produtosResult.Result);
        }

        [Fact]
        public async Task ConsultarPorId_Erro_LogaESobeExcecao()
        {
            _ordemCompraRepoMock.Setup(m => m.ConsultarPorId("102")).ThrowsAsync(new IOException());

            await Assert.ThrowsAsync<IOException>(async () =>
            {
                var produtosResult = await _ordemCompraController.ConsultarPorId("102");
            });
            _loggerMock.Verify(mocks => mocks.Log(It.IsAny<string>(), EventLevel.Error));
        }
    }
}
