/******************************************************************************

	FILE NAME:		DanLoad.h


	PURPOSE:			Declaration of the CDanLoad


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
#include "resource.h"       // main symbols

// CDanLoad

class ATL_NO_VTABLE CDanLoad : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CDanLoad, &CLSID_DanLoad>,
	public ISupportErrorInfo,
	public IProvideClassInfoImpl< &CLSID_DanLoad, &LIBID_DanielOPCObjectsLib>,
	public IDispatchImpl<IDanLoad, &IID_IDanLoad, &LIBID_DanielOPCObjectsLib>,
	public IPersistStreamInitImpl<CDanLoad>,
	public IMarshalOnStreamImpl<CDanLoad>
{
public:
	BOOL							m_bRequiresSave;
protected:
	LONG							m_lIndex;
	_bstr_t						m_bstrID;
	DANLOAD_TYPE				m_Type;
	LONG							m_lPortIndex;
	_bstr_t						m_bstrPortID;
	BYTE							m_bAddress;
public:
	CDanLoad()
	{
		raw_Reset();
	}


DECLARE_REGISTRY_RESOURCEID(IDR_DANLOAD)

DECLARE_NOT_AGGREGATABLE(CDanLoad)

BEGIN_COM_MAP(CDanLoad)
	COM_INTERFACE_ENTRY(IDanLoad)
	COM_INTERFACE_ENTRY2(IDispatch,IDanLoad)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IPersistStreamInit)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()

// Note: the property map must match the recordset

BEGIN_PROP_MAP(CDanLoad)
	PROP_DATA_ENTRY("[Index]", m_lIndex, VT_UI4)
	PROP_ENTRY_EX("ID", 2, CLSID_NULL, IID_IDanLoad)
	PROP_DATA_ENTRY("Type", m_Type, VT_UI4)
	PROP_DATA_ENTRY("PortIndex", m_lPortIndex, VT_UI4)
	PROP_DATA_ENTRY("Address", m_bAddress, VT_UI1)
	PROP_ENTRY_EX("PortID", 5, CLSID_NULL, IID_IDanLoad)
END_PROP_MAP()


// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid);

	DECLARE_PROTECT_FINAL_CONSTRUCT()

// IDanLoad
public:
	STDMETHOD(get_Index)(LONG* pVal);
	STDMETHOD(put_Index)(LONG newVal);
	STDMETHOD(get_ID)(BSTR* pVal);
	STDMETHOD(put_ID)(BSTR newVal);
	STDMETHOD(get_Type)(DANLOAD_TYPE* pVal);
	STDMETHOD(put_Type)(DANLOAD_TYPE newVal);
	STDMETHOD(get_PortIndex)(LONG* pVal);
	STDMETHOD(put_PortIndex)(LONG newVal);
	STDMETHOD(get_PortID)(BSTR* pVal);
	STDMETHOD(put_PortID)(BSTR newVal);
	STDMETHOD(get_Address)(BYTE* pVal);
	STDMETHOD(put_Address)(BYTE newVal);
	STDMETHOD(raw_TypeID)(DANLOAD_TYPE Type,BSTR* pVal);
	STDMETHOD(raw_Load)(IDispatch* pRecordset);
	STDMETHOD(raw_Reset)(void);
	STDMETHOD(get_InsertSQL)(BSTR* pVal);
	STDMETHOD(get_UpdateSQL)(BSTR* pVal);
	STDMETHOD(get_PurgeSQL)(BSTR* pVal);
	STDMETHOD(get_SelectSQL)(BSTR* pVal);
	STDMETHOD(get_SelectByIDSQL)(BSTR* pVal);
	STDMETHOD(get_EnumerateSQL)(BSTR* pVal);
};

OBJECT_ENTRY_AUTO(__uuidof(DanLoad), CDanLoad)
