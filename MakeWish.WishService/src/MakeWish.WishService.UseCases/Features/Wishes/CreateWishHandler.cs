using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Abstractions.Dto;
using MakeWish.WishService.UseCases.Abstractions.Features.Wishes;
using MakeWish.WishService.UseCases.Abstractions.Services;
using MakeWish.WishService.UseCases.Converters;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes;

public sealed class CreateWishHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateWishCommand, Result<WishDto>>
{
    public async Task<Result<WishDto>> Handle(CreateWishCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }

        var owner = await unitOfWork.Users.GetByIdAsync(userContext.UserId, cancellationToken);
        if (owner is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), userContext.UserId);
        }
        
        var wish = Wish.Create(request.Title, request.Description, owner, request.IsPublic);
        unitOfWork.Wishes.Add(wish);

        var wishList = request.IsPublic
            ? await unitOfWork.WishLists.GetByIdWithoutWishesAsync(owner.PublicWishListId, cancellationToken)
            : await unitOfWork.WishLists.GetByIdWithoutWishesAsync(owner.PrivateWishListId, cancellationToken);
        var addResult = wishList!.Add(wish, by: owner);
        if (addResult.IsFailed)
        {
            return addResult;
        }
        
        unitOfWork.WishLists.AddWish(wishList, wish);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return WishDtoConverter.Convert(wish, currUser: owner);
    }
}