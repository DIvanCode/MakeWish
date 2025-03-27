using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.Create;

public sealed record CreateWishListCommand(string Title) : IRequest<Result<WishListDto>>;