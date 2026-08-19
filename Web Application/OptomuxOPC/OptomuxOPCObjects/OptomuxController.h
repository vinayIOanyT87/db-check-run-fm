/******************************************************************************

	FILE NAME:		OptomuxController.h


	PURPOSE:			Declaration of the COptomuxController


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

class ATL_NO_VTABLE COptomuxController : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<COptomuxController, &CLSID_OptomuxController>,
	public ISupportErrorInfo,
	public IProvideClassInfoImpl< &CLSID_OptomuxController, &LIBID_OptomuxOPCObjectsLib>,
	public IDispatchImpl<IOptomuxController, &IID_IOptomuxController, &LIBID_OptomuxOPCObjectsLib, /*wMajor =*/ 1, /*wMinor =*/ 0>,
	public IPersistStreamInitImpl<COptomuxController>,
	public IMarshalOnStreamImpl<COptomuxController>
{
public:
	BOOL									m_bRequiresSave;
protected:
	LONG									m_lIndex;
	_bstr_t								m_bstrID;
	OPTOMUX_TYPE						m_Type;
	BYTE									m_bAddress;
	LONG									m_lPortIndex;
	BYTE									m_bModuleInputOutputMap;
	_bstr_t								m_bstrPortID;
	BOOL									m_bNetworkCommunications;
	_bstr_t								m_bstrIPAddress;
	LONG									m_lPort;

public:
	COptomuxController()
	{
		raw_Reset();
	}

DECLARE_REGISTRY_RESOURCEID(IDR_OPTOMUXCONTROLLER)


BEGIN_COM_MAP(COptomuxController)
	COM_INTERFACE_ENTRY(IOptomuxController)
	COM_INTERFACE_ENTRY2(IDispatch,IOptomuxController)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IPersistStreamInit)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()

BEGIN_PROP_MAP(COptomuxController)
	PROP_DATA_ENTRY("[Index]", m_lIndex, VT_UI4)
	PROP_ENTRY_EX("ID", 2, CLSID_NULL, IID_IOptomuxController)
	PROP_DATA_ENTRY("Type", m_Type, VT_UI4)
	PROP_DATA_ENTRY("Address", m_bAddress, VT_UI1)
	PROP_DATA_ENTRY("PortIndex", m_lPortIndex, VT_UI4)
	PROP_DATA_ENTRY("ModuleInputOutputMap", m_bModuleInputOutputMap, VT_UI1)
	PROP_DATA_ENTRY("NetworkCommunications", m_bNetworkCommunications, VT_BOOL)
	PROP_ENTRY_EX("IPAddress", 8, CLSID_NULL, IID_IOptomuxController)
	PROP_DATA_ENTRY("Port", m_lPort, VT_UI4)
	PROP_ENTRY_EX("PortID", 10, CLSID_NULL, IID_IOptomuxController)
END_PROP_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid);

	DECLARE_PROTECT_FINAL_CONSTRUCT()


public:
	STDMETHOD(get_Index)(LONG* pVal);
	STDMETHOD(put_Index)(LONG newVal);
	STDMETHOD(get_ID)(BSTR* pVal);
	STDMETHOD(put_ID)(BSTR newVal);
	STDMETHOD(get_Type)(OPTOMUX_TYPE* pVal);
	STDMETHOD(put_Type)(OPTOMUX_TYPE newVal);
	STDMETHOD(get_Address)(BYTE* pVal);
	STDMETHOD(put_Address)(BYTE newVal);
	STDMETHOD(get_PortIndex)(LONG* pVal);
	STDMETHOD(put_PortIndex)(LONG newVal);
	STDMETHOD(get_ModuleInputOutputMap)(BYTE* pVal);
	STDMETHOD(put_ModuleInputOutputMap)(BYTE newVal);
	STDMETHOD(get_NetworkCommunications)(VARIANT_BOOL* pVal);
	STDMETHOD(put_NetworkCommunications)(VARIANT_BOOL newVal);
	STDMETHOD(get_IPAddress)(BSTR* pVal);
	STDMETHOD(put_IPAddress)(BSTR newVal);
	STDMETHOD(get_Port)(LONG* pVal);
	STDMETHOD(put_Port)(LONG newVal);
	STDMETHOD(get_PortID)(BSTR* pVal);
	STDMETHOD(put_PortID)(BSTR newVal);
	STDMETHOD(raw_TypeID)(OPTOMUX_TYPE Type,BSTR* pVal);
	STDMETHOD(raw_Load)(IDispatch* pRecordset);
	STDMETHOD(raw_Reset)(void);
	STDMETHOD(get_InsertSQL)(BSTR* pVal);
	STDMETHOD(get_UpdateSQL)(BSTR* pVal);
	STDMETHOD(get_PurgeSQL)(BSTR* pVal);
	STDMETHOD(get_SelectSQL)(BSTR* pVal);
	STDMETHOD(get_SelectByIDSQL)(BSTR* pVal);
	STDMETHOD(get_EnumerateSQL)(BSTR* pVal);

};

OBJECT_ENTRY_AUTO(__uuidof(OptomuxController), COptomuxController)
