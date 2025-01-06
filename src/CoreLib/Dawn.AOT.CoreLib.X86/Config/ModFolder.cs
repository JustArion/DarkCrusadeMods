namespace Dawn.AOT.CoreLib.X86.Config;

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using global::Serilog;
using FileAccess = System.IO.FileAccess;

public class ModFolder
{
    public ModFolder(HINSTANCE hInstance)
    {
        var fileInfo = new FileInfo(GetModuleFileName(hInstance));
        var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);

        ModDirectory = fileInfo.Directory!.CreateSubdirectory(fileName);
    }
    public DirectoryInfo ModDirectory { get; }

    public FileInfo? GetFile(string fileName)
    {
        var retVal =  ModDirectory.GetFiles(fileName).FirstOrDefault();

        // We try the parent if there's no files here
        return retVal ?? ModDirectory.Parent!.GetFiles(fileName).FirstOrDefault();
    }
}
public class ModFolder<TConfig> : ModFolder where TConfig : class, new()
{
    private ConfigWatcher _configWatcher = null!;
    private readonly JsonTypeInfo<TConfig> _configInfo;
    private TConfig _config = null!;

    public ModFolder(HINSTANCE hInstance, JsonTypeInfo<TConfig> sourceGenerator) : base(hInstance)
    {       
        _configInfo = sourceGenerator;
        InitializeConfig();
    }

    public TConfig Config => _config;
    public event Action? ConfigUpdated;


    private const string MOD_CONFIG_FILE_NAME = "ModConfig.json";
    
    private void InitializeConfig()
    {
        

        var configFile = new FileInfo(Path.Combine(ModDirectory.FullName, MOD_CONFIG_FILE_NAME));

        if (configFile.Exists)
        {
            var configJson = File.ReadAllText(configFile.FullName);
            var modConfig = JsonSerializer.Deserialize(configJson, _configInfo);
            
            if (modConfig != null)
            {
                _config = modConfig;
            }
            else
            {
                Log.Error("Unable to read config, it may be corrupted. Config: {ConfigJson}", configJson);
                CreateNewConfigFile(configFile, out _config, _configInfo);
            }
        }
        else
            CreateNewConfigFile(configFile, out _config, _configInfo);
        
        _configWatcher = new ConfigWatcher(configFile);
        _configWatcher.Changed += OnConfigNeedsUpdate;
        _configWatcher.Start();
    }

    private void OnConfigNeedsUpdate(object? _, FileInfo info)
    {
        try
        {
            // Allow multiple reads
            using var hConfigFile = File.Open(info.FullName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write | FileShare.Delete);
            
            using var reader = new StreamReader(hConfigFile);
            var configJson = reader.ReadToEnd();

            TConfig? modConfig = null;

            try
            {
                modConfig = JsonSerializer.Deserialize(configJson, _configInfo);
            }
            catch {}


            if (modConfig == null)
                Log.Warning("Unable to read config, it may be corrupted. Config: {ConfigJson}", info.FullName);
            else
            {
                _config = modConfig;
                ConfigUpdated?.Invoke();
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Config Watcher Error");
        }
    }

    private static void CreateNewConfigFile(FileInfo configFile, out TConfig config, JsonTypeInfo<TConfig> sourceGenerator)
    {
        Log.Information("Creating a new config file");
        config = new();
                
        SafeConfigWriter.WriteTo(JsonSerializer.Serialize(config, sourceGenerator), configFile.FullName);
    }

    public FileInfo? GetFile(string fileName)
    {
        return ModDirectory.GetFiles(fileName).FirstOrDefault();
    }
}