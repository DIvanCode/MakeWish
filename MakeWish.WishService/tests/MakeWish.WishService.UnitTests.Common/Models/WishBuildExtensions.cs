using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.Models;

public static class WishBuildExtensions
{
    public static Wish PromisedBy(this Wish wish, User user)
    {
        wish.PromiseBy(user);
        return wish;
    }
    
    public static Wish CompletedBy(this Wish wish, User user)
    {
        wish.CompleteBy(user);
        return wish;
    }
    
    public static Wish DeletedBy(this Wish wish, User user)
    {
        wish.DeleteBy(user);
        return wish;
    }
    
    public static Wish ApprovedBy(this Wish wish, User user)
    {
        wish.CompleteApproveBy(user);
        return wish;
    }
}