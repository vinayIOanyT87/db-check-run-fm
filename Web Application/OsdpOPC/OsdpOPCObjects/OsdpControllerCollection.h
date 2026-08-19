// OsdpControllerCollection.h : Declaration of the COsdpControllerCollection

#pragma once
#include "resource.h"       // main symbols
#include "ComCollection.h"	// Template Collection class


// COsdpControllerCollection

class ATL_NO_VTABLE COsdpControllerCollection : 
	public CComCollection<IOsdpController, COsdpControllerCollection, IOsdpControllerCollection, &CLSID_OsdpControllerCollection, &IID_IOsdpControllerCollection, &LIBID_OsdpOPCObjectsLib>
{
public:
	COsdpControllerCollection()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_OSDPCONTROLLERCOLLECTION)


BEGIN_COM_MAP(COsdpControllerCollection)
	COM_INTERFACE_ENTRY(IOsdpControllerCollection)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(IEnumVARIANT)
	COM_INTERFACE_ENTRY(IPersistStream)
	COM_INTERFACE_ENTRY2(IPersist, IPersistStream)
	COM_INTERFACE_ENTRY(IMarshal)
	COM_INTERFACE_ENTRY(IProvideClassInfo)
END_COM_MAP()


	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

public:

};

OBJECT_ENTRY_AUTO(__uuidof(OsdpControllerCollection), COsdpControllerCollection)
