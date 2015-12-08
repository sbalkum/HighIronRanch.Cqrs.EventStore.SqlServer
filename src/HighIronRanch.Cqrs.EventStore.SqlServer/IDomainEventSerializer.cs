using System;
using SimpleCqrs.Eventing;

namespace HighIronRanch.Cqrs.EventStore.SqlServer
{
    public interface IDomainEventSerializer
    {
        string Serialize(DomainEvent domainEvent);
        DomainEvent Deserialize(Type targetType, string serializedDomainEvent);
    }
}
