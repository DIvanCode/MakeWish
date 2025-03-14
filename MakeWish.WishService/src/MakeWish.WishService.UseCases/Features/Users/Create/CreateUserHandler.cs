using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Users.Create;

public sealed class CreateUserHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateUserCommand, Result>
{
    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (existingUser is not null)
        {
            return new EntityAlreadyExistsError(nameof(User), nameof(User.Id), request.Id);
        }

        var user = new User(request.Id, request.Name, request.Surname);
        unitOfWork.Users.Add(user);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}