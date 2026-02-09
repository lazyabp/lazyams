namespace Lazy.Application.Contracts.Dto;

public class ConfigDto : BaseEntityDto
{
    public string Key { get; set; }

    public string TypeName { get; set; }

    public string Value { get; set; }
}
