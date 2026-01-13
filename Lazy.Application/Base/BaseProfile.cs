
namespace Lazy.Application;

public class BaseProfile : Profile
{
    public BaseProfile()
    {
        CreateMap<Lazy.Model.Entity.File, FileDto>().ReverseMap();
        CreateMap<CreateFileDto, Lazy.Model.Entity.File>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateFileDto, Lazy.Model.Entity.File>();

        CreateMap<Setting, SettingDto>().ReverseMap();
        CreateMap<CreateSettingDto, Setting>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateSettingDto, Setting>();

        CreateMap<SocialiteUser, SocialiteUserDto>().ReverseMap();
        CreateMap<CreateSocialiteUserDto, SocialiteUser>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateSocialiteUserDto, SocialiteUser>();

        CreateMap<Carousel, CarouselDto>().ReverseMap();
        CreateMap<CreateCarouselDto, Carousel>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateCarouselDto, Carousel>();
    }
}
