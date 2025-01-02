namespace Dawn.DarkCrusade.ModdingTools.Patterns;

using Reloaded.Memory.Sigscan.Definitions.Structs;

public static class PatternScanResultEx
{
    public static void AddOffsetFixed(ref this PatternScanResult result, int offset)
    {
        result = result.AddOffset(offset);
    }
}