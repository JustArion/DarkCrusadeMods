namespace DummyDLL;

using System.Diagnostics;
using Dawn.AOT.CoreLib.X86;

internal static class EntryPoint
{
    [UnmanagedCallersOnly(EntryPoint = nameof(Init))]
    public static unsafe void Init(LoaderInformation* loaderInfo) // BootstrapInformation*
    {
        Console.WriteLine("Dummy dll checking in!");
    }
}