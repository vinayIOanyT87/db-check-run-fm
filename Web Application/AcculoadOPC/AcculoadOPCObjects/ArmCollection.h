// ArmCollection.h : Declaration of the CArmCollection

#pragma once
#include "resource.h"       // main symbols
#include "ComCollection.h"	// Template Collection class


// CArmCollection

class ATL_NO_VTABLE CArmCollection : 
	public CComCollection<IArm, CArmCollection, IArmCollection, &CLSID_ArmCollection, &IID_IArmCollection, &LIBID_AcculoadOPCObjectsLib>
{
public:
	CArmCollection()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ARMCOLLECTION)


BEGIN_COM_MAP(CArmCollection)
	COM_INTERFACE_ENTRY(IArmCollection)
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

OBJECT_ENTRY_AUTO(__uuidof(ArmCollection), CArmCollection)
