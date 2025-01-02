namespace Dawn.DarkCrusade.ModdingTools.Collections;

using System.Collections;
using System.Diagnostics;
using Threading;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(NativeArrayDebugView<>))]
public unsafe struct NativeArray<T>(T* pointer, int count) : IDisposable, IEnumerable<T> where T : unmanaged
{
    private AtomicBoolean _disposed;
    public T* Pointer { get; private set; } = pointer;

    public readonly int Count = count;

    public T this[int index] => index < Count
        ? Pointer[index]
        : throw new IndexOutOfRangeException();

    public readonly Span<T> AsSpan() => new(Pointer, Count);

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Free()
    {
        if (Pointer == null)
            return;

        free(Pointer, sizeof(T) * Count);
        Pointer = null;
    }

    public void Dispose()
    {
        if (!_disposed.CompareAndSet(false, true))
            return;

        Free();
    }
}


public class NativeArrayDebugView<T>(NativeArray<T> array) where T : unmanaged
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items => array.AsSpan().ToArray();
}