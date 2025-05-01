using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.Models.Entities;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.Register;

public sealed class RegisterHandler(IUnitOfWork unitOfWork, IPasswordService passwordService)
    : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var hasUserWithEmail = await unitOfWork.Users.HasWithEmailAsync(request.Email, cancellationToken);
        if (hasUserWithEmail)
        {
            return new EntityAlreadyExistsError(nameof(User), nameof(User.Email), request.Email);
        }
        
        var passwordHash = passwordService.GetHash(request.Password);
        var user = User.Create(request.Email, passwordHash, request.Name, request.Surname);
        unitOfWork.Users.Add(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return UserDto.FromUser(user);
    }
}