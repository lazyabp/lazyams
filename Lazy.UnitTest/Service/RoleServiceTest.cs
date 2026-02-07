using AutoMapper;
using Lazy.Core.Caching;
using Lazy.Model.DBContext;
using Lazy.Model.Entity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Lazy.UnitTest.Service;

public class RoleServiceTest
{
    private readonly IRoleService _service;
    public RoleServiceTest(IRoleService service)
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
        var options = GetDbContextOptions("InMemoryRoleDB_Paged");
        using (var context = new LazyDBContext(options))
        {
            context.Roles.AddRange(new List<Role>
            {
                new Role { Id = 1, RoleName = "Admin1", Description = "full power"},
                new Role { Id = 2, RoleName = "Admin2", Description = "full power" },
                new Role { Id = 3, RoleName = "Teacher", Description = "limited power"}
            });
            context.SaveChanges();

            var filterInput = new RolePagedResultRequestDto
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
            Assert.That(result.Items[0].RoleName, Is.EqualTo("Admin1"));
            Assert.That(result.Items[1].RoleName, Is.EqualTo("Admin2"));
        }
    }

    [Test]
    public async Task CreateAsync_ShouldCreateRoleSuccessfully()
    {
        // Arrange
        var options = GetDbContextOptions("InMemoryRoleDB_Create");
        var createRoleDto = new CreateRoleDto
        {
            RoleName = "Admin",
            Description = "full power",

        };

        using (var context = new LazyDBContext(options))
        {
            // Act
            var result = await _service.CreateAsync(createRoleDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RoleName, Is.EqualTo(createRoleDto.RoleName));
            Assert.That(result.Description, Is.EqualTo(createRoleDto.Description));
        }
    }

    [Test]
    public async Task GetAsync_ShouldReturnRoleWhenExists()
    {
        // Arrange
        var options = GetDbContextOptions("InMemoryRoleDB_Get");
        using (var context = new LazyDBContext(options))
        {
            var role = new Role
            {
                Id = 1,
                RoleName = "Admin",
                Description = "full power",

            };
            context.Roles.Add(role);
            context.SaveChanges();

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RoleName, Is.EqualTo(role.RoleName));
            Assert.That(result.Description, Is.EqualTo(role.Description));
        }
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateRoleSuccessfully()
    {
        // Arrange
        var options = GetDbContextOptions("InMemoryRoleDB_Update");
        using (var context = new LazyDBContext(options))
        {
            var role = new Role
            {
                Id = 1,
                RoleName = "teacher",
                Description = "limited power",

            };
            context.Roles.Add(role);
            context.SaveChanges();

            var updateDto = new UpdateRoleDto
            {
                RoleName = "Admin",
                Description = "full power",

            };

            // Act
            var result = await _service.UpdateAsync(1, updateDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RoleName, Is.EqualTo(updateDto.RoleName));
            Assert.That(result.Description, Is.EqualTo(updateDto.Description));

        }
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteRoleSuccessfully()
    {
        // Arrange
        var options = GetDbContextOptions("InMemorRoleDB_Delete");
        using (var context = new LazyDBContext(options))
        {
            var role = new Role
            {
                Id = 1L,
                RoleName = "admin",
                Description = "Test Description",

            };
            context.Roles.Add(role);
            context.SaveChanges();

            // Act
            await _service.DeleteAsync(1L);

            // Assert
            var deletedRole = await context.Roles.FindAsync(1L);
            Assert.That(deletedRole, Is.Null);
        }
    }
}
