using DojoDDD.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DojoDDD.UnitTests.Services
{
    public class OrdemCompraServicoTestes
    {
        Mock<IClienteRepositorio> _clienteRepoMock;
        Mock<IProdutoRepositorio> _produtoRepoMock;
        Mock<IOrdemCompraRepositorio> _ordemCompraRepoMock;
        Mock<ICustomLogger> _loggerMock;
        OrdemCompraServico _ordemCompraServico;

        /// <summary>
        /// Inicializa uma instância da classe de testes. Cada UnitTest usa uma instância independente. Por isso é tranquilo esse tipo de reuso com xUnit.
        /// </summary>
        public OrdemCompraServicoTestes()
        {
            _clienteRepoMock = new Mock<IClienteRepositorio>();
            _produtoRepoMock = new Mock<IProdutoRepositorio>();
            _ordemCompraRepoMock = new Mock<IOrdemCompraRepositorio>();
            _loggerMock = new Mock<ICustomLogger>();
            _ordemCompraServico = new OrdemCompraServico(_clienteRepoMock.Object, _produtoRepoMock.Object, _ordemCompraRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task RegistrarOrdemCompra_Sucesso_RetornaIdCompra()
        {
            var cliente = CriarCliente();
            var produto = CriarProduto();
            var novoId = Guid.NewGuid().ToString();
            _clienteRepoMock.Setup(m => m.ConsultarPorId(It.IsAny<string>())).ReturnsAsync(cliente);
            _produtoRepoMock.Setup(m => m.ConsultarPorId(It.IsAny<int>())).ReturnsAsync(produto);
            _ordemCompraRepoMock.Setup(m => m.RegistrarOrdemCompra(It.IsAny<OrdemCompra>())).ReturnsAsync(novoId);

            var resultado = await _ordemCompraServico.RegistrarOrdemCompra(cliente.Id, produto.Id, 1);

            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
        }

        [Theory]
        [InlineData(0, 1, 1, 0)]
        [InlineData(1, 1, -1, 0)]
        [InlineData(1, 1, 100000, 0)]
        [InlineData(1, 100000, 1, 0)]
        [InlineData(1, 1, 1, 10000)]
        public async Task RegistrarOrdemCompra_Erro_LancaInvalidOperationException(int estoque, int valorUnitario, int quantidade, decimal valorMinimo)
        {
            var cliente = CriarCliente();
            var produto = CriarProduto(estoque, valorUnitario, valorMinimo);
            var novoId = Guid.NewGuid().ToString();
            _clienteRepoMock.Setup(m => m.ConsultarPorId(It.IsAny<string>())).ReturnsAsync(cliente);
            _produtoRepoMock.Setup(m => m.ConsultarPorId(It.IsAny<int>())).ReturnsAsync(produto);
            _ordemCompraRepoMock.Setup(m => m.RegistrarOrdemCompra(It.IsAny<OrdemCompra>())).ReturnsAsync(novoId);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var resultado = await _ordemCompraServico.RegistrarOrdemCompra(cliente.Id, produto.Id, quantidade);
            });

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AlterarStatudOrdemDeCompraParaEmAnalise_RetornaValorDoRepositorio(bool statusAlteracao)
        {
            var novoId = Guid.NewGuid().ToString();
            _ordemCompraRepoMock.Setup(m => m.AlterarStatusOrdemCompra(novoId, It.IsAny<OrdemCompraStatus>())).ReturnsAsync(statusAlteracao);

            var resultado = await _ordemCompraServico.AlterarStatudOrdemDeCompraParaEmAnalise(novoId);

            Assert.Equal(statusAlteracao, resultado);
        }

        [Fact]
        public async Task AlterarStatudOrdemDeCompraParaEmAnalise_Erro_LogaSobeErro()
        {
            var novoId = Guid.NewGuid().ToString();
            _ordemCompraRepoMock.Setup(m => m.AlterarStatusOrdemCompra(novoId, It.IsAny<OrdemCompraStatus>())).ThrowsAsync(new IOException());

            await Assert.ThrowsAsync<IOException>(async () =>
            {
                var resultado = await _ordemCompraServico.AlterarStatudOrdemDeCompraParaEmAnalise(novoId);
            });
            _loggerMock.Verify(m => m.Log(It.IsAny<string>(), EventLevel.Error));
        }

        private static Produto CriarProduto(int estoque = 10, int valorUnitario = 100, decimal valorMinimo = 0)
        {
            var id = new Random().Next();
            return new Produto
            {
                Id = id,
                Descricao = $"Produto {id}",
                Estoque = estoque,
                PrecoUnitario = valorUnitario,
                ValorMinimoDeCompra = (int)valorMinimo
            };
        }

        private static Cliente CriarCliente()
        {
            var id = Guid.NewGuid().ToString();
            var numero = new Random().Next(20, 60);
            return new Cliente
            {
                Id = id,
                Nome = $"Cliente {id}",
                Saldo = 1000,
                Endereco = $"Rua {numero} número {numero}, Bairro XXX",
            };
        }
    }
}
