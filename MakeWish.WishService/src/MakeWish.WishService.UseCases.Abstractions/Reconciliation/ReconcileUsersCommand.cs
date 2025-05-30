using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Reconciliation;

public sealed record ReconcileUsersCommand : IRequest<Result>;
