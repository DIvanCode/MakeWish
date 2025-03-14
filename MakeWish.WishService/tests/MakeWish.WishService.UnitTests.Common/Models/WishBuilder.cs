using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.Models;

public sealed class WishBuilder
{
    private string _title = "title";
    private string? _description = "description";
    private User _owner = new(Guid.NewGuid(), "name", "surname");

    public WishBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public WishBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public WishBuilder WithOwner(User owner)
    {
        _owner = owner;
        return this;
    }

    public Wish Build()
    {
        return Wish.Create(_title, _description, _owner);
    }
}