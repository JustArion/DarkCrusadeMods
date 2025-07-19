// #define DEBUG

using System.IO.Pipes;
using System.Text;
using System.Text.RegularExpressions;
using Dawn.AOT.CoreLib.X86.Logging.Serilog;
using Dawn.AOT.CoreLib.X86.Logging.Serilog.Themes;
using Microsoft.Win32.SafeHandles;
using FileAccess = System.IO.FileAccess;

namespace Dawn.DarkCrusade.ModLoader;

using System.Diagnostics;
using global::Serilog;
using global::Serilog.Events;

public static partial class Logging
{
    private const string LOGGING_FORMAT = "{Level:u1} {Timestamp:yyyy-MM-dd HH:mm:ss.ffffff}   [{Source}] {Message:lj}{NewLine}{Exception}";

    public static void Initialize(DirectoryInfo directory)
    {
        // Console.SetOut(TextWriter.Synchronized(Console.Out));

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

    private static bool _handleOnce;
    public static void SaveConsoleOutputTo(DirectoryInfo modsFolder)
    {
        var path = new FileInfo(Path.Combine(modsFolder.FullName, "Console.log"));
        
        try
        {
            var fs = new FileStream(path.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

            var stdOut = GetStdHandle(StdHandleType.STD_OUTPUT_HANDLE);
            var stdErr = GetStdHandle(StdHandleType.STD_ERROR_HANDLE);

            var stdOutpipe = new AnonymousPipeServerStream(PipeDirection.In);
            var stdErrPipe = new AnonymousPipeServerStream(PipeDirection.In);

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var outBuffer = new byte[4096];
                    var errBuffer = new byte[4096];
                    Task<int>? outTask = null;
                    Task<int>? errTask = null;
                    while (true)
                    {
                        outTask ??= stdOutpipe.ReadAsync(outBuffer, CancellationToken.None).AsTask();
                        errTask ??= stdErrPipe.ReadAsync(errBuffer, CancellationToken.None).AsTask();

                        var readTask = await Task.WhenAny(outTask, errTask);

                        if (readTask == outTask)
                            outTask = await TranslateConsole(new TranslateOptions(outBuffer, (uint)readTask.Result, stdOut, stdOutpipe, fs));
                        else
                            errTask = await TranslateConsole(new TranslateOptions(errBuffer, (uint)readTask.Result, stdErr, stdErrPipe, fs));

                    }
                }
                catch (Exception e)
                {
                    if (!_handleOnce)
                    {
                        _handleOnce = true;
                        Log.Error(e, "Exception within proxy loop");
                    }
                }
            }, TaskCreationOptions.LongRunning);

            if (!SetStdHandle(StdHandleType.STD_OUTPUT_HANDLE, stdOutpipe.ClientSafePipeHandle.DangerousGetHandle()))
                Log.Error(GetLastError().GetException(), "Unable to save console output to new STD-OUT");
            if (!SetStdHandle(StdHandleType.STD_ERROR_HANDLE, stdErrPipe.ClientSafePipeHandle.DangerousGetHandle()))
                Log.Error(GetLastError().GetException(), "Unable to save console output to new STD-ERROR");
        }
        catch (Exception e)
        {
            if (!_handleOnce)
            {
                _handleOnce = true;
                Log.Error(e, "Unable to proxy console output to file {FileName}", path.Name);
            }
        }
        
    }

    private static async Task<Task<int>> TranslateConsole(TranslateOptions options) 
    {
        Task<int> outTask;
        try
        {
            unsafe
            {
                var outBufferPtr = Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(options.Buffer));
                WriteFile(options.StandardHandle, (byte*)outBufferPtr, options.BufferContentSize);
            }

            var chunk = Encoding.UTF8.GetString(options.Buffer, 0, (int)options.BufferContentSize);

            // We're removing any Ansi Escape codes (Our logger uses that instead of setting the console output (for speed i believe))
            var cleaned = AnsiEscapeRegex().Replace(chunk, string.Empty);

            var cleanedBytes = Encoding.UTF8.GetBytes(cleaned);
            foreach (var stream in options.Streams)
            {
                await stream.WriteAsync(cleanedBytes);
                await stream.FlushAsync();
            }
        }
        catch (Exception e)
        {
            if (!_handleOnce)
            {
                _handleOnce = true;
                Log.Error(e, "Exception within translation");
            }
        }
        finally
        {
            outTask = options.ReadingPipe.ReadAsync(options.Buffer, CancellationToken.None).AsTask();
        }

        return outTask;
    }

    [GeneratedRegex(@"\x1B\[[0-9;]*m")]
    private static partial Regex AnsiEscapeRegex();

    private readonly record struct TranslateOptions(
        byte[] Buffer,
        uint BufferContentSize,
        HFILE StandardHandle,
        AnonymousPipeServerStream ReadingPipe,
        params IEnumerable<Stream> Streams);
}