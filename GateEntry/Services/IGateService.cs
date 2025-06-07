using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace GateEntry.Services;

public enum Gate
{
    In,
    Out
}

public class OpenGateCommand(Gate gate)
{
    [JsonPropertyName("entity_id")]
    public string EntityId => gate == Gate.In ? "automation.openingate" : "automation.openoutgate";
}

public interface IGateService
{
    Task<bool> Open(Gate gate);
}

public class GateService : IGateService
{
    private readonly IOptions<Settings> _settings;
    private readonly HttpClient _client = new HttpClient();

    public GateService(IOptions<Settings> settings)
    {
        _settings = settings;
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", settings.Value.Gate.Token);
    }

    public async Task<bool> Open(Gate gate)
    {
        var command = new OpenGateCommand(gate);
        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(_settings.Value.Gate.Endpoint, content);

        return response.IsSuccessStatusCode;
    }
}

public class MockGateService : IGateService
{
    public Task<bool> Open(Gate gate)
    {
        Console.WriteLine("Opening gate: {0}", gate);

        return Task.FromResult(true);
    }
}