namespace Dawn.DarkCrusade.ModdingTools;

using System.Diagnostics;

public unsafe ref struct GameMemory
{
    public required nint BaseAddress { get; init; }
    public required Span<byte> Memory { get; init; }

    public static GameMemory ForProcess(Process proc)
    {
        var module = proc.MainModule!;
        var baseAddress = module.BaseAddress;
        
        return new GameMemory
        {
            BaseAddress = baseAddress,
            Memory = new(baseAddress.ToPointer(), module.ModuleMemorySize)
        };
    }
}