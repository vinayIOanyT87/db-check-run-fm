/******************************************************************************

	FILE NAME:		Scully.h


	PURPOSE:			Declaration of the CScully


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
#include "resource.h"       // main symbols

// CScully

class ATL_NO_VTABLE CScully : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CScully, &CLSID_Scully>,
	public ISupportErrorInfo,
	public IProvideClassInfoImpl< &CLSID_Scully, &LIBID_ScullyOPCObjectsLib>,
	public IDispatchImpl<IScully, &IID_IScully, &LIBID_ScullyOPCObjectsLib>,
	public IPersistStreamInitImpl<CScully>,
	public IMarshalOnStreamImpl<CScully>
{
public:
	BOOL							m_bRequiresSave;
	LONG							m_lIndex;
	_bstr_t						m_bstrID;
	LONG							m_lPortIndex;
	LONG							m_DeviceID;


	CScully()
	{
		raw_Reset();
	}

DECLARE_REGISTRY_RESOURCEID(IDR_Scully)


BEGIN_COM_MAP(CScully)
	COM_INTERFACE_ENTRY(IScully)
	COM_INTERFACE_ENTRY2(IDispatch,IScully)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IPersistStreamInit)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()

BEGIN_PROP_MAP(CScully)
	PROP_DATA_ENTRY("[Index]", m_lIndex, VT_UI4)
	PROP_ENTRY_EX("ID", 2, CLSID_NULL, IID_IScully)
	PROP_DATA_ENTRY("PortIndex", m_lPortIndex, VT_UI4)
	PROP_DATA_ENTRY("DeviceID", m_DeviceID, VT_UI4)
//	PROP_ENTRY_EX("Port", 4, CLSID_NULL, IID_IScully)
END_PROP_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid);

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

public:

	STDMETHOD(get_Index)(LONG* pVal);
	STDMETHOD(put_Index)(LONG newVal);
	STDMETHOD(get_ID)(BSTR* pVal);
	STDMETHOD(put_ID)(BSTR newVal);
//	STDMETHOD(get_Port)(BSTR* pVal);
//	STDMETHOD(put_Port)(BSTR newVal);
	STDMETHOD(raw_Load)(IDispatch* pRecordset);
	STDMETHOD(raw_Reset)(void);
	STDMETHOD(get_InsertSQL)(BSTR* pVal);
	STDMETHOD(get_UpdateSQL)(BSTR* pVal);
	STDMETHOD(get_PurgeSQL)(BSTR* pVal);
	STDMETHOD(get_SelectSQL)(BSTR* pVal);
	STDMETHOD(get_SelectByIDSQL)(BSTR* pVal);
	STDMETHOD(get_EnumerateSQL)(BSTR* pVal);
	STDMETHOD(get_PortIndex)(LONG* pVal);
	STDMETHOD(put_PortIndex)(LONG newVal);
	STDMETHOD(get_DeviceID)(LONG* pVal);
	STDMETHOD(put_DeviceID)(LONG newVal);
};

OBJECT_ENTRY_AUTO(__uuidof(Scully), CScully)
