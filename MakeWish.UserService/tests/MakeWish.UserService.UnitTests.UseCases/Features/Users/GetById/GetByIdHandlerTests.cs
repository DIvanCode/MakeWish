using MakeWish.UserService.Adapters.DataAccess.InMemory;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.Models;
using MakeWish.UserService.UseCases.Features.Users.GetById;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using Moq;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Users.GetById;

public class GetByIdHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetByIdHandler _sut;

    public GetByIdHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _sut = new GetByIdHandler(_unitOfWork, _userContextMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsAuthenticationError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new GetByIdCommand(userId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<AuthenticationError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsEntityNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        
        var command = new GetByIdCommand(userId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityNotFoundError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_UserFound_ReturnsUserDto()
    {
        // Arrange
        var user = new UserBuilder().Build();
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _unitOfWork.Users.Add(user);
        
        var command = new GetByIdCommand(user.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
        Assert.Equal(user.Email, result.Value.Email);
        Assert.Equal(user.Name, result.Value.Name);
        Assert.Equal(user.Surname, result.Value.Surname);
    }
}
