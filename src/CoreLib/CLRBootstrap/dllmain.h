// ReSharper disable CppInconsistentNaming
#pragma once
#define ulong unsigned long
#include <windows.h>

typedef struct
{
    HINSTANCE Module;
    DWORD MainThreadId;
    int MainThreadPriority;
} LOADER_INFORMATION, *PLOADER_INFORMATION;

typedef void (*InitFunc)(PLOADER_INFORMATION);

typedef struct
{
    InitFunc CLREntryPoint;
    PLOADER_INFORMATION PLoaderInfo;
} CLRInitFunc, *PCLRInitFunc;

// Function declarations
DWORD WINAPI DllThread(const PLOADER_INFORMATION lpParameter);
DWORD GetMainThreadId();

