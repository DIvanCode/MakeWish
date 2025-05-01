using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.Models.Entities;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.GetCurrent;

public sealed class GetCurrentHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<GetCurrentCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetCurrentCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        var user = await unitOfWork.Users.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), userContext.UserId);
        }

        return UserDto.FromUser(user);
    }
}