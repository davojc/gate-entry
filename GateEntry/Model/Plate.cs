using System.Text.Json.Serialization;

namespace GateEntry.Model;

public record Plate
{
    [JsonPropertyName("number")]
    public required string Number { get; set; }

    [JsonPropertyName("added")]
    public DateTime Added { get; set; }

    [JsonPropertyName("seen")]
    public DateTime? Seen { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}