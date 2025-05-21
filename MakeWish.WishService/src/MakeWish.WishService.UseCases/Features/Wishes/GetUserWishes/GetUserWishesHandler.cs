using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.GetUserWishes;

public sealed class GetUserWishesHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserWishesCommand, Result<IReadOnlyList<WishDto>>>
{
    public async Task<Result<IReadOnlyList<WishDto>>> Handle(GetUserWishesCommand request, CancellationToken cancellationToken)
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

        var owner = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (owner is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.UserId);
        }

        var wishes = new List<Wish>();
        if (user.Id == owner.Id)
        {
            wishes = await unitOfWork.Wishes.GetWithOwnerAsync(owner, cancellationToken);
        }
        else
        {
            var publicWishList = await unitOfWork.WishLists.GetByIdWithoutWishesAsync(owner.PublicWishListId, cancellationToken);
            if (await unitOfWork.WishLists.HasUserAccessAsync(publicWishList!, user, cancellationToken))
            {
                publicWishList = await unitOfWork.WishLists.GetByIdAsync(owner.PublicWishListId, cancellationToken); 
                wishes = publicWishList!.Wishes.ToList();
            }
        }

        if (request.Query is not null)
        {
            wishes = wishes.
                Where(wish => wish.Title.Contains(request.Query, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }
        
        return wishes.Select(wish => WishDto.FromWish(wish, currUser: user)).ToList();
    }
}