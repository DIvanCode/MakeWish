using FluentResults;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.Delete;

public sealed record DeleteCommand(int UserId) : IRequest<Result>;