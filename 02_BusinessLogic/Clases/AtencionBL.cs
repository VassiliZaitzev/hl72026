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

namespace _02_BusinessLogic.Clases
{
    public class AtencionBL : IAtencionBL
    {
        
        public string recibirAtencion(DatosAtencionEN request)
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
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/BundleAtenderLE"]
                    },
                    Type = Bundle.BundleType.Transaction,
                    Timestamp = new DateTimeOffset(ahora),
                    Entry = new List<Bundle.EntryComponent>()
                };

                //URN
                string urnRolProfesional = Guid.NewGuid().ToString();
                string urnServiceRequest = Guid.NewGuid().ToString();
                string urnProfesional = Guid.NewGuid().ToString();
                string urnEncounter = Guid.NewGuid().ToString();
                string idPatient = Guid.NewGuid().ToString();
                string urnDiagnosis = Guid.NewGuid().ToString();
                string urnEstablecimiento = Guid.NewGuid().ToString();
                

                //ENCOUNTER
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    FullUrl = "https://interoperabilidad.minsal.cl/fhir/ig/tei/Encounter/" + urnEncounter,
                    Resource = new Encounter()
                    {
                        Id = urnEncounter,
                        Meta = new Meta()
                        {
                            Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/EncounterAtenderLE"]
                        },
                        Status = EncounterStatus.Finished,
                        Class = new Coding()
                        {
                            System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSModalidadAtencionCodigo",
                            Code = "1",
                            Display = "Presencial"
                        },
                        Type = new List<CodeableConcept>()
                        {
                            new CodeableConcept()
                            {
                                Coding = new List<Coding>()
                                {
                                    new Coding()
                                    {
                                        System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSTipoConsulta",
                                        Code = "1",
                                        Display = "Nueva"
                                    }
                                }
                            }
                        },
                        Subject = new ResourceReference()
                        {
                            Reference = idPatient
                        },
                        Period = new Period()
                        {
                            Start = "2024-02-22T08:00:00-03:00",
                            End = "2024-02-22T08:30:00-03:00"
                        },

                        Diagnosis = new List<DiagnosisComponent>()
                        {
                            new DiagnosisComponent()
                            {
                                Condition = new ResourceReference()
                                {                                    
                                    Reference = "Condition/" + urnDiagnosis
                                }
                            }
                        },
                        Extension = new List<Extension>()
                        {
                            new Extension()
                            {
                                Url = "https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/ExtensionPertinenciaAtencionBox",
                                Value = new FhirBoolean(true)
                            },
                            new Extension
                            {
                                Url = "https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/ExtensionConsecuenciaAtencionCodigo",
                                Value = new CodeableConcept
                                {
                                    Coding = new List<Coding>()
                                    {
                                        new Coding
                                        {
                                            System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSConsecuenciaAtencionCodigo",
                                            Code = "1",
                                            Display = "Presencial"
                                        }

                                    }
                                }
                            }
                        },
                        Identifier = new List<Identifier>()
                        {
                            new Identifier()
                            {
                                //System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSEstablecimientoDestino",
                                Value = urnEncounter
                            }
                        },
                        BasedOn = new List<ResourceReference>()
                        {
                            new ResourceReference()
                            {
                                Reference = "ServiceRequest/" + urnServiceRequest
                            }
                        }
                    }

                });
                //ORGANIZATION
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    FullUrl = "https://interoperabilidad.minsal.cl/fhir/ig/tei/Organization/" + urnEstablecimiento,
                    Resource = new Organization()
                    {
                        Id = urnEstablecimiento,
                        Identifier = new List<Identifier>()
                        {
                            new Identifier()
                            {
                                System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSEstablecimientoDestino",
                                Value = request.derivadaDesdeCorr
                            }
                        },
                        Name = request.derivadaDesde
                    }
                });
                //PRACTITIONER
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    FullUrl = "https://interoperabilidad.minsal.cl/fhir/ig/tei/Practitioner/" + urnProfesional,
                    Resource = new Practitioner()
                    {
                        Id = urnProfesional,
                        Meta = new Meta()
                        {
                            Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/PractitionerProfesionalLE"]
                        },
                        Identifier = new List<Identifier>() {
                            new Identifier()
                            {
                                Use = IdentifierUse.Official,
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
                                Value = "19.221.773-3",
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
                                Family = "",
                                Given = [request.medico],
                                Prefix = ["Dr"]
                            }
                        },
                        Gender = AdministrativeGender.Male,
                        BirthDate = "1980-01-01",
                        Qualification = new List<QualificationComponent>()
                        {
                            new QualificationComponent()
                            {
                                Identifier = new List<Identifier>()
                                {
                                    new Identifier()
                                    {
                                        Value = "cert"
                                    }
                                },
                                Code = new CodeableConcept(){
                                    Coding = new List<Coding>()
                                    {
                                        new Coding()
                                        {
                                            System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSTituloProfesional",
                                            Code = "1",
                                            Display = "MÉDICO CIRUJANO"
                                        }
                                    },
                                    Text = "MÉDICO CIRUJANO"
                                },
                                Period = new Period(){
                                    Start = "2007-11-01"
                                },
                                Issuer = new ResourceReference(){
                                    Display = "Universidad de Chile"
                                }
                            }
                        }
                    }
                });
                //PRACTITIONER ROLE
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    FullUrl = "https://interoperabilidad.minsal.cl/fhir/ig/tei/PractitionerRole/" + urnRolProfesional,
                    Resource = new PractitionerRole()
                    {
                        Id = urnRolProfesional,
                        Meta = new Meta()
                        {
                            VersionId = "2.0",
                            Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/PractitionerRoleLE"]
                        },
                        Active = true,
                        Practitioner = new ResourceReference()
                        {
                            Reference = "Practitioner/" + urnProfesional
                        },
                        Organization = new ResourceReference()
                        {
                            Reference = "Organization/" + request.derivadaHacia
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


                //PATIENT
                bundle.Entry.Add(new Bundle.EntryComponent()
                {
                    FullUrl = "",
                    Resource = new Patient()
                    {
                        Id = "",
                        Identifier = new List<Identifier>() {
                            new Identifier()
                            {
                                Use = IdentifierUse.Official,
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
                                Value = "19.221.773-3",
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
                                Use = NameUse.Official
                            }
                        },
                        Gender = AdministrativeGender.Male,
                        BirthDate = "1980-01-01",
                    }
                });

                //CONDITION
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    FullUrl = "https://interoperabilidad.minsal.cl/fhir/ig/tei/Condition/" + urnDiagnosis,
                    Resource = new Condition()
                    {
                        Id = urnDiagnosis,
                        Meta = new Meta()
                        {
                            Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/ConditionDiagnosticoLE"]
                        },
                        Code = new CodeableConcept()
                        {
                            Text = request.diagnostico,
                            Coding = new List<Coding>()
                            {
                                new Coding()
                                {
                                    System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/ValueSet/VSTerminologiasDiag",
                                    Code = request.diagnosticoCorr,
                                    Display = request.diagnostico
                                }
                            }
                        },
                        ClinicalStatus = new CodeableConcept()
                        {
                            Text = "Activo",
                            Coding = new List<Coding>()
                            {
                                new Coding()
                                {
                                    System = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                                    Code = "active",
                                    Display = "Active"
                                }
                            }
                        },
                        VerificationStatus = new CodeableConcept()
                        {
                            Text = "Confirmado",
                            Coding = new List<Coding>()
                            {
                                new Coding()
                                {
                                    System = "http://terminology.hl7.org/CodeSystem/condition-ver-status",
                                    Code = "confirmed",
                                    Display = "Confirmed"
                                }
                            }
                        },
                        Category = new List<CodeableConcept>()
                        {
                            new CodeableConcept()
                            {
                                Text = "Diagnostico del encuentro",
                                Coding = new List<Coding>()
                                {
                                    new Coding()
                                    {
                                        System = "http://terminology.hl7.org/CodeSystem/condition-category",
                                        Code = "encounter-diagnosis",
                                        Display = "Encounter Diagnosis"
                                    }
                                }
                            }
                        },
                        Subject = new ResourceReference()
                        {
                            Reference = idPatient
                        },
                        Encounter = new ResourceReference()
                        {
                            Reference = "Encounter/" + urnEncounter
                        }
                    }
                });

                //SERVICE REQUEST
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    FullUrl = "https://interoperabilidad.minsal.cl/fhir/ig/tei/ServiceRequest/" + urnServiceRequest,
                    Resource = new ServiceRequest
                    {
                        Id = urnServiceRequest,
                        Identifier = new List<Identifier>()
                        {
                            new Identifier()
                            {
                                System = "http://www.minsal.cl/MINSAL", // Se debe de extraer desde el serviceRequest del segundo entry
                                Value = "datosAtender.idMinsal"
                            }
                        },
                        Meta = new Meta
                        {
                            Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/ServiceRequestLE"]
                        },
                        Status = RequestStatus.Draft,
                        Intent = RequestIntent.Order,
                        Extension = new List<Extension>()
                        {
                           
                        },
                        Subject = new ResourceReference()
                        {
                            Reference = idPatient
                        },
                        Performer = new List<ResourceReference>()
                        {
                            new ResourceReference()
                            {
                                Reference = "PractitionerRole/" + urnRolProfesional
                            }
                        }
                        /*SupportingInfo = new List<ResourceReference>()
                        {                            
                            new ResourceReference()
                            {
                                Reference = "AllergyIntolerance/AllergyIntoleranceExample"
                            },
                            new ResourceReference()
                            {
                                Reference = "Observation/AnticuerpoAdrenal"
                            }
                        }*/
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
}
