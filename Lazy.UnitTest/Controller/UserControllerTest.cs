using Moq;

namespace Lazy.UnitTest.Controller;

public class UserControllerTest
{
    private readonly UserController _controller;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IRoleService> _roleServiceMock;
    private readonly Mock<IMenuService> _menuServiceMock;

    public UserControllerTest()
    {
        _userServiceMock = new Mock<IUserService>();
        _roleServiceMock = new Mock<IRoleService>();
        _menuServiceMock = new Mock<IMenuService>();

        _controller = new UserController(_userServiceMock.Object, _roleServiceMock.Object, _menuServiceMock.Object);
    }

    [Test]
    public async Task GetByPageAsync_WhenPageUserexit_ShouldReturnPageUsers()
    {
        // Arrange
        var input = new UserPagedResultRequestDto();
        var users = new List<UserDto>
        {
            new UserDto { Id = 1, UserName = "A1", Password="123", Age= 1, Email="abc@uow.edu.au", Gender=Gender.Male, Avatar="123" },
            new UserDto { Id = 2, UserName = "A2", Password="123", Age= 1, Email="abc@uow.edu.au", Gender=Gender.Male, Avatar="123" },
            new UserDto { Id = 3, UserName = "A3", Password="123", Age= 1, Email="abc@uow.edu.au", Gender=Gender.Male, Avatar="123" },
            new UserDto { Id = 4, UserName = "A4", Password="123", Age= 1, Email="abc@uow.edu.au", Gender=Gender.Male, Avatar="123" },
            new UserDto { Id = 5, UserName = "A5", Password="123", Age= 1, Email="abc@uow.edu.au", Gender=Gender.Male, Avatar="123" },
        };
        var pagedResult = new PagedResultDto<UserDto>
        {
            Items = users,
            Total = 5,
        };
        _userServiceMock.Setup(service => service.GetListAsync(input)).ReturnsAsync(pagedResult);
        // Act
        var result = await _controller.GetByPageAsync(input);
        // Assert
        Assert.That(pagedResult, Is.EqualTo(result));
        Assert.That(pagedResult.Total, Is.EqualTo(result.Total));
        _userServiceMock.Verify(s => s.GetListAsync(input), Times.Once);
    }

    [Test]
    public async Task Add_WhenUserIsAdded_ShouldReturnTrue()
    {
        // Arrange
        var input = new CreateUserDto()
        {
            Id = 1,
            UserName = "A5",
            Password = "123",
            Age = 1,
            Email = "abc@uow.edu.au",
            Gender = Gender.Male,
            Avatar = "123",
            RoleIds = new List<long>() { 1, 2, 3 }
        };
        var useDto = new UserDto()
        {
            Id = 1,
            UserName = "A5",
            Password = "123",
            Age = 1,
            Email = "abc@uow.edu.au",
            Gender = Gender.Male,
            Avatar = "123",
        };
        _userServiceMock.Setup(s => s.CreateAsync(input)).Returns(Task.FromResult(useDto));
        // Act
        var userDtoadded = await _userServiceMock.Object.CreateAsync(input);
        var result = await _controller.Add(input);
        // Assert
        Assert.That(useDto.Age, Is.EqualTo(userDtoadded.Age));
        Assert.That(useDto.UserName, Is.EqualTo(userDtoadded.UserName));
        Assert.That(useDto.Avatar, Is.EqualTo(userDtoadded.Avatar));
        Assert.That(useDto.Email, Is.EqualTo(userDtoadded.Email));
        Assert.That(useDto.Gender, Is.EqualTo(userDtoadded.Gender));
        Assert.That(useDto.Avatar, Is.EqualTo(userDtoadded.Avatar));
        Assert.That(result, Is.True);
        _userServiceMock.Verify(s => s.CreateAsync(input), Times.AtLeastOnce);
    }
    [Test]
    public async Task Add_WhenUserIsAdded_ShouldReturnFalse()
    {
        //bad case
        //Arrange
        var input = new CreateUserDto()
        {
            UserName = "A5",
            Password = "123",
            Age = 1,
            Email = "abc@uow.edu.au",
            Avatar = "123",
            RoleIds = new List<long>() { 1, 2, 3 }
        };
        var inputIncomplete = new UserDto()
        {
            UserName = "A5",
            Password = "123",
            Age = 1,
            Email = "abc@uow.edu.au",
            Avatar = "123",
        };
        _userServiceMock.Setup(s => s.CreateAsync(input)).Returns(Task.FromResult(inputIncomplete));

        //Act
        _controller.ModelState.AddModelError("Gender", "Gender is a requried field");
        var badResponse = await _userServiceMock.Object.CreateAsync(input);
        var result = await _controller.Add(input);
        //Assert
        Assert.That(result, Is.False);
        _userServiceMock.Verify(s => s.CreateAsync(input), Times.AtLeastOnce);
    }
    [Test]
    public async Task Update_WhenUserIsUpdated_ShouldReturnTrue()
    {
        //arrange
        var input = new UpdateUserDto()
        {
            Id = 1,
            UserName = "A5",

            Age = 1,
            Email = "abc@uow.edu.au",
            Gender = Gender.Male,

            RoleIds = new List<long>() { 1, 2, 3 }
        };
        var output = new UserDto()
        {
            Id = 1,
            UserName = "A5",
            Password = "123",
            Age = 1,
            Email = "abc@uow.edu.au",
            Gender = Gender.Male,
            Avatar = "123",
        };
        //act
        _userServiceMock.Setup(s => s.UpdateAsync(input.Id, input)).Returns(Task.FromResult(output));
        var updatedUser = await _userServiceMock.Object.UpdateAsync(input.Id, input);
        //Assert
        Assert.That(output.Age, Is.EqualTo(updatedUser.Age));
        Assert.That(output.UserName, Is.EqualTo(updatedUser.UserName));
        Assert.That(output.Avatar, Is.EqualTo(updatedUser.Avatar));
        Assert.That(output.Email, Is.EqualTo(updatedUser.Email));
        Assert.That(output.Gender, Is.EqualTo(updatedUser.Gender));
        Assert.That(output.Avatar, Is.EqualTo(updatedUser.Avatar));
        _userServiceMock.Verify(s => s.UpdateAsync(input.Id, input), Times.Once);
    }
    [Test]
    public async Task Update_WhenUserIsUpdated_ShouldReturnFalse()
    {
        //arrange
        var input = new UpdateUserDto()
        {
            Id = 1,
            UserName = "A5",

            Age = 1,
            Email = "abc@uow.edu.au",
            Gender = Gender.Male,

            RoleIds = new List<long>() { 1, 2, 3 }
        };
        var output = new UserDto()
        {
            Id = 1,
            UserName = "A5",
            Password = "",
            Age = 1,
            Email = "abc@uow.edu.au",
            Gender = Gender.Male,
            Avatar = "123",
        };
        _userServiceMock.Setup(s => s.UpdateAsync(input.Id, input)).Returns(Task.FromResult(output));
        _controller.ModelState.AddModelError("Password", "Password is null ");
        //act
        var updatedUser = await _userServiceMock.Object.UpdateAsync(input.Id, input);
        //Assert
        Assert.That(updatedUser.Password == "");
        _userServiceMock.Verify(s => s.UpdateAsync(input.Id, input), Times.Once);
    }
    [Test]
    [TestCase(3, 7)]
    public async Task Delete_WhenUserIsDeleted_ShouldReturnTrue(int id, int id2)
    {
        //valid Id case
        //Arrange
        var validId = id;
        var input = new UserPagedResultRequestDto();
        var usersBeforeDelete = new List<UserDto>
        {
            new UserDto { Id = 1, UserName = "A1", Password="123", Age= 1, Email="abc@uow.edu.au", Gender=Gender.Male, Avatar="123" },
            new UserDto { Id = 2, UserName = "A2", Password="123", Age= 1, Email="abc@uow.edu.au", Gender=Gender.Male, Avatar="123" },
            new UserDto { Id = 3, UserName = "A3", Password="123", Age= 1, Email="abc@uow.edu.au", Gender=Gender.Male, Avatar="123" },
            new UserDto { Id = 4, UserName = "A4", Password="123", Age= 1, Email="abc@uow.edu.au", Gender=Gender.Male, Avatar="123" },
            new UserDto { Id = 5, UserName = "A5", Password="123", Age= 1, Email="abc@uow.edu.au", Gender=Gender.Male, Avatar="123" },
        };
        var usersAfterDeleteValid = from listItem in usersBeforeDelete where id != validId select listItem;
        var pagedResult = new PagedResultDto<UserDto>
        {
            Items = usersAfterDeleteValid as List<UserDto>,
            Total = 4,
        };
        _userServiceMock.Setup(service => service.DeleteAsync(validId)).Returns(Task.FromResult(pagedResult));
        _userServiceMock.Setup(service => service.GetListAsync(input)).ReturnsAsync(pagedResult);//Act
        var ValidResult = _controller.Delete(validId);
        var listCountResult = await _controller.GetByPageAsync(input);
        //Assert
        Assert.That(listCountResult.Total, Is.EqualTo(4));
        Assert.That(ValidResult.Result, Is.True);
        _userServiceMock.Verify(s => s.DeleteAsync(validId), Times.Once);
        //invalid Id case
        //arrange
        var invalidId = id2;
        var input2 = new UserPagedResultRequestDto();
        var usersAfterDeleteInvalid = from listItem in usersBeforeDelete where id != invalidId select listItem;
        var pagedResultInvalid = new PagedResultDto<UserDto>
        {
            Items = usersAfterDeleteValid as List<UserDto>,
            Total = 5,
        };
        _userServiceMock.Setup(service => service.DeleteAsync(invalidId)).Returns(Task.FromResult(pagedResultInvalid));
        _userServiceMock.Setup(service => service.GetListAsync(input2)).ReturnsAsync(pagedResultInvalid);
        //Act
        var InvalidResult = _controller.Delete(validId);
        var listCountResult2 = await _controller.GetByPageAsync(input2);
        //Assert
        Assert.That(listCountResult2.Total, Is.EqualTo(5));
        _userServiceMock.Verify(s => s.DeleteAsync(validId), Times.AtLeastOnce);
    }

    [Test]
    public async Task GetByUserName_ShouldReturnUserInfoWhenValidUserName()
    {
        // Arrange
        var user = new UserDto()
        {
            Id = 999,
            UserName = "Apple",
            Password = "",
            Age = 1,
            Email = "abc@uow.edu.au",
            Gender = Gender.Male,
            Avatar = "123",
        };

        _userServiceMock.Setup(service => service.GetByUserNameAsync("Apple")).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById(user.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserName, Is.EqualTo("Apple"));
        Assert.That(result.Email, Is.EqualTo("abc@uow.edu.au"));
        _userServiceMock.Verify(service => service.GetByUserNameAsync("Apple"), Times.Once);
    }

}
