using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGEST.Models
{
    /// <summary>
    /// Modelo para la tabla Documentos_Verificacion
    /// </summary>
    [Table("Documentos_Verificacion")]
    public class DocumentoVerificacion
    {
        [Key]
        [Column("ID_Documento")]
        public int IdDocumento { get; set; }

        [Required]
        [Column("ID_Tecnico")]
        public int IdTecnico { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Tipo_documento")]
        public string TipoDocumento { get; set; }

        [Required]
        [StringLength(255)]
        [Column("Nombre_archivo")]
        public string NombreArchivo { get; set; }

        [Required]
        [StringLength(500)]
        [Column("Ruta_archivo")]
        public string RutaArchivo { get; set; }

        [Column("Tamano_bytes")]
        public int? TamanoBytes { get; set; }

        [StringLength(100)]
        [Column("Tipo_mime")]
        public string TipoMime { get; set; }

        [StringLength(20)]
        [Column("Estado_revision")]
        public string EstadoRevision { get; set; }

        [Column("Fecha_subida")]
        public DateTime FechaSubida { get; set; }

        [Column("Fecha_revision")]
        public DateTime? FechaRevision { get; set; }

        [Column("Notas_revision")]
        public string NotasRevision { get; set; }

        [Column("Motivo_rechazo")]
        public string MotivoRechazo { get; set; }

        [Column("Revisado_por")]
        public int? RevisadoPor { get; set; }

        // Relaciones
        [ForeignKey("IdTecnico")]
        public virtual Tecnico Tecnico { get; set; }

        [ForeignKey("RevisadoPor")]
        public virtual Usuario Revisor { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla Historial_Verificacion
    /// </summary>
    [Table("Historial_Verificacion")]
    public class HistorialVerificacion
    {
        [Key]
        [Column("ID_Historial")]
        public int IdHistorial { get; set; }

        [Required]
        [Column("ID_Tecnico")]
        public int IdTecnico { get; set; }

        [StringLength(20)]
        [Column("Estado_anterior")]
        public string EstadoAnterior { get; set; }

        [Required]
        [StringLength(20)]
        [Column("Estado_nuevo")]
        public string EstadoNuevo { get; set; }

        [StringLength(20)]
        [Column("Nivel_anterior")]
        public string NivelAnterior { get; set; }

        [StringLength(20)]
        [Column("Nivel_nuevo")]
        public string NivelNuevo { get; set; }

        [Column("Motivo")]
        public string Motivo { get; set; }

        [Column("Observaciones")]
        public string Observaciones { get; set; }

        [Required]
        [Column("Modificado_por")]
        public int ModificadoPor { get; set; }

        [Column("Fecha_cambio")]
        public DateTime FechaCambio { get; set; }

        // Relaciones
        [ForeignKey("IdTecnico")]
        public virtual Tecnico Tecnico { get; set; }

        [ForeignKey("ModificadoPor")]
        public virtual Usuario Usuario { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla Reportes_Tecnicos
    /// </summary>
    [Table("Reportes_Tecnicos")]
    public class ReporteTecnico
    {
        [Key]
        [Column("ID_Reporte")]
        public int IdReporte { get; set; }

        [Required]
        [Column("ID_Tecnico")]
        public int IdTecnico { get; set; }

        [Required]
        [Column("ID_Cliente")]
        public int IdCliente { get; set; }

        [Column("ID_Orden")]
        public int? IdOrden { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Tipo_reporte")]
        public string TipoReporte { get; set; }

        [Required]
        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [StringLength(20)]
        [Column("Gravedad")]
        public string Gravedad { get; set; }

        [StringLength(20)]
        [Column("Estado")]
        public string Estado { get; set; }

        [StringLength(500)]
        [Column("Evidencia_adjunta")]
        public string EvidenciaAdjunta { get; set; }

        [Column("Resolucion")]
        public string Resolucion { get; set; }

        [StringLength(100)]
        [Column("Accion_tomada")]
        public string AccionTomada { get; set; }

        [Column("Fecha_reporte")]
        public DateTime FechaReporte { get; set; }

        [Column("Fecha_revision")]
        public DateTime? FechaRevision { get; set; }

        [Column("Fecha_resolucion")]
        public DateTime? FechaResolucion { get; set; }

        [Column("Atendido_por")]
        public int? AtendidoPor { get; set; }

        // Relaciones
        [ForeignKey("IdTecnico")]
        public virtual Tecnico Tecnico { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("IdOrden")]
        public virtual OrdenServicio OrdenServicio { get; set; }

        [ForeignKey("AtendidoPor")]
        public virtual Usuario Administrador { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla Verificaciones_Contacto
    /// </summary>
    [Table("Verificaciones_Contacto")]
    public class VerificacionContacto
    {
        [Key]
        [Column("ID_Verificacion")]
        public int IdVerificacion { get; set; }

        [Required]
        [Column("ID_Tecnico")]
        public int IdTecnico { get; set; }

        [Required]
        [StringLength(20)]
        [Column("Tipo")]
        public string Tipo { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Valor_verificar")]
        public string ValorVerificar { get; set; }

        [Required]
        [StringLength(10)]
        [Column("Codigo_verificacion")]
        public string CodigoVerificacion { get; set; }

        [Column("Verificado")]
        public bool Verificado { get; set; }

        [Column("Fecha_envio")]
        public DateTime FechaEnvio { get; set; }

        [Column("Fecha_verificacion")]
        public DateTime? FechaVerificacion { get; set; }

        [Column("Fecha_expiracion")]
        public DateTime FechaExpiracion { get; set; }

        [Column("Intentos_fallidos")]
        public int IntentosFallidos { get; set; }

        // Relaciones
        [ForeignKey("IdTecnico")]
        public virtual Tecnico Tecnico { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla Catalogo_Tipos_Documento
    /// </summary>
    [Table("Catalogo_Tipos_Documento")]
    public class CatalogoTipoDocumento
    {
        [Key]
        [StringLength(50)]
        [Column("Tipo_documento")]
        public string TipoDocumento { get; set; }

        [StringLength(200)]
        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("Requerido")]
        public bool Requerido { get; set; }

        [Column("Orden_presentacion")]
        public int OrdenPresentacion { get; set; }
    }
}
