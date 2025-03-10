using DPUruNet;

public class LectorService
{
    private readonly ILogger<LectorService> _logger;
    private readonly object _lockObject = new(); // Objeto para sincronización
    private Reader _reader; // Propiedad para acceder al lector

    public LectorService(ILogger<LectorService> logger)
    {
        _logger = logger;
        InicializeReader();
    }

    // Método para inicializar el lector y devolverlo
    public Reader InicializeReader()
    {
        try
        {
            lock (_lockObject)
            {
                // Liberar recursos si ya existe un lector inicializado
                if (_reader != null)
                {
                    _reader.Dispose();
                }

                // Obtener la lista de lectores conectados
                var readers = ReaderCollection.GetReaders();

                if (readers == null || readers.Count == 0)
                {
                    _logger.LogWarning("No se encontraron lectores de huellas conectados.");
                    return null;
                }

                // Seleccionar el primer lector disponible
                _reader = readers[0];
                var result = _reader.Open(Constants.CapturePriority.DP_PRIORITY_EXCLUSIVE);
                if (result != Constants.ResultCode.DP_SUCCESS)
                {
                    _logger.LogError($"Error al abrir el lector: {result}");
                }

                return _reader;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar el lector de huellas.");
            return null; // Devolver null si ocurre un error
        }
    }
}