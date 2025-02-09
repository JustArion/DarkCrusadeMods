namespace Dawn.DarkCrusade.Mods.UnlockMouse.Config;

using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(ModConfig))]
[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
public partial class ConfigSourceGenerator : JsonSerializerContext;