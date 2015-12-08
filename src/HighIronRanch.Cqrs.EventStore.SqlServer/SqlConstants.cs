namespace HighIronRanch.Cqrs.EventStore.SqlServer
{
    internal class SqlConstants
    {
        internal const string EventTypeColumn = "EventType";
        internal const string DataColumn = "Data";

        internal const string GetEventsByType = "SELECT EventType, Data FROM {0} WHERE EventType IN ('{1}')";
        internal const string GetEventsByTypeAndDate = "SELECT EventType, Data FROM {0} WHERE EventType IN ('{1}') AND EventDate >= '{2}' AND EventDate < '{3}'";

        internal const string InsertEvents = "INSERT INTO {0} VALUES ('{1}', '{2}', '{3}', {4}, '{5}')";
        internal const string GetEventsByAggregateRootAndSequence = "SELECT EventType, Data FROM {1} WHERE AggregateRootId = '{2}' AND Sequence >= {3}";
        internal const string CreateTheEventStoreTable = @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[{0}](
	[EventId] [int] IDENTITY(1,1) NOT NULL,
	[EventType] [nvarchar](255) NULL,
	[AggregateRootId] [uniqueidentifier] NOT NULL,
	[EventDate] [datetime] NOT NULL,
	[Sequence] [int] NOT NULL,
	[Data] [nvarchar](max) NULL,
    CONSTRAINT [PK_EventStore] PRIMARY KEY NONCLUSTERED 
    (
	    [EventId] ASC
    )
)

CREATE CLUSTERED INDEX [ClusteredIndex-AggRootSeq] ON [dbo].[{0}]
(
	[AggregateRootId] ASC,
	[Sequence] ASC
)

END";
    }
}