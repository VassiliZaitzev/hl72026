using _00_Entities.Clases;

namespace _02_BusinessLogic.Interfaces;

public interface IAtencionUrgenciaBL
{
    public string Admision(DatosAdmisionEN request);
    public string  Categorizacion();
    public string Atencion();
    public string Egreso();
}