using AutoMapper;
using Lazy.Core.Caching;
using Lazy.Model.DBContext;
using Lazy.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Lazy.UnitTest.Service;

public class MenuServiceTest
{
    private readonly IMenuService _service;

    public MenuServiceTest(IMenuService service)
    {
        _service = service;
    }

    private DbContextOptions<LazyDBContext> GetDbContextOptions(string dbName)
    {
        return new DbContextOptionsBuilder<LazyDBContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    [Test]
    public async Task GetListAsync_ShouldReturnPagedResultWithTotalCount()
    {
        // Arrange
        var options = GetDbContextOptions("InMemoryMenuDB_Paged");

        using (var context = new LazyDBContext(options))
        {
            context.Menus.AddRange(new List<Menu>
            {
                new Menu { Id = 1, Name = "Menu1", Title = "Title1", MenuType = MenuType.Dir, CreatedAt = DateTime.Now, CreatedBy = 1 },
                new Menu { Id = 2, Name = "Menu2", Title = "Title2", MenuType = MenuType.Menu, CreatedAt = DateTime.Now, CreatedBy = 1  },
                new Menu { Id = 3, Name = "Menu3", Title = "Title3", MenuType = MenuType.Btn, CreatedAt = DateTime.Now, CreatedBy = 1  }
            });
            context.SaveChanges();

            var filterInput = new MenuPagedResultRequestDto
            {
                PageIndex = 1,
                PageSize = 10 
            };

            // Act
            var result = await _service.GetListAsync(filterInput);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Total, Is.EqualTo(3));
            Assert.That(result.Items.Count, Is.EqualTo(3)); 
            Assert.That(result.Items[0].Name, Is.EqualTo("Menu1")); 
            Assert.That(result.Items[1].Name, Is.EqualTo("Menu2")); 
        }
    }

    [Test]
    public async Task CreateAsync_ShouldCreateMenuSuccessfully()
    {
        // Arrange
        var options = GetDbContextOptions("InMemoryMenuDB_Create");
        var createMenuDto = new CreateMenuDto
        {
            Name = "Test Menu",
            Permission = "View",
            MenuType = MenuType.Dir,
            Title = "Test Title",
            OrderNum = 1,
            Route = "/test-menu",
            Component = "/components/test-menu",
            ParentId = null,
        };

        using (var context = new LazyDBContext(options))
        {
            // Act
            var result = await _service.CreateAsync(createMenuDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(createMenuDto.Name));
            Assert.That(result.MenuType, Is.EqualTo(createMenuDto.MenuType));
        }
    }

    [Test]
    public async Task GetAsync_ShouldReturnMenuWhenExists()
    {
        // Arrange
        var options = GetDbContextOptions("InMemoryMenuDB_Get");
        using (var context = new LazyDBContext(options))
        {
            var menu = new Menu
            {
                Id = 1,
                Name = "Test Menu",
                Title = "Test Title",
                MenuType = MenuType.Menu,
                CreatedAt = DateTime.Now,
                CreatedBy = 1
            };
            context.Menus.Add(menu);
            context.SaveChanges();

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(menu.Name));
            Assert.That(result.MenuType, Is.EqualTo(menu.MenuType));
        }
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateMenuSuccessfully()
    {
        // Arrange
        var options = GetDbContextOptions("InMemoryMenuDB_Update");
        using (var context = new LazyDBContext(options))
        {
            var menu = new Menu
            {
                Id = 1L, 
                Name = "Old Menu",
                Title = "Old Title",
                MenuType = MenuType.Btn,
                CreatedAt = DateTime.Now,
                CreatedBy = 1,
                UpdatedAt = DateTime.Now,
                UpdatedBy = 1
            };
            context.Menus.Add(menu);
            context.SaveChanges();

            var updateDto = new UpdateMenuDto
            {
                Name = "Updated Menu",
                Title = "Updated Title",
                MenuType = MenuType.Dir,
                OrderNum = 2
            };

            // Act
            var result = await _service.UpdateAsync(1L, updateDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(updateDto.Name));
            Assert.That(result.Title, Is.EqualTo(updateDto.Title));
            Assert.That(result.MenuType, Is.EqualTo(updateDto.MenuType));
        }
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteMenuSuccessfully()
    {
        // Arrange
        var options = GetDbContextOptions("InMemoryMenuDB_Delete");
        using (var context = new LazyDBContext(options))
        {
            var menu = new Menu
            {
                Id = 1L,
                Name = "Test Menu",
                Title = "Test Title",
                MenuType = MenuType.Menu,
                CreatedAt = DateTime.Now,
                CreatedBy = 1
            };
            context.Menus.Add(menu);
            context.SaveChanges();

            // Act
            await _service.DeleteAsync(1);

            // Assert
            var deletedMenu = await context.Menus.FindAsync(1L);
            Assert.That(deletedMenu, Is.Null);
        }
    }

    [Test]
    public async Task GetMenuTreeAsync_ShouldReturnCorrectTreeStructure()
    {
        var options = GetDbContextOptions("InMemoryMenuDB_Tree");
        using (var context = new LazyDBContext(options))
        {
            context.Menus.AddRange(new List<Menu>
    {
        new Menu { Id = 1, Name = "Root Menu", ParentId = null, CreatedAt = DateTime.Now, CreatedBy = 1  },
        new Menu { Id = 2, Name = "Child Menu 1", ParentId = 1, CreatedAt = DateTime.Now, CreatedBy = 1  },
        new Menu { Id = 3, Name = "Child Menu 2", ParentId = 1, CreatedAt = DateTime.Now, CreatedBy = 1  },
        new Menu { Id = 4, Name = "Sub Child Menu", ParentId = 2, CreatedAt = DateTime.Now, CreatedBy = 1  }
    });
            context.SaveChanges();

            var result = await _service.GetMenuTreeAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Root Menu"));

            var children = result[0].Children.ToList();
            Assert.That(children.Count, Is.EqualTo(2));
            Assert.That(children[0].Name, Is.EqualTo("Child Menu 1"));
            Assert.That(children[1].Name, Is.EqualTo("Child Menu 2"));

            var subChildren = children[0].Children.ToList();
            Assert.That(subChildren.Count, Is.EqualTo(1));
            Assert.That(subChildren[0].Name, Is.EqualTo("Sub Child Menu"));
        }
    }
}
