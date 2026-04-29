using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SIGEST.Models;

namespace SIGEST.Services
{
    public class AuthService
    {
        /// <summary>
        /// Valida las credenciales del usuario
        /// </summary>
        public UserAuthResult ValidarUsuario(string nombreUsuario, string contrasena)
        {
            using (var context = new SigestContext())
            {
                // Buscar usuario por nombre de usuario o email
                var usuario = context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefault(u => u.NombreUsuario == nombreUsuario || u.NombreUsuario == nombreUsuario);

                if (usuario == null)
                {
                    return new UserAuthResult { Success = false };
                }

                // Verificar la contraseña
                bool contrasenaValida = VerificarContrasena(contrasena, usuario.ContrasenaHash, usuario.Salt);

                if (!contrasenaValida)
                {
                    return new UserAuthResult { Success = false };
                }

                // Actualizar último acceso
                usuario.UltimoAcceso = DateTime.Now;
                context.SaveChanges();

                // Obtener información de la entidad asociada (Cliente o Técnico)
                int? idEntidad = null;
                string telefono = null;
                string correoElectronico = null;

                if (usuario.IdRol == 2) // Técnico
                {
                    var tecnico = context.Tecnicos.FirstOrDefault(t => t.IdTecnico == usuario.IdUsuario);
                    if (tecnico != null)
                    {
                        idEntidad = tecnico.IdTecnico;
                        telefono = tecnico.Telefono;
                    }
                }
                else if (usuario.IdRol == 3) // Cliente
                {
                    var cliente = context.Clientes.FirstOrDefault(c => c.IdCliente == usuario.IdUsuario);
                    if (cliente != null)
                    {
                        idEntidad = cliente.IdCliente;
                        telefono = cliente.Telefono;
                        correoElectronico = cliente.CorreoElectronico;
                    }
                }

                return new UserAuthResult
                {
                    Success = true,
                    IdUsuario = usuario.IdUsuario,
                    IdEntidad = idEntidad ?? usuario.IdUsuario,
                    Nombre = usuario.Nombre,
                    Rol = usuario.Rol.NombreRol,
                    CorreoElectronico = correoElectronico ?? nombreUsuario,
                    Telefono = telefono
                };
            }
        }

        /// <summary>
        /// Registra un nuevo usuario cliente
        /// </summary>
        public UserAuthResult RegistrarUsuario(RegistroModel model)
        {
            using (var context = new SigestContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Verificar si el usuario ya existe
                        if (context.Usuarios.Any(u => u.NombreUsuario == model.Email))
                        {
                            return new UserAuthResult
                            {
                                Success = false,
                                Message = "El correo electrónico ya está registrado"
                            };
                        }

                        // Generar salt y hash de contraseña
                        byte[] salt = GenerarSalt();
                        byte[] hash = HashearContrasena(model.Password, salt);

                        // Crear usuario
                        var nuevoUsuario = new Usuario
                        {
                            NombreUsuario = model.Email,
                            ContrasenaHash = hash,
                            Salt = salt,
                            Nombre = model.Name,
                            IdRol = 3, // Cliente
                            FechaCreacion = DateTime.Now,
                            UltimoAcceso = DateTime.Now
                        };

                        context.Usuarios.Add(nuevoUsuario);
                        context.SaveChanges();

                        // Crear cliente
                        var cliente = new Cliente
                        {
                            IdCliente = nuevoUsuario.IdUsuario,
                            Nombre = model.Name,
                            Telefono = model.Phone,
                            CorreoElectronico = model.Email,
                            Direccion = model.Address
                        };
                        context.Clientes.Add(cliente);

                        context.SaveChanges();
                        transaction.Commit();

                        return new UserAuthResult
                        {
                            Success = true,
                            IdUsuario = nuevoUsuario.IdUsuario,
                            IdEntidad = cliente.IdCliente,
                            Nombre = model.Name,
                            Rol = "Cliente",
                            CorreoElectronico = model.Email,
                            Telefono = model.Phone
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new UserAuthResult
                        {
                            Success = false,
                            Message = "Error al crear la cuenta: " + ex.Message
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Registra un nuevo técnico con información profesional
        /// </summary>
        public UserAuthResult RegistrarTecnico(
            RegistroModel model,
            string especialidad,
            string direccion,
            string ciudad,
            string estado,
            decimal tarifaHora,
            decimal? tarifaVisita,
            string descripcion)
        {
            using (var context = new SigestContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Verificar si el usuario ya existe
                        if (context.Usuarios.Any(u => u.NombreUsuario == model.Email))
                        {
                            return new UserAuthResult
                            {
                                Success = false,
                                Message = "El correo electrónico ya está registrado"
                            };
                        }

                        // Generar salt y hash de contraseña
                        byte[] salt = GenerarSalt();
                        byte[] hash = HashearContrasena(model.Password, salt);

                        // Crear usuario
                        var nuevoUsuario = new Usuario
                        {
                            NombreUsuario = model.Email,
                            ContrasenaHash = hash,
                            Salt = salt,
                            Nombre = model.Name,
                            IdRol = 2, // Técnico
                            FechaCreacion = DateTime.Now,
                            UltimoAcceso = DateTime.Now
                        };

                        context.Usuarios.Add(nuevoUsuario);
                        context.SaveChanges();

                        // Crear técnico con estado "Pendiente de Verificación"
                        var tecnico = new Tecnico
                        {
                            IdTecnico = nuevoUsuario.IdUsuario,
                            Nombre = model.Name,
                            Especialidad = especialidad,
                            Telefono = model.Phone,
                            Estado = "Disponible",
                            FechaAlta = DateTime.Now,
                            TarifaHora = tarifaHora,
                            TarifaVisita = tarifaVisita,
                            
                            // Campos de verificación
                            EstadoVerificacion = "Pendiente",
                            NivelConfianza = "Basico",
                            INEVerificada = false,
                            DomicilioVerificado = false,
                            CertificadoVerificado = false,
                            AntecedentesVerificados = false,
                            SeguroVerificado = false,
                            
                            // Límites para técnicos no verificados
                            LimiteTrabajosmes = 3,
                            LimiteMontoTrabajo = 500,
                            
                            // Contadores iniciales
                            TrabajosCompletados = 0,
                            TrabajosCancelados = 0,
                            CantidadServiciosAsignados = 0,
                            Calificacion = 0,
                            
                            Badge = null // Sin badge hasta verificación
                        };

                        context.Tecnicos.Add(tecnico);
                        context.SaveChanges();

                        // Registrar en historial de verificación
                        var historial = new HistorialVerificacion
                        {
                            IdTecnico = tecnico.IdTecnico,
                            EstadoAnterior = null,
                            EstadoNuevo = "Pendiente",
                            NivelAnterior = null,
                            NivelNuevo = "Basico",
                            Motivo = "Registro inicial de técnico",
                            Observaciones = $"Especialidad: {especialidad}, Tarifa/hora: ${tarifaHora}",
                            ModificadoPor = nuevoUsuario.IdUsuario,
                            FechaCambio = DateTime.Now
                        };

                        context.HistorialVerificacion.Add(historial);
                        context.SaveChanges();

                        transaction.Commit();

                        return new UserAuthResult
                        {
                            Success = true,
                            IdUsuario = nuevoUsuario.IdUsuario,
                            IdEntidad = tecnico.IdTecnico,
                            Nombre = model.Name,
                            Rol = "Tecnico",
                            CorreoElectronico = model.Email,
                            Telefono = model.Phone
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new UserAuthResult
                        {
                            Success = false,
                            Message = "Error al crear la cuenta de técnico: " + ex.Message
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        public bool CambiarContrasena(int idUsuario, string contrasenaActual, string contrasenaNueva)
        {
            using (var context = new SigestContext())
            {
                var usuario = context.Usuarios.Find(idUsuario);
                if (usuario == null)
                    return false;

                // Verificar contraseña actual
                if (!VerificarContrasena(contrasenaActual, usuario.ContrasenaHash, usuario.Salt))
                    return false;

                // Generar nuevo hash
                byte[] nuevoSalt = GenerarSalt();
                byte[] nuevoHash = HashearContrasena(contrasenaNueva, nuevoSalt);

                usuario.ContrasenaHash = nuevoHash;
                usuario.Salt = nuevoSalt;

                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Restablece la contraseña (para recuperación)
        /// </summary>
        public bool RestablecerContrasena(string nombreUsuario, string contrasenaNueva)
        {
            using (var context = new SigestContext())
            {
                var usuario = context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
                if (usuario == null)
                    return false;

                // Generar nuevo hash
                byte[] nuevoSalt = GenerarSalt();
                byte[] nuevoHash = HashearContrasena(contrasenaNueva, nuevoSalt);

                usuario.ContrasenaHash = nuevoHash;
                usuario.Salt = nuevoSalt;

                context.SaveChanges();
                return true;
            }
        }

        #region Métodos de Criptografía

        /// <summary>
        /// Genera un salt aleatorio de 32 bytes
        /// </summary>
        private byte[] GenerarSalt()
        {
            byte[] salt = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        /// <summary>
        /// Genera el hash SHA256 de una contraseña con salt
        /// </summary>
        private byte[] HashearContrasena(string contrasena, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                // Combinar contraseña + salt
                byte[] contrasenaBytes = Encoding.UTF8.GetBytes(contrasena);
                byte[] combinado = new byte[contrasenaBytes.Length + salt.Length];
                
                Buffer.BlockCopy(contrasenaBytes, 0, combinado, 0, contrasenaBytes.Length);
                Buffer.BlockCopy(salt, 0, combinado, contrasenaBytes.Length, salt.Length);

                // Calcular hash
                return sha256.ComputeHash(combinado);
            }
        }

        /// <summary>
        /// Verifica si una contraseña coincide con su hash
        /// </summary>
        private bool VerificarContrasena(string contrasena, byte[] hashAlmacenado, byte[] salt)
        {
            byte[] hashCalculado = HashearContrasena(contrasena, salt);

            // Comparar byte por byte
            if (hashCalculado.Length != hashAlmacenado.Length)
                return false;

            for (int i = 0; i < hashCalculado.Length; i++)
            {
                if (hashCalculado[i] != hashAlmacenado[i])
                    return false;
            }

            return true;
        }

        #endregion
    }

    /// <summary>
    /// Resultado de autenticación de usuario
    /// </summary>
    public class UserAuthResult
    {
        public bool Success { get; set; }
        public int IdUsuario { get; set; }
        public int IdEntidad { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
        public string CorreoElectronico { get; set; }
        public string Telefono { get; set; }
        public string Message { get; set; }
    }
}
