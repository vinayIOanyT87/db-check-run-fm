/******************************************************************************

	FILE NAME:		WeightScales.cpp


	PURPOSE:			Implementation of CWeightScales


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
#include "WeightScales.h"

inline void TESTHR( HRESULT _hr ) { if FAILED(_hr) throw(_hr); }

// CWeightScales

HRESULT CWeightScales::Activate()
{
	HRESULT hr = GetObjectContext(&m_oObjectContext);
	if( FAILED( hr ))
		return hr;

	hr=m_oDataAccess.CreateInstance(CLSID_DataAccess);
	if( FAILED( hr ))
		return hr;

	return hr;
} 

BOOL CWeightScales::CanBePooled()
{
	return FALSE;
} 

void CWeightScales::Deactivate()
{
	m_oObjectContext.Release();
	m_oDataAccess.Release();
} 

STDMETHODIMP CWeightScales::raw_Enumerate(IDispatch** ppWeightScaleCollection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	try
	{
		if(!ppWeightScaleCollection)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;
		}

		IWeightScalePtr	oWeightScale(CLSID_WeightScale);
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oWeightScale->EnumerateSQL);
		IWeightScaleCollectionPtr	oWeightScaleCollection(CLSID_WeightScaleCollection);
		while(!oRecordset->EndOfFile)
		{
			IWeightScalePtr		oWeightScale(CLSID_WeightScale);
			oWeightScale->Load(oRecordset);
			oRecordset->MoveNext();
			oWeightScaleCollection->Add(oWeightScale);
		}
		*ppWeightScaleCollection=oWeightScaleCollection.Detach();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
		{
			return Error((LPOLESTR) e.Description(),IID_IWeightScales);
		}
		else
		{
			return Error((LPOLESTR) e.ErrorMessage(),IID_IWeightScales);
		}
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Enumerate Error"),IID_IWeightScales);
	}
}

HRESULT CWeightScales::Validate(IWeightScalePtr oWeightScale)
{
	if(!oWeightScale->ID.length())
	{
		CString strError;
		strError.LoadString(IDS_ERROR_ID_REQUIRED);
		return Error(strError,IID_IWeightScales);
	}
	return S_OK;
}

STDMETHODIMP CWeightScales::raw_Add(IDispatch* pWeightScale,LONG* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IWeightScalePtr			oWeightScale=pWeightScale;
		if(!plIndex
		|| oWeightScale == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		if(FAILED(Validate(oWeightScale)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oWeightScale->InsertSQL);

		*plIndex=GetIndex(oWeightScale->ID);
		oWeightScale->Index=*plIndex;

//		g_pDeviceManager->AddWeightScale(oWeightScale);

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
		return Error(_T("Add Error"),IID_IWeightScales);
	}
}

STDMETHODIMP CWeightScales::raw_Modify(IDispatch* pWeightScale)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IWeightScalePtr				oWeightScale=pWeightScale;
		if(oWeightScale == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		// Verify ID does not exist
		LONG lIndex=GetIndex(oWeightScale->ID);
		if(lIndex
		&& lIndex != oWeightScale->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_WEIGHTSCALE_EXISTS);
			return Error(strError,IID_IWeightScales);
		}

		if(FAILED(Validate(oWeightScale)))
		{
			m_oObjectContext->SetAbort();
			return E_FAIL;
		}

		m_oDataAccess->ExecuteQuery(oWeightScale->UpdateSQL);

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
		return Error(_T("Modify Error"),IID_IWeightScales);
	}
}

STDMETHODIMP CWeightScales::raw_Purge(LONG lIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IWeightScalePtr	oWeightScale=Get(lIndex);
		if(!oWeightScale->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_WEIGHTSCALE_NOT_FOUND);
			return Error(strError,IID_IWeightScales);
		}

		m_oDataAccess->ExecuteQuery(oWeightScale->PurgeSQL);

//		g_pDeviceManager->PurgeDevice(oWeightScale->ID);

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
		return Error(_T("Purge Error"),IID_IWeightScales);
	}
}

STDMETHODIMP CWeightScales::raw_Get(long lIndex, LPDISPATCH *ppWeightScale)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IWeightScalePtr	oWeightScale(CLSID_WeightScale);
		oWeightScale->Index=lIndex;
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oWeightScale->SelectSQL);
		oWeightScale->Load(oRecordset);
		*ppWeightScale=oWeightScale.Detach();
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
		return Error(_T("Get Error"),IID_IWeightScales);
	}
}

STDMETHODIMP CWeightScales::raw_GetIndex(BSTR bstrID, long* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IWeightScalePtr	oWeightScale(CLSID_WeightScale);
		oWeightScale->ID=bstrID;
		oWeightScale->Load(m_oDataAccess->GetRecordSet(oWeightScale->SelectByIDSQL));
		*plIndex=oWeightScale->Index;
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
		return Error(_T("GetIndex Error"),IID_IWeightScales);
	}
}

STDMETHODIMP CWeightScales::raw_EnumeratePortIDs(VARIANT* pIDs)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	IWeightScaleCollectionPtr	oWeightScaleCollection=Enumerate();

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
		for(LONG lItem=0;lItem < oWeightScaleCollection->Count;lItem++)
		{
			IWeightScalePtr oWeightScale=oWeightScaleCollection->Item(lItem);
			if(oWeightScale->ID == _bstr_t(szRegPortName))
				break;
		}

		if(lItem == oWeightScaleCollection->Count)
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

