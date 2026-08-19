// DataAccess.cpp : Implementation of CDataAccess

#include "stdafx.h"
#include "DataAccess.h"

// CDataAccess

HRESULT CDataAccess::Activate()
{
	HRESULT hr = GetObjectContext(&m_oObjectContext);
	if (FAILED(hr))
		return hr;

	return hr;
} 

BOOL CDataAccess::CanBePooled()
{
	return FALSE;
} 

void CDataAccess::Deactivate()
{
	m_oObjectContext.Release();
} 

STDMETHODIMP CDataAccess::raw_GetRecordSet(BSTR bstrSQL, IDispatch** ppRecordSet)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		_RecordsetPtr	oADORecordset;
		_ConnectionPtr	oADOConnection(CLSID_Connection);

		//OpenDatabaseConnection
		oADOConnection->CursorLocation = adUseClient;
		HRESULT hr=oADOConnection->Open((LPCTSTR) theApp.m_strConnectionString,L"",L"",adCmdUnspecified);
		if(FAILED(hr))
		{
			m_oObjectContext->SetAbort();
			return Error(_T("Database Open Error"),IID_IDataAccess);
		}
		if(!theApp.m_strProviderString.Compare(_T("SQLOLEDB")))
			oADOConnection->Execute(L"SET TRANSACTION ISOLATION LEVEL READ COMMITTED",NULL,adCmdText);
		oADORecordset=oADOConnection->Execute( bstrSQL, NULL, adCmdText);
		oADORecordset->putref_ActiveConnection(NULL);
		hr=oADORecordset.QueryInterface(IID_IDispatch,(void**) ppRecordSet);		
		if(FAILED(hr))
			m_oObjectContext->SetAbort();
		else
			m_oObjectContext->SetComplete();
		return hr;
	}
	//Return Back any COM+ Errors
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
			return Error((LPOLESTR) e.Description(),IID_IDataAccess);
		else
			return Error(e.ErrorMessage(),IID_IDataAccess);
	}
	//Return Back other errors
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("GetRecordSet Error"),IID_IDataAccess);
	}
}

STDMETHODIMP CDataAccess::raw_ExecuteQuery(BSTR bstrSQL)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		_ConnectionPtr	oADOConnection(CLSID_Connection);

		//OpenDatabaseConnection
		HRESULT hr=oADOConnection->Open((LPCTSTR) theApp.m_strConnectionString,L"",L"",adCmdUnspecified);
		if(FAILED(hr))
		{
			m_oObjectContext->SetAbort();
			return Error(_T("Database Open Error"),IID_IDataAccess);
		}

		oADOConnection->Execute( bstrSQL, NULL, adCmdText);
		m_oObjectContext->SetComplete();
		return S_OK;		
	}
	//Return back any COM+ Errors
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
			return Error((LPOLESTR) e.Description(),IID_IDataAccess);
		else
			return Error(e.ErrorMessage(),IID_IDataAccess);
	}
	//Return back all other errors
	catch (...)
	{
		return Error(_T("ExecuteQuery Error"),IID_IDataAccess);
	}
}
