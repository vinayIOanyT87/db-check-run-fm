/******************************************************************************

	FILE NAME:		Scullys.cpp


	PURPOSE:			Implementation of CScullys


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	S. Jiang


	VERSION:		9.0.0.0  Current version



	MODIFICATION HISTORY:		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/

#include "stdafx.h"
#include "Scullys.h"

inline void TESTHR( HRESULT _hr ) { if FAILED(_hr) throw(_hr); }

// CScullys

HRESULT CScullys::Activate()
{
	HRESULT hr = GetObjectContext(&m_oObjectContext);
	if( FAILED( hr ))
		return hr;

	hr=m_oDataAccess.CreateInstance(CLSID_DataAccess);
	if( FAILED( hr ))
		return hr;

	return hr;
} 

BOOL CScullys::CanBePooled()
{
	return FALSE;
} 

void CScullys::Deactivate()
{
	m_oObjectContext.Release();
	m_oDataAccess.Release();
} 

STDMETHODIMP CScullys::raw_Enumerate(IDispatch** ppScullyCollection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	try
	{
		if(!ppScullyCollection)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;
		}

		IScullyPtr	oScully(CLSID_Scully);
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oScully->EnumerateSQL);
		IScullyCollectionPtr	oScullyCollection(CLSID_ScullyCollection);
		while(!oRecordset->EndOfFile)
		{
			IScullyPtr		oScully(CLSID_Scully);
			oScully->Load(oRecordset);
			oRecordset->MoveNext();
			oScullyCollection->Add(oScully);
		}
		*ppScullyCollection=oScullyCollection.Detach();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
		{
			return Error((LPOLESTR) e.Description(),IID_IScullys);
		}
		else
		{
			return Error((LPOLESTR) e.ErrorMessage(),IID_IScullys);
		}
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Enumerate Error"),IID_IScullys);
	}
}

HRESULT CScullys::Validate(IScullyPtr oScully)
{
	if(!oScully->ID.length())
	{
		CString strError;
		strError.LoadString(IDS_ERROR_ID_REQUIRED);
		return Error(strError,IID_IScullys);
	}
	return S_OK;
}

STDMETHODIMP CScullys::raw_Add(IDispatch* pScully,LONG* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IScullyPtr			oScully=pScully;
		if(!plIndex
		|| oScully == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		if(FAILED(Validate(oScully)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oScully->InsertSQL);

		*plIndex=GetIndex(oScully->ID);
		oScully->Index=*plIndex;

//		g_pDeviceManager->AddScully(oScully);

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
		return Error(_T("Add Error"),IID_IScullys);
	}
}

STDMETHODIMP CScullys::raw_Modify(IDispatch* pScully)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IScullyPtr				oScully=pScully;
		if(oScully == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		// Verify ID does not exist
		LONG lIndex=GetIndex(oScully->ID);
		if(lIndex
		&& lIndex != oScully->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_SCULLY_EXISTS);
			return Error(strError,IID_IScullys);
		}

		if(FAILED(Validate(oScully)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oScully->UpdateSQL);

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
		return Error(_T("Modify Error"),IID_IScullys);
	}
}

STDMETHODIMP CScullys::raw_Purge(LONG lIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IScullyPtr	oScully=Get(lIndex);
		if(!oScully->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_SCULLY_NOT_FOUND);
			return Error(strError,IID_IScullys);
		}

		m_oDataAccess->ExecuteQuery(oScully->PurgeSQL);

//		g_pDeviceManager->PurgeDevice(oScully->ID);

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
		return Error(_T("Purge Error"),IID_IScullys);
	}
}

STDMETHODIMP CScullys::raw_Get(long lIndex, LPDISPATCH *ppScully)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IScullyPtr	oScully(CLSID_Scully);
		oScully->Index=lIndex;
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oScully->SelectSQL);
		oScully->Load(oRecordset);
		*ppScully=oScully.Detach();
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
		return Error(_T("Get Error"),IID_IScullys);
	}
}

STDMETHODIMP CScullys::raw_GetIndex(BSTR bstrID, long* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IScullyPtr	oScully(CLSID_Scully);
		oScully->ID=bstrID;
		oScully->Load(m_oDataAccess->GetRecordSet(oScully->SelectByIDSQL));
		*plIndex=oScully->Index;
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
		return Error(_T("GetIndex Error"),IID_IScullys);
	}
}

STDMETHODIMP CScullys::raw_EnumeratePortIDs(VARIANT* pIDs)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	IScullyCollectionPtr	oScullyCollection=Enumerate();

	CRegKey RegKey;

	if(ERROR_SUCCESS != RegKey.Open(HKEY_LOCAL_MACHINE,_T("HARDWARE\\DEVICEMAP\\SERIALCOMM")))
		return Error(_T("RegKey.Open Error : HKEY_LOCAL_MACHINE\\HARDWARE\\DEVICEMAP\\SERIALCOMM"));

	DWORD 		dwValueType 			= REG_SZ;					// buffer for type code
	TCHAR			szValueName[ MAX_PATH ];							// value name buffer
	DWORD 		dwValueNameSiz;										// size of value name buffer
	TCHAR			szRegPortName[ MAX_PATH ];							// value data buffer
	DWORD 		dwValueDataSiz;										// size of value data buffer

	COleSafeArray	saPorts;
	LONG		lIndex[]={0};
	dwValueNameSiz = MAX_PATH;
	dwValueDataSiz = MAX_PATH;
	DWORD dwIndex=0;
	while(ERROR_SUCCESS == RegEnumValue(	RegKey.m_hKey,
										 				dwIndex,
										 				szValueName,
														&dwValueNameSiz,
														NULL,						// reserved param
														&dwValueType,
														(LPBYTE) szRegPortName,
														&dwValueDataSiz ))
	{
		// Exclude Ports already configured
		for(LONG lItem=0;lItem < oScullyCollection->Count;lItem++)
		{
			IScullyPtr oScully=oScullyCollection->Item(lItem);
			if(oScully->ID == _bstr_t(szRegPortName))
				break;
		}

		if(lItem == oScullyCollection->Count)
		{
			if(saPorts.vt == VT_EMPTY)
			{
				DWORD rgElements[]={1};	
				saPorts.Create(VT_BSTR,1,rgElements);
			}
			else
			{
				lIndex[0]++;
				SAFEARRAYBOUND	SafeArrayBound={lIndex[0]+1,0};
				saPorts.Redim(&SafeArrayBound);
			}
			saPorts.PutElement(lIndex,SysAllocString(szRegPortName));
		}

		dwValueNameSiz = MAX_PATH;
		dwValueDataSiz = MAX_PATH;
		dwIndex++;
	}

	RegKey.Close();

	if(saPorts.vt != VT_EMPTY)
	{
		*pIDs=saPorts.Detach();
		return S_OK;
	}
	else
		return S_FALSE;
}

