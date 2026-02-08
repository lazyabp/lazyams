using System.Threading.Tasks;

namespace Lazy.Application.Contracts;

public interface IAutoJobService : ICrudService<AutoJobDto, AutoJobDto, long, FilterPagedResultRequestDto, CreateAutoJobDto, UpdateAutoJobDto>
{
    Task<AutoJobDto> ExecuteAsync(long id, JobAction action);
}
