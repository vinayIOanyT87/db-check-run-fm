// VarecEnrollment.cpp : Implementation of CVarecEnrollmentApp and DLL registration.

#include "stdafx.h"
#include "VarecEnrollment.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


CVarecEnrollmentApp NEAR theApp;

const GUID CDECL BASED_CODE _tlid =
		{ 0x328129DD, 0xD0E4, 0x4F09, { 0xA7, 0x82, 0xBF, 0x5, 0xEB, 0xE3, 0x56, 0xB9 } };
const WORD _wVerMajor = 1;
const WORD _wVerMinor = 0;



// CVarecEnrollmentApp::InitInstance - DLL initialization

BOOL CVarecEnrollmentApp::InitInstance()
{
	BOOL bInit = COleControlModule::InitInstance();

	if (bInit)
	{
		// TODO: Add your own module initialization code here.
	}

	return bInit;
}



// CVarecEnrollmentApp::ExitInstance - DLL termination

int CVarecEnrollmentApp::ExitInstance()
{
	// TODO: Add your own module termination code here.

	return COleControlModule::ExitInstance();
}



// DllRegisterServer - Adds entries to the system registry

STDAPI DllRegisterServer(void)
{
	AFX_MANAGE_STATE(_afxModuleAddrThis);

	if (!AfxOleRegisterTypeLib(AfxGetInstanceHandle(), _tlid))
		return ResultFromScode(SELFREG_E_TYPELIB);

	if (!COleObjectFactoryEx::UpdateRegistryAll(TRUE))
		return ResultFromScode(SELFREG_E_CLASS);

	return NOERROR;
}



// DllUnregisterServer - Removes entries from the system registry

STDAPI DllUnregisterServer(void)
{
	AFX_MANAGE_STATE(_afxModuleAddrThis);

	if (!AfxOleUnregisterTypeLib(_tlid, _wVerMajor, _wVerMinor))
		return ResultFromScode(SELFREG_E_TYPELIB);

	if (!COleObjectFactoryEx::UpdateRegistryAll(FALSE))
		return ResultFromScode(SELFREG_E_CLASS);

	return NOERROR;
}
