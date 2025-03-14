using FluentResults;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.Authenticate;

public sealed record AuthenticateCommand(string Email, string Password) : IRequest<Result<string>>;