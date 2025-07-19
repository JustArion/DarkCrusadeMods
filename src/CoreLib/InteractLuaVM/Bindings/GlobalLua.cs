// ReSharper disable UnusedMember.Global

using System.Diagnostics.CodeAnalysis;

namespace Dawn.DarkCrusade.InteractLuaVM;

[SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible")]
public static partial class GlobalLua
{
    internal const string DLL_NAME = "LuaConfig";

    [LibraryImport(DLL_NAME, EntryPoint = "?Initialize@GlobalLua@@SGXXZ")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [Obsolete("This function should not be called, the game normally calls this function")]
    public static partial void Initialize();
    
    [LibraryImport(DLL_NAME, EntryPoint = "?GetState@GlobalLua@@SGPAVLuaConfig@@XZ")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial LuaConfig GetState();
    
    [LibraryImport(DLL_NAME, EntryPoint = "?Shutdown@GlobalLua@@SGXXZ")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [Obsolete("This function should not be called, the game normally calls this function")]
    public static partial void Shutdown();
}