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
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}
		/*
		// POST: Account/Login
		[HttpPost]
		[ValidateAntiForgeryToken]
		
		public ActionResult Login(LoginViewModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				// Aquí va tu lógica de autenticación
				// Ejemplo básico:
				var user = ValidateUser(model.Email, model.Password);

				if (user != null)
				{
					// Crear cookie de autenticación
					FormsAuthentication.SetAuthCookie(user.Email, model.RememberMe);

					// Redirigir según el rol
					if (user.Role == "Tecnico")
					{
						return RedirectToAction("Dashboard", "Tecnico");
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}

				ModelState.AddModelError("", "Usuario o contraseña incorrectos");
			}

			return View(model);
		}

		// POST: Account/Register
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				// Validar que las contraseñas coincidan
				if (model.Password != model.ConfirmPassword)
				{
					return Json(new
					{
						success = false,
						message = "Las contraseñas no coinciden"
					});
				}

				try
				{
					// Crear usuario
					var user = CreateUser(model);

					// Autenticar automáticamente
					FormsAuthentication.SetAuthCookie(user.Email, false);

					// Redirigir según el rol
					string redirectUrl = model.Role == "tecnico"
						? Url.Action("Dashboard", "Tecnico")
						: Url.Action("Index", "Home");

					return Json(new
					{
						success = true,
						redirectUrl = redirectUrl
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

			return Json(new
			{
				success = false,
				message = "Datos inválidos"
			});
		}

		// GET: Account/Logout
		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			return RedirectToAction("Login");
		}

		// POST: Account/ForgotPassword
		[HttpPost]
		public ActionResult ForgotPassword(string email)
		{
			// Lógica para enviar correo de recuperación
			try
			{
				SendPasswordResetEmail(email);
				return Json(new
				{
					success = true,
					message = "Se ha enviado un correo con instrucciones"
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = ex.Message
				});
			}
		}

		// Métodos privados (implementar según tu BD)
		private User ValidateUser(string email, string password)
		{
			// Implementar validación contra base de datos
			// return db.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);
			return null;
		}

		private User CreateUser(RegisterViewModel model)
		{
			// Implementar creación de usuario en base de datos
			// Hash de contraseña, etc.
			return null;
		}
		
		private void SendPasswordResetEmail(string email)
		{
			// Implementar envío de correo
		}
		*/
	}
}