using System.Data.Common;

namespace FastPackForShare.Interceptor;

public class SlowQueryInterceptor : DbCommandInterceptor
{
    private const int _slowQueryThresholdMilliSeconds = 200;

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        if (eventData.Duration.TotalMilliseconds > _slowQueryThresholdMilliSeconds)
        {
            Console.WriteLine($"Slow query ({eventData.Duration.TotalMilliseconds} ms): {command.CommandText}");
        }

        return base.ReaderExecuted(command, eventData, result);
    }
}
