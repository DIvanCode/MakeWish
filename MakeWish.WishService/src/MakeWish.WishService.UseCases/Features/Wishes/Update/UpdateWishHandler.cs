using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Update;

public sealed class UpdateWishHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateWishCommand, Result<WishDto>>
{
    public async Task<Result<WishDto>> Handle(UpdateWishCommand request, CancellationToken cancellationToken)
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
        
        var wish = await unitOfWork.Wishes.GetByIdAsync(request.Id, cancellationToken);
        if (wish is null)
        {
            return new EntityNotFoundError(nameof(Wish), nameof(Wish.Id), request.Id);
        }

        var updateResult = wish.Update(request.Title, request.Description, request.IsPublic, by: user);
        if (updateResult.IsFailed)
        {
            return updateResult;
        }
        
        unitOfWork.Wishes.Update(wish);
        
        var publicWishList = await unitOfWork.WishLists.GetByIdWithoutWishesAsync(
            user.PublicWishListId,
            cancellationToken);
        var privateWishList = await unitOfWork.WishLists.GetByIdWithoutWishesAsync(
            user.PrivateWishListId,
            cancellationToken);

        publicWishList!.Remove(wish, by: user);
        unitOfWork.WishLists.RemoveWish(publicWishList, wish);
        
        privateWishList!.Remove(wish, by: user);
        unitOfWork.WishLists.RemoveWish(privateWishList, wish);

        if (request.IsPublic)
        {
            publicWishList.Add(wish, by: user);
            unitOfWork.WishLists.AddWish(publicWishList, wish);
        }
        else
        {
            privateWishList.Add(wish, by: user);
            unitOfWork.WishLists.AddWish(privateWishList, wish);   
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return WishDto.FromWish(wish, currUser: user);
    }
}