using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace InovaAcceso.Filters
{
    public class AuthorizeSessionAttribute : ActionFilterAttribute
    {
        private readonly string _rolRequerido;

        public AuthorizeSessionAttribute(string rolRequerido = "")
        {
            _rolRequerido = rolRequerido;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var usuarioRol = httpContext.Session.GetString("UsuarioRol");

            if (string.IsNullOrEmpty(usuarioRol)) // Si no hay sesión, redirige al login
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }

            if (!string.IsNullOrEmpty(_rolRequerido) && usuarioRol != _rolRequerido) // Verifica el rol si es necesario
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }

}
