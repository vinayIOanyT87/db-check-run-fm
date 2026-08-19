/******************************************************************************

	FILE NAME:		OsdpControllers.cpp


	PURPOSE:			Implementation of COsdpControllers


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
#include "OsdpControllers.h"
#include "OsdpControllerManager.h"

extern COsdpControllerManager		g_OsdpControllerManager;

inline void TESTHR( HRESULT _hr ) { if FAILED(_hr) throw(_hr); }

// COsdpControllers

HRESULT COsdpControllers::Activate()
{
	HRESULT hr = GetObjectContext(&m_oObjectContext);
	if( FAILED( hr ))
		return hr;

	hr=m_oDataAccess.CreateInstance(CLSID_DataAccess);
	if( FAILED( hr ))
		return hr;

	return hr;
} 

BOOL COsdpControllers::CanBePooled()
{
	return FALSE;
} 

void COsdpControllers::Deactivate()
{
	m_oObjectContext.Release();
	m_oDataAccess.Release();
} 

STDMETHODIMP COsdpControllers::raw_Enumerate(IDispatch** ppOsdpControllerCollection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	try
	{
		if(!ppOsdpControllerCollection)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;
		}

		IOsdpControllerPtr	oOsdpController(CLSID_OsdpController);
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oOsdpController->EnumerateSQL);
		IOsdpControllerCollectionPtr	oOsdpControllerCollection(CLSID_OsdpControllerCollection);
		while(!oRecordset->EndOfFile)
		{
			IOsdpControllerPtr		oOsdpController(CLSID_OsdpController);
			oOsdpController->Load(oRecordset);
			oRecordset->MoveNext();
			oOsdpControllerCollection->Add(oOsdpController);
		}
		*ppOsdpControllerCollection=oOsdpControllerCollection.Detach();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
		{
			return Error((LPOLESTR) e.Description(),IID_IOsdpControllers);
		}
		else
		{
			return Error((LPOLESTR) e.ErrorMessage(),IID_IOsdpControllers);
		}
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Enumerate Error"),IID_IOsdpControllers);
	}
}

HRESULT COsdpControllers::CheckConstraints(IOsdpControllerPtr oOsdpController)
{
	if(!oOsdpController->ID.length())
	{
		CString strError;
		strError.LoadString(IDS_ERROR_ID_REQUIRED);
		return Error(strError,IID_IOsdpControllers);
	}

	return S_OK;
}

STDMETHODIMP COsdpControllers::raw_Add(IDispatch* pOsdpController,LONG* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IOsdpControllerPtr			oOsdpController=pOsdpController;
		if(!plIndex
		|| oOsdpController == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		if(FAILED(CheckConstraints(oOsdpController)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oOsdpController->InsertSQL);

		*plIndex=GetIndex(oOsdpController->ID);
		oOsdpController->Index=*plIndex;

		g_OsdpControllerManager.AddOsdpController(oOsdpController);

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
		return Error(_T("Add Error"),IID_IOsdpControllers);
	}
}

STDMETHODIMP COsdpControllers::raw_Modify(IDispatch* pOsdpController)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IOsdpControllerPtr				oOsdpController=pOsdpController;
		if(oOsdpController == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		// Verify ID does not exist
		LONG lIndex=GetIndex(oOsdpController->ID);
		if(lIndex
		&& lIndex != oOsdpController->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_OSDP_CONTROLLER_EXISTS);
			return Error(strError,IID_IOsdpControllers);
		}

		if(FAILED(CheckConstraints(oOsdpController)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oOsdpController->UpdateSQL);

		g_OsdpControllerManager.PurgeOsdpController(oOsdpController);
		g_OsdpControllerManager.AddOsdpController(oOsdpController);

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
		return Error(_T("Modify Error"),IID_IOsdpControllers);
	}
}

STDMETHODIMP COsdpControllers::raw_Purge(LONG lIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IOsdpControllerPtr	oOsdpController=Get(lIndex);
		if(!oOsdpController->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_OSDP_CONTROLLER_NOT_FOUND);
			return Error(strError,IID_IOsdpControllers);
		}

		m_oDataAccess->ExecuteQuery(oOsdpController->PurgeSQL);

		g_OsdpControllerManager.PurgeOsdpController(oOsdpController);

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
		return Error(_T("Purge Error"),IID_IOsdpControllers);
	}
}

STDMETHODIMP COsdpControllers::raw_Get(long lIndex, LPDISPATCH *ppOsdpController)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IOsdpControllerPtr	oOsdpController(CLSID_OsdpController);
		oOsdpController->Index=lIndex;
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oOsdpController->SelectSQL);
		oOsdpController->Load(oRecordset);
		*ppOsdpController=oOsdpController.Detach();
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
		return Error(_T("Get Error"),IID_IOsdpControllers);
	}
}

STDMETHODIMP COsdpControllers::raw_GetIndex(BSTR bstrID, long* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IOsdpControllerPtr	oOsdpController(CLSID_OsdpController);
		oOsdpController->ID=bstrID;
		oOsdpController->Load(m_oDataAccess->GetRecordSet(oOsdpController->SelectByIDSQL));
		*plIndex=oOsdpController->Index;
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
		return Error(_T("GetIndex Error"),IID_IOsdpControllers);
	}
}

