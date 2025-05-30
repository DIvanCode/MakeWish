using FluentResults;
using MakeWish.WishService.UseCases.Abstractions.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Features.Wishes;

public sealed record CompleteApproveWishCommand(Guid Id) : IRequest<Result<WishDto>>;