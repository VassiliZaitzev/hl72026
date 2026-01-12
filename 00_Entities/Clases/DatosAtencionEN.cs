using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace _00_Entities.Clases
{
    public class DatosAtencionEN
    {  
        public string nombrePaciente { get; set; }
        public string apellidoPat {  get; set; }
        public string apellidoMat {  get; set; }
        public string comuna { get; set; }
        public string derivadaDesde { get; set; }
        public string derivadaDesdeCorr { get; set; }
        public string derivadaHacia { get; set; }
        public string derivadaHaciaCorr { get; set; }
        public string sintoma { get; set; }
        public string medico { get; set; }
        public string diagnostico { get; set; }
        public string intervencionElectiva { get; set; }
        public string solicitudIq { get; set; }
        public string prioridad { get; set; }
        public string prioridadCorr { get; set; }
        public string entrevistaPrequirurgica { get; set; }
        public string tipoServicio { get; set; }
        public string especialidad { get; set; }
        public string organizacion { get; set; }
        public string organizacionCorr { get; set; }
        public string profesionalTentativo { get; set; }
        public string citaProgramada { get; set; }
        public string rol { get; set; }
        public string diagnosticoCorr { get; set; }
        public string estado { get; set; }
    }
}