#pragma once

// VarecEnrollmentPropPage.h : Declaration of the CVarecEnrollmentPropPage property page class.


// CVarecEnrollmentPropPage : See VarecEnrollmentPropPage.cpp for implementation.

class CVarecEnrollmentPropPage : public COlePropertyPage
{
	DECLARE_DYNCREATE(CVarecEnrollmentPropPage)
	DECLARE_OLECREATE_EX(CVarecEnrollmentPropPage)

// Constructor
public:
	CVarecEnrollmentPropPage();

// Dialog Data
	enum { IDD = IDD_PROPPAGE_VARECENROLLMENT };

// Implementation
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

// Message maps
protected:
	DECLARE_MESSAGE_MAP()
};

