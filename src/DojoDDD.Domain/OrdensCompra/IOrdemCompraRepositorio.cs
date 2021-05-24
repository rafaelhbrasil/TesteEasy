using System.Threading.Tasks;

namespace DojoDDD.Domain
{
    public interface IOrdemCompraRepositorio
    {
        Task<string> RegistrarOrdemCompra(OrdemCompra ordemCompra);
        Task<bool> AlterarOrdemCompra(OrdemCompra ordemCompra);
        Task<bool> AlterarStatusOrdemCompra(string ordemId, OrdemCompraStatus novoOrdemCompraStatus);
        Task<OrdemCompra> ConsultarPorId(string id);
    }
}