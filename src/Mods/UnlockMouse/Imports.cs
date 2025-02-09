using System.Runtime.CompilerServices;

[assembly: DisableRuntimeMarshalling]
namespace Dawn.DarkCrusade.Mods.UnlockMouse;

using System.Runtime.CompilerServices;
using global::Serilog;
using Vanara;

internal static partial class Plat
{
    internal static partial class Input
    {
        internal static void EnableCursorClip(bool enable)
        {
            Log.Debug("EnableCursorClip({Enable})", enable);
            Internal_EnableCursorClip(enable);
        }

        // void __stdcall Plat::Input::EnableCursorClip(bool)
        [LibraryImport("Platform", EntryPoint = "?EnableCursorClip@Input@Plat@@YGX_N@Z")]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        private static partial void Internal_EnableCursorClip(BOOL enable);
    }
}