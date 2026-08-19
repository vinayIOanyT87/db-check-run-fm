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
		02/03/2005	WG				7.0.0.1 - Changed to return OPC_QUALITY_GOOD when
										card reader data is unavailable

		08/04/2005	W.Gray		7.0.0.2 - Changed to not log NO06 error for Reset Card Reader
										Data 


		01/09/2007	W.Gray		7.1.0.1 - Changed to close port after each operation

		01/22/2007	W.Gray		7.1.0.2 - Added FS - Force Full Screen View (CSI 4079)

		02/13/2007	W.Gray		7.1.0.3 - Added call to RemoveTagFromGroupItems at end
										of tag destructor so that m_pCurrentTag in OPCServer
										could be reset if current browse position was to tag

		08/28/2007	W.Gray		7.1.1.1 - Added SetCommunicationsFailure and
										SetCommunicationsRestored

		10/05/2007	W.Gray		7.1.1.2 - Correction to RB response processing (CSI 5255)
		
		10/16/2007	W.Gray		7.1.1.3 - Change to ignore NO94 response to LO Command (CSI 5282)

		01/18/2008	W.Gray		7.2.1.0 - Added support for TCP/IP

		11/25/2008	W.Gray		7.6.1.0 - Changed OpenComPort to delay 5 seconds on error (CSI 6319)

		01/27/2009	W.Gray		7.6.2.0 - Added CloseComPort

		05/05/2009	W.Gray		7.6.2.1 - Revised to delay 500 msec after CloseComPort

		06/22/2009	W.Gray		7.4.6.4 - Revised scan logic to be more accurate in scan timing.

		02/22/2010	W.Gray		7.5.1.0 - Revised scan logic back to 7.4.6.0 method.
*******************************************************************************/

#include "StdAfx.h"
#include "io.h"
#include "DeviceManager.h"

extern CDeviceManager*		g_pDeviceManager;


// CTag
CTag::CTag(LPCTSTR szName)
{
	m_pParent=NULL;
	m_oName=szName;
	m_dwAccessRights=OPC_READABLE;
	m_dwScanCount=0;
	m_dwUpdateSequence=0;
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
	m_dwUpdateSequence=0;
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

CTag* CTag::AddBranch(LPCTSTR szName,CIO* pIO,CDevice* pDevice)
{
	CTag* pTag=new CTag(szName);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_bLeaf=FALSE;
	pTag->m_pParent=this;
	pTag->m_pIO=pIO;
	pTag->m_pDevice=pDevice;
	
	m_Branch.AddTail(pTag);
	return pTag;
}

CTag* CTag::AddBranch(INT iID,CIO* pIO,CDevice* pDevice)
{
	CTag* pTag=new CTag(iID);
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
							BYTE		bAddress,
							LPSTR		pszCommand,
							LPSTR		pszSection,
							DWORD		dwItem,
							DWORD		dwAccessRights,
							VARTYPE	NativeType,
							CIO*		pIO,
							CDevice* pDevice)
{
	CTag* pTag=new CTag(szName);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_bLeaf=TRUE;
	pTag->m_bAddress=bAddress;
	pTag->m_pszCommand=pszCommand;
	pTag->m_pszSection=pszSection;
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
							BYTE		bAddress,
							LPSTR		pszCommand,
							LPSTR		pszSection,
							DWORD		dwItem,
							DWORD		dwAccessRights,
							VARTYPE	NativeType,
							CIO*		pIO,
							CDevice* pDevice)
{
	CTag* pTag=new CTag(iID);
	if(!pTag)
		throw(CString(_T("Memory Allocation Error")));

	pTag->m_bLeaf=TRUE;
	pTag->m_bAddress=bAddress;
	pTag->m_pszCommand=pszCommand;
	pTag->m_pszSection=pszSection;
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



CIO::CIO(LONG						lIndex,
			LPCTSTR					szPort,
			ACCULOAD_BAUD			Baud,
			ACCULOAD_DATA_BITS	DataBits,
			ACCULOAD_PARITY		Parity,
			ACCULOAD_STOP_BITS	StopBits)
{
	m_lIndex=lIndex;
	m_oPort=szPort;
	m_Baud=Baud;
	m_DataBits=DataBits;
	m_Parity=Parity;
	m_StopBits=StopBits;

	m_bNetworkCommunications=FALSE;

	Init();
}

CIO::CIO(LPTSTR pIPAddress,	UINT uiPort)
{
	m_oIPAddress=pIPAddress;
	m_uiPort=uiPort;
	m_lIndex=0;
	m_bNetworkCommunications=TRUE;

	Init();
}

void CIO::Init()
{
	m_hKillEvent=NULL;
	m_pScanThread=NULL;
	InitializeCriticalSection(&m_cs);

	m_dwUseCount=0;
	m_iInactivityCounter=0;

	m_hSocket=NULL;
	m_pSocket=NULL;
	m_hPort=INVALID_HANDLE_VALUE;
	m_bCommFailLogged=FALSE;
	m_bPortParametersChanged=FALSE;

	// Set initial value
	m_dwCommunicationsTimeOut = 1000;

	m_hLogFile = INVALID_HANDLE_VALUE;

	ZeroMemory(&m_WriteOverLapped,sizeof(OVERLAPPED));
	ZeroMemory(&m_ReadOverLapped,sizeof(OVERLAPPED));
	ZeroMemory(&m_CommOverLapped,sizeof(OVERLAPPED));

	if(!m_bNetworkCommunications)
	{
		m_WriteOverLapped.hEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
		if(m_WriteOverLapped.hEvent == NULL )
			throw (CString(_T("IO: CreateEvent Error")));

		m_ReadOverLapped.hEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
		if(m_ReadOverLapped.hEvent == NULL )
			throw (CString(_T("IO: CreateEvent Error")));

		m_CommOverLapped.hEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
		if(m_CommOverLapped.hEvent == NULL )
			throw (CString(_T("IO: CreateEvent Error")));
	}

	// Check and start log file
	CString logMsg;
	HKEY hAcculoadOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	if (ERROR_SUCCESS == RegOpenKeyEx(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Varec\\AcculoadOPC"),0,KEY_READ,&hAcculoadOPCKey))
	{
		DWORD dwLogPorts;
		DWORD cbLogPorts = 4;
		if (ERROR_SUCCESS == RegQueryValueEx(hAcculoadOPCKey, _T("LogPorts"), NULL, NULL, reinterpret_cast<LPBYTE>(&dwLogPorts), &cbLogPorts))
		{
			if (0 != dwLogPorts)
			{
				DWORD cbBasePath = 0;
				if (ERROR_SUCCESS == RegQueryValueEx(hAcculoadOPCKey, _T("LogBasePath"), NULL, NULL, NULL, &cbBasePath))
				{
					logMsg.Format(_T("IO.Init() - LogBasePath size of %d"), cbBasePath);
					OutputDebugString((LPCTSTR)logMsg);
					LPTSTR pszBasePath;
					pszBasePath = new TCHAR[cbBasePath + 1];
					if (pszBasePath != NULL)
					{
						if (ERROR_SUCCESS == RegQueryValueEx(hAcculoadOPCKey, _T("LogBasePath"), NULL, NULL, reinterpret_cast<LPBYTE>(pszBasePath), &cbBasePath))
						{
							pszBasePath[cbBasePath] = _T('\0');
							CString csLogFile;
							m_csBaseLogFileName = pszBasePath;
							m_csBaseLogFileName.TrimRight(_T('\\'));
							m_csBaseLogFileName += (_T("\\"));
							if (m_bNetworkCommunications)
							{
								m_csBaseLogFileName += m_oIPAddress;
							}
							else
							{
								m_csBaseLogFileName += m_oPort;
							}
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
							else {
								// move the file pointer to the end, in case the OPC Server
								// gets shut down and restarted in the middle of an hour
								SetFilePointer(m_hLogFile, 0, NULL, FILE_END);
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
		RegCloseKey(hAcculoadOPCKey);
		hAcculoadOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
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
		oError.Format(_T("IO Error : tags orphaned in scan list"));
		theApp.LogError(oError);
	}

	DeleteCriticalSection(&m_cs);

	if (m_hLogFile != INVALID_HANDLE_VALUE)
	{
		CloseHandle(m_hLogFile);
		m_hLogFile = INVALID_HANDLE_VALUE;
	}

	if(!m_bNetworkCommunications)
	{
		if(m_hPort != INVALID_HANDLE_VALUE)
			CloseHandle(m_hPort);

		if(m_WriteOverLapped.hEvent != NULL )
			CloseHandle(m_WriteOverLapped.hEvent );

		if(m_ReadOverLapped.hEvent != NULL )
			CloseHandle(m_ReadOverLapped.hEvent );

		if(m_CommOverLapped.hEvent != NULL )
			CloseHandle(m_CommOverLapped.hEvent );
	}

	else
	{
		if(m_hSocket != NULL)
		{
			CAsyncSocket* pSocket=new CAsyncSocket();
			pSocket->Attach(m_hSocket);
			delete pSocket;
		}
	}
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


HRESULT CIO::OpenComPort(ACCULOAD_TYPE type)
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
			DWORD dw = GetLastError();
			CString oError;
			oError.Format(_T("IO Error : GetCommState on : %s, error = %u"), m_oPort, dw);
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}

		return E_FAIL;
	}

	switch(m_Baud)
	{
		case ACCULOAD_BAUD_1200:
			Dcb.BaudRate=CBR_1200;
			break;
		case ACCULOAD_BAUD_2400:
			Dcb.BaudRate=CBR_2400;
			break;
		case ACCULOAD_BAUD_4800:
			Dcb.BaudRate=CBR_4800;
			break;
		case ACCULOAD_BAUD_9600:
			Dcb.BaudRate=CBR_9600;
			break;
		case ACCULOAD_BAUD_19200:
			Dcb.BaudRate=CBR_19200;
			break;
		case ACCULOAD_BAUD_38400:
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
		case ACCULOAD_PARITY_NONE:
			Dcb.Parity=NOPARITY;
			break;
		case ACCULOAD_PARITY_EVEN:
			Dcb.Parity=EVENPARITY;
			break;
		case ACCULOAD_PARITY_ODD:
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
	Dcb.fRtsControl=RTS_CONTROL_ENABLE;
	Dcb.fDtrControl=DTR_CONTROL_ENABLE;
	Dcb.fAbortOnError=TRUE;
	if(type == SMITH_PROXIMITY)
		Dcb.EvtChar=0x03;
	else
		Dcb.EvtChar=0x7F;

	if(!SetCommState(m_hPort,&Dcb))
	{
		if(!m_bCommFailLogged)
		{
			DWORD dw = GetLastError();
			CString oError;
			oError.Format(_T("IO Error : SetCommState Error on : %s, error = %u"), m_oPort, dw);
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}

		return E_FAIL;
	}

	COMMTIMEOUTS	CommTimeouts;
	float	fTimeoutMult;
	fTimeoutMult = 1000 * 11 / (float)Dcb.BaudRate;
	if(fTimeoutMult < 1)
		fTimeoutMult=1;
	CommTimeouts.ReadIntervalTimeout 			= 0;
	CommTimeouts.ReadTotalTimeoutMultiplier 	= (DWORD)fTimeoutMult*2;
	CommTimeouts.ReadTotalTimeoutConstant 		= 1000;
	CommTimeouts.WriteTotalTimeoutMultiplier 	= (DWORD)fTimeoutMult*2;
	CommTimeouts.WriteTotalTimeoutConstant 	= 500;
	if(!SetCommTimeouts(m_hPort,&CommTimeouts))
	{
		if(!m_bCommFailLogged)
		{
			DWORD dw = GetLastError();
			CString oError;
			oError.Format(_T("IO Error : SetCommTimeouts Error on : %s, error = %u"), m_oPort, dw);
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}

		return E_FAIL;
	}

	m_bCommFailLogged=FALSE;
	m_bPortParametersChanged = FALSE;

	return S_OK;
}

HRESULT CIO::OpenSocket(CTag* pTag)
{
	m_pSocket=new CAsyncSocket();
	if(!m_pSocket)
	{
		if(!m_bCommFailLogged)
		{
			CString oError;
			oError.Format(_T("IO Error : CAsyncSocket"));
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}
		return E_FAIL;
	}

	if(m_hSocket != NULL)
	{
		m_pSocket->Attach(m_hSocket);
		m_hSocket=NULL;

		if(!m_pSocket->AsyncSelect(0))
		{
			if(!m_bCommFailLogged)
			{
				CString oError;
				oError.Format(_T("IO Error = %s : CAsyncSocket.AsyncSelect"),SocketError(m_pSocket->GetLastError()));
				theApp.LogError(oError);
				m_bCommFailLogged=TRUE;
			}
			delete m_pSocket;
			m_pSocket=NULL;
			return E_FAIL;
		}

		DWORD dwBlocking=0;
		if(!m_pSocket->IOCtl(FIONBIO,&dwBlocking))
		{
			if(!m_bCommFailLogged)
			{
				CString oError;
				oError.Format(_T("IO Error = %s : CAsyncSocket.IOCtl"),SocketError(m_pSocket->GetLastError()));
				theApp.LogError(oError);
				m_bCommFailLogged=TRUE;
			}
			delete m_pSocket;
			m_pSocket=NULL;
			return E_FAIL;
		}

		return S_OK;
	}

	if(!m_pSocket->Create(0,SOCK_STREAM,0,NULL))
	{
		if(!m_bCommFailLogged)
		{
			CString oError;
			oError.Format(_T("IO Error = %s : CAsyncSocket.Create"),SocketError(m_pSocket->GetLastError()));
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}
		delete m_pSocket;
		m_pSocket=NULL;
		return E_FAIL;
	}

	DWORD dwBlocking=0;
	if(!m_pSocket->IOCtl(FIONBIO,&dwBlocking))
	{
		if(!m_bCommFailLogged)
		{
			CString oError;
			oError.Format(_T("IO Error = %s : CAsyncSocket.IOCtl"),SocketError(m_pSocket->GetLastError()));
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}
		delete m_pSocket;
		m_pSocket=NULL;
		return E_FAIL;
	}

	if(!m_pSocket->Connect(m_oIPAddress,m_uiPort))
	{
		if(!m_bCommFailLogged)
		{
			CString oError;
			oError.Format(_T("IO Error = %s : CAsyncSocket.Connect"),SocketError(m_pSocket->GetLastError()));
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}
		delete m_pSocket;
		m_pSocket=NULL;
		return E_FAIL;
	}

	DWORD dwRcvTimeout=5000;
	if(!m_pSocket->SetSockOpt(SO_RCVTIMEO,&dwRcvTimeout,sizeof(dwRcvTimeout),SOL_SOCKET))
	{
		if(!m_bCommFailLogged)
		{
			CString oError;
			oError.Format(_T("IO Error = %s : CAsyncSocket.SetSockOpt"),SocketError(m_pSocket->GetLastError()));
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}
		delete m_pSocket;
		m_pSocket=NULL;
		return E_FAIL;
	}

	DWORD dwSndTimeout=5000;
	if(!m_pSocket->SetSockOpt(SO_SNDTIMEO,&dwSndTimeout,sizeof(dwSndTimeout),SOL_SOCKET))
	{
		if(!m_bCommFailLogged)
		{
			CString oError;
			oError.Format(_T("IO Error = %s : CAsyncSocket.SetSockOpt"),SocketError(m_pSocket->GetLastError()));
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}
		delete m_pSocket;
		m_pSocket=NULL;
		return E_FAIL;
	}

	m_bCommFailLogged=FALSE;

	return S_OK;
}

LPTSTR CIO::SocketError(DWORD dwError)
{
	switch(dwError)
	{
		case WSAECONNRESET:
			return _T("WSAECONNRESET");
		case WSANOTINITIALISED:
			return _T("WSANOTINITIALISED");
		case WSAENETDOWN:
			return _T("WSAENETDOWN");
		case WSAEADDRINUSE:
			return _T("WSAEADDRINUSE");
		case WSAEINPROGRESS:
			return _T("WSAEINPROGRESS");
		case WSAEADDRNOTAVAIL:
			return _T("WSAEADDRNOTAVAIL");
		case WSAEAFNOSUPPORT:
			return _T("WSAEAFNOSUPPORT");
		case WSAECONNREFUSED:
			return _T("WSAECONNREFUSED");
		case WSAEDESTADDRREQ:
			return _T("WSAEDESTADDRREQ");
		case WSAEFAULT:
			return _T("WSAEFAULT");
		case WSAEINVAL:
			return _T("WSAEINVAL");
		case WSAEISCONN:
			return _T("WSAEISCONN");
		case WSAEMFILE:
			return _T("WSAEMFILE");
		case WSAENETUNREACH:
			return _T("WSAENETUNREACH");
		case WSAENOBUFS:
			return _T("WSAENOBUFS");
		case WSAENOTSOCK:
			return _T("WSAENOTSOCK");
		case WSAETIMEDOUT:
			return _T("WSAETIMEDOUT");
		case WSAEWOULDBLOCK:
			return _T("WSAEWOULDBLOCK");
		default:
			return _T("Unknown");
	}
}



HRESULT CIO::ReadTag(CTag* pTag)
{
	// Do not read tags that are Read Writable
	if(pTag->m_dwAccessRights & OPC_READABLE
	&& pTag->m_dwAccessRights & OPC_WRITEABLE)
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

	m_iInactivityCounter=MAX_INACTIVITY;

	CoFileTimeNow(&pTag->m_Timestamp);

	CDevice* pDevice=pTag->m_pDevice;

	HRESULT hr=pDevice->PrepareRequest(pTag,FALSE);
	if(FAILED(hr))
	{
		pTag->m_wQuality=OPC_QUALITY_CONFIG_ERROR;
		return hr;
	}

	LogWrite(m_bXmtBuffer,m_wXmtLength);

	if(m_bNetworkCommunications)
		hr=pDevice->PerformNetworkIO(pTag);
	else
		hr=pDevice->PerformSerialIO(pTag);

	if (FAILED(hr))
	{
		LogError();
	}
	
	LogRead(m_bRcvBuffer,m_wRcvLength);

	return pDevice->ProcessResponse(pTag,hr);
}

HRESULT CIO::WriteTag(CTag* pTag)
{
	m_iInactivityCounter=MAX_INACTIVITY;

	CoFileTimeNow(&pTag->m_Timestamp);

	CDevice* pDevice=pTag->m_pDevice;

	HRESULT hr=pDevice->PrepareRequest(pTag,TRUE);
	if(FAILED(hr))
	{
		pTag->m_wQuality=OPC_QUALITY_CONFIG_ERROR;
		return hr;
	}

	LogWrite(m_bXmtBuffer,m_wXmtLength);

	if(m_bNetworkCommunications)
		hr=pDevice->PerformNetworkIO(pTag);
	else
		hr=pDevice->PerformSerialIO(pTag);

	if (FAILED(hr))
	{
		LogError();
	}
	LogRead(m_bRcvBuffer,m_wRcvLength);

	return pDevice->ProcessResponse(pTag,hr);
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

		// Close Comm Port when m_iInactivityCount decrements to 0 
		if(m_iInactivityCounter != 0)
			m_iInactivityCounter--;

		if(m_iInactivityCounter == 0)
			CloseComPort();

		LeaveCriticalSection(&m_cs);
	}
}

void CIO::SetPortParameters(	LPCTSTR					szPort,
										ACCULOAD_BAUD			Baud,
										ACCULOAD_DATA_BITS	DataBits,
										ACCULOAD_PARITY		Parity,
										ACCULOAD_STOP_BITS	StopBits)
{
	CSLock Lock(&m_cs);
	
	m_oPort=szPort;
	m_Baud=Baud;
	m_DataBits=DataBits;
	m_Parity=Parity;
	m_StopBits=StopBits;
	m_bPortParametersChanged=TRUE;
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
		g_pDeviceManager->UpdateGroups();
	}
}

void CIO::SignalCommunicationsFailure(CTag* pTag, DWORD dwError)
{
	// Signal all tags bad
	CTag* pParent=pTag->m_pParent;
	while(pParent->m_pParent->m_pParent != NULL)
		pParent=pParent->m_pParent;

	if(pParent->m_wQuality != OPC_QUALITY_COMM_FAILURE)
	{
		CString oError;
		oError.Format(_T("IO Communications Failure on : %s; error number %d"),pParent->m_oName, dwError);
		theApp.LogError(oError);
		pParent->m_wQuality=OPC_QUALITY_COMM_FAILURE;
		SetQuality(pParent,OPC_QUALITY_COMM_FAILURE);
	}
}

void CIO::SignalCommunicationsRestored(CTag* pTag)
{
	CTag* pParent=pTag->m_pParent;
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

BYTE CIO::LRC(PBYTE pbBuffer,WORD wLength)
{
	BYTE bLRC=0;

	for(WORD wItem=0;wItem < wLength;wItem++)
		bLRC^=pbBuffer[wItem];

	return bLRC;
}

void CIO::LogRead(BYTE* buffer, WORD length)
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