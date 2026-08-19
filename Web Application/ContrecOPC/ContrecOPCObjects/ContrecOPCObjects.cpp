// ContrecOPCObjects.cpp : Implementation of DLL Exports.

#include "stdafx.h"
#include "resource.h"
#include "dlldatax.h"

class CContrecOPCObjectsModule : public CAtlDllModuleT< CContrecOPCObjectsModule >
{
public :
	DECLARE_LIBID(LIBID_ContrecOPCObjectsLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_CONTRECOPCOBJECTS, "{0A15929E-C46A-4166-A619-20D33679F222}")
};

CContrecOPCObjectsModule _AtlModule;


class CContrecOPCObjectsApp : public CWinApp
{
public:

// Overrides
    virtual BOOL InitInstance();
    virtual int ExitInstance();

    DECLARE_MESSAGE_MAP()
};

BEGIN_MESSAGE_MAP(CContrecOPCObjectsApp, CWinApp)
END_MESSAGE_MAP()

CContrecOPCObjectsApp theApp;

BOOL CContrecOPCObjectsApp::InitInstance()
{
#ifdef _MERGE_PROXYSTUB
    if (!PrxDllMain(m_hInstance, DLL_PROCESS_ATTACH, NULL))
		return FALSE;
#endif
    return CWinApp::InitInstance();
}

int CContrecOPCObjectsApp::ExitInstance()
{
    return CWinApp::ExitInstance();
}

// Used to determine whether the DLL can be unloaded by OLE
STDAPI DllCanUnloadNow(void)
{
#ifdef _MERGE_PROXYSTUB
    HRESULT hr = PrxDllCanUnloadNow();
    if (FAILED(hr))
        return hr;
#endif
    return _AtlModule.DllCanUnloadNow();
}


// Returns a class factory to create an object of the requested type
STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
#ifdef _MERGE_PROXYSTUB
    if (PrxDllGetClassObject(rclsid, riid, ppv) == S_OK)
        return S_OK;
#endif
    return _AtlModule.DllGetClassObject(rclsid, riid, ppv);
}


// DllRegisterServer - Adds entries to the system registry
STDAPI DllRegisterServer(void)
{
    // registers object, typelib and all interfaces in typelib
    HRESULT hr = _AtlModule.DllRegisterServer();
#ifdef _MERGE_PROXYSTUB
    if (FAILED(hr))
        return hr;
    hr = PrxDllRegisterServer();
#endif
	return hr;
}


// DllUnregisterServer - Removes entries from the system registry
STDAPI DllUnregisterServer(void)
{
	HRESULT hr = _AtlModule.DllUnregisterServer();
#ifdef _MERGE_PROXYSTUB
    if (FAILED(hr))
        return hr;
    hr = PrxDllRegisterServer();
    if (FAILED(hr))
        return hr;
    hr = PrxDllUnregisterServer();
#endif
	return hr;
}
