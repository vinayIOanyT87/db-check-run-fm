/******************************************************************************

	FILE NAME:		OPCGroupBase.cpp


	PURPOSE:			Implementation of COPCGroupBase

	MODIFICATION HISTORY:
	Date:			By:					Reason:
	----------	-----------------	-------------------------------------------
	11-12-2007	W.Gray				7.2.0.0 - Added AFX_MANAGE_STATE to ASyncThreadStub

	27-Oct-08	B. Schaal			7.5.0.30 - Added _AtlInitialConstruct() in OPCGroupBase constructor
											since the MFC calls and operations have changed with the release of
											the .NET 2005 compiler.

	03-Dec-09	W.Gray				7.4.6.0 - Remove lock in synchronous Read and Write as it is reasonable
											that the client will not alter the item map on the group during a write.
											Also this could cause deadlock with ProcessResponse on call to FindTag (WI 9637)

*******************************************************************************/


#include "stdafx.h"
#include "OPCServerBase.h"
#include "OPCGroupBase.h"
#include <math.h>
#include <olectl.h>
#include <process.h>
#include "OsdpControllerManager.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

extern COsdpControllerManager		g_OsdpControllerManager;
//*******************************************************************
//    class COPCItem
//*******************************************************************
COPCItem::COPCItem()
: m_bActive(FALSE), m_ClientHandle(0), m_bChanged(TRUE),
  m_wQuality(OPC_QUALITY_BAD), m_ClientType( VT_EMPTY )
{
   CoFileTimeNow( &m_Timestamp );
}

COPCItem::~COPCItem()
{
}

//*******************************************************************
//    class OPCGroupBase
//*******************************************************************
// Initialize does the real work
OPCGroupBase::OPCGroupBase()
: m_name(), m_active(FALSE), m_updateRate(1000), m_clientHandle(0), m_timeBias(0),
  m_deadBand(0.0), m_LCID(0), m_running(TRUE), m_dataWaiting(FALSE), m_removed(FALSE),
  m_transactionID(2), m_parent(NULL), m_bEnable(TRUE)
{
	_AtlInitialConstruct();
   InitializeCriticalSection( &m_cs );
   m_DataAdviseSink = NULL;
   m_DataTimeAdviseSink = NULL;
   m_AsyncAdviseSink = NULL;
   m_cmdWaiting = FALSE;
	m_hDataThread = NULL;
}

//*******************************************************************
OPCGroupBase::~OPCGroupBase()
{
   EnterCriticalSection( &m_cs );
   while( m_asyncRequests.GetCount() > 0 )
   {
      ASyncRequest* pRequest = m_asyncRequests.RemoveTail();
      delete pRequest;
   }
   LeaveCriticalSection( &m_cs );

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
   EnterCriticalSection( &m_cs );
   POSITION pos = m_itemMap.GetStartPosition();
   while( pos )
   {
      m_itemMap.GetNextAssoc( pos, key, pItem );
      delete (COsdpControllerItem*) pItem;
   }
   m_itemMap.RemoveAll();
   LeaveCriticalSection( &m_cs );

   if(m_DataAdviseSink)
      m_DataAdviseSink->Release();
   if(m_DataTimeAdviseSink)
      m_DataTimeAdviseSink->Release();
   if(m_AsyncAdviseSink)
      m_AsyncAdviseSink->Release();
   CloseHandle( m_hTimer );
   // Don't close handle on hDataThread! _beginthread does it automatically.
   DeleteCriticalSection( &m_cs );
}

//*******************************************************************
void OPCGroupBase::Initialize(LPCWSTR szName,
             BOOL bActive,
             DWORD dwRequestedUpdateRate,
             OPCHANDLE hClientGroup,
             LONG* pTimeBias,
             FLOAT*percentDeadband,
             DWORD dwLCID,
             OPCServerBase* pServer)
{
   m_parent = pServer;
   m_name = szName;
   m_active = bActive;
   m_updateRate = dwRequestedUpdateRate;
   DoSetUpdateRate( m_updateRate );
   m_clientHandle = hClientGroup;
   if( pTimeBias )
      m_timeBias = *pTimeBias;
	else
		m_timeBias = _timezone/60;
   if( percentDeadband )
      m_deadBand = *percentDeadband;
   m_LCID = dwLCID;

   // event to signal data scan thread
   m_hTimer = CreateEvent( NULL, FALSE, FALSE, NULL );
   ASSERT( m_hTimer );

   // start the data scan thread
	UINT uiThreadID;
   m_hDataThread = (HANDLE) _beginthreadex(NULL,0,ThreadStub, this, 0, &uiThreadID);
   if(!m_hDataThread)
		return;
   SetThreadPriority( m_hDataThread, THREAD_PRIORITY_HIGHEST );
}

void OPCGroupBase::UnInitialize()
{
   EnterCriticalSection( &m_cs );
   while( m_asyncRequests.GetCount() > 0 )
   {
      ASyncRequest* pRequest = m_asyncRequests.RemoveTail();
      delete pRequest;
   }
   LeaveCriticalSection( &m_cs );

   // End the data thread
   m_active = FALSE;
   m_running = FALSE;
	if(m_hTimer)
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
   EnterCriticalSection( &m_cs );
   POSITION pos = m_itemMap.GetStartPosition();
   while( pos )
   {
      m_itemMap.GetNextAssoc( pos, key, pItem );
      delete pItem;
   }
   m_itemMap.RemoveAll();
   LeaveCriticalSection( &m_cs );

   if(m_DataAdviseSink)
	{
      m_DataAdviseSink->Release();
		m_DataAdviseSink=NULL;
	}

   if(m_DataTimeAdviseSink)
	{
      m_DataTimeAdviseSink->Release();
		m_DataTimeAdviseSink=NULL;
	}

   if(m_AsyncAdviseSink)
	{
      m_AsyncAdviseSink->Release();
		m_AsyncAdviseSink=NULL;
	}

	if(m_hTimer)
	{
	   CloseHandle( m_hTimer );
		m_hTimer=NULL;
	}

}

//*******************************************************************
STDMETHODIMP OPCGroupBase::GetState(
    DWORD     * pUpdateRate,
    BOOL      * pActive,
    LPWSTR    * ppName,
    LONG      * pTimeBias,
    FLOAT     * pPercentDeadband,
    DWORD     * pLCID,
    OPCHANDLE * phClientGroup,
    OPCHANDLE * phServerGroup)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;
   if( pUpdateRate )
      *pUpdateRate   = m_updateRate;
   if( pActive )
      *pActive       = m_active;
   if( pTimeBias )
      *pTimeBias     = m_timeBias;
   if( pPercentDeadband )
      *pPercentDeadband = m_deadBand;
   if( pLCID )
      *pLCID         = m_LCID;
   if( phClientGroup )
      *phClientGroup = m_clientHandle;
   if( phServerGroup )
      *phServerGroup = (OPCHANDLE)this;
   if( ppName )
      {
      *ppName     = (LPWSTR)CoTaskMemAlloc( 2*(m_name.GetLength()+1) );
      if( *ppName == NULL )
         return E_OUTOFMEMORY;
      USES_CONVERSION;
      wcscpy( *ppName, T2OLE(m_name.GetBuffer(0)) );
      }
   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::SetState(
    DWORD     * pRequestedUpdateRate,
    DWORD     * pRevisedUpdateRate,
    BOOL      * pActive,
    LONG      * pTimeBias,
    FLOAT     * pPercentDeadband,
    DWORD     * pLCID,
    OPCHANDLE * phClientGroup)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   if( pRevisedUpdateRate==NULL )
      return E_POINTER;

   HRESULT hr = S_OK;
   if( pRequestedUpdateRate )
   {
      DoSetUpdateRate( *pRequestedUpdateRate );
      if( m_updateRate != *pRequestedUpdateRate )
         hr = OPC_S_UNSUPPORTEDRATE;
   }
   *pRevisedUpdateRate = m_updateRate;
   if( pTimeBias )
      m_timeBias = *pTimeBias;
   if( pPercentDeadband )
      m_deadBand = *pPercentDeadband;
   if( pLCID )
      m_LCID = *pLCID;
   if( phClientGroup )
      m_clientHandle = *phClientGroup;

   BOOL activeChanged = FALSE;
   if( pActive )
      if( m_active != *pActive )   // change of state (to active) is an advise condition
      {
         activeChanged = TRUE;
         m_active = *pActive;
         if( m_active )
         {
            m_dataWaiting = TRUE;
            UpdateClients();
				return(hr);
         }
         else  // Set all items to bad quality
         {
            EnterCriticalSection( &m_cs );
            COPCItem* pItem = NULL;
            LPVOID key = 0;
            POSITION pos = m_itemMap.GetStartPosition();
            while( pos )
            {
               m_itemMap.GetNextAssoc( pos, key, (COPCItem*&)pItem );
               pItem->m_wQuality = OPC_QUALITY_BAD | OPC_QUALITY_OUT_OF_SERVICE;
            }
            LeaveCriticalSection( &m_cs );
         }
      }

   DoSetState(activeChanged);
   return hr;
}

//*******************************************************************
void OPCGroupBase::DoSetUpdateRate( DWORD newUpdateRate )
{
   m_updateRate = newUpdateRate;
}

//*******************************************************************
// The state parameters have been set by the client.
// If the active state has changed, activeChanged is true
void OPCGroupBase::DoSetState(BOOL activeChanged)
{
   if( activeChanged )
   {
      // m_active contains the new active state
   }
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::SetName(
    LPCWSTR szName)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   USES_CONVERSION;
   if( m_removed )
		return E_FAIL;
   if( szName == NULL )
		return E_POINTER;
	if(!lstrlen(szName))
		return E_INVALIDARG;
   if( m_name == szName )
      return S_OK;

   // check for duplicates
   CString groupName( szName );
   OPCGroupObject* pGroup = m_parent->FindNamedGroup( T2OLE(groupName.GetBuffer(0)) );
   if( pGroup && pGroup!=this )
      return OPC_E_DUPLICATENAME;

   m_name = szName;
   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::CloneGroup(
    LPCWSTR     szName,
    REFIID      riid,
    LPUNKNOWN * ppUnk)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;
   DWORD newServer=0;
   HRESULT hr = m_parent->AddGroup( szName,
                                  FALSE,
                                  m_updateRate,
                                  m_clientHandle,
                                  &m_timeBias,
                                  &m_deadBand,
                                  m_LCID,
                                  &newServer,
                                  NULL,
                                  riid,
                                  ppUnk);
   if( SUCCEEDED( hr ) )
		hr=DoCopyItems(szName);

   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::Read(
    OPCDATASOURCE   dwSource,
    DWORD           dwNumItems,
    OPCHANDLE     * phServer,
    OPCITEMSTATE ** ppItemValues,
    HRESULT      ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( !phServer || !ppItemValues || !ppErrors )
      return E_POINTER;

	if( !dwNumItems )
		return E_INVALIDARG;

   // create return data
   *ppErrors = NULL;
   *ppItemValues = (OPCITEMSTATE*)CoTaskMemAlloc(dwNumItems*sizeof(OPCITEMSTATE));
   if( *ppItemValues == NULL )
      return E_OUTOFMEMORY;
   memset( *ppItemValues, 0, dwNumItems*sizeof(OPCITEMSTATE));
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
   {
      CoTaskMemFree( *ppItemValues );
      *ppItemValues = NULL;
      return E_OUTOFMEMORY;
   }

   // Base just initializes the arrays
   OPCGroupBase::DoRead(dwSource,dwNumItems,(COPCItem**)phServer,*ppItemValues,*ppErrors);

   HRESULT hr = S_OK;
   // verify all server handles
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      // server handle is the address of its Item
      COPCItem* pItem = (COPCItem*)phServer[index];
      (*ppErrors)[index] = S_OK;
      if( !IsItemValid( pItem ) )
      {
         (*ppErrors)[index] = OPC_E_INVALIDHANDLE;
         hr = S_FALSE;
      }
   }

   hr = DoRead(dwSource, dwNumItems, (COPCItem**)phServer, *ppItemValues, *ppErrors);
   if( FAILED(hr) )
   {
      CoTaskMemFree( *ppItemValues );
      *ppItemValues = NULL;
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
      return hr;
   }

   // Convert to client datatypes - Added Ver 1.31
   for(index=0; index<dwNumItems; index++ )
   {
      COPCItem* pItem = (COPCItem*)phServer[index];
      if( (*ppErrors)[index] == S_OK
       && ((*ppItemValues)[index].wQuality & OPC_QUALITY_MASK) == OPC_QUALITY_GOOD
       && ((*ppItemValues)[index].vDataValue.vt & VT_ARRAY) != VT_ARRAY )
      {
         (*ppErrors)[index] = VariantChangeType( &((*ppItemValues)[index].vDataValue),
                                                 &((*ppItemValues)[index].vDataValue),
                                                 0, pItem->m_ClientType );
			if((*ppErrors)[index] != S_OK)
				hr=S_FALSE;
      }
   }
   return hr;
}

HRESULT OPCGroupBase::DoRead(
    OPCDATASOURCE   dwSource,
    DWORD           dwNumItems,
    COPCItem     ** ppItems,
    OPCITEMSTATE  * pItemValues,
    HRESULT       * pErrors)
{
   HRESULT hr = S_OK;

   // Initialize the arrays
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      pErrors[index] = S_OK;
      pItemValues[index].hClient = ppItems[index]->m_ClientHandle;
      CoFileTimeNow( &pItemValues[index].ftTimeStamp );
      pItemValues[index].wQuality = 0;
      pItemValues[index].wReserved = 0;
      VariantInit( &pItemValues[index].vDataValue );
   }
   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::Write(
    DWORD        dwNumItems,
    OPCHANDLE  * phServer,
    VARIANT    * pItemValues,
    HRESULT   ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( !phServer || !pItemValues || !ppErrors )
      return E_POINTER;

	if( !dwNumItems )
		return E_INVALIDARG;

   // create return data
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
      return E_OUTOFMEMORY;
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   HRESULT hr = S_OK;
   // verify all server handles
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      // server handle is the address of its Item
      COPCItem* pItem = (COPCItem*)phServer[index];
      (*ppErrors)[index] = S_OK;
      if( !IsItemValid( pItem ) )
      {
         (*ppErrors)[index] = OPC_E_INVALIDHANDLE;
         hr = S_FALSE;
      }
   }

   hr = DoWrite(dwNumItems,(COPCItem**)phServer,pItemValues,*ppErrors);
   if( FAILED(hr) )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
   }
   return hr;
}

//*******************************************************************
HRESULT OPCGroupBase::DoWrite(
    DWORD        dwNumItems,
    COPCItem  ** ppItems,
    VARIANT    * pItemValues,
    HRESULT    * pErrors)
{
   HRESULT hr = E_NOTIMPL;
   return hr;
}

//*******************************************************************
// ASynchronous version
STDMETHODIMP OPCGroupBase::Read(
    DWORD           dwConnection,
    OPCDATASOURCE   dwSource,
    DWORD           dwNumItems,
    OPCHANDLE     * phServer,
    DWORD         * pTransactionID,
    HRESULT      ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( !phServer || !pTransactionID || !ppErrors )
      return E_POINTER;

	if( !dwNumItems )
		return E_INVALIDARG;

   if( dwConnection == OPCSTMFORMATDATA )
   {
      if ( m_DataAdviseSink == NULL )
         return CONNECT_E_NOCONNECTION;
   }
   else if( dwConnection == OPCSTMFORMATDATATIME )
   {
      if ( m_DataTimeAdviseSink == NULL )
         return CONNECT_E_NOCONNECTION;
   }
   else  // unknown format
      return E_INVALIDARG;

   HRESULT hr = S_OK;
   // create return data
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
      return E_OUTOFMEMORY;
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   // verify all server handles
	CSLock wait( &m_cs );
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      // server handle is the address of its Item
      COPCItem* pItem = (COPCItem*)phServer[index];
      (*ppErrors)[index] = S_OK;
      if( !IsItemValid( pItem ) )
      {
         (*ppErrors)[index] = OPC_E_INVALIDHANDLE;
         hr = S_FALSE;
      }
   }

   if( hr == S_FALSE )  // return if any handles are invalid
      return hr;

   // Create a request that will store this information for the thread
   ASyncRequest* request = new ASyncRequest;
   if( request == NULL )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
      return E_OUTOFMEMORY;
   }
   request->dwConnection = dwConnection;
   request->dwNumItems = dwNumItems;
   request->dwSource = dwSource;
   request->handles = new OPCHANDLE[dwNumItems];
   request->errors = new HRESULT[dwNumItems];
   if( request->handles == NULL || request->errors == NULL )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
      delete request;
      return E_OUTOFMEMORY;
   }
   request->type = ASyncRequest::READ;
   *pTransactionID = request->dwTransactionID = m_transactionID++;

   for( index=0; index<dwNumItems; index++ )
   {
      request->handles[index] = phServer[index];
      request->errors[index] = S_OK;
   }

   {  // protect access to the list
	   CSLock wait( &m_cs );
	   m_asyncRequests.AddHead( request );
   }

   m_hASyncThread = (HANDLE)_beginthread(ASyncThreadStub, 0, this);
   ASSERT( (ULONG)m_hASyncThread != -1 );
   if( (ULONG)m_hASyncThread == -1 )
      hr = E_FAIL;
   return hr;
}

//*******************************************************************
// ASynchronous version
STDMETHODIMP OPCGroupBase::Write(
    DWORD       dwConnection,
    DWORD       dwNumItems,
    OPCHANDLE * phServer,
    VARIANT   * pItemValues,
    DWORD     * pTransactionID,
    HRESULT ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( !phServer || !pItemValues || !pTransactionID || !ppErrors )
      return E_POINTER;

	if( !dwNumItems )
		return E_INVALIDARG;


   if ( m_AsyncAdviseSink == NULL || dwConnection != OPCSTMFORMATWRITECOMPLETE )
      return CONNECT_E_NOCONNECTION;

   HRESULT hr = S_OK;
   // create return data
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
      return E_OUTOFMEMORY;
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   // verify all server handles
	CSLock wait( &m_cs );
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      // server handle is the address of its Item
      COPCItem* pItem = (COPCItem*)phServer[index];
      (*ppErrors)[index] = S_OK;
      if( !IsItemValid( pItem ) )
      {
         (*ppErrors)[index] = OPC_E_INVALIDHANDLE;
         hr = S_FALSE;
      }
   }

   if( hr == S_FALSE )
      return hr;

   // Create a request that will store this information for the thread
   ASyncRequest* request = new ASyncRequest;
   if( request == NULL )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
      return E_OUTOFMEMORY;
   }
   request->dwConnection = dwConnection;
   request->dwNumItems = dwNumItems;
   request->handles = new OPCHANDLE[dwNumItems];
   request->values = new VARIANT[dwNumItems];
   request->errors = new HRESULT[dwNumItems];
   if( request->handles == NULL || request->values == NULL || request->errors == NULL )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
      delete request;
      return E_OUTOFMEMORY;
   }
   request->type = ASyncRequest::WRITE;
   *pTransactionID = request->dwTransactionID = m_transactionID++;

   for( index=0; index<dwNumItems; index++ )
   {
      request->handles[index] = phServer[index];
      VariantInit( &request->values[index] );
      VariantCopy( &request->values[index], &pItemValues[index] );
      request->errors[index] = S_OK;
   }

   {  // protect access to the list
	   CSLock wait( &m_cs );
		m_asyncRequests.AddHead( request );
   }

   m_hASyncThread = (HANDLE)_beginthread(ASyncThreadStub, 0, this);
   ASSERT( (ULONG)m_hASyncThread != -1 );
   if( (ULONG)m_hASyncThread == -1 )
      hr = E_FAIL;
   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::Refresh(
    DWORD           dwConnection,
    OPCDATASOURCE   dwSource,
    DWORD         * pTransactionID)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   if( !m_active )
		return E_FAIL;

   // All args should be valid
   if( pTransactionID==NULL )
      return E_POINTER;
   if( dwConnection == OPCSTMFORMATDATA )
   {
      if ( m_DataAdviseSink == NULL )
         return CONNECT_E_NOCONNECTION;
   }
   else if( dwConnection == OPCSTMFORMATDATATIME )
   {
      if ( m_DataTimeAdviseSink == NULL )
         return CONNECT_E_NOCONNECTION;
   }
   else  // unknown format
      return E_INVALIDARG;

   HRESULT hr = S_OK;

   // Create a request that will store this information for the thread
   ASyncRequest* request = new ASyncRequest;
   if( request == NULL )
   {
      return E_OUTOFMEMORY;
   }
   request->dwConnection = dwConnection;
   request->dwNumItems = 0;
   request->dwSource = dwSource;
   request->type = ASyncRequest::REFRESH;
   *pTransactionID = request->dwTransactionID = m_transactionID++;

   {  // protect access to the list
		CSLock wait( &m_cs );
		m_asyncRequests.AddHead( request );
   }

   m_hASyncThread = (HANDLE)_beginthread(ASyncThreadStub, 0, this);
   ASSERT( (ULONG)m_hASyncThread != -1 );
   if( (ULONG)m_hASyncThread == -1 )
      hr = E_FAIL;
   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::Cancel(
    DWORD dwTransactionID)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	return E_NOTIMPL;
}

//*******************************************************************
// ASynchronous version 2
STDMETHODIMP OPCGroupBase::Read(
    DWORD           dwNumItems,
    OPCHANDLE     * phServer,
    DWORD           TransactionID,
    DWORD         * pCancelID,
    HRESULT      ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( !phServer || !pCancelID || !ppErrors )
      return E_POINTER;

	if(!dwNumItems)
		return E_INVALIDARG;

   // verify connection 
	if(!m_vec.GetSize())
		return CONNECT_E_NOCONNECTION;

   // create return data
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
      return E_OUTOFMEMORY;
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   // verify all server handles
	CSLock wait( &m_cs );
	HRESULT hr=S_OK;
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      // server handle is the address of its Item
      COPCItem* pItem = (COPCItem*)phServer[index];
      (*ppErrors)[index] = S_OK;
      if( !IsItemValid( pItem ) )
      {
         (*ppErrors)[index] = OPC_E_INVALIDHANDLE;
         hr = S_FALSE;
      }
   }

   if( hr == S_FALSE )  // return if any handles are invalid
      return hr;

   // Create a request that will store this information for the thread
   ASyncRequest* request = new ASyncRequest;
   if( request == NULL )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
      return E_OUTOFMEMORY;
   }
   request->dwConnection = 0;
   request->dwNumItems = dwNumItems;
   request->dwSource = OPC_DS_DEVICE;
   request->handles = new OPCHANDLE[dwNumItems];
   request->errors = new HRESULT[dwNumItems];
   if( request->handles == NULL || request->errors == NULL )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
      delete request;
      return E_OUTOFMEMORY;
   }
   request->type = ASyncRequest::READCP;
   request->dwTransactionID = TransactionID;
   *pCancelID = m_transactionID++;

   for( index=0; index<dwNumItems; index++ )
   {
      request->handles[index] = phServer[index];
      request->errors[index] = S_OK;
   }

   {  // protect access to the list
   CSLock wait( &m_cs );
   m_asyncRequests.AddHead( request );
   }

   m_hASyncThread = (HANDLE)_beginthread(ASyncThreadStub, 0, this);
   ASSERT( (ULONG)m_hASyncThread != -1 );
   if( (ULONG)m_hASyncThread == -1 )
      hr = E_FAIL;
   return hr;
}

//*******************************************************************
// ASynchronous version
STDMETHODIMP OPCGroupBase::Write(
    DWORD       dwNumItems,
    OPCHANDLE * phServer,
    VARIANT   * pItemValues,
    DWORD       TransactionID,
    DWORD     * pCancelID,
    HRESULT  ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( !phServer || !pItemValues || !pCancelID || !ppErrors )
      return E_POINTER;

	if(!dwNumItems)
		return E_INVALIDARG;

   // verify connection 
	if(!m_vec.GetSize())
		return CONNECT_E_NOCONNECTION;

   // create return data
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
      return E_OUTOFMEMORY;
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   // verify all server handles
	CSLock wait( &m_cs );
	HRESULT hr=S_OK;
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      // server handle is the address of its Item
      COPCItem* pItem = (COPCItem*)phServer[index];
      (*ppErrors)[index] = S_OK;
      if( !IsItemValid( pItem ) )
      {
         (*ppErrors)[index] = OPC_E_INVALIDHANDLE;
         hr = S_FALSE;
      }
   }

   if( hr == S_FALSE )
      return hr;

   // Create a request that will store this information for the thread
   ASyncRequest* request = new ASyncRequest;
   if( request == NULL )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
      return E_OUTOFMEMORY;
   }
   request->dwConnection = 0;
   request->dwNumItems = dwNumItems;
   request->handles = new OPCHANDLE[dwNumItems];
   request->values = new VARIANT[dwNumItems];
   request->errors = new HRESULT[dwNumItems];
   if( request->handles == NULL || request->values == NULL || request->errors == NULL )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
      delete request;
      return E_OUTOFMEMORY;
   }
   request->type = ASyncRequest::WRITECP;
   request->dwTransactionID = TransactionID;
   *pCancelID = m_transactionID++;

   for( index=0; index<dwNumItems; index++ )
   {
      request->handles[index] = phServer[index];
      VariantInit( &request->values[index] );
      VariantCopy( &request->values[index], &pItemValues[index] );
      request->errors[index] = S_OK;
   }

   {  // protect access to the list
	   CSLock wait( &m_cs );
		m_asyncRequests.AddHead( request );
   }

   m_hASyncThread = (HANDLE)_beginthread(ASyncThreadStub, 0, this);
   ASSERT( (ULONG)m_hASyncThread != -1 );
   if( (ULONG)m_hASyncThread == -1 )
      hr = E_FAIL;
   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::Refresh2(
    OPCDATASOURCE   dwSource,
    DWORD           TransactionID,
    DWORD         * pCancelID )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   if( !m_active )
		return E_FAIL;

   // All args should be valid
   if( pCancelID==NULL )
      return E_POINTER;

   // verify connection 
	if(!m_vec.GetSize())
		return CONNECT_E_NOCONNECTION;

   // Create a request that will store this information for the thread
   ASyncRequest* request = new ASyncRequest;
   if( request == NULL )
      return E_OUTOFMEMORY;

   request->dwConnection = 0;
   request->dwNumItems = 0;
   request->dwSource = dwSource;
   request->type = ASyncRequest::REFRESHCP;
   request->dwTransactionID = TransactionID;
   *pCancelID = m_transactionID++;

   {  // protect access to the list
	   CSLock wait( &m_cs );
	   m_asyncRequests.AddHead( request );
   }

   m_hASyncThread = (HANDLE)_beginthread(ASyncThreadStub, 0, this);
   ASSERT( (ULONG)m_hASyncThread != -1 );
   if( (ULONG)m_hASyncThread == -1 )
      return  E_FAIL;

   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::Cancel2( DWORD dwCancelID )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   return E_FAIL;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::SetEnable( BOOL bEnable )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // verify connection 
	if(!m_vec.GetSize())
		return CONNECT_E_NOCONNECTION;

   m_bEnable = bEnable;
   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::GetEnable( BOOL *pbEnable )
{
   if( m_removed )
		return E_FAIL;
   if( pbEnable == NULL )
	   return E_POINTER;

   // verify connection 
	if(!m_vec.GetSize())
		return CONNECT_E_NOCONNECTION;

   *pbEnable = m_bEnable;
   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::AddItems(
    DWORD            dwNumItems,
    OPCITEMDEF     * pItemArray,
    OPCITEMRESULT ** ppAddResults,
    HRESULT       ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( pItemArray==NULL || ppAddResults==NULL || ppErrors==NULL )
      return E_POINTER;

	if(!dwNumItems)
		return E_INVALIDARG;

   // create return data
   *ppErrors = NULL;
   *ppAddResults = (OPCITEMRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(OPCITEMRESULT));
   if( *ppAddResults == NULL )
      return E_OUTOFMEMORY;
   memset( *ppAddResults, 0, dwNumItems*sizeof(OPCITEMRESULT));
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
   {
      CoTaskMemFree( *ppAddResults );
      *ppAddResults = NULL;
      return E_OUTOFMEMORY;
   }
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   HRESULT hr = DoAddItems(dwNumItems,pItemArray,*ppAddResults,*ppErrors);
   if( FAILED(hr) )
   {
      CoTaskMemFree( *ppAddResults );
      *ppAddResults = NULL;
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
   }
   return hr;
}

//*******************************************************************
HRESULT OPCGroupBase::DoAddItems(
    DWORD            dwNumItems,
    OPCITEMDEF     * pItemArray,
    OPCITEMRESULT  * pAddResults,
    HRESULT        * pErrors)
{
   HRESULT hr = E_NOTIMPL;
   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::ValidateItems(
    DWORD             dwNumItems,
    OPCITEMDEF      * pItemArray,
    BOOL              bBlobUpdate,
    OPCITEMRESULT  ** ppValidationResults,
    HRESULT        ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( !pItemArray || !ppValidationResults || !ppErrors )
      return E_POINTER;

	if(!dwNumItems)
		return E_INVALIDARG;

   // create return data
   *ppErrors = NULL;
   *ppValidationResults = (OPCITEMRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(OPCITEMRESULT));
   if( *ppValidationResults == NULL )
      return E_OUTOFMEMORY;
   memset( *ppValidationResults, 0, dwNumItems*sizeof(OPCITEMRESULT));
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
   {
      CoTaskMemFree( *ppValidationResults );
      *ppValidationResults = NULL;
      return E_OUTOFMEMORY;
   }
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   HRESULT hr = DoValidateItems(dwNumItems,pItemArray,*ppValidationResults,*ppErrors);
   if( FAILED(hr) )
   {
      CoTaskMemFree( *ppValidationResults );
      *ppValidationResults = NULL;
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
   }
   return hr;
}

//*******************************************************************
HRESULT OPCGroupBase::DoValidateItems(
    DWORD             dwNumItems,
    OPCITEMDEF      * pItemArray,
    OPCITEMRESULT   * pValidationResults,
    HRESULT         * pErrors)
{

   return E_NOTIMPL;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::RemoveItems(
    DWORD        dwNumItems,
    OPCHANDLE  * phServer,
    HRESULT   ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;
   HRESULT hr = S_OK;

   // All args should be valid
   if( phServer==NULL || ppErrors==NULL )
      return E_POINTER;

	if(!dwNumItems)
		return E_INVALIDARG;

   // create return data
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
      return E_OUTOFMEMORY;
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   // verify all server handles
	CSLock wait(&m_cs);
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      // server handle is the address of its Item
      COPCItem* pItem = (COPCItem*)phServer[index];
      (*ppErrors)[index] = S_OK;
      if( !IsItemValid( pItem ) )
      {
         (*ppErrors)[index] = OPC_E_INVALIDHANDLE;
         hr = S_FALSE;
      }
   }

   hr = DoRemoveItems(dwNumItems,(COPCItem**)phServer,*ppErrors);
   if( FAILED(hr) )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
   }
   return hr;
}

//*******************************************************************
HRESULT OPCGroupBase::DoRemoveItems(
    DWORD        dwNumItems,
    COPCItem  ** ppItems,
    HRESULT    * pErrors)
{
   HRESULT hr = S_OK;
   CSLock wait( &m_cs );
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      if( pErrors[index] == S_OK )
      {
         COPCItem* pItem = ppItems[index];
         // remove from map of all items
         VERIFY(m_itemMap.RemoveKey( (LPVOID)pItem ));
         delete (COsdpControllerItem*) pItem;
      }
      else
         hr = S_FALSE;
   }
   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::SetActiveState(
    DWORD        dwNumItems,
    OPCHANDLE  * phServer,
    BOOL         bActive,
    HRESULT   ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( phServer==NULL || ppErrors==NULL )
      return E_POINTER;

	if(!dwNumItems)
		return E_INVALIDARG;

   // create return data
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
      return E_OUTOFMEMORY;
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   HRESULT hr = S_OK;

   // verify all server handles
   CSLock wait( &m_cs );
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      // server handle is the address of its Item
      COPCItem* pItem = (COPCItem*)phServer[index];
      (*ppErrors)[index] = S_OK;
      if( IsItemValid( pItem ) )
      {
         if( !bActive ) // set item quality bad
            pItem->m_wQuality = OPC_QUALITY_BAD | OPC_QUALITY_OUT_OF_SERVICE;
      }
      else
      {
         (*ppErrors)[index] = OPC_E_INVALIDHANDLE;
         hr = S_FALSE;
      }
   }

   hr = DoSetActiveState(dwNumItems,(COPCItem**)phServer,bActive,*ppErrors);
   if( FAILED(hr) )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
   }
   return hr;
}

//*******************************************************************
HRESULT OPCGroupBase::DoSetActiveState(
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
         COPCItem* pItem = ppItems[index];
         pItem->m_bActive = bActive;
      }
      else
         hr = S_FALSE;
   }
   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::SetClientHandles(
    DWORD        dwNumItems,
    OPCHANDLE  * phServer,
    OPCHANDLE  * phClient,
    HRESULT   ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( !phServer || !phClient || !ppErrors )
      return E_POINTER;

	if(!dwNumItems)
		return E_INVALIDARG;

   // create return data
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
      return E_OUTOFMEMORY;
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   HRESULT hr = S_OK;

   // verify all server handles
   CSLock wait( &m_cs );
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      // server handle is the address of its Item
      COPCItem* pItem = (COPCItem*)phServer[index];
      (*ppErrors)[index] = S_OK;
      if( !IsItemValid( pItem ) )
      {
         (*ppErrors)[index] = OPC_E_INVALIDHANDLE;
         hr = S_FALSE;
      }
   }

   hr = DoSetClientHandles(dwNumItems,(COPCItem**)phServer,phClient,*ppErrors);
   if( FAILED(hr) )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
   }
   return hr;
}

//*******************************************************************
HRESULT OPCGroupBase::DoSetClientHandles(
    DWORD        dwNumItems,
    COPCItem  ** ppItems,
    OPCHANDLE  * phClient,
    HRESULT    * pErrors)
{
   HRESULT hr = S_OK;

   for( DWORD index=0; index<dwNumItems; index++ )
   {
      if( pErrors[index] == S_OK )
      {
         COPCItem* pItem = ppItems[index];
         pItem->m_ClientHandle = phClient[index];
      }
      else
         hr = S_FALSE;
   }

   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::SetDatatypes(
    DWORD        dwNumItems,
    OPCHANDLE  * phServer,
    VARTYPE    * pRequestedDatatypes,
    HRESULT   ** ppErrors)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;

   // All args should be valid
   if( !phServer || !pRequestedDatatypes || !ppErrors )
      return E_POINTER;

	if(!dwNumItems)
		return E_INVALIDARG;

   // create return data
   *ppErrors = (HRESULT*)CoTaskMemAlloc(dwNumItems*sizeof(HRESULT));
   if( *ppErrors == NULL )
      return E_OUTOFMEMORY;
   memset( *ppErrors, 0, dwNumItems*sizeof(HRESULT));

   HRESULT hr = S_OK;

   // verify all server handles
   CSLock wait( &m_cs );
   for( DWORD index=0; index<dwNumItems; index++ )
   {
      // server handle is the address of its Item
      COPCItem* pItem = (COPCItem*)phServer[index];
      (*ppErrors)[index] = S_OK;
      if( !IsItemValid( pItem ) )
      {
         (*ppErrors)[index] = OPC_E_INVALIDHANDLE;
         hr = S_FALSE;
      }
   }

   hr = DoSetDatatypes(dwNumItems,(COPCItem**)phServer,pRequestedDatatypes,*ppErrors);
   if( FAILED(hr) )
   {
      CoTaskMemFree( *ppErrors );
      *ppErrors = NULL;
   }
   return hr;
}

//*******************************************************************
HRESULT OPCGroupBase::DoSetDatatypes(
    DWORD        dwNumItems,
    COPCItem  ** ppItems,
    VARTYPE    * pRequestedDatatypes,
    HRESULT    * pErrors)
{
   HRESULT hr = S_OK;

   for( DWORD index=0; index<dwNumItems; index++ )
   {
      if( pErrors[index] == S_OK )
      {
         COPCItem* pItem = ppItems[index];
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
STDMETHODIMP OPCGroupBase::CreateEnumerator(
    REFIID      riid,
    LPUNKNOWN * ppUnk)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;
   *ppUnk = NULL;
   IUnknown* pUnk = DoCreateEnumerator();
   if( pUnk == NULL )
      return E_OUTOFMEMORY;

   return pUnk->QueryInterface( riid, (LPVOID*)ppUnk );
}

IUnknown* OPCGroupBase::DoCreateEnumerator()
{
   return NULL;
}
//*******************************************************************
// IDataObject
STDMETHODIMP OPCGroupBase::DAdvise(
                            FORMATETC* pformatetc,
                            DWORD advf,
                            IAdviseSink* pAdvSink,
                            DWORD* pdwConnection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if( m_removed )
		return E_FAIL;
   if( pformatetc == NULL ) return E_POINTER;
   if( pAdvSink == NULL ) return E_POINTER;
   if( pdwConnection == NULL ) return E_POINTER;
   // enforce OPC limitations on format
   if( pformatetc->dwAspect != DVASPECT_CONTENT )
      return E_FAIL;
   if( pformatetc->ptd != NULL )
      return E_FAIL;
   if( pformatetc->tymed != TYMED_HGLOBAL )
      return E_FAIL;
   if( pformatetc->lindex != -1 )
      return E_FAIL;
   *pdwConnection = 0;

   HRESULT hr = E_FAIL;
   // Allow one advise per format
   if( pformatetc->cfFormat == OPCSTMFORMATDATA )
   {
      if ( m_DataAdviseSink != NULL)
         return CONNECT_E_ADVISELIMIT;
      hr = pAdvSink->QueryInterface( IID_IAdviseSink, (LPVOID*) &m_DataAdviseSink);
      if(FAILED(hr))
         return hr;
      m_DataAdviseSink->AddRef();
   }
   else if( pformatetc->cfFormat == OPCSTMFORMATDATATIME )
   {
      if ( m_DataTimeAdviseSink != NULL)
         return CONNECT_E_ADVISELIMIT;
      hr = pAdvSink->QueryInterface( IID_IAdviseSink, (LPVOID*) &m_DataTimeAdviseSink);
      if(FAILED(hr))
         return hr;
      m_DataTimeAdviseSink->AddRef();
   }
   else if( pformatetc->cfFormat == OPCSTMFORMATWRITECOMPLETE )
   {
      if ( m_AsyncAdviseSink != NULL)
         return CONNECT_E_ADVISELIMIT;
      hr = pAdvSink->QueryInterface( IID_IAdviseSink, (LPVOID*) &m_AsyncAdviseSink);
      if(FAILED(hr))
         return hr;
      m_AsyncAdviseSink->AddRef();
   }
   else
      return hr;

   *pdwConnection = pformatetc->cfFormat;

    return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::DUnadvise( DWORD dwConnection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   HRESULT hr = OLE_E_NOCONNECTION;
   if( dwConnection == OPCSTMFORMATDATA )
   {
      if( m_DataAdviseSink )
      {
         m_DataAdviseSink->Release();
         m_DataAdviseSink = NULL;
         hr = S_OK;
      }
   }
   if( dwConnection == OPCSTMFORMATDATATIME )
   {
      if( m_DataTimeAdviseSink )
      {
         m_DataTimeAdviseSink->Release();
         m_DataTimeAdviseSink = NULL;
         hr = S_OK;
      }
   }
   if( dwConnection == OPCSTMFORMATWRITECOMPLETE )
   {
      if( m_AsyncAdviseSink )
      {
         m_AsyncAdviseSink->Release();
         m_AsyncAdviseSink = NULL;
         hr = S_OK;
      }
   }
   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::EnumDAdvise( IEnumSTATDATA** ppenumAdvise)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   return E_NOTIMPL;
}

//*******************************************************************
// validate the format
STDMETHODIMP OPCGroupBase::QueryGetData(FORMATETC* pformatetc)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
    if ( pformatetc == NULL)
        return E_POINTER;

    if ( pformatetc->tymed != TYMED_HGLOBAL )
        return DV_E_TYMED;

    if ( pformatetc->cfFormat != OPCSTMFORMATDATA
     &&  pformatetc->cfFormat != OPCSTMFORMATDATATIME)
        return DV_E_FORMATETC;

    return NOERROR;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::GetData( FORMATETC* pformatetcIn, STGMEDIUM* pmedium)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   if ( pmedium == NULL || pformatetcIn == NULL)
      return E_INVALIDARG;

   HRESULT hr = QueryGetData( pformatetcIn);
   if ( hr != NOERROR )
      return hr;

   if( pformatetcIn->cfFormat == OPCSTMFORMATDATA )
      hr = CreateDataStream(pmedium, 0);
   else
      hr = CreateDataTimeStream( pmedium, 0 );

  return hr;
}

//*******************************************************************
#include <COMDEF.H>
STDMETHODIMP OPCGroupBase::UpdateClients()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   HRESULT hr = S_OK;
   if( !m_dataWaiting )
      return hr;

   //*******************************************************************
   // ConnectionPoint advise for OPC 2.0 clients
   if( m_bEnable )
   {
		CSLock lock(&m_cs);

      // find number of changed items to update
      COPCItem* pItem = NULL;
      DWORD itemCount = 0;
      HRESULT masterError = S_OK;
      HRESULT masterQual = S_OK;
      CPtrArray changedItems;
      changedItems.SetSize( m_itemMap.GetCount() );
      POSITION pos = m_itemMap.GetStartPosition();
      while( pos )
      {
         LPVOID key;
         m_itemMap.GetNextAssoc( pos, key, pItem );
         if( pItem->m_bChanged && pItem->m_bActive )
            changedItems[itemCount++] = key;
      }

      if( itemCount == 0 )
		{
		   m_dataWaiting = FALSE;
         return hr;
		}

      OPCHANDLE*  pHandles    = (OPCHANDLE*)_alloca(itemCount*sizeof(OPCHANDLE));
      VARIANT*    pValues     = (VARIANT*) _alloca(itemCount*sizeof(VARIANT));
      WORD*       pQualities  = (WORD*)    _alloca(itemCount*sizeof(WORD));
      FILETIME*   pTimes      = (FILETIME*)_alloca(itemCount*sizeof(FILETIME));
      HRESULT*    pErrors     = (HRESULT*) _alloca(itemCount*sizeof(HRESULT));

      memset( pHandles, 0, itemCount*sizeof(OPCHANDLE) );
      memset( pQualities, 0, itemCount*sizeof(WORD) );
      memset( pTimes, 0, itemCount*sizeof(FILETIME) );
      memset( pErrors, 0, itemCount*sizeof(HRESULT) );

      // populate the arrays
      for( DWORD index=0; index<itemCount; index++ )
      {
         VERIFY( m_itemMap.Lookup( changedItems[index], pItem ) );
         pHandles[index] = pItem->m_ClientHandle;
         pQualities[index] = pItem->m_wQuality;
         if( pQualities[index] != OPC_QUALITY_GOOD )
            masterQual = S_FALSE;
         pTimes[index] = pItem->m_Timestamp;
         pErrors[index] = S_OK;
         if( pErrors[index] != S_OK )
            masterError = S_FALSE;
         VariantInit( &pValues[index] );
         if ( (pItem->m_Value.vt & VT_ARRAY) != VT_ARRAY )
         {
             if( (pItem->m_wQuality & OPC_QUALITY_MASK) == OPC_QUALITY_GOOD )
                pErrors[index] = VariantChangeType( &pValues[index],
                                   (LPVARIANT) pItem->m_Value,
                                   0, pItem->m_ClientType );
         }
         else
         {
            pErrors[index] = VariantCopy( &pValues[index], pItem->m_Value );
         }
      }

      //*******************************************************************
      // advise the connection points
		Lock();
		if(m_vec.GetSize())
		{
			IUnknown** pp = m_vec.begin();
			while (pp < m_vec.end() && hr == S_OK)
			{
				if (*pp != NULL)
				{
					IOPCDataCallback* pIOPCDataCallback = (IOPCDataCallback*)*pp;
					hr = pIOPCDataCallback->OnDataChange(0, m_clientHandle,
								masterQual, masterError, itemCount,
								pHandles, pValues, pQualities, pTimes, pErrors);
				}
				pp++;
			}
		}
		Unlock();
      //*******************************************************************
      // release the memory
      for( index=0; index<itemCount; index++ )
      {
         VariantClear( &pValues[index] );
      }
      //*******************************************************************
   }

   // IDataObject advise for OPC 1.0 clients
   FORMATETC formatetc;
   formatetc.cfFormat = OPCSTMFORMATDATA;
   formatetc.ptd = NULL;
   formatetc.dwAspect = DVASPECT_CONTENT;
   formatetc.lindex = -1;
   formatetc.tymed = TYMED_HGLOBAL;

   STGMEDIUM stm;
   stm.tymed = TYMED_HGLOBAL;
   stm.pUnkForRelease = NULL;

   if ( m_DataAdviseSink )
   {
      formatetc.cfFormat = OPCSTMFORMATDATA;
      hr = GetData(&formatetc, &stm);
      if( FAILED(hr) )
         return hr;
      if ( m_DataAdviseSink )
         m_DataAdviseSink->OnDataChange(&formatetc, &stm);
      ReleaseStgMedium( &stm );
   }
   if ( m_DataTimeAdviseSink )
   {
      formatetc.cfFormat = OPCSTMFORMATDATATIME;
      hr = GetData(&formatetc, &stm);
      if( FAILED(hr) )
         return hr;
      if ( m_DataTimeAdviseSink )
         m_DataTimeAdviseSink->OnDataChange(&formatetc, &stm);
      ReleaseStgMedium( &stm );
   }

   // Set all items changed flag to false
	CSLock wait( &m_cs );
   COPCItem* pItem = NULL;
   POSITION pos = m_itemMap.GetStartPosition();
   while( pos )
   {
      LPVOID key;
      m_itemMap.GetNextAssoc( pos, key, pItem );
      pItem->m_bChanged = FALSE;
   }

   m_dataWaiting = FALSE;
   return hr;
}

//*******************************************************************
BOOL OPCGroupBase::IsItemValid( const COPCItem* pItem )
{
   if( pItem == NULL )
      return FALSE;
   COPCItem* pDummy;
   CSLock wait( &m_cs );
   if( !m_itemMap.Lookup( (LPVOID)pItem, pDummy ) )
      return FALSE;
   if( pItem != pDummy )
      return FALSE;
   return TRUE;
}

//*******************************************************************
HRESULT OPCGroupBase::DoCopyItems(
	LPCWSTR szName)
{

   return E_NOTIMPL;
}
//*******************************************************************
//*******************************************************************
// Background data scan thread
//*******************************************************************
unsigned int _stdcall OPCGroupBase::ThreadStub(void* arg)
{
   ((OPCGroupBase*)arg)->DataThread();
	return 0;
}

//*******************************************************************
void OPCGroupBase::DataThread()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())

/*
	Calling CoInitializeEx does not seem to be required.  The thread
	does not instantiate any com objects, it only makes calls to
	connection point callbacks.  This was part of the original FactorySoft
	OPC SDK.  But calling CoInitializeEx cause the application to terminate
	abnormally.  Normally the OPCServer objects are released by COM on
	shutdown.  If CoInitializeEx is called these releases do not occur.

	if(FAILED(CoInitializeEx(NULL, COINIT_MULTITHREADED)) )
   {
      TRACE(_T("CoInitializeEx failed\n"));
      return;
   }
*/
   while( m_running )
   {
      DWORD start = GetTickCount();
      if( m_active )
      {
         BOOL changed = DoUpdateGroup();
         EnterCriticalSection( &m_cs );
         COPCItem* pItem = NULL;
         LPVOID key = 0;
         POSITION pos = m_itemMap.GetStartPosition();
         while( pos )
         {
            m_itemMap.GetNextAssoc( pos, key, (COPCItem*&)pItem );
            if( pItem->m_bChanged )
            {
               changed = TRUE;
            }
         }
         LeaveCriticalSection( &m_cs );

         if( changed )
         {
            m_dataWaiting = TRUE;

            UpdateClients();
            m_parent->UpdateTime();
         }
      }
      DWORD end = GetTickCount();
      DWORD elapsed = end - start;
      if( end < start )    // when it wraps (49 days)
         elapsed = 0;
      if( elapsed < m_updateRate )    // limit it
      {
          // wait for the rest of our period
          WaitForSingleObject( m_hTimer, m_updateRate - elapsed );
      }
      else  // if
      {
         while( m_cmdWaiting )
            Sleep( 5 );
      }
   }
//   CoUninitialize();
}

BOOL OPCGroupBase::DoUpdateGroup()
{
   return FALSE;
}
//*******************************************************************
// Async thread
//*******************************************************************
void OPCGroupBase::ASyncThreadStub(void* arg)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	((OPCGroupBase*)arg)->ASyncThread();
}

//*******************************************************************
// This thread executes a single Async I/O request and exits
void OPCGroupBase::ASyncThread()
{
   if(FAILED(CoInitializeEx(NULL, COINIT_MULTITHREADED)) )
   {
      TRACE(_T("CoInitializeEx failed\n"));
      return;
   }

   EnterCriticalSection( &m_cs );
   ASyncRequest* pRequest = m_asyncRequests.RemoveTail();
   LeaveCriticalSection( &m_cs );

   COPCItem* pItem = NULL;
   if( pRequest->type == ASyncRequest::REFRESH
    || pRequest->type == ASyncRequest::REFRESHCP)
   {
      // Scan tags
      DoUpdateGroup();

      // update all items from tags
      LPVOID key = 0;
      CSLock wait( &m_cs );
      POSITION pos = m_itemMap.GetStartPosition();
      while( pos )
      {
         m_itemMap.GetNextAssoc( pos, key, (COPCItem*&)pItem );
         if( pItem->m_bActive )
         {
            // mark all values to be sent to clients (not just changed ones)
            pItem->m_bChanged = TRUE;
         }
      }
   }
   else if( pRequest->type == ASyncRequest::READ
   || pRequest->type == ASyncRequest::READCP )
   {
      // read tags regardless of active state
      OPCITEMSTATE* pItemValues = (OPCITEMSTATE*)_alloca(pRequest->dwNumItems*sizeof(OPCITEMSTATE));

		// Base just initializes the arrays
		OPCGroupBase::DoRead(pRequest->dwSource,pRequest->dwNumItems,(COPCItem**)pRequest->handles,pItemValues,pRequest->errors);

		HRESULT hr = S_OK;
		CSLock wait( &m_cs );

		// verify all server handles
		for( DWORD index=0; index < pRequest->dwNumItems; index++ )
		{
			// server handle is the address of its Item
			COPCItem* pItem = (COPCItem*)pRequest->handles[index];
			pRequest->errors[index] = S_OK;
			if( !IsItemValid( pItem ) )
			{
				pRequest->errors[index] = OPC_E_INVALIDHANDLE;
				hr = S_FALSE;
			}
		}


      DoRead(pRequest->dwSource, pRequest->dwNumItems,
            (COPCItem**)pRequest->handles, pItemValues, pRequest->errors);

      // now put results into the items
      for( index=0; index<pRequest->dwNumItems; index++ )
      {
			if(pRequest->errors[index] != S_OK)
				continue;

         // server handle is the address of its Item
         COPCItem* pItem = (COPCItem*)pRequest->handles[index];

         if(pRequest->dwSource == OPC_DS_DEVICE
			|| (m_active && pItem->m_bActive ))
            pItem->m_wQuality = pItemValues[index].wQuality;
         else
            pItem->m_wQuality = OPC_QUALITY_OUT_OF_SERVICE;

         pItem->m_Value = pItemValues[index].vDataValue;
         pItem->m_bChanged = TRUE;

			VariantClear(&pItemValues[index].vDataValue);
      }
   }

   else if( pRequest->type == ASyncRequest::WRITE
   || pRequest->type == ASyncRequest::WRITECP )
   {
      DoWrite(pRequest->dwNumItems,(COPCItem**)pRequest->handles, pRequest->values, pRequest->errors);
   }
   else
      ASSERT( FALSE );

   // Send notification
   if( pRequest->type == ASyncRequest::READ
    || pRequest->type == ASyncRequest::WRITE
    || pRequest->type == ASyncRequest::REFRESH )
      AsyncUpdate( pRequest );
   else
      AsyncUpdate2( pRequest );

   delete pRequest;

   CoUninitialize();
}

//*******************************************************************
// Asynchronous advises are different from the regular ones.
// There is a unique transaction ID sent
// and it only goes to the specified connection rather than
// all advised clients.
STDMETHODIMP OPCGroupBase::AsyncUpdate(ASyncRequest* pRequest)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   HRESULT hr = S_FALSE;

   FORMATETC formatetc;
   formatetc.cfFormat = (USHORT)pRequest->dwConnection;
   formatetc.ptd = NULL;
   formatetc.dwAspect = DVASPECT_CONTENT;
   formatetc.lindex = -1;
   formatetc.tymed = TYMED_HGLOBAL;
   // Send notification
   STGMEDIUM   medium;
   if( pRequest->type == ASyncRequest::WRITE )
   {
      OPCGROUPHEADERWRITE header;
      header.dwItemCount = pRequest->dwNumItems;
      header.hClientGroup = m_clientHandle;
      header.dwTransactionID = pRequest->dwTransactionID;
      header.hrStatus = S_OK;
      for( DWORD index=0; index<header.dwItemCount; index++ )
      {
         if( FAILED(pRequest->errors[index]) )
            header.hrStatus = S_FALSE;
      }

      CSharedFile file;
      file.Write( &header, sizeof(header) );

      for( index=0; index<header.dwItemCount; index++ )
      {
         COPCItem* pItem = (COPCItem*)pRequest->handles[index];
         OPCITEMHEADERWRITE itemHeader;
         itemHeader.hClient = pItem->m_ClientHandle;
         itemHeader.dwError = pRequest->errors[index];
         file.Write( &itemHeader, sizeof(itemHeader) );
      }
      medium.tymed = TYMED_HGLOBAL;
      medium.hGlobal = file.Detach();
      medium.pUnkForRelease = NULL;
      // The format is different from the advise format
      if( m_AsyncAdviseSink )
         m_AsyncAdviseSink->OnDataChange(&formatetc, &medium);
   }
   else  // either a read or refresh
   {
      if( formatetc.cfFormat == OPCSTMFORMATDATA )
      {
         hr = CreateDataStream( &medium, pRequest->dwTransactionID );
         if( FAILED(hr) )
            return hr;
         if( m_DataAdviseSink )
            m_DataAdviseSink->OnDataChange(&formatetc, &medium);
      }
      else
      {
         hr = CreateDataTimeStream( &medium, pRequest->dwTransactionID );
         if( FAILED(hr) )
            return hr;
         if( m_DataTimeAdviseSink )
            m_DataTimeAdviseSink->OnDataChange(&formatetc, &medium);
      }
   }

   ReleaseStgMedium( &medium );
   return hr;
}

//*******************************************************************
// AsyncUpdate2 handles the IOPCAsyncIO2 calls corresponding to
// Connection Points.
STDMETHODIMP OPCGroupBase::AsyncUpdate2(ASyncRequest* pRequest)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   HRESULT hr = S_OK;

   // Send notification
   if( pRequest->type == ASyncRequest::WRITECP )
   {
      COPCItem* pItem = NULL;
      DWORD itemCount = pRequest->dwNumItems;
      HRESULT masterError = S_OK;

      OPCHANDLE*  pHandles    = (OPCHANDLE*)_alloca(itemCount*sizeof(OPCHANDLE));
      HRESULT*    pErrors     = (HRESULT*) _alloca(itemCount*sizeof(HRESULT));

      memset( pHandles, 0, itemCount*sizeof(OPCHANDLE) );
      memset( pErrors, 0, itemCount*sizeof(HRESULT) );

      // populate the arrays
      for( DWORD index=0; index<itemCount; index++ )
      {
         pItem = (COPCItem*)pRequest->handles[index];
         pHandles[index] = pItem->m_ClientHandle;
         pErrors[index] = pRequest->errors[index];
         if( pErrors[index] != S_OK )
            masterError = S_FALSE;
      }
      //*******************************************************************
      // advise the connection points
		Lock();
		if(m_vec.GetSize())
		{
			IUnknown** pp = m_vec.begin();
			while (pp < m_vec.end() && hr == S_OK)
			{
				if (*pp != NULL)
				{
					IOPCDataCallback* pIOPCDataCallback = (IOPCDataCallback*)*pp;
					hr = pIOPCDataCallback->OnWriteComplete(pRequest->dwTransactionID, m_clientHandle,
								masterError, itemCount,
								pHandles, pErrors);
				}
				pp++;
			}
		}
		Unlock();
      //*******************************************************************
   }
   else if( pRequest->type == ASyncRequest::READCP )
   {

      COPCItem* pItem = NULL;
      DWORD itemCount = pRequest->dwNumItems;
      HRESULT masterError = S_OK;
      HRESULT masterQual = S_OK;

      OPCHANDLE*  pHandles    = (OPCHANDLE*)_alloca(itemCount*sizeof(OPCHANDLE));
      VARIANT*    pValues     = (VARIANT*) _alloca(itemCount*sizeof(VARIANT));
      WORD*       pQualities  = (WORD*)    _alloca(itemCount*sizeof(WORD));
      FILETIME*   pTimes      = (FILETIME*)_alloca(itemCount*sizeof(FILETIME));
      HRESULT*    pErrors     = (HRESULT*) _alloca(itemCount*sizeof(HRESULT));

      memset( pHandles, 0, itemCount*sizeof(OPCHANDLE) );
      memset( pQualities, 0, itemCount*sizeof(WORD) );
      memset( pTimes, 0, itemCount*sizeof(FILETIME) );
      memset( pErrors, 0, itemCount*sizeof(HRESULT) );

      // populate the arrays
      for( DWORD index=0; index<itemCount; index++ )
      {
         pItem = (COPCItem*)pRequest->handles[index];
         pHandles[index] = pItem->m_ClientHandle;
         pQualities[index] = pItem->m_wQuality;
         if( pItem->m_wQuality != OPC_QUALITY_GOOD )
            masterQual = S_FALSE;
         pTimes[index] = pItem->m_Timestamp;
         pErrors[index] = S_OK;
         VariantInit( &pValues[index] );

         if ( (pItem->m_Value.vt & VT_ARRAY) != VT_ARRAY )
         {
             if( (pItem->m_wQuality & OPC_QUALITY_MASK) == OPC_QUALITY_GOOD )
                pErrors[index] = VariantChangeType( &pValues[index],
                                   (LPVARIANT) pItem->m_Value,
                                   0, pItem->m_ClientType );
         }
         else
            pErrors[index] = VariantCopy( &pValues[index], pItem->m_Value );

         if( pErrors[index] != S_OK )
            masterError = S_FALSE;

			VariantClear(&pItem->m_Value);
      }
      //*******************************************************************
      // advise the connection points
		Lock();
		if(m_vec.GetSize())
		{
			IUnknown** pp = m_vec.begin();
			while (pp < m_vec.end() && hr == S_OK)
			{
				if (*pp != NULL)
				{
					IOPCDataCallback* pIOPCDataCallback = (IOPCDataCallback*)*pp;
					hr = pIOPCDataCallback->OnReadComplete(pRequest->dwTransactionID, m_clientHandle,
								masterQual, masterError, itemCount,
								pHandles, pValues, pQualities, pTimes, pErrors);
				}
				pp++;
			}
		}
		Unlock();

      //*******************************************************************
      // release the memory
      for( index=0; index<itemCount; index++ )
      {
         VariantClear( &pValues[index] );
      }
      //*******************************************************************


   }
   else if( pRequest->type == ASyncRequest::REFRESHCP )
   {
      // Same as advise but all items are sent
// Is enable used here?
		CSLock lock(&m_cs);
      COPCItem* pItem = NULL;
      DWORD itemCount = 0;
      HRESULT masterError = S_OK;
      HRESULT masterQual = S_OK;
      CPtrArray changedItems;
      changedItems.SetSize( m_itemMap.GetCount() );
      POSITION pos = m_itemMap.GetStartPosition();
      while( pos )
      {
         LPVOID key;
         m_itemMap.GetNextAssoc( pos, key, pItem );
         changedItems[itemCount++] = key;
      }
      OPCHANDLE*  pHandles    = (OPCHANDLE*)_alloca(itemCount*sizeof(OPCHANDLE));
      VARIANT*    pValues     = (VARIANT*) _alloca(itemCount*sizeof(VARIANT));
      WORD*       pQualities  = (WORD*)    _alloca(itemCount*sizeof(WORD));
      FILETIME*   pTimes      = (FILETIME*)_alloca(itemCount*sizeof(FILETIME));
      HRESULT*    pErrors     = (HRESULT*) _alloca(itemCount*sizeof(HRESULT));

      memset( pHandles, 0, itemCount*sizeof(OPCHANDLE) );
      memset( pQualities, 0, itemCount*sizeof(WORD) );
      memset( pTimes, 0, itemCount*sizeof(FILETIME) );
      memset( pErrors, 0, itemCount*sizeof(HRESULT) );

      // populate the arrays
      for( DWORD index=0; index<itemCount; index++ )
      {
         VERIFY( m_itemMap.Lookup( changedItems[index], pItem ) );
         pHandles[index] = pItem->m_ClientHandle;
         pQualities[index] = pItem->m_wQuality;
         if( pItem->m_wQuality != OPC_QUALITY_GOOD )
            masterQual = S_FALSE;
         pTimes[index] = pItem->m_Timestamp;
         pErrors[index] = S_OK;
         if( pErrors[index] != S_OK )
            masterError = S_FALSE;
         VariantInit( &pValues[index] );
         if ( (pItem->m_Value.vt & VT_ARRAY) != VT_ARRAY )
         {
             if( (pItem->m_wQuality & OPC_QUALITY_MASK) == OPC_QUALITY_GOOD )
                pErrors[index] = VariantChangeType( &pValues[index],
                                   (LPVARIANT) pItem->m_Value,
                                   0, pItem->m_ClientType );
         }
         else
         {
            pErrors[index] = VariantCopy( &pValues[index], pItem->m_Value );
         }
      }
      //*******************************************************************
      // advise the connection points
		Lock();
		if(m_vec.GetSize())
		{
			IUnknown** pp = m_vec.begin();
			while (pp < m_vec.end() && hr == S_OK)
			{
				if (*pp != NULL)
				{
					IOPCDataCallback* pIOPCDataCallback = (IOPCDataCallback*)*pp;
					hr = pIOPCDataCallback->OnDataChange(pRequest->dwTransactionID, m_clientHandle,
								masterQual, masterError, itemCount,
								pHandles, pValues, pQualities, pTimes, pErrors);
				}
				pp++;
			}
		}
		Unlock();
      //*******************************************************************
      // release the memory
      for( index=0; index<itemCount; index++ )
      {
         VariantClear( &pValues[index] );
      }
      //*******************************************************************
   }

   return hr;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::CreateDataStream(STGMEDIUM* pmedium, DWORD transactionID)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   COPCItem* pItem = NULL;

   OPCGROUPHEADER header;
   header.dwSize = 0;
   header.hClientGroup = m_clientHandle;
   header.dwTransactionID = transactionID;
   header.hrStatus = S_OK;

   // find number of changed items to update
   CSLock wait( &m_cs );   // protect data
   header.dwItemCount = 0;
   CPtrArray changedItems;
   changedItems.SetSize( m_itemMap.GetCount() );
   POSITION pos = m_itemMap.GetStartPosition();
   while( pos )
   {
      LPVOID key;
      m_itemMap.GetNextAssoc( pos, key, pItem );
      if( pItem->m_bChanged && pItem->m_bActive )
      {
         changedItems[header.dwItemCount++] = key;
         pItem->m_bChanged = FALSE;
      }
   }
   CSharedFile file;
   file.Write( &header, sizeof(header) );

   DWORD dataOffset = sizeof(header) + sizeof(OPCITEMHEADER2)*header.dwItemCount;
   DWORD headerOffset = (DWORD) file.GetPosition();
   for( DWORD index=0; index<header.dwItemCount; index++ )
   {
      VERIFY( m_itemMap.Lookup( changedItems[index], pItem ) );
      // The header
      OPCITEMHEADER2 itemHeader;
      itemHeader.hClient = pItem->m_ClientHandle;
      itemHeader.dwValueOffset = dataOffset;

      itemHeader.wQuality = pItem->m_wQuality;
      itemHeader.wReserved = 0;
      file.Seek( headerOffset, CFile::begin );
      file.Write( &itemHeader, sizeof(itemHeader) );
      headerOffset += sizeof(itemHeader);

      // The data
      COleVariant value( pItem->m_Value );
      if ( (value.vt & VT_ARRAY) != VT_ARRAY )
      {
          if( (pItem->m_wQuality & OPC_QUALITY_MASK) == OPC_QUALITY_GOOD )
             VariantChangeType( (LPVARIANT)value,
                                (LPVARIANT)value,
                                0, pItem->m_ClientType );
      }
      file.Seek( dataOffset, CFile::begin );
      file.Write( (LPVARIANT)value, sizeof(VARIANT) );
      dataOffset += sizeof(VARIANT);
      // add external data (BSTR or arrays)
      if( value.vt == VT_BSTR )
      {  // length is string + prepended length + NULL
         ULONG len = SysStringByteLen( value.bstrVal )+sizeof(DWORD)+sizeof(WCHAR);
         // write the DWORD length that preceeds the string, too
         file.Write( ((BYTE*)value.bstrVal - sizeof(DWORD)), len );
         dataOffset += len;
      }
      else if( value.vt & VT_ARRAY )
      {
         file.Write( value.parray, sizeof(SAFEARRAY) );
         dataOffset += sizeof(SAFEARRAY);
         ULONG dataSize = value.parray->rgsabound[0].cElements * value.parray->cbElements;
         file.Write( value.parray->pvData, dataSize );
         dataOffset += dataSize;
      }
   }
   // update the header's stream size parameter
   header.dwSize = (DWORD) file.GetPosition();
   file.SeekToBegin();
   file.Write( &header, sizeof(header) );
   pmedium->tymed = TYMED_HGLOBAL;
   pmedium->hGlobal = file.Detach();
   pmedium->pUnkForRelease = NULL;

   return S_OK;
}

//*******************************************************************
STDMETHODIMP OPCGroupBase::CreateDataTimeStream(STGMEDIUM* pmedium, DWORD transactionID)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
   COPCItem* pItem = NULL;

   OPCGROUPHEADER header;
   header.dwSize = 0;
   header.hClientGroup = m_clientHandle;
   header.dwTransactionID = transactionID;
   header.hrStatus = S_OK;

   // find number of changed items to update
   CSLock wait( &m_cs );   // protect data
   header.dwItemCount = 0;
   CPtrArray changedItems;
   changedItems.SetSize( m_itemMap.GetCount() );
   POSITION pos = m_itemMap.GetStartPosition();
   while( pos )
   {
      LPVOID key;
      m_itemMap.GetNextAssoc( pos, key, pItem );
      if( pItem->m_bChanged && pItem->m_bActive )
      {
         changedItems[header.dwItemCount++] = key;
         pItem->m_bChanged = FALSE;
      }
   }
   CSharedFile file;
   file.Write( &header, sizeof(header) );

   DWORD dataOffset = sizeof(header) + sizeof(OPCITEMHEADER1)*header.dwItemCount;
   DWORD headerOffset = (DWORD) file.GetPosition();
   for( DWORD index=0; index<header.dwItemCount; index++ )
   {
      VERIFY( m_itemMap.Lookup( changedItems[index], pItem ) );
      // The header
      OPCITEMHEADER1 itemHeader;
      itemHeader.hClient = pItem->m_ClientHandle;
      itemHeader.dwValueOffset = dataOffset;

      itemHeader.wQuality = pItem->m_wQuality;
      itemHeader.wReserved = 0;
      itemHeader.ftTimeStampItem = pItem->m_Timestamp;
      file.Seek( headerOffset, CFile::begin );
      file.Write( &itemHeader, sizeof(itemHeader) );
      headerOffset += sizeof(itemHeader);

      // The data
      COleVariant value( pItem->m_Value );
      if ( (value.vt & VT_ARRAY) != VT_ARRAY )
      {
          if( (pItem->m_wQuality & OPC_QUALITY_MASK) == OPC_QUALITY_GOOD )
             VariantChangeType( (LPVARIANT)value,
                                (LPVARIANT)value,
                                0, pItem->m_ClientType );
      }
      file.Seek( dataOffset, CFile::begin );
      file.Write( (LPVARIANT)value, sizeof(VARIANT) );
      dataOffset += sizeof(VARIANT);
      // add external data (BSTR or arrays)
      if( value.vt == VT_BSTR )
      {  // length is string + prepended length + NULL
         ULONG len = SysStringByteLen( value.bstrVal )+sizeof(DWORD)+sizeof(WCHAR);
         // write the DWORD length that preceeds the string, too
         file.Write( ((BYTE*)value.bstrVal - sizeof(DWORD)), len );
         dataOffset += len;
      }
      else if( value.vt & VT_ARRAY )
      {
         file.Write( value.parray, sizeof(SAFEARRAY) );
         dataOffset += sizeof(SAFEARRAY);
         ULONG dataSize = value.parray->rgsabound[0].cElements * value.parray->cbElements;
         file.Write( value.parray->pvData, dataSize );
         dataOffset += dataSize;
      }
   }
   header.dwSize = (DWORD) file.GetPosition();
   file.SeekToBegin();
   file.Write( &header, sizeof(header) );
   pmedium->tymed = TYMED_HGLOBAL;
   pmedium->hGlobal = file.Detach();
   pmedium->pUnkForRelease = NULL;

   return S_OK;
}

