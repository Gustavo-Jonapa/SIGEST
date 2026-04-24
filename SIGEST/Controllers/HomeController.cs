using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIGEST.Controllers
{
	[Authorize]
	public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Inicio()
        {
            return View();
        }
        public ActionResult Historial()
        {
            return View();
        }
        public ActionResult Perfil()
        {
            return View();
        }
    }
}