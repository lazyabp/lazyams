using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lazy.UnitTest;

public abstract class BaseTest
{
    protected LazyWebApplicationFactory Factory { get; private set; }
    protected HttpClient Client { get; private set; }
    public BaseTest()
    {
        Factory = new LazyWebApplicationFactory();
        Client = this.Factory.CreateClient();
    }

    [OneTimeTearDown]
    protected void Clean()
    {
        if (Client != null)
        {
            Client.Dispose();
            Client = null;
        }

        if (Factory != null)
        {
            Factory.Dispose();
            Factory = null;
        }
    }

    private JsonSerializerOptions serializeOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public T Deserialize<T>(string value)
    {
        return JsonSerializer.Deserialize<T>(value, serializeOptions);
    }
}
