using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using SIGEST.Models;

namespace SIGEST.Services
{
    /// <summary>
    /// Servicio para gestión de verificación de técnicos
    /// </summary>
    public class TecnicoVerificacionService
    {
        private readonly SigestContext _context;

        public TecnicoVerificacionService()
        {
            _context = new SigestContext();
        }

        public TecnicoVerificacionService(SigestContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Guarda un documento de verificación subido por el técnico
        /// </summary>
        public DocumentoVerificacionResult GuardarDocumento(
            int idTecnico, 
            string tipoDocumento, 
            HttpPostedFileBase archivo)
        {
            try
            {
                // Validar que el archivo no sea nulo
                if (archivo == null || archivo.ContentLength == 0)
                {
                    return new DocumentoVerificacionResult
                    {
                        Success = false,
                        ErrorMessage = "No se ha seleccionado ningún archivo"
                    };
                }

                // Validar tamaño (máximo 5MB)
                if (archivo.ContentLength > 5 * 1024 * 1024)
                {
                    return new DocumentoVerificacionResult
                    {
                        Success = false,
                        ErrorMessage = "El archivo no puede superar los 5MB"
                    };
                }

                // Validar tipo de archivo
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var extension = Path.GetExtension(archivo.FileName).ToLower();
                
                if (!extensionesPermitidas.Contains(extension))
                {
                    return new DocumentoVerificacionResult
                    {
                        Success = false,
                        ErrorMessage = "Solo se permiten archivos JPG, PNG o PDF"
                    };
                }

                // Crear carpeta si no existe
                var rutaBase = HttpContext.Current.Server.MapPath("~/Uploads/Verificacion");
                var rutaTecnico = Path.Combine(rutaBase, idTecnico.ToString());
                
                if (!Directory.Exists(rutaTecnico))
                {
                    Directory.CreateDirectory(rutaTecnico);
                }

                // Generar nombre único para el archivo
                var nombreUnico = $"{tipoDocumento}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                var rutaCompleta = Path.Combine(rutaTecnico, nombreUnico);

                // Guardar archivo físicamente
                archivo.SaveAs(rutaCompleta);

                // Guardar registro en base de datos
                var documento = new DocumentoVerificacion
                {
                    IdTecnico = idTecnico,
                    TipoDocumento = tipoDocumento,
                    NombreArchivo = archivo.FileName,
                    RutaArchivo = rutaCompleta,
                    TamanoBytes = archivo.ContentLength,
                    TipoMime = archivo.ContentType,
                    EstadoRevision = "Pendiente",
                    FechaSubida = DateTime.Now
                };

                _context.DocumentosVerificacion.Add(documento);
                _context.SaveChanges();

                return new DocumentoVerificacionResult
                {
                    Success = true,
                    IdDocumento = documento.IdDocumento,
                    RutaArchivo = rutaCompleta,
                    Message = "Documento subido exitosamente"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GuardarDocumento: {ex.Message}");
                return new DocumentoVerificacionResult
                {
                    Success = false,
                    ErrorMessage = "Error al guardar el documento: " + ex.Message
                };
            }
        }

        /// <summary>
        /// Obtiene el estado de verificación de un técnico
        /// </summary>
        public EstadoVerificacionTecnico ObtenerEstadoVerificacion(int idTecnico)
        {
            var tecnico = _context.Tecnicos.Find(idTecnico);
            if (tecnico == null)
                return null;

            var documentos = _context.DocumentosVerificacion
                .Where(d => d.IdTecnico == idTecnico)
                .ToList();

            var documentosRequeridos = _context.Database.SqlQuery<string>(
                "SELECT Tipo_documento FROM Catalogo_Tipos_Documento WHERE Requerido = 1"
            ).ToList();

            var documentosSubidos = documentos.Select(d => d.TipoDocumento).Distinct().ToList();
            var documentosFaltantes = documentosRequeridos.Except(documentosSubidos).ToList();

            var porcentajeCompletado = documentosRequeridos.Count > 0
                ? (documentosSubidos.Count * 100) / documentosRequeridos.Count
                : 0;

            return new EstadoVerificacionTecnico
            {
                IdTecnico = idTecnico,
                EstadoVerificacion = tecnico.EstadoVerificacion,
                NivelConfianza = tecnico.NivelConfianza,
                PorcentajeCompletado = porcentajeCompletado,
                DocumentosRequeridos = documentosRequeridos.Count,
                DocumentosSubidos = documentosSubidos.Count,
                DocumentosFaltantes = documentosFaltantes,
                INEVerificada = tecnico.INEVerificada,
                DomicilioVerificado = tecnico.DomicilioVerificado,
                CertificadoVerificado = tecnico.CertificadoVerificado,
                FechaVerificacion = tecnico.FechaVerificacion,
                NotasVerificacion = tecnico.NotasVerificacion,
                MotivoRechazo = tecnico.MotivoRechazo,
                Badge = tecnico.Badge
            };
        }

        /// <summary>
        /// Obtiene todos los documentos de un técnico
        /// </summary>
        public List<DocumentoVerificacion> ObtenerDocumentosTecnico(int idTecnico)
        {
            return _context.DocumentosVerificacion
                .Where(d => d.IdTecnico == idTecnico)
                .OrderBy(d => d.FechaSubida)
                .ToList();
        }

        /// <summary>
        /// Verifica si un técnico puede aceptar un trabajo según su nivel de verificación
        /// </summary>
        public ValidacionTrabajoResult ValidarPuedeAceptarTrabajo(int idTecnico, decimal? montoTrabajo)
        {
            var tecnico = _context.Tecnicos.Find(idTecnico);
            if (tecnico == null)
            {
                return new ValidacionTrabajoResult
                {
                    PuedeAceptar = false,
                    Motivo = "Técnico no encontrado"
                };
            }

            // Si está suspendido, no puede trabajar
            if (tecnico.EstadoVerificacion == "Suspendido")
            {
                return new ValidacionTrabajoResult
                {
                    PuedeAceptar = false,
                    Motivo = "Tu cuenta está suspendida. Contacta al administrador."
                };
            }

            // Si está rechazado, no puede trabajar
            if (tecnico.EstadoVerificacion == "Rechazado")
            {
                return new ValidacionTrabajoResult
                {
                    PuedeAceptar = false,
                    Motivo = "Tu solicitud de verificación fue rechazada. Por favor revisa tus documentos."
                };
            }

            // Si está pendiente de verificación, verificar límites
            if (tecnico.EstadoVerificacion == "Pendiente")
            {
                // Contar trabajos del mes actual
                var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var trabajosEsteMes = _context.OrdenesServicio
                    .Count(o => o.IdTecnico == idTecnico 
                           && o.FechaSolicitud >= inicioMes
                           && o.EstadoServicio != "Cancelado");

                if (tecnico.LimiteTrabajosmes.HasValue && trabajosEsteMes >= tecnico.LimiteTrabajosmes.Value)
                {
                    return new ValidacionTrabajoResult
                    {
                        PuedeAceptar = false,
                        Motivo = $"Has alcanzado el límite de {tecnico.LimiteTrabajosmes} trabajos mensuales. Completa tu verificación para trabajos ilimitados."
                    };
                }

                if (montoTrabajo.HasValue && tecnico.LimiteMontoTrabajo.HasValue 
                    && montoTrabajo.Value > tecnico.LimiteMontoTrabajo.Value)
                {
                    return new ValidacionTrabajoResult
                    {
                        PuedeAceptar = false,
                        Motivo = $"El monto del trabajo (${montoTrabajo:N2}) supera tu límite actual (${tecnico.LimiteMontoTrabajo:N2}). Completa tu verificación para trabajos sin límite."
                    };
                }

                return new ValidacionTrabajoResult
                {
                    PuedeAceptar = true,
                    RequiereSupervision = true,
                    Advertencia = "Este trabajo será supervisado hasta que completes tu verificación."
                };
            }

            // Si está verificado, puede aceptar sin restricciones
            return new ValidacionTrabajoResult
            {
                PuedeAceptar = true,
                RequiereSupervision = false
            };
        }

        /// <summary>
        /// Registra un reporte contra un técnico
        /// </summary>
        public ReporteResult RegistrarReporte(ReporteTecnicoModel modelo)
        {
            try
            {
                var reporte = new ReporteTecnico
                {
                    IdTecnico = modelo.IdTecnico,
                    IdCliente = modelo.IdCliente,
                    IdOrden = modelo.IdOrden,
                    TipoReporte = modelo.TipoReporte,
                    Descripcion = modelo.Descripcion,
                    Gravedad = modelo.Gravedad,
                    FechaReporte = DateTime.Now,
                    Estado = "Pendiente"
                };

                _context.ReportesTecnicos.Add(reporte);
                _context.SaveChanges();

                // Si es crítico, suspender automáticamente
                if (modelo.Gravedad == "Critica")
                {
                    var tecnico = _context.Tecnicos.Find(modelo.IdTecnico);
                    if (tecnico != null)
                    {
                        tecnico.EstadoVerificacion = "Suspendido";
                        tecnico.Estado = "Suspendido";
                        tecnico.FechaSuspension = DateTime.Now;
                        _context.SaveChanges();
                    }
                }

                return new ReporteResult
                {
                    Success = true,
                    IdReporte = reporte.IdReporte,
                    Message = "Reporte registrado exitosamente. Será revisado por nuestro equipo."
                };
            }
            catch (Exception ex)
            {
                return new ReporteResult
                {
                    Success = false,
                    ErrorMessage = "Error al registrar el reporte: " + ex.Message
                };
            }
        }

        /// <summary>
        /// Actualiza el contador de trabajos completados y calificación
        /// </summary>
        public void ActualizarEstadisticasTecnico(int idTecnico, bool completado, decimal? calificacion)
        {
            var tecnico = _context.Tecnicos.Find(idTecnico);
            if (tecnico == null) return;

            if (completado)
            {
                tecnico.TrabajosCompletados++;
            }
            else
            {
                tecnico.TrabajosCancelados++;
            }

            // Recalcular calificación promedio si se proporciona
            if (calificacion.HasValue)
            {
                var calificaciones = _context.Calificaciones
                    .Where(c => c.IdTecnico == idTecnico)
                    .Select(c => c.Puntuacion)
                    .ToList();

                if (calificaciones.Any())
                {
                    tecnico.Calificacion = (decimal)calificaciones.Average();
                }
            }

            _context.SaveChanges();

            // Evaluar si debe subir de nivel de confianza
            ActualizarNivelConfianza(idTecnico);
        }

        /// <summary>
        /// Actualiza el nivel de confianza según el rendimiento
        /// </summary>
        private void ActualizarNivelConfianza(int idTecnico)
        {
            var tecnico = _context.Tecnicos.Find(idTecnico);
            if (tecnico == null || tecnico.EstadoVerificacion != "Verificado")
                return;

            // Contar reportes negativos graves
            var reportesGraves = _context.ReportesTecnicos
                .Count(r => r.IdTecnico == idTecnico 
                       && (r.Gravedad == "Alta" || r.Gravedad == "Critica")
                       && r.Estado != "Desestimado");

            string nuevoNivel = tecnico.NivelConfianza;

            // Criterios para Profesional Certificado
            if (tecnico.TrabajosCompletados >= 50 
                && tecnico.Calificacion >= 4.8m 
                && reportesGraves == 0
                && tecnico.CertificadoVerificado
                && tecnico.AntecedentesVerificados)
            {
                nuevoNivel = "Profesional_Certificado";
                tecnico.Badge = "Premium";
            }
            // Criterios para Verificado
            else if (tecnico.TrabajosCompletados >= 10 
                     && tecnico.Calificacion >= 4.5m)
            {
                nuevoNivel = "Verificado";
                tecnico.Badge = "Verificado";
            }
            // Mantener en Básico
            else
            {
                nuevoNivel = "Basico";
                tecnico.Badge = null;
            }

            if (nuevoNivel != tecnico.NivelConfianza)
            {
                tecnico.NivelConfianza = nuevoNivel;
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Genera un código de verificación para teléfono/email
        /// </summary>
        public string GenerarCodigoVerificacion(int idTecnico, string tipo, string valor)
        {
            // Generar código aleatorio de 6 dígitos
            var random = new Random();
            var codigo = random.Next(100000, 999999).ToString();

            var verificacion = new VerificacionContacto
            {
                IdTecnico = idTecnico,
                Tipo = tipo,
                ValorVerificar = valor,
                CodigoVerificacion = codigo,
                FechaEnvio = DateTime.Now,
                FechaExpiracion = DateTime.Now.AddMinutes(15), // Válido por 15 minutos
                Verificado = false,
                IntentosFallidos = 0
            };

            _context.VerificacionesContacto.Add(verificacion);
            _context.SaveChanges();

            return codigo;
        }

        /// <summary>
        /// Verifica un código de verificación
        /// </summary>
        public bool VerificarCodigo(int idTecnico, string tipo, string codigo)
        {
            var verificacion = _context.VerificacionesContacto
                .Where(v => v.IdTecnico == idTecnico 
                       && v.Tipo == tipo
                       && !v.Verificado
                       && v.FechaExpiracion > DateTime.Now)
                .OrderByDescending(v => v.FechaEnvio)
                .FirstOrDefault();

            if (verificacion == null)
                return false;

            if (verificacion.CodigoVerificacion == codigo)
            {
                verificacion.Verificado = true;
                verificacion.FechaVerificacion = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            else
            {
                verificacion.IntentosFallidos++;
                _context.SaveChanges();
                return false;
            }
        }
    }

    #region Modelos de Resultado

    public class DocumentoVerificacionResult
    {
        public bool Success { get; set; }
        public int IdDocumento { get; set; }
        public string RutaArchivo { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class EstadoVerificacionTecnico
    {
        public int IdTecnico { get; set; }
        public string EstadoVerificacion { get; set; }
        public string NivelConfianza { get; set; }
        public int PorcentajeCompletado { get; set; }
        public int DocumentosRequeridos { get; set; }
        public int DocumentosSubidos { get; set; }
        public List<string> DocumentosFaltantes { get; set; }
        public bool INEVerificada { get; set; }
        public bool DomicilioVerificado { get; set; }
        public bool CertificadoVerificado { get; set; }
        public DateTime? FechaVerificacion { get; set; }
        public string NotasVerificacion { get; set; }
        public string MotivoRechazo { get; set; }
        public string Badge { get; set; }
    }

    public class ValidacionTrabajoResult
    {
        public bool PuedeAceptar { get; set; }
        public bool RequiereSupervision { get; set; }
        public string Motivo { get; set; }
        public string Advertencia { get; set; }
    }

    public class ReporteResult
    {
        public bool Success { get; set; }
        public int IdReporte { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ReporteTecnicoModel
    {
        public int IdTecnico { get; set; }
        public int IdCliente { get; set; }
        public int? IdOrden { get; set; }
        public string TipoReporte { get; set; }
        public string Descripcion { get; set; }
        public string Gravedad { get; set; }
    }

    #endregion
}
