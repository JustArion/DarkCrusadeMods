namespace Dawn.AOTCoreLib.ASM.Hooking;

using System.Runtime.CompilerServices;

public static unsafe class MemoryToolbelt
{
    public static void CopyTo<T>(nint dst, T strukt) where T : unmanaged
    {
        if (dst == nint.Zero)
                throw new ArgumentException("Destination address cannot be zero.");

        CopyTo(dst.ToPointer(), &strukt, sizeof(T));
    }
    public static void CopyTo(nint dst, nint src, int size)
    {
        if (dst == nint.Zero)
            throw new ArgumentException("Destination address cannot be zero.");
        
        if (src == nint.Zero)
            throw new ArgumentException("Source address cannot be zero.");

        CopyTo(dst.ToPointer(), src.ToPointer(), size);
    }
    public static void CopyTo(void* dst, void* src, int size) => Unsafe.CopyBlock(dst, src, (uint)size);
    public static bool ChangeMemoryProtection(void* value, MEM_PROTECTION memProtection, out MEM_PROTECTION oldProtection) => VirtualProtect((nint)value, 1, memProtection, out oldProtection);
    public static bool ChangeMemoryProtection(void* value, MEM_PROTECTION memProtection, SizeT size, out MEM_PROTECTION oldProtection) => VirtualProtect((nint)value, size, memProtection, out oldProtection);

    public static (MEM_PROTECTION Protect, MEM_ALLOCATION_TYPE State) QueryAddress(nint address)
    {
        var arr = new MEMORY_BASIC_INFORMATION[1];
        if (VirtualQuery(address, arr, sizeof(MEMORY_BASIC_INFORMATION)) == 0)
            throw GetLastError().GetException()!;
        
        var mi = arr[0];
        var state = (MEM_ALLOCATION_TYPE)mi.State;
        var protect = (MEM_PROTECTION)mi.Protect;

        return (protect, state);
    }
    public static (bool CanRead, bool CanWrite) CanReadWriteMemory(nint address)
    {
        var (protect, state) = QueryAddress(address);

        if (state != MEM_ALLOCATION_TYPE.MEM_COMMIT)
            return (false, false);

        var canWrite = (protect & Page_Write_Flags) != 0;
        
        var canRead = (protect & Page_Read_Flags) != 0;
        
        return (canRead, canWrite);
    }

    private const MEM_PROTECTION Page_Execute_Flags = MEM_PROTECTION.PAGE_EXECUTE |
                                                      MEM_PROTECTION.PAGE_EXECUTE_READ |
                                                      MEM_PROTECTION.PAGE_EXECUTE_READWRITE |
                                                      MEM_PROTECTION.PAGE_EXECUTE_WRITECOPY;

    private const MEM_PROTECTION Page_Read_Flags = MEM_PROTECTION.PAGE_READONLY |
                                                   MEM_PROTECTION.PAGE_READWRITE |
                                                   MEM_PROTECTION.PAGE_EXECUTE_READ |
                                                   MEM_PROTECTION.PAGE_EXECUTE_READWRITE |
                                                   MEM_PROTECTION.PAGE_EXECUTE_WRITECOPY |
                                                   MEM_PROTECTION.PAGE_WRITECOPY;

    private const MEM_PROTECTION Page_Write_Flags = MEM_PROTECTION.PAGE_READWRITE |
                                                    MEM_PROTECTION.PAGE_EXECUTE_READWRITE |
                                                    MEM_PROTECTION.PAGE_EXECUTE_WRITECOPY;
    public static bool CanReadMemoryAt(nint address) => CanReadWriteMemory(address).CanRead;

    public static bool CanWriteMemoryAt(nint address) => CanReadWriteMemory(address).CanWrite;

    public static bool IsExecutableMemory(nint address)
    {
        var arr = new MEMORY_BASIC_INFORMATION[1];
        VirtualQuery(address, arr, sizeof(MEMORY_BASIC_INFORMATION) * arr.Length);

        var mi = arr[0];
        var state = (MEM_ALLOCATION_TYPE)mi.State;
        var protection = (MEM_PROTECTION)mi.Protect;
        return state == MEM_ALLOCATION_TYPE.MEM_COMMIT &&
               (protection & Page_Execute_Flags) != 0;
    }
    
    public static bool free(ref void* ptr, int size)
    {
        var retVal = free(ref Unsafe.AsRef<nint>(ptr), size);
        
        if (retVal)
            ptr = null;
        
        return retVal;
    }
    public static bool free(void* ptr, int size) => free((nint)ptr, size);

    public static bool free<T>(ref T* ptr) where T : unmanaged
    {
        var retVal = free(ref Unsafe.AsRef<nint>(ptr), sizeof(T));
        
        if (retVal)
            ptr = null;
        
        return retVal;
    }
    
    public static bool free<T>(T* ptr) where T : unmanaged => free((nint)ptr, sizeof(T));

    public static bool free(ref nint ptr, int size)
    {
        var retVal = VirtualFree(ptr, size, MEM_ALLOCATION_TYPE.MEM_RELEASE);
        
        if (retVal)
            ptr = nint.Zero;
        
        return retVal;
    }
    public static bool free(nint ptr, int size) => VirtualFree(ptr, size, MEM_ALLOCATION_TYPE.MEM_RELEASE);

}