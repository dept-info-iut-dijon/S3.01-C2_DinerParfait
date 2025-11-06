using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicurAPP_Partage.Models
{
    public class Allergene
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Nom;
        }
    }
}
