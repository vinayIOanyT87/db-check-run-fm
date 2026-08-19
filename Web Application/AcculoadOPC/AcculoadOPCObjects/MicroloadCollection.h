// MicroloadCollection.h : Declaration of the CMicroloadCollection

#pragma once
#include "resource.h"       // main symbols
#include "ComCollection.h"	// Template Collection class


// CMicroloadCollection

class ATL_NO_VTABLE CMicroloadCollection : 
	public CComCollection<IMicroload, CMicroloadCollection, IMicroloadCollection, &CLSID_MicroloadCollection, &IID_IMicroloadCollection, &LIBID_AcculoadOPCObjectsLib>
{
public:
	CMicroloadCollection()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_MICROLOADCOLLECTION)


BEGIN_COM_MAP(CMicroloadCollection)
	COM_INTERFACE_ENTRY(IMicroloadCollection)
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

OBJECT_ENTRY_AUTO(__uuidof(MicroloadCollection), CMicroloadCollection)
