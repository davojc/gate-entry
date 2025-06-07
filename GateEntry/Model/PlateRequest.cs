using System.Text.Json.Serialization;

namespace GateEntry.Model;

public record PlateRequest
{
    [JsonPropertyName("number")]
    public required string Number { get; set; }
}