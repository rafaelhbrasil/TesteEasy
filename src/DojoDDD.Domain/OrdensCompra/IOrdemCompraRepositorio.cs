using System.Threading.Tasks;

namespace DojoDDD.Domain
{
    public interface IOrdemCompraRepositorio
    {
        Task<string> RegistrarOrdemCompra(OrdemCompra ordemCompra);
        Task<bool> AlterarOrdemCompra(string ordemId, OrdemCompraStatus novoOrdemCompraStatus);
        Task<string> ConsultarPorId(string id);
    }
}