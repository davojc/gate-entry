using GateEntry.Model;

namespace GateEntry.Repository;

public interface IPlateAccessRepository
{
    Task<bool> TryAdd(Plate plate);

    Task<bool> TryDelete(string plate);

    Task<Plate?> TryGet(string plate);

    Task<bool> Contains(string plate);

    Task<IEnumerable<Plate>> Get();

    Task<bool> Updated(Plate plate);
}