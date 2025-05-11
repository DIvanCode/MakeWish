using MakeWish.WishService.Models;
using Neo4j.Driver;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;

public sealed class UsersMapper : MapperBase<User>
{
    public const string UserType = nameof(User);
    public const string UserNode = "user";

    public const string NameProperty = "Name";
    public const string SurnameProperty = "Surname";
    public const string PublicWishListIdProperty = "PublicWishListId";
    public const string PrivateWishListIdProperty = "PrivateWishListId";
    
    public override string EntityType => UserType;
    public override string EntityNode => UserNode;
    
    protected override List<string> Properties => [
        IMapper<User>.IdProperty,
        NameProperty,
        SurnameProperty,
        PublicWishListIdProperty,
        PrivateWishListIdProperty];
    
    public override User? MapToEntity(IRecord record, string? node = null)
    {
        var user = base.MapToEntity(record, node);
        return user;
    }
}