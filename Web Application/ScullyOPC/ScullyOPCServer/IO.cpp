/******************************************************************************

	FILE NAME:		IO.cpp


	PURPOSE:			Implementation of the CIO


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	S. Jiang


	VERSION:		9.0.0.0  Current version



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

// CTag
CTag::CTag(LPCTSTR szName)
{
	m_pParent=NULL;
	m_oName=szName;
	m_dwAccessRights=OPC_READABLE;
	m_dwScanCount=0;
	m_pIO=NULL;
	VariantInit(&m_Value);
	CoFileTimeNow(&m_Timestamp);
	m_wQuality=OPC_QUALITY_BAD;
}

CTag::CTag(INT iID)
{
	m_pParent=NULL;
	m_oName.LoadString(iID);
	m_dwAccessRights=OPC_READABLE;
	m_dwScanCount=0;
	m_pIO=NULL;
	VariantInit(&m_Value);
	CoFileTimeNow(&m_Timestamp);
	m_wQuality=OPC_QUALITY_BAD;
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

CTag* CTag::AddBranch(LPCTSTR szName,CIO* pIO)
{
	CTag* pTag=new CTag(szName);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_bLeaf=FALSE;
	pTag->m_pParent=this;
	pTag->m_pIO=pIO;
	
	m_Branch.AddTail(pTag);
	return pTag;
}

CTag* CTag::AddBranch(INT iID,CIO* pIO)
{
	CTag* pTag=new CTag(iID);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_bLeaf=FALSE;
	pTag->m_pParent=this;
	pTag->m_pIO=pIO;
	
	m_Branch.AddTail(pTag);
	return pTag;
}


CTag* CTag::AddLeaf(	SCULLY_TAG_TYPE	ScullyTagType,
							LPCTSTR					szName,
							BYTE						bAddress,
							DWORD						dwAccessRights,
							VARTYPE					NativeType,
							CIO*						pIO)
{
	CTag* pTag=new CTag(szName);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_ScullyTagType=ScullyTagType;
	pTag->m_bLeaf=TRUE;
	pTag->m_bAddress=bAddress;
	pTag->m_dwAccessRights=dwAccessRights;
	pTag->m_NativeType=NativeType;
	pTag->m_pParent=this;
	pTag->m_pIO=pIO;	
	m_Leaf.AddTail(pTag);
	g_pDeviceManager->AddTagToGroupItems(pTag);
	return pTag;
}

CTag* CTag::AddLeaf(	SCULLY_TAG_TYPE	ScullyTagType,
							INT						iID,
							BYTE						bAddress,
							DWORD						dwAccessRights,
							VARTYPE					NativeType,
							CIO*						pIO)
{
	CTag* pTag=new CTag(iID);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_ScullyTagType=ScullyTagType;
	pTag->m_bLeaf=TRUE;
	pTag->m_bAddress=bAddress;
	pTag->m_dwAccessRights=dwAccessRights;
	pTag->m_NativeType=NativeType;
	pTag->m_pParent=this;
	pTag->m_pIO=pIO;	
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

CIO::CIO( BYTE								lDeviceID,
			LONG								lIndex,
			LPCTSTR							szPort,
			SCULLY_BAUD				dwBaud,
			SCULLY_DATA_BITS		bDataBits,
			SCULLY_PARITY			bParity,
			SCULLY_STOP_BITS		bStopBits)
{
	m_hKillEvent=NULL;
	m_pScanThread=NULL;
	m_oPort=szPort;
	m_DeviceID = lDeviceID;

	SetDeviceBaudRate(dwBaud);
	SetDeviceParity(bParity);
	SetDeviceDataBits(bDataBits);
	SetDeviceStopBits(bStopBits);

	m_bPortParametersChanged = FALSE;

	InitializeCriticalSection(&m_cs);

	m_dwUseCount=0;

	m_hPort=INVALID_HANDLE_VALUE;
	m_bCommFailLogged=FALSE;

	ZeroMemory(&WriteOverLapped,sizeof(OVERLAPPED));
	ZeroMemory(&ReadOverLapped,sizeof(OVERLAPPED));
	ZeroMemory(&CommOverLapped,sizeof(OVERLAPPED));

	WriteOverLapped.hEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
	if(WriteOverLapped.hEvent == NULL )
		throw (CString(_T("IO: CreateEvent Error")));

   ReadOverLapped.hEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
	if(ReadOverLapped.hEvent == NULL )
		throw (CString(_T("IO: CreateEvent Error")));

   CommOverLapped.hEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
	if(CommOverLapped.hEvent == NULL )
		throw (CString(_T("IO: CreateEvent Error")));

	// Check and start log file
	CString logMsg;
	HKEY hScullyOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	if (ERROR_SUCCESS == RegOpenKeyEx(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Varec\\ScullyOPC"),0,KEY_READ,&hScullyOPCKey))
	{
		DWORD dwLogPorts;
		DWORD cbLogPorts = 4;
		if (ERROR_SUCCESS == RegQueryValueEx(hScullyOPCKey, _T("LogPorts"), NULL, NULL, reinterpret_cast<LPBYTE>(&dwLogPorts), &cbLogPorts))
		{
			if (0 != dwLogPorts)
			{
				DWORD cbBasePath = 0;
				if (ERROR_SUCCESS == RegQueryValueEx(hScullyOPCKey, _T("LogBasePath"), NULL, NULL, NULL, &cbBasePath))
				{
					logMsg.Format(_T("IO.Init() - LogBasePath size of %d"), cbBasePath);
					OutputDebugString((LPCTSTR)logMsg);
					LPTSTR pszBasePath;
					pszBasePath = new TCHAR[cbBasePath + 1];
					if (pszBasePath != NULL)
					{
						if (ERROR_SUCCESS == RegQueryValueEx(hScullyOPCKey, _T("LogBasePath"), NULL, NULL, reinterpret_cast<LPBYTE>(pszBasePath), &cbBasePath))
						{
							pszBasePath[cbBasePath] = _T('\0');
							CString csLogFile;
							m_csBaseLogFileName = pszBasePath;
							m_csBaseLogFileName.TrimRight(_T('\\'));
							m_csBaseLogFileName += (_T("\\"));
							m_csBaseLogFileName += m_oPort;
							m_csBaseLogFileName += _T("-");
							csLogFile = m_csBaseLogFileName;
							m_odtLastLogTime = COleDateTime::GetCurrentTime();
							csLogFile += m_odtLastLogTime.Format(_T("%Y-%m-%d-%H"));
							csLogFile += _T(".log");
							m_hLogFile = CreateFile(csLogFile, 
													GENERIC_WRITE, 
													FILE_SHARE_READ, 
													NULL, 
													OPEN_ALWAYS,
													FILE_ATTRIBUTE_NORMAL,
													NULL);
							if (m_hLogFile == INVALID_HANDLE_VALUE)
							{	
								CString oError;
								oError.Format(_T("Unable to create log file %s"), csLogFile);
								theApp.LogError(oError);
							}
						}
						delete[] pszBasePath;
					}
				}
				else
				{
					OutputDebugString(_T("IO.Init() - LogBasePath not read"));
				}
			}
		}
		RegCloseKey(hScullyOPCKey);
		hScullyOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	}

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

	if (m_hLogFile != INVALID_HANDLE_VALUE)
	{
		CloseHandle(m_hLogFile);
		m_hLogFile = INVALID_HANDLE_VALUE;
	}

	if(m_hPort != INVALID_HANDLE_VALUE)
	{
		CloseHandle(m_hPort);
		m_hPort=INVALID_HANDLE_VALUE;
	}

	if(WriteOverLapped.hEvent != NULL )
		CloseHandle(WriteOverLapped.hEvent );

	if(ReadOverLapped.hEvent != NULL )
		CloseHandle(ReadOverLapped.hEvent );

	if(CommOverLapped.hEvent != NULL )
		CloseHandle(CommOverLapped.hEvent );

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
		DWORD dw1 = GetLastError();
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

	DWORD dwSize = sizeof(COMMCONFIG);
	COMMCONFIG config;
  if( !GetCommConfig(m_hPort, &config, &dwSize) ) {
    TRACE( _T("Failed in call to GetCommConfig\n") );
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

	Dcb.BaudRate=m_dwBaud;
	Dcb.ByteSize=m_bDataBits;
	Dcb.Parity=m_bParity;
	Dcb.StopBits=m_bStopBits;
	Dcb.fOutxCtsFlow=FALSE;
	Dcb.fOutxDsrFlow=FALSE;
	Dcb.fOutX=FALSE;
	Dcb.fInX=FALSE;
	Dcb.fRtsControl=RTS_CONTROL_DISABLE;
	Dcb.fDtrControl=DTR_CONTROL_DISABLE;
	Dcb.fAbortOnError=TRUE;
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

HRESULT CIO::PrepareRequest(CTag* pTag,BOOL bWrite)
{
	if(pTag->m_ScullyTagType == TRUCK_PRESENT_TAG
		|| pTag->m_ScullyTagType == BYPASS_TAG)
	{
		
		m_bXmtBuffer[0]=pTag->m_bAddress;  // Adress Device address configured 00- 63
		m_bXmtBuffer[1]=0x02;  // Read Input. Function Code 02
		m_bXmtBuffer[2]=0x00;  // Start Bit# MSB 00
		m_bXmtBuffer[3]=0x00;  // Start bit# LSB 00
		m_bXmtBuffer[4]=0x00;  // Bit Count MSB
		m_bXmtBuffer[5]=0x10;  // Bit Count LSB
		*((PWORD) &m_bXmtBuffer[6])=CRC(m_bXmtBuffer,6);
		m_wXmtLength=8;		
	}
	else if(pTag->m_ScullyTagType == TRUCK_SERIAL_NUMBER_TAG)
	{
		m_bXmtBuffer[0]=pTag->m_bAddress;  // Adress Device address configured 00- 63
		m_bXmtBuffer[1]=0x03;  // Read Multiple Registers. Function Code 03
		m_bXmtBuffer[2]=0x01;  // Start Reg# MSB
		m_bXmtBuffer[3]=0x0A;  // Start Reg# LSB
		m_bXmtBuffer[4]=0x00;  // Register Count MSB 00
		m_bXmtBuffer[5]=0x03;  // Register Count MSB 03 three registers
		*((PWORD) &m_bXmtBuffer[6])=CRC(m_bXmtBuffer,6);
		m_wXmtLength=8;
	}

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

HRESULT CIO::ProcessResponse(CTag* pTag,HRESULT hr)
{		
	if(pTag->m_ScullyTagType == TRUCK_SERIAL_NUMBER_TAG)
		{
			if(11 != m_wRcvLength
         || pTag->m_bAddress != m_bRcvBuffer[0]				// Did it come from device #1
         || 0x03 != m_bRcvBuffer[1]				// Is it an Read multiple regiesters code 03
         || 0x06 != m_bRcvBuffer[2]				// Is it six Bytes?
		 || CRC(m_bRcvBuffer,9) != *((PWORD) &m_bRcvBuffer[9]))			
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			else
			{
				//  The transmission looks okay.  Let's convert it to a TRUCK SERIAL NUMBER
				CString resultString;				
				CString strTemp;

				for(int j = 0; j <= 5; j++)
				{
					strTemp.Format(_T("%02X"), m_bRcvBuffer[j+3]);
					resultString += strTemp;
				}
				pTag->m_Value.vt = VT_BSTR; 
				pTag->m_Value = resultString;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
		}

		else if(pTag->m_ScullyTagType == TRUCK_PRESENT_TAG 
				|| pTag->m_ScullyTagType == BYPASS_TAG)
		{			
			if(7 != m_wRcvLength 
		 || pTag->m_bAddress != m_bRcvBuffer[0]	
         || 0x02 != m_bRcvBuffer[1]				// Is it Read Input status Code
         || 0x02 != m_bRcvBuffer[2]				// Is two Byte counts 
         //|| 0x00 != m_bRcvBuffer[4]		  sijuan: new board returns '0xA0', old one returns '0x00' for now let's ignore this part until new instruction
		 || CRC(m_bRcvBuffer,5) != *((PWORD) &m_bRcvBuffer[5]))	
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			else
			{				
				if(getBit(m_bRcvBuffer[3], 0) == 1)
					ReportError(pTag);
				pTag->m_Value.vt=VT_BOOL;
				int bflag;
				if(pTag->m_ScullyTagType == TRUCK_PRESENT_TAG )
					bflag = getBit(m_bRcvBuffer[3], 1);
				else
					bflag = getBit(m_bRcvBuffer[3], 4);
				if (bflag == 1)
					pTag->m_Value.boolVal=VARIANT_TRUE;
				else
					pTag->m_Value.boolVal=VARIANT_FALSE;

				pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
		}

	return hr;
}

void CIO::ReportError(CTag* pTag)
{
	CString oCode = _T("Fault (Intellitrol service LED blinking).");
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

		LogWrite(m_bXmtBuffer,m_wXmtLength);
		// Write the request
		if(!WriteFile(m_hPort,m_bXmtBuffer,m_wXmtLength,&dwNumberOfBytesWritten,NULL))
			continue;

		if(m_wXmtLength != dwNumberOfBytesWritten)
			continue;
		
		// Read the response header
		if(!ReadFile(m_hPort,m_bRcvBuffer,3,&dwNumberOfBytesRead,NULL))
		{
			int error = GetLastError();
			continue;
		}

		if(3 != dwNumberOfBytesRead)
			continue;
		
		// Validate Address
		if(m_bRcvBuffer[0] != m_bXmtBuffer[0])
			continue;

		// Validate Command
		if(m_bXmtBuffer[1] != m_bRcvBuffer[1])
			continue;

		DWORD dwRcvLength=m_bRcvBuffer[2]+2;
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
	LogRead(m_bRcvBuffer,m_wRcvLength);
	if(iTry == 3 || m_bPortParametersChanged)
	{
		CloseHandle(m_hPort);
		m_hPort=INVALID_HANDLE_VALUE;
		m_bPortParametersChanged=FALSE;
		pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

		SignalCommunicationsFailure(pTag);

		return E_FAIL;
	}


	return S_OK;
}

void CIO::CloseComPort()
{
	if(m_hPort == INVALID_HANDLE_VALUE)
		return;

	int iTry=0;
	while(!CloseHandle(m_hPort) && iTry < 3)
	{
		DWORD dw = GetLastError();

		CString oError;
		oError.Format(_T("IO Error : CloseHandle on : %s\nGetLastError()=%u"),m_oPort,dw);
		theApp.LogError(oError);

		WaitForSingleObject(m_hKillEvent,5000);
		iTry++;			
	}

	WaitForSingleObject(m_hKillEvent,500);

	m_hPort=INVALID_HANDLE_VALUE;
}


void CIO::SignalCommunicationsFailure(CTag* pTag)
{
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

   CoFileTimeNow(&pTag->m_Timestamp);

	HRESULT hr=PrepareRequest(pTag,FALSE);
	if(FAILED(hr))
	{
		pTag->m_wQuality=OPC_QUALITY_CONFIG_ERROR;
		return hr;
	}

	return ProcessResponse(pTag,PerformIO(pTag));
}

HRESULT CIO::WriteTag(CTag* pTag)
{
	CSLock Lock(&m_cs);

   CoFileTimeNow(&pTag->m_Timestamp);

	HRESULT hr=PrepareRequest(pTag,TRUE);
	if(FAILED(hr))
	{
		pTag->m_wQuality=OPC_QUALITY_CONFIG_ERROR;
		return hr;
	}

	return ProcessResponse(pTag,PerformIO(pTag));
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
						if(pReTag->m_bAddress == pTag->m_bAddress)
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
										SCULLY_BAUD			Baud,
										SCULLY_DATA_BITS	DataBits,
										SCULLY_PARITY		Parity,
										SCULLY_STOP_BITS	StopBits)
{
	CSLock Lock(&m_cs);
	
	m_oPort=szPort;

	SetDeviceBaudRate(Baud);
	SetDeviceParity(Parity);
	SetDeviceDataBits(DataBits);
	SetDeviceStopBits(StopBits);

	m_bPortParametersChanged=TRUE;
}

void CIO::SetDeviceBaudRate(SCULLY_BAUD dwBaud)
{
	switch(dwBaud)
	{
	case SCULLY_BAUD_1200:
		m_dwBaud = CBR_1200;
		break;
	case SCULLY_BAUD_2400:
		m_dwBaud = CBR_2400;
		break;
	case SCULLY_BAUD_4800:
		m_dwBaud = CBR_4800;
		break;
	case SCULLY_BAUD_9600:
		m_dwBaud = CBR_9600;
		break;
	case SCULLY_BAUD_19200:
		m_dwBaud = CBR_19200;
		break;
	case SCULLY_BAUD_38400:
		m_dwBaud = CBR_38400;
		break;
	default:
		m_dwBaud = CBR_9600;
		break;
	}
}

void CIO::SetDeviceParity(SCULLY_PARITY bParity)
{
	switch(bParity)
	{
	case SCULLY_PARITY_NONE:
		m_bParity=NOPARITY;
		break;
	case SCULLY_PARITY_EVEN:
		m_bParity=EVENPARITY;
		break;
	case SCULLY_PARITY_ODD:
		m_bParity=ODDPARITY;
		break;
	default:
		m_bParity=EVENPARITY;
		break;
	}
}

void CIO::SetDeviceDataBits(SCULLY_DATA_BITS bDataBits)
{
	switch(bDataBits)
	{
	case DATA_BITS_7:
		m_bDataBits=7;
		break;
	case DATA_BITS_8:
		m_bDataBits=8;
		break;
	default:
		m_bDataBits=7;
		break;
	}
}

void CIO::SetDeviceStopBits(SCULLY_STOP_BITS bStopBits)
{
	switch(bStopBits)
	{
	case STOP_BITS_1:
			m_bStopBits=ONESTOPBIT;
		break;
	case STOP_BITS_2:
			m_bStopBits=TWOSTOPBITS;
		break;
	default:
			m_bStopBits=ONESTOPBIT;
		break;
	}
}

void CIO::LogRead(BYTE* buffer, WORD length)
{
	if (m_hLogFile == INVALID_HANDLE_VALUE)
	{
		return;
	}

	SetFilePointer(m_hLogFile, 0, NULL, FILE_END);

	// write datetime
	CStringA logString;
	COleDateTime logTime = COleDateTime::GetCurrentTime();
	if (logTime.GetHour() != m_odtLastLogTime.GetHour())
	{
		CycleLogFile();
	}

	// Need to check again, because CycleLogFile could have failed
	if (m_hLogFile == INVALID_HANDLE_VALUE)
	{
		return;
	}

	m_odtLastLogTime = logTime;

	logString = logTime.Format();
	logString += "\r\n";

	// write direction indicator
	logString += "<<\r\n";

	// write bytes 16 at a time
	WORD nLine, nPosition;
	BYTE pbLoggedSegment[16];
	CStringA strTemp;
	for (nLine = 0; nLine < length / 16; nLine++)
	{
		ZeroMemory(pbLoggedSegment, 16);
		for (nPosition = 0; nPosition < 16; nPosition++)
		{
			strTemp.Format("%02X ", buffer[(nLine * 16) + nPosition]);
			logString += strTemp;
			if ('\x00' == buffer[(nLine * 16) + nPosition])
			{
				pbLoggedSegment[nPosition] = '\x80';
			}
			else
			{
				pbLoggedSegment[nPosition] = buffer[(nLine * 16) + nPosition];
			}
		}

		char cPrintableByte;

		for (nPosition = 0; nPosition < 16; nPosition++)
		{
			if (isprint(pbLoggedSegment[nPosition]))
			{
				cPrintableByte = static_cast<char>(pbLoggedSegment[nPosition]);
			}
			else
			{
				cPrintableByte = '.';
			}
			logString += cPrintableByte;
		}
		logString += "\r\n";
	}

	// Write remaining bytes
	ZeroMemory(pbLoggedSegment, 16);
	int nRemainingSize;
	nRemainingSize = length - (nLine * 16);
	for (nPosition = 0; nPosition < nRemainingSize; nPosition++)
	{
		strTemp.Format("%02X ", buffer[(nLine * 16) + nPosition]);
		logString += strTemp;
		if ('\x00' == buffer[(nLine * 16) + nPosition])
		{
			pbLoggedSegment[nPosition] = '\x80';
		}
		else
		{
			pbLoggedSegment[nPosition] = buffer[(nLine * 16) + nPosition];
		}
	}
	for (; nPosition < 16; nPosition++)
	{
		strTemp.Format("   ");
		logString += strTemp;
	}

	char cPrintableByte;

	for (nPosition = 0; nPosition < nRemainingSize; nPosition++)
	{
		if (isprint(pbLoggedSegment[nPosition]))
		{
			cPrintableByte = static_cast<char>(pbLoggedSegment[nPosition]);
		}
		else
		{
			cPrintableByte = '.';
		}
		logString += cPrintableByte;
	}

	logString += "\r\n\r\n";

	DWORD cbWritten;
	WriteFile(m_hLogFile, static_cast<LPCSTR>(logString), logString.GetLength(), &cbWritten, NULL); 
}

void CIO::LogWrite(BYTE* buffer, WORD length)
{
	if (m_hLogFile == INVALID_HANDLE_VALUE)
	{
		return;
	}

	SetFilePointer(m_hLogFile, 0, NULL, FILE_END);

	// write datetime
	CStringA logString;
	COleDateTime logTime = COleDateTime::GetCurrentTime();
	if (logTime.GetHour() != m_odtLastLogTime.GetHour())
	{
		CycleLogFile();
	}

	// Need to check again, because CycleLogFile could have failed
	if (m_hLogFile == INVALID_HANDLE_VALUE)
	{
		return;
	}

	m_odtLastLogTime = logTime;

	logString = logTime.Format();
	logString += "\r\n";

	// write direction indicator
	logString += ">>\r\n";

	// write bytes 16 at a time
	WORD nLine, nPosition;
	BYTE pbLoggedSegment[16];
	CStringA strTemp;
	for (nLine = 0; nLine < length / 16; nLine++)
	{
		ZeroMemory(pbLoggedSegment, 16);
		for (nPosition = 0; nPosition < 16; nPosition++)
		{
			strTemp.Format("%02X ", buffer[(nLine * 16) + nPosition]);
			logString += strTemp;
			if ('\x00' == buffer[(nLine * 16) + nPosition])
			{
				pbLoggedSegment[nPosition] = '\x80';
			}
			else
			{
				pbLoggedSegment[nPosition] = buffer[(nLine * 16) + nPosition];
			}
		}

		char cPrintableByte;

		for (nPosition = 0; nPosition < 16; nPosition++)
		{
			if (isprint(pbLoggedSegment[nPosition]))
			{
				cPrintableByte = static_cast<char>(pbLoggedSegment[nPosition]);
			}
			else
			{
				cPrintableByte = '.';
			}
			logString += cPrintableByte;
		}
		logString += "\r\n";
	}

	// Write remaining bytes
	ZeroMemory(pbLoggedSegment, 16);
	int nRemainingSize;
	nRemainingSize = length - (nLine * 16);
	for (nPosition = 0; nPosition < nRemainingSize; nPosition++)
	{
		strTemp.Format("%02X ", buffer[nPosition]);
		logString += strTemp;
		if ('\x00' == buffer[(nLine * 16) + nPosition])
		{
			pbLoggedSegment[nPosition] = '\x80';
		}
		else
		{
			pbLoggedSegment[nPosition] = buffer[(nLine * 16) + nPosition];
		}
	}
	for (; nPosition < 16; nPosition++)
	{
		strTemp.Format("   ");
		logString += strTemp;
	}

	char cPrintableByte;

	for (nPosition = 0; nPosition < nRemainingSize; nPosition++)
	{
		if (isprint(buffer[(nLine * 16) + nPosition]))
		{
			cPrintableByte = static_cast<char>(pbLoggedSegment[nPosition]);
		}
		else
		{
			cPrintableByte = '.';
		}
		logString += cPrintableByte;
	}

	logString += "\r\n\r\n";

	DWORD cbWritten;
	WriteFile(m_hLogFile, static_cast<LPCSTR>(logString), logString.GetLength(), &cbWritten, NULL); 
}

void CIO::LogError()
{
	if (m_hLogFile == INVALID_HANDLE_VALUE)
	{
		return;
	}

	SetFilePointer(m_hLogFile, 0, NULL, FILE_END);

	// write datetime
	CStringA logString;
	COleDateTime logTime = COleDateTime::GetCurrentTime();
	if (logTime.GetHour() != m_odtLastLogTime.GetHour())
	{
		CycleLogFile();
	}

	// Need to check again, because CycleLogFile could have failed
	if (m_hLogFile == INVALID_HANDLE_VALUE)
	{
		return;
	}

	m_odtLastLogTime = logTime;

	logString = logTime.Format();
	logString += "\r\n";

	// write direction indicator
	logString += "COMMUNICATIONS ERROR\r\n\r\n";

	DWORD cbWritten;
	WriteFile(m_hLogFile, static_cast<LPCSTR>(logString), logString.GetLength(), &cbWritten, NULL);
}

void CIO::CycleLogFile()
{
	CloseHandle(m_hLogFile);
	CString csLogFile;
	csLogFile = m_csBaseLogFileName;
	m_odtLastLogTime = COleDateTime::GetCurrentTime();
	csLogFile += m_odtLastLogTime.Format(_T("%Y-%m-%d-%H"));
	csLogFile += _T(".log");
	m_hLogFile = CreateFile(csLogFile, 
							GENERIC_WRITE, 
							FILE_SHARE_READ, 
							NULL, 
							OPEN_ALWAYS,
							FILE_ATTRIBUTE_NORMAL,
							NULL);

	if (m_hLogFile == INVALID_HANDLE_VALUE)
	{	
		CString oError;
		oError.Format(_T("Unable to create log file %s"), csLogFile);
		theApp.LogError(oError);
	}
}
int CIO::getBit(int number, int position) {
    unsigned int bitmask = 1 << position;
    return (number & bitmask) ? 1 : 0;
}
