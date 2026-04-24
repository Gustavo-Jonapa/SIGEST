using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace SIGEST.Models
{
	public class LoginModel
	{
		[Required(ErrorMessage = "El usuario es requerido")]
		public string Usuario { get; set; }

		[Required(ErrorMessage = "La contraseña es requerida")]
		public string Contraseña { get; set; }
	}
}