// OPCServer.h : Declaration of the COPCServer

#pragma once
#include "resource.h"       // main symbols
#include "OPCServerBase.h"
#define __GUID_DEFINED__
#include "opcda_cats.c"

//*******************************************************************
// A string enumerator for ItemIDs - used for browsing
class CEnumItemIDs :
   public IEnumString,
   public CComObjectRoot
{
public:
   CEnumItemIDs();
   ~CEnumItemIDs();
   void  Initialize( CTag*					pTag,
                     OPCBROWSETYPE     dwFilterType,
                     LPCWSTR           szCriteria,
                     VARTYPE           vtTypeFilter,
                     DWORD             dwRightsFilter);

BEGIN_COM_MAP(CEnumItemIDs)
   COM_INTERFACE_ENTRY(IEnumString)
END_COM_MAP()

   STDMETHODIMP Next( ULONG celt,
                      LPOLESTR * ppStrings,
                      ULONG * pceltFetched );

   STDMETHODIMP Skip( ULONG celt );

   STDMETHODIMP Reset( void );

   STDMETHODIMP Clone( IEnumString ** ppEnumString );

private:
   POSITION          m_pos;
   CTag*					m_pCurrentTag;

   OPCBROWSETYPE     m_BrowseFilterType;
   CString           m_oFilterCriteria;
   VARTYPE           m_DataTypeFilter;
   DWORD             m_dwAccessRightsFilter;

   // When Browsing FLAT
   CStringList      m_Paths;
   void  AddTags( CTag* pTag );
};

typedef CComObject<CEnumItemIDs> CComEnumItemIDs;


class ATL_NO_VTABLE COPCServer : 
   public OPCServerBase
{
friend class CDeviceManager;
friend class COPCLock;

public:
	COPCServer()
	{
		m_pCurrentTag=NULL;
	}

DECLARE_REGISTRY_RESOURCEID(IDR_OPCSERVER)

	BEGIN_CATEGORY_MAP(OPCServer)
		IMPLEMENTED_CATEGORY(CATID_OPCDAServer10)
		IMPLEMENTED_CATEGORY(CATID_OPCDAServer20)
	END_CATEGORY_MAP()


	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct();
	void FinalRelease(); 

public:
   // Overrides
   virtual HRESULT DoQueryNumProperties(
                              LPWSTR      szItemID,
                              DWORD     * pdwNumItems,
                              LPVOID    * ppVoid);
   virtual HRESULT DoQueryAvailableProperties(
                              LPWSTR      szItemID,
                              DWORD       dwNumItems,
                              LPVOID      pVoid,
                              DWORD     * pPropertyIDs,
                              LPWSTR    * pDescriptions,
                              VARTYPE   * pvtDataTypes);
   virtual HRESULT DoGetItemProperties(
                              LPWSTR      szItemID,
                              DWORD       dwNumItems,
                              DWORD     * pPropertyIDs,
                              VARIANT   * ppvData,
                              HRESULT   * ppErrors);
	virtual HRESULT DoLookupItemIDs(
                              LPWSTR      szItemID,
                              DWORD       dwNumItems,
                              DWORD     * pPropertyIDs,
                              LPWSTR    * pszNewItemIDs,
                              HRESULT   * pErrors);
   virtual HRESULT DoGetStatus( OPCSERVERSTATUS *pServerStatus);
   virtual OPCNAMESPACETYPE DoQueryOrganization();
   virtual HRESULT DoChangeBrowsePosition(
                              OPCBROWSEDIRECTION dwBrowseDirection,
                              LPCWSTR           szString);
   virtual HRESULT DoBrowseOPCItemIDs(
                              OPCBROWSETYPE     dwBrowseFilterType,
                              LPCWSTR           szFilterCriteria,
                              VARTYPE           vtDataTypeFilter,
                              DWORD             dwAccessRightsFilter,
                              LPENUMSTRING *    ppIEnumString);
   virtual HRESULT DoGetItemID(
                              LPWSTR      szItemDataID,
                              LPWSTR *    szItemID);


	protected:
	CTag*					m_pCurrentTag;
};

typedef CTypedPtrList<CPtrList,COPCServer*> COPCServerList;

OBJECT_ENTRY_AUTO(__uuidof(OPCServer), COPCServer)
