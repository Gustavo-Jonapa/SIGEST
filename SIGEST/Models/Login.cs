using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIGEST.Models
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "El correo es requerido")]
		[EmailAddress(ErrorMessage = "Correo inválido")]
		public string Email { get; set; }

		[Required(ErrorMessage = "La contraseña es requerida")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public bool RememberMe { get; set; }
	}
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "El nombre es requerido")]
		[StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
		public string Name { get; set; }

		[Required(ErrorMessage = "El correo es requerido")]
		[EmailAddress(ErrorMessage = "Correo inválido")]
		public string Email { get; set; }

		[Required(ErrorMessage = "El teléfono es requerido")]
		[Phone(ErrorMessage = "Teléfono inválido")]
		public string Phone { get; set; }

		[Required(ErrorMessage = "La contraseña es requerida")]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirma tu contraseña")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
		public string ConfirmPassword { get; set; }

		[Required(ErrorMessage = "Selecciona un tipo de cuenta")]
		public string Role { get; set; } // "cliente" o "tecnico"

		[Required(ErrorMessage = "Debes aceptar los términos")]
		public bool AcceptTerms { get; set; }
	}
}