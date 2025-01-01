namespace Dawn.DarkCrusade.ModLoader;

using System.Diagnostics.CodeAnalysis;

internal static class FeatureFlags
{
    [FeatureSwitchDefinition("ModLoader.Features.FilterDuplicates")]
    internal static bool FilterDuplicateMods =>
        !AppContext.TryGetSwitch("ModLoader.Features.FilterDuplicates", out var enabled) || enabled;
}