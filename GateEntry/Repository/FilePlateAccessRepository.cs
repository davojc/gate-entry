using System.Collections.Generic;
using System.Text.Json;
using GateEntry.Model;

namespace GateEntry.Repository;

public class FilePlateAccessRepository : IPlateAccessRepository
{
    private readonly Dictionary<string, Plate> _plates = new Dictionary<string, Plate>();
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly string _filePath;

    public FilePlateAccessRepository(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "plates.json");

        if (File.Exists(_filePath))
        {
            _plates = Deserialise();
        }
    }

    public async Task<bool> TryAdd(Plate plate)
    {
        await _writeLock.WaitAsync();
        try
        {
            if (_plates.TryAdd(plate.Number, plate))
            {
                await Serialise();
                return true;
            }
            
            return false;
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task<bool> TryDelete(string plate)
    {
        await _writeLock.WaitAsync();

        try
        {
            if (_plates.Remove(plate))
            {
                await Serialise();
                return true;
            }

            return false;
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task<Plate?> TryGet(string plate)
    {
        return await Task.FromResult(_plates.GetValueOrDefault(plate));
    }

    public async Task<bool> Contains(string plate)
    {
        return await Task.FromResult(_plates.ContainsKey(plate));
    }

    public async Task<IEnumerable<Plate>> Get()
    {
        return await Task.FromResult(_plates.Values);
    }

    public async Task<bool> Updated(Plate plate)
    {
        await Serialise();
        return true;
    }

    private async Task Serialise()
    {
        var json = JsonSerializer.Serialize(_plates);
        await File.WriteAllTextAsync(_filePath, json);
    }

    private Dictionary<string, Plate> Deserialise()
    {
        return JsonSerializer.Deserialize<Dictionary<string, Plate>>(File.OpenRead(_filePath));
    }
}