// Contrec.h : Declaration of the CContrec

#pragma once
#include "resource.h"       // main symbols

//#include "ContrecOPCObjects.h"


// CContrec

class ATL_NO_VTABLE CContrec : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CContrec, &CLSID_Contrec>,
	public ISupportErrorInfo,
	public IProvideClassInfoImpl< &CLSID_Contrec, &LIBID_ContrecOPCObjectsLib>,
	public IDispatchImpl<IContrec, &IID_IContrec, &LIBID_ContrecOPCObjectsLib, /*wMajor =*/ 1, /*wMinor =*/ 0>,
	public IPersistStreamInitImpl<CContrec>,
	public IMarshalOnStreamImpl<CContrec>
{
public:
	BOOL							m_bRequiresSave;
protected:
	LONG							m_lIndex;
	_bstr_t						m_bstrID;
	CONTREC_TYPE				m_Type;
	LONG							m_lPortIndex;
	_bstr_t						m_bstrPortID;
	BYTE							m_bAddress;
public:
	CContrec()
	{
		raw_Reset();
	}

DECLARE_REGISTRY_RESOURCEID(IDR_CONTREC)


BEGIN_COM_MAP(CContrec)
	COM_INTERFACE_ENTRY(IContrec)
	COM_INTERFACE_ENTRY2(IDispatch,IContrec)
	COM_INTERFACE_ENTRY(ISupportErrorInfo)
	COM_INTERFACE_ENTRY(IPersistStreamInit)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()

BEGIN_PROP_MAP(CContrec)
	PROP_DATA_ENTRY("[Index]", m_lIndex, VT_UI4)
	PROP_ENTRY_EX("ID", 2, CLSID_NULL, IID_IContrec)
	PROP_DATA_ENTRY("Type", m_Type, VT_UI4)
	PROP_DATA_ENTRY("PortIndex", m_lPortIndex, VT_UI4)
	PROP_DATA_ENTRY("Address", m_bAddress, VT_UI1)
	PROP_ENTRY_EX("PortID", 5, CLSID_NULL, IID_IContrec)
END_PROP_MAP()

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
	STDMETHOD(get_Index)(LONG* pVal);
	STDMETHOD(put_Index)(LONG newVal);
	STDMETHOD(get_ID)(BSTR* pVal);
	STDMETHOD(put_ID)(BSTR newVal);
	STDMETHOD(get_Type)(CONTREC_TYPE* pVal);
	STDMETHOD(put_Type)(CONTREC_TYPE newVal);
	STDMETHOD(get_PortIndex)(LONG* pVal);
	STDMETHOD(put_PortIndex)(LONG newVal);
	STDMETHOD(get_PortID)(BSTR* pVal);
	STDMETHOD(put_PortID)(BSTR newVal);
	STDMETHOD(get_Address)(BYTE* pVal);
	STDMETHOD(put_Address)(BYTE newVal);
	STDMETHOD(raw_TypeID)(CONTREC_TYPE Type,BSTR* pVal);
	STDMETHOD(raw_Load)(IDispatch* pRecordset);
	STDMETHOD(raw_Reset)(void);
	STDMETHOD(get_InsertSQL)(BSTR* pVal);
	STDMETHOD(get_UpdateSQL)(BSTR* pVal);
	STDMETHOD(get_PurgeSQL)(BSTR* pVal);
	STDMETHOD(get_SelectSQL)(BSTR* pVal);
	STDMETHOD(get_SelectByIDSQL)(BSTR* pVal);
	STDMETHOD(get_EnumerateSQL)(BSTR* pVal);

};

OBJECT_ENTRY_AUTO(__uuidof(Contrec), CContrec)
