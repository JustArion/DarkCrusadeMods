namespace Dawn.DarkCrusade.ModLoader;

using global::Serilog.Events;

public class NullLogger : ILogger
{
    public void Write(LogEvent logEvent)
    {
    }
}