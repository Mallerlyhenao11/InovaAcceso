using InovaAcceso.Data;
using InovaAcceso.Service;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using InovaAcceso.Extension;

var builder = WebApplication.CreateBuilder(args);

// ?? Configurar conexión a la base de datos
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSql"))
);
builder.Services.AddSingleton<UsuarioService>();
// ?? Configurar sesiones
builder.Services.AddHttpContextAccessor();  // ? No es necesario agregarlo de nuevo como Singleton
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Expira en 30 minutos
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ?? Configurar controladores con vistas
builder.Services.AddControllersWithViews();

// ?? Configurar DinkToPdf para generación de PDFs
string pdfLibraryPath = Path.Combine(Directory.GetCurrentDirectory(), "LibreriaPDF/libwkhtmltox.dll");
if (File.Exists(pdfLibraryPath))
{
    var context = new CustomAssemblyLoadContext();
    context.LoadUnmanagedLibrary(pdfLibraryPath);
    builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
}

var app = builder.Build();

// ?? Configuración del middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Asegurar HTTPS en producción
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();  // ? Correctamente antes de Authentication y Authorization
app.UseAuthentication();
app.UseAuthorization();

// ?? Configurar rutas por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Reloj}/{id?}");

app.Run();
