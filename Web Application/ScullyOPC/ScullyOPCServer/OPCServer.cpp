// OPCServer.cpp : Implementation of COPCServer

#include "stdafx.h"
#include "OPCServer.h"
#include "DeviceManager.h"
#include <OPCProps.h>

extern CDeviceManager*		g_pDeviceManager;

const WORD MAJOR_VERSION = 2;
const WORD MINOR_VERSION = 0;
const WORD BUILD_NUMBER = 1;


// COPCServer
HRESULT COPCServer::FinalConstruct()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		return g_pDeviceManager->AddServer(this);
	}
	catch (_com_error& e)
	{
		if(e.Description().length())
			return Error((LPOLESTR) e.Description(),IID_IOPCServer);
		else
			return Error((LPOLESTR) e.ErrorMessage(),IID_IOPCServer);
	}
	catch (...)
	{
		return Error(_T("FinalConstruct Error"),IID_IOPCServer);
	}
}

void COPCServer::FinalRelease() 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	EnterCriticalSection(&m_cs);
	POSITION pos;

   // Release all groups owned by this server
   OPCGroupObject* pGroup = NULL;
   LPVOID key = 0;
   ULONG left = 0;
   pos = m_groupMap.GetStartPosition();
   while( pos )
   {
      m_groupMap.GetNextAssoc( pos, key, pGroup );
      ASSERT( pGroup );
      IUnknown* pUnk=0;
      HRESULT hr = pGroup->QueryInterface( IID_IUnknown, (LPVOID*)&pUnk );
      if( SUCCEEDED(hr) )
          CoDisconnectObject( pUnk, 0 );
      delete pGroup;
   }
   m_groupMap.RemoveAll();
	LeaveCriticalSection(&m_cs);
	g_pDeviceManager->RemoveServer(this);
}


//*******************************************************************
// Return the number of properties for this ItemID (if it is valid)
// ppVoid will be passed back in DoQueryAvailableProperties,
// so save the tag pointer in it.
HRESULT COPCServer::DoQueryNumProperties(
                              LPWSTR      szItemID,
                              DWORD     * pdwNumItems,
                              LPVOID    * ppVoid)
{
   CTag*	pTag = g_pDeviceManager->FindTag( szItemID );
   if( pTag 
	&& pTag->m_bLeaf)
   {
      *ppVoid = (LPVOID) pTag;
      *pdwNumItems = 9;
      return S_OK;
   }

   return OPC_E_UNKNOWNITEMID;
}

//*******************************************************************
// Return the properties for this ItemID (if it is valid)
// pVoid contains the tag pointer if DoQueryNumProperties returned
// successfully (If it failed, this function is not called)
HRESULT COPCServer::DoQueryAvailableProperties(
                              LPWSTR      szItemID,
                              DWORD       dwNumItems,
                              LPVOID      pVoid,
                              DWORD     * pPropertyIDs,
                              LPWSTR    * pDescriptions,
                              VARTYPE   * pvtDataTypes)
{
   USES_CONVERSION;

	CSLock	Lock(&g_pDeviceManager->m_cs);
	CTag*	pTag=g_pDeviceManager->FindTag(szItemID);
   if( pTag == NULL
	&& pTag->m_bLeaf )
      return OPC_E_UNKNOWNITEMID;

   CString description;
   DWORD index=0;

   description = _T("Item Canonical DataType");
   pPropertyIDs[index] = OPC_PROP_CDT;
   pDescriptions[index] = (LPWSTR)CoTaskMemAlloc(2*description.GetLength()+2);
   lstrcpy( pDescriptions[index], T2OLE(description.GetBuffer(0)) );
   pvtDataTypes[index] = VT_I2;

   index++;
   if( index==dwNumItems )
		return S_OK;
   description = _T("Item Value");
   pPropertyIDs[index] = OPC_PROP_VALUE;
   pDescriptions[index] = (LPWSTR)CoTaskMemAlloc(2*description.GetLength()+2);
   wcscpy( pDescriptions[index], T2OLE(description.GetBuffer(0)) );
   pvtDataTypes[index] = pTag->m_NativeType;

   index++;
   if( index==dwNumItems )
		return S_OK;
   description = _T("Item Quality");
   pPropertyIDs[index] = OPC_PROP_QUALITY;
   pDescriptions[index] = (LPWSTR)CoTaskMemAlloc(2*description.GetLength()+2);
   wcscpy( pDescriptions[index], T2OLE(description.GetBuffer(0)) );
   pvtDataTypes[index] = VT_I2;

   index++;
   if( index==dwNumItems )
		return S_OK;
   description = _T("Item Timestamp");
   pPropertyIDs[index] = OPC_PROP_TIME;
   pDescriptions[index] = (LPWSTR)CoTaskMemAlloc(2*description.GetLength()+2);
   wcscpy( pDescriptions[index], T2OLE(description.GetBuffer(0)) );
   pvtDataTypes[index] = VT_DATE;

   index++;
   if( index==dwNumItems )
		return S_OK;
   description = _T("Item Access Rights");
   pPropertyIDs[index] = OPC_PROP_RIGHTS;
   pDescriptions[index] = (LPWSTR)CoTaskMemAlloc(2*description.GetLength()+2);
   wcscpy( pDescriptions[index], T2OLE(description.GetBuffer(0)) );
   pvtDataTypes[index] = VT_I4;

   index++;
   if( index==dwNumItems )
		return S_OK;
   description = _T("Item Description");
   pPropertyIDs[index] = OPC_PROP_DESC;
   pDescriptions[index] = (LPWSTR)CoTaskMemAlloc(2*description.GetLength()+2);
   wcscpy( pDescriptions[index], T2OLE(description.GetBuffer(0)) );
   pvtDataTypes[index] = VT_BSTR;

   index++;
   if( index==dwNumItems )
		return S_OK;
   description = _T("Item Units");
   pPropertyIDs[index] = OPC_PROP_UNIT;
   pDescriptions[index] = (LPWSTR)CoTaskMemAlloc(2*description.GetLength()+2);
   wcscpy( pDescriptions[index], T2OLE(description.GetBuffer(0)) );
   pvtDataTypes[index] = VT_BSTR;

   index++;
   if( index==dwNumItems )
		return S_OK;
   description = _T("Item Maximum");
   pPropertyIDs[index] = OPC_PROP_HIRANGE;
   pDescriptions[index] = (LPWSTR)CoTaskMemAlloc(2*description.GetLength()+2);
   wcscpy( pDescriptions[index], T2OLE(description.GetBuffer(0)) );
   pvtDataTypes[index] =  pTag->m_NativeType;

   index++;
   if( index==dwNumItems )
		return S_OK;
   description = _T("Item Minimum");
   pPropertyIDs[index] = OPC_PROP_LORANGE;
   pDescriptions[index] = (LPWSTR)CoTaskMemAlloc(2*description.GetLength()+2);
   wcscpy( pDescriptions[index], T2OLE(description.GetBuffer(0)) );
   pvtDataTypes[index] =  pTag->m_NativeType;

   return S_OK;
}

//*******************************************************************
HRESULT COPCServer::DoGetItemProperties(
                              LPWSTR      szItemID,
                              DWORD       dwNumItems,
                              DWORD     * pPropertyIDs,
                              VARIANT   * ppvData,
                              HRESULT   * ppErrors)
{
   CString itemName( szItemID );

	CSLock	Lock(&g_pDeviceManager->m_cs);
   CTag* pTag = g_pDeviceManager->FindTag(szItemID);
   if( pTag == NULL
	&& pTag->m_bLeaf )
      return OPC_E_UNKNOWNITEMID;
 
	USES_CONVERSION;
   DATE date;
   WORD dosDate=0, dosTime=0;
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      ppErrors[index] = S_OK;
      switch(pPropertyIDs[index])
      {
         case OPC_PROP_CDT:
            ppvData[index].vt = VT_I2;
            ppvData[index].iVal = pTag->m_NativeType;
            break;
         case OPC_PROP_VALUE:
            VariantCopy( &ppvData[index], &pTag->m_Value );
            break;
         case OPC_PROP_QUALITY:
            ppvData[index].vt = VT_I2;
            ppvData[index].iVal = pTag->m_wQuality;
            break;
         case OPC_PROP_TIME:
            ppvData[index].vt = VT_DATE;
            FileTimeToDosDateTime( &pTag->m_Timestamp, &dosDate, &dosTime);
            DosDateTimeToVariantTime( dosDate, dosTime, &date);
            ppvData[index].date = date;
            break;
         case OPC_PROP_RIGHTS:
            ppvData[index].vt = VT_I4;
            ppvData[index].lVal = pTag->m_dwAccessRights;
            break;
         case OPC_PROP_DESC:
            ppvData[index].vt = VT_BSTR;
            ppvData[index].bstrVal = pTag->m_oDescription.AllocSysString();
            break;

			case OPC_PROP_UNIT:
			{
				ppvData[index].vt = VT_BSTR;
				ppvData[index].bstrVal = pTag->m_oUnits.AllocSysString();
				break;
			}

         case OPC_PROP_HIRANGE:
            break;

         case OPC_PROP_LORANGE:
            break;
      }
   }

   return S_OK;
}

//*******************************************************************
HRESULT COPCServer::DoLookupItemIDs(
                              LPWSTR      szItemID,
                              DWORD       dwNumItems,
                              DWORD     * pPropertyIDs,
                              LPWSTR    * pszNewItemIDs,
                              HRESULT   * pErrors)
{
	CSLock	Lock(&g_pDeviceManager->m_cs);

	CString itemID;
	
   CTag* pTag = g_pDeviceManager->FindTag( szItemID );
   if( pTag == NULL 
	&& pTag->m_bLeaf)
      return OPC_E_UNKNOWNITEMID;

	for(DWORD index=0;index < dwNumItems;index++)
	{
      pErrors[index] = S_OK;
      switch(pPropertyIDs[index])
      {
         case OPC_PROP_CDT:
				itemID=_T("Data Type");
            break;
         case OPC_PROP_VALUE:
				itemID=_T("Value");
            break;
         case OPC_PROP_QUALITY:
				itemID=_T("Quality");
            break;
         case OPC_PROP_TIME:
				itemID=_T("Time");
            break;
         case OPC_PROP_RIGHTS:
				itemID=_T("Rights");
            break;
         case OPC_PROP_DESC:
				itemID=_T("Description");
            break;
			case OPC_PROP_UNIT:
				itemID=_T("Units");
				break;
      }
		pszNewItemIDs[index]=(LPWSTR) CoTaskMemAlloc(sizeof(TCHAR) * (itemID.GetLength()+1));
		if(!pszNewItemIDs[index])
	      return E_OUTOFMEMORY;
		lstrcpy(pszNewItemIDs[index],itemID.GetBuffer(0));
	}

   return S_OK;
}


//*******************************************************************
HRESULT COPCServer::DoGetStatus( OPCSERVERSTATUS *pServerStatus)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())

   CSLock wait( &m_cs );
   pServerStatus->ftStartTime = theApp.m_ServerStartTime;
   CoFileTimeNow( &pServerStatus->ftCurrentTime );
   pServerStatus->ftLastUpdateTime = m_lastUpdateTime;
   pServerStatus->dwServerState = OPC_STATUS_RUNNING;
   pServerStatus->dwGroupCount = m_groupMap.GetCount();
   pServerStatus->dwBandWidth = 0;
   pServerStatus->wMajorVersion = MAJOR_VERSION;
   pServerStatus->wMinorVersion = MINOR_VERSION;
   pServerStatus->wBuildNumber = BUILD_NUMBER;
   CString vendor;
   if( !vendor.LoadString(IDS_VENDOR_INFO) )
   {
      TRACE(_T("Cannot load vendor info string\n"));
   }
   pServerStatus->szVendorInfo = (LPWSTR)CoTaskMemAlloc(2*vendor.GetLength()+2);
   USES_CONVERSION;
   wcscpy( pServerStatus->szVendorInfo, T2OLE(vendor.GetBuffer(0)) );

   return S_OK;
}

//*******************************************************************
OPCNAMESPACETYPE COPCServer::DoQueryOrganization()
{
	return OPC_NS_HIERARCHIAL;
}

//*******************************************************************
HRESULT COPCServer::DoChangeBrowsePosition(
                              OPCBROWSEDIRECTION dwBrowseDirection,
                              LPCWSTR           szString)
{
	CSLock	Lock(&g_pDeviceManager->m_cs);

	// Browse up
   if( dwBrowseDirection == OPC_BROWSE_UP )
	{
		if( m_pCurrentTag)
		{
			if( m_pCurrentTag->m_pParent != NULL)
			{
				m_pCurrentTag=m_pCurrentTag->m_pParent;
				return S_OK;
			}
			else
				return E_FAIL;    // cannot go up any more
		}
		else
			return E_FAIL;
	}
	// browse into named group
   else if( dwBrowseDirection == OPC_BROWSE_DOWN )  
	{
		if(m_pCurrentTag == NULL)
			return E_FAIL;

		if(!m_pCurrentTag->m_Branch.GetCount())
			return E_FAIL;

      CString oName( szString );
      POSITION pos = m_pCurrentTag->m_Branch.GetHeadPosition();
      while( pos )
      {
         CTag* pTag = m_pCurrentTag->m_Branch.GetNext( pos );
         if( pTag->m_oName == oName )
         {
            m_pCurrentTag = pTag;
            return S_OK;
         }
      }
   }

	// Browse to specific position
	else if( dwBrowseDirection == OPC_BROWSE_TO)
	{
		m_pCurrentTag=g_pDeviceManager->m_pRoot;

		CString oName( szString );

		int delimiter = oName.Find( _T('.') );

		while( delimiter != -1 )
		{
			HRESULT hr=DoChangeBrowsePosition(OPC_BROWSE_DOWN,oName.Left( delimiter) );
			if(!SUCCEEDED( hr ))
				return hr;
			oName=oName.Mid( delimiter+1 );
			delimiter=oName.Find(_T('.'));
		}

		if(!oName.GetLength())
			return S_OK;

		return DoChangeBrowsePosition(OPC_BROWSE_DOWN,oName);
	}

   return E_INVALIDARG;
}

//*******************************************************************
// Should just create an enumerator, not the rest...
HRESULT COPCServer::DoBrowseOPCItemIDs(
                              OPCBROWSETYPE     dwBrowseFilterType,
                              LPCWSTR           szFilterCriteria,
                              VARTYPE           vtDataTypeFilter,
                              DWORD             dwAccessRightsFilter,
                              LPENUMSTRING *    ppIEnumString)
{
	CSLock Lock(&g_pDeviceManager->m_cs);

	if(m_pCurrentTag == NULL)
		m_pCurrentTag=g_pDeviceManager->m_pRoot;

   CComEnumItemIDs*	pEnumString = new CComEnumItemIDs;

   if(!pEnumString) 
      return E_OUTOFMEMORY;

	pEnumString->Initialize(m_pCurrentTag,
									dwBrowseFilterType,
									szFilterCriteria,
									vtDataTypeFilter,
									dwAccessRightsFilter);

   return pEnumString->QueryInterface( IID_IEnumString, (LPVOID*)ppIEnumString );
}

//*******************************************************************
// if szItemDataID is NULL, return the current location
HRESULT COPCServer::DoGetItemID(
                              LPWSTR      szItemDataID,
                              LPWSTR *    szItemID)
{
   BOOL found = FALSE;

	CString path((LPCTSTR) m_pCurrentTag->GetPathName());

   CString name( szItemDataID );

	if(name.GetLength())
	{
		if(path.GetLength())
			path=path+name;
		else
			path=name;

		CTag* pTag=g_pDeviceManager->FindTag(path.GetBuffer(0));
		
		if( pTag == NULL )
			return OPC_E_UNKNOWNITEMID;
	}

   *szItemID = (LPWSTR)CoTaskMemAlloc( 2*(path.GetLength()+1) );
   USES_CONVERSION;
   wcscpy( *szItemID, T2OLE(path.GetBuffer(0)) );

   return S_OK;
}

//*******************************************************************
// CEnumItemIDs implementation
//*******************************************************************
CEnumItemIDs::CEnumItemIDs()
{
}

CEnumItemIDs::~CEnumItemIDs()
{
}


//*******************************************************************
void CEnumItemIDs::Initialize(CTag*					pTag,
                              OPCBROWSETYPE     dwFilterType,
                              LPCWSTR           szCriteria,
                              VARTYPE           vtTypeFilter,
                              DWORD             dwRightsFilter)
{
	m_pCurrentTag = pTag;
   m_BrowseFilterType = dwFilterType;
   m_oFilterCriteria = szCriteria;
   m_DataTypeFilter = vtTypeFilter;
   m_dwAccessRightsFilter = dwRightsFilter;

	if(m_dwAccessRightsFilter == 0)
		m_dwAccessRightsFilter=OPC_READABLE | OPC_WRITEABLE;

   switch( m_BrowseFilterType )
   {
      case OPC_BRANCH:
         m_pos = m_pCurrentTag->m_Branch.GetHeadPosition();
         break;
      case OPC_LEAF:
         m_pos = m_pCurrentTag->m_Leaf.GetHeadPosition();
         break;
      case OPC_FLAT:
         // Added 2.0
         AddTags( pTag );
         m_pos = m_Paths.GetHeadPosition();
         break;
   }
}

//*******************************************************************
// Recursive function to add tag names from all groups to a list.
// This is only called when browsing OPC_FLAT
// Added 2.0
void  CEnumItemIDs::AddTags( CTag* pTag )
{
   // First add full path names for this tag's leaf tags
   CString oPath(pTag->GetPathName());
   POSITION pos = pTag->m_Leaf.GetHeadPosition();
   while( pos )
   {
      CTag* pLeaf = pTag->m_Leaf.GetNext( pos );
      CString oName( oPath + pLeaf->m_oName );
      m_Paths.AddTail( oName );
   }

   // And recurse into the branch tags
   pos = pTag->m_Branch.GetHeadPosition();
   while( pos )
   {
      AddTags( pTag->m_Branch.GetNext( pos ) );
   }
}


//*******************************************************************
// Note: No protection agains an Scully being deleted during
//       the middle of browsing.  Deemed to much trouble at this time.
//			If warranted, the current position in the list might be
//			determined by saving the current path and then doing a
//       FindTag.  If the Tag is deleted, the browseing is complete.
STDMETHODIMP CEnumItemIDs::Next(
                            ULONG celt,
                            LPOLESTR * ppStrings,
                            ULONG * pceltFetched )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	for( ULONG i = 0; i < celt && m_pos; )
	{
		CString oName;
		switch( m_BrowseFilterType )
		{
			case OPC_BRANCH:
			{
				CTag* pTag = m_pCurrentTag->m_Branch.GetNext( m_pos );

				oName = pTag->m_oName;
			}
			break;

			case OPC_LEAF:
			{
				CTag* pTag = m_pCurrentTag->m_Leaf.GetNext( m_pos );
				if((pTag->m_dwAccessRights & m_dwAccessRightsFilter) == 0)
					continue;

				if(m_DataTypeFilter != VT_EMPTY 
				&& m_DataTypeFilter != pTag->m_NativeType)
					continue;
				oName = pTag->m_oName;
			}
			break;

			case OPC_FLAT:
			{
				oName = m_Paths.GetNext( m_pos );
			}
			break;
		}

		if( m_oFilterCriteria.IsEmpty()
		|| MatchPattern( oName, m_oFilterCriteria, FALSE) )
		{
			ppStrings[i] = (LPWSTR) CoTaskMemAlloc(2*oName.GetLength()+2);
			USES_CONVERSION;
			wcscpy( ppStrings[i], T2OLE(oName.GetBuffer(0)) );
			i++;
		}
	}

	if( pceltFetched )
		*pceltFetched = i;

	return (celt==i) ? S_OK : S_FALSE;
}

//*******************************************************************
// just iterate celt times to skip those items
STDMETHODIMP CEnumItemIDs::Skip( ULONG celt )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	for( ULONG i = 0; i < celt && m_pos; )
	{
		CString oName;
		switch( m_BrowseFilterType )
		{
			case OPC_BRANCH:
			{
				CTag* pTag = m_pCurrentTag->m_Branch.GetNext( m_pos );
				oName = pTag->m_oName;
			}
			break;

			case OPC_LEAF:
			{
				CTag* pTag = m_pCurrentTag->m_Leaf.GetNext( m_pos );
				if((pTag->m_dwAccessRights & m_dwAccessRightsFilter) == 0)
					continue;
				oName = pTag->m_oName;
			}
			break;

			case OPC_FLAT:
			{
				oName = m_Paths.GetNext( m_pos );
			}
			break;
		}
		if( m_oFilterCriteria.IsEmpty()
		|| MatchPattern( oName, m_oFilterCriteria, FALSE) )
		{
			i++;
		}
	}

	return (celt==i) ? S_OK : S_FALSE;
}

//*******************************************************************
STDMETHODIMP CEnumItemIDs::Reset( void )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   switch( m_BrowseFilterType )
   {
      case OPC_BRANCH:
         m_pos = m_pCurrentTag->m_Branch.GetHeadPosition();
         break;
      case OPC_LEAF:
         m_pos = m_pCurrentTag->m_Leaf.GetHeadPosition();
         break;
      case OPC_FLAT:
         m_pos = m_Paths.GetHeadPosition();
         break;
   }
   return S_OK;
}

//*******************************************************************
STDMETHODIMP CEnumItemIDs::Clone( IEnumString ** ppEnumString )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   USES_CONVERSION;
   *ppEnumString = NULL;
   CComEnumItemIDs* pEnumString = new CComEnumItemIDs;
   pEnumString->Initialize( m_pCurrentTag,
                            m_BrowseFilterType,
                            T2OLE(m_oFilterCriteria.GetBuffer(0)),
                            m_DataTypeFilter,
                            m_dwAccessRightsFilter);

   return pEnumString->QueryInterface( IID_IEnumString, (LPVOID*)ppEnumString );
}


