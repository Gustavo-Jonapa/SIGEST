using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SIGEST
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			// Ruta específica para Login (para URLs limpias)
			routes.MapRoute(
				name: "Login",
				url: "Login",
				defaults: new { controller = "Login", action = "Login" }
			);

			// Ruta específica para Logout
			routes.MapRoute(
				name: "Logout",
				url: "Logout",
				defaults: new { controller = "Login", action = "Logout" }
			);

			// Ruta para área de Técnicos
			routes.MapRoute(
				name: "Tecnico",
				url: "Tecnico/{action}/{id}",
				defaults: new { controller = "Tecnico", action = "DashboardTecnico", id = UrlParameter.Optional }
			);

			// Ruta para área de Clientes (Home)
			routes.MapRoute(
				name: "Cliente",
				url: "Cliente/{action}/{id}",
				defaults: new { controller = "Home", action = "Inicio", id = UrlParameter.Optional }
			);

			// Ruta por defecto - Redirige al Login
			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
			);
		}
	}
}
