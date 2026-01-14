using _00_Entities.Clases;
using Microsoft.AspNetCore.Mvc;
using _02_BusinessLogic.Clases;
using _02_BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;


namespace HPC_HL7_CONNECTBACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtencionUrgenciaController : Controller
    {
        private readonly IAtencionUrgenciaBL _iatencionUrgenciaBL;
        public AtencionUrgenciaController(IAtencionUrgenciaBL iatencionUrgenciaBL)
        {
            _iatencionUrgenciaBL = iatencionUrgenciaBL;
        }

        // [HttpPost("recibirAtencion")]
        // public string recibirAtencion([FromBody] DatosAtencionEN request)
        // {
        //     return _IAtencionBL.recibirAtencion(request);
        // }
        
        [HttpPost("Admision")]
        public string Admision([FromBody] DatosAdmisionEN request)
        {
            return _iatencionUrgenciaBL.Admision(request);
        }
        
        [HttpPost("Triage")]
        public string Categorizacion([FromBody] DatosAdmisionEN request)
        {
            return _iatencionUrgenciaBL.Categorizacion();
        }
        
        [HttpPost("Atencion")]
        public string Atencion([FromBody] DatosAdmisionEN request)
        {
            return _iatencionUrgenciaBL.Atencion();
        }
        
        [HttpPost("Egreso")]
        public string Egreso([FromBody] DatosAdmisionEN request)
        {
            return _iatencionUrgenciaBL.Egreso();
        }
    }
}
