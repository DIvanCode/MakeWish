using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.GetAll;

public sealed class GetAllUsersHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllUsersCommand, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
    {
        var users = await unitOfWork.Users.GetAllAsync(cancellationToken);
        return users.Select(UserDto.FromUser).ToList();
    }
}