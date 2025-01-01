namespace Dawn.Serilog.CustomEnrichers;

using global::Serilog;
using global::Serilog.Configuration;

public static class SerilogExtensions
{
    public static LoggerConfiguration WithClassName(
        this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        ArgumentNullException.ThrowIfNull(enrichmentConfiguration);
        
        return enrichmentConfiguration.With<ClassNameEnricher>();
    }
}