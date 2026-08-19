/******************************************************************************

	FILE NAME:		Scully.cpp


	PURPOSE:			Implementation of CScully


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	S. Jiang


	VERSION:		9.0.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
*******************************************************************************/

#include "stdafx.h"
#include "Scully.h"

// CScully

STDMETHODIMP CScully::InterfaceSupportsErrorInfo(REFIID riid)
{
	static const IID* arr[] = 
	{
		&IID_IScully
	};

	for (int i=0; i < sizeof(arr) / sizeof(arr[0]); i++)
	{
		if (InlineIsEqualGUID(*arr[i],riid))
			return S_OK;
	}
	return S_FALSE;
}

STDMETHODIMP CScully::get_Index(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_lIndex;
	return S_OK;
}

STDMETHODIMP CScully::put_Index(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lIndex=newVal;
	return S_OK;
}

STDMETHODIMP CScully::get_ID(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bstrID.copy();
	return S_OK;
}

STDMETHODIMP CScully::put_ID(BSTR newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bstrID=newVal;
	return S_OK;
}

STDMETHODIMP CScully::get_PortIndex(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_lPortIndex;
	return S_OK;
}

STDMETHODIMP CScully::put_PortIndex(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_lPortIndex=newVal;
	return S_OK;
}

STDMETHODIMP CScully::get_DeviceID(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_DeviceID;
	return S_OK;
}

STDMETHODIMP CScully::put_DeviceID(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_DeviceID=newVal;
	return S_OK;
}

STDMETHODIMP CScully::raw_Load(IDispatch *pRecordSet)
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
		return S_OK;
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


STDMETHODIMP CScully::raw_Reset(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lIndex=0;
	m_bstrID=_T("");
	m_DeviceID = 1;
//	m_bstrPort=_T("COM1");

	return S_OK;
}

STDMETHODIMP CScully::get_InsertSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format(	_T("INSERT INTO tblScullys ")\
						_T("(ID,")\
						_T("PortIndex,")\
						_T("DeviceID")\
						_T(") VALUES ('%s',%d,%d)"),
						(LPCTSTR) m_bstrID,
						m_lPortIndex,
						m_DeviceID
						);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CScully::get_UpdateSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	
	strSQL.Format(	_T("UPDATE tblScullys ")\
						_T("SET ID = '%s',")\
						_T("PortIndex = %d,")\
						_T("DeviceID = %d ")\
						_T("WHERE [Index] = '%d'"),
						(LPCTSTR) m_bstrID,
						m_lPortIndex,
						m_DeviceID,
						m_lIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CScully::get_PurgeSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format(	_T("DELETE FROM tblScullys WHERE [Index] = '%d'"),
						m_lIndex);


	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CScully::get_SelectSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format( _T("SELECT *")\
						_T(" FROM tblScullys WHERE [Index] = '%d'"),
						m_lIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CScully::get_SelectByIDSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format( _T("SELECT *")\
						_T(" FROM tblScullys WHERE ID = '%s'"),
						(LPCTSTR) m_bstrID);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CScully::get_EnumerateSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	
	strSQL.Format(	_T("SELECT *")\
						_T(" FROM tblScullys ORDER BY ID"));

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

