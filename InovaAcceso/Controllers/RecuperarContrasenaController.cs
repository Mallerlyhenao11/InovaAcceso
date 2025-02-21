using InovaAcceso.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InovaAcceso.Controllers
{
    public class RecuperarContrasenaController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public RecuperarContrasenaController(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> OlvideContrasena(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Por favor, introduce un correo electrónico válido.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // No revelar si el correo existe o no por seguridad
                return RedirectToAction("OlvideContrasenaConfirmacion");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("RestablecerContrasena", "RecuperarContrasena",
                new { token, email = user.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(
                email,
                "Restablecimiento de Contraseña",
                $"Por favor, restablece tu contraseña haciendo clic <a href='{callbackUrl}'>aquí</a>.");

            return RedirectToAction("OlvideContrasenaConfirmacion");
        }

        [HttpGet]
        public IActionResult OlvideContrasenaConfirmacion()
        {
            return View();
        }
    }
}
