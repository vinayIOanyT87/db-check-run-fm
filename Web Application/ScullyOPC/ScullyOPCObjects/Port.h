/******************************************************************************

	FILE NAME:		Port.h


	PURPOSE:			Declaration of the CPort


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	S. Jiang


	VERSION:		9.0.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
*******************************************************************************/

#pragma once
#include "resource.h"       // main symbols


// CPort

class ATL_NO_VTABLE CPort : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CPort, &CLSID_Port>,
	public ISupportErrorInfo,
	public IProvideClassInfoImpl< &CLSID_Port, &LIBID_ScullyOPCObjectsLib>,
	public IDispatchImpl<IPort, &IID_IPort, &LIBID_ScullyOPCObjectsLib>,
	public IPersistStreamInitImpl<CPort>,
	public IMarshalOnStreamImpl<CPort>
{
public:
	BOOL							m_bRequiresSave;
protected:
	LONG							m_lIndex;
	_bstr_t						m_bstrID;
	SCULLY_BAUD				m_Baud;
	SCULLY_DATA_BITS			m_DataBits;
	SCULLY_PARITY				m_Parity;
	SCULLY_STOP_BITS			m_StopBits;
public:
	CPort()
	{
		raw_Reset();
	}


DECLARE_REGISTRY_RESOURCEID(IDR_PORT)

BEGIN_COM_MAP(CPort)
	COM_INTERFACE_ENTRY(IPort)
	COM_INTERFACE_ENTRY2(IDispatch,IPort)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IPersistStreamInit)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()

BEGIN_PROP_MAP(CPort)
	PROP_DATA_ENTRY("[Index]", m_lIndex, VT_UI4)
	PROP_ENTRY_EX("ID", 2, CLSID_NULL, IID_IPort)
	PROP_DATA_ENTRY("Baud", m_Baud, VT_UI4)
	PROP_DATA_ENTRY("DataBits", m_DataBits, VT_UI4)
	PROP_DATA_ENTRY("Parity", m_Parity, VT_UI4)
	PROP_DATA_ENTRY("StopBits", m_StopBits, VT_UI4)
END_PROP_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid);

	DECLARE_PROTECT_FINAL_CONSTRUCT()

public:
	STDMETHOD(get_Index)(LONG* pVal);
	STDMETHOD(put_Index)(LONG newVal);
	STDMETHOD(get_ID)(BSTR* pVal);
	STDMETHOD(put_ID)(BSTR newVal);
	STDMETHOD(get_Baud)(SCULLY_BAUD* pVal);
	STDMETHOD(put_Baud)(SCULLY_BAUD newVal);
	STDMETHOD(get_DataBits)(SCULLY_DATA_BITS* pVal);
	STDMETHOD(put_DataBits)(SCULLY_DATA_BITS newVal);
	STDMETHOD(get_Parity)(SCULLY_PARITY* pVal);
	STDMETHOD(put_Parity)(SCULLY_PARITY newVal);
	STDMETHOD(get_StopBits)(SCULLY_STOP_BITS* pVal);
	STDMETHOD(put_StopBits)(SCULLY_STOP_BITS newVal);
	STDMETHOD(raw_BaudID)(SCULLY_BAUD Baud,BSTR* pVal);
	STDMETHOD(raw_DataBitsID)(SCULLY_DATA_BITS DataBits,BSTR* pVal);
	STDMETHOD(raw_ParityID)(SCULLY_PARITY Parity,BSTR* pVal);
	STDMETHOD(raw_StopBitsID)(SCULLY_STOP_BITS StopBits,BSTR* pVal);
	STDMETHOD(raw_Load)(IDispatch* pRecordset);
	STDMETHOD(raw_Reset)(void);
	STDMETHOD(get_InsertSQL)(BSTR* pVal);
	STDMETHOD(get_UpdateSQL)(BSTR* pVal);
	STDMETHOD(get_PurgeSQL)(BSTR* pVal);
	STDMETHOD(get_SelectSQL)(BSTR* pVal);
	STDMETHOD(get_SelectByIDSQL)(BSTR* pVal);
	STDMETHOD(get_EnumerateSQL)(BSTR* pVal);
};

OBJECT_ENTRY_AUTO(__uuidof(Port), CPort)
