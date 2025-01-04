namespace Dawn.DarkCrusade.InteractLuaVM;

using System.Diagnostics.CodeAnalysis;
using System.Text;

public partial struct LuaConfig
{
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public ref struct LuaFunc
    {
        public Lua.lua_CFunction Function;
        public int UpvalueCount;
        public nint LightUserData;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct _LuaFunc
    {
        public required nint Function;
        public int UpvalueCount;
        public nint LightUserData;
    }

    private static nint HandleNullFor(Lua.lua_CFunction? fn)
    {
        return fn == null 
            ? 0 
            : Marshal.GetFunctionPointerForDelegate(fn);
    }
    internal const string DLL_NAME = "LuaConfig";
        
    [SuppressMessage("ReSharper", "UnassignedField.Global")] 
    public required nint Handle;
    
    public bool RunString(string str, bool b) => RunString(Handle, str, b);
    public lua_State GetState() => GetState(Handle);
    
    public void SetNumber(string key, double value) => SetNumber(Handle, key, value);
    
    public void SetBoolean(string key, bool value) => SetBoolean(Handle, key, value);
    
    public void ClearVariable(string key) => ClearVariable(Handle, key);

    public void BindVar(string key, Lua.lua_CFunction fn1, Lua.lua_CFunction fn2)
    {
        BindVar(key, new LuaFunc
        {
            Function = fn1
        }, new LuaFunc
        {
            Function = fn2
        });
    }
    public void BindVar(string key, LuaFunc fn1 = default, LuaFunc fn2 = default)
    {
        var func1 = new _LuaFunc
        {
            Function = HandleNullFor(fn1.Function), 
            UpvalueCount = fn1.UpvalueCount,
            LightUserData = fn1.LightUserData
        };
        var func2 = new _LuaFunc
        {
            Function = HandleNullFor(fn2.Function),
            UpvalueCount = fn2.UpvalueCount,
            LightUserData = fn2.LightUserData
        };
        BindVar(Handle, key, func1, func2);
    }
    public void BindConstant(string key, Lua.lua_CFunction fn)
    {
        BindVar(key, new LuaFunc { Function = fn });
    }
    public void BindConstant(string key, LuaFunc fn1) => BindVar(key, fn1);

    // public bool GetString(string key, out string str)
    // {
    //     return 
    // }
    
    public void SetString(string key, string value) => SetString(Handle, key, value);

    public void RegisterFunction(string name, Lua.lua_CFunction fn)
    {
        RegisterCFunc(Handle, name, new _LuaFunc
        {
            Function = HandleNullFor(fn)
        });
        // int __stdcall Function(lua_State)
        // delegate* unmanaged[Stdcall]<lua_State, int> func;
    }

    public void RegisterFunction(string name, LuaFunc func)
    {
        RegisterCFunc(Handle, name, new _LuaFunc
        {
            Function = HandleNullFor(func.Function),
            UpvalueCount = func.UpvalueCount,
            LightUserData = func.LightUserData 
        });
    }
    
    // void (__thiscall* LuaConfig::RegisterCFunc(LuaConfig* this, char const*, struct LuaConfig::LuaFunc)
    [LibraryImport(DLL_NAME, EntryPoint = "?RegisterCFunc@LuaConfig@@QAEXPBDULuaFunc@1@@Z", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
    private static partial void RegisterCFunc(nint @this, string name, _LuaFunc func);
    
    // void (__thiscall* const LuaConfig:LuaConfig::SetString(LuaConfig* this, char const*, char const*)
    [LibraryImport(DLL_NAME, EntryPoint = "?SetString@LuaConfig@@QAEXPBD0@Z", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
    private static partial void SetString(nint @this, string key, string value);
    
    // bool __thiscall LuaConfig::GetString(LuaConfig* this, char const*, char*, uint32_t)
    [LibraryImport(DLL_NAME, EntryPoint = "?GetString@LuaConfig@@QAE_NPBDPADI@Z", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool GetString(nint @this, string key, out string str, uint length);
    
    // void (__thiscall* const LuaConfig:LuaConfig::BindVar(LuaConfig* this, char const*, struct LuaConfig::LuaFunc, struct LuaConfig::LuaFunc)
    [LibraryImport(DLL_NAME, EntryPoint = "?BindVar@LuaConfig@@QAEXPBDPAU1@Z0@Z", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
    private static partial void BindVar(nint @this, string key, _LuaFunc func, _LuaFunc func2);
    
    // void (__thiscall* const LuaConfig:LuaConfig::ClearVariable(LuaConfig* this, char const*)
    [LibraryImport(DLL_NAME, EntryPoint = "?ClearVariable@LuaConfig@@QAEXPBD@Z", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
    private static partial void ClearVariable(nint @this, string key);
    
    // void (__thiscall* const LuaConfig:LuaConfig::SetBoolean(LuaConfig* this, char const*, bool)
    [LibraryImport(DLL_NAME, EntryPoint = "?SetBoolean@LuaConfig@@QAEXPBD_N@Z", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
    private static partial void SetBoolean(nint @this, string key, [MarshalAs(UnmanagedType.U1)] bool value);
        
    [LibraryImport(DLL_NAME, EntryPoint = "?GetState@LuaConfig@@QAEPAUlua_State@@XZ")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
    private static partial lua_State GetState(nint config);
    
    // bool (__thiscall* const LuaConfig:LuaConfig::RunString(LuaConfig* this, char const*, bool)
    [LibraryImport(DLL_NAME, EntryPoint = "?RunString@LuaConfig@@QAE_NPBD_N@Z", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool RunString(nint @this, string str, [MarshalAs(UnmanagedType.U1)] bool b);
    
    // void __thiscall LuaConfig::SetNumber(LuaConfig* this, char const*, double)
    [LibraryImport(DLL_NAME, EntryPoint = "?SetNumber@LuaConfig@@QAEXPBDN@Z", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvThiscall)])]
    private static partial void SetNumber(nint @this, string key, double value);
    
    // int __stdcall LuaConfig::Print(lua_State state)
    [LibraryImport(DLL_NAME, EntryPoint = "?Print@LuaConfig@@CGHPAUlua_State@@@Z")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private static partial int Print(lua_State state);
    
    // LuaConfig __stdcall GetLC(LuaConfig)
    [LibraryImport(DLL_NAME, EntryPoint = "?GetLC@LuaConfig@@SGPAV1@PAUlua_State@@@Z")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial LuaConfig GetLC(lua_State state);
}