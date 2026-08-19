/******************************************************************************

	FILE NAME:		OsdpControllerManager.h


	PURPOSE:			Declaration of the COsdpControllerManager


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
*******************************************************************************/

#pragma once
#include "resource.h"       // main symbols
#include "IO.h"
#include "opcserver.h"

class COPCLock
{
public:
   COPCLock(COPCServerList* OPCServerList) : pOPCServerList(OPCServerList)
	{
		POSITION pos=pOPCServerList->GetHeadPosition();
		while(pos)
		{
			COPCServer*	pOPCServer=pOPCServerList->GetNext(pos);
			EnterCriticalSection(&pOPCServer->m_cs);

			POSITION pos = pOPCServer->m_groupMap.GetStartPosition();
			LPVOID	key=0;
			OPCGroupObject* pGroup = NULL;
			while(pos)
			{
				pOPCServer->m_groupMap.GetNextAssoc(pos,key,pGroup);
				EnterCriticalSection(&pGroup->m_cs);
			}
		}
	}

	~COPCLock()
   {
		POSITION pos=pOPCServerList->GetHeadPosition();
		while(pos)
		{
			COPCServer*	pOPCServer=pOPCServerList->GetNext(pos);

			POSITION pos = pOPCServer->m_groupMap.GetStartPosition();
			LPVOID	key=0;
			OPCGroupObject* pGroup = NULL;
			while(pos)
			{
				pOPCServer->m_groupMap.GetNextAssoc(pos,key,pGroup);
				LeaveCriticalSection(&pGroup->m_cs);
			}

			LeaveCriticalSection(&pOPCServer->m_cs);
		}
	}
private:
   COPCServerList* pOPCServerList;
};

enum PollResponseItem
{
	POLL = 0,
	CARD_DATA = 1,
	KEYPAD_DATA = 2
};

// COsdpControllerManager
class COsdpControllerManager 
{
	CIOList				m_IOList;
	COPCServerList		m_OPCServerList;

	void AddOsdpController(IOsdpControllerPtr oOsdpController,CIO* pIO);
	void InitializeTagDatabase();
	void UninitializeTagDatabase();
public:
	CRITICAL_SECTION	m_cs;
	CTag*	m_pRoot;

	COsdpControllerManager();
	~COsdpControllerManager();
	HRESULT AddServer(COPCServer* pOPCServer);
	void RemoveServer(COPCServer* pOPCServer);
	CTag*	FindTag(LPTSTR szTag);
	void RemoveTag(CTag* pTag);
	void AddOsdpController(IOsdpControllerPtr oOsdpController);
	void PurgeOsdpController(IOsdpControllerPtr oOsdpController);
	void AddTagToGroupItems(CTag* pTag);
	void RemoveTagFromGroupItems(CTag* pTag);
	void ModifyPort(IPortPtr oPort);
};

