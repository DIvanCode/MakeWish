using MakeWish.UserService.Models.Events;

namespace MakeWish.UserService.Models.Entities;

public abstract class DomainEntity
{
    protected readonly List<DomainEvent> DomainEvents = [];

    public IReadOnlyList<DomainEvent> CollectDomainEvents()
    {
        var domainEvents = new DomainEvent[DomainEvents.Count];
        DomainEvents.CopyTo(domainEvents);
        
        DomainEvents.Clear();
        
        return domainEvents.AsReadOnly();
    }
}