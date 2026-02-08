namespace Lazy.Application;

public class CarouselService : CrudService<Carousel, CarouselDto, CarouselDto, long, FilterPagedResultRequestDto, CreateCarouselDto, UpdateCarouselDto>,
    ICarouselService, ITransientDependency
{
    //private readonly ICaching _cacheing;

    public CarouselService(LazyDBContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
        //_cacheing = CacheFactory.Cache;
    }

    protected override IQueryable<Carousel> CreateFilteredQuery(FilterPagedResultRequestDto input)
    {
        if (!string.IsNullOrEmpty(input.Filter))
        {
            return GetQueryable().Where(x => x.Title.Contains(input.Filter));
        }

        return base.CreateFilteredQuery(input);
    }
}
