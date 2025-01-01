namespace Dawn.DarkCrusade.ModLoader.Config;

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

internal static class ModLoaderConfig
{
    private const string MOD_CONFIG_FILE_NAME = "LoaderSettings.json";

    public static LoaderConfig FetchConfig(DirectoryInfo directory)
    {
        var configTypeInformation = ConfigSourceGenerator.Default.LoaderConfig;
        var configFile = new FileInfo(Path.Combine(directory.FullName, MOD_CONFIG_FILE_NAME));

        if (!configFile.Exists)
            return CreateNewConfigFile(configFile, configTypeInformation);
        
        var configJson = File.ReadAllText(configFile.FullName);
        var modConfig = JsonSerializer.Deserialize(configJson, configTypeInformation);

        if (modConfig != null)
            return modConfig;

        Logging.InitializeConsole();
        Log.Error("Unable to read config, it may be corrupted. Config: {ConfigJson}", configJson);

        return CreateNewConfigFile(configFile, configTypeInformation);
    }
    

    private static LoaderConfig CreateNewConfigFile(FileInfo configFile, JsonTypeInfo<LoaderConfig> sourceGenerator)
    {
        Log.Information("Creating a new config file");
        var config = new LoaderConfig();
        config.LoadOrder.Add("ExampleMod.dll", 0);
        config.LoadOrder.Add("ExampleMod", 0);
                
        SafeConfigWriter.WriteTo(JsonSerializer.Serialize(config, sourceGenerator), configFile.FullName);

        return config;
    }
}