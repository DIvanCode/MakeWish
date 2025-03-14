using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Users.Delete;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result>;