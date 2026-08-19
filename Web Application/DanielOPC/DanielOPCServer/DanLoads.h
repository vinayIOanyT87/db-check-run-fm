/******************************************************************************

	FILE NAME:		DanLoads.h


	PURPOSE:			Declaration of the CDanLoads


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
#include <comsvcs.h>
#include <mtxattr.h>


// CDanLoads

class ATL_NO_VTABLE CDanLoads : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public IObjectControl,
	public CComCoClass<CDanLoads, &CLSID_DanLoads>,
	public ISupportErrorInfo,
	public IDispatchImpl<IDanLoads, &IID_IDanLoads, &LIBID_DanielOPCServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
	IDataAccessPtr		m_oDataAccess;
public:
	CDanLoads()
	{
	}
	HRESULT Validate(IDanLoadPtr	oDanLoad);

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_DANLOADS)

BEGIN_COM_MAP(CDanLoads)
	COM_INTERFACE_ENTRY(IDanLoads)
	COM_INTERFACE_ENTRY(IObjectControl)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
	{
		static const IID* arr[] = 
		{
			&IID_IDanLoads
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

// IDanLoads
public:
	STDMETHOD(raw_Enumerate)(IDispatch** ppDanLoadCollection);
	STDMETHOD(raw_Add)(IDispatch* pDanLoad,LONG* plIndex);
	STDMETHOD(raw_Modify)(IDispatch* pDanLoad);
	STDMETHOD(raw_Purge)(LONG lIndex);
	STDMETHOD(raw_GetIndex)(BSTR bstrID, LONG* plIndex);
	STDMETHOD(raw_Get)(LONG lIndex, IDispatch** ppDanLoad);
};

OBJECT_ENTRY_AUTO(__uuidof(DanLoads), CDanLoads)
