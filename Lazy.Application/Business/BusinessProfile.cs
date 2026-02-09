
namespace Lazy.Application;

public class BusinessProfile : Profile
{
    public BusinessProfile()
    {
        CreateMap<Order, OrderDto>().ReverseMap();

        CreateMap<Package, PackageDto>().ReverseMap();
        CreateMap<CreatePackageDto, Package>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdatePackageDto, Package>();

        CreateMap<PackageFeature, PackageFeatureDto>().ReverseMap();
        CreateMap<CreatePackageFeatureDto, PackageFeature>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdatePackageFeatureDto, PackageFeature>();

        CreateMap<UserSubscription, UserSubscriptionDto>().ReverseMap();
        CreateMap<CreateUserSubscriptionDto, UserSubscription>().ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
