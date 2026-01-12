namespace Lazy.Application.Contracts;

public interface IDBSeedDataService
{
    Task<bool> InitAsync();
}
