using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.GetUserWishes;

public sealed record GetUserWishesCommand(Guid UserId, string? Query) : IRequest<Result<IReadOnlyList<WishDto>>>;