namespace Dawn.AOT.CoreLib.X86.Config;

using global::Serilog;
using global::Serilog.Core;

public class ConfigWatcher(FileInfo file) : IDisposable
{
    private readonly FileSystemWatcher _watcher = new();
    private readonly ILogger _logger = Log.ForContext<ConfigWatcher>();

    public void Start()
    {
        _watcher.Path = file.DirectoryName!;
        _watcher.Filter = file.Name;
        _watcher.NotifyFilter = NotifyFilters.LastWrite;
        _watcher.Changed += delegate
        {
            _logger.Debug("File Change for {FilePath}", file.Name);
            Changed?.Invoke(this, file);
        };
        _watcher.EnableRaisingEvents = true;
        _watcher.Error += (_, args) => { Log.Error(args.GetException(), "File Watcher Error"); };
        _logger.Information("WatchDog active on {FilePath}. Watching for config changes", file.Name);
    }

    public void Stop()
    {
        _watcher.EnableRaisingEvents = false;
    }
    
    public event EventHandler<FileInfo>? Changed;
    
    public void Dispose()
    {
        _watcher.Dispose();
        GC.SuppressFinalize(this);
    }
}