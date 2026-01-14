using _00_Entities.Clases;
using _02_BusinessLogic.Interfaces;
using _02_BusinessLogic.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Hl7.Fhir.Model.Encounter;
using static Hl7.Fhir.Model.HumanName;
using static Hl7.Fhir.Model.Identifier;
using static Hl7.Fhir.Model.MessageHeader;
using static Hl7.Fhir.Model.Practitioner;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    Text = new Narrative
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns=\"http://www.w3.org/1999/xhtml\">Paciente Luke Skywalker</div>"
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
                            Url = "https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/PueblosOriginariosPerteneciente",
                            Value = new FhirBoolean(false)
                        },
                        new Extension
                        {
                            Url = "https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/PueblosOriginarios",
                            Value = new CodeableConcept
                            {
                                Coding = new List<Coding>()
                                {
                                    new Coding
                                    {
                                        System = "https://interoperabilidad.minsal.cl/fhir/ig/urgencia/CodeSystem/PueblosOriginariosCS",
                                        Code = request.etniaCode,
                                        Display = request.etniaDisplay
                                    }

                                }
                            }
                        }
                    },
                    /*
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
                    },
                    */

                    Address = new List<Address>() 
                    {
                        new Address
                        {
                            Use = Address.AddressUse.Home,
                            Type = Address.AddressType.Physical,
                            Text = "Granja de Humedad 42, Anchorhead, Huara, Tarapaca",
                            Line = new[]
                            {
                                "Granja de Humedad 42"
                            },
                            City = "Huara",
                            CityElement = new FhirString("Huara")
                            {
                                Extension = new List<Extension>
                                {
                                    new Extension
                                    {
                                        Url = "https://hl7chile.cl/fhir/ig/clcore/StructureDefinition/ComunasCl",
                                        Value = new CodeableConcept
                                        {
                                            Coding = new List<Coding>()
                                            {
                                                new Coding
                                                {
                                                    System = "https://hl7chile.cl/fhir/ig/clcore/CodeSystem/CSCodComunasCL",
                                                    Code = "01404",
                                                    Display = "Huara"
                                                }

                                            }
                                        }
                                    }

                                }
                            },
                            District = "Anchorhead",
                            DistrictElement = new FhirString("Anchorhead")
                            {
                                Extension = new List<Extension>
                                {
                                    new Extension(
                                        "https://hl7chile.cl/fhir/ig/clcore/StructureDefinition/ProvinciasCl",
                                        new CodeableConcept(
                                            "https://hl7chile.cl/fhir/ig/clcore/CodeSystem/CSCodProvinciasCL",
                                            "014"
                                            // ?????? "Anchorhead"
                                        )
                                    )
                                }
                            },
                            State = "Región de Tarapacá",
                            StateElement = new FhirString("Región de Tarapacá")
                            {
                                Extension = new List<Extension>
                                {
                                    new Extension(
                                        "https://hl7chile.cl/fhir/ig/clcore/StructureDefinition/RegionesCl",
                                        new CodeableConcept(
                                            "https://hl7chile.cl/fhir/ig/clcore/CodeSystem/CSCodRegionCL",
                                            "01",
                                            "Tarapacá"
                                        )
                                    )
                                }
                            },

                            Country = "CL",
                            CountryElement = new FhirString("Chile")
                            {
                                Extension = new List<Extension>
                                {
                                    new Extension
                                    {
                                        Url = "https://hl7chile.cl/fhir/ig/clcore/StructureDefinition/CodigoPaises",
                                        Value = new CodeableConcept
                                        {
                                            Coding = new List<Coding>()
                                            {
                                                new Coding
                                                {
                                                    System = "urn:iso:std:iso:3166",
                                                    Code = "152",
                                                    Display = "Chile"
                                                }

                                            }
                                        }
                                    }
                                    
                                }
                            },
                        }
                    },
                    Deceased = new FhirBoolean(false)
                },
                Request = new Bundle.RequestComponent()
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Patient",
                    IfNoneExist = "identifier="+idPatient
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
                    Text = new Narrative()
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Encuentro de urgencia para el paciente iniciado.</div>"
                    },
                    Status = Encounter.EncounterStatus.Arrived,
                    StatusHistory = new List<StatusHistoryComponent>()
                    {
                        new StatusHistoryComponent
                        {
                            Status = EncounterStatus.Arrived,
                            Period = new Period()
                            {
                                Start = "2024-10-25T13:18:00-04:00"
                            }
                        }
                    },
                    Class = new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                        Code = request.modalidadCode,
                        Display = request.modalidadDisplay
                    },
                    Priority = new CodeableConcept()
                    {
                        Coding = new List<Coding>()
                        {
                            new Coding
                            {
                                System = "https://interoperabilidad.minsal.cl/fhir/ig/urgencia/CodeSystem/categorizacion-no-realizada",
                                Code = "99",
                                Display = "Sin Categorizar"
                            }

                        }
                    },
                    ServiceProvider = new ResourceReference()
                    {
                        Reference = "urn:uuid:" + urnEstablecimiento
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
                            },
                            Period = new Period()
                            {
                                Start = "2024-10-25T13:18:00-04:00"
                            }
                        }
                    },
                    /*ReasonCode = new List<CodeableConcept>()
                    {
                        new CodeableConcept()
                        {
                            Coding = new List<Coding>()
                            {
                                new Coding
                                {
                                    System = "http://snomed.info/sct",
                                    Code = "65363002",
                                    Display = "Sin Categorizar"
                                }

                            }
                        }
                    }*/
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
                    Text = new Narrative(){
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Encuentro de urgencia para Organizacion.</div>"
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
                    Url = "Organization",
                    IfNoneExist = "identifier=" + urnEstablecimiento
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
                    Text = new Narrative()
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Encuentro de urgencia para Atendedor.</div>"
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
                    Url = "Practitioner",
                    IfNoneExist = "identifier=" + urnProfesional
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

    public string Categorizacion(DatosCategorizacionEN request)
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
                    Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/BundleCategorizacion"]
                },
                Type = Bundle.BundleType.Transaction,
                Timestamp = new DateTimeOffset(ahora),
                Entry = new List<Bundle.EntryComponent>()
            };

            string urnProfesional = Guid.NewGuid().ToString();
            string urnCategorizacion = Guid.NewGuid().ToString();
            string idPatient = Guid.NewGuid().ToString();
            string urnRolProfesional = Guid.NewGuid().ToString();
            string urnEstablecimiento = Guid.NewGuid().ToString();

            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnCategorizacion,
                Resource = new Procedure
                {
                    Id = urnCategorizacion,
                    Meta = new Meta()
                    {
                        //PREGUNTAR
                        Profile = [""]
                    },
                    Text = new Narrative()
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Categorización de triage C3 realizada para el paciente.</div>"
                    },
                    Status = EventStatus.Completed,
                    Code = new CodeableConcept
                    {
                        Text = "Categorización - Triage C3"
                    },
                    Subject = new ResourceReference { 
                        Reference = "urn:uuid:" + idPatient 
                    },
                    Performed = new FhirDateTime("2024-12-25T13:35:00-04:00"), // hora de la categorización
                    Performer = new List<Procedure.PerformerComponent>
                    {
                        new Procedure.PerformerComponent
                        {
                            Actor = new ResourceReference { 
                                Reference = "urn:uuid:" + urnProfesional 
                            }                            
                        }
                    },
                    Note = new List<Annotation>
                    {
                        new Annotation { Text = "Dolor abdominal de 24 horas de evolución. Categoría asignada: C3 - Atención Prioritaria." }
                    }
                 },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Procedure",
                    IfNoneExist = "identifier=" + urnCategorizacion
                }
            });



            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnRolProfesional,
                Resource = new PractitionerRole
                {
                    Id = urnRolProfesional,
                    Meta = new Meta()
                    {
                        //PREGUNTAR
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/PractitionerRole"]
                    },
                    Text = new Narrative()
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns='http://www.w3.org/1999/xhtml'>PractitionerRole.</div>"
                    },
                    Practitioner = new ResourceReference { 
                        Reference = "urn:uuid:" + urnProfesional 
                    },
                    Organization = new ResourceReference { 
                        Reference = "urn:uuid:" + urnEstablecimiento 
                    },
                    Code = new List<CodeableConcept>
                    {
                        new CodeableConcept
                        {
                            Text = "Enfermera categorizadora"
                        }
                    }
                },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "PractitionerRole",
                    IfNoneExist = "identifier=" + urnRolProfesional
                }
            });


            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnProfesional,
                Resource = new Practitioner
                {
                    Id = urnProfesional,     
                    Meta = new Meta()
                    {
                        //PREGUNTAR
                        Profile = [""]
                    },
                    Text = new Narrative()
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Encuentro de urgencia para Atendedor.</div>"
                    },
                    Identifier = new List<Identifier>() {
                        new Identifier()
                        {
                            Use = Identifier.IdentifierUse.Official,
                            Value = "22222222-2",
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
                            Family = "Tano Bonteri",
                            Given = ["Ahsoka"],
                            Prefix = ["Enf"]
                        }
                    },
                    BirthDate = "1987-12-25",
                    Gender = AdministrativeGender.Female
                },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Practitioner",
                    IfNoneExist = "identifier=" + urnProfesional
                }
            });



            var signosVitales = new List<(string nombre, string codigo, string unidad, decimal valor)>
            {
                ("Frecuencia Cardíaca", "8867-4", "/min", 98),
                ("Presión Arterial Sistólica", "8480-6", "mm[Hg]", 130),
                ("Presión Arterial Diastólica", "8462-4", "mm[Hg]", 85),
                ("Temperatura", "8310-5", "Cel", 38.2m),
                ("Frecuencia Respiratoria", "9279-1", "/min", 18),
                ("Saturación O2", "2708-6", "%", 98),
                ("Escala de Dolor (EVA)", "72514-3", "{score}", 8)
            };


            foreach (var sv in signosVitales)
            {
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    FullUrl = "urn:uuid:" + Guid.NewGuid().ToString(),
                    Resource = new Observation
                    {
                        Text = new Narrative()
                        {
                            Status = Narrative.NarrativeStatus.Generated,
                            Div = "<div xmlns='http://www.w3.org/1999/xhtml'>"+ sv.nombre + "</div>"
                        },
                        Status = ObservationStatus.Final,
                        Code = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding { 
                                    Code = sv.codigo, 
                                    Display = sv.nombre 
                                }
                            }
                        },
                        Subject = new ResourceReference { Reference = "urn:uuid:" + idPatient },
                        Effective = new FhirDateTime("2024-12-25T13:35:00-04:00"),
                        Value = new Quantity { Value = sv.valor, Unit = sv.unidad },
                        Performer = new List<ResourceReference>()
                        {
                            new ResourceReference
                            {
                                Reference = "urn:uuid:" + urnProfesional,
                                Display = "Ahsoka Tano Bonteri - Enfermera categorizadora"
                            }
                        }
                    },
                    Request = new Bundle.RequestComponent
                    {
                        Method = Bundle.HTTPVerb.POST,
                        Url = "Observation"
                    }
                });
            }

            Json = new FhirJsonSerializer().SerializeToString(bundle);
            Json = Json.Replace("+00:00", "Z");

        }
        catch (Exception ex)
        {
            Json = ex.Message;
        }

        return Json;
    }

    public string Atencion(DatosAtencionEN request)
    {
        string Json = "";
        try
        {
            var ahoraAtencion = new DateTime(2024, 12, 25, 14, 12, 0, DateTimeKind.Local);

            string urnPractitionerKenobi = Guid.NewGuid().ToString();
            string urnPractitionerAmidala = Guid.NewGuid().ToString();
            string urnPatient = Guid.NewGuid().ToString();
            string urnEncounter = Guid.NewGuid().ToString();
            string urnProcedure = Guid.NewGuid().ToString();
            string urnMedication1 = Guid.NewGuid().ToString();
            string urnMedication2 = Guid.NewGuid().ToString();
            string urnMedication3 = Guid.NewGuid().ToString();
            string urnDiagnosis = Guid.NewGuid().ToString();

            var bundle = new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta
                {
                    Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/BundleAtencion"]
                },
                Type = Bundle.BundleType.Transaction,
                Timestamp = new DateTimeOffset(ahoraAtencion),
                Entry = new List<Bundle.EntryComponent>()
            };

            // Practitioner - Dr. Obi-Wan Kenobi
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnPractitionerKenobi,
                Resource = new Practitioner
                {
                    Id = urnPractitionerKenobi,
                    Name = new List<HumanName>
                    {
                        new HumanName
                        {
                            Given = new string[] { "Obi-Wan" },
                            Family = "Kenobi"
                        }
                    }
                },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Practitioner"
                }
            });

            // Practitioner - Dra. Padmé Amidala Naberrie
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnPractitionerAmidala,
                Resource = new Practitioner
                {
                    Id = urnPractitionerAmidala,
                    Name = new List<HumanName>
                    {
                        new HumanName
                        {
                            Given = new string[] { "Padmé", "Amidala" },
                            Family = "Naberrie"
                        }
                    }
                },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Practitioner"
                }
            });

            // Encounter
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnEncounter,
                Resource = new Encounter
                {
                    Id = urnEncounter,
                    Meta = new Meta
                    {
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/EncounterUrg"]
                    },
                    Text = new Narrative
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Atención clínica de urgencia en box 3 para evaluación de abdomen agudo.</div>"
                    },
                    Status = Encounter.EncounterStatus.InProgress,
                    Period = new Period
                    {
                        Start = "2024-12-25T14:12:00-04:00"
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "urn:uuid:" + urnPatient
                    }
                },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Encounter"
                }
            });

            // Diagnosis - Apendicitis Aguda
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnDiagnosis,
                Resource = new Condition
                {
                    Id = urnDiagnosis,
                    ClinicalStatus = new CodeableConcept { Text = "Activo" },
                    VerificationStatus = new CodeableConcept { Text = "Confirmado" },
                    Code = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding { Code = "K35", Display = "Apendicitis Aguda" }
                        },
                        Text = "Apendicitis Aguda"
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "urn:uuid:" + urnPatient
                    },
                    Encounter = new ResourceReference
                    {
                        Reference = "urn:uuid:" + urnEncounter
                    }
                },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Condition"
                }
            });

            // Procedimientos/Tratamientos iniciales
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnProcedure,
                Resource = new Procedure
                {
                    Id = urnProcedure,
                    Status = EventStatus.Completed,
                    Code = new CodeableConcept
                    {
                        Text = "Tratamiento inicial y manejo preoperatorio",
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "urn:uuid:" + urnPatient
                    },
                    Performed = new FhirDateTime("2024-12-25T14:20:00-04:00"),
                    Performer = new List<Procedure.PerformerComponent>
                    {
                        new Procedure.PerformerComponent
                        {
                            Actor = new ResourceReference
                            {
                                Reference = "urn:uuid:" + urnPractitionerKenobi,
                                Display = "Dr. Obi-Wan Kenobi"
                            }
                        },
                        new Procedure.PerformerComponent
                        {
                            Actor = new ResourceReference
                            {
                                Reference = "urn:uuid:" + urnPractitionerAmidala,
                                Display = "Dra. Padmé Amidala Naberrie"
                            }
                        }
                    },
                    Note = new List<Annotation>
                    {
                        new Annotation { Text = "Administración de Ketoprofeno 100mg IV, Metoclopramida 10mg IV y suero fisiológico 500 cc. Indicaciones preoperatorias establecidas." }
                    }
                },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Procedure"
                }
            });

            // Medicación específica - Ketoprofeno
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnMedication1,
                Resource = new MedicationAdministration
                {
                    Status = MedicationAdministration.MedicationAdministrationStatusCodes.Completed,
                    Medication = new CodeableConcept { Text = "Ketoprofeno 100mg IV" },
                    Subject = new ResourceReference { Reference = "urn:uuid:" + urnPatient },
                    Performer = new List<MedicationAdministration.PerformerComponent>
                    {
                        new MedicationAdministration.PerformerComponent
                        {
                            Actor = new ResourceReference { Reference = "urn:uuid:" + urnPractitionerKenobi, Display = "Dr. Obi-Wan Kenobi" }
                        }
                    },
                    Effective = new FhirDateTime("2024-12-25T14:20:00-04:00")
                },
                Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.POST, Url = "MedicationAdministration" }
            });

            // Medicación específica - Metoclopramida
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnMedication2,
                Resource = new MedicationAdministration
                {
                    Status = MedicationAdministration.MedicationAdministrationStatusCodes.Completed,
                    Medication = new CodeableConcept { Text = "Metoclopramida 10mg IV" },
                    Subject = new ResourceReference { Reference = "urn:uuid:" + urnPatient },
                    Performer = new List<MedicationAdministration.PerformerComponent>
                    {
                        new MedicationAdministration.PerformerComponent
                        {
                            Actor = new ResourceReference { Reference = "urn:uuid:" + urnPractitionerKenobi, Display = "Dr. Obi-Wan Kenobi" }
                        }
                    },
                    Effective = new FhirDateTime("2024-12-25T14:25:00-04:00")
                },
                Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.POST, Url = "MedicationAdministration" }
            });

            // Hidratación IV - Suero Fisiológico
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnMedication3,
                Resource = new MedicationAdministration
                {
                    Status = MedicationAdministration.MedicationAdministrationStatusCodes.Completed,
                    Medication = new CodeableConcept { Text = "Suero Fisiológico 500 cc IV" },
                    Subject = new ResourceReference { Reference = "urn:uuid:" + urnPatient },
                    Performer = new List<MedicationAdministration.PerformerComponent>
                    {
                        new MedicationAdministration.PerformerComponent
                        {
                            Actor = new ResourceReference { Reference = "urn:uuid:" + urnPractitionerAmidala, Display = "Dra. Padmé Amidala Naberrie" }
                        }
                    },
                    Effective = new FhirDateTime("2024-12-25T14:30:00-04:00")
                },
                Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.POST, Url = "MedicationAdministration" }
            });

            Json = new FhirJsonSerializer().SerializeToString(bundle);
            Json = Json.Replace("+00:00", "Z");

        }
        catch (Exception ex)
        {
            Json = ex.Message;
        }


        return Json;
    }

    public string Egreso(DatosEgresoEN request)
    {
        string Json = "";
        try
        {
            var ahora = new DateTime(2024, 12, 25, 15, 23, 0, DateTimeKind.Local);

            var bundle = new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta
                {
                    Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/BundleEgreso"]
                },
                Type = Bundle.BundleType.Transaction,
                Timestamp = new DateTimeOffset(ahora),
                Entry = new List<Bundle.EntryComponent>()
            };

            string urnEgreso = Guid.NewGuid().ToString();
            string urnEncounter = Guid.NewGuid().ToString();
            string urnProfesional = Guid.NewGuid().ToString();
            string idPatient = Guid.NewGuid().ToString();
            string urnEstablecimiento = Guid.NewGuid().ToString();


            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnProfesional,
                Resource = new Practitioner
                {
                    Id = urnProfesional,
                    Meta = new Meta
                    {
                        Profile = []
                    },
                    Text = new Narrative()
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Profeisonal que da egreso del paciente a hospitalización en pabellón quirúrgico.</div>"
                    },
                    Name = new List<HumanName>
                    {
                        new HumanName
                        {
                            Given = new string[] { "Obi-Wan" },
                            Family = "Kenobi"
                        }
                    }
                },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Practitioner"
                }
            });


            // Egreso - Procedure
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnEgreso,
                Resource = new Procedure
                {
                    Id = urnEgreso,
                    Meta = new Meta
                    {
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/ProcedureEgreso"]
                    },
                    Text = new Narrative
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Egreso del paciente a hospitalización en pabellón quirúrgico.</div>"
                    },
                    Status = EventStatus.Completed,
                    Code = new CodeableConcept
                    {
                        Text = "Egreso - Hospitalización",
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                Code = "1",
                                Display = "Hospitalización"
                            }
                        }
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "urn:uuid:" + idPatient
                    },
                    Performed = new FhirDateTime(ahora),
                    Performer = new List<Procedure.PerformerComponent>
                    {
                        new Procedure.PerformerComponent
                        {
                            Actor = new ResourceReference
                            {
                                Reference = "urn:uuid:" + urnProfesional,
                                Display = "Dr. Obi-Wan Kenobi"
                            }
                        }
                    },
                    Note = new List<Annotation>
                    {
                        new Annotation
                        {
                            Text = "Paciente egresado a hospitalización en el mismo establecimiento. Destino: Pabellón Quirúrgico (unidad 501)."
                        }
                    }
                },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Procedure"
                }
            });

            // Encounter finalizado
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = "urn:uuid:" + urnEncounter,
                Resource = new Encounter
                {
                    Id = urnEncounter,
                    Meta = new Meta
                    {
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/urgencia/StructureDefinition/EncounterUrg"]
                    },
                    Text = new Narrative
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Encuentro de urgencia concluido. Paciente trasladado a hospitalización.</div>"
                    },
                    Status = Encounter.EncounterStatus.Finished,
                    Period = new Period
                    {
                        Start = "2024-12-25T13:18:00-04:00", // Hora de llegada
                        End = "2024-12-25T15:23:00-04:00"    // Hora de egreso
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "urn:uuid:" + idPatient
                    }
                },
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Encounter"
                }
            });

            Json = new FhirJsonSerializer().SerializeToString(bundle);
            Json = Json.Replace("+00:00", "Z");

        }
        catch (Exception ex)
        {
            Json = ex.Message;
        }


        return Json;
    }
}