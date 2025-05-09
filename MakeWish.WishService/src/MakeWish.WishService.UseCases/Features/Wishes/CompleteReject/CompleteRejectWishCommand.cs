using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.CompleteReject;

public sealed record CompleteRejectWishCommand(Guid Id) : IRequest<Result<WishDto>>;