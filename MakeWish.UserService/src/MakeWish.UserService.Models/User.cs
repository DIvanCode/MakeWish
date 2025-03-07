using EnsureThat;

namespace MakeWish.UserService.Models;

public sealed class User
{
    public int Id { get; init; }

    public string Email { get; init; }

    public string PasswordHash { get; }

    public string Name { get; private set; }

    public string Surname { get; private set; }

    private User(string email, string passwordHash, string name, string surname)
    {
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
        Surname = surname;
    }
    
    public static User Create(string email, string passwordHash, string name, string surname)
    {
        EnsureArg.IsNotNullOrWhiteSpace(email);
        EnsureArg.IsNotNullOrWhiteSpace(passwordHash);
        
        return new User(email, passwordHash, name, surname);
    }
}
