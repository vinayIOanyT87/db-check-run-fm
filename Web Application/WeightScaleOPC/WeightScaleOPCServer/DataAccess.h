// DataAccess.h : Declaration of the CDataAccess

#pragma once
#include "resource.h"       // main symbols
#include <comsvcs.h>
#include <mtxattr.h>


// CDataAccess

class ATL_NO_VTABLE CDataAccess : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public IObjectControl,
	public ISupportErrorInfo,
	public CComCoClass<CDataAccess, &CLSID_DataAccess>,
	public IDispatchImpl<IDataAccess, &IID_IDataAccess, &LIBID_WeightScaleOPCServerLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CDataAccess()
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

DECLARE_REGISTRY_RESOURCEID(IDR_DATAACCESS)

DECLARE_NOT_AGGREGATABLE(CDataAccess)

BEGIN_COM_MAP(CDataAccess)
	COM_INTERFACE_ENTRY(IDataAccess)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IObjectControl)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
	{
		static const IID* arr[] = 
		{
			&IID_IDataAccess
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

// IDataAccess
public:
	STDMETHOD(raw_GetRecordSet)(BSTR bstrSQL, IDispatch** ppRecordSet);
	STDMETHOD(raw_ExecuteQuery)(BSTR bstrSQL);
};

OBJECT_ENTRY_AUTO(__uuidof(DataAccess), CDataAccess)
