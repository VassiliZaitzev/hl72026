using _02_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HPC_HL7_CONNECTBACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorizacionController : Controller
    {
        private readonly ICategorizacionBL _ICategorizacionBL;
        public CategorizacionController(ICategorizacionBL iCategorizacionBL)
        {
            _ICategorizacionBL = iCategorizacionBL;
        }

        [HttpGet("test")]
        public string test()
        {
            return _ICategorizacionBL.test();
        }
    }
}
