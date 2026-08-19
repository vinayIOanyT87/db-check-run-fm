// DeviceManager.cpp : Implementation of CDeviceManager

#include "stdafx.h"
#include "DeviceManager.h"

// CDeviceManager
CDeviceManager::CDeviceManager()
{
	m_pRoot=NULL;
	InitializeCriticalSection(&m_cs);
	m_bTagUpdatePending=false;
	m_iOPCUpdateClientsCount=0;
	m_hUpdateClientsEvent = CreateEvent( NULL,TRUE,TRUE,NULL );
	if(!m_hUpdateClientsEvent)
		throw (CString(_T("DeviceManager : CreateEvent Error")));
}

CDeviceManager::~CDeviceManager()
{
	POSITION pos;
	while((pos = m_OPCServerList.GetHeadPosition()))
	{
		OPCServerBase* pServer = m_OPCServerList.GetNext( pos );
		IUnknown* pUnk=0;
		HRESULT hr = pServer->QueryInterface( IID_IUnknown, (LPVOID*)&pUnk );
		if( SUCCEEDED(hr) )
			CoDisconnectObject( pUnk, 0 );  // disconnect from any remaining clients
		delete pServer;
	}
		
	m_OPCServerList.RemoveAll();

	CloseHandle(m_hUpdateClientsEvent);
	m_hUpdateClientsEvent=NULL;

	DeleteCriticalSection(&m_cs);
}

void CDeviceManager::BeginOPCClientUpdates()
{
	CSLock Lock(&m_cs);
	m_iOPCUpdateClientsCount++;
	ResetEvent(m_hUpdateClientsEvent);
}

void CDeviceManager::EndOPCClientUpdates()
{
	CSLock Lock(&m_cs);
	m_iOPCUpdateClientsCount--;
	if(m_iOPCUpdateClientsCount == 0)
		SetEvent(m_hUpdateClientsEvent);
}

HRESULT CDeviceManager::AddServer(COPCServer* pOPCServer)
{
	CSLock Lock(&m_cs);

	if(!m_OPCServerList.GetCount()
	&& m_pRoot == NULL)
	{
		m_OPCServerList.AddTail(pOPCServer);
		InitializeTagDatabase();
	}
	else
		m_OPCServerList.AddTail(pOPCServer);

	return S_OK;
}

void CDeviceManager::RemoveServer(COPCServer* pOPCServer)
{
	CSLock Lock(&m_cs);

	POSITION pos=m_OPCServerList.Find(pOPCServer);
	if(!pos)
		return;

	// do this prior to actual removal from m_OPCServerList
	// so that as Tags are removed, they may be removed
	// from items within groups still active in the server.
	// this synerio is only likely when a server has terminated
	// abnormally and the server is being removed by COM.
	if(m_OPCServerList.GetCount() == 1)
		UninitializeTagDatabase();

	m_OPCServerList.RemoveAt(pos);

}

CTag* CDeviceManager::FindTag(LPTSTR szTag)
{
	return m_pRoot->FindTag(szTag);
}

void CDeviceManager::RemoveTag(CTag* pTag)
{
	if(pTag->m_pParent)
	{
		POSITION pos=pTag->m_pParent->m_Branch.Find(pTag);
		if(pos)
			pTag->m_pParent->m_Branch.RemoveAt(pos);
		else
		{
			pos=pTag->m_pParent->m_Leaf.Find(pTag);
			if(pos)
				pTag->m_pParent->m_Leaf.RemoveAt(pos);
		}
	}

	delete pTag;
}			


void CDeviceManager::AddWeightScale(IWeightScalePtr oWeightScale,CIO* pIO)
{
	CTag* pWeightScaleTag=m_pRoot->AddBranch((LPTSTR) oWeightScale->ID,pIO);

	pWeightScaleTag->AddLeaf(WEIGHT_TAG,IDS_WEIGHT,0x32,OPC_READABLE,VT_I4,pIO);
	if(oWeightScale->Type != FAIRBANKS_90_164)
		pWeightScaleTag->AddLeaf(SCALE_IN_MOTION_TAG,IDS_SCALE_IN_MOTION,0x32,OPC_READABLE,VT_BOOL,pIO);
}

void CDeviceManager::InitializeTagDatabase()
{
	try
	{
		m_pRoot=new CTag(_T(""));
		if(!m_pRoot)
			throw(CString(_T("Memory Allocation Error")));

		// Log Startup Event
		CString	strMessage;
		strMessage.LoadString(IDS_STARTED);
		theApp.LogInfo(strMessage);

		// Check for Hardware Key
		IFMAccessPtr	oFMAccess(CLSID_FMAccess);
		WORD wVersion;
		oFMAccess->GetProgramVersionLIN(&wVersion);
		if (wVersion != 0)
		{
			WORD wOptions2;
			oFMAccess->GetWord2ValueLIN(&wOptions2);
			if ((wOptions2 & 0x1) == 0)
			{
				strMessage.LoadString(IDS_HARDWARE_KEY_FAILURE);
				theApp.LogError(strMessage);
				return;
			}
		}
		else
		{
			DWORD dwFunctions;
			oFMAccess->GetOPCAllowedFunctions(&dwFunctions);
			if ((dwFunctions & 0x10) == 0)
			{
				strMessage.LoadString(IDS_HARDWARE_KEY_FAILURE);
				theApp.LogError(strMessage);
				return;
			}
		}

		// For each WeightScale
		IWeightScalesPtr	oWeightScales(CLSID_WeightScales);
		IWeightScaleCollectionPtr	oWeightScaleCollection=oWeightScales->Enumerate();
		for(LONG lItem=0;lItem < oWeightScaleCollection->Count;lItem++)
		{
			IWeightScalePtr	oWeightScale=oWeightScaleCollection->Item(lItem);
			AddWeightScale(oWeightScale);
		}
	}
	catch (_com_error& e)
	{
		if(e.Description().length())
			theApp.LogError(e.Description());
		else
			theApp.LogError(e.ErrorMessage());
	}
	catch (CString strError)
	{
		strError=_T("DeviceManager: InitializeTagDatabase ")+strError;
		theApp.LogError(strError);
	}
	catch (...)
	{
		theApp.LogError(_T("DeviceManager: InitializeTagDatabase Unknown error"));
	}
}

void CDeviceManager::UninitializeTagDatabase()
{
	delete m_pRoot;
	m_pRoot=NULL;

	while(m_IOList.GetCount())
	{
		CIO*	pIO=m_IOList.RemoveTail();
		delete pIO;
	}

	// Log Stopped Event
	CString	strMessage;
	strMessage.LoadString(IDS_STOPPED);
	theApp.LogInfo(strMessage);

}

void CDeviceManager::AddWeightScale(IWeightScalePtr oWeightScale)
{
	m_bTagUpdatePending=true;

	WaitForOPCClientUpdates:

	WaitForSingleObject(m_hUpdateClientsEvent,INFINITE);
	EnterCriticalSection(&m_cs);
	if(m_iOPCUpdateClientsCount != 0)
	{
		LeaveCriticalSection(&m_cs);
		goto WaitForOPCClientUpdates;
	}
	CSLock Lock(&m_cs);
	LeaveCriticalSection(&m_cs);
	m_bTagUpdatePending=false;


	if(!m_OPCServerList.GetCount())
		return;

	COPCLock OPCLock(&m_OPCServerList);

	// Determine the I/O Object for this Comm Port
	CIO*	pIO=NULL;
	POSITION	pos=m_IOList.GetHeadPosition();
	while(pos)
	{
		pIO=m_IOList.GetNext(pos);
		if(pIO->m_lIndex == oWeightScale->PortIndex)
			break;
		pIO=NULL;
	}

	if(!pIO
	&& oWeightScale->PortIndex != 0)
	{
		IPortsPtr	oPorts(CLSID_Ports);
		IPortPtr		oPort=oPorts->Get(oWeightScale->PortIndex);

		pIO=new CIO(oWeightScale->Type, 
						(BYTE)oWeightScale->DeviceID,
						oPort->Index,
						(LPCTSTR) oPort->ID,
						oPort->Baud,
						oPort->DataBits,
						oPort->Parity,
						oPort->StopBits);
		if(!pIO)
			throw (CString(_T("Memory Allocation Error")));

		m_IOList.AddTail(pIO);
	}

	if ( pIO != NULL )
		pIO->m_dwUseCount++;

	AddWeightScale(oWeightScale,pIO);
}

void CDeviceManager::PurgeDevice(LPTSTR szID)
{
	m_bTagUpdatePending=true;

	WaitForOPCClientUpdates:

	WaitForSingleObject(m_hUpdateClientsEvent,INFINITE);
	EnterCriticalSection(&m_cs);
	if(m_iOPCUpdateClientsCount != 0)
	{
		LeaveCriticalSection(&m_cs);
		goto WaitForOPCClientUpdates;
	}
	CSLock Lock(&m_cs);
	LeaveCriticalSection(&m_cs);
	m_bTagUpdatePending=false;


	if(!m_OPCServerList.GetCount())
		return;

	COPCLock OPCLock(&m_OPCServerList);

	CString	oTag;

	oTag=szID;

	CTag*	pTag=FindTag(oTag.GetBuffer(0));
	if(!pTag)
		return;

	CIO*	pIO=pTag->m_pIO;
	
	RemoveTag(pTag);

	if(pIO)
	{
		pIO->m_dwUseCount--;
		if(!pIO->m_dwUseCount)
		{
			m_IOList.RemoveAt(m_IOList.Find(pIO));
			delete pIO;
		}
	}
}

void CDeviceManager::ModifyPort(IPortPtr oPort)\
{
	// Determine the I/O Object for this Comm Port
	CIO*	pIO=NULL;
	POSITION	pos=m_IOList.GetHeadPosition();
	while(pos)
	{
		pIO=m_IOList.GetNext(pos);
		if(pIO->m_lIndex == oPort->Index)
		{
			pIO->SetPortParameters((LPCTSTR) oPort->ID,
											oPort->Baud,
											oPort->DataBits,
											oPort->Parity,
											oPort->StopBits);
			break;
		}
	}
}

void CDeviceManager::AddTagToGroupItems(CTag* pTag)
{
	CString oPath=pTag->GetPathName();

	POSITION pos=m_OPCServerList.GetHeadPosition();
	while(pos)
	{
		COPCServer*	pOPCServer=m_OPCServerList.GetNext(pos);

	   POSITION pos = pOPCServer->m_groupMap.GetStartPosition();
		LPVOID	key=0;
	   OPCGroupObject* pGroup = NULL;
		while(pos)
		{
			pOPCServer->m_groupMap.GetNextAssoc(pos,key,pGroup);

			{
				POSITION pos=pGroup->m_itemMap.GetStartPosition();
				LPVOID	key=0;
				COPCItem* pItem=NULL;
				while(pos)
				{
					pGroup->m_itemMap.GetNextAssoc(pos,key,pItem);
					if(((CWeightScaleItem*) pItem)->m_oTag == oPath)
					{
						((CWeightScaleItem*) pItem)->m_pTag=pTag;
						if(pItem->m_bActive
						&& pTag->m_dwAccessRights & OPC_READABLE
						&& pTag->m_pIO)
							pTag->m_pIO->AddTagToScanList(pTag,pGroup->m_updateRate);
					}
				}
			}
		}
	}
}

void CDeviceManager::RemoveTagFromGroupItems(CTag* pTag)
{
	POSITION pos=m_OPCServerList.GetHeadPosition();
	while(pos)
	{
		COPCServer*	pOPCServer=m_OPCServerList.GetNext(pos);

		// If browsing at current tag then set to NULL
		// to force browsing back to root.
		if(pOPCServer->m_pCurrentTag == pTag)
			pOPCServer->m_pCurrentTag=NULL;

	   POSITION pos = pOPCServer->m_groupMap.GetStartPosition();
		LPVOID	key=0;
	   OPCGroupObject* pGroup = NULL;
		while(pos)
		{
			pOPCServer->m_groupMap.GetNextAssoc(pos,key,pGroup);

			{
				POSITION pos=pGroup->m_itemMap.GetStartPosition();
				LPVOID	key=0;
				COPCItem* pItem=NULL;
				while(pos)
				{
					pGroup->m_itemMap.GetNextAssoc(pos,key,pItem);
					if(((CWeightScaleItem*) pItem)->m_pTag == pTag)
					{
						if(pItem->m_bActive
						&& pTag->m_dwAccessRights & OPC_READABLE
						&& pTag->m_pIO)
							pTag->m_pIO->RemoveTagFromScanList(pTag);
						((CWeightScaleItem*) pItem)->m_pTag=NULL;
					}
				}
			}
		}
	}
}
