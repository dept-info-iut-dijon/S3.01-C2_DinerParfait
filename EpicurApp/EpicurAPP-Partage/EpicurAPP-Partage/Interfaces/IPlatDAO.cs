using EpicurApp_API.Models;

namespace EpicurAPP_Partage.Interfaces
{
    public interface IPlatDAO
    {
        Task<IEnumerable<Plat>> GetAllAsync();
        Task AddAsync(Plat plat);
        Task<Plat>GetByIdAsync(int id);
    }
}
