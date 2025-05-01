using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.GetMain;

public sealed record GetMainWishListForCurrentUserCommand : IRequest<Result<WishListDto>>;