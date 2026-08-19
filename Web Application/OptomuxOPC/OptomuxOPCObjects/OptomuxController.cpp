/******************************************************************************

	FILE NAME:		OptomuxController.cpp


	PURPOSE:			Implementation of COptomuxController


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
*******************************************************************************/

#include "stdafx.h"
#include "OptomuxController.h"


// COptomuxController

STDMETHODIMP COptomuxController::InterfaceSupportsErrorInfo(REFIID riid)
{
	static const IID* arr[] = 
	{
		&IID_IOptomuxController
	};
	for (int i=0; i < sizeof(arr) / sizeof(arr[0]); i++)
	{
		if (::InlineIsEqualGUID(*arr[i],riid))
			return S_OK;
	}
	return S_FALSE;
}

STDMETHODIMP COptomuxController::get_Index(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_lIndex;
	return S_OK;
}

STDMETHODIMP COptomuxController::put_Index(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lIndex=newVal;
	return S_OK;
}

STDMETHODIMP COptomuxController::get_ID(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bstrID.copy();
	return S_OK;
}

STDMETHODIMP COptomuxController::put_ID(BSTR newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bstrID=newVal;
	return S_OK;
}

STDMETHODIMP COptomuxController::get_Type(OPTOMUX_TYPE* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_Type;
	return S_OK;
}

STDMETHODIMP COptomuxController::put_Type(OPTOMUX_TYPE newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_Type=newVal;
	return S_OK;
}

STDMETHODIMP COptomuxController::get_Address(BYTE* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bAddress;
	return S_OK;
}

STDMETHODIMP COptomuxController::put_Address(BYTE newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_bAddress=newVal;
	return S_OK;
}

STDMETHODIMP COptomuxController::get_PortIndex(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_lPortIndex;
	return S_OK;
}

STDMETHODIMP COptomuxController::put_PortIndex(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_lPortIndex=newVal;
	return S_OK;
}


STDMETHODIMP COptomuxController::get_ModuleInputOutputMap(BYTE* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bModuleInputOutputMap;
	return S_OK;
}

STDMETHODIMP COptomuxController::put_ModuleInputOutputMap(BYTE newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_bModuleInputOutputMap=newVal;
	return S_OK;
}

STDMETHODIMP COptomuxController::get_NetworkCommunications(VARIANT_BOOL* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bNetworkCommunications;
	return S_OK;
}

STDMETHODIMP COptomuxController::put_NetworkCommunications(VARIANT_BOOL newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_bNetworkCommunications=newVal;
	return S_OK;
}

STDMETHODIMP COptomuxController::get_IPAddress(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bstrIPAddress.copy();
	return S_OK;
}

STDMETHODIMP COptomuxController::put_IPAddress(BSTR newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bstrIPAddress=newVal;
	return S_OK;
}

STDMETHODIMP COptomuxController::get_Port(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_lPort;
	return S_OK;
}

STDMETHODIMP COptomuxController::put_Port(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lPort=newVal;
	return S_OK;
}

STDMETHODIMP COptomuxController::get_PortID(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bstrPortID.copy();
	return S_OK;
}

STDMETHODIMP COptomuxController::put_PortID(BSTR newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bstrPortID=newVal;
	return S_OK;
}

STDMETHODIMP COptomuxController::raw_TypeID(OPTOMUX_TYPE Type,BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strType;

	switch(Type)
	{
		case PASSCONTROLLER_HC05:
			strType=_T("PASSController HC05");
			break;
		case PASSCONTROLLER_HC12:
			strType=_T("PASSController HC12");
			break;
		case VAREC_DET:
			strType=_T("Varec DET");
			break;
		default:
			return E_INVALIDARG;
	}

	*pVal=strType.AllocSysString();

	return S_OK;
}



STDMETHODIMP COptomuxController::raw_Load(IDispatch *pRecordSet)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())

	try
	{
		raw_Reset();
		_RecordsetPtr	oADORecordset=pRecordSet;
		if(oADORecordset == NULL)
			return E_INVALIDARG;

		if(oADORecordset->EndOfFile)
			return S_FALSE;

		FieldsPtr	oFields = oADORecordset->Fields;
		FieldPtr		oField;

		const ATL_PROPMAP_ENTRY* pMap=GetPropertyMap();
		if(!pMap)
			return E_FAIL;

		CComPtr<IDispatch> pDispatch;
		const IID* piidOld = NULL;

		HRESULT hr=S_OK;

		for (LONG i = 0; pMap[i].pclsidPropPage != NULL; i++)
		{
			if(i >= oFields->Count)
				break;

			oField=oFields->GetItem(i);

			if(oField->Value.vt == VT_NULL)
				continue;

			if (pMap[i].szDesc == NULL)
				continue;

			// check if raw data entry
			if (pMap[i].dwSizeData != 0)
			{
				void* pData = (void*) (pMap[i].dwOffsetData + (DWORD_PTR) this);

				switch(pMap[i].vt)
				{
					case VT_I2:
					case VT_BOOL:
						*((SHORT*) pData)=oField->Value;
						break;
					case VT_I4:
						*((LONG*) pData)=oField->Value;
						break;
					case VT_R4:
						*((FLOAT*) pData)=oField->Value;
						break;
					case VT_R8:
						*((DOUBLE*) pData)=oField->Value;
						break;
					case VT_DATE:
						*((DATE*) pData)=oField->Value;
						break;
					case VT_BSTR:
						*((BSTR) pData)=(OLECHAR)oField->Value.bstrVal;
						break;
					case VT_I1:
						*((SHORT*) pData)=oField->Value;
						break;
					case VT_UI1:
						*((BYTE*) pData)=oField->Value;
						break;
					case VT_UI2:
						*((WORD*) pData)=oField->Value;
						break;
					case VT_UI4:
						*((DWORD*) pData)=oField->Value;
						break;
					case VT_I8:
						*((__int64*) pData)=(__int64)oField->Value.llVal;
						break;
					case VT_UI8:
						*((__int64*) pData)=(__int64)oField->Value.ullVal;
						break;
					case VT_INT:
						*((INT*) pData)=oField->Value;
						break;
					case VT_UINT:
						*((UINT*) pData)=oField->Value;
						break;

					default:
						break;
				}
				continue;
			}

			if (pMap[i].piidDispatch != piidOld)
			{
				pDispatch.Release();
				if (FAILED(QueryInterface(*pMap[i].piidDispatch, (void**)&pDispatch)))
				{
					CString strError;
					strError.Format(_T("Failed to get dispatch pointer for property %s"),pMap[i].szDesc);
					return Error(strError);
				}
				piidOld = pMap[i].piidDispatch;
			}

			if (FAILED(pDispatch.PutProperty(pMap[i].dispid, &oField->Value)))
			{
				CString strError;
				strError.Format(_T("Invoked failed on %s DISPID %x"),pMap[i].szDesc,pMap[i].dispid);
				return Error(strError);
			}
		}
		return hr;
	}
	catch (_com_error& e)
	{
		if(e.Description().length())
			return Error((LPOLESTR) e.Description());
		else
			return Error((LPOLESTR) e.ErrorMessage());
	}
	catch(...)
	{
		CString strError;
		strError.Format(_T("Error Loading Data Object"));
		return Error(strError);
	}
}


STDMETHODIMP COptomuxController::raw_Reset(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lIndex=0;
	m_bstrID=_T("");
	m_Type=VAREC_DET;
	m_bAddress=0;
	m_lPortIndex=0;
	m_bModuleInputOutputMap=0xFF;
	m_bNetworkCommunications=false;
	m_bstrIPAddress=_T("");
	m_lPort=80;
	m_bstrPortID=_T("{None}");

	return S_OK;
}

STDMETHODIMP COptomuxController::get_InsertSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	TCHAR		szValue[34];

	strSQL.Format(	_T("INSERT INTO tblOptomuxControllers ")\
						_T("(ID,")\
						_T("Type,")\
						_T("Address,")\
						_T("PortIndex,")\
						_T("ModuleInputOutputMap,")\
						_T("NetworkCommunications,")\
						_T("IPAddress,")\
						_T("Port")\
						_T(") VALUES ('%s',%d,%d,%s,%d,%d,'%s',%d)"),
						(LPCTSTR) m_bstrID,
						m_Type,
						m_bAddress,
						(m_lPortIndex) ? CString(_ltow(m_lPortIndex,szValue,10)) : _T("NULL"),
						m_bModuleInputOutputMap,
						(m_bNetworkCommunications) ? 1 : 0,
						(LPCTSTR) m_bstrIPAddress,
						m_lPort
						);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP COptomuxController::get_UpdateSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	TCHAR		szValue[34];
	
	strSQL.Format(	_T("UPDATE tblOptomuxControllers ")\
						_T("SET ID = '%s',")\
						_T("Type = %d,")\
						_T("Address = %d,")\
						_T("PortIndex = %s,")\
						_T("ModuleInputOutputMap = %d,")\
						_T("NetworkCommunications = %d,")\
						_T("IPAddress = '%s',")\
						_T("Port = %d ")\
						_T("WHERE [Index] = '%d'"),
						(LPCTSTR) m_bstrID,
						m_Type,
						m_bAddress,
						(m_lPortIndex) ? CString(_ltow(m_lPortIndex,szValue,10)) : _T("NULL"),
						m_bModuleInputOutputMap,
						(m_bNetworkCommunications) ? 1 : 0,
						(LPCTSTR) m_bstrIPAddress,
						m_lPort,
						m_lIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP COptomuxController::get_PurgeSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format(	_T("DELETE FROM tblOptomuxControllers WHERE [Index] = '%d'"),
						m_lIndex);


	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP COptomuxController::get_SelectSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format( _T("SELECT *,")\
						_T("(SELECT ID FROM tblPorts WHERE tblPorts.[Index] = tblOptomuxControllers.PortIndex) AS PortID ")\
						_T(" FROM tblOptomuxControllers WHERE [Index] = '%d'"),
						m_lIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP COptomuxController::get_SelectByIDSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format( _T("SELECT *,")\
						_T("(SELECT ID FROM tblPorts WHERE tblPorts.[Index] = tblOptomuxControllers.PortIndex) AS PortID ")\
						_T(" FROM tblOptomuxControllers WHERE ID = '%s'"),
						(LPCTSTR) m_bstrID);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP COptomuxController::get_EnumerateSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	
	strSQL.Format(	_T("SELECT *,")\
						_T("(SELECT ID FROM tblPorts WHERE tblPorts.[Index] = tblOptomuxControllers.PortIndex) AS PortID ")\
						_T(" FROM tblOptomuxControllers ORDER BY ID"));

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

