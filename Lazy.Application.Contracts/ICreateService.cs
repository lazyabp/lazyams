namespace Lazy.Application.Contracts;

public interface ICreateService<TGetOutputDto, in TCreateInput>
{
    Task<TGetOutputDto> CreateAsync(TCreateInput input);
}
