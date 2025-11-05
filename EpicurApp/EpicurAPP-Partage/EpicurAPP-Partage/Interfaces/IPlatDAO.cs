using EpicurApp_API.Models;

namespace EpicurAPP_Partage.Interfaces
{
    public interface IPlatDAO
    {
        Task<IEnumerable<Plat>> GetAllAsync();
    }
}
