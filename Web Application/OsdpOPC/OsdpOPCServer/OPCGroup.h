/******************************************************************************

	FILE NAME:		OPCGroup.h


	PURPOSE:			Declaration of COPCGroup

*******************************************************************************/


#ifndef __OPCGROUP_H__              // Only Include Once
#define __OPCGROUP_H__

#include "OPCGroupBase.h"
#include "IO.h"
#include "resource.h"

class COPCServer;
class CEnumItemAttributes;


class COsdpControllerItem : public COPCItem
{
public:

   COsdpControllerItem(CTag* pTag,LPTSTR szTag)
	{
		m_pTag=pTag;
		m_oTag=szTag;
		m_bChanged=FALSE;
		m_dwUpdateSequence=0;
	};
   ~COsdpControllerItem()
	{
		if(m_pTag
		&& m_bActive
		&& m_pTag->m_dwAccessRights & OPC_READABLE
		&& m_pTag->m_pIO)
			m_pTag->m_pIO->RemoveTagFromScanList(m_pTag);
	};

	CTag*		m_pTag;
	CString	m_oTag;
	DWORD		m_dwUpdateSequence;
};

//*******************************************************************
class ATL_NO_VTABLE COPCGroup :
   public OPCGroupBase
{
   friend CEnumItemAttributes;
	friend class COsdpControllerManager;
	friend class COPCLock;
public:
   COPCGroup();
   virtual ~COPCGroup();


   // Overrides
   virtual void DoSetUpdateRate( DWORD newUpdateRate );
   virtual HRESULT DoRead(
                              OPCDATASOURCE    dwSource,
                              DWORD            dwNumItems,
                              COPCItem      ** ppItems,
                              OPCITEMSTATE   * pItemValues,
                              HRESULT        * pErrors);
   virtual HRESULT DoWrite(
                              DWORD            dwNumItems,
                              COPCItem      ** ppItems,
                              VARIANT        * pItemValues,
                              HRESULT        * pErrors);
   virtual BOOL DoUpdateGroup();
   virtual HRESULT DoAddItems(
                              DWORD            dwNumItems,
                              OPCITEMDEF     * pItemArray,
                              OPCITEMRESULT  * pAddResults,
                              HRESULT        * pErrors);
   virtual HRESULT DoValidateItems(
                              DWORD            dwNumItems,
                              OPCITEMDEF     * pItemArray,
                              OPCITEMRESULT  * pValidationResults,
                              HRESULT        * pErrors);
   virtual HRESULT DoRemoveItems(
                              DWORD        dwNumItems,
                              COPCItem  ** ppItems,
                              HRESULT    * pErrors);
   virtual HRESULT DoSetActiveState(
                              DWORD        dwNumItems,
                              COPCItem  ** ppItems,
                              BOOL         bActive,
                              HRESULT    * pErrors);
   virtual HRESULT DoSetClientHandles(
                              DWORD        dwNumItems,
                              COPCItem  ** ppItems,
                              OPCHANDLE  * phClient,
                              HRESULT    * pErrors);
   virtual HRESULT DoSetDatatypes(
                              DWORD        dwNumItems,
                              COPCItem  ** ppItems,
                              VARTYPE    * pRequestedDatatypes,
                              HRESULT    * pErrors);

	virtual HRESULT	DoCopyItems(
										LPCWSTR	szName);

   virtual IUnknown* DoCreateEnumerator();

protected:

   // General attributes
	DWORD					m_dwGroupID;   // unique group ID, gotten from document, used w/subsciptions
};

typedef CComObject<COPCGroup> OPCGroupObject;
typedef CTypedPtrMap<CMapPtrToPtr, LPVOID, OPCGroupObject*> GroupMap;

//*******************************************************************
// The group's item attributes enumerator object
class CEnumItemAttributes :
   public IEnumOPCItemAttributes,
   public CComObjectRoot
{
   friend class CEnumItemAttributes;
public:
   CEnumItemAttributes();
   ~CEnumItemAttributes();
   void  Initialize(COPCGroup* pGroup);

BEGIN_COM_MAP(CEnumItemAttributes)
   COM_INTERFACE_ENTRY(IEnumOPCItemAttributes)
END_COM_MAP()

  // Enumerator
  STDMETHODIMP Next(
                ULONG celt,
                OPCITEMATTRIBUTES ** ppItemArray,
                ULONG * pceltFetched );

  STDMETHODIMP Skip( ULONG celt );

  STDMETHODIMP Reset( void );

  STDMETHODIMP Clone( IEnumOPCItemAttributes ** ppEnumItemAttributes );


private:
   POSITION					m_pos;  // iterator for any of the things we may enum (devices or tags)
   COPCGroup*	m_parent;
};

typedef CComObject<CEnumItemAttributes> CComEnumItemAttributes;

#endif