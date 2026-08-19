/******************************************************************************

	FILE NAME:		Accuload.cpp


	PURPOSE:			Implementation of CAccuload


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
		01/17/2008	W.Gray		7.3.1.0 -  Added support for TCP/IP

		08/12/2009	W.Gray		7.4.6.0 - Added support for Accuload III-SA (CSI-5640)
*******************************************************************************/

#include "stdafx.h"
#include "Accuload.h"


// CAccuload

STDMETHODIMP CAccuload::InterfaceSupportsErrorInfo(REFIID riid)
{
	static const IID* arr[] = 
	{
		&IID_IAccuload
	};
	for (int i=0; i < sizeof(arr) / sizeof(arr[0]); i++)
	{
		if (::InlineIsEqualGUID(*arr[i],riid))
			return S_OK;
	}
	return S_FALSE;
}

STDMETHODIMP CAccuload::get_Index(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_lIndex;
	return S_OK;
}

STDMETHODIMP CAccuload::put_Index(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lIndex=newVal;
	return S_OK;
}

STDMETHODIMP CAccuload::get_ID(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bstrID.copy();
	return S_OK;
}

STDMETHODIMP CAccuload::put_ID(BSTR newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bstrID=newVal;
	return S_OK;
}

STDMETHODIMP CAccuload::get_Type(ACCULOAD_TYPE* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_Type;
	return S_OK;
}

STDMETHODIMP CAccuload::put_Type(ACCULOAD_TYPE newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_Type=newVal;
	return S_OK;
}

STDMETHODIMP CAccuload::get_PortIndex(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_lPortIndex;
	return S_OK;
}

STDMETHODIMP CAccuload::put_PortIndex(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_lPortIndex=newVal;
	return S_OK;
}

STDMETHODIMP CAccuload::get_Arms(IDispatch** pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	if(m_oArms)
		return m_oArms.QueryInterface(IID_IDispatch,(void**) pVal);
	else
	{
		*pVal=NULL;
		return S_FALSE;
	}
	return S_OK;
}

STDMETHODIMP CAccuload::put_Arms(IDispatch* newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_oArms=newVal;
	return S_OK;
}

STDMETHODIMP CAccuload::get_PortID(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bstrPortID.copy();
	return S_OK;
}

STDMETHODIMP CAccuload::put_PortID(BSTR newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bstrPortID=newVal;
	return S_OK;
}

STDMETHODIMP CAccuload::get_NetworkCommunications(VARIANT_BOOL* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bNetworkCommunications;
	return S_OK;
}

STDMETHODIMP CAccuload::put_NetworkCommunications(VARIANT_BOOL newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_bNetworkCommunications=newVal;
	return S_OK;
}

STDMETHODIMP CAccuload::get_IPAddress(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bstrIPAddress.copy();
	return S_OK;
}

STDMETHODIMP CAccuload::put_IPAddress(BSTR newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bstrIPAddress=newVal;
	return S_OK;
}

STDMETHODIMP CAccuload::raw_TypeID(ACCULOAD_TYPE Type,BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strType;

	switch(Type)
	{
		case ACCULOAD_2_STD:
			strType.LoadString(IDS_ACCULOAD_2_STD);
			break;
		case ACCULOAD_2_SEQ:
			strType.LoadString(IDS_ACCULOAD_2_SEQ);
			break;
		case ACCULOAD_2_RBM:
			strType.LoadString(IDS_ACCULOAD_2_RBM);
			break;
		case ACCULOAD_III_Q:
			strType.LoadString(IDS_ACCULOAD_III_Q);
			break;
		case MICROLOAD_NET:
			strType.LoadString(IDS_MICROLOAD_NET);
			break;
		case MULTILOAD_II_SMP:
			strType.LoadString(IDS_MULTILOAD_II_SMP);
			break;
		case ACCULOAD_III_SA:
			strType.LoadString(IDS_ACCULOAD_III_SA);
			break;
		case MULTILOAD_II:
			strType.LoadString(IDS_MULTILOAD_II);
			break;
		case SMITH_PROXIMITY:
			strType.LoadString(IDS_SMITH_PROXIMITY);
			break;
		case RCU_II_OPEN:
			strType.LoadString(IDS_RCU_II_OPEN);
			break;
		case RCU_II_RCU:
			strType.LoadString(IDS_RCU_II_MULTILOAD);
			break;
		default:
			return E_INVALIDARG;
	}

	*pVal=strType.AllocSysString();

	return S_OK;
}

STDMETHODIMP CAccuload::raw_Load(IDispatch *pRecordSet)
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
						*((__int64*) pData)=oField->Value.llVal;
						break;
					case VT_UI8:
						*((__int64*) pData)=oField->Value.ullVal;
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

		if(m_lPortIndex == 0)
			m_bstrPortID=_T("{None}");

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


STDMETHODIMP CAccuload::raw_Reset(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lIndex=0;
	m_bstrID=_T("");
	m_Type=ACCULOAD_III_Q;
	m_lPortIndex=0;
	m_bstrPortID=_T("{None}");
	m_bNetworkCommunications=false;
	m_bstrIPAddress=_T("");
	m_oArms.CreateInstance(CLSID_ArmCollection);

	return S_OK;
}

STDMETHODIMP CAccuload::get_InsertSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	TCHAR		szValue[34];

	strSQL.Format(	_T("INSERT INTO tblAcculoads ")\
						_T("(ID,")\
						_T("Type,")\
						_T("PortIndex,")\
						_T("NetworkCommunications,")\
						_T("IPAddress")\
						_T(") VALUES ('%s',%d,%s,%d,'%s')"),
						(LPCTSTR) m_bstrID,
						m_Type,
						(m_lPortIndex) ? CString(_ltow(m_lPortIndex,szValue,10)) : _T("NULL"),
						(m_bNetworkCommunications) ? 1 : 0,
						(LPCTSTR) m_bstrIPAddress
						);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CAccuload::get_UpdateSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	TCHAR		szValue[34];
	
	strSQL.Format(	_T("UPDATE tblAcculoads ")\
						_T("SET ID = '%s',")\
						_T("Type = %d,")\
						_T("PortIndex = %s,")\
						_T("NetworkCommunications = %d,")\
						_T("IPAddress = '%s' ")\
						_T("WHERE [Index] = '%d'"),
						(LPCTSTR) m_bstrID,
						m_Type,
						(m_lPortIndex) ? CString(_ltow(m_lPortIndex,szValue,10)) : _T("NULL"),
						(m_bNetworkCommunications) ? 1 : 0,
						(LPCTSTR) m_bstrIPAddress,
						m_lIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CAccuload::get_PurgeSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format(	_T("DELETE FROM tblAcculoads WHERE [Index] = '%d'"),
						m_lIndex);


	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CAccuload::get_SelectSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format( _T("SELECT *,")\
						_T("(SELECT ID FROM tblPorts WHERE tblPorts.[Index] = tblAcculoads.PortIndex) AS PortID ")\
						_T(" FROM tblAcculoads WHERE [Index] = '%d'"),
						m_lIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CAccuload::get_SelectByIDSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format( _T("SELECT *,")\
						_T("(SELECT ID FROM tblPorts WHERE tblPorts.[Index] = tblAcculoads.PortIndex) AS PortID ")\
						_T(" FROM tblAcculoads WHERE ID = '%s'"),
						(LPCTSTR) m_bstrID);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CAccuload::get_EnumerateSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	
	strSQL.Format(	_T("SELECT *,")\
						_T("(SELECT ID FROM tblPorts WHERE tblPorts.[Index] = tblAcculoads.PortIndex) AS PortID ")\
						_T(" FROM tblAcculoads ORDER BY ID"));

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

