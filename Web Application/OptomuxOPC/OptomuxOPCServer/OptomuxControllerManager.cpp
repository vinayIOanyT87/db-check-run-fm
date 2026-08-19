// OptomuxControllerManager.cpp : Implementation of COptomuxControllerManager

#include "stdafx.h"
#include "OptomuxControllerManager.h"


// COptomuxControllerManager

COptomuxControllerManager::COptomuxControllerManager()
{
	InitializeCriticalSection(&m_cs);
}

COptomuxControllerManager::~COptomuxControllerManager()
{
	DeleteCriticalSection(&m_cs);
}

HRESULT COptomuxControllerManager::AddServer(COPCServer* pOPCServer)
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

void COptomuxControllerManager::RemoveServer(COPCServer* pOPCServer)
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

CTag* COptomuxControllerManager::FindTag(LPTSTR szTag)
{
	return m_pRoot->FindTag(szTag);
}

void COptomuxControllerManager::RemoveTag(CTag* pTag)
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


void COptomuxControllerManager::AddOptomuxController(IOptomuxControllerPtr oOptomuxController,CIO* pIO)
{
	CTag* pOptomuxControllerTag=m_pRoot->AddBranch((LPTSTR) oOptomuxController->ID,pIO);

	BYTE	bAddress=oOptomuxController->Address;

	// Tags that are common to HC05, HC12, and Varec DET
	pOptomuxControllerTag->AddLeaf(IDS_CARD_READER_DATA,bAddress,"X","00",0,OPC_READABLE,VT_BSTR,pIO);
	pOptomuxControllerTag->AddLeaf(IDS_KEYPAD_DATA,bAddress,"X","01",0,OPC_READABLE,VT_BSTR,pIO);
	pOptomuxControllerTag->AddLeaf(IDS_TIME,bAddress,"X","02",0,OPC_READABLE,VT_BSTR,pIO);
	pOptomuxControllerTag->AddLeaf(IDS_WRITE_FIRST_LINE,bAddress,"S","\\f",0,OPC_WRITEABLE,VT_BSTR,pIO);
	pOptomuxControllerTag->AddLeaf(IDS_WRITE_SECOND_LINE,bAddress,"S","\\n",0,OPC_WRITEABLE,VT_BSTR,pIO);
	pOptomuxControllerTag->AddLeaf(IDS_PIN_DISPLAY_MODE,bAddress,"p",NULL,0,OPC_WRITEABLE,VT_BOOL,pIO);

	if(oOptomuxController->Type == PASSCONTROLLER_HC12)
	{
		pOptomuxControllerTag->AddLeaf(IDS_CLEAR_LIST,bAddress,"s","01",0,OPC_WRITEABLE,VT_EMPTY,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_WRITE_LIST_ITEM,bAddress,"s","02",0,OPC_WRITEABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_DISPLAY_LIST,bAddress,"s","03",0,OPC_WRITEABLE,VT_EMPTY,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_SELECTED_LIST_ITEM,bAddress,"s","04",0,OPC_READABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_SELECT_LIST_ITEM,bAddress,"s","05",0,OPC_WRITEABLE,VT_UI1,pIO);
	}

	else if(oOptomuxController->Type == VAREC_DET)
	{
		pOptomuxControllerTag->AddLeaf(IDS_CLEAR_LIST,bAddress,"s","01",0,OPC_WRITEABLE,VT_EMPTY,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_WRITE_LIST_ITEM,bAddress,"s","02",0,OPC_WRITEABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_DISPLAY_LIST,bAddress,"s","03",0,OPC_WRITEABLE,VT_EMPTY,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_SELECTED_LIST_ITEM,bAddress,"s","04",0,OPC_READABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_SELECT_LIST_ITEM,bAddress,"s","05",0,OPC_WRITEABLE,VT_I2,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_WRITE_THIRD_LINE,bAddress,"S","\\o",0,OPC_WRITEABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_WRITE_FORTH_LINE,bAddress,"S","\\p",0,OPC_WRITEABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_WRITE_FIFTH_LINE,bAddress,"S","\\q",0,OPC_WRITEABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_WRITE_SIXTH_LINE,bAddress,"S","\\r",0,OPC_WRITEABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_WRITE_SEVENTH_LINE,bAddress,"S","\\s",0,OPC_WRITEABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_WRITE_EIGHTH_LINE,bAddress,"S","\\t",0,OPC_WRITEABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_WRITE_STATUS_LINE,bAddress,"S","\\u",0,OPC_WRITEABLE,VT_BSTR,pIO);
		pOptomuxControllerTag->AddLeaf(IDS_WRITE_RESPONSE_LINE,bAddress,"S","\\v",0,OPC_WRITEABLE,VT_BSTR,pIO);
	}

	CTag* pInputs=pOptomuxControllerTag->AddBranch(IDS_INPUTS,pIO);
	CTag* pOutputs=pOptomuxControllerTag->AddBranch(IDS_OUTPUTS,pIO);
	CTag* pCounters=pOptomuxControllerTag->AddBranch(IDS_COUNTERS,pIO);

	for(int iIndex=0;iIndex < 8;iIndex++)
	{
		CString strTag;
		strTag.Format(_T("%02d"),iIndex+1);
		if(oOptomuxController->ModuleInputOutputMap & (0x01 << iIndex))
		{
			pInputs->AddLeaf(strTag,bAddress,"M",NULL,iIndex,OPC_READABLE,VT_BOOL,pIO);
			CTag* pCounter=pCounters->AddBranch(strTag,pIO);
			pCounter->m_dwItem=iIndex;

			// Note: Position here is important.  Code in IO.cpp expects CountTag to be
			//			first in collection
			pCounter->AddLeaf(IDS_COUNT,bAddress,"W",NULL,iIndex,OPC_READABLE,VT_UI2,pIO);
			pCounter->AddLeaf(IDS_START,bAddress,"U",NULL,iIndex,OPC_WRITEABLE,VT_EMPTY,pIO);
			pCounter->AddLeaf(IDS_STOP,bAddress,"V",NULL,iIndex,OPC_WRITEABLE,VT_EMPTY,pIO);
			pCounter->AddLeaf(IDS_CLEAR,bAddress,"Y",NULL,iIndex,OPC_WRITEABLE,VT_EMPTY,pIO);
		}
		else
			pOutputs->AddLeaf(strTag,bAddress,"K",NULL,iIndex,OPC_WRITEABLE,VT_BOOL,pIO);
	}
}

void COptomuxControllerManager::InitializeTagDatabase()
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

		// For each OptomuxController
		IOptomuxControllersPtr	oOptomuxControllers(CLSID_OptomuxControllers);
		IOptomuxControllerCollectionPtr	oOptomuxControllerCollection=oOptomuxControllers->Enumerate();
		for(LONG lItem=0;lItem < oOptomuxControllerCollection->Count;lItem++)
		{
			IOptomuxControllerPtr	oOptomuxController=oOptomuxControllerCollection->Item(lItem);
			AddOptomuxController(oOptomuxController);
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
		strError=_T("OptomuxControllerManager: InitializeTagDatabase ")+strError;
		theApp.LogError(strError);
	}
	catch (...)
	{
		theApp.LogError(_T("OptomuxControllerManager: InitializeTagDatabase Unknown error"));
	}
}

void COptomuxControllerManager::UninitializeTagDatabase()
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

void COptomuxControllerManager::AddOptomuxController(IOptomuxControllerPtr oOptomuxController)
{
	CSLock Lock(&m_cs);

	if(!m_OPCServerList.GetCount())
		return;

	COPCLock OPCLock(&m_OPCServerList);

	CIO*	pIO=NULL;

	// Determine the I/O Object for this Comm Port
	if(!oOptomuxController->NetworkCommunications)
	{
		POSITION	pos=m_IOList.GetHeadPosition();
		while(pos)
		{
			pIO=m_IOList.GetNext(pos);
			if(!pIO->m_bNetworkCommunications
			&& pIO->m_lIndex == oOptomuxController->PortIndex)
				break;
			pIO=NULL;
		}
	}

	// Create I/O Object if PortIndex is != 0 or Network Communications
	if(!pIO
	&& (oOptomuxController->NetworkCommunications
	|| oOptomuxController->PortIndex))
	{
		if(oOptomuxController->NetworkCommunications)
			pIO=new CIO(oOptomuxController->IPAddress,
							oOptomuxController->Port);
		else
		{
			IPortsPtr	oPorts(CLSID_Ports);
			IPortPtr		oPort=oPorts->Get(oOptomuxController->PortIndex);

			pIO=new CIO(oPort->Index,
							(LPCTSTR) oPort->ID,
							oPort->Baud,
							oPort->DataBits,
							oPort->Parity,
							oPort->StopBits);
			if(!pIO)
				throw (CString(_T("Memory Allocation Error")));
		}
		m_IOList.AddTail(pIO);
	}

	if(pIO)
		pIO->m_dwUseCount++;

	AddOptomuxController(oOptomuxController,pIO);
}

void COptomuxControllerManager::PurgeOptomuxController(IOptomuxControllerPtr oOptomuxController)
{
	CSLock Lock(&m_cs);

	if(!m_OPCServerList.GetCount())
		return;

	COPCLock OPCLock(&m_OPCServerList);

	CString	oTag;

	oTag=(LPTSTR) oOptomuxController->ID;

	CTag*	pOptomuxControllerTag=FindTag(oTag.GetBuffer(0));
	if(!pOptomuxControllerTag)
		return;

	CIO*	pIO=pOptomuxControllerTag->m_pIO;
	
	RemoveTag(pOptomuxControllerTag);

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

void COptomuxControllerManager::ModifyPort(IPortPtr oPort)\
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

void COptomuxControllerManager::AddTagToGroupItems(CTag* pTag)
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
					if(((COptomuxControllerItem*) pItem)->m_oTag == oPath)
					{
						((COptomuxControllerItem*) pItem)->m_pTag=pTag;
						if(pItem->m_bActive
						&& pTag->m_dwAccessRights & OPC_READABLE)
							pTag->m_pIO->AddTagToScanList(pTag,pGroup->m_updateRate);
					}
				}
			}
		}
	}
}

void COptomuxControllerManager::RemoveTagFromGroupItems(CTag* pTag)
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
					if(((COptomuxControllerItem*) pItem)->m_pTag == pTag)
					{
						if(pItem->m_bActive
						&& pTag->m_dwAccessRights & OPC_READABLE)
							pTag->m_pIO->RemoveTagFromScanList(pTag);
						((COptomuxControllerItem*) pItem)->m_pTag=NULL;
					}
				}
			}
		}
	}
}