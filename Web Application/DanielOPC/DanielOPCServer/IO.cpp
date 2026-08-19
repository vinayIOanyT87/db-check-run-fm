/******************************************************************************

	FILE NAME:		IO.cpp


	PURPOSE:			Implementation of the CIO


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
		06/25/2008	W.Gray		7.3.2.0 - Correction to ProcessResponse handling
										of StartCommunications, response length varies
										based upon number of components

		06/29/2008	W.Gray		7.3.3.0 - Corrected error reading Additive & Component
										totalizers.

		11/25/2008	W.Gray		7.6.1.0 - Changed to delay 5 seconds on OpenCommPort error (CSI 6319)

		11/25/2008	W.Gray		7.4.6.1 - Change PerformSerialIO to call SignalCommunicationsFailure
										when OpenCommPort fails


		06/22/2009	W.Gray		7.4.6.2 - Revised scan logic to be more accurate in scan timing.

		12/14/2009	W.Gray		7.5.1.0 - Revised to support reading passcodes

*******************************************************************************/

#include "StdAfx.h"
#include ".\io.h"
#include "DeviceManager.h"

extern CDeviceManager*		g_pDeviceManager;

#define STX 2
#define ETX 3
#define MAX_INACTIVITY 20	// Constant which determines in 100 msec when port is closed
									// after inactivity.

// CTag
CTag::CTag(LPCTSTR szName,BYTE bCommand)
{
	m_pParent=NULL;
	m_oName=szName;
	m_dwAccessRights=OPC_READABLE;
	m_dwScanCount=0;
	m_dwUpdateSequence=0;
	m_pIO=NULL;
	m_bCommand=bCommand;
	VariantInit(&m_Value);
   CoFileTimeNow(&m_Timestamp);
	m_wQuality=OPC_QUALITY_BAD;
	m_pDevice=NULL;
}

CTag::CTag(INT iID,BYTE bCommand)
{
	m_pParent=NULL;
	m_oName.LoadString(iID);
	m_dwAccessRights=OPC_READABLE;
	m_dwScanCount=0;
	m_dwUpdateSequence=0;
	m_pIO=NULL;
	m_bCommand=bCommand;
	VariantInit(&m_Value);
   CoFileTimeNow(&m_Timestamp);
	m_wQuality=OPC_QUALITY_BAD;
	m_pDevice=NULL;
}

CTag::~CTag()
{
	while(m_Branch.GetCount())
	{
		CTag* pTag=m_Branch.RemoveTail();
		delete pTag;
	}

	// Remove all Leaf Tags from OPCGroup items which removes them from CIO.m_TagScanList
	POSITION pos=m_Leaf.GetHeadPosition();
	while(pos)
	{
		CTag* pTag=m_Leaf.GetNext(pos);
		g_pDeviceManager->RemoveTagFromGroupItems(pTag);
	}

	while(m_Leaf.GetCount())
	{
		CTag* pTag=m_Leaf.RemoveTail();
		delete pTag;
	}

	// if m_pParent is null this tag isn't in the tag hierarchy
	if(m_pParent != NULL)
		g_pDeviceManager->RemoveTagFromGroupItems(this);

	VariantClear(&m_Value);
}

CString CTag::GetPathName()
{
	if(m_pParent)
	{
		if(m_pParent->m_Branch.Find(this))
			return CString(m_pParent->GetPathName() + m_oName + _T(".") );
		else
			return CString(m_pParent->GetPathName() + m_oName);
	}
	else
		return _T("");
}

CTag*	CTag::FindTag(const CString& strTag)
{
	int iDelimiter = strTag.Find( _T('.') );
	if( iDelimiter < 1 )
   {
		POSITION pos = m_Leaf.GetHeadPosition();
		while( pos )
		{
			CTag* pTag = m_Leaf.GetNext( pos );
			if( pTag->m_oName.CompareNoCase( strTag ) == 0 )
				return pTag;
		}

		pos = m_Branch.GetHeadPosition();
		while(pos)
		{
			CTag* pTag = m_Branch.GetNext( pos );
			if( pTag->m_oName.CompareNoCase( strTag ) == 0 )
				return pTag;
		}
	}
	else
	{
		CString strName(strTag.Left(iDelimiter));
		POSITION pos = m_Branch.GetHeadPosition();
		while( pos )
		{
			CTag* pTag = m_Branch.GetNext( pos );
			if( pTag->m_oName.CompareNoCase( strName ) == 0 )
				return pTag->FindTag( strTag.Mid( iDelimiter+1 ) );
		}
	}
	return NULL;
}

CTag* CTag::AddBranch(	LPCTSTR	szName,
								BYTE		bCommand,
								CIO*		pIO,
								CDevice* pDevice)
{
	CTag* pTag=new CTag(szName,bCommand);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_bLeaf=FALSE;
	pTag->m_pParent=this;
	pTag->m_pIO=pIO;
	pTag->m_pDevice=pDevice;

	m_Branch.AddTail(pTag);
	return pTag;
}

CTag* CTag::AddBranch(	INT		iID,
								BYTE		bCommand,
								CIO*		pIO,
								CDevice* pDevice)
{
	CTag* pTag=new CTag(iID,bCommand);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_bLeaf=FALSE;
	pTag->m_pParent=this;
	pTag->m_pIO=pIO;
	pTag->m_pDevice=pDevice;
	
	m_Branch.AddTail(pTag);
	return pTag;
}


CTag* CTag::AddLeaf(	LPCTSTR	szName,
							BYTE		bCommand,
							BYTE		bSection,
							DWORD		dwItem,
							DWORD		dwAccessRights,
							VARTYPE	NativeType,
							CIO*		pIO,
							CDevice* pDevice)
{
	CTag* pTag=new CTag(szName,bCommand);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_bLeaf=TRUE;
	pTag->m_bSection=bSection;
	pTag->m_dwItem=dwItem;
	pTag->m_dwAccessRights=dwAccessRights;
	pTag->m_NativeType=NativeType;
	pTag->m_pParent=this;
	pTag->m_pIO=pIO;	
	pTag->m_pDevice=pDevice;

	m_Leaf.AddTail(pTag);
	g_pDeviceManager->AddTagToGroupItems(pTag);
	return pTag;
}

CTag* CTag::AddLeaf(	INT		iID,
							BYTE		bCommand,
							BYTE		bSection,
							DWORD		dwItem,
							DWORD		dwAccessRights,
							VARTYPE	NativeType,
							CIO*		pIO,
							CDevice* pDevice)
{
	CTag* pTag=new CTag(iID,bCommand);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_bLeaf=TRUE;
	pTag->m_bSection=bSection;
	pTag->m_dwItem=dwItem;
	pTag->m_dwAccessRights=dwAccessRights;
	pTag->m_NativeType=NativeType;
	pTag->m_pParent=this;
	pTag->m_pIO=pIO;
	pTag->m_pDevice=pDevice;
	m_Leaf.AddTail(pTag);
	g_pDeviceManager->AddTagToGroupItems(pTag);
	return pTag;
}

// CIO
UINT CIO::ScanThread(LPVOID lpIO)
{
	CIO* pIO = (CIO*) lpIO;

	pIO->Scan();

	AfxEndThread(0);

	return( 0 );
}

CIO::CIO(LONG					lIndex,
			LPCTSTR				szPort,
			DANLOAD_BAUD		Baud,
			DANLOAD_DATA_BITS	DataBits,
			DANLOAD_PARITY		Parity,
			DANLOAD_STOP_BITS	StopBits)
{
	m_hKillEvent=NULL;
	m_pScanThread=NULL;
	m_lIndex=lIndex;
	m_oPort=szPort;
	m_Baud=Baud;
	m_DataBits=DataBits;
	m_Parity=Parity;
	m_StopBits=StopBits;

	InitializeCriticalSection(&m_cs);

	m_dwUseCount=0;
	m_iInactivityCounter=0;

	m_hPort=INVALID_HANDLE_VALUE;
	m_bCommFailLogged=FALSE;
	m_bPortParametersChanged=FALSE;


	// Launch Scan Thread
	m_hKillEvent = CreateEvent( NULL,TRUE,FALSE,NULL );
	if(!m_hKillEvent)
		throw (CString(_T("IO: CreateEvent Error")));

	m_pScanThread = AfxBeginThread((AFX_THREADPROC) ScanThread,(LPVOID) this);
	if(!m_pScanThread)
		throw (CString(_T("DeviceManager: AfxBeginThread Error")));

	m_pScanThread->m_bAutoDelete=FALSE;
}

CIO::~CIO()
{
	if(m_hKillEvent
	&& m_pScanThread)
	{
		SetEvent(m_hKillEvent);
		WaitForSingleObject(m_pScanThread->m_hThread,INFINITE);
		CloseHandle(m_hKillEvent);
		m_hKillEvent=NULL;
		delete m_pScanThread;
		m_pScanThread=NULL;
	}

	if(m_TagScanList.GetCount())
	{
		m_TagScanList.RemoveAll();
		CString oError;
		oError.Format(_T("IO Error : tags orphined in scan list"));
		theApp.LogError(oError);
	}

	DeleteCriticalSection(&m_cs);

	if(m_hPort != INVALID_HANDLE_VALUE)
		CloseHandle(m_hPort);
}


HRESULT CIO::OpenComPort()
{
	CString oPort;

	oPort=_T("\\\\.\\")+m_oPort;
	m_hPort=CreateFile(	oPort,
								GENERIC_READ | GENERIC_WRITE,
								0,
								NULL,	
								OPEN_EXISTING,
								0,
								NULL);					  	

	if(m_hPort == INVALID_HANDLE_VALUE)
	{
		if(!m_bCommFailLogged)
		{
			DWORD dw = GetLastError();

			CString oError;
			oError.Format(_T("IO Error : CreateFile on : %s\nGetLastError()=%u"),m_oPort,dw);
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}

		return E_FAIL;
	}

	DCB Dcb;

	Dcb.DCBlength = sizeof( DCB );
	if(!GetCommState(m_hPort,&Dcb))
	{
		if(!m_bCommFailLogged)
		{
			CString oError;
			oError.Format(_T("IO Error : GetCommState on : %s"),m_oPort);
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}

		return E_FAIL;
	}

	switch(m_Baud)
	{
		case DANLOAD_BAUD_1200:
			Dcb.BaudRate=CBR_1200;
			break;
		case DANLOAD_BAUD_2400:
			Dcb.BaudRate=CBR_2400;
			break;
		case DANLOAD_BAUD_4800:
			Dcb.BaudRate=CBR_4800;
			break;
		case DANLOAD_BAUD_9600:
			Dcb.BaudRate=CBR_9600;
			break;
		case DANLOAD_BAUD_19200:
			Dcb.BaudRate=CBR_19200;
			break;
		case DANLOAD_BAUD_38400:
			Dcb.BaudRate=CBR_38400;
			break;
		default:
		{
			if(!m_bCommFailLogged)
			{
				CString oError;
				oError.Format(_T("IO Error : Invalid Baud Rate"));
				theApp.LogError(oError);
				m_bCommFailLogged=TRUE;
			}

			return E_FAIL;
		}
	}
	
	switch(m_DataBits)
	{
		case DATA_BITS_7:
			Dcb.ByteSize=7;
			break;
		case DATA_BITS_8:
			Dcb.ByteSize=8;
			break;
		default:
		{
			if(!m_bCommFailLogged)
			{
				CString oError;
				oError.Format(_T("IO Error : Invalid DataBits"));
				theApp.LogError(oError);
				m_bCommFailLogged=TRUE;
			}

			return E_FAIL;
		}
	}

	switch(m_Parity)
	{
		case DANLOAD_PARITY_NONE:
			Dcb.Parity=NOPARITY;
			break;
		case DANLOAD_PARITY_EVEN:
			Dcb.Parity=EVENPARITY;
			break;
		case DANLOAD_PARITY_ODD:
			Dcb.Parity=ODDPARITY;
			break;
		default:
		{
			if(!m_bCommFailLogged)
			{
				CString oError;
				oError.Format(_T("IO Error : Invalid Parity"));
				theApp.LogError(oError);
				m_bCommFailLogged=TRUE;
			}

			return E_FAIL;
		}
	}

	switch(m_StopBits)
	{
		case STOP_BITS_1:
			Dcb.StopBits=ONESTOPBIT;
			break;
		case STOP_BITS_2:
			Dcb.StopBits=TWOSTOPBITS;
			break;
		default:
		{
			if(!m_bCommFailLogged)
			{
				CString oError;
				oError.Format(_T("IO Error : Invalid StopBits"));
				theApp.LogError(oError);
				m_bCommFailLogged=TRUE;
			}

			return E_FAIL;
		}
	}

	Dcb.fOutxCtsFlow=FALSE;
	Dcb.fOutxDsrFlow=FALSE;
	Dcb.fOutX=FALSE;
	Dcb.fInX=FALSE;
	Dcb.fRtsControl=RTS_CONTROL_DISABLE;
	Dcb.fDtrControl=DTR_CONTROL_DISABLE;
	Dcb.fAbortOnError=FALSE;
	Dcb.EvtChar=0x00;

	if(!SetCommState(m_hPort,&Dcb))
	{
		if(!m_bCommFailLogged)
		{
			CString oError;
			oError.Format(_T("IO Error : SetCommState Error on : %s"),m_oPort);
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}

		return E_FAIL;
	}

	COMMTIMEOUTS	CommTimeouts;
	float	fTimeoutMult;
	fTimeoutMult = 1000 * 11 / (float)Dcb.BaudRate;
	CommTimeouts.ReadIntervalTimeout 			= 0;
	CommTimeouts.ReadTotalTimeoutMultiplier 	= (DWORD)fTimeoutMult*2;
	CommTimeouts.ReadTotalTimeoutConstant 		= 1000;
	CommTimeouts.WriteTotalTimeoutMultiplier 	= (DWORD)fTimeoutMult*2;
	CommTimeouts.WriteTotalTimeoutConstant 	= 500;
	if(!SetCommTimeouts(m_hPort,&CommTimeouts))
	{
		if(!m_bCommFailLogged)
		{
			CString oError;
			oError.Format(_T("IO Error : SetCommTimeouts Error on : %s"),m_oPort);
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}

		return E_FAIL;
	}

	m_bCommFailLogged=FALSE;
	m_bPortParametersChanged = FALSE;

	return S_OK;
}

WORD CIO::CRC(PBYTE pbBuffer,WORD wLength)
{
	const unsigned short crc16Table[256] = {
	0x0000,	0xc1c0,	0x81c1,	0x4001,	0x01c3,	0xc003,	0x8002,	0x41c2,
	0x01c6,	0xc006,	0x8007,	0x41c7,	0x0005,	0xc1c5,	0x81c4,	0x4004,
	0x01cc,	0xc00c,	0x800d,	0x41cd,	0x000f,	0xc1cf,	0x81ce,	0x400e,
	0x000a,	0xc1ca,	0x81cb,	0x400b,	0x01c9,	0xc009,	0x8008,	0x41c8,
	0x01d8,	0xc018,	0x8019,	0x41d9,	0x001b,	0xc1db,	0x81da,	0x401a,
	0x001e,	0xc1de,	0x81df,	0x401f,	0x01dd,	0xc01d,	0x801c,	0x41dc,
	0x0014,	0xc1d4,	0x81d5,	0x4015,	0x01d7,	0xc017,	0x8016,	0x41d6,
	0x01d2,	0xc012,	0x8013,	0x41d3,	0x0011,	0xc1d1,	0x81d0,	0x4010,
	0x01f0,	0xc030,	0x8031,	0x41f1,	0x0033,	0xc1f3,	0x81f2,	0x4032,
	0x0036,	0xc1f6,	0x81f7,	0x4037,	0x01f5,	0xc035,	0x8034,	0x41f4,
	0x003c,	0xc1fc,	0x81fd,	0x403d,	0x01ff,	0xc03f,	0x803e,	0x41fe,
	0x01fa,	0xc03a,	0x803b,	0x41fb,	0x0039,	0xc1f9,	0x81f8,	0x4038,
	0x0028,	0xc1e8,	0x81e9,	0x4029,	0x01eb,	0xc02b,	0x802a,	0x41ea,
	0x01ee,	0xc02e,	0x802f,	0x41ef,	0x002d,	0xc1ed,	0x81ec,	0x402c,
	0x01e4,	0xc024,	0x8025,	0x41e5,	0x0027,	0xc1e7,	0x81e6,	0x4026,
	0x0022,	0xc1e2,	0x81e3,	0x4023,	0x01e1,	0xc021,	0x8020,	0x41e0,
	0x01a0,	0xc060,	0x8061,	0x41a1,	0x0063,	0xc1a3,	0x81a2,	0x4062,	
	0x0066,	0xc1a6,	0x81a7,	0x4067,	0x01a5,	0xc065,	0x8064,	0x41a4,	
	0x006c,	0xc1ac,	0x81ad,	0x406d,	0x01af,	0xc06f,	0x806e,	0x41ae,	
	0x01aa,	0xc06a,	0x806b,	0x41ab,	0x0069,	0xc1a9,	0x81a8,	0x4068,	
	0x0078,	0xc1b8,	0x81b9,	0x4079,	0x01bb,	0xc07b,	0x807a,	0x41ba,	
	0x01be,	0xc07e,	0x807f,	0x41bf,	0x007d,	0xc1bd,	0x81bc,	0x407c,	
	0x01b4,	0xc074,	0x8075,	0x41b5,	0x0077,	0xc1b7,	0x81b6,	0x4076,	
	0x0072,	0xc1b2,	0x81b3,	0x4073,	0x01b1,	0xc071,	0x8070,	0x41b0,	
	0x0050,	0xc190,	0x8191,	0x4051,	0x0193,	0xc053,	0x8052,	0x4192,	
	0x0196,	0xc056,	0x8057,	0x4197,	0x0055,	0xc195,	0x8194,	0x4054,	
	0x019c,	0xc05c,	0x805d,	0x419d,	0x005f,	0xc19f,	0x819e,	0x405e,	
	0x005a,	0xc19a,	0x819b,	0x405b,	0x0199,	0xc059,	0x8058,	0x4198,	
	0x0188,	0xc048,	0x8049,	0x4189,	0x004b,	0xc18b,	0x818a,	0x404a,	
	0x004e,	0xc18e,	0x818f,	0x404f,	0x018d,	0xc04d,	0x804c,	0x418c,	
	0x0044,	0xc184,	0x8185,	0x4045,	0x0187,	0xc047,	0x8046,	0x4186,	
	0x0182,	0xc042,	0x8043,	0x4183,	0x0041,	0xc181,	0x8180,	0x4040};	



	WORD wCRC=0xffff;
	for(WORD wIndex=0;wIndex < wLength;wIndex++)
	{
		WORD w=wCRC << 8;
		WORD x=wCRC >> 8;
		WORD y=pbBuffer[wIndex] ^ x;
		wCRC=crc16Table[y] ^ w ;
	}

	return ((wCRC & 0x00FF) << 8) | ((wCRC & 0xFF00) >> 8);
}

VARENUM CIO::DanLoadDataType(short sCode)
{
	if(sCode == 480)
		return VT_UI2;

	// Pass Codes
	else if(sCode >= 1 && sCode <= 16 && (sCode-1) % 3 == 0)
		return VT_UI4;

	// Additive Ratio Quantity
	else if(sCode >= 140 && sCode <= 165 && (sCode-140) % 5 == 0)
		return VT_UI2;

	// Additive Volume/pulse or K Factor
	else if(sCode >= 142 && sCode <= 167 && (sCode-142) % 5 == 0)
		return VT_UI4; 

	// Additive Volume per 1000 units of Product
	else if(sCode >= 143 && sCode <= 168 && (sCode-143) % 5 == 0)
		return VT_UI4; 

	// Recipes
	else if(sCode >= 481 && sCode <= 660)
	{
		// Names
		if((sCode-481) % 6 == 0)
			return VT_BSTR;

		// Sequence/Low Proportion (nnnn)
		else if((sCode-486) % 6 == 0)
			return VT_UI8;

		// Percentages
		else
			return VT_UI2;
	}

	// Backup Density
	else if(sCode >= 457  && sCode <= 463 && (sCode-457) % 2 == 0)
		return VT_UI4;

	else
		return VT_EMPTY;
}


HRESULT CIO::PrepareRequest(CTag* pTag)
{

	m_bXmtBuffer[0]=pTag->m_pDevice->m_bAddress;

	CDevice* pDevice=pTag->m_pDevice;
	if(pDevice == NULL)
		return E_FAIL;

	if(pDevice->m_bLastFunctionCode == 0x41)
		pDevice->m_bLastFunctionCode=0x42;
	else
		pDevice->m_bLastFunctionCode=0x41;

	m_bXmtBuffer[1]=pDevice->m_bLastFunctionCode;

	switch (pTag->m_bCommand)
	{
		case PROMPT_RECIPE_CMD:
		{
			m_bXmtBuffer[2]=4;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			ZeroMemory(&m_bXmtBuffer[4],sizeof(m_bXmtBuffer)-4);
	
			// Command value is timeout in seconds
			sscanf(oString,"%hd",&m_bXmtBuffer[4]);
			*((PWORD) &m_bXmtBuffer[6])=CRC(m_bXmtBuffer,6);
			m_wXmtLength=8;
			break;
		}

		case PROMPT_PRESET_VOLUME_CMD:
		{
			m_bXmtBuffer[2]=12;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			ZeroMemory(&m_bXmtBuffer[4],sizeof(m_bXmtBuffer)-4);

			// command value is suggested setpoint, maximum setpoint, and timeout in seconds
			sscanf(oString,"%d %d %hd",&m_bXmtBuffer[4],&m_bXmtBuffer[8],&m_bXmtBuffer[12]);
			*((PWORD) &m_bXmtBuffer[14])=CRC(m_bXmtBuffer,14);
			m_wXmtLength=16;
			break;
		} 

		case REQUEST_COMPONENT_VALUES_CMD:
		{
			m_bXmtBuffer[2]=4;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			short sComponent;
			swscanf(pTag->m_pParent->m_oName,_T("%hd"),&sComponent);
			*((PSHORT) &m_bXmtBuffer[4])=sComponent;
			*((PWORD) &m_bXmtBuffer[6])=CRC(m_bXmtBuffer,6);
			m_wXmtLength=8;
			break;
		}

		case REQUEST_STATUS_CMD:
		case ADDITIVE_TOTALIZERS_CMD:
		case COMPONENT_TOTALIZERS_CMD:
		case REQUEST_KEYPAD_DATA_CMD:
		case REQUEST_SELECTED_RECIPE_CMD:
		case REQUEST_PRESET_VOLUME_CMD:
		case TIMEOUT_OPERATION_CMD:
		case START_COMMUNICATIONS_CMD:
		case CLEAR_DISPLAY_CMD:
		case END_BATCH_CMD:
		case LAST_KEY_PRESSED_CMD:
		{
			m_bXmtBuffer[2]=2;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			*((PWORD) &m_bXmtBuffer[4])=CRC(m_bXmtBuffer,4);
			m_wXmtLength=6;
			break;
		}

		case AUTHORIZE_TRANSACTION_CMD:
		{
			m_bXmtBuffer[2]=8;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			ZeroMemory(&m_bXmtBuffer[4],sizeof(m_bXmtBuffer)-4);

			// Tag value will be recipe followed by swing arm position
			sscanf(oString,"%hd %hd",&m_bXmtBuffer[4],&m_bXmtBuffer[8]);

			// Additive Selection by program code 136 which should be recipe
			m_bXmtBuffer[6]=1;

			*((PWORD) &m_bXmtBuffer[10])=CRC(m_bXmtBuffer,10);
			m_wXmtLength=12;
			break;
		}

		case END_TRANSACTION_CMD:
		{
			m_bXmtBuffer[2]=3;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			ZeroMemory(&m_bXmtBuffer[4],sizeof(m_bXmtBuffer)-4);

			// Tag value will be swing arm position
			sscanf(oString,"%hd",&m_bXmtBuffer[4]);

			*((PWORD) &m_bXmtBuffer[5])=CRC(m_bXmtBuffer,5);
			m_wXmtLength=7;
			break;
		}


		case AUTHORIZE_BATCH_CMD:
		{
			short sNumberOfComponents=pTag->m_pDevice->m_sNumberOfComponents;
			m_bXmtBuffer[2]=10+8*sNumberOfComponents;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			ZeroMemory(&m_bXmtBuffer[4],sizeof(m_bXmtBuffer)-4);

			// Tag value is preset followed by timeout
			sscanf(oString,"%d %hd",&m_bXmtBuffer[4],&m_bXmtBuffer[10]);

			*((PSHORT) &m_bXmtBuffer[8])=sNumberOfComponents;

			*((PWORD) &m_bXmtBuffer[2+m_bXmtBuffer[2]])=CRC(m_bXmtBuffer,2+m_bXmtBuffer[2]);
			m_wXmtLength=4+m_bXmtBuffer[2];
			break;
		}


		case DISPLAY_MESSAGE_CMD:
		{
			m_bXmtBuffer[2]=136;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);

			// OPC Value is of the form Prompt Width, Control, Timeout, Message
			//
			// Prompt Width is a number from 0 to 8
			//
			// Control is 0/1 -> Echo/Non-echo
			//
			// Timeout is a number -1 / 0 / > 0
			//		-1 Timeout indicates use timeout value configured in program location 036
			//		0 Timeout indicates no timeout
			//		> 0 indicates supplied timeout in seconds
			//
			// Message is a maximum of 129 characters null terminated
			//
			ZeroMemory(&m_bXmtBuffer[4],sizeof(m_bXmtBuffer)-4);
			int iFields=sscanf(oString,"%1hd %1hd %5hd %128[a-z A-Z0-9.:()=,?-!]",&m_bXmtBuffer[133],&m_bXmtBuffer[135],&m_bXmtBuffer[136],&m_bXmtBuffer[4]);
			if(iFields != 4)
				return E_FAIL;

			*((PWORD) &m_bXmtBuffer[138])=CRC(m_bXmtBuffer,138);
			m_wXmtLength=140;
			break;
		}

		case CHANGE_OPERATING_MODE_CMD:
		{
			m_bXmtBuffer[2]=3;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			m_bXmtBuffer[4]=pTag->m_Value.bVal;
			*((PWORD) &m_bXmtBuffer[5])=CRC(m_bXmtBuffer,5);
			m_wXmtLength=7;
			break;
		}

		case SET_DATE_AND_TIME_CMD:
		{
			CString	oString((LPTSTR) pTag->m_Value.bstrVal);

			int iMonth=0;
			int iDay=0;
			int iYear=0;
			int iHour=0;
			int iMinute=0;
			int iSecond=0;

			int iFields=swscanf(oString,_T("%2d/%2d/%2d %2d:%2d:%2d"),&iMonth,&iDay,&iYear,&iHour,&iMinute,&iSecond);
			if(iFields < 5)
				return E_FAIL;

			m_bXmtBuffer[2]=8;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			m_bXmtBuffer[4]=LOBYTE(iYear);
			m_bXmtBuffer[5]=LOBYTE(iMonth);
			m_bXmtBuffer[6]=LOBYTE(iDay);
			m_bXmtBuffer[7]=LOBYTE(iHour);
			m_bXmtBuffer[8]=LOBYTE(iMinute);
			m_bXmtBuffer[9]=LOBYTE(iSecond);
			*((PWORD) &m_bXmtBuffer[10])=CRC(m_bXmtBuffer,10);
			m_wXmtLength=12;
			break;
		}

		case RESET_PRIMARY_ALARMS_CMD:
		{
			m_bXmtBuffer[2]=11;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			m_bXmtBuffer[4]=0xff;
			m_bXmtBuffer[5]=0xff;
			m_bXmtBuffer[6]=0xff;
			m_bXmtBuffer[7]=0xff;
			m_bXmtBuffer[8]=0xff;
			m_bXmtBuffer[9]=0xff;
			m_bXmtBuffer[10]=0xff;
			m_bXmtBuffer[11]=0xff;
			m_bXmtBuffer[12]=0xff;
			*((PWORD) &m_bXmtBuffer[13])=CRC(m_bXmtBuffer,13);
			m_wXmtLength=15;
			break;

		}

		case READ_INPUT_CMD:
		{
			m_bXmtBuffer[2]=6;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			m_bXmtBuffer[4]=pTag->m_bSection;
			m_bXmtBuffer[5]=0;
			m_bXmtBuffer[6]=LOBYTE(LOWORD(pTag->m_dwItem));
			m_bXmtBuffer[7]=0;
			*((PWORD) &m_bXmtBuffer[8])=CRC(m_bXmtBuffer,8);
			m_wXmtLength=10;
			break;
		}

		case WRITE_OUTPUT_CMD:
		{
			m_bXmtBuffer[2]=6;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			m_bXmtBuffer[4]=pTag->m_bSection;
			m_bXmtBuffer[5]=0;
			m_bXmtBuffer[6]=LOBYTE(LOWORD(pTag->m_dwItem));
			m_bXmtBuffer[7]=0;
			if(pTag->m_Value.boolVal == VARIANT_TRUE)
				m_bXmtBuffer[8]=1;
			else
				m_bXmtBuffer[8]=0;
			m_bXmtBuffer[9]=0;
			*((PWORD) &m_bXmtBuffer[10])=CRC(m_bXmtBuffer,10);
			m_wXmtLength=12;
			break;
		}

		case REQUEST_PROGRAM_CODE_VALUES_AND_ATTRIBUTES_CMD:
		{
			CString	oString((LPTSTR) pTag->m_Value.bstrVal);
			short sCode;

			int iFields=swscanf(oString,_T("%3hd"),&sCode);
			if(iFields != 1)
				return E_FAIL;

			// Only select Program Codes are supported
			if(VT_EMPTY == DanLoadDataType(sCode))
				return E_FAIL;

			m_bXmtBuffer[2]=6;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			*((PSHORT) &m_bXmtBuffer[4])=sCode;
			*((PSHORT) &m_bXmtBuffer[6])=sCode;
			*((PWORD) &m_bXmtBuffer[8])=CRC(m_bXmtBuffer,8);
			m_wXmtLength=10;
			break;
		} 

		case SET_PROGRAM_CODE_VALUE_CMD:
		{
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);

			short sCode;
			char szValue[256];

			int iFields=sscanf(oString,"%3hd %255[a-z A-Z0-9.:()]",&sCode,szValue);
			if(iFields != 2)
				return E_FAIL;

			// Only select Program Codes are supported
			if(VT_EMPTY == DanLoadDataType(sCode))
				return E_FAIL;

			ZeroMemory(&m_bXmtBuffer[6],sizeof(m_bXmtBuffer)-6);

			int iLength=0;

			if(VT_BSTR == DanLoadDataType(sCode))
			{
				// Recipe Names
				if(sCode >= 481 && sCode <= 660 && (sCode-481) % 6 == 0)
				{
					iLength=17;
					strncpy((LPSTR) &m_bXmtBuffer[6],szValue,iLength);
				}
				else
					return E_FAIL;
			}

			else if(VT_UI2 == DanLoadDataType(sCode))
			{
				iLength=2;

				if(!sscanf(szValue,"%hd",&m_bXmtBuffer[6]))
					return E_FAIL;
			}

			else if(VT_UI4 == DanLoadDataType(sCode))
			{
				iLength=4;

				if(!sscanf(szValue,"%d",&m_bXmtBuffer[6]))
					return E_FAIL;
			}

			else if(VT_UI8 == DanLoadDataType(sCode))
			{
				iLength=8;

				// Recipes Sequence/Low Proportion (nnnn)
 				if(sCode >= 481 && sCode <= 660 && (sCode-486) % 6 == 0)
				{
					if(4 != sscanf(szValue,"%1hd%1hd%1hd%1hd",&m_bXmtBuffer[6],&m_bXmtBuffer[8],&m_bXmtBuffer[10],&m_bXmtBuffer[12]))
						return E_FAIL;
				}
				else
					return E_FAIL;
			}

			m_bXmtBuffer[2]=4+iLength;
			m_bXmtBuffer[3]=pTag->m_bCommand;
			*((PSHORT) &m_bXmtBuffer[4])=sCode;
			*((PWORD) &m_bXmtBuffer[6+iLength])=CRC(m_bXmtBuffer,6+iLength);
			m_wXmtLength=8+iLength;

			break;
		}

		default:
			return E_FAIL;
	}	

	return S_OK;
}

void CIO::UpdateAdditiveTotalizerTags(CTag* pParent,WORD wQuality)
{
	POSITION pos=pParent->m_Branch.GetHeadPosition();
	while(pos)
	{
		CTag* pTag=pParent->m_Branch.GetNext(pos);

		if(pTag->m_bCommand != ADDITIVE_TOTALIZERS_CMD)
			continue;

		UpdateAdditiveTotalizerTags(pTag,wQuality);
	}

	pos=pParent->m_Leaf.GetHeadPosition();
	while(pos)
	{
		CTag* pTag=pParent->m_Leaf.GetNext(pos);

		if(pTag->m_bCommand != ADDITIVE_TOTALIZERS_CMD)
			continue;

		if(wQuality == OPC_QUALITY_GOOD)
		{
			short NumberOfAdditives=*((SHORT*) &m_bRcvBuffer[4]);
			if(pTag->m_bSection < NumberOfAdditives)
			{
				pTag->m_Value.vt=VT_I4;
				pTag->m_Value.lVal=*((long*) &m_bRcvBuffer[6+pTag->m_bSection*4]);
				pTag->m_wQuality=wQuality;
			}
			else
				pTag->m_wQuality=OPC_QUALITY_NOT_CONNECTED; 
		}
		else
			pTag->m_wQuality=wQuality;

		CoFileTimeNow(&pTag->m_Timestamp);
		pTag->m_bCurrent=TRUE;
	}
}

void CIO::UpdateComponentValueTags(CTag* pParent,WORD wQuality)
{
	POSITION pos=pParent->m_Branch.GetHeadPosition();
	while(pos)
	{
		CTag* pTag=pParent->m_Branch.GetNext(pos);

		if(pTag->m_bCommand != REQUEST_COMPONENT_VALUES_CMD)
			continue;

		UpdateComponentValueTags(pTag,wQuality);
	}

	pos=pParent->m_Leaf.GetHeadPosition();
	while(pos)
	{
		CTag* pTag=pParent->m_Leaf.GetNext(pos);

		if(pTag->m_bCommand != REQUEST_COMPONENT_VALUES_CMD)
			continue;

		if(wQuality == OPC_QUALITY_GOOD)
		{
			if(pTag->m_NativeType == VT_I4)
			{
				pTag->m_Value.vt=VT_I4;
				pTag->m_Value.lVal=*((long*) &m_bRcvBuffer[6+pTag->m_bSection]);
			}
			else if(pTag->m_NativeType == VT_I2)
			{
				pTag->m_Value.vt=VT_I2;
				pTag->m_Value.iVal=*((short*) &m_bRcvBuffer[6+pTag->m_bSection]);
			}
		}

		pTag->m_wQuality=wQuality;

		CoFileTimeNow(&pTag->m_Timestamp);
		pTag->m_bCurrent=TRUE;
	}
}




void CIO::UpdateComponentTotalizerTags(CTag* pParent,WORD wQuality)
{
	POSITION pos=pParent->m_Branch.GetHeadPosition();
	while(pos)
	{
		CTag* pTag=pParent->m_Branch.GetNext(pos);

		if(pTag->m_bCommand != COMPONENT_TOTALIZERS_CMD)
			continue;

		UpdateComponentTotalizerTags(pTag,wQuality);
	}

	pos=pParent->m_Leaf.GetHeadPosition();
	while(pos)
	{
		CTag* pTag=pParent->m_Leaf.GetNext(pos);

		if(pTag->m_bCommand != COMPONENT_TOTALIZERS_CMD)
			continue;

		if(wQuality == OPC_QUALITY_GOOD)
		{
			short NumberOfComponents=*((SHORT*) &m_bRcvBuffer[4]);
			if(pTag->m_bSection < NumberOfComponents*2)
			{
				pTag->m_Value.vt=VT_I4;
				pTag->m_Value.lVal=*((long*) &m_bRcvBuffer[6+pTag->m_bSection*4]);
				pTag->m_wQuality=wQuality;
			}
			else
				pTag->m_wQuality=OPC_QUALITY_NOT_CONNECTED; 
		}
		else
			pTag->m_wQuality=wQuality;

		CoFileTimeNow(&pTag->m_Timestamp);
		pTag->m_bCurrent=TRUE;
	}
}

void CIO::UpdateStatusTags(CTag* pParent,WORD wQuality)
{
	POSITION pos=pParent->m_Branch.GetHeadPosition();
	while(pos)
	{
		CTag* pTag=pParent->m_Branch.GetNext(pos);

		if(pTag->m_bCommand != REQUEST_STATUS_CMD)
			continue;

		UpdateStatusTags(pTag,wQuality);
	}

	pos=pParent->m_Leaf.GetHeadPosition();
	while(pos)
	{
		CTag* pTag=pParent->m_Leaf.GetNext(pos);

		if(pTag->m_bCommand != REQUEST_STATUS_CMD)
			continue;

		if(wQuality == OPC_QUALITY_GOOD)
		{
			switch(pTag->m_NativeType)
			{
				case VT_BOOL:
				{
					BOOL boolVal=((m_bRcvBuffer[4+pTag->m_bSection]) & (1 << pTag->m_dwItem)) ? VARIANT_TRUE : VARIANT_FALSE;
					pTag->m_Value.vt=VT_BOOL;
					if(pTag->m_Value.boolVal != boolVal)
					{
						pTag->m_Value.boolVal=((m_bRcvBuffer[4+pTag->m_bSection]) & (1 << pTag->m_dwItem)) ? VARIANT_TRUE : VARIANT_FALSE;
						pTag->m_dwUpdateSequence++;
					}
					break;
				}

				case VT_UI1:
				{
					pTag->m_Value.vt=VT_UI1;
					BYTE bVal=m_bRcvBuffer[4+pTag->m_bSection];
					if(pTag->m_Value.bVal != bVal)
					{
						pTag->m_Value.bVal=bVal;
						pTag->m_dwUpdateSequence++;
					}
					break;
				}

				case VT_I4:
				{
					pTag->m_Value.vt=VT_I4;
					long lVal=*((long*) &m_bRcvBuffer[4+pTag->m_bSection]);
					if(pTag->m_Value.lVal != lVal)
					{
						pTag->m_Value.lVal=lVal;
						pTag->m_dwUpdateSequence++;
					}
					break;
				}
			}
		}

		pTag->m_wQuality=wQuality;
		CoFileTimeNow(&pTag->m_Timestamp);
		pTag->m_bCurrent=TRUE;
	}
}


HRESULT CIO::ProcessResponse(CTag* pTag,HRESULT hr)
{
	switch (pTag->m_bCommand)
	{
		case REQUEST_KEYPAD_DATA_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}

				else if(m_bRcvBuffer[2] != 6
				|| m_bRcvBuffer[3] != REQUEST_KEYPAD_DATA_CMD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_I4;
					pTag->m_Value.lVal=*((PLONG) &m_bRcvBuffer[4]);
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

			break;
		}

		case AUTHORIZE_TRANSACTION_CMD:
		case END_TRANSACTION_CMD:
		case AUTHORIZE_BATCH_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}

				else if(m_bRcvBuffer[2] != 4
				|| m_bRcvBuffer[3] != pTag->m_bCommand)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					CString strData;
					strData.Format(_T("%hd"),*((short*)&m_bRcvBuffer[4]));
					pTag->m_Value=strData;

					if(pTag->m_bCommand == AUTHORIZE_BATCH_CMD)
					{
						CTag* pBatchAborted=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Status.Batch Aborted")).GetBuffer());
						if(pBatchAborted != NULL)
						{
							pBatchAborted->m_wQuality=OPC_QUALITY_GOOD;
							pBatchAborted->m_Value.vt=VT_BOOL;
							pBatchAborted->m_Value.boolVal=VARIANT_FALSE;
							CoFileTimeNow(&pBatchAborted->m_Timestamp);
							pBatchAborted->m_dwUpdateSequence++;
						}
					}
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

			break;
		}

		case REQUEST_PRESET_VOLUME_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}

				else if(m_bRcvBuffer[2] != 6
				|| m_bRcvBuffer[3] != REQUEST_PRESET_VOLUME_CMD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_I4;
					pTag->m_Value.lVal=*((PLONG) &m_bRcvBuffer[4]);
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

			break;
		}


		case LAST_KEY_PRESSED_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}

				else if(m_bRcvBuffer[2] != 3
				|| m_bRcvBuffer[3] != LAST_KEY_PRESSED_CMD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_UI1;
					pTag->m_Value.bVal=m_bRcvBuffer[4];
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

			break;
		}

		case REQUEST_SELECTED_RECIPE_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}

				else if(m_bRcvBuffer[2] != 4
				|| m_bRcvBuffer[3] != REQUEST_SELECTED_RECIPE_CMD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_I2;
					pTag->m_Value.iVal=*((PSHORT) &m_bRcvBuffer[4]);
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

			break;
		}

		case REQUEST_STATUS_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else if(m_bRcvBuffer[2] != 27
				|| m_bRcvBuffer[3] != REQUEST_STATUS_CMD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
					pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;


			// Get the Device Tag
			CTag*	pDevice=pTag->m_pParent;
			while(pDevice->m_pParent->m_pParent != NULL)
				pDevice=pDevice->m_pParent;

			UpdateStatusTags(pDevice,pTag->m_wQuality);
			break;
		}

		case REQUEST_COMPONENT_VALUES_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else if(m_bRcvBuffer[2] != 34
				|| m_bRcvBuffer[3] != REQUEST_COMPONENT_VALUES_CMD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
					pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

			UpdateComponentValueTags(pTag->m_pParent,pTag->m_wQuality);
			break;
		}


		case COMPONENT_TOTALIZERS_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else if(m_bRcvBuffer[3] != COMPONENT_TOTALIZERS_CMD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
					pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;


			// Get the Device Tag
			CTag*	pDevice=pTag->m_pParent;
			while(pDevice->m_pParent->m_pParent != NULL)
				pDevice=pDevice->m_pParent;

			UpdateComponentTotalizerTags(pDevice,pTag->m_wQuality);
			break;
		}

		case ADDITIVE_TOTALIZERS_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else if(m_bRcvBuffer[3] != ADDITIVE_TOTALIZERS_CMD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
					pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;


			// Get the Device Tag
			CTag*	pDevice=pTag->m_pParent;
			while(pDevice->m_pParent->m_pParent != NULL)
				pDevice=pDevice->m_pParent;

			UpdateAdditiveTotalizerTags(pDevice,pTag->m_wQuality);
			break;
		}



		case START_COMMUNICATIONS_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[3] != START_COMMUNICATIONS_CMD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
				{
					CDevice* pDevice=pTag->m_pDevice;
					pDevice->m_sNumberOfMeters			=	*((PWORD) &m_bRcvBuffer[4]);
					pDevice->m_sNumberOfComponents	=	*((PWORD) &m_bRcvBuffer[6]);
					pDevice->m_sNumberOfValves			=	*((PWORD) &m_bRcvBuffer[8]);
					pDevice->m_sNumberOfFactors		=	*((PWORD) &m_bRcvBuffer[10]);
					pDevice->m_sNumberOfRecipes		=	*((PWORD) &m_bRcvBuffer[12]);
					pDevice->m_sNumberOfAdditives		=	*((PWORD) &m_bRcvBuffer[14]);
					pDevice->m_bTempUnits				=	*((PBYTE) &m_bRcvBuffer[16]);
				}
			}
			break;
		}

		case PROMPT_RECIPE_CMD:
		case PROMPT_PRESET_VOLUME_CMD:
		case TIMEOUT_OPERATION_CMD:
		case DISPLAY_MESSAGE_CMD:
		case CHANGE_OPERATING_MODE_CMD:
		case SET_DATE_AND_TIME_CMD:
		case RESET_PRIMARY_ALARMS_CMD:
		case SET_PROGRAM_CODE_VALUE_CMD:
		case CLEAR_DISPLAY_CMD:
		case END_BATCH_CMD:
		case WRITE_OUTPUT_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					return E_FAIL;
				}
				else if(m_bRcvBuffer[2] != 2
				|| m_bRcvBuffer[3] != pTag->m_bCommand)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;

					// Display Message Clears KeyPadDataAvailable immediately
					// but system may not see change of state sof force one
					if(pTag->m_bCommand == DISPLAY_MESSAGE_CMD)
					{
						CTag* pKeypadDataPending=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Status.Keypad Data Available")).GetBuffer());
						if(pKeypadDataPending != NULL)
						{
							pKeypadDataPending->m_wQuality=OPC_QUALITY_GOOD;
							pKeypadDataPending->m_Value.vt=VT_BOOL;
							pKeypadDataPending->m_Value.boolVal=VARIANT_FALSE;
							CoFileTimeNow(&pKeypadDataPending->m_Timestamp);
							pKeypadDataPending->m_dwUpdateSequence++;
						}

						CTag* pOperationTimedout=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Status.Operation Timedout")).GetBuffer());
						if(pOperationTimedout != NULL)
						{
							pOperationTimedout->m_wQuality=OPC_QUALITY_GOOD;
							pOperationTimedout->m_Value.vt=VT_BOOL;
							pOperationTimedout->m_Value.boolVal=VARIANT_FALSE;
							CoFileTimeNow(&pOperationTimedout->m_Timestamp);
							pOperationTimedout->m_dwUpdateSequence++;
						}

						g_pDeviceManager->UpdateGroups();
					}
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

			break;
		}

		case READ_INPUT_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					return E_FAIL;
				}
				else if(m_bRcvBuffer[3] != pTag->m_bCommand)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;

					if(VT_I2 == pTag->m_NativeType)
					{
						pTag->m_Value.vt=VT_I2;
						pTag->m_Value.iVal=*((short*)&m_bRcvBuffer[4]);
					}

					else if(VT_BOOL == pTag->m_NativeType)
					{
						pTag->m_Value.vt=VT_BOOL;
						if(m_bRcvBuffer[4] == 0)
							pTag->m_Value.boolVal=VARIANT_FALSE;
						else
							pTag->m_Value.boolVal=VARIANT_TRUE;
					}
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

			break;
		}
	
		case REQUEST_PROGRAM_CODE_VALUES_AND_ATTRIBUTES_CMD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[1] == 0xc1
				|| m_bRcvBuffer[1] == 0xc2)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					return E_FAIL;
				}
				else if(m_bRcvBuffer[3] != pTag->m_bCommand)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					short sCode=*((PSHORT) &m_bXmtBuffer[4]);
					if(VT_BSTR == DanLoadDataType(sCode))
					{
						CString strData((LPSTR) &m_bRcvBuffer[5]);
						pTag->m_Value=strData;
					}

					else if(VT_UI2 == DanLoadDataType(sCode))
					{
						CString strData;
						strData.Format(_T("%hd"),*((short*)&m_bRcvBuffer[5]));
						pTag->m_Value=strData;
					}

					else if(VT_UI4 == DanLoadDataType(sCode))
					{
						long lValue=*((long*) &m_bRcvBuffer[5]);
						CString strData;
						strData.Format(_T("%d"),lValue);
						pTag->m_Value=strData;
					}

					else if(VT_UI8 == DanLoadDataType(sCode))
					{
						// Recipes Sequence/Low Proportion (nnnn)
 						if(sCode >= 481 && sCode <= 660 && (sCode-486) % 6 == 0)
						{
							CString strData;
							strData.Format(_T("%1hd%1hd%1hd%1hd"),*((PSHORT) &m_bRcvBuffer[5]),*((PSHORT) &m_bRcvBuffer[7]),*((PSHORT) &m_bRcvBuffer[9]),*((PSHORT) &m_bRcvBuffer[11]));
							pTag->m_Value=strData;
						}
					}
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

			break;
		}

		default:
		{
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			return E_FAIL;
		}
	}

	return hr;
}


void CIO::ReportError(CTag* pTag)
{
	CString oCode;

	switch(m_bRcvBuffer[4])
	{
		case 0x00:
			oCode="Invalid Command Code";
			break;
		case 0x01:
			oCode="Passcode entry in progress";
			break;
		case 0x02:
			oCode="No transaction ended";
			break;
		case 0x03:
			oCode="Response data field to long";
			break;
		case 0x04:
			oCode="Program code value is Weights and Measures";
			break;
		case 0x05:
			oCode="Reserved";
			break;
		case 0x06:
			oCode="No batch in progress";
			break;
		case 0x07:
			oCode="No transaction in progress";
			break;
		case 0x08:
			oCode="Batch in progress";
			break;
		case 0x09:
			oCode="Transaction in progress";
			break;
		case 0x0a:
			oCode="Primary alarm is active";
			break;
		case 0x0b:
			oCode="Batch authorized";
			break;
		case 0x0c:
			oCode="Transaction authorized";
			break;
		case 0x0e:
			oCode="No keypad data available";
			break;
		case 0x0f:
			oCode="Component not available";
			break;
		case 0x10:
			oCode="Additive not available";
			break;
		case 0x11:
			oCode="Program code value is read only";
			break;
		case 0x12:
			oCode="Status not set or cannot be reset";
			break;
		case 0x13:
			oCode="No additives configured";
			break;
		case 0x14:
			oCode="No batch authorized";
			break;
		case 0x15:
			oCode="Operating mode is manual";
			break;
		case 0x16:
			oCode="No preset volume entered";
			break;
		case 0x17:
			oCode="No recipe selected";
			break;
		case 0x18:
			oCode="No additive selection made";
			break;
		case 0x19:
			oCode="Data items not entered";
			break;
		case 0x1a:
			return; // "No key pressed";
		case 0x1b:
			oCode="Diagnostic not started";
			break;
		case 0x1c:
			oCode="Diagnostic running";
			break;
		case 0x1d:
			oCode="Transaction not on file";
			break;
		case 0x1e:
			oCode="Batch not on file";
			break;
		case 0x20:
			oCode="Number of recipes < 2";
			break;
		case 0x22:
			oCode="No transaction authorized";
			break;
		case 0x24:
			oCode="Keypad and display lockout";
			break;
		case 0x25:
			oCode="No batch stopped";
			break;
		case 0x26:
			oCode="No batch ended";
			break;
		case 0x27:
			oCode="Operating mode cannot be changed";
			break;
		case 0x40:
			oCode="Invalid recipe";
			break;
		case 0x41:
			oCode="Invalid meter number";
			break;
		case 0x42:
			oCode="Invalid component number";
			break;
		case 0x43:
			oCode="Invalid transaction sequence number";
			break;
		case 0x44:
			oCode="Invalid program code";
			break;
		case 0x45:
			oCode="Invalid program code value";
			break;
		case 0x46:
			oCode="Invalid CPU number";
			break;
		case 0x47:
			oCode="Invalid number of components";
			break;
		case 0x48:
			oCode="Invalid number of data items";
			break;
		case 0x49:
			oCode="Invalid swing-arm side";
			break;
		case 0x4a:
			oCode="Invalid I/O point type";
			break;
		case 0x4b:
			oCode="Invalid I/O point number";
			break;
		case 0x4c:
			oCode="Invalid output value";
			break;
		case 0x4d:
			oCode="Invalid operating mode";
			break;
		case 0x4e:
			oCode="Invalid additive selection method";
			break;
		case 0x4f:
			oCode="Invalid preset volume";
			break;
		case 0x50:
			oCode="Invalid date";
			break;
		case 0x51:
			oCode="Invalid time";
			break;
		case 0x52:
			oCode="Invalid data code";
			break;
		case 0x53:
			oCode="Invalid override maximum preset volume";
			break;
		case 0x54:
			oCode="Invalid board type";
			break;
		case 0x55:
			oCode="Invalid bit #";
			break;
		default:
			oCode.Format(_T("Reserved error code: %d"),m_bRcvBuffer[4]);
			break;
	}

	CString oError;
	oError.Format(_T("IO Error : %s for %s"),oCode,pTag->GetPathName());
	theApp.LogError(oError);
}

HRESULT CIO::PerformIO(CTag* pTag)
{
	if ( m_bPortParametersChanged )
	{
		CloseHandle(m_hPort);
		m_hPort=INVALID_HANDLE_VALUE;
		m_bPortParametersChanged=FALSE;
	}
	
	if ( m_hPort == INVALID_HANDLE_VALUE )
	{
		HRESULT hr=OpenComPort();
		if(FAILED(hr))
		{
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			SignalCommunicationsFailure(pTag);
			WaitForSingleObject(m_hKillEvent,5000);			
			return E_FAIL;
		}
	}

	BYTE bAddress=0;
	CDevice* pDevice=pTag->m_pDevice;
	if(pDevice != NULL)
		bAddress=pDevice->m_bAddress;

	for(INT iTry=0;iTry < 3;iTry++)
	{
		DWORD		dwNumberOfBytesWritten=0;
		DWORD		dwNumberOfBytesRead=0;
		DWORD		dwCommErrFlags=0;
		DWORD		dwCommEvtFlags=0;
		COMSTAT	ComStat;

		m_wRcvLength=0;

		if(!ClearCommError(m_hPort,&dwCommErrFlags,&ComStat))
			continue;

		if(!PurgeComm(	m_hPort,
							PURGE_RXCLEAR |
							PURGE_RXABORT |
							PURGE_TXCLEAR |
							PURGE_TXABORT))
			continue;

		// Write the request
		if(!WriteFile(m_hPort,m_bXmtBuffer,m_wXmtLength,&dwNumberOfBytesWritten,NULL))
			continue;

		if(m_wXmtLength != dwNumberOfBytesWritten)
			continue;

		// Read the response header
		if(!ReadFile(m_hPort,m_bRcvBuffer,3,&dwNumberOfBytesRead,NULL))
		{
			CDevice* pDevice=pTag->m_pDevice;
			if(pDevice != NULL
			&& pDevice->m_bOffline)
			{
				CloseHandle(m_hPort);
				m_hPort=INVALID_HANDLE_VALUE;
				m_bPortParametersChanged=FALSE;
				return E_FAIL;
			}

			continue;
		}

		if(3 != dwNumberOfBytesRead)
		{
			CDevice* pDevice=pTag->m_pDevice;
			if(pDevice != NULL
			&& pDevice->m_bOffline)
			{
				CloseHandle(m_hPort);
				m_hPort=INVALID_HANDLE_VALUE;
				m_bPortParametersChanged=FALSE;
				return E_FAIL;
			}

			continue;
		}

		// Validate Address
		if(m_bRcvBuffer[0] != m_bXmtBuffer[0])
			continue;

		// Validate Command
		if(m_bXmtBuffer[1] == 0x41
		&& m_bRcvBuffer[1] != 0x41
		&& m_bRcvBuffer[1] != 0xC1)
			continue;

		if(m_bXmtBuffer[1] == 0x42
		&& m_bRcvBuffer[1] != 0x42
		&& m_bRcvBuffer[1] != 0xC2)
			continue;

		DWORD dwRcvLength=m_bRcvBuffer[2]+1;
		if(!ReadFile(m_hPort,&m_bRcvBuffer[3],dwRcvLength,&dwNumberOfBytesRead,NULL))
			continue;

		if(dwRcvLength != dwNumberOfBytesRead)
			continue;

		m_wRcvLength=LOWORD(dwRcvLength)+3;

		WORD wCRC;

		wCRC=*((PWORD) &m_bRcvBuffer[m_wRcvLength-2]);

		if(wCRC != CRC(m_bRcvBuffer,m_wRcvLength-2))
			continue;

		break;
	}

	if(iTry == 3
	|| m_bPortParametersChanged)
	{
		CloseHandle(m_hPort);
		m_hPort=INVALID_HANDLE_VALUE;
		m_bPortParametersChanged=FALSE;
		pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

		SignalCommunicationsFailure(pTag);

		return E_FAIL;
	}

	SignalCommunicationsRestored(pTag);

	return S_OK;
}

void CIO::SignalCommunicationsFailure(CTag* pTag)
{
	CDevice* pDevice=pTag->m_pDevice;
	if(pDevice == NULL)
	{
		CString oError;
		oError.Format(_T("SignalCommunicationFailure invalid pDevice"));
		theApp.LogError(oError);
		return;
	}


	if(pTag->m_pParent == NULL)
	{
		if(!pDevice->m_bOffline)
		{
			pDevice->m_bOffline=TRUE;
			CString oError;
			oError.Format(_T("IO Communications Failure on : StartCommunications %s Address %d"),m_oPort,pDevice->m_bAddress);
			theApp.LogError(oError);
		}			

		return;
	}

	pDevice->m_bOffline=TRUE;


	// Signal all tags bad
	CTag* pParent=pTag->m_pParent;
	while(pParent->m_pParent->m_pParent != NULL)
		pParent=pParent->m_pParent;

	if(pParent->m_wQuality != OPC_QUALITY_COMM_FAILURE)
	{
		CString oError;
		oError.Format(_T("IO Communications Failure on : %s"),pParent->m_oName);
		theApp.LogError(oError);
		pParent->m_wQuality=OPC_QUALITY_COMM_FAILURE;
		SetQuality(pParent,OPC_QUALITY_COMM_FAILURE);
		g_pDeviceManager->UpdateGroups();
	}
}

void CIO::SignalCommunicationsRestored(CTag* pTag)
{
	CTag* pParent=pTag->m_pParent;
	if(pParent == NULL)
		return;

	CDevice* pDevice=pTag->m_pDevice;
	if(pDevice == NULL)
	{
		CString oError;
		oError.Format(_T("SignalCommunicationFailure invalid pDevice"));
		theApp.LogError(oError);
		return;
	}

	pDevice->m_bOffline=FALSE;

	while(pParent->m_pParent->m_pParent != NULL)
		pParent=pParent->m_pParent;

	if(pParent->m_wQuality == OPC_QUALITY_COMM_FAILURE)
	{
		CString oInfo;
		oInfo.Format(_T("IO Communications Restored on : %s"),pParent->m_oName);
		theApp.LogInfo(oInfo);
		pParent->m_wQuality=OPC_QUALITY_GOOD;
	}
}

void CIO::SetQuality(CTag* pRoot,WORD wQuality)
{
	POSITION pos=pRoot->m_Branch.GetHeadPosition();
	while(pos)
	{
		CTag* pBranch=pRoot->m_Branch.GetNext(pos);
		SetQuality(pBranch,wQuality);
	}

	pos=pRoot->m_Leaf.GetHeadPosition();
	while(pos)
	{
		CTag* pLeaf=pRoot->m_Leaf.GetNext(pos);
		pLeaf->m_wQuality=wQuality;
	}
}

HRESULT CIO::ReadTag(CTag* pTag)
{
	// These are Write/Read tag, the read returns the value resulting from last write
	if(pTag->m_bCommand == REQUEST_PROGRAM_CODE_VALUES_AND_ATTRIBUTES_CMD
	|| pTag->m_bCommand == SET_PROGRAM_CODE_VALUE_CMD
	|| pTag->m_bCommand == AUTHORIZE_TRANSACTION_CMD
	|| pTag->m_bCommand == AUTHORIZE_BATCH_CMD)
		return S_OK;

	// Read Tags that are not Current
	// this precludes reading tags that
	// are read via a common transaction
	// multiple times
	if(pTag->m_bCurrent)
	{
		pTag->m_dwUpdateCount=0;
		return S_OK;
	}

	CSLock Lock(&m_cs);

	CDevice* pDevice=pTag->m_pDevice;

	// When device is off line issue Start Communications
	if(pDevice->m_bOffline)
	{
		CTag StartCommunications(IDS_START_COMMUNICATIONS,START_COMMUNICATIONS_CMD);
		StartCommunications.m_pDevice=pDevice;
		PrepareRequest(&StartCommunications);
		HRESULT hr=ProcessResponse(&StartCommunications,PerformIO(&StartCommunications));
		if(FAILED(hr))
		{
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			return hr;
		}	
	}

	m_iInactivityCounter=MAX_INACTIVITY;

	CoFileTimeNow(&pTag->m_Timestamp);

	HRESULT hr=PrepareRequest(pTag);
	if(FAILED(hr))
	{
		pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
		return hr;
	}

	return ProcessResponse(pTag,PerformIO(pTag));
}

HRESULT CIO::WriteTag(CTag* pTag)
{
	CSLock Lock(&m_cs);

	CDevice* pDevice=pTag->m_pDevice;

	// When device is off line issue Start Communications
	if(pDevice->m_bOffline)
	{
		CTag StartCommunications(IDS_START_COMMUNICATIONS,START_COMMUNICATIONS_CMD);
		StartCommunications.m_pDevice=pDevice;
		PrepareRequest(&StartCommunications);
		HRESULT hr=ProcessResponse(&StartCommunications,PerformIO(&StartCommunications));
		if(FAILED(hr))
		{
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			return hr;
		}	
	}

	m_iInactivityCounter=MAX_INACTIVITY;

	CoFileTimeNow(&pTag->m_Timestamp);

	HRESULT hr=PrepareRequest(pTag);
	if(FAILED(hr))
	{
		pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
		return hr;
	}

	hr=PerformIO(pTag);

	return ProcessResponse(pTag,hr);
}

void CIO::AddTagToScanList(CTag* pTag,DWORD dwUpdateRate)
{
	CSLock Lock(&m_cs);
	POSITION	pos=m_TagScanList.Find(pTag);
	if(!pos)
		m_TagScanList.AddTail(pTag);

	pTag->m_dwScanCount++;
	if(pTag->m_dwScanCount == 1
	|| pTag->m_dwUpdateRate > dwUpdateRate)
	{
		pTag->m_dwUpdateRate=dwUpdateRate;
		pTag->m_dwUpdateCount=0;
	}
}

void CIO::RemoveTagFromScanList(CTag* pTag)
{
	CSLock Lock(&m_cs);
	pTag->m_dwScanCount--;
	if(!pTag->m_dwScanCount)
	{
		POSITION pos=m_TagScanList.Find(pTag);
		if(pos)
			m_TagScanList.RemoveAt(pos);
		else
		{
			CString oError;
			oError.Format(_T("IO Error : RemoveTagFromScanList for %s"),pTag->GetPathName());
			theApp.LogError(oError);
		}
	}
}

void CIO::Scan()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())

	LONG lScanDelay=0;

	// Perform Routine Logic
	while(WAIT_OBJECT_0 != WaitForSingleObject(m_hKillEvent,(DWORD) lScanDelay))
	{
		EnterCriticalSection(&m_cs);

		POSITION	pos=m_TagScanList.GetHeadPosition();

		// Reset all the Current Flags
		while(pos)
		{
			CTag*	pTag=m_TagScanList.GetNext(pos);
			pTag->m_bCurrent=FALSE;

			// Scan offline tags at 1/4 normal rate;
			if(pTag->m_wQuality == OPC_QUALITY_COMM_FAILURE)
				pTag->m_dwUpdateCount+=25;
			else
				pTag->m_dwUpdateCount+=lScanDelay;
		}

		lScanDelay=100;

		FILETIME startFileTime;
		CoFileTimeNow(&startFileTime);
		_int64 startTime = (_int64)(((_int64) startFileTime.dwHighDateTime << 32) + ((_int64) startFileTime.dwLowDateTime));

		pos=m_TagScanList.GetHeadPosition();
		while(pos)
		{
			CTag*	pTag=m_TagScanList.GetNext(pos);

			if(pTag->m_dwUpdateCount >= pTag->m_dwUpdateRate)
			{
				ReadTag(pTag);
				pTag->m_dwUpdateCount=0;

				// If tag Failed, no reason to scan additional tags on this address
				if(pTag->m_wQuality == OPC_QUALITY_COMM_FAILURE)
				{
					POSITION pos=m_TagScanList.GetHeadPosition();
					while(pos)
					{
						CTag* pReTag=m_TagScanList.GetNext(pos);

						// Setting m_dwUpdateCount to 100 should create a round robin effect			
						if(pReTag->m_pDevice->m_bAddress == pTag->m_pDevice->m_bAddress)
							pReTag->m_dwUpdateCount=100;
					}
				}

				// Allow Synchronous Reads and Writes to interleave
				LeaveCriticalSection(&m_cs);
				EnterCriticalSection(&m_cs);

				// List may have been altered, redetermine pos
				pos=m_TagScanList.Find(pTag);
				if(!pos)
					break;

				pTag=m_TagScanList.GetNext(pos);
			}
		}

		// Close Comm Port when m_iInactivityCount decrements to 0 
		if(m_iInactivityCounter != 0)
			m_iInactivityCounter--;

		if(m_iInactivityCounter == 0
		&& m_hPort != INVALID_HANDLE_VALUE)
		{
			CloseHandle(m_hPort);
			m_hPort=INVALID_HANDLE_VALUE;
		}

		FILETIME endFileTime;
		CoFileTimeNow(&endFileTime);
		_int64 endTime = (_int64)(((_int64) endFileTime.dwHighDateTime << 32) + ((_int64) endFileTime.dwLowDateTime));

		_int64 elapsedMilliseconds = (endTime-startTime)/10000;
		if(elapsedMilliseconds < 0)
			elapsedMilliseconds=0;

		if(elapsedMilliseconds != 0)
		{
			lScanDelay-=(LONG) elapsedMilliseconds;
			if(lScanDelay < 0)
				lScanDelay=0;

			POSITION pos=m_TagScanList.GetHeadPosition();
			while(pos)
			{
				CTag* pTag=m_TagScanList.GetNext(pos);

				if(pTag->m_wQuality != OPC_QUALITY_COMM_FAILURE
				&& !pTag->m_bCurrent)
					pTag->m_dwUpdateCount+=(DWORD) elapsedMilliseconds;
			}
		}

		LeaveCriticalSection(&m_cs);
	}
}

void CIO::SetPortParameters(	LPCTSTR					szPort,
										DANLOAD_BAUD			Baud,
										DANLOAD_DATA_BITS	DataBits,
										DANLOAD_PARITY		Parity,
										DANLOAD_STOP_BITS	StopBits)
{
	CSLock Lock(&m_cs);
	
	m_oPort=szPort;
	m_Baud=Baud;
	m_DataBits=DataBits;
	m_Parity=Parity;
	m_StopBits=StopBits;
	m_bPortParametersChanged=TRUE;
}

