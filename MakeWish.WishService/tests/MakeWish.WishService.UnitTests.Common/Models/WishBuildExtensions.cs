using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.Models;

public static class WishBuildExtensions
{
    public static Wish PromisedBy(this Wish wish, User user)
    {
        wish.Promise(user);
        return wish;
    }
    
    public static Wish CompletedBy(this Wish wish, User user)
    {
        wish.Complete(user);
        return wish;
    }
    
    public static Wish DeletedBy(this Wish wish, User user)
    {
        wish.Delete(user);
        return wish;
    }
    
    public static Wish ApprovedBy(this Wish wish, User user)
    {
        wish.CompleteApprove(user);
        return wish;
    }
}