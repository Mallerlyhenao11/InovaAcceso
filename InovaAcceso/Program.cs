using InovaAcceso.Data;
using InovaAcceso.Service;
using InovaAcceso.Extension;
using InovaAcceso.Controllers;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using DPUruNet; // Importación necesaria para Reader

var builder = WebApplication.CreateBuilder(args);

// ======================== CONFIGURACIÓN DE SERVICIOS ========================

// Configuración de controladores
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

// Configuración de CORS (permitir solicitudes desde cualquier origen)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Configuración de logging (consola y depuración)
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

// Configuración de la base de datos (usando SQL Server)
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSql"))
);

// Registro de servicios
builder.Services.AddSingleton<UsuarioService>(); // Servicio singleto
builder.Services.AddSingleton<LectorService>();
builder.Services.AddSingleton<HuellaService>();

// Configuración de sesiones (30 minutos de tiempo inactivo)
builder.Services.AddHttpContextAccessor(); // Acceso al contexto HTTP
builder.Services.AddDistributedMemoryCache(); // Cache en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configuración de DinkToPdf para generación de PDFs
string pdfLibraryPath = Path.Combine(Directory.GetCurrentDirectory(), "LibreriaPDF/libwkhtmltox.dll");
if (File.Exists(pdfLibraryPath))
{
    var context = new CustomAssemblyLoadContext();
    context.LoadUnmanagedLibrary(pdfLibraryPath);
    builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
}

// ======================== CONSTRUCCIÓN DE LA APLICACIÓN ========================
var app = builder.Build();

// ======================== CONFIGURACIÓN DEL MIDDLEWARE ========================

// Manejo de errores
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Página de errores detallados en desarrollo
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Página de errores personalizada en producción
    app.UseHsts(); // Habilitar HSTS (seguridad para HTTPS)
}

// Redirección a HTTPS y archivos estáticos
app.UseHttpsRedirection();
app.UseStaticFiles();

// Configuración de enrutamiento y sesiones
app.UseRouting();
app.UseSession();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// ======================== CONFIGURACIÓN DE RUTAS ========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Reloj}/{id?}"
);

// ======================== EJECUCIÓN DE LA APLICACIÓN ========================
app.Run();
