using Moq;

namespace Lazy.UnitTest.Controller;

[TestFixture]
public class MenuControllerTest
{
    private MenuController _controller;
    private Mock<IMenuService> _menuServiceMock;

    [SetUp]
    public void SetUp()
    {
        _menuServiceMock = new Mock<IMenuService>();
        _controller = new MenuController(_menuServiceMock.Object);
    }

    [Test]
    public async Task GetByPageAsync_ShouldReturnPagedMenus()
    {
        // Arrange
        var input = new FilterPagedResultRequestDto();
        var menus = new List<MenuDto>
        {
            new MenuDto { Id = 1, Title = "Menu1", Description = "Description1", MenuType = MenuType.Dir },
            new MenuDto { Id = 2, Title = "Menu2", Description = "Description2", MenuType = MenuType.Menu }
        };
        var pagedResult = new PagedResultDto<MenuDto>(menus.Count, menus);

        _menuServiceMock.Setup(service => service.GetListAsync(input)).ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetByPageAsync(input);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Total, Is.EqualTo(2));
        Assert.That(result.Items.Count, Is.EqualTo(2));
        _menuServiceMock.Verify(service => service.GetListAsync(input), Times.Once);
    }

    [Test]
    public async Task Add_ShouldReturnTrueWhenMenuAddedSuccessfully()
    {
        // Arrange
        var input = new CreateMenuDto
        {
            Title = "New Menu",
            Description = "Description for new menu",
            MenuType = MenuType.Menu
        };
        var createdMenu = new MenuDto { Id = 1, Title = "New Menu" };

        _menuServiceMock.Setup(service => service.CreateAsync(input)).ReturnsAsync(createdMenu);

        // Act
        var result = await _controller.Add(input);

        // Assert
        Assert.That(result, Is.True);
        _menuServiceMock.Verify(service => service.CreateAsync(input), Times.Once);
    }

    [Test]
    public async Task Update_ShouldReturnTrueWhenMenuUpdatedSuccessfully()
    {
        // Arrange
        var input = new UpdateMenuDto
        {
            Id = 1,
            Title = "Updated Menu",
            Description = "Updated Description",
            MenuType = MenuType.Dir
        };

        var menu = new MenuDto
        {
            Id = 1,
            Title = "Updated Menu",
            Description = "Updated Description",
            MenuType = MenuType.Menu
        };
        _menuServiceMock.Setup(service => service.GetAsync(input.Id))
    .ReturnsAsync(menu);

        _menuServiceMock.Setup(service => service.UpdateAsync(It.IsAny<long>(), It.IsAny<UpdateMenuDto>()))
            .Returns(Task.FromResult(menu));

        // Act
        var result = await _controller.Update(input);

        // Assert
        Assert.That(result, Is.True);
        _menuServiceMock.Verify(service => service.UpdateAsync(input.Id, input), Times.Once);
    }

    [Test]
    public async Task Delete_ShouldReturnTrueWhenMenuDeletedSuccessfully()
    {
        // Arrange
        var menuId = 1L;
        var existingMenu = new MenuDto
        {
            Id = menuId,
            Title = "Sample Menu",
            Description = "Sample Description",
            MenuType = MenuType.Menu
        };

        _menuServiceMock.Setup(service => service.GetAsync(menuId))
            .ReturnsAsync(existingMenu);

        _menuServiceMock.Setup(service => service.DeleteAsync(menuId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(menuId);

        // Assert
        Assert.That(result, Is.True);
        _menuServiceMock.Verify(service => service.DeleteAsync(menuId), Times.Once);
    }

    [Test]
    public async Task GetById_ShouldReturnMenuWhenExists()
    {
        // Arrange
        var menuId = 1L;
        var menu = new MenuDto
        {
            Id = menuId,
            Title = "Menu1",
            Description = "Description1",
            MenuType = MenuType.Menu
        };

        _menuServiceMock.Setup(service => service.GetAsync(menuId)).ReturnsAsync(menu);

        // Act
        var result = await _controller.GetById(menuId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(menuId));
        Assert.That(result.Title, Is.EqualTo("Menu1"));
        _menuServiceMock.Verify(service => service.GetAsync(menuId), Times.Once);
    }

    [Test]
    public async Task GetMenuTreeAsync_ShouldReturnCorrectTreeStructure()
    {
        var mockTree = new List<MenuDto>
{
    new MenuDto
    {
        Id = 1,
        Title = "Root Menu",
        ParentId = null,
        Children = new List<MenuDto>
        {
            new MenuDto { Id = 2, Title = "Child Menu 1", ParentId = 1 },
            new MenuDto { Id = 3, Title = "Child Menu 2", ParentId = 1 }
        }
    }
};

        _menuServiceMock.Setup(service => service.GetMenuTreeAsync())
            .ReturnsAsync(mockTree);

        var result = await _controller.GetMenuTree();

        Assert.That(result, Is.InstanceOf<List<MenuDto>>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Title, Is.EqualTo("Root Menu"));

        var children = result[0].Children.ToList();
        Assert.That(children.Count, Is.EqualTo(2));
        Assert.That(children[0].Title, Is.EqualTo("Child Menu 1"));
        Assert.That(children[1].Title, Is.EqualTo("Child Menu 2"));
    }


}
