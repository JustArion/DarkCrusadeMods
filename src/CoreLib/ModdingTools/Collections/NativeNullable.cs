// Don't this this is needed anymore. 
// Reloaded has a nice implementation of Ptr<> it seems

// namespace Dawn.DarkCrusade.ModdingTools.Collections;
//
// using Serilog;
//
// public unsafe struct NativeNullable<T> where T : unmanaged
// {
//     public T Value
//     {
//         get
//         {
//             if (!HasPointer)
//                 return default;
//             
//             if (CanRead)
//                 return *_value;
//             
//             return CantReadQueryAgain();
//         }
//         set
//         {
//             if (!HasPointer)
//                 return;
//
//             if (CanWrite)
//                 *_value = value;
//             else
//                 CantWriteQueryAgain(value);
//         }
//     }
//
//     private T CantReadQueryAgain()
//     {
//         (CanRead, CanWrite) = CanReadWriteMemory((nint)_value);
//         if (CanRead)
//             return *_value;
//
//         var memInfo = QueryAddress((nint)_value);
//         Log.Error("NativeNullable<{Type}> Prevented Application Crash, Unable to read memory at 0x{Pointer:X}, MemoryInfo: {Protect} | {State}", 
//             typeof(T).Name, (nint)_value, memInfo.Protect, memInfo.State);
//         return default;
//     }
//
//     private void CantWriteQueryAgain(T value)
//     {
//         (CanRead, CanWrite) = CanReadWriteMemory((nint)_value);
//         if (CanWrite)
//             *_value = value;
//         else
//         {
//             var memInfo = QueryAddress((nint)_value);
//             Log.Error("NativeNullable<{Type}> Prevented Application Crash, Unable to write memory at 0x{Pointer:X}, MemoryInfo: {Protect} | {State}",
//                 typeof(T).Name, (nint)_value, memInfo.Protect, memInfo.State);
//         }
//     }
//
//     /// <summary>
//     /// If the memory cannot be written to, tries to change the memory protection to be writable, writes to it, then resets the protection
//     /// </summary>
//     /// <param name="value"></param>
//     public void CoerceValue(T value)
//     {
//         if (!HasPointer)
//             throw new NullReferenceException("Tried to set a value to an object without an underlying reference");
//
//         (CanRead, CanWrite) = CanReadWriteMemory((nint)_value);
//         
//         if (CanWrite)
//         {
//             Value = value;
//             return;
//         }
//         
//         if (ChangeMemoryProtection(_value, MEM_PROTECTION.PAGE_EXECUTE_READWRITE, sizeof(T), out var oldProtection))
//         {
//             *_value = value;
//                 
//             ChangeMemoryProtection(_value, oldProtection, sizeof(T), out _);
//         }
//         else
//             Log.Error("NativeNullable<{Type}> Unable to Force Value, changing memory protection failed", nameof(T));
//     }
//
//     public bool CanRead { get; private set; }
//     public bool CanWrite { get; private set; }
//
//     public void SetPointer(nint value) => SetPointer(value.ToPointer());
//
//     public void SetPointer(void* value)
//     {        
//         _value = (T*)value;
//         HasPointer = true;
//
//         (CanRead, CanWrite) = CanReadWriteMemory((nint)value);
//
//         if (!CanRead) 
//             Log.Warning("NativeNullable<{Type}> Unable to read memory at 0x{Pointer:X}", typeof(T).Name, (nint)_value);
//         
//         if (!CanWrite)
//                 Log.Warning("NativeNullable<{Type}> Unable to write memory at 0x{Pointer:X}", typeof(T).Name, (nint)_value);
//     }
//     public bool HasPointer { get; private set; }
//
//     private T* _value;
//
//     public override string ToString()
//     {
//         return CanRead ? Value.ToString()! : nameof(NativeNullable<T>);
//     }
//     
//     public static implicit operator T?(NativeNullable<T> value) => value.Value;
// }