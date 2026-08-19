/******************************************************************************

	FILE NAME:		Arm.h


	PURPOSE:			Declaration of the CArm


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

// CArm

class ATL_NO_VTABLE CArm : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CArm, &CLSID_Arm>,
	public ISupportErrorInfo,
	public IProvideClassInfoImpl< &CLSID_Arm, &LIBID_AcculoadOPCObjectsLib>,
	public IDispatchImpl<IArm, &IID_IArm, &LIBID_AcculoadOPCObjectsLib, /*wMajor =*/ 1, /*wMinor =*/ 0>,
	public IPersistStreamInitImpl<CArm>,
	public IMarshalOnStreamImpl<CArm>
{
public:
	BOOL					m_bRequiresSave;
protected:
	LONG					m_lIndex;
	LONG					m_lAcculoadIndex;
	BYTE					m_bNumber;
	BYTE					m_bAddress;
	ACCULOAD_ARM_TYPE	m_Type;
	BYTE					m_bProducts;


public:
	CArm()
	{
		raw_Reset();
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ARM)


BEGIN_COM_MAP(CArm)
	COM_INTERFACE_ENTRY(IArm)
	COM_INTERFACE_ENTRY2(IDispatch,IArm)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IPersistStreamInit)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()

BEGIN_PROP_MAP(CArm)
	PROP_DATA_ENTRY("[Index]", m_lIndex, VT_UI4)
	PROP_DATA_ENTRY("AcculoadIndex", m_lAcculoadIndex, VT_UI4)
	PROP_DATA_ENTRY("Number", m_bNumber, VT_UI1)
	PROP_DATA_ENTRY("Address", m_bAddress, VT_UI1)
	PROP_DATA_ENTRY("Type", m_Type, VT_UI4)
	PROP_DATA_ENTRY("Products", m_bProducts, VT_UI1)
END_PROP_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid);

	DECLARE_PROTECT_FINAL_CONSTRUCT()


public:
	STDMETHOD(get_Index)(LONG* pVal);
	STDMETHOD(put_Index)(LONG newVal);
	STDMETHOD(get_AcculoadIndex)(LONG* pVal);
	STDMETHOD(put_AcculoadIndex)(LONG newVal);
	STDMETHOD(get_Number)(BYTE* pVal);
	STDMETHOD(put_Number)(BYTE newVal);
	STDMETHOD(get_Address)(BYTE* pVal);
	STDMETHOD(put_Address)(BYTE newVal);
	STDMETHOD(get_Type)(ACCULOAD_ARM_TYPE* pVal);
	STDMETHOD(put_Type)(ACCULOAD_ARM_TYPE newVal);
	STDMETHOD(get_Products)(BYTE* pVal);
	STDMETHOD(put_Products)(BYTE newVal);
	STDMETHOD(raw_TypeID)(ACCULOAD_ARM_TYPE Type,BSTR* pVal);
	STDMETHOD(raw_Load)(IDispatch* pRecordset);
	STDMETHOD(raw_Reset)(void);
	STDMETHOD(get_InsertSQL)(BSTR* pVal);
	STDMETHOD(get_UpdateSQL)(BSTR* pVal);
	STDMETHOD(get_PurgeSQL)(BSTR* pVal);
	STDMETHOD(get_SelectSQL)(BSTR* pVal);
	STDMETHOD(get_SelectByAcculoadIndexAndNumberSQL)(BSTR* pVal);
	STDMETHOD(get_EnumerateByAcculoadIndexSQL)(BSTR* pVal);
};

OBJECT_ENTRY_AUTO(__uuidof(Arm), CArm)
