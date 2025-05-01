using System.Reflection;
using MakeWish.UserService.Adapters.DataAccess.EntityFramework.Configurations;
using MakeWish.UserService.Adapters.DataAccess.EntityFramework.Repositories;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework;

public sealed class UnitOfWork : DbContext, IUnitOfWork
{
    public IUsersRepository Users => _userRepository.Value;
    public IFriendshipsRepository Friendships => _friendshipsRepository.Value;
    
    private readonly Lazy<IUsersRepository> _userRepository;
    private readonly Lazy<IFriendshipsRepository> _friendshipsRepository;
    private readonly IMediator _mediator;
    private bool _savingChanges;

    public UnitOfWork(DbContextOptions<UnitOfWork> options, IMediator mediator) : base(options)
    {
        _userRepository = new Lazy<IUsersRepository>(() => new UsersRepository(Set<User>()));
        _friendshipsRepository = new Lazy<IFriendshipsRepository>(() => new FriendshipsRepository(Set<Friendship>()));
        _mediator = mediator;
        _savingChanges = false;
    }
    
    public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        if (_savingChanges)
        {
            return 0;
        }

        _savingChanges = true;
        
        while (true)
        {
            var domainEvents = ChangeTracker
                .Entries<DomainEntity>()
                .SelectMany(entry => entry.Entity.CollectDomainEvents())
                .ToList();

            if (domainEvents.Count == 0)
            {
                break;
            }
            
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
        
        var result = await base.SaveChangesAsync(cancellationToken);
        
        _savingChanges = false;
        return result;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}