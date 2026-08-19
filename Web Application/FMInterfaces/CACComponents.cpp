// CACComponents.cpp : Implementation of CCACComponents

#include "stdafx.h"
#include "CACComponents.h"

#include <winscard.h>
#include <winsmcrd.h>
#include <wincrypt.h>

#define MALLOC(size)  ((LPBYTE) LocalAlloc(LPTR, size))
#define FREE(buffer)  (LocalFree((LPBYTE) buffer))

// CCACComponents
LONG CACEnabled();
LONG SCardPropCert (IN SCARDCONTEXT hContext, IN LPCTSTR mszReaderNames);
LONG CryptPropCert (IN HCRYPTPROV hCryptProv, IN LPCTSTR szCSPName);
LONG GetCert (IN HCRYPTPROV hCryptProv, IN DWORD dwKeySpec, OUT LPBYTE * lplpbCert, OUT DWORD *lpdwCertLength);

#define MAX_PASS 25 
#define MAX_USER 50

TCHAR szSubjectName[MAX_USER+1]={0};

// CCACComponents

STDMETHODIMP CCACComponents::CACEnable(BSTR* szUserID, VARIANT_BOOL* bEnabled)
{
	// TODO: Add your implementation code here
// TODO: Add your implementation code here
	if(CACEnabled()== SCARD_S_SUCCESS)
	{
		CComBSTR str(szSubjectName);
		*szUserID = str.Detach();
		*bEnabled = TRUE;
	}
	else
		*bEnabled = FALSE;

	return S_OK;
}

LONG CACEnabled()
{
	LONG lResult;
	DWORD dwNumReaders = 0;
	SCARDCONTEXT hContext = NULL;
	LPTSTR mszReaderNames = NULL;

   __try
   {
      // Establish context with the resource manager.
      lResult = SCardEstablishContext(SCARD_SCOPE_USER, NULL, NULL, &hContext);
      if (lResult != SCARD_S_SUCCESS)
         __leave;

      DWORD dwAutoAllocate = SCARD_AUTOALLOCATE;
      lResult = SCardListReaders(hContext, SCARD_DEFAULT_READERS, 
                                 (LPTSTR)&mszReaderNames, &dwAutoAllocate);

      if (lResult != SCARD_S_SUCCESS)
         __leave;

      lResult = SCardPropCert(hContext, mszReaderNames);
   }

   __finally
   {
      LONG lReturn;
      if (mszReaderNames != NULL)
      {
         lReturn = SCardFreeMemory(hContext, (LPVOID) mszReaderNames);
         if (lResult == SCARD_S_SUCCESS)
            lResult = lReturn;
      }
      if (hContext != NULL)
      {
         lReturn = SCardReleaseContext(hContext);
         if (lResult == SCARD_S_SUCCESS)
            lResult = lReturn;
      }
   }
  return lResult; 
}

LONG SCardPropCert (IN SCARDCONTEXT hContext, IN LPCTSTR mszReaderNames)
{
	BYTE *pbBuffer= (BYTE *)"Data"; // this is to prompt a user to enter PIN
	DWORD dwBufferLen = strlen((char *)pbBuffer)+1;
	HCRYPTHASH hHash = 0;
	BYTE *pbSignature = NULL;
	DWORD dwSigLen;
	LPTSTR szDescription = _T("Test Data Description");

   LONG lResult;
   LPSCARD_READERSTATE lpReaderStates = NULL;

   // Make sure pointer parameters are not NULL.
   if (mszReaderNames == NULL)
      return SCARD_E_INVALID_PARAMETER;

   __try
   {
      DWORD dwNumReaders;
      LPCTSTR szReaderName;
      // Count number of readers.
      for (dwNumReaders = 0, szReaderName = mszReaderNames;
           *szReaderName != _T('\0'); dwNumReaders++)
      {
         szReaderName += lstrlen(szReaderName) + 1;
      }
      // Allocate memory for SCARD_READERSTATE array.
      lpReaderStates = (LPSCARD_READERSTATE)MALLOC(dwNumReaders * sizeof(SCARD_READERSTATE));
      if (lpReaderStates == NULL)
      {
         lResult = SCARD_E_NO_MEMORY;
         __leave;
      }
      // Prepare state array.
      ZeroMemory((LPVOID)lpReaderStates, dwNumReaders * sizeof(SCARD_READERSTATE));

	  DWORD i;
      for (i = 0, szReaderName = mszReaderNames; i < dwNumReaders; i++)
      {
         lpReaderStates[i].szReader = (LPCTSTR) szReaderName;
         lpReaderStates[i].dwCurrentState = SCARD_STATE_UNAWARE;
         szReaderName += lstrlen(szReaderName) + 1;
      }
      // Initialize card status.
      lResult = SCardGetStatusChange(hContext, INFINITE, lpReaderStates, dwNumReaders);
      if (lResult != SCARD_S_SUCCESS)
         __leave;

      // For each card found, find the proper CSP and propagate the
      // certificate(s) to the specified local store.
      for (i = 0; i < dwNumReaders && lResult == SCARD_S_SUCCESS; i++)
      {
         DWORD dwAutoAllocate;
         LPTSTR szCardName = NULL;
         LPTSTR szCSPName = NULL;
         LPTSTR szContainerName = NULL;
         HCRYPTPROV hCryptProv = NULL;

         __try
         {
            if (!(lpReaderStates[i].dwEventState & SCARD_STATE_PRESENT))
			{
				lResult = SCARD_E_NO_SMARTCARD;
				continue; // No card in this reader.
			}

            // Get card name.
            dwAutoAllocate = SCARD_AUTOALLOCATE;
            lResult = SCardListCards(hContext, lpReaderStates[i].rgbAtr,
                                     NULL, 0, (LPTSTR) &szCardName, &dwAutoAllocate);
            if (lResult != SCARD_S_SUCCESS)
               __leave;

            // Get card's CSP name.
            dwAutoAllocate = SCARD_AUTOALLOCATE;
            lResult = SCardGetCardTypeProviderName(hContext, szCardName, SCARD_PROVIDER_CSP, 
													(LPTSTR) &szCSPName, &dwAutoAllocate);
            if (lResult != SCARD_S_SUCCESS)
               __leave;

            szContainerName = (LPTSTR) MALLOC((sizeof(_T("\\\\.\\")) +
                              lstrlen(lpReaderStates[i].szReader) +
                              sizeof(_T("\\\0"))) * sizeof(TCHAR));

            if (szContainerName == NULL)
			{
				lResult = SCARD_E_NO_MEMORY;
				__leave;
			}

            wsprintf(szContainerName, _T("\\\\.\\%s\\"), lpReaderStates[i].szReader);

            if (!CryptAcquireContext(&hCryptProv, szContainerName,szCSPName, PROV_RSA_FULL, 0))
            {
               lResult = GetLastError();
               __leave;
            }
			if(!CryptCreateHash(hCryptProv, CALG_MD5, 0, 0, &hHash)) 			
			{
				lResult = GetLastError();
               __leave;
			}
			if(!CryptHashData(hHash, pbBuffer, dwBufferLen, 0)) 
			{
				lResult = GetLastError();
               __leave;
			}
			dwSigLen= 0;
			if(!CryptSignHash(hHash, AT_KEYEXCHANGE, szDescription, 0, NULL, &dwSigLen)) 
			{
				lResult = GetLastError();
               __leave;
			}
			if(!(pbSignature = (BYTE *)malloc(dwSigLen)))
			{
				lResult = GetLastError();
               __leave;
			}
			if(!CryptSignHash(hHash, AT_KEYEXCHANGE, szDescription, 0, pbSignature, &dwSigLen)) 
			{
				lResult = GetLastError();
                __leave;
			}
			
            // Propagate the cert.
            lResult = CryptPropCert(hCryptProv, szCSPName);

         }

         __finally
         {
			if(hHash) 
				CryptDestroyHash(hHash);

			if(pbSignature)
				free(pbSignature);

            LONG lReturn;
            // Don't forget to free resources, if allocated.
            if (hCryptProv != NULL)
            {
               if (!CryptReleaseContext(hCryptProv, 0))
               {
                  if (lResult == SCARD_S_SUCCESS)
                     lResult = GetLastError();
               }
            }
            if (szContainerName != NULL)
               FREE((LPVOID) szContainerName);
            if (szCSPName != NULL)
            {
               lReturn = SCardFreeMemory(hContext, (LPVOID) szCSPName);
               if (lResult == SCARD_S_SUCCESS)
                  lResult = lReturn;
            }
            if (szCardName != NULL)
            {
               lReturn = SCardFreeMemory(hContext, (LPVOID) szCardName);
               if (lResult == SCARD_S_SUCCESS)
                  lResult = lReturn;
            }
         }
      }

   }

   __finally
   {
      // Don't forget to free resources, if allocated.
      if (lpReaderStates != NULL)
         FREE((LPVOID) lpReaderStates);
   }

   return lResult;
}

LONG CryptPropCert (IN HCRYPTPROV hCryptProv, IN LPCTSTR szCSPName)
{	 
	LONG lResult = SCARD_F_UNKNOWN_ERROR;
	// Make sure pointer parameters are not NULL.
	if (szCSPName == NULL)
      return SCARD_E_INVALID_PARAMETER;

	const DWORD rgdwKeys[] = {AT_KEYEXCHANGE, AT_SIGNATURE};
	const DWORD cdwKeys = sizeof(rgdwKeys) / sizeof(rgdwKeys[0]);

	LPBYTE lpbCert = NULL;
	for (DWORD i = 0; i < cdwKeys; i++)
	{
		DWORD dwCertLength = 0;
	
	 
		// Get the certificate data.
		if (GetCert(hCryptProv, rgdwKeys[i], &lpbCert, &dwCertLength) != SCARD_S_SUCCESS)
			continue; 

		// Allocate memory for UNICODE strings.
		PCCERT_CONTEXT           pCertContext = NULL;
		pCertContext = CertCreateCertificateContext(X509_ASN_ENCODING, lpbCert, dwCertLength);

		if(!pCertContext)
		{
			if (lpbCert != NULL)
				FREE(lpbCert);
			continue; 
		}

		if(CertVerifyTimeValidity(NULL, pCertContext->pCertInfo)!=0 )
			 continue; 

		CERT_REVOCATION_STATUS	revocationStatus;
		memset(&revocationStatus, 0, sizeof(CERT_REVOCATION_STATUS));
		revocationStatus.cbSize = sizeof(CERT_REVOCATION_STATUS);

		if (FALSE == CertVerifyRevocation(X509_ASN_ENCODING, 
														CERT_CONTEXT_REVOCATION_TYPE, 
														1, 
														const_cast<void**>(reinterpret_cast<const void**>(&pCertContext)), 
														0, 
														NULL,
														&revocationStatus))
		{
			/*
			//Error checking code
			DWORD dwError = GetLastError();
			LPVOID lpMsgBuf;
			FormatMessage( 
				 FORMAT_MESSAGE_ALLOCATE_BUFFER | 
				 FORMAT_MESSAGE_FROM_SYSTEM | 
				 FORMAT_MESSAGE_IGNORE_INSERTS,
				 NULL,
				 dwError,
				 MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
				 (LPTSTR) &lpMsgBuf,
				 0,
				 NULL 
			);
			// Process any inserts in lpMsgBuf.
			// ...
			// Display the string.
			MessageBox( NULL, (LPCTSTR)lpMsgBuf, _T("Error"), MB_OK | MB_ICONINFORMATION );
			// Free the buffer.
			LocalFree( lpMsgBuf );
			*/
			continue;
		}

		/*
		This section of code was replaced 1/22/2009 by CertVerifyRevocation call above.

		DWORD dwFlags, cbUrlArray;    
		PCRYPT_URL_ARRAY pUrlArray;    
		dwFlags = CRYPT_GET_URL_FROM_PROPERTY | CRYPT_GET_URL_FROM_EXTENSION;    
		if (!CryptGetObjectUrl(URL_OID_CERTIFICATE_CRL_DIST_POINT, (LPVOID)pCertContext, dwFlags, 
								NULL, &cbUrlArray, NULL, NULL, NULL))  
			continue; 
		if (!(pUrlArray = (PCRYPT_URL_ARRAY)LocalAlloc(LMEM_FIXED, cbUrlArray)))
			continue; 
		if (!CryptGetObjectUrl(URL_OID_CERTIFICATE_CRL_DIST_POINT, (LPVOID)pCertContext,dwFlags, 
								pUrlArray, &cbUrlArray, NULL, NULL, NULL))  
			continue; 
		
		if (!pUrlArray )  
			continue; 

		PCCRL_CONTEXT    pCrlContext;
		PCRL_ENTRY pCrlEntry = NULL;
		//dwFlags = CRYPT_WIRE_ONLY_RETRIEVAL;   			
		dwFlags = 0;

		for ( DWORD i = 0; i  < pUrlArray->cUrl;  i++) 
		{ 			
			if (!CryptRetrieveObjectByUrl(pUrlArray->rgwszUrl[i], CONTEXT_OID_CRL, dwFlags, 300000, (LPVOID *)&pCrlContext, 0, 0, 0, 0))   
			{
				DWORD dwError = GetLastError();
				LPVOID lpMsgBuf;
				FormatMessage( 
					 FORMAT_MESSAGE_ALLOCATE_BUFFER | 
					 FORMAT_MESSAGE_FROM_SYSTEM | 
					 FORMAT_MESSAGE_IGNORE_INSERTS,
					 NULL,
					 dwError,
					 MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
					 (LPTSTR) &lpMsgBuf,
					 0,
					 NULL 
				);
				// Process any inserts in lpMsgBuf.
				// ...
				// Display the string.
				MessageBox( NULL, (LPCTSTR)lpMsgBuf, _T("Error"), MB_OK | MB_ICONINFORMATION );
				// Free the buffer.
				LocalFree( lpMsgBuf );
				continue;   
			}

			if(CertFindCertificateInCRL(pCertContext,  pCrlContext, 0,  0, &pCrlEntry))
			{
				if(pCrlEntry)
					break;
			}  
		} 

		LocalFree(pUrlArray);
			
		if(!pCrlContext || pCrlEntry)
			break;
		*/

		// get common name, 		
		if(CertGetNameString (pCertContext, CERT_NAME_ATTR_TYPE, NULL, szOID_COMMON_NAME, szSubjectName, MAX_USER+1)) 
		{
			lResult = SCARD_S_SUCCESS;
			break;
		}
	}

    if (lpbCert != NULL)
			FREE(lpbCert);
	return lResult;
}

LONG GetCert (IN HCRYPTPROV hCryptProv, IN DWORD dwKeySpec, OUT LPBYTE *lplpbCert, OUT DWORD *lpdwCertLength)
{
   LONG lResult = SCARD_S_SUCCESS;
   HCRYPTKEY hCryptKey = NULL;
   LPBYTE lpbCert = NULL;
   DWORD dwCertLength = 0;
   // Make sure pointer parameters are not NULL.
   if (lplpbCert == NULL || lpdwCertLength == NULL)
      return SCARD_E_INVALID_PARAMETER;

   __try
   {
      // Get key handle.
      if (!CryptGetUserKey(hCryptProv, dwKeySpec, &hCryptKey))
      {
         lResult = GetLastError();
         __leave;
      }
      // Query certificate data length.
      if (!CryptGetKeyParam(hCryptKey,KP_CERTIFICATE,NULL, &dwCertLength, 0))
      {
         // We expect ERROR_MORE_DATA. If that's not the case, then
         // something is not right.
         lResult = GetLastError();
         if (lResult == ERROR_MORE_DATA)
            lResult = SCARD_S_SUCCESS;
         else
            __leave;
      }
      // Allocate memory for certificate data.
      lpbCert = (LPBYTE) MALLOC(dwCertLength);
      if (lpbCert == NULL)
	  {
			lResult = SCARD_E_NO_MEMORY;
			__leave;
	  }
      // Now read the certificate data.
      if (!CryptGetKeyParam(hCryptKey, KP_CERTIFICATE, lpbCert, &dwCertLength, 0))
      {
         lResult = GetLastError();
         __leave;
      }
   }

   __finally
   {
      // Don't forget to free resources, if allocated.
      if (lResult == SCARD_S_SUCCESS)
      {
         *lplpbCert = lpbCert;
         *lpdwCertLength = dwCertLength;
      }
      else if (lpbCert != NULL)
         FREE(lpbCert);
      if (hCryptKey != NULL)
      {
         if (!CryptDestroyKey(hCryptKey))
		 {			
            if (lResult == SCARD_S_SUCCESS)
               lResult = GetLastError();
         }
      }
   }
   return lResult;
}
