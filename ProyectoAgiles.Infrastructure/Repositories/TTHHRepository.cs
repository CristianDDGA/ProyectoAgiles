using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;
using System.Threading.Tasks;

namespace ProyectoAgiles.Infrastructure.Repositories
{
    public class TTHHRepository : Repository<TTHH>, ITTHHRepository
    {
        public TTHHRepository(ApplicationDbContext context) : base(context) { }

        public async Task<TTHH?> GetByCedulaAsync(string cedula)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.Cedula == cedula);
        }
    }
}
