// ContrecCollection.h : Declaration of the CContrecCollection

#pragma once
#include "resource.h"       // main symbols
#include "ComCollection.h"	// Template Collection class
//#include "ContrecOPCObjects.h"


// CContrecCollection

//class ATL_NO_VTABLE CContrecCollection : 
//	public CComObjectRootEx<CComMultiThreadModel>,
//	public CComCoClass<CContrecCollection, &CLSID_ContrecCollection>,
//	public IDispatchImpl<IContrecCollection, &IID_IContrecCollection, &LIBID_ContrecOPCObjectsLib, /*wMajor =*/ 1, /*wMinor =*/ 0>

class ATL_NO_VTABLE CContrecCollection : 
	public CComCollection<IContrec, CContrecCollection, IContrecCollection, &CLSID_ContrecCollection, &IID_IContrecCollection, &LIBID_ContrecOPCObjectsLib>

{
public:
	CContrecCollection()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_CONTRECCOLLECTION)


BEGIN_COM_MAP(CContrecCollection)
	COM_INTERFACE_ENTRY(IContrecCollection)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(IEnumVARIANT)
	COM_INTERFACE_ENTRY(IPersistStream)
	COM_INTERFACE_ENTRY2(IPersist, IPersistStream)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()


	DECLARE_PROTECT_FINAL_CONSTRUCT()

public:

};

OBJECT_ENTRY_AUTO(__uuidof(ContrecCollection), CContrecCollection)
