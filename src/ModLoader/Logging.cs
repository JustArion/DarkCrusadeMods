// #define DEBUG
namespace Dawn.DarkCrusade.ModLoader;

using System.Diagnostics;
using global::Serilog;
using global::Serilog.Events;
using Serilog.CustomEnrichers;
using Serilog.Themes;

public static class Logging
{
    private const string LOGGING_FORMAT = "{Level:u1} {Timestamp:yyyy-MM-dd HH:mm:ss.ffffff}   [{Source}] {Message:lj}{NewLine}{Exception}";

    public static void Initialize(DirectoryInfo directory)
    {
        Console.SetOut(TextWriter.Synchronized(Console.Out));

        var logPath = Path.Combine(directory.FullName, "Loader.log");
        
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
                .WriteTo.Debug(outputTemplate: LOGGING_FORMAT)
                .WriteTo.Console(outputTemplate: LOGGING_FORMAT, theme: BlizzardTheme.GetTheme, applyThemeToRedirectedOutput: true, standardErrorFromLevel: LogEventLevel.Error)
                .WriteTo.File(logPath, 
                    outputTemplate: LOGGING_FORMAT,
                    shared: true,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 1,
                    fileSizeLimitBytes: (int)Math.Pow(1024, 2), // 1 mb
                    restrictedToMinimumLevel: LogEventLevel.Debug, 
                    flushToDiskInterval: TimeSpan.FromSeconds(1));
            
            #if RELEASE
            config
                // This is personal preference, but you can set your Seq server to catch :9999 too.
                // (Logs to nowhere if there's no Seq server listening on port 9999
                .WriteTo.Seq("http://localhost:9999");
            #endif

            Log.Logger = config.CreateLogger();

            AppDomain.CurrentDomain.UnhandledException +=
                (_, eo) =>
                {
                    Log.Fatal(eo.ExceptionObject as Exception, "Unhandled Exception");
                    Log.CloseAndFlush();
                };
            
            AppDomain.CurrentDomain.ProcessExit  +=
                (_, _) =>
                {
                    Log.Information("Shutting Down...");
                    Log.CloseAndFlush();
                    Console.Clear();
                };

            #if DEBUG
            AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
            {
                Log.Debug(args.Exception, "First Chance Exception");
                Debug.WriteLine($"First Chance Exception - {sender} {args.Exception}");
            };
            #endif
        }
        catch (Exception e)
        {
            Log.Logger = new NullLogger();
            InitializeConsole();
            
            Console.Error.WriteLine(e);
        }
    }

    public static bool InitializeConsole()
    {
        if (AttachConsole(ATTACH_PARENT_PROCESS))
        {
            Console.WriteLine();
            return true;
        }
        
        AllocConsole();
        return false;
    }
}