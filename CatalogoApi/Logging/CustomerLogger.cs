namespace CatalogoApi.Logging;

public class CustomerLogger : ILogger
{
    readonly string _loggerName;

    readonly CustomLoggerProviderConfig _loggerConfig;

    public CustomerLogger(string loggerName, CustomLoggerProviderConfig loggerConfig)
    {
        _loggerName = loggerName;
        _loggerConfig = loggerConfig;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
       return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
       return logLevel == _loggerConfig.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel, 
                            EventId eventId, 
                            TState state, 
                            Exception? exception, 
                            Func<TState, 
                            Exception?, string> formatter)
    {
        string message = $"{logLevel.ToString()}: {eventId} - {formatter(state, exception)}";

        EscreverTextoNoArquivo(message);

    }

    private void EscreverTextoNoArquivo(string message)
    {
        string caminhoArquivo = @"c:\dados\log\Daniel_log.txt";

        using (StreamWriter streamWriter = new StreamWriter(caminhoArquivo, true))
        {
            try
            {
                streamWriter.WriteLine(message);
                streamWriter.Close();
            }
            catch (Exception) 
            {
                throw;
            }

        }
        
    }
}
