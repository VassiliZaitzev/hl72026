using _02_BusinessLogic.Clases;
using _02_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HPC_HL7_CONNECTBACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmisionController : Controller
    {
        private readonly IAdmisionBL _IAdmisionBL;
        public AdmisionController(IAdmisionBL iAdmisionBL)
        {
            _IAdmisionBL = iAdmisionBL;
        }

        [HttpGet("test")]
        public string test()
        {
            return _IAdmisionBL.test();
        }
    }
}
