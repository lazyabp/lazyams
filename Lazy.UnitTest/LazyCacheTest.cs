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
            var LazyCache = scope.ServiceProvider.GetRequiredService<ILazyCache>();
            string key = "key1";
            string val = "123dadfdsaf";
            await LazyCache.SetAsync(key, val, 5);
            var cVal =await LazyCache.GetAsync<string>(key);
            Assert.That(val == cVal, "");
        }
    }
}
