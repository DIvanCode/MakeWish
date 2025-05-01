using EnsureThat;
using MakeWish.UserService.Models.Events;

namespace MakeWish.UserService.Models.Entities;

public sealed class User : DomainEntity
{
    public Guid Id { get; init; }

    public string Email { get; init; }

    public string PasswordHash { get; }

    public string Name { get; private set; }

    public string Surname { get; private set; }

    private User(Guid id, string email, string passwordHash, string name, string surname)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
        Surname = surname;
    }
    
    public static User Create(string email, string passwordHash, string name, string surname)
    {
        EnsureArg.IsNotNullOrWhiteSpace(email);
        EnsureArg.IsNotNullOrWhiteSpace(passwordHash);
        
        var id = Guid.NewGuid();
        var user = new User(id, email, passwordHash, name, surname);
        
        user.DomainEvents.Add(new UserCreatedEvent(user));
        
        return user;
    }

    public void Delete()
    {
        DomainEvents.Add(new UserDeletedEvent(this));
    }
}
