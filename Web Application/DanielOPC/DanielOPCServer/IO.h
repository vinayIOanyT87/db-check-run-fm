/******************************************************************************

	FILE NAME:		IO.h


	PURPOSE:			Declaration of the CIO


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
*******************************************************************************/
#pragma once

class CDevice;
class CIO;
class CTag;

class CDevice
{
public:
	BYTE				m_bAddress;
	BYTE				m_bLastFunctionCode;
	BOOL				m_bOffline;
	SHORT				m_sNumberOfMeters;
	SHORT				m_sNumberOfComponents;
	SHORT				m_sNumberOfValves;
	SHORT				m_sNumberOfFactors;
	SHORT				m_sNumberOfRecipes;
	SHORT				m_sNumberOfAdditives;
	BYTE				m_bTempUnits; /* 0=Celcius, 1=Fahrenheit */

public:
	CDevice(BYTE bAddress)
	{
		m_bAddress=bAddress;
		m_bOffline=TRUE;
	}
};


typedef CTypedPtrList<CPtrList,CTag*> CTagList;

class CTag
{
	public:
	CTag(LPCTSTR szName,BYTE bCommand);
	CTag(INT iID,BYTE bCommand);
	~CTag();

	BOOL				m_bLeaf;
   CString        m_oName;
   CString        m_oDescription;
	CString			m_oUnits;
	BYTE				m_bCommand;
	BYTE				m_bSection;
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
	DWORD				m_dwUpdateSequence;  // Used to insure change of state detected 

	CTag*				m_pParent;
	CTagList			m_Branch;
	CTagList			m_Leaf;
	CIO*				m_pIO;
	CDevice*			m_pDevice;

   CString  GetPathName();

	CTag*	FindTag(const CString& oTag);

	CTag* AddBranch(LPCTSTR szName,BYTE bCommand,CIO* pIO,CDevice* pDevice);
	CTag* AddBranch(INT iID,BYTE bCommand,CIO* pIO,CDevice* pDevice);

	CTag* AddLeaf(LPCTSTR szName,BYTE bCommand,BYTE bSection,DWORD dwItem,DWORD dwAccessRights,VARTYPE NativeType,CIO* pIO,CDevice* pDevice);
	CTag* AddLeaf(INT iID,BYTE bCommand,BYTE bSection,DWORD dwItem,DWORD dwAccessRights,VARTYPE NativeType,CIO* pIO,CDevice* pDevice);
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
	DANLOAD_BAUD			m_Baud;
	DANLOAD_DATA_BITS		m_DataBits;
	DANLOAD_PARITY			m_Parity;
	DANLOAD_STOP_BITS		m_StopBits;
	HANDLE					m_hPort;
	CRITICAL_SECTION		m_cs;
	WORD						m_wXmtLength;
	BYTE						m_bXmtBuffer[BUFFER_MAX];
	WORD						m_wRcvLength;
	BYTE						m_bRcvBuffer[BUFFER_MAX];
	CTagList					m_TagScanList;
	BOOL						m_bCommFailLogged;
	BOOL						m_bPortParametersChanged;
	DWORD						m_dwCommunicationsTimeOut;
	int						m_iInactivityCounter;

	CIO(	LONG						lIndex,
			LPCTSTR					szPort,
			DANLOAD_BAUD			Baud,
			DANLOAD_DATA_BITS		DataBits,
			DANLOAD_PARITY			Parity,
			DANLOAD_STOP_BITS		StopBits);
	~CIO();

	HRESULT OpenComPort();
	HRESULT ReadTag(CTag* pTag);
	HRESULT WriteTag(CTag* pTag);
	HRESULT PrepareRequest(CTag* pTag);
	HRESULT PerformIO(CTag* pTag);
	HRESULT ProcessResponse(CTag* pTag,HRESULT hr);
	void ReportError(CTag* pTag);
	WORD	CRC(BYTE* pbBuffer,WORD wXmtLength);
	void AddTagToScanList(CTag* pTag,DWORD dwUpdateRate);
	void RemoveTagFromScanList(CTag* pTag);
	void SetPortParameters(	LPCTSTR					szPort,
									DANLOAD_BAUD			Baud,
									DANLOAD_DATA_BITS	DataBits,
									DANLOAD_PARITY		Parity,
									DANLOAD_STOP_BITS	StopBits);

	protected:
	static UINT ScanThread(LPVOID lpDeviceManager);
	void Scan();
	void SignalCommunicationsFailure(CTag* pTag);
	void SignalCommunicationsRestored(CTag* pTag);
	void SetQuality(CTag* pRoot,WORD wQuality);
	void UpdateComponentValueTags(CTag* pParent,WORD wQuality);
	void UpdateStatusTags(CTag* pParent,WORD wQuality);
	void UpdateAdditiveTotalizerTags(CTag* pParent,WORD wQuality);
	void UpdateComponentTotalizerTags(CTag* pParent,WORD wQuality);
	VARENUM DanLoadDataType(short Code);
};

typedef CTypedPtrList<CPtrList,CIO*> CIOList;
