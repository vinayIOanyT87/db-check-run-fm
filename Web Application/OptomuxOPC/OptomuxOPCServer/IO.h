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
*******************************************************************************/
#pragma once

class CIO;
class CTag;

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

	CTag*				m_pParent;
	CTagList			m_Branch;
	CTagList			m_Leaf;
	CIO*				m_pIO;

   CString  GetPathName();

	CTag*	FindTag(const CString& oTag);

	CTag* AddBranch(LPCTSTR szName,CIO* pIO);
	CTag* AddBranch(INT iID,CIO* pIO);

	CTag* AddLeaf(LPCTSTR szName,BYTE bAddress,LPSTR pszCommand,LPSTR pszSection,DWORD dwItem,DWORD dwAccessRights,VARTYPE NativeType,CIO* pIO);
	CTag* AddLeaf(INT iID,BYTE bAddress,LPSTR pszCommand,LPSTR pszSection,DWORD dwItem,DWORD dwAccessRights,VARTYPE NativeType,CIO* pIO);
};

class CIO
{
public:
	CWinThread*						m_pScanThread;			// Scan Thread
	HANDLE							m_hKillEvent;			// Event Handle for signaling termination
	DWORD								m_dwUseCount;
	LONG								m_lIndex;
	CString							m_oPort;
	OPTOMUX_BAUD					m_Baud;
	OPTOMUX_DATA_BITS				m_DataBits;
	OPTOMUX_PARITY					m_Parity;
	OPTOMUX_STOP_BITS				m_StopBits;
	HANDLE							m_hPort;
	OVERLAPPED						ReadOverLapped;
	OVERLAPPED						WriteOverLapped;
	OVERLAPPED						CommOverLapped;
	CRITICAL_SECTION				m_cs;
	WORD								m_wXmtLength;
	BYTE								m_bXmtBuffer[255];
	WORD								m_wRcvLength;
	BYTE								m_bRcvBuffer[255];
	CTagList							m_TagScanList;
	BOOL								m_bCommFailLogged;
	BOOL								m_bPortParametersChanged;
	BOOL								m_bNetworkCommunications;
	CString							m_oIPAddress;
	LONG								m_lPort;
protected:
	HANDLE					m_hLogFile;
	CString m_csBaseLogFileName;
	COleDateTime m_odtLastLogTime;

public:
	CIO(	LPCTSTR	szIPAddress,
			LONG		lPort);

	CIO(	LONG					lIndex,
			LPCTSTR				szPort,
			OPTOMUX_BAUD		Baud,
			OPTOMUX_DATA_BITS	DataBits,
			OPTOMUX_PARITY		Parity,
			OPTOMUX_STOP_BITS	StopBits);
	~CIO();

	void CloseComPort();
	HRESULT OpenComPort();
	HRESULT OpenSocket();
	HRESULT ReadTag(CTag* pTag);
	HRESULT WriteTag(CTag* pTag);
	HRESULT PrepareRequest(CTag* pTag,BOOL bWrite);
	HRESULT PerformSerialIO(CTag* pTag);
	HRESULT PerformNetworkIO(CTag*pTag);
	HRESULT ProcessResponse(CTag* pTag);
	BYTE	LRC(BYTE* pbBuffer,WORD wXmtLength);
	WORD	BinaryToHex(BYTE bBinary);
	BYTE	HexToBinary(WORD wHex);
	void AddTagToScanList(CTag* pTag,DWORD dwUpdateRate);
	void RemoveTagFromScanList(CTag* pTag);
	void SetPortParameters(	LPCTSTR				szPort,
									OPTOMUX_BAUD		Baud,
									OPTOMUX_DATA_BITS	DataBits,
									OPTOMUX_PARITY		Parity,
									OPTOMUX_STOP_BITS	StopBits);
	LPTSTR SocketError(DWORD dwError);

	protected:
	static UINT ScanThread(LPVOID lpOptomuxControllerManager);
	void Scan();
	SOCKET			m_hSocket;
	CAsyncSocket*	m_pSocket;
	void LogRead(BYTE* buffer, WORD length);
	void LogWrite(BYTE* buffer, WORD length);
	void LogError();
	void CycleLogFile();
};

typedef CTypedPtrList<CPtrList,CIO*> CIOList;
