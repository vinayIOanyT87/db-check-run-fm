// Contrecs.cpp : Implementation of CContrecs
/******************************************************************************

	FILE NAME:		Contrecs.cpp


	PURPOSE:			Implementation of Contrecs


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	B. Schaal


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/


#include "stdafx.h"
#include "Contrecs.h"
#include "DeviceManager.h"

extern CDeviceManager*		g_pDeviceManager;

inline void TESTHR( HRESULT _hr ) { if FAILED(_hr) throw(_hr); }

// CContrecs

HRESULT CContrecs::Activate()
{
	HRESULT hr = GetObjectContext(&m_oObjectContext);
	if( FAILED( hr ))
		return hr;

	hr=m_oDataAccess.CreateInstance(CLSID_DataAccess);
	if( FAILED( hr ))
		return hr;

	return hr;
} 

BOOL CContrecs::CanBePooled()
{
	return FALSE;
} 

void CContrecs::Deactivate()
{
	m_oObjectContext.Release();
	m_oDataAccess.Release();
} 


STDMETHODIMP CContrecs::raw_Enumerate(IDispatch** ppContrecCollection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	try
	{
		if(!ppContrecCollection)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;
		}

		IContrecPtr	oContrec(CLSID_Contrec);
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oContrec->EnumerateSQL);
		IContrecCollectionPtr	oContrecCollection(CLSID_ContrecCollection);
		while(!oRecordset->EndOfFile)
		{
			IContrecPtr		oContrec(CLSID_Contrec);
			oContrec->Load(oRecordset);
			oRecordset->MoveNext();
			oContrecCollection->Add(oContrec);
		}
		*ppContrecCollection=oContrecCollection.Detach();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
		{
			return Error((LPOLESTR) e.Description(),IID_IContrecs);
		}
		else
		{
			return Error((LPOLESTR) e.ErrorMessage(),IID_IContrecs);
		}
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Enumerate Error"),IID_IContrecs);
	}
}

HRESULT CContrecs::Validate(IContrecPtr oContrec)
{
	if(!oContrec->ID.length())
	{
		CString strError;
		strError.LoadString(IDS_ERROR_ID_REQUIRED);
		return Error(strError,IID_IContrecs);
	}

	return S_OK;
}

STDMETHODIMP CContrecs::raw_Add(IDispatch* pContrec,LONG* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IContrecPtr			oContrec=pContrec;
		if(!plIndex
		|| oContrec == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		if(FAILED(Validate(oContrec)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oContrec->InsertSQL);

		*plIndex=GetIndex(oContrec->ID);
		oContrec->Index=*plIndex;

		g_pDeviceManager->AddContrec(oContrec);

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
		return Error(_T("Add Error"),IID_IContrecs);
	}
}

STDMETHODIMP CContrecs::raw_Modify(IDispatch* pContrec)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IContrecPtr				oContrec=pContrec;
		if(oContrec == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		// Verify ID does not exist
		LONG lIndex=GetIndex(oContrec->ID);
		if(lIndex
		&& lIndex != oContrec->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_CONTREC_EXISTS);
			return Error(strError,IID_IContrecs);
		}

		if(FAILED(Validate(oContrec)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oContrec->UpdateSQL);

		g_pDeviceManager->PurgeDevice(oContrec->ID);
		g_pDeviceManager->AddContrec(oContrec);

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
		return Error(_T("Modify Error"),IID_IContrecs);
	}
}

STDMETHODIMP CContrecs::raw_Purge(LONG lIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IContrecPtr	oContrec=Get(lIndex);
		if(!oContrec->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_CONTREC_NOT_FOUND);
			return Error(strError,IID_IContrecs);
		}

		m_oDataAccess->ExecuteQuery(oContrec->PurgeSQL);

		g_pDeviceManager->PurgeDevice(oContrec->ID);

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
		return Error(_T("Purge Error"),IID_IContrecs);
	}
}

STDMETHODIMP CContrecs::raw_Get(long lIndex, LPDISPATCH *ppContrec)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IContrecPtr	oContrec(CLSID_Contrec);
		oContrec->Index=lIndex;
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oContrec->SelectSQL);
		oContrec->Load(oRecordset);
		*ppContrec=oContrec.Detach();
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
		return Error(_T("Get Error"),IID_IContrecs);
	}
}

STDMETHODIMP CContrecs::raw_GetIndex(BSTR bstrID, long* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IContrecPtr	oContrec(CLSID_Contrec);
		oContrec->ID=bstrID;
		oContrec->Load(m_oDataAccess->GetRecordSet(oContrec->SelectByIDSQL));
		*plIndex=oContrec->Index;
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
		return Error(_T("GetIndex Error"),IID_IContrecs);
	}
}

