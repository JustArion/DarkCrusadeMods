using System.Runtime.CompilerServices;
using Serilog;
using Vanara;

[assembly: DisableRuntimeMarshalling]
namespace PlatformBindings;

public static partial class Plat
{
    internal const string PLATFORM = "Platform";
    public static partial class Input
    {
        public static void EnableCursorClip(bool enable)
        {
            Log.Debug("EnableCursorClip({Enable})", enable);
            Internal_EnableCursorClip(enable);
        }
        
        public static bool IsComboKeyPressed(in Plat_ComboKey key, in Plat_InputEvent? evt)
        {
            return Internal_IsComboKeyPressed(in key, evt);
        }
        
        public static void ShowCursor()
        {
            Internal_ShowCursor();
        }
        
        public static void GetCursorPos(ref float x, ref float y)
        {
            Internal_GetCursorPos(ref x, ref y);
        }
        
        public static Plat_ComboKey GetComboKeyFromName(string name)
        {
            return Internal_GetComboKeyFromName(name);
        }
        
        public static void HideCursor()
        {
            Internal_HideCursor();
        }
        
        public static void SetCursorPos(float x, float y)
        {
            Internal_SetCursorPos(x, y);
        }
        

    // void __stdcall Plat::Input::EnableCursorClip(bool)
    [LibraryImport(PLATFORM, EntryPoint = "?EnableCursorClip@Input@Plat@@YGX_N@Z")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private static partial void Internal_EnableCursorClip(BOOL enable);

    // void __stdcall Plat::Input::GetCursorPos(float&, float&)(float&, float&)
    [LibraryImport(PLATFORM, EntryPoint = "?GetCursorPos@Input@Plat@@YGXAAM0@Z")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private static partial void Internal_GetCursorPos(ref float x, ref float y);

    // void __stdcall Plat::Input::ShowCursor()()
    [LibraryImport(PLATFORM, EntryPoint = "?ShowCursor@Input@Plat@@YGXXZ")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private static partial void Internal_ShowCursor();

    // bool __stdcall Plat::Input::IsComboKeyPressed(struct Plat::ComboKey const&, struct Plat::InputEvent const*)(struct Plat::ComboKey const&, struct Plat::InputEvent const*)
    [LibraryImport(PLATFORM, EntryPoint = "?IsComboKeyPressed@Input@Plat@@YG_NABUComboKey@2@PBUInputEvent@2@@Z")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private static partial bool Internal_IsComboKeyPressed(in Plat_ComboKey key, [MarshalAs(UnmanagedType.LPStruct)] Plat_InputEvent? evt);

    [LibraryImport(PLATFORM, EntryPoint = "?GetComboKeyFromName@Input@Plat@@YG?AUComboKey@12@PEBD@Z", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private static partial Plat_ComboKey Internal_GetComboKeyFromName(string name);

    [LibraryImport(PLATFORM, EntryPoint = "?HideCursor@Input@Plat@@YGXXZ")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private static partial void Internal_HideCursor();

    [LibraryImport(PLATFORM, EntryPoint = "?SetCursorPos@Input@Plat@@YGXMM@Z")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private static partial void Internal_SetCursorPos(float x, float y);
    }
}