using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.DataAccess;

public sealed class GlobalStorage
{
    public readonly List<User> Users = [];
    public readonly List<Wish> Wishes = [];
    public readonly List<WishList> WishLists = [];
    public readonly Dictionary<WishList, List<User>> WishListUserAccess = [];
}