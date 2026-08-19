/******************************************************************************

	FILE NAME:		DeviceManager.cpp


	PURPOSE:			Implementation of the CDeviceManager


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
		01/22/2007	WG				7.1.0.1 - Added FS - Force Full Screen View (CSI 4079)

		08/06/2009	W.Gray		7.4.6.0 - Move Alarm Tag to Arm Status (CSI 5208)

		08/12/2009	W.Gray		7.4.6.0 - Added support for Accuload III-SA (CSI-5640)

*******************************************************************************/

#include "stdafx.h"
#include "DeviceManager.h"

char szBX[10][3]={"B1","B2","B3","B4","B5","B6","B7","B8","B9","BA"};

char szPX[6][3]={"P1","P2","P3","P4","P5","P6"};

char szAXX[24][4]={	"A01","A02","A03","A04","A05","A06","A07","A08",
							"A09","A10","A11","A12","A13","A14","A15","A16",
							"A17","A18","A19","A20","A21","A22","A23","A24"};

char szAX[4][3]={ "A1", "A2", "A3", "A4" };

char szMX[6][3]={"M1","M2","M3","M4","M5","M6"};

char szXX[78][3]={"01","02","03","04","05","06","07","08","09","10",
						"11","12","13","14","15","16","17","18","19","20",
						"21","22","23","24","25","26","27","28","29","30",
						"31","32","33","34","35","36","37","38","39","40",
						"41","42","43","44","45","46","47","48","49","50",
						"51","52","53","54","55","56","57","58","59","60",
						"61","62","63","64","65","66","67","68","69","70",
						"71","72","73"};

char chMRBA[5] = "MRBA";
char chQ[2] = "Q";
char chR[2] = "R";
char chU[2] = "U";
char chT[2] = "T";
char chMET[4] = "MET";
char chFPOWERUP[9] = "FPOWERUP";
char chFHOSTUP[8] = "FHOSTUP";
char chFHOSTDOWN[10] = "FHOSTDOWN";
char chFCLEARPROD[11] = "FCLEARPROD";
char chR960[5] = "R960";
char chR961[5] = "R961";
char chMAM[11][7] = {"MAM000","MAM001","MAM002","MAM003","MAM004","MAM005","MAM006","MAM007","MAM008","MAM009","MAM010"};
char chMCA[11][7] = {"MCA000","MCA001","MCA002","MCA003","MCA004","MCA005","MCA006","MCA007","MCA008","MCA009","MCA010"};
char chMEM[11][7] = {"MEM000","MEM001","MEM002","MEM003","MEM004","MEM005","MEM006","MEM007","MEM008","MEM009","MEM010"};
char chMBC[11][7] = {"MBC000","MBC001","MBC002","MBC003","MBC004","MBC005","MBC006","MBC007","MBC008","MBC009","MBC010"};
char chMBE[11][7] = {"MBE000","MBE001","MBE002","MBE003","MBE004","MBE005","MBE006","MBE007","MBE008","MBE009","MBE010"};
char chMEB[11][7] = {"MEB000","MEB001","MEB002","MEB003","MEB004","MEB005","MEB006","MEB007","MEB008","MEB009","MEB010"};
char chMPM[11][7] = {"MPM000","MPM001","MPM002","MPM003","MPM004","MPM005","MPM006","MPM007","MPM008","MPM009","MPM010"};
char chMMS[11][7] = {"MMS000","MMS001","MMS002","MMS003","MMS004","MMS005","MMS006","MMS007","MMS008","MMS009","MMS010"};
char chMSM[11][7] = {"MSM000","MSM001","MSM002","MSM003","MSM004","MSM005","MSM006","MSM007","MSM008","MSM009","MSM010"};
char chMRAA[11][8] = {"MRAA000","MRAA001","MRAA002","MRAA003","MRAA004","MRAA005","MRAA006","MRAA007","MRAA008","MRAA009","MRAA010"};
char chMRMA[11][8] = {"MRMA000","MRMA001","MRMA002","MRMA003","MRMA004","MRMA005","MRMA006","MRMA007","MRMA008","MRMA009","MRMA010"};
char chMRCA[11][8] = {"MRCA000","MRCA001","MRCA002","MRCA003","MRCA004","MRCA005","MRCA006","MRCA007","MRCA008","MRCA009","MRCA010"};
char chMTPA[11][8] = {"MTPA000","MTPA001","MTPA002","MTPA003","MTPA004","MTPA005","MTPA006","MTPA007","MTPA008","MTPA009","MTPA010"};
char chMRS[11][11] = {"MRS0000001","MRS0010001","MRS0020001","MRS0030001","MRS0040001","MRS0050001","MRS0060001","MRS0070001","MRS0080001","MRS0090001","MRS0100001"};
char chMRS2[11][11] = {"MRS0000002","MRS0010002","MRS0020002","MRS0030002","MRS0040002","MRS0050002","MRS0060002","MRS0070002","MRS0080002","MRS0090002","MRS0100002"};
char chMRS1F8[11][11] = {"MRS00001F8","MRS00101F8","MRS00201F8","MRS00301F8","MRS00401F8","MRS00501F8","MRS00601F8","MRS00701F8","MRS00801F8","MRS00901F8","MRS01001F8"};
char chMRS200[11][11] = {"MRS0000200","MRS0010200","MRS0020200","MRS0030200","MRS0040200","MRS0050200","MRS0060200","MRS0070200","MRS0080200","MRS0090200","MRS0100200"};
char chMRS7C00[11][11] = {"MRS0007C00","MRS0017C00","MRS0027C00","MRS0037C00","MRS0047C00","MRS0057C00","MRS0067C00","MRS0077C00","MRS0087C00","MRS0097C00","MRS0107C00"};
char chMRS8000[11][11] = {"MRS0008000","MRS0018000","MRS0028000","MRS0038000","MRS0048000","MRS0058000","MRS0068000","MRS0078000","MRS0088000","MRS0098000","MRS0108000"};

char chMSS[11][11] = {"MSS0000001","MSS0010001","MSS0020001","MSS0030001","MSS0040001","MSS0050001","MSS0060001","MSS0070001","MSS0080001","MSS0090001","MSS0100001"};
char chMSS2[11][11] = {"MSS0000002","MSS0010002","MSS0020002","MSS0030002","MSS0040002","MSS0050002","MSS0060002","MSS0070002","MSS0080002","MSS0090002","MSS0100002"};
char chMSS1F8[11][11] = {"MSS00001F8","MSS00101F8","MSS00201F8","MSS00301F8","MSS00401F8","MSS00501F8","MSS00601F8","MSS00701F8","MSS00801F8","MSS00901F8","MSS01001F8"};
char chMSS200[11][11] = {"MSS0000200","MSS0010200","MSS0020200","MSS0030200","MSS0040200","MSS0050200","MSS0060200","MSS0070200","MSS0080200","MSS0090200","MSS0100200"};
char chMSS7C00[11][11] = {"MSS0007C00","MSS0017C00","MSS0027C00","MSS0037C00","MSS0047C00","MSS0057C00","MSS0067C00","MSS0077C00","MSS0087C00","MSS0097C00","MSS0107C00"};
char chMSS8000[11][11] = {"MSS0008000","MSS0018000","MSS0028000","MSS0038000","MSS0048000","MSS0058000","MSS0068000","MSS0078000","MSS0088000","MSS0098000","MSS0108000"};

char chR112[11][8] = {"R112000","R112001","R112002","R112003","R112004","R112005","R112006","R112007","R112008","R112009","R112010"};
char chR113[11][8] = {"R113000","R113001","R113002","R113003","R113004","R113005","R113006","R113007","R113008","R113009","R113010"};
char chR400[12][8] = {
	"R400000","R400001","R400002","R400003","R400004","R400005",
	"R400006","R400007","R400008","R400009","R400010","R400011"};
char chR500[99][8] = {
	"R500001","R500002","R500003","R500004","R500005","R500006","R500007","R500008","R500009","R500010",
	"R500011","R500012","R500013","R500014","R500015","R500016","R500017","R500018","R500019","R500020",
	"R500021","R500022","R500023","R500024","R500025","R500026","R500027","R500028","R500029","R500030",
	"R500031","R500032","R500033","R500034","R500035","R500036","R500037","R500038","R500039","R500040",
	"R500041","R500042","R500043","R500044","R500045","R500046","R500047","R500048","R500049","R500050",
	"R500051","R500052","R500053","R500054","R500055","R500056","R500057","R500058","R500059","R500060",
	"R500061","R500062","R500063","R500064","R500065","R500066","R500067","R500068","R500069","R500070",
	"R500071","R500072","R500073","R500074","R500075","R500076","R500077","R500078","R500079","R500080",
	"R500081","R500082","R500083","R500084","R500085","R500086","R500087","R500088","R500089","R500090",
	"R500091","R500092","R500093","R500094","R500095","R500096","R500097","R500098","R500099"};

char chMRST[7] = "MRS000";

char chR962[5] = "R962";

// RCU II Commands

char chQC[3] = "QC";
char chCD[3] = "CD";
char chDT[3] = "DT";
char chKI[3] = "KI";
char chSI[3] = "SI";
char chQI[3] = "QI";
char chAI[3] = "AI";
char chRD[3] = "RD";
char chPORT0[6] = "PORT0";
char chPORT1[6] = "PORT1";
char chPORT2[6] = "PORT2";
char chPORT3[6] = "PORT3";
char chPORT8[6] = "PORT8";
char chPORT10[7] = "PORT10";
char chPORT11[7] = "PORT11";
char chPORT12[7] = "PORT12";

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

void CDeviceManager::AddMultiloadSMP(IAcculoadPtr oAccuload,CIO* pIO)
{
	CDevice* pDevice=(CDevice*) new CMultiloadDevice(oAccuload->Type);

	CTag* pMultiloadTag=m_pRoot->AddBranch((LPTSTR) oAccuload->ID,pIO,pDevice);

	IArmCollectionPtr	oArms=oAccuload->Arms;
	if(!oArms->Count)
		return;

	// Some of the tags are System level and accessible through any arm
	// use the first to establish the address
	IArmPtr	oArm=oArms->Item(0L);
	BYTE	bAddress;

	bAddress=oArm->Address;

	((CMultiloadDevice*) pDevice)->m_pRcuStatusTag=pMultiloadTag->AddLeaf(IDS_RCU_STATUS,bAddress,chQ,NULL,0,OPC_READABLE,VT_UI1,pIO,pDevice);
	((CMultiloadDevice*) pDevice)->m_pCardStatusTag=pMultiloadTag->AddLeaf(IDS_CARD_STATUS,bAddress,chQ,NULL,0,OPC_READABLE,VT_UI1,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_READ_REGISTER,bAddress,chR,NULL,0,OPC_READABLE | OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_WRITE_REGISTER,bAddress,chU,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_TERMINAL_COMMAND,bAddress,chT,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_END_TRANSACTION,bAddress,chMET,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_FPOWERUP,bAddress,chFPOWERUP,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_FHOSTUP,bAddress,chFHOSTUP,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_FHOSTDOWN,bAddress,chFHOSTDOWN,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_FCLEARPROD,bAddress,chFCLEARPROD,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

	// RCU Status
	CTag* pStatusTag=pMultiloadTag->AddBranch(IDS_STATUS,pIO,pDevice);
	((CMultiloadDevice*) pDevice)->m_pStatusTag=pStatusTag;
	pStatusTag->AddLeaf(IDS_RCU_TRANS_HEADER,bAddress,chQ,NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PROVING_MODE,bAddress,chQ,NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_POWER_UP,bAddress,chQ,NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_CONFIGURED,bAddress,chQ,NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_LOG_MSG_QUEUED,bAddress,chQ,NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_HOST_UP,bAddress,chQ,NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_INPUT_IN_PROGRESS,bAddress,chQ,NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_INPUT_DONE,bAddress,chQ,NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_WEIGHTS_AND_MEASURES_KEY_ACTIVE,bAddress,chQ,NULL,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PROGRAM_KEY_ACTIVE,bAddress,chQ,NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);

	// Bay Alarms
	CTag* pBayAlarmsTag=pMultiloadTag->AddBranch(IDS_BAY_ALARMS,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_FCM_COMM,bAddress,chMRBA,NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_0,bAddress,chMRBA,NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_1,bAddress,chMRBA,NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_2,bAddress,chMRBA,NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_3,bAddress,chMRBA,NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_4,bAddress,chMRBA,NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_5,bAddress,chMRBA,NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_6,bAddress,chMRBA,NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_7,bAddress,chMRBA,NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_ALL_STOP,bAddress,chMRBA,NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PCM_COMM,bAddress,chMRBA,NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PRINTER_ERROR,bAddress,chMRBA,NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_FCM_TRACE,bAddress,chMRBA,NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);

	pMultiloadTag->AddLeaf(IDS_AUTHORIZE_PRESET,bAddress,chMAM[0],NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_CLEAR_ALARMS,bAddress,chMCA[0],NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_ENABLE_PRESET,bAddress,chMEM[0],NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_BATCH_COMPLETE,bAddress,chMBC[0],NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_BATCH_END,bAddress,chMBE[0],NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_END_BATCH,bAddress,chMEB[0],NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_PRESET_MESSAGE,bAddress,chMPM[0],NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_START,bAddress,chMMS[0],NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_STOP,bAddress,chMSM[0],NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);

	// Additive Alarms
	CTag* pAdditiveAlarmsTag=pMultiloadTag->AddBranch(IDS_ADDITIVE_ALARMS,pIO,pDevice);

	for(int iAdditive=0;iAdditive < 2;iAdditive++)
	{
		CString	oAdditive;
		oAdditive.Format(_T("%d"),iAdditive+1);

		CTag* pAdditiveTag=pAdditiveAlarmsTag->AddBranch(oAdditive,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_FCM_COMM,bAddress,chMRAA[0],NULL,iAdditive*16+12,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_PERMISSIVE,bAddress,chMRAA[0],NULL,iAdditive*16+13,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_UNDER_ADDITIZED,bAddress,chMRAA[0],NULL,iAdditive*16+14,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_OVER_ADDITIZED,bAddress,chMRAA[0],NULL,iAdditive*16+15,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_VALVE_FAULT,bAddress,chMRAA[0],NULL,iAdditive*16+8,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_PUMP_STATUS,bAddress,chMRAA[0],NULL,iAdditive*16+9,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_INJECTION,bAddress,chMRAA[0],NULL,iAdditive*16+10,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_UNAUTHORIZED_FLOW,bAddress,chMRAA[0],NULL,iAdditive*16+11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_LINE_NOT_FLUSHED,bAddress,chMRAA[0],NULL,iAdditive*16+4,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_METER_CREEP,bAddress,chMRAA[0],NULL,iAdditive*16+5,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_BLOCK_VALVE,bAddress,chMRAA[0],NULL,iAdditive*16+6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	}

	// Meter Alarms
	CTag* pMeterAlarmsTag=pMultiloadTag->AddBranch(IDS_METER_ALARMS,pIO,pDevice);

	pMeterAlarmsTag->AddLeaf(IDS_FCM_COMM,bAddress,chMRMA[0],NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_PERMISSIVE,bAddress,chMRMA[0],NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_FCM_INVALID_CONFIG,bAddress,chMRMA[0],NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_FCM_WDT_RESET,bAddress,chMRMA[0],NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_FCM_COMM_TIMEOUT,bAddress,chMRMA[0],NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_VALVE_FAULT,bAddress,chMRMA[0],NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_LOW_FLOW,bAddress,chMRMA[0],NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_EXCESS_FLOW,bAddress,chMRMA[0],NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_QUAD_ENCODING,bAddress,chMRMA[0],NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_DENSITY_ERROR,bAddress,chMRMA[0],NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_TEMP_ERROR,bAddress,chMRMA[0],NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_VALVE_CONTROL,bAddress,chMRMA[0],NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_METER_CREEP,bAddress,chMRMA[0],NULL,0,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_PRESSURE_ERROR,bAddress,chMRMA[0],NULL,1,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_QUAD_ENCODING_A,bAddress,chMRMA[0],NULL,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pMeterAlarmsTag->AddLeaf(IDS_QUAD_ENCODING_B,bAddress,chMRMA[0],NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);

	// Component Alarms
	CTag* pComponentAlarmsTag=pMultiloadTag->AddBranch(IDS_COMPONENT_ALARMS,pIO,pDevice);

	pComponentAlarmsTag->AddLeaf(IDS_FCM_COMM,bAddress,chMRCA[0],NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pComponentAlarmsTag->AddLeaf(IDS_PERMISSIVE,bAddress,chMRCA[0],NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pComponentAlarmsTag->AddLeaf(IDS_BLOCK_VALVE,bAddress,chMRCA[0],NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pComponentAlarmsTag->AddLeaf(IDS_UNAUTHORIZED_FLOW,bAddress,chMRCA[0],NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pComponentAlarmsTag->AddLeaf(IDS_PUMP_STATUS,bAddress,chMRCA[0],NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pComponentAlarmsTag->AddLeaf(IDS_METER_ALARM,bAddress,chMRCA[0],NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pComponentAlarmsTag->AddLeaf(IDS_OVER_BLEND,bAddress,chMRCA[0],NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pComponentAlarmsTag->AddLeaf(IDS_UNDER_BLEND,bAddress,chMRCA[0],NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pComponentAlarmsTag->AddLeaf(IDS_API_TABLE,bAddress,chMRCA[0],NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);

	// Preset Alarms
	CTag* pPresetAlarmsTag=pMultiloadTag->AddBranch(IDS_PRESET_ALARMS,pIO,pDevice);

	pPresetAlarmsTag->AddLeaf(IDS_FCM_COMM,bAddress,chMTPA[0],NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_PERMISSIVE,bAddress,chMTPA[0],NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_SWING_ARM_PERMISSIVE,bAddress,chMTPA[0],NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_LINE_NOT_FLUSHED,bAddress,chMTPA[0],NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_BLOCK_VALVE,bAddress,chMTPA[0],NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_BAY,bAddress,chMTPA[0],NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_COMPONENT,bAddress,chMTPA[0],NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_ADDITIVE,bAddress,chMTPA[0],NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_METER_STOP,bAddress,chMTPA[0],NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_OVERRUN,bAddress,chMTPA[0],NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_EXCESS_FLOW,bAddress,chMTPA[0],NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetAlarmsTag->AddLeaf(IDS_CONFIGURATION_ERROR,bAddress,chMTPA[0],NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);

	// Preset State
	pMultiloadTag->AddLeaf(IDS_PRESET_STATE,bAddress,chMSS[0],NULL,0,OPC_READABLE,VT_UI2,pIO,pDevice);

	// Preset Status
	CTag* pPresetStatusTag=pMultiloadTag->AddBranch(IDS_PRESET_STATUS,pIO,pDevice);

	pPresetStatusTag->AddLeaf(IDS_ENABLED,bAddress,chMSS2[0],NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_HOST_ENABLED,bAddress,chMSS2[0],NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_AUTHORIZED,bAddress,chMSS2[0],NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_FLOW_ACTIVE,bAddress,chMSS2[0],NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_BATCH_AUTHORIZED,bAddress,chMSS2[0],NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_REMOTE_MSG,bAddress,chMSS2[0],NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_REMOTE_DESC,bAddress,chMSS2[0],NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_LOAD_COMPLETE,bAddress,chMSS2[0],NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_BATCH_END,bAddress,chMSS2[0],NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_BATCH_END_DONE,bAddress,chMSS2[0],NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_END_BATCH,bAddress,chMSS2[0],NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_ARCHIVED,bAddress,chMSS2[0],NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_BATCH_CLEARED,bAddress,chMSS2[0],NULL,0,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_CLEAR_LOAD,bAddress,chMSS2[0],NULL,1,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_LOAD_CLEARED,bAddress,chMSS2[0],NULL,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pPresetStatusTag->AddLeaf(IDS_TRANS_DONE,bAddress,chMSS2[0],NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);
	// Batch Data
	CTag* pBatchTag=pMultiloadTag->AddBranch(IDS_BATCH,pIO,pDevice);

	pBatchTag->AddLeaf(IDS_PRESET_QUANTITY,bAddress,chMSS1F8[0],NULL,0,OPC_READABLE,VT_UI4,pIO,pDevice);
	pBatchTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,chMSS1F8[0],NULL,9,OPC_READABLE,VT_UI4,pIO,pDevice);
	pBatchTag->AddLeaf(IDS_NET_VOLUME,bAddress,chMSS1F8[0],NULL,18,OPC_READABLE,VT_UI4,pIO,pDevice);
	pBatchTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,chMSS1F8[0],NULL,27,OPC_READABLE,VT_I2,pIO,pDevice);
	pBatchTag->AddLeaf(IDS_AVERAGE_PRESSURE,bAddress,chMSS1F8[0],NULL,33,OPC_READABLE,VT_I2,pIO,pDevice);

	pBatchTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,chMRS1F8[0],NULL,39,OPC_READABLE,VT_UI2,pIO,pDevice);

	pMultiloadTag->AddLeaf(IDS_FLOW_RATE,bAddress,chMSS200[0],NULL,0,OPC_READABLE,VT_UI2,pIO,pDevice);

	// Batch Component Data
	CTag* pComponentTag=pBatchTag->AddBranch(IDS_COMPONENT,pIO,pDevice);
	pComponentTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,chMSS7C00[0],NULL,0,OPC_READABLE,VT_UI4,pIO,pDevice);
	pComponentTag->AddLeaf(IDS_NET_VOLUME,bAddress,chMSS7C00[0],NULL,9,OPC_READABLE,VT_UI4,pIO,pDevice);
	pComponentTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,chMSS7C00[0],NULL,18,OPC_READABLE,VT_I2,pIO,pDevice);
	pComponentTag->AddLeaf(IDS_AVERAGE_PRESSURE,bAddress,chMSS7C00[0],NULL,24,OPC_READABLE,VT_I2,pIO,pDevice);
	pComponentTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,chMSS7C00[0],NULL,30,OPC_READABLE,VT_UI2,pIO,pDevice);

	// Batch Additive Data
	CTag* pAdditivesTag=pBatchTag->AddBranch(IDS_ADDITIVE,pIO,pDevice);
	for(int iAdditive=0;iAdditive < 2;iAdditive++)
	{
		CString	oAdditive;
		oAdditive.Format(_T("%d"),iAdditive+1);

		CTag* pAdditiveTag=pAdditivesTag->AddBranch(oAdditive,pIO,pDevice);
		pAdditiveTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,chMSS8000[0],NULL,iAdditive*9+0,OPC_READABLE,VT_UI4,pIO,pDevice);
	}

	// Gross Totalizer Data
	CTag* pGrossTotalizerTag=pMultiloadTag->AddBranch(IDS_GROSS_TOTALIZER,pIO,pDevice);

	CTag* pMeterTotalizerTag=pGrossTotalizerTag->AddBranch(IDS_METER,pIO,pDevice);

	for(int iMeter=0;iMeter < 1;iMeter++)
	{
		CString	oMeter;
		oMeter.Format(_T("%d"),iMeter+1);

		pMeterTotalizerTag->AddLeaf(oMeter,bAddress,chR112[0],NULL,iMeter*9+9,OPC_READABLE,VT_UI4,pIO,pDevice);
	}

	CTag* pComponentTotalizerTag=pGrossTotalizerTag->AddBranch(IDS_COMPONENT,pIO,pDevice);

	for(int iComponent=0;iComponent < 1;iComponent++)
	{
		CString	oComponent;
		oComponent.Format(_T("%d"),iComponent+1);

		pComponentTotalizerTag->AddLeaf(oComponent,bAddress,chR112[0],NULL,iComponent*9+45,OPC_READABLE,VT_UI4,pIO,pDevice);
	}

	CTag* pAdditiveTotalizerTag=pGrossTotalizerTag->AddBranch(IDS_ADDITIVE,pIO,pDevice);

	for(int iAdditive=0;iAdditive < 2;iAdditive++)
	{
		CString	oAdditive;
		oAdditive.Format(_T("%d"),iAdditive+1);

		pAdditiveTotalizerTag->AddLeaf(oAdditive,bAddress,chR112[0],NULL,iAdditive*9+117,OPC_READABLE,VT_UI4,pIO,pDevice);
	}

	pMultiloadTag->AddLeaf(IDS_KEYPAD_DATA,bAddress,chR960,NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_TERMINATING_KEY,bAddress,chR961,NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);

	CTag* pProductConfigurationTag = pMultiloadTag->AddBranch(IDS_PRODUCT, pIO, pDevice);
	for(int iProduct = 1; iProduct < 100; iProduct++)
	{
		CString	oProduct;
		oProduct.Format(_T("%d"), iProduct);

		pProductConfigurationTag->AddLeaf(oProduct, bAddress, chR500[iProduct - 1], NULL, 0, OPC_READABLE, VT_BSTR, pIO, pDevice);
	}

	pMultiloadTag->AddLeaf(IDS_ARM_PRODUCT_CONFIGURATION, bAddress, chR400[0], NULL, 0, OPC_READABLE, VT_BSTR, pIO, pDevice);
}

void CDeviceManager::AddMultiload(IAcculoadPtr oAccuload,CIO* pIO)
{
	CDevice* pDevice=(CDevice*) new CMultiloadDevice(oAccuload->Type);

	CTag* pMultiloadTag=m_pRoot->AddBranch((LPTSTR) oAccuload->ID,pIO,pDevice);

	IArmCollectionPtr	oArms=oAccuload->Arms;
	if(!oArms->Count)
		return;

	// Some of the tags are System level and accessible through any arm
	// use the first to establish the address
	IArmPtr	oArm=oArms->Item(0L);
	BYTE	bAddress;

	bAddress=oArm->Address;


	((CMultiloadDevice*) pDevice)->m_pRcuStatusTag=pMultiloadTag->AddLeaf(IDS_RCU_STATUS,bAddress,chQ,NULL,0,OPC_READABLE,VT_UI1,pIO,pDevice);
	((CMultiloadDevice*) pDevice)->m_pCardStatusTag=pMultiloadTag->AddLeaf(IDS_CARD_STATUS,bAddress,chQ,NULL,0,OPC_READABLE,VT_UI1,pIO,pDevice);
	((CMultiloadDevice*) pDevice)->m_pCardNumberTag=pMultiloadTag->AddLeaf(IDS_CARD_NUMBER,bAddress,chR962,NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_READ_REGISTER,bAddress,chR,NULL,0,OPC_READABLE | OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_WRITE_REGISTER,bAddress,chU,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_TERMINAL_COMMAND,bAddress,chT,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_END_TRANSACTION,bAddress,chMET,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_FPOWERUP,bAddress,chFPOWERUP,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_FHOSTUP,bAddress,chFHOSTUP,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_FHOSTDOWN,bAddress,chFHOSTDOWN,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_FCLEARPROD,bAddress,chFCLEARPROD,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

	// RCU Status
	CTag* pStatusTag=pMultiloadTag->AddBranch(IDS_STATUS,pIO,pDevice);
	((CMultiloadDevice*) pDevice)->m_pStatusTag=pStatusTag;
	pStatusTag->AddLeaf(IDS_RCU_TRANS_HEADER,bAddress,chQ,NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PROVING_MODE,bAddress,chQ,NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_POWER_UP,bAddress,chQ,NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_CONFIGURED,bAddress,chQ,NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_LOG_MSG_QUEUED,bAddress,chQ,NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_HOST_UP,bAddress,chQ,NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_INPUT_IN_PROGRESS,bAddress,chQ,NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_INPUT_DONE,bAddress,chQ,NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_WEIGHTS_AND_MEASURES_KEY_ACTIVE,bAddress,chQ,NULL,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PROGRAM_KEY_ACTIVE,bAddress,chQ,NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);

	// Bay Alarms
	CTag* pBayAlarmsTag=pMultiloadTag->AddBranch(IDS_BAY_ALARMS,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_FCM_COMM,bAddress,chMRBA,NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_0,bAddress,chMRBA,NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_1,bAddress,chMRBA,NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_2,bAddress,chMRBA,NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_3,bAddress,chMRBA,NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_4,bAddress,chMRBA,NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_5,bAddress,chMRBA,NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_6,bAddress,chMRBA,NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PERMISSIVE_7,bAddress,chMRBA,NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_ALL_STOP,bAddress,chMRBA,NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PCM_COMM,bAddress,chMRBA,NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_PRINTER_ERROR,bAddress,chMRBA,NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pBayAlarmsTag->AddLeaf(IDS_FCM_TRACE,bAddress,chMRBA,NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);

	for(LONG lItem=0;lItem < oArms->Count;lItem++)
	{
		IArmPtr	oArm=oArms->Item(lItem);

		CString	oArmName;
		oArmName.Format(IDS_ARM_NUMBER,oArm->Number);

		BYTE	bAddress=oArm->Address;

		int iArm=oArm->Number-1;

		
		CTag* pArmTag=pMultiloadTag->AddBranch(oArmName,pIO,pDevice);

		pArmTag->AddLeaf(IDS_AUTHORIZE_PRESET,bAddress,chMAM[iArm],NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);

		pArmTag->AddLeaf(IDS_CLEAR_ALARMS,bAddress,chMCA[iArm],NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

		pArmTag->AddLeaf(IDS_ENABLE_PRESET,bAddress,chMEM[iArm],NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);

		pArmTag->AddLeaf(IDS_BATCH_COMPLETE,bAddress,chMBC[iArm],NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

		pArmTag->AddLeaf(IDS_BATCH_END,bAddress,chMBE[iArm],NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

		pArmTag->AddLeaf(IDS_END_BATCH,bAddress,chMEB[iArm],NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

		pArmTag->AddLeaf(IDS_PRESET_MESSAGE,bAddress,chMPM[iArm],NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);

		pArmTag->AddLeaf(IDS_START,bAddress,chMMS[iArm],NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

		pArmTag->AddLeaf(IDS_STOP,bAddress,chMSM[iArm],NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);

		pArmTag->AddLeaf(IDS_ARM_PRODUCT_CONFIGURATION, bAddress, chR400[iArm], NULL, 0, OPC_READABLE, VT_BSTR, pIO, pDevice);

		// Additive Alarms
		CTag* pAdditiveAlarmsTag=pArmTag->AddBranch(IDS_ADDITIVE_ALARMS,pIO,pDevice);

		for(int iAdditive=0;iAdditive < 2;iAdditive++)
		{
			CString	oAdditive;
			oAdditive.Format(_T("%d"),iAdditive+1);

			CTag* pAdditiveTag=pAdditiveAlarmsTag->AddBranch(oAdditive,pIO,pDevice);

			pAdditiveTag->AddLeaf(IDS_FCM_COMM,bAddress,chMRAA[iArm],NULL,iAdditive*16+12,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_PERMISSIVE,bAddress,chMRAA[iArm],NULL,iAdditive*16+13,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_UNDER_ADDITIZED,bAddress,chMRAA[iArm],NULL,iAdditive*16+14,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_OVER_ADDITIZED,bAddress,chMRAA[iArm],NULL,iAdditive*16+15,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_VALVE_FAULT,bAddress,chMRAA[iArm],NULL,iAdditive*16+8,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_PUMP_STATUS,bAddress,chMRAA[iArm],NULL,iAdditive*16+9,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_INJECTION,bAddress,chMRAA[iArm],NULL,iAdditive*16+10,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_UNAUTHORIZED_FLOW,bAddress,chMRAA[iArm],NULL,iAdditive*16+11,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_LINE_NOT_FLUSHED,bAddress,chMRAA[iArm],NULL,iAdditive*16+4,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_METER_CREEP,bAddress,chMRAA[iArm],NULL,iAdditive*16+5,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_BLOCK_VALVE,bAddress,chMRAA[iArm],NULL,iAdditive*16+6,OPC_READABLE,VT_BOOL,pIO,pDevice);
		}

		// Meter Alarms
		CTag* pMeterAlarmsTag=pArmTag->AddBranch(IDS_METER_ALARMS,pIO,pDevice);

		pMeterAlarmsTag->AddLeaf(IDS_FCM_COMM,bAddress,chMRMA[iArm],NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_PERMISSIVE,bAddress,chMRMA[iArm],NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_FCM_INVALID_CONFIG,bAddress,chMRMA[iArm],NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_FCM_WDT_RESET,bAddress,chMRMA[iArm],NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_FCM_COMM_TIMEOUT,bAddress,chMRMA[iArm],NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_VALVE_FAULT,bAddress,chMRMA[iArm],NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_LOW_FLOW,bAddress,chMRMA[iArm],NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_EXCESS_FLOW,bAddress,chMRMA[iArm],NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_QUAD_ENCODING,bAddress,chMRMA[iArm],NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_DENSITY_ERROR,bAddress,chMRMA[iArm],NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_TEMP_ERROR,bAddress,chMRMA[iArm],NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_VALVE_CONTROL,bAddress,chMRMA[iArm],NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_METER_CREEP,bAddress,chMRMA[iArm],NULL,0,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_PRESSURE_ERROR,bAddress,chMRMA[iArm],NULL,1,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_QUAD_ENCODING_A,bAddress,chMRMA[iArm],NULL,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pMeterAlarmsTag->AddLeaf(IDS_QUAD_ENCODING_B,bAddress,chMRMA[iArm],NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);

		// Component Alarms
		CTag* pComponentAlarmsTag=pArmTag->AddBranch(IDS_COMPONENT_ALARMS,pIO,pDevice);

		pComponentAlarmsTag->AddLeaf(IDS_FCM_COMM,bAddress,chMRCA[iArm],NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pComponentAlarmsTag->AddLeaf(IDS_PERMISSIVE,bAddress,chMRCA[iArm],NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pComponentAlarmsTag->AddLeaf(IDS_BLOCK_VALVE,bAddress,chMRCA[iArm],NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pComponentAlarmsTag->AddLeaf(IDS_UNAUTHORIZED_FLOW,bAddress,chMRCA[iArm],NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pComponentAlarmsTag->AddLeaf(IDS_PUMP_STATUS,bAddress,chMRCA[iArm],NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pComponentAlarmsTag->AddLeaf(IDS_METER_ALARM,bAddress,chMRCA[iArm],NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pComponentAlarmsTag->AddLeaf(IDS_OVER_BLEND,bAddress,chMRCA[iArm],NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pComponentAlarmsTag->AddLeaf(IDS_UNDER_BLEND,bAddress,chMRCA[iArm],NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pComponentAlarmsTag->AddLeaf(IDS_API_TABLE,bAddress,chMRCA[iArm],NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);

		// Preset Alarms
		CTag* pPresetAlarmsTag=pArmTag->AddBranch(IDS_PRESET_ALARMS,pIO,pDevice);

		pPresetAlarmsTag->AddLeaf(IDS_FCM_COMM,bAddress,chMTPA[iArm],NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_PERMISSIVE,bAddress,chMTPA[iArm],NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_SWING_ARM_PERMISSIVE,bAddress,chMTPA[iArm],NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_LINE_NOT_FLUSHED,bAddress,chMTPA[iArm],NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_BLOCK_VALVE,bAddress,chMTPA[iArm],NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_BAY,bAddress,chMTPA[iArm],NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_COMPONENT,bAddress,chMTPA[iArm],NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_ADDITIVE,bAddress,chMTPA[iArm],NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_METER_STOP,bAddress,chMTPA[iArm],NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_OVERRUN,bAddress,chMTPA[iArm],NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_EXCESS_FLOW,bAddress,chMTPA[iArm],NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetAlarmsTag->AddLeaf(IDS_CONFIGURATION_ERROR,bAddress,chMTPA[iArm],NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);

		// Preset State
		pArmTag->AddLeaf(IDS_PRESET_STATE,bAddress,chMRS[iArm],NULL,0,OPC_READABLE,VT_UI2,pIO,pDevice);

		// Preset Status
		CTag* pPresetStatusTag=pArmTag->AddBranch(IDS_PRESET_STATUS,pIO,pDevice);

		pPresetStatusTag->AddLeaf(IDS_ENABLED,bAddress,chMRS2[iArm],NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_HOST_ENABLED,bAddress,chMRS2[iArm],NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_AUTHORIZED,bAddress,chMRS2[iArm],NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_FLOW_ACTIVE,bAddress,chMRS2[iArm],NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_BATCH_AUTHORIZED,bAddress,chMRS2[iArm],NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_REMOTE_MSG,bAddress,chMRS2[iArm],NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_REMOTE_DESC,bAddress,chMRS2[iArm],NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_LOAD_COMPLETE,bAddress,chMRS2[iArm],NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_BATCH_END,bAddress,chMRS2[iArm],NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_BATCH_END_DONE,bAddress,chMRS2[iArm],NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_END_BATCH,bAddress,chMRS2[iArm],NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_ARCHIVED,bAddress,chMRS2[iArm],NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_BATCH_CLEARED,bAddress,chMRS2[iArm],NULL,0,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_CLEAR_LOAD,bAddress,chMRS2[iArm],NULL,1,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_LOAD_CLEARED,bAddress,chMRS2[iArm],NULL,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pPresetStatusTag->AddLeaf(IDS_TRANS_DONE,bAddress,chMRS2[iArm],NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);

		// Batch Data
		CTag* pBatchTag=pArmTag->AddBranch(IDS_BATCH,pIO,pDevice);

		pBatchTag->AddLeaf(IDS_PRESET_QUANTITY,bAddress,chMRS1F8[iArm],NULL,0,OPC_READABLE,VT_UI4,pIO,pDevice);
		pBatchTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,chMRS1F8[iArm],NULL,9,OPC_READABLE,VT_UI4,pIO,pDevice);
		pBatchTag->AddLeaf(IDS_NET_VOLUME,bAddress,chMRS1F8[iArm],NULL,18,OPC_READABLE,VT_UI4,pIO,pDevice);
		pBatchTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,chMRS1F8[iArm],NULL,27,OPC_READABLE,VT_I2,pIO,pDevice);
		pBatchTag->AddLeaf(IDS_AVERAGE_PRESSURE,bAddress,chMRS1F8[iArm],NULL,33,OPC_READABLE,VT_I2,pIO,pDevice);

		pBatchTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,chMRS1F8[iArm],NULL,39,OPC_READABLE,VT_UI2,pIO,pDevice);

		pArmTag->AddLeaf(IDS_FLOW_RATE,bAddress,chMRS200[iArm],NULL,0,OPC_READABLE,VT_UI2,pIO,pDevice);

		// Batch Component Data
		CTag* pComponentGrossTag=pBatchTag->AddBranch(IDS_GROSS_VOLUME,pIO,pDevice);
		for(int iComponent=0;iComponent < 8;iComponent++)
		{
			CString	oComponent;
			oComponent.Format(_T("%d"),iComponent+1);

			pComponentGrossTag->AddLeaf(oComponent,bAddress,chMRS7C00[iArm],NULL,iComponent*36+0,OPC_READABLE,VT_UI4,pIO,pDevice);
		}
		
		CTag* pComponentNetTag=pBatchTag->AddBranch(IDS_NET_VOLUME,pIO,pDevice);
		for(int iComponent=0;iComponent < 8;iComponent++)
		{
			CString	oComponent;
			oComponent.Format(_T("%d"),iComponent+1);

			pComponentNetTag->AddLeaf(oComponent,bAddress,chMRS7C00[iArm],NULL,iComponent*36+9,OPC_READABLE,VT_UI4,pIO,pDevice);
		}
		
		CTag* pComponentAverTag=pBatchTag->AddBranch(IDS_AVERAGE_TEMPERATURE,pIO,pDevice);
		for(int iComponent=0;iComponent < 8;iComponent++)
		{
			CString	oComponent;
			oComponent.Format(_T("%d"),iComponent+1);

			pComponentAverTag->AddLeaf(oComponent,bAddress,chMRS7C00[iArm],NULL,iComponent*36+18,OPC_READABLE,VT_I2,pIO,pDevice);
		}
		
		CTag* pComponentPressTag=pBatchTag->AddBranch(IDS_AVERAGE_PRESSURE,pIO,pDevice);
		for(int iComponent=0;iComponent < 8;iComponent++)
		{
			CString	oComponent;
			oComponent.Format(_T("%d"),iComponent+1);

			pComponentPressTag->AddLeaf(oComponent,bAddress,chMRS7C00[iArm],NULL,iComponent*36+24,OPC_READABLE,VT_I2,pIO,pDevice);
		}
		
		CTag* pComponentDensityTag=pBatchTag->AddBranch(IDS_AVERAGE_DENSITY,pIO,pDevice);
		for(int iComponent=0;iComponent < 8;iComponent++)
		{
			CString	oComponent;
			oComponent.Format(_T("%d"),iComponent+1);

			pComponentDensityTag->AddLeaf(oComponent,bAddress,chMRS7C00[iArm],NULL,iComponent*36+30,OPC_READABLE,VT_UI2,pIO,pDevice);
		}
		
		// Batch Additive Data
		CTag* pAdditivesTag=pBatchTag->AddBranch(IDS_ADDITIVE,pIO,pDevice);
		for(int iAdditive=0;iAdditive < 16;iAdditive++)
		{
			CString	oAdditive;
			oAdditive.Format(_T("%d"),iAdditive+1);

			CTag* pAdditiveTag=pAdditivesTag->AddBranch(oAdditive,pIO,pDevice);
			pAdditiveTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,chMRS8000[iArm],NULL,iAdditive*9+0,OPC_READABLE,VT_UI4,pIO,pDevice);
		}

		// Gross Totalizer Data
		CTag* pGrossTotalizerTag=pArmTag->AddBranch(IDS_GROSS_TOTALIZER,pIO,pDevice);

		CTag* pMeterTotalizerTag=pGrossTotalizerTag->AddBranch(IDS_METER,pIO,pDevice);

		for(int iMeter=0;iMeter < 4;iMeter++)
		{
			CString	oMeter;
			oMeter.Format(_T("%d"),iMeter+1);

			pMeterTotalizerTag->AddLeaf(oMeter,bAddress,chR112[iArm],NULL,iMeter*9+9,OPC_READABLE,VT_UI4,pIO,pDevice);
		}

		CTag* pComponentTotalizerTag=pGrossTotalizerTag->AddBranch(IDS_COMPONENT,pIO,pDevice);

		for(int iComponent=0;iComponent < 8;iComponent++)
		{
			CString	oComponent;
			oComponent.Format(_T("%d"),iComponent+1);

			pComponentTotalizerTag->AddLeaf(oComponent,bAddress,chR112[iArm],NULL,iComponent*9+45,OPC_READABLE,VT_UI4,pIO,pDevice);
		}

		CTag* pAdditiveTotalizerTag=pGrossTotalizerTag->AddBranch(IDS_ADDITIVE,pIO,pDevice);

		for(int iAdditive=0;iAdditive < 16;iAdditive++)
		{
			CString	oAdditive;
			oAdditive.Format(_T("%d"),iAdditive+1);

			pAdditiveTotalizerTag->AddLeaf(oAdditive,bAddress,chR112[iArm],NULL,iAdditive*9+117,OPC_READABLE,VT_UI4,pIO,pDevice);
		}
	}
	pMultiloadTag->AddLeaf(IDS_KEYPAD_DATA,bAddress,chR960,NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pMultiloadTag->AddLeaf(IDS_TERMINATING_KEY,bAddress,chR961,NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);

	CTag* pProductConfigurationTag = pMultiloadTag->AddBranch(IDS_PRODUCT, pIO, pDevice);
	for(int iProduct = 1; iProduct < 100; iProduct++)
	{
		CString	oProduct;
		oProduct.Format(_T("%d"), iProduct);

		pProductConfigurationTag->AddLeaf(oProduct, bAddress, chR500[iProduct - 1], NULL, 0, OPC_READABLE, VT_BSTR, pIO, pDevice);
	}
}

void CDeviceManager::AddMicroloadNet(IAcculoadPtr oAccuload,CIO* pIO)
{
	CDevice* pDevice=(CDevice*) new CAcculoadDevice(oAccuload->Type);

	CTag* pMicroloadTag=m_pRoot->AddBranch((LPTSTR) oAccuload->ID,pIO,pDevice);


	IArmCollectionPtr	oArms=oAccuload->Arms;
	if(!oArms->Count)
		return;

	// Some of the tags are System level and accessible through any arm
	// use the first to establish the address
	IArmPtr	oArm=oArms->Item(0L);
	BYTE	bAddress;

	// For Network Communications the Address is the lowest Octet of IP Address
	if(oAccuload->NetworkCommunications)
	{
		int iA,iB,iC,iD;
		if(4 != swscanf(oAccuload->IPAddress,_T("%d.%d.%d.%d"),&iA,&iB,&iC,&iD))
		{
			CString oError;
			oError.Format(_T("Device Manager : AddMicroloadNET bad IP Address for %s"),oAccuload->ID);
			theApp.LogError(oError);
			return;
		}

		bAddress=(BYTE) iD;
	}
	else
		bAddress=oArm->Address;

	// NOTE: Microload does not support CT - Clear Transactions

	pMicroloadTag->AddLeaf(IDS_STOP_ALL_ARMS,bAddress,"SP",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMicroloadTag->AddLeaf(IDS_STOP,bAddress,"SP",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pMicroloadTag->AddLeaf(IDS_SET_DATE_AND_TIME,bAddress,"SD",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pMicroloadTag->AddLeaf(IDS_START,bAddress,"SA",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

	// The ordering of the tags is important here. Logic in IO.cpp depends upon it
	CTag* pCardReaderTag=pMicroloadTag->AddBranch(IDS_CARD_READER,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_DATA,bAddress,"CD",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_I_O_CONTROL,bAddress,"CD",NULL,3,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_GREEN_LED,bAddress,"CD",NULL,4,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_RED_LED,bAddress,"CD",NULL,5,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_BEEP,bAddress,"CD",NULL,6,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_CONTACT,bAddress,"CD",NULL,7,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_RESET_DATA,bAddress,"RE","CD",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

	// Digital Outputs
	CTag* pDigitalOutputsTag=pMicroloadTag->AddBranch(IDS_DIGITAL_OUTPUTS,pIO,pDevice);
	for(int i=0;i < 6;i++)
	{
		CString strOutput;
		strOutput.Format(_T("%02d"),i+1);
		pDigitalOutputsTag->AddLeaf(strOutput,bAddress,"OR",szXX[i],0,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	}

	// Tags associated with Status ("EQ" command)
	CTag* pDigitalInputsTag=pMicroloadTag->AddBranch(IDS_DIGITAL_INPUTS,pIO,pDevice);

	pDigitalInputsTag->AddLeaf(IDS_INPUT_03,bAddress,"EQ",NULL,16,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_02,bAddress,"EQ",NULL,17,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_01,bAddress,"EQ",NULL,18,OPC_READABLE,VT_BOOL,pIO,pDevice);

	CTag* pStatusTag=pMicroloadTag->AddBranch(IDS_STATUS,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PROGRAM_MODE,bAddress,"EQ",NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);

	pStatusTag->AddLeaf(IDS_POWER_FAIL_OCCURED,bAddress,"EQ",NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_DELAYED_PROMPT_IN_EFFECT,bAddress,"EQ",NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PROGRAM_VALUE_CHANGED,bAddress,"EQ",NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_NEW_CARD_DATA,bAddress,"EQ",NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PRINTING_IN_PROGRESS,bAddress,"EQ",NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_RESET_POWERFAIL,bAddress,"RE","PF",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_GET_BATCH_NUMBER,bAddress,"RB",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);

	// Tags assocated with Alarms ("EA" command)
	CTag* pAlarmsTag=pMicroloadTag->AddBranch(IDS_ALARMS,pIO,pDevice);
	pAlarmsTag->AddLeaf(IDS_RESET_POWER_FAIL_ALARM,bAddress,"AR","PA",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pAlarmsTag->AddLeaf(IDS_RESET_COMM_FAIL_ALARM,bAddress,"AR","CM",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

	CTag* pSystemTag=pAlarmsTag->AddBranch(IDS_SYSTEM,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ROM_BAD,bAddress,"EA","SY",0,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_RAM_BAD,bAddress,"EA","SY",1,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_FLASH_ERROR,bAddress,"EA","SY",2,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_RAM_CORRUPT,bAddress,"EA","SY",3,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_FLASH_BACKUP_BAD,bAddress,"EA","SY",4,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_WATCHDOG,bAddress,"EA","SY",5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_SYS_PROG_ERROR,bAddress,"EA","SY",6,OPC_READABLE,VT_BOOL,pIO,pDevice);

	pSystemTag->AddLeaf(IDS_PASSCODE_RESET,bAddress,"EA","SY",7,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_POWERFAIL,bAddress,"EA","SY",8,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_01,bAddress,"EA","SY",9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_02,bAddress,"EA","SY",10,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_03,bAddress,"EA","SY",11,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_04,bAddress,"EA","SY",12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_05,bAddress,"EA","SY",13,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_COMMUNICATION,bAddress,"EA","SY",14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_TICKET,bAddress,"EA","SY",15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ZERO_FLOW,bAddress,"EA","SY",16,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_OVERRUN,bAddress,"EA","SY",17,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ADDITIVE_CLEAN_LINE,bAddress,"EA","SY",18,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_PULSE_SECURITY,bAddress,"EA","SY",19,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_VALVE_FAULT,bAddress,"EA","SY",20,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_BACK_PRESSURE,bAddress,"EA","SY",21,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_TEMPERATURE_PROBE,bAddress,"EA","SY",22,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_DENSITY_TRANSMITTER,bAddress,"EA","SY",23,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_PRESSURE_TRANSMITTER,bAddress,"EA","SY",24,OPC_READABLE,VT_BOOL,pIO,pDevice);

	pSystemTag->AddLeaf(IDS_HIGH_FLOW,bAddress,"EA","SY",25,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_HIGH_TEMPERATURE,bAddress,"EA","SY",26,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_HIGH_DENSITY,bAddress,"EA","SY",27,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_HIGH_PRESSURE,bAddress,"EA","SY",28,OPC_READABLE,VT_BOOL,pIO,pDevice);

	pSystemTag->AddLeaf(IDS_LOW_FLOW,bAddress,"EA","SY",29,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_LOW_TEMPERATURE,bAddress,"EA","SY",30,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_LOW_DENSITY,bAddress,"EA","SY",31,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_LOW_PRESSURE,bAddress,"EA","SY",32,OPC_READABLE,VT_BOOL,pIO,pDevice);

	pSystemTag->AddLeaf(IDS_MASS_METER_COMM_FAIL,bAddress,"EA","SY",33,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_MASS_METER_OVERDRIVE,bAddress,"EA","SY",34,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_MASS_METER_TUBE,bAddress,"EA","SY",35,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_PTB_PRINTER,bAddress,"EA","SY",36,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_SHARED_PRINTER,bAddress,"EA","SY",37,OPC_READABLE,VT_BOOL,pIO,pDevice);

	// MicroLoads have 4 injectors
	for(LONG lInjector=0;lInjector < 4;lInjector++)
	{
		CString	oInjector;
		oInjector.Format(IDS_INJECTOR_NUMBER,lInjector+1);
		CTag*	pInjectorTag=pAlarmsTag->AddBranch(oInjector,pIO,pDevice);

		pInjectorTag->AddLeaf(IDS_COMM_ERROR,bAddress,"EA","IN",40+lInjector,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_COMMAND_REFUSED,bAddress,"EA","IN",44+lInjector,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_FEEDBACK,bAddress,"EA","IN",48+lInjector,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_GENERAL_ADDITIVE_ERROR,bAddress,"EA","IN",52+lInjector,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_LOW_ADD_ERROR,bAddress,"EA","IN",56+lInjector,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_EXCESS_PULSES,bAddress,"EA","IN",60+lInjector,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_NO_PULSES,bAddress,"EA","IN",64+lInjector,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_OVER_SPEED,bAddress,"EA","IN",68+lInjector,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_FREQUENCY,bAddress,"EA","IN",72+lInjector,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_UNAUTH_FAILED,bAddress,"EA","IN",76+lInjector,OPC_READABLE,VT_BOOL,pIO,pDevice);
	}

	// Tags associated with Load Arms
	// MicroLoads only have one arm
	for(LONG lItem=0;lItem < 1;lItem++)
	{
		IArmPtr	oArm=oArms->Item(lItem);

		CString	oArmName;
		oArmName.Format(IDS_ARM_NUMBER,oArm->Number);

		int iArm=oArm->Number-1;

		CTag* pArmTag=pMicroloadTag->AddBranch(oArmName,pIO,pDevice);

		CTag*	pPromptsTag=pArmTag->AddBranch(IDS_PROMPTS,pIO,pDevice);
		pPromptsTag->AddLeaf(_T("1"),bAddress,"TI","1",0,OPC_READABLE,VT_I4,pIO,pDevice);
		pPromptsTag->AddLeaf(_T("2"),bAddress,"TI","2",0,OPC_READABLE,VT_I4,pIO,pDevice);
		pPromptsTag->AddLeaf(_T("3"),bAddress,"TI","3",0,OPC_READABLE,VT_I4,pIO,pDevice);
		pPromptsTag->AddLeaf(_T("4"),bAddress,"TI","4",0,OPC_READABLE,VT_I4,pIO,pDevice);
		pPromptsTag->AddLeaf(_T("5"),bAddress,"TI","5",0,OPC_READABLE,VT_I4,pIO,pDevice);


		CTag* pStatusTag=pArmTag->AddBranch(IDS_STATUS,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_AUTHORIZED,bAddress,"EQ",NULL,0,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_FLOWING,bAddress,"EQ",NULL,1,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_RELEASED,bAddress,"EQ",NULL,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_KEYPAD_DATA_PENDING,bAddress,"EQ",NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_BATCH_DONE,bAddress,"EQ",NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_TRANSACTION_DONE,bAddress,"EQ",NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_TRANSACTION_IN_PROGRESS,bAddress,"EQ",NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_ALARM,bAddress,"EQ",NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_DISPLAY_MESSAGE_TIME_OUT,bAddress,"EQ",NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_CHECKING_ENTRIES,bAddress,"EQ",NULL,19,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_PERMISSIVE_DELAY,bAddress,"EQ",NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_RESET_TRANSACTION_DONE,bAddress,"RE","TD",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_RESET_BATCH_DONE,bAddress,"RE","BD",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

		CTag* pDynamicValuesTag=pArmTag->AddBranch(IDS_DYNAMIC_VALUES,pIO,pDevice);

		CTag* pNonResettableTotals=pArmTag->AddBranch(IDS_NON_RESETTABLE_TOTALS,pIO,pDevice);

		CTag* pSystemTag=pDynamicValuesTag->AddBranch(IDS_SYSTEM,pIO,pDevice);

		pSystemTag->AddLeaf(IDS_CURRENT_FLOW_RATE_UNITS_MIN,bAddress,"DY","SY",0,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_FLOW_RATE_UNITS_HR,bAddress,"DY","SY",1,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_RECIPE,bAddress,"DY","SY",2,OPC_READABLE,VT_I4,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_PRESET_UNITS_MIN,bAddress,"DY","SY",3,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_DELIVERED_VOLUME,bAddress,"DY","SY",4,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_REMAINING_VOLUME,bAddress,"DY","SY",5,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_METER_FACTOR,bAddress,"DY","SY",6,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_TEMPERATURE,bAddress,"DY","SY",7,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_DENSITY,bAddress,"DY","SY",8,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_PRESSURE,bAddress,"DY","SY",9,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_VAPOR_PRESSURE,bAddress,"DY","SY",10,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_VALVE_REQUESTED_POSITION,bAddress,"DY","SY",11,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_TIME_OF_LAST_POWER_FAIL,bAddress,"DY","SY",12,OPC_READABLE,VT_BSTR,pIO,pDevice);

		CString oName;
		oName.Format(IDS_PRODUCT);

		CTag* pProductTag=pNonResettableTotals->AddBranch(oName,pIO,pDevice);
		pProductTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"VT","",0,OPC_READABLE,VT_R8,pIO,pDevice);
		pProductTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"VT","",1,OPC_READABLE,VT_R8,pIO,pDevice);
		pProductTag->AddLeaf(IDS_GST_VOLUME,bAddress,"VT","",2,OPC_READABLE,VT_R8,pIO,pDevice);
		pProductTag->AddLeaf(IDS_GSV_VOLUME,bAddress,"VT","",3,OPC_READABLE,VT_R8,pIO,pDevice);
		pProductTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"VT","",4,OPC_READABLE,VT_R8,pIO,pDevice);
			
		CTag* pCurrentBatchTag=pDynamicValuesTag->AddBranch(IDS_CURRENT_BATCH,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_RECIPE,bAddress,"DY","CB",0,OPC_READABLE,VT_I4,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"DY","CB",1,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"DY","CB",2,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_GST_VOLUME,bAddress,"DY","CB",3,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_GSV_VOLUME,bAddress,"DY","CB",4,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"DY","CB",5,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_METER_FACTOR,bAddress,"DY","CB",6,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,"DY","CB",7,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,"DY","CB",8,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_REFERENCE_DENSITY,bAddress,"DY","CB",10,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_PRESSURE,bAddress,"DY","CB",12,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_CTL,bAddress,"DY","CB",14,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_CPL,bAddress,"DY","CB",15,OPC_READABLE,VT_R8,pIO,pDevice);

		CTag* pAdditivesTag=pCurrentBatchTag->AddBranch(IDS_ADDITIVES,pIO,pDevice);
		for(LONG lInjector=0;lInjector < 4;lInjector++)
		{
			CString	oInjector;
			oInjector.Format(_T("%02d"),lInjector+1);
			CTag* pInjectorTag=pAdditivesTag->AddBranch(oInjector,pIO,pDevice);
			pInjectorTag->AddLeaf(IDS_VOLUME,bAddress,"DY","CB",16+lInjector,OPC_READABLE,VT_R8,pIO,pDevice);
		}


		CTag* pBatchTag=pDynamicValuesTag->AddBranch(IDS_BATCH,pIO,pDevice);
		for(long lBatch=0;lBatch < 10;lBatch++)
		{
			CString	oBatch;
			oBatch.Format(_T("%02d"),lBatch+1);

			CTag* pSpecificBatchTag=pBatchTag->AddBranch(oBatch,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_RECIPE,bAddress,"DY",szBX[lBatch],0,OPC_READABLE,VT_I4,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"DY",szBX[lBatch],1,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"DY",szBX[lBatch],2,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_GST_VOLUME,bAddress,"DY",szBX[lBatch],3,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_GSV_VOLUME,bAddress,"DY",szBX[lBatch],4,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"DY",szBX[lBatch],5,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_METER_FACTOR,bAddress,"DY",szBX[lBatch],6,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,"DY",szBX[lBatch],7,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,"DY",szBX[lBatch],8,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_REFERENCE_DENSITY,bAddress,"DY",szBX[lBatch],10,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_PRESSURE,bAddress,"DY",szBX[lBatch],12,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_CTL,bAddress,"DY",szBX[lBatch],14,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_CPL,bAddress,"DY",szBX[lBatch],15,OPC_READABLE,VT_R8,pIO,pDevice);

			CTag* pAdditivesTag=pSpecificBatchTag->AddBranch(IDS_ADDITIVES,pIO,pDevice);
			for(LONG lInjector=0;lInjector < 4;lInjector++)
			{
				CString	oInjector;
				oInjector.Format(_T("%02d"),lInjector+1);
				CTag* pInjectorTag=pAdditivesTag->AddBranch(oInjector,pIO,pDevice);
				pInjectorTag->AddLeaf(IDS_VOLUME,bAddress,"DY",szBX[lBatch],16+lInjector,OPC_READABLE,VT_R8,pIO,pDevice);
			}
		}

		CTag* pTransactionTag=pDynamicValuesTag->AddBranch(IDS_TRANSACTION,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"DY","TR",1,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"DY","TR",2,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_GST_VOLUME,bAddress,"DY","TR",3,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_GSV_VOLUME,bAddress,"DY","TR",4,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"DY","TR",5,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_METER_FACTOR,bAddress,"DY","TR",6,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,"DY","TR",7,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,"DY","TR",8,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_PRESSURE,bAddress,"DY","TR",9,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_CTL,bAddress,"DY","TR",10,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_CPL,bAddress,"DY","TR",11,OPC_READABLE,VT_R8,pIO,pDevice);

		pAdditivesTag=pTransactionTag->AddBranch(IDS_ADDITIVES,pIO,pDevice);
		for(LONG lInjector=0;lInjector < 4;lInjector++)
		{
			CString	oInjector;
			oInjector.Format(_T("%02d"),lInjector+1);
			CTag* pInjectorTag=pAdditivesTag->AddBranch(oInjector,pIO,pDevice);
			pInjectorTag->AddLeaf(IDS_VOLUME,bAddress,"DY","TR",12+lInjector,OPC_READABLE,VT_R8,pIO,pDevice);
		}


		// Swing arm position not supported by Microload
		//pArmTag->AddLeaf(IDS_SWING_ARM_POSITION,bAddress,"SW",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_ALLOCATE_RECIPES,bAddress,"AB",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_AUTHORIZE_TRANSACTION,bAddress,"AP",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_END_BATCH,bAddress,"EB",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pArmTag->AddLeaf(IDS_END_TRANSACTION,bAddress,"ET",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

		// NOTE: Prompt commands for Microload are different than AccuLoad commands.  Not sure why
		// they departed from the AccuLoad commands.  Also, there are seven lines on a MicroLoad.
		pArmTag->AddLeaf(IDS_WRITE_FIRST_LINE_WITH_PROMPT,bAddress,"WA",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_SECOND_LINE,bAddress,"WB",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_THIRD_LINE,bAddress,"WC",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_FOURTH_LINE,bAddress,"WD",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_FIFTH_LINE,bAddress,"WE",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_SIXTH_LINE,bAddress,"WF",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_SEVENTH_LINE,bAddress,"WG",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);

		pArmTag->AddLeaf(IDS_WRITE_FIRST_LINE_WITH_PROMPT_ON_SET,bAddress,"WP",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_FIRST_LINE_WITH_PROMPT_ON_SET_NO_ECHO,bAddress,"WQ",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_FIRST_LINE_WITH_PROMPT_NO_ECHO,bAddress,"WX",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_KEYPAD_DATA,bAddress,"RK",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_RELEASE_KEYPAD_AND_DISPLAY,bAddress,"DA",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pArmTag->AddLeaf(IDS_PRESET_AMOUNT,bAddress,"RP",NULL,0,OPC_READABLE,VT_R8,pIO,pDevice);
		pArmTag->AddLeaf(IDS_AUTHORIZE_AND_SET_BATCH_AMOUNT,bAddress,"SB",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_LOG_OUT_OF_PROGRAM_MODE,bAddress,"LO",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pArmTag->AddLeaf(IDS_PROGRAM_CODE_CHANGE,bAddress,"PC",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_GET_KEY,bAddress,"GK",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_RECIPE,bAddress,"RR",NULL,0,OPC_READABLE,VT_UI2,pIO,pDevice);

		pAdditivesTag=pNonResettableTotals->AddBranch(IDS_ADDITIVES,pIO,pDevice);
		for(LONG lInjector=0;lInjector < 4;lInjector++)
		{
			CString	oInjector;
			oInjector.Format(_T("%02d"),lInjector+1);
			CTag* pInjectorTag=pAdditivesTag->AddBranch(oInjector,pIO,pDevice);
			pInjectorTag->AddLeaf(IDS_VOLUME,bAddress,"VT",szAX[lInjector],0,OPC_READABLE,VT_R8,pIO,pDevice);
		}

	}

	// Tags associatd with Recipes
	CTag* pRecipesTag=pMicroloadTag->AddBranch(IDS_RECIPES,pIO,pDevice);
	for(int iRecipe=0;iRecipe < 12;iRecipe++)
	{
		CString strRecipe;
		strRecipe.Format(_T("%02d"),iRecipe+1);
		CTag* pRecipeTag=pRecipesTag->AddBranch(strRecipe,pIO,pDevice);

		pRecipeTag->AddLeaf(IDS_USED,bAddress,"PV",szXX[iRecipe],1,OPC_READABLE,VT_UI1,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_NAME,bAddress,"PV",szXX[iRecipe],2,OPC_READABLE,VT_BSTR,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_HM_CLASS_PRODUCT,bAddress,"PV",szXX[iRecipe],3,OPC_READABLE,VT_UI1,pIO,pDevice);
		
		for(int iInjector=0;iInjector < 4;iInjector++)
		{
			CString strTag;
			strTag.Format(IDS_ADDITIVE_INJECTOR_AMOUNT_PER_CYCLE,iInjector+1);
			pRecipeTag->AddLeaf(strTag,bAddress,"PV",szXX[iRecipe],11+iInjector*2,OPC_READABLE,VT_R8,pIO,pDevice);
			strTag.Format(IDS_ADDITIVE_INJECTOR_RATE,iInjector+1);
			pRecipeTag->AddLeaf(strTag,bAddress,"PV",szXX[iRecipe],12+iInjector*2,OPC_READABLE,VT_R8,pIO,pDevice);
		}
	}
}


void CDeviceManager::AddAccuload3(IAcculoadPtr oAccuload,CIO* pIO)
{
	CDevice* pDevice=(CDevice*) new CAcculoadDevice(oAccuload->Type);

	CTag* pAcculoadTag=m_pRoot->AddBranch((LPTSTR) oAccuload->ID,pIO,pDevice);

	IArmCollectionPtr	oArms=oAccuload->Arms;
	if(!oArms->Count)
		return;

	// Some of the tags are System level and accessible through any arm
	// use the first to establish the address
	IArmPtr	oArm=oArms->Item(0L);
	BYTE	bAddress=oArm->Address;

	pAcculoadTag->AddLeaf(IDS_STOP_ALL_ARMS,bAddress,"SP",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pAcculoadTag->AddLeaf(IDS_CLEAR_TRANSACTIONS,bAddress,"CT",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pAcculoadTag->AddLeaf(IDS_SET_DATE_AND_TIME,bAddress,"SD",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pAcculoadTag->AddLeaf(IDS_TRANSACTION_TERMINATION,bAddress,"PV","SY",315,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pAcculoadTag->AddLeaf(IDS_INCLUDE_ADDITIVE_TOTALS,bAddress,"PV","SY",93,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pAcculoadTag->AddLeaf(IDS_NUMBER_OF_ARMS, bAddress, "PV", "CF", 1, OPC_READABLE, VT_INT, pIO, pDevice);

	// The ordering of the tags is important here. Logic in IO.cpp depends upon it
	CTag* pCardReaderTag=pAcculoadTag->AddBranch(IDS_CARD_READER,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_DATA,bAddress,"CD",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	if(oAccuload->Type != ACCULOAD_III_SA)
	{
		pCardReaderTag->AddLeaf(IDS_I_O_CONTROL,bAddress,"CD",NULL,3,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
		pCardReaderTag->AddLeaf(IDS_GREEN_LED,bAddress,"CD",NULL,4,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
		pCardReaderTag->AddLeaf(IDS_RED_LED,bAddress,"CD",NULL,5,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
		pCardReaderTag->AddLeaf(IDS_BEEP,bAddress,"CD",NULL,6,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
		pCardReaderTag->AddLeaf(IDS_CONTACT,bAddress,"CD",NULL,7,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	}
	pCardReaderTag->AddLeaf(IDS_RESET_DATA,bAddress,"RE","CD",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

	CTag* pDigitalOutputsTag=pAcculoadTag->AddBranch(IDS_DIGITAL_OUTPUTS,pIO,pDevice);
	for(int i=0;i < 78;i++)
	{
		CString strOutput;
		strOutput.Format(_T("%02d"),i+1);
		pDigitalOutputsTag->AddLeaf(strOutput,bAddress,"OR",szXX[i],0,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	}


	// Tags associated with Status ("EQ" command)
	CTag* pDigitalInputsTag=pAcculoadTag->AddBranch(IDS_DIGITAL_INPUTS,pIO,pDevice);

	pDigitalInputsTag->AddLeaf(IDS_INPUT_03,bAddress,"EQ",NULL,16,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_02,bAddress,"EQ",NULL,17,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_01,bAddress,"EQ",NULL,18,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_07,bAddress,"EQ",NULL,20,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_06,bAddress,"EQ",NULL,21,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_05,bAddress,"EQ",NULL,22,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_04,bAddress,"EQ",NULL,23,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_11,bAddress,"EQ",NULL,24,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_10,bAddress,"EQ",NULL,25,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_09,bAddress,"EQ",NULL,26,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_08,bAddress,"EQ",NULL,27,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_15,bAddress,"EQ",NULL,28,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_14,bAddress,"EQ",NULL,29,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_13,bAddress,"EQ",NULL,30,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_12,bAddress,"EQ",NULL,31,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_19,bAddress,"EQ",NULL,32,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_18,bAddress,"EQ",NULL,33,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_17,bAddress,"EQ",NULL,34,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_16,bAddress,"EQ",NULL,35,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_23,bAddress,"EQ",NULL,36,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_22,bAddress,"EQ",NULL,37,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_21,bAddress,"EQ",NULL,38,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_20,bAddress,"EQ",NULL,39,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_27,bAddress,"EQ",NULL,40,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_26,bAddress,"EQ",NULL,41,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_25,bAddress,"EQ",NULL,42,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_24,bAddress,"EQ",NULL,43,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_31,bAddress,"EQ",NULL,44,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_30,bAddress,"EQ",NULL,45,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_29,bAddress,"EQ",NULL,46,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_28,bAddress,"EQ",NULL,47,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_35,bAddress,"EQ",NULL,48,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_34,bAddress,"EQ",NULL,49,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_33,bAddress,"EQ",NULL,50,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_32,bAddress,"EQ",NULL,51,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_39,bAddress,"EQ",NULL,52,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_38,bAddress,"EQ",NULL,53,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_37,bAddress,"EQ",NULL,54,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_36,bAddress,"EQ",NULL,55,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_43,bAddress,"EQ",NULL,56,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_42,bAddress,"EQ",NULL,57,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_41,bAddress,"EQ",NULL,58,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pDigitalInputsTag->AddLeaf(IDS_INPUT_40,bAddress,"EQ",NULL,59,OPC_READABLE,VT_BOOL,pIO,pDevice);

	CTag* pStatusTag=pAcculoadTag->AddBranch(IDS_STATUS,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PROGRAM_MODE,bAddress,"EQ",NULL,3,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_IN_STANDBY_MODE,bAddress,"EQ",NULL,8,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_STORAGE_FULL,bAddress,"EQ",NULL,9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_STANDBY_TRANSACTIONS_EXIST,bAddress,"EQ",NULL,10,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_POWER_FAIL_OCCURED,bAddress,"EQ",NULL,12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_DELAYED_PROMPT_IN_EFFECT,bAddress,"EQ",NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PROGRAM_VALUE_CHANGED,bAddress,"EQ",NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_NEW_CARD_DATA,bAddress,"EQ",NULL,61,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_PRINTING_IN_PROGRESS,bAddress,"EQ",NULL,63,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_RESET_STANDBY_MODE,bAddress,"RE","SA",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_RESET_POWERFAIL,bAddress,"RE","PF",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

	// Tags assocated with Alarms ("EA" command)
	CTag* pAlarmsTag=pAcculoadTag->AddBranch(IDS_ALARMS,pIO,pDevice);
	pAlarmsTag->AddLeaf(IDS_RESET_POWER_FAIL_ALARM,bAddress,"AR","PA",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pAlarmsTag->AddLeaf(IDS_RESET_COMM_FAIL_ALARM,bAddress,"AR","CM",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);

	CTag* pSystemTag=pAlarmsTag->AddBranch(IDS_SYSTEM,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ROM_BAD,bAddress,"EA","SY",0,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_RAM_BAD,bAddress,"EA","SY",1,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_FLASH_ERROR,bAddress,"EA","SY",2,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_RAM_CORRUPT,bAddress,"EA","SY",3,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_FLASH_BACKUP_BAD,bAddress,"EA","SY",4,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_WATCHDOG,bAddress,"EA","SY",5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_SYS_PROG_ERROR,bAddress,"EA","SY",6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_EAAI_FAILURE,bAddress,"EA","SY",7,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_BSE_FAILURE,bAddress,"EA","SY",8,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_PASSCODE_RESET,bAddress,"EA","SY",9,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_POWERFAIL,bAddress,"EA","SY",10,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_COMMUNICATION,bAddress,"EA","SY",11,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_CIVACON_COMM_FAILURE,bAddress,"EA","SY",12,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_SHARED_PRINTER,bAddress,"EA","SY",13,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_PTB_PRINTER,bAddress,"EA","SY",14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_01,bAddress,"EA","SY",15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_02,bAddress,"EA","SY",16,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_03,bAddress,"EA","SY",17,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_04,bAddress,"EA","SY",18,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_05,bAddress,"EA","SY",19,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_06,bAddress,"EA","SY",20,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_07,bAddress,"EA","SY",21,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_08,bAddress,"EA","SY",22,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_09,bAddress,"EA","SY",23,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_USER_ALARM_10,bAddress,"EA","SY",24,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ADD_PAK_1_POWERFAIL,bAddress,"EA","SY",25,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ADD_PAK_2_POWERFAIL,bAddress,"EA","SY",26,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ADD_PAK_1_DIAGNOSTIC,bAddress,"EA","SY",27,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ADD_PAK_2_DIAGNOSTIC,bAddress,"EA","SY",28,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ADD_PAK_1_AUTO_DETECT_FAILED,bAddress,"EA","SY",29,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ADD_PAK_2_AUTO_DETECT_FAILED,bAddress,"EA","SY",30,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ADD_PAK_1_COMM_FAIL,bAddress,"EA","SY",31,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_ADD_PAK_2_COMM_FAIL,bAddress,"EA","SY",32,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pSystemTag->AddLeaf(IDS_DISPLAY_FAILURE,bAddress,"EA","SY",33,OPC_READABLE,VT_BOOL,pIO,pDevice);

	for(LONG lInjector=0;lInjector < 24;lInjector++)
	{
		CString	oInjector;
		oInjector.Format(IDS_INJECTOR_NUMBER,lInjector+1);
		CTag*	pInjectorTag=pAlarmsTag->AddBranch(oInjector,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_FEEDBACK,bAddress,"EA","IN",0+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_COMM_ERROR,bAddress,"EA","IN",1+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_LOW_ADD_ERROR,bAddress,"EA","IN",2+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_EXCESS_PULSES,bAddress,"EA","IN",3+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_NO_PULSES,bAddress,"EA","IN",4+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_FREQUENCY,bAddress,"EA","IN",5+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_UNAUTH_FAILED,bAddress,"EA","IN",6+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_GENERAL_ADDITIVE_ERROR,bAddress,"EA","IN",7+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_OVER_SPEED,bAddress,"EA","IN",8+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_COMMAND_REFUSED,bAddress,"EA","IN",9+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pInjectorTag->AddLeaf(IDS_AUTODETECT_FAIL,bAddress,"EA","IN",10+lInjector*11,OPC_READABLE,VT_BOOL,pIO,pDevice);
	}

	// Tags associated with Load Arms
	for(LONG lItem=0;lItem < oArms->Count;lItem++)
	{
		IArmPtr	oArm=oArms->Item(lItem);

		CString	oArmName;
		oArmName.Format(IDS_ARM_NUMBER,oArm->Number);

		BYTE	bAddress=oArm->Address;

		int iArm=oArm->Number-1;

		CTag* pArmTag=pAcculoadTag->AddBranch(oArmName,pIO,pDevice);

		CTag*	pPromptsTag=pArmTag->AddBranch(IDS_PROMPTS,pIO,pDevice);
		pPromptsTag->AddLeaf(_T("1"),bAddress,"TI","1",0,OPC_READABLE,VT_I4,pIO,pDevice);
		pPromptsTag->AddLeaf(_T("2"),bAddress,"TI","2",0,OPC_READABLE,VT_I4,pIO,pDevice);
		pPromptsTag->AddLeaf(_T("3"),bAddress,"TI","3",0,OPC_READABLE,VT_I4,pIO,pDevice);
		pPromptsTag->AddLeaf(_T("4"),bAddress,"TI","4",0,OPC_READABLE,VT_I4,pIO,pDevice);
		pPromptsTag->AddLeaf(_T("5"),bAddress,"TI","5",0,OPC_READABLE,VT_I4,pIO,pDevice);

		CTag* pStatusTag=pArmTag->AddBranch(IDS_STATUS,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_AUTHORIZED,bAddress,"EQ",NULL,0,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_FLOWING,bAddress,"EQ",NULL,1,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_RELEASED,bAddress,"EQ",NULL,2,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_KEYPAD_DATA_PENDING,bAddress,"EQ",NULL,4,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_BATCH_DONE,bAddress,"EQ",NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_TRANSACTION_DONE,bAddress,"EQ",NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_TRANSACTION_IN_PROGRESS,bAddress,"EQ",NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_ALARM,bAddress,"EQ",NULL,11,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_DISPLAY_MESSAGE_TIME_OUT,bAddress,"EQ",NULL,13,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_CHECKING_ENTRIES,bAddress,"EQ",NULL,19,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_PRESETTING_IN_PROGRESS,bAddress,"EQ",NULL,60,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_PERMISSIVE_DELAY,bAddress,"EQ",NULL,62,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_RESET_TRANSACTION_DONE,bAddress,"RE","TD",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_RESET_BATCH_DONE,bAddress,"RE","BD",0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_GET_BATCH_NUMBER,bAddress,"RB",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
		pStatusTag->AddLeaf(IDS_PRESET_ARM_NUMBER, bAddress, "OA", NULL, 0, OPC_READABLE, VT_INT, pIO, pDevice);

		CTag* pAlarmsTag=pArmTag->AddBranch(IDS_ALARMS,pIO,pDevice);
		CTag* pRecipeTag=pAlarmsTag->AddBranch(IDS_RECIPE,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_PROGRAM_ERROR,bAddress,"EA","RR",0,OPC_READABLE,VT_BOOL,pIO,pDevice);

		pAlarmsTag->AddLeaf(IDS_PROGRAM_ERROR,bAddress,"EA","AR",0,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAlarmsTag->AddLeaf(IDS_ZERO_FLOW,bAddress,"EA","AR",1,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAlarmsTag->AddLeaf(IDS_OVERRUN,bAddress,"EA","AR",2,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAlarmsTag->AddLeaf(IDS_TICKET,bAddress,"EA","AR",3,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAlarmsTag->AddLeaf(IDS_CLEAN_LINE,bAddress,"EA","AR",4,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pAlarmsTag->AddLeaf(IDS_ADDITIVE_CLEAN_LINE,bAddress,"EA","AR",5,OPC_READABLE,VT_BOOL,pIO,pDevice);

		CTag* pDynamicValuesTag=pArmTag->AddBranch(IDS_DYNAMIC_VALUES,pIO,pDevice);

		CTag* pNonResettableTotals=pArmTag->AddBranch(IDS_NON_RESETTABLE_TOTALS,pIO,pDevice);

		CTag* pSystemTag=pDynamicValuesTag->AddBranch(IDS_SYSTEM,pIO,pDevice);

		pSystemTag->AddLeaf(IDS_CURRENT_FLOW_RATE_UNITS_MIN,bAddress,"DY","SY",0+iArm,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_FLOW_RATE_UNITS_HR,bAddress,"DY","SY",6+iArm,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_RECIPE,bAddress,"DY","SY",12+iArm,OPC_READABLE,VT_I4,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_PRESET_UNITS_MIN,bAddress,"DY","SY",18+iArm,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_DELIVERED_VOLUME,bAddress,"DY","SY",24+iArm,OPC_READABLE,VT_R8,pIO,pDevice);
		pSystemTag->AddLeaf(IDS_CURRENT_REMAINING_VOLUME,bAddress,"DY","SY",30+iArm,OPC_READABLE,VT_R8,pIO,pDevice);

		CTag* pAnalogInputs=pArmTag->AddBranch(IDS_ANALOG_INPUTS,pIO,pDevice);

		CTag* pMeterTag;

		for(LONG lProduct=0;lProduct < oArm->Products;lProduct++)
		{
			CString	oName;

			if(oArm->Type != RATIO)
			{
				if(lProduct == 0)
				{
					oName.Format(IDS_METER);

					pMeterTag=pAnalogInputs->AddBranch(oName,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_TEMPERATURE,bAddress,"RD","",0,OPC_READABLE,VT_R8,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_PRESSURE,bAddress,"RD","",1,OPC_READABLE,VT_R8,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_DENSITY,bAddress,"RD","",2,OPC_READABLE,VT_R8,pIO,pDevice);

					pMeterTag=pAlarmsTag->AddBranch(oName,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_PROGRAM_ERROR,bAddress,"EA",szMX[lProduct],0,OPC_READABLE,VT_BOOL,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_TRANSMITTER_INTEGRITY,bAddress,"EA",szMX[lProduct],1,OPC_READABLE,VT_BOOL,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_PULSE_SECURITY,bAddress,"EA",szMX[lProduct],2,OPC_READABLE,VT_BOOL,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_VALVE_FAULT,bAddress,"EA",szMX[lProduct],3,OPC_READABLE,VT_BOOL,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_TEMPERATURE_PROBE,bAddress,"EA",szMX[lProduct],4,OPC_READABLE,VT_BOOL,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_PRESSURE_TRANSMITTER,bAddress,"EA",szMX[lProduct],5,OPC_READABLE,VT_BOOL,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_DENSITY_TRANSMITTER,bAddress,"EA",szMX[lProduct],6,OPC_READABLE,VT_BOOL,pIO,pDevice);
					pMeterTag->AddLeaf(IDS_TURBINE_METER,bAddress,"EA",szMX[lProduct],7,OPC_READABLE,VT_BOOL,pIO,pDevice);

				}
			}			
			else
			{
				oName.Format(IDS_METER_NUMBER,lProduct+1);

				pMeterTag=pAnalogInputs->AddBranch(oName,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_TEMPERATURE,bAddress,"RD",szPX[lProduct],0,OPC_READABLE,VT_R8,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_PRESSURE,bAddress,"RD",szPX[lProduct],1,OPC_READABLE,VT_R8,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_DENSITY,bAddress,"RD",szPX[lProduct],2,OPC_READABLE,VT_R8,pIO,pDevice);

				pMeterTag=pAlarmsTag->AddBranch(oName,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_PROGRAM_ERROR,bAddress,"EA",szMX[lProduct],0,OPC_READABLE,VT_BOOL,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_TRANSMITTER_INTEGRITY,bAddress,"EA",szMX[lProduct],1,OPC_READABLE,VT_BOOL,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_PULSE_SECURITY,bAddress,"EA",szMX[lProduct],2,OPC_READABLE,VT_BOOL,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_VALVE_FAULT,bAddress,"EA",szMX[lProduct],3,OPC_READABLE,VT_BOOL,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_TEMPERATURE_PROBE,bAddress,"EA",szMX[lProduct],4,OPC_READABLE,VT_BOOL,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_PRESSURE_TRANSMITTER,bAddress,"EA",szMX[lProduct],5,OPC_READABLE,VT_BOOL,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_DENSITY_TRANSMITTER,bAddress,"EA",szMX[lProduct],6,OPC_READABLE,VT_BOOL,pIO,pDevice);
				pMeterTag->AddLeaf(IDS_TURBINE_METER,bAddress,"EA",szMX[lProduct],7,OPC_READABLE,VT_BOOL,pIO,pDevice);

			}

			oName.Format(IDS_PRODUCT_NUMBER,lProduct+1);

			CTag* pProductTag=pMeterTag->AddBranch(oName,pIO,pDevice);

			pProductTag->AddLeaf(IDS_PROGRAM_ERROR,bAddress,"EA",szPX[lProduct],0,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_BACK_PRESSURE,bAddress,"EA",szPX[lProduct],1,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_HIGH_DENSITY,bAddress,"EA",szPX[lProduct],2,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_HIGH_FLOW,bAddress,"EA",szPX[lProduct],3,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_HIGH_PRESSURE,bAddress,"EA",szPX[lProduct],4,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_HIGH_TEMPERATURE,bAddress,"EA",szPX[lProduct],5,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_LOW_DENSITY,bAddress,"EA",szPX[lProduct],6,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_LOW_FLOW,bAddress,"EA",szPX[lProduct],7,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_LOW_PRESSURE,bAddress,"EA",szPX[lProduct],8,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_LOW_TEMPERATURE,bAddress,"EA",szPX[lProduct],9,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_ZERO_FLOW,bAddress,"EA",szPX[lProduct],10,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_OVERRUN,bAddress,"EA",szPX[lProduct],11,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_BLOCK_VALVE,bAddress,"EA",szPX[lProduct],12,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_BLEND_HIGH,bAddress,"EA",szPX[lProduct],13,OPC_READABLE,VT_BOOL,pIO,pDevice);
			pProductTag->AddLeaf(IDS_BLEND_LOW,bAddress,"EA",szPX[lProduct],14,OPC_READABLE,VT_BOOL,pIO,pDevice);
		
			pProductTag=pDynamicValuesTag->AddBranch(oName,pIO,pDevice);
			pProductTag->AddLeaf(IDS_CURRENT_FLOW_RATE_UNITS_MIN,bAddress,"DY",szPX[lProduct],0,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_CURRENT_FLOW_RATE_UNITS_HR,bAddress,"DY",szPX[lProduct],1,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_CURRENT_TEMPERATURE,bAddress,"DY",szPX[lProduct],12,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_CURRENT_DENSITY,bAddress,"DY",szPX[lProduct],13,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_CURRENT_METER_FACTOR,bAddress,"DY",szPX[lProduct],14,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_CURRENT_VALVE_REQUESTED_POSITION,bAddress,"DY",szPX[lProduct],15,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_CURRENT_PERCENTAGE_OF_BATCH,bAddress,"DY",szPX[lProduct],16,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_DESIRED_PERCENTAGE_OF_BATCH,bAddress,"DY",szPX[lProduct],17,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_CURRENT_REFERENCE_DENSITY,bAddress,"DY",szPX[lProduct],28,OPC_READABLE,VT_R8,pIO,pDevice);

			CTag* pBatchTag=pProductTag->AddBranch(IDS_BATCH,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,"DY",szPX[lProduct],2,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,"DY",szPX[lProduct],3,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_API,bAddress,"DY",szPX[lProduct],4,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_REFERENCE_DENSITY,bAddress,"DY",szPX[lProduct],5,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_RELATIVE_DENSITY,bAddress,"DY",szPX[lProduct],6,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_PRESSURE,bAddress,"DY",szPX[lProduct],7,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_VAPOR_PRESSURE,bAddress,"DY",szPX[lProduct],8,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_METER_FACTOR,bAddress,"DY",szPX[lProduct],9,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_CTL,bAddress,"DY",szPX[lProduct],10,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_CPL,bAddress,"DY",szPX[lProduct],11,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"DY",szPX[lProduct],18,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"DY",szPX[lProduct],19,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_GST_VOLUME,bAddress,"DY",szPX[lProduct],20,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_GSV_VOLUME,bAddress,"DY",szPX[lProduct],21,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"DY",szPX[lProduct],22,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_CTPL,bAddress,"DY",szPX[lProduct],29,OPC_READABLE,VT_R8,pIO,pDevice);

			CTag* pTransactionTag=pProductTag->AddBranch(IDS_TRANSACTION,pIO,pDevice);
			pTransactionTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"DY",szPX[lProduct],23,OPC_READABLE,VT_R8,pIO,pDevice);
			pTransactionTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"DY",szPX[lProduct],24,OPC_READABLE,VT_R8,pIO,pDevice);
			pTransactionTag->AddLeaf(IDS_GST_VOLUME,bAddress,"DY",szPX[lProduct],25,OPC_READABLE,VT_R8,pIO,pDevice);
			pTransactionTag->AddLeaf(IDS_GSV_VOLUME,bAddress,"DY",szPX[lProduct],26,OPC_READABLE,VT_R8,pIO,pDevice);
			pTransactionTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"DY",szPX[lProduct],27,OPC_READABLE,VT_R8,pIO,pDevice);

			pProductTag=pNonResettableTotals->AddBranch(oName,pIO,pDevice);
			pProductTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"VT",szPX[lProduct],0,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"VT",szPX[lProduct],1,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_GST_VOLUME,bAddress,"VT",szPX[lProduct],2,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_GSV_VOLUME,bAddress,"VT",szPX[lProduct],3,OPC_READABLE,VT_R8,pIO,pDevice);
			pProductTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"VT",szPX[lProduct],4,OPC_READABLE,VT_R8,pIO,pDevice);
			
		}

		CTag* pCurrentBatchTag=pDynamicValuesTag->AddBranch(IDS_CURRENT_BATCH,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_RECIPE,bAddress,"DY","CB",0,OPC_READABLE,VT_I4,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"DY","CB",1,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"DY","CB",2,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_GST_VOLUME,bAddress,"DY","CB",3,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_GSV_VOLUME,bAddress,"DY","CB",4,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"DY","CB",5,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,"DY","CB",6,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,"DY","CB",7,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_PRESSURE,bAddress,"DY","CB",8,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_METER_FACTOR,bAddress,"DY","CB",9,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_CTL,bAddress,"DY","CB",10,OPC_READABLE,VT_R8,pIO,pDevice);
		pCurrentBatchTag->AddLeaf(IDS_AVERAGE_CPL,bAddress,"DY","CB",11,OPC_READABLE,VT_R8,pIO,pDevice);

		CTag* pAdditivesTag=pCurrentBatchTag->AddBranch(IDS_ADDITIVES,pIO,pDevice);
		for(LONG lInjector=0;lInjector < 24;lInjector++)
		{
			CString	oInjector;
			oInjector.Format(_T("%02d"),lInjector+1);
			CTag* pInjectorTag=pAdditivesTag->AddBranch(oInjector,pIO,pDevice);
			pInjectorTag->AddLeaf(IDS_VOLUME,bAddress,"DY","CB",12+lInjector,OPC_READABLE,VT_R8,pIO,pDevice);
		}


		CTag* pBatchTag=pDynamicValuesTag->AddBranch(IDS_BATCH,pIO,pDevice);
		for(long lBatch=0;lBatch < 10;lBatch++)
		{
			CString	oBatch;
			oBatch.Format(_T("%02d"),lBatch+1);

			CTag* pSpecificBatchTag=pBatchTag->AddBranch(oBatch,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_RECIPE,bAddress,"DY",szBX[lBatch],0,OPC_READABLE,VT_I4,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"DY",szBX[lBatch],1,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"DY",szBX[lBatch],2,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_GST_VOLUME,bAddress,"DY",szBX[lBatch],3,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_GSV_VOLUME,bAddress,"DY",szBX[lBatch],4,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"DY",szBX[lBatch],5,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,"DY",szBX[lBatch],6,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,"DY",szBX[lBatch],7,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_PRESSURE,bAddress,"DY",szBX[lBatch],8,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_METER_FACTOR,bAddress,"DY",szBX[lBatch],9,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_CTL,bAddress,"DY",szBX[lBatch],10,OPC_READABLE,VT_R8,pIO,pDevice);
			pSpecificBatchTag->AddLeaf(IDS_AVERAGE_CPL,bAddress,"DY",szBX[lBatch],11,OPC_READABLE,VT_R8,pIO,pDevice);

			CTag* pAdditivesTag=pSpecificBatchTag->AddBranch(IDS_ADDITIVES,pIO,pDevice);
			for(LONG lInjector=0;lInjector < 24;lInjector++)
			{
				CString	oInjector;
				oInjector.Format(_T("%02d"),lInjector+1);
				CTag* pInjectorTag=pAdditivesTag->AddBranch(oInjector,pIO,pDevice);
				pInjectorTag->AddLeaf(IDS_VOLUME,bAddress,"DY",szBX[lBatch],12+lInjector,OPC_READABLE,VT_R8,pIO,pDevice);
			}
		}

		// Tags associated with Flow Control Additives
		CTag* pFlowControlAdditivesTag=pDynamicValuesTag->AddBranch(IDS_FLOW_CONTROLLED_ADDITIVES,pIO,pDevice);
		for(LONG lFlowControlledAdditive=0;lFlowControlledAdditive < 4;lFlowControlledAdditive++)
		{
			CString	oFlowControlledAdditive;
			oFlowControlledAdditive.Format(_T("%02d"),lFlowControlledAdditive+1);
			CTag* pFlowControlledAdditiveTag=pFlowControlAdditivesTag->AddBranch(oFlowControlledAdditive,pIO,pDevice);

			CTag* pBatchTag=pFlowControlledAdditiveTag->AddBranch(IDS_BATCH,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"DY","FA",lFlowControlledAdditive*12,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"DY","FA",lFlowControlledAdditive*12+1,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_GST_VOLUME,bAddress,"DY","FA",lFlowControlledAdditive*12+2,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"DY","FA",lFlowControlledAdditive*12+3,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_CURRENT_TEMPERATURE,bAddress,"DY","FA",lFlowControlledAdditive*12+4,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,"DY","FA",lFlowControlledAdditive*12+5,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,"DY","FA",lFlowControlledAdditive*12+6,OPC_READABLE,VT_R8,pIO,pDevice);
			pBatchTag->AddLeaf(IDS_AVERAGE_CTL,bAddress,"DY","FA",lFlowControlledAdditive*12+7,OPC_READABLE,VT_R8,pIO,pDevice);

			CTag* pTransactionTag=pFlowControlledAdditiveTag->AddBranch(IDS_TRANSACTION,pIO,pDevice);
			pTransactionTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"DY","FA",lFlowControlledAdditive*12+8,OPC_READABLE,VT_R8,pIO,pDevice);
			pTransactionTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"DY","FA",lFlowControlledAdditive*12+9,OPC_READABLE,VT_R8,pIO,pDevice);
			pTransactionTag->AddLeaf(IDS_GST_VOLUME,bAddress,"DY","FA",lFlowControlledAdditive*12+10,OPC_READABLE,VT_R8,pIO,pDevice);
			pTransactionTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"DY","FA",lFlowControlledAdditive*12+11,OPC_READABLE,VT_R8,pIO,pDevice);
		}

		CTag* pTransactionTag=pDynamicValuesTag->AddBranch(IDS_TRANSACTION,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_RAW_VOLUME,bAddress,"DY","TR",1,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_GROSS_VOLUME,bAddress,"DY","TR",2,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_GST_VOLUME,bAddress,"DY","TR",3,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_GSV_VOLUME,bAddress,"DY","TR",4,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_MASS_TOTAL,bAddress,"DY","TR",5,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_TEMPERATURE,bAddress,"DY","TR",6,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_DENSITY,bAddress,"DY","TR",7,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_PRESSURE,bAddress,"DY","TR",8,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_METER_FACTOR,bAddress,"DY","TR",9,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_CTL,bAddress,"DY","TR",10,OPC_READABLE,VT_R8,pIO,pDevice);
		pTransactionTag->AddLeaf(IDS_AVERAGE_CPL,bAddress,"DY","TR",11,OPC_READABLE,VT_R8,pIO,pDevice);

		pAdditivesTag=pTransactionTag->AddBranch(IDS_ADDITIVES,pIO,pDevice);
		for(LONG lInjector=0;lInjector < 24;lInjector++)
		{
			CString	oInjector;
			oInjector.Format(_T("%02d"),lInjector+1);
			CTag* pInjectorTag=pAdditivesTag->AddBranch(oInjector,pIO,pDevice);
			pInjectorTag->AddLeaf(IDS_VOLUME,bAddress,"DY","TR",12+lInjector,OPC_READABLE,VT_R8,pIO,pDevice);
		}


		pArmTag->AddLeaf(IDS_SWING_ARM_POSITION,bAddress,"SW",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);

		pArmTag->AddLeaf(IDS_ALLOCATE_RECIPES,bAddress,"AB",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_AUTHORIZE_TRANSACTION,bAddress,"AP",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_END_BATCH,bAddress,"EB",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pArmTag->AddLeaf(IDS_END_TRANSACTION,bAddress,"ET",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pArmTag->AddLeaf(IDS_STOP,bAddress,"ST",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pArmTag->AddLeaf(IDS_START,bAddress,"SA",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_SECOND_LINE,bAddress,"WA",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_THIRD_LINE,bAddress,"WB",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_FOURTH_LINE,bAddress,"WC",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_FIRST_LINE_WITH_PROMPT,bAddress,"WD",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_FIRST_LINE_WITH_PROMPT_ON_SET,bAddress,"WP",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_FIRST_LINE_WITH_PROMPT_ON_SET_NO_ECHO,bAddress,"WQ",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_WRITE_FIRST_LINE_WITH_PROMPT_NO_ECHO,bAddress,"WX",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_KEYPAD_DATA,bAddress,"RK",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_RELEASE_KEYPAD_AND_DISPLAY,bAddress,"DA",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pArmTag->AddLeaf(IDS_PRESET_AMOUNT,bAddress,"RP",NULL,0,OPC_READABLE,VT_R8,pIO,pDevice);
		pArmTag->AddLeaf(IDS_AUTHORIZE_AND_SET_BATCH_AMOUNT,bAddress,"SB",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_GET_KEY,bAddress,"GK",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_FORCE_FULL_SCREEN_VIEW,bAddress,"FS",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pArmTag->AddLeaf(IDS_LOG_OUT_OF_PROGRAM_MODE,bAddress,"LO",NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
		pArmTag->AddLeaf(IDS_PROGRAM_CODE_CHANGE,bAddress,"PC",NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
		pArmTag->AddLeaf(IDS_RECIPE,bAddress,"RR",NULL,0,OPC_READABLE,VT_UI2,pIO,pDevice);
		pArmTag->AddLeaf(IDS_ARM_CLEAN_LINE_AMOUNT,bAddress,"PV","AR",221,OPC_READABLE,VT_R8,pIO,pDevice);
		pArmTag->AddLeaf(IDS_ARM_CLEAN_LINE_PRODUCT,bAddress,"PV","AR",222,OPC_READABLE,VT_UI1,pIO,pDevice);
		pArmTag->AddLeaf(IDS_ARM_CLEAN_LINE_BLEND,bAddress,"PV","AR",230,OPC_READABLE,VT_BOOL,pIO,pDevice);
		pArmTag->AddLeaf(IDS_PRESET_ARM_NUMBER, bAddress, "OA", NULL, 0, OPC_READABLE, VT_INT, pIO, pDevice);

		pAdditivesTag=pNonResettableTotals->AddBranch(IDS_ADDITIVES,pIO,pDevice);
		for(LONG lInjector=0;lInjector < 24;lInjector++)
		{
			CString	oInjector;
			oInjector.Format(_T("%02d"),lInjector+1);
			CTag* pInjectorTag=pAdditivesTag->AddBranch(oInjector,pIO,pDevice);
			pInjectorTag->AddLeaf(IDS_VOLUME,bAddress,"VT",szAXX[lInjector],0,OPC_READABLE,VT_R8,pIO,pDevice);
		}
	}

		// Tags associated with Recipes
	CTag* pRecipesTag=pAcculoadTag->AddBranch(IDS_RECIPES,pIO,pDevice);
	for(int iRecipe=0;iRecipe < 50;iRecipe++)
	{
		CString strRecipe;
		strRecipe.Format(_T("%02d"),iRecipe+1);
		CTag* pRecipeTag=pRecipesTag->AddBranch(strRecipe,pIO,pDevice);

		pRecipeTag->AddLeaf(IDS_USED,bAddress,"PV",szXX[iRecipe],1,OPC_READABLE,VT_UI1,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_NAME,bAddress,"PV",szXX[iRecipe],2,OPC_READABLE,VT_BSTR,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_HM_CLASS_PRODUCT,bAddress,"PV",szXX[iRecipe],3,OPC_READABLE,VT_UI1,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_1ST_DELIVERED,bAddress,"PV",szXX[iRecipe],4,OPC_READABLE,VT_UI1,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_1ST_PERCENTAGE,bAddress,"PV",szXX[iRecipe],5,OPC_READABLE,VT_R8,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_2ND_DELIVERED,bAddress,"PV",szXX[iRecipe],6,OPC_READABLE,VT_UI1,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_2ND_PERCENTAGE,bAddress,"PV",szXX[iRecipe],7,OPC_READABLE,VT_R8,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_3RD_DELIVERED,bAddress,"PV",szXX[iRecipe],8,OPC_READABLE,VT_UI1,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_3RD_PERCENTAGE,bAddress,"PV",szXX[iRecipe],9,OPC_READABLE,VT_R8,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_4TH_DELIVERED,bAddress,"PV",szXX[iRecipe],10,OPC_READABLE,VT_UI1,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_4TH_PERCENTAGE,bAddress,"PV",szXX[iRecipe],11,OPC_READABLE,VT_R8,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_5TH_DELIVERED,bAddress,"PV",szXX[iRecipe],12,OPC_READABLE,VT_UI1,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_5TH_PERCENTAGE,bAddress,"PV",szXX[iRecipe],13,OPC_READABLE,VT_R8,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_6TH_DELIVERED,bAddress,"PV",szXX[iRecipe],14,OPC_READABLE,VT_UI1,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_6TH_PERCENTAGE,bAddress,"PV",szXX[iRecipe],15,OPC_READABLE,VT_R8,pIO,pDevice);
		pRecipeTag->AddLeaf(IDS_CLEAN_LINE_DEDUCT,bAddress,"PV",szXX[iRecipe],16,OPC_READABLE,VT_UI1,pIO,pDevice);

		for(int iInjector=0;iInjector < 24;iInjector++)
		{
			CString strTag;
			strTag.Format(IDS_ADDITIVE_INJECTOR_AMOUNT_PER_CYCLE,iInjector+1);
			pRecipeTag->AddLeaf(strTag,bAddress,"PV",szXX[iRecipe],17+iInjector*3,OPC_READABLE,VT_R8,pIO,pDevice);
			strTag.Format(IDS_ADDITIVE_INJECTOR_RATE,iInjector+1);
			pRecipeTag->AddLeaf(strTag,bAddress,"PV",szXX[iRecipe],18+iInjector*3,OPC_READABLE,VT_R8,pIO,pDevice);
			strTag.Format(IDS_ADDITIVE_INJECTOR_PRODUCTS_USING_INJECTOR,iInjector+1);
			pRecipeTag->AddLeaf(strTag,bAddress,"PV",szXX[iRecipe],19+iInjector*3,OPC_READABLE,VT_UI1,pIO,pDevice);
		}

		pRecipeTag->AddLeaf(IDS_CLEAN_LINE_PRODUCT,bAddress,"PV",szXX[iRecipe],89,OPC_READABLE,VT_UI1,pIO,pDevice);
	}
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
			if((dwFunctions & 0x10) == 0)
			{
				strMessage.LoadString(IDS_HARDWARE_KEY_FAILURE);
				theApp.LogError(strMessage);
				return;
			}
		}

		// For each Preset (Accuload/Microload)
		IAcculoadsPtr	oAcculoads(CLSID_Acculoads);
		IAcculoadCollectionPtr	oAcculoadCollection=oAcculoads->Enumerate();
		for(LONG lItem=0;lItem < oAcculoadCollection->Count;lItem++)
		{
			IAcculoadPtr	oAccuload=oAcculoadCollection->Item(lItem);
			AddAccuload(oAccuload);
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

void CDeviceManager::AddAccuload(IAcculoadPtr oAccuload)
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

		if(oAccuload->NetworkCommunications)
		{
			if(!pIO->m_oIPAddress.Compare(oAccuload->IPAddress))
				break;
		}

		else
		{
			if(pIO->m_lIndex == oAccuload->PortIndex)
				break;
		}

		pIO=NULL;
	}

	if(!pIO)
	{
		if(oAccuload->NetworkCommunications)
		{
			pIO=new CIO(oAccuload->IPAddress,7734);
		}

		else
		{
			IPortsPtr	oPorts(CLSID_Ports);
			IPortPtr		oPort=oPorts->Get(oAccuload->PortIndex);

			pIO=new CIO(oPort->Index,
								(LPCTSTR) oPort->ID,
								oPort->Baud,
								oPort->DataBits,
								oPort->Parity,
								oPort->StopBits);

		}

		if(!pIO)
			throw (CString(_T("Memory Allocation Error")));

		m_IOList.AddTail(pIO);
	}

	if ( pIO != NULL )
		pIO->m_dwUseCount++;

	if(oAccuload->Type == ACCULOAD_III_Q
	|| oAccuload->Type == ACCULOAD_III_SA)
		AddAccuload3(oAccuload,pIO);

	else if(oAccuload->Type == MICROLOAD_NET)
		AddMicroloadNet(oAccuload,pIO);

	else if(oAccuload->Type == MULTILOAD_II_SMP)
		AddMultiloadSMP(oAccuload,pIO);

	else if(oAccuload->Type == MULTILOAD_II)
		AddMultiload(oAccuload,pIO);

	else if(oAccuload->Type == SMITH_PROXIMITY)
		AddSmithProximity(oAccuload,pIO);

	else if(oAccuload->Type == RCU_II_OPEN)
		AddRcuIIOpenProtocol(oAccuload,pIO);

	else if(oAccuload->Type == RCU_II_RCU)
		AddRcuIIRcuProtocol(oAccuload,pIO);
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
void CDeviceManager::AddSmithProximity(IAcculoadPtr oAccuload,CIO* pIO)
{
	CDevice* pDevice=(CDevice*) new CAcculoadDevice(oAccuload->Type);

	CTag* pCardReaderTag=m_pRoot->AddBranch((LPTSTR) oAccuload->ID,pIO,pDevice);

	IArmCollectionPtr	oArms=oAccuload->Arms;
	if(!oArms->Count)
		return;

	// Some of the tags are System level and accessible through any arm
	// use the first to establish the address
	IArmPtr	oArm=oArms->Item(0L);
	BYTE	bAddress;

	bAddress=oArm->Address;

	// The ordering of the tags is important here. Logic in IO.cpp depends upon it
	pCardReaderTag->AddLeaf(IDS_DATA,bAddress,"CD",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_I_O_CONTROL,bAddress,"CD",NULL,7,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_GREEN_LED,bAddress,"CD",NULL,0,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_RED_LED,bAddress,"CD",NULL,1,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_BEEP,bAddress,"CD",NULL,2,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
	pCardReaderTag->AddLeaf(IDS_CONTACT,bAddress,"CD",NULL,3,OPC_WRITEABLE,VT_BOOL,pIO,pDevice);
}


void CDeviceManager::AddRcuIIRcuProtocol(IAcculoadPtr oAccuload, CIO* pIO)
{
	CDevice* pDevice=(CDevice*) new CMultiloadDevice(oAccuload->Type);

	CTag* pRcuIITag=m_pRoot->AddBranch((LPTSTR) oAccuload->ID,pIO,pDevice);

	IArmCollectionPtr	oArms=oAccuload->Arms;
	if(!oArms->Count)
		return;

	// Some of the tags are System level and accessible through any arm
	// use the first to establish the address
	IArmPtr	oArm=oArms->Item(0L);
	BYTE	bAddress;

	bAddress=oArm->Address;


	((CMultiloadDevice*) pDevice)->m_pRcuStatusTag=pRcuIITag->AddLeaf(IDS_RCU_STATUS,bAddress,chQ,NULL,0,OPC_READABLE,VT_UI1,pIO,pDevice);
	((CMultiloadDevice*) pDevice)->m_pCardStatusTag=pRcuIITag->AddLeaf(IDS_CARD_STATUS,bAddress,chQ,NULL,0,OPC_READABLE,VT_UI1,pIO,pDevice);
	pRcuIITag->AddLeaf(IDS_READ_REGISTER,bAddress,chR,NULL,0,OPC_READABLE | OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuIITag->AddLeaf(IDS_WRITE_REGISTER,bAddress,chU,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuIITag->AddLeaf(IDS_TERMINAL_COMMAND,bAddress,chT,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuIITag->AddLeaf(IDS_KEYPAD_DATA,bAddress,"R302",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pRcuIITag->AddLeaf(IDS_TERMINATING_KEY,bAddress,"R303",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pRcuIITag->AddLeaf(IDS_CARD_NUMBER,bAddress,"R305",NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pRcuIITag->AddLeaf(IDS_PORT_0,bAddress,chPORT0,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);

	// RCU Status
	CTag* pStatusTag=pRcuIITag->AddBranch(IDS_STATUS,pIO,pDevice);
	((CMultiloadDevice*) pDevice)->m_pStatusTag=pStatusTag;
	pStatusTag->AddLeaf(IDS_POWER_UP,bAddress,chQ,NULL,14,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_CONFIGURED,bAddress,chQ,NULL,15,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_HOST_UP,bAddress,chQ,NULL,5,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_INPUT_IN_PROGRESS,bAddress,chQ,NULL,6,OPC_READABLE,VT_BOOL,pIO,pDevice);
	pStatusTag->AddLeaf(IDS_INPUT_DONE,bAddress,chQ,NULL,7,OPC_READABLE,VT_BOOL,pIO,pDevice);
}

void CDeviceManager::AddRcuIIOpenProtocol(IAcculoadPtr oAccuload,CIO* pIO)
{
	CDevice* pDevice=(CDevice*) new CMultiloadDevice(oAccuload->Type);

	CTag* pRcuII=m_pRoot->AddBranch((LPTSTR) oAccuload->ID,pIO,pDevice);

	IArmCollectionPtr	oArms=oAccuload->Arms;
	if(!oArms->Count)
		return;

	IArmPtr	oArm=oArms->Item(0L);

	BYTE	bAddress;
	bAddress=oArm->Address;

	pRcuII->AddLeaf(IDS_QUERY_CARD,bAddress,chQC,NULL,0,OPC_READABLE,VT_BSTR,pIO,pDevice);
	pRcuII->AddLeaf(IDS_CLEAR_DISPLAY,bAddress,chCD,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pRcuII->AddLeaf(IDS_DISPLAY_TEXT,bAddress,chDT,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuII->AddLeaf(IDS_KEY_INPUT,bAddress,chKI,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pRcuII->AddLeaf(IDS_STRING_INPUT,bAddress,chSI,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pRcuII->AddLeaf(IDS_QUERY_INPUT,bAddress,chQI,NULL,0,OPC_READABLE,VT_EMPTY,pIO,pDevice);
	pRcuII->AddLeaf(IDS_ABORT_INPUT,bAddress,chAI,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pRcuII->AddLeaf(IDS_RESET_DISPLAY,bAddress,chRD,NULL,0,OPC_WRITEABLE,VT_EMPTY,pIO,pDevice);
	pRcuII->AddLeaf(IDS_PORT_0,bAddress,chPORT0,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuII->AddLeaf(IDS_PORT_1,bAddress,chPORT1,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuII->AddLeaf(IDS_PORT_2,bAddress,chPORT2,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuII->AddLeaf(IDS_PORT_3,bAddress,chPORT3,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuII->AddLeaf(IDS_PORT_8,bAddress,chPORT8,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuII->AddLeaf(IDS_PORT_10,bAddress,chPORT10,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuII->AddLeaf(IDS_PORT_11,bAddress,chPORT11,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);
	pRcuII->AddLeaf(IDS_PORT_12,bAddress,chPORT12,NULL,0,OPC_WRITEABLE,VT_BSTR,pIO,pDevice);


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
					if(((CAcculoadItem*) pItem)->m_oTag == oPath)
					{
						((CAcculoadItem*) pItem)->m_pTag=pTag;
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
					if(((CAcculoadItem*) pItem)->m_pTag == pTag)
					{
						if(pItem->m_bActive
						&& pTag->m_dwAccessRights & OPC_READABLE
						&& pTag->m_pIO)
							pTag->m_pIO->RemoveTagFromScanList(pTag);
						((CAcculoadItem*) pItem)->m_pTag=NULL;
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