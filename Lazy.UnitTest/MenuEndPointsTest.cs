using Lazy.Core.WrapperResult;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lazy.UnitTest;

public class MenuEndPointsTest : BaseTest
{
    [Order(1)]
    [Test, Sequential]
    public async Task TestGetByPageAsync([Values("Menu1", "Menu2")] string filter)
    {
        var resp = await Client.GetAsync("/api/menu/GetByPage?PageIndex=1&PageSize=12&Filter=" + filter);
        Assert.That(resp, Is.Not.Null);
        Assert.That(HttpStatusCode.OK == resp.StatusCode, "The status code is incorrect");

        var stringResult = await resp.Content.ReadAsStringAsync();
        Assert.That(stringResult, Is.Not.Null);

        var serializeOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var jsonResult = JsonSerializer.Deserialize<ApiResponseResult<PagedResultDto<MenuDto>>>(stringResult, serializeOptions);

        Assert.That(jsonResult, Is.Not.Null);
        Assert.That(jsonResult.IsSuccess, Is.True);
    }

    [Order(2)]
    [Test, Sequential]
    public async Task TestGetByIdAsync([Values(1, 2)] long id)
    {
        var resp = await Client.GetAsync("/api/menu/GetById/" + id);
        Assert.That(resp, Is.Not.Null);
        if(HttpStatusCode.OK == resp.StatusCode)
        {
            Assert.That(HttpStatusCode.OK == resp.StatusCode, "The status code is incorrect");
            var stringResult = await resp.Content.ReadAsStringAsync();
            Assert.That(stringResult, Is.Not.Null);

            var jsonResult = Deserialize<ApiResponseResult<MenuDto>>(stringResult);

            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult.IsSuccess, Is.True);
            Assert.That(id, Is.EqualTo(jsonResult.Data?.Id));
        }
        else
        {
            Assert.That(HttpStatusCode.NotFound == resp.StatusCode, "The status code is incorrect");
        } 
    }

    [Order(3)]
    [Test, Sequential]
    public async Task TestAddAsync(
        [Values("Menu1", "Menu2")] string title,
        [Values("Description1", "Description2")] string description,
        [Values(MenuType.Dir, MenuType.Menu)] MenuType menuType)
    {
        CreateMenuDto menu = new CreateMenuDto
        {
            Title = title,
            Description = description,
            MenuType = menuType,
            ParentId = null
        };

        var jsonContent = JsonContent.Create(menu);

        var resp = await Client.PostAsync("/api/menu/Add", jsonContent);
        Assert.That(resp, Is.Not.Null);
        Assert.That(HttpStatusCode.OK == resp.StatusCode, "The status code is incorrect");

        var stringResult = await resp.Content.ReadAsStringAsync();
        Assert.That(stringResult, Is.Not.Null);

        var jsonResult = Deserialize<ApiResponseResult<bool>>(stringResult);

        Assert.That(jsonResult, Is.Not.Null);
        Assert.That(jsonResult.IsSuccess, Is.True);
    }

    [Order(4)]
    [Test, Sequential]
    public async Task TestUpdateAsync(
        [Values(1, 2)] long id,
        [Values("Updated Menu1", "Updated Menu2")] string title,
        [Values("Updated Description1", "Updated Description2")] string description,
        [Values(MenuType.Menu, MenuType.Btn)] MenuType menuType)
    {
        UpdateMenuDto menu = new UpdateMenuDto
        {
            Id = id,
            Title = title,
            Description = description,
            MenuType = menuType
        };

        var jsonContent = JsonContent.Create(menu);

        var resp = await Client.PostAsync("/api/menu/Update", jsonContent);
        Assert.That(resp, Is.Not.Null);
        if(HttpStatusCode.OK == resp.StatusCode)
        {
            Assert.That(HttpStatusCode.OK == resp.StatusCode, "The status code is incorrect");

            var stringResult = await resp.Content.ReadAsStringAsync();
            Assert.That(stringResult, Is.Not.Null);

            var jsonResult = Deserialize<ApiResponseResult<bool>>(stringResult);

            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult.IsSuccess, Is.True);
        }
        else
        {
            Assert.That(HttpStatusCode.NotFound == resp.StatusCode, "The status code is incorrect");
        }
        
    }

    [Order(5)]
    [Test, Sequential]
    public async Task TestDeleteAsync()
    {
        var ids = await GetMenuIds();
        foreach (var id in ids)
        {
            var getByIdResp = await Client.GetAsync($"/api/menu/GetById/{id}");
            Assert.That(getByIdResp, Is.Not.Null);
            Assert.That(HttpStatusCode.OK == getByIdResp.StatusCode);

            var menu = Deserialize<ApiResponseResult<MenuDto>>(await getByIdResp.Content.ReadAsStringAsync());
            Assert.That(menu, Is.Not.Null);
            Assert.That(menu.IsSuccess, Is.True);

            if (menu.Data?.Children != null && menu.Data.Children.Any())
            {
                var deleteResp = await Client.DeleteAsync($"/api/menu/Delete/{id}");
                Assert.That(deleteResp, Is.Not.Null);
                Assert.That(HttpStatusCode.BadRequest == deleteResp.StatusCode);
            }
            else
            {
                var deleteResp = await Client.DeleteAsync($"/api/menu/Delete/{id}");
                Assert.That(deleteResp, Is.Not.Null);
                Assert.That(HttpStatusCode.OK == deleteResp.StatusCode);
            }
        }
    }

    private async Task<List<long>> GetMenuIds()
    {
        var resp = await this.Client.GetAsync($"/api/menu/GetByPage?PageIndex=1&PageSize=100");
        Assert.That(resp, Is.Not.Null);
        Assert.That(HttpStatusCode.OK == resp.StatusCode, $"Expected status code 200, but got {resp.StatusCode}");

        var stringResult = await resp.Content.ReadAsStringAsync();
        var jsonResult = Deserialize<ApiResponseResult<PagedResultDto<MenuDto>>>(stringResult);

        return jsonResult.IsSuccess && jsonResult.Data != null
            ? jsonResult.Data.Items.Select(x => x.Id).ToList()
            : new List<long>();
    }

    private new T Deserialize<T>(string json)
    {
        var serializeOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Deserialize<T>(json, serializeOptions);
    }

    [Order(6)]
    [Test, Sequential]
    public async Task TestGetMenuTreeAsync()
    {
        var response = await Client.GetAsync("/api/menu/GetMenuTree");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var stringResult = await response.Content.ReadAsStringAsync();
        Assert.That(stringResult, Is.Not.Null);

        var result = Deserialize<ApiResponseResult<List<MenuDto>>>(stringResult);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
    }
}
