using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Abstractions.Dto;
using MakeWish.WishService.UseCases.Abstractions.Features.WishLists;
using MakeWish.WishService.UseCases.Abstractions.Services;
using MakeWish.WishService.UseCases.Converters;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists;

public sealed class GetUserWishListsHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserWishListsCommand, Result<List<WishListDto>>>
{
    public async Task<Result<List<WishListDto>>> Handle(GetUserWishListsCommand request, CancellationToken cancellationToken)
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

        var wishLists = owner.Id == user.Id
            ? await unitOfWork.WishLists.GetWithOwnerAsync(owner, cancellationToken)
            : await unitOfWork.WishLists.GetWithOwnerAndUserAccessAsync(owner, user, cancellationToken);

        return wishLists
            .Where(wishList => wishList.Id != owner.PublicWishListId && wishList.Id != owner.PrivateWishListId)
            .Select(wishList => WishListDtoConverter.Convert(wishList, currUser: user))
            .ToList();
    }
}