using System;
using System.Threading.Tasks;

namespace DojoDDD.Api.DojoDDD.Domain
{
    public interface IClienteRepositorio
    {
        Task<Cliente> ConsultarPorId(string id);
    }

    public class ClienteRepositorio
    {
        public async Task<Cliente> ConsultarPorId(string id)
        {
            return await Task.FromResult<Cliente>(new Cliente { });
        }
    }

    public interface IProdutoRepositorio
    {
        Task<Produto> ConsultarPorId(int id);
    }

    public class ProdutoRepositorio
    {
        public async Task<Produto> ConsultarPorId(int id)
        {
            return await Task.FromResult(new Produto { });
        }
    }

    public interface IOrdemCompraRepositorio
    {
        Task<int> RegistrarOrdemCompra(OrdemCompra ordemCompra);
        Task<bool> AlterarOrdemCompra(OrdemCompra ordemCompra);
        Task<int> ConsultarPorId(int id);
    }

    public class OrdemCompraRepositorio : IOrdemCompraRepositorio
    {
        public Task<bool> AlterarOrdemCompra(OrdemCompra ordemCompra)
        {
            throw new NotImplementedException();
        }

        public Task<int> ConsultarPorId(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RegistrarOrdemCompra(OrdemCompra ordemCompra)
        {
            return await Task.FromResult(new Random().Next()).ConfigureAwait(false);
        }
    }

    public class OrdemCompraServico
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly IOrdemCompraRepositorio _ordemCompraRepositorio;

        public OrdemCompraServico(IClienteRepositorio clienteRepositorio,
                                  IProdutoRepositorio produtoRepositorio,
                                  IOrdemCompraRepositorio ordemCompraRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
            _produtoRepositorio = produtoRepositorio;
            _ordemCompraRepositorio = ordemCompraRepositorio;
        }

        public async Task<int> RegistrarOrdemCompra(string clienteId, int produtoId, int quantidadeCompra)
        {
            var cliente = await _clienteRepositorio.ConsultarPorId(clienteId).ConfigureAwait(false);
            var produto = await _produtoRepositorio.ConsultarPorId(produtoId).ConfigureAwait(false);

            if (quantidadeCompra > 0)
                throw new InvalidOperationException("");

            if (produto.Estoque > 0)
                throw new InvalidOperationException("");

            if (quantidadeCompra < produto.QuantidadeMinima)
            {

            }

            var valorOperacao = Math.Round(produto.ValorUnitario * quantidadeCompra);
            if (valorOperacao > cliente.Saldo)
                throw new InvalidOperationException("");

            var novaOrdemDeCompra = new OrdemCompra
            {
                ClienteId = cliente.Id,
                ProdutoId = produto.Id,
                DataOperacao = DateTime.Now,
                ValorOperacao = valorOperacao
            };

            return await _ordemCompraRepositorio.RegistrarOrdemCompra(novaOrdemDeCompra).ConfigureAwait(false);
        }

        public async Task<bool> AlterarStatudOrdemDeCompra(int ordemDeCompraId)
        {
            var ordemDeCompra = await _ordemCompraRepositorio.ConsultarPorId(ordemDeCompraId).ConfigureAwait(false);

            return false;
        }
    }
}