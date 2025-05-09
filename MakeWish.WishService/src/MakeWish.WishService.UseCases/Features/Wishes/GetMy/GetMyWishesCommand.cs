using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.GetMy;

public sealed record GetMyWishesCommand : IRequest<Result<IReadOnlyList<WishDto>>>;