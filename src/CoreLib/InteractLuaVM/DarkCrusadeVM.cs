namespace Dawn.DarkCrusade.InteractLuaVM;

using System.Buffers.Binary;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using ModdingTools;
using Dawn.DarkCrusade.ModdingTools.Patterns;
using global::Serilog;
using Reloaded.Memory.Exceptions;
using Reloaded.Memory.Pointers;
using Serilog;

public static class DarkCrusadeVM
{
    static DarkCrusadeVM()
    {
        _luaConfig = GlobalLua.GetState();
        Log.Debug("Lua Config Pointer: 0x{Address:X}", _luaConfig.Handle);
    }
    
    public static lua_State GetState() => _luaConfig.GetState();
    private static LuaConfig _luaConfig;
}