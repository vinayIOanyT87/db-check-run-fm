/******************************************************************************

	FILE NAME:		OsdpController.h


	PURPOSE:			Declaration of the COsdpController


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
#include "resource.h"       // main symbols

class ATL_NO_VTABLE COsdpController : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<COsdpController, &CLSID_OsdpController>,
	public ISupportErrorInfo,
	public IProvideClassInfoImpl< &CLSID_OsdpController, &LIBID_OsdpOPCObjectsLib>,
	public IDispatchImpl<IOsdpController, &IID_IOsdpController, &LIBID_OsdpOPCObjectsLib, /*wMajor =*/ 1, /*wMinor =*/ 0>,
	public IPersistStreamInitImpl<COsdpController>,
	public IMarshalOnStreamImpl<COsdpController>
{
public:
	BOOL									m_bRequiresSave;
protected:
	LONG									m_lIndex;
	_bstr_t								m_bstrID;
	BYTE									m_bAddress;
	LONG									m_lPortIndex;
	_bstr_t								m_bstrPortID;

public:
	COsdpController()
	{
		raw_Reset();
	}

DECLARE_REGISTRY_RESOURCEID(IDR_OSDPCONTROLLER)


BEGIN_COM_MAP(COsdpController)
	COM_INTERFACE_ENTRY(IOsdpController)
	COM_INTERFACE_ENTRY2(IDispatch,IOsdpController)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IPersistStreamInit)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()

BEGIN_PROP_MAP(COsdpController)
	PROP_DATA_ENTRY("[Index]", m_lIndex, VT_UI4)
	PROP_ENTRY_EX("ID", 2, CLSID_NULL, IID_IOsdpController)
	PROP_DATA_ENTRY("PortIndex", m_lPortIndex, VT_UI4)
	PROP_DATA_ENTRY("Address", m_bAddress, VT_UI1)
	PROP_ENTRY_EX("PortID", 5, CLSID_NULL, IID_IOsdpController)
END_PROP_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid);

	DECLARE_PROTECT_FINAL_CONSTRUCT()


public:
	STDMETHOD(get_Index)(LONG* pVal);
	STDMETHOD(put_Index)(LONG newVal);
	STDMETHOD(get_ID)(BSTR* pVal);
	STDMETHOD(put_ID)(BSTR newVal);
	STDMETHOD(get_Address)(BYTE* pVal);
	STDMETHOD(put_Address)(BYTE newVal);
	STDMETHOD(get_PortIndex)(LONG* pVal);
	STDMETHOD(put_PortIndex)(LONG newVal);
	STDMETHOD(get_PortID)(BSTR* pVal);
	STDMETHOD(put_PortID)(BSTR newVal);
	STDMETHOD(raw_Load)(IDispatch* pRecordset);
	STDMETHOD(raw_Reset)(void);
	STDMETHOD(get_InsertSQL)(BSTR* pVal);
	STDMETHOD(get_UpdateSQL)(BSTR* pVal);
	STDMETHOD(get_PurgeSQL)(BSTR* pVal);
	STDMETHOD(get_SelectSQL)(BSTR* pVal);
	STDMETHOD(get_SelectByIDSQL)(BSTR* pVal);
	STDMETHOD(get_EnumerateSQL)(BSTR* pVal);

};

OBJECT_ENTRY_AUTO(__uuidof(OsdpController), COsdpController)
