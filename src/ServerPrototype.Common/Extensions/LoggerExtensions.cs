using Microsoft.Extensions.Logging;

public static class LoggerExtensions
{
    public static IDisposable WithScope(this ILogger logger, params (string key, object value)[] pairs)
    {
        var dict = new Dictionary<string, object>(pairs.Length);
        foreach (var (key, value) in pairs)
        {
            dict[key] = value;
        }

        return logger.BeginScope(dict);
    }
}
