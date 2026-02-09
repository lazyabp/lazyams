namespace Lazy.Application.Contracts;

public interface IAutoJobLogService : ICrudService<AutoJobLogDto, AutoJobLogListDto, long, FilterPagedResultRequestDto, CreateAutoJobLogDto, UpdateAutoJobLogDto>
{
    Task ClearAsync();
}
