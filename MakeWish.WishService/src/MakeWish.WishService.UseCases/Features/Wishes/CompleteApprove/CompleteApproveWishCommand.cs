using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.CompleteApprove;

public sealed record CompleteApproveWishCommand(Guid WishId) : IRequest<Result<WishDto>>;