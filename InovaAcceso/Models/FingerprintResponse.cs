namespace InovaAcceso.Models
{
    public class FingerprintResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public string ErrorCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
