namespace Dawn.DarkCrusade.ModLoader.Config;

using System.Text.Json.Serialization;

[JsonSerializable(typeof(LoaderConfig))]
[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
public partial class ConfigSourceGenerator : JsonSerializerContext;