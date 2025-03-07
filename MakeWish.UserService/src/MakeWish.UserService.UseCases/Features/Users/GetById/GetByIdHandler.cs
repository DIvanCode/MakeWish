using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.GetById;

public sealed class GetByIdHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<GetByIdCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetByIdCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        var user = await unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.Id);
        }

        return new UserDto(user.Id, user.Email, user.Name, user.Surname);
    }
}