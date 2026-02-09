namespace Lazy.Application.Contracts.Dto;

public class CreateMenuDto : CreateOrUpdateMenuBaseDto
{
    public bool CreateAddButton { get; set; } = false;
    public bool CreateUpdateButton { get; set; } = false;
    public bool CreateDeleteButton { get; set; } = false;
}
