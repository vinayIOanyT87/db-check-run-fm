/******************************************************************************

	FILE NAME:		Arms.h


	PURPOSE:			Declaration of the CArms


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


// CArms

class ATL_NO_VTABLE CArms : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public IObjectControl,
	public CComCoClass<CArms, &CLSID_Arms>,
	public ISupportErrorInfo,
	public IDispatchImpl<IArms, &IID_IArms, &LIBID_AcculoadOPCServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
	IDataAccessPtr		m_oDataAccess;
public:
	CArms()
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

DECLARE_REGISTRY_RESOURCEID(IDR_ARMS)

BEGIN_COM_MAP(CArms)
	COM_INTERFACE_ENTRY(IArms)
	COM_INTERFACE_ENTRY(IObjectControl)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
	{
		static const IID* arr[] = 
		{
			&IID_IArms
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

// IArms
public:
	STDMETHOD(raw_EnumerateByAcculoadIndex)(LONG lIndex,IDispatch** ppArm);
	STDMETHOD(raw_Add)(IDispatch* pArm,LONG* plIndex);
	STDMETHOD(raw_Modify)(IDispatch* pAccuload);
	STDMETHOD(raw_Purge)(LONG lIndex);
	STDMETHOD(raw_GetIndex)(LONG lAcculoadIndex, BYTE bNumber, LONG* plIndex);
	STDMETHOD(raw_Get)(LONG lIndex, IDispatch** ppAccuload);
	STDMETHOD(raw_ModifyCollection)(LONG lIndex, IDispatch* pArms);
};

OBJECT_ENTRY_AUTO(__uuidof(Arms), CArms)
