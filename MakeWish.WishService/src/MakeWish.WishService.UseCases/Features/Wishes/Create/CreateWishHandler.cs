using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Create;

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
        
        var wish = Wish.Create(request.Title, request.Description, owner);
        
        var mainWishList = await unitOfWork.WishLists.GetMainForUserAsync(owner, cancellationToken);
        var addResult = mainWishList.Add(wish, by: owner);
        if (addResult.IsFailed)
        {
            return addResult;
        }
        
        unitOfWork.Wishes.Add(wish);
        unitOfWork.WishLists.AddWish(mainWishList, wish);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return WishDto.FromWish(wish, currUser: owner);
    }
}