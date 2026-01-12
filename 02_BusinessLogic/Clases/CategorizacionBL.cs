using _01_DataLogic.Clases;
using _02_BusinessLogic.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Hl7.Fhir.Model.CarePlan;

namespace _02_BusinessLogic.Clases
{
    public class CategorizacionBL : ICategorizacionBL
    {
        public string test()
        {
            CategorizacionDAL oCateDal = new CategorizacionDAL();
            return oCateDal.test();
        }




        /*public async Task<EventResponse> enviarBundle(string Json, string tipo)
        {
            EventResponse resp = null;
            try
            {
                HttpClient _httpClient = new HttpClient();
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var url = config["UrlFhir:urlApiTeio"] + "/token";

                var requestData = new Dictionary<string, string>
                {
                    { "grant_type", config["UrlFhir:urlTeioGrantType"] },
                    { "username", config["UrlFhir:urlTeioUsername"] },
                    { "password", config["UrlFhir:urlTeioPassword"] }
                };

                var content = new FormUrlEncodedContent(requestData);
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var token = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (token.access_token == "" || token.access_token == null)
                    {
                        throw new Exception($"Error en la autenticación: authResponse");
                    }
                    else
                    {
                        string fullUrl = config["UrlFhir:urlApiTeio"] + "/api/event/" + tipo;
                        var contentUrl = new StringContent(JsonSerializer.Serialize(Json), Encoding.UTF8, "application/json");
                        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);
                        var responseToken = await _httpClient.PostAsync(fullUrl, contentUrl);
                        var resultToken = await responseToken.Content.ReadFromJsonAsync<EventResponse>();
                        resp = new EventResponse();

                        if (resultToken.IdRegistroLog >= 0)
                        {
                            resp = resultToken;
                        }
                        else
                        {
                            resp.IdRegistroLog = 0;
                            resp.IdEstado = 99;
                            resp.Descripcion = "Error interno";
                            resp.Respuesta = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resp = new EventResponse();
                resp.IdRegistroLog = 0;
                resp.IdEstado = 99;
                resp.Descripcion = "Error interno";
                resp.Respuesta = null;
            }

            return resp;
        }*/

        
        /*public string teest2()
        {
            try
            {
                var ahora = DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, 0), DateTimeKind.Utc);



                string bundleGuardado = apiTeio.obtenerBundleEnriquecido(interconsultaCorr);
                if (bundleGuardado == null || bundleGuardado == "")
                {
                    return "error";
                }

                var parser = new FhirJsonParser();
                Bundle bundleEnriquecido = parser.Parse<Bundle>(bundleGuardado);


                var bundle = new Bundle
                {
                    Id = Guid.NewGuid().ToString(),
                    Meta = new Meta
                    {
                        Profile = ["https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/BundleAtenderLE"]
                    },
                    Type = Bundle.BundleType.Message,
                    Timestamp = new DateTimeOffset(ahora),
                    Entry = new List<Bundle.EntryComponent>()
                };

                string urnMesssageHeader = Guid.NewGuid().ToString();
                //string urnServiceRequest = Guid.NewGuid().ToString();
                string urnServiceRequest = datos.IdServiceRequest;
                string urnEncounter = datos.idEncounter.Split('/')[1] ?? "";
                string urnCareplan = Guid.NewGuid().ToString();
                string urnDiagnosis = Guid.NewGuid().ToString();
                //string urnRolProfesional = datosAtender.rolCorrAgendador.ToString();
                string urnRolProfesional = datos.IdPractitionerRole;
                string urnProfesional = datos.idRequester.Split('/')[1] ?? "";
                string urnEstablecimiento = datos.IdOrganization;

                // MessageHeader ************************************************************************************************************************************************************
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    FullUrl = "https://interoperabilidad.minsal.cl/fhir/ig/tei/MessageHeader/" + urnMesssageHeader,
                    Resource = new MessageHeader
                    {
                        Id = urnMesssageHeader,
                        Meta = new Meta
                        {
                            LastUpdated = new DateTimeOffset(ahora),
                        },
                        Event = new Coding
                        {
                            System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSTipoEventoLE",
                            Code = "atender",
                            Display = "Atender"
                        },
                        Author = new ResourceReference
                        {
                            Reference = "PractitionerRole/" + urnRolProfesional
                        },
                        Source = new MessageSourceComponent
                        {
                            Endpoint = "http://pulso.hpcordillera.cl",
                            Software = "PULSO"
                        },
                        Focus = new List<ResourceReference>
                        {
                            new ResourceReference()
                            {
                                Reference = "ServiceRequest/"+urnServiceRequest
                            }
                        }
                    }
                });

                // ServiceRequest ************************************************************************************************************************************************************
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
                                Value = datosAtender.idMinsal
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
                            new Hl7.Fhir.Model.Extension
                            {
                                Url = "https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/ExtensionEstadoInterconsultaCodigoLE",
                                Value = new CodeableConcept
                                {
                                    Coding = new List<Coding>()
                                    {
                                        new Coding
                                        {
                                            System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSEstadoInterconsulta",
                                            Code = "6",
                                            Display = "A la espera de cierre"
                                        }

                                    }
                                }
                            }
                        },
                        Subject = new ResourceReference()
                        {
                            Reference = datos.idPatient
                        },
                        Performer = new List<ResourceReference>()
                        {
                            new ResourceReference()
                            {
                                Reference = "PractitionerRole/" + urnRolProfesional
                            }
                        }

                    }
                });

                // PRACTITIONER ************************************************************************************************************************************************************
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
                                Value = datosAtender.usuRutAgendador.ToString() +"-"+ datosAtender.usuDvAgendador.ToString(),
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
                                Family = datosAtender.usuApepatAgendador,
                                Given = [datosAtender.usuNombreAgendador],
                                Prefix = [datosAtender.pflNombreAgendador]
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

                // ENCOUNTER   ************************************************************************************************************************************************
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
                            Reference = datos.idPatient
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
                                    //FALTA
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

                // careplan ************************************************************************************************************************************************************
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    FullUrl = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CarePlan/" + urnCareplan,
                    Resource = new CarePlan()
                    {
                        Id = urnCareplan,
                        Status = RequestStatus.Active,
                        Intent = CarePlanIntent.Plan,
                        Description = atencion.ateIndicacion,
                        Extension = new List<Extension>()
                        {
                            new Extension()
                            {
                                Url = "https://interoperabilidad.minsal.cl/fhir/ig/tei/StructureDefinition/ExtensionSolicitudExamenes",
                                Value = new FhirBoolean(true)
                            }
                        },
                        Subject = new ResourceReference()
                        {
                            Reference = datos.idPatient
                        },
                        Encounter = new ResourceReference()
                        {
                            Reference = "Encounter/" + urnEncounter
                        },
                    }
                });

                // PRACTITIONER ROLE ************************************************************************************************************************************************************
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
                            Reference = "Organization/" + datos.IdOrganization
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

                // Organization -----------------------------------------------------------------------------------------------------------
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    FullUrl = "https://interoperabilidad.minsal.cl/fhir/ig/tei/Organization/" + datos.IdOrganization,
                    Resource = new Organization()
                    {
                        Id = urnEstablecimiento,
                        Identifier = new List<Identifier>()
                        {
                            new Identifier()
                            {
                                System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/CodeSystem/CSEstablecimientoDestino",
                                Value = datosAtender.corrEstAgendador.ToString()
                            }
                        },
                        Name = datosAtender.nombreEstAgendador
                    }
                });

                // Condition --------------------------------------------------------------------------------------------------------------
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
                            Text = atencion.ateDiagnoText,
                            Coding = new List<Coding>()
                            {
                                new Coding()
                                {
                                    System = "https://interoperabilidad.minsal.cl/fhir/ig/tei/ValueSet/VSTerminologiasDiag",
                                    Code = atencion.ateDiagno.ToString(),
                                    Display = atencion.ateDiagnoText
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
                            Reference = datos.idPatient
                        },
                        Encounter = new ResourceReference()
                        {
                            Reference = "Encounter/" + urnEncounter
                        }
                    }
                });


                Json = new FhirJsonSerializer().SerializeToString(bundle);
                Json = Json.Replace("+00:00", "Z");


                if (!string.IsNullOrEmpty(Json))
                {

                    logBundleRecibo = new LogBundleEN()
                    {
                        estado = 1,
                        bundle = Json,
                        respuesta = "",
                        tipoEventoCorr = 6,
                        tipoEventoDesc = "atender",
                        idMinsal = datosAtender.idMinsal,
                        observacion = "Bundle Atender Enviado correctamente!",
                        interconsultaCorr = interconsultaCorr
                    };

                    EventResponse envio = await _envioBundle.enviarBundle(Json, "atender");
                    var resultToken = JsonSerializer.Serialize(envio);


                    if (envio.IdRegistroLog > 0)
                    {
                        logBundleRecibo.respuesta = resultToken;
                    }


                    resp = resultToken;
                }
                else
                {
                    logBundleRecibo = new LogBundleEN()
                    {
                        estado = 1,
                        bundle = Json,
                        respuesta = "",
                        tipoEventoCorr = 6,
                        tipoEventoDesc = "atender",
                        idMinsal = datosAtender.idMinsal,
                        observacion = "El JSON es nulo, vacío o inválido!",
                        interconsultaCorr = interconsultaCorr
                    };


                    EventResponse er = new EventResponse()
                    {
                        IdRegistroLog = 0,
                        IdEstado = 99,
                        Descripcion = "El JSON es nulo, vacío o inválido!",
                        Respuesta = null
                    };

                    resp = JsonSerializer.Serialize(er);
                }
            }
            catch (Exception ex)
            {
                EventResponse er = new EventResponse()
                {
                    IdRegistroLog = 0,
                    IdEstado = 99,
                    Descripcion = "Error interno / Interconsulta no encontrada",
                    Respuesta = null
                };

                LogBundleEN logFail = new LogBundleEN()
                {
                    estado = 1,
                    bundle = Json,
                    respuesta = "",
                    tipoEventoCorr = 6,
                    tipoEventoDesc = "atender",
                    idMinsal = datosAtender.idMinsal,
                    observacion = resp,
                    interconsultaCorr = interconsultaCorr
                };
                logBundleRecibo = logFail;
            }
        }*/
    }
}
