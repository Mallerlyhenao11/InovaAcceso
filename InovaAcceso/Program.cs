using InovaAcceso.Data;
using InovaAcceso.Service;
using InovaAcceso.Extension;
using InovaAcceso.Controllers;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using DPUruNet; // Importaci�n necesaria para Reader

var builder = WebApplication.CreateBuilder(args);

// ======================== CONFIGURACI�N DE SERVICIOS ========================

// Configuraci�n de controladores
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

// Configuraci�n de CORS (permitir solicitudes desde cualquier origen)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Configuraci�n de logging (consola y depuraci�n)
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

// Configuraci�n de la base de datos (usando SQL Server)
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSql"))
);

// Registro de servicios
builder.Services.AddSingleton<UsuarioService>(); // Servicio singleto
builder.Services.AddSingleton<LectorService>();
builder.Services.AddSingleton<HuellaService>();

// Configuraci�n de sesiones (30 minutos de tiempo inactivo)
builder.Services.AddHttpContextAccessor(); // Acceso al contexto HTTP
builder.Services.AddDistributedMemoryCache(); // Cache en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configuraci�n de DinkToPdf para generaci�n de PDFs
string pdfLibraryPath = Path.Combine(Directory.GetCurrentDirectory(), "LibreriaPDF/libwkhtmltox.dll");
if (File.Exists(pdfLibraryPath))
{
    var context = new CustomAssemblyLoadContext();
    context.LoadUnmanagedLibrary(pdfLibraryPath);
    builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
}

// ======================== CONSTRUCCI�N DE LA APLICACI�N ========================
var app = builder.Build();

// ======================== CONFIGURACI�N DEL MIDDLEWARE ========================

// Manejo de errores
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // P�gina de errores detallados en desarrollo
}
else
{
    app.UseExceptionHandler("/Home/Error"); // P�gina de errores personalizada en producci�n
    app.UseHsts(); // Habilitar HSTS (seguridad para HTTPS)
}

// Redirecci�n a HTTPS y archivos est�ticos
app.UseHttpsRedirection();
app.UseStaticFiles();

// Configuraci�n de enrutamiento y sesiones
app.UseRouting();
app.UseSession();

// Autenticaci�n y autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

// ======================== CONFIGURACI�N DE RUTAS ========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Reloj}/{id?}"
);

// ======================== EJECUCI�N DE LA APLICACI�N ========================
app.Run();
