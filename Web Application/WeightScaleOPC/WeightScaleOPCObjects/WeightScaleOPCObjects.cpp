// WeightScaleOPCObjects.cpp : Implementation of DLL Exports.

#include "stdafx.h"
#include "resource.h"
#include "dlldatax.h"

class CWeightScaleOPCObjectsModule : public CAtlDllModuleT< CWeightScaleOPCObjectsModule >
{
public :
	DECLARE_LIBID(LIBID_WeightScaleOPCObjectsLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_WEIGHTSCALEOPCOBJECTS, "{27B327D3-3CC6-435D-AFD6-D4D678E364EC}")
};

CWeightScaleOPCObjectsModule _AtlModule;

class CWeightScaleOPCObjectsApp : public CWinApp
{
public:

// Overrides
    virtual BOOL InitInstance();
    virtual int ExitInstance();

    DECLARE_MESSAGE_MAP()
};

BEGIN_MESSAGE_MAP(CWeightScaleOPCObjectsApp, CWinApp)
END_MESSAGE_MAP()

CWeightScaleOPCObjectsApp theApp;

BOOL CWeightScaleOPCObjectsApp::InitInstance()
{
#ifdef _MERGE_PROXYSTUB
    if (!PrxDllMain(m_hInstance, DLL_PROCESS_ATTACH, NULL))
		return FALSE;
#endif
    return CWinApp::InitInstance();
}

int CWeightScaleOPCObjectsApp::ExitInstance()
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
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    return (AfxDllCanUnloadNow()==S_OK && _AtlModule.GetLockCount()==0) ? S_OK : S_FALSE;
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
