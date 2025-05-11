using MakeWish.WishService.Models;
using Neo4j.Driver;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;

public sealed class WishesMapper(IMapper<User> usersMapper) : MapperBase<Wish>
{
    public const string WishType = nameof(Wish);
    
    public const string WishNode = "wish";
    public const string OwnerNode = "wish_owner";
    public const string PromiserNode = "wish_promiser";
    public const string CompleterNode = "wish_completer";
    
    public const string OwnsLabel = "owns_wish";
    public const string OwnsLabelType = "Owns";
    
    public const string PromisedLabel = "promised_wish";
    public const string PromisedLabelType = "Promised";
    
    public const string CompletedLabel = "completed_wish";
    public const string CompletedLabelType = "Completed";

    private const string TitleProperty = "Title";
    private const string DescriptionProperty = "Description";
    public const string StatusProperty = "_status";
    
    public const string OwnerProperty = "Owner";
    public const string PromiserProperty = "_promiser";
    public const string CompleterProperty = "_completer";

    public override string EntityType => WishType;
    public override string EntityNode => WishNode;
    
    protected override List<string> Properties => [
        IMapper<Wish>.IdProperty,
        TitleProperty,
        DescriptionProperty,
        StatusProperty];
    
    public override List<string> GetReturningRecordProperties(string node, params string[] neighbours)
    {
        var properties = base.GetReturningRecordProperties(node);
        
        if (neighbours.Contains(OwnerNode))
            properties = properties.Concat(usersMapper.GetReturningRecordProperties(OwnerNode, neighbours)).ToList();
        if (neighbours.Contains(PromiserNode))
            properties = properties.Concat(usersMapper.GetReturningRecordProperties(PromiserNode, neighbours)).ToList();
        if (neighbours.Contains(CompleterNode))
            properties = properties.Concat(usersMapper.GetReturningRecordProperties(CompleterNode, neighbours)).ToList();
        
        return properties;
    }
    
    public override Wish? MapToEntity(IRecord record, string? node = null)
    {
        var wish = base.MapToEntity(record, node);
        if (wish is null)
        {
            return wish;
        }
        
        var owner = usersMapper.MapToEntity(record, OwnerNode);
        ReflectionHelper<Wish>.SetPropertyValue(wish, OwnerProperty, owner, extractValue: false);
        
        var promiser = usersMapper.MapToEntity(record, PromiserNode);
        ReflectionHelper<Wish>.SetPropertyValue(wish, PromiserProperty, promiser, extractValue: false);
        
        var completer = usersMapper.MapToEntity(record, CompleterNode);
        ReflectionHelper<Wish>.SetPropertyValue(wish, CompleterProperty, completer, extractValue: false);
        
        return wish;
    }
}