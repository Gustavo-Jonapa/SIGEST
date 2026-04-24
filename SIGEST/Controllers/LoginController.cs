using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SIGEST.Models;

namespace SIGEST.Controllers
{
    public class LoginController : Controller
    {
		
		// GET: Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			// Si ya está autenticado, redirigir al área correspondiente
			//if (User.Identity.IsAuthenticated)
			//{
			//	return RedirectToUserArea();
			//}

			//ViewBag.ReturnUrl = returnUrl;
			return View();
		}
		/*
		// POST: Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Login(string email, string password, bool rememberMe = false, string returnUrl = null)
		{
			try
			{
				// TODO: Implementar validación real contra base de datos
				// Por ahora, validación de prueba
				var user = ValidateUser(email, password);

				if (user != null)
				{
					// Crear cookie de autenticación
					FormsAuthentication.SetAuthCookie(user.Email, rememberMe);

					// Guardar rol en sesión para acceso rápido
					Session["UserRole"] = user.Role;
					Session["UserName"] = user.Name;

					// Si hay URL de retorno válida, redirigir ahí
					if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}

					// Redirigir según el rol
					if (user.Role == "Tecnico")
					{
						return RedirectToAction("DashboardTecnico", "Tecnico");
					}
					else
					{
						return RedirectToAction("Inicio", "Home");
					}
				}

				// Usuario no encontrado o credenciales incorrectas
				TempData["Error"] = "Usuario o contraseña incorrectos";
				return View();
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Error al iniciar sesión: " + ex.Message;
				return View();
			}
		}

		// POST: Register
		[HttpPost]
		[AllowAnonymous]
		public JsonResult Register(RegisterViewModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return Json(new
					{
						success = false,
						message = "Datos inválidos. Verifica la información."
					});
				}

				// Validar que las contraseñas coincidan
				if (model.Password != model.ConfirmPassword)
				{
					return Json(new
					{
						success = false,
						message = "Las contraseñas no coinciden"
					});
				}

				// TODO: Implementar creación de usuario en base de datos
				var user = CreateUser(model);

				if (user != null)
				{
					// Autenticar automáticamente
					FormsAuthentication.SetAuthCookie(user.Email, false);

					// Guardar rol en sesión
					Session["UserRole"] = user.Role;
					Session["UserName"] = user.Name;

					// Determinar URL de redirección
					string redirectUrl = model.Role == "tecnico"
						? Url.Action("DashboardTecnico", "Tecnico")
						: Url.Action("Inicio", "Home");

					return Json(new
					{
						success = true,
						redirectUrl = redirectUrl,
						message = "Cuenta creada exitosamente"
					});
				}

				return Json(new
				{
					success = false,
					message = "Error al crear la cuenta"
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

		// GET: Logout
		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			Session.Clear();
			Session.Abandon();
			return RedirectToAction("Login");
		}

		// POST: ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		public JsonResult ForgotPassword(string email)
		{
			try
			{
				// TODO: Implementar envío de correo de recuperación
				// SendPasswordResetEmail(email);

				return Json(new
				{
					success = true,
					message = "Se ha enviado un correo con instrucciones para recuperar tu contraseña"
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = "Error al procesar la solicitud: " + ex.Message
				});
			}
		}

		#region Métodos Privados (Helper Methods)

		/// <summary>
		/// Valida las credenciales del usuario contra la base de datos
		/// </summary>
		private UsuarioModel ValidateUser(string email, string password)
		{
			// TODO: Implementar validación real contra base de datos
			// Ejemplo temporal para pruebas:

			// Usuario de prueba - Cliente
			if (email == "cliente@ejemplo.com" && password == "12345678")
			{
				return new UsuarioModel
				{
					Id = 1,
					Email = "cliente@ejemplo.com",
					Name = "Cliente Prueba",
					Role = "Cliente"
				};
			}

			// Usuario de prueba - Técnico
			if (email == "tecnico@ejemplo.com" && password == "12345678")
			{
				return new UsuarioModel
				{
					Id = 2,
					Email = "tecnico@ejemplo.com",
					Name = "Sergio Cs",
					Role = "Tecnico"
				};
			}

			// Usuario admin
			if (email == "admin" && password == "admin")
			{
				return new UsuarioModel
				{
					Id = 3,
					Email = "admin@sigest.com",
					Name = "Administrador",
					Role = "Tecnico" // Por defecto redirige a Dashboard Técnico
				};
			}

			return null; // Credenciales inválidas
		}

		/// <summary>
		/// Crea un nuevo usuario en la base de datos
		/// </summary>
		private UsuarioModel CreateUser(RegisterViewModel model)
		{
			// TODO: Implementar creación de usuario en base de datos
			// 1. Hash de contraseña con BCrypt o similar
			// 2. Validar que el email no exista
			// 3. Crear usuario en BD
			// 4. Retornar usuario creado

			// Ejemplo temporal para pruebas:
			return new UsuarioModel
			{
				Id = new Random().Next(1000, 9999),
				Email = model.Email,
				Name = model.Name,
				Role = model.Role == "tecnico" ? "Tecnico" : "Cliente"
			};
		}

		/// <summary>
		/// Redirige al usuario al área correspondiente según su rol
		/// </summary>
		private ActionResult RedirectToUserArea()
		{
			var role = Session["UserRole"]?.ToString();

			if (role == "Tecnico")
			{
				return RedirectToAction("DashboardTecnico", "Tecnico");
			}
			else
			{
				return RedirectToAction("Inicio", "Home");
			}
		}
		*/
	}
}