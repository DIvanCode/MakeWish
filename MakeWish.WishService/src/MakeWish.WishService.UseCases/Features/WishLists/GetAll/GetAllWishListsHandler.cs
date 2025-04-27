using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.GetAll;

public sealed class GetAllWishListsHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllWishListsCommand, Result<List<WishListDto>>>
{
    public async Task<Result<List<WishListDto>>> Handle(GetAllWishListsCommand request, CancellationToken cancellationToken)
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

        var wishLists = await unitOfWork.WishLists.GetWishListsWithOwnerAsync(user, cancellationToken);

        return wishLists
            .Select(wishList => WishListDto.FromWishList(wishList, currUser: user))
            .ToList();
    }
}