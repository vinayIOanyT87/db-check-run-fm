/******************************************************************************

	FILE NAME:		Port.cpp


	PURPOSE:			Implementation of CPort


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
#include "Port.h"

// CPort

STDMETHODIMP CPort::InterfaceSupportsErrorInfo(REFIID riid)
{
	static const IID* arr[] = 
	{
		&IID_IPort
	};
	for (int i=0; i < sizeof(arr) / sizeof(arr[0]); i++)
	{
		if (::InlineIsEqualGUID(*arr[i],riid))
			return S_OK;
	}
	return S_FALSE;
}

STDMETHODIMP CPort::get_Index(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_lIndex;
	return S_OK;
}

STDMETHODIMP CPort::put_Index(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lIndex=newVal;
	return S_OK;
}

STDMETHODIMP CPort::get_ID(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bstrID.copy();
	return S_OK;
}

STDMETHODIMP CPort::put_ID(BSTR newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bstrID=newVal;
	return S_OK;
}

STDMETHODIMP CPort::get_Baud(OSDP_BAUD* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_Baud;
	return S_OK;
}

STDMETHODIMP CPort::put_Baud(OSDP_BAUD newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_Baud=newVal;
	return S_OK;
}

STDMETHODIMP CPort::get_DataBits(OSDP_DATA_BITS* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_DataBits;
	return S_OK;
}

STDMETHODIMP CPort::put_DataBits(OSDP_DATA_BITS newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_DataBits=newVal;
	return S_OK;
}

STDMETHODIMP CPort::get_Parity(OSDP_PARITY* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_Parity;
	return S_OK;
}

STDMETHODIMP CPort::put_Parity(OSDP_PARITY newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_Parity=newVal;
	return S_OK;
}

STDMETHODIMP CPort::get_StopBits(OSDP_STOP_BITS* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_StopBits;
	return S_OK;
}

STDMETHODIMP CPort::put_StopBits(OSDP_STOP_BITS newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_StopBits=newVal;
	return S_OK;
}

STDMETHODIMP CPort::raw_BaudID(OSDP_BAUD Baud,BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strBaud;

	switch(Baud)
	{
		case OSDP_BAUD_1200:
			strBaud=_T("1200");
			break;
		case OSDP_BAUD_2400:
			strBaud=_T("2400");
			break;
		case OSDP_BAUD_4800:
			strBaud=_T("4800");
			break;
		case OSDP_BAUD_9600:
			strBaud=_T("9600");
			break;
		case OSDP_BAUD_19200:
			strBaud=_T("19200");
			break;
		case OSDP_BAUD_38400:
			strBaud=_T("38400");
			break;
		default:
			return E_INVALIDARG;
	}

	*pVal=strBaud.AllocSysString();

	return S_OK;
}

STDMETHODIMP CPort::raw_DataBitsID(OSDP_DATA_BITS DataBits,BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strDataBits;

	switch(DataBits)
	{
		case DATA_BITS_7:
			strDataBits=_T("7");
			break;
		case DATA_BITS_8:
			strDataBits=_T("8");
			break;
		default:
			return E_INVALIDARG;
	}

	*pVal=strDataBits.AllocSysString();

	return S_OK;
}

STDMETHODIMP CPort::raw_ParityID(OSDP_PARITY Baud,BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strParity;

	switch(Baud)
	{
		case OSDP_PARITY_NONE:
			strParity=_T("None");
			break;
		case OSDP_PARITY_EVEN:
			strParity=_T("Even");
			break;
		case OSDP_PARITY_ODD:
			strParity=_T("Odd");
			break;
		default:
			return E_INVALIDARG;
	}

	*pVal=strParity.AllocSysString();

	return S_OK;
}

STDMETHODIMP CPort::raw_StopBitsID(OSDP_STOP_BITS StopBits,BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strStopBits;

	switch(StopBits)
	{
		case STOP_BITS_1:
			strStopBits=_T("1");
			break;
		case STOP_BITS_2:
			strStopBits=_T("2");
			break;
		default:
			return E_INVALIDARG;
	}

	*pVal=strStopBits.AllocSysString();

	return S_OK;
}


STDMETHODIMP CPort::raw_Load(IDispatch *pRecordSet)
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
						*((__int64*) pData)=(__int64)oField->Value.llVal;
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


STDMETHODIMP CPort::raw_Reset(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lIndex=0;
	m_bstrID=_T("COM1");
	m_Baud=OSDP_BAUD_38400;
	m_DataBits=DATA_BITS_8;
	m_Parity=OSDP_PARITY_NONE;
	m_StopBits=STOP_BITS_1;

	return S_OK;
}

STDMETHODIMP CPort::get_InsertSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format(	_T("INSERT INTO tblPorts ")\
						_T("(ID,")\
						_T("Baud,")\
						_T("DataBits,")\
						_T("Parity,")\
						_T("StopBits")\
						_T(") VALUES ('%s',%d,%d,%d,%d)"),
						(LPCTSTR) m_bstrID,
						m_Baud,
						m_DataBits,
						m_Parity,
						m_StopBits
						);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CPort::get_UpdateSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	
	strSQL.Format(	_T("UPDATE tblPorts ")\
						_T("SET ID = '%s',")\
						_T("Baud = %d,")\
						_T("DataBits = %d,")\
						_T("Parity = %d,")\
						_T("StopBits = %d ")\
						_T("WHERE [Index] = '%d'"),
						(LPCTSTR) m_bstrID,
						m_Baud,
						m_DataBits,
						m_Parity,
						m_StopBits,
						m_lIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CPort::get_PurgeSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format(	_T("DELETE FROM tblPorts WHERE [Index] = '%d'"),
						m_lIndex);


	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CPort::get_SelectSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format( _T("SELECT *")\
						_T(" FROM tblPorts WHERE [Index] = '%d'"),
						m_lIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CPort::get_SelectByIDSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format( _T("SELECT *")\
						_T(" FROM tblPorts WHERE ID = '%s'"),
						(LPCTSTR) m_bstrID);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CPort::get_EnumerateSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	
	strSQL.Format(	_T("SELECT *")\
						_T(" FROM tblPorts ORDER BY ID"));

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

