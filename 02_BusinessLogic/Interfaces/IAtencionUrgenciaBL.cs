using _00_Entities.Clases;

namespace _02_BusinessLogic.Interfaces;

public interface IAtencionUrgenciaBL
{

    /*
    {
      "run": "19221773-3",
      "fecNac": "1995-05-25",
      "telefono": "56 9 8765 4321",
      "etniaCode": "07",
      "etniaDisplay": "Diaguita",
      "modalidadDisplay": "Emergencia",
      "modalidadCode": "EMER",
      "urgenciacode": "114105",
      "urgencianame": "Servicio de Urgencia del Hospital Base Mos Eisley",
      "nombrePaciente": "Luke",
      "apellidoPat": "Skywalker",
      "apellidoMat": "Lars",
      "line": "Granja de Humedad 42",
      "city": "Anchorhead",
      "district": "Huara",
      "state": "Tarapaca",
      "country": "CL",
      "nombremedico": "Lando",
      "apellidomedico": "Calrissian Antilles",
      "runMedico": "11111111-1"
    }
    */
    public string Admision(DatosAdmisionEN request);

    public string  Categorizacion(DatosCategorizacionEN request);
    public string Atencion(DatosAtencionEN request);
    public string Egreso(DatosEgresoEN request);
}