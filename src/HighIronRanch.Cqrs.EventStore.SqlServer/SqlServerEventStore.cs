using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SimpleCqrs.Eventing;

namespace HighIronRanch.Cqrs.EventStore.SqlServer
{
    public class SqlServerEventStore : IEventStore
    {
        private readonly IDomainEventSerializer _serializer;
        private readonly ISqlServerEventStoreSettings _settings;

        public SqlServerEventStore(ISqlServerEventStoreSettings settings, IDomainEventSerializer serializer)
        {
            this._serializer = serializer;
            this._settings = settings;
            Init();
        }

        public void Init()
        {
            using (var connection = new SqlConnection(_settings.SqlServerConnectionString))
            {
                connection.Open();
                var sql = string.Format(SqlConstants.CreateTheEventStoreTable, _settings.SqlServerEventStoreTableName);
                using (var command = new SqlCommand(sql, connection))
                    command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var events = new List<DomainEvent>();
            using (var connection = new SqlConnection(_settings.SqlServerConnectionString))
            {
                connection.Open();
                var sql = string.Format(SqlConstants.GetEventsByAggregateRootAndSequence, "", _settings.SqlServerEventStoreTableName, aggregateRootId,
                                        startSequence);
                using (var command = new SqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        var type = reader[SqlConstants.EventTypeColumn].ToString();
                        var data = reader[SqlConstants.DataColumn].ToString();

                        try
                        {
                            events.Add(_serializer.Deserialize(Type.GetType(type), data));
                        } catch(ArgumentNullException ex) 
                        {
                            throw new Exception(string.Format("Cannot find type '{0}', yet the type is in the event store. Are you sure you haven't changed a class name or something arising from mental dullness?", type.Split(',')[0]), ex.InnerException);
                        }
                    }
                connection.Close();
            }
            return events;
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            var sql = new StringBuilder();
            foreach (var domainEvent in domainEvents)
                sql.AppendFormat(SqlConstants.InsertEvents, _settings.SqlServerEventStoreTableName, TypeToStringHelperMethods.GetString(domainEvent.GetType()), domainEvent.AggregateRootId, domainEvent.EventDate, domainEvent.Sequence,
                                 (_serializer.Serialize(domainEvent) ?? string.Empty)
                                 .Replace("'", "''"));

            if (sql.Length <= 0) return;

            using (var connection = new SqlConnection(_settings.SqlServerConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql.ToString(), connection))
                    command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            var events = new List<DomainEvent>();

            var eventParameters = domainEventTypes.Select(TypeToStringHelperMethods.GetString).Join("','");

            using (var connection = new SqlConnection(_settings.SqlServerConnectionString))
            {
                connection.Open();
                var sql = string.Format(SqlConstants.GetEventsByType, _settings.SqlServerEventStoreTableName, eventParameters);
                using (var command = new SqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        var type = reader[SqlConstants.EventTypeColumn].ToString();
                        var data = reader[SqlConstants.DataColumn].ToString();

                        var domainEvent = _serializer.Deserialize(Type.GetType(type), data);
                        events.Add(domainEvent);
                    }
                connection.Close();
            }
            return events;
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, Guid aggregateRootId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate, DateTime endDate)
        {
            var events = new List<DomainEvent>();

            var eventParameters = domainEventTypes.Select(TypeToStringHelperMethods.GetString).Join("','");

            using (var connection = new SqlConnection(_settings.SqlServerConnectionString))
            {
                connection.Open();
                var sql = string.Format(SqlConstants.GetEventsByTypeAndDate, 
                    _settings.SqlServerEventStoreTableName, 
                    eventParameters,
                    startDate.ToString("yyyy-MM-ddThh:mm:ss"),
                    endDate.ToString("yyyy-MM-ddThh:mm:ss"));

                using (var command = new SqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        var type = reader[SqlConstants.EventTypeColumn].ToString();
                        var data = reader[SqlConstants.DataColumn].ToString();

                        var domainEvent = _serializer.Deserialize(Type.GetType(type), data);
                        events.Add(domainEvent);
                    }
                connection.Close();
            }
            return events;
        }
    }
}