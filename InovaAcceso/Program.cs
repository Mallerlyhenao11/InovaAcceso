using InovaAcceso.Data;
using InovaAcceso.Models;
using InovaAcceso.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using InovaAcceso.Extension;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Se agrega la autenticaci�n por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login"; // Ruta de inicio de sesi�n
        options.LogoutPath = "/Home/Logout"; // Ruta de cierre de sesi�n
        options.AccessDeniedPath = "/Home/AccessDenied"; // Ruta de acceso denegado
    });

// Configurar servicios
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSql"));
});

// Se agrega para la autenticaci�n de la librer�a del PDF
var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "LibreriaPDF/libwkhtmltox.dll"));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

var app = builder.Build();

// Configuraci�n del middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Configurar el pipeline de la aplicaci�n
app.UseRouting();
app.UseAuthentication(); // Habilitar autenticaci�n
app.UseAuthorization(); // Habilitar autorizaci�n
// Configurar rutas por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
