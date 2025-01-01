﻿namespace Dawn.AOT.CoreLib.X86;

[StructLayout(LayoutKind.Sequential)]
public readonly struct LoaderInformation
{
    public readonly HINSTANCE Module;
    public readonly uint MainThreadId;
    public readonly THREAD_PRIORITY MainThreadPriority;
}