namespace Dawn.DarkCrusade.Mods.UnlockMouse;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using AOT.CoreLib.X86;
using AOT.CoreLib.X86.Config;
using AOT.CoreLib.X86.Logging;
using Config;
using global::Serilog;
using static WindowStyles;
internal static class EntryPoint
{
    internal static LoaderInformation _loaderInfo;
    private static ModFolder<ModConfig> _modFolder = null!;
    
    [UnmanagedCallersOnly(EntryPoint = nameof(Init))]
    public static void Init(nint loaderInfo) // BootstrapInformation*
    {
        _loaderInfo = LoaderInformation.FromPointer(loaderInfo); 
        Task.Run(() =>
        {
            try
            {
                ModLogging.Initialize(_loaderInfo.Module);
                _modFolder = new(_loaderInfo.Module, ConfigSourceGenerator.Default.ModConfig);
                
                DllMain();
            }
            catch (Exception e)
            {
                try
                {
                    Log.Fatal(e, "Fatal Error");
                    Log.CloseAndFlush();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(new AggregateException(e, ex));
                }
            }
        });
    }


    private static void DllMain()
    {
        _modFolder.ConfigUpdated += OnConfigUpdated;

        if (!_modFolder.Config.Enabled) 
            return;
        
        Plat.Input.EnableCursorClip(false);
    }

    private static void OnConfigUpdated() => Plat.Input.EnableCursorClip(!_modFolder.Config.Enabled);
}