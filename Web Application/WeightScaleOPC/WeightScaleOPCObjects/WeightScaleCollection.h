// WeightScaleCollection.h : Declaration of the CWeightScaleCollection

#pragma once
#include "resource.h"       // main symbols
#include "ComCollection.h"	// Template Collection class


// CWeightScaleCollection

class ATL_NO_VTABLE CWeightScaleCollection : 
	public CComCollection<IWeightScale, CWeightScaleCollection, IWeightScaleCollection, &CLSID_WeightScaleCollection, &IID_IWeightScaleCollection, &LIBID_WeightScaleOPCObjectsLib>
{
public:
	CWeightScaleCollection()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_WEIGHTSCALECOLLECTION)


BEGIN_COM_MAP(CWeightScaleCollection)
	COM_INTERFACE_ENTRY(IWeightScaleCollection)
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

OBJECT_ENTRY_AUTO(__uuidof(WeightScaleCollection), CWeightScaleCollection)
