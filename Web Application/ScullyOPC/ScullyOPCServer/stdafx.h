// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

#ifndef STRICT
#define STRICT
#endif

// Modify the following defines if you have to target a platform prior to the ones specified below.
// Refer to MSDN for the latest info on corresponding values for different platforms.
#ifndef WINVER				// Allow use of features specific to Windows 95 and Windows NT 4 or later.
#define WINVER 0x0602		// Change this to the appropriate value to target Windows 98 and Windows 2000 or later.
#endif

#ifndef _WIN32_WINNT		// Allow use of features specific to Windows NT 4 or later.
#define _WIN32_WINNT 0x0602	// Change this to the appropriate value to target Windows 2000 or later.
#endif						

#ifndef _WIN32_WINDOWS		// Allow use of features specific to Windows 98 or later.
#define _WIN32_WINDOWS 0x0602 // Change this to the appropriate value to target Windows Me or later.
#endif

#ifndef _WIN32_IE			// Allow use of features specific to IE 4.0 or later.
#define _WIN32_IE 0x0550	// Change this to the appropriate value to target IE 5.0 or later.
#endif

#define _ATL_FREE_THREADED
#define _ATL_NO_AUTOMATIC_NAMESPACE

#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS	// some CString constructors will be explicit

// turns off ATL's hiding of some common and often safely ignored warning messages
#define _ATL_ALL_WARNINGS


#include <afxwin.h>
#include <afxdisp.h>
#include <afxcoll.h>       // MFC collections
#include <afxtempl.h>      // MFC template collections
#include <afxadv.h>        // CSharedFile

#include <comsvcs.h>

#include "resource.h"
#include <atlbase.h>
#include <atlcom.h>

using namespace ATL;

#import "ScullyOPCObjects.tlb" no_namespace named_guids \
			exclude("_FILETIME","IRecord")
#import "ScullyOPCServer.tlb" no_namespace named_guids \
			exclude("_FILETIME","IEnumString","tagOPCBROWSETYPE",\
			"tagOPCNAMESPACETYPE","tagOPCBROWSEDIRECTION","tagOPCSERVERSTATE",\
			"tagOPCENUMSCOPE","tagOPCSERVERSTATUS","IOPCServer",\
			"OPCSERVERSATUS","OPCENUMSCOPE","IOPCBrowseServerAddressSpace")
#import "msado25.tlb" no_namespace named_guids rename ("EOF", "EndOfFile" ) 
#import "../../binaries/FMUtil.dll" no_namespace named_guids

_COM_SMARTPTR_TYPEDEF(IObjectContext, __uuidof(IObjectContext));

//*******************************************************************
// Use to enter a critical section and automatically leave it
// when the object goes out of scope (including exceptions, etc).
class CSLock
{
public:
   CSLock(CRITICAL_SECTION* lock)
      : pLock(lock) {EnterCriticalSection(pLock);}
   ~CSLock()
      {LeaveCriticalSection(pLock);}
private:
   CRITICAL_SECTION* pLock;
};

class CScullyOPCServerApp : public CWinApp
{
public:
	FILETIME					m_ServerStartTime;
	CString					m_strConnectionString;
	CString					m_strProviderString;

	// Logging methods
	void LogInfo(LPCTSTR pszFormat, ...) const;
	void LogError(LPCTSTR pszFormat, ...) const;
	void LogWarning(LPCTSTR pszFormat, ...) const;

	void	GetConnectionString();
	void	GetProviderString();

// Overrides
    virtual BOOL InitInstance();
    virtual int ExitInstance();

    DECLARE_MESSAGE_MAP()

	private:
	void WriteTextToEventLog(WORD wEventType, LPCTSTR pszMsg) const;
	void WriteToEventLog(WORD wEventType,
								DWORD dwEventID,
								USHORT nNumStrings = 0,
								LPCTSTR* pszStrings = NULL,
								ULONG dwDataSize = 0,
								LPVOID lpRawData = NULL) const;
};

extern CScullyOPCServerApp theApp;

#include "ConsolidatedCategories.c"

#include "opccomn.h"
#include "opcda.h"
