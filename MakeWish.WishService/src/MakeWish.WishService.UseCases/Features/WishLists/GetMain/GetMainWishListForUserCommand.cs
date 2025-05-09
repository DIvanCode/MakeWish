using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.GetMain;

public sealed record GetMainWishListForUserCommand(Guid UserId) : IRequest<Result<WishListDto>>;