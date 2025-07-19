using Dawn.AOT.CoreLib.X86.Loader;

namespace Dawn.DarkCrusade.Mods.BorderlessWindowed;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using AOT.CoreLib.X86;
using AOT.CoreLib.X86.Config;
using AOT.CoreLib.X86.Logging;
using Config;
using global::Serilog;
using static WindowStyles;
internal static class EntryPoint
{
    internal static LoaderInformation _loaderInfo;
    private static ModFolder<ModConfig> _modFolder = null!;
    
    [UnmanagedCallersOnly(EntryPoint = nameof(Init))]
    public static void Init(nint loaderInfo) // BootstrapInformation*
    {
        _loaderInfo = LoaderInformation.FromPointer(loaderInfo); 
        Task.Run(() =>
        {
            try
            {
                ModLogging.Initialize(_loaderInfo.Module);
                _modFolder = new(_loaderInfo.Module, ConfigSourceGenerator.Default.ModConfig);
                
                DllMain();
            }
            catch (Exception e)
            {
                try
                {
                    Log.Fatal(e, "Fatal Error");
                    Log.CloseAndFlush();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(new AggregateException(e, ex));
                }
            }
        });
    }

    private static HWND GetMainThreadHandle(uint threadId = 0)
    {
        var mainThread = HWND.NULL;
        EnumWindows((hwnd, procId) =>
        {
            var tid = GetWindowThreadProcessId(hwnd, out var pid);
            if (pid != procId)
                return true;

            if (threadId != 0)
            {
                if (tid == threadId)
                {
                    mainThread = hwnd;
                    return false;
                }
            }

            // The first thread id is always the oldest (aka the main thread)
            mainThread = hwnd;
            return false;

        }, Environment.ProcessId);

        return mainThread;
    }

    private static nint _originalWindowStyle;
    private static void DllMain()
    {
        _modFolder.ConfigUpdated += OnConfigUpdated;

        if (!_modFolder.Config.Enabled) 
            return;
        
        var hwnd = GetMainThreadHandle(_loaderInfo.MainThreadId);
        _originalWindowStyle = GetWindowLongAuto(hwnd, WindowLongFlags.GWL_STYLE);

        var sb = new StringBuilder();
        _ = GetClassName(hwnd, sb, 255);
        if (sb.ToString() == "ConsoleWindowClass")
            return;

        if (!CanMakeWindowBorderless(hwnd))
            return;

        MakeWindowBorderless(hwnd);
    }

    private static bool CanMakeWindowBorderless(HWND hwnd)
    {
        if (hwnd.IsNull)
            return false;
        
        if (!File.Exists("Local.ini"))
        {
            Log.Error("Failed to find Local.ini, unable to detect if the game is in windowed mode");
            return false;
        }

        var isWindowed = File.ReadAllLines("Local.ini").Contains("screenwindowed=1");

        if (isWindowed) 
            return true;
        
        Log.Error("The game is not in windowed mode");
        return false;
    }

    private static void OnConfigUpdated()
    {
        var hwnd = GetMainThreadHandle();
        
        if (_modFolder.Config.Enabled)
            MakeWindowBorderless(hwnd);
        else
            RestoreWindow(hwnd);
    }

    [SuppressMessage("ReSharper", "RedundantOverflowCheckingContext")]
    private static void MakeWindowBorderless(HWND handle)
    {
        var style = unchecked(_originalWindowStyle & (nint)~(WS_CAPTION | WS_MAXIMIZEBOX | WS_SYSMENU | WS_THICKFRAME));
        SetWindowLong(handle, WindowLongFlags.GWL_STYLE, style);

        ShowWindow(handle, ShowWindowCommand.SW_MAXIMIZE);
        
        Log.Information("The game is now borderless");
    }
    
    private static void RestoreWindow(HWND handle)
    {
        SetWindowLong(handle, WindowLongFlags.GWL_STYLE, _originalWindowStyle);
        ShowWindow(handle, ShowWindowCommand.SW_MAXIMIZE);
    }
}