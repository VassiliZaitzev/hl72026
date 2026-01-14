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
    public class DatosAdmisionEN
    {
        public string run { get; set; }
        public string fecNac { get; set; }
        public string telefono { get; set; }
        public string etniaCode { get; set; }
        public string etniaDisplay { get; set; }
        public string modalidadSystem { get; set; }
        public string modalidadDisplay { get; set; }
        public string modalidadCode { get; set; }
        public string urgenciacode { get; set; }
        public string urgencianame { get; set; }
        public string nombrePaciente { get; set; }
        public string apellidoPat {  get; set; }
        public string apellidoMat {  get; set; }
        public string Line {  get; set; }
        public string City{  get; set; } 
        public string District {  get; set; }
        public string State {  get; set; }
        public string Country {  get; set; }
        public string nombremedico { get; set; }
        public string apellidomedico { get; set; }
        public string runMedico { get; set; }
 
    }
}