using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace controlLuces.Controllers
{
    public class ArchivoController : Controller
    {
        // GET: Archivo
        public ActionResult NuevoArchivo()
        {
            return View();
        }
    }
}