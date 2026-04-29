using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SIGEST.Models;
using SIGEST.Services;

namespace SIGEST.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthService _authService;
        private readonly TecnicoVerificacionService _verificacionService;

        public LoginController()
        {
            _authService = new AuthService();
            _verificacionService = new TecnicoVerificacionService();
        }

        // GET: Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Si ya está autenticado, redirigir según su rol
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToHome();
            }

            return View();
        }

        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Por favor completa todos los campos";
                return View(model);
            }

            try
            {
                // Validar usuario con el servicio de autenticación
                var resultado = _authService.ValidarUsuario(model.Username, model.Password);

                if (resultado.Success)
                {
                    // Crear el ticket de autenticación
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);

                    // Guardar información en sesión
                    Session["IdUsuario"] = resultado.IdUsuario;
                    Session["IdEntidad"] = resultado.IdEntidad;
                    Session["UserRole"] = resultado.Rol;
                    Session["UserName"] = resultado.Nombre;
                    Session["UserEmail"] = resultado.CorreoElectronico;
                    Session["UserPhone"] = resultado.Telefono;

                    // Redirigir según el rol
                    switch (resultado.Rol.ToLower())
                    {
                        case "admin":
                            return RedirectToAction("Dashboard", "Admin");
                        case "tecnico":
                            return RedirectToAction("DashboardTecnico", "Tecnico");
                        case "cliente":
                            return RedirectToAction("Inicio", "Home");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Usuario o contraseña incorrectos";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al iniciar sesión: " + ex.Message;
                return View(model);
            }
        }

        // GET: Register (Cliente)
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register (Cliente)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegistroModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Por favor completa todos los campos correctamente";
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Las contraseñas no coinciden";
                return View(model);
            }

            try
            {
                // Establecer el rol como Cliente
                model.Role = "cliente";

                // Registrar el usuario
                var resultado = _authService.RegistrarUsuario(model);

                if (resultado.Success)
                {
                    // Login automático después del registro
                    FormsAuthentication.SetAuthCookie(model.Email, false);

                    Session["IdUsuario"] = resultado.IdUsuario;
                    Session["IdEntidad"] = resultado.IdEntidad;
                    Session["UserRole"] = "Cliente";
                    Session["UserName"] = model.Name;
                    Session["UserEmail"] = model.Email;
                    Session["UserPhone"] = model.Phone;

                    // Redirigir a la página principal del cliente
                    return RedirectToAction("Inicio", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = resultado.Message ?? "Error al crear la cuenta";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al registrar usuario: " + ex.Message;
                return View(model);
            }
        }

        // GET: RegistroTecnico
        [AllowAnonymous]
        public ActionResult RegistroTecnico()
        {
            return View();
        }

        // POST: RegistroTecnico
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarTecnico()
        {
            try
            {
                // Recoger datos del formulario
                var model = new RegistroModel
                {
                    Name = Request.Form["Name"],
                    Email = Request.Form["Email"],
                    Phone = Request.Form["Phone"],
                    Password = Request.Form["Password"],
                    ConfirmPassword = Request.Form["ConfirmPassword"],
                    Role = "tecnico"
                };

                // Validaciones básicas
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email) || 
                    string.IsNullOrEmpty(model.Password))
                {
                    return Json(new { success = false, message = "Todos los campos son requeridos" });
                }

                if (model.Password != model.ConfirmPassword)
                {
                    return Json(new { success = false, message = "Las contraseñas no coinciden" });
                }

                // Registrar el técnico
                var resultado = _authService.RegistrarTecnico(
                    model,
                    Request.Form["Especialidad"],
                    Request.Form["Direccion"],
                    Request.Form["Ciudad"],
                    Request.Form["Estado"],
                    decimal.TryParse(Request.Form["TarifaHora"], out decimal tarifaHora) ? tarifaHora : 0,
                    decimal.TryParse(Request.Form["TarifaVisita"], out decimal tarifaVisita) ? (decimal?)tarifaVisita : null,
                    Request.Form["Descripcion"]
                );

                if (!resultado.Success)
                {
                    return Json(new { success = false, message = resultado.Message });
                }

                // Procesar documentos subidos
                var idTecnico = resultado.IdEntidad;
                var documentosSubidos = new List<string>();
                var erroresDocumentos = new List<string>();

                foreach (string fileKey in Request.Files)
                {
                    var file = Request.Files[fileKey];
                    if (file != null && file.ContentLength > 0)
                    {
                        // Determinar el tipo de documento basado en el nombre del campo
                        string tipoDocumento = DeterminarTipoDocumento(fileKey);
                        
                        if (string.IsNullOrEmpty(tipoDocumento))
                            continue;

                        // Guardar el documento
                        var resultadoDoc = _verificacionService.GuardarDocumento(
                            idTecnico,
                            tipoDocumento,
                            file
                        );

                        if (resultadoDoc.Success)
                        {
                            documentosSubidos.Add(tipoDocumento);
                        }
                        else
                        {
                            erroresDocumentos.Add($"{tipoDocumento}: {resultadoDoc.Message}");
                        }
                    }
                }

                // Verificar que se subieron los documentos obligatorios
                var documentosObligatorios = new[] { "INE_Frente", "INE_Reverso", "Comprobante_Domicilio", "Foto_Perfil" };
                var faltantes = documentosObligatorios.Where(d => !documentosSubidos.Contains(d)).ToList();

                if (faltantes.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Faltan documentos obligatorios: " + string.Join(", ", faltantes)
                    });
                }

                // Login automático
                FormsAuthentication.SetAuthCookie(model.Email, false);

                Session["IdUsuario"] = resultado.IdUsuario;
                Session["IdEntidad"] = idTecnico;
                Session["UserRole"] = "Tecnico";
                Session["UserName"] = model.Name;
                Session["UserEmail"] = model.Email;
                Session["UserPhone"] = model.Phone;

                // Retornar éxito
                return Json(new
                {
                    success = true,
                    message = "Cuenta creada exitosamente",
                    redirectUrl = Url.Action("DashboardTecnico", "Tecnico"),
                    documentosSubidos = documentosSubidos.Count,
                    erroresDocumentos = erroresDocumentos
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al crear la cuenta: " + ex.Message
                });
            }
        }

        // GET: ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Por favor ingresa tu correo electrónico";
                return View();
            }

            try
            {
                // TODO: Implementar recuperación de contraseña
                // 1. Generar token de recuperación
                // 2. Enviar email con link de recuperación
                // 3. El link debe redirigir a ResetPassword con el token

                ViewBag.SuccessMessage = "Se ha enviado un correo con instrucciones para recuperar tu contraseña";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al procesar la solicitud: " + ex.Message;
                return View();
            }
        }

        // GET: Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login");
        }

        #region Métodos Auxiliares

        private ActionResult RedirectToHome()
        {
            if (Session["UserRole"] != null)
            {
                switch (Session["UserRole"].ToString().ToLower())
                {
                    case "admin":
                        return RedirectToAction("Dashboard", "Admin");
                    case "tecnico":
                        return RedirectToAction("DashboardTecnico", "Tecnico");
                    case "cliente":
                        return RedirectToAction("Inicio", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        private string DeterminarTipoDocumento(string fileKey)
        {
            // Mapeo de nombres de campos del formulario a tipos de documento en BD
            var mapeo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Document_ine-frente", "INE_Frente" },
                { "Document_ine-reverso", "INE_Reverso" },
                { "Document_comprobante", "Comprobante_Domicilio" },
                { "Document_foto-perfil", "Foto_Perfil" },
                { "Document_certificado", "Certificado" },
                { "Document_antecedentes", "Antecedentes" },
                { "Document_seguro", "Seguro" },
                { "Document_rfc", "RFC" }
            };

            return mapeo.ContainsKey(fileKey) ? mapeo[fileKey] : null;
        }

        #endregion
    }
}
