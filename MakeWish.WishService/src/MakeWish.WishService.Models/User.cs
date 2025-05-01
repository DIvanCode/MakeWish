using EnsureThat;

namespace MakeWish.WishService.Models;

public sealed class User : Entity
{
    public string Name { get; set; } =  default!;
    public string Surname { get; set; } = default!;
    
    // ReSharper disable once UnusedMember.Local
    public User()
    {
    }
    
    public User(Guid id, string name, string surname)
    {
        EnsureArg.IsNotNullOrEmpty(name, nameof(name));
        EnsureArg.IsNotNullOrEmpty(surname, nameof(surname));

        Id = id;
        Name = name;
        Surname = surname;
    }
}