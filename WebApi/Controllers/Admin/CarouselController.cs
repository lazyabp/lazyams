
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Admin
{
    /// <summary>
    /// Provides API endpoints for managing carousel resources in the administration service.
    /// </summary>
    /// <remarks>
    /// This controller is part of the AdminService API group and is intended for administrative
    /// operations related to carousels. All routes are prefixed with 'api/Carousel/'.
    /// </remarks>
    [ApiExplorerSettings(GroupName = nameof(SwaggerGroup.AdminService))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CarouselController : ControllerBase
    {
    }
}
