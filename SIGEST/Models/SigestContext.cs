using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGEST.Models
{
    /// <summary>
    /// Contexto de Entity Framework para la base de datos SIGEST
    /// </summary>
    public class SigestContext : DbContext
    {
        public SigestContext() : base("name=SigestConnection")
        {
        }

        // DbSets para las tablas principales
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Tecnico> Tecnicos { get; set; }
        public DbSet<OrdenServicio> OrdenesServicio { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<CategoriaServicio> CategoriasServicio { get; set; }
        
        // DbSets para las tablas de verificación
        public DbSet<DocumentoVerificacion> DocumentosVerificacion { get; set; }
        public DbSet<HistorialVerificacion> HistorialVerificacion { get; set; }
        public DbSet<ReporteTecnico> ReportesTecnicos { get; set; }
        public DbSet<VerificacionContacto> VerificacionesContacto { get; set; }
        public DbSet<CatalogoTipoDocumento> CatalogoTiposDocumento { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configuraciones adicionales si son necesarias
            base.OnModelCreating(modelBuilder);

            // Configurar el nombre de las tablas para que coincidan con la BD
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Rol>().ToTable("Rol");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<Tecnico>().ToTable("Tecnicos");
            modelBuilder.Entity<OrdenServicio>().ToTable("Orden_Servicio");
            modelBuilder.Entity<Calificacion>().ToTable("Calificaciones");
            modelBuilder.Entity<CategoriaServicio>().ToTable("Categorias_Servicio");
            
            // Configurar tablas de verificación
            modelBuilder.Entity<DocumentoVerificacion>().ToTable("Documentos_Verificacion");
            modelBuilder.Entity<HistorialVerificacion>().ToTable("Historial_Verificacion");
            modelBuilder.Entity<ReporteTecnico>().ToTable("Reportes_Tecnicos");
            modelBuilder.Entity<VerificacionContacto>().ToTable("Verificaciones_Contacto");
            modelBuilder.Entity<CatalogoTipoDocumento>().ToTable("Catalogo_Tipos_Documento");
        }
    }

    // ==================== MODELOS DE ENTIDADES ====================

    /// <summary>
    /// Modelo para la tabla Usuarios
    /// </summary>
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("ID_Usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Nombre_usuario")]
        public string NombreUsuario { get; set; }

        [Required]
        [Column("ContrasenaHash")]
        public byte[] ContrasenaHash { get; set; }

        [Required]
        [Column("Salt")]
        public byte[] Salt { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("ID_Rol")]
        public int IdRol { get; set; }

        [Column("Fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("Ultimo_acceso")]
        public DateTime? UltimoAcceso { get; set; }

        // Relaciones
        [ForeignKey("IdRol")]
        public virtual Rol Rol { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla Rol
    /// </summary>
    [Table("Rol")]
    public class Rol
    {
        [Key]
        [Column("ID_Rol")]
        public int IdRol { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Nombre_Rol")]
        public string NombreRol { get; set; }

        public virtual System.Collections.Generic.ICollection<Usuario> Usuarios { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla Clientes
    /// </summary>
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        [Column("ID_Cliente")]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(15)]
        [Column("Telefono")]
        public string Telefono { get; set; }

        [StringLength(100)]
        [Column("Correo_electronico")]
        public string CorreoElectronico { get; set; }

        [Required]
        [StringLength(200)]
        [Column("Direccion")]
        public string Direccion { get; set; }

        [Column("Latitud")]
        public decimal? Latitud { get; set; }

        [Column("Longitud")]
        public decimal? Longitud { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla Tecnicos
    /// </summary>
    [Table("Tecnicos")]
    public partial class Tecnico
    {
        [Key]
        [Column("ID_Tecnico")]
        public int IdTecnico { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Especialidad")]
        public string Especialidad { get; set; }

        [Required]
        [StringLength(15)]
        [Column("Telefono")]
        public string Telefono { get; set; }

        [Required]
        [StringLength(20)]
        [Column("Estado")]
        public string Estado { get; set; }

        [Column("Cantidad_servicios_asignados")]
        public int CantidadServiciosAsignados { get; set; }

        [Column("Calificacion")]
        public decimal? Calificacion { get; set; }

        [Column("Fecha_alta")]
        public DateTime FechaAlta { get; set; }

        [Column("Latitud")]
        public decimal? Latitud { get; set; }

        [Column("Longitud")]
        public decimal? Longitud { get; set; }

        [Column("Radio_trabajo_km")]
        public int? RadioTrabajoKm { get; set; }

        [Column("Tarifa_hora")]
        public decimal? TarifaHora { get; set; }

        [Column("Tarifa_visita")]
        public decimal? TarifaVisita { get; set; }
        
        // Campos de verificación
        [StringLength(20)]
        [Column("Estado_verificacion")]
        public string EstadoVerificacion { get; set; }

        [StringLength(20)]
        [Column("Nivel_confianza")]
        public string NivelConfianza { get; set; }

        [Column("INE_verificada")]
        public bool INEVerificada { get; set; }

        [Column("Domicilio_verificado")]
        public bool DomicilioVerificado { get; set; }

        [Column("Certificado_verificado")]
        public bool CertificadoVerificado { get; set; }

        [Column("Antecedentes_verificados")]
        public bool AntecedentesVerificados { get; set; }

        [Column("Seguro_verificado")]
        public bool SeguroVerificado { get; set; }

        [Column("Fecha_verificacion")]
        public DateTime? FechaVerificacion { get; set; }

        [Column("Fecha_rechazo")]
        public DateTime? FechaRechazo { get; set; }

        [Column("Fecha_suspension")]
        public DateTime? FechaSuspension { get; set; }

        [Column("Notas_verificacion")]
        public string NotasVerificacion { get; set; }

        [Column("Motivo_rechazo")]
        public string MotivoRechazo { get; set; }

        [Column("Limite_trabajos_mes")]
        public int? LimiteTrabajosmes { get; set; }

        [Column("Limite_monto_trabajo")]
        public decimal? LimiteMontoTrabajo { get; set; }

        [Column("Trabajos_completados")]
        public int TrabajosCompletados { get; set; }

        [Column("Trabajos_cancelados")]
        public int TrabajosCancelados { get; set; }

        [StringLength(50)]
        [Column("Badge")]
        public string Badge { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla Orden_Servicio
    /// </summary>
    [Table("Orden_Servicio")]
    public partial class OrdenServicio
    {
        [Key]
        [Column("ID_Orden")]
        public int IdOrden { get; set; }

        [Column("ID_Cliente")]
        public int IdCliente { get; set; }

        [Column("ID_Tecnico")]
        public int? IdTecnico { get; set; }

        [Column("Fecha_solicitud")]
        public DateTime FechaSolicitud { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Tipo_servicio")]
        public string TipoServicio { get; set; }

        [Required]
        [Column("Descripcion_problema")]
        public string DescripcionProblema { get; set; }

        [Required]
        [StringLength(20)]
        [Column("Estado_servicio")]
        public string EstadoServicio { get; set; }

        [Column("Observaciones")]
        public string Observaciones { get; set; }

        [Column("Costo_servicio")]
        public decimal? CostoServicio { get; set; }

        [Required]
        [StringLength(200)]
        [Column("Direccion_servicio")]
        public string DireccionServicio { get; set; }
        
        // Campos de verificación
        [Column("Tecnico_verificado")]
        public bool TecnicoVerificado { get; set; }

        [Column("Requiere_supervision")]
        public bool RequiereSupervision { get; set; }

        [Column("Trabajo_supervisado")]
        public bool TrabajoSupervisado { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("IdTecnico")]
        public virtual Tecnico Tecnico { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla Calificaciones
    /// </summary>
    [Table("Calificaciones")]
    public class Calificacion
    {
        [Key]
        [Column("ID_Calificacion")]
        public int IdCalificacion { get; set; }

        [Column("ID_Orden")]
        public int IdOrden { get; set; }

        [Column("ID_Cliente")]
        public int IdCliente { get; set; }

        [Column("ID_Tecnico")]
        public int IdTecnico { get; set; }

        [Required]
        [Column("Puntuacion")]
        public int Puntuacion { get; set; }

        [Column("Comentario")]
        public string Comentario { get; set; }

        [Column("Fecha_calificacion")]
        public DateTime FechaCalificacion { get; set; }

        [ForeignKey("IdOrden")]
        public virtual OrdenServicio OrdenServicio { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("IdTecnico")]
        public virtual Tecnico Tecnico { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla Categorias_Servicio
    /// </summary>
    [Table("Categorias_Servicio")]
    public class CategoriaServicio
    {
        [Key]
        [Column("ID_Categoria")]
        public int IdCategoria { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Nombre_Categoria")]
        public string NombreCategoria { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [StringLength(50)]
        [Column("Icono")]
        public string Icono { get; set; }

        [Column("Activo")]
        public bool Activo { get; set; }
    }
}
