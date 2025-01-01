namespace Dawn.AOT.CoreLib.X86.Logging;

using global::Serilog;
using global::Serilog.Events;

public class NullLogger : ILogger
{
    public void Write(LogEvent logEvent)
    {
        
    }
}