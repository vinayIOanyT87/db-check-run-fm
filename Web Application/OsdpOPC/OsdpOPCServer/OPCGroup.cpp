/******************************************************************************

	FILE NAME:		OPCGroup.cpp


	PURPOSE:			Implementation of COPCGroup

	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
		03/30/2005	WG				Changed to lock IO Critical Section
										before testing for changed data in DoUpdateGroup

		12/03/2009	W.Gray		7.4.6.0 changed DoAddItems to not lock DeviceManager m_cs.
										this is accceptable if the group m_cs is locked after FindTag
										because not tag will be deleted until each group has been
										locked and searched to insure tag isn't in item map. (WI 9637)

*******************************************************************************/


#include "stdafx.h"
#include "OPCServer.h"
#include <math.h>
#include "OsdpControllerManager.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

extern COsdpControllerManager		g_OsdpControllerManager;

//*******************************************************************
//    class OPCGroup
//*******************************************************************
// Initialize does the real work
COPCGroup::COPCGroup()
{
}

//*******************************************************************
COPCGroup::~COPCGroup()
{
   // End the data thread
   m_active = FALSE;
   m_running = FALSE;
   SetEvent( m_hTimer );
   if( m_hDataThread )
	{
      WaitForSingleObject( m_hDataThread, INFINITE );
		CloseHandle(m_hDataThread);
		m_hDataThread=NULL;
	}

   // remove all items
   COPCItem* pItem = NULL;
   LPVOID key = 0;
   POSITION pos = m_itemMap.GetStartPosition();
   while( pos )
   {
      m_itemMap.GetNextAssoc( pos, key, pItem );
      delete pItem;
   }
   m_itemMap.RemoveAll();
}


//*******************************************************************
// OPCGroup overrides
//*******************************************************************
void COPCGroup::DoSetUpdateRate( DWORD newUpdateRate )
{
   m_updateRate = newUpdateRate;
   if( m_updateRate < 10 )
      m_updateRate = 10;
}

//*******************************************************************
// DoRead performs Sync and ASync read
//    Values are put into the OPCITEMSTATE structures,
//    not the COPCItems themselves
//    (there is no interaction with subscriptions)
//*******************************************************************
HRESULT COPCGroup::DoRead(
    OPCDATASOURCE		dwSource,
    DWORD				dwNumItems,
    COPCItem			** ppItems,
    OPCITEMSTATE		* pItemValues,
    HRESULT				* pErrors)
{
	CSLock wait(&m_cs);

   HRESULT hr = S_OK;
	CIO*	pIO = NULL;

	// Reset m_bCurrent on all Tags
   for( DWORD index=0; index < dwNumItems; index++ )
   {
      if( pErrors[index] == S_OK )
      {
         // server handle is the address of its Item
         COsdpControllerItem* pItem = (COsdpControllerItem*) ppItems[index];
         ASSERT( pItem );

			CTag*	pTag=pItem->m_pTag;
			if(!pTag)
				continue;
			
			pTag->m_bCurrent=FALSE;
		}
	}

   // verify all server handles
   for( DWORD index=0; index < dwNumItems; index++ )
   {
      if( pErrors[index] == S_OK )
      {
         // server handle is the address of its Item
         COsdpControllerItem* pItem = (COsdpControllerItem*) ppItems[index];
         ASSERT( pItem );

         pItemValues[index].hClient = pItem->m_ClientHandle;

			CTag*	pTag=pItem->m_pTag;
			if(!pTag)
			{
				pItemValues[index].wQuality = OPC_QUALITY_CONFIG_ERROR;
			   CoFileTimeNow(&pItemValues[index].ftTimeStamp);
			}
			else
			{
				if(pTag->m_dwAccessRights & OPC_READABLE)
				{
					if(dwSource == OPC_DS_DEVICE)
					{
						if(pIO != pTag->m_pIO)
						{
							if(pIO)
								LeaveCriticalSection(&pIO->m_cs);
							pIO=pTag->m_pIO;
							if(pIO)
								EnterCriticalSection(&pIO->m_cs);
						}
						if(pIO)
							pIO->ReadTag(pTag);
						pItemValues[index].wQuality = pTag->m_wQuality;
					}
					else
					{
						if(m_active && pItem->m_bActive )
							pItemValues[index].wQuality = pItem->m_wQuality;
						else
							pItemValues[index].wQuality = OPC_QUALITY_OUT_OF_SERVICE;
					}				
					VariantCopy( &pItemValues[index].vDataValue, &pTag->m_Value );
					pItemValues[index].ftTimeStamp = pTag->m_Timestamp;
				}
				else
				{
					pItemValues[index].wQuality = pTag->m_wQuality;
					VariantCopy( &pItemValues[index].vDataValue, &pTag->m_Value );
					pItemValues[index].ftTimeStamp = pTag->m_Timestamp;
				}
			}

			pItemValues[index].wReserved = 0;
      }
      else
      {
         hr = S_FALSE;
      }
   }

	if(pIO)
		LeaveCriticalSection(&pIO->m_cs);

   return hr;
}

//*******************************************************************
// This is called from both Sync and ASync calls
HRESULT COPCGroup::DoWrite(
    DWORD        dwNumItems,
    COPCItem  ** ppItems,
    VARIANT    * pItemValues,
    HRESULT    * pErrors)
{
	CSLock wait(&m_cs);

   HRESULT hr = S_OK;
	CIO*	pIO = NULL;

   for( DWORD index=0; index < dwNumItems; index++ )
   {

      if( pErrors[index] == S_OK )
      {
         // server handle is the address of its Item
         COsdpControllerItem* pItem = (COsdpControllerItem*)ppItems[index];
         ASSERT( pItem );

			CTag*	pTag=pItem->m_pTag;
			if(!pTag)
				pErrors[index] = E_FAIL;
			else
			{
	         pTag->m_Value = pItemValues[index];
				CoFileTimeNow( &pTag->m_Timestamp );

				if(pIO != pTag->m_pIO)
				{
					if(pIO)
						LeaveCriticalSection(&pIO->m_cs);
					pIO=pTag->m_pIO;
					if(pIO)
						EnterCriticalSection(&pIO->m_cs);
				}

				if(pIO)
					pErrors[index]=pIO->WriteTag(pTag);
			}				
      }
      else
      {
         hr = S_FALSE;
      }
   }

	if(pIO)
		LeaveCriticalSection(&pIO->m_cs);
	
	return hr;
}

//*******************************************************************
// This is called from the group's scan thread (at its update rate)
BOOL COPCGroup::DoUpdateGroup()
{
   BOOL				changed = FALSE;

   CSLock wait( &m_cs );

   // update items from tags
   COsdpControllerItem* pItem = NULL;
   LPVOID key = 0;
   POSITION pos = m_itemMap.GetStartPosition();
   while( pos )
   {
      m_itemMap.GetNextAssoc( pos, key, (COPCItem*&)pItem );
		changed=FALSE;

		if(!pItem->m_pTag)
		{
			if(pItem->m_wQuality != OPC_QUALITY_CONFIG_ERROR)
			{
				pItem->m_wQuality=OPC_QUALITY_CONFIG_ERROR;
	         pItem->m_bChanged = changed = TRUE;
			   CoFileTimeNow(&pItem->m_Timestamp);
			}
			continue;
		}

		if(!(pItem->m_pTag->m_dwAccessRights & OPC_READABLE))
			continue;

		// mark changed values to be sent to clients
      // MFC's COleVariant doesn't handle the VT_UI2 case...
      if(pItem->m_Value.vt == VT_UI2)
		{
			if(pItem->m_Value.uiVal != pItem->m_pTag->m_Value.uiVal)
				changed=TRUE;
		}

		// MFC's COleVariant doesn't handle the VT_UI4 case...
		else if(pItem->m_pTag->m_Value.vt == VT_UI4)
		{
			if(pItem->m_Value.ulVal != pItem->m_pTag->m_Value.ulVal)
				changed=TRUE;
		}

      // MFC's COleVariant doesn't handle the VT_I1 case...
      else if(pItem->m_pTag->m_Value.vt == VT_I1)
		{
			if(pItem->m_Value.iVal != pItem->m_pTag->m_Value.iVal)
				changed=TRUE;
		}

      else if(pItem->m_Value == pItem->m_pTag->m_Value )
         ;  // only "operator==" works, not "operator !="
      else  // the values are different
			changed=TRUE;

		if(pItem->m_dwUpdateSequence != pItem->m_pTag->m_dwUpdateSequence)
		{
			changed=TRUE;
			pItem->m_dwUpdateSequence=pItem->m_pTag->m_dwUpdateSequence;
		}

		if(changed)
		{
			// ADD ANY CHECKS FOR DEADBANDING HERE
         pItem->m_Value = pItem->m_pTag->m_Value;
      }
      pItem->m_bChanged = changed;

      if( pItem->m_wQuality != pItem->m_pTag->m_wQuality )
      {
         pItem->m_wQuality = pItem->m_pTag->m_wQuality;
         pItem->m_bChanged = changed = TRUE;
      }

      if( pItem->m_bChanged )
         pItem->m_Timestamp = pItem->m_pTag->m_Timestamp;
   }
   return changed;
}

//*******************************************************************
HRESULT COPCGroup::DoAddItems(
    DWORD            dwNumItems,
    OPCITEMDEF     * pItemArray,
    OPCITEMRESULT  * pAddResults,
    HRESULT        * pErrors)
{
   CSLock wait( &m_cs );

	HRESULT hr = S_OK;

   for(DWORD i=0; i < dwNumItems; i++ )
   {
      // search for this tag
      CString itemName( pItemArray[i].szItemID );
		CTag*	pTag=g_OsdpControllerManager.FindTag( itemName.GetBuffer(0) );
      if( pTag == NULL
		|| !pTag->m_bLeaf )
      {
         pErrors[i] = OPC_E_INVALIDITEMID;
         hr = S_FALSE;
      }
      else
      {
         // Create the OPC item
         COsdpControllerItem* pItem = new COsdpControllerItem(pTag,itemName.GetBuffer(0));
         if( pItem )
         {
            pErrors[i] = S_OK;
				pItem->m_strID = itemName;
            pItem->m_bActive = pItemArray[i].bActive;
            pItem->m_ClientHandle = pItemArray[i].hClient;
            pItem->m_ClientType = pItemArray[i].vtRequestedDataType;
				if(pItem->m_ClientType != VT_EMPTY
				&& pItem->m_ClientType != VT_I2
				&& pItem->m_ClientType != VT_I4
				&& pItem->m_ClientType != VT_R4
				&& pItem->m_ClientType != VT_R8
				&& pItem->m_ClientType != VT_DATE
				&& pItem->m_ClientType != VT_BSTR
				&& pItem->m_ClientType != VT_BOOL
				&& pItem->m_ClientType != VT_I1
				&& pItem->m_ClientType != VT_UI1
				&& pItem->m_ClientType != VT_UI2
				&& pItem->m_ClientType != VT_UI4
				&& pItem->m_ClientType != VT_INT
				&& pItem->m_ClientType != VT_UINT
				&& pItem->m_ClientType != VT_CY)
				{
					pErrors[i] = OPC_E_BADTYPE;
					hr=S_FALSE;
					delete pItem;
					continue;
				}
            if( pItem->m_ClientType == VT_EMPTY )
               pItem->m_ClientType = pItem->m_pTag->m_NativeType;
            pAddResults[i].hServer = (OPCHANDLE) pItem;
            pAddResults[i].vtCanonicalDataType = pItem->m_pTag->m_NativeType;
            pAddResults[i].wReserved = 0;
            pAddResults[i].dwAccessRights = pItem->m_pTag->m_dwAccessRights;
            pAddResults[i].dwBlobSize = 0;
            pAddResults[i].pBlob = NULL;
            m_itemMap.SetAt( (LPVOID)pItem, pItem );
				if(pItem->m_bActive
				&& pTag->m_dwAccessRights & OPC_READABLE
				&& pTag->m_pIO)
					pTag->m_pIO->AddTagToScanList(pTag,m_updateRate);
         }
         else
         {
            pErrors[i] = E_OUTOFMEMORY;
            hr = S_FALSE;
         }
      }
   }
   return hr;
}

//*******************************************************************
HRESULT COPCGroup::DoValidateItems(
    DWORD             dwNumItems,
    OPCITEMDEF      * pItemArray,
    OPCITEMRESULT   * pValidationResults,
    HRESULT         * pErrors)
{
	CSLock	Lock(&g_OsdpControllerManager.m_cs);

	HRESULT hr = S_OK;

   for(DWORD i=0; i<dwNumItems; i++ )
   {
      // search for this tag
      CString itemName( pItemArray[i].szItemID );
      CTag*	pTag = g_OsdpControllerManager.FindTag( itemName.GetBuffer(0) );
      if( pTag == NULL
		|| !pTag->m_bLeaf )
      {
         pErrors[i] = OPC_E_UNKNOWNITEMID;
         hr = S_FALSE;
      }
      else
      {
         pValidationResults[i].hServer = NULL;
         pValidationResults[i].vtCanonicalDataType = pTag->m_NativeType;
         pValidationResults[i].wReserved = 0;
         pValidationResults[i].dwAccessRights = pTag->m_dwAccessRights;
         pValidationResults[i].dwBlobSize = 0;
         pValidationResults[i].pBlob = NULL;
      }
   }
   return hr;
}

//*******************************************************************
HRESULT COPCGroup::DoRemoveItems(
    DWORD        dwNumItems,
    COPCItem  ** ppItems,
    HRESULT    * pErrors)
{
   HRESULT hr = S_OK;
   for( DWORD index=0; index < dwNumItems; index++ )
   {
      if( pErrors[index] == S_OK )
      {
         // server handle is the address of its Item
         COsdpControllerItem* pItem = (COsdpControllerItem*)ppItems[index];
         ASSERT( pItem );

         // remove from map of all items
         VERIFY( m_itemMap.RemoveKey( (LPVOID)pItem ) );

         delete pItem;
      }
      else
         hr = S_FALSE;
   }
   return hr;
}

//*******************************************************************
HRESULT COPCGroup::DoSetActiveState(
    DWORD        dwNumItems,
    COPCItem  ** ppItems,
    BOOL         bActive,
    HRESULT    * pErrors)
{
   HRESULT hr = S_OK;
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      if( pErrors[index] == S_OK )
      {
         // server handle is the address of its Item
         COsdpControllerItem* pItem = (COsdpControllerItem*)ppItems[index];
         ASSERT( pItem );
         BOOL change = (pItem->m_bActive != bActive);
         pItem->m_bActive = bActive;
         if( pItem->m_bActive && change )
         {
				// set item change flag
				pItem->m_bChanged = TRUE;
				if(pItem->m_pTag
				&& pItem->m_pTag->m_dwAccessRights & OPC_READABLE
				&& pItem->m_pTag->m_pIO)
				{
					pItem->m_wQuality = OPC_QUALITY_GOOD;
					pItem->m_pTag->m_pIO->AddTagToScanList(pItem->m_pTag,m_updateRate);
				}
         }
         else if( change )
         {
				pItem->m_wQuality = OPC_QUALITY_OUT_OF_SERVICE | OPC_QUALITY_BAD;
         }
      }
      else
         hr = S_FALSE;
   }
   return hr;
}

//*******************************************************************
HRESULT COPCGroup::DoSetClientHandles(
    DWORD        dwNumItems,
    COPCItem  ** ppItems,
    OPCHANDLE  * phClient,
    HRESULT    * pErrors)
{
   HRESULT hr = S_OK;

   CSLock wait( &m_cs );
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      if( pErrors[index] == S_OK )
      {
         // server handle is the address of its Item
         COsdpControllerItem* pItem = (COsdpControllerItem*)ppItems[index];
         ASSERT( pItem );
         pItem->m_ClientHandle = phClient[index];
      }
      else
         hr = S_FALSE;
   }

   return hr;
}

//*******************************************************************
HRESULT COPCGroup::DoSetDatatypes(
    DWORD        dwNumItems,
    COPCItem  ** ppItems,
    VARTYPE    * pRequestedDatatypes,
    HRESULT    * pErrors)
{
   HRESULT hr = S_OK;

   CSLock wait( &m_cs );
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      if( pErrors[index] == S_OK )
      {
         // server handle is the address of its Item
         COsdpControllerItem* pItem = (COsdpControllerItem*)ppItems[index];
         ASSERT( pItem );
         pItem->m_ClientType = pRequestedDatatypes[index];
         if( pItem->m_ClientType == VT_EMPTY )    // if none, use Double
            pItem->m_ClientType = VT_R8;
      }
      else
         hr = S_FALSE;
   }

   return hr;
}

//*******************************************************************
HRESULT COPCGroup::DoCopyItems(LPCWSTR szName)
{
   HRESULT hr = S_OK;
	COPCGroup* pGroup=m_parent->FindNamedGroup(szName);

	CSLock wait( &m_cs );

   COsdpControllerItem* pItem = NULL;
   LPVOID key = 0;
   POSITION pos = m_itemMap.GetStartPosition();
   while( pos )
   {
      m_itemMap.GetNextAssoc( pos, key, (COPCItem*&) pItem );
		OPCITEMDEF	OpcItemDef;
		OpcItemDef.szItemID=pItem->m_strID.GetBuffer(0);
		OpcItemDef.bActive=pItem->m_bActive;
		OpcItemDef.hClient=pItem->m_ClientHandle;
		OpcItemDef.dwBlobSize=0;
		OpcItemDef.pBlob=NULL;
		OpcItemDef.vtRequestedDataType=pItem->m_ClientType;
		OPCITEMRESULT	OpcItemResult;
		HRESULT	Error;

		hr=pGroup->DoAddItems(1,&OpcItemDef,&OpcItemResult,&Error);
		if(FAILED(hr))
			break;
	}

	return hr;
}

IUnknown* COPCGroup::DoCreateEnumerator()
{
   CComEnumItemAttributes* pEnumerator = new CComEnumItemAttributes;
   if( pEnumerator )
      pEnumerator->Initialize(this);
   return pEnumerator;
}

//*******************************************************************
// CEnumItemAttributes implementation
//*******************************************************************
CEnumItemAttributes::CEnumItemAttributes()
{
   m_pos = NULL;
   m_parent = NULL;
}

CEnumItemAttributes::~CEnumItemAttributes()
{
   m_pos = NULL;
}

//*******************************************************************
void CEnumItemAttributes::Initialize(COPCGroup* pGroup)
{
   m_parent = pGroup;
   Reset();
}

//*******************************************************************
STDMETHODIMP CEnumItemAttributes::Next(
                                     ULONG celt,
                                     OPCITEMATTRIBUTES ** ppItemArray,
                                     ULONG * pceltFetched )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
//   USES_CONVERSION;

   // All args should be valid
   if( ppItemArray==NULL )
      return E_INVALIDARG;

   *ppItemArray = NULL;
   OPCITEMATTRIBUTES * pItemArray = (OPCITEMATTRIBUTES*)CoTaskMemAlloc(celt*sizeof(OPCITEMATTRIBUTES));
   for( ULONG index = 0; index < celt && m_pos; index++ )
   {
      COsdpControllerItem* pItem = NULL;
      LPVOID key = 0;
      CSLock wait( &m_parent->m_cs );
      m_parent->m_itemMap.GetNextAssoc( m_pos, key, (COPCItem*&)pItem );

      pItemArray[index].szAccessPath = (LPWSTR)CoTaskMemAlloc(4);
      wcscpy( pItemArray[index].szAccessPath, L"" );

      pItemArray[index].szItemID = (LPWSTR)CoTaskMemAlloc(2+pItem->m_strID.GetLength()*2);
      wcscpy( pItemArray[index].szItemID, T2OLE(pItem->m_strID.GetBuffer(0)));
      pItemArray[index].bActive = pItem->m_bActive;
      pItemArray[index].hClient = pItem->m_ClientHandle;
      pItemArray[index].hServer = (OPCHANDLE) pItem;
	
		// Nothing is writable in Field Gate
      pItemArray[index].dwAccessRights = OPC_READABLE;
      pItemArray[index].dwBlobSize = 0;
      pItemArray[index].pBlob = NULL;
      pItemArray[index].vtRequestedDataType = pItem->m_ClientType;
      pItemArray[index].vtCanonicalDataType = pItem->m_pTag->m_NativeType;
      pItemArray[index].pBlob = NULL;
      pItemArray[index].dwEUType = OPC_NOENUM;
      VariantInit( &pItemArray[index].vEUInfo );
/*      if( pItem->pTag->m_enableProcessing && pItem->pTag->m_pProcess )
      {
         pItemArray[index].dwEUType = OPC_ANALOG;
         SAFEARRAYBOUND bound;
         bound.lLbound = 0;
         bound.cElements = 2;
         SAFEARRAY *pArray = SafeArrayCreate(VT_R8, 1, &bound);
         if(pArray == NULL)
            return E_OUTOFMEMORY;
         LONG eu = 0;
         SafeArrayPutElement(pArray, &eu, (void *)&pItem->pTag->pProcess->MinEngRange);
         eu++;
         SafeArrayPutElement(pArray, &eu, (void *)&pItem->pTag->pProcess->MaxEngRange);
         pItemArray[index].vEUInfo.vt = VT_ARRAY | VT_R8;
         pItemArray[index].vEUInfo.parray = pArray;
      }
*/   }
   if( pceltFetched )
      *pceltFetched = index;
   *ppItemArray = pItemArray;
   return (celt==index) ? S_OK : S_FALSE;
}

//*******************************************************************
// just iterate celt times to skip those items
STDMETHODIMP CEnumItemAttributes::Skip( ULONG celt )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   for( ULONG i = 0; i < celt && m_pos; i++ )
   {
      COPCItem* pItem = NULL;
      LPVOID key = 0;
      m_parent->m_itemMap.GetNextAssoc( m_pos, key, pItem );
   }
   return (celt==i) ? S_OK : S_FALSE;
}

//*******************************************************************
STDMETHODIMP CEnumItemAttributes::Reset( void )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   m_pos = m_parent->m_itemMap.GetStartPosition();
   return S_OK;
}

//*******************************************************************
STDMETHODIMP CEnumItemAttributes::Clone( IEnumOPCItemAttributes ** ppEnumItemAttributes )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   *ppEnumItemAttributes = NULL;
   CComEnumItemAttributes* pEnumerator = new CComEnumItemAttributes;
   pEnumerator->Initialize(m_parent);
   pEnumerator->m_pos = m_pos;

   return pEnumerator->QueryInterface( IID_IEnumOPCItemAttributes, (LPVOID*)ppEnumItemAttributes );
}

