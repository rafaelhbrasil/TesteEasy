using System.Collections.Generic;
using System.Threading.Tasks;

namespace DojoDDD.Domain
{
    public interface IProdutoRepositorio
    {
        Task<Produto> ConsultarPorId(int id);
        Task<List<Produto>> Consultar();
    }
}