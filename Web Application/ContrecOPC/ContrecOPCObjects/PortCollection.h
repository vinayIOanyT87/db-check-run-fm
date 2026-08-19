// PortCollection.h : Declaration of the CPortCollection
#pragma once
#include "resource.h"       // main symbols
#include "ComCollection.h"	// Template Collection class
//#include "ContrecOPCObjects.h"


// CPortCollection

//class ATL_NO_VTABLE CPortCollection : 
//	public CComObjectRootEx<CComMultiThreadModel>,
//	public CComCoClass<CPortCollection, &CLSID_PortCollection>,
//	public IDispatchImpl<IPortCollection, &IID_IPortCollection, &LIBID_ContrecOPCObjectsLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
class ATL_NO_VTABLE CPortCollection : 
	public CComCollection<IPort, CPortCollection, IPortCollection, &CLSID_PortCollection, &IID_IPortCollection, &LIBID_ContrecOPCObjectsLib>
{
public:
	CPortCollection()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_PORTCOLLECTION)


BEGIN_COM_MAP(CPortCollection)
	COM_INTERFACE_ENTRY(IPortCollection)
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

OBJECT_ENTRY_AUTO(__uuidof(PortCollection), CPortCollection)
