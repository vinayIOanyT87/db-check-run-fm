// ScullyCollection.h : Declaration of the CScullyCollection

#pragma once
#include "resource.h"       // main symbols
#include "ComCollection.h"	// Template Collection class


// CScullyCollection

class ATL_NO_VTABLE CScullyCollection : 
	public CComCollection<IScully, CScullyCollection, IScullyCollection, &CLSID_ScullyCollection, &IID_IScullyCollection, &LIBID_ScullyOPCObjectsLib>
{
public:
	CScullyCollection()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ScullyCOLLECTION)


BEGIN_COM_MAP(CScullyCollection)
	COM_INTERFACE_ENTRY(IScullyCollection)
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

OBJECT_ENTRY_AUTO(__uuidof(ScullyCollection), CScullyCollection)
