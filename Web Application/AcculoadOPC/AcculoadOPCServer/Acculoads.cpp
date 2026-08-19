/******************************************************************************

	FILE NAME:		Acculoads.cpp


	PURPOSE:			Implementation of CAcculoads


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/

#include "stdafx.h"
#include "Acculoads.h"
#include "DeviceManager.h"

extern CDeviceManager*		g_pDeviceManager;

inline void TESTHR( HRESULT _hr ) { if FAILED(_hr) throw(_hr); }

// CAcculoads

HRESULT CAcculoads::Activate()
{
	HRESULT hr = GetObjectContext(&m_oObjectContext);
	if( FAILED( hr ))
		return hr;

	hr=m_oDataAccess.CreateInstance(CLSID_DataAccess);
	if( FAILED( hr ))
		return hr;

	return hr;
} 

BOOL CAcculoads::CanBePooled()
{
	return FALSE;
} 

void CAcculoads::Deactivate()
{
	m_oObjectContext.Release();
	m_oDataAccess.Release();
} 

STDMETHODIMP CAcculoads::raw_Enumerate(IDispatch** ppAcculoadCollection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	try
	{
		if(!ppAcculoadCollection)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;
		}

		IArmsPtr	oArms(CLSID_Arms);
		IAcculoadPtr	oAccuload(CLSID_Accuload);
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oAccuload->EnumerateSQL);
		IAcculoadCollectionPtr	oAcculoadCollection(CLSID_AcculoadCollection);
		while(!oRecordset->EndOfFile)
		{
			IAcculoadPtr		oAccuload(CLSID_Accuload);
			oAccuload->Load(oRecordset);
			oAccuload->Arms=oArms->EnumerateByAcculoadIndex(oAccuload->Index);
			oRecordset->MoveNext();
			oAcculoadCollection->Add(oAccuload);
		}
		*ppAcculoadCollection=oAcculoadCollection.Detach();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
		{
			return Error((LPOLESTR) e.Description(),IID_IAcculoads);
		}
		else
		{
			return Error((LPOLESTR) e.ErrorMessage(),IID_IAcculoads);
		}
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Enumerate Error"),IID_IAcculoads);
	}
}

HRESULT CAcculoads::Validate(IAcculoadPtr oAccuload)
{
	if(!oAccuload->ID.length())
	{
		CString strError;
		strError.LoadString(IDS_ERROR_ID_REQUIRED);
		return Error(strError,IID_IAcculoads);
	}

	IArmCollectionPtr	oArms=oAccuload->Arms;
	if(!oArms->Count)
	{
		CString strError;
		strError.LoadString(IDS_ERROR_AT_LEAST_ONE_ARM_REQUIRED);
		return Error(strError,IID_IAcculoads);
	}

	for(LONG lItem1=0;lItem1 < oArms->Count;lItem1++)
	{
		IArmPtr	oArm=oArms->Item(lItem1);

		if(oAccuload->Type == SMITH_PROXIMITY
		|| oAccuload->Type == RCU_II_OPEN
		|| oAccuload->Type == RCU_II_RCU)
			continue;

		if(oArm->Products == 0
		|| oArm->Products > 6)
		{
			CString strError;
			strError.LoadString(IDS_ERROR_PRODUCTS);
			return Error(strError,IID_IAcculoads);
		}

		BYTE	bAddress=oArm->Address;
		for(LONG lItem2=0;lItem2 < oArms->Count;lItem2++)
		{
			if(lItem2 == lItem1)
				continue;

			oArm=oArms->Item(lItem2);
			if(oAccuload->Type != MULTILOAD_II &&	// the arm address must be the preset address and be the same for all arms
				oArm->Address == bAddress)
			{
				CString strError;
				strError.LoadString(IDS_ERROR_ADDRESS_CONFLICT);
				return Error(strError,IID_IAcculoads);
			}
		}
	}
	return S_OK;
}

STDMETHODIMP CAcculoads::raw_Add(IDispatch* pAccuload,LONG* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IAcculoadPtr			oAccuload=pAccuload;
		if(!plIndex
		|| oAccuload == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		if(FAILED(Validate(oAccuload)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oAccuload->InsertSQL);

		*plIndex=GetIndex(oAccuload->ID);
		oAccuload->Index=*plIndex;

		IArmsPtr	oArms(CLSID_Arms);
		oArms->ModifyCollection(oAccuload->Index,oAccuload->Arms);

		g_pDeviceManager->AddAccuload(oAccuload);

		m_oObjectContext->SetComplete();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
			return Error((LPOLESTR) e.Description(),e.GUID(),e.Error());
		else
			return Error((LPOLESTR) e.ErrorMessage(),e.GUID(),e.Error());
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Add Error"),IID_IAcculoads);
	}
}

STDMETHODIMP CAcculoads::raw_Modify(IDispatch* pAccuload)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IAcculoadPtr				oAccuload=pAccuload;
		if(oAccuload == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		// Verify ID does not exist
		LONG lIndex=GetIndex(oAccuload->ID);
		if(lIndex
		&& lIndex != oAccuload->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_ACCULOAD_EXISTS);
			return Error(strError,IID_IAcculoads);
		}

		if(FAILED(Validate(oAccuload)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oAccuload->UpdateSQL);

		IArmsPtr	oArms(CLSID_Arms);
		oArms->ModifyCollection(oAccuload->Index,oAccuload->Arms);

		g_pDeviceManager->PurgeDevice(oAccuload->ID);
		g_pDeviceManager->AddAccuload(oAccuload);

		m_oObjectContext->SetComplete();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
			return Error((LPOLESTR) e.Description(),e.GUID(),e.Error());
		else
			return Error((LPOLESTR) e.ErrorMessage(),e.GUID(),e.Error());
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Modify Error"),IID_IAcculoads);
	}
}

STDMETHODIMP CAcculoads::raw_Purge(LONG lIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IAcculoadPtr	oAccuload=Get(lIndex);
		if(!oAccuload->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_ACCULOAD_NOT_FOUND);
			return Error(strError,IID_IAcculoads);
		}

		IArmsPtr	oArms(CLSID_Arms);
		IArmCollectionPtr	oArmCollection=oAccuload->Arms;
		for(LONG lItem=0;lItem < oArmCollection->Count;lItem++)
		{
			IArmPtr	oArm=oArmCollection->Item(lItem);
			oArms->Purge(oArm->Index);
		}

		m_oDataAccess->ExecuteQuery(oAccuload->PurgeSQL);

		g_pDeviceManager->PurgeDevice(oAccuload->ID);

		m_oObjectContext->SetComplete();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
			return Error((LPOLESTR) e.Description(),e.GUID(),e.Error());
		else
			return Error((LPOLESTR) e.ErrorMessage(),e.GUID(),e.Error());
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Purge Error"),IID_IAcculoads);
	}
}

STDMETHODIMP CAcculoads::raw_Get(long lIndex, LPDISPATCH *ppAccuload)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IAcculoadPtr	oAccuload(CLSID_Accuload);
		oAccuload->Index=lIndex;
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oAccuload->SelectSQL);
		oAccuload->Load(oRecordset);
		IArmsPtr	oArms(CLSID_Arms);
		oAccuload->Arms=oArms->EnumerateByAcculoadIndex(oAccuload->Index);
		*ppAccuload=oAccuload.Detach();
		m_oObjectContext->SetComplete();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
			return Error((LPOLESTR) e.Description(),e.GUID(),e.Error());
		else
			return Error((LPOLESTR) e.ErrorMessage(),e.GUID(),e.Error());
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Get Error"),IID_IAcculoads);
	}
}

STDMETHODIMP CAcculoads::raw_GetIndex(BSTR bstrID, long* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IAcculoadPtr	oAccuload(CLSID_Accuload);
		oAccuload->ID=bstrID;
		oAccuload->Load(m_oDataAccess->GetRecordSet(oAccuload->SelectByIDSQL));
		*plIndex=oAccuload->Index;
		m_oObjectContext->SetComplete();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
			return Error((LPOLESTR) e.Description(),e.GUID(),e.Error());
		else
			return Error((LPOLESTR) e.ErrorMessage(),e.GUID(),e.Error());
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("GetIndex Error"),IID_IAcculoads);
	}
}

