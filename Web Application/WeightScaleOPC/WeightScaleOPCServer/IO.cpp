/******************************************************************************

	FILE NAME:		IO.cpp


	PURPOSE:			Implementation of the CIO


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
		02/03/2005	WG				7.1.0.1 - Changed to return OPC_QUALITY_GOOD when
										card reader data is unavailable

		08/04/2005 W.Gray			7.1.0.2 - Changed to not log NO06 error for Reset Card Reader
										Data 

		02/13/2007	W.Gray		7.1.0.3 - Added call to RemoveTagFromGroupItems at end
										of tag destructor so that m_pCurrentTag in OPCServer
										could be reset if current browse position was to tag

		03-05-2008	B. Schaal	Changed create events to use auto reset instead of manual for serial
										communications since there was no resetevent call being made. Changed serial
										timeout from two seconds to four.

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


CTag* CTag::AddLeaf(	WEIGHTSCALE_TAG_TYPE	WeightScaleTagType,
							LPCTSTR					szName,
							BYTE						bAddress,
							DWORD						dwAccessRights,
							VARTYPE					NativeType,
							CIO*						pIO)
{
	CTag* pTag=new CTag(szName);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_WeightScaleTagType=WeightScaleTagType;
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

CTag* CTag::AddLeaf(	WEIGHTSCALE_TAG_TYPE	WeightScaleTagType,
							INT						iID,
							BYTE						bAddress,
							DWORD						dwAccessRights,
							VARTYPE					NativeType,
							CIO*						pIO)
{
	CTag* pTag=new CTag(iID);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_WeightScaleTagType=WeightScaleTagType;
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

CIO::CIO(WEIGHTSCALE_TYPE				WeightScaleType,
			BYTE								lDeviceID,
			LONG								lIndex,
			LPCTSTR							szPort,
			WEIGHTSCALE_BAUD				dwBaud,
			WEIGHTSCALE_DATA_BITS		bDataBits,
			WEIGHTSCALE_PARITY			bParity,
			WEIGHTSCALE_STOP_BITS		bStopBits)
{
	m_hKillEvent=NULL;
	m_pScanThread=NULL;
	m_WeightScaleType=WeightScaleType;
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
	HKEY hWeightScaleOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	if (ERROR_SUCCESS == RegOpenKeyEx(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Varec\\WeightScaleOPC"),0,KEY_READ,&hWeightScaleOPCKey))
	{
		DWORD dwLogPorts;
		DWORD cbLogPorts = 4;
		if (ERROR_SUCCESS == RegQueryValueEx(hWeightScaleOPCKey, _T("LogPorts"), NULL, NULL, reinterpret_cast<LPBYTE>(&dwLogPorts), &cbLogPorts))
		{
			if (0 != dwLogPorts)
			{
				DWORD cbBasePath = 0;
				if (ERROR_SUCCESS == RegQueryValueEx(hWeightScaleOPCKey, _T("LogBasePath"), NULL, NULL, NULL, &cbBasePath))
				{
					logMsg.Format(_T("IO.Init() - LogBasePath size of %d"), cbBasePath);
					OutputDebugString((LPCTSTR)logMsg);
					LPTSTR pszBasePath;
					pszBasePath = new TCHAR[cbBasePath + 1];
					if (pszBasePath != NULL)
					{
						if (ERROR_SUCCESS == RegQueryValueEx(hWeightScaleOPCKey, _T("LogBasePath"), NULL, NULL, reinterpret_cast<LPBYTE>(pszBasePath), &cbBasePath))
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
		RegCloseKey(hWeightScaleOPCKey);
		hWeightScaleOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
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
		CloseHandle(m_hPort);

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
								FILE_FLAG_OVERLAPPED,
								NULL);					  	

	if(m_hPort == INVALID_HANDLE_VALUE)
	{
		if(!m_bCommFailLogged)
		{
			CString oError;
			oError.Format(_T("IO Error : CreateFile on : %s"),m_oPort);
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

	if(m_WeightScaleType == FAIRBANKS_90_164)
		Dcb.EvtChar=0x04;
	else if(m_WeightScaleType == SIPELARIES_ASCII)
		Dcb.EvtChar=0x03;
	else if (m_WeightScaleType == RICE_LAKE_720I || m_WeightScaleType == REVUELTARADMTX)
		Dcb.EvtChar=0x0A;
	else 
		Dcb.EvtChar=0x0D;

	if(!SetCommState(m_hPort,&Dcb))
	{
		CString oError;
		oError.Format(_T("IO Error : SetCommState Error on : %s"),m_oPort);
		theApp.LogError(oError);
		m_bCommFailLogged=TRUE;
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

	return S_OK;
}

BYTE CIO::LRC(PBYTE pbBuffer,WORD wLength)
{
	BYTE bLRC=0;

	for(WORD wItem=0;wItem < wLength;wItem++)
		bLRC^=pbBuffer[wItem];

	return bLRC;
}

HRESULT CIO::PrepareRequest(CTag* pTag,BOOL bWrite)
{
	if(m_WeightScaleType == TOLEDO_8142)
	{
		if(pTag->m_WeightScaleTagType == WEIGHT_TAG)
		{
			m_bXmtBuffer[0]=0x02;  // Hex 02 is the start of all scale commands
			m_bXmtBuffer[1]=0x32;  // Scale device #1 (hex 33 would be device #2 etc.)
			m_bXmtBuffer[2]='U';   // This is an upload command
			m_bXmtBuffer[3]=0x42;  // Hex 42 is the command to return indicated weight
			m_bXmtBuffer[4]=0x0D;  // Carraige Return
			m_wXmtLength=5;
		}
		else if(pTag->m_WeightScaleTagType == SCALE_IN_MOTION_TAG)
		{
			m_bXmtBuffer[0]=0x02;  // Hex 02 is the start of all scale commands
			m_bXmtBuffer[1]=0x32;  // Scale device #1 (hex 33 would be device #2 etc.)
			m_bXmtBuffer[2]='U';   // This is an upload command
			m_bXmtBuffer[3]=0x49;  // Hex 49 is the command to return scale status
			m_bXmtBuffer[4]=0x0D;  // Carraige Return
			m_wXmtLength=5;
		}
	}

	else if(m_WeightScaleType == FAIRBANKS_90_164)
	{
		m_bXmtBuffer[0]=0x0D;	// Hex 0D says return the strings
		m_wXmtLength=1;
	}

	else if(m_WeightScaleType == BRECHBUHLER_UMC600 ||
		m_WeightScaleType == SIPELARIES_ASCII ||
		m_WeightScaleType == RICE_LAKE_720I ||
		m_WeightScaleType == REVUELTARADMTX)
	{
		// Weight Scale continuously spits out a string; we do not send a request
		m_wXmtLength=0;
	}

	else if(m_WeightScaleType == METTLER_SICS)
	{
		m_bXmtBuffer[0] = 'S';   // 'SI' is the command for Send Weight Immediate
		m_bXmtBuffer[1] = 'I';   // 'SI' is the command for Send Weight Immediate
		m_bXmtBuffer[2] = 0x0d;  // Mettler documentation specified send line end with line feed.
		m_bXmtBuffer[3] = 0x0a;
		m_wXmtLength = 4;
	}

	return S_OK;
}

unsigned int CIO::CRC16(unsigned char *byte, int count)
{
	register int ppp;
	int bytes = 0, j = 0;

	unsigned int crc = 0xFFFF;

	// strip and set the required varibles
	// do calculation on ascii only starting at position 2
		for (ppp=0; ppp<count;ppp++){
			crc = crc ^ byte[ppp];
			for(j=0; j<8; ++j){
				if (crc & 0x01){
					crc = crc >> 1;
					crc = crc ^ 0xa001;
				}
				else{
					crc = crc >> 1;
				}
			}
		}

	return(crc);

}	

HRESULT CIO::ProcessResponse(CTag* pTag,HRESULT hr)
{
	if(m_WeightScaleType == TOLEDO_8142)
	{
		if(pTag->m_WeightScaleTagType == WEIGHT_TAG)
		{
			if(12 != m_wRcvLength
         || 0x32 != m_bRcvBuffer[1]				// Did it come from device #1
         || 'U' != m_bRcvBuffer[2]				// Is it an Upload command
         || 0x42 != m_bRcvBuffer[3])			// Is it the 'Indicated Weight' command?
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			else
			{
				//  The transmission looks okay.  Let's convert it to a weight
				CString resultString;
         
				//  Chars 4 thru 10
				for (int i=4; i <= 10; i++)
					resultString += (char)m_bRcvBuffer[i];
				
				LONG lWeight;
				int numRead = swscanf(resultString,_T("%d"), &lWeight);
				if(numRead <= 0)
					pTag->m_wQuality=OPC_QUALITY_BAD;
				else
				{
					pTag->m_Value=lWeight;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
				}
			}
		}

		else if(pTag->m_WeightScaleTagType == SCALE_IN_MOTION_TAG)
		{
			if(11 != m_wRcvLength
         || 0x32 != m_bRcvBuffer[1]				// Did it come from device #1
         || 'U' != m_bRcvBuffer[2]				// Is it an Upload command
         || 0x49 != m_bRcvBuffer[3])			// Is it the status command?
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			else
			{
				//	Status byte B bit definitions
				//
				//  Bit  Function
				//   0   Gross/Net, Net = 1
				//   1   Under Zero, Negative = 1
				//   2   Overcapacity = 1
				//   3   Motion, In motion = 1
				//   4   lb/kg, kg = 1
				//   5   Always a 1
				//   6   Powerup not zeroed = 1
				//
				pTag->m_Value.vt=VT_BOOL;
				if (m_bRcvBuffer[5] & 0x08)
					pTag->m_Value.boolVal=VARIANT_TRUE;
				else
					pTag->m_Value.boolVal=VARIANT_FALSE;

				pTag->m_wQuality=OPC_QUALITY_GOOD;

			}
		}
	}
		
	else if(m_WeightScaleType == FAIRBANKS_90_164)
	{
		if(m_wRcvLength < 16
		|| 0x04 != m_bRcvBuffer[m_wRcvLength-1])
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
		else
		{
			CString resultString;

			//  Chars 0 thru 6 make up the weight string
			for (int i=0;i <= 6;i++)
				resultString += (char) m_bRcvBuffer[i];

			LONG lWeight;
			int numRead = swscanf(resultString,_T("%d"), &lWeight);
			if(numRead <= 0)
				pTag->m_wQuality=OPC_QUALITY_BAD;
			else
			{
				pTag->m_Value=lWeight;
				pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
		}
	}

	else if(m_WeightScaleType == BRECHBUHLER_UMC600)
	{
		CTag* pWeightTag;
		CTag* pScaleInMotionTag;
		if(pTag->m_WeightScaleTagType == WEIGHT_TAG)
		{
			pWeightTag=pTag;
			pScaleInMotionTag=pTag->m_pParent->m_Leaf.GetTail();
		}
		else
		{
			pWeightTag=pTag->m_pParent->m_Leaf.GetHead();
			pScaleInMotionTag=pTag;
		}

		if(m_wRcvLength < 12)
		{
			pWeightTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			pScaleInMotionTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
		}

		else
		{
			// Look for the last Carrage Return, extra data may have been received
			WORD wRcvBufferEnd=m_wRcvLength-1;
			while(wRcvBufferEnd >= 12 && m_bRcvBuffer[wRcvBufferEnd] != 0x0D)
				wRcvBufferEnd--;

			if(m_bRcvBuffer[wRcvBufferEnd] != 0x0D)
			{
				pWeightTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
				pScaleInMotionTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			}

			else
			{
				CString resultString;

				//  Chars 2 thru 8 make up the weight string
				for (int i=2;i <= 8;i++)
					resultString += (char) m_bRcvBuffer[wRcvBufferEnd-12+i];

				LONG lWeight;
				int numRead = swscanf(resultString,_T("%d"), &lWeight);
				if(numRead <= 0)
					pWeightTag->m_wQuality=OPC_QUALITY_BAD;
				else
				{
					pWeightTag->m_Value=lWeight;
					pWeightTag->m_wQuality=OPC_QUALITY_GOOD;
					pWeightTag->m_bCurrent=TRUE;
				}


				pScaleInMotionTag->m_Value.vt=VT_BOOL;

				if(m_bRcvBuffer[wRcvBufferEnd-1] == 'M')				// 'M' is for Motion
					pScaleInMotionTag->m_Value.boolVal=VARIANT_TRUE;

				else if(m_bRcvBuffer[wRcvBufferEnd-3] != 'L'			// 'L' is lbs
				|| m_bRcvBuffer[wRcvBufferEnd-2] != 'G'				// 'G' is gross weight
				|| m_bRcvBuffer[wRcvBufferEnd-1] != ' ')				// ' ' is for no other status
					pScaleInMotionTag->m_Value.boolVal=VARIANT_TRUE;

				else
					pScaleInMotionTag->m_Value.boolVal=VARIANT_FALSE;

				pScaleInMotionTag->m_wQuality=OPC_QUALITY_GOOD;
				pScaleInMotionTag->m_bCurrent=TRUE;
			} 
		}
	}
	else if(m_WeightScaleType == SIPELARIES_ASCII)
	{
		if(pTag->m_WeightScaleTagType == WEIGHT_TAG)	// net volume
		{
			int iSTXPosition = -1;
			int iETXPosition = -1;

			// find the stx and etx positions in the message
			for(int iLoop = 0;iLoop < m_wRcvLength;iLoop++)
			{
				if (m_bRcvBuffer[iLoop] == 0x02 &&
					iSTXPosition == -1)
				{
					iSTXPosition = iLoop;
				}
				else if (m_bRcvBuffer[iLoop] == 0x03 &&
					iETXPosition == -1 &&
					iSTXPosition > -1)
				{
					iETXPosition = iLoop;
				}
			}

			if(iSTXPosition == -1 ||
				iETXPosition == -1 ||
				iSTXPosition > iETXPosition)
			{
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			}
			else
			{
				// Protocal
				//<STX>AAAAAAAAAAA<CR><LF>BBBBBBBBBB<CR><LF>CCCCCCCCCC<CR><LF><ETX>
				//Where:
				//AAAAAAAAAA = ID space filed to a length of 10
				//BBBBBBBBBB = Net space filled to a length of 10
				//CCCCCCCCCC = Flag word space filled to a length of 10
				// parse the received message
				CString resultString;
				int iNumLFToSkip = 0;

				// Weight
				resultString = "";
				iNumLFToSkip = 0;
				for (int i=iSTXPosition + 1;i <= iETXPosition;i++)
				{
					if(m_bRcvBuffer[i] == 0x0A)
					{
						++iNumLFToSkip;
					}
					else if (iNumLFToSkip == 1 &&
						m_bRcvBuffer[i] != 0x0D &&
						m_bRcvBuffer[i] != 0x03)
						resultString += (char) m_bRcvBuffer[i];
				}

				LONG lWeight;
				int numRead = swscanf(resultString,_T("%d"), &lWeight);
				if(numRead <= 0)
					pTag->m_wQuality=OPC_QUALITY_BAD;
				else
				{
					pTag->m_Value=lWeight;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
				}
			}
		}

		else if(pTag->m_WeightScaleTagType == SCALE_IN_MOTION_TAG)
		{
				//	Status byte B bit definitions
				//
				//  Bit  Function
				//   0   display negative
				//   1   zero center
				//   2   weight unstable
				//   3   mode eto
				//   4   gross negative
				//   5   - not used
				//   6   low power mode (display off)
				//   7   taking inhibit zero
			int iSTXPosition = -1;
			int iETXPosition = -1;

			// find the stx and etx positions in the message
			for(int iLoop = 0;iLoop < m_wRcvLength;iLoop++)
			{
				if (m_bRcvBuffer[iLoop] == 0x02 &&
					iSTXPosition == -1)
				{
					iSTXPosition = iLoop;
				}
				else if (m_bRcvBuffer[iLoop] == 0x03 &&
					iETXPosition == -1 &&
					iSTXPosition > -1)
				{
					iETXPosition = iLoop;
				}
			}

			if(iSTXPosition == -1 ||
				iETXPosition == -1 ||
				iSTXPosition > iETXPosition)
			{
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			}
			else
			{
				// Protocal
				//<STX>AAAAAAAAAA<CR><LF>BBBBBBBBBB<CR><LF>CCCCCCCCCC<CR><LF><ETX>
				//Where:
				//AAAAAAAAAA = ID space filed to a length of 10
				//BBBBBBBBBB = Net space filled to a length of 10
				//CCCCCCCCCC = Flag word space filled to a length of 10
				// parse the received message
				// Flag Field
				CString resultString;
				int iNumLFToSkip = 0;
				resultString = "";
				iNumLFToSkip = 0;
				for (int i=iSTXPosition + 1;i <= iETXPosition;i++)
				{
					if(m_bRcvBuffer[i] == 0x0A)
					{
						++iNumLFToSkip;
					}
					else if (iNumLFToSkip == 2)
						resultString += (char) m_bRcvBuffer[i];
				}


				LONG lStatus;
				int numRead = swscanf(resultString,_T("%d"), &lStatus);
				if(numRead <= 0)
					pTag->m_wQuality=OPC_QUALITY_BAD;
				else
				{
					pTag->m_Value.vt=VT_BOOL;
					if(lStatus & 0x04)
						pTag->m_Value.boolVal=VARIANT_TRUE;
					else
						pTag->m_Value.boolVal=VARIANT_FALSE;

					pTag->m_wQuality=OPC_QUALITY_GOOD;
				}
			}
		}
	}
	else if (m_WeightScaleType == METTLER_SICS)
	{
		if (pTag->m_WeightScaleTagType == WEIGHT_TAG || pTag->m_WeightScaleTagType == SCALE_IN_MOTION_TAG)
		{
			// response to 'SI' is simply 'S S     20 kg' or 'S D     20 kg'
			// uncertain of the number of spaces between the second S and the number; all we
			// got was a screen capture (image, not text).
			// For second character, S indicates stable weight while D indicates not stable (scale in motion)
			int iSTXPosition = -1;
			int iETXPosition = -1;

			// find the stx and etx positions in the message
			// start character appears to be an echo of the command sent.
			for(int iLoop = 0;iLoop < m_wRcvLength;iLoop++)
			{
				if (m_bRcvBuffer[iLoop] == 'S' &&
					iSTXPosition == -1)
				{
					iSTXPosition = iLoop;
				}
			}

			iETXPosition = m_wRcvLength - 1; // There does not appear an end of transmission character
			if(iSTXPosition == -1 ||
				iETXPosition <= -1 ||
				iSTXPosition > iETXPosition)
			{
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			}
			else
			{
				// Protocol
				//S q nnnnnnn uu
				//Where:
				//q = quality - S means stable weight, D means unstable weight (scale in motion)
				//nnnnnnn = weight number space filed to a length of ??
				//uu = 2 character units
				// parse the received message
				CString resultString;
				int iNumLFToSkip = 0;

				// skip past the S<space>
				iSTXPosition += 2;
				
				// read the quality
				char chQuality;
				bool bStable = false;
				chQuality = m_bRcvBuffer[iSTXPosition];
				iSTXPosition += 1;
				if (chQuality == 'S')
				{
					bStable = true;
				}

				// Pull end marker in past the units, then drop a null there to terminate the string
				// note that we assume that the last character received is part of the unit string, not a space
				while (iETXPosition > 0 && m_bRcvBuffer[iETXPosition] != ' ')
				{
					iETXPosition--;
				}
				m_bRcvBuffer[iETXPosition] = 0x00;

				if (iSTXPosition < iETXPosition)
				{
					// at this point, &(m_bRcvBuffer[iSTXPosition]) should point to a space-padded number
					LONG lWeight;
					int numRead = sscanf((LPSTR) &(m_bRcvBuffer[iSTXPosition]),"%d", &lWeight);
					if(numRead <= 0)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
					}
					else
					{
						if (pTag->m_WeightScaleTagType == WEIGHT_TAG)
						{
							pTag->m_Value=lWeight;
						}
						else if (pTag->m_WeightScaleTagType == SCALE_IN_MOTION_TAG)
						{
							pTag->m_Value.vt=VT_BOOL;
							if(bStable)
							{
								pTag->m_Value.boolVal=VARIANT_FALSE;
							}
							else
							{
								pTag->m_Value.boolVal=VARIANT_TRUE;
							}
						}
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_bCurrent=TRUE;
					}
				}
				else
				{
					pTag->m_wQuality = OPC_QUALITY_BAD;
				}
			}
		}
	}
	else if(m_WeightScaleType == RICE_LAKE_720I)
	{
		// Protocal
		//<STX><POL><WWWWWWW><UNIT><G/N><S><CR><LF>
		//Where:
		//STX = 0x02
		//POL is polarity - space for positive, '-' for negative.
		//WWWWWWW = 7 position (7 digit or 6 digit + 1 decimal point) floating point weight.  Right justified,
		//  space padded.
		//UNIT:
		//  L -> pounds
		//  K -> kilograms
		//  T -> tons
		//  G -> grains
		//  <space> -> grams
		//  O -> ounces
		//G/N
		//  G = Gross
		//  N = Net
		//S: status
		//  ' ' -> valid
		//  I -> invalid
		//  M -> motion
		//  O -> over/under range
		//  Z -> COZ (??)
		CTag* pWeightTag;
		CTag* pScaleInMotionTag;
		if(pTag->m_WeightScaleTagType == WEIGHT_TAG)
		{
			pWeightTag=pTag;
			pScaleInMotionTag=pTag->m_pParent->m_Leaf.GetTail();
		}
		else
		{
			pWeightTag=pTag->m_pParent->m_Leaf.GetHead();
			pScaleInMotionTag=pTag;
		}

		CString logMessage;

		if(m_wRcvLength != 14)
		{
			// Packet from the Rice Lake should be 14 bytes; no more, no less
			pWeightTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			pScaleInMotionTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			logMessage.Format(_T("Received bad message - length is %d instead of 14\r\n"), m_wRcvLength);
			LogMessage((LPCTSTR)logMessage);
		}
		else
		{
			// Look for the last Carrage Return, extra data may have been received
			WORD wRcvBufferEnd=m_wRcvLength-1;
			while(wRcvBufferEnd >= 13 && m_bRcvBuffer[wRcvBufferEnd] != 0x0D)
				wRcvBufferEnd--;

			if(m_bRcvBuffer[wRcvBufferEnd] != 0x0D)
			{
				pWeightTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
				pScaleInMotionTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
				logMessage.Format(_T("Received bad message - doesn't end in CRLF\r\n"));
				LogMessage((LPCTSTR)logMessage);
			}
			else
			{
				CString resultString;
				
				// second character is a sign indicator
				int nSign = (char) m_bRcvBuffer[1] == '-' ? -1 : 1;

				//  3rd through 9th make up the weight string
				for (int i=2;i <= 8;i++)
					resultString += (char) m_bRcvBuffer[i];

				resultString = resultString.TrimLeft();

				LONG lWeight;
				int numRead = swscanf((LPCTSTR)resultString ,_T("%d"), &lWeight);
				if(numRead <= 0)
				{
					pWeightTag->m_wQuality=OPC_QUALITY_BAD;
					logMessage.Format(_T("Bad quality for weight, zero characters scanned from string %s\r\n"), resultString);
					LogMessage((LPCTSTR)logMessage);
				}
				else
				{
					pWeightTag->m_Value=lWeight * nSign;
					pWeightTag->m_wQuality=OPC_QUALITY_GOOD;
					pWeightTag->m_bCurrent=TRUE;
				}


				pScaleInMotionTag->m_Value.vt=VT_BOOL;

				if(m_bRcvBuffer[11] == ' ')				// ' ' is good weight
					pScaleInMotionTag->m_Value.boolVal=VARIANT_FALSE;
				else  // anything else is  in motion, out of range, or invalid 
					pScaleInMotionTag->m_Value.boolVal=VARIANT_TRUE;

				pScaleInMotionTag->m_wQuality=OPC_QUALITY_GOOD;
				pScaleInMotionTag->m_bCurrent=TRUE;
			} 
		}
	}
	
	if(m_WeightScaleType == REVUELTARADMTX)
	{
		//	Status byte B bit definitions
		//
		//  Bit  Function
		//   0 -1 Staus  OL: overload, UL: under weight, ST: stable ER: error 
		//   2 seperator ,
		//   3 - 4  Mode TA: tara
		//   5 seperator ,
		//   6 - sign - neg, + pos
		//   7 - 13 weight
		//   14 - 15 Unit
		//   16 CR
		//   17 LF

		if(pTag->m_WeightScaleTagType == WEIGHT_TAG)
		{
			if( m_wRcvLength < 18
				 || 0x0D != m_bRcvBuffer[16]				// Did it come from device #1
				 || 0x0A != m_bRcvBuffer[17])				// Is it an Upload command	
					 pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			else
			{
				//  The transmission looks okay.  Let's convert it to a weight
				CString resultString;
         
				//  Chars 4 thru 10
				for (int i=7; i <= 13; i++)
					resultString += (char)m_bRcvBuffer[i];
				 theApp.LogError(resultString);
				LONG lWeight;
				int numRead = swscanf(resultString,_T("%d"), &lWeight);
				if(numRead <= 0)
					pTag->m_wQuality=OPC_QUALITY_BAD;
				else
				{
					pTag->m_Value=lWeight;
					pTag->m_wQuality=OPC_QUALITY_GOOD;
				}
			}
		}

		else if(pTag->m_WeightScaleTagType == SCALE_IN_MOTION_TAG)
		{
			if( m_wRcvLength < 18
				 || 0x0D != m_bRcvBuffer[16]				// Did it come from device #1
				 || 0x0A != m_bRcvBuffer[17])				// Is it an Upload command	
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			else
			{
				pTag->m_Value.vt=VT_BOOL;

				if((char)m_bRcvBuffer[0] == 'S' && (char)m_bRcvBuffer[1] == 'T')
					pTag->m_Value.boolVal=VARIANT_FALSE;
				else					
					pTag->m_Value.boolVal=VARIANT_TRUE;

				pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
		}
	}

	return hr;
}

void CIO::ReportError(CTag* pTag)
{
	CString oCode;
	CString oError;
	oError.Format(_T("IO Error : %s for %s"),oCode,pTag->GetPathName());
	theApp.LogError(oError);
}

HRESULT CIO::PerformIO(CTag* pTag)
{
	if(m_hPort == INVALID_HANDLE_VALUE)
	{
		HRESULT hr=OpenComPort();
		if(FAILED(hr))
		{
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
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
		DWORD    dwRcvBufferOffset=0;
		DWORD		dwTimeoutValue = 2000;

		if(m_WeightScaleType == BRECHBUHLER_UMC600 ||
			m_WeightScaleType == SIPELARIES_ASCII ||
			m_WeightScaleType == RICE_LAKE_720I ||
			m_WeightScaleType == REVUELTARADMTX)
			dwTimeoutValue = 1000;

		m_wRcvLength=0;

		if(!ClearCommError(m_hPort,&dwCommErrFlags,&ComStat))
			continue;

		if(!PurgeComm(	m_hPort,
							PURGE_RXCLEAR |
							PURGE_RXABORT |
							PURGE_TXCLEAR |
							PURGE_TXABORT))
			continue;

		ReadAgain:

		// Write the request
		if(m_wXmtLength)
		{
			WriteOverLapped.Offset=0;
			WriteOverLapped.OffsetHigh=0;
			LogWrite(m_bXmtBuffer,m_wXmtLength);
			if(!WriteFile(m_hPort,m_bXmtBuffer,m_wXmtLength,&dwNumberOfBytesWritten,&WriteOverLapped))
			{
				if(GetLastError() != ERROR_IO_PENDING)
					continue;

				if(!GetOverlappedResult(m_hPort,&WriteOverLapped,&dwNumberOfBytesWritten,TRUE))
					continue;
			}

			if(m_wXmtLength != dwNumberOfBytesWritten)
				continue;
		}

		// Read the response
	 	if(!SetCommMask(m_hPort,EV_ERR | EV_RXFLAG))
			continue;

	 	if(!WaitCommEvent(m_hPort,&dwCommEvtFlags,&CommOverLapped)
		&& GetLastError() != ERROR_IO_PENDING)
			continue;

		switch(WaitForSingleObject(CommOverLapped.hEvent,dwTimeoutValue))
		{
			case WAIT_OBJECT_0:
	   		if((dwCommEvtFlags & EV_ERR ) == EV_ERR)
					continue;

   			else if((dwCommEvtFlags & EV_RXFLAG ) == EV_RXFLAG )
				{
					if(!ClearCommError(m_hPort,&dwCommErrFlags,&ComStat))
						continue;

					if(ComStat.cbInQue > (BUFFER_MAX-1)-dwRcvBufferOffset)
						continue;

					ReadOverLapped.Offset=0;
					ReadOverLapped.OffsetHigh=0;
					if(!ReadFile(m_hPort,&m_bRcvBuffer[dwRcvBufferOffset],ComStat.cbInQue,&dwNumberOfBytesRead,&ReadOverLapped)
					&& GetLastError() != ERROR_IO_PENDING )
						continue;

				 	if(!GetOverlappedResult(m_hPort,&ReadOverLapped,&dwNumberOfBytesRead,TRUE))
						continue;

					break;
				}
				else
				{
					continue;
				}

	      case WAIT_TIMEOUT:
				if(pTag->m_wQuality == OPC_QUALITY_COMM_FAILURE)
				{
					CloseHandle(m_hPort);
					m_hPort=INVALID_HANDLE_VALUE;
					return E_FAIL;
				}
				else
					continue;

	      case WAIT_FAILED:
			default:
				continue;
		}

		// It looks as though the BRECHBUHLER_UMC600 repeatedly sends out a string
		// terminated by CR.  We may have clipped a string with the initial
		// PurgeComm.  Look for the Carrage Return and if < 12 characters.
		// read again to get a complete message.
		if(m_WeightScaleType == BRECHBUHLER_UMC600
		&& dwRcvBufferOffset == 0
		&& dwNumberOfBytesRead >= 0)
		{
			DWORD wRcvEnd=dwNumberOfBytesRead;
			while(wRcvEnd > 0 && m_bRcvBuffer[wRcvEnd-1] != 0x0D)
				wRcvEnd--;

			if(wRcvEnd < 12)
			{
				dwRcvBufferOffset+=dwNumberOfBytesRead;
				goto ReadAgain;
			}
		}
		if(m_WeightScaleType == SIPELARIES_ASCII
		&& dwRcvBufferOffset == 0
		&& dwNumberOfBytesRead >= 0)
		{
			DWORD wRcvEnd=dwNumberOfBytesRead;
			while(wRcvEnd > 0 && m_bRcvBuffer[wRcvEnd-1] != 0x03)
				wRcvEnd--;

			if(wRcvEnd < 38)
			{
				dwRcvBufferOffset+=dwNumberOfBytesRead;
				goto ReadAgain;
			}
		}

		// For Rice Lake at least (probably need to consider Brechbuhler and Sipel as well),
		// if the PurgeComm clipped the message, then we got only the tail end of the message.  
		// There is no way we can use that to assemble a complete message; reset the receive buffer
		// and read again; we'll get the next send.
		if(m_WeightScaleType == RICE_LAKE_720I
		&& dwRcvBufferOffset == 0
		&& dwNumberOfBytesRead >= 0)
		{
			DWORD wRcvEnd=dwNumberOfBytesRead;
			while(wRcvEnd > 0 && m_bRcvBuffer[wRcvEnd-1] != 0x0D)
				wRcvEnd--;

			if(wRcvEnd < 13)
			{
				dwRcvBufferOffset = 0; // The segment we received previously is useless to us.  Try again to get a complete message.
				goto ReadAgain;
			}
		}

		if(m_WeightScaleType == REVUELTARADMTX
		&& dwRcvBufferOffset == 0
		&& dwNumberOfBytesRead >= 0)
		{
			DWORD wRcvEnd=dwNumberOfBytesRead;
			while(wRcvEnd > 0 && m_bRcvBuffer[wRcvEnd-1] != 0x0A)
				wRcvEnd--;

			if(wRcvEnd < 18)
			{
				dwRcvBufferOffset = 0;
				goto ReadAgain;
			}
		}

		m_wRcvLength+=(WORD) dwNumberOfBytesRead; 

		break;
	}

	LogRead(m_bRcvBuffer,m_wRcvLength);

	if(iTry == 3)
	{
		CloseHandle(m_hPort);
		m_hPort=INVALID_HANDLE_VALUE;
		pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
		return E_FAIL;
	}

	return S_OK;
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

	hr = ProcessResponse(pTag, PerformIO(pTag));
	if (FAILED(hr))
	{
		if(m_hPort != INVALID_HANDLE_VALUE)
		{
			CloseHandle(m_hPort);
			m_hPort = INVALID_HANDLE_VALUE;
		}
	}

	return hr;
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

	hr = ProcessResponse(pTag, PerformIO(pTag));
	if (FAILED(hr))
	{
		if(m_hPort != INVALID_HANDLE_VALUE)
		{
			CloseHandle(m_hPort);
			m_hPort = INVALID_HANDLE_VALUE;
		}
	}

	return hr;
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
										WEIGHTSCALE_BAUD			Baud,
										WEIGHTSCALE_DATA_BITS	DataBits,
										WEIGHTSCALE_PARITY		Parity,
										WEIGHTSCALE_STOP_BITS	StopBits)
{
	CSLock Lock(&m_cs);
	
	m_oPort=szPort;

	SetDeviceBaudRate(Baud);
	SetDeviceParity(Parity);
	SetDeviceDataBits(DataBits);
	SetDeviceStopBits(StopBits);

	m_bPortParametersChanged=TRUE;
}

void CIO::SetDeviceBaudRate(WEIGHTSCALE_BAUD dwBaud)
{
	switch(dwBaud)
	{
	case WEIGHTSCALE_BAUD_1200:
		m_dwBaud = CBR_1200;
		break;
	case WEIGHTSCALE_BAUD_2400:
		m_dwBaud = CBR_2400;
		break;
	case WEIGHTSCALE_BAUD_4800:
		m_dwBaud = CBR_4800;
		break;
	case WEIGHTSCALE_BAUD_9600:
		m_dwBaud = CBR_9600;
		break;
	case WEIGHTSCALE_BAUD_19200:
		m_dwBaud = CBR_19200;
		break;
	case WEIGHTSCALE_BAUD_38400:
		m_dwBaud = CBR_38400;
		break;
	default:
		m_dwBaud = CBR_9600;
		break;
	}
}

void CIO::SetDeviceParity(WEIGHTSCALE_PARITY bParity)
{
	switch(bParity)
	{
	case WEIGHTSCALE_PARITY_NONE:
		m_bParity=NOPARITY;
		break;
	case WEIGHTSCALE_PARITY_EVEN:
		m_bParity=EVENPARITY;
		break;
	case WEIGHTSCALE_PARITY_ODD:
		m_bParity=ODDPARITY;
		break;
	default:
		m_bParity=EVENPARITY;
		break;
	}
}

void CIO::SetDeviceDataBits(WEIGHTSCALE_DATA_BITS bDataBits)
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

void CIO::SetDeviceStopBits(WEIGHTSCALE_STOP_BITS bStopBits)
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

void CIO::LogMessage(LPCTSTR tszMessage)
{
	if (m_hLogFile == INVALID_HANDLE_VALUE)
	{
		return;
	}

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

#if defined(_UNICODE)
	char* pszMessage = NULL;
	int cbMessage;
	cbMessage = WideCharToMultiByte(CP_ACP, 0, tszMessage, -1, NULL, 0, NULL, NULL);
	if (cbMessage > 0)
	{
		pszMessage = (char *)malloc(cbMessage);
		if (pszMessage != NULL)
		{
			if (WideCharToMultiByte(CP_ACP, 0, tszMessage, -1, pszMessage, cbMessage, NULL, NULL) > 0)
			{
				logString += pszMessage;
			}
			free(pszMessage);
			pszMessage = NULL;
		}
	}
#else
	logString += tszMessage;
#endif

	logString += "\r\n\r\n";

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