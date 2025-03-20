using MakeWish.UserService.Adapters.DataAccess.InMemory;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.Models;
using MakeWish.UserService.UseCases.Features.Users.Register;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using Moq;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Users.Register;

public class RegisterHandlerTests
{
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RegisterHandler _sut;

    public RegisterHandlerTests()
    {
        _passwordServiceMock = new Mock<IPasswordService>();
        _unitOfWork = new UnitOfWorkStub();
        _sut = new RegisterHandler(_unitOfWork, _passwordServiceMock.Object);
    }

    [Fact]
    public async Task Handle_UserWithEmailAlreadyExists_ReturnsEntityAlreadyExistsError()
    {
        // Arrange
        var existingUser = new UserBuilder().Build();

        _unitOfWork.Users.Add(existingUser);
        
        const string password = "password";
        const string name = "name";
        const string surname = "surname";
        
        var command = new RegisterCommand(existingUser.Email, password, name, surname);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityAlreadyExistsError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_SuccessfulRegistration_ReturnsUserDto()
    {
        // Arrange
        const string email = "test@example.com";
        const string password = "password123";
        const string name = "John";
        const string surname = "Doe";
        const string hashedPassword = "hashedPassword123";

        _passwordServiceMock.Setup(ps => ps.GetHash(password)).Returns(hashedPassword);

        var command = new RegisterCommand(email, password, name, surname);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(surname, result.Value.Surname);
    }
}
