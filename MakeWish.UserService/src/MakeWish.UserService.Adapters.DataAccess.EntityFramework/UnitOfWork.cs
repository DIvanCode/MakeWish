using System.Reflection;
using MakeWish.UserService.Adapters.DataAccess.EntityFramework.Configurations;
using MakeWish.UserService.Adapters.DataAccess.EntityFramework.Repositories;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework;

public sealed class UnitOfWork : DbContext, IUnitOfWork
{
    public IUsersRepository Users => _userRepository.Value;
    public IFriendshipsRepository Friendships => _friendshipsRepository.Value;
    
    private readonly Lazy<IUsersRepository> _userRepository;
    private readonly Lazy<IFriendshipsRepository> _friendshipsRepository;

    public UnitOfWork(DbContextOptions<UnitOfWork> options) : base(options)
    {
        _userRepository = new Lazy<IUsersRepository>(() => new UsersRepository(Set<User>()));
        _friendshipsRepository = new Lazy<IFriendshipsRepository>(() => new FriendshipsRepository(Set<Friendship>()));
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}