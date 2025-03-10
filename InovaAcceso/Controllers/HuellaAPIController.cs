using InovaAcceso.Data;
using InovaAcceso.Models;
using InovaAcceso.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/HuellaApi")]
public class HuellaApiController : ControllerBase
{
    private readonly HuellaService _huellaService;
    private readonly AppDBContext _appDbContext;
    private readonly UsuarioService _usuarioService;
    public HuellaApiController(HuellaService huellaService, AppDBContext appDbContext, UsuarioService usuarioService)
    {
        _appDbContext = appDbContext;
        _huellaService = huellaService;
        _usuarioService = usuarioService;
    }

    [HttpGet("verificar-dispositivo")]
    public IActionResult verificarDispositivo()
    {
        var response = _huellaService.verificarDispositivo();


        if (!response)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Error al verificar el dispositivo.",
                ErrorCode = "DEVICE_VERIFICATION_ERROR"
            });
        }

        return BadRequest(new
        {
            Success = true,
            Message = "Inicio correcto",
            ErrorCode = "200"
        });
    }

    [HttpPost("capturar-huella")]
    public async Task<IActionResult> CapturarHuella()
    {
        var response = await _huellaService.CapturarHuella();

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("verificar-persona/{documentoNumero}")]
    public async Task<IActionResult> ConsultarPersona(int documentoNumero)
    {
        var persona = await _appDbContext.Personas
            .Include(p => p.Cargo)
            .Include(p => p.Estado)
            .Include(p => p.TipoDocumento)
            .Include(p => p.Rol)
            .FirstOrDefaultAsync(p => p.NumeroDocumento == documentoNumero);

        if (persona == null)
        {
            return NotFound(new FingerprintResponse
            {
                Success = false,
                Message = $"No se encontraron registros para la cédula: {documentoNumero}",
                ErrorCode = "404"
            });
        }

        string nombreCompleto = string.Join(" ",
            new[]
            {
                persona.PrimerNombre,
                persona.SegundoNombre,
                persona.PrimerApellido,
                persona.SegundoApellido
            }.Where(n => !string.IsNullOrEmpty(n)));

        return Ok(new FingerprintResponse
        {
            Success = true,
            Message = nombreCompleto,
            ErrorCode = "200"
        });
    }

    [HttpPost("enrolar-huella")]
    public async Task<IActionResult> enrolarHuella([FromBody] enrolarHuellaRequest request)
    {
        var persona = await _appDbContext.Personas
            .Include(p => p.Cargo)
            .Include(p => p.Estado)
            .Include(p => p.TipoDocumento)
            .Include(p => p.Rol)
            .FirstOrDefaultAsync(p => p.NumeroDocumento == request.DocumentoNumero);

        if (persona == null)
        {
            return NotFound(new FingerprintResponse
            {
                Success = false,
                Message = $"No se encontraron registros para la cédula: {request.DocumentoNumero}",
                ErrorCode = "404"
            });
        }
        // Lógica para guardar la huella en la base de datos
        var nuevaHuella = new Huella
        {
            IdPersona = persona.IdPersona,
            DatosHuella = request.ImagenBytes,
            ResponsableModificacion = _usuarioService.UsuarioNombres
        };

        await _appDbContext.Huellas.AddAsync(nuevaHuella);
        await _appDbContext.SaveChangesAsync();


        return null;
    }

    public class enrolarHuellaRequest
    {
        public int DocumentoNumero { get; set; }
        public byte[] ImagenBytes { get; set; }
    }
    public class FingerprintResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
    }
}