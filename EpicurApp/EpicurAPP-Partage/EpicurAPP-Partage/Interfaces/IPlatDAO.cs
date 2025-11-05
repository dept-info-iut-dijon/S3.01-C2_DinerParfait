using EpicurApp_API.Models;
using EpicurAppData;

namespace EpicurApp_API.DAO
{
    public interface IPlatDAO
    {
        Task<IEnumerable<Plat>> GetAllAsync();
    }
}
