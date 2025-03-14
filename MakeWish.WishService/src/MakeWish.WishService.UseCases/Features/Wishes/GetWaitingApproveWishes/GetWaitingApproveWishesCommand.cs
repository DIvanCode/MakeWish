using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.GetWaitingApproveWishes;

public sealed record GetWaitingApproveWishesCommand : IRequest<Result<List<WishDto>>>;