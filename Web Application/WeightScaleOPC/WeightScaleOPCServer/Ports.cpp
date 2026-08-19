/******************************************************************************

	FILE NAME:		Ports.cpp


	PURPOSE:			Implementation of CPorts


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
#include "Ports.h"
#include "DeviceManager.h"
#include ".\ports.h"

extern CDeviceManager*		g_pDeviceManager;

// CPorts

HRESULT CPorts::Activate()
{
	HRESULT hr = GetObjectContext(&m_oObjectContext);
	if( FAILED( hr ))
		return hr;

	hr=m_oDataAccess.CreateInstance(CLSID_DataAccess);
	if( FAILED( hr ))
		return hr;

	return hr;
} 

BOOL CPorts::CanBePooled()
{
	return FALSE;
} 

void CPorts::Deactivate()
{
	m_oObjectContext.Release();
	m_oDataAccess.Release();
} 

STDMETHODIMP CPorts::raw_Enumerate(IDispatch** ppPortCollection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	try
	{
		if(!ppPortCollection)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;
		}

		IPortPtr	oPort(CLSID_Port);
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oPort->EnumerateSQL);
		IPortCollectionPtr	oPortCollection(CLSID_PortCollection);
		while(!oRecordset->EndOfFile)
		{
			IPortPtr		oPort(CLSID_Port);
			oPort->Load(oRecordset);
			oRecordset->MoveNext();
			oPortCollection->Add(oPort);
		}
		*ppPortCollection=oPortCollection.Detach();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
		{
			return Error((LPOLESTR) e.Description(),IID_IPorts);
		}
		else
		{
			return Error((LPOLESTR) e.ErrorMessage(),IID_IPorts);
		}
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Enumerate Error"),IID_IPorts);
	}
}

STDMETHODIMP CPorts::raw_Add(IDispatch* pPort,LONG* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IPortPtr			oPort=pPort;
		if(!plIndex
		|| oPort == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		m_oDataAccess->ExecuteQuery(oPort->InsertSQL);

		*plIndex=GetIndex(oPort->ID);
		oPort->Index=*plIndex;

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
		return Error(_T("Add Error"),IID_IPorts);
	}
}

STDMETHODIMP CPorts::raw_Modify(IDispatch* pPort)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IPortPtr				oPort=pPort;
		if(oPort == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		// Verify ID does not exist
		LONG lIndex=GetIndex(oPort->ID);
		if(lIndex
		&& lIndex != oPort->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_PORT_EXISTS);
			return Error(strError,IID_IPorts);
		}

		m_oDataAccess->ExecuteQuery(oPort->UpdateSQL);

		g_pDeviceManager->ModifyPort(oPort);

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
		return Error(_T("Modify Error"),IID_IPorts);
	}
}

STDMETHODIMP CPorts::raw_Purge(LONG lIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IPortPtr	oPort=Get(lIndex);
		if(!oPort->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_PORT_NOT_FOUND);
			return Error(strError,IID_IPorts);
		}

		m_oDataAccess->ExecuteQuery(oPort->PurgeSQL);

		// For each DanLoad
		IWeightScalesPtr	oWeightScales(CLSID_WeightScales);
		IWeightScaleCollectionPtr	oWeightScaleCollection=oWeightScales->Enumerate();
		for(LONG lItem=0;lItem < oWeightScaleCollection->Count;lItem++)
		{
			IWeightScalePtr	oWeightScale=oWeightScaleCollection->Item(lItem);
			if(oWeightScale->PortIndex == oPort->Index)
			{
				oWeightScale->PortIndex=0;
				oWeightScales->Modify(oWeightScale);
			}
		}

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
		return Error(_T("Purge Error"),IID_IPorts);
	}
}

STDMETHODIMP CPorts::raw_Get(long lIndex, LPDISPATCH *ppPort)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IPortPtr	oPort(CLSID_Port);
		oPort->Index=lIndex;
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oPort->SelectSQL);
		oPort->Load(oRecordset);
		*ppPort=oPort.Detach();
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
		return Error(_T("Get Error"),IID_IPorts);
	}
}

STDMETHODIMP CPorts::raw_GetIndex(BSTR bstrID, long* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IPortPtr	oPort(CLSID_Port);
		oPort->ID=bstrID;
		oPort->Load(m_oDataAccess->GetRecordSet(oPort->SelectByIDSQL));
		*plIndex=oPort->Index;
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
		return Error(_T("GetIndex Error"),IID_IPorts);
	}
}


STDMETHODIMP CPorts::raw_EnumeratePortIDs(VARIANT* pIDs)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	IPortCollectionPtr	oPortCollection=Enumerate();

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
		for(LONG lItem=0;lItem < oPortCollection->Count;lItem++)
		{
			IPortPtr oPort=oPortCollection->Item(lItem);
			if(oPort->ID == _bstr_t(szRegPortName))
				break;
		}

		if(lItem == oPortCollection->Count)
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
