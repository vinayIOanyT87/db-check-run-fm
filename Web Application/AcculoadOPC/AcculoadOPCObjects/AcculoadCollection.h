// AcculoadCollection.h : Declaration of the CAcculoadCollection

#pragma once
#include "resource.h"       // main symbols
#include "ComCollection.h"	// Template Collection class


// CAcculoadCollection

class ATL_NO_VTABLE CAcculoadCollection : 
	public CComCollection<IAccuload, CAcculoadCollection, IAcculoadCollection, &CLSID_AcculoadCollection, &IID_IAcculoadCollection, &LIBID_AcculoadOPCObjectsLib>
{
public:
	CAcculoadCollection()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ACCULOADCOLLECTION)


BEGIN_COM_MAP(CAcculoadCollection)
	COM_INTERFACE_ENTRY(IAcculoadCollection)
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

OBJECT_ENTRY_AUTO(__uuidof(AcculoadCollection), CAcculoadCollection)
