using DojoDDD.Domain;
using System;
using System.Threading.Tasks;

namespace DojoDDD.Infrastructure
{
    public class OrdemCompraRepositorio : IOrdemCompraRepositorio
    {
        private readonly DataStore _dataStore;

        public OrdemCompraRepositorio(DataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <summary>
        /// Altera a ordem de compra. Por ser mockado, remove o item anterior e insere um novo, simulando uma substituição.
        /// </summary>
        /// <param name="ordemCompra">Ordem de compra com os novos dados</param>
        /// <returns>true se sucesso, false se não encontrou o item com mesmo ID para atualizar.</returns>
        public async Task<bool> AlterarOrdemCompra(OrdemCompra ordemCompra)
        {
            var ordemCompraDb = await ConsultarPorId(ordemCompra.Id).ConfigureAwait(false);
            if (ordemCompraDb == null) return false;
            _dataStore.OrdensCompras.Remove(ordemCompraDb);
            _dataStore.OrdensCompras.Add(ordemCompra);
            return true;
        }

        public async Task<bool> AlterarStatusOrdemCompra(string ordemId, OrdemCompraStatus novoOrdemCompraStatus)
        {
            var ordemCompra = await ConsultarPorId(ordemId).ConfigureAwait(false);
            if (ordemCompra == null) return false;
            ordemCompra.Status = novoOrdemCompraStatus;
            return await AlterarOrdemCompra(ordemCompra);
        }

        public async Task<OrdemCompra> ConsultarPorId(string id)
        {
            var ordemCompra = await Task.FromResult(_dataStore.OrdensCompras.Find(x => x.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase))).ConfigureAwait(false);
            return ordemCompra;
        }

        public async Task<string> RegistrarOrdemCompra(OrdemCompra ordemCompra)
        {
            await Task.Run(() => _dataStore.OrdensCompras.Add(ordemCompra)).ConfigureAwait(false);
            return ordemCompra.Id;
        }
    }
}