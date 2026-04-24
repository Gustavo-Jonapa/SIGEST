using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIGEST.Models
{
	public class RegistroModel
	{
		[Required(ErrorMessage = "El nombre es requerido")]
		public string Name { get; set; }

		[Required(ErrorMessage = "El correo es requerido")]
		[EmailAddress(ErrorMessage = "Correo inválido")]
		public string Email { get; set; }

		[Required(ErrorMessage = "El teléfono es requerido")]
		public string Phone { get; set; }

		[Required(ErrorMessage = "La contraseña es requerida")]
		[StringLength(100, MinimumLength = 8)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
		public string ConfirmPassword { get; set; }

		[Required]
		public string Role { get; set; } // "cliente" o "tecnico"

		[Required]
		public bool AcceptTerms { get; set; }
	}
}