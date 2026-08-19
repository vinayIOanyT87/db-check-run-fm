/******************************************************************************

	FILE NAME:		Acculoads.h


	PURPOSE:			Declaration of the CAcculoads


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


// CAcculoads

class ATL_NO_VTABLE CAcculoads : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public IObjectControl,
	public CComCoClass<CAcculoads, &CLSID_Acculoads>,
	public ISupportErrorInfo,
	public IDispatchImpl<IAcculoads, &IID_IAcculoads, &LIBID_AcculoadOPCServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
	IDataAccessPtr		m_oDataAccess;
public:
	CAcculoads()
	{
	}
	HRESULT Validate(IAcculoadPtr	oAccuload);

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ACCULOADS)

BEGIN_COM_MAP(CAcculoads)
	COM_INTERFACE_ENTRY(IAcculoads)
	COM_INTERFACE_ENTRY(IObjectControl)
	COM_INTERFACE_ENTRY2(IDispatch,IAcculoads)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()


// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
	{
		static const IID* arr[] = 
		{
			&IID_IAcculoads
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

// IAcculoads
public:
	STDMETHOD(raw_Enumerate)(IDispatch** ppAcculoadCollection);
	STDMETHOD(raw_Add)(IDispatch* pAccuload,LONG* plIndex);
	STDMETHOD(raw_Modify)(IDispatch* pAccuload);
	STDMETHOD(raw_Purge)(LONG lIndex);
	STDMETHOD(raw_GetIndex)(BSTR bstrID, LONG* plIndex);
	STDMETHOD(raw_Get)(LONG lIndex, IDispatch** ppAccuload);
};

OBJECT_ENTRY_AUTO(__uuidof(Acculoads), CAcculoads)
