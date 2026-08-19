// VarecEnrollmentPropPage.cpp : Implementation of the CVarecEnrollmentPropPage property page class.

#include "stdafx.h"
#include "VarecEnrollment.h"
#include "VarecEnrollmentPropPage.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


IMPLEMENT_DYNCREATE(CVarecEnrollmentPropPage, COlePropertyPage)



// Message map

BEGIN_MESSAGE_MAP(CVarecEnrollmentPropPage, COlePropertyPage)
END_MESSAGE_MAP()



// Initialize class factory and guid

IMPLEMENT_OLECREATE_EX(CVarecEnrollmentPropPage, "VARECENROLLMEN.VarecEnrollmenPropPage.1",
	0x20a4479b, 0xf3e0, 0x4436, 0x98, 0x60, 0xb5, 0x37, 0xe5, 0xb1, 0xb6, 0x91)



// CVarecEnrollmentPropPage::CVarecEnrollmentPropPageFactory::UpdateRegistry -
// Adds or removes system registry entries for CVarecEnrollmentPropPage

BOOL CVarecEnrollmentPropPage::CVarecEnrollmentPropPageFactory::UpdateRegistry(BOOL bRegister)
{
	if (bRegister)
		return AfxOleRegisterPropertyPageClass(AfxGetInstanceHandle(),
			m_clsid, IDS_VARECENROLLMENT_PPG);
	else
		return AfxOleUnregisterClass(m_clsid, NULL);
}



// CVarecEnrollmentPropPage::CVarecEnrollmentPropPage - Constructor

CVarecEnrollmentPropPage::CVarecEnrollmentPropPage() :
	COlePropertyPage(IDD, IDS_VARECENROLLMENT_PPG_CAPTION)
{
}



// CVarecEnrollmentPropPage::DoDataExchange - Moves data between page and properties

void CVarecEnrollmentPropPage::DoDataExchange(CDataExchange* pDX)
{
	DDP_PostProcessing(pDX);
}



// CVarecEnrollmentPropPage message handlers
