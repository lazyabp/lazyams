using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WebApi;

namespace Lazy.UnitTest;

public class LazyWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddHttpClient();
            services.AddAuthentication(defaultScheme: "testScheme").
            AddScheme<AuthenticationSchemeOptions, LazyTestAuthHandler>("testScheme", options =>
            {

            }); 
        });
    }
}
