#nullable enable
namespace Dawn.Serilog.CustomEnrichers;

using System.Diagnostics;
using System.Text.RegularExpressions;
using global::Serilog.Core;
using global::Serilog.Events;

public partial class ClassNameEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var frame = new StackTrace()
            .GetFrames().FirstOrDefault(x =>
        {
            var type = x.GetMethod()?.ReflectedType;

            if (type == null)
            {
                var decType = DiagnosticMethodInfo.Create(x)?.DeclaringTypeName;
                if (decType == null || decType == typeof(ClassNameEnricher).FullName)
                    return false;

                return !decType.StartsWith("Serilog.");
            }
            
            if (type == typeof(ClassNameEnricher))
                return false;

            return !type.FullName!.StartsWith("Serilog.");
        });
        if (frame == null)
            return;
        
        var decTypeName = DiagnosticMethodInfo.Create(frame)?.DeclaringTypeName;

        if (decTypeName == null)
            return;
        
        
        // logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Source", GetClassName(type)));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Source", GetClassName(decTypeName)));
    }

    private string? GetClassName(Type type)
    {
        var last = type.FullName!.Split('.').LastOrDefault();

        var className = last?.Split('+').FirstOrDefault()?.Replace("`1", string.Empty).Replace('_', '-');
        var retVal = type.Namespace != null 
            ? $"{type.Namespace}.{className}" 
            : className;

        return retVal;
    }
    
    private string? GetClassName(string type)
    {
        var last = type.Split('.').LastOrDefault();

        var str = last?.Split('+').FirstOrDefault()?.Replace("`1", string.Empty).Replace('_', '-');
        
        // Add a space before every capital letter except the first instance of a capital letter
        // eg. "ConfigWatcher" becomes "Config Watcher" and NOT " Config Watcher"
        if (str == null)
            return null;
        
        var retVal = CapitalLetterReplacement().Replace(str, "$1 $2");

        return retVal;
    }

    private const string BLUE_ANSI = "\e[38;2;59;120;255m";

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex CapitalLetterReplacement();
}