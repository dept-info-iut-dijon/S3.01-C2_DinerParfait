using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicurAPP_Partage.Exceptions
{
    //Exception levée en cas d'erreur de validation
    public class InvalidFieldException : Exception
    {
        public InvalidFieldException(string message) : base(message)
        {

        }
    }
}
