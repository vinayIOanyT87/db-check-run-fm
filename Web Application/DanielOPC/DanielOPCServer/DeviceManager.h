/******************************************************************************

	FILE NAME:		DeviceManager.h


	PURPOSE:			Declaration of the CDeviceManager


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2000

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

// Commands
#define PROMPT_RECIPE_CMD											0x01
#define REQUEST_SELECTED_RECIPE_CMD								0x02
#define TIMEOUT_OPERATION_CMD										0x05
#define AUTHORIZE_TRANSACTION_CMD								0x06
#define END_TRANSACTION_CMD										0x07
#define PROMPT_PRESET_VOLUME_CMD									0x08
#define REQUEST_PRESET_VOLUME_CMD								0x09
#define AUTHORIZE_BATCH_CMD										0x0A
#define END_BATCH_CMD												0x0D
#define ADDITIVE_TOTALIZERS_CMD									0x11
#define REQUEST_STATUS_CMD											0x12
#define RESET_PRIMARY_ALARMS_CMD									0x14
#define COMPONENT_TOTALIZERS_CMD									0x16
#define REQUEST_COMPONENT_VALUES_CMD							0x1A
#define DISPLAY_MESSAGE_CMD										0x1C
#define REQUEST_KEYPAD_DATA_CMD									0x1D
#define START_COMMUNICATIONS_CMD									0x21
#define REQUEST_PROGRAM_CODE_VALUES_AND_ATTRIBUTES_CMD	0x22
#define SET_PROGRAM_CODE_VALUE_CMD								0x23
#define SET_DATE_AND_TIME_CMD										0x29
#define READ_INPUT_CMD												0x2B
#define WRITE_OUTPUT_CMD											0x2C
#define LAST_KEY_PRESSED_CMD										0x31
#define CHANGE_OPERATING_MODE_CMD								0x37
#define CLEAR_DISPLAY_CMD											0x38

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

	void AddDanLoad6000(IDanLoadPtr oDanLoad,CIO* pIO,CDevice* pDevice);
	void InitializeTagDatabase();
	void UninitializeTagDatabase();
public:
	enum IO_TYPE
	{
		METER,
		RTD,
		CURRENT_LOOP,
		DISCRETE_INPUT,
		DISCRETE_OUTPUT
	};

	CRITICAL_SECTION	m_cs;
	CTag*	m_pRoot;

	CDeviceManager();
	~CDeviceManager();
	HRESULT AddServer(COPCServer* pOPCServer);
	void RemoveServer(COPCServer* pOPCServer);
	CTag*	FindTag(LPTSTR szTag);
	void RemoveTag(CTag* pTag);
	void AddDanLoad(IDanLoadPtr oDanLoad);
	void PurgeDevice(LPTSTR szID);
	void AddTagToGroupItems(CTag* pTag);
	void RemoveTagFromGroupItems(CTag* pTag);
	void ModifyPort(IPortPtr oPort);
	void UpdateGroups();
};

