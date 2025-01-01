namespace Dawn.DarkCrusade.ModLoader.Version;

internal static unsafe partial class Exports
{
    // GetFileVersionInfoByHandle
    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoByHandle))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetFileVersionInfoByHandle(
        uint dwFlags,
        nint hFile,
        nint lplpData,
        uint* pdwLen);
    
    
    // GetFileVersionInfo
    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoA))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetFileVersionInfoA(
        nint lpcstrFilename,
        uint dwHandle,
        uint dwLen,
        nint* lpData);
    
    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoW))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetFileVersionInfoW(
        nint lpcstrFilename,
        uint dwHandle,
        uint dwLen,
        nint* lpData);

    
    // GetFileVersionInfoSize
    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoSizeA))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
	public static partial uint GetFileVersionInfoSizeA(
        nint lpcstrFilename,
        uint* lpdwHandle);
    
    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoSizeW))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    public static partial uint GetFileVersionInfoSizeW(
        nint lptstrFilename,
        uint* lpdwHandle);
    
    
    // GetFileVersionInfoSizeEx
    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoSizeExA))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    public static partial uint GetFileVersionInfoSizeExA(
        uint dwFlags,
        nint lpcstrFilename,
        uint* lpdwHandle);
    
    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoSizeExW))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    public static partial uint GetFileVersionInfoSizeExW(
        uint dwFlags,
        nint lpcstrFilename,
        uint* lpdwHandle);
    
    
    // GetFileVersionInfoEx
    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoExA))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetFileVersionInfoExA(
        uint dwFlags,
        nint lpcstrFilename,
        uint dwHandle,
        uint dwLen,
        nint* lpData);
    
    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoExW))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetFileVersionInfoExW(
        uint dwFlags,
        nint lpcstrFilename,
        uint dwHandle,
        uint dwLen,
        nint* lpData);
    
    
    // VerFindFile
    [UnmanagedCallersOnly(EntryPoint = nameof(VerFindFileA))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    public static partial uint VerFindFileA(
        uint uFlags, nint szFileName,
        [Optional] nint szWinDir,
        nint szAppDir,
        nint* szCurDir,
        uint* puCurDirLen,
        nint* szDestDir,
        uint* puDestDirLen);
    
    [UnmanagedCallersOnly(EntryPoint = nameof(VerFindFileW))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    public static partial uint VerFindFileW(
        uint uFlags,
        nint szFileName,
        [Optional] nint szWinDir,
        nint szAppDir,
        nint* szCurDir,
        uint* puCurDirLen,
        nint* szDestDir,
        uint* puDestDirLen);
    
    
    // VerInstallFile
    [UnmanagedCallersOnly(EntryPoint = nameof(VerInstallFileA))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    public static partial uint VerInstallFileA(
        uint uFlags,
        nint szSrcFileName,
        nint szDestFileName,
        nint szSrcDir,
        nint szDestDir,
        nint szCurDir,
        nint* szTmpFile,
        uint* puTmpFileLen);
    
    [UnmanagedCallersOnly(EntryPoint = nameof(VerInstallFileW))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    public static partial uint VerInstallFileW(
        uint uFlags,
        nint szSrcFileName,
        nint szDestFileName,
        nint szSrcDir,
        nint szDestDir,
        nint szCurDir,
        nint* szTmpFile,
        uint* puTmpFileLen);
    
    
    // VerLanguageName
    [UnmanagedCallersOnly(EntryPoint = nameof(VerLanguageNameA))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    public static partial uint VerLanguageNameA(
        uint wLang,
        nint* szLang,
        uint cchLang);
    
    [UnmanagedCallersOnly(EntryPoint = nameof(VerLanguageNameW))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    public static partial uint VerLanguageNameW(
        uint wLang,
        nint* szLang,
        uint cchLang);
    
    
    // VerQueryValue
    [UnmanagedCallersOnly(EntryPoint = nameof(VerQueryValueA))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool VerQueryValueA(
        nint pBlock,
        nint lpSubBlock,
        nint* lplpBuffer,
        uint* puLen);
    
    [UnmanagedCallersOnly(EntryPoint = nameof(VerQueryValueW))]
    [LibraryImport(Imports.VERSION_KEY, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool VerQueryValueW(
        nint pBlock,
        nint lpSubBlock,
        nint* lplpBuffer,
        uint* puLen);
}