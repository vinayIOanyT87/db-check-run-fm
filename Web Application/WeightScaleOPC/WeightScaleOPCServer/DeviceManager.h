/******************************************************************************

	FILE NAME:		DeviceManager.h


	PURPOSE:			Declaration of the CDeviceManager


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


// CDeviceManager
class CDeviceManager 
{
	CIOList				m_IOList;
	COPCServerList		m_OPCServerList;

	void AddWeightScale(IWeightScalePtr oWeightScale,CIO* pIO);
	void InitializeTagDatabase();
	void UninitializeTagDatabase();
public:
	CRITICAL_SECTION	m_cs;
	CTag*	m_pRoot;
	bool m_bTagUpdatePending;
	int m_iOPCUpdateClientsCount;
	HANDLE	m_hUpdateClientsEvent;

	CDeviceManager();
	~CDeviceManager();
	HRESULT AddServer(COPCServer* pOPCServer);
	void RemoveServer(COPCServer* pOPCServer);
	CTag*	FindTag(LPTSTR szTag);
	void RemoveTag(CTag* pTag);
	void AddWeightScale(IWeightScalePtr oWeightScale);
	void PurgeDevice(LPTSTR szID);
	void AddTagToGroupItems(CTag* pTag);
	void RemoveTagFromGroupItems(CTag* pTag);
	void ModifyPort(IPortPtr oPort);
	void BeginOPCClientUpdates();
	void EndOPCClientUpdates();
};

