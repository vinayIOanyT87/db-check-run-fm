/******************************************************************************

	FILE NAME:		DeviceManager.cpp


	PURPOSE:			Implementation of the CDeviceManager


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
		06/29/2006	W.Gray		7.3.3.0 - Correction to AddDanLoad6000 for the
										number of additive totalizers

*******************************************************************************/

#include "stdafx.h"
#include "DeviceManager.h"

// CDeviceManager

CDeviceManager::CDeviceManager()
{
	m_pRoot=NULL;
	InitializeCriticalSection(&m_cs);
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




void CDeviceManager::AddDanLoad6000(IDanLoadPtr oDanLoad,CIO* pIO,CDevice* pDevice)
{
	CTag* pDanLoadTag=m_pRoot->AddBranch((LPTSTR) oDanLoad->ID,0,pIO,pDevice);

	pDanLoadTag->AddLeaf(IDS_PROMPT_PRESET_VOLUME,PROMPT_PRESET_VOLUME_CMD,0,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_REQUEST_PRESET_VOLUME,REQUEST_PRESET_VOLUME_CMD,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_PROMPT_PRESET_VOLUME,PROMPT_PRESET_VOLUME_CMD,0,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_PROMPT_RECIPE,PROMPT_RECIPE_CMD,0,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_REQUEST_SELECTED_RECIPE,REQUEST_SELECTED_RECIPE_CMD,0,0,OPC_READABLE,VT_I2,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_DISPLAY_MESSAGE,DISPLAY_MESSAGE_CMD,0,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_CHANGE_OPERATING_MODE,CHANGE_OPERATING_MODE_CMD,0,0,OPC_WRITEABLE,VT_UI1,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_SET_DATE_AND_TIME,SET_DATE_AND_TIME_CMD,0,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_RESET_PRIMARY_ALARMS,RESET_PRIMARY_ALARMS_CMD,0,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_TIMEOUT_OPERATION,TIMEOUT_OPERATION_CMD,0,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_KEYPAD_DATA,REQUEST_KEYPAD_DATA_CMD,0,0,OPC_READABLE,VT_I4,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_SET_PROGRAM_CODE_VALUE,SET_PROGRAM_CODE_VALUE_CMD,0,0,OPC_WRITEABLE | OPC_READABLE,VT_BSTR,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_READ_PROGRAM_CODE_VALUE,REQUEST_PROGRAM_CODE_VALUES_AND_ATTRIBUTES_CMD,0,0,OPC_WRITEABLE | OPC_READABLE,VT_BSTR,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_AUTHORIZE_TRANSACTION,AUTHORIZE_TRANSACTION_CMD,0,0,OPC_WRITEABLE | OPC_READABLE,VT_BSTR,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_END_TRANSACTION,END_TRANSACTION_CMD,0,0,OPC_WRITEABLE | OPC_READABLE,VT_BSTR,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_AUTHORIZE_BATCH,AUTHORIZE_BATCH_CMD,0,0,OPC_WRITEABLE | OPC_READABLE,VT_BSTR,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_CLEAR_DISPLAY,CLEAR_DISPLAY_CMD,0,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_END_BATCH,END_BATCH_CMD,0,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pDanLoadTag->AddLeaf(IDS_LAST_KEY_PRESSED,LAST_KEY_PRESSED_CMD,0,0,OPC_READABLE,VT_UI1,pIO,pDevice);

	// Status Tags
	CTag* pStatusTag=pDanLoadTag->AddBranch(IDS_STATUS,REQUEST_STATUS_CMD,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_MANUAL,REQUEST_STATUS_CMD,0,0,OPC_READABLE,VT_BOOL,pIO,pDevice);
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
	pStatusTag->AddLeaf(IDS_BATCH_AUTHORIZED,REQUEST_STATUS_CMD,2,1,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_TRANSACTION_AUTHORIZED,REQUEST_STATUS_CMD,2,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
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

	pStatusTag->AddLeaf(IDS_OLDEST_ALARM,REQUEST_STATUS_CMD,14,0,OPC_READABLE,VT_UI1,pIO,pDevice);


	CTag* pAlarmTag=pDanLoadTag->AddBranch(IDS_ALARMS,REQUEST_STATUS_CMD,pIO,pDevice);

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

	CTag* pComponentTotalizersTag=pDanLoadTag->AddBranch(IDS_COMPONENT_TOTALIZERS,COMPONENT_TOTALIZERS_CMD,pIO,pDevice);

	
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

	CTag* pAdditiveTotalizersTag=pDanLoadTag->AddBranch(IDS_ADDITIVE_TOTALIZERS,ADDITIVE_TOTALIZERS_CMD,pIO,pDevice);

	
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


	CTag* pComponentValuesTag=pDanLoadTag->AddBranch(IDS_COMPONENT_VALUES,REQUEST_COMPONENT_VALUES_CMD,pIO,pDevice);

	
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

	CTag* pInputTag=pDanLoadTag->AddBranch(IDS_INPUT,READ_INPUT_CMD,pIO,pDevice);

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

	CTag* pOutputTag=pDanLoadTag->AddBranch(IDS_OUTPUT,WRITE_OUTPUT_CMD,pIO,pDevice);
	CTag* pDiscreteOutputTag=pOutputTag->AddBranch(IDS_DISCRETE,WRITE_OUTPUT_CMD,pIO,pDevice);
	for(int iDiscrete=1;iDiscrete < 29;iDiscrete++)
	{
		CString oDiscrete;
		oDiscrete.Format(_T("%2d"),iDiscrete);
		CTag* pDiscreteTag=pDiscreteOutputTag->AddLeaf(oDiscrete,WRITE_OUTPUT_CMD,DISCRETE_OUTPUT,iDiscrete,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	}
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

		// For each Preset (DanLoad/Microload)
		IDanLoadsPtr	oDanLoads(CLSID_DanLoads);
		IDanLoadCollectionPtr	oDanLoadCollection=oDanLoads->Enumerate();
		for(LONG lItem=0;lItem < oDanLoadCollection->Count;lItem++)
		{
			IDanLoadPtr	oDanLoad=oDanLoadCollection->Item(lItem);
			AddDanLoad(oDanLoad);
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

void CDeviceManager::AddDanLoad(IDanLoadPtr oDanLoad)
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
		if(pIO->m_lIndex == oDanLoad->PortIndex)
			break;
		pIO=NULL;
	}

	if(!pIO
	&& oDanLoad->PortIndex != 0)
	{
		IPortsPtr	oPorts(CLSID_Ports);
		IPortPtr		oPort=oPorts->Get(oDanLoad->PortIndex);

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

	CDevice* pDevice=new CDevice(oDanLoad->Address);

	if(oDanLoad->Type == DANLOAD6000)
		AddDanLoad6000(oDanLoad,pIO,pDevice);

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
					if(((CDanLoadItem*) pItem)->m_oTag == oPath)
					{
						((CDanLoadItem*) pItem)->m_pTag=pTag;
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
					if(((CDanLoadItem*) pItem)->m_pTag == pTag)
					{
						if(pItem->m_bActive
						&& pTag->m_dwAccessRights & OPC_READABLE
						&& pTag->m_pIO)
							pTag->m_pIO->RemoveTagFromScanList(pTag);
						((CDanLoadItem*) pItem)->m_pTag=NULL;
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