using _00_Entities.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_BusinessLogic.Interfaces
{
    public interface IAtencionBL
    {
        public string recibirAtencion(DatosAtencionEN request);
    }
}
