/******************************************************************************

	FILE NAME:		WeightScale.h


	PURPOSE:			Declaration of the CWeightScale


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

// CWeightScale

class ATL_NO_VTABLE CWeightScale : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CWeightScale, &CLSID_WeightScale>,
	public ISupportErrorInfo,
	public IProvideClassInfoImpl< &CLSID_WeightScale, &LIBID_WeightScaleOPCObjectsLib>,
	public IDispatchImpl<IWeightScale, &IID_IWeightScale, &LIBID_WeightScaleOPCObjectsLib>,
	public IPersistStreamInitImpl<CWeightScale>,
	public IMarshalOnStreamImpl<CWeightScale>
{
public:
	BOOL							m_bRequiresSave;
	LONG							m_lIndex;
	_bstr_t							m_bstrID;
	WEIGHTSCALE_TYPE				m_Type;
	LONG							m_lPortIndex;
	LONG							m_DeviceID;
	_bstr_t							m_bstrPort;


	CWeightScale()
	{
		raw_Reset();
	}

DECLARE_REGISTRY_RESOURCEID(IDR_WEIGHTSCALE)


BEGIN_COM_MAP(CWeightScale)
	COM_INTERFACE_ENTRY(IWeightScale)
	COM_INTERFACE_ENTRY2(IDispatch,IWeightScale)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IPersistStreamInit)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()

BEGIN_PROP_MAP(CWeightScale)
	PROP_DATA_ENTRY("[Index]", m_lIndex, VT_UI4)
	PROP_ENTRY_EX("ID", 2, CLSID_NULL, IID_IWeightScale)
	PROP_DATA_ENTRY("Type", m_Type, VT_UI4)
	PROP_DATA_ENTRY("PortIndex", m_lPortIndex, VT_UI4)
	PROP_DATA_ENTRY("DeviceID", m_DeviceID, VT_UI4)
	PROP_ENTRY_EX("Port", 5, CLSID_NULL, IID_IWeightScale)
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
	STDMETHOD(get_Type)(WEIGHTSCALE_TYPE* pVal);
	STDMETHOD(put_Type)(WEIGHTSCALE_TYPE newVal);
	STDMETHOD(get_Port)(BSTR* pVal);
	STDMETHOD(put_Port)(BSTR newVal);
	STDMETHOD(raw_TypeID)(WEIGHTSCALE_TYPE Type,BSTR* pVal);
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

OBJECT_ENTRY_AUTO(__uuidof(WeightScale), CWeightScale)
