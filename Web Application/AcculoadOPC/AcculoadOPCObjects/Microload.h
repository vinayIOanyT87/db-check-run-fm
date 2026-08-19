/******************************************************************************

	FILE NAME:		Microload.h


	PURPOSE:			Declaration of the CMicroload


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2006

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
*******************************************************************************/

#pragma once
#include "resource.h"       // main symbols

// CMicroload

class ATL_NO_VTABLE CMicroload : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CMicroload, &CLSID_Microload>,
	public ISupportErrorInfo,
	public IProvideClassInfoImpl< &CLSID_Microload, &LIBID_AcculoadOPCObjectsLib>,
	public IDispatchImpl<IMicroload, &IID_IMicroload, &LIBID_AcculoadOPCObjectsLib>,
	public IPersistStreamInitImpl<CMicroload>,
	public IMarshalOnStreamImpl<CMicroload>
{
public:
	BOOL							m_bRequiresSave;
protected:
	LONG							m_lIndex;
	_bstr_t						m_bstrID;
	ACCULOAD_TYPE				m_Type;
	LONG							m_lPortIndex;
	IArmCollectionPtr			m_oArms;
	_bstr_t						m_bstrPortID;
public:
	CMicroload()
	{
		raw_Reset();
	}

DECLARE_REGISTRY_RESOURCEID(IDR_MICROLOAD)


BEGIN_COM_MAP(CMicroload)
	COM_INTERFACE_ENTRY(IMicroload)
	COM_INTERFACE_ENTRY2(IDispatch,IMicroload)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IPersistStreamInit)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()

// Note: the property map must match the recordset
//       Arms is not in the record set and must after
//       items in the recordset

BEGIN_PROP_MAP(CMicroload)
	PROP_DATA_ENTRY("[Index]", m_lIndex, VT_UI4)
	PROP_ENTRY_EX("ID", 2, CLSID_NULL, IID_IMicroload)
	PROP_DATA_ENTRY("Type", m_Type, VT_UI4)
	PROP_DATA_ENTRY("PortIndex", m_lPortIndex, VT_UI4)
	PROP_ENTRY_EX("PortID", 6, CLSID_NULL, IID_IMicroload)
	PROP_ENTRY_EX("Arms", 5, CLSID_NULL, IID_IMicroload)
END_PROP_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid);

	DECLARE_PROTECT_FINAL_CONSTRUCT()


public:

	STDMETHOD(get_Index)(LONG* pVal);
	STDMETHOD(put_Index)(LONG newVal);
	STDMETHOD(get_ID)(BSTR* pVal);
	STDMETHOD(put_ID)(BSTR newVal);
	STDMETHOD(get_Type)(ACCULOAD_TYPE* pVal);
	STDMETHOD(put_Type)(ACCULOAD_TYPE newVal);
	STDMETHOD(get_PortIndex)(LONG* pVal);
	STDMETHOD(put_PortIndex)(LONG newVal);
	STDMETHOD(get_Arms)(IDispatch** pVal);
	STDMETHOD(put_Arms)(IDispatch* newVal);
	STDMETHOD(get_PortID)(BSTR* pVal);
	STDMETHOD(put_PortID)(BSTR newVal);
	STDMETHOD(raw_TypeID)(ACCULOAD_TYPE Type,BSTR* pVal);
	STDMETHOD(raw_Load)(IDispatch* pRecordset);
	STDMETHOD(raw_Reset)(void);
	STDMETHOD(get_InsertSQL)(BSTR* pVal);
	STDMETHOD(get_UpdateSQL)(BSTR* pVal);
	STDMETHOD(get_PurgeSQL)(BSTR* pVal);
	STDMETHOD(get_SelectSQL)(BSTR* pVal);
	STDMETHOD(get_SelectByIDSQL)(BSTR* pVal);
	STDMETHOD(get_EnumerateSQL)(BSTR* pVal);
};

OBJECT_ENTRY_AUTO(__uuidof(Microload), CMicroload)
