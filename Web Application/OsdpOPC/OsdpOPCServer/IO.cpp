/******************************************************************************

	FILE NAME:		IO.cpp


	PURPOSE:			Implementation of the CIO


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version


*******************************************************************************/

#include "StdAfx.h"
#include ".\io.h"
#include "OsdpControllerManager.h"

extern COsdpControllerManager		g_OsdpControllerManager;

constexpr BYTE STX = 2;
constexpr BYTE ETX = 3;

// CTag
CTag::CTag(LPCTSTR szName)
{
	m_pParent=NULL;
	m_oName=szName;
	m_dwAccessRights=OPC_READABLE;
	m_dwScanCount=0;
	m_pIO=NULL;
	m_dwUpdateCount = 0;
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
	m_dwUpdateCount = 0;
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
		g_OsdpControllerManager.RemoveTagFromGroupItems(pTag);
	}

	while(m_Leaf.GetCount())
	{
		CTag* pTag=m_Leaf.RemoveTail();
		delete pTag;
	}

	g_OsdpControllerManager.RemoveTagFromGroupItems(this);

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


CTag* CTag::AddLeaf(	LPCTSTR	szName,
							BYTE		bAddress,
							LPSTR		pszCommand,
							LPSTR		pszSection,
							DWORD		dwItem,
							DWORD		dwAccessRights,
							VARTYPE	NativeType,
							CIO*		pIO)
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
	m_Leaf.AddTail(pTag);
	g_OsdpControllerManager.AddTagToGroupItems(pTag);
	return pTag;
}

CTag* CTag::AddLeaf(	INT		iID,
							BYTE		bAddress,
							LPSTR		pszCommand,
							LPSTR		pszSection,
							DWORD		dwItem,
							DWORD		dwAccessRights,
							VARTYPE	NativeType,
							CIO*		pIO)
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
	m_Leaf.AddTail(pTag);
	g_OsdpControllerManager.AddTagToGroupItems(pTag);
	return pTag;
}

// CIO
UINT CIO::ScanThread(LPVOID lpIO)
{
	CIO* pIO = (CIO*) lpIO;

	CoInitializeEx(NULL,COINIT_MULTITHREADED);

	pIO->Scan();

	CoUninitialize();

	AfxEndThread(0);

	return( 0 );
}

CIO::CIO(LPCTSTR	szIPAddress,
			LONG		lPort)
{
	m_hKillEvent=NULL;
	m_pScanThread=NULL;
	m_dwUseCount=0;
	m_pSocket=NULL;
	m_bCommFailLogged=FALSE;
	m_bNetworkCommunications=true;
	m_hSocket=NULL;

	m_oIPAddress=szIPAddress;
	m_lPort=lPort;

	InitializeCriticalSection(&m_cs);

	// Check and start log file
	CString logMsg;
	HKEY hOsdpOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	if (ERROR_SUCCESS == RegOpenKeyEx(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Varec\\OsdpOPCServer"),0,KEY_READ,&hOsdpOPCKey))
	{
		DWORD dwLogPorts = 0;
		DWORD cbLogPorts = 4;
		if (ERROR_SUCCESS == RegQueryValueEx(hOsdpOPCKey, _T("LogPorts"), NULL, NULL, reinterpret_cast<LPBYTE>(&dwLogPorts), &cbLogPorts))
		{
			if (0 != dwLogPorts)
			{
				DWORD cbBasePath = 0;
				if (ERROR_SUCCESS == RegQueryValueEx(hOsdpOPCKey, _T("LogBasePath"), NULL, NULL, NULL, &cbBasePath))
				{
					logMsg.Format(_T("IO.Init() - LogBasePath size of %d"), cbBasePath);
					OutputDebugString((LPCTSTR)logMsg);
					LPTSTR pszBasePath;
					pszBasePath = new TCHAR[cbBasePath + 1];
					if (pszBasePath != NULL)
					{
						if (ERROR_SUCCESS == RegQueryValueEx(hOsdpOPCKey, _T("LogBasePath"), NULL, NULL, reinterpret_cast<LPBYTE>(pszBasePath), &cbBasePath))
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
							else
							{
								SetFilePointer(m_hLogFile, 0, NULL, FILE_END); // Jump file pointer to the end in case of a reopen of the file; prevents log overwrite in the case of a restart.
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
		RegCloseKey(hOsdpOPCKey);
		hOsdpOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	}

	// Launch Scan Thread
	m_hKillEvent = CreateEvent( NULL,TRUE,FALSE,NULL );
	if(!m_hKillEvent)
		throw (CString(_T("IO: CreateEvent Error")));

	m_pScanThread = AfxBeginThread((AFX_THREADPROC) ScanThread,(LPVOID) this);
	if(!m_pScanThread)
		throw (CString(_T("OsdpControllerManager: AfxBeginThread Error")));

	m_pScanThread->m_bAutoDelete=FALSE;

}

CIO::CIO(LONG					lIndex,
			LPCTSTR				szPort,
			OSDP_BAUD		Baud,
			OSDP_DATA_BITS DataBits,
			OSDP_PARITY		Parity,
			OSDP_STOP_BITS StopBits)
{
	m_hKillEvent=NULL;
	m_pScanThread=NULL;
	m_dwUseCount=0;
	m_bCommFailLogged=FALSE;
	m_bNetworkCommunications=false;
	m_hSocket=NULL;

	m_lIndex=lIndex;
	m_oPort=szPort;
	m_Baud=Baud;
	m_DataBits=DataBits;
	m_Parity=Parity;
	m_StopBits=StopBits;

	InitializeCriticalSection(&m_cs);


	m_hPort=INVALID_HANDLE_VALUE;
	m_bPortParametersChanged=FALSE;

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
	HKEY hOsdpOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	if (ERROR_SUCCESS == RegOpenKeyEx(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Varec\\OsdpOPCServer"),0,KEY_READ,&hOsdpOPCKey))
	{
		DWORD dwLogPorts = 0;
		DWORD cbLogPorts = 4;
		if (ERROR_SUCCESS == RegQueryValueEx(hOsdpOPCKey, _T("LogPorts"), NULL, NULL, reinterpret_cast<LPBYTE>(&dwLogPorts), &cbLogPorts))
		{
			if (0 != dwLogPorts)
			{
				DWORD cbBasePath = 0;
				if (ERROR_SUCCESS == RegQueryValueEx(hOsdpOPCKey, _T("LogBasePath"), NULL, NULL, NULL, &cbBasePath))
				{
					logMsg.Format(_T("IO.Init() - LogBasePath size of %d"), cbBasePath);
					OutputDebugString((LPCTSTR)logMsg);
					LPTSTR pszBasePath;
					pszBasePath = new TCHAR[cbBasePath + 1];
					if (pszBasePath != NULL)
					{
						if (ERROR_SUCCESS == RegQueryValueEx(hOsdpOPCKey, _T("LogBasePath"), NULL, NULL, reinterpret_cast<LPBYTE>(pszBasePath), &cbBasePath))
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
							else
							{
								SetFilePointer(m_hLogFile, 0, NULL, FILE_END); // Jump file pointer to the end in case of a reopen of the file; prevents log overwrite in the case of a restart.
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
		RegCloseKey(hOsdpOPCKey);
		hOsdpOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	}

	// Launch Scan Thread
	m_hKillEvent = CreateEvent( NULL,TRUE,FALSE,NULL );
	if(!m_hKillEvent)
		throw (CString(_T("IO: CreateEvent Error")));

	m_pScanThread = AfxBeginThread((AFX_THREADPROC) ScanThread,(LPVOID) this);
	if(!m_pScanThread)
		throw (CString(_T("OsdpControllerManager: AfxBeginThread Error")));

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

	while(m_TagScanList.GetCount())
		m_TagScanList.RemoveTail();

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

		if(WriteOverLapped.hEvent != NULL )
			CloseHandle(WriteOverLapped.hEvent );

		if(ReadOverLapped.hEvent != NULL )
			CloseHandle(ReadOverLapped.hEvent );

		if(CommOverLapped.hEvent != NULL )
			CloseHandle(CommOverLapped.hEvent );
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

HRESULT CIO::OpenSocket()
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

	if(!m_pSocket->Connect(m_oIPAddress,m_lPort))
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
			DWORD dw = GetLastError();

			CString oError;
			oError.Format(_T("IO Error : CreateFile on : %s\nGetLastError()=%u"),m_oPort,dw);
			theApp.LogError(oError);
			m_bCommFailLogged=TRUE;
		}

		return E_FAIL;
	}

	DCB Dcb{};

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
		case OSDP_BAUD_1200:
			Dcb.BaudRate=CBR_1200;
			break;
		case OSDP_BAUD_2400:
			Dcb.BaudRate=CBR_2400;
			break;
		case OSDP_BAUD_4800:
			Dcb.BaudRate=CBR_4800;
			break;
		case OSDP_BAUD_9600:
			Dcb.BaudRate=CBR_9600;
			break;
		case OSDP_BAUD_19200:
			Dcb.BaudRate=CBR_19200;
			break;
		case OSDP_BAUD_38400:
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
		case OSDP_PARITY_NONE:
			Dcb.Parity=NOPARITY;
			break;
		case OSDP_PARITY_EVEN:
			Dcb.Parity=EVENPARITY;
			break;
		case OSDP_PARITY_ODD:
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
	Dcb.fRtsControl= RTS_CONTROL_DISABLE;
	Dcb.fDtrControl=DTR_CONTROL_DISABLE;
	Dcb.fAbortOnError=TRUE;
	Dcb.EvtChar=0xFF; 

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

	COMMTIMEOUTS	CommTimeouts{};
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

	return S_OK;
}

BYTE CIO::CalcCksum(PBYTE pbBuffer,WORD wLength)
{
	WORD wLRC=0;

	for(WORD wItem=0;wItem < wLength;wItem++)
		wLRC+=pbBuffer[wItem];

	return LOBYTE(wLRC);
}

WORD CIO::CalcCrcCcitt(PBYTE pbBuffer, WORD wLength)
{
	WORD crc = 0x1d0f;
	const WORD poly = 0x1021;

	for (WORD i = 0; i < wLength; i++)
	{
		crc ^= static_cast<WORD>(pbBuffer[i]) << 8;
		for (int j = 0; j < 8; ++j)
		{
			if (crc & 0x8000) {
				crc = (crc << 1) ^ poly;
			}
			else
			{
				crc <<= 1;
			}
		}
	}

	return crc;
}

WORD CIO::BinaryToHex(BYTE bBinary)
{
	WORD wLowNibble=bBinary & 0xf;
	WORD wHighNibble=(bBinary & 0xf0) >> 4;
	if(wLowNibble < 10)
		wLowNibble+=0x30;
	else
		wLowNibble+=0x37;

	if(wHighNibble < 10)
		wHighNibble+=0x30;
	else
		wHighNibble+=0x37;
	return (wLowNibble << 8) | wHighNibble;
}

BYTE CIO::HexToBinary(WORD wHex)
{
	BYTE bLowNibble=HIBYTE(wHex);
	BYTE bHighNibble=LOBYTE(wHex);
	bLowNibble-=0x30;
	bHighNibble-=0x30;
	if(bLowNibble > 9)
		bLowNibble-=7;
	if(bHighNibble > 9)
		bHighNibble-=7;
	return (bHighNibble << 4) | bLowNibble;
}

HRESULT CIO::PrepareRequest(CTag* pTag,BOOL bWrite)
{
	m_bXmtBuffer[0] = 0x53;
	m_bXmtBuffer[1] = pTag->m_bAddress;

	m_bXmtBuffer[2] = 0; // LEN_LSB;
	m_bXmtBuffer[3] = 0; // LEN_MSB;

	// Get sequence number
	// This is in the IDS_COUNT tag
	CTag* pCountersTag = pTag->m_pParent;
	CTag* pCountTag = pCountersTag->m_Leaf.GetHead();
	BYTE bCount = 0;
	bCount = pCountTag->m_Value.bVal;

	// Set message control block
	BYTE mcb = 0;
	mcb |= (0x03 & bCount); // sequence number
	mcb |= (0x04 & (1 << 2)); // 16-bit CRC validity check (bit unset specifies 8-bit checksum)
	mcb |= (0x08 & 0); // Unset to indicate no Security Control Block
	mcb |= (0xF0 & 0); // 4 high bits "Shall be set to zero"
	m_bXmtBuffer[4] = mcb;

	if(pTag->m_pszCommand)
	{
		if(!strcmp(pTag->m_pszCommand,"\x60"))
		{
			// Poll
			// Read Card Data
			// Read Keypad Data
			// These all are executed via the POLL command to the card reader
			m_bXmtBuffer[5]=pTag->m_pszCommand[0];
			m_wXmtLength=8; // allow for the 2 bytes for the Crc
		}
		else if (!strcmp(pTag->m_pszCommand,"\x69"))
		{
			// Activate LED
			// we expect to send a single 14-byte LED data record
			// total number of bytes will be 22
			m_bXmtBuffer[5] = pTag->m_pszCommand[0];
			m_wXmtLength = 22;

			m_bXmtBuffer[6] = 0; // First Reader
			m_bXmtBuffer[7] = 0; // First LED
			
			if (pTag->m_Value.uiVal == 0)
			{
				// Duration of 0 seconds indicates a desire to permanently set the LED
				// Temporary LED settings
				m_bXmtBuffer[8] = 0; // No-op.  Ignore rest of permanent settings, leaving unchanged
				m_bXmtBuffer[9] = 0; // On time/cycle
				m_bXmtBuffer[10] = 0; // Off time/cycle
				m_bXmtBuffer[11] = 0; // On color
				m_bXmtBuffer[12] = 0; // Off color
				m_bXmtBuffer[13] = 0; // Total time LSB
				m_bXmtBuffer[14] = 0; // Total time MSB

				// Permanent LED settings
				m_bXmtBuffer[15] = 1; // Permanent control Code - 1 = Set Permanent State
				m_bXmtBuffer[16] = 0xff; // 25.6 second "on" in LED duty cycle (appears to be actually 2.56 seconds; this is a deviation from the OSDP spec)
				m_bXmtBuffer[17] = 0; // 0 seconds "off" in LED duty cycle (trying for steady on)
				m_bXmtBuffer[18] = static_cast<BYTE>(pTag->m_dwItem); // Set "on" cycle to the desired color
				m_bXmtBuffer[19] = static_cast<BYTE>(pTag->m_dwItem); // Set "off" cycle to the desired color (same as on)
			}
			else
			{
				// Temporary LED settings
				m_bXmtBuffer[8] = 2; // Temporary control Code - 2 = Set Temporary State
				m_bXmtBuffer[9] = 0xff; // 25.6 second "on" in LED duty cycle (appears to be actually 2.56 seconds; this is a deviation from the OSDP spec)
				m_bXmtBuffer[10] = 0; // 0 seconds "off" in LED duty cycle (trying for steady on)
				m_bXmtBuffer[11] = static_cast<BYTE>(pTag->m_dwItem); // Set "on" cycle to the desired color
				m_bXmtBuffer[12] = static_cast<BYTE>(pTag->m_dwItem); // Set "off" cycle to the desired color (same as on)

				// Multiply number of seconds to display by 10; ths value specifies number of 100ms intervals to keep the led on (10 per second)
				// Experience indicates that we need to multiply by 100, the Farpoint card reader (our current choice) is counting 10ms intervals
				// instead of 100ms.  This is a deviation from the OSDP spec.
				WORD nTime = static_cast<WORD>(pTag->m_Value.uiVal * 100);
				m_bXmtBuffer[13] = static_cast<BYTE>(nTime & 0x00FF);
				m_bXmtBuffer[14] = static_cast<BYTE>((nTime & 0xFF00) >> 8);

				// Permanent LED settings
				m_bXmtBuffer[15] = 0; // No-op.  Ignore rest of permanent settings, leaving unchanged
				m_bXmtBuffer[16] = 0; // On time
				m_bXmtBuffer[17] = 0; // Off time
				m_bXmtBuffer[18] = 0; // On color
				m_bXmtBuffer[19] = 0; // Off color
			}
		}
		else if (!strcmp(pTag->m_pszCommand, "\x6a"))
		{
			// Activate Beeper/buzzer
			// we expect to send a single 5-byte buzzer command data record
			// total number of bytes will be 13
			m_bXmtBuffer[5] = pTag->m_pszCommand[0];
			m_wXmtLength = 13;

			m_bXmtBuffer[6] = 0; // First Reader
			m_bXmtBuffer[7] = 0x02; // default tone

			// try to set up continuous sound - total 100ms interval
			m_bXmtBuffer[8] = 0x01; // duty cycle 100ms (1/10 sec) on
			m_bXmtBuffer[9] = 0x00; // duty cycle off

			//Number of cycles  to repeat (should be 10 cycles/sec)
			m_bXmtBuffer[10] = static_cast<BYTE>((pTag->m_Value.uiVal) & 0x00FF);
		}
		else if (!strcmp(pTag->m_pszCommand, "\x00"))
		{
			// Counter is an internal tag.  Don't send anything
			m_wXmtLength = 0; // allow for the 2 bytes for the Crc
			return S_OK;
		}
		else
		{
			return E_FAIL;
		}

		m_bXmtBuffer[2] = static_cast<BYTE>(m_wXmtLength & 0x00FF);
		m_bXmtBuffer[3] = static_cast<BYTE>((m_wXmtLength & 0xFF00) >> 8);
		
		WORD wCRC = CalcCrcCcitt(m_bXmtBuffer, m_wXmtLength - 2);
		m_bXmtBuffer[m_wXmtLength - 2] = static_cast<BYTE>(wCRC & 0x00FF);
		m_bXmtBuffer[m_wXmtLength - 1] = static_cast<BYTE>((wCRC & 0xFF00) >> 8);
	}
	else
	{
		return E_FAIL;
	}

	return S_OK;
}

HRESULT CIO::ProcessResponse(CTag* pTag)
{
	BOOL fAdvanceCounter = FALSE;
	BOOL fResetCounter = FALSE;

	// Sticky detail: we may get spurious characters prior to the beginning character (0x53)
	// Find that.
	WORD wOffset = 0;
	while (m_bRcvBuffer[wOffset] != '\x53' && wOffset < m_wRcvLength)
	{
		wOffset++;
	}

	// Make sure this response is for us
	if ((m_bRcvBuffer[wOffset + 1] & 0x7F) != (pTag->m_bAddress & 0x7F))
	{
		return E_FAIL;
	}

	// Need to get message length again, as we'll need it below
	BYTE byLSB = m_bRcvBuffer[wOffset + 2];
	BYTE byMSB = m_bRcvBuffer[wOffset + 3];
	WORD cbMessage = (static_cast<WORD>(byMSB) << 8) + static_cast<WORD>(byLSB);

	if(pTag->m_pszCommand)
	{
		// We need the root tag for incrementing the counter
		// All of our tags are on one level, so only go up one parent to get the root
		CTag* pRootTag;
		pRootTag = pTag->m_pParent;

		// Need to get the counter to verify we're checking the correct response.
		// If it doesn't match up, we fail and reset the counter because we're out
		// of sync
		CTag* pCountTag = pRootTag->m_Leaf.GetHead();
		BYTE bCount = 0;
		pCountTag->m_Value.vt = VT_UI1;
		bCount = pCountTag->m_Value.bVal;

		BYTE bResponseMcb = m_bRcvBuffer[wOffset + 4];
		BYTE bResponseCounter = bResponseMcb & 0x03;

		if (bResponseCounter != bCount)
		{
			// Counter sent doesn't match counter received; we lost the thread.
			// Mark tag bad and reset counter
			pTag->m_wQuality = OPC_QUALITY_BAD;
			fResetCounter = TRUE;
		}
		else
		{
			// Read Poll response
			// Tricky behavior here is that any poll request to an OSDP device can
			// return any poll response, whichever one is ready.  We could be trying to scan
			// for keypad data but get card data back.  What we need to do is for each response to a poll
			//
			if (!strcmp(pTag->m_pszCommand, "\x60"))
			{
				POSITION pos = pRootTag->m_Leaf.GetHeadPosition();
				CTag* pCurrentTag;
				CTag* pPollStatusTag = NULL;
				CTag* pKeypadDataTag = NULL;
				CTag* pCardDataTag = NULL;

				while (pos)
				{
					pCurrentTag = pRootTag->m_Leaf.GetNext(pos);
					if (!strcmp(pCurrentTag->m_pszCommand, "\x60")) {
						// Poll command tag
						switch (pCurrentTag->m_dwItem)
						{
						case POLL:
							pPollStatusTag = pCurrentTag;
							break;
						case CARD_DATA:
							pCardDataTag = pCurrentTag;
							break;
						case KEYPAD_DATA:
							pKeypadDataTag = pCurrentTag;
							break;
						}
					}
				}

				int nCommandPosition = wOffset + 5;
				int nCrcSize = 1;
				int nMACSize = 0;

				if ((bResponseMcb & 0x08) == 0x08)
				{
					// We have a security control block, which comes before the command/response.
					// The length of the SCB will be in message byte 5; the command/response will be shoved by this size
					nCommandPosition += static_cast<int>(m_bRcvBuffer[wOffset + 5]);
					if ((m_bRcvBuffer[wOffset + 6] > 0x14) && (m_bRcvBuffer[wOffset + 6] < 0x19))
					{
						//Security block types between 0x15 and 0x18 inclusive have a 4-Byte MAC immediately preceding the chksum/CRC
						nMACSize = 4;
					}
				}

				if ((bResponseMcb & 0x04) == 0x04)
				{
					// Using CRC instead of checksum
					nCrcSize++;
				}

				int nDataStart;
				int nDataEnd;
				int nDataLength;

				BYTE bResponse = m_bRcvBuffer[nCommandPosition];
				switch (bResponse)
				{
				case 0x41: // osdp_NAK
				{
					// In this case, our specific tag is dead, and the POLL tag valid but false
					pTag->m_wQuality = OPC_QUALITY_BAD;

					VariantClear(&pPollStatusTag->m_Value);
					pPollStatusTag->m_Value.vt = VT_BOOL;
					pPollStatusTag->m_Value.boolVal = FALSE;
					pPollStatusTag->m_wQuality = OPC_QUALITY_GOOD;

					fResetCounter = TRUE;
				}
				break;
				case 0x50: // osdp_RAW (binary card reader data)
				{
					// Card data record begins the next byte after the command
					nDataStart = nCommandPosition + 1;
					nDataEnd = wOffset + cbMessage - 1 - nCrcSize - nMACSize;
					nDataLength = nDataEnd - nDataStart + 1;
					BYTE* pData = &(m_bRcvBuffer[nDataStart]);
					if (nDataLength > 3)
					{
						// Reader number on the device is data byte 0.  We're assuming single-reader devices at the moment.
						// Data format is in byte 1.
						BOOL fWiegandFormat = (pData[1] == 0x01);

						// Data bytes 2 and 3 are the bit-length of data
						BYTE cbitCardDataLSB = pData[2];
						BYTE cbitCardDataMSB = pData[3];
						WORD cbitCardData = (static_cast<WORD>(cbitCardDataMSB) << 8) ^ static_cast<WORD>(cbitCardDataLSB);
						if (fWiegandFormat)
						{
							// We don't care about the trailing parity bit.
							// Starting parity bit variance will be handled by
							// FM; it already has to deal with the difference between how 
							// SmithMeter card readers and DET card readers report it.
							cbitCardData--;
						}
						WORD cchCardData = (cbitCardData / 4) + ((cbitCardData % 4) > 0 ? 1 : 0);
						
						// Need to shift the bits over to match the DET
						// OSDP passes the bits left justified, but our DET reads right justified to a nibble boundry
						// Also work in a copied buffer
						BYTE* byBuffer = new BYTE[nDataLength - 4];
						for (int ndx = 0; ndx < nDataLength - 4; ndx++)
						{
							byBuffer[ndx] = pData[ndx + 4];
						}

						WORD cbitShift = (4 - (cbitCardData % 4) % 4);
						for (int shiftRound = cbitShift; shiftRound > 0; shiftRound--)
						{
							for (int ndx = (nDataLength - 4 - 1); ndx >= 0; ndx--)
							{
								if (ndx < (nDataLength - 4 - 1))
								{
									byBuffer[ndx + 1] ^= (byBuffer[ndx] & 0x01) << 7;
								}

								byBuffer[ndx] = byBuffer[ndx] >> 1;
							}
						}

						TCHAR* ptszCardData = new TCHAR[cchCardData + 1];
						ptszCardData[cchCardData] = '\x00';
						for (int ndx = 0; ndx < cchCardData; ndx++)
						{
							BYTE currentNibble = (byBuffer[ndx / 2] >> ((ndx % 2) == 0 ? 4 : 0) & 0x0f);
							ptszCardData[ndx] = (currentNibble >= 10) ? currentNibble - 10 + 'A' : currentNibble + '0';
						}

						CString csCardData;
						csCardData = ptszCardData;
						delete[] ptszCardData;
						pCardDataTag->m_Value = (LPCTSTR)csCardData;
						pCardDataTag->m_wQuality = OPC_QUALITY_GOOD;
						pCardDataTag->m_bCurrent = TRUE;
						pCardDataTag->m_dwUpdateSequence++;

						// Poll status is good
						VariantClear(&pPollStatusTag->m_Value);
						pPollStatusTag->m_Value.vt = VT_BOOL;
						pPollStatusTag->m_Value.boolVal = TRUE;
						pPollStatusTag->m_wQuality = OPC_QUALITY_GOOD;
						pPollStatusTag->m_bCurrent = TRUE;

						pKeypadDataTag->m_bCurrent = TRUE; // Issueing the poll for a keypad data tag can return card data, erasing what we have.
															// We need to wait for the next scan loop.

						fAdvanceCounter = TRUE;
					}
				}
				break;
				case 0x51: // osdp_FMT (formatted card reader data)
				{
					// Data is card number formatted as a string
					nDataStart = nCommandPosition + 1;
					nDataEnd = wOffset + cbMessage - 1 - nCrcSize - nMACSize;
					nDataLength = nDataEnd - nDataStart + 1;
					BYTE* pData = &(m_bRcvBuffer[nDataStart]);
					if (nDataLength > 3)
					{
						// Reader number on the device is data byte 0.  We're assuming single-reader devices at the moment.
						// Read direction is data byte 1.  We need to deal with this.
						BOOL fReverseRead = (pData[1] == 0x01);

						// Data byte 2  is the card data character count
						int cchCardData = static_cast<int>(pData[2]);
						char szCardData[50];
						strncpy(szCardData, reinterpret_cast<char*>(&(pData[3])), (cchCardData < 49 ? cchCardData : 49));
						szCardData[cchCardData < 49 ? cchCardData : 49] = '\x00';

						if (fReverseRead)
						{
							strrev(szCardData);
						}

						CString csCardData;
						csCardData = szCardData;
						pCardDataTag->m_Value = (LPCTSTR)csCardData;
						pCardDataTag->m_wQuality = OPC_QUALITY_GOOD;
						pCardDataTag->m_bCurrent = TRUE;
						pCardDataTag->m_dwUpdateSequence++;

						// Poll status is good
						VariantClear(&pPollStatusTag->m_Value);
						pPollStatusTag->m_Value.vt = VT_BOOL;
						pPollStatusTag->m_Value.boolVal = TRUE;
						pPollStatusTag->m_wQuality = OPC_QUALITY_GOOD;
						pPollStatusTag->m_bCurrent = TRUE;

						pKeypadDataTag->m_bCurrent = TRUE; // Issueing the poll for a keypad data tag can return card data, erasing what we have.
															// We need to wait for the next scan loop.

						fAdvanceCounter = TRUE;
					}
				}
				break;
				case 0x53: // osdp_KEYPAD (keypad data)
				{
					// Data is card number formatted as a string
					nDataStart = nCommandPosition + 1;
					nDataEnd = wOffset + cbMessage - 1 - nCrcSize - nMACSize;
					nDataLength = nDataEnd - nDataStart + 1;
					BYTE* pData = &(m_bRcvBuffer[nDataStart]);
					if (nDataLength > 2)
					{
						// Reader number on the device is data byte 0.  We're assuming single-reader devices at the moment.
						// Data byte 1 is the character data character count (since last poll)
						// Also, expect that we'll have less than 50
						int cchKeyData = static_cast<int>(pData[1]);
						char szKeyData[50];
						strncpy(szKeyData, reinterpret_cast<char*>(&(pData[2])), (cchKeyData < 49 ? cchKeyData : 49));
						szKeyData[cchKeyData < 49 ? cchKeyData : 49] = '\x00';

						CString csKeyData;
						csKeyData = szKeyData;
						pKeypadDataTag->m_Value = (LPCTSTR)csKeyData;
						pKeypadDataTag->m_wQuality = OPC_QUALITY_GOOD;
						pKeypadDataTag->m_bCurrent = TRUE;
						pKeypadDataTag->m_dwUpdateSequence++;


						// Poll status is good
						VariantClear(&pPollStatusTag->m_Value);
						pPollStatusTag->m_Value.vt = VT_BOOL;
						pPollStatusTag->m_Value.boolVal = TRUE;
						pPollStatusTag->m_wQuality = OPC_QUALITY_GOOD;
						pPollStatusTag->m_bCurrent = TRUE;

						pCardDataTag->m_bCurrent = TRUE; // Issueing the poll for a card data tag can return keypad data, erasing what we have.
															// We need to wait for the next scan loop.

						fAdvanceCounter = TRUE;
						break;
					}
				}
				default: // other possible POLL responses, including command accepted, nothing to report
				{
					// Poll status is good
					VariantClear(&pPollStatusTag->m_Value);
					pPollStatusTag->m_Value.vt = VT_BOOL;
					pPollStatusTag->m_Value.boolVal = TRUE;
					pPollStatusTag->m_wQuality = OPC_QUALITY_GOOD;
					pPollStatusTag->m_bCurrent = TRUE;

					fAdvanceCounter = TRUE;
					break;
				}
				}
			}
			else if ((!strcmp(pTag->m_pszCommand, "\x69"))
				|| (!strcmp(pTag->m_pszCommand, "\x6a")))
			{
				int nCommandPosition = wOffset + 5;
				int nCrcSize = 1;
				int nMACSize = 0;
				BYTE bResponseMcb = m_bRcvBuffer[wOffset + 4];

				if ((bResponseMcb & 0x08) == 0x08)
				{
					// We have a security control block, which comes before the command/response.
					// The length of the SCB will be in message byte 5; the command/response will be shoved by this size
					nCommandPosition += static_cast<int>(m_bRcvBuffer[wOffset + 5]);
					if ((m_bRcvBuffer[wOffset + 6] > 0x14) && (m_bRcvBuffer[wOffset + 6] < 0x19))
					{
						//Security block types between 0x15 and 0x18 inclusive have a 4-Byte MAC immediately preceding the chksum/CRC
						nMACSize = 4;
					}
				}

				if ((bResponseMcb & 0x04) == 0x04)
				{
					// Using CRC instead of checksum
					nCrcSize++;
				}

				//int nDataStart;
				//int nDataEnd;
				//int nDataLength;

				BYTE bResponse = m_bRcvBuffer[nCommandPosition];
				switch (bResponse)
				{
				case 0x41: // osdp_NAK
					// Card reader responded that it couldn't honor the command, but it was well formed
					// Just deal with it.  Report bad
					pTag->m_wQuality = OPC_QUALITY_BAD;

					fResetCounter = TRUE;
					break;
				default: // other possible POLL responses, including command accepted, nothing to report
					pTag->m_wQuality = OPC_QUALITY_GOOD;

					fAdvanceCounter = TRUE;
					break;
				}
			}
			else
			{
				return E_FAIL;
			}
		}

		if (TRUE == fResetCounter)
		{
			bCount = 0;
			pCountTag->m_Value.bVal = bCount;
		}
		if (TRUE == fAdvanceCounter)
		{
			bCount++;
			if (bCount > 3)
			{
				bCount = 1;
			}
			pCountTag->m_Value.bVal = bCount;
		}
		pCountTag->m_wQuality = OPC_QUALITY_GOOD;
		pCountTag->m_bCurrent = TRUE;
	}
	else
	{
		return E_FAIL;
	}

	return S_OK;
}

HRESULT CIO::PerformNetworkIO(CTag* pTag)
{
	if(m_pSocket == NULL)
	{
		HRESULT hr=OpenSocket();
		if(FAILED(hr))
		{
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			return E_FAIL;
		}
	}

	for(INT iTry=0;iTry < 3;iTry++)
	{

		if(!m_pSocket->Send(m_bXmtBuffer,m_wXmtLength))
		{
			delete m_pSocket;
			m_pSocket=NULL;
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			return E_FAIL;
		}

		m_wRcvLength=0;
		DWORD dwNumberOfBytesRead=0;
		while(true)
		{
			dwNumberOfBytesRead=m_pSocket->Receive(&m_bRcvBuffer[m_wRcvLength],sizeof(m_bRcvBuffer)-m_wRcvLength-1);

			if(dwNumberOfBytesRead == SOCKET_ERROR
			|| dwNumberOfBytesRead == 0)
			{
				CString oError;
				oError.Format(_T("IO Error = %s : CAsyncSocket.Receive"),SocketError(m_pSocket->GetLastError()));
				theApp.LogError(oError);

				delete m_pSocket;
				m_pSocket=NULL;
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
				return E_FAIL;
			}

			m_wRcvLength+=(WORD) dwNumberOfBytesRead; 

			// Receipt is complete on '\r'
			if(m_bRcvBuffer[m_wRcvLength-1] == '\r')
				break;
		}

		// First Byte is N or A
		if(m_bRcvBuffer[0] != 'N'
		&& m_bRcvBuffer[0] != 'A')
			continue;


		// Last character should be '\r'
		if(m_bRcvBuffer[m_wRcvLength-1] != '\r')
			continue;


		// Verify Checksum
		if(m_bRcvBuffer[0] == 'A'
		&& m_wRcvLength > 4)
		{
			BYTE bLRC=CalcCksum(m_bRcvBuffer,m_wRcvLength-1);
			if (bLRC != m_bRcvBuffer[m_wRcvLength - 1])
			{
				continue;
			}
		}
		break;
	}

	if(iTry == 3)
	{
		delete m_pSocket;
		m_pSocket=NULL;
		pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
		return E_FAIL;
	}

	// PerformNetworkIO called from multiple threads so must detach after each use
	m_hSocket=m_pSocket->Detach();
	delete m_pSocket;
	m_pSocket=NULL;

	return S_OK; 
}

HRESULT CIO::PerformSerialIO(CTag* pTag)
{
	if(m_hPort == INVALID_HANDLE_VALUE)
	{
		HRESULT hr=OpenComPort();
		if(FAILED(hr))
		{
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
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

		if(!ClearCommError(m_hPort,&dwCommErrFlags,&ComStat))
			continue;

		if(!PurgeComm(	m_hPort,
							PURGE_RXCLEAR |
							PURGE_RXABORT |
							PURGE_TXCLEAR |
							PURGE_TXABORT))
			continue;

		// Write the request
		if(!WriteFile(m_hPort,m_bXmtBuffer,m_wXmtLength,&dwNumberOfBytesWritten,&WriteOverLapped))
		{
			if(GetLastError() != ERROR_IO_PENDING)
				continue;

			if(!GetOverlappedResult(m_hPort,&WriteOverLapped,&dwNumberOfBytesWritten,TRUE))
				continue;
		}

		if(m_wXmtLength != dwNumberOfBytesWritten)
			continue;

		// Read the response
	 	if(!SetCommMask(m_hPort,EV_ERR | EV_RXFLAG | EV_RXCHAR))
			continue;

		if(!WaitCommEvent(m_hPort,&dwCommEvtFlags,&CommOverLapped)
		&& GetLastError() != ERROR_IO_PENDING)
		{
			if(pTag->m_wQuality == OPC_QUALITY_COMM_FAILURE)
			{
				CloseComPort();
				m_bPortParametersChanged=FALSE;

				return E_FAIL;
			}
			else
				continue;
		}

		// Delay to give device time to respond
		Sleep(100);

		switch(WaitForSingleObject(CommOverLapped.hEvent,500))
		{
			case WAIT_OBJECT_0:
				if ((dwCommEvtFlags & EV_ERR) == EV_ERR)
				{
					continue;
				}
				else if((dwCommEvtFlags & (EV_RXFLAG | EV_RXCHAR)) != 0 )
				{
					if (!ClearCommError(m_hPort, &dwCommErrFlags, &ComStat))
					{
						continue;
					}

					if (ComStat.cbInQue > sizeof(m_bRcvBuffer) - 1)
					{
						continue;
					}

					if (!ReadFile(m_hPort, m_bRcvBuffer, ComStat.cbInQue, &dwNumberOfBytesRead, &ReadOverLapped)
						&& GetLastError() != ERROR_IO_PENDING)
					{
						continue;
					}

					if (!GetOverlappedResult(m_hPort, &ReadOverLapped, &dwNumberOfBytesRead, TRUE))
					{
						continue;
					}

					break;
				}
				else
				{
					continue;
				}
			case WAIT_TIMEOUT:
				if(pTag->m_wQuality == OPC_QUALITY_COMM_FAILURE)
				{
					CloseComPort();
					m_bPortParametersChanged=FALSE;
					return E_FAIL;
				}
				else
					continue;

			case WAIT_FAILED:
			default:
				continue;
		}

		m_wRcvLength= static_cast<WORD>(dwNumberOfBytesRead);

		// First Byte of message is 0x53; we may get attention bytes before.  
		WORD wOffset = 0;
		while (m_bRcvBuffer[wOffset] != '\x53' && wOffset < static_cast<WORD>(dwNumberOfBytesRead))
		{
			wOffset++;
		}

		// shortest valid message possible is 7 bytes, starting from the 0x53
		if ((static_cast<WORD>(dwNumberOfBytesRead) - wOffset) < 7)
		{
			continue;
		}

		// Need to check length now, as its needed to locate error check bytes
		BYTE byLSB = m_bRcvBuffer[wOffset + 2];
		BYTE byMSB = m_bRcvBuffer[wOffset + 3];
		WORD cbMessage = (static_cast<WORD>(byMSB) << 8) + static_cast<WORD>(byLSB);
		if ((static_cast<WORD>(dwNumberOfBytesRead) - wOffset) < cbMessage)
		{
			// Somewhere we're missing bytes the message is expecting.  Try again.
			continue;
		}

		// Now, we need to determine what error check we're using: Cksum or CRC
		BYTE byMCB = m_bRcvBuffer[wOffset + 4];
		BOOL fUseCrc = ((byMCB & (1 >> 2)) == (1 >> 2)); // bit 2 is set for CRC, unset for Cksum

		if (fUseCrc)
		{
			// Verify CRC lo and hi bytes
			WORD wCRC = CalcCrcCcitt(&(m_bRcvBuffer[wOffset]), cbMessage - 2);

			if (static_cast<BYTE>(wCRC & 0x00FF) != m_bRcvBuffer[wOffset + cbMessage - 2])
			{
				continue;
			}

			if (static_cast<BYTE>((wCRC & 0xFF00) >> 8) != m_bRcvBuffer[wOffset + cbMessage - 1])
			{
				continue;
			}
		}
		else
		{
			// Verify Checksum
			BYTE bLRC = CalcCksum(&(m_bRcvBuffer[wOffset]), cbMessage - 1);
			if (bLRC != m_bRcvBuffer[wOffset + cbMessage - 1])
			{
				continue;
			}
		}

		break;
	}

	if(iTry == 3
	|| m_bPortParametersChanged)
	{
		CloseComPort();
		m_bPortParametersChanged=FALSE;
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

	if (m_wXmtLength == 0)
	{
		return S_OK;
	}

	LogWrite(m_bXmtBuffer,m_wXmtLength);

	if(m_bNetworkCommunications)
		hr=PerformNetworkIO(pTag);
	else
		hr=PerformSerialIO(pTag);

	if(FAILED(hr))
	{
		LogError();
		return hr;
	}

	LogRead(m_bRcvBuffer, m_wRcvLength);
	hr=ProcessResponse(pTag);
	if(FAILED(hr))
	{
		pTag->m_wQuality=OPC_QUALITY_CONFIG_ERROR;
		return hr;
	}

	return S_OK;
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

	LogWrite(m_bXmtBuffer,m_wXmtLength);

	if(m_bNetworkCommunications)
		hr=PerformNetworkIO(pTag);
	else
		hr=PerformSerialIO(pTag);

	if(FAILED(hr))
	{
		LogError();
		return hr;
	}

	LogRead(m_bRcvBuffer,m_wRcvLength);
	hr=ProcessResponse(pTag);
	if(FAILED(hr))
	{
		pTag->m_wQuality=OPC_QUALITY_CONFIG_ERROR;
		return hr;
	}

	return S_OK;
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
		m_TagScanList.RemoveAt(m_TagScanList.Find(pTag));
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
										OSDP_BAUD			Baud,
										OSDP_DATA_BITS	DataBits,
										OSDP_PARITY		Parity,
										OSDP_STOP_BITS	StopBits)
{
	CSLock Lock(&m_cs);
	
	m_oPort=szPort;
	m_Baud=Baud;
	m_DataBits=DataBits;
	m_Parity=Parity;
	m_StopBits=StopBits;
	m_bPortParametersChanged=TRUE;
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

void CIO::CycleLogFile()
{
	CloseHandle(m_hLogFile);

	if (m_csBaseLogFileName == "")
		return;

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
	else
	{
		SetFilePointer(m_hLogFile, 0, NULL, FILE_END); // Jump file pointer to the end in case of a reopen of the file; prevents log overwrite in the case of a restart.
	}
}

void CIO::strrev(char* head)
{
	if (!head) return;
	char* tail = head;
	while (*tail) ++tail;    // find the 0 terminator, like head+strlen
	--tail;               // tail points to the last real char
	// head still points to the first
	for (; head < tail; ++head, --tail) {
		// walk pointers inwards until they meet or cross in the middle
		char h = *head, t = *tail;
		*head = t;           // swapping as we go
		*tail = h;
	}
}