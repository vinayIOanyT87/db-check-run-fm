/******************************************************************************

	FILE NAME:		IO.h


	PURPOSE:			Declaration of the CIO


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
	Date:			By:			Reason:
	-----------	----------  -------------------------------------------
	01/18/2008	W.Gray		7.3.1.1 - Added support TCP/IP	
*******************************************************************************/
#pragma once

#define MAX_INACTIVITY 20	// Constant which determines in 100 msec when port is closed
									// after inactivity.
#define BUFFER_MAX 512


class CIO;
class CTag;
class CDevice;

typedef CTypedPtrList<CPtrList,CTag*> CTagList;

class CTag
{
	public:
	CTag(LPCTSTR szName);
	CTag(INT iID);
	~CTag();

	BOOL				m_bLeaf;
   CString        m_oName;
   CString        m_oDescription;
	CString			m_oUnits;
	BYTE				m_bAddress;
	LPSTR				m_pszCommand;
	LPSTR				m_pszSection;
	DWORD				m_dwItem;
   VARTYPE        m_NativeType;
	DWORD				m_dwAccessRights;
   COleVariant    m_Value;
   BOOL           m_bActive;     // FALSE when manually overriding
   FILETIME       m_Timestamp;
   WORD           m_wQuality;
   BOOL           m_bEnableProcessing;
	DWORD				m_dwScanCount;
	DWORD				m_dwUpdateRate;
	DWORD				m_dwUpdateCount;
	BOOL				m_bCurrent;
	DWORD				m_dwUpdateSequence;  // Used to insure 

	CTag*				m_pParent;
	CTagList			m_Branch;
	CTagList			m_Leaf;
	CIO*				m_pIO;
	CDevice*			m_pDevice;

   CString  GetPathName();

	CTag*	FindTag(const CString& oTag);

	CTag* AddBranch(LPCTSTR szName,CIO* pIO,CDevice* pDevice);
	CTag* AddBranch(INT iID,CIO* pIO,CDevice* pDevice);

	CTag* AddLeaf(LPCTSTR szName,BYTE bAddress,LPSTR pszCommand,LPSTR pszSection,DWORD dwItem,DWORD dwAccessRights,VARTYPE NativeType,CIO* pIO,CDevice* pDevice);
	CTag* AddLeaf(INT iID,BYTE bAddress,LPSTR pszCommand,LPSTR pszSection,DWORD dwItem,DWORD dwAccessRights,VARTYPE NativeType,CIO* pIO,CDevice* pDevice);
};


class CIO
{
public:
	CWinThread*				m_pScanThread;			// Scan Thread
	HANDLE					m_hKillEvent;			// Event Handle for signaling termination
	DWORD						m_dwUseCount;
	LONG						m_lIndex;
	CString					m_oPort;
	ACCULOAD_BAUD			m_Baud;
	ACCULOAD_DATA_BITS	m_DataBits;
	ACCULOAD_PARITY		m_Parity;
	ACCULOAD_STOP_BITS	m_StopBits;
	HANDLE					m_hPort;
	OVERLAPPED				m_ReadOverLapped;
	OVERLAPPED				m_WriteOverLapped;
	OVERLAPPED				m_CommOverLapped;
	CRITICAL_SECTION		m_cs;
	CTagList					m_TagScanList;
	BOOL						m_bCommFailLogged;
	BOOL						m_bPortParametersChanged;
	int						m_iInactivityCounter;
	BOOL						m_bNetworkCommunications;
	CString					m_oIPAddress;
	UINT						m_uiPort;
	SOCKET					m_hSocket;
	CAsyncSocket*			m_pSocket;
	DWORD						m_dwCommunicationsTimeOut;
	WORD						m_wXmtLength;
	BYTE						m_bXmtBuffer[BUFFER_MAX];
	WORD						m_wRcvLength;
	BYTE						m_bRcvBuffer[BUFFER_MAX];
protected:
	HANDLE					m_hLogFile;
	CString m_csBaseLogFileName;
	COleDateTime m_odtLastLogTime;

public:
	CIO(	LONG						lIndex,
			LPCTSTR					szPort,
			ACCULOAD_BAUD			Baud,
			ACCULOAD_DATA_BITS	DataBits,
			ACCULOAD_PARITY		Parity,
			ACCULOAD_STOP_BITS	StopBits);

	CIO(LPTSTR pIPAddress,	UINT uiPort);
	~CIO();

	void Init();
	void CloseComPort();
	HRESULT OpenComPort(ACCULOAD_TYPE type);
	LPTSTR SocketError(DWORD dwError);
	HRESULT OpenSocket(CTag* pTag);
	HRESULT ReadTag(CTag* pTag);
	HRESULT WriteTag(CTag* pTag);
	void AddTagToScanList(CTag* pTag,DWORD dwUpdateRate);
	void RemoveTagFromScanList(CTag* pTag);
	void SetPortParameters(	LPCTSTR					szPort,
									ACCULOAD_BAUD			Baud,
									ACCULOAD_DATA_BITS	DataBits,
									ACCULOAD_PARITY		Parity,
									ACCULOAD_STOP_BITS	StopBits);

	void SignalCommunicationsFailure(CTag* pTag);
	void SignalCommunicationsFailure(CTag* pTag, DWORD dwError);
	void SignalCommunicationsRestored(CTag* pTag);
	void SetQuality(CTag* pRoot,WORD wQuality);
	BYTE	LRC(BYTE* pbBuffer,WORD wXmtLength);

	protected:
	static UINT ScanThread(LPVOID lpDeviceManager);
	void Scan();

public:
	void LogRead(BYTE* buffer, WORD length);
	void LogWrite(BYTE* buffer, WORD length);
	void LogError();
	void LogMessage(LPCTSTR tszMessage);
protected:
	void CycleLogFile();
};

typedef CTypedPtrList<CPtrList,CIO*> CIOList;
