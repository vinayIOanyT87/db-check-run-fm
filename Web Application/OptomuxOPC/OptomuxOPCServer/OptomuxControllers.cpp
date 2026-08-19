/******************************************************************************

	FILE NAME:		OptomuxControllers.cpp


	PURPOSE:			Implementation of COptomuxControllers


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
#include "OptomuxControllers.h"
#include "OptomuxControllerManager.h"

extern COptomuxControllerManager		g_OptomuxControllerManager;

inline void TESTHR( HRESULT _hr ) { if FAILED(_hr) throw(_hr); }

// COptomuxControllers

HRESULT COptomuxControllers::Activate()
{
	HRESULT hr = GetObjectContext(&m_oObjectContext);
	if( FAILED( hr ))
		return hr;

	hr=m_oDataAccess.CreateInstance(CLSID_DataAccess);
	if( FAILED( hr ))
		return hr;

	return hr;
} 

BOOL COptomuxControllers::CanBePooled()
{
	return FALSE;
} 

void COptomuxControllers::Deactivate()
{
	m_oObjectContext.Release();
	m_oDataAccess.Release();
} 

STDMETHODIMP COptomuxControllers::raw_Enumerate(IDispatch** ppOptomuxControllerCollection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	try
	{
		if(!ppOptomuxControllerCollection)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;
		}

		IOptomuxControllerPtr	oOptomuxController(CLSID_OptomuxController);
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oOptomuxController->EnumerateSQL);
		IOptomuxControllerCollectionPtr	oOptomuxControllerCollection(CLSID_OptomuxControllerCollection);
		while(!oRecordset->EndOfFile)
		{
			IOptomuxControllerPtr		oOptomuxController(CLSID_OptomuxController);
			oOptomuxController->Load(oRecordset);
			oRecordset->MoveNext();
			oOptomuxControllerCollection->Add(oOptomuxController);
		}
		*ppOptomuxControllerCollection=oOptomuxControllerCollection.Detach();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
		{
			return Error((LPOLESTR) e.Description(),IID_IOptomuxControllers);
		}
		else
		{
			return Error((LPOLESTR) e.ErrorMessage(),IID_IOptomuxControllers);
		}
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Enumerate Error"),IID_IOptomuxControllers);
	}
}

HRESULT COptomuxControllers::CheckConstraints(IOptomuxControllerPtr oOptomuxController)
{
	if(!oOptomuxController->ID.length())
	{
		CString strError;
		strError.LoadString(IDS_ERROR_ID_REQUIRED);
		return Error(strError,IID_IOptomuxControllers);
	}

	return S_OK;
}

STDMETHODIMP COptomuxControllers::raw_Add(IDispatch* pOptomuxController,LONG* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IOptomuxControllerPtr			oOptomuxController=pOptomuxController;
		if(!plIndex
		|| oOptomuxController == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		if(FAILED(CheckConstraints(oOptomuxController)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oOptomuxController->InsertSQL);

		*plIndex=GetIndex(oOptomuxController->ID);
		oOptomuxController->Index=*plIndex;

		g_OptomuxControllerManager.AddOptomuxController(oOptomuxController);

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
		return Error(_T("Add Error"),IID_IOptomuxControllers);
	}
}

STDMETHODIMP COptomuxControllers::raw_Modify(IDispatch* pOptomuxController)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IOptomuxControllerPtr				oOptomuxController=pOptomuxController;
		if(oOptomuxController == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		// Verify ID does not exist
		LONG lIndex=GetIndex(oOptomuxController->ID);
		if(lIndex
		&& lIndex != oOptomuxController->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_OPTOMUX_CONTROLLER_EXISTS);
			return Error(strError,IID_IOptomuxControllers);
		}

		if(FAILED(CheckConstraints(oOptomuxController)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oOptomuxController->UpdateSQL);

		g_OptomuxControllerManager.PurgeOptomuxController(oOptomuxController);
		g_OptomuxControllerManager.AddOptomuxController(oOptomuxController);

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
		return Error(_T("Modify Error"),IID_IOptomuxControllers);
	}
}

STDMETHODIMP COptomuxControllers::raw_Purge(LONG lIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IOptomuxControllerPtr	oOptomuxController=Get(lIndex);
		if(!oOptomuxController->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_OPTOMUX_CONTROLLER_NOT_FOUND);
			return Error(strError,IID_IOptomuxControllers);
		}

		m_oDataAccess->ExecuteQuery(oOptomuxController->PurgeSQL);

		g_OptomuxControllerManager.PurgeOptomuxController(oOptomuxController);

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
		return Error(_T("Purge Error"),IID_IOptomuxControllers);
	}
}

STDMETHODIMP COptomuxControllers::raw_Get(long lIndex, LPDISPATCH *ppOptomuxController)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IOptomuxControllerPtr	oOptomuxController(CLSID_OptomuxController);
		oOptomuxController->Index=lIndex;
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oOptomuxController->SelectSQL);
		oOptomuxController->Load(oRecordset);
		*ppOptomuxController=oOptomuxController.Detach();
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
		return Error(_T("Get Error"),IID_IOptomuxControllers);
	}
}

STDMETHODIMP COptomuxControllers::raw_GetIndex(BSTR bstrID, long* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IOptomuxControllerPtr	oOptomuxController(CLSID_OptomuxController);
		oOptomuxController->ID=bstrID;
		oOptomuxController->Load(m_oDataAccess->GetRecordSet(oOptomuxController->SelectByIDSQL));
		*plIndex=oOptomuxController->Index;
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
		return Error(_T("GetIndex Error"),IID_IOptomuxControllers);
	}
}

