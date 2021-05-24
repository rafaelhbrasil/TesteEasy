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
    public class ProdutoControllerTestes
    {
        Mock<ICustomLogger> _loggerMock;
        Mock<IProdutoRepositorio> _produtoRepoMock;
        ProdutoController _produtoController;

        /// <summary>
        /// Inicializa uma instância da classe de testes. Cada UnitTest usa uma instância independente. Por isso é tranquilo esse tipo de reuso com xUnit.
        /// </summary>
        public ProdutoControllerTestes()
        {
            _produtoRepoMock = new Mock<IProdutoRepositorio>();
            _loggerMock = new Mock<ICustomLogger>();
            _produtoController = new ProdutoController(_produtoRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Get_ExistemProdutos_RetornaProdutos()
        {
            var produto1 = CriarProduto();
            var produto2 = CriarProduto();
            _produtoRepoMock.Setup(m => m.Consultar()).ReturnsAsync(new List<Produto> { produto1, produto2 });
            var produtosResult = await _produtoController.Get();

            var resultadoRequest = (IEnumerable<Produto>)((OkObjectResult)produtosResult.Result).Value;
            Assert.Equal(2, resultadoRequest.Count());
            Assert.Equal(produto1, resultadoRequest.First());
            Assert.Equal(produto2, resultadoRequest.Last());
        }

        [Fact]
        public async Task Get_NaoExistemProdutos_RetornaListaVazia()
        {
            _produtoRepoMock.Setup(m => m.Consultar()).ReturnsAsync(new List<Produto>());
            var produtosResult = await _produtoController.Get();

            var resultadoRequest = (IEnumerable<Produto>)((OkObjectResult)produtosResult.Result).Value;
            Assert.Empty(resultadoRequest);
        }

        [Fact]
        public async Task Get_Erro_LogaESobeExcecao()
        {
            _produtoRepoMock.Setup(m => m.Consultar()).ThrowsAsync(new IOException());

            await Assert.ThrowsAsync<IOException>(async () =>
            {
                var produtosResult = await _produtoController.Get();
            });
            _loggerMock.Verify(mocks => mocks.Log(It.IsAny<string>(), EventLevel.Error));
        }

        private static Produto CriarProduto()
        {
            var id = new Random().Next();
            return new Produto
            {
                Id = id,
                Descricao = $"Produto {id}",
                Estoque = id * 10,
                PrecoUnitario = id + 100,
                ValorMinimoDeCompra = id + 99
            };
        }


        [Fact]
        public async Task GetById_EncontrouProduto_RetornaProduto()
        {
            var produto1 = CriarProduto();
            _produtoRepoMock.Setup(m => m.ConsultarPorId(produto1.Id)).ReturnsAsync(produto1);
            var produtosResult = await _produtoController.GetById(produto1.Id.ToString());

            var resultadoRequest = (Produto)((OkObjectResult)produtosResult.Result).Value;
            Assert.Equal(produto1, resultadoRequest);
            Assert.Equal(produto1.Id, resultadoRequest.Id);
        }

        [Fact]
        public async Task GetById_NaoEncontrouProduto_RetornaNoContent404()
        {
            _produtoRepoMock.Setup(m => m.ConsultarPorId(101)).ReturnsAsync(null as Produto);
            var produtosResult = await _produtoController.GetById("101");
            Assert.IsType<NotFoundResult>(produtosResult.Result);
        }

        [Fact]
        public async Task GetById_Erro_LogaESobeExcecao()
        {
            _produtoRepoMock.Setup(m => m.ConsultarPorId(102)).ThrowsAsync(new IOException());

            await Assert.ThrowsAsync<IOException>(async () =>
            {
                var produtosResult = await _produtoController.GetById("102");
            });
            _loggerMock.Verify(mocks => mocks.Log(It.IsAny<string>(), EventLevel.Error));
        }
    }
}
