// SecureHash.cpp : Implementation of CSecureHash

#include "stdafx.h"
#include "SecureHash.h"
#include ".\securehash.h"
#include "SecureHashAlgorithm.h"


// CSecureHash

STDMETHODIMP CSecureHash::InterfaceSupportsErrorInfo(REFIID riid)
{
	static const IID* arr[] = 
	{
		&IID_ISecureHash
	};

	for (int i=0; i < sizeof(arr) / sizeof(arr[0]); i++)
	{
		if (InlineIsEqualGUID(*arr[i],riid))
			return S_OK;
	}
	return S_FALSE;
}

STDMETHODIMP CSecureHash::HashPassword(BSTR bstrUserID, BSTR bstrPassword, BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	// Hash the password
	BYTE		hashValue[20] = {0};
	CString	strPassword;
	CString	strName;
	CString	strHash;
	CString	strNextByte;
	char* mbstrName;
	char* mbstrPassword;
	long lNameLength = 0;
	long lPasswordLength = 0;
	strPassword=(LPCTSTR) bstrPassword;
	strName=(LPCTSTR) bstrUserID;
	strName.MakeUpper();
	lNameLength = _tcslen(strName);
	lPasswordLength = _tcslen(strPassword);
	mbstrName = (char*)malloc((lNameLength * 2 + 1) * sizeof(char));
	mbstrPassword = (char*)malloc((lPasswordLength * 2 + 1) * sizeof(char));
	memset(mbstrName,0,(lNameLength * 2 + 1) * sizeof(char));
	memset(mbstrPassword,0,(lPasswordLength * 2 + 1) * sizeof(char));
	wcstombs(mbstrName,strName,lNameLength * 2);
	wcstombs(mbstrPassword,strPassword,lPasswordLength * 2);
	
	SecureHashAlgorithm1 sha1;

	sha1.ComputeHash( mbstrPassword, strPassword.GetLength());
	sha1.ComputeHash( mbstrName, strName.GetLength());	
	sha1.GetHashValue( hashValue, sizeof( hashValue ));
	free(mbstrName);
	free(mbstrPassword);
	// Format hash value for display and storage
	for( int i=0;i < 20;i++ )
	{
		strNextByte.Format( _T("%02x"), hashValue[ i ]);
		strHash += strNextByte;
	}

	*pVal=strHash.AllocSysString();

	return S_OK;
}
