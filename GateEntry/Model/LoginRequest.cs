using System.Text.Json.Serialization;

namespace GateEntry.Model;

public record LoginRequest
{
    [JsonPropertyName("pin")]
    public required string Pin { get; set; }
}