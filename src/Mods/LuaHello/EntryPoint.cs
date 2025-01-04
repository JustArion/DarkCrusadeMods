namespace Dawn.DarkCrusade.Mods.BorderlessWindowed;

using System.Runtime.CompilerServices;
using AOT.CoreLib.X86;
using AOT.CoreLib.X86.Config;
using AOT.CoreLib.X86.Logging;
using AOT.CoreLib.X86.Threading;
using Config;
using global::Serilog;
using InteractLuaVM;

internal static unsafe class EntryPoint
{
    internal static LoaderInformation _loaderInfo;
    private static ModFolder<ModConfig> _modFolder = null!;
    
    [UnmanagedCallersOnly(EntryPoint = nameof(Init))]
    public static unsafe void Init(LoaderInformation* loaderInfo) // BootstrapInformation*
    {
        _loaderInfo = *loaderInfo;
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

    private static void DllMain()
    {
        

        Dispatcher.MainThread.Post(() =>
        {
            // var l = DarkCrusadeVM.GetState();

            // Log.Debug("lua_State* - 0x{Handle:X}", l.Handle);

            Log.Debug("Lua Version: {Version}", lua_version());

            // Push a string onto the Lua stack
            const string testValue = "Hello from C#!";
            
            GlobalLua.GetState().RunString($"print('{testValue}')", false);

            var state = GlobalLua.GetState();
            state.RegisterFunction(nameof(CallOut), CallOut);
            Log.Debug("Registered CallOut function");

            state.RunString("CallOut('Hello, Lua!')", false);

            // hook print



            // lua_register(l, nameof(CallOut), CallOut);
            // Log.Debug("Registered CallOut function");
            //
            // const string code = "CallOut('Hello, Lua!')";
            //
            // if (lua_dostring(l, code) == 0)
            //     return;
            //
            // var error = lua_tostring(l, -1);
            // Log.Error("LuaVM Exception: {Error}", new string(error));
            // lua_pop(l, 1);
        });
    }

    private static int CallOut(lua_State luaState)
    {
        var str = lua_tostring(luaState, 1);
        Log.Debug("DarkCrusadeVM: {Str}", str);
        return 0;
    }
}