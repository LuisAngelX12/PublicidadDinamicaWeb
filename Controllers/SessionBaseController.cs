using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PublicidadDinamicaWeb.Controllers
{
    public class SessionBaseController : Controller
    {
        protected int UsuarioId => HttpContext.Session.GetInt32("IdUsuario") ?? 0;
        protected string NombreUsuario => HttpContext.Session.GetString("Nombre") ?? "";
        protected string RolUsuario => HttpContext.Session.GetString("Rol") ?? "Usuario";
        protected string RolesUsuario => HttpContext.Session.GetString("Roles") ?? "Usuario";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var estado = HttpContext.Session.GetInt32("Estado");

                // 🔒 No hay sesión
                if (idUsuario == null)
                {
                    TempData["Error"] = "Sesión expirada. Por favor inicie sesión nuevamente.";
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                    return;
                }

                // 🚫 Usuario inactivo
                if (estado == null || estado == 0)
                {
                    HttpContext.Session.Clear();
                    TempData["Error"] = "Su cuenta está inactiva o suspendida.";
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                    return;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Session.Clear();
                TempData["Error"] = "Ocurrió un error de sesión: " + ex.Message;
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}