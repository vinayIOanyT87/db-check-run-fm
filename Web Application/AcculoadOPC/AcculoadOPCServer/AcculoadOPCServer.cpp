// AcculoadOPCServer.cpp : Implementation of DLL Exports.
//
// Note: COM+ 1.0 Information:
//      Please remember to run Microsoft Transaction Explorer to install the component(s).
//      Registration is not done by default. 

#include "stdafx.h"
#include "resource.h"
#include "messages.h"
#include "dlldatax.h"
#include "DeviceManager.h"

CDeviceManager*		g_pDeviceManager=NULL;


class CAcculoadOPCServerModule : public CAtlDllModuleT< CAcculoadOPCServerModule >
{
public :
	DECLARE_LIBID(LIBID_AcculoadOPCServerLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ACCULOADOPCSERVER, "{E23BF9CF-8D1F-4A31-8099-2882BEC28418}")
};

CAcculoadOPCServerModule _AtlModule;

#define SERVICE_NAME _T("AcculoadOPC Server")


namespace // unnamed -- see Section 9.2 of The C++ Programming Language, 3rd Ed.
{
	void RegisterEvents()
	{
		// Get the executable file path
		TCHAR szFilePath[_MAX_PATH];
		GetModuleFileName(theApp.m_hInstance, szFilePath, _MAX_PATH);

		CRegKey keyEventLog;
		LONG lRes = keyEventLog.Open(HKEY_LOCAL_MACHINE,
										_T("SYSTEM\\CurrentControlSet\\Services\\EventLog\\Application"),
										KEY_WRITE);
		if (lRes == ERROR_SUCCESS)
		{
			CRegKey key;
			lRes = key.Create(keyEventLog, SERVICE_NAME);
			if (lRes == ERROR_SUCCESS)
			{
				key.SetValue(_T("EventMessageFile"),REG_SZ,szFilePath,sizeof(TCHAR)*(lstrlen(szFilePath)+1));
				DWORD	dwTypesSupported=	EVENTLOG_SUCCESS |
												EVENTLOG_ERROR_TYPE |
									         EVENTLOG_INFORMATION_TYPE |
												EVENTLOG_WARNING_TYPE;

				key.SetValue(_T("TypesSupported"),REG_DWORD,&dwTypesSupported,sizeof(DWORD));
			}
		}
	}

	void UnregisterEvents()
	{
		CRegKey keyEventLog;
		LONG lRes = keyEventLog.Open(HKEY_LOCAL_MACHINE,
											  _T("SYSTEM\\CurrentControlSet\\Services\\EventLog\\Application"),
											  KEY_WRITE);
		if (lRes == ERROR_SUCCESS)
			keyEventLog.RecurseDeleteKey(SERVICE_NAME);
	}

}


BEGIN_MESSAGE_MAP(CAcculoadOPCServerApp, CWinApp)
END_MESSAGE_MAP()

CAcculoadOPCServerApp theApp;

// Logging functions
void CAcculoadOPCServerApp::WriteToEventLog(	WORD wEventType,
											DWORD dwEventID,
											USHORT nNumStrings,
											LPCTSTR* pszStrings,
											ULONG nDataSize,
											LPVOID lpRawData) const
{
	// Get a handle to use with ReportEvent().
	HANDLE hEventSource = ::RegisterEventSource(NULL, SERVICE_NAME);
	if (hEventSource != NULL)
	{
		// Write to event log.
		::ReportEvent(hEventSource,
						  wEventType,
						  0,
						  dwEventID,
						  NULL,
						  nNumStrings,
						  nDataSize,
						  pszStrings,
						  lpRawData);
		::DeregisterEventSource(hEventSource);
	}
}

void CAcculoadOPCServerApp::WriteTextToEventLog(WORD wEventType, LPCTSTR pszMsg) const
{
	LPCTSTR	lpszStrings[1];
	lpszStrings[0] = pszMsg;

	WriteToEventLog(wEventType, EVLOG_TEXT, 1, &lpszStrings[0]);
}

void CAcculoadOPCServerApp::LogInfo(LPCTSTR pszFormat, ...) const
{
	TCHAR		chMsg[1024];
	va_list	pArg;

   va_start(pArg, pszFormat);
   _vstprintf(chMsg, pszFormat, pArg);
	va_end(pArg);

	WriteTextToEventLog(EVENTLOG_INFORMATION_TYPE, chMsg);
}

void CAcculoadOPCServerApp::LogError(LPCTSTR pszFormat, ...) const
{
	TCHAR		chMsg[1024];
	va_list	pArg;

   va_start(pArg, pszFormat);
   _vstprintf(chMsg, pszFormat, pArg);
	va_end(pArg);

	WriteTextToEventLog(EVENTLOG_ERROR_TYPE, chMsg);
}

void CAcculoadOPCServerApp::LogWarning(LPCTSTR pszFormat, ...) const
{
	TCHAR		chMsg[1024];
	va_list	pArg;

   va_start(pArg, pszFormat);
   _vstprintf(chMsg, pszFormat, pArg);
	va_end(pArg);

	WriteTextToEventLog(EVENTLOG_WARNING_TYPE, chMsg);
}

BOOL CAcculoadOPCServerApp::InitInstance()
{
#ifdef _MERGE_PROXYSTUB
    if (!PrxDllMain(m_hInstance, DLL_PROCESS_ATTACH, NULL))
		return FALSE;
#endif


	GetConnectionString();
	GetProviderString();

   CoFileTimeNow(&m_ServerStartTime);

	if(!AfxSocketInit())
	{
		CString strError;
		strError=_T("Error AfxSocketInit");
		theApp.LogError(strError);
	}

	g_pDeviceManager=new CDeviceManager();

	return CWinApp::InitInstance();
}

int CAcculoadOPCServerApp::ExitInstance()
{
	 if(g_pDeviceManager)
		delete g_pDeviceManager;

    return CWinApp::ExitInstance();
}

void	CAcculoadOPCServerApp::GetProviderString()
{
	CString	strConnection;
	
	strConnection=m_strConnectionString;
	strConnection.MakeUpper();
	INT iDelimiter=strConnection.Find(_T("PROVIDER="),0);
	if(iDelimiter == -1)
		theApp.LogError(_T("Invalid Connection String"));
	else
	{
		strConnection=strConnection.Mid(iDelimiter);
		iDelimiter=strConnection.Find(_T("="));
		strConnection=strConnection.Mid(iDelimiter+1);
		iDelimiter=strConnection.Find(_T(";"));
		if(iDelimiter == -1)
			m_strProviderString=strConnection;
		else
			m_strProviderString=strConnection.Left(iDelimiter);
	}
}

void	CAcculoadOPCServerApp::GetConnectionString()
{
	// ODBC
	// TCHAR szString[512]={_T("Provider=MSDASQL;DSN=AcculoadOPC;")};

	// Jet OLEDB - Note: does not support transactions
	// TCHAR szString[512]={_T("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=c:\\Program Files\\AcculoadOPC\\AcculoadOPC\\AcculoadOPC.mdb;")};

	// SQL Server OLEDB
	TCHAR szString[512]={_T("provider=SQLOLEDB;Data Source=127.0.0.1;Initial Catalog=AcculoadOPC;Integrated Security=SSPI;")};

	CRegKey RegKey;

	if(ERROR_SUCCESS == RegKey.Create(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Varec\\AcculoadOPC")))
	{
		DWORD	dwSize=sizeof(szString);
		DWORD	dwType=REG_SZ;
		if(ERROR_SUCCESS != RegQueryValueEx(RegKey.m_hKey,_T("ConnectionString"),0,&dwType,(LPBYTE) szString,&dwSize))
			RegSetValueEx(RegKey.m_hKey,_T("ConnectionString"),0,REG_SZ,(LPBYTE) szString,lstrlen(szString)*sizeof(TCHAR));
		m_strConnectionString=szString;
		RegKey.Close();
	}
	else
	{
		CString strError;
		strError=_T("Error Creating Registry Key");
		theApp.LogError(strError);
	}
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
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
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
	RegisterEvents();
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
	UnregisterEvents();
	return hr;
}
