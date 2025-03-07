using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.GetById;

public sealed record GetByIdCommand(int Id) : IRequest<Result<UserDto>>;