/******************************************************************************

	FILE NAME:		Accuload.h


	PURPOSE:			Declaration of the CAccuload


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
		01/17/2008	W.Gray		7.3.1.0 - Added support for TCP/IP
*******************************************************************************/

#pragma once
#include "resource.h"       // main symbols

// CAccuload

class ATL_NO_VTABLE CAccuload : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CAccuload, &CLSID_Accuload>,
	public ISupportErrorInfo,
	public IProvideClassInfoImpl< &CLSID_Accuload, &LIBID_AcculoadOPCObjectsLib>,
	public IDispatchImpl<IAccuload, &IID_IAccuload, &LIBID_AcculoadOPCObjectsLib>,
	public IPersistStreamInitImpl<CAccuload>,
	public IMarshalOnStreamImpl<CAccuload>
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
	BOOL							m_bNetworkCommunications;
	_bstr_t						m_bstrIPAddress;
public:
	CAccuload()
	{
		raw_Reset();
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ACCULOAD)


BEGIN_COM_MAP(CAccuload)
	COM_INTERFACE_ENTRY(IAccuload)
	COM_INTERFACE_ENTRY2(IDispatch,IAccuload)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IPersistStreamInit)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()

// Note: the property map must match the recordset
//       PortID & Arms is not in the record set and
//			must after items in the recordset

BEGIN_PROP_MAP(CAccuload)
	PROP_DATA_ENTRY("[Index]", m_lIndex, VT_UI4)
	PROP_ENTRY_EX("ID", 2, CLSID_NULL, IID_IAccuload)
	PROP_DATA_ENTRY("Type", m_Type, VT_UI4)
	PROP_DATA_ENTRY("PortIndex", m_lPortIndex, VT_UI4)
	PROP_DATA_ENTRY("NetworkCommunications", m_bNetworkCommunications, VT_BOOL)
	PROP_ENTRY_EX("IPAddress", 8, CLSID_NULL, IID_IAccuload)
	PROP_ENTRY_EX("PortID", 6, CLSID_NULL, IID_IAccuload)
	PROP_ENTRY_EX("Arms", 5, CLSID_NULL, IID_IAccuload)
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
	STDMETHOD(get_NetworkCommunications)(VARIANT_BOOL* pVal);
	STDMETHOD(put_NetworkCommunications)(VARIANT_BOOL newVal);
	STDMETHOD(get_IPAddress)(BSTR* pVal);
	STDMETHOD(put_IPAddress)(BSTR newVal);
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

OBJECT_ENTRY_AUTO(__uuidof(Accuload), CAccuload)
