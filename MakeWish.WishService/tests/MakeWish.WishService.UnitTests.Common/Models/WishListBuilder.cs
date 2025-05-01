using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.Models;

public class WishListBuilder
{
    private string _title = "Test Wish List";
    private User _owner = new UserBuilder().Build();
    private bool _isMain;
    private List<Wish> _wishes = [];

    public WishListBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public WishListBuilder WithOwner(User owner)
    {
        _owner = owner;
        return this;
    }

    public WishListBuilder IsMain()
    {
        _isMain = true;
        return this;
    }

    public WishListBuilder WithWishes(IEnumerable<Wish> wishes)
    {
        _wishes = wishes.ToList();
        return this;
    }

    public WishList Build()
    {
        var wishList = WishList.Create(_title, _owner, _isMain);
        
        foreach (var wish in _wishes)
        {
            wishList.Add(wish, _owner);
        }

        return wishList;
    }
} 