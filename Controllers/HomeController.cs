using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PublicidadDinamicaWeb.Models;

namespace PublicidadDinamicaWeb.Controllers
{
    public class HomeController : SessionBaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol == "Admin")
                return RedirectToAction("Index", "Admin");
            else if (rol == "Usuario")
                return RedirectToAction("Index", "Cliente");
            else if (rol == "Pantalla")
                return RedirectToAction("Pantalla", "Publicidad");
            else if (rol == "Operador")
                return RedirectToAction("Index", "Operador");
            else
                return RedirectToAction("Login", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
