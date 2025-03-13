using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.Register;

public sealed record RegisterCommand(string Email, string Password, string Name, string Surname)
    : IRequest<Result<UserDto>>;