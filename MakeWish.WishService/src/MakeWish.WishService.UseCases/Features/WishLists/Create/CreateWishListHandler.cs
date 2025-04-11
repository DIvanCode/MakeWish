using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.Create;

public sealed class CreateWishListHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateWishListCommand, Result<WishListDto>>
{
    public async Task<Result<WishListDto>> Handle(CreateWishListCommand request, CancellationToken cancellationToken)
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

        var wishList = WishList.Create(request.Title, owner);
        
        unitOfWork.WishLists.Add(wishList);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return WishListDto.FromWishList(wishList, currUser: owner);
    }
}