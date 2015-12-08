namespace HighIronRanch.Cqrs.EventStore.SqlServer
{
    public interface ISqlServerEventStoreSettings
    {
        string SqlServerConnectionString { get; }
        string SqlServerEventStoreTableName { get; }
    }
}