namespace Dawn.DarkCrusade.ModLoader.Version;

using System.Reflection;

internal static class Imports
{
    internal const string VERSION_KEY = "VERSION_IMPORT";

    internal static nint Resolve(string libraryname, Assembly assembly, DllImportSearchPath? searchpath)
    {
        if (libraryname != VERSION_KEY)
            return 0;
        
        if (!OperatingSystem.IsWindows()) 
            throw new PlatformNotSupportedException();
        
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "version.dll");

        return NativeLibrary.Load(path);
    }
    
}