// SecureHash.h : Declaration of the CSecureHash

#pragma once
#include "resource.h"       // main symbols

#include "ConsolidatedUtilities.h"


// CSecureHash

class ATL_NO_VTABLE CSecureHash : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CSecureHash, &CLSID_SecureHash>,
	public ISupportErrorInfo,
	public IDispatchImpl<ISecureHash, &IID_ISecureHash, &LIBID_ConsolidatedUtilitiesLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CSecureHash()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_SECUREHASH)


BEGIN_COM_MAP(CSecureHash)
	COM_INTERFACE_ENTRY(ISecureHash)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
END_COM_MAP()

// ISupportsErrorInfo
	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid);

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

public:

	STDMETHOD(HashPassword)(BSTR bstrUserID, BSTR bstrPassword, BSTR* pVal);
};

OBJECT_ENTRY_AUTO(__uuidof(SecureHash), CSecureHash)
