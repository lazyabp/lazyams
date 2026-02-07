using System.Threading.Tasks;

namespace Lazy.Application.Contracts;

public interface IAutoJobService : ICrudService<AutoJobDto, AutoJobDto, long, FilterPagedResultRequestDto, CreateAutoJobDto, UpdateAutoJobDto>
{

}
