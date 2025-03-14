using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Create;

public sealed record CreateWishCommand(string Title, string? Description) : IRequest<Result<WishDto>>;