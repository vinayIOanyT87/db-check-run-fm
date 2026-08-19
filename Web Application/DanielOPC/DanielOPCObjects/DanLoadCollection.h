// DanLoadCollection.h : Declaration of the CDanLoadCollection

#pragma once
#include "resource.h"       // main symbols
#include "ComCollection.h"	// Template Collection class


// CDanLoadCollection

class ATL_NO_VTABLE CDanLoadCollection : 
	public CComCollection<IDanLoad, CDanLoadCollection, IDanLoadCollection, &CLSID_DanLoadCollection, &IID_IDanLoadCollection, &LIBID_DanielOPCObjectsLib>
{
public:
	CDanLoadCollection()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_DANLOADCOLLECTION)


BEGIN_COM_MAP(CDanLoadCollection)
	COM_INTERFACE_ENTRY(IDanLoadCollection)
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

OBJECT_ENTRY_AUTO(__uuidof(DanLoadCollection), CDanLoadCollection)
