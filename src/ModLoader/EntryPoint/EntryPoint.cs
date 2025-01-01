namespace Dawn.DarkCrusade.ModLoader;

using System.Reflection;
using AOT.CoreLib.X86;
using Version;

internal static class EntryPoint
{
    [UnmanagedCallersOnly(EntryPoint = nameof(Init))]
    internal static unsafe void Init(LoaderInformation* loaderInfo)
    {
        // MessageBox(0, "Hello from the mod loader!", "Hello");
        LoaderInfo = *loaderInfo;

        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), Imports.Resolve);
        // Main thread is frozen until the current function (Init) finishes
        Task.Run(Start.DllMain);
    }

    internal static LoaderInformation LoaderInfo { get; private set; }
}