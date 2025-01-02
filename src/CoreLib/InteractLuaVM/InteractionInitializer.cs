namespace InteractLuaVM;

using System.Buffers.Binary;
using System.Diagnostics;
using Dawn.DarkCrusade.ModdingTools;
using Dawn.DarkCrusade.ModdingTools.Collections;
using Dawn.DarkCrusade.ModdingTools.Patterns;
using Reloaded.Memory.Exceptions;
using Reloaded.Memory.Pointers;
using Serilog;

public sealed unsafe class InteractionInitializer
{
    public static readonly InteractionInitializer Instance = new();
    public Ptr<nint> PtrLuaConfigPtr;

    internal InteractionInitializer()
    {
        var memory = GameMemory.ForProcess(Process.GetCurrentProcess());

        var result = memory.TryFindPatternSse2(Patterns.LUA_CONFIG_STATIC_POINTER_PATTERN);

        if (!result.Found)
        {
            Log.Fatal("Failed to find the Lua config pointer");
            throw new MemoryException("Failed to find the Lua config pointer");
        }

        // 0x076aa76  a3f838a100         mov     dword [data_a138f8], eax
        // We skip the 'mov' instruction
        result.AddOffsetFixed(1);
        
        var codeAddress = nint.Add(memory.BaseAddress, result.Offset);
        Log.Debug("Found Lua Config Pointer in Code at address 0x{Address:X}", codeAddress);

        
        // This should be 0xA138F8
        var address = BitConverter.IsLittleEndian 
            ? BinaryPrimitives.ReadIntPtrLittleEndian(new(codeAddress.ToPointer(), nint.Size)) 
            : BinaryPrimitives.ReadIntPtrBigEndian(new(codeAddress.ToPointer(), nint.Size));

        PtrLuaConfigPtr.Pointer = (nint*)address;
        Log.Debug("Lua Config Pointer is at address 0x{Address:X}, Value is 0x{Value:X}", address, PtrLuaConfigPtr.Get());
    }
}