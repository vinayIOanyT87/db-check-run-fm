#include "pch.h"
#include <link.h>
#include <corecrt_malloc.h>
#include "utility.hpp"

// this file is used for the RPC utility functions.

#define STRING_SIZE(x) (sizeof(x)/sizeof(TCHAR));

// movement system warning messages
DWORD	MvmntCombinationWarning(PMOVEMENTNAME	pMovementName,
	WORD				wNameCount,
	PMOVEMENTNAME	pCombineName)
{
	return(IDCANCEL);
}

DWORD	MvmntContaminateWarning(PMOVEMENTNAME		pMovementName,
	PNODEPRODUCTDATA	pNodeProductData,
	WORD					wNodeCount,
	PNODEPRODUCTDATA	pContaminateData)
{
	return(IDCANCEL);
}


DWORD	MvmntAddUnitNodeWarning(PMOVEMENTNAME	pMovementName,
	LPTSTR			szNodeName)
{
	return(IDCANCEL);
}

DWORD	MvmntLockoutWarning(PMOVEMENTNAME	pMovementName)
{
	return(IDCANCEL);
}

DWORD	MvmntSetupWarning(PMOVEMENTNAME	pMovementName)
{
	return(IDCANCEL);
}

DWORD MvmntChangeTransferWarning(LPTSTR			szNodeName,
	WORD				wNewTankMode,
	WORD				wOldTankMode,
	WORD				wNewXfrMode,
	WORD				wOldXfrMode,
	double			dNewXfrSetpoint,
	BYTE				bNewXfrSetpointStyle,
	double			dOldXfrSetpoint,
	BYTE				bOldXfrSetpointStyle)
{
	return(IDCANCEL);
}

// rpc memory functions these must be part of the file for VS2019 and above.
// The developer controls how the memory and what type is allocated
void __RPC_FAR* __RPC_USER midl_user_allocate(size_t cBytes)
{
	return((void __RPC_FAR*) malloc(cBytes));
}

// Memory deallocation function for RPC.
void __RPC_USER midl_user_free(void __RPC_FAR* p)
{
	free(p);
}

// common routines for binding to RPC servers

RPC_STATUS	FuelManagerBind(RPC_BINDING_HANDLE* pBind, 			// Pointer to Client Binding
	RPC_IF_HANDLE* pClientIf, 		// Interface Handle
	DWORD							dwSystemType,	//	System Type
	LPTSTR						pServer)		// Server Name
{
	TCHAR* pszUUID;
	TCHAR				szSystem[MAX_COMPUTERNAME_LENGTH + 10];
	TCHAR				szEndPoint[35];
	RPC_STATUS		status;
	RPC_IF_ID		InterfaceID;
	LPTSTR			pszStringBinding = NULL;
	DWORD				dwSize;


	status = RpcIfInqId(*pClientIf, &InterfaceID);
	status = UuidToString(&InterfaceID.Uuid, (RPC_WSTR*)&pszUUID);

	*pBind = NULL;
	if (!pServer)
	{
		dwSize = MAX_COMPUTERNAME_LENGTH + 3;

		lstrcpy( szSystem, TEXT( "\\\\" ));	// this forces the use of named pipes locally.
		GetComputerName(&szSystem[2], &dwSize);
		pServer = szSystem;

	}
	//	Check for Type of Binding 

	//	Use Named Pipe Protocol
	if ((*pServer == L'\\') && (*(pServer + 1) == L'\\'))
	{
		switch (dwSystemType)
		{
		case  FM_SYSTEM_AM:
			lstrcpy(szEndPoint, AM_SERV_PIPE_NAME);
			break;

		case  FM_SYSTEM_DB:
			lstrcpy(szEndPoint, DM_SERV_PIPE_NAME);
			break;

		case	FM_SYSTEM_CM:
			lstrcpy(szEndPoint, CM_SERV_PIPE_NAME);
			break;

		case	FM_SYSTEM_REP:
			lstrcpy(szEndPoint, RM_SERV_PIPE_NAME);
			break;

		case	FM_SYSTEM_FM:
			lstrcpy(szEndPoint, FM_SERV_PIPE_NAME);
			break;

		default:
			return(RPC_S_INVALID_ARG);

		}
		status = RpcStringBindingCompose((RPC_WSTR)pszUUID,
			(RPC_WSTR)TEXT("ncacn_np"),
			(RPC_WSTR)pServer,
			(RPC_WSTR)szEndPoint,
			NULL,
			(RPC_WSTR*)&pszStringBinding);
	}

	//	Otherwise Use TCP/IP Protocol
	else
	{
		switch (dwSystemType)
		{
		case  FM_SYSTEM_AM:
			lstrcpy(szEndPoint, AM_SERV_SOCK_NAME);
			break;

		case  FM_SYSTEM_DB:
			lstrcpy(szEndPoint, DM_SERV_SOCK_NAME);
			break;

		case	FM_SYSTEM_CM:
			lstrcpy(szEndPoint, CM_SERV_SOCK_NAME);
			break;

		case	FM_SYSTEM_REP:
			lstrcpy(szEndPoint, RM_SERV_SOCK_NAME);
			break;

		case	FM_SYSTEM_FM:
			lstrcpy(szEndPoint, FM_SERV_SOCK_NAME);
			break;

		default:
			return(RPC_S_INVALID_ARG);

		}
		status = RpcStringBindingCompose((RPC_WSTR)pszUUID,
			(RPC_WSTR)TEXT("ncacn_ip_tcp"),
			(RPC_WSTR)pServer,
			(RPC_WSTR)szEndPoint,
			NULL,
			(RPC_WSTR*)&pszStringBinding);
	}


	// Free Memory for UUID String
	RpcStringFree((RPC_WSTR*)&pszUUID);

	if (status != RPC_S_OK)
		return(status);

	// Try To Get Actual Binding
	status = RpcBindingFromStringBinding((RPC_WSTR)pszStringBinding, pBind);
	RpcStringFree((RPC_WSTR*)&pszStringBinding);
	return(status);
}

// get current local computer name
BOOL GetComputerName(LPTSTR lpBuffer, LPDWORD lpnSize, BOOL	bRemoveBackSlash)
{
	BOOL						bReturn = TRUE;
	TCHAR						szSystemName[MAX_COMPUTERNAME_LENGTH + 3] = TEXT("");
	DWORD						dwSize = *lpnSize;

	memset(lpBuffer, 0x00, dwSize);
	if (bReturn)
	{
		if (!bRemoveBackSlash)
		{
			// verify that we have "\\" at the beginning
			if (szSystemName[0] != '\\' && szSystemName[1] != '\\')
			{
				wsprintf(lpBuffer, TEXT("\\\\%s"), szSystemName);
			}
			else if (szSystemName[0] == '\\' && szSystemName[1] == '\\')
			{
				lstrcpy(lpBuffer, szSystemName);
			}
			else
			{
				bReturn = FALSE;
			}
		}
		else
		{
			// there are places where we do not want the backslash so if they are there remove them
			if (szSystemName[0] == '\\' && szSystemName[1] == '\\')
			{
				wsprintf(lpBuffer, TEXT("%s"), &szSystemName[2]);
			}
			else if (szSystemName[0] == '\\' && szSystemName[1] != '\\')
			{
				wsprintf(lpBuffer, TEXT("%s"), &szSystemName[1]);
			}
			else
			{
				lstrcpy(lpBuffer, szSystemName);
			}
		}
	}
	return(bReturn);
}


BOOL ConvertStringToLevelNames(PLEVELNAMES Name, LPTSTR szName)
{
	int		iLoop;
	BYTE		bDBLevels = 0;
	BYTE		bLevels = 0;
	TCHAR		szID[64];

	if (lstrlen(szName) > 63)
		return(FALSE);

	lstrcpy(szID, szName);

	// determine the number of levels based on the passed in string

	if (GetDefaultLevelS(&bLevels))
	{
		bDBLevels = bLevels;
	}

	// Parse the TAG into levelnames

	Name->szLevelString[0][0] = 0;
	Name->szLevelString[1][0] = 0;
	Name->szLevelString[2][0] = 0;
	Name->szLevelString[3][0] = 0;

	for (iLoop = bDBLevels - 1; iLoop >= 0; iLoop--)
	{
		TCHAR* lpszLevelTag;

		if (iLoop == (int)bDBLevels - 1)
		{
			if (iLoop == 0)
				lpszLevelTag = szID;
			else
				lpszLevelTag = wcstok(szID, TEXT(".\0"));
		}
		else if (iLoop == 0)
			lpszLevelTag = wcstok(NULL, TEXT("\0"));
		else
			lpszLevelTag = wcstok(NULL, TEXT(".\0"));

		if (!lpszLevelTag)
			return(FALSE);

		if (TrimSpaces(lpszLevelTag, TRUE, TRUE) > 15 || lpszLevelTag[0] == 0)
			return(FALSE);
		lstrcpy((LPTSTR)&Name->szLevelString[iLoop], lpszLevelTag);
	}

	return(TRUE);
}

DWORD		GetDefaultLevelS(PBYTE pbLevels)
{
	HKEY	 		hKeyDefault, hKey;
	LPTSTR 		pszKeyPath = (LPTSTR)TEXT("Software\\Varec\\SCADA");
	DWORD			dwError, dwSize, dwType;
	PDWORD		pdwParameter;
	BYTE			bParam[255], bLev;

	while (1)
	{
		hKeyDefault = (HKEY)INVALID_HANDLE_VALUE;
		hKey = hKeyDefault;
		// Get Base Key

		if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, pszKeyPath, 0L, KEY_READ, &hKey) != ERROR_SUCCESS)
		{
			*pbLevels = 0;
			dwError = FALSE;
			break;
		}

		// On Success Key	Get SubKey

		if (RegOpenKeyEx(hKey, TEXT("DataManager\\DefaultLevel"), 0L, KEY_ALL_ACCESS, &hKeyDefault) != ERROR_SUCCESS)
		{
			*pbLevels = 0;
			dwError = TRUE;
			break;
		}

		//	Get default Number of Levels for New Database
		pdwParameter = (PDWORD)&bParam[0];
		dwSize = 511;
		if ((RegQueryValueEx(hKeyDefault, TEXT("bNumberLevels"), NULL, &dwType, (LPBYTE)&bParam, &dwSize) != ERROR_SUCCESS) ||
			(dwType != REG_DWORD))
		{
			dwError = FALSE;
			break;
		}
		bLev = (BYTE)*pdwParameter;
		*pbLevels = bLev;
		dwError = TRUE;

		break;

	}
	if (hKeyDefault != (HKEY)INVALID_HANDLE_VALUE) RegCloseKey(hKeyDefault);

	if (hKey != (HKEY)INVALID_HANDLE_VALUE) RegCloseKey(hKey);

	return(dwError);
}

int TrimSpaces(LPTSTR pString, BYTE bLead, BYTE bTrail)
{
	LPTSTR  pChar, pEnd;

	// Check input
	if (!pString) return(0);

	// Trim Trailing Spaces Upon Request
	if (bTrail)
	{
		for (pChar = pString + lstrlen(pString) - 1;
			pChar >= pString;
			pChar--)
		{
			if (*pChar == L' ' || *pChar == L'\t')
				*pChar = 0;
			else
				break;
		}
	}

	// If Leading Spaces are to Be Trimmed - Move the String

	if (bLead)
	{
		pChar = pString;
		pEnd = pString + lstrlen(pString);

		while (pChar < pEnd)
		{
			if (*pChar == L' ' || *pChar == L'\t')
				pChar++;
			else
				break;
		}
		if (pChar > pString) lstrcpy(pString, pChar);
	}
	return(lstrlen(pString));
}

//////////////////////////SYSTEM FUNCTIONS GO HERE ///////////////////////////
BOOL bGetRegistryStringValue(LPTSTR lpszKey, LPTSTR lpszValue, LPTSTR lpszOutput)
{
	// Retrieve the Specified directory from the Registry
	DWORD		dwSize;
	HKEY		hRegKey;
	LONG		lRegOpenRtn = -1;
	LONG		lRegRtn;


	// Open Registry Key "lpszKey"
	lRegOpenRtn = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
		(LPCWSTR)lpszKey,
		0L,
		KEY_READ,
		&hRegKey);

	if (lRegOpenRtn != ERROR_SUCCESS)
	{
		return(FALSE);

	}
	// Get Desired Value "lpszValue" under Key...store at "lpszOutput"

	dwSize = (MAX_PATH + 1) * sizeof(TCHAR);

	lRegRtn = RegQueryValueEx(hRegKey,
		(LPCWSTR)lpszValue,
		NULL,
		NULL,
		(LPBYTE)lpszOutput,
		&dwSize);

	if (lRegRtn != ERROR_SUCCESS)
	{
		RegCloseKey(hRegKey);
		return(FALSE);

	}
	// Close Registry if Opened Successfully

	if (lRegOpenRtn == ERROR_SUCCESS)
	{
		lRegRtn = RegCloseKey(hRegKey);
	}
	return(TRUE);
}

BOOL	MvmntBind(	RPC_BINDING_HANDLE* phBinding,
						LPTSTR	lpszServer)
{
	RPC_IF_ID	InterfaceID;
	TCHAR*		pszUUID;
	LPTSTR		pszStringBinding;

	*phBinding = NULL;

	// Get a binding handle to server

	if (RpcIfInqId(mvmntlink_ClientIfHandle, &InterfaceID) != RPC_S_OK)
	{
		return(FALSE);
	}

	if (UuidToString(&InterfaceID.Uuid, (unsigned short**)&pszUUID) != RPC_S_OK)
	{
		return(FALSE);
	}

	if (RpcStringBindingCompose((unsigned short*)pszUUID,
		(unsigned short*)TEXT("ncacn_np"),
		(unsigned short*)lpszServer,
		(unsigned short*)MVMNT_SERV_PIPE_NAME,
		NULL,
		(unsigned short**)&pszStringBinding) != RPC_S_OK)
	{
		return(FALSE);
	}

	RpcStringFree((unsigned short**)&pszUUID);

	if (RpcBindingFromStringBinding((unsigned short*)pszStringBinding, phBinding) != RPC_S_OK)
	{
		return(FALSE);
	}

	RpcStringFree((unsigned short**)&pszStringBinding);

	return(TRUE);
}
