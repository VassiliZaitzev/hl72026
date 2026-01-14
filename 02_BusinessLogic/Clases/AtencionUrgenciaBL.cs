using _02_BusinessLogic.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using _00_Entities.Clases;
using _02_BusinessLogic.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hl7.Fhir.Model.Encounter;
using static Hl7.Fhir.Model.HumanName;
using static Hl7.Fhir.Model.Identifier;
using static Hl7.Fhir.Model.MessageHeader;
using static Hl7.Fhir.Model.Practitioner;

namespace _02_BusinessLogic.Clases;

public class AtencionUrgenciaBL : IAtencionUrgenciaBL
{
    public string Admision(DatosAdmisionEN request)
    {
        string Json = "";
        try
        {
            var ahora = DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, 0), DateTimeKind.Utc);

            var bundle = new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta
                {
                    Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/BundleAdmision"]
                },
                Type = Bundle.BundleType.Transaction,
                Timestamp = new DateTimeOffset(ahora),
                Entry = new List<Bundle.EntryComponent>()
            };

            //URN
            string urnRolProfesional = Guid.NewGuid().ToString();
            //string urnServiceRequest = Guid.NewGuid().ToString();
            string urnProfesional = Guid.NewGuid().ToString();
            string urnEncounter = Guid.NewGuid().ToString();
            string idPatient = Guid.NewGuid().ToString();
            string urnDiagnosis = Guid.NewGuid().ToString();
            string urnEstablecimiento = Guid.NewGuid().ToString();
            
             //PATIENT
            bundle.Entry.Add(new Bundle.EntryComponent()
            {
                FullUrl = "urn:uuid:" + idPatient,
                Resource = new Patient()
                {
                    Id = idPatient,
                    Meta = new Meta()
                    {
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/PatientUrg"]
                    },
                    Identifier = new List<Identifier>() {
                        new Identifier()
                        {
                            Use = Identifier.IdentifierUse.Official,
                            Type = new CodeableConcept()
                            {
                                Coding = new List<Coding>()
                                {
                                    new Coding
                                    {
                                        System = "https://hl7chile.cl/fhir/ig/clcore/CodeSystem/CSTipoIdentificador",
                                        Code = "01",
                                        Display = "RUN"
                                    }
                                },
                                Text = "Rol Único Nacional"
                            },
                            Value = request.run,
                            Assigner = new ResourceReference()
                            {
                                Display = "Republica de Chile"
                            }
                        }
                    },
                    Active = true,
                    Name = new List<HumanName>
                    {
                        new HumanName()
                        {
                            Family = request.apellidoPat + " " + request.apellidoMat,
                            Given = [request.nombrePaciente],
                            Prefix = ["Paciente"],
                            Use = HumanName.NameUse.Official
                        }
                    },
                    Gender = AdministrativeGender.Male,
                    BirthDate = request.fecNac,
                    Telecom =
                    {
                        new ContactPoint
                        {
                            System = ContactPoint.ContactPointSystem.Phone,
                            Value = request.telefono,
                            Use = ContactPoint.ContactPointUse.Mobile
                        }
                    },
                    Extension =
                    {
                        new Extension
                        {
                            Url = "https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/PuebloOriginario",
                            Value = new CodeableConcept
                            {
                                Coding = new List<Coding>()
                                {
                                    new Coding
                                    {
                                        System = "https://api.minsal.cl/codigos-pueblos-originarios",
                                        Code = request.etniaCode,
                                        Display = request.etniaDisplay
                                    }

                                }
                            }
                        }
                    },
                    Address = new List<Address>
                    {
                        new Address
                        {
                            Use = Address.AddressUse.Home,
                            Type = Address.AddressType.Physical,
                            Line = new[]
                            {
                                request.Line, //"Granja de Humedad 42"
                            },
                            City = request.City, //"Anchorhead",
                            District = request.District, //"Huara",
                            State = request.State, //"Tarapacá",
                            Country = request.Country //"CL"
                        }
                    }
                },
                Request = new Bundle.RequestComponent()
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Patient",
                    IfNoneExist = "19221773-3"
                }
            });
            
            
            var fechaLlegada = new DateTimeOffset(
                2024, 12, 25, 13, 18, 0,
                TimeSpan.FromHours(-3) // Chile continental
            );
            
            //ENCOUNTER
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnEncounter,
                Resource = new Encounter()
                {
                    Id = urnEncounter,
                    Meta = new Meta()
                    {
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/EncounterUrg"]
                    },
                    Status = Encounter.EncounterStatus.InProgress,
                    Class = new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                        Code = request.modalidadCode,
                        Display = request.modalidadDisplay
                    },
                    Identifier = new List<Identifier>()
                    {
                        new Identifier
                        {
                            System = "https://interoperabilidad.minsal.cl/fhir/ig/urgencia/identifier/numero-atencion",
                            Value = urnEncounter
                        }
                    },
                    Subject = new ResourceReference()
                    {
                        Reference = "urn:uuid:" + idPatient
                    },
                    Period = new Period()
                    {
                        Start = fechaLlegada.ToString("o")
                    },
                    
                    Extension = new List<Extension>()
                    {
                        new Extension()
                        {
                            Url = "https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/Acompanante",
                            Value = new FhirBoolean(false)
                        }
                    },
                    // Identifier = new List<Identifier>()
                    // {
                    //     new Identifier()
                    //     {
                    //         //System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSEstablecimientoDestino",
                    //         Value = urnEncounter
                    //     }
                    // },
                    Participant =
                    {
                        new Encounter.ParticipantComponent
                        {
                            Individual = new ResourceReference()
                            {
                                Reference = "urn:uuid:"  + urnProfesional
                            },
                            Type =
                            {
                                new CodeableConcept(
                                    "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                                    "ADM",
                                    "admitter"
                                )
                            }
                        }
                    },
                },
                Request = new Bundle.RequestComponent()
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Encounter"
                }

            });
            //ORGANIZATION
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:"+ urnEstablecimiento,
                Resource = new Organization()
                {
                    Id = urnEstablecimiento,
                    Meta = new Meta()
                    {
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/EstablecimientoUrg"]
                    },
                    Identifier = new List<Identifier>()
                    {
                        new Identifier()
                        {
                            System = "https://deis.minsal.cl",
                            Value = request.urgenciacode
                        }
                    },
                    Name = request.urgencianame
                },
                Request = new Bundle.RequestComponent()
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Organization"
                }
            });
            //PRACTITIONER
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnProfesional,
                Resource = new Practitioner()
                {
                    Id = urnProfesional,
                    Meta = new Meta()
                    {
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/PrestadorAdministrativo"]
                    },
                    Identifier = new List<Identifier>() {
                        new Identifier()
                        {
                            Use = Identifier.IdentifierUse.Official,
                            Type = new CodeableConcept()
                            {
                                Coding = new List<Coding>()
                                {
                                    new Coding
                                    {
                                        System = "https://hl7chile.cl/fhir/ig/clcore/CodeSystem/CSTipoIdentificador",
                                        Code = "01",
                                        Display = "RUN"
                                    }
                                },
                                Text = "Rol Único Nacional"
                            },
                            Value = request.runMedico,
                            Assigner = new ResourceReference()
                            {
                                Display = "Republica de Chile"
                            }
                        }
                    },
                    Active = true,
                    Name = new List<HumanName>
                    {
                        new HumanName()
                        {
                            Family = request.apellidomedico,
                            Given = [request.nombremedico],
                            Prefix = ["Dr"]
                        }
                    },
                    Gender = AdministrativeGender.Male
                },
                Request = new Bundle.RequestComponent()
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Practitioner"
                }
            });
            //PRACTITIONER ROLE
            /*
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnRolProfesional,
                Resource = new PractitionerRole()
                {
                    Id = urnRolProfesional,
                    Meta = new Meta()
                    {
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/PractitionerRoleLE"]
                    },
                    Active = true,
                    Practitioner = new ResourceReference()
                    {
                        Reference = "urn:uuid:" + urnProfesional
                    },
                    Organization = new ResourceReference()
                    {
                        Reference = "urn:uuid:"  + urnEstablecimiento
                    },
                    Code = new List<CodeableConcept>()
                    {
                        new CodeableConcept()
                        {
                            Coding = new List<Coding>()
                            {
                                new Coding()
                                {
                                    System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSPractitionerTipoRolLE",
                                    Code = "atendedor",
                                    Display = "Atendedor"
                                }
                            }
                        }
                    }
                }

            });
            */
            
    
            Json = new FhirJsonSerializer().SerializeToString(bundle);
            Json = Json.Replace("+00:00", "Z");

        }
        catch (Exception ex)
        {
            Json = ex.Message;
        }

        return Json;
    
    
    }

    public string Categorizacion()
    {
        string resp = "";
        try
        {

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return resp;
    }

    public string Atencion()
    {
        string resp = "";
        try
        {

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return resp;
    }

    public string Egreso()
    {
        string resp = "";
        try
        {

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return resp;
    }
}