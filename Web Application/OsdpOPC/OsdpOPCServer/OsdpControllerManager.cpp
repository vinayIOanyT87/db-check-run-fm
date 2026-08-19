// OsdpControllerManager.cpp : Implementation of COsdpControllerManager

#include "stdafx.h"
#include "OsdpControllerManager.h"


// COsdpControllerManager

COsdpControllerManager::COsdpControllerManager()
{
	InitializeCriticalSection(&m_cs);
}

COsdpControllerManager::~COsdpControllerManager()
{
	DeleteCriticalSection(&m_cs);
}

HRESULT COsdpControllerManager::AddServer(COPCServer* pOPCServer)
{
	CSLock Lock(&m_cs);

	if(!m_OPCServerList.GetCount())
	{
		m_OPCServerList.AddTail(pOPCServer);
		InitializeTagDatabase();
	}
	else
		m_OPCServerList.AddTail(pOPCServer);

	return S_OK;
}

void COsdpControllerManager::RemoveServer(COPCServer* pOPCServer)
{
	CSLock Lock(&m_cs);

	POSITION pos=m_OPCServerList.Find(pOPCServer);
	if(!pos)
		return;

	// do this prior to actual removal from m_OPCServerList
	// so that as Tags are removed thay may be removed
	// from items within groups still active in the server.
	// this synerio is only likely when a server has terminated
	// abnormally and the server is being removed by COM.
	if(m_OPCServerList.GetCount() == 1)
		UninitializeTagDatabase();

	m_OPCServerList.RemoveAt(pos);

}

CTag* COsdpControllerManager::FindTag(LPTSTR szTag)
{
	return m_pRoot->FindTag(szTag);
}

void COsdpControllerManager::RemoveTag(CTag* pTag)
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


void COsdpControllerManager::AddOsdpController(IOsdpControllerPtr oOsdpController,CIO* pIO)
{
	CTag* pOsdpControllerTag=m_pRoot->AddBranch((LPTSTR) oOsdpController->ID,pIO);

	BYTE	bAddress=oOsdpController->Address;

	// Count tag needs to be the first tag, as we need to be able to find it in IO requests to increment
	pOsdpControllerTag->AddLeaf(IDS_COUNT, bAddress, "\x00", "", 0, OPC_READABLE, VT_UI1, pIO);

	// Poll-based tags need to be next.  Order is important
	pOsdpControllerTag->AddLeaf(IDS_POLL, bAddress, "\x60", "", POLL, OPC_READABLE, VT_BOOL, pIO);
	pOsdpControllerTag->AddLeaf(IDS_CARD_READER_DATA,bAddress,"\x60","00",CARD_DATA,OPC_READABLE,VT_BSTR,pIO);
	pOsdpControllerTag->AddLeaf(IDS_KEYPAD_DATA,bAddress,"\x60","01",KEYPAD_DATA,OPC_READABLE,VT_BSTR,pIO);
	pOsdpControllerTag->AddLeaf(IDS_BEEP, bAddress, "\x6a", "", 0, OPC_WRITEABLE, VT_I4, pIO);
	pOsdpControllerTag->AddLeaf(IDS_LEDOFF, bAddress, "\x69", "", 0, OPC_WRITEABLE, VT_UI2, pIO);
	pOsdpControllerTag->AddLeaf(IDS_REDLED, bAddress, "\x69", "", 1, OPC_WRITEABLE, VT_UI2, pIO);
	pOsdpControllerTag->AddLeaf(IDS_GREENLED, bAddress, "\x69", "", 2, OPC_WRITEABLE, VT_UI2, pIO);
	pOsdpControllerTag->AddLeaf(IDS_AMBERLED, bAddress, "\x69", "", 3, OPC_WRITEABLE, VT_UI2, pIO);
}

void COsdpControllerManager::InitializeTagDatabase()
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

		// For each OsdpController
		IOsdpControllersPtr	oOsdpControllers(CLSID_OsdpControllers);
		IOsdpControllerCollectionPtr	oOsdpControllerCollection=oOsdpControllers->Enumerate();
		for(LONG lItem=0;lItem < oOsdpControllerCollection->Count;lItem++)
		{
			IOsdpControllerPtr	oOsdpController=oOsdpControllerCollection->Item(lItem);
			AddOsdpController(oOsdpController);
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
		strError=_T("OsdpControllerManager: InitializeTagDatabase ")+strError;
		theApp.LogError(strError);
	}
	catch (...)
	{
		theApp.LogError(_T("OsdpControllerManager: InitializeTagDatabase Unknown error"));
	}
}

void COsdpControllerManager::UninitializeTagDatabase()
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

void COsdpControllerManager::AddOsdpController(IOsdpControllerPtr oOsdpController)
{
	CSLock Lock(&m_cs);

	if(!m_OPCServerList.GetCount())
		return;

	COPCLock OPCLock(&m_OPCServerList);

	CIO*	pIO=NULL;

	// Determine the I/O Object for this Comm Port
	POSITION	pos=m_IOList.GetHeadPosition();
	while(pos)
	{
		pIO=m_IOList.GetNext(pos);
		if(pIO->m_lIndex == oOsdpController->PortIndex)
			break;
		pIO=NULL;
	}

	// Create I/O Object if PortIndex is != 0 or Network Communications
	if(!pIO && oOsdpController->PortIndex)
	{
		IPortsPtr	oPorts(CLSID_Ports);
		IPortPtr		oPort=oPorts->Get(oOsdpController->PortIndex);

		pIO=new CIO(oPort->Index,
						(LPCTSTR) oPort->ID,
						oPort->Baud,
						oPort->DataBits,
						oPort->Parity,
						oPort->StopBits);
		if(!pIO)
			throw (CString(_T("Memory Allocation Error")));

		m_IOList.AddTail(pIO);
	}

	if(pIO)
		pIO->m_dwUseCount++;

	AddOsdpController(oOsdpController,pIO);
}

void COsdpControllerManager::PurgeOsdpController(IOsdpControllerPtr oOsdpController)
{
	CSLock Lock(&m_cs);

	if(!m_OPCServerList.GetCount())
		return;

	COPCLock OPCLock(&m_OPCServerList);

	CString	oTag;

	oTag=(LPTSTR) oOsdpController->ID;

	CTag*	pOsdpControllerTag=FindTag(oTag.GetBuffer(0));
	if(!pOsdpControllerTag)
		return;

	CIO*	pIO=pOsdpControllerTag->m_pIO;
	
	RemoveTag(pOsdpControllerTag);

	if (pIO != NULL)
	{
		pIO->m_dwUseCount--;
		if(!pIO->m_dwUseCount)
		{
			m_IOList.RemoveAt(m_IOList.Find(pIO));
			delete pIO;
		}
	}
}

void COsdpControllerManager::ModifyPort(IPortPtr oPort)\
{
	// Determine the I/O Object for this Comm Port
	CIO*	pIO=NULL;
	POSITION	pos=m_IOList.GetHeadPosition();
	while(pos)
	{
		pIO=m_IOList.GetNext(pos);
		if(!pIO->m_bNetworkCommunications
		&& pIO->m_lIndex == oPort->Index)
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

void COsdpControllerManager::AddTagToGroupItems(CTag* pTag)
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
					if(((COsdpControllerItem*) pItem)->m_oTag == oPath)
					{
						((COsdpControllerItem*) pItem)->m_pTag=pTag;
						if(pItem->m_bActive
						&& pTag->m_dwAccessRights & OPC_READABLE)
							pTag->m_pIO->AddTagToScanList(pTag,pGroup->m_updateRate);
					}
				}
			}
		}
	}
}

void COsdpControllerManager::RemoveTagFromGroupItems(CTag* pTag)
{
	POSITION pos=m_OPCServerList.GetHeadPosition();
	while(pos)
	{
		COPCServer*	pOPCServer=m_OPCServerList.GetNext(pos);
		CSLock Lock(&pOPCServer->m_cs);

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
				CSLock Lock(&pGroup->m_cs);
				POSITION pos=pGroup->m_itemMap.GetStartPosition();
				LPVOID	key=0;
				COPCItem* pItem=NULL;
				while(pos)
				{
					pGroup->m_itemMap.GetNextAssoc(pos,key,pItem);
					if(((COsdpControllerItem*) pItem)->m_pTag == pTag)
					{
						if(pItem->m_bActive
						&& pTag->m_dwAccessRights & OPC_READABLE)
							pTag->m_pIO->RemoveTagFromScanList(pTag);
						((COsdpControllerItem*) pItem)->m_pTag=NULL;
					}
				}
			}
		}
	}
}