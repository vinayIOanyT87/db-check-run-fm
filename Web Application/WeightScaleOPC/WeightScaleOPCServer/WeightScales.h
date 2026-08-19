/******************************************************************************

	FILE NAME:		WeightScales.h


	PURPOSE:			Declaration of the CWeightScales


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
#include <comsvcs.h>
#include <mtxattr.h>


// CWeightScales

class ATL_NO_VTABLE CWeightScales : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public IObjectControl,
	public CComCoClass<CWeightScales, &CLSID_WeightScales>,
	public ISupportErrorInfo,
	public IDispatchImpl<IWeightScales, &IID_IWeightScales, &LIBID_WeightScaleOPCServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
	IDataAccessPtr		m_oDataAccess;
public:
	CWeightScales()
	{
	}

	HRESULT Validate(IWeightScalePtr	oWeightScale);

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_WEIGHTSCALES)

DECLARE_NOT_AGGREGATABLE(CWeightScales)

BEGIN_COM_MAP(CWeightScales)
	COM_INTERFACE_ENTRY(IWeightScales)
	COM_INTERFACE_ENTRY(IObjectControl)
	COM_INTERFACE_ENTRY2(IDispatch,IWeightScales)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
	{
		static const IID* arr[] = 
		{
			&IID_IWeightScales
		};
		for (int i = 0; i < sizeof(arr) / sizeof(arr[0]); i++)
		{
			if (InlineIsEqualGUID(*arr[i], riid))
				return S_OK;
		}
		return S_FALSE;
	}

// IObjectControl
public:
	STDMETHOD(Activate)();
	STDMETHOD_(BOOL, CanBePooled)();
	STDMETHOD_(void, Deactivate)();

	IObjectContextPtr m_oObjectContext;

// IWeightScales
public:
	STDMETHOD(raw_Enumerate)(IDispatch** ppWeightScaleCollection);
	STDMETHOD(raw_Add)(IDispatch* pWeightScale,LONG* plIndex);
	STDMETHOD(raw_Modify)(IDispatch* pWeightScale);
	STDMETHOD(raw_Purge)(LONG lIndex);
	STDMETHOD(raw_GetIndex)(BSTR bstrID, LONG* plIndex);
	STDMETHOD(raw_Get)(LONG lIndex, IDispatch** ppWeightScale);
	STDMETHOD(raw_EnumeratePortIDs)(VARIANT* pIDs);
};

OBJECT_ENTRY_AUTO(__uuidof(WeightScales), CWeightScales)
