using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PublicidadDinamicaWeb.Controllers
{
    public class AdminBaseController : SessionBaseController
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (RolUsuario != "Admin")
            {
                TempData["Error"] = "No tiene permisos para acceder a esta sección.";
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }
        }

    }
}
