
namespace Lazy.Application;

public class BaseProfile : Profile
{
    public BaseProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserWithRoleIdsDto>();
        CreateMap<CreateUserDto, User>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateUserDto, User>();

        CreateMap<Menu, MenuDto>();
        CreateMap<MenuIdDto, Menu>().ReverseMap().ForMember(dest => dest.MenuId, opt => opt.MapFrom(src => src.Id));
        CreateMap<CreateMenuDto, Menu>();
        CreateMap<UpdateMenuDto, Menu>();

        CreateMap<Role, RoleDto>();
        CreateMap<CreateRoleDto, Role>();
        CreateMap<UpdateRoleDto, Role>();
        CreateMap<LoginRequestDto, User>();

        CreateMap<Setting, SettingDto>().ReverseMap();
        CreateMap<CreateSettingDto, Setting>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateSettingDto, Setting>();

        CreateMap<SocialiteUser, SocialiteUserDto>().ReverseMap();
        CreateMap<CreateSocialiteUserDto, SocialiteUser>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateSocialiteUserDto, SocialiteUser>();

        CreateMap<Lazy.Model.Entity.File, FileDto>().ReverseMap();
        CreateMap<CreateFileDto, Lazy.Model.Entity.File>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateFileDto, Lazy.Model.Entity.File>();

        CreateMap<Carousel, CarouselDto>().ReverseMap();
        CreateMap<CreateCarouselDto, Carousel>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateCarouselDto, Carousel>();
    }
}
