namespace Dawn.DarkCrusade.InteractLuaVM;

public static partial class Laux
{
    internal const string DLL_NAME = "LuaConfig";
    
    // void (* const LuaConfig:luax_print(struct lua_State*, char const*)
    [LibraryImport(DLL_NAME, EntryPoint = "?luax_print@@YAXPAUlua_State@@PBDZZ", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void luax_print(lua_State luaState, string message);
}