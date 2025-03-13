using FluentResults;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.Delete;

public sealed record DeleteCommand(Guid UserId) : IRequest<Result>;