using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.GetPromisedWishes;

public sealed class GetPromisedWishesHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetPromisedWishesCommand, Result<List<WishDto>>>
{
    public async Task<Result<List<WishDto>>> Handle(
        GetPromisedWishesCommand request,
        CancellationToken cancellationToken)
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

        var wishes = await unitOfWork.Wishes.GetWithPromiserAsync(user, cancellationToken);
        
        return wishes.Select(wish => new WishDto(
            wish.Id,
            wish.Title,
            wish.Description,
            Status: wish.GetStatusFor(user),
            wish.Owner.Id,
            wish.GetPromiserFor(user)?.Id,
            wish.GetCompleter()?.Id)).ToList();
    }
}