//******************************************************************************
//	FILE NAME:	VarecEnrollmentCtrl.h
//	PURPOSE:	Declaration of the CVarecEnrollmentCtrl ActiveX Control class.		
//
//				See VarecEnrollmentCtrl.cpp for implementation.
//
//	COMMENTS:
//		Copyright (C) Varec, Inc. Norcross, GA, USA, 
//		2008.  This file shall not be copied or reproduced in any form 
//		without the express written consent of Varec, Inc.
//
//	AUTHOR(S):	Ivan Orndorff
//	VERSION:	1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:		By:				Reason:
//		----------	--------------	-------------------------------------------
//		2008-10-07	I. Orndorff		1.0.0.0 - Initial Revision.
//*******************************************************************************       

#pragma once

#include <objsafe.h>

class CVarecEnrollmentCtrl : public COleControl
{
	DECLARE_DYNCREATE(CVarecEnrollmentCtrl)

	DECLARE_INTERFACE_MAP()
	BEGIN_INTERFACE_PART(ObjSafe, IObjectSafety)
		STDMETHOD_(HRESULT, GetInterfaceSafetyOptions) ( 
			/* [in] */ REFIID riid,
			/* [out] */ DWORD __RPC_FAR *pdwSupportedOptions,
			/* [out] */ DWORD __RPC_FAR *pdwEnabledOptions
		);
	    
		STDMETHOD_(HRESULT, SetInterfaceSafetyOptions) ( 
			/* [in] */ REFIID riid,
			/* [in] */ DWORD dwOptionSetMask,
			/* [in] */ DWORD dwEnabledOptions
		);
	END_INTERFACE_PART(ObjSafe);

// Constructor
public:
	CVarecEnrollmentCtrl();

// Overrides
public:
	virtual void OnDraw(CDC* pdc, const CRect& rcBounds, const CRect& rcInvalid);
	virtual void DoPropExchange(CPropExchange* pPX);
	virtual void OnResetState();
	virtual DWORD GetControlFlags();

// Implementation
protected:
	~CVarecEnrollmentCtrl();

	DECLARE_OLECREATE_EX(CVarecEnrollmentCtrl)    // Class factory and guid
	DECLARE_OLETYPELIB(CVarecEnrollmentCtrl)      // GetTypeInfo
	DECLARE_PROPPAGEIDS(CVarecEnrollmentCtrl)     // Property page IDs
	DECLARE_OLECTLTYPE(CVarecEnrollmentCtrl)		// Type name and misc status

// Message maps
	DECLARE_MESSAGE_MAP()

// Dispatch maps
	DECLARE_DISPATCH_MAP()

// Event maps
	DECLARE_EVENT_MAP()

// Dispatch and event IDs
public:
	enum {
			dispidTWIC_ExpirationDate = 4,		
			dispidTWIC_PersonIdentifier = 3, 
			dispidTWIC_ErrorMessage = 2,	
			dispidGetTWICData = 1L
		};

	enum { 
			TWIC_Success = 1, 
			TWIC_Failure = 0 
		};

protected:
	SHORT GetTWICData(void);	
	CString m_TWIC_ErrorMessage;
	CString m_TWIC_PersonIdentifier;
	CString m_TWIC_ExpirationDate;
	void OnTWIC_ErrorMessageChanged(void);
	void OnTWIC_PersonIdentifierChanged(void);
	void OnTWIC_ExpirationDateChanged(void);
};

