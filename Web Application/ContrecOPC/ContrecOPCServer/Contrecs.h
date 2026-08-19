// Contrecs.h : Declaration of the CContrecs
/******************************************************************************

	FILE NAME:		Contrecs.h


	PURPOSE:			Declaration of the Contrecs


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	B. Schaal


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
*******************************************************************************/


#pragma once
#include "resource.h"       // main symbols
#include <comsvcs.h>
#include <mtxattr.h>


// CContrecs

class ATL_NO_VTABLE CContrecs : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public IObjectControl,
	public CComCoClass<CContrecs, &CLSID_Contrecs>,
	public ISupportErrorInfo,
	public IDispatchImpl<IContrecs, &IID_IContrecs, &LIBID_ContrecOPCServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
	IDataAccessPtr		m_oDataAccess;
public:
	CContrecs()
	{
	}
	HRESULT Validate(IContrecPtr	oContrec);

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_CONTRECS)


BEGIN_COM_MAP(CContrecs)
	COM_INTERFACE_ENTRY(IContrecs)
	COM_INTERFACE_ENTRY(IObjectControl)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
	{
		static const IID* arr[] = 
		{
			&IID_IContrecs
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

public:
	STDMETHOD(raw_Enumerate)(IDispatch** ppContrecCollection);
	STDMETHOD(raw_Add)(IDispatch* pContrec,LONG* plIndex);
	STDMETHOD(raw_Modify)(IDispatch* pContrec);
	STDMETHOD(raw_Purge)(LONG lIndex);
	STDMETHOD(raw_GetIndex)(BSTR bstrID, LONG* plIndex);
	STDMETHOD(raw_Get)(LONG lIndex, IDispatch** ppContrec);

};

OBJECT_ENTRY_AUTO(__uuidof(Contrecs), CContrecs)
