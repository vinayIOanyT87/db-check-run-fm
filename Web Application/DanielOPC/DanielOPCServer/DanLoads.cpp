/******************************************************************************

	FILE NAME:		DanLoads.cpp


	PURPOSE:			Implementation of CDanLoads


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/

#include "stdafx.h"
#include "DanLoads.h"
#include "DeviceManager.h"

extern CDeviceManager*		g_pDeviceManager;

inline void TESTHR( HRESULT _hr ) { if FAILED(_hr) throw(_hr); }

// CDanLoads

HRESULT CDanLoads::Activate()
{
	HRESULT hr = GetObjectContext(&m_oObjectContext);
	if( FAILED( hr ))
		return hr;

	hr=m_oDataAccess.CreateInstance(CLSID_DataAccess);
	if( FAILED( hr ))
		return hr;

	return hr;
} 

BOOL CDanLoads::CanBePooled()
{
	return FALSE;
} 

void CDanLoads::Deactivate()
{
	m_oObjectContext.Release();
	m_oDataAccess.Release();
} 

STDMETHODIMP CDanLoads::raw_Enumerate(IDispatch** ppDanLoadCollection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	try
	{
		if(!ppDanLoadCollection)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;
		}

		IDanLoadPtr	oDanLoad(CLSID_DanLoad);
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oDanLoad->EnumerateSQL);
		IDanLoadCollectionPtr	oDanLoadCollection(CLSID_DanLoadCollection);
		while(!oRecordset->EndOfFile)
		{
			IDanLoadPtr		oDanLoad(CLSID_DanLoad);
			oDanLoad->Load(oRecordset);
			oRecordset->MoveNext();
			oDanLoadCollection->Add(oDanLoad);
		}
		*ppDanLoadCollection=oDanLoadCollection.Detach();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
		{
			return Error((LPOLESTR) e.Description(),IID_IDanLoads);
		}
		else
		{
			return Error((LPOLESTR) e.ErrorMessage(),IID_IDanLoads);
		}
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Enumerate Error"),IID_IDanLoads);
	}
}

HRESULT CDanLoads::Validate(IDanLoadPtr oDanLoad)
{
	if(!oDanLoad->ID.length())
	{
		CString strError;
		strError.LoadString(IDS_ERROR_ID_REQUIRED);
		return Error(strError,IID_IDanLoads);
	}

	return S_OK;
}

STDMETHODIMP CDanLoads::raw_Add(IDispatch* pDanLoad,LONG* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IDanLoadPtr			oDanLoad=pDanLoad;
		if(!plIndex
		|| oDanLoad == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		if(FAILED(Validate(oDanLoad)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oDanLoad->InsertSQL);

		*plIndex=GetIndex(oDanLoad->ID);
		oDanLoad->Index=*plIndex;

		g_pDeviceManager->AddDanLoad(oDanLoad);

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
		return Error(_T("Add Error"),IID_IDanLoads);
	}
}

STDMETHODIMP CDanLoads::raw_Modify(IDispatch* pDanLoad)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IDanLoadPtr				oDanLoad=pDanLoad;
		if(oDanLoad == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		// Verify ID does not exist
		LONG lIndex=GetIndex(oDanLoad->ID);
		if(lIndex
		&& lIndex != oDanLoad->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_DANLOAD_EXISTS);
			return Error(strError,IID_IDanLoads);
		}

		if(FAILED(Validate(oDanLoad)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oDanLoad->UpdateSQL);

		g_pDeviceManager->PurgeDevice(oDanLoad->ID);
		g_pDeviceManager->AddDanLoad(oDanLoad);

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
		return Error(_T("Modify Error"),IID_IDanLoads);
	}
}

STDMETHODIMP CDanLoads::raw_Purge(LONG lIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IDanLoadPtr	oDanLoad=Get(lIndex);
		if(!oDanLoad->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_DANLOAD_NOT_FOUND);
			return Error(strError,IID_IDanLoads);
		}

		m_oDataAccess->ExecuteQuery(oDanLoad->PurgeSQL);

		g_pDeviceManager->PurgeDevice(oDanLoad->ID);

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
		return Error(_T("Purge Error"),IID_IDanLoads);
	}
}

STDMETHODIMP CDanLoads::raw_Get(long lIndex, LPDISPATCH *ppDanLoad)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IDanLoadPtr	oDanLoad(CLSID_DanLoad);
		oDanLoad->Index=lIndex;
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oDanLoad->SelectSQL);
		oDanLoad->Load(oRecordset);
		*ppDanLoad=oDanLoad.Detach();
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
		return Error(_T("Get Error"),IID_IDanLoads);
	}
}

STDMETHODIMP CDanLoads::raw_GetIndex(BSTR bstrID, long* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IDanLoadPtr	oDanLoad(CLSID_DanLoad);
		oDanLoad->ID=bstrID;
		oDanLoad->Load(m_oDataAccess->GetRecordSet(oDanLoad->SelectByIDSQL));
		*plIndex=oDanLoad->Index;
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
		return Error(_T("GetIndex Error"),IID_IDanLoads);
	}
}

