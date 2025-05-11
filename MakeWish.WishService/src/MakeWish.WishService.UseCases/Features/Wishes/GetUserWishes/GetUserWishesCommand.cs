using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.GetUserWishes;

public sealed record GetUserWishesCommand(Guid UserId) : IRequest<Result<IReadOnlyList<WishDto>>>;