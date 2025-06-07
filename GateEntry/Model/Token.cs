using System.Text.Json.Serialization;

namespace GateEntry.Model;

public record Token
{
    [JsonPropertyName("value")]
    public required string Value { get; set; }

    [JsonPropertyName("created")]
    public DateTime Created { get; set; }
}