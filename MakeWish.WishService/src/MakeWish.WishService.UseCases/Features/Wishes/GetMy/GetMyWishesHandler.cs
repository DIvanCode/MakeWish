using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.GetMy;

public sealed class GetMyWishesHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetMyWishesCommand, Result<IReadOnlyList<WishDto>>>
{
    public async Task<Result<IReadOnlyList<WishDto>>> Handle(GetMyWishesCommand request, CancellationToken cancellationToken)
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

        var wishes = await unitOfWork.Wishes.GetWithOwnerAsync(user, cancellationToken);
        
        return wishes.Select(wish => WishDto.FromWish(wish, currUser: user)).ToList();
    }
}