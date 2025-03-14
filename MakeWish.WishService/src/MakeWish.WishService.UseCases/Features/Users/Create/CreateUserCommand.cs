using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Users.Create;

public sealed record CreateUserCommand(Guid Id, string Name, string Surname)
    : IRequest<Result>; 