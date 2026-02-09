using System.Threading.Tasks;

namespace Lazy.Application.Contracts;

public interface ICarouselService : ICrudService<CarouselDto, CarouselDto, long, FilterPagedResultRequestDto, CreateCarouselDto, UpdateCarouselDto>
{

}
