/******************************************************************************

	FILE NAME:		OptomuxControllers.h


	PURPOSE:			Declaration of the COptomuxControllers


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


// COptomuxControllers

class ATL_NO_VTABLE COptomuxControllers : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public IObjectControl,
	public CComCoClass<COptomuxControllers, &CLSID_OptomuxControllers>,
	public ISupportErrorInfo,
	public IDispatchImpl<IOptomuxControllers, &IID_IOptomuxControllers, &LIBID_OptomuxOPCServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
	IDataAccessPtr		m_oDataAccess;
public:
	COptomuxControllers()
	{
	}

	HRESULT CheckConstraints(IOptomuxControllerPtr	oOptomuxController);

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_OPTOMUXCONTROLLERS)

DECLARE_NOT_AGGREGATABLE(COptomuxControllers)

BEGIN_COM_MAP(COptomuxControllers)
	COM_INTERFACE_ENTRY(IOptomuxControllers)
	COM_INTERFACE_ENTRY(IObjectControl)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
	{
		static const IID* arr[] = 
		{
			&IID_IOptomuxControllers
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

// IOptomuxControllers
public:
	STDMETHOD(raw_Enumerate)(IDispatch** ppOptomuxControllerCollection);
	STDMETHOD(raw_Add)(IDispatch* pOptomuxController,LONG* plIndex);
	STDMETHOD(raw_Modify)(IDispatch* pOptomuxController);
	STDMETHOD(raw_Purge)(LONG lIndex);
	STDMETHOD(raw_GetIndex)(BSTR bstrID, LONG* plIndex);
	STDMETHOD(raw_Get)(LONG lIndex, IDispatch** ppOptomuxController);
};

OBJECT_ENTRY_AUTO(__uuidof(OptomuxControllers), COptomuxControllers)
