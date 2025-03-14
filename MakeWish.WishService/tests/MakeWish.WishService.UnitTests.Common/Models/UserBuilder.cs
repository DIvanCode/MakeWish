using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.Models;

public sealed class UserBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "name";
    private string _surname = "surname";

    public UserBuilder WithId(Guid id)
    {
        _id = id;
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
        return new User(_id, _name, _surname);
    }
}