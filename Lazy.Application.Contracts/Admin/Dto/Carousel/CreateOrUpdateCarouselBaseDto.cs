
namespace Lazy.Application.Contracts.Admin;

public class CreateOrUpdateCarouselBaseDto : BaseEntityDto
{
    public string Title { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public string RedirectUrl { get; set; }

    public bool IsActive { get; set; }

    public int Position { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
