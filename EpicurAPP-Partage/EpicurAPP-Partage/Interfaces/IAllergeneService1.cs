using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;

namespace EpicurAPP_Partage.Interfaces
{
    public interface IAllergeneService
    {
        List<Allergene> GetAll();
        List<Allergene> GetAllergenesByClient(int clientId);
        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);
        void AjouterAllergene(Allergene allergene);
    }
}
