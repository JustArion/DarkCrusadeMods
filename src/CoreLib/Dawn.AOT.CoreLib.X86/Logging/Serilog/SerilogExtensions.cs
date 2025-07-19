using Dawn.AOT.CoreLib.X86.Logging.Serilog.Enrichers;
using Serilog;
using Serilog.Configuration;

namespace Dawn.AOT.CoreLib.X86.Logging.Serilog;

public static class SerilogExtensions
{
    public static LoggerConfiguration WithClassName(
        this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        ArgumentNullException.ThrowIfNull(enrichmentConfiguration);
        
        return enrichmentConfiguration.With<ClassNameEnricher>();
    }
}