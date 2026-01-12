using _00_Entities.Clases;
using _02_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HPC_HL7_CONNECTBACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtencionController : Controller
    {
        private readonly IAtencionBL _IAtencionBL;
        public AtencionController(IAtencionBL iAtencionBL)
        {
            _IAtencionBL = iAtencionBL;
        }

        [HttpPost("recibirAtencion")]
        public string recibirAtencion([FromBody] DatosAtencionEN request)
        {
            return _IAtencionBL.recibirAtencion(request);
        }
    }
}
