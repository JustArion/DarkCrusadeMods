using System.Runtime.CompilerServices;
using Dawn.AOT.CoreLib.X86;
using Dawn.AOT.CoreLib.X86.Config;
using Dawn.AOT.CoreLib.X86.Loader;
using Dawn.AOT.CoreLib.X86.Logging;
using Dawn.DarkCrusade.InteractLuaVM;
using Serilog;

namespace Dawn.DarkCrusade.Mods.LuaHello;

internal static class EntryPoint
{
    internal static LoaderInformation _loaderInfo;
    private static ModFolder _modFolder = null!;
    
    [UnmanagedCallersOnly(EntryPoint = nameof(Init))]
    public static void Init(nint loaderInfo) // BootstrapInformation*
    {
        unsafe
        {
            _loaderInfo = Unsafe.AsRef<LoaderInformation>(loaderInfo.ToPointer());
        }
        Task.Run(async () =>
        {
            try
            {
                var module = _loaderInfo.Module;
                ModLogging.Initialize(module);
                _modFolder = new(module);
                
                await DllMain();
            }
            catch (Exception e)
            {
                try
                {
                    Log.Fatal(e, "Fatal Error");
                    await Log.CloseAndFlushAsync();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(new AggregateException(e, ex));
                }
            }
        });
    }

    private static void ExecuteInlineLua()
    {
        Config.RunString("print('Hello, from C#!')");
    }

    private static void RegisterCustomFunctionInVM()
    {
        Config.RegisterFunction(CallOut);

        Config.RunString("CallOut('Hello, from custom function!')");
    }

    private static void RunFileExample()
    {

        const string FILE_NAME = "CountingExample.lua";
        var path = _modFolder.GetFile(FILE_NAME);

        if (path != null)
            Config.RunFile(path);
        else 
            Log.Warning("Could not find file: '{FileName}' in path '{Path}'", FILE_NAME, path);
    }
    
    private static int CallOut(lua_State luaState)
    {
        var str = lua_tostring(luaState, 1);
        Log.Debug("CustomFunction: {Str}", str);
        
        return 0;
    }

    private static LuaConfig Config 
        => field == default 
            ? field = GlobalLua.GetState() 
            : field;

    private static Task DllMain()
    {
        Log.Debug("Lua Version: {Version}", lua_version());
        
        
        // If you want to run the following code on the main thread instead
        // await Dispatcher.EnsureRunningOnMainThread();

        ExecuteInlineLua();
        RegisterCustomFunctionInVM();
        RunFileExample();
        return Task.CompletedTask;
    }
}