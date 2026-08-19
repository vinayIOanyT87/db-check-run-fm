/******************************************************************************

	FILE NAME:		IO.h


	PURPOSE:			Declaration of the CIO


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
#pragma once

class CIO;
class CTag;

typedef enum 
{
	TRUCK_PRESENT_TAG = 1,
	TRUCK_SERIAL_NUMBER_TAG = 2,
	BYPASS_TAG = 3
} SCULLY_TAG_TYPE;

typedef CTypedPtrList<CPtrList,CTag*> CTagList;

class CTag
{
	public:
	CTag(LPCTSTR szName);
	CTag(INT iID);
	~CTag();

	SCULLY_TAG_TYPE	m_ScullyTagType;
	BOOL						m_bLeaf;
   CString					m_oName;
   CString					m_oDescription;
	CString					m_oUnits;
	BYTE						m_bAddress;
   VARTYPE					m_NativeType;
	DWORD						m_dwAccessRights;
   COleVariant				m_Value;
   BOOL						m_bActive;     // FALSE when manually overriding
   FILETIME					m_Timestamp;
   WORD						m_wQuality;
   BOOL						m_bEnableProcessing;
	DWORD						m_dwScanCount;
	DWORD						m_dwUpdateRate;
	DWORD						m_dwUpdateCount;
	BOOL						m_bCurrent;

	CTag*						m_pParent;
	CTagList					m_Branch;
	CTagList					m_Leaf;
	CIO*						m_pIO;

   CString  GetPathName();

	CTag*	FindTag(const CString& oTag);

	CTag* AddBranch(LPCTSTR szName,CIO* pIO);
	CTag* AddBranch(INT iID,CIO* pIO);

	CTag* AddLeaf(	SCULLY_TAG_TYPE	ScullyTagType,
						LPCTSTR					szName,
						BYTE						bAddress,
						DWORD						dwAccessRights,
						VARTYPE					NativeType,
						CIO*						pIO);

	CTag* AddLeaf(	SCULLY_TAG_TYPE	ScullyTagType,
						INT						iID,
						BYTE						bAddress,
						DWORD						dwAccessRights,
						VARTYPE					NativeType,
						CIO*						pIO);
};

#define BUFFER_MAX 255

class CIO
{
public:
	CWinThread*				m_pScanThread;			// Scan Thread
	HANDLE					m_hKillEvent;			// Event Handle for signaling termination
	DWORD						m_dwUseCount;
	LONG						m_lIndex;
	CString					m_oPort;

	BYTE						m_DeviceID;
	DWORD						m_dwBaud;
	BYTE						m_bDataBits;
	BYTE						m_bParity;
	BYTE						m_bStopBits;

	BYTE m_bPortParametersChanged;


	HANDLE					m_hPort;
	OVERLAPPED				ReadOverLapped;
	OVERLAPPED				WriteOverLapped;
	OVERLAPPED				CommOverLapped;
	CRITICAL_SECTION		m_cs;
	WORD						m_wXmtLength;
	BYTE						m_bXmtBuffer[BUFFER_MAX];
	WORD						m_wRcvLength;
	BYTE						m_bRcvBuffer[BUFFER_MAX];
	CTagList					m_TagScanList;
	BOOL						m_bCommFailLogged;
protected:
	HANDLE					m_hLogFile;
	CString m_csBaseLogFileName;
	COleDateTime m_odtLastLogTime;
public:
	CIO(	BYTE								lDeviceID,
			LONG								lIndex,
			LPCTSTR							szPort,
			SCULLY_BAUD				dwBaud,
			SCULLY_DATA_BITS		bDataBits,
			SCULLY_PARITY			bParity,
			SCULLY_STOP_BITS		bStopBits);
	~CIO();

	HRESULT OpenComPort();
	void CloseComPort();
	void SignalCommunicationsFailure(CTag* pTag);
	void SetQuality(CTag* pRoot,WORD wQuality);
	HRESULT ReadTag(CTag* pTag);
	HRESULT WriteTag(CTag* pTag);
	HRESULT PrepareRequest(CTag* pTag,BOOL bWrite);
	HRESULT PerformIO(CTag* pTag);
	HRESULT ProcessResponse(CTag* pTag,HRESULT hr);
	void ReportError(CTag* pTag);
	void AddTagToScanList(CTag* pTag,DWORD dwUpdateRate);
	void RemoveTagFromScanList(CTag* pTag);
	void SetPortParameters(	LPCTSTR					szPort,
									SCULLY_BAUD			Baud,
									SCULLY_DATA_BITS	DataBits,
									SCULLY_PARITY		Parity,
									SCULLY_STOP_BITS	StopBits);

	protected:
	static UINT ScanThread(LPVOID lpDeviceManager);
	void Scan();
	void SetDeviceBaudRate(SCULLY_BAUD dwBaud);
	void SetDeviceParity(SCULLY_PARITY m_bParity);
	void SetDeviceDataBits(SCULLY_DATA_BITS bDataBits);
	void SetDeviceStopBits(SCULLY_STOP_BITS bStopBits);

	protected:
	void LogRead(BYTE* buffer, WORD length);
	void LogWrite(BYTE* buffer, WORD length);
	void LogError();
	void CycleLogFile();
	WORD CRC(BYTE* pbBuffer,WORD wXmtLength);
	int getBit(int number, int position);
};

typedef CTypedPtrList<CPtrList,CIO*> CIOList;
