using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_BusinessLogic.Clases
{
    public class AdmisionBL : IAdmisionBL
    {
        public string test()
        {
            AdmisionDAL oAdmision = new AdmisionDAL();
            return oAdmision.test();
        }
    }
}
