using Microsoft.AspNetCore.Http;

namespace Lazy.Application.Contracts;

public interface IAvatarService
{
    public Task<string> UploadAvatarAsync(string userName, IFormFile file);
    public Task DeleteAvatarAsync(string userName);
    public Task<string> GetAvatarUrlAsync(string userName);
}
