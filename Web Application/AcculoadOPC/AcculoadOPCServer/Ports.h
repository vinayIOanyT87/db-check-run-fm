/******************************************************************************

	FILE NAME:		Ports.h


	PURPOSE:			Declaration of the CPorts


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


// CPorts

class ATL_NO_VTABLE CPorts : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public IObjectControl,
	public CComCoClass<CPorts, &CLSID_Ports>,
	public ISupportErrorInfo,
	public IDispatchImpl<IPorts, &IID_IPorts, &LIBID_AcculoadOPCServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
	IDataAccessPtr		m_oDataAccess;
public:
	CPorts()
	{
	}

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_PORTS)

DECLARE_NOT_AGGREGATABLE(CPorts)

BEGIN_COM_MAP(CPorts)
	COM_INTERFACE_ENTRY(IPorts)
	COM_INTERFACE_ENTRY(IObjectControl)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
	{
		static const IID* arr[] = 
		{
			&IID_IPorts
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

// IPorts
public:
	STDMETHOD(raw_Enumerate)(IDispatch** ppPortCollection);
	STDMETHOD(raw_Add)(IDispatch* pPort,LONG* plIndex);
	STDMETHOD(raw_Modify)(IDispatch* pPort);
	STDMETHOD(raw_Purge)(LONG lIndex);
	STDMETHOD(raw_GetIndex)(BSTR bstrID, LONG* plIndex);
	STDMETHOD(raw_Get)(LONG lIndex, IDispatch** ppPort);
	STDMETHOD(raw_EnumeratePortIDs)(VARIANT* pIDs);
};

OBJECT_ENTRY_AUTO(__uuidof(Ports), CPorts)
