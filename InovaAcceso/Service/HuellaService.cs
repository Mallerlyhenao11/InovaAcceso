using DPUruNet;
using InovaAcceso.Models;
using SkiaSharp;
using System.Runtime.InteropServices;


public class HuellaService
{
    private readonly ILogger<HuellaService> _logger;
    private readonly Reader _reader;
    private readonly LectorService _lectorService;
    public HuellaService(ILogger<HuellaService> logger, LectorService lectorService)
    {
        _logger = logger;
        _lectorService = lectorService;
        _reader = _lectorService.InicializeReader();
    }

    public bool verificarDispositivo()
    {
        _lectorService.InicializeReader();
        return true;
    }

    public async Task<FingerprintResponse> CapturarHuella()
    {
        // Verificar si el lector está inicializado
        if (_reader == null)
        {
            _logger.LogError("❌ No se puede capturar la huella porque el lector no está inicializado.");
            return new FingerprintResponse
            {
                Success = false,
                Message = "El lector de huellas no está inicializado o no se puede abrir.",
                ErrorCode = "READER_NOT_INITIALIZED"
            };
        }
        
        try
        {
            // Configuración de parámetros para la captura
            var format = Constants.Formats.Fid.ANSI;
            var processing = Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT;
            var timeout = 5000; // Tiempo de espera en milisegundos (5 segundos)
            var resolution = 500;

            _logger.LogInformation("Iniciando la captura de huella...");


            // Ejecutar la captura de huella
            var captureResult = await Task.Run(() =>
                _reader.Capture(format, processing, timeout, resolution));

            // Validar si se obtuvieron datos de la huella
            if (captureResult?.Data?.Bytes == null || captureResult.Data.Bytes.Length == 0)
            {
                _logger.LogWarning("No se capturaron datos de la huella.");
                return new FingerprintResponse
                {
                    Success = false,
                    Message = "No se capturaron datos de la huella.",
                    ErrorCode = "NO_FINGERPRINT_DATA"
                };
            }

            // Convertir los datos de la huella a Base64
            string base64Image = ConvertFidToBase64(captureResult.Data);

            _logger.LogInformation("Captura de huella exitosa.");
            return new FingerprintResponse
            {
                Success = true,
                Message = "Captura de huella exitosa",
                Data = base64Image
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la captura de la huella");
            return new FingerprintResponse
            {
                Success = false,
                Message = $"Error durante la captura de la huella: {ex.Message}",
                ErrorCode = "CAPTURE_ERROR"
            };
        }
    }

    public string ConvertFidToBase64(DPUruNet.Fid fid)
    {
        if (fid == null || fid.Views == null || fid.Views.Count == 0)
        {
            throw new ArgumentException("La captura de huella no contiene datos de imagen.");
        }

        byte[] rawImage = fid.Views[0].RawImage;
        int width = fid.Views[0].Width;
        int height = fid.Views[0].Height;

        if (rawImage == null || rawImage.Length == 0)
        {
            throw new ArgumentException("No se pudo obtener la imagen de la huella.");
        }

        using (var bitmap = new SKBitmap(width, height, SKColorType.Gray8, SKAlphaType.Opaque))
        {
            // Copiar los datos de la huella al bitmap correctamente
            IntPtr ptr = bitmap.GetPixels();
            Marshal.Copy(rawImage, 0, ptr, rawImage.Length);

            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                return Convert.ToBase64String(data.ToArray());
            }
        }
    }
}