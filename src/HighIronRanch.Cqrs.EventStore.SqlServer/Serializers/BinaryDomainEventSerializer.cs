using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SimpleCqrs.Eventing;

namespace HighIronRanch.Cqrs.EventStore.SqlServer.Serializers
{
    public class BinaryDomainEventSerializer : IDomainEventSerializer
    {
        public string Serialize(DomainEvent domainEvent) {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream()) {
                formatter.Serialize(stream, domainEvent);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public DomainEvent Deserialize(Type targetType, string serializedDomainEvent) {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(Convert.FromBase64String(serializedDomainEvent))) {
                return (DomainEvent)formatter.Deserialize(stream);
            }
        }
    }
}