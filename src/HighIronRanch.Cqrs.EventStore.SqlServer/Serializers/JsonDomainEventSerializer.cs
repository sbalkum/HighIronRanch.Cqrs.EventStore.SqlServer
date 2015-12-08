using System;
using Newtonsoft.Json;
using SimpleCqrs.Eventing;

namespace HighIronRanch.Cqrs.EventStore.SqlServer.Serializers
{
    public class JsonDomainEventSerializer : IDomainEventSerializer
    {
        public string Serialize(DomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(domainEvent);
        }

        public DomainEvent Deserialize(Type targetType, string serializedDomainEvent)
        {
            return (DomainEvent)JsonConvert.DeserializeObject(serializedDomainEvent, targetType);
        }
    }
}