using Lazy.Core.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Lazy.UnitTest;

public class LazyCacheTest : BaseTest
{
    [Test]
    public async Task Test()
    {
        using (var scope = Factory.Services.CreateScope())
        {
            var LazyCache = CacheFactory.Cache;
            string key = "key1";
            string val = "123dadfdsaf";
            LazyCache.SetCache(key, val);
            var cVal = LazyCache.GetCache<string>(key);
            Assert.That(val == cVal, "");
        }
    }
}
