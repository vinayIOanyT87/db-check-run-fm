/******************************************************************************

	FILE NAME:		OPCServerBase.h


	PURPOSE:			Declaration of COPCServerBase

*******************************************************************************/




#ifndef __OPCSERVERBASE_H__             // Only Include Once
#define __OPCSERVERBASE_H__

#pragma warning( disable : 4786 )

#include "OPCError.h"

extern UINT OPCSTMFORMATDATA;
extern UINT OPCSTMFORMATDATATIME;
extern UINT OPCSTMFORMATWRITECOMPLETE;

extern LPWSTR empty;
extern CComBSTR emptyString;

//*******************************************************************
#include "OPCGroup.h"

// The "standard" OPC pattern match function (used in browsing)
BOOL MatchPattern( LPCTSTR String, LPCTSTR Pattern, BOOL bCaseSensitive );

class CEnumGroupNames;
class CEnumGroupIUnknown;

//*******************************************************************
class ATL_NO_VTABLE OPCServerBase :
   public IOPCServer,
   public IOPCCommon,
   public IOPCBrowseServerAddressSpace,
   public IOPCItemProperties,
   public IConnectionPointContainerImpl<OPCServerBase>,
   public IConnectionPointImpl<OPCServerBase, &IID_IOPCShutdown, CComDynamicUnkArray>,
   public CComObjectRootEx<CComMultiThreadModel>,
   public CComCoClass<OPCServerBase,&CLSID_OPCServer>
{

friend class CEnumGroupNames;
friend class CEnumGroupIUnknown;

public:
                   OPCServerBase();
   virtual         ~OPCServerBase();
   OPCGroupObject* FindNamedGroup( LPCWSTR name );
   void            UpdateTime();
   STDMETHODIMP    UpdateClients();
   void            ServerShutdown( LPTSTR reason );


BEGIN_COM_MAP(OPCServerBase)
   COM_INTERFACE_ENTRY(IOPCServer)
   COM_INTERFACE_ENTRY(IOPCCommon)
   COM_INTERFACE_ENTRY(IOPCBrowseServerAddressSpace)
   COM_INTERFACE_ENTRY(IOPCItemProperties)
   COM_INTERFACE_ENTRY_IMPL(IConnectionPointContainer)
END_COM_MAP()

DECLARE_NOT_AGGREGATABLE(COPCServer)

   // Connection Point
BEGIN_CONNECTION_POINT_MAP(OPCServerBase)
   CONNECTION_POINT_ENTRY(IID_IOPCShutdown)
END_CONNECTION_POINT_MAP()

   // IOPCServer
   STDMETHODIMP AddGroup(
      LPCWSTR     szName,
      BOOL        bActive,
      DWORD       dwRequestedUpdateRate,
      OPCHANDLE   hClientGroup,
      LONG      * pTimeBias,
      FLOAT     * pPercentDeadband,
      DWORD       dwLCID,
      OPCHANDLE * phServerGroup,
      DWORD     * pRevisedUpdateRate,
      REFIID      riid,
      LPUNKNOWN * ppUnk);

   STDMETHODIMP GetErrorString(
      HRESULT     dwError,
      LCID        dwLocale,
      LPWSTR    * ppString);

   STDMETHODIMP GetGroupByName(
      LPCWSTR     szName,
      REFIID      riid,
      LPUNKNOWN * ppUnk);

   STDMETHODIMP GetStatus(
      OPCSERVERSTATUS **ppServerStatus);

   STDMETHODIMP RemoveGroup(
      OPCHANDLE   hServerGroup,
      BOOL     bForce);

   STDMETHODIMP CreateGroupEnumerator(
      OPCENUMSCOPE dwScope,
      REFIID      riid,
      LPUNKNOWN * ppUnk);

   // IOPCCommon
   STDMETHODIMP SetLocaleID(LCID dwLcid);

   STDMETHODIMP GetLocaleID(LCID * pdwLcid);

   STDMETHODIMP QueryAvailableLocaleIDs(
      DWORD          * pdwCount,
      LCID          ** pdwLcid);

   STDMETHODIMP GetErrorString(
      HRESULT          dwError,
      LPWSTR         * ppString);

   STDMETHODIMP SetClientName(LPCWSTR szName);

   // IOPCBrowseServerAddressSpace
   STDMETHODIMP QueryOrganization(
      OPCNAMESPACETYPE * pNameSpaceType);

   STDMETHODIMP ChangeBrowsePosition(
      OPCBROWSEDIRECTION dwBrowseDirection,
      LPCWSTR           szString);

   STDMETHODIMP BrowseOPCItemIDs(
      OPCBROWSETYPE     dwBrowseFilterType,
      LPCWSTR           szFilterCriteria,
      VARTYPE           vtDataTypeFilter,
      DWORD             dwAccessRightsFilter,
      LPENUMSTRING *    ppIEnumString);

   STDMETHODIMP GetItemID(
      LPWSTR      szItemDataID,
      LPWSTR *    szItemID);

   STDMETHODIMP BrowseAccessPaths(
      LPCWSTR        szItemID,
      LPENUMSTRING * ppIEnumString);

   // IOPCItemProperties
   STDMETHODIMP  QueryAvailableProperties(
      LPWSTR      szItemID,
      DWORD     * pdwCount,
      DWORD    ** ppPropertyIDs,
      LPWSTR   ** ppDescriptions,
      VARTYPE  ** ppvtDataTypes);

   STDMETHODIMP  GetItemProperties(
      LPWSTR      szItemID,
      DWORD       dwCount,
      DWORD     * pdwPropertyIDs,
      VARIANT  ** ppvData,
      HRESULT  ** ppErrors);

   STDMETHODIMP  LookupItemIDs(
      LPWSTR      szItemID,
      DWORD       dwCount,
      DWORD     * pdwPropertyIDs,
      LPWSTR   ** ppszNewItemIDs,
      HRESULT  ** ppErrors);

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
                              VARIANT   * pData,
                              HRESULT   * pErrors);
   virtual HRESULT DoLookupItemIDs(
                              LPWSTR      szItemID,
                              DWORD       dwNumItems,
                              DWORD     * pPropertyIDs,
                              LPWSTR    * pszNewItemIDs,
                              HRESULT   * pErrors);
   virtual HRESULT DoGetStatus( OPCSERVERSTATUS *pServerStatus);
   virtual HRESULT DoGetErrorString(
                              HRESULT     dwError,
                              LCID        dwLocale,
                              LPWSTR      pString);
   virtual OPCGroupObject* DoAddGroup(
                              LPCWSTR     szName,
                              BOOL        bActive,
                              DWORD       dwRequestedUpdateRate,
                              OPCHANDLE   hClientGroup,
                              LONG      * pTimeBias,
                              FLOAT     * pPercentDeadband,
                              DWORD       dwLCID,
                              OPCHANDLE * phServerGroup,
                              DWORD     * pRevisedUpdateRate);
   virtual OPCNAMESPACETYPE DoQueryOrganization()=0;
   virtual HRESULT DoChangeBrowsePosition(
                              OPCBROWSEDIRECTION dwBrowseDirection,
                              LPCWSTR           szString)=0;
   virtual HRESULT DoBrowseOPCItemIDs(
                              OPCBROWSETYPE     dwBrowseFilterType,
                              LPCWSTR           szFilterCriteria,
                              VARTYPE           vtDataTypeFilter,
                              DWORD             dwAccessRightsFilter,
                              LPENUMSTRING *    ppIEnumString)=0;
   virtual HRESULT DoGetItemID(
                              LPWSTR      szItemDataID,
                              LPWSTR *    szItemID)=0;

protected:
   CRITICAL_SECTION  m_cs;
   GroupMap				m_groupMap;
   FILETIME				m_lastUpdateTime;
   LCID					m_localeID;
   CString				m_client;
};

//*******************************************************************
// A string enumerator for Groups
class CEnumGroupNames :
   public IEnumString,
   public CComObjectRoot
{
public:
   CEnumGroupNames();
   ~CEnumGroupNames();
   void  Initialize(OPCServerBase* pServer);

BEGIN_COM_MAP(CEnumGroupNames)
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
	OPCServerBase*		m_pServer;
};

typedef CComObject<CEnumGroupNames> CComEnumGroupNames;

//*******************************************************************
// A IUnknown enumerator for Groups
class CEnumGroupIUnknown :
   public IEnumUnknown,
   public CComObjectRoot
{
public:
   CEnumGroupIUnknown();
   ~CEnumGroupIUnknown();
   void  Initialize(	OPCServerBase*		pServer);

BEGIN_COM_MAP(CEnumGroupIUnknown)
   COM_INTERFACE_ENTRY(IEnumUnknown)
END_COM_MAP()

   STDMETHODIMP Next( ULONG celt,
                      IUnknown** ppIUnknown,
                      ULONG * pceltFetched );

   STDMETHODIMP Skip( ULONG celt );

   STDMETHODIMP Reset( void );

   STDMETHODIMP Clone( IEnumUnknown** ppEnumIUnknown );

private:
   POSITION          m_pos;
	OPCServerBase*		m_pServer;
};

typedef CComObject<CEnumGroupIUnknown> CComEnumGroupIUnknown;

#endif