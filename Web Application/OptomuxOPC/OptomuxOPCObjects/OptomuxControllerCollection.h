// OptomuxControllerCollection.h : Declaration of the COptomuxControllerCollection

#pragma once
#include "resource.h"       // main symbols
#include "ComCollection.h"	// Template Collection class


// COptomuxControllerCollection

class ATL_NO_VTABLE COptomuxControllerCollection : 
	public CComCollection<IOptomuxController, COptomuxControllerCollection, IOptomuxControllerCollection, &CLSID_OptomuxControllerCollection, &IID_IOptomuxControllerCollection, &LIBID_OptomuxOPCObjectsLib>
{
public:
	COptomuxControllerCollection()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_OPTOMUXCONTROLLERCOLLECTION)


BEGIN_COM_MAP(COptomuxControllerCollection)
	COM_INTERFACE_ENTRY(IOptomuxControllerCollection)
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

OBJECT_ENTRY_AUTO(__uuidof(OptomuxControllerCollection), COptomuxControllerCollection)
