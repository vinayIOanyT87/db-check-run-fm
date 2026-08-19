/******************************************************************************

	FILE NAME:		DeviceManager.h


	PURPOSE:			Declaration of the CDeviceManager


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
#pragma once

#include "resource.h"       // main symbols
#include "IO.h"
#include "opcserver.h"

// Commands
#define ADDITIVE_TOTALIZERS_CMD									0x11
#define REQUEST_STATUS_CMD											0x12
#define COMPONENT_TOTALIZERS_CMD									0x16
#define REQUEST_COMPONENT_VALUES_CMD							0x1A
#define START_COMMUNICATIONS_CMD									0x21

#define	ISSUE_ENQ_COMMAND_STATUS								1		//0xc0
#define	ISSUE_ENQ_COMMAND_LASTTRANSNUMBER					2		//0xc0
#define	ISSUE_ENQ_COMMAND_FIRSTARM								3		//0xc0
#define	ISSUE_ENQ_COMMAND_NUMARMS								4		//0xc0
#define	ISSUE_ENQ_COMMAND_ARM1STATUS							5		//0xc0
#define	ISSUE_ENQ_COMMAND_ARM2STATUS							6		//0xc0
#define	ISSUE_ENQ_COMMAND_ARM3STATUS							7		//0xc0
#define	ISSUE_ENQ_COMMAND_ARM4STATUS							8		//0xc0
#define	ISSUE_ARM1_COMMAND_GROSS_TOTAL						9		//GT
#define	ISSUE_ARM2_COMMAND_GROSS_TOTAL						10		//GT
#define	ISSUE_ARM3_COMMAND_GROSS_TOTAL						11		//GT
#define	ISSUE_ARM4_COMMAND_GROSS_TOTAL						12		//GT
#define	ISSUE_COMMAND_APP_VERSION								13		//AV
#define	ISSUE_COMMAND_APP_VERSION_DATETIME					14		//AV
#define	READ_ARM1_FLOW_RATE										15		//FR
#define	READ_ARM2_FLOW_RATE										16		//FR
#define	READ_ARM3_FLOW_RATE										17		//FR
#define	READ_ARM4_FLOW_RATE										18		//FR
#define	ISSUE_ARM1_TEMPERATURE									19		//IT
#define	ISSUE_ARM2_TEMPERATURE									20		//IT
#define	ISSUE_ARM3_TEMPERATURE									21		//IT
#define	ISSUE_ARM4_TEMPERATURE									22		//IT
#define	ISSUE_ARM1_LASTLOAD_TEMPERATURE						23		//LT
#define	ISSUE_ARM2_LASTLOAD_TEMPERATURE						24		//LT
#define	ISSUE_ARM3_LASTLOAD_TEMPERATURE						25		//LT
#define	ISSUE_ARM4_LASTLOAD_TEMPERATURE						26		//LT
#define	ISSUE_ARM1_COMMAND_NET_TOTAL							27		//NT
#define	ISSUE_ARM2_COMMAND_NET_TOTAL							28		//NT
#define	ISSUE_ARM3_COMMAND_NET_TOTAL							29		//NT
#define	ISSUE_ARM4_COMMAND_NET_TOTAL							30		//NT
#define	ISSUE_COMMAND_SYSTEM_VERSION_DATETIME				31		//GD
#define	ISSUE_COMMAND_POWER_CYCLE_DATETIME					32		//PD
#define	ISSUE_ENQ_COMMAND_FIELD									33		//0xC0
#define	READ_BATCH_INPROGRESS									34		//0xC0

#define	ISSUE_DISPLAY_PROMPT										35		//DP
#define	ISSUE_DISPLAY_MESSAGE									36		//DM
#define	ISSUE_CLEAR_DISPLAY										37		//CM
#define	ISSUE_REMOTE_AUTHORIZE									38		//RA
#define	GET_DRIVER_PIN_NUMBER									39		//0xC0
#define	ISSUE_TRUCK_ID												40		//0xC0
#define	ISSUE_LOAD_NUMBER											41		//0xC0
#define	ISSUE_GETANSWER_MESSAGE									42		//GA
#define	GET_ENTERED_KEYBOARD_DATA								43		//0xC0
#define	ISSUE_HIDDENANSWER_MESSAGE								44		//GH
#define	ISSUE_TERMINATE_TRANSACTION							45		//TT
#define	REQUEST_POWERFAIL_ALARM_STATUS						46		//0xC0
#define	ISSUE_POWERFAIL_ALARM_CLEAR							47		//CC
#define	REQUEST_BATCH_COMPLETE									48		//0xC0
#define	DISPLAY_MESSAGE_TIMEOUT									49		//Internal
#define	SET_MESSAGE_TIMEOUT										50		//Internal
#define	ALTER_ARM_NAME												51		//AA
#define	LOAD_NUMBER_RESPONSE										52		//RL
#define	COMPARTMENT_RESPONSE										53		//RC
#define	BATCH_TOTALS												54		//BT
#define	TRANSACTION_COMPLETE										55		//TC
#define	ARM1_PRESET_AMOUNT										56		//PR
#define	ARM2_PRESET_AMOUNT										57		//PR
#define	ARM3_PRESET_AMOUNT										58		//PR
#define	ARM4_PRESET_AMOUNT										59		//PR
#define	GET_ARM1_ERROR_STATUS									60		//AM
#define	GET_ARM2_ERROR_STATUS									61		//AM
#define	GET_ARM3_ERROR_STATUS									62		//AM
#define	GET_ARM4_ERROR_STATUS									63		//AM
#define	GET_ARM1_ACCUM_GROSS_TOTAL								64		//AT
#define	GET_ARM2_ACCUM_GROSS_TOTAL								65		//AT
#define	GET_ARM3_ACCUM_GROSS_TOTAL								66		//AT
#define	GET_ARM4_ACCUM_GROSS_TOTAL								67		//AT
#define	GET_ARM1_ACCUM_NET_TOTAL								68		//AN
#define	GET_ARM2_ACCUM_NET_TOTAL								69		//AN
#define	GET_ARM3_ACCUM_NET_TOTAL								70		//AN
#define	GET_ARM4_ACCUM_NET_TOTAL								71		//AN
#define	GET_ARM1_BATCH_AVER_TEMP								72		//BT
#define	GET_ARM2_BATCH_AVER_TEMP								73		//BT
#define	GET_ARM3_BATCH_AVER_TEMP								74		//BT
#define	GET_ARM4_BATCH_AVER_TEMP								75		//BT
#define	GET_ARM1_BATCH_PROD_DENSITY							76		//BT
#define	GET_ARM2_BATCH_PROD_DENSITY							77		//BT
#define	GET_ARM3_BATCH_PROD_DENSITY							78		//BT
#define	GET_ARM4_BATCH_PROD_DENSITY							79		//BT
#define	GET_ARM1_BATCH_COMPARTMENT_NUMBER					80		//BT
#define	GET_ARM2_BATCH_COMPARTMENT_NUMBER					81		//BT
#define	GET_ARM3_BATCH_COMPARTMENT_NUMBER					82		//BT
#define	GET_ARM4_BATCH_COMPARTMENT_NUMBER					83		//BT
#define	WRITE_ARM1_DENSITY										84		//DN
#define	WRITE_ARM2_DENSITY										85		//DN
#define	WRITE_ARM3_DENSITY										86		//DN
#define	WRITE_ARM4_DENSITY										87		//DN
#define	SET_INITIAL_MESSAGE										88		//MI
#define	SET_INITIAL_MESSAGE_CONTROLLED						89		//MI
#define	ISSUE_GETTOUCHKEY_PROMPT								90		//GK
#define	GET_TOUCHKEY_DATA											91		//0xC0
#define	READ_ARM1_DENSITY											92		//AS
#define	READ_ARM2_DENSITY											93		//AS
#define	READ_ARM3_DENSITY											94		//AS
#define	READ_ARM4_DENSITY											95		//AS
#define	READ_ARM1_BATCH_TRANSACTION_NUMBER					96		//BT
#define	READ_ARM2_BATCH_TRANSACTION_NUMBER					97		//BT
#define	READ_ARM3_BATCH_TRANSACTION_NUMBER					98		//BT
#define	READ_ARM4_BATCH_TRANSACTION_NUMBER					99		//BT
#define	ISSUE_MANAGER_RESET										100	//MR
#define	ISSUE_RESET_DATE_TIME									101	//RD
#define	ISSUE_SET_PIN_NUMBERS									102	//CP

// options menu items
#define	GET_OPTION_TESTMODE										103	//OP
#define	GET_OPTION_DEADMANTIMER									104	//OP
#define	GET_OPTION_ILLEGALACCESS								105	//OP
#define	GET_OPTION_ALARMONFAULT									106	//OP
#define	GET_OPTION_COMPARTMENTPROMPT							107	//OP
#define	GET_OPTION_RETURNPROMPT									108	//OP
#define	GET_OPTION_LOADNUMBERPROMPT							109	//OP
#define	GET_OPTION_LOADSCHEDULING								110	//OP
#define	GET_OPTION_SLAVEMODE										111	//OP
#define	GET_OPTION_REMOTEAUTH									112	//OP
#define	GET_OPTION_SIMARMLOADING								113	//OP
#define	GET_OPTION_PRESETQUANPROMPT							114	//OP
#define	GET_OPTION_MULLOADSPERARM								115	//OP
#define	GET_OPTION_MAXPRESET										116	//OP
#define	GET_DRIVER_TOUCH_KEY										117	//0xC0
#define	GET_TRUCK_PIN_NUMBER										118	//0xC0
#define	GET_TRUCK_TOUCH_KEY										119	//0xC0
#define	ISSUE_REMOTEAUTH_ERRORMESSAGE							120	//RA

// send transaction defines used to retrieve the stored transactions from the contrec
// because we use an OPC server we need to break the returned data into individual elements
// not the most efficient but that is how we are going to do this
#define	SET_STORED_TRANSACTION_NUMBER							121	//ST
#define	GET_STORED_TRANSACTION_UNITADDRESS					122	//ST
#define	GET_STORED_TRANSACTION_TRANSACTIONUMBER			123	//ST
#define	GET_STORED_TRANSACTION_DATE							124	//ST
#define	GET_STORED_TRANSACTION_STARTTIME						125	//ST
#define	GET_STORED_TRANSACTION_STOPTIME						126	//ST
#define	GET_STORED_TRANSACTION_CALIBRATIONNUMBER			127	//ST
#define	GET_STORED_TRANSACTION_ENTRYSTART					128	//ST
#define	GET_STORED_TRANSACTION_ENTRYSTOP						129	//ST
#define	GET_STORED_TRANSACTION_DRIVERINDEX					130	//ST
#define	GET_STORED_TRANSACTION_TRUCKINDEX					131	//ST
#define	GET_STORED_TRANSACTION_LOADNUMBER					132	//ST
#define	GET_STORED_TRANSACTION_ARMNUMBER						133	//ST
#define	GET_STORED_TRANSACTION_ARM1DENSITY					134	//ST
#define	GET_STORED_TRANSACTION_ARM2DENSITY					135	//ST
#define	GET_STORED_TRANSACTION_ARM3DENSITY					136	//ST
#define	GET_STORED_TRANSACTION_ARM4DENSITY					137	//ST
#define	GET_STORED_TRANSACTION_UNIQUENUMBER					138	//ST
#define	GET_STORED_TRANSACTION_FIRSTARMNUMBER				139	//ST
#define	GET_STORED_TRANSACTION_CHECKSUMRESULT				140	//ST

// send entry defines used to get the stored transactions from the Contrec
#define SET_STORED_ENTRIES_NUMBER								141	//SY
#define GET_STORED_ENTRIES_ENTRYNUMBER							142	//SY
#define GET_STORED_ENTRIES_TRANSACTIONNUMBER					143	//SY
#define GET_STORED_ENTRIES_ARMNUMBER							144	//SY
#define GET_STORED_ENTRIES_COMPARTMENTNUMBER					145	//SY
#define GET_STORED_ENTRIES_GROSSTOTAL							146	//SY
#define GET_STORED_ENTRIES_NETTOTAL								147	//SY
#define GET_STORED_ENTRIES_GROSSACCUMBEFORE					148	//SY
#define GET_STORED_ENTRIES_GROSSACCUMAFTER					149	//SY
#define GET_STORED_ENTRIES_NETACCUMBEFORE						150	//SY
#define GET_STORED_ENTRIES_NETACCUMAFTER						151	//SY
#define GET_STORED_ENTRIES_AVERTEMP								152	//SY
#define GET_STORED_ENTRIES_PRESETQUANTITY						153	//SY
#define GET_STORED_ENTRIES_ERRORSTATUS							154	//SY
#define GET_STORED_ENTRIES_RETURNQUANTITY						155	//SY




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


class CDeviceManager
{
	CIOList				m_IOList;
	COPCServerList		m_OPCServerList;

	void AddContrecDevice(IContrecPtr oContrec,CIO* pIO,CDevice* pDevice);
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

	CDeviceManager(void);
	~CDeviceManager(void);
	HRESULT AddServer(COPCServer* pOPCServer);
	void RemoveServer(COPCServer* pOPCServer);
	CTag*	FindTag(LPTSTR szTag);
	void RemoveTag(CTag* pTag);
	void AddContrec(IContrecPtr oContrec);
	void PurgeDevice(LPTSTR szID);
	void AddTagToGroupItems(CTag* pTag);
	void RemoveTagFromGroupItems(CTag* pTag);
	void ModifyPort(IPortPtr oPort);
	void UpdateGroups();
};
