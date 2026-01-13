using Microsoft.AspNetCore.Authorization;

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
        private readonly ICarouselService _carouselService;

        public CarouselController(ICarouselService carouselService)
        {
            _carouselService = carouselService;
        }

        [HttpGet]
        //[Authorize(PermissionConsts.Carousel.Default)]
        public async Task<PagedResultDto<CarouselDto>> GetByPageAsync([FromQuery] FilterPagedResultRequestDto input)
        {
            return await _carouselService.GetListAsync(input);
        }

        /// <summary>
        /// Get carousel by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize(PermissionConsts.Carousel.Default)]
        [HttpGet("{id}")]
        public async Task<CarouselDto> GetById(long id)
        {
            return await _carouselService.GetAsync(id);
        }

        /// <summary>
        /// Creates a new carousel item using the specified input data.
        /// </summary>
        /// <remarks>
        /// Requires the caller to have the appropriate permission to add carousel items. This
        /// endpoint is accessible via HTTP GET, which is unconventional for resource creation; consider using HTTP POST
        /// for standard RESTful practices.
        /// </remarks>
        /// <param name="input">
        /// The data used to create the new carousel item. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CarouselDto"/>
        /// representing the newly created carousel item.
        /// </returns>
        [HttpGet]
        [Authorize(PermissionConsts.Carousel.Add)]
        public async Task<CarouselDto> Add([FromBody] CreateCarouselDto input)
        {
            return await _carouselService.CreateAsync(input);
        }

        /// <summary>
        /// udpate carousel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(PermissionConsts.Carousel.Update)]
        [HttpPost]
        public async Task<CarouselDto> Update([FromBody] UpdateCarouselDto input)
        {
            return await _carouselService.UpdateAsync(input.Id, input);
        }

        /// <summary>
        /// delete carousel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(PermissionConsts.Carousel.Delete)]
        [HttpDelete("{id}")]
        public async Task<bool> Delete(long id)
        {
            await _carouselService.DeleteAsync(id);

            return true;
        }
    }
}
