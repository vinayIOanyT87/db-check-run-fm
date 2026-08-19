//******************************************************************************
//	FILE NAME:	VarecEnrollmentCtrl.cpp
//	PURPOSE:	Implementation of the CVarecEnrollmentCtrl ActiveX Control class.		
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

#include "stdafx.h"
#include "VarecEnrollment.h"
#include "VarecEnrollmentCtrl.h"
#include "VarecEnrollmentPropPage.h"
#include ".\varecenrollmentctrl.h"
#include "winscard.h"
#include <bitset>
#include <string>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

const DWORD dwSupportedBits = INTERFACESAFE_FOR_UNTRUSTED_CALLER | INTERFACESAFE_FOR_UNTRUSTED_DATA;
const DWORD dwNotSupportedBits = ~ dwSupportedBits;

IMPLEMENT_DYNCREATE(CVarecEnrollmentCtrl, COleControl)

/////////////////////////////////////////////////////////////////////////////
// Interface map for IObjectSafety
BEGIN_INTERFACE_MAP(CVarecEnrollmentCtrl, COleControl)
   INTERFACE_PART(CVarecEnrollmentCtrl, IID_IObjectSafety, ObjSafe)
END_INTERFACE_MAP()


/////////////////////////////////////////////////////////////////////////////
// IObjectSafety member functions
// Delegate AddRef, Release, QueryInterface
ULONG FAR EXPORT CVarecEnrollmentCtrl::XObjSafe::AddRef()
{
    METHOD_PROLOGUE(CVarecEnrollmentCtrl, ObjSafe)
    return pThis->ExternalAddRef();
}

ULONG FAR EXPORT CVarecEnrollmentCtrl::XObjSafe::Release()
{
    METHOD_PROLOGUE(CVarecEnrollmentCtrl, ObjSafe)
    return pThis->ExternalRelease();
}

HRESULT FAR EXPORT CVarecEnrollmentCtrl::XObjSafe::QueryInterface(
    REFIID iid, void FAR* FAR* ppvObj)
{
    METHOD_PROLOGUE(CVarecEnrollmentCtrl, ObjSafe)
    return (HRESULT)pThis->ExternalQueryInterface(&iid, ppvObj);
}


// Message map

BEGIN_MESSAGE_MAP(CVarecEnrollmentCtrl, COleControl)
	ON_OLEVERB(AFX_IDS_VERB_PROPERTIES, OnProperties)
END_MESSAGE_MAP()



// Dispatch map

BEGIN_DISPATCH_MAP(CVarecEnrollmentCtrl, COleControl)
	DISP_FUNCTION_ID(CVarecEnrollmentCtrl, "GetTWICData", dispidGetTWICData, GetTWICData, VT_I2, VTS_NONE)
	DISP_PROPERTY_NOTIFY_ID(CVarecEnrollmentCtrl, "TWIC_ErrorMessage", dispidTWIC_ErrorMessage, m_TWIC_ErrorMessage, OnTWIC_ErrorMessageChanged, VT_BSTR)
	DISP_PROPERTY_NOTIFY_ID(CVarecEnrollmentCtrl, "TWIC_PersonIdentifier", dispidTWIC_PersonIdentifier, m_TWIC_PersonIdentifier, OnTWIC_PersonIdentifierChanged, VT_BSTR)
	DISP_PROPERTY_NOTIFY_ID(CVarecEnrollmentCtrl, "TWIC_ExpirationDate", dispidTWIC_ExpirationDate, m_TWIC_ExpirationDate, OnTWIC_ExpirationDateChanged, VT_BSTR)
END_DISPATCH_MAP()



// Event map

BEGIN_EVENT_MAP(CVarecEnrollmentCtrl, COleControl)
END_EVENT_MAP()



// Property pages

// TODO: Add more property pages as needed.  Remember to increase the count!
BEGIN_PROPPAGEIDS(CVarecEnrollmentCtrl, 1)
	PROPPAGEID(CVarecEnrollmentPropPage::guid)
END_PROPPAGEIDS(CVarecEnrollmentCtrl)



// Initialize class factory and guid

IMPLEMENT_OLECREATE_EX(CVarecEnrollmentCtrl, "VARECENROLLMENT.VarecEnrollmentCtrl.1",
	0xc6ad5c3a, 0xdb26, 0x450a, 0x82, 0xc7, 0x89, 0xd, 0x2d, 0x23, 0xa8, 0xd9)



// Type library ID and version

IMPLEMENT_OLETYPELIB(CVarecEnrollmentCtrl, _tlid, _wVerMajor, _wVerMinor)



// Interface IDs

const IID BASED_CODE IID_DVarecEnrollment =
		{ 0xAF61B8BF, 0xB180, 0x49F8, { 0xAD, 0x3E, 0x95, 0x7D, 0x66, 0x6C, 0x11, 0xE5 } };
const IID BASED_CODE IID_DVarecEnrollmentEvents =
		{ 0xEDE76D49, 0x783F, 0x4AAD, { 0x95, 0x3D, 0xF4, 0xCF, 0xCF, 0x3C, 0x6F, 0x76 } };



// Control type information

static const DWORD BASED_CODE _dwVarecEnrollmentOleMisc =
	OLEMISC_INVISIBLEATRUNTIME |
	OLEMISC_SETCLIENTSITEFIRST |
	OLEMISC_INSIDEOUT |
	OLEMISC_CANTLINKINSIDE |
	OLEMISC_RECOMPOSEONRESIZE;

IMPLEMENT_OLECTLTYPE(CVarecEnrollmentCtrl, IDS_VARECENROLLMENT, _dwVarecEnrollmentOleMisc)



// CVarecEnrollmentCtrl::CVarecEnrollmentCtrlFactory::UpdateRegistry -
// Adds or removes system registry entries for CVarecEnrollmentCtrl

BOOL CVarecEnrollmentCtrl::CVarecEnrollmentCtrlFactory::UpdateRegistry(BOOL bRegister)
{
	// TODO: Verify that your control follows apartment-model threading rules.
	// Refer to MFC TechNote 64 for more information.
	// If your control does not conform to the apartment-model rules, then
	// you must modify the code below, changing the 6th parameter from
	// afxRegApartmentThreading to 0.

	if (bRegister)
		return AfxOleRegisterControlClass(
			AfxGetInstanceHandle(),
			m_clsid,
			m_lpszProgID,
			IDS_VARECENROLLMENT,
			IDB_VARECENROLLMENT,
			afxRegApartmentThreading,
			_dwVarecEnrollmentOleMisc,
			_tlid,
			_wVerMajor,
			_wVerMinor);
	else
		return AfxOleUnregisterClass(m_clsid, m_lpszProgID);
}



// CVarecEnrollmentCtrl::CVarecEnrollmentCtrl - Constructor

CVarecEnrollmentCtrl::CVarecEnrollmentCtrl()
{
	InitializeIIDs(&IID_DVarecEnrollment, &IID_DVarecEnrollmentEvents);
	// TODO: Initialize your control's instance data here.
}



// CVarecEnrollmentCtrl::~CVarecEnrollmentCtrl - Destructor

CVarecEnrollmentCtrl::~CVarecEnrollmentCtrl()
{
	// TODO: Cleanup your control's instance data here.
}



// CVarecEnrollmentCtrl::OnDraw - Drawing function

void CVarecEnrollmentCtrl::OnDraw(
			CDC* pdc, const CRect& rcBounds, const CRect& rcInvalid)
{
	if (!pdc)
		return;

	// TODO: Replace the following code with your own drawing code.
	pdc->FillRect(rcBounds, CBrush::FromHandle((HBRUSH)GetStockObject(WHITE_BRUSH)));
	pdc->Ellipse(rcBounds);
}



// CVarecEnrollmentCtrl::DoPropExchange - Persistence support

void CVarecEnrollmentCtrl::DoPropExchange(CPropExchange* pPX)
{
	ExchangeVersion(pPX, MAKELONG(_wVerMinor, _wVerMajor));
	COleControl::DoPropExchange(pPX);

	// TODO: Call PX_ functions for each persistent custom property.
}



// CVarecEnrollmentCtrl::GetControlFlags -
// Flags to customize MFC's implementation of ActiveX controls.
//
DWORD CVarecEnrollmentCtrl::GetControlFlags()
{
	DWORD dwFlags = COleControl::GetControlFlags();


	// The control can activate without creating a window.
	// TODO: when writing the control's message handlers, avoid using
	//		the m_hWnd member variable without first checking that its
	//		value is non-NULL.
	dwFlags |= windowlessActivate;
	return dwFlags;
}



// CVarecEnrollmentCtrl::OnResetState - Reset control to default state

void CVarecEnrollmentCtrl::OnResetState()
{
	COleControl::OnResetState();  // Resets defaults found in DoPropExchange

	// TODO: Reset any other control state here.
}


/////////////////////////////////////////////////////////////////////////////
// CVarecEnrollmentCtrl::XObjSafe::GetInterfaceSafetyOptions
// Allows container to query what interfaces are safe for what. We're
// optimizing significantly by ignoring which interface the caller is
// asking for.
HRESULT STDMETHODCALLTYPE CVarecEnrollmentCtrl::XObjSafe::GetInterfaceSafetyOptions( 
	/* [in] */ REFIID riid,
	/* [out] */ DWORD __RPC_FAR *pdwSupportedOptions,
	/* [out] */ DWORD __RPC_FAR *pdwEnabledOptions)
{
   METHOD_PROLOGUE(CVarecEnrollmentCtrl, ObjSafe)

   HRESULT retval = ResultFromScode(S_OK);

   // Does interface exist?
   IUnknown FAR* punkInterface;
   retval = pThis->ExternalQueryInterface(&riid, 
               (void * *)&punkInterface);
   if (retval != E_NOINTERFACE) { // interface exists
      punkInterface->Release(); // release it—just checking!
   }
   
   // We support both kinds of safety and have always both set,
   // regardless of interface.
   *pdwSupportedOptions = *pdwEnabledOptions = dwSupportedBits;
   return retval; // E_NOINTERFACE if QI failed
}

/////////////////////////////////////////////////////////////////////////////
// CVarecEnrollmentCtrl::XObjSafe::SetInterfaceSafetyOptions
// Since we're always safe, this is a no-brainer—but we do check to make
// sure the interface requested exists and that the options we're asked to
// set exist and are set on (we don't support unsafe mode).
HRESULT STDMETHODCALLTYPE CVarecEnrollmentCtrl::XObjSafe::SetInterfaceSafetyOptions( 
	/* [in] */ REFIID riid,
	/* [in] */ DWORD dwOptionSetMask,
	/* [in] */ DWORD dwEnabledOptions)
{
    METHOD_PROLOGUE(CVarecEnrollmentCtrl, ObjSafe)
   
   // Does interface exist?
   IUnknown FAR* punkInterface;
   pThis->ExternalQueryInterface(&riid, (void * *)&punkInterface);
   if (punkInterface) { // interface exists
      punkInterface->Release(); // release it—just checking!
   }
   else { // Interface doesn't exist.
      return ResultFromScode(E_NOINTERFACE);
   }
   // Can't set bits we don't support.
   if (dwOptionSetMask & dwNotSupportedBits) { 
      return ResultFromScode(E_FAIL);
   }
   
   // Can't set bits we do support to zero
   dwEnabledOptions &= dwSupportedBits;
   // (We already know there are no extra bits in mask. )
   if ((dwOptionSetMask & dwEnabledOptions) !=
       dwOptionSetMask) {
      return ResultFromScode(E_FAIL);
   }                                    
   
   // Don't need to change anything since we're always safe.
   return ResultFromScode(S_OK);
}


// CVarecEnrollmentCtrl message handlers

SHORT CVarecEnrollmentCtrl::GetTWICData(void)	
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	SCARDCONTEXT    hSC;
	SCARDHANDLE     hCardHandle;
	LONG            lReturn;
	DWORD           dwActiceProtocol;
	LPTSTR          pmszReaders = NULL;
	LPTSTR          pReader;
	LONG            lReturn2;
	DWORD           cch = SCARD_AUTOALLOCATE;
	CString			strCardReader;

	m_TWIC_PersonIdentifier = _T("");
	m_TWIC_ExpirationDate = _T("");

	// Establish the context.
	lReturn = SCardEstablishContext(SCARD_SCOPE_SYSTEM,
									NULL,
									NULL,
									&hSC);
	if ( SCARD_S_SUCCESS != lReturn )
	{
		m_TWIC_ErrorMessage = _T("Error - Failed to establish context with card reader");
		return TWIC_Failure;	
	}
	else
	{
		// Use the context as needed. When done,
		// free the context by calling SCardReleaseContext.
		// ...
	}

	// Retrieve the list the readers.
	// hSC was set by a previous call to SCardEstablishContext.
	lReturn = SCardListReaders(hSC,
								NULL,
								(LPTSTR)&pmszReaders,
								&cch );
	switch( lReturn )
	{
		case SCARD_E_NO_READERS_AVAILABLE:
		{
			m_TWIC_ErrorMessage = _T("Error - No card readers are available");
			return TWIC_Failure;
		}

		case SCARD_S_SUCCESS:
		{
			// Do something with the multi string of readers.
			// Output the values.
			// A double-null terminates the list of values.
			pReader = pmszReaders;
			while ( '\0' != *pReader )
			{
				strCardReader = pReader;
				// Advance to the next value.
				pReader = pReader + wcslen(pReader) + 1;
			}
			// Free the memory.
			lReturn2 = SCardFreeMemory( hSC,
										pmszReaders );
			if ( SCARD_S_SUCCESS != lReturn2 )
			{
				//_T("Failed SCardFreeMemory\r\n")
			}
			break;
		}

		default:
		{
			m_TWIC_ErrorMessage = _T("Error - Failed to get list of smart card readers");
			return TWIC_Failure;
		}
	}

	lReturn = SCardConnect( hSC, 
							strCardReader, 
							SCARD_SHARE_SHARED,
							SCARD_PROTOCOL_T0 | SCARD_PROTOCOL_T1,
							&hCardHandle,
							&dwActiceProtocol );
	if ( SCARD_S_SUCCESS != lReturn )
	{
		// SCARD_W_REMOVED_CARD 
		m_TWIC_ErrorMessage = _T("Error - Failed connecting to smart card reader");
		return TWIC_Failure;
	}

	// Use the connection.
	// Display the active protocol.
	switch ( dwActiceProtocol )
	{
		case SCARD_PROTOCOL_T0:
		{
			//_T("Active protocol T0\r\n")
			break;
		}

		case SCARD_PROTOCOL_T1:
		{
			//_T("Active protocol T1\r\n")
			break;
		}

		case SCARD_PROTOCOL_UNDEFINED:
		default:
		{
			m_TWIC_ErrorMessage = _T("Error - Active protocol unnegotiated or unknown for smart card reader");
			return TWIC_Failure;
		}
	}

	BYTE pbSendSelectTWICBuffer[] = {	0x00, 0xA4, 0x04, 0x00, 0x09, 0xA0, 0x00, 0x00, 0x03, 0x67, 0x20, 0x00, 0x00, 0x01, 0x00 };
	BYTE pbSendGetUnsignedCardHolderUniqueIdentifierBuffer[] = { 0x00, 0xCB, 0x3F, 0xFF, 0x05, 0x5C, 0x03, 0x5F, 0xC1, 0x04, 0x00 };

	BYTE pbRecvBuffer[256]; 
	DWORD dwRecv = sizeof(pbRecvBuffer);

	SCARD_IO_REQUEST RecvRequest;
	RecvRequest.dwProtocol = dwActiceProtocol;
	RecvRequest.cbPciLength = dwRecv; 

	lReturn = SCardTransmit(hCardHandle,
							SCARD_PROTOCOL_T0 == dwActiceProtocol ? SCARD_PCI_T0 : SCARD_PCI_T1,	
							pbSendSelectTWICBuffer,
							sizeof(pbSendSelectTWICBuffer),
							&RecvRequest,
							pbRecvBuffer,
							&dwRecv );

	if ( SCARD_S_SUCCESS != lReturn )
	{
		m_TWIC_ErrorMessage = _T("Error - Failed transmitting to smart card reader");
		return TWIC_Failure;
	}

	dwRecv = 255;
	RecvRequest.cbPciLength = dwRecv; 
	ZeroMemory(pbRecvBuffer,sizeof(pbRecvBuffer));

	lReturn = SCardTransmit(hCardHandle,
							SCARD_PROTOCOL_T0 == dwActiceProtocol ? SCARD_PCI_T0 : SCARD_PCI_T1,	// SCARD_PCI_T1
							pbSendGetUnsignedCardHolderUniqueIdentifierBuffer,
							sizeof(pbSendGetUnsignedCardHolderUniqueIdentifierBuffer),
							&RecvRequest,
							pbRecvBuffer,
							&dwRecv );

	if ( SCARD_S_SUCCESS != lReturn )
	{
		m_TWIC_ErrorMessage = _T("Error - Failed transmitting to smart card reader");
		return TWIC_Failure;
	}

	// Extract the expiration date
	char szyear[5], szmonth[3], szday[3];

	CopyMemory(szyear, (&pbRecvBuffer[49]), 4);
    szyear[4] = '\0';
	CopyMemory(szmonth, (&pbRecvBuffer[53]), 2);
    szmonth[2] = '\0';
	CopyMemory(szday, (&pbRecvBuffer[55]), 2);
    szday[2] = '\0';

	COleDateTime codtexpirationdate(atoi(szyear),atoi(szmonth),atoi(szday),0,0,0);

	// Extract the and convert the Agency, System, Creditial and PIN

	// Convert from hex to binary, one byte at a time
	std::string strfascnbinary;
	for(int ncount = 4; ncount < 29; ncount++)
	{
		unsigned short x = pbRecvBuffer[ncount];	
		std::bitset<8> bits(x); 
		strfascnbinary += bits.template to_string<char, std::char_traits<char>, std::allocator<char> >();
	}
	CString csfasc(strfascnbinary.c_str());

	// loop thru each five bits dropping the partity bit and inverting the rest
	CString csbcdoutput;
	CString cstmp;
	for(int npos = 0; npos < 196; npos+=5 )
	{
		CString cspart( csfasc.Mid(npos, 4));
		CString cspartflip;
		cspartflip += cspart.Mid(3,1) + cspart.Mid(2,1) + cspart.Mid(1,1) + cspart.Mid(0,1);
		LPCTSTR lpszpart = cspartflip.GetBuffer();
		LPTSTR lpszstop;
		unsigned long lpart = wcstoul(lpszpart, &lpszstop, 2);
		cstmp.Format(_T("%ld"),lpart);
		csbcdoutput += cstmp;
	}
	
	lReturn = SCardDisconnect( hCardHandle, 
							   SCARD_LEAVE_CARD);
	if ( SCARD_S_SUCCESS != lReturn )
	{
		m_TWIC_ErrorMessage = _T("Error - Failed disconnecting from smart card reader");
		return TWIC_Failure;
	}

    // build up the success return result 
	m_TWIC_PersonIdentifier = csbcdoutput.Mid(28,10);
	m_TWIC_ExpirationDate = codtexpirationdate.Format(_T("%Y-%m-%d"));
	m_TWIC_ErrorMessage = _T("");
    
	return TWIC_Success;
}

void CVarecEnrollmentCtrl::OnTWIC_PersonIdentifierChanged(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	SetModifiedFlag();
}

void CVarecEnrollmentCtrl::OnTWIC_ErrorMessageChanged(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	SetModifiedFlag();
}

void CVarecEnrollmentCtrl::OnTWIC_ExpirationDateChanged(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	SetModifiedFlag();
}
