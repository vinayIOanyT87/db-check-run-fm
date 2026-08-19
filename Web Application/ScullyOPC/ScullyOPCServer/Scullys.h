/******************************************************************************

	FILE NAME:		Scullys.h


	PURPOSE:			Declaration of the CScullys


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
#include <comsvcs.h>
#include <mtxattr.h>


// CScullys

class ATL_NO_VTABLE CScullys : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public IObjectControl,
	public CComCoClass<CScullys, &CLSID_Scullys>,
	public ISupportErrorInfo,
	public IDispatchImpl<IScullys, &IID_IScullys, &LIBID_ScullyOPCServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
	IDataAccessPtr		m_oDataAccess;
public:
	CScullys()
	{
	}

	HRESULT Validate(IScullyPtr	oScully);

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_SCULLYS)

DECLARE_NOT_AGGREGATABLE(CScullys)

BEGIN_COM_MAP(CScullys)
	COM_INTERFACE_ENTRY(IScullys)
	COM_INTERFACE_ENTRY(IObjectControl)
	COM_INTERFACE_ENTRY2(IDispatch,IScullys)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
	{
		static const IID* arr[] = 
		{
			&IID_IScullys
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

// IScullys
public:
	STDMETHOD(raw_Enumerate)(IDispatch** ppScullyCollection);
	STDMETHOD(raw_Add)(IDispatch* pScully,LONG* plIndex);
	STDMETHOD(raw_Modify)(IDispatch* pScully);
	STDMETHOD(raw_Purge)(LONG lIndex);
	STDMETHOD(raw_GetIndex)(BSTR bstrID, LONG* plIndex);
	STDMETHOD(raw_Get)(LONG lIndex, IDispatch** ppScully);
	STDMETHOD(raw_EnumeratePortIDs)(VARIANT* pIDs);
};

OBJECT_ENTRY_AUTO(__uuidof(Scullys), CScullys)
