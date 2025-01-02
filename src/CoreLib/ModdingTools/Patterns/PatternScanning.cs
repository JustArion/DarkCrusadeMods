namespace Dawn.DarkCrusade.ModdingTools.Patterns;

using Reloaded.Memory.Sigscan;
using Reloaded.Memory.Sigscan.Definitions.Structs;

public static unsafe class PatternScanning
{
    public static PatternScanResult TryFindPatternSse2(this GameMemory game, string pattern)
    {
        var result = Scanner.FindPatternSse2((byte*)game.BaseAddress, game.Memory.Length, pattern);

        return result;
    }
}