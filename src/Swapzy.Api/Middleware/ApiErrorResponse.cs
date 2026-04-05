using System.Text.Json.Serialization;

namespace Swapzy.Api.Middleware
{
    public class ApiErrorResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; } = default!;

        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;

        [JsonPropertyName("traceId")]
        public string TraceId { get; set; } = default!;

        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
