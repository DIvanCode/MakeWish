using MakeWish.UserService.Models;

namespace MakeWish.UserService.UnitTests.Common.Models;

public sealed class UserBuilder
{
    private string _email = "default@email.com";
    private string _passwordHash = "passwordhash";
    private string _name = "name";
    private string _surname = "surname";
    
    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
        return this;
    }

    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserBuilder WithSurname(string surname)
    {
        _surname = surname;
        return this;
    }

    public User Build()
    {
        return User.Create(_email, _passwordHash, _name, _surname);
    }
}