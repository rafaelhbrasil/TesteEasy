using System.Collections.Generic;
using System.Threading.Tasks;

namespace DojoDDD.Domain
{
    public interface IClienteRepositorio
    {
        Task<Cliente> ConsultarPorId(string id);
        Task<IEnumerable<Cliente>> ConsultarTodosCliente();
    }
}