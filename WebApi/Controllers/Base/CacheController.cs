using Lazy.Core.Caching;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 缓存
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
[ApiController]
public class CacheController : ControllerBase
{
    private readonly ICaching _caching;

    public CacheController()
    {
        _caching = CacheFactory.Cache;
    }

    /// <summary>
    /// 获取缓存列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(PermissionConsts.Cache.Default)]
    public List<KeyValueDto> GetCacheTags()
    {
        return [
            new KeyValueDto(CacheConsts.RoleCacheTag, "角色缓存", "管理与角色相关的所有缓存内容"),
            new KeyValueDto(CacheConsts.UserCacheTag, "用户缓存", "管理与用户相关的所有缓存内容"),
            new KeyValueDto(CacheConsts.UserPermissionCacheTag, "权限缓存", "管理与权限相关的所有缓存内容"),
            new KeyValueDto(CacheConsts.ConfigCacheTag, "配置缓存", "管理与配置相关的所有缓存内容"),
            new KeyValueDto(CacheConsts.MenuCacheTag, "菜单缓存", "管理与菜单相关的所有缓存内容")
        ];
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    [HttpDelete("{tag}")]
    [Authorize(PermissionConsts.Cache.Delete)]
    public bool Delete(string tag)
    {
        _caching.RemoveHashCache(tag);

        return true;
    }

    /// <summary>
    /// 清空缓存
    /// </summary>
    /// <returns></returns>
    [HttpDelete()]
    [Authorize(PermissionConsts.Cache.Delete)]
    public bool Clear()
    {
        var tags = new List<string> {
            CacheConsts.MenuCacheTag,
            CacheConsts.RoleCacheTag,
            CacheConsts.UserCacheTag,
            CacheConsts.ConfigCacheTag,
            CacheConsts.UserPermissionCacheTag
        };

        foreach (var tag in tags)
        {
            _caching.RemoveHashCache(tag);
        }

        return true;
    }
}
