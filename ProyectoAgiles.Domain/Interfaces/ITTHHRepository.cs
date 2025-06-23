using ProyectoAgiles.Domain.Entities;
using System.Threading.Tasks;

namespace ProyectoAgiles.Domain.Interfaces
{
    public interface ITTHHRepository : IRepository<TTHH>
    {
        Task<TTHH?> GetByCedulaAsync(string cedula);
    }
}
