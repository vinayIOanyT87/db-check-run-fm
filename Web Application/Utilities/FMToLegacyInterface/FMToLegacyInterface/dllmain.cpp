// dllmain.cpp : Defines the entry point for the DLL application.
// this application will be used as the interface between the FuelsManager web app and the Legacy FuelsManager RPC interface
// the Legacy version this is being built for is Version 7.6. If you want to connect to another version make sure you verify that
// the RPC functions have not changed.
// In addition we will only be adding the functions that are required for Version 12.0 SP1 Movement and Leak Detection interfaces
#include "pch.h"


BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

