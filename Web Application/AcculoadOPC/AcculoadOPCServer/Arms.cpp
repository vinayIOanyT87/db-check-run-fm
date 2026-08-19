/******************************************************************************

	FILE NAME:		Arms.cpp


	PURPOSE:			Implementation of CArms


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
#include "Arms.h"


// CArms

HRESULT CArms::Activate()
{
	HRESULT hr = GetObjectContext(&m_oObjectContext);
	if( FAILED( hr ))
		return hr;

	hr=m_oDataAccess.CreateInstance(CLSID_DataAccess);
	if( FAILED( hr ))
		return hr;

	return hr;
} 

BOOL CArms::CanBePooled()
{
	return FALSE;
} 

void CArms::Deactivate()
{
	m_oObjectContext.Release();
	m_oDataAccess.Release();
} 


STDMETHODIMP CArms::raw_EnumerateByAcculoadIndex(LONG lIndex,IDispatch** ppArmCollection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	try
	{
		if(!ppArmCollection)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;
		}

		IArmPtr	oArm(CLSID_Arm);
		oArm->AcculoadIndex=lIndex;
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oArm->EnumerateByAcculoadIndexSQL);
		IArmCollectionPtr	oArmCollection(CLSID_ArmCollection);
		while(!oRecordset->EndOfFile)
		{
			IArmPtr		oArm(CLSID_Arm);
			oArm->Load(oRecordset);
			oRecordset->MoveNext();
			oArmCollection->Add(oArm);
		}
		*ppArmCollection=oArmCollection.Detach();
		return S_OK;
	}
	catch (_com_error& e)
	{
		m_oObjectContext->SetAbort();
		if(e.Description().length())
		{
			return Error((LPOLESTR) e.Description(),IID_IArms);
		}
		else
		{
			return Error((LPOLESTR) e.ErrorMessage(),IID_IArms);
		}
	}
	catch (...)
	{
		m_oObjectContext->SetAbort();
		return Error(_T("Enumerate Error"),IID_IArms);
	}
}

STDMETHODIMP CArms::raw_Add(IDispatch* pArm,LONG* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IArmPtr			oArm=pArm;
		if(!plIndex
		|| oArm == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		if(GetIndex(oArm->AcculoadIndex,oArm->Number))
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_ARM_EXISTS);
			return Error(strError,IID_IArms);
		}

		m_oDataAccess->ExecuteQuery(oArm->InsertSQL);

		*plIndex=GetIndex(oArm->AcculoadIndex,oArm->Number);
		oArm->Index=*plIndex;

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
		return Error(_T("Add Error"),IID_IArms);
	}
}

STDMETHODIMP CArms::raw_Modify(IDispatch* pArm)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IArmPtr				oArm=pArm;
		if(oArm == NULL)
		{
			m_oObjectContext->SetAbort();
			return E_INVALIDARG;	
		}

		// Verify ID does not exist
		LONG lIndex=GetIndex(oArm->AcculoadIndex,oArm->Number);
		if(lIndex
		&& lIndex != oArm->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_ARM_EXISTS);
			return Error(strError,IID_IArms);
		}

		m_oDataAccess->ExecuteQuery(oArm->UpdateSQL);

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
		return Error(_T("Modify Error"),IID_IArms);
	}
}

STDMETHODIMP CArms::raw_Purge(LONG lIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IArmPtr	oArm=Get(lIndex);
		if(!oArm->Index)
		{
			m_oObjectContext->SetAbort();
			CString strError;
			strError.LoadString(IDS_ERROR_ARM_NOT_FOUND);
			return Error(strError,IID_IArms);
		}

		m_oDataAccess->ExecuteQuery(oArm->PurgeSQL);
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
		return Error(_T("Purge Error"),IID_IArms);
	}
}

STDMETHODIMP CArms::raw_Get(long lIndex, LPDISPATCH *ppArm)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IArmPtr	oArm(CLSID_Arm);
		oArm->Index=lIndex;
		_RecordsetPtr	oRecordset=m_oDataAccess->GetRecordSet(oArm->SelectSQL);
		oArm->Load(oRecordset);
		*ppArm=oArm.Detach();
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
		return Error(_T("Get Error"),IID_IArms);
	}
}

STDMETHODIMP CArms::raw_GetIndex(LONG lAcculoadIndex,BYTE bNumber, long* plIndex)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IArmPtr	oArm(CLSID_Arm);
		oArm->AcculoadIndex=lAcculoadIndex;
		oArm->Number=bNumber;
		oArm->Load(m_oDataAccess->GetRecordSet(oArm->SelectByAcculoadIndexAndNumberSQL));
		*plIndex=oArm->Index;
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
		return Error(_T("GetIndex Error"),IID_IArms);
	}
}

STDMETHODIMP CArms::raw_ModifyCollection(	LONG			lIndex,
														LPDISPATCH	pArms)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState())
	try
	{
		IArmCollectionPtr	oNewArms=pArms;
		if(oNewArms == NULL)
			return E_INVALIDARG;

		IArmCollectionPtr	oExistingArms=EnumerateByAcculoadIndex(lIndex);

		for(LONG lNewItem=0;lNewItem < oNewArms->Count;lNewItem++)
		{
			IArmPtr	oNewArm=oNewArms->Item(lNewItem);
			oNewArm->AcculoadIndex=lIndex;
			for(LONG lExistingItem=0;lExistingItem < oExistingArms->Count;lExistingItem++)
			{
				IArmPtr	oExistingArm=oExistingArms->Item(lExistingItem);
				if(oExistingArm->Index == oNewArm->Index
				|| (oNewArm->Index == 0
				&& oExistingArm->Number == oNewArm->Number))
				{
					oNewArm->Index=oExistingArm->Index;
					Modify(oNewArm);
					oExistingArms->Remove(lExistingItem);
					oNewArm=NULL;
					break;
				}
			}

			if(oNewArm != NULL)
				Add(oNewArm);
		}

		for(LONG lItem=0;lItem < oExistingArms->Count;lItem++)
		{
			IArmPtr	oExistingArm=oExistingArms->Item(lItem);
			Purge(oExistingArm->Index);
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
		return Error(_T("ModifyCollection Error"),IID_IArms);
	}

}
