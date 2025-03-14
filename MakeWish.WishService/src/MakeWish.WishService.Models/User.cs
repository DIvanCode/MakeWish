namespace MakeWish.WishService.Models;

public sealed class User(Guid id, string name, string surname)
{
    public Guid Id { get; init; } = id;

    public string Name { get; init; } = name;

    public string Surname { get; init; } = surname;
}