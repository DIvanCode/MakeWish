using EnsureThat;
using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models.Entities;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.UserService.UseCases.Features.Users.GetAll;

public sealed class GetAllUsersHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<GetAllUsersCommand, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        if (request.OnlyFriends ?? false)
        {
            var user = await unitOfWork.Users.GetByIdAsync(userContext.UserId, cancellationToken);
            if (user is null)
            {
                return new EntityNotFoundError(nameof(User), nameof(User.Id), userContext.UserId);
            }

            var users = (await unitOfWork.Friendships.GetConfirmedForUserAsync(user, cancellationToken))
                .Select(f => f.FirstUser.Id == user.Id ? f.SecondUser : f.FirstUser);

            if (request.Query is not null)
            {
                users = users.Where(u =>
                    $"{u.Name} {u.Surname}" == request.Query || $"{u.Surname} {u.Name}" == request.Query);
            }

            return users.Select(UserDto.FromUser).ToList();
        }
        else
        {
            List<User> users;
            if (request.Query is not null)
            {
                users = await unitOfWork.Users.GetBySearchQueryAsync(request.Query, cancellationToken);
            }
            else
            {
                if (!userContext.IsAdmin)
                {
                    return new ForbiddenError("Cannot get all users");
                }
                
                users = await unitOfWork.Users.GetAllAsync(cancellationToken);
            }
            
            return users.Select(UserDto.FromUser).ToList();
        }
    }
}