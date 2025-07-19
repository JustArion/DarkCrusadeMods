namespace Dawn.DarkCrusade.ModLoader.Mods;

using System.Diagnostics;
using System.Security.Cryptography;

internal static class Heuristics
{
    public static IReadOnlyDictionary<string, short> LoadOrder => _loadOrder;
    
    private static readonly Dictionary<string, short> _loadOrder = new();
    private static readonly FileInfo _currentModule = new(GetModuleFileName(EntryPoint.LoaderInfo.Module));
    public static DirectoryInfo CurrentModuleDirectory => _currentModule.Directory!;

    // private static DirectoryInfo ModFolder => Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "mods"));
    public static DirectoryInfo ModsFolder => Directory.CreateDirectory(Path.Combine(_currentModule.Directory!.FullName, "mods"));

    public static string ModFolderPath => ModsFolder.FullName;
    public static List<FileInfo> GetMods() => ModsFolder.EnumerateFiles("*.dll", SearchOption.AllDirectories).ToList();

    public static FileInfo[] GetOrderedMods()
    {
        Log.Verbose("Looking for mods in {ModFolder}", ModsFolder.FullName);
        var mods = GetMods();
        var modSHAs= new Dictionary<string, FileInfo>();

        var modsToRemove = new List<FileInfo>();
        // We build the load order of the unfaulted mods
        foreach (var mod in mods)
        {
            if (IsFaulted(mod, modSHAs))
                modsToRemove.Add(mod);
            else
                _loadOrder.TryAdd(mod.Name, 0);
        }
        
        foreach (var mod in modsToRemove)
            mods.Remove(mod);
        
        foreach (var modLoadOrder in _loadOrder)
        {
            var mod = mods.FirstOrDefault(m => m.Name == modLoadOrder.Key || m.Name == StripExtension(modLoadOrder.Key));
            if (mod is null)
                continue;

            _loadOrder[mod.Name] = modLoadOrder.Value;
        }

        return mods.OrderByDescending(mod => _loadOrder[mod.Name]).ToArray();
    }
    
    private static string StripExtension(string name) => name.Replace(".dll", string.Empty);
    
    internal static bool IsFaulted(FileInfo mod, Dictionary<string, FileInfo> modSHAs)
    {
        try
        {
            if (!File.Exists(mod.FullName))
            {
                Log.Warning("Mod {ModName} was possibly removed during loading", mod.Name);
                return true;
            }

            if (!FeatureFlags.FilterDuplicateMods) 
                return false;
            
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(mod.FullName);
            var hash = sha256.ComputeHash(stream);
            var hashString = Convert.ToBase64String(hash);

            var modDirectoryPath = ModsFolder.FullName;
            
            if (modSHAs.Any(h => h.Key == hashString))
            {
                var hashedModRelativePath = modSHAs.First(h => h.Key == hashString).Value.FullName
                    .Replace(modDirectoryPath, string.Empty);
                
                var currentModRelativePath = mod.FullName.Replace(modDirectoryPath, string.Empty);

                Log.Warning("Duplicate mod found: {ModName}", FileToModName(mod));
                Log.Warning("Duplicates are {HashedModName} and {CurrentModName}",
                    _modsRoot + hashedModRelativePath,
                    _modsRoot + currentModRelativePath);
                return true;
            }

            modSHAs.TryAdd(hashString, mod);
            return false;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error verifying duplicate for mod {ModName}", mod.Name);
            return true;
        }
    }
    
    public static string FileToModName(FileInfo file)
    {
        var defaultModName = file.Name.Replace(".dll", string.Empty);
        try
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(file.FullName);

            return string.IsNullOrEmpty(versionInfo.ProductName) 
                ? defaultModName 
                : string.IsNullOrEmpty(versionInfo.FileVersion) 
                    ? versionInfo.ProductName 
                    : $"{versionInfo.ProductName} ({versionInfo.FileVersion})";
        }
        catch (Exception e)
        {
            Log.Debug(e, "Error trying to get Mod Name for {Mod}", file.Name);
            return defaultModName;
        }
    }
    
    private static readonly string _modsRoot = Path.DirectorySeparatorChar + "mods";
}