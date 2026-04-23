using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIGEST.Controllers
{
    public class TecnicoController : Controller
    {
        // GET: Tecnico
        public ActionResult DashboardTecnico()
        {
            return View();
        }
        public ActionResult PerfilTecnico()
        {
            return View();
        }
    }
}