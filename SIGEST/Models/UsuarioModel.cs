using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIGEST.Models
{
	public class UsuarioModel
	{
		[Required(ErrorMessage = "El correo es requerido")]
		[EmailAddress(ErrorMessage = "Correo inválido")]
		public string Email { get; set; }

		[Required(ErrorMessage = "La contraseña es requerida")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public bool RememberMe { get; set; }
	}
}