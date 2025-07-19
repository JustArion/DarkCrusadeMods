using Dawn.AOT.CoreLib.X86.Logging.Serilog;
using Dawn.AOT.CoreLib.X86.Logging.Serilog.Themes;

namespace Dawn.AOT.CoreLib.X86.Logging;

using System.Diagnostics.CodeAnalysis;
using global::Serilog;
using global::Serilog.Events;
using Vanara.PInvoke;

public static class ModLogging
{
    private const string LOGGING_FORMAT = "{Level:u1} {Timestamp:yyyy-MM-dd HH:mm:ss.ffffff}   [{Source}] {Message:lj}{NewLine}{Exception}";

    [SuppressMessage("ReSharper", "NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract")]
    public static void Initialize(HINSTANCE hinstance)
    {
        string logPath;
        if (hinstance.IsNull)
            logPath = AppContext.BaseDirectory;
        else
        {
            var fileInfo = new FileInfo(GetModuleFileName(hinstance));
            var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            var modDirectory = fileInfo.Directory!.CreateSubdirectory(fileName);

            logPath = Path.Combine(modDirectory.FullName, $"{fileName}.log");
        }
        
        
        try
        {
            var config = new LoggerConfiguration()
                #if DEBUG
                .MinimumLevel.Is(LogEventLevel.Verbose)
                #else
                .MinimumLevel.Is(LogEventLevel.Information)
                #endif
                .Enrich.WithClassName()
                .Enrich.WithProcessName()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: LOGGING_FORMAT, theme: BlizzardTheme.GetTheme, applyThemeToRedirectedOutput: true, standardErrorFromLevel: LogEventLevel.Error)
                .WriteTo.File(logPath, 
                    outputTemplate: LOGGING_FORMAT, 
                    restrictedToMinimumLevel: LogEventLevel.Verbose, 
                    flushToDiskInterval: TimeSpan.FromSeconds(1));
            
            #if RELEASE
            config
                // This is personal preference, but you can set your Seq server to catch :9999 too.
                // (Logs to nowhere if there's no Seq server listening on port 9999
                .WriteTo.Seq("http://localhost:9999");
            #endif

            Log.Logger = config.CreateLogger();
            
            Log.Information("Logs can be found in {LogPath}", logPath);

            AppDomain.CurrentDomain.UnhandledException +=
                (_, eo) =>
                {
                    Log.Fatal(eo.ExceptionObject as Exception, "Unhandled Exception");
                    Log.CloseAndFlush();
                };
            
            AppDomain.CurrentDomain.ProcessExit  +=
                (_, _) => Log.Information("Shutting Down...");

            #if DEBUG
            AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
            {
                Console.Error.WriteLine($"First Chance Exception - {sender}{args.Exception}");
            };
            #endif
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            Log.Logger ??= new NullLogger();
        }
    }
}