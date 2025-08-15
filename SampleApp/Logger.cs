using System.Text;
using Microsoft.Extensions.Logging;

/// <summary>
/// Do not use this logger in production, it is only for demonstration purposes!!!
/// </summary>
internal class Logger<T>(LogLevel minLevel = LogLevel.Information) : ILogger<T>
{
    private static readonly AsyncLocal<Scope?> _currentScope = new();
    private static readonly object _lock = new();

    private readonly string _categoryName = typeof(T).FullName ?? typeof(T).Name;

    private sealed class Scope(object state, Scope? parent) : IDisposable
    {
        public object State { get; } = state;
        public Scope? Parent { get; } = parent;

        public void Dispose()
        {
            _currentScope.Value = Parent;
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var scope = new Scope(state, _currentScope.Value);
        _currentScope.Value = scope;
        return scope;
    }

    public bool IsEnabled(LogLevel logLevel)
        => logLevel != LogLevel.None && logLevel >= minLevel;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
        if (formatter is null)
            throw new ArgumentNullException(nameof(formatter));

        var message = formatter(state, exception);
        if (string.IsNullOrEmpty(message) && exception is null)
            return;

        var sb = new StringBuilder();
        sb.Append('[')
          .Append(DateTimeOffset.Now.ToString("u"))
          .Append("] ");

        sb.Append(GetShortLevel(logLevel))
          .Append(' ')
          .Append(_categoryName);

        // Append scope chain
        var scopes = CollectScopes();
        if (scopes.Count > 0)
        {
            sb.Append(" => ");
            for (int i = 0; i < scopes.Count; i++)
            {
                if (i > 0) sb.Append(" -> ");
                sb.Append(scopes[i]);
            }
        }

        sb.Append(": ").Append(message);

        if (exception is not null)
        {
            sb.AppendLine()
              .Append(exception.GetType().FullName)
              .Append(": ")
              .Append(exception.Message)
              .AppendLine()
              .Append(exception.StackTrace);
        }

        lock (_lock)
        {
            var originalColor = Console.ForegroundColor;
            var originalBg = Console.BackgroundColor;
            SetColors(logLevel);
            Console.WriteLine(sb.ToString());
            Console.ForegroundColor = originalColor;
            Console.BackgroundColor = originalBg;
        }
    }

    private static List<object> CollectScopes()
    {
        var list = new List<object>();
        var current = _currentScope.Value;
        while (current is not null)
        {
            list.Insert(0, current.State);
            current = current.Parent;
        }
        return list;
    }

    private static string GetShortLevel(LogLevel level) => level switch
    {
        LogLevel.Trace => "trce",
        LogLevel.Debug => "dbug",
        LogLevel.Information => "info",
        LogLevel.Warning => "warn",
        LogLevel.Error => "fail",
        LogLevel.Critical => "crit",
        _ => "none"
    };

    private static void SetColors(LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Trace:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
            case LogLevel.Debug:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            case LogLevel.Information:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogLevel.Critical:
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Red;
                break;
        }
    }
}