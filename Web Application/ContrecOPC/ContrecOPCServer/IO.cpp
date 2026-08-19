/******************************************************************************

	FILE NAME:		IO.cpp


	PURPOSE:			Implementation of the CIO


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	B. Schaal


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
		
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
			CONTREC_BAUD		Baud,
			CONTREC_DATA_BITS	DataBits,
			CONTREC_PARITY		Parity,
			CONTREC_STOP_BITS	StopBits)
{
	m_hKillEvent=NULL;
	m_pScanThread=NULL;
	m_lIndex=lIndex;
	m_oPort=szPort;
	m_Baud=Baud;
	m_DataBits=DataBits;
	m_Parity=Parity;
	m_StopBits=StopBits;

	m_bMessageTimeout = false;
	m_bMessageInProgress = false;
	m_iTimeRemaining = 0;
	m_iResetTimeValue = 30;
	m_bResetTimeChanged = false;

	m_iArm1Density = 0;
	m_iArm2Density = 0;
	m_iArm3Density = 0;
	m_iArm4Density = 0;
	m_StoredTransactionNumber = 0;
	m_StoredEntryNumber = 0;

	m_CSForceChange = _T("");
	m_CSLastTouchKeyData = _T("");

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

	m_pTimerThread = AfxBeginThread((AFX_THREADPROC) TimerThread,(LPVOID) this);
	if(!m_pTimerThread)
		throw (CString(_T("DeviceManager: AfxBeginThread Error")));

	m_pScanThread->m_bAutoDelete=FALSE;
	m_pTimerThread->m_bAutoDelete=FALSE;
}

CIO::~CIO()
{
	if(m_hKillEvent
	&& m_pScanThread)
	{
		SetEvent(m_hKillEvent);
		WaitForSingleObject(m_pScanThread->m_hThread,INFINITE);
		delete m_pScanThread;
		m_pScanThread=NULL;
	}
	if(m_hKillEvent
	&& m_pTimerThread)
	{
		SetEvent(m_hKillEvent);
		WaitForSingleObject(m_pTimerThread->m_hThread,INFINITE);
		delete m_pTimerThread;
		m_pTimerThread=NULL;
	}
	CloseHandle(m_hKillEvent);
	m_hKillEvent=NULL;

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
		case CONTREC_BAUD_1200:
			Dcb.BaudRate=CBR_1200;
			break;
		case CONTREC_BAUD_2400:
			Dcb.BaudRate=CBR_2400;
			break;
		case CONTREC_BAUD_4800:
			Dcb.BaudRate=CBR_4800;
			break;
		case CONTREC_BAUD_9600:
			Dcb.BaudRate=CBR_9600;
			break;
		case CONTREC_BAUD_19200:
			Dcb.BaudRate=CBR_19200;
			break;
		case CONTREC_BAUD_38400:
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
				return E_FAIL;
			}
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
		case CONTREC_PARITY_NONE:
			Dcb.Parity=NOPARITY;
			break;
		case CONTREC_PARITY_EVEN:
			Dcb.Parity=EVENPARITY;
			break;
		case CONTREC_PARITY_ODD:
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
				return E_FAIL;
			}
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
			return E_FAIL;
		}
	}

	m_bCommFailLogged=FALSE;
	m_bPortParametersChanged = FALSE;

	return S_OK;
}

VARENUM CIO::ContrecDataType(short sCode)
{
	if(sCode == 480)
		return VT_UI2;

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


unsigned char CIO::CalculateLCRC(int iLength, unsigned char *ucBuf)
{
	int	iLoop = 0;
	unsigned char	cCRC = 0x00;

	// the crc calculation is a simple xor for everything between the 0xc0 and does not include the crc
	for (iLoop = 1;iLoop < iLength;iLoop++)
	{
		cCRC ^= ucBuf[iLoop];
	}
	return cCRC;
}

HRESULT CIO::PrepareRequest(CTag* pTag)
{
	m_bXmtBuffer[0]=0xc0;
	m_bXmtBuffer[1]=pTag->m_pDevice->m_bAddress + 0x80;

	CDevice* pDevice=pTag->m_pDevice;
	if(pDevice == NULL)
		return E_FAIL;

	switch (pTag->m_bCommand)
	{
		case ISSUE_ENQ_COMMAND_STATUS:
		case ISSUE_ENQ_COMMAND_LASTTRANSNUMBER:
		case ISSUE_ENQ_COMMAND_FIRSTARM:
		case ISSUE_ENQ_COMMAND_NUMARMS:
		case ISSUE_ENQ_COMMAND_ARM1STATUS:
		case ISSUE_ENQ_COMMAND_ARM2STATUS:
		case ISSUE_ENQ_COMMAND_ARM3STATUS:
		case ISSUE_ENQ_COMMAND_ARM4STATUS:
		case ISSUE_ENQ_COMMAND_FIELD:
		case READ_BATCH_INPROGRESS:
		case GET_DRIVER_PIN_NUMBER:
		case ISSUE_TRUCK_ID:
		case ISSUE_LOAD_NUMBER:
		case GET_ENTERED_KEYBOARD_DATA:
		case REQUEST_POWERFAIL_ALARM_STATUS:
		case REQUEST_BATCH_COMPLETE:
		case GET_TOUCHKEY_DATA:
		case GET_DRIVER_TOUCH_KEY:
		case GET_TRUCK_PIN_NUMBER:
		case GET_TRUCK_TOUCH_KEY:
		{
			m_bXmtBuffer[2]=0x05;
			m_bXmtBuffer[3]=0x84;
			m_bXmtBuffer[4]=0xc0;
			m_bXmtBuffer[5]=0x00;
			m_wXmtLength=5;
			break;
		}

		case ISSUE_ARM1_COMMAND_GROSS_TOTAL:
		case ISSUE_ARM2_COMMAND_GROSS_TOTAL:
		case ISSUE_ARM3_COMMAND_GROSS_TOTAL:
		case ISSUE_ARM4_COMMAND_GROSS_TOTAL:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='G';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ISSUE_COMMAND_APP_VERSION:
		case ISSUE_COMMAND_APP_VERSION_DATETIME:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='A';
			m_bXmtBuffer[4]='V';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case READ_ARM1_FLOW_RATE:
		case READ_ARM2_FLOW_RATE:
		case READ_ARM3_FLOW_RATE:
		case READ_ARM4_FLOW_RATE:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='F';
			m_bXmtBuffer[4]='R';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ISSUE_ARM1_TEMPERATURE:
		case ISSUE_ARM2_TEMPERATURE:
		case ISSUE_ARM3_TEMPERATURE:
		case ISSUE_ARM4_TEMPERATURE:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='I';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ISSUE_ARM1_LASTLOAD_TEMPERATURE:
		case ISSUE_ARM2_LASTLOAD_TEMPERATURE:
		case ISSUE_ARM3_LASTLOAD_TEMPERATURE:
		case ISSUE_ARM4_LASTLOAD_TEMPERATURE:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='L';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ISSUE_ARM1_COMMAND_NET_TOTAL:
		case ISSUE_ARM2_COMMAND_NET_TOTAL:
		case ISSUE_ARM3_COMMAND_NET_TOTAL:
		case ISSUE_ARM4_COMMAND_NET_TOTAL:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='N';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ISSUE_COMMAND_SYSTEM_VERSION_DATETIME:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='G';
			m_bXmtBuffer[4]='D';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ISSUE_COMMAND_POWER_CYCLE_DATETIME:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='P';
			m_bXmtBuffer[4]='D';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ISSUE_DISPLAY_PROMPT:
		{
			int iLoop = 0;
			int iCurrentPosition = 0;
			int iReturnedLength = 0;

			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			char	MessageString[500];

redomessage:
			// format the message to the contrec based on the data passed in
			if (!formatContrecPrompt(oString,MessageString,&iReturnedLength))
					return E_FAIL;

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='D';
			m_bXmtBuffer[4]='P';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='1';
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]='3';
			m_bXmtBuffer[9]='5';
			m_bXmtBuffer[10]=0x00;
			iCurrentPosition = 11;
			for(iLoop = 0; iLoop < iReturnedLength;iLoop++)
			{
				m_bXmtBuffer[iCurrentPosition] = MessageString[iLoop];
				++iCurrentPosition;
			}

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			if(m_bXmtBuffer[iCurrentPosition] == 0xc0)
			{
				oString += " ";
				goto redomessage;
			}
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case ISSUE_GETTOUCHKEY_PROMPT:
		{
			int iLoop = 0;
			int iCurrentPosition = 0;
			int iReturnedLength = 0;

			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			char	MessageString[500];

			m_CSLastTouchKeyData = _T("");

redotouchkeymessage:
			// format the message to the contrec based on the data passed in
			if (!formatContrecPrompt(oString,MessageString,&iReturnedLength))
					return E_FAIL;

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='G';
			m_bXmtBuffer[4]='K';
			m_bXmtBuffer[5]=0x00;
			iCurrentPosition = 6;
			for(iLoop = 0; iLoop < iReturnedLength;iLoop++)
			{
				m_bXmtBuffer[iCurrentPosition] = MessageString[iLoop];
				++iCurrentPosition;
			}

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			if(m_bXmtBuffer[iCurrentPosition] == 0xc0)
			{
				oString += " ";
				goto redotouchkeymessage;
			}
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case ISSUE_DISPLAY_MESSAGE:
		{
			int iLoop = 0;
			int iCurrentPosition = 0;
			int iReturnedLength = 0;

			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			char	MessageString[500];

redomessage1:
			// format the message to the contrec based on the data passed in
			if (!formatContrecPrompt(oString,MessageString,&iReturnedLength))
					return E_FAIL;

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='D';
			m_bXmtBuffer[4]='M';
			m_bXmtBuffer[5]=0x00;
			iCurrentPosition = 6;
			for(iLoop = 0; iLoop < iReturnedLength;iLoop++)
			{
				m_bXmtBuffer[iCurrentPosition] = MessageString[iLoop];
				++iCurrentPosition;
			}

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			if(m_bXmtBuffer[iCurrentPosition] == 0xc0)
			{
				oString += " ";
				goto redomessage1;
			}
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			m_CSForceChange = _T("DM");
			break;
		}

		case ISSUE_GETANSWER_MESSAGE:
		{
			int iLoop = 0;
			int iCurrentPosition = 0;
			int iReturnedLength = 0;

			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			char	MessageString[500];

redomessage2:
			// format the message to the contrec based on the data passed in
			if (!formatContrecPrompt(oString,MessageString,&iReturnedLength))
					return E_FAIL;
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='G';
			m_bXmtBuffer[4]='A';
			m_bXmtBuffer[5]=0x00;
			iCurrentPosition = 6;
			for(iLoop = 0; iLoop < iReturnedLength;iLoop++)
			{
				m_bXmtBuffer[iCurrentPosition] = MessageString[iLoop];
				++iCurrentPosition;
			}

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			if(m_bXmtBuffer[iCurrentPosition] == 0xc0)
			{
				oString += " ";
				goto redomessage2;
			}

			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			m_CSForceChange = _T("GA");
			break;
		}

		case ISSUE_HIDDENANSWER_MESSAGE:
		{
			int iLoop = 0;
			int iCurrentPosition = 0;
			int iReturnedLength = 0;

			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			char	MessageString[500];

redomessage3:
			// format the message to the contrec based on the data passed in
			if (!formatContrecPrompt(oString,MessageString,&iReturnedLength))
					return E_FAIL;

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='G';
			m_bXmtBuffer[4]='H';
			m_bXmtBuffer[5]=0x00;
			iCurrentPosition = 6;
			for(iLoop = 0; iLoop < iReturnedLength;iLoop++)
			{
				m_bXmtBuffer[iCurrentPosition] = MessageString[iLoop];
				++iCurrentPosition;
			}

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			if(m_bXmtBuffer[iCurrentPosition] == 0xc0)
			{
				oString += " ";
				goto redomessage3;
			}
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			m_CSForceChange = _T("GH");
			break;
		}

		case ISSUE_CLEAR_DISPLAY:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='C';
			m_bXmtBuffer[4]='M';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ISSUE_REMOTE_AUTHORIZE:
		{
			CStringA csMessage;
			int		iLoop = 0;
			int		iCurrentPosition = 0;

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='R';
			m_bXmtBuffer[4]='A';
			m_bXmtBuffer[5]=0x00;
			iCurrentPosition = 6;
			if(pTag->m_Value.boolVal == VARIANT_TRUE)
			{
				m_bXmtBuffer[iCurrentPosition] = 'Y';
				++iCurrentPosition;
				m_bXmtBuffer[iCurrentPosition] = 0x00;
				++iCurrentPosition;
				m_bXmtBuffer[iCurrentPosition] = '0';
				++iCurrentPosition;
				m_bXmtBuffer[iCurrentPosition] = 0x00;
				++iCurrentPosition;
				m_bXmtBuffer[iCurrentPosition] = '0';
				++iCurrentPosition;
			}
			else
			{
				m_bXmtBuffer[iCurrentPosition] = 'N';
				++iCurrentPosition;
				m_bXmtBuffer[iCurrentPosition] = 0x00;
				++iCurrentPosition;
				csMessage = "Not Authorized";
				for(iLoop = 0;iLoop < csMessage.GetLength();iLoop++)
				{
					m_bXmtBuffer[iCurrentPosition] = csMessage.GetAt(iLoop);
					++iCurrentPosition;
				}
			}

			m_bXmtBuffer[iCurrentPosition]=0x00;
			++iCurrentPosition;
			
			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case ISSUE_REMOTEAUTH_ERRORMESSAGE:
		{
			int		iLoop = 0;
			int		iCurrentPosition = 0;
			CStringA	csMessage((LPTSTR) pTag->m_Value.bstrVal);

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='R';
			m_bXmtBuffer[4]='A';
			m_bXmtBuffer[5]=0x00;
			iCurrentPosition = 6;

			m_bXmtBuffer[iCurrentPosition] = 'N';
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition] = 0x00;
			++iCurrentPosition;

			int LengthMessage = csMessage.GetLength();
			if (LengthMessage >= 30)
				LengthMessage = 29;
			for(iLoop = 0;iLoop < LengthMessage;iLoop++)
			{
				m_bXmtBuffer[iCurrentPosition] = csMessage.GetAt(iLoop);
				++iCurrentPosition;
			}

			m_bXmtBuffer[iCurrentPosition]=0x00;
			++iCurrentPosition;
			
			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case ISSUE_TERMINATE_TRANSACTION:
		{
			if(pTag->m_Value.boolVal == VARIANT_TRUE)
			{
				m_bXmtBuffer[2]=0x02;
				m_bXmtBuffer[3]='T';
				m_bXmtBuffer[4]='T';
				m_bXmtBuffer[5]=0x00;
				m_bXmtBuffer[6]=0x03;
				m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
				m_bXmtBuffer[8]=0xc0;
				m_bXmtBuffer[9]=0x00;
				m_wXmtLength=9;
			}
			break;
		}

		case ISSUE_POWERFAIL_ALARM_CLEAR:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='C';
			m_bXmtBuffer[4]='C';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case SET_MESSAGE_TIMEOUT:
		{
			if(m_iResetTimeValue != pTag->m_Value.lVal)
			{
				m_bMessageTimeout = false;
				m_bResetTimeChanged = true;
			}
			m_iResetTimeValue = pTag->m_Value.lVal;
			break;
		}

		case ALTER_ARM_NAME:
		{
			int iLoop = 0;
			int iCurrentPosition = 0;

			// this message is sent as XAAAAAAAAA
			// where x = the arm number AAAAAAA = new name

			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='A';
			m_bXmtBuffer[4]='A';
			m_bXmtBuffer[5]=0x00;
			iCurrentPosition = 6;
			for(iLoop = 0; iLoop < oString.GetLength();iLoop++)
			{
				if(iLoop == 0)
				{
					m_bXmtBuffer[iCurrentPosition] = oString.GetAt(iLoop);
					++iCurrentPosition;
					m_bXmtBuffer[iCurrentPosition] = 0x00;
					++iCurrentPosition;
				}
				else
				{
					m_bXmtBuffer[iCurrentPosition] = oString.GetAt(iLoop);
					++iCurrentPosition;
				}
			}

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case LOAD_NUMBER_RESPONSE:
		{
			int iLoop = 0;
			int iCurrentPosition = 0;

			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='R';
			m_bXmtBuffer[4]='L';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='Y';
			m_bXmtBuffer[7]=0x00;
			iCurrentPosition = 8;
			for(iLoop = 0; iLoop < oString.GetLength();iLoop++)
			{
				m_bXmtBuffer[iCurrentPosition] = oString.GetAt(iLoop);
				++iCurrentPosition;
			}

			m_bXmtBuffer[iCurrentPosition]=0x00;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]='0';
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]='0';
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			++iCurrentPosition;

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case COMPARTMENT_RESPONSE:
		{
			int iLoop = 0;
			int iCurrentPosition = 0;
			//sent as XXXXXXX@YYYYYYYY
			// where x = preset quantity
			// y = maximum preset quantity

			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='R';
			m_bXmtBuffer[4]='C';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='Y';
			m_bXmtBuffer[7]=0x00;
			iCurrentPosition = 8;
			for(iLoop = 0; iLoop < oString.GetLength();iLoop++)
			{
				if(oString.GetAt(iLoop) == '@')
				{
					m_bXmtBuffer[iCurrentPosition] = 0x00;
					++iCurrentPosition;
				}
				else
				{
					m_bXmtBuffer[iCurrentPosition] = oString.GetAt(iLoop);
					++iCurrentPosition;
				}
			}

			m_bXmtBuffer[iCurrentPosition]=0x00;
			++iCurrentPosition;

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case BATCH_TOTALS:
		{
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='B';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=oString.GetAt(0);
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]=0x03;
			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[10]=0xc0;
			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case SET_STORED_TRANSACTION_NUMBER:
		{
			CStringA	oString;

			if(m_StoredTransactionNumber != pTag->m_Value.lVal)
				m_StoredTransactionNumber = pTag->m_Value.lVal;

			oString.Format("%07i",m_StoredTransactionNumber);

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='S';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=oString.GetAt(0);
			m_bXmtBuffer[7]=oString.GetAt(1);
			m_bXmtBuffer[8]=oString.GetAt(2);
			m_bXmtBuffer[9]=oString.GetAt(3);
			m_bXmtBuffer[10]=oString.GetAt(4);
			m_bXmtBuffer[11]=oString.GetAt(5);
			m_bXmtBuffer[12]=oString.GetAt(6);
			m_bXmtBuffer[13]=0x00;
			m_bXmtBuffer[14]=0x03;
			m_bXmtBuffer[15]=CalculateLCRC(15,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[16]=0xc0;
			m_bXmtBuffer[17]=0x00;
			m_wXmtLength=17;
			break;
		}

		case GET_STORED_TRANSACTION_UNITADDRESS:
		case GET_STORED_TRANSACTION_TRANSACTIONUMBER:
		case GET_STORED_TRANSACTION_DATE:
		case GET_STORED_TRANSACTION_STARTTIME:
		case GET_STORED_TRANSACTION_STOPTIME:
		case GET_STORED_TRANSACTION_CALIBRATIONNUMBER:
		case GET_STORED_TRANSACTION_ENTRYSTART:
		case GET_STORED_TRANSACTION_ENTRYSTOP:
		case GET_STORED_TRANSACTION_DRIVERINDEX:
		case GET_STORED_TRANSACTION_TRUCKINDEX:
		case GET_STORED_TRANSACTION_LOADNUMBER:
		case GET_STORED_TRANSACTION_ARMNUMBER:
		case GET_STORED_TRANSACTION_ARM1DENSITY:
		case GET_STORED_TRANSACTION_ARM2DENSITY:
		case GET_STORED_TRANSACTION_ARM3DENSITY:
		case GET_STORED_TRANSACTION_ARM4DENSITY:
		case GET_STORED_TRANSACTION_UNIQUENUMBER:
		case GET_STORED_TRANSACTION_FIRSTARMNUMBER:
		case GET_STORED_TRANSACTION_CHECKSUMRESULT:
		{
			CStringA	oString;

			oString.Format("%07i",m_StoredTransactionNumber);

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='S';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=oString.GetAt(0);
			m_bXmtBuffer[7]=oString.GetAt(1);
			m_bXmtBuffer[8]=oString.GetAt(2);
			m_bXmtBuffer[9]=oString.GetAt(3);
			m_bXmtBuffer[10]=oString.GetAt(4);
			m_bXmtBuffer[11]=oString.GetAt(5);
			m_bXmtBuffer[12]=oString.GetAt(6);
			m_bXmtBuffer[13]=0x00;
			m_bXmtBuffer[14]=0x03;
			m_bXmtBuffer[15]=CalculateLCRC(15,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[16]=0xc0;
			m_bXmtBuffer[17]=0x00;
			m_wXmtLength=17;
			break;
		}

		case SET_STORED_ENTRIES_NUMBER:
		{
			CStringA	oString;

			if(m_StoredEntryNumber != pTag->m_Value.lVal)
				m_StoredEntryNumber = pTag->m_Value.lVal;

			oString.Format("%04i",m_StoredEntryNumber);

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='S';
			m_bXmtBuffer[4]='Y';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=oString.GetAt(0);
			m_bXmtBuffer[7]=oString.GetAt(1);
			m_bXmtBuffer[8]=oString.GetAt(2);
			m_bXmtBuffer[9]=oString.GetAt(3);
			m_bXmtBuffer[10]=0x00;
			m_bXmtBuffer[11]=0x03;
			m_bXmtBuffer[12]=CalculateLCRC(12,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[13]=0xc0;
			m_bXmtBuffer[14]=0x00;
			m_wXmtLength=14;
			break;
		}

		case GET_STORED_ENTRIES_ENTRYNUMBER:
		case GET_STORED_ENTRIES_TRANSACTIONNUMBER:
		case GET_STORED_ENTRIES_ARMNUMBER:
		case GET_STORED_ENTRIES_COMPARTMENTNUMBER:
		case GET_STORED_ENTRIES_GROSSTOTAL:
		case GET_STORED_ENTRIES_NETTOTAL:
		case GET_STORED_ENTRIES_GROSSACCUMBEFORE:
		case GET_STORED_ENTRIES_GROSSACCUMAFTER:
		case GET_STORED_ENTRIES_NETACCUMBEFORE:
		case GET_STORED_ENTRIES_NETACCUMAFTER:
		case GET_STORED_ENTRIES_AVERTEMP:
		case GET_STORED_ENTRIES_PRESETQUANTITY:
		case GET_STORED_ENTRIES_ERRORSTATUS:
		case GET_STORED_ENTRIES_RETURNQUANTITY:
		{
			CStringA	oString;

			oString.Format("%04i",m_StoredEntryNumber);

			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='S';
			m_bXmtBuffer[4]='Y';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=oString.GetAt(0);
			m_bXmtBuffer[7]=oString.GetAt(1);
			m_bXmtBuffer[8]=oString.GetAt(2);
			m_bXmtBuffer[9]=oString.GetAt(3);
			m_bXmtBuffer[10]=0x00;
			m_bXmtBuffer[11]=0x03;
			m_bXmtBuffer[12]=CalculateLCRC(12,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[13]=0xc0;
			m_bXmtBuffer[14]=0x00;
			m_wXmtLength=14;
			break;
		}

		case GET_ARM1_BATCH_AVER_TEMP:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='B';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='1';
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]=0x03;
			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[10]=0xc0;
			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case GET_ARM2_BATCH_AVER_TEMP:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='B';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='2';
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]=0x03;
			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[10]=0xc0;
			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case GET_ARM3_BATCH_AVER_TEMP:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='B';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='3';
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]=0x03;
			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[10]=0xc0;
			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case GET_ARM4_BATCH_AVER_TEMP:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='B';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='4';
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]=0x03;
			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[10]=0xc0;
			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case GET_ARM1_BATCH_PROD_DENSITY:
		case GET_ARM1_BATCH_COMPARTMENT_NUMBER:
		case READ_ARM1_BATCH_TRANSACTION_NUMBER:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='B';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='1';
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]=0x03;
			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[10]=0xc0;
			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case GET_ARM2_BATCH_PROD_DENSITY:
		case GET_ARM2_BATCH_COMPARTMENT_NUMBER:
		case READ_ARM2_BATCH_TRANSACTION_NUMBER:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='B';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='2';
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]=0x03;
			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[10]=0xc0;
			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case GET_ARM3_BATCH_PROD_DENSITY:
		case GET_ARM3_BATCH_COMPARTMENT_NUMBER:
		case READ_ARM3_BATCH_TRANSACTION_NUMBER:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='B';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='3';
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]=0x03;
			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[10]=0xc0;
			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case GET_ARM4_BATCH_PROD_DENSITY:
		case GET_ARM4_BATCH_COMPARTMENT_NUMBER:
		case READ_ARM4_BATCH_TRANSACTION_NUMBER:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='B';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]='4';
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]=0x03;
			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[10]=0xc0;
			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case TRANSACTION_COMPLETE:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='T';
			m_bXmtBuffer[4]='C';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ARM1_PRESET_AMOUNT:
		case ARM2_PRESET_AMOUNT:
		case ARM3_PRESET_AMOUNT:
		case ARM4_PRESET_AMOUNT:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='P';
			m_bXmtBuffer[4]='R';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case GET_ARM1_ERROR_STATUS:
		case GET_ARM2_ERROR_STATUS:
		case GET_ARM3_ERROR_STATUS:
		case GET_ARM4_ERROR_STATUS:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='A';
			m_bXmtBuffer[4]='M';
			m_bXmtBuffer[5]=0x00;
			if(pTag->m_bCommand == GET_ARM1_ERROR_STATUS)
				m_bXmtBuffer[6]='1';
			else if(pTag->m_bCommand == GET_ARM2_ERROR_STATUS)
				m_bXmtBuffer[6]='2';
			else if(pTag->m_bCommand == GET_ARM3_ERROR_STATUS)
				m_bXmtBuffer[6]='3';
			else if(pTag->m_bCommand == GET_ARM4_ERROR_STATUS)
				m_bXmtBuffer[6]='4';
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]=0x03;
			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[10]=0xc0;
			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case WRITE_ARM1_DENSITY:
		case WRITE_ARM2_DENSITY:
		case WRITE_ARM3_DENSITY:
		case WRITE_ARM4_DENSITY:
		{
			int iCurrentPosition = 0;
			int iLoop = 0;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='D';
			m_bXmtBuffer[4]='N';
			m_bXmtBuffer[5]=0x00;
			if(pTag->m_bCommand == WRITE_ARM1_DENSITY)
			{
				m_bXmtBuffer[6]='1';
				m_iArm1Density = atoi(oString.GetBuffer(0));
			}
			else if(pTag->m_bCommand == WRITE_ARM2_DENSITY)
			{
				m_bXmtBuffer[6]='2';
				m_iArm2Density = atoi(oString.GetBuffer(0));
			}
			else if(pTag->m_bCommand == WRITE_ARM3_DENSITY)
			{
				m_bXmtBuffer[6]='3';
				m_iArm3Density = atoi(oString.GetBuffer(0));
			}
			else if(pTag->m_bCommand == WRITE_ARM4_DENSITY)
			{
				m_bXmtBuffer[6]='4';
				m_iArm4Density = atoi(oString.GetBuffer(0));
			}
			m_bXmtBuffer[7]=0x00;
			m_bXmtBuffer[8]='1';
			m_bXmtBuffer[9]=0x00;

			iCurrentPosition = 10;
			for(iLoop = 0; iLoop < oString.GetLength();iLoop++)
			{
				m_bXmtBuffer[iCurrentPosition] = oString.GetAt(iLoop);
				++iCurrentPosition;
			}

			m_bXmtBuffer[iCurrentPosition]=0x00;
			++iCurrentPosition;

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case READ_ARM1_DENSITY:
		case READ_ARM2_DENSITY:
		case READ_ARM3_DENSITY:
		case READ_ARM4_DENSITY:
		{
			int iCurrentPosition = 0;
			int iLoop = 0;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='A';
			m_bXmtBuffer[4]='S';
			m_bXmtBuffer[5]=0x00;
			if(pTag->m_bCommand == READ_ARM1_DENSITY)
			{
				m_bXmtBuffer[6]='1';
			}
			else if(pTag->m_bCommand == READ_ARM2_DENSITY)
			{
				m_bXmtBuffer[6]='2';
			}
			else if(pTag->m_bCommand == READ_ARM3_DENSITY)
			{
				m_bXmtBuffer[6]='3';
			}
			else if(pTag->m_bCommand == READ_ARM4_DENSITY)
			{
				m_bXmtBuffer[6]='4';
			}

			m_bXmtBuffer[7]=0x00;

			m_bXmtBuffer[8]=0x03;

			m_bXmtBuffer[9]=CalculateLCRC(9,m_bXmtBuffer); // set the value passed in equal to the crc location

			m_bXmtBuffer[10]=0xc0;

			m_bXmtBuffer[11]=0x00;
			m_wXmtLength=11;
			break;
		}

		case SET_INITIAL_MESSAGE:
		case SET_INITIAL_MESSAGE_CONTROLLED:
		{
			int iCurrentPosition = 0;
			int iLoop = 0;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='M';
			m_bXmtBuffer[4]='I';
			m_bXmtBuffer[5]=0x00;
			if(pTag->m_bCommand == SET_INITIAL_MESSAGE_CONTROLLED)
				m_bXmtBuffer[6]='0';
			else
				m_bXmtBuffer[6]='1';
			m_bXmtBuffer[7]=0x00;
			iCurrentPosition = 8;
			for(iLoop = 0; iLoop < oString.GetLength();iLoop++)
			{
				if(oString.GetAt(iLoop) == '@')
				{
					m_bXmtBuffer[iCurrentPosition] = 0x00;
					++iCurrentPosition;
				}
				else
				{
					m_bXmtBuffer[iCurrentPosition] = oString.GetAt(iLoop);
					++iCurrentPosition;
				}
			}

			m_bXmtBuffer[iCurrentPosition]=0x00;
			++iCurrentPosition;

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case DISPLAY_MESSAGE_TIMEOUT:
			break;

		case GET_ARM1_ACCUM_GROSS_TOTAL:
		case GET_ARM2_ACCUM_GROSS_TOTAL:
		case GET_ARM3_ACCUM_GROSS_TOTAL:
		case GET_ARM4_ACCUM_GROSS_TOTAL:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='A';
			m_bXmtBuffer[4]='T';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case GET_ARM1_ACCUM_NET_TOTAL:
		case GET_ARM2_ACCUM_NET_TOTAL:
		case GET_ARM3_ACCUM_NET_TOTAL:
		case GET_ARM4_ACCUM_NET_TOTAL:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='A';
			m_bXmtBuffer[4]='N';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ISSUE_MANAGER_RESET:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='M';
			m_bXmtBuffer[4]='R';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
			break;
		}

		case ISSUE_RESET_DATE_TIME:
		{
			int iCurrentPosition = 0;
			int iLoop = 0;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='R';
			m_bXmtBuffer[4]='D';
			m_bXmtBuffer[5]=0x00;

			iCurrentPosition = 6;

			for(iLoop = 0; iLoop < oString.GetLength();iLoop++)
			{
				// data format is ddmmyyyyhhmmss
				m_bXmtBuffer[iCurrentPosition] = oString.GetAt(iLoop);
				++iCurrentPosition;
				if(iLoop == 7)
				{
					m_bXmtBuffer[iCurrentPosition] = 0x00;
					++iCurrentPosition;
				}
			}

			m_bXmtBuffer[iCurrentPosition]=0x00;
			++iCurrentPosition;

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case ISSUE_SET_PIN_NUMBERS:
		{
			int iCurrentPosition = 0;
			int iLoop = 0;
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='C';
			m_bXmtBuffer[4]='P';
			m_bXmtBuffer[5]=0x00;

			iCurrentPosition = 6;

			//oString = _T("DR/001/0003");

			for(iLoop = 0; iLoop < oString.GetLength();iLoop++)
			{
				if(oString.GetAt(iLoop) == '/')
				{
					m_bXmtBuffer[iCurrentPosition] = 0x00;
					++iCurrentPosition;
				}
				else
				{
					m_bXmtBuffer[iCurrentPosition] = oString.GetAt(iLoop);
					++iCurrentPosition;
				}
			}

			m_bXmtBuffer[iCurrentPosition]=0x00;
			++iCurrentPosition;

			m_bXmtBuffer[iCurrentPosition]=0x03;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=CalculateLCRC(iCurrentPosition,m_bXmtBuffer); // set the value passed in equal to the crc location
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0xc0;
			++iCurrentPosition;
			m_bXmtBuffer[iCurrentPosition]=0x00;
			m_wXmtLength=iCurrentPosition;
			break;
		}

		case	GET_OPTION_TESTMODE:
		case	GET_OPTION_DEADMANTIMER:
		case	GET_OPTION_ILLEGALACCESS:
		case	GET_OPTION_ALARMONFAULT:
		case	GET_OPTION_COMPARTMENTPROMPT:
		case	GET_OPTION_RETURNPROMPT:
		case	GET_OPTION_LOADNUMBERPROMPT:
		case	GET_OPTION_LOADSCHEDULING:
		case	GET_OPTION_SLAVEMODE:
		case	GET_OPTION_REMOTEAUTH:
		case	GET_OPTION_SIMARMLOADING:
		case	GET_OPTION_PRESETQUANPROMPT:
		case	GET_OPTION_MULLOADSPERARM:
		case	GET_OPTION_MAXPRESET:
		{
			m_bXmtBuffer[2]=0x02;
			m_bXmtBuffer[3]='O';
			m_bXmtBuffer[4]='P';
			m_bXmtBuffer[5]=0x00;
			m_bXmtBuffer[6]=0x03;
			m_bXmtBuffer[7]=CalculateLCRC(7,m_bXmtBuffer); // set the value passed in equal to the crc location
			m_bXmtBuffer[8]=0xc0;
			m_bXmtBuffer[9]=0x00;
			m_wXmtLength=9;
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
	int iCurrentPos = 0;
	int iNumNullsToFind = 0;

	switch (pTag->m_bCommand)
	{
		case ISSUE_ENQ_COMMAND_STATUS:
		case ISSUE_ENQ_COMMAND_LASTTRANSNUMBER:
		case ISSUE_ENQ_COMMAND_FIRSTARM:
		case ISSUE_ENQ_COMMAND_NUMARMS:
		case ISSUE_ENQ_COMMAND_ARM1STATUS:
		case ISSUE_ENQ_COMMAND_ARM2STATUS:
		case ISSUE_ENQ_COMMAND_ARM3STATUS:
		case ISSUE_ENQ_COMMAND_ARM4STATUS:
		case REQUEST_POWERFAIL_ALARM_STATUS:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| !StatusIsValid(m_bRcvBuffer[3],m_bRcvBuffer[4]))
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}
				else	// valid response so process the data
				{
					if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_STATUS)
					{
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_I4;
						pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[6]);
					}
					else if(pTag->m_bCommand == REQUEST_POWERFAIL_ALARM_STATUS)
					{
						int iStatus;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_BOOL;
						iStatus = atoi((const char *)&m_bRcvBuffer[6]);
						if(iStatus & 0x10)
							pTag->m_Value.boolVal = VARIANT_TRUE;
						else
							pTag->m_Value.boolVal = VARIANT_FALSE;
					}
					else if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_LASTTRANSNUMBER)
					{
						iCurrentPos = 6;
						while(m_bRcvBuffer[iCurrentPos] != 0x00)
							++iCurrentPos;
						++iCurrentPos;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_I4;
						pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
					}
					else if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_FIRSTARM)
					{
						iCurrentPos = 6;
						iNumNullsToFind = 2;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_I4;
						pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
					}
					else if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_NUMARMS)
					{
						iCurrentPos = 6;
						iNumNullsToFind = 3;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_I4;
						pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
					}
					else if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM1STATUS ||
					pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM2STATUS ||
					pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM3STATUS ||
					pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM4STATUS)
					{
						int iNumberOfArms = 0;
						// first get the number of arms installed
						iCurrentPos = 6;
						iNumNullsToFind = 3;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;
						iNumberOfArms = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM1STATUS &&
							iNumberOfArms < 1)
						{
							pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
							break;
						}
						else if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM2STATUS &&
							iNumberOfArms < 2)
						{
							pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
							break;
						}
						else if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM3STATUS &&
							iNumberOfArms < 3)
						{
							pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
							break;
						}
						else if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM4STATUS &&
							iNumberOfArms < 4)
						{
							pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
							break;
						}
						else
						{
							CString csTemp;
							int	iStatusValue = 0;
							// get the status value
							iCurrentPos = 6;
							iNumNullsToFind = 4;
							if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM3STATUS ||
								pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM4STATUS)
								iNumNullsToFind = 5;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							iStatusValue= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
							if(pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM1STATUS ||
								pTag->m_bCommand == ISSUE_ENQ_COMMAND_ARM3STATUS)
							{
								if(iStatusValue & 0x10)
									csTemp.LoadStringW(IDS_BATCH_ERROR);
								else if(iStatusValue & 0x20)
									csTemp.LoadStringW(IDS_BATCH_COMPLETE);
								else if(iStatusValue & 0x40)
									csTemp.LoadStringW(IDS_PAUSED);
								else if(iStatusValue & 0x80)
									csTemp.LoadStringW(IDS_LOADING);
								else
									csTemp.LoadStringW(IDS_STRING_OK);
							}
							else
							{
								if(iStatusValue & 0x1)
									csTemp.LoadStringW(IDS_BATCH_ERROR);
								else if(iStatusValue & 0x2)
									csTemp.LoadStringW(IDS_BATCH_COMPLETE);
								else if(iStatusValue & 0x4)
									csTemp.LoadStringW(IDS_PAUSED);
								else if(iStatusValue & 0x8)
									csTemp.LoadStringW(IDS_LOADING);
								else
									csTemp.LoadStringW(IDS_STRING_OK);
							}
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_BSTR;
							pTag->m_Value.bstrVal = csTemp.AllocSysString();
						}
					}
					else
						pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

			break;
		}

		case ISSUE_ARM1_COMMAND_GROSS_TOTAL:
		case ISSUE_ARM2_COMMAND_GROSS_TOTAL:
		case ISSUE_ARM3_COMMAND_GROSS_TOTAL:
		case ISSUE_ARM4_COMMAND_GROSS_TOTAL:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'G'
				|| m_bRcvBuffer[4] != 'T')
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}
				else	// valid response so process the data
				{
					int iFirstArmNumber = 0;
					int iNumberOfArms = 0;
					iCurrentPos = 4;
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					iFirstArmNumber = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);

					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					iNumberOfArms = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);

					if(pTag->m_bCommand == ISSUE_ARM1_COMMAND_GROSS_TOTAL)
					{
						if(iFirstArmNumber > 1 ||
							iNumberOfArms < 1)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 2;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
					else if(pTag->m_bCommand == ISSUE_ARM2_COMMAND_GROSS_TOTAL)
					{
						if(iFirstArmNumber > 2 ||
							iNumberOfArms < 2)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 3;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
					else if(pTag->m_bCommand == ISSUE_ARM3_COMMAND_GROSS_TOTAL)
					{
						if(iFirstArmNumber > 3 ||
							iNumberOfArms < 3)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 4;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
					else if(pTag->m_bCommand == ISSUE_ARM4_COMMAND_GROSS_TOTAL)
					{
						if(iFirstArmNumber > 4 ||
							iNumberOfArms < 4)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 5;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case ISSUE_COMMAND_APP_VERSION:
		{
			if(SUCCEEDED(hr))
			{
				CString csTemp;
				TCHAR	szTemp[100];
				pTag->m_wQuality=OPC_QUALITY_GOOD;
				pTag->m_Value.vt=VT_BSTR;
				MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[6], -1, szTemp, 100);
				csTemp = szTemp;

				iCurrentPos = 6;
				iNumNullsToFind = 1;
				while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
				{
					++iCurrentPos;
					if(m_bRcvBuffer[iCurrentPos] == 0x00)
						--iNumNullsToFind;
				}
				++iCurrentPos;
				MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
				csTemp += szTemp;

				iNumNullsToFind = 1;
				while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
				{
					++iCurrentPos;
					if(m_bRcvBuffer[iCurrentPos] == 0x00)
						--iNumNullsToFind;
				}
				++iCurrentPos;
				MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
				csTemp += szTemp;

				pTag->m_Value.bstrVal = csTemp.AllocSysString();
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case ISSUE_COMMAND_APP_VERSION_DATETIME:
		{
			if(SUCCEEDED(hr))
			{
				CString csTemp;
				TCHAR	szTemp[100];
				pTag->m_wQuality=OPC_QUALITY_GOOD;
				pTag->m_Value.vt=VT_BSTR;

				iCurrentPos = 6;
				iNumNullsToFind = 3;
				while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
				{
					++iCurrentPos;
					if(m_bRcvBuffer[iCurrentPos] == 0x00)
						--iNumNullsToFind;
				}
				++iCurrentPos;
				MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
				csTemp = szTemp;

				iNumNullsToFind = 1;
				while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
				{
					++iCurrentPos;
					if(m_bRcvBuffer[iCurrentPos] == 0x00)
						--iNumNullsToFind;
				}
				++iCurrentPos;
				MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
				csTemp += _T(" ");
				csTemp += szTemp;

				pTag->m_Value.bstrVal = csTemp.AllocSysString();
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case READ_ARM1_FLOW_RATE:
		case READ_ARM2_FLOW_RATE:
		case READ_ARM3_FLOW_RATE:
		case READ_ARM4_FLOW_RATE:
		{
			if(SUCCEEDED(hr))
			{
				int iNumberOfArms = 0;
				// first get the number of arms installed
				iCurrentPos = 6;
				iNumNullsToFind = 1;
				while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
				{
					++iCurrentPos;
					if(m_bRcvBuffer[iCurrentPos] == 0x00)
						--iNumNullsToFind;
				}
				++iCurrentPos;
				iNumberOfArms = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
				if(pTag->m_bCommand == READ_ARM1_FLOW_RATE &&
					iNumberOfArms < 1)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == READ_ARM2_FLOW_RATE &&
					iNumberOfArms < 2)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == READ_ARM3_FLOW_RATE &&
					iNumberOfArms < 3)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == READ_ARM4_FLOW_RATE &&
					iNumberOfArms < 4)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else
				{
					if(pTag->m_bCommand == READ_ARM1_FLOW_RATE)
					{
						iNumNullsToFind = 2;
					}
					if(pTag->m_bCommand == READ_ARM2_FLOW_RATE)
					{
						iNumNullsToFind = 3;
					}
					if(pTag->m_bCommand == READ_ARM3_FLOW_RATE)
					{
						iNumNullsToFind = 4;
					}
					if(pTag->m_bCommand == READ_ARM4_FLOW_RATE)
					{
						iNumNullsToFind = 5;
					}
					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_R8;
					pTag->m_Value.dblVal= atof((const char *)&m_bRcvBuffer[iCurrentPos]);
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}
			
		case ISSUE_ARM1_TEMPERATURE:
		case ISSUE_ARM2_TEMPERATURE:
		case ISSUE_ARM3_TEMPERATURE:
		case ISSUE_ARM4_TEMPERATURE:
		{
			if(SUCCEEDED(hr))
			{
				int iNumberOfArms = 0;
				// first get the number of arms installed
				iCurrentPos = 6;
				iNumNullsToFind = 1;
				while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
				{
					++iCurrentPos;
					if(m_bRcvBuffer[iCurrentPos] == 0x00)
						--iNumNullsToFind;
				}
				++iCurrentPos;
				iNumberOfArms = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
				if(pTag->m_bCommand == ISSUE_ARM1_TEMPERATURE &&
					iNumberOfArms < 1)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == ISSUE_ARM2_TEMPERATURE &&
					iNumberOfArms < 2)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == ISSUE_ARM3_TEMPERATURE &&
					iNumberOfArms < 3)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == ISSUE_ARM4_TEMPERATURE &&
					iNumberOfArms < 4)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else
				{
					if(pTag->m_bCommand == ISSUE_ARM1_TEMPERATURE)
					{
						iNumNullsToFind = 2;
					}
					else if(pTag->m_bCommand == ISSUE_ARM2_TEMPERATURE)
					{
						iNumNullsToFind = 3;
					}
					else if(pTag->m_bCommand == ISSUE_ARM3_TEMPERATURE)
					{
						iNumNullsToFind = 4;
					}
					else if(pTag->m_bCommand == ISSUE_ARM4_TEMPERATURE)
					{
						iNumNullsToFind = 5;
					}

					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_R8;
					pTag->m_Value.dblVal= atof((const char *)&m_bRcvBuffer[iCurrentPos]);
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}
		
		case ISSUE_ARM1_LASTLOAD_TEMPERATURE:
		case ISSUE_ARM2_LASTLOAD_TEMPERATURE:
		case ISSUE_ARM3_LASTLOAD_TEMPERATURE:
		case ISSUE_ARM4_LASTLOAD_TEMPERATURE:
		{
			if(SUCCEEDED(hr))
			{
				int iNumberOfArms = 0;
				// first get the number of arms installed
				iCurrentPos = 6;
				iNumNullsToFind = 1;
				while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
				{
					++iCurrentPos;
					if(m_bRcvBuffer[iCurrentPos] == 0x00)
						--iNumNullsToFind;
				}
				++iCurrentPos;
				iNumberOfArms = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
				if(pTag->m_bCommand == ISSUE_ARM1_LASTLOAD_TEMPERATURE &&
					iNumberOfArms < 1)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == ISSUE_ARM2_LASTLOAD_TEMPERATURE &&
					iNumberOfArms < 2)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == ISSUE_ARM3_LASTLOAD_TEMPERATURE &&
					iNumberOfArms < 3)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == ISSUE_ARM4_LASTLOAD_TEMPERATURE &&
					iNumberOfArms < 4)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else
				{
					if(pTag->m_bCommand == ISSUE_ARM1_LASTLOAD_TEMPERATURE)
					{
						iNumNullsToFind = 2;
					}
					else if(pTag->m_bCommand == ISSUE_ARM2_LASTLOAD_TEMPERATURE)
					{
						iNumNullsToFind = 3;
					}
					else if(pTag->m_bCommand == ISSUE_ARM3_LASTLOAD_TEMPERATURE)
					{
						iNumNullsToFind = 4;
					}
					else if(pTag->m_bCommand == ISSUE_ARM4_LASTLOAD_TEMPERATURE)
					{
						iNumNullsToFind = 5;
					}

					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_R8;
					pTag->m_Value.dblVal= atof((const char *)&m_bRcvBuffer[iCurrentPos]);
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case ISSUE_ARM1_COMMAND_NET_TOTAL:
		case ISSUE_ARM2_COMMAND_NET_TOTAL:
		case ISSUE_ARM3_COMMAND_NET_TOTAL:
		case ISSUE_ARM4_COMMAND_NET_TOTAL:
		{
			if(SUCCEEDED(hr))
			{
				int iNumberOfArms = 0;
				// first get the number of arms installed
				iCurrentPos = 6;
				iNumNullsToFind = 1;
				while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
				{
					++iCurrentPos;
					if(m_bRcvBuffer[iCurrentPos] == 0x00)
						--iNumNullsToFind;
				}
				++iCurrentPos;
				iNumberOfArms = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
				if(pTag->m_bCommand == ISSUE_ARM1_COMMAND_NET_TOTAL &&
					iNumberOfArms < 1)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == ISSUE_ARM2_COMMAND_NET_TOTAL &&
					iNumberOfArms < 2)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == ISSUE_ARM3_COMMAND_NET_TOTAL &&
					iNumberOfArms < 3)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else if(pTag->m_bCommand == ISSUE_ARM4_COMMAND_NET_TOTAL &&
					iNumberOfArms < 4)
				{
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
					break;
				}
				else
				{
					if(pTag->m_bCommand == ISSUE_ARM1_COMMAND_NET_TOTAL)
					{
						iNumNullsToFind = 2;
					}
					else if(pTag->m_bCommand == ISSUE_ARM2_COMMAND_NET_TOTAL)
					{
						iNumNullsToFind = 3;
					}
					else if(pTag->m_bCommand == ISSUE_ARM3_COMMAND_NET_TOTAL)
					{
						iNumNullsToFind = 4;
					}
					else if(pTag->m_bCommand == ISSUE_ARM4_COMMAND_NET_TOTAL)
					{
						iNumNullsToFind = 5;
					}

					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_R8;
					pTag->m_Value.dblVal= atof((const char *)&m_bRcvBuffer[iCurrentPos]);
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case ISSUE_COMMAND_SYSTEM_VERSION_DATETIME:
		case ISSUE_COMMAND_POWER_CYCLE_DATETIME:
		{
			if(SUCCEEDED(hr))
			{
				CString csTemp;
				TCHAR	szTemp[100];
				pTag->m_wQuality=OPC_QUALITY_GOOD;
				pTag->m_Value.vt=VT_BSTR;

				iCurrentPos = 5;
				iNumNullsToFind = 0;
				while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
				{
					++iCurrentPos;
					if(m_bRcvBuffer[iCurrentPos] == 0x00)
						--iNumNullsToFind;
				}
				++iCurrentPos;
				MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
				csTemp = szTemp;

				iNumNullsToFind = 1;
				while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
				{
					++iCurrentPos;
					if(m_bRcvBuffer[iCurrentPos] == 0x00)
						--iNumNullsToFind;
				}
				++iCurrentPos;
				MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
				csTemp += _T(" ");
				csTemp += szTemp;

				pTag->m_Value.bstrVal = csTemp.AllocSysString();
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case ISSUE_ENQ_COMMAND_FIELD:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}
				else
				{
					CString csTemp;
					TCHAR	szTemp[100];
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;

					StatusIsValid(m_bRcvBuffer[3],m_bRcvBuffer[4]);

					iCurrentPos = 3;
					MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
					csTemp = szTemp;

					if(m_CSForceChange.GetLength() > 0)
					{
						m_CSForceChange = _T("");
						pTag->m_Value.bstrVal = m_CSForceChange.AllocSysString();
					}
					else
					{
						pTag->m_Value.bstrVal = csTemp.AllocSysString();
					}
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case READ_BATCH_INPROGRESS:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}
				else
				{
					int iNumberOfArms = 0;
					// first get the number of arms installed
					iCurrentPos = 6;
					iNumNullsToFind = 3;
					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					iNumberOfArms = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);

					if(iNumberOfArms <= 0)
					{
						pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
						break;
					}

					int	iStatusValue1 = 0;
					int	iStatusValue2 = 0;
					// get the status value
					iCurrentPos = 6;
					iNumNullsToFind = 4;

					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					iStatusValue1= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);

					if(iNumberOfArms >= 3)
					{
						iCurrentPos = 6;
						iNumNullsToFind = 5;

						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;
						iStatusValue2= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
					}

					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_I4;
					pTag->m_Value.lVal = 0;

					if(iStatusValue1 & 0x80) // arm1 inprogress
						pTag->m_Value.lVal += 0x01;
					if(iStatusValue1 & 0x8) // arm2 inprogress
						pTag->m_Value.lVal += 0x02;

					if(iStatusValue2 & 0x80) // arm3 inprogress
						pTag->m_Value.lVal += 0x04;
					if(iStatusValue2 & 0x8) // arm4 inprogress
						pTag->m_Value.lVal += 0x08;

				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case REQUEST_BATCH_COMPLETE:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02)
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}
				else
				{
					int iNumberOfArms = 0;
					// first get the number of arms installed
					iCurrentPos = 6;
					iNumNullsToFind = 3;
					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					iNumberOfArms = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);

					if(iNumberOfArms <= 0)
					{
						pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
						break;
					}

					int	iStatusValue1 = 0;
					int	iStatusValue2 = 0;
					// get the status value
					iCurrentPos = 6;
					iNumNullsToFind = 4;

					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					iStatusValue1= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);

					if(iNumberOfArms >= 3)
					{
						iCurrentPos = 6;
						iNumNullsToFind = 5;

						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;
						iStatusValue2= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
					}

					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BOOL;
					pTag->m_Value.boolVal = VARIANT_FALSE;

					if(!(iStatusValue1 & 0x80) &&// arm1 inprogress 
						!(iStatusValue1 & 0x8) &&
						!(iStatusValue1 & 0x80) &&
						!(iStatusValue1 & 0x8))
						pTag->m_Value.boolVal = VARIANT_TRUE;

				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case ISSUE_DISPLAY_PROMPT:
		case ISSUE_DISPLAY_MESSAGE:
		case ISSUE_GETANSWER_MESSAGE:
		case ISSUE_HIDDENANSWER_MESSAGE:
		case ISSUE_GETTOUCHKEY_PROMPT:
		{
			pTag->m_wQuality=OPC_QUALITY_GOOD;
			pTag->m_Value.vt=VT_BSTR;

			break;
		}

		case ISSUE_TRUCK_ID:
		{
			if(SUCCEEDED(hr))
			{
				CString	csTemp;
				TCHAR		szTemp[100];
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'R'
				|| m_bRcvBuffer[4] != 'A')
				{
					csTemp = "";
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();
				}
				else	// valid response so process the data
				{
					// find the entered data
					iCurrentPos = 6;
					iNumNullsToFind = 9;
					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;
					MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
					if(lstrlen(szTemp) == 4)
						csTemp = szTemp;
					else
						csTemp = "Bad ID";

					pTag->m_Value.bstrVal = csTemp.AllocSysString();

				}
			}
			break;
		}

		case ISSUE_LOAD_NUMBER:
		{
			if(SUCCEEDED(hr))
			{
				CString	csTemp;
				TCHAR		szTemp[100];
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'R'
				|| m_bRcvBuffer[4] != 'L')
				{
					csTemp = "";
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();
				}
				else	// valid response so process the data
				{
					// find the entered data
					iCurrentPos = 6;
					iNumNullsToFind = 6;
					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;
					MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
					csTemp = szTemp;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();

				}
			}
			break;
		}

		case GET_ENTERED_KEYBOARD_DATA:
		{
			if(SUCCEEDED(hr))
			{
				CString	csTemp;
				TCHAR		szTemp[100];
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'A'
				|| m_bRcvBuffer[4] != 'A')
				{
					csTemp = "";
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();
				}
				else	// valid response so process the data
				{
					// get the number of arms configured
					int NumberOfArms = 0;
					iCurrentPos = 6;
					iNumNullsToFind = 3;
					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					NumberOfArms= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
					// find the touch key data
					iCurrentPos = 6;
					if(NumberOfArms > 2)
						iNumNullsToFind = 6;
					else
						iNumNullsToFind = 5;
					// find the entered data
					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;
					MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);

					csTemp = szTemp;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();

				}
			}
			break;
		}

		case GET_TOUCHKEY_DATA:
		{
			if(SUCCEEDED(hr))
			{
				CString	csTemp;
				TCHAR		szTemp[100];
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'K'
				|| m_bRcvBuffer[4] != 'A')
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;

					pTag->m_Value.bstrVal = m_CSLastTouchKeyData.AllocSysString();
				}
				else	// valid response so process the data
				{
					// find the touch key data
						iCurrentPos = 6;
						iNumNullsToFind = 6;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_BSTR;
						MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);

						csTemp = szTemp;

						if(m_CSLastTouchKeyData.CompareNoCase(csTemp))
							m_CSLastTouchKeyData = csTemp;

						pTag->m_Value.bstrVal = csTemp.AllocSysString();

				}
			}
			break;
		}

		case ISSUE_CLEAR_DISPLAY:
		case ALTER_ARM_NAME:
		case LOAD_NUMBER_RESPONSE:
		case COMPARTMENT_RESPONSE:
		case TRANSACTION_COMPLETE:
		case ISSUE_TERMINATE_TRANSACTION:
			break;
		case DISPLAY_MESSAGE_TIMEOUT:
		{
			pTag->m_wQuality=OPC_QUALITY_GOOD;
			pTag->m_Value.vt=VT_BOOL;

			pTag->m_Value.boolVal = m_bMessageTimeout;
			break;
		}
		case SET_MESSAGE_TIMEOUT:
		{
			pTag->m_wQuality=OPC_QUALITY_GOOD;
			pTag->m_Value.vt=VT_I4;

			pTag->m_Value.lVal = m_iResetTimeValue;
			break;
		}
		case BATCH_TOTALS:
		case GET_ARM1_BATCH_AVER_TEMP:
		case GET_ARM2_BATCH_AVER_TEMP:
		case GET_ARM3_BATCH_AVER_TEMP:
		case GET_ARM4_BATCH_AVER_TEMP:
		case GET_ARM1_BATCH_PROD_DENSITY:
		case GET_ARM2_BATCH_PROD_DENSITY:
		case GET_ARM3_BATCH_PROD_DENSITY:
		case GET_ARM4_BATCH_PROD_DENSITY:
		case GET_ARM1_BATCH_COMPARTMENT_NUMBER:
		case GET_ARM2_BATCH_COMPARTMENT_NUMBER:
		case GET_ARM3_BATCH_COMPARTMENT_NUMBER:
		case GET_ARM4_BATCH_COMPARTMENT_NUMBER:
		case READ_ARM1_BATCH_TRANSACTION_NUMBER:
		case READ_ARM2_BATCH_TRANSACTION_NUMBER:
		case READ_ARM3_BATCH_TRANSACTION_NUMBER:
		case READ_ARM4_BATCH_TRANSACTION_NUMBER:
		{
			if(SUCCEEDED(hr))
			{
				CString	csTemp;

				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'B'
				|| m_bRcvBuffer[4] != 'T')
				{
					csTemp = "";
					pTag->m_wQuality=OPC_QUALITY_BAD;
					pTag->m_Value.vt=VT_BSTR;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();
				}
				else	// valid response so process the data
				{
					if(pTag->m_bCommand == BATCH_TOTALS )
					{
						int	iLoop = 6;
						TCHAR	szTemp[500];

						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_BSTR;
						// loop through the receive buffer and set the nulls to @
						while(m_bRcvBuffer[iLoop] != 0x03)
						{
							if(m_bRcvBuffer[iLoop] == 0x00)
								m_bRcvBuffer[iLoop] = '@';
							++iLoop;
						}

						MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[6], -1, szTemp, 500);
						csTemp = szTemp;

						pTag->m_Value.bstrVal = csTemp.AllocSysString();
					}
					else if(pTag->m_bCommand == GET_ARM1_BATCH_AVER_TEMP || 
						pTag->m_bCommand == GET_ARM2_BATCH_AVER_TEMP ||
						pTag->m_bCommand == GET_ARM3_BATCH_AVER_TEMP ||
						pTag->m_bCommand == GET_ARM4_BATCH_AVER_TEMP)
					{
						iCurrentPos = 6;
						TCHAR	szTemp[100];

						iNumNullsToFind = 9;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;

						// find the entered data
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_BSTR;

						MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 99);
						csTemp = szTemp;

						pTag->m_Value.bstrVal = csTemp.AllocSysString();
					}
					else if(pTag->m_bCommand == GET_ARM1_BATCH_PROD_DENSITY || 
						pTag->m_bCommand == GET_ARM2_BATCH_PROD_DENSITY ||
						pTag->m_bCommand == GET_ARM3_BATCH_PROD_DENSITY ||
						pTag->m_bCommand == GET_ARM4_BATCH_PROD_DENSITY)
					{
						iCurrentPos = 6;
						TCHAR	szTemp[100];

						iNumNullsToFind = 16;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;

						// find the entered data
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_BSTR;

						MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 99);
						csTemp = szTemp;

						pTag->m_Value.bstrVal = csTemp.AllocSysString();
					}

					else if(pTag->m_bCommand == GET_ARM1_BATCH_COMPARTMENT_NUMBER || 
						pTag->m_bCommand == GET_ARM2_BATCH_COMPARTMENT_NUMBER ||
						pTag->m_bCommand == GET_ARM3_BATCH_COMPARTMENT_NUMBER ||
						pTag->m_bCommand == GET_ARM4_BATCH_COMPARTMENT_NUMBER)
					{
						iCurrentPos = 6;
						TCHAR	szTemp[100];

						iNumNullsToFind = 17;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;

						// find the entered data
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_BSTR;

						MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 99);
						csTemp = szTemp;

						pTag->m_Value.bstrVal = csTemp.AllocSysString();
					}
					else if (pTag->m_bCommand == READ_ARM1_BATCH_TRANSACTION_NUMBER ||
						pTag->m_bCommand == READ_ARM2_BATCH_TRANSACTION_NUMBER ||
						pTag->m_bCommand == READ_ARM3_BATCH_TRANSACTION_NUMBER ||
						pTag->m_bCommand == READ_ARM4_BATCH_TRANSACTION_NUMBER)
					{
						iCurrentPos = 6;
						TCHAR	szTemp[100];

						iNumNullsToFind = 1;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;

						// find the entered data
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_BSTR;

						MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 99);
						csTemp = szTemp;

						pTag->m_Value.bstrVal = csTemp.AllocSysString();
					}

				}
			}
			break;
		}

		case ARM1_PRESET_AMOUNT:
		case ARM2_PRESET_AMOUNT:
		case ARM3_PRESET_AMOUNT:
		case ARM4_PRESET_AMOUNT:
		{
			if(SUCCEEDED(hr))
			{
				CString	csTemp;
				char		szTemp[100];
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'P'
				|| m_bRcvBuffer[4] != 'R')
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
					pTag->m_Value.vt=VT_R8;

				}
				else	// valid response so process the data
				{
					// find the number of arms returned	pTag->m_bCommand
					int	iNumberOfArms = atoi((const char *)&m_bRcvBuffer[8]);

					if(pTag->m_bCommand == ARM1_PRESET_AMOUNT &&
						iNumberOfArms < 1)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						pTag->m_Value.vt=VT_R8;
						break;
					}
					else if(pTag->m_bCommand == ARM2_PRESET_AMOUNT &&
						iNumberOfArms < 2)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						pTag->m_Value.vt=VT_R8;
						break;
					}
					else if(pTag->m_bCommand == ARM3_PRESET_AMOUNT &&
						iNumberOfArms < 3)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						pTag->m_Value.vt=VT_R8;
						break;
					}
					else if(pTag->m_bCommand == ARM4_PRESET_AMOUNT &&
						iNumberOfArms < 4)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						pTag->m_Value.vt=VT_R8;
						break;
					}


					if(pTag->m_bCommand == ARM1_PRESET_AMOUNT)
					{
						iCurrentPos = 8;
						iNumNullsToFind = 2;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_R8;
						strcpy(szTemp,(const char*)&m_bRcvBuffer[iCurrentPos]);
						pTag->m_Value.dblVal = strtod(szTemp,NULL);
					}
					else if(pTag->m_bCommand == ARM2_PRESET_AMOUNT)
					{
						iCurrentPos = 8;
						iNumNullsToFind = 3;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_R8;
						strcpy(szTemp,(const char*)&m_bRcvBuffer[iCurrentPos]);
						pTag->m_Value.dblVal = strtod(szTemp,NULL);
					}
					else if(pTag->m_bCommand == ARM3_PRESET_AMOUNT)
					{
						iCurrentPos = 8;
						iNumNullsToFind = 4;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_R8;
						strcpy(szTemp,(const char*)&m_bRcvBuffer[iCurrentPos]);
						pTag->m_Value.dblVal = strtod(szTemp,NULL);
					}
					else if(pTag->m_bCommand == ARM4_PRESET_AMOUNT)
					{
						iCurrentPos = 8;
						iNumNullsToFind = 5;
						while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
								--iNumNullsToFind;
						}
						++iCurrentPos;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_R8;
						strcpy(szTemp,(const char*)&m_bRcvBuffer[iCurrentPos]);
						pTag->m_Value.dblVal = strtod(szTemp,NULL);
					}
				}
			}
			break;
		}

		case GET_ARM1_ERROR_STATUS:
		case GET_ARM2_ERROR_STATUS:
		case GET_ARM3_ERROR_STATUS:
		case GET_ARM4_ERROR_STATUS:
		{
			if(SUCCEEDED(hr))
			{
				CString	csTemp;
				long		iArmStatus;
				long		iArmErrorStatus;

				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'A'
				|| m_bRcvBuffer[4] != 'M')
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
					pTag->m_Value.vt=VT_I4;

				}
				else	// valid response so process the data
				{
					iArmStatus=atoi((const char *)&m_bRcvBuffer[6]);
					iArmErrorStatus=atoi((const char *)&m_bRcvBuffer[8]);
					if(iArmStatus != 1)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						pTag->m_Value.vt=VT_I4;
					}
					else
					{
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_I4;
						pTag->m_Value.lVal=iArmErrorStatus;
					}
					break;
				}
			}
			break;
		}

		case GET_ARM1_ACCUM_GROSS_TOTAL:
		case GET_ARM2_ACCUM_GROSS_TOTAL:
		case GET_ARM3_ACCUM_GROSS_TOTAL:
		case GET_ARM4_ACCUM_GROSS_TOTAL:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'A'
				|| m_bRcvBuffer[4] != 'T')
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}
				else	// valid response so process the data
				{
					int iFirstArmNumber = 0;
					int iNumberOfArms = 0;
					iCurrentPos = 4;
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					iFirstArmNumber = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);

					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					iNumberOfArms = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);

					if(pTag->m_bCommand == GET_ARM1_ACCUM_GROSS_TOTAL)
					{
						if(iFirstArmNumber > 1 ||
							iNumberOfArms < 1)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 2;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
					else if(pTag->m_bCommand == GET_ARM2_ACCUM_GROSS_TOTAL)
					{
						if(iFirstArmNumber > 2 ||
							iNumberOfArms < 2)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 3;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
					else if(pTag->m_bCommand == GET_ARM3_ACCUM_GROSS_TOTAL)
					{
						if(iFirstArmNumber > 3 ||
							iNumberOfArms < 3)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 4;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
					else if(pTag->m_bCommand == GET_ARM4_ACCUM_GROSS_TOTAL)
					{
						if(iFirstArmNumber > 4 ||
							iNumberOfArms < 4)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 5;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case GET_ARM1_ACCUM_NET_TOTAL:
		case GET_ARM2_ACCUM_NET_TOTAL:
		case GET_ARM3_ACCUM_NET_TOTAL:
		case GET_ARM4_ACCUM_NET_TOTAL:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'A'
				|| m_bRcvBuffer[4] != 'N')
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}
				else	// valid response so process the data
				{
					int iFirstArmNumber = 0;
					int iNumberOfArms = 0;
					iCurrentPos = 4;
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					iFirstArmNumber = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);

					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					iNumberOfArms = atoi((const char *)&m_bRcvBuffer[iCurrentPos]);

					if(pTag->m_bCommand == GET_ARM1_ACCUM_NET_TOTAL)
					{
						if(iFirstArmNumber > 1 ||
							iNumberOfArms < 1)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 2;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
					else if(pTag->m_bCommand == GET_ARM2_ACCUM_NET_TOTAL)
					{
						if(iFirstArmNumber > 2 ||
							iNumberOfArms < 2)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 3;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
					else if(pTag->m_bCommand == GET_ARM3_ACCUM_NET_TOTAL)
					{
						if(iFirstArmNumber > 3 ||
							iNumberOfArms < 3)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 4;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
					else if(pTag->m_bCommand == GET_ARM4_ACCUM_NET_TOTAL)
					{
						if(iFirstArmNumber > 4 ||
							iNumberOfArms < 4)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_BAD;
							return E_FAIL;
						}
						else
						{
							iNumNullsToFind = 5;
							while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
							{
								++iCurrentPos;
								if(m_bRcvBuffer[iCurrentPos] == 0x00)
									--iNumNullsToFind;
							}
							++iCurrentPos;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_Value.vt=VT_I4;
							pTag->m_Value.lVal= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
						}
					}	// end tag 1
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case WRITE_ARM1_DENSITY:
		case WRITE_ARM2_DENSITY:
		case WRITE_ARM3_DENSITY:
		case WRITE_ARM4_DENSITY:
			{
				CString	csTemp;
				if(SUCCEEDED(hr))
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;
					if(pTag->m_bCommand == WRITE_ARM1_DENSITY)
					{
						csTemp.Format(_T("%i"),m_iArm1Density);
					}
					else if(pTag->m_bCommand == WRITE_ARM2_DENSITY)
					{
						csTemp.Format(_T("%i"),m_iArm2Density);
					}
					else if(pTag->m_bCommand == WRITE_ARM3_DENSITY)
					{
						csTemp.Format(_T("%i"),m_iArm3Density);
					}
					else if(pTag->m_bCommand == WRITE_ARM4_DENSITY)
					{
						csTemp.Format(_T("%i"),m_iArm4Density);
					}
					pTag->m_Value=csTemp;
				}
				
				break;
			}

		case READ_ARM1_DENSITY:
		case READ_ARM2_DENSITY:
		case READ_ARM3_DENSITY:
		case READ_ARM4_DENSITY:
		{
			if(SUCCEEDED(hr))
			{
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'A'
				|| m_bRcvBuffer[4] != 'S')
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
					return E_FAIL;
				}
				else	// valid response so process the data
				{
					TCHAR	szTemp[100];
					CString csTemp;

					// arm number
					iCurrentPos = 6;
					if(pTag->m_bCommand == READ_ARM1_DENSITY &&
						atoi((const char *)&m_bRcvBuffer[iCurrentPos]) != 1)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						return E_FAIL;
					}
					else if(pTag->m_bCommand == READ_ARM2_DENSITY &&
						atoi((const char *)&m_bRcvBuffer[iCurrentPos]) != 2)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						return E_FAIL;
					}
					else if(pTag->m_bCommand == READ_ARM3_DENSITY &&
						atoi((const char *)&m_bRcvBuffer[iCurrentPos]) != 3)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						return E_FAIL;
					}
					else if(pTag->m_bCommand == READ_ARM4_DENSITY &&
						atoi((const char *)&m_bRcvBuffer[iCurrentPos]) != 4)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						return E_FAIL;
					}

					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					// single or dual pulse
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					// full flow rate
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					// arm over run
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					// quantity/additive puls
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					// linear or non-linear k factor
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					// frequency
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					// k factor
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					// 4ma temperature
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					// 20 ma temperature
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					// correction type
					while(m_bRcvBuffer[iCurrentPos] != 0x00)
						++iCurrentPos;
					++iCurrentPos;

					//density
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;

					MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 99);
					csTemp = szTemp;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();
				}
			}
			else
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			break;
		}

		case SET_INITIAL_MESSAGE:
		case SET_INITIAL_MESSAGE_CONTROLLED:
			{
				pTag->m_wQuality=OPC_QUALITY_GOOD;
				pTag->m_Value.vt=VT_BSTR;
				pTag->m_Value=_T("Can Not Display");
				break;
			}

		case	GET_OPTION_TESTMODE:
		case	GET_OPTION_DEADMANTIMER:
		case	GET_OPTION_ILLEGALACCESS:
		case	GET_OPTION_ALARMONFAULT:
		case	GET_OPTION_COMPARTMENTPROMPT:
		case	GET_OPTION_RETURNPROMPT:
		case	GET_OPTION_LOADNUMBERPROMPT:
		case	GET_OPTION_LOADSCHEDULING:
		case	GET_OPTION_SLAVEMODE:
		case	GET_OPTION_REMOTEAUTH:
		case	GET_OPTION_SIMARMLOADING:
		case	GET_OPTION_PRESETQUANPROMPT:
		case	GET_OPTION_MULLOADSPERARM:
		case	GET_OPTION_MAXPRESET:
		{
			if(SUCCEEDED(hr))
			{
				CString	csTemp;
				TCHAR		szTemp[100];
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'O'
				|| m_bRcvBuffer[4] != 'P')
				{
					csTemp = "";
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();
				}
				else	// valid response so process the data
				{
					// find the entered data
					iCurrentPos = 6;
					iNumNullsToFind = 1;
					switch (pTag->m_bCommand)
					{
						case	GET_OPTION_TESTMODE:
							iNumNullsToFind = 0;
							break;
						case	GET_OPTION_DEADMANTIMER:
							iNumNullsToFind = 1;
							break;
						case	GET_OPTION_ILLEGALACCESS:
							iNumNullsToFind = 2;
							break;
						case	GET_OPTION_ALARMONFAULT:
							iNumNullsToFind = 4;
							break;
						case	GET_OPTION_COMPARTMENTPROMPT:
							iNumNullsToFind = 5;
							break;
						case	GET_OPTION_RETURNPROMPT:
							iNumNullsToFind = 6;
							break;
						case	GET_OPTION_LOADNUMBERPROMPT:
							iNumNullsToFind = 7;
							break;
						case	GET_OPTION_LOADSCHEDULING:
							iNumNullsToFind = 9;
							break;
						case	GET_OPTION_SLAVEMODE:
							iNumNullsToFind = 10;
							break;
						case	GET_OPTION_REMOTEAUTH:
							iNumNullsToFind = 11;
							break;
						case	GET_OPTION_SIMARMLOADING:
							iNumNullsToFind = 12;
							break;
						case	GET_OPTION_PRESETQUANPROMPT:
							iNumNullsToFind = 13;
							break;
						case	GET_OPTION_MULLOADSPERARM:
							iNumNullsToFind = 15;
							break;
						case	GET_OPTION_MAXPRESET:
							iNumNullsToFind = 16;
							break;
						default:
							break;
					}
					if(iNumNullsToFind > 0)
					{
						while(m_bRcvBuffer[iCurrentPos] != 0x00 && iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
							{
								m_bRcvBuffer[iCurrentPos] = ' ';
								--iNumNullsToFind;
							}
						}
						++iCurrentPos;
					}
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;
					MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);

					csTemp = szTemp;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();

				}
			}
			break;
		}

		case GET_DRIVER_TOUCH_KEY:
		case GET_DRIVER_PIN_NUMBER:
		{
			if(SUCCEEDED(hr))
			{
				CString	csTemp;
				TCHAR		szTemp[100];
				int NumberOfArms = 0;

				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'R'
				|| m_bRcvBuffer[4] != 'A')
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;

					pTag->m_Value.bstrVal = m_CSLastTouchKeyData.AllocSysString();
				}
				else	// valid response so process the data
				{
					// get the number of arms configured
					iCurrentPos = 6;
					iNumNullsToFind = 3;
					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					NumberOfArms= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
					// find the touch key data
					iCurrentPos = 6;
					if(NumberOfArms > 2)
						iNumNullsToFind = 8;
					else
						iNumNullsToFind = 7;
					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;
					MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
					if(pTag->m_bCommand == GET_DRIVER_TOUCH_KEY &&
						lstrlen(szTemp) == 12)
						csTemp = szTemp;
					else if(pTag->m_bCommand == GET_DRIVER_PIN_NUMBER &&
						lstrlen(szTemp) == 4)
						csTemp = szTemp;
					else
						csTemp = "Bad Pin";

					pTag->m_Value.bstrVal = csTemp.AllocSysString();

				}
			}
			break;
		}

		case GET_TRUCK_PIN_NUMBER:
		case GET_TRUCK_TOUCH_KEY:
		{
			if(SUCCEEDED(hr))
			{
				CString	csTemp;
				TCHAR		szTemp[100];
				int	NumberOfArms = 0;

				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'R'
				|| m_bRcvBuffer[4] != 'A')
				{
					csTemp = "";
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();
				}
				else	// valid response so process the data
				{
					// get the number of arms configured
					iCurrentPos = 6;
					iNumNullsToFind = 3;
					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					NumberOfArms= atoi((const char *)&m_bRcvBuffer[iCurrentPos]);
					// find the touch key data
					iCurrentPos = 6;
					if(NumberOfArms > 2)
						iNumNullsToFind = 9;
					else
						iNumNullsToFind = 8;
					// find the entered data

					while(m_bRcvBuffer[iCurrentPos] != 0x00 || iNumNullsToFind > 0)
					{
						++iCurrentPos;
						if(m_bRcvBuffer[iCurrentPos] == 0x00)
							--iNumNullsToFind;
					}
					++iCurrentPos;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
					pTag->m_Value.vt=VT_BSTR;
					MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[iCurrentPos], -1, szTemp, 100);
					if(pTag->m_bCommand == GET_TRUCK_TOUCH_KEY &&
						lstrlen(szTemp) == 12)
						csTemp = szTemp;
					else if(pTag->m_bCommand == GET_TRUCK_PIN_NUMBER &&
						lstrlen(szTemp) == 4)
						csTemp = szTemp;
					else
						csTemp = "Bad Pin";

					pTag->m_Value.bstrVal = csTemp.AllocSysString();

				}
			}
			break;
		}

		case SET_STORED_TRANSACTION_NUMBER:
			{
				pTag->m_wQuality=OPC_QUALITY_GOOD;
				pTag->m_Value.vt=VT_I4;

				break;
			}

		case GET_STORED_TRANSACTION_UNITADDRESS:
		case GET_STORED_TRANSACTION_TRANSACTIONUMBER:
		case GET_STORED_TRANSACTION_DATE:
		case GET_STORED_TRANSACTION_STARTTIME:
		case GET_STORED_TRANSACTION_STOPTIME:
		case GET_STORED_TRANSACTION_CALIBRATIONNUMBER:
		case GET_STORED_TRANSACTION_ENTRYSTART:
		case GET_STORED_TRANSACTION_ENTRYSTOP:
		case GET_STORED_TRANSACTION_DRIVERINDEX:
		case GET_STORED_TRANSACTION_TRUCKINDEX:
		case GET_STORED_TRANSACTION_LOADNUMBER:
		case GET_STORED_TRANSACTION_ARMNUMBER:
		case GET_STORED_TRANSACTION_ARM1DENSITY:
		case GET_STORED_TRANSACTION_ARM2DENSITY:
		case GET_STORED_TRANSACTION_ARM3DENSITY:
		case GET_STORED_TRANSACTION_ARM4DENSITY:
		case GET_STORED_TRANSACTION_UNIQUENUMBER:
		case GET_STORED_TRANSACTION_FIRSTARMNUMBER:
		case GET_STORED_TRANSACTION_CHECKSUMRESULT:
			{
				CString	csTemp;
				TCHAR		szTemp[100];
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'S'
				|| m_bRcvBuffer[4] != 'T')
				{
					csTemp = "";
					pTag->m_wQuality=OPC_QUALITY_BAD;
					pTag->m_Value.vt=VT_BSTR;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();
				}
				else	// valid response so process the data
				{
					int StartPos = 6;
					int NumberOfArms = 0;
					int OffsetAmount = 0;
					// determine the number of arms
					NumberOfArms = atoi((const char *)&m_bRcvBuffer[56]);

					OffsetAmount = 13 + NumberOfArms;

					iCurrentPos = 6;
					iNumNullsToFind = 0;
					switch(pTag->m_bCommand)
					{
					case GET_STORED_TRANSACTION_UNITADDRESS:
						iNumNullsToFind = 1;
						break;
					case GET_STORED_TRANSACTION_TRANSACTIONUMBER:
						iNumNullsToFind = 2;
						break;
					case GET_STORED_TRANSACTION_DATE:
						iNumNullsToFind = 3;
						break;
					case GET_STORED_TRANSACTION_STARTTIME:
						iNumNullsToFind = 4;
						break;
					case GET_STORED_TRANSACTION_STOPTIME:
						iNumNullsToFind = 5;
						break;
					case GET_STORED_TRANSACTION_CALIBRATIONNUMBER:
						iNumNullsToFind = 6;
						break;
					case GET_STORED_TRANSACTION_ENTRYSTART:
						iNumNullsToFind = 7;
						break;
					case GET_STORED_TRANSACTION_ENTRYSTOP:
						iNumNullsToFind = 8;
						break;
					case GET_STORED_TRANSACTION_DRIVERINDEX:
						iNumNullsToFind = 9;
						break;
					case GET_STORED_TRANSACTION_TRUCKINDEX:
						iNumNullsToFind = 10;
						break;
					case GET_STORED_TRANSACTION_LOADNUMBER:
						iNumNullsToFind = 11;
						break;
					case GET_STORED_TRANSACTION_ARMNUMBER:
						iNumNullsToFind = 12;
						break;
					case GET_STORED_TRANSACTION_ARM1DENSITY:
						{
							if(NumberOfArms > 0)
								iNumNullsToFind = 13;
							else
								iNumNullsToFind = 0;
							break;
						}
					case GET_STORED_TRANSACTION_ARM2DENSITY:
						{
							if(NumberOfArms > 1)
								iNumNullsToFind = 14;
							else
								iNumNullsToFind = 0;
							break;
						}
					case GET_STORED_TRANSACTION_ARM3DENSITY:
						{
							if(NumberOfArms > 2)
								iNumNullsToFind = 15;
							else
								iNumNullsToFind = 0;
							break;
						}
					case GET_STORED_TRANSACTION_ARM4DENSITY:
						{
							if(NumberOfArms > 3)
								iNumNullsToFind = 16;
							else
								iNumNullsToFind = 0;
							break;
						}
					case GET_STORED_TRANSACTION_UNIQUENUMBER:
						iNumNullsToFind = OffsetAmount;
						break;
					case GET_STORED_TRANSACTION_FIRSTARMNUMBER:
						iNumNullsToFind = OffsetAmount + 1;
						break;
					case GET_STORED_TRANSACTION_CHECKSUMRESULT:
						iNumNullsToFind = OffsetAmount + 2;
						break;
					default:
						break;
					}
					// find the entered data
					if(iNumNullsToFind > 0)
					{
						while(iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
							{
								m_bRcvBuffer[iCurrentPos] = ' ';
								--iNumNullsToFind;
								if(iNumNullsToFind == 1)
									StartPos = iCurrentPos;
							}
						}
						++iCurrentPos;
						m_bRcvBuffer[iCurrentPos] = 0x00;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_BSTR;

						MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[StartPos], -1, szTemp, 100);

						csTemp = szTemp;

						pTag->m_Value.bstrVal = csTemp.AllocSysString();
					}
					else
					{
						csTemp = "";
						pTag->m_wQuality=OPC_QUALITY_BAD;
						pTag->m_Value.vt=VT_BSTR;

						pTag->m_Value.bstrVal = csTemp.AllocSysString();
					}

				}
				break;
			}

		case SET_STORED_ENTRIES_NUMBER:
			{
				pTag->m_wQuality=OPC_QUALITY_GOOD;
				pTag->m_Value.vt=VT_I4;

				break;
			}

		case GET_STORED_ENTRIES_ENTRYNUMBER:
		case GET_STORED_ENTRIES_TRANSACTIONNUMBER:
		case GET_STORED_ENTRIES_ARMNUMBER:
		case GET_STORED_ENTRIES_COMPARTMENTNUMBER:
		case GET_STORED_ENTRIES_GROSSTOTAL:
		case GET_STORED_ENTRIES_NETTOTAL:
		case GET_STORED_ENTRIES_GROSSACCUMBEFORE:
		case GET_STORED_ENTRIES_GROSSACCUMAFTER:
		case GET_STORED_ENTRIES_NETACCUMBEFORE:
		case GET_STORED_ENTRIES_NETACCUMAFTER:
		case GET_STORED_ENTRIES_AVERTEMP:
		case GET_STORED_ENTRIES_PRESETQUANTITY:
		case GET_STORED_ENTRIES_ERRORSTATUS:
		case GET_STORED_ENTRIES_RETURNQUANTITY:
			{
				CString	csTemp;
				TCHAR		szTemp[100];
				if(m_bRcvBuffer[0] != 0xc0
				|| m_bRcvBuffer[1] != (pTag->m_pDevice->m_bAddress + 0x80)
				|| m_bRcvBuffer[2] != 0x02
				|| m_bRcvBuffer[3] != 'S'
				|| m_bRcvBuffer[4] != 'Y')
				{
					csTemp = "";
					pTag->m_wQuality=OPC_QUALITY_BAD;
					pTag->m_Value.vt=VT_BSTR;

					pTag->m_Value.bstrVal = csTemp.AllocSysString();
				}
				else	// valid response so process the data
				{
					int StartPos = 6;

					iCurrentPos = 6;
					iNumNullsToFind = 0;
					switch(pTag->m_bCommand)
					{
					case GET_STORED_ENTRIES_ENTRYNUMBER:
						iNumNullsToFind = 1;
						break;
					case GET_STORED_ENTRIES_TRANSACTIONNUMBER:
						iNumNullsToFind = 2;
						break;
					case GET_STORED_ENTRIES_ARMNUMBER:
						iNumNullsToFind = 3;
						break;
					case GET_STORED_ENTRIES_COMPARTMENTNUMBER:
						iNumNullsToFind = 4;
						break;
					case GET_STORED_ENTRIES_GROSSTOTAL:
						iNumNullsToFind = 5;
						break;
					case GET_STORED_ENTRIES_NETTOTAL:
						iNumNullsToFind = 6;
						break;
					case GET_STORED_ENTRIES_GROSSACCUMBEFORE:
						iNumNullsToFind = 7;
						break;
					case GET_STORED_ENTRIES_GROSSACCUMAFTER:
						iNumNullsToFind = 8;
						break;
					case GET_STORED_ENTRIES_NETACCUMBEFORE:
						iNumNullsToFind = 9;
						break;
					case GET_STORED_ENTRIES_NETACCUMAFTER:
						iNumNullsToFind = 10;
						break;
					case GET_STORED_ENTRIES_AVERTEMP:
						iNumNullsToFind = 11;
						break;
					case GET_STORED_ENTRIES_PRESETQUANTITY:
						iNumNullsToFind = 12;
						break;
					case GET_STORED_ENTRIES_ERRORSTATUS:
						iNumNullsToFind = 13;
						break;
					case GET_STORED_ENTRIES_RETURNQUANTITY:
						iNumNullsToFind = 14;
						break;
					default:
						break;
					}
					// find the entered data
					if(iNumNullsToFind > 0)
					{
						while(iNumNullsToFind > 0)
						{
							++iCurrentPos;
							if(m_bRcvBuffer[iCurrentPos] == 0x00)
							{
								m_bRcvBuffer[iCurrentPos] = ' ';
								--iNumNullsToFind;
								if(iNumNullsToFind == 1)
									StartPos = iCurrentPos;
							}
						}
						++iCurrentPos;
						m_bRcvBuffer[iCurrentPos] = 0x00;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value.vt=VT_BSTR;

						MultiByteToWideChar(CP_ACP, 0, (LPCSTR)&m_bRcvBuffer[StartPos], -1, szTemp, 100);

						csTemp = szTemp;

						pTag->m_Value.bstrVal = csTemp.AllocSysString();
					}
					else
					{
						csTemp = "";
						pTag->m_wQuality=OPC_QUALITY_BAD;
						pTag->m_Value.vt=VT_BSTR;

						pTag->m_Value.bstrVal = csTemp.AllocSysString();
					}

				}
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
	DWORD	dwCommEvtFlags = 0;

	// these tags are internal tags and our maintained by the opc server not the contrec
	if(pTag->m_bCommand == DISPLAY_MESSAGE_TIMEOUT ||
		pTag->m_bCommand == SET_MESSAGE_TIMEOUT)
		return S_OK;

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

		if(!WriteFile(m_hPort,m_bXmtBuffer,m_wXmtLength,&dwNumberOfBytesWritten,NULL))
			continue;

		if(m_wXmtLength != dwNumberOfBytesWritten)
			continue;


		unsigned char ucRecvChar = 0x00;
		DWORD				dwRecvChars = 0;
		BOOL				bStart = FALSE;
		BOOL				bEnd = FALSE;

		dwNumberOfBytesRead = 0;

		while(ReadFile(m_hPort,&ucRecvChar,1,&dwRecvChars,NULL))
		{
			if(dwRecvChars == 0)
				break;

			if(ucRecvChar == 0xc0 && !bStart)
				bStart = TRUE;

			if(bStart == TRUE)
			{
				m_bRcvBuffer[dwNumberOfBytesRead] = ucRecvChar;
				++dwNumberOfBytesRead;
			}

			if(ucRecvChar == 0xc0 && bStart && dwNumberOfBytesRead > 1)
			{
				bEnd = TRUE;
				break;
			}
		}

		if(dwNumberOfBytesRead == 0)
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

		if(!bStart || !bEnd)
			continue;

		// Validate start character
		if(m_bRcvBuffer[0] != 0xc0)
			continue;

		// Validate Address
		if(m_bRcvBuffer[1] != m_bXmtBuffer[1])
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
	HRESULT hr = S_OK;
	// These are Write/Read tag, the read returns the value resulting from last write
//	if(pTag->m_bCommand == ISSUE_GETANSWER_MESSAGE)
//	|| pTag->m_bCommand == SET_PROGRAM_CODE_VALUE_CMD
//	|| pTag->m_bCommand == AUTHORIZE_TRANSACTION_CMD
//	|| pTag->m_bCommand == AUTHORIZE_BATCH_CMD)
//		return S_OK;

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

	// When device is off line issue enq request
	if(pDevice->m_bOffline)
	{
		CTag StartCommunications(IDS_START_COMMUNICATIONS,ISSUE_ENQ_COMMAND_STATUS);
		StartCommunications.m_pDevice=pDevice;
		PrepareRequest(&StartCommunications);
		hr=ProcessResponse(&StartCommunications,PerformIO(&StartCommunications));
		if(FAILED(hr))
		{
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			return hr;
		}	
	}

	m_iInactivityCounter=MAX_INACTIVITY;

	CoFileTimeNow(&pTag->m_Timestamp);

	hr=PrepareRequest(pTag);
	if(FAILED(hr))
	{
		pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
		return hr;
	}

	hr = ProcessResponse(pTag,PerformIO(pTag));
	return hr;
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


	// Perform Routine Logic
	while(WAIT_OBJECT_0 != WaitForSingleObject(m_hKillEvent,100))
	{
		EnterCriticalSection(&m_cs);

		POSITION	pos=m_TagScanList.GetHeadPosition();

		// Reset all the Current Flags
		while(pos)
		{
			CTag*	pTag=m_TagScanList.GetNext(pos);
			pTag->m_bCurrent=FALSE;
		}

		pos=m_TagScanList.GetHeadPosition();
		while(pos)
		{
			CTag*	pTag=m_TagScanList.GetNext(pos);

			// Scan offline tags at 1/4 the rate of online tags
			if(pTag->m_wQuality == OPC_QUALITY_COMM_FAILURE)
				pTag->m_dwUpdateCount+=25;
			else
				pTag->m_dwUpdateCount+=100;

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

						// Setting m_dwUpdateCount to 10 should create a round robin effect			
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

		LeaveCriticalSection(&m_cs);
	}
}

void CIO::SetPortParameters(	LPCTSTR					szPort,
										CONTREC_BAUD			Baud,
										CONTREC_DATA_BITS	DataBits,
										CONTREC_PARITY		Parity,
										CONTREC_STOP_BITS	StopBits)
{
	CSLock Lock(&m_cs);
	
	m_oPort=szPort;
	m_Baud=Baud;
	m_DataBits=DataBits;
	m_Parity=Parity;
	m_StopBits=StopBits;
	m_bPortParametersChanged=TRUE;
}

BOOL CIO::formatContrecPrompt(CStringA oString,char *MessageString,int *iReturnedLength)
{
	int	iLoop = 0;
	int	iLoop1 = 0;
	int	iLines = 0;
	int	iPosition = 0;
	int	iLength = 0;
	// the message can handle upto 8 line at 30 characters each. The line delimiter is | @ for a value of -2 which means clear line
	// determine how many lines are present
	for(iLoop = 0;iLoop < oString.GetLength();iLoop++)
	{
		if(oString.GetAt(iLoop) == '|')//'@')
			++iLines;
	}

	// format the message into the correct format and return
	ZeroMemory(MessageString,sizeof(MessageString));
	iLength = oString.GetLength();
	for(iLoop = 0;iLoop < iLength;iLoop++)
	{
		if((oString.GetAt(iLoop) == '|' /*'@'*/ && iLoop == (iLength - 1)) ||
			(oString.GetAt(iLoop) == '|' /*'@'*/ && oString.GetAt(iLoop+1) == '|' /*'@'*/))
		{
			MessageString[iPosition] = '-';
			++iPosition;
			MessageString[iPosition] = '2';
			++iPosition;
			MessageString[iPosition] = 0x00;
			++iPosition;
		}
		else
		{
			// must be a string so set in the returned value
			if(oString.GetAt(iLoop) == '|' /*'@'*/)
				++iLoop;
			MessageString[iPosition] = '|';	// center string
			++iPosition;
			for(iLoop1 = iLoop;iLoop1 < iLength;iLoop1++)
			{
				if(oString.GetAt(iLoop) == '|' /*'@'*/)
				{
					--iLoop;
					break;
				}
				MessageString[iPosition] = oString.GetAt(iLoop);
				++iPosition;
				++iLoop;
			}
			MessageString[iPosition] = 0x00;
			++iPosition;
		}
	}

	if (iLines == 0)
		return FALSE;

	for(iLoop = iLines;iLoop < 8;iLoop++)
	{
			MessageString[iPosition] = '-';
			++iPosition;
			MessageString[iPosition] = '2';
			++iPosition;
			MessageString[iPosition] = 0x00;
			++iPosition;
	}
	*iReturnedLength = iPosition;
	return TRUE;
}

BOOL CIO::StatusIsValid(char chTestByte3,char chTestByte4)
{
	// this routine will check the mode of the 1010 and determine if it is valid to continue
	if(chTestByte3 == 'S' &&
		chTestByte4 == 'S')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'D' &&
		chTestByte4 == 'M')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'D' &&
		chTestByte4 == 'P')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'G' &&
		chTestByte4 == 'A')
	{
		m_bMessageInProgress = true;
		return TRUE;
	}
	else if(chTestByte3 == 'G' &&
		chTestByte4 == 'C')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'G' &&
		chTestByte4 == 'H')
	{
		m_bMessageInProgress = true;
		return TRUE;
	}
	else if(chTestByte3 == 'G' &&
		chTestByte4 == 'K')
	{
		m_bMessageInProgress = true;
		return TRUE;
	}
	else if(chTestByte3 == 'M' &&
		chTestByte4 == 'T')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'P' &&
		chTestByte4 == 'L')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'A' &&
		chTestByte4 == 'A')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'C' &&
		chTestByte4 == 'A')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'K' &&
		chTestByte4 == 'A')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'R' &&
		chTestByte4 == 'A')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'R' &&
		chTestByte4 == 'C')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'R' &&
		chTestByte4 == 'L')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}
	else if(chTestByte3 == 'R' &&
		chTestByte4 == 'P')
	{
		m_bMessageInProgress = false;
		return TRUE;
	}

	return FALSE;
}

UINT CIO::TimerThread(LPVOID lpIO)
{
	CIO* pIO = (CIO*) lpIO;

	pIO->CheckTimer();

	AfxEndThread(0);

	return( 0 );
}

void CIO::CheckTimer()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())

	// Perform Routine Logic
	while(WAIT_OBJECT_0 != WaitForSingleObject(m_hKillEvent,1000))
	{
		if(m_bResetTimeChanged)
		{
			m_iTimeRemaining = m_iResetTimeValue;
			m_bMessageTimeout = false;
			m_bResetTimeChanged = false;
		}
		if(m_bMessageInProgress)
		{
			--m_iTimeRemaining;
			if(m_iTimeRemaining <= 0)
			{
				m_bMessageTimeout = true;
			}
			else
				m_bMessageTimeout = false;
		}
		else
		{
			m_iTimeRemaining = m_iResetTimeValue;
			m_bMessageTimeout = false;
		}
	}
}
