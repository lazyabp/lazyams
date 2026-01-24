using AutoMapper;
using Lazy.Model.DBContext;
using Lazy.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Lazy.UnitTest.Service;

public class MenuServiceTest
{
    private readonly IMapper _mapper;

    public MenuServiceTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Menu, MenuDto>();
            cfg.CreateMap<CreateMenuDto, Menu>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
            cfg.CreateMap<UpdateMenuDto, Menu>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }, null);
        _mapper = config.CreateMapper();
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
                new Menu { Id = 1, Title = "Menu1", Description = "Desc1", MenuType = MenuType.Dir, CreatedAt = DateTime.Now, CreatedBy = 1 },
                new Menu { Id = 2, Title = "Menu2", Description = "Desc2", MenuType = MenuType.Menu, CreatedAt = DateTime.Now, CreatedBy = 1  },
                new Menu { Id = 3, Title = "Menu3", Description = "Desc3", MenuType = MenuType.Btn, CreatedAt = DateTime.Now, CreatedBy = 1  }
            });
            context.SaveChanges();

            var service = new MenuService(context, _mapper);
            var filterInput = new MenuPagedResultRequestDto
            {
                PageIndex = 1,
                PageSize = 10 
            };

            // Act
            var result = await service.GetListAsync(filterInput);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Total, Is.EqualTo(3));
            Assert.That(result.Items.Count, Is.EqualTo(3)); 
            Assert.That(result.Items[0].Title, Is.EqualTo("Menu1")); 
            Assert.That(result.Items[1].Title, Is.EqualTo("Menu2")); 
        }
    }

    [Test]
    public async Task CreateAsync_ShouldCreateMenuSuccessfully()
    {
        // Arrange
        var options = GetDbContextOptions("InMemoryMenuDB_Create");
        var createMenuDto = new CreateMenuDto
        {
            Title = "Test Menu",
            Permission = "View",
            MenuType = MenuType.Dir,
            Description = "Test Description",
            OrderNum = 1,
            Route = "/test-menu",
            ComponentPath = "/components/test-menu",
            ParentId = null,
        };

        using (var context = new LazyDBContext(options))
        {
            var service = new MenuService(context, _mapper);

            // Act
            var result = await service.CreateAsync(createMenuDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(createMenuDto.Title));
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
                Title = "Test Menu",
                Description = "Test Description",
                MenuType = MenuType.Menu,
                CreatedAt = DateTime.Now,
                CreatedBy = 1
            };
            context.Menus.Add(menu);
            context.SaveChanges();

            var service = new MenuService(context, _mapper);

            // Act
            var result = await service.GetAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(menu.Title));
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
                Title = "Old Menu",
                Description = "Old Description",
                MenuType = MenuType.Btn,
                CreatedAt = DateTime.Now,
                CreatedBy = 1,
                UpdatedAt = DateTime.Now,
                UpdatedBy = 1
            };
            context.Menus.Add(menu);
            context.SaveChanges();

            var service = new MenuService(context, _mapper);
            var updateDto = new UpdateMenuDto
            {
                Title = "Updated Menu",
                Description = "Updated Description",
                MenuType = MenuType.Dir,
                OrderNum = 2
            };

            // Act
            var result = await service.UpdateAsync(1L, updateDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(updateDto.Title));
            Assert.That(result.Description, Is.EqualTo(updateDto.Description));
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
                Title = "Test Menu",
                Description = "Test Description",
                MenuType = MenuType.Menu,
                CreatedAt = DateTime.Now,
                CreatedBy = 1
            };
            context.Menus.Add(menu);
            context.SaveChanges();

            var service = new MenuService(context, _mapper);

            // Act
            await service.DeleteAsync(1);

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
        new Menu { Id = 1, Title = "Root Menu", ParentId = null, CreatedAt = DateTime.Now, CreatedBy = 1  },
        new Menu { Id = 2, Title = "Child Menu 1", ParentId = 1, CreatedAt = DateTime.Now, CreatedBy = 1  },
        new Menu { Id = 3, Title = "Child Menu 2", ParentId = 1, CreatedAt = DateTime.Now, CreatedBy = 1  },
        new Menu { Id = 4, Title = "Sub Child Menu", ParentId = 2, CreatedAt = DateTime.Now, CreatedBy = 1  }
    });
            context.SaveChanges();

            var service = new MenuService(context, _mapper);

            var result = await service.GetMenuTreeAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Root Menu"));

            var children = result[0].Children.ToList();
            Assert.That(children.Count, Is.EqualTo(2));
            Assert.That(children[0].Title, Is.EqualTo("Child Menu 1"));
            Assert.That(children[1].Title, Is.EqualTo("Child Menu 2"));

            var subChildren = children[0].Children.ToList();
            Assert.That(subChildren.Count, Is.EqualTo(1));
            Assert.That(subChildren[0].Title, Is.EqualTo("Sub Child Menu"));
        }
    }
}
