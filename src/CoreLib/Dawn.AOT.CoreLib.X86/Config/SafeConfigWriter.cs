namespace Dawn.AOT.CoreLib.X86.Config;

using global::Serilog;

public static class SafeConfigWriter
{
    public static void WriteTo(string content, string path)
    {
        try
        {
            var tempPath = Path.GetTempFileName();
        
            File.WriteAllText(tempPath, content);
        
            File.Move(tempPath, path, true);
        }
        catch (Exception e)
        {
            Log.Error(e, "IO Error");
        }
    }
}