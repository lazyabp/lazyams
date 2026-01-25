namespace Lazy.Application.Contracts.Dto;

public class CreateOrUpdateConfigBaseDto : BaseEntityDto
{
    public string Key { get; set; }

    public string Value { get; set; }
}
