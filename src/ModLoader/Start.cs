namespace Dawn.DarkCrusade.ModLoader;

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using AOT.CoreLib.X86.Threading;
using Config;
using Mods;
using NtdllEx;

internal static class Start
{
    internal static LoaderConfig _config = null!;

    public static async Task DllMain()
    {
        await InitializePrerequisites();

        var proc = Process.GetCurrentProcess();
        var module = proc.MainModule!;
        var baseAddress = module.BaseAddress;
        LogSettings(baseAddress);

        var mods = Heuristics.GetOrderedMods();
        LogLoadOrder();

        Log.Verbose("Waiting on Load Delay");
        await Task.Delay(TimeSpan.FromSeconds(_config.LoadDelaySeconds));
        Log.Verbose("Delay Complete, Loading Mods");


        SoftSuspendMainThread(out var originalPriority, out var mainThread);
        
        await LoadMods(mods);
        
        RestoreMainThread(mainThread, originalPriority);
        Log.Information("All Mods Loaded!");
    }

    private static void LogLoadOrder()
    {
        Log.Information("Mod Load Orders (Lower = Later)");
        foreach (var mod in Heuristics.LoadOrder.OrderByDescending(kvp => kvp.Value)) 
            Log.Information("[{LoadOrder}] {ModName}", mod.Value, 
                Heuristics.FileToModName(
                    new FileInfo(
                        Path.Combine(Heuristics.ModFolderPath, mod.Key))));
    }

    private static void RestoreMainThread(SafeHTHREAD mainThread, int originalPriority)
    {
        SetThreadPriority(mainThread, originalPriority);
    }

    private static void SoftSuspendMainThread(out int originalPriority, out SafeHTHREAD mainThread)
    {
        mainThread = OpenThread(0x0002, false, EntryPoint.LoaderInfo.MainThreadId);
        originalPriority = GetThreadPriority(mainThread);
        SetThreadPriority(mainThread, (int)THREAD_PRIORITY.THREAD_PRIORITY_IDLE);
    }

    private static void LogSettings(nint baseAddress)
    {
        Log.Verbose("Process Base Address: 0x{BaseAddress:X}", baseAddress);
        Log.Verbose("Load Delay: {Delay} sec", Math.Round(_config.LoadDelaySeconds, 2));
        Log.Verbose("Use Console: {UseConsole}", _config.UseConsole);
    }

    private static async Task InitializePrerequisites()
    {
        _config = ModLoaderConfig.FetchConfig(Heuristics.CurrentModuleDirectory);
               
        await Task.Delay(TimeSpan.FromMilliseconds(150));

        if (_config.UseConsole && Logging.InitializeConsole())
            Console.WriteLine("[*] Attached to existing Console!");
        
        // We don't call Logging.Initialize before Logging.InitializeConsole as this can cause log artifacts in the console for AllocConsole users
        Logging.Initialize(Heuristics.CurrentModuleDirectory);
        Log.Verbose("Game Main Thread Id: 0x{MainThreadId:X}, Current Thread Id: 0x{CurrentThreadId:X}", EntryPoint.LoaderInfo.MainThreadId, GetCurrentThreadId());
    }

    private static async Task LoadMods(FileInfo[] mods)
    {
        try
        {
            foreach (var mod in mods)
            {
                try
                {
                    var modName = Heuristics.FileToModName(mod);
                    Log.Information("Loading {ModName}!", modName);
                    
                    #if RELEASE
                    var bufferWidth = Console.BufferWidth / 2;
                    Console.WriteLine(new string('-', bufferWidth));
                    #endif
                    
                    var modHandle = NativeLibrary.Load(mod.FullName);
                    
                    Log.Debug("Loaded {ModName} at 0x{Address:X}", modName, modHandle);
                    await Task.Delay(TimeSpan.FromMilliseconds(25));
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to load mod {ModName}", mod.FullName);
                }

            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Exception within Mod Loader");
        }
    }
}