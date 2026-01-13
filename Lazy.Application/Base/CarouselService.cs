namespace Lazy.Application;

public class CarouselService : CrudService<Carousel, CarouselDto, CarouselDto, long, FilterPagedResultRequestDto, CreateCarouselDto, UpdateCarouselDto>,
    ICarouselService, ITransientDependency
{
    private readonly ILazyCache _lazyCache;

    public CarouselService(LazyDBContext dbContext, IMapper mapper, ILazyCache lazyCache)
        : base(dbContext, mapper)
    {
        _lazyCache = lazyCache;
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
