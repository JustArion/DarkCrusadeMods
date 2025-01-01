namespace DummyDLL;

using System.Diagnostics;

internal static class EntryPoint
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct BootstrapInformation
    {
        public readonly nint HINSTANCE;
        public readonly uint MainThreadId;
        public readonly int MainThreadPriority;
    }
    internal static BootstrapInformation _loaderInfo;
    
    [UnmanagedCallersOnly(EntryPoint = nameof(Init))]
    public static unsafe void Init(BootstrapInformation* loaderInfo) // BootstrapInformation*
    {
        Console.WriteLine("Dummy dll checking in!");
    }
}