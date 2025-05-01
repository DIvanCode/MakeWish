using MakeWish.UserService.Adapters.DataAccess.InMemory;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.UnitTests.Common.Models;
using MakeWish.UserService.UseCases.Features.Users.GetAll;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Users.GetAll;

public class GetAllUsersHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetAllUsersHandler _sut;

    public GetAllUsersHandlerTests()
    {
        _unitOfWork = new UnitOfWorkStub();
        _sut = new GetAllUsersHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_SuccessfulUsersRetrieval_ReturnsUserDtos()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();

        _unitOfWork.Users.Add(firstUser);
        _unitOfWork.Users.Add(secondUser);

        var command = new GetAllUsersCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Count, 2);
        
        var userDto1 = result.Value.First();
        Assert.Equivalent(userDto1, firstUser);
        var userDto2 = result.Value.Last();
        Assert.Equivalent(userDto2, secondUser);
    }
}
