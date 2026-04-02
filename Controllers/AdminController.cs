using Microsoft.AspNetCore.Mvc;

namespace PublicidadDinamicaWeb.Controllers
{
    public class AdminController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
