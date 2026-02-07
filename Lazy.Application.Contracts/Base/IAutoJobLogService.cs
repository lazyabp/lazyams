namespace Lazy.Application.Contracts;

public interface IAutoJobLogService : ICrudService<AutoJobLogDto, AutoJobLogDto, long, FilterPagedResultRequestDto, CreateAutoJobLogDto, UpdateAutoJobLogDto>
{
    Task Clear();
}
