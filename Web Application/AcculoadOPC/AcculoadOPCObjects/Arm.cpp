/******************************************************************************

	FILE NAME:		Arm.cpp


	PURPOSE:			Implementation of CArm


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
#include "Arm.h"


// CArm

STDMETHODIMP CArm::InterfaceSupportsErrorInfo(REFIID riid)
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

STDMETHODIMP CArm::get_Index(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_lIndex;
	return S_OK;
}

STDMETHODIMP CArm::put_Index(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lIndex=newVal;
	return S_OK;
}

STDMETHODIMP CArm::get_AcculoadIndex(LONG* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_lAcculoadIndex;
	return S_OK;
}

STDMETHODIMP CArm::put_AcculoadIndex(LONG newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lAcculoadIndex=newVal;
	return S_OK;
}

STDMETHODIMP CArm::get_Number(BYTE* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bNumber;
	return S_OK;
}

STDMETHODIMP CArm::put_Number(BYTE newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bNumber=newVal;
	return S_OK;
}

STDMETHODIMP CArm::get_Address(BYTE* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bAddress;
	return S_OK;
}

STDMETHODIMP CArm::put_Address(BYTE newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bAddress=newVal;
	return S_OK;
}

STDMETHODIMP CArm::get_Type(ACCULOAD_ARM_TYPE* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_Type;
	return S_OK;
}

STDMETHODIMP CArm::put_Type(ACCULOAD_ARM_TYPE newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_Type=newVal;
	return S_OK;
}

STDMETHODIMP CArm::get_Products(BYTE* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	if(!pVal)
		return E_INVALIDARG;
	*pVal=m_bProducts;
	return S_OK;
}

STDMETHODIMP CArm::put_Products(BYTE newVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_bProducts=newVal;
	return S_OK;
}


STDMETHODIMP CArm::raw_TypeID(ACCULOAD_ARM_TYPE Type,BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strType;

	switch(Type)
	{
		case STRAIGHT:
			strType.LoadString(IDS_STRAIGHT);
			break;
		case SEQUENTIAL:
			strType.LoadString(IDS_SEQUENTIAL);
			break;
		case RATIO:
			strType.LoadString(IDS_RATIO);
			break;
		case SIDE_STREAM:
			strType.LoadString(IDS_SIDE_STREAM);
			break;
		case UNLOADING:
			strType.LoadString(IDS_UNLOADING);
			break;
		case NONE:
			strType.LoadString(IDS_NONE);
			break;
		default:
			strType.LoadString(IDS_UNDEFINED);
			break;
	}

	*pVal=strType.AllocSysString();

	return S_OK;
}

STDMETHODIMP CArm::raw_Load(IDispatch *pRecordSet)
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
					ATLTRACE(atlTraceCOM, 0, _T("Failed to get a dispatch pointer for property #%i\n"), i);
					hr = E_FAIL;
					break;
				}
				piidOld = pMap[i].piidDispatch;
			}

			if (FAILED(pDispatch.PutProperty(pMap[i].dispid, &oField->Value)))
			{
				ATLTRACE(atlTraceCOM, 0, _T("Invoked failed on DISPID %x\n"), pMap[i].dispid);
				hr = E_FAIL;
				break;
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



STDMETHODIMP CArm::raw_Reset(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	m_lIndex=0;
	m_lAcculoadIndex=0;
	m_bNumber=1;
	m_bAddress=1;
	m_Type=MAX_ACCULOAD_ARM_TYPE;
	m_bProducts=1;
	return S_OK;
}

STDMETHODIMP CArm::get_InsertSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format(	_T("INSERT INTO tblArms ")\
						_T("(AcculoadIndex,")\
						_T("Number,")\
						_T("Address,")\
						_T("Type,")\
						_T("Products")\
						_T(") VALUES (%d,%d,%d,%d,%d)"),
						m_lAcculoadIndex,
						m_bNumber,
						m_bAddress,
						m_Type,
						m_bProducts
						);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CArm::get_UpdateSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	
	strSQL.Format(	_T("UPDATE tblArms ")\
						_T("SET Number = %d,")\
						_T("Address = %d,")\
						_T("Type = %d,")\
						_T("Products = %d ")\
						_T("WHERE [Index] = '%d'"),
						m_bNumber,
						m_bAddress,
						m_Type,
						m_bProducts,
						m_lIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CArm::get_PurgeSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format(	_T("DELETE FROM tblArms WHERE [Index] = '%d'"),
						m_lIndex);


	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CArm::get_SelectSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format( _T("SELECT *")\
						_T(" FROM tblArms WHERE [Index] = '%d'"),
						m_lIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CArm::get_SelectByAcculoadIndexAndNumberSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;

	strSQL.Format( _T("SELECT *")\
						_T(" FROM tblArms WHERE AcculoadIndex = '%d' AND Number = '%d'"),
						m_lAcculoadIndex,
						m_bNumber);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}

STDMETHODIMP CArm::get_EnumerateByAcculoadIndexSQL(BSTR* pVal)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(!pVal)
		return E_INVALIDARG;

	CString strSQL;
	
	strSQL.Format(	_T("SELECT *")\
						_T(" FROM tblArms WHERE AcculoadIndex = '%d' ORDER BY Number"),
						m_lAcculoadIndex);

	*pVal=strSQL.AllocSysString();

	return S_OK;
}


