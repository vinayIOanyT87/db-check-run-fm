/******************************************************************************

	FILE NAME:		OPCServerBase.cpp


	PURPOSE:			Implementation of COPCServerBase

	MODIFICATION HISTORY:
	Date:			By:					Reason:
	----------	-----------------	-------------------------------------------
	9-Apr-08		B. Schaal			7.4.0.0 - Changed CreateGroupEnumerator() to be OPC 2.0 compliant
*******************************************************************************/


#include "stdafx.h"
#include "OPCServerBase.h"
#define IID_DEFINED
#include "OPCDa_i.c"
#include "OPCComn_i.c"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

FILETIME     serverStartTime;

// define OPC data stream formats
UINT OPCSTMFORMATDATA          = RegisterClipboardFormat(_T("OPCSTMFORMATDATA"));
UINT OPCSTMFORMATDATATIME      = RegisterClipboardFormat(_T("OPCSTMFORMATDATATIME"));
UINT OPCSTMFORMATWRITECOMPLETE = RegisterClipboardFormat(_T("OPCSTMFORMATWRITECOMPLETE"));

//*******************************************************************
OPCServerBase::OPCServerBase()
: m_localeID( LOCALE_SYSTEM_DEFAULT )
{
   InitializeCriticalSection( &m_cs );
   m_lastUpdateTime.dwLowDateTime=m_lastUpdateTime.dwHighDateTime=0;
}

//*******************************************************************
// Destructor is only called when there are no more clients using the object.
OPCServerBase::~OPCServerBase()
{
   DeleteCriticalSection( &m_cs );
}

//*******************************************************************
void OPCServerBase::UpdateTime()
{
    CSLock wait( &m_cs );
    CoFileTimeNow( &m_lastUpdateTime );
}

//*******************************************************************
// Server should send shutdown callback to its client
// This function can be called from any thread.
void OPCServerBase::ServerShutdown( LPTSTR reason )
{
   USES_CONVERSION;
   // notify the connection points
   IUnknown** pp = m_vec.begin();
   while (pp < m_vec.end())
   {
      if (*pp != NULL)
      {
         IOPCShutdown* pIOPCShutdown = (IOPCShutdown*)*pp;
         HRESULT hr = pIOPCShutdown->ShutdownRequest(T2OLE(reason));
      }
      pp++;
   }
}

//*******************************************************************
OPCGroupObject* OPCServerBase::FindNamedGroup( LPCWSTR name )
{
   CSLock wait( &m_cs );
   OPCGroupObject* pGroup = NULL;
   LPVOID key = 0;
   POSITION pos = m_groupMap.GetStartPosition();
   while( pos )
   {
      m_groupMap.GetNextAssoc( pos, key, pGroup );
      ASSERT( pGroup );
      if( pGroup->CompareName( name ) )
         return pGroup;
   }

   return NULL;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::AddGroup(
                            LPCWSTR     szName,
                            BOOL        bActive,
                            DWORD       dwRequestedUpdateRate,
                            OPCHANDLE   hClientGroup,
                            LONG      * pTimeBias,
                            FLOAT     * pPercentDeadband,
                            DWORD       dwLCID,
                            OPCHANDLE * phServerGroup,
                            DWORD     * pRevisedUpdateRate,
                            REFIID      riid,
                            LPUNKNOWN * ppUnk)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   USES_CONVERSION;
   *ppUnk = NULL;
   CSLock wait( &m_cs );
   OPCGroupObject* pGroup = NULL;
   CString groupName( szName );  // convert wide to native string
   if( groupName.IsEmpty() ) // if no name, create a unique one
   {
      for( int count=1; TRUE; count++ )
      {
         groupName.Format( _T("Group%d"), count);
         OPCGroupObject* pGroup = FindNamedGroup( T2OLE(groupName.GetBuffer(0)) );
         if( pGroup == NULL ) // keep looking until no match
            break;
      }
   }
   else  // check for unique name
   {
      OPCGroupObject* pGroup = FindNamedGroup( T2OLE(groupName.GetBuffer(0)) );
      if( pGroup )
         return OPC_E_DUPLICATENAME;
   }

   pGroup = DoAddGroup( T2OLE(groupName.GetBuffer(0)),
                        bActive,
                        dwRequestedUpdateRate,
                        hClientGroup,
                        pTimeBias,
                        pPercentDeadband,
                        dwLCID,
                        phServerGroup,
                        pRevisedUpdateRate);
   if( pGroup == NULL )
      return E_OUTOFMEMORY;

   if( phServerGroup != NULL )
      *phServerGroup = (OPCHANDLE)pGroup;
   if( pRevisedUpdateRate != NULL )
      *pRevisedUpdateRate = pGroup->GetUpdateRate();
   m_groupMap.SetAt( pGroup, pGroup );

   pGroup->AddRef();    // our reference
   HRESULT hr = pGroup->QueryInterface( riid, (LPVOID*)ppUnk );
   if( FAILED(hr) )
	{
		m_groupMap.RemoveKey( (LPVOID)pGroup );
		pGroup->Release();
      return hr;
	}
   if( !dwRequestedUpdateRate )
      return OPC_S_UNSUPPORTEDRATE;
		
	if(dwRequestedUpdateRate != pGroup->GetUpdateRate() )
      return OPC_S_UNSUPPORTEDRATE;
   return hr;
}

//*******************************************************************
OPCGroupObject* OPCServerBase::DoAddGroup(
                            LPCWSTR     szName,
                            BOOL        bActive,
                            DWORD       dwRequestedUpdateRate,
                            OPCHANDLE   hClientGroup,
                            LONG      * pTimeBias,
                            FLOAT     * pPercentDeadband,
                            DWORD       dwLCID,
                            OPCHANDLE * phServerGroup,
                            DWORD     * pRevisedUpdateRate)
{
   OPCGroupObject* pGroup = new OPCGroupObject;
   if( pGroup == NULL )
      return NULL;

   pGroup->Initialize ( szName,
                        bActive,
                        dwRequestedUpdateRate,
                        hClientGroup,
                        pTimeBias,
                        pPercentDeadband,
                        dwLCID,
                        this);
   return pGroup;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::SetLocaleID(LCID dwLcid)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(dwLcid != LOCALE_SYSTEM_DEFAULT
	&& !IsValidLocale(dwLcid,LCID_INSTALLED))
      return E_INVALIDARG;

   m_localeID = dwLcid;
   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::GetLocaleID(LCID * pdwLcid)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( pdwLcid==NULL )
      return E_POINTER;

   *pdwLcid = m_localeID;
   return S_OK;
}

//*******************************************************************
typedef struct{
	DWORD	dwCount;
	LCID*	lpLcid;
} ENUM_LCID;


BOOL CALLBACK EnumResLangProc(HANDLE	hModule,
										LPCTSTR	lpszType,
										LPCTSTR	lpszName,
										WORD		wIDLanguage,
										LPARAM	lParam)
{
	ENUM_LCID*	lpEnumLcid=(ENUM_LCID*) lParam;

	if(IsValidLocale(MAKELCID(wIDLanguage,SORT_DEFAULT),LCID_INSTALLED))
	{
		lpEnumLcid->lpLcid = (LCID*) CoTaskMemRealloc(lpEnumLcid->lpLcid,lpEnumLcid->dwCount+1*sizeof(LCID));
		
		if(lpEnumLcid->lpLcid)
		{
			lpEnumLcid->lpLcid[lpEnumLcid->dwCount]=MAKELCID(wIDLanguage,SORT_DEFAULT);
			lpEnumLcid->dwCount++;
		}
	}

	return(TRUE);
}

STDMETHODIMP OPCServerBase::QueryAvailableLocaleIDs(
      DWORD          * pdwCount,
      LCID          ** pdwLcid)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	ENUM_LCID EnumLcid={0,NULL};

   if( !pdwCount || !pdwLcid )
      return E_POINTER;

	EnumLcid.lpLcid = (LCID*) CoTaskMemAlloc(sizeof(LCID));
		
	if(EnumLcid.lpLcid)
	{
		EnumLcid.lpLcid[EnumLcid.dwCount]=MAKELCID(LOCALE_SYSTEM_DEFAULT,SORT_DEFAULT);
		EnumLcid.dwCount++;
		EnumResourceLanguages(	theApp.m_hInstance,
										RT_STRING,
										MAKEINTRESOURCE(IDS_PROJNAME),
										(ENUMRESLANGPROC) EnumResLangProc,
										(LPARAM) &EnumLcid);
	}

	*pdwCount=EnumLcid.dwCount;
	*pdwLcid=EnumLcid.lpLcid;

   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::GetErrorString(
      HRESULT          dwError,
      LPWSTR         * ppString)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   return GetErrorString( dwError, m_localeID, ppString );
}

//*******************************************************************
STDMETHODIMP OPCServerBase::SetClientName(LPCWSTR szName)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( szName==NULL )
      return E_POINTER;

   m_client = szName;
   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::GetErrorString(
                           HRESULT  dwError,
                           LCID     dwLocale,
                           LPWSTR * ppString)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())

   *ppString = NULL;
   CString message(_T("Unknown error."));
   switch(dwError)
   {
      case OPC_E_INVALIDHANDLE:
         message.LoadString( IDS_OPC_E_INVALIDHANDLE );
         break;
      case OPC_E_BADTYPE:
         message.LoadString( IDS_OPC_E_BADTYPE );
         break;
      case OPC_E_PUBLIC:
         message.LoadString( IDS_OPC_E_PUBLIC );
         break;
      case OPC_E_BADRIGHTS:
         message.LoadString( IDS_OPC_E_BADRIGHTS );
         break;
      case OPC_E_UNKNOWNITEMID:
         message.LoadString( IDS_OPC_E_UNKNOWNITEMID );
         break;
      case OPC_E_INVALIDITEMID:
         message.LoadString( IDS_OPC_E_INVALIDITEMID );
         break;
      case OPC_E_INVALIDFILTER:
         message.LoadString( IDS_OPC_E_INVALIDFILTER );
         break;
      case OPC_E_UNKNOWNPATH:
         message.LoadString( IDS_OPC_E_UNKNOWNPATH );
         break;
      case OPC_E_RANGE:
         message.LoadString( IDS_OPC_E_RANGE );
         break;
      case OPC_E_DUPLICATENAME:
         message.LoadString( IDS_OPC_E_DUPLICATENAME );
         break;
      case OPC_S_UNSUPPORTEDRATE:
         message.LoadString( IDS_OPC_S_UNSUPPORTEDRATE );
         break;
      case OPC_S_CLAMP:
         message.LoadString( IDS_OPC_S_CLAMP );
         break;
      case OPC_S_INUSE:
         message.LoadString( IDS_OPC_S_INUSE );
         break;
      case OPC_E_INVALIDCONFIGFILE:
         message.LoadString( IDS_OPC_E_INVALIDCONFIGFILE );
         break;
      case OPC_E_NOTFOUND:
         message.LoadString( IDS_OPC_E_NOTFOUND );
         break;
      default:
         {
         WCHAR buffer[256];
         HRESULT hr = DoGetErrorString( dwError, dwLocale, buffer );
         if( SUCCEEDED(hr) )
            message = buffer;
         }
         break;
   }
   *ppString = (LPWSTR)CoTaskMemAlloc(2+message.GetLength()*2);
   USES_CONVERSION;
   wcscpy( *ppString, T2OLE(message.GetBuffer(0)) );
   return S_OK;
}

//*******************************************************************
HRESULT OPCServerBase::DoGetErrorString(  HRESULT  dwError,
                                          LCID     dwLocale,
                                          LPWSTR   pString)
{
    return E_FAIL;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::GetGroupByName(
                           LPCWSTR     szName,
                           REFIID      riid,
                           LPUNKNOWN * ppUnk)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   *ppUnk = NULL;
   CSLock wait( &m_cs );
   OPCGroupObject* pGroup = FindNamedGroup( szName );
   if( pGroup )
      return pGroup->QueryInterface( riid, (LPVOID*)ppUnk );

   return E_INVALIDARG;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::GetStatus(OPCSERVERSTATUS **ppServerStatus)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( ppServerStatus == NULL )
      return E_POINTER;
   *ppServerStatus = (OPCSERVERSTATUS*)CoTaskMemAlloc(sizeof(OPCSERVERSTATUS));
   if( *ppServerStatus == NULL )
      return E_OUTOFMEMORY;

   return DoGetStatus( *ppServerStatus );
}

//*******************************************************************
HRESULT OPCServerBase::DoGetStatus( OPCSERVERSTATUS *pServerStatus)
{
   CSLock wait( &m_cs );
   pServerStatus->ftStartTime = serverStartTime;
   CoFileTimeNow( &pServerStatus->ftCurrentTime );
   pServerStatus->ftLastUpdateTime = m_lastUpdateTime;
   pServerStatus->dwServerState = OPC_STATUS_RUNNING;
   pServerStatus->dwGroupCount = m_groupMap.GetCount();
   pServerStatus->dwBandWidth = 0;
   pServerStatus->wMajorVersion = 0;
   pServerStatus->wMinorVersion = 0;
   pServerStatus->wBuildNumber = 0;
   CString vendor(_T(""));
   pServerStatus->szVendorInfo = (LPWSTR)CoTaskMemAlloc(2*vendor.GetLength()+2);
   USES_CONVERSION;
   wcscpy( pServerStatus->szVendorInfo, T2OLE(vendor.GetBuffer(0)) );

   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::RemoveGroup(
                           OPCHANDLE   hServerGroup,
                           BOOL     bForce)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( hServerGroup == 0 )
      return E_INVALIDARG;

   OPCGroupObject* pGroup = (OPCGroupObject*)hServerGroup;
   _ASSERT( pGroup );

   pGroup->Remove();
   // remove from list
	EnterCriticalSection(&m_cs);
	BOOL ok = m_groupMap.RemoveKey( (LPVOID)pGroup );
	LeaveCriticalSection(&m_cs);
	if(bForce)
	{
		IUnknown* pUnk=0;
		HRESULT hr = pGroup->QueryInterface( IID_IUnknown, (LPVOID*)&pUnk );
		if( SUCCEEDED(hr) )
		{
          if(S_OK != CoDisconnectObject( pUnk, 0 ))
				 theApp.LogError(_T("OPCServer Error : CoDisconnectObject"));
			 pUnk->Release();
		}
		pGroup->UnInitialize();
	}
	LONG left = pGroup->Release();
	if(left)
		return OPC_S_INUSE;

   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::QueryAvailableProperties(
                           LPWSTR      szItemID,
                           DWORD     * pdwCount,
                           DWORD    ** ppPropertyIDs,
                           LPWSTR   ** ppDescriptions,
                           VARTYPE  ** ppvtDataTypes)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   // All args should be valid
   if( szItemID==NULL || pdwCount==NULL || ppPropertyIDs==NULL
    || ppDescriptions==NULL || ppvtDataTypes==NULL )
      return E_POINTER;

   *pdwCount = 0;
   *ppPropertyIDs = NULL;
   *ppDescriptions = NULL;
   *ppvtDataTypes = NULL;

   // Find this Item, and get the number of properties
   DWORD dwNumItems = 0;
   LPVOID pVoid = NULL;
   HRESULT hr = DoQueryNumProperties(szItemID, &dwNumItems, &pVoid);
   if( FAILED(hr) )
      return hr;

   // create return data
   *ppPropertyIDs = (DWORD*)CoTaskMemAlloc(dwNumItems*sizeof(DWORD));
   if( *ppPropertyIDs == NULL )
      return E_OUTOFMEMORY;
   memset( *ppPropertyIDs, 0, dwNumItems*sizeof(DWORD));

   *ppDescriptions = (LPWSTR*)CoTaskMemAlloc(dwNumItems*sizeof(LPWSTR));
   if( *ppDescriptions == NULL )
   {
      CoTaskMemFree( *ppPropertyIDs );
      *ppPropertyIDs = NULL;
      return E_OUTOFMEMORY;
   }
   memset( *ppDescriptions, 0, dwNumItems*sizeof(LPWSTR));

   *ppvtDataTypes = (VARTYPE*)CoTaskMemAlloc(dwNumItems*sizeof(VARTYPE));
   if( *ppvtDataTypes == NULL )
   {
      CoTaskMemFree( *ppPropertyIDs );
      *ppPropertyIDs = NULL;
      CoTaskMemFree( *ppDescriptions );
      *ppDescriptions = NULL;
      return E_OUTOFMEMORY;
   }
   memset( *ppvtDataTypes, 0, dwNumItems*sizeof(VARTYPE));

   *pdwCount = dwNumItems;
   // Now get the properties
   hr = DoQueryAvailableProperties(szItemID, dwNumItems, pVoid,
                        *ppPropertyIDs, *ppDescriptions, *ppvtDataTypes);
   return hr;
}

//*******************************************************************
// Return the number of properties for this ItemID (if it is valid)
// ppVoid will be passed back in DoQueryAvailableProperties,
// so save the tag pointer in it.
HRESULT OPCServerBase::DoQueryNumProperties(
                              LPWSTR      szItemID,
                              DWORD     * pdwNumItems,
                              LPVOID    * ppVoid)
{
   return OPC_E_UNKNOWNITEMID;
}

//*******************************************************************
// Return the properties for this ItemID (if it is valid)
// pVoid contains the tag pointer if DoQueryNumProperties returned
// successfully (If it failed, this function is not called)
HRESULT OPCServerBase::DoQueryAvailableProperties(
                              LPWSTR      szItemID,
                              DWORD       dwNumItems,
                              LPVOID      pVoid,
                              DWORD     * pPropertyIDs,
                              LPWSTR    * pDescriptions,
                              VARTYPE   * pvtDataTypes)
{
   return E_NOTIMPL;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::GetItemProperties(
                           LPWSTR      szItemID,
                           DWORD       dwCount,
                           DWORD     * pdwPropertyIDs,
                           VARIANT  ** ppvData,
                           HRESULT  ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   // All args should be valid
   if( szItemID==NULL || pdwPropertyIDs==NULL
    || ppvData==NULL || ppErrors==NULL )
      return E_POINTER;

   *ppvData = NULL;
   *ppErrors = NULL;

   // create return data
   VARIANT* pV = *ppvData = (VARIANT*)CoTaskMemAlloc(dwCount*sizeof(VARIANT));
   if( *ppvData == NULL )
      return E_OUTOFMEMORY;
   memset( *ppvData, 0, dwCount*sizeof(DWORD));

   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwCount*sizeof(HRESULT));
   if( *ppErrors == NULL )
   {
      CoTaskMemFree( *ppvData );
      *ppvData = NULL;
      return E_OUTOFMEMORY;
   }
   memset( *ppErrors, 0, dwCount*sizeof(HRESULT));

   for( DWORD index=0; index<dwCount; index++ )
      VariantInit(&(pV[index]));

   // Now get the properties
   HRESULT hr = DoGetItemProperties(szItemID, dwCount,
                        pdwPropertyIDs, *ppvData, *ppErrors);

	if(!SUCCEEDED(hr))
	{
      CoTaskMemFree( *ppvData );
      *ppvData = NULL;
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
	}
   return hr;
}

//*******************************************************************
HRESULT OPCServerBase::DoGetItemProperties(
                              LPWSTR      szItemID,
                              DWORD       dwNumItems,
                              DWORD     * pPropertyIDs,
                              VARIANT   * pData,
                              HRESULT   * pErrors)
{
   return E_NOTIMPL;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::LookupItemIDs(
                           LPWSTR      szItemID,
                           DWORD       dwCount,
                           DWORD     * pdwPropertyIDs,
                           LPWSTR   ** ppszNewItemIDs,
                           HRESULT  ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   // All args should be valid
   if( szItemID==NULL || pdwPropertyIDs==NULL
    || ppszNewItemIDs==NULL || ppErrors==NULL )
      return E_POINTER;

   *ppszNewItemIDs = NULL;
   *ppErrors = NULL;

   // create return data
   *ppszNewItemIDs = (LPWSTR*)CoTaskMemAlloc(dwCount*sizeof(LPWSTR));
   if( *ppszNewItemIDs == NULL )
      return E_OUTOFMEMORY;
   memset( *ppszNewItemIDs, 0, dwCount*sizeof(LPWSTR));

   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwCount*sizeof(HRESULT));
   if( *ppErrors == NULL )
   {
      CoTaskMemFree( *ppszNewItemIDs );
      *ppszNewItemIDs = NULL;
      return E_OUTOFMEMORY;
   }
   memset( *ppErrors, 0, dwCount*sizeof(HRESULT));

   // Now get the properties
   HRESULT hr = DoLookupItemIDs(szItemID, dwCount, pdwPropertyIDs,
                        *ppszNewItemIDs, *ppErrors);
	if(!SUCCEEDED(hr))
	{
		for(DWORD index=0;index < dwCount;index++)
			if((*ppszNewItemIDs)[index])
				CoTaskMemFree((*ppszNewItemIDs)[index]);
      CoTaskMemFree( *ppszNewItemIDs );
      *ppszNewItemIDs = NULL;
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
	}
   return hr;
}

//*******************************************************************
HRESULT OPCServerBase::DoLookupItemIDs(
                              LPWSTR      szItemID,
                              DWORD       dwNumItems,
                              DWORD     * pPropertyIDs,
                              LPWSTR    * pszNewItemIDs,
                              HRESULT   * pErrors)
{
   return E_NOTIMPL;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::CreateGroupEnumerator(
                           OPCENUMSCOPE dwScope,
                           REFIID      riid,
                           LPUNKNOWN * ppUnk)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	HRESULT hResult = E_NOINTERFACE;

	if(!ppUnk)
		return E_POINTER;

	if(riid == IID_IEnumUnknown)
	{
		switch (dwScope)
		{
			case OPC_ENUM_PUBLIC_CONNECTIONS:
			case OPC_ENUM_PUBLIC:
				return S_FALSE;
			default:
			{
				CComEnumGroupIUnknown* pEnumerator = new CComEnumGroupIUnknown;
				if( pEnumerator )
					pEnumerator->Initialize(this);

				// return requested interface.
				hResult = pEnumerator->QueryInterface( riid, (LPVOID*)ppUnk );

				if (FAILED(hResult))
				{
					return hResult;
				}
				break;
			}
		}
	}
	else if(riid == IID_IEnumString)
	{
		switch (dwScope)
		{
			case OPC_ENUM_PUBLIC_CONNECTIONS:
			case OPC_ENUM_PUBLIC:
				return S_FALSE;
			default:
				{
				CComEnumGroupNames* pEnumerator = new CComEnumGroupNames;
				if( pEnumerator )
					pEnumerator->Initialize(this);
				hResult = pEnumerator->QueryInterface( riid, (LPVOID*)ppUnk );
				if (FAILED(hResult))
				{
					return hResult;
				}
				break;
				}
		}
	}
	if(m_groupMap.GetCount() == 0)
		return S_FALSE;

	return hResult;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::UpdateClients()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   HRESULT hr = S_OK;
   OPCGroupObject* pGroup = NULL;
   LPVOID key = 0;
   POSITION pos = m_groupMap.GetStartPosition();
   while( pos && SUCCEEDED(hr) )
   {
      m_groupMap.GetNextAssoc( pos, key, pGroup );
      ASSERT( pGroup );
      hr = pGroup->UpdateClients();
      if( hr == S_OK )
         CoFileTimeNow( &m_lastUpdateTime );
   }
   return hr;
}

//*******************************************************************
// IOPCBrowseServerAddressSpace
//*******************************************************************
STDMETHODIMP OPCServerBase::QueryOrganization( OPCNAMESPACETYPE * pNameSpaceType)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( pNameSpaceType == NULL )
      return E_POINTER;

   *pNameSpaceType = DoQueryOrganization();

   return S_OK;
}

//*******************************************************************
OPCNAMESPACETYPE OPCServerBase::DoQueryOrganization()
{
   return OPC_NS_HIERARCHIAL;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::ChangeBrowsePosition(
                              OPCBROWSEDIRECTION dwBrowseDirection,
                              LPCWSTR           szString)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   return DoChangeBrowsePosition(dwBrowseDirection,szString);
}

//*******************************************************************
HRESULT OPCServerBase::DoChangeBrowsePosition(
                              OPCBROWSEDIRECTION dwBrowseDirection,
                              LPCWSTR           szString)
{
   return E_NOTIMPL;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::BrowseOPCItemIDs(
                              OPCBROWSETYPE     dwBrowseFilterType,
                              LPCWSTR           szFilterCriteria,
                              VARTYPE           vtDataTypeFilter,
                              DWORD             dwAccessRightsFilter,
                              LPENUMSTRING *    ppIEnumString)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   *ppIEnumString = NULL;
   return DoBrowseOPCItemIDs( dwBrowseFilterType,
                              szFilterCriteria,
                              vtDataTypeFilter,
                              dwAccessRightsFilter,
                              ppIEnumString);
}

//*******************************************************************
// Should just create an enumerator, not the rest...
HRESULT OPCServerBase::DoBrowseOPCItemIDs(
                              OPCBROWSETYPE     dwBrowseFilterType,
                              LPCWSTR           szFilterCriteria,
                              VARTYPE           vtDataTypeFilter,
                              DWORD             dwAccessRightsFilter,
                              LPENUMSTRING *    ppIEnumString)
{
   return E_NOTIMPL;
}

//*******************************************************************
// if szItemDataID is NULL, return the current location
STDMETHODIMP OPCServerBase::GetItemID(
                              LPWSTR      szItemDataID,
                              LPWSTR *    szItemID)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( szItemID == NULL )
      return E_POINTER;

   return DoGetItemID(szItemDataID,szItemID);
}

HRESULT OPCServerBase::DoGetItemID(
                              LPWSTR      szItemDataID,
                              LPWSTR *    szItemID)
{
   return E_NOTIMPL;
}

//*******************************************************************
STDMETHODIMP OPCServerBase::BrowseAccessPaths(
                              LPCWSTR        szItemID,
                              LPENUMSTRING * ppIEnumString)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   return E_NOTIMPL;
}

//*******************************************************************
int ConvertCase( int c, BOOL bCaseSensitive )
{
   return bCaseSensitive ? c : toupper(c);
}

//*******************************************************************
// CEnumGroupNames implementation
//*******************************************************************
CEnumGroupNames::CEnumGroupNames()
{
   m_pos = NULL;
}

CEnumGroupNames::~CEnumGroupNames()
{
}


//*******************************************************************
void CEnumGroupNames::Initialize(OPCServerBase* pServer)
{
	m_pServer=pServer;
	CSLock(&m_pServer->m_cs);
   m_pos = m_pServer->m_groupMap.GetStartPosition();
}

//*******************************************************************
STDMETHODIMP CEnumGroupNames::Next(
                            ULONG celt,
                            LPOLESTR * ppStrings,
                            ULONG * pceltFetched )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   OPCGroupObject* pGroup = NULL;
   LPVOID key = 0;
	CSLock(&m_pServer->m_cs);
	
   for( ULONG i = 0; i < celt && m_pos; )
   {
		m_pServer->m_groupMap.GetNextAssoc( m_pos, key, pGroup );
		ppStrings[i] = (LPWSTR)CoTaskMemAlloc(2*pGroup->GetName().GetLength()+2);
		USES_CONVERSION;
		wcscpy( ppStrings[i], T2OLE(pGroup->GetName().GetBuffer(0)) );
		i++;
	}

   if( pceltFetched )
      *pceltFetched = i;
   return (celt==i) ? S_OK : S_FALSE;

}

//*******************************************************************
// just iterate celt times to skip those items
STDMETHODIMP CEnumGroupNames::Skip( ULONG celt )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   OPCGroupObject* pGroup = NULL;
   LPVOID key = 0;
	CSLock(&m_pServer->m_cs);
   for( ULONG i = 0; i < celt && m_pos; )
   {
		m_pServer->m_groupMap.GetNextAssoc( m_pos, key, pGroup );
      i++;
	}
   return (celt==i) ? S_OK : S_FALSE;

}

//*******************************************************************
STDMETHODIMP CEnumGroupNames::Reset( void )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	CSLock(&m_pServer->m_cs);
   m_pos = m_pServer->m_groupMap.GetStartPosition();
	return S_OK;
}

//*******************************************************************
STDMETHODIMP CEnumGroupNames::Clone( IEnumString ** ppEnumString )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   USES_CONVERSION;
   *ppEnumString = NULL;
   CComEnumGroupNames* pEnumString = new CComEnumGroupNames;
   pEnumString->Initialize(m_pServer);

   return pEnumString->QueryInterface( IID_IEnumString, (LPVOID*)ppEnumString );
}


//*******************************************************************
// CEnumGroupIUnknwon implementation
//*******************************************************************
CEnumGroupIUnknown::CEnumGroupIUnknown()
{
   m_pos = NULL;
}

CEnumGroupIUnknown::~CEnumGroupIUnknown()
{
}


//*******************************************************************
void CEnumGroupIUnknown::Initialize(OPCServerBase* pServer)
{
	m_pServer=pServer;
	CSLock(&m_pServer->m_cs);
   m_pos = m_pServer->m_groupMap.GetStartPosition();
}

//*******************************************************************
STDMETHODIMP CEnumGroupIUnknown::Next(
                            ULONG celt,
                            IUnknown** ppIUnknown,
                            ULONG * pceltFetched )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   OPCGroupObject* pGroup = NULL;
   IUnknown* pUnk=0;
   LPVOID key = 0;
	CSLock(&m_pServer->m_cs);

   for( ULONG i = 0; i < celt && m_pos; )
   {
		m_pServer->m_groupMap.GetNextAssoc( m_pos, key, pGroup );
      HRESULT hr = pGroup->QueryInterface( IID_IUnknown, (LPVOID*)&pUnk );
      ppIUnknown[i]=pUnk;
      i++;
   }
   if( pceltFetched )
      *pceltFetched = i;
   return (celt==i) ? S_OK : S_FALSE;

}

//*******************************************************************
// just iterate celt times to skip those items
STDMETHODIMP CEnumGroupIUnknown::Skip( ULONG celt )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   OPCGroupObject* pGroup = NULL;
   LPVOID key = 0;
	CSLock(&m_pServer->m_cs);
   for( ULONG i = 0; i < celt && m_pos; )
   {
		m_pServer->m_groupMap.GetNextAssoc( m_pos, key, pGroup );
      i++;
	}
   return (celt==i) ? S_OK : S_FALSE;

}

//*******************************************************************
STDMETHODIMP CEnumGroupIUnknown::Reset( void )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	CSLock(&m_pServer->m_cs);
   m_pos = m_pServer->m_groupMap.GetStartPosition();
	return S_OK;
}

//*******************************************************************
STDMETHODIMP CEnumGroupIUnknown::Clone( IEnumUnknown** ppEnumIUnknown )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   USES_CONVERSION;
   *ppEnumIUnknown = NULL;
   CComEnumGroupIUnknown* pEnumIUnknown = new CComEnumGroupIUnknown;
   pEnumIUnknown->Initialize(m_pServer);

   return pEnumIUnknown->QueryInterface( IID_IEnumUnknown, (LPVOID*)ppEnumIUnknown );
}



//*************************************************************************
// return TRUE if String Matches Pattern --
// -- uses Visual Basic LIKE operator syntax
// CAUTION: Function is recursive
//*************************************************************************
BOOL MatchPattern( LPCTSTR String, LPCTSTR Pattern, BOOL bCaseSensitive )
{
    TCHAR   c, p, l;
    for (; ;)
    {
        switch (p = ConvertCase( *Pattern++, bCaseSensitive ) )
        {
        case 0:                             // end of pattern
            return *String ? FALSE : TRUE;  // if end of string TRUE

        case _T('*'):
            while (*String)
            {               // match zero or more char
                if (MatchPattern (String++, Pattern, bCaseSensitive))
                    return TRUE;
            }
            return MatchPattern (String, Pattern, bCaseSensitive );

        case _T('?'):
            if (*String++ == 0)             // match any one char
                return FALSE;               // not end of string
            break;

        case _T('['):
            // match char set
            if ( (c = ConvertCase( *String++, bCaseSensitive) ) == 0)
                return FALSE;                // syntax
            l = 0;
            if( *Pattern == _T('!') )  // match a char if NOT in set []
            {
                ++Pattern;

                while( (p = ConvertCase( *Pattern++, bCaseSensitive) )
                         != _T('\0') )
                {
                    if (p == _T(']'))     // if end of char set, then
                        break;            // no match found

                    if (p == _T('-'))
                    {   // check a range of chars?
                        p = ConvertCase( *Pattern, bCaseSensitive );
                        // get high limit of range
                        if (p == 0  ||  p == _T(']'))
                            return FALSE;     // syntax

                        if (c >= l  &&  c <= p)
                            return FALSE;     // if in range, return FALSE
                    }
                    l = p;
                    if (c == p)               // if char matches this element
                        return FALSE;         // return false
                }
            }
            else    // match if char is in set []
            {
                while( (p = ConvertCase( *Pattern++, bCaseSensitive) )
                         != _T('\0') )
                {
                    if (p == _T(']'))         // if end of char set, then
                        return FALSE;         // no match found

                    if (p == _T('-'))
                    {   // check a range of chars?
                        p = ConvertCase( *Pattern, bCaseSensitive );
                        // get high limit of range
                        if (p == 0  ||  p == _T(']'))
                            return FALSE;       // syntax

                        if (c >= l  &&  c <= p)
                            break;              // if in range, move on
                    }
                    l = p;
                    if (c == p)                 // if char matches this element
                        break;                  // move on
                }

                while (p  &&  p != _T(']'))     // got a match in char set
                    p = *Pattern++;             // skip to end of set
            }

            break;

        case _T('#'):
            c = *String++;
            if( !_istdigit( c ) )
                return FALSE;        // not a digit

            break;

        default:
            c = ConvertCase( *String++, bCaseSensitive );
            if( c != p )            // check for exact char
                return FALSE;                   // not a match

            break;
        }
    }
}
