using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace DojoDDD.Domain
{
    public class OrdemCompraServico : IOrdemCompraServico
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly IOrdemCompraRepositorio _ordemCompraRepositorio;
        private readonly ICustomLogger _logger;

        public OrdemCompraServico(IClienteRepositorio clienteRepositorio,
                                  IProdutoRepositorio produtoRepositorio,
                                  IOrdemCompraRepositorio ordemCompraRepositorio,
                                  ICustomLogger logger)
        {
            _clienteRepositorio = clienteRepositorio;
            _produtoRepositorio = produtoRepositorio;
            _ordemCompraRepositorio = ordemCompraRepositorio;
            _logger = logger;
        }

        public async Task<string> RegistrarOrdemCompra(string clienteId, int produtoId, int quantidadeCompra)
        {
            var produto = await _produtoRepositorio.ConsultarPorId(produtoId).ConfigureAwait(false);

            await ValidarRequisitosOrdemCompra(clienteId, produto, quantidadeCompra).ConfigureAwait(false);

            var novaOrdemDeCompra = new OrdemCompra
            {
                ClienteId = clienteId,
                ProdutoId = produtoId,
                DataOperacao = DateTime.Now,
                PrecoUnitario = produto.PrecoUnitario,
                ValorOperacao = CalcularValorTotalDaCompra(produto.PrecoUnitario, quantidadeCompra),
                QuantidadeSolicitada = quantidadeCompra
            };

            var id = await _ordemCompraRepositorio.RegistrarOrdemCompra(novaOrdemDeCompra).ConfigureAwait(false);
            return id;
        }

        private async Task ValidarRequisitosOrdemCompra(string clienteId, Produto produto, int quantidadeCompra)
        {
            var cliente = await _clienteRepositorio.ConsultarPorId(clienteId).ConfigureAwait(false);
            var valorOperacao = Math.Round(produto.PrecoUnitario * quantidadeCompra, 2);

            if (quantidadeCompra <= 0)
                throw new InvalidOperationException("Quantidade solicitada não suficiente para compra.");

            if (produto.Estoque <= 0)
                throw new InvalidOperationException("Quantidade em estoque não suficiente para compra.");

            if (valorOperacao > cliente.Saldo)
                throw new InvalidOperationException("Cliente não possui saldo suficiente para compra.");

            if (CalcularValorTotalDaCompra(produto.PrecoUnitario, quantidadeCompra) < produto.ValorMinimoDeCompra)
                throw new InvalidOperationException("Quantidade mínima não atendida para compra.");

            if (quantidadeCompra > produto.Estoque)
                throw new InvalidOperationException("Quantidade em estoque não suficiente para compra.");
        }

        private decimal CalcularValorTotalDaCompra(decimal precoUnitario, int quantidadeCompra)
        {
            return Math.Round(quantidadeCompra * precoUnitario, 2);
        }

        public async Task<bool> AlterarStatudOrdemDeCompraParaEmAnalise(string ordemDeCompraId)
        {
            try
            {
                return await _ordemCompraRepositorio.AlterarStatusOrdemCompra(ordemDeCompraId, OrdemCompraStatus.EmAnalise).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao atualizar status da Ordem de Compra {ordemDeCompraId} para {OrdemCompraStatus.EmAnalise}. {ex}", EventLevel.Error);
                throw;
            }
        }
    }
}