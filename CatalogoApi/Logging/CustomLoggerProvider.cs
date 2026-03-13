using System.Collections.Concurrent;

namespace CatalogoApi.Logging;

public class CustomLoggerProvider : ILoggerProvider
{

    readonly CustomLoggerProviderConfig _loggerConfig;

    readonly ConcurrentDictionary<string, CustomerLogger> loggers = 
                                 new ConcurrentDictionary<string, CustomerLogger>();

    public CustomLoggerProvider(CustomLoggerProviderConfig loggerConfig)
    {
        _loggerConfig = loggerConfig;
    }
    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(categoryName, name => new CustomerLogger(name, _loggerConfig));
    }

    public void Dispose()
    {
        loggers.Clear();
    }
}
