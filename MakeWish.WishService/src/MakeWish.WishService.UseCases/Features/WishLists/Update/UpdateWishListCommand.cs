using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.Update;

public sealed record UpdateWishListCommand(Guid Id, string Title) : IRequest<Result<WishListDto>>;