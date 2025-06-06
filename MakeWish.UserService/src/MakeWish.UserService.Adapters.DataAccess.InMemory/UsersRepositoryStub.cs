﻿using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.Models.Entities;

namespace MakeWish.UserService.Adapters.DataAccess.InMemory;

public sealed class UsersRepositoryStub : IUsersRepository
{
    private readonly List<User> _users = [];
    
    public void Add(User entity)
    {
        _users.Add(entity);
    }

    public void Remove(User entity)
    {
        _users.Remove(entity);
    }

    public void Update(User entity)
    {
        Remove(entity);
        Add(entity);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.SingleOrDefault(e => e.Email == email));
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.SingleOrDefault(e => e.Id == id));
    }

    public Task<List<User>> GetBySearchQueryAsync(string searchQuery, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users
            .Where(u => $"{u.Name} {u.Surname}" == searchQuery || $"{u.Surname} {u.Name}" == searchQuery)
            .ToList());
    }
    
    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.ToList());
    }

    public Task<bool> HasWithEmailAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.Any(e => e.Email == email));
    }
}