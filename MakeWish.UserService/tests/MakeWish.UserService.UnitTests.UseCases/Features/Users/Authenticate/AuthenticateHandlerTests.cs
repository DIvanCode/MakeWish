using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.DataAccess;
using MakeWish.UserService.UseCases.Features.Users.Authenticate;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using Moq;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Users.Authenticate;
    
public class AuthenticateHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IAuthTokenProvider> _authTokenProviderMock;
    private readonly AuthenticateHandler _sut;

    public AuthenticateHandlerTests()
    {
        _unitOfWork = new UnitOfWorkStub();
        _passwordServiceMock = new Mock<IPasswordService>();
        _authTokenProviderMock = new Mock<IAuthTokenProvider>();
        _sut = new AuthenticateHandler(_unitOfWork, _passwordServiceMock.Object, _authTokenProviderMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsEntityNotFoundError()
    {
        // Arrange
        const string email = "test@example.com";
        const string password = "password123";

        var command = new AuthenticateCommand(email, password);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityNotFoundError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_IncorrectPassword_ReturnsAuthenticationError()
    {
        // Arrange
        var user = new UserBuilder().Build();
        
        const string password = "incorrect";
        
        _unitOfWork.Users.Add(user);
        _passwordServiceMock.Setup(ps => ps.Verify(password, user.PasswordHash)).Returns(false);

        var command = new AuthenticateCommand(user.Email, password);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<AuthenticationError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_SuccessfulAuthentication_ReturnsToken()
    {
        // Arrange
        var user = new UserBuilder().Build();
        
        const string password = "correct";
        const string expectedToken = "generatedToken";
        
        _unitOfWork.Users.Add(user);
        _passwordServiceMock.Setup(ps => ps.Verify(password, user.PasswordHash)).Returns(true);
        _authTokenProviderMock.Setup(atp => atp.GenerateToken(user)).Returns(expectedToken);

        var command = new AuthenticateCommand(user.Email, password);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedToken, result.Value);
    }
}
