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
    // private static void Initialize()
    // {
    //     // var memory = GameMemory.ForProcess(Process.GetCurrentProcess());
    //     //
    //     // var result = memory.TryFindPatternSse2(Patterns.LUA_CONFIG_STATIC_POINTER_PATTERN);
    //     //
    //     // if (!result.Found)
    //     // {
    //     //     Log.Fatal("Failed to find the Lua config pointer");
    //     //     throw new MemoryException("Failed to find the Lua config pointer");
    //     // }
    //     //
    //     // // 0x076aa76  a3f838a100         mov     dword [data_a138f8], eax
    //     // // We skip the 'mov' instruction
    //     // result.AddOffsetFixed(1);
    //     //
    //     // var codeAddress = nint.Add(memory.BaseAddress, result.Offset);
    //     // Log.Debug("Found Lua Config Pointer in Code at address 0x{Address:X}", codeAddress);
    //     //
    //     //
    //     // // This should be 0xA138F8
    //     // var address = BitConverter.IsLittleEndian 
    //     //     ? BinaryPrimitives.ReadIntPtrLittleEndian(new(codeAddress.ToPointer(), nint.Size)) 
    //     //     : BinaryPrimitives.ReadIntPtrBigEndian(new(codeAddress.ToPointer(), nint.Size));
    //     //
    //     // // PtrLuaConfigPtr.Pointer = (lua_State*)address;
    //     // _luaConfig = address;
    //
    //     // var luaConfig = LoadLibrary("LuaConfig");
    //     //
    //     // var s_luaState = (nint*)luaConfig.GetProcAddress("?s_luaState@GlobalLua@@0PAVLuaConfig@@A");
    //     //
    //     // _luaConfig = *s_luaState;
    // }
}