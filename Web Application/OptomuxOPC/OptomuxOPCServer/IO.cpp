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
		02/13/2007	W.Gray		7.1.0.1 - Added call to RemoveTagFromGroupItems at end
										of tag destructor so that m_pCurrentTag in OPCServer
										could be reset if current browse position was to tag

		11/25/2008	W.Gray		7.6.1.0 - Changed to delay 5 seconds on OpenCommPort error (CSI 6319)

		03/24/2009	W.Gray		7.6.1.1 - Correction for I/O to match optomux protocol

		06/22/2009	W.Gray		7.4.6.2 - Revised scan logic to be more accurate in scan timing.

		12/10/2009	W.Gray		7.5.1.0 - Revised to handle error on WaitCommEvent (WI 9947)

*******************************************************************************/

#include "StdAfx.h"
#include ".\io.h"
#include "OptomuxControllerManager.h"

extern COptomuxControllerManager		g_OptomuxControllerManager;

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
		g_OptomuxControllerManager.RemoveTagFromGroupItems(pTag);
	}

	while(m_Leaf.GetCount())
	{
		CTag* pTag=m_Leaf.RemoveTail();
		delete pTag;
	}

	g_OptomuxControllerManager.RemoveTagFromGroupItems(this);

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
	g_OptomuxControllerManager.AddTagToGroupItems(pTag);
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
	g_OptomuxControllerManager.AddTagToGroupItems(pTag);
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
	HKEY hOptomuxOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	if (ERROR_SUCCESS == RegOpenKeyEx(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Varec\\OptomuxOPCServer"),0,KEY_READ,&hOptomuxOPCKey))
	{
		DWORD dwLogPorts;
		DWORD cbLogPorts = 4;
		if (ERROR_SUCCESS == RegQueryValueEx(hOptomuxOPCKey, _T("LogPorts"), NULL, NULL, reinterpret_cast<LPBYTE>(&dwLogPorts), &cbLogPorts))
		{
			if (0 != dwLogPorts)
			{
				DWORD cbBasePath = 0;
				if (ERROR_SUCCESS == RegQueryValueEx(hOptomuxOPCKey, _T("LogBasePath"), NULL, NULL, NULL, &cbBasePath))
				{
					logMsg.Format(_T("IO.Init() - LogBasePath size of %d"), cbBasePath);
					OutputDebugString((LPCTSTR)logMsg);
					LPTSTR pszBasePath;
					pszBasePath = new TCHAR[cbBasePath + 1];
					if (pszBasePath != NULL)
					{
						if (ERROR_SUCCESS == RegQueryValueEx(hOptomuxOPCKey, _T("LogBasePath"), NULL, NULL, reinterpret_cast<LPBYTE>(pszBasePath), &cbBasePath))
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
		RegCloseKey(hOptomuxOPCKey);
		hOptomuxOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	}

	// Launch Scan Thread
	m_hKillEvent = CreateEvent( NULL,TRUE,FALSE,NULL );
	if(!m_hKillEvent)
		throw (CString(_T("IO: CreateEvent Error")));

	m_pScanThread = AfxBeginThread((AFX_THREADPROC) ScanThread,(LPVOID) this);
	if(!m_pScanThread)
		throw (CString(_T("OptomuxControllerManager: AfxBeginThread Error")));

	m_pScanThread->m_bAutoDelete=FALSE;

}

CIO::CIO(LONG					lIndex,
			LPCTSTR				szPort,
			OPTOMUX_BAUD		Baud,
			OPTOMUX_DATA_BITS DataBits,
			OPTOMUX_PARITY		Parity,
			OPTOMUX_STOP_BITS StopBits)
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
	HKEY hOptomuxOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	if (ERROR_SUCCESS == RegOpenKeyEx(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Varec\\OptomuxOPCServer"),0,KEY_READ,&hOptomuxOPCKey))
	{
		DWORD dwLogPorts;
		DWORD cbLogPorts = 4;
		if (ERROR_SUCCESS == RegQueryValueEx(hOptomuxOPCKey, _T("LogPorts"), NULL, NULL, reinterpret_cast<LPBYTE>(&dwLogPorts), &cbLogPorts))
		{
			if (0 != dwLogPorts)
			{
				DWORD cbBasePath = 0;
				if (ERROR_SUCCESS == RegQueryValueEx(hOptomuxOPCKey, _T("LogBasePath"), NULL, NULL, NULL, &cbBasePath))
				{
					logMsg.Format(_T("IO.Init() - LogBasePath size of %d"), cbBasePath);
					OutputDebugString((LPCTSTR)logMsg);
					LPTSTR pszBasePath;
					pszBasePath = new TCHAR[cbBasePath + 1];
					if (pszBasePath != NULL)
					{
						if (ERROR_SUCCESS == RegQueryValueEx(hOptomuxOPCKey, _T("LogBasePath"), NULL, NULL, reinterpret_cast<LPBYTE>(pszBasePath), &cbBasePath))
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
		RegCloseKey(hOptomuxOPCKey);
		hOptomuxOPCKey = static_cast<HKEY>(INVALID_HANDLE_VALUE);
	}

	// Launch Scan Thread
	m_hKillEvent = CreateEvent( NULL,TRUE,FALSE,NULL );
	if(!m_hKillEvent)
		throw (CString(_T("IO: CreateEvent Error")));

	m_pScanThread = AfxBeginThread((AFX_THREADPROC) ScanThread,(LPVOID) this);
	if(!m_pScanThread)
		throw (CString(_T("OptomuxControllerManager: AfxBeginThread Error")));

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
		case OPTOMUX_BAUD_1200:
			Dcb.BaudRate=CBR_1200;
			break;
		case OPTOMUX_BAUD_2400:
			Dcb.BaudRate=CBR_2400;
			break;
		case OPTOMUX_BAUD_4800:
			Dcb.BaudRate=CBR_4800;
			break;
		case OPTOMUX_BAUD_9600:
			Dcb.BaudRate=CBR_9600;
			break;
		case OPTOMUX_BAUD_19200:
			Dcb.BaudRate=CBR_19200;
			break;
		case OPTOMUX_BAUD_38400:
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
		case OPTOMUX_PARITY_NONE:
			Dcb.Parity=NOPARITY;
			break;
		case OPTOMUX_PARITY_EVEN:
			Dcb.Parity=EVENPARITY;
			break;
		case OPTOMUX_PARITY_ODD:
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
	Dcb.fAbortOnError=TRUE;
	Dcb.EvtChar=0x0D; // '\r'

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

	return S_OK;
}

BYTE CIO::LRC(PBYTE pbBuffer,WORD wLength)
{
	WORD wLRC=0;

	for(WORD wItem=0;wItem < wLength;wItem++)
		wLRC+=pbBuffer[wItem];

	return LOBYTE(wLRC);
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
	m_bXmtBuffer[0]='>';
	*((PWORD) &m_bXmtBuffer[1])=BinaryToHex(pTag->m_bAddress);

	if(pTag->m_pszCommand)
	{
		// Power Up Clear
		// Reset
		// Read Input Modules ON/OFF Status
		if(!strcmp(pTag->m_pszCommand,"A")
		|| !strcmp(pTag->m_pszCommand,"B")
		|| !strcmp(pTag->m_pszCommand,"M"))
		{
			m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			*((PWORD) &m_bXmtBuffer[4])=BinaryToHex(LRC(&m_bXmtBuffer[1],3));
			m_bXmtBuffer[6]='\r';
			m_wXmtLength=7;
		}

		// Configure Module for Input
		// Configure Module for Output
		else if(!strcmp(pTag->m_pszCommand,"H")
		|| !strcmp(pTag->m_pszCommand,"I"))
		{
			BYTE	bData=0x01 << pTag->m_dwItem;
			m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			m_bXmtBuffer[4]='0';
			m_bXmtBuffer[5]='0';
			*((PWORD) &m_bXmtBuffer[6])=BinaryToHex(bData);
			*((PWORD) &m_bXmtBuffer[8])=BinaryToHex(LRC(&m_bXmtBuffer[1],7));
			m_bXmtBuffer[10]='\r';
			m_wXmtLength=11;
		}

		// Activate/Deactivate Output Module
		else if(!strcmp(pTag->m_pszCommand,"K"))
		{
			// Issue Command to Configure module for output
			pTag->m_pszCommand="I";
			HRESULT hr=WriteTag(pTag);
			pTag->m_pszCommand="K";
			if(hr != S_OK)
			{
				pTag->m_wQuality=OPC_QUALITY_BAD;
				return hr;
			}

			BYTE	bData=0x01 << pTag->m_dwItem;
			if(pTag->m_Value.boolVal == VARIANT_TRUE)
				m_bXmtBuffer[3]='K';
			else
				m_bXmtBuffer[3]='L';
			m_bXmtBuffer[4]='0';
			m_bXmtBuffer[5]='0';
			*((PWORD) &m_bXmtBuffer[6])=BinaryToHex(bData);
			*((PWORD) &m_bXmtBuffer[8])=BinaryToHex(LRC(&m_bXmtBuffer[1],7));
			m_bXmtBuffer[10]='\r';
			m_wXmtLength=11;
		}

		// PIN Display Mode
		else if(!strcmp(pTag->m_pszCommand,"p"))
		{
			if(pTag->m_Value.boolVal == VARIANT_TRUE)
				m_bXmtBuffer[3]='p';
			else
				m_bXmtBuffer[3]='q';
			*((PWORD) &m_bXmtBuffer[4])=BinaryToHex(LRC(&m_bXmtBuffer[1],3));
			m_bXmtBuffer[6]='\r';
			m_wXmtLength=7;
		}

		// Access/Modify Display Attributes
		else if(!strcmp(pTag->m_pszCommand,"s"))
		{
			m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			m_bXmtBuffer[4]=pTag->m_pszSection[0];
			m_bXmtBuffer[5]=pTag->m_pszSection[1];

			// Clear List
			// Display List
			// Selected List Item
			if(!strcmp(pTag->m_pszSection,"01")
			|| !strcmp(pTag->m_pszSection,"03")
			|| !strcmp(pTag->m_pszSection,"04"))
			{
				*((PWORD) &m_bXmtBuffer[6])=BinaryToHex(LRC(&m_bXmtBuffer[1],5));
				m_bXmtBuffer[8]='\r';
				m_wXmtLength=9;
			}

			// Write List Item - first 2 characters are list item number in hex (00 - FF)
			else if(!strcmp(pTag->m_pszSection,"02"))
			{
				CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
				int iLen=oString.GetLength();
				strncpy((LPSTR) &m_bXmtBuffer[6],oString,iLen);
				m_bXmtBuffer[iLen+6]='\\';
				m_bXmtBuffer[iLen+7]='r';
				*((PWORD) &m_bXmtBuffer[iLen+8])=BinaryToHex(LRC(&m_bXmtBuffer[1],iLen+7));
				m_bXmtBuffer[iLen+10]='\r';
				m_wXmtLength=11+iLen;
			}

			// Select List Item
			else
			{
				*((PWORD) &m_bXmtBuffer[6])=BinaryToHex(((PBYTE) &pTag->m_Value.iVal)[0]);
				*((PWORD) &m_bXmtBuffer[8])=BinaryToHex(((PBYTE) &pTag->m_Value.iVal)[1]);
				*((PWORD) &m_bXmtBuffer[10])=BinaryToHex(LRC(&m_bXmtBuffer[1],9));
				m_bXmtBuffer[12]='\r';
				m_wXmtLength=13;
			}
		}

		// Write Text Message To Display
		else if(!strcmp(pTag->m_pszCommand,"S"))
		{
			m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			m_bXmtBuffer[4]=pTag->m_pszSection[0];
			m_bXmtBuffer[5]=pTag->m_pszSection[1];
			CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
			int iLen=oString.GetLength();
			strncpy((LPSTR) &m_bXmtBuffer[6],oString,iLen);
			m_bXmtBuffer[iLen+6]='\\';
			m_bXmtBuffer[iLen+7]='r';
			*((PWORD) &m_bXmtBuffer[iLen+8])=BinaryToHex(LRC(&m_bXmtBuffer[1],iLen+7));
			m_bXmtBuffer[iLen+10]='\r';
			m_wXmtLength=11+iLen;
		}

		// Read Input Module Counter
		else if(!strcmp(pTag->m_pszCommand,"W"))
		{
			m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			m_bXmtBuffer[4]='0';
			m_bXmtBuffer[5]='0';
			CTag* pCountersTag=pTag->m_pParent->m_pParent;
			POSITION	pos=pCountersTag->m_Branch.GetHeadPosition();
			BYTE	bCounters=0;
			while(pos)
			{
				CTag* pCounterTag=pCountersTag->m_Branch.GetNext(pos);
				bCounters|=0x01 << pCounterTag->m_dwItem;
			}
			*((PWORD) &m_bXmtBuffer[6])=BinaryToHex(bCounters);
			*((PWORD) &m_bXmtBuffer[8])=BinaryToHex(LRC(&m_bXmtBuffer[1],7));
			m_bXmtBuffer[10]='\r';
			m_wXmtLength=11;
		}


		// Read Requested Parameter/Variable to Host
		else if(!strcmp(pTag->m_pszCommand,"X"))
		{
			m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			m_bXmtBuffer[4]=pTag->m_pszSection[0];
			m_bXmtBuffer[5]=pTag->m_pszSection[1];
			*((PWORD) &m_bXmtBuffer[6])=BinaryToHex(LRC(&m_bXmtBuffer[1],5));
			m_bXmtBuffer[8]='\r';
			m_wXmtLength=9;
		}

		// Start Input Module Counter
		// Stop Input Module Counter
		// Clear Input Module Counter
		else if(!strcmp(pTag->m_pszCommand,"U")
		|| !strcmp(pTag->m_pszCommand,"V")
		|| !strcmp(pTag->m_pszCommand,"Y"))
		{
			// Issue Command to Configure module for input
			LPSTR	pszCommand=pTag->m_pszCommand;
			pTag->m_pszCommand="H";
			HRESULT hr=WriteTag(pTag);
			pTag->m_pszCommand=pszCommand;
			if(hr != S_OK)
			{
				pTag->m_wQuality=OPC_QUALITY_BAD;
				return hr;
			}

			BYTE	bData=0x01 << pTag->m_dwItem;
			m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			m_bXmtBuffer[4]='0';
			m_bXmtBuffer[5]='0';
			*((PWORD) &m_bXmtBuffer[6])=BinaryToHex(bData);
			*((PWORD) &m_bXmtBuffer[8])=BinaryToHex(LRC(&m_bXmtBuffer[1],7));
			m_bXmtBuffer[10]='\r';
			m_wXmtLength=11;
		}		

		else
			return E_FAIL;
	}	
	else
		return E_FAIL;

	return S_OK;
}

HRESULT CIO::ProcessResponse(CTag* pTag)
{
	// Replace the '\r' with '\0' to ensure string is null terminated
	m_bRcvBuffer[m_wRcvLength-1]='\0';
	if(pTag->m_pszCommand)
	{
		// Read Input Module On/Off Status
		if(!strcmp(pTag->m_pszCommand,"M"))
		{
			BYTE	bData=HexToBinary(*((PWORD) &m_bRcvBuffer[3]));
			CTag* pInputsTag=pTag->m_pParent;
			POSITION pos=pInputsTag->m_Leaf.GetHeadPosition();
			while(pos)
			{
				CTag* pInputTag=pInputsTag->m_Leaf.GetNext(pos);
				if(m_bRcvBuffer[0] == 'N')
					pInputTag->m_wQuality=OPC_QUALITY_BAD;
				else
				{
					pInputTag->m_Value.vt=VT_BOOL;
					pInputTag->m_Value.boolVal=(bData & (0x01 << pInputTag->m_dwItem)) ? TRUE : FALSE;
					pInputTag->m_wQuality=OPC_QUALITY_GOOD;
				}
				pInputTag->m_bCurrent=TRUE;
			}
		}

		// Power Up Clear
		// Reset
		// PIN Display Mode
		// Configure Module for Input
		// Configure Module for Output
		// Activate Output Module
		// Deactivate Output Module
		// Write Text Message To Display
		// Start Input Module Counter
		// Stop Input Module Counter
		// Clear Input Module Counter
		else if(!strcmp(pTag->m_pszCommand,"A")
		|| !strcmp(pTag->m_pszCommand,"B")
		|| !strcmp(pTag->m_pszCommand,"H")
		|| !strcmp(pTag->m_pszCommand,"I")
		|| !strcmp(pTag->m_pszCommand,"p")
		|| !strcmp(pTag->m_pszCommand,"q")
		|| !strcmp(pTag->m_pszCommand,"K")
		|| !strcmp(pTag->m_pszCommand,"L")
		|| !strcmp(pTag->m_pszCommand,"S")
		|| !strcmp(pTag->m_pszCommand,"U")
		|| !strcmp(pTag->m_pszCommand,"V")
		|| !strcmp(pTag->m_pszCommand,"Y"))
		{
			if(m_bRcvBuffer[0] == 'N')
				pTag->m_wQuality=OPC_QUALITY_BAD;
			else
				pTag->m_wQuality=OPC_QUALITY_GOOD;
		}

		// Read Input Module Counter
		else if(!strcmp(pTag->m_pszCommand,"W"))
		{
			CTag* pCountersTag=pTag->m_pParent->m_pParent;
			POSITION	pos=pCountersTag->m_Branch.GetHeadPosition();
			int iCount=0;
			while(pos)
			{
				CTag* pCounterTag=pCountersTag->m_Branch.GetNext(pos);
				CTag* pCountTag=pCounterTag->m_Leaf.GetHead();
				if(m_bRcvBuffer[0] == 'N')
					pCountTag->m_wQuality=OPC_QUALITY_BAD;
				else
				{
					WORD	wCount=0;
					*(((PBYTE) &wCount)+1)=HexToBinary(*((PWORD) &m_bRcvBuffer[1+4*iCount]));
					*((PBYTE) &wCount)=HexToBinary(*((PWORD) &m_bRcvBuffer[3+4*iCount]));
					pCountTag->m_Value.vt=VT_UI2;
					pCountTag->m_Value.lVal=wCount;
					pCountTag->m_wQuality=OPC_QUALITY_GOOD;
					pCountTag->m_bCurrent=TRUE;
				}
				iCount++;
			}
		}

		// Access/Modify display Attributes
		else if(!strcmp(pTag->m_pszCommand,"s"))
		{
			if(m_bRcvBuffer[0] == 'N')
			{
				VariantClear(&pTag->m_Value);
				pTag->m_wQuality=OPC_QUALITY_BAD;
			}
			else
			{
				// Selected List Item
				if(!strcmp(pTag->m_pszSection,"04"))
				{
					VariantClear(&pTag->m_Value);

					if(m_wRcvLength < 4)
						pTag->m_wQuality=OPC_QUALITY_BAD;
					else
					{
						m_bRcvBuffer[m_wRcvLength-3]='\0';
						CString strData((LPSTR) &m_bRcvBuffer[1]);
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value=SysAllocString(strData);
						pTag->m_bCurrent=TRUE;
					}
				}
				else
					pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
		}

		// Read Requested Parameter/Variable to host
		else if(!strcmp(pTag->m_pszCommand,"X"))
		{
			if(m_bRcvBuffer[0] == 'N')
			{
				VariantClear(&pTag->m_Value);
				pTag->m_wQuality=OPC_QUALITY_BAD;
			}
			else
			{
				// Magstripe Card
				// Keypad Data
				// Time
				if(!strcmp(pTag->m_pszSection,"00")
				|| !strcmp(pTag->m_pszSection,"01")
				|| !strcmp(pTag->m_pszSection,"02"))
				{
					VariantClear(&pTag->m_Value);

					if(m_wRcvLength < 4)
						pTag->m_wQuality=OPC_QUALITY_BAD;
					else
					{
						m_bRcvBuffer[m_wRcvLength-3]='\0';
						CString strData((LPSTR) &m_bRcvBuffer[1]);
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						pTag->m_Value=SysAllocString(strData);
						pTag->m_bCurrent=TRUE;
					}
				}
			}
		}

		else
			return E_FAIL;
	}
	else
		return E_FAIL;

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
			WORD wLRC=BinaryToHex(LRC(&m_bRcvBuffer[1],m_wRcvLength-4));
			if(LOBYTE(wLRC) != m_bRcvBuffer[m_wRcvLength-3]
			|| HIBYTE(wLRC) != m_bRcvBuffer[m_wRcvLength-2])
				continue;
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
	 	if(!SetCommMask(m_hPort,EV_ERR | EV_RXFLAG))
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

		switch(WaitForSingleObject(CommOverLapped.hEvent,500))
		{
			case WAIT_OBJECT_0:
	   		if((dwCommEvtFlags & EV_ERR ) == EV_ERR)
					continue;

   			else if((dwCommEvtFlags & EV_RXFLAG ) == EV_RXFLAG )
				{
					if(!ClearCommError(m_hPort,&dwCommErrFlags,&ComStat))
						continue;

					if(ComStat.cbInQue > sizeof(m_bRcvBuffer)-1)
						continue;

					if(!ReadFile(m_hPort,m_bRcvBuffer,ComStat.cbInQue,&dwNumberOfBytesRead,&ReadOverLapped)
					&& GetLastError() != ERROR_IO_PENDING )
						continue;

				 	if(!GetOverlappedResult(m_hPort,&ReadOverLapped,&dwNumberOfBytesRead,TRUE))
						continue;

					break;
				}
				else
					continue;

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

		m_wRcvLength=(WORD) dwNumberOfBytesRead; 

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
			WORD wLRC=BinaryToHex(LRC(&m_bRcvBuffer[1],m_wRcvLength-4));
			if(LOBYTE(wLRC) != m_bRcvBuffer[m_wRcvLength-3]
			|| HIBYTE(wLRC) != m_bRcvBuffer[m_wRcvLength-2])
				continue;
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
										OPTOMUX_BAUD			Baud,
										OPTOMUX_DATA_BITS	DataBits,
										OPTOMUX_PARITY		Parity,
										OPTOMUX_STOP_BITS	StopBits)
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