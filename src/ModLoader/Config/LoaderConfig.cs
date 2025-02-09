namespace Dawn.DarkCrusade.ModLoader.Config;

public record LoaderConfig
{
    /// <summary>
    /// The delay in milliseconds between startup and when mods start loading
    /// </summary>
    public double LoadDelaySeconds = 5;
    
    /// <summary>
    /// A Console will be attached or created 
    /// </summary>
    public bool UseConsole = false;

    public Dictionary<string, short> LoadOrder = new();
}
