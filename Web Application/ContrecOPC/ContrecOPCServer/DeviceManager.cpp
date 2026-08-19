/******************************************************************************

	FILE NAME:		DeviceManager.cpp


	PURPOSE:			Implementation of the CDeviceManager


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	B. Schaal


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------

*******************************************************************************/

#include "StdAfx.h"
#include ".\devicemanager.h"

CDeviceManager::CDeviceManager(void)
{
	m_pRoot=NULL;
	InitializeCriticalSection(&m_cs);
}

CDeviceManager::~CDeviceManager(void)
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

	DeleteCriticalSection(&m_cs);
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

void CDeviceManager::AddContrecDevice(IContrecPtr oContrec,CIO* pIO,CDevice* pDevice)
{
	CTag* pContrecTag=m_pRoot->AddBranch((LPTSTR) oContrec->ID,0,pIO,pDevice);
	/***************** Main Tags ***************************************/
	pContrecTag->AddLeaf(IDS_APPLICATION_VERSION,ISSUE_COMMAND_APP_VERSION,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_APPLICATION_VERSION_DATETIME,ISSUE_COMMAND_APP_VERSION_DATETIME,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_APPLICATION_SYSTEM_DATETIME,ISSUE_COMMAND_SYSTEM_VERSION_DATETIME,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_APPLICATION_POWERCYCLE_DATETIME,ISSUE_COMMAND_POWER_CYCLE_DATETIME,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_APPLICATION_COMMAND_FIELD,ISSUE_ENQ_COMMAND_FIELD,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);

	pContrecTag->AddLeaf(IDS_DISPLAY_PROMPT,ISSUE_DISPLAY_PROMPT,1,5,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_DISPLAY_MESSAGE,ISSUE_DISPLAY_MESSAGE,1,6,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_GETANSWER_MESSAGE,ISSUE_GETANSWER_MESSAGE,1,7,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_HIDDENANSWER_MESSAGE,ISSUE_HIDDENANSWER_MESSAGE,1,8,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_CLEAR_DISPLAY,ISSUE_CLEAR_DISPLAY,1,9,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_REMOTE_AUTHORIZE,ISSUE_REMOTE_AUTHORIZE,1,10,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_ENTERED_TRUCK_ID,ISSUE_TRUCK_ID,1,12,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_LOAD_NUMBER,ISSUE_LOAD_NUMBER,1,13,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_ENTERED_DATA,GET_ENTERED_KEYBOARD_DATA,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_TERMINATE_TRANSACTION,ISSUE_TERMINATE_TRANSACTION,1,15,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_DISPLAY_MESSAGE_TIMEOUT,DISPLAY_MESSAGE_TIMEOUT,1,16,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_SET_MESSAGE_TIMEOUT,SET_MESSAGE_TIMEOUT,1,17,OPC_READABLE | OPC_WRITEABLE,VT_I4,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_ALTER_ARM_NAME,ALTER_ARM_NAME,1,18,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_LOAD_NUMBER_RESPONSE,LOAD_NUMBER_RESPONSE,1,19,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_COMPARTMENT_RESPONSE,COMPARTMENT_RESPONSE,1,20,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_BATCH_TOTALS,BATCH_TOTALS,1,21,OPC_WRITEABLE | OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_TRANSACTION_COMPLETE,TRANSACTION_COMPLETE,1,22,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_INITIAL_MESSAGE,SET_INITIAL_MESSAGE,1,21,OPC_WRITEABLE | OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_INITIAL_MESSAGE_CONTROLLED,SET_INITIAL_MESSAGE_CONTROLLED,1,21,OPC_WRITEABLE | OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_ISSUE_GETTOUCHKEY_PROMPT,ISSUE_GETTOUCHKEY_PROMPT,1,21,OPC_WRITEABLE | OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_GET_TOUCHKEY_DATA,GET_TOUCHKEY_DATA,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_ISSUE_MANAGER_RESET,ISSUE_MANAGER_RESET,1,19,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_ISSUE_RESET_DATE_TIME,ISSUE_RESET_DATE_TIME,1,19,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_ISSUE_SET_PIN_NUMBERS,ISSUE_SET_PIN_NUMBERS,1,19,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_GET_DRIVER_PIN_NUMBER,GET_DRIVER_PIN_NUMBER,1,11,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_GET_DRIVER_TOUCH_KEY,GET_DRIVER_TOUCH_KEY,1,11,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_ISSUE_REMOTEAUTH_ERRORMESSAGE,ISSUE_REMOTEAUTH_ERRORMESSAGE,1,11,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_GET_TRUCK_PIN_NUMBER,GET_TRUCK_PIN_NUMBER,1,11,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pContrecTag->AddLeaf(IDS_GET_TRUCK_TOUCH_KEY,GET_TRUCK_TOUCH_KEY,1,11,OPC_READABLE,VT_BSTR,pIO,pDevice);

	/***************** Stored Transactions ***************************************/
	CTag* pStoredTransactionsTag=pContrecTag->AddBranch(IDS_STORED_TRANSACTIONS,REQUEST_STATUS_CMD,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_SET_STORED_TRANSACTION_NUMBER,SET_STORED_TRANSACTION_NUMBER,1,1,OPC_WRITEABLE | OPC_READABLE,VT_I4,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_UNITADDRESS,GET_STORED_TRANSACTION_UNITADDRESS,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_TRANSACTIONUMBER,GET_STORED_TRANSACTION_TRANSACTIONUMBER,1,3,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_DATE,GET_STORED_TRANSACTION_DATE,1,5,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_STARTTIME,GET_STORED_TRANSACTION_STARTTIME,1,6,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_STOPTIME,GET_STORED_TRANSACTION_STOPTIME,1,7,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_CALIBRATIONNUMBER,GET_STORED_TRANSACTION_CALIBRATIONNUMBER,1,4,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_ENTRYSTART,GET_STORED_TRANSACTION_ENTRYSTART,1,8,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_ENTRYSTOP,GET_STORED_TRANSACTION_ENTRYSTOP,1,9,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_DRIVERINDEX,GET_STORED_TRANSACTION_DRIVERINDEX,1,10,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_TRUCKINDEX,GET_STORED_TRANSACTION_TRUCKINDEX,1,11,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_LOADNUMBER,GET_STORED_TRANSACTION_LOADNUMBER,1,12,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_ARMNUMBER,GET_STORED_TRANSACTION_ARMNUMBER,1,13,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_ARM1DENSITY,GET_STORED_TRANSACTION_ARM1DENSITY,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_ARM2DENSITY,GET_STORED_TRANSACTION_ARM2DENSITY,1,15,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_ARM3DENSITY,GET_STORED_TRANSACTION_ARM3DENSITY,1,16,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_ARM4DENSITY,GET_STORED_TRANSACTION_ARM4DENSITY,1,17,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_UNIQUENUMBER,GET_STORED_TRANSACTION_UNIQUENUMBER,1,18,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_FIRSTARMNUMBER,GET_STORED_TRANSACTION_FIRSTARMNUMBER,1,19,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredTransactionsTag->AddLeaf(IDS_GET_STORED_TRANSACTION_CHECKSUMRESULT,GET_STORED_TRANSACTION_CHECKSUMRESULT,1,20,OPC_READABLE,VT_BSTR,pIO,pDevice);
	
	

	/***************** Stored Entries ***************************************/
	CTag* pStoredEntriesTag=pContrecTag->AddBranch(IDS_STORED_ENTRIES,REQUEST_STATUS_CMD,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_SET_STORED_ENTRIES_NUMBER,SET_STORED_ENTRIES_NUMBER,1,1,OPC_WRITEABLE | OPC_READABLE,VT_I4,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_ENTRYNUMBER,GET_STORED_ENTRIES_ENTRYNUMBER,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_TRANSACTIONNUMBER,GET_STORED_ENTRIES_TRANSACTIONNUMBER,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_ARMNUMBER,GET_STORED_ENTRIES_ARMNUMBER,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_COMPARTMENTNUMBER,GET_STORED_ENTRIES_COMPARTMENTNUMBER,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_GROSSTOTAL,GET_STORED_ENTRIES_GROSSTOTAL,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_NETTOTAL,GET_STORED_ENTRIES_NETTOTAL,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_GROSSACCUMBEFORE,GET_STORED_ENTRIES_GROSSACCUMBEFORE,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_GROSSACCUMAFTER,GET_STORED_ENTRIES_GROSSACCUMAFTER,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_NETACCUMBEFORE,GET_STORED_ENTRIES_NETACCUMBEFORE,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_NETACCUMAFTER,GET_STORED_ENTRIES_NETACCUMAFTER,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_AVERTEMP,GET_STORED_ENTRIES_AVERTEMP,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_PRESETQUANTITY,GET_STORED_ENTRIES_PRESETQUANTITY,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_ERRORSTATUS,GET_STORED_ENTRIES_ERRORSTATUS,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pStoredEntriesTag->AddLeaf(IDS_GET_STORED_ENTRIES_RETURNQUANTITY,GET_STORED_ENTRIES_RETURNQUANTITY,1,2,OPC_READABLE,VT_BSTR,pIO,pDevice);
	

	
	/***************** Option Tags ***************************************/

	CTag* pOptionTag=pContrecTag->AddBranch(IDS_OPTION,REQUEST_STATUS_CMD,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_TESTMODE,GET_OPTION_TESTMODE,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_DEADMANTIMER,GET_OPTION_DEADMANTIMER,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_ILLEGALACCESS,GET_OPTION_ILLEGALACCESS,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_ALARMONFAULT,GET_OPTION_ALARMONFAULT,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_COMPARTMENTPROMPT,GET_OPTION_COMPARTMENTPROMPT,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_RETURNPROMPT,GET_OPTION_RETURNPROMPT,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_LOADNUMBERPROMPT,GET_OPTION_LOADNUMBERPROMPT,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_LOADSCHEDULING,GET_OPTION_LOADSCHEDULING,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_SLAVEMODE,GET_OPTION_SLAVEMODE,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_REMOTEAUTH,GET_OPTION_REMOTEAUTH,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_SIMARMLOADING,GET_OPTION_SIMARMLOADING,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_PRESETQUANPROMPT,GET_OPTION_PRESETQUANPROMPT,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_MULLOADSPERARM,GET_OPTION_MULLOADSPERARM,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pOptionTag->AddLeaf(IDS_GET_OPTION_MAXPRESET,GET_OPTION_MAXPRESET,1,14,OPC_READABLE,VT_BSTR,pIO,pDevice);


	/***************** Status Tags ***************************************/

	CTag* pStatusTag=pContrecTag->AddBranch(IDS_STATUS,REQUEST_STATUS_CMD,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_CONTREC_STATUS,ISSUE_ENQ_COMMAND_STATUS,1,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_LASTTRANS_NUMBER,ISSUE_ENQ_COMMAND_LASTTRANSNUMBER,1,1,OPC_READABLE,VT_I4,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_FIRSTARM_NUMBER,ISSUE_ENQ_COMMAND_FIRSTARM,1,2,OPC_READABLE,VT_I4,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_NUMBER_ARMS,ISSUE_ENQ_COMMAND_NUMARMS,1,3,OPC_READABLE,VT_I4,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_BATCH_INPROGRESS,READ_BATCH_INPROGRESS,1,4,OPC_READABLE,VT_I4,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_BATCH_COMPLETE,REQUEST_BATCH_COMPLETE,1,5,OPC_READABLE,VT_BOOL,pIO,pDevice);

	/***************** Arm Tags ***************************************/

	CTag* pArmsTag=pContrecTag->AddBranch(IDS_ARMS,REQUEST_STATUS_CMD,pIO,pDevice);
	CTag* pArmsTag1=pArmsTag->AddBranch(IDS_ARM_1,REQUEST_STATUS_CMD,pIO,pDevice);
	CTag* pArmsTag2=pArmsTag->AddBranch(IDS_ARM_2,REQUEST_STATUS_CMD,pIO,pDevice);
	CTag* pArmsTag3=pArmsTag->AddBranch(IDS_ARM_3,REQUEST_STATUS_CMD,pIO,pDevice);
	CTag* pArmsTag4=pArmsTag->AddBranch(IDS_ARM_4,REQUEST_STATUS_CMD,pIO,pDevice);

	pArmsTag1->AddLeaf(IDS_ARM_FLOW_RATE,READ_ARM1_FLOW_RATE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag2->AddLeaf(IDS_ARM_FLOW_RATE,READ_ARM2_FLOW_RATE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag3->AddLeaf(IDS_ARM_FLOW_RATE,READ_ARM3_FLOW_RATE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag4->AddLeaf(IDS_ARM_FLOW_RATE,READ_ARM4_FLOW_RATE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);

	pArmsTag1->AddLeaf(IDS_ARM_TEMPERATURE,ISSUE_ARM1_TEMPERATURE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag2->AddLeaf(IDS_ARM_TEMPERATURE,ISSUE_ARM2_TEMPERATURE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag3->AddLeaf(IDS_ARM_TEMPERATURE,ISSUE_ARM3_TEMPERATURE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag4->AddLeaf(IDS_ARM_TEMPERATURE,ISSUE_ARM4_TEMPERATURE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);

	pArmsTag1->AddLeaf(IDS_ARM_LASTLOAD_TEMPERATURE,ISSUE_ARM1_LASTLOAD_TEMPERATURE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag2->AddLeaf(IDS_ARM_LASTLOAD_TEMPERATURE,ISSUE_ARM2_LASTLOAD_TEMPERATURE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag3->AddLeaf(IDS_ARM_LASTLOAD_TEMPERATURE,ISSUE_ARM3_LASTLOAD_TEMPERATURE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag4->AddLeaf(IDS_ARM_LASTLOAD_TEMPERATURE,ISSUE_ARM4_LASTLOAD_TEMPERATURE,0,0,OPC_READABLE,VT_R8,pIO,pDevice);

	pArmsTag1->AddLeaf(IDS_ARM_STATUS,ISSUE_ENQ_COMMAND_ARM1STATUS,0,1,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTag2->AddLeaf(IDS_ARM_STATUS,ISSUE_ENQ_COMMAND_ARM2STATUS,0,1,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTag3->AddLeaf(IDS_ARM_STATUS,ISSUE_ENQ_COMMAND_ARM3STATUS,0,1,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTag4->AddLeaf(IDS_ARM_STATUS,ISSUE_ENQ_COMMAND_ARM4STATUS,0,1,OPC_READABLE,VT_BSTR,pIO,pDevice);

	pArmsTag1->AddLeaf(IDS_ARM_PRESET_AMOUNT,ARM1_PRESET_AMOUNT,0,1,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag2->AddLeaf(IDS_ARM_PRESET_AMOUNT,ARM2_PRESET_AMOUNT,0,1,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag3->AddLeaf(IDS_ARM_PRESET_AMOUNT,ARM3_PRESET_AMOUNT,0,1,OPC_READABLE,VT_R8,pIO,pDevice);
	pArmsTag4->AddLeaf(IDS_ARM_PRESET_AMOUNT,ARM4_PRESET_AMOUNT,0,1,OPC_READABLE,VT_R8,pIO,pDevice);

	pArmsTag1->AddLeaf(IDS_ARM_ERROR_STATUS,GET_ARM1_ERROR_STATUS,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTag2->AddLeaf(IDS_ARM_ERROR_STATUS,GET_ARM2_ERROR_STATUS,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTag3->AddLeaf(IDS_ARM_ERROR_STATUS,GET_ARM3_ERROR_STATUS,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTag4->AddLeaf(IDS_ARM_ERROR_STATUS,GET_ARM4_ERROR_STATUS,0,0,OPC_READABLE,VT_I4,pIO,pDevice);

	pArmsTag1->AddLeaf(IDS_ARM_DENSITY,WRITE_ARM1_DENSITY,0,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pArmsTag2->AddLeaf(IDS_ARM_DENSITY,WRITE_ARM2_DENSITY,0,0,OPC_READABLE | OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pArmsTag3->AddLeaf(IDS_ARM_DENSITY,WRITE_ARM3_DENSITY,0,0,OPC_READABLE | OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pArmsTag4->AddLeaf(IDS_ARM_DENSITY,WRITE_ARM4_DENSITY,0,0,OPC_READABLE | OPC_WRITEABLE,VT_BSTR,pIO,pDevice);

	pArmsTag1->AddLeaf(IDS_READ_ARM_DENSITY,READ_ARM1_DENSITY,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTag2->AddLeaf(IDS_READ_ARM_DENSITY,READ_ARM2_DENSITY,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTag3->AddLeaf(IDS_READ_ARM_DENSITY,READ_ARM3_DENSITY,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTag4->AddLeaf(IDS_READ_ARM_DENSITY,READ_ARM4_DENSITY,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	/***************** Arm Totals Tags ***************************************/

	CTag* pArmsTotalsTag=pContrecTag->AddBranch(IDS_ARMS_TOTALS,REQUEST_STATUS_CMD,pIO,pDevice);
	CTag* pArmsTotalsTag1=pArmsTotalsTag->AddBranch(IDS_ARM_1,REQUEST_STATUS_CMD,pIO,pDevice);
	CTag* pArmsTotalsTag2=pArmsTotalsTag->AddBranch(IDS_ARM_2,REQUEST_STATUS_CMD,pIO,pDevice);
	CTag* pArmsTotalsTag3=pArmsTotalsTag->AddBranch(IDS_ARM_3,REQUEST_STATUS_CMD,pIO,pDevice);
	CTag* pArmsTotalsTag4=pArmsTotalsTag->AddBranch(IDS_ARM_4,REQUEST_STATUS_CMD,pIO,pDevice);

	pArmsTotalsTag1->AddLeaf(IDS_GROSS_VOLUME,ISSUE_ARM1_COMMAND_GROSS_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag2->AddLeaf(IDS_GROSS_VOLUME,ISSUE_ARM2_COMMAND_GROSS_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag3->AddLeaf(IDS_GROSS_VOLUME,ISSUE_ARM3_COMMAND_GROSS_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag4->AddLeaf(IDS_GROSS_VOLUME,ISSUE_ARM4_COMMAND_GROSS_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);

	pArmsTotalsTag1->AddLeaf(IDS_NET_VOLUME,ISSUE_ARM1_COMMAND_NET_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag2->AddLeaf(IDS_NET_VOLUME,ISSUE_ARM2_COMMAND_NET_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag3->AddLeaf(IDS_NET_VOLUME,ISSUE_ARM3_COMMAND_NET_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag4->AddLeaf(IDS_NET_VOLUME,ISSUE_ARM4_COMMAND_NET_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);

	pArmsTotalsTag1->AddLeaf(IDS_ACCUM_GROSS_VOLUME,GET_ARM1_ACCUM_GROSS_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag2->AddLeaf(IDS_ACCUM_GROSS_VOLUME,GET_ARM2_ACCUM_GROSS_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag3->AddLeaf(IDS_ACCUM_GROSS_VOLUME,GET_ARM3_ACCUM_GROSS_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag4->AddLeaf(IDS_ACCUM_GROSS_VOLUME,GET_ARM4_ACCUM_GROSS_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);

	pArmsTotalsTag1->AddLeaf(IDS_ACCUM_NET_VOLUME,GET_ARM1_ACCUM_NET_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag2->AddLeaf(IDS_ACCUM_NET_VOLUME,GET_ARM2_ACCUM_NET_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag3->AddLeaf(IDS_ACCUM_NET_VOLUME,GET_ARM3_ACCUM_NET_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pArmsTotalsTag4->AddLeaf(IDS_ACCUM_NET_VOLUME,GET_ARM4_ACCUM_NET_TOTAL,0,0,OPC_READABLE,VT_I4,pIO,pDevice);

	pArmsTotalsTag1->AddLeaf(IDS_BATCH_AVER_TEMP,GET_ARM1_BATCH_AVER_TEMP,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag2->AddLeaf(IDS_BATCH_AVER_TEMP,GET_ARM2_BATCH_AVER_TEMP,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag3->AddLeaf(IDS_BATCH_AVER_TEMP,GET_ARM3_BATCH_AVER_TEMP,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag4->AddLeaf(IDS_BATCH_AVER_TEMP,GET_ARM4_BATCH_AVER_TEMP,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);

	pArmsTotalsTag1->AddLeaf(IDS_BATCH_PROD_DENSITY,GET_ARM1_BATCH_PROD_DENSITY,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag2->AddLeaf(IDS_BATCH_PROD_DENSITY,GET_ARM2_BATCH_PROD_DENSITY,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag3->AddLeaf(IDS_BATCH_PROD_DENSITY,GET_ARM3_BATCH_PROD_DENSITY,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag4->AddLeaf(IDS_BATCH_PROD_DENSITY,GET_ARM4_BATCH_PROD_DENSITY,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);

	pArmsTotalsTag1->AddLeaf(IDS_BATCH_COMPARTMENT_NUMBER,GET_ARM1_BATCH_COMPARTMENT_NUMBER,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag2->AddLeaf(IDS_BATCH_COMPARTMENT_NUMBER,GET_ARM2_BATCH_COMPARTMENT_NUMBER,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag3->AddLeaf(IDS_BATCH_COMPARTMENT_NUMBER,GET_ARM3_BATCH_COMPARTMENT_NUMBER,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag4->AddLeaf(IDS_BATCH_COMPARTMENT_NUMBER,GET_ARM4_BATCH_COMPARTMENT_NUMBER,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);

	pArmsTotalsTag1->AddLeaf(IDS_READ_ARM_BATCH_TRANSACTION_NUMBER,READ_ARM1_BATCH_TRANSACTION_NUMBER,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag2->AddLeaf(IDS_READ_ARM_BATCH_TRANSACTION_NUMBER,READ_ARM2_BATCH_TRANSACTION_NUMBER,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag3->AddLeaf(IDS_READ_ARM_BATCH_TRANSACTION_NUMBER,READ_ARM3_BATCH_TRANSACTION_NUMBER,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pArmsTotalsTag4->AddLeaf(IDS_READ_ARM_BATCH_TRANSACTION_NUMBER,READ_ARM4_BATCH_TRANSACTION_NUMBER,0,0,OPC_READABLE,VT_BSTR,pIO,pDevice);




	/***************** Alarm Tags ***************************************/

	CTag* pAlarmsTag=pContrecTag->AddBranch(IDS_ALARMS_BRANCH,REQUEST_STATUS_CMD,pIO,pDevice);
	pAlarmsTag->AddLeaf(IDS_POWERFAIL_ALARM,REQUEST_POWERFAIL_ALARM_STATUS,0,0,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pAlarmsTag->AddLeaf(IDS_POWERFAIL_ALARM_CLEAR,ISSUE_POWERFAIL_ALARM_CLEAR,0,0,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);

/*
	pStatusTag->AddLeaf(IDS_PRIMARY_ALARM,REQUEST_STATUS_CMD,0,1,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PASS_CODE_ENTRY_IN_PROGRESS,REQUEST_STATUS_CMD,0,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_OPERATION_TIMEDOUT,REQUEST_STATUS_CMD,0,3,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_RECIPE_SELECTED,REQUEST_STATUS_CMD,0,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_ADDITIVES_SELECTED,REQUEST_STATUS_CMD,0,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PRESET_ENTERED,REQUEST_STATUS_CMD,0,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_KEYPAD_DATA_AVAILABLE,REQUEST_STATUS_CMD,0,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PROGRAM_CODE_CHANGED,REQUEST_STATUS_CMD,1,0,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_TRANSACTION_IN_PROGRESS,REQUEST_STATUS_CMD,1,1,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_BATCH_IN_PROGRESS,REQUEST_STATUS_CMD,1,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_KEY_PRESSED,REQUEST_STATUS_CMD,1,3,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_TRANSACTION_ENDED,REQUEST_STATUS_CMD,1,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_BATCH_ENDED,REQUEST_STATUS_CMD,1,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_BATCH_ABORTED,REQUEST_STATUS_CMD,1,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_INTERMEDIATE_ALARM_STOPPED_BATCH,REQUEST_STATUS_CMD,1,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_TRANSACTION_END_REQUESTED,REQUEST_STATUS_CMD,2,3,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_KEYPAD_AND_DISPLAY_LOCKED,REQUEST_STATUS_CMD,2,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PROGRAM_MODE,REQUEST_STATUS_CMD,2,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_FLOWING,REQUEST_STATUS_CMD,2,6,OPC_READABLE,VT_BOOL,pIO,pDevice);

	pStatusTag->AddLeaf(IDS_SWING_ARM,REQUEST_STATUS_CMD,4,0,OPC_READABLE,VT_UI1,pIO,pDevice);

	pStatusTag->AddLeaf(IDS_GROSS_VOLUME,REQUEST_STATUS_CMD,5,0,OPC_READABLE,VT_I4,pIO,pDevice);

	pStatusTag->AddLeaf(IDS_NET_VOLUME,REQUEST_STATUS_CMD,9,0,OPC_READABLE,VT_I4,pIO,pDevice);

	CTag* pSafetyStatusTag=pStatusTag->AddBranch(IDS_SAFETY,REQUEST_STATUS_CMD,pIO,pDevice);

	BYTE bSection=13;
	BYTE bBit=0;
	for(int iSafetyStatus=1;iSafetyStatus < 9;iSafetyStatus++)
	{
		CString	oSafety;
		oSafety.Format(_T("%d"),iSafetyStatus);
		pSafetyStatusTag->AddLeaf(oSafety,REQUEST_STATUS_CMD,bSection,bBit++,OPC_READABLE,VT_BOOL,pIO,pDevice);
	}

//	pStatusTag->AddLeaf(IDS_OLDEST_ALARM,REQUEST_STATUS_CMD,14,0,OPC_READABLE,VT_UI1,pIO,pDevice);


	CTag* pAlarmTag=pContrecTag->AddBranch(IDS_ALARMS,REQUEST_STATUS_CMD,pIO,pDevice);

	pAlarmTag->AddLeaf(IDS_PRIMARY_DISPLAY_FAILURE,REQUEST_STATUS_CMD,21,0,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pAlarmTag->AddLeaf(IDS_SECONDARY_DISPLAY_FAILURE,REQUEST_STATUS_CMD,21,1,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pAlarmTag->AddLeaf(IDS_CHANNEL_A_COMM_FAILURE,REQUEST_STATUS_CMD,21,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pAlarmTag->AddLeaf(IDS_CHANNEL_B_COMM_FAILURE,REQUEST_STATUS_CMD,21,3,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pAlarmTag->AddLeaf(IDS_BLEND,REQUEST_STATUS_CMD,21,4,OPC_READABLE,VT_BOOL,pIO,pDevice);

	bSection=21;
	bBit=5;

	CTag* pMetersTag=pAlarmTag->AddBranch(IDS_METER,REQUEST_STATUS_CMD,pIO,pDevice);

	int MeterIDs[9]={	IDS_LOW_FLOW,
							IDS_HIGH_FLOW,
							IDS_VALVE_CLOSED_EARLY,
							IDS_FLOW_TIMEOUT,
							IDS_UNAUTHORIZED_FLOW,
							IDS_PULSE_SECURITY_ERROR,
							IDS_TEMPERATURE_FAILURE,
							IDS_PRESSURE_FAILURE,
							IDS_VALVE_CLOSE_FAILURE};

	for(int iMeter=1;iMeter < 5;iMeter++)
	{
		CString	oMeter;
		oMeter.Format(_T("%d"),iMeter);
		CTag* pMeterTag=pMetersTag->AddBranch(oMeter,REQUEST_STATUS_CMD,pIO,pDevice);
		for(int iID=0;iID < 9;iID++)
		{
			pMeterTag->AddLeaf(MeterIDs[iID],REQUEST_STATUS_CMD,bSection,bBit++,OPC_READABLE,VT_BOOL,pIO,pDevice);
			if(bBit > 7)
			{
				bBit=0;
				bSection--;
			}
		}
	}

	CTag* pComponentsTag=pAlarmTag->AddBranch(IDS_COMPONENT,REQUEST_STATUS_CMD,pIO,pDevice);

	int ComponentIDs[2]={	IDS_DENSITY_FAILURE,
									IDS_BLOCK_VALVE_CLOSE_FAILURE};

	for(int iComponent=1;iComponent < 5;iComponent++)
	{
		CString	oComponent;
		oComponent.Format(_T("%d"),iComponent);
		CTag* pComponentTag=pComponentsTag->AddBranch(oComponent,REQUEST_STATUS_CMD,pIO,pDevice);
		for(int iID=0;iID < 2;iID++)
		{
			pComponentTag->AddLeaf(ComponentIDs[iID],REQUEST_STATUS_CMD,bSection,bBit++,OPC_READABLE,VT_BOOL,pIO,pDevice);
			if(bBit > 7)
			{
				bBit=0;
				bSection--;
			}
		}
	}

	CTag* pAdditivesTag=pAlarmTag->AddBranch(IDS_ADDITIVE,REQUEST_STATUS_CMD,pIO,pDevice);

	for(int iAdditive=1;iAdditive < 7;iAdditive++)
	{
		CString	oAdditive;
		oAdditive.Format(_T("%d"),iAdditive);
		pAdditivesTag->AddLeaf(oAdditive,REQUEST_STATUS_CMD,bSection,bBit++,OPC_READABLE,VT_BOOL,pIO,pDevice);
		if(bBit > 7)
		{
			bBit=0;
			bSection--;
		}
	}

	CTag* pSafetyAlarmsTag=pAlarmTag->AddBranch(IDS_SAFETY,REQUEST_STATUS_CMD,pIO,pDevice);

	for(int iSafetyAlarm=1;iSafetyAlarm < 9;iSafetyAlarm++)
	{
		CString	oSafety;
		oSafety.Format(_T("%d"),iSafetyAlarm);
		pSafetyAlarmsTag->AddLeaf(oSafety,REQUEST_STATUS_CMD,bSection,bBit++,OPC_READABLE,VT_BOOL,pIO,pDevice);
		if(bBit > 7)
		{
			bBit=0;
			bSection--;
		}
	}

	int ComponentTotalizerIDs[2]={	IDS_GROSS_VOLUME,
												IDS_NET_VOLUME};

	CTag* pComponentTotalizersTag=pContrecTag->AddBranch(IDS_COMPONENT_TOTALIZERS,COMPONENT_TOTALIZERS_CMD,pIO,pDevice);

	
	bSection=0;
	for(int iComponent=1;iComponent < 5;iComponent++)
	{
		CString	oComponent;
		oComponent.Format(_T("%d"),iComponent);
		CTag* pComponentTag=pComponentTotalizersTag->AddBranch(oComponent,COMPONENT_TOTALIZERS_CMD,pIO,pDevice);
		for(int iID=0;iID < 2;iID++)
		{
			pComponentTag->AddLeaf(ComponentTotalizerIDs[iID],COMPONENT_TOTALIZERS_CMD,bSection,0,OPC_READABLE,VT_I4,pIO,pDevice);
			bSection++;
		}
	}

	CTag* pAdditiveTotalizersTag=pContrecTag->AddBranch(IDS_ADDITIVE_TOTALIZERS,ADDITIVE_TOTALIZERS_CMD,pIO,pDevice);

	
	bSection=0;
	for(int iAdditive=1;iAdditive < 7;iAdditive++)
	{
		CString	oAdditive;
		oAdditive.Format(_T("%d"),iAdditive);
		CTag* pAdditiveTag=pAdditiveTotalizersTag->AddBranch(oAdditive,ADDITIVE_TOTALIZERS_CMD,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_GROSS_VOLUME,ADDITIVE_TOTALIZERS_CMD,bSection,0,OPC_READABLE,VT_I4,pIO,pDevice);
		bSection++;
	}


	// This Section is for Component Batch Values
	int ComponentValueIDs[9]={	IDS_GROSS_TOTALIZER,
										IDS_NET_TOTALIZER,
										IDS_GROSS_VOLUME,
										IDS_NET_VOLUME,
										IDS_AVERAGE_TEMPERATURE,
										IDS_AVERAGE_DENSITY,
										IDS_AVERAGE_PRESSURE,
										IDS_AVERAGE_VALVE_CLOSURES,
										IDS_PERCENT};

	VARENUM ComponentValueType[9] = {	VT_I4,
													VT_I4,
													VT_I4,
													VT_I4,
													VT_I2,
													VT_I4,
													VT_I4,
													VT_I2,
													VT_I2};


	CTag* pComponentValuesTag=pContrecTag->AddBranch(IDS_COMPONENT_VALUES,REQUEST_COMPONENT_VALUES_CMD,pIO,pDevice);

	
	for(int iComponent=1;iComponent < 5;iComponent++)
	{
		bSection=0;
		CString	oComponent;
		oComponent.Format(_T("%d"),iComponent);
		CTag* pComponentTag=pComponentValuesTag->AddBranch(oComponent,REQUEST_COMPONENT_VALUES_CMD,pIO,pDevice);
		for(int iID=0;iID < 9;iID++)
		{
			pComponentTag->AddLeaf(ComponentValueIDs[iID],REQUEST_COMPONENT_VALUES_CMD,bSection,0,OPC_READABLE,ComponentValueType[iID],pIO,pDevice);
			if(ComponentValueType[iID] == VT_I2)
				bSection+=2;
			else if(ComponentValueType[iID] == VT_I4)
				bSection+=4;
		}
	}

	CTag* pInputTag=pContrecTag->AddBranch(IDS_INPUT,READ_INPUT_CMD,pIO,pDevice);

	CTag* pMeterInputTag=pInputTag->AddBranch(IDS_METER,READ_INPUT_CMD,pIO,pDevice);
	for(int iMeter=1;iMeter < 5;iMeter++)
	{
		CString oMeter;
		oMeter.Format(_T("%d"),iMeter);
		CTag* pMeterTag=pMeterInputTag->AddLeaf(oMeter,READ_INPUT_CMD,METER,iMeter,OPC_READABLE,VT_I2,pIO,pDevice);
	}

	CTag* pRTDInputTag=pInputTag->AddBranch(IDS_RTD,READ_INPUT_CMD,pIO,pDevice);
	for(int iRTD=1;iRTD < 6;iRTD++)
	{
		CString oRTD;
		oRTD.Format(_T("%d"),iRTD);
		CTag* pRTDTag=pRTDInputTag->AddLeaf(oRTD,READ_INPUT_CMD,RTD,iRTD,OPC_READABLE,VT_I2,pIO,pDevice);
	}

	CTag* pCurrentLoopInputTag=pInputTag->AddBranch(IDS_CURRENT_LOOP,READ_INPUT_CMD,pIO,pDevice);
	for(int iCurrentLoop=1;iCurrentLoop < 10;iCurrentLoop++)
	{
		CString oCurrentLoop;
		oCurrentLoop.Format(_T("%d"),iCurrentLoop);
		CTag* pCurrentLoopTag=pCurrentLoopInputTag->AddLeaf(oCurrentLoop,READ_INPUT_CMD,CURRENT_LOOP,iCurrentLoop,OPC_READABLE,VT_I2,pIO,pDevice);
	}

	CTag* pDiscreteInputTag=pInputTag->AddBranch(IDS_DISCRETE,READ_INPUT_CMD,pIO,pDevice);
	for(int iDiscrete=1;iDiscrete < 25;iDiscrete++)
	{
		CString oDiscrete;
		oDiscrete.Format(_T("%2d"),iDiscrete);
		CTag* pDiscreteTag=pDiscreteInputTag->AddLeaf(oDiscrete,READ_INPUT_CMD,DISCRETE_INPUT,iDiscrete,OPC_READABLE,VT_BOOL,pIO,pDevice);
	}

	CTag* pOutputTag=pContrecTag->AddBranch(IDS_OUTPUT,WRITE_OUTPUT_CMD,pIO,pDevice);
	CTag* pDiscreteOutputTag=pOutputTag->AddBranch(IDS_DISCRETE,WRITE_OUTPUT_CMD,pIO,pDevice);
	for(int iDiscrete=1;iDiscrete < 29;iDiscrete++)
	{
		CString oDiscrete;
		oDiscrete.Format(_T("%2d"),iDiscrete);
		CTag* pDiscreteTag=pDiscreteOutputTag->AddLeaf(oDiscrete,WRITE_OUTPUT_CMD,DISCRETE_OUTPUT,iDiscrete,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	}
*/
}

void CDeviceManager::InitializeTagDatabase()
{
	try
	{
		m_pRoot=new CTag(_T(""),0);
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

		// For each Preset (Contrec/Microload)
		IContrecsPtr	oContrecs(CLSID_Contrecs);
		IContrecCollectionPtr	oContrecCollection=oContrecs->Enumerate();
		for(LONG lItem=0;lItem < oContrecCollection->Count;lItem++)
		{
			IContrecPtr	oContrec=oContrecCollection->Item(lItem);
			AddContrec(oContrec);
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

void CDeviceManager::AddContrec(IContrecPtr oContrec)
{
	CSLock Lock(&m_cs);

	if(!m_OPCServerList.GetCount())
		return;

	COPCLock OPCLock(&m_OPCServerList);

	// Determine the I/O Object for this Comm Port
	CIO*	pIO=NULL;
	POSITION	pos=m_IOList.GetHeadPosition();
	while(pos)
	{
		pIO=m_IOList.GetNext(pos);
		if(pIO->m_lIndex == oContrec->PortIndex)
			break;
		pIO=NULL;
	}

	if(!pIO
	&& oContrec->PortIndex != 0)
	{
		IPortsPtr	oPorts(CLSID_Ports);
		IPortPtr		oPort=oPorts->Get(oContrec->PortIndex);

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

	if ( pIO != NULL )
		pIO->m_dwUseCount++;

	CDevice* pDevice=new CDevice(oContrec->Address);

	if(oContrec->Type == CONTREC1010)
		AddContrecDevice(oContrec,pIO,pDevice);

}

void CDeviceManager::PurgeDevice(LPTSTR szID)
{
	CSLock Lock(&m_cs);

	if(!m_OPCServerList.GetCount())
		return;

	COPCLock OPCLock(&m_OPCServerList);

	CString	oTag;

	oTag=szID;

	CTag*	pTag=FindTag(oTag.GetBuffer(0));
	if(!pTag)
		return;

	CIO*	pIO=pTag->m_pIO;
	CDevice* pDevice=pTag->m_pDevice;
	
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

	if(pDevice)
		delete pDevice;
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
					if(((CContrecItem*) pItem)->m_oTag == oPath)
					{
						((CContrecItem*) pItem)->m_pTag=pTag;
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
					if(((CContrecItem*) pItem)->m_pTag == pTag)
					{
						if(pItem->m_bActive
						&& pTag->m_dwAccessRights & OPC_READABLE
						&& pTag->m_pIO)
							pTag->m_pIO->RemoveTagFromScanList(pTag);
						((CContrecItem*) pItem)->m_pTag=NULL;
					}
				}
			}
		}
	}
}

void CDeviceManager::UpdateGroups()
{
	CSLock wait(&m_cs);
	POSITION pos=m_OPCServerList.GetHeadPosition();
	while(pos)
	{
		COPCServer*	pOPCServer=m_OPCServerList.GetNext(pos);
		CSLock Lock(&pOPCServer->m_cs);


	   POSITION pos = pOPCServer->m_groupMap.GetStartPosition();
		LPVOID	key=0;
	   OPCGroupObject* pGroup = NULL;
		while(pos)
		{
			pOPCServer->m_groupMap.GetNextAssoc(pos,key,pGroup);
			SetEvent(pGroup->m_hTimer);
		}
	}
}