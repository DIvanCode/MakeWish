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

public sealed class UpdateWishListHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateWishListCommand, Result<WishListDto>>
{
    public async Task<Result<WishListDto>> Handle(UpdateWishListCommand request, CancellationToken cancellationToken)
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
        
        var wishList = await unitOfWork.WishLists.GetByIdWithoutWishesAsync(request.Id, cancellationToken);
        if (wishList is null)
        {
            return new EntityNotFoundError(nameof(WishList), nameof(WishList.Id), request.Id);
        }

        var updateResult = wishList.Update(request.Title, by: user);
        if (updateResult.IsFailed)
        {
            return updateResult;
        }
        
        unitOfWork.WishLists.Update(wishList);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return WishListDtoConverter.Convert(wishList, currUser: user, excludeWishes: true);
    }
}