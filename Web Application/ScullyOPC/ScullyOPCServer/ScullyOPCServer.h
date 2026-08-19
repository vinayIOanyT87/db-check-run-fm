

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for ScullyOPCServer.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 8.01.0622 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */



/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 500
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif /* __RPCNDR_H_VERSION__ */

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __ScullyOPCServer_h__
#define __ScullyOPCServer_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IScullys_FWD_DEFINED__
#define __IScullys_FWD_DEFINED__
typedef interface IScullys IScullys;

#endif 	/* __IScullys_FWD_DEFINED__ */


#ifndef __IDataAccess_FWD_DEFINED__
#define __IDataAccess_FWD_DEFINED__
typedef interface IDataAccess IDataAccess;

#endif 	/* __IDataAccess_FWD_DEFINED__ */


#ifndef __IPorts_FWD_DEFINED__
#define __IPorts_FWD_DEFINED__
typedef interface IPorts IPorts;

#endif 	/* __IPorts_FWD_DEFINED__ */


#ifndef __Scullys_FWD_DEFINED__
#define __Scullys_FWD_DEFINED__

#ifdef __cplusplus
typedef class Scullys Scullys;
#else
typedef struct Scullys Scullys;
#endif /* __cplusplus */

#endif 	/* __Scullys_FWD_DEFINED__ */


#ifndef __DataAccess_FWD_DEFINED__
#define __DataAccess_FWD_DEFINED__

#ifdef __cplusplus
typedef class DataAccess DataAccess;
#else
typedef struct DataAccess DataAccess;
#endif /* __cplusplus */

#endif 	/* __DataAccess_FWD_DEFINED__ */


#ifndef __Ports_FWD_DEFINED__
#define __Ports_FWD_DEFINED__

#ifdef __cplusplus
typedef class Ports Ports;
#else
typedef struct Ports Ports;
#endif /* __cplusplus */

#endif 	/* __Ports_FWD_DEFINED__ */


#ifndef __OPCServer_FWD_DEFINED__
#define __OPCServer_FWD_DEFINED__

#ifdef __cplusplus
typedef class OPCServer OPCServer;
#else
typedef struct OPCServer OPCServer;
#endif /* __cplusplus */

#endif 	/* __OPCServer_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "opcda.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_ScullyOPCServer_0000_0000 */
/* [local] */ 

#pragma warning(push)
#pragma warning(disable:4001) 
#pragma once
#pragma warning(push)
#pragma warning(disable:4001) 
#pragma once
#pragma warning(pop)
#pragma warning(pop)
#pragma region Desktop Family
#pragma endregion


extern RPC_IF_HANDLE __MIDL_itf_ScullyOPCServer_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ScullyOPCServer_0000_0000_v0_0_s_ifspec;

#ifndef __IScullys_INTERFACE_DEFINED__
#define __IScullys_INTERFACE_DEFINED__

/* interface IScullys */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IScullys;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3922F2C9-A5DB-4f67-A6A0-A840B733309C")
    IScullys : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Enumerate( 
            /* [retval][out] */ IDispatch **ppScullyCollection) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ IDispatch *pScully,
            /* [retval][out] */ LONG *pIndex) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Modify( 
            /* [in] */ IDispatch *pScully) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Purge( 
            /* [in] */ LONG lIndex) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetIndex( 
            /* [in] */ BSTR bstrID,
            /* [retval][out] */ LONG *pIndex) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Get( 
            /* [in] */ LONG lIndex,
            /* [retval][out] */ IDispatch **ppScully) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE EnumeratePortIDs( 
            /* [retval][out] */ VARIANT *pIDs) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IScullysVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IScullys * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IScullys * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IScullys * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IScullys * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IScullys * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IScullys * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IScullys * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Enumerate )( 
            IScullys * This,
            /* [retval][out] */ IDispatch **ppScullyCollection);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            IScullys * This,
            /* [in] */ IDispatch *pScully,
            /* [retval][out] */ LONG *pIndex);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Modify )( 
            IScullys * This,
            /* [in] */ IDispatch *pScully);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Purge )( 
            IScullys * This,
            /* [in] */ LONG lIndex);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetIndex )( 
            IScullys * This,
            /* [in] */ BSTR bstrID,
            /* [retval][out] */ LONG *pIndex);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Get )( 
            IScullys * This,
            /* [in] */ LONG lIndex,
            /* [retval][out] */ IDispatch **ppScully);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *EnumeratePortIDs )( 
            IScullys * This,
            /* [retval][out] */ VARIANT *pIDs);
        
        END_INTERFACE
    } IScullysVtbl;

    interface IScullys
    {
        CONST_VTBL struct IScullysVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IScullys_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IScullys_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IScullys_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IScullys_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IScullys_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IScullys_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IScullys_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IScullys_Enumerate(This,ppScullyCollection)	\
    ( (This)->lpVtbl -> Enumerate(This,ppScullyCollection) ) 

#define IScullys_Add(This,pScully,pIndex)	\
    ( (This)->lpVtbl -> Add(This,pScully,pIndex) ) 

#define IScullys_Modify(This,pScully)	\
    ( (This)->lpVtbl -> Modify(This,pScully) ) 

#define IScullys_Purge(This,lIndex)	\
    ( (This)->lpVtbl -> Purge(This,lIndex) ) 

#define IScullys_GetIndex(This,bstrID,pIndex)	\
    ( (This)->lpVtbl -> GetIndex(This,bstrID,pIndex) ) 

#define IScullys_Get(This,lIndex,ppScully)	\
    ( (This)->lpVtbl -> Get(This,lIndex,ppScully) ) 

#define IScullys_EnumeratePortIDs(This,pIDs)	\
    ( (This)->lpVtbl -> EnumeratePortIDs(This,pIDs) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IScullys_INTERFACE_DEFINED__ */


#ifndef __IDataAccess_INTERFACE_DEFINED__
#define __IDataAccess_INTERFACE_DEFINED__

/* interface IDataAccess */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IDataAccess;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C4C45BB0-49E0-4813-A87D-F6D2DE1D461C")
    IDataAccess : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetRecordSet( 
            BSTR bstrSQL,
            /* [retval][out] */ IDispatch **ppRecordSet) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ExecuteQuery( 
            BSTR bstrSQL) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDataAccessVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDataAccess * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDataAccess * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDataAccess * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IDataAccess * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IDataAccess * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IDataAccess * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IDataAccess * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetRecordSet )( 
            IDataAccess * This,
            BSTR bstrSQL,
            /* [retval][out] */ IDispatch **ppRecordSet);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *ExecuteQuery )( 
            IDataAccess * This,
            BSTR bstrSQL);
        
        END_INTERFACE
    } IDataAccessVtbl;

    interface IDataAccess
    {
        CONST_VTBL struct IDataAccessVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDataAccess_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDataAccess_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDataAccess_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDataAccess_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IDataAccess_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IDataAccess_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IDataAccess_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IDataAccess_GetRecordSet(This,bstrSQL,ppRecordSet)	\
    ( (This)->lpVtbl -> GetRecordSet(This,bstrSQL,ppRecordSet) ) 

#define IDataAccess_ExecuteQuery(This,bstrSQL)	\
    ( (This)->lpVtbl -> ExecuteQuery(This,bstrSQL) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDataAccess_INTERFACE_DEFINED__ */


#ifndef __IPorts_INTERFACE_DEFINED__
#define __IPorts_INTERFACE_DEFINED__

/* interface IPorts */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPorts;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D0E7E3F3-EEE7-440e-BA80-14D1835090A2")
    IPorts : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Enumerate( 
            /* [retval][out] */ IDispatch **ppCardReaderCollection) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ IDispatch *pCardReader,
            /* [retval][out] */ LONG *pIndex) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Modify( 
            /* [in] */ IDispatch *pCardReader) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Purge( 
            /* [in] */ LONG lIndex) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetIndex( 
            /* [in] */ BSTR bstrID,
            /* [retval][out] */ LONG *pIndex) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Get( 
            /* [in] */ LONG lIndex,
            /* [retval][out] */ IDispatch **ppCardReader) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE EnumeratePortIDs( 
            /* [retval][out] */ VARIANT *pIDs) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IPortsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPorts * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPorts * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPorts * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IPorts * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IPorts * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IPorts * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IPorts * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Enumerate )( 
            IPorts * This,
            /* [retval][out] */ IDispatch **ppCardReaderCollection);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            IPorts * This,
            /* [in] */ IDispatch *pCardReader,
            /* [retval][out] */ LONG *pIndex);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Modify )( 
            IPorts * This,
            /* [in] */ IDispatch *pCardReader);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Purge )( 
            IPorts * This,
            /* [in] */ LONG lIndex);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetIndex )( 
            IPorts * This,
            /* [in] */ BSTR bstrID,
            /* [retval][out] */ LONG *pIndex);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Get )( 
            IPorts * This,
            /* [in] */ LONG lIndex,
            /* [retval][out] */ IDispatch **ppCardReader);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *EnumeratePortIDs )( 
            IPorts * This,
            /* [retval][out] */ VARIANT *pIDs);
        
        END_INTERFACE
    } IPortsVtbl;

    interface IPorts
    {
        CONST_VTBL struct IPortsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPorts_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPorts_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPorts_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPorts_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IPorts_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IPorts_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IPorts_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IPorts_Enumerate(This,ppCardReaderCollection)	\
    ( (This)->lpVtbl -> Enumerate(This,ppCardReaderCollection) ) 

#define IPorts_Add(This,pCardReader,pIndex)	\
    ( (This)->lpVtbl -> Add(This,pCardReader,pIndex) ) 

#define IPorts_Modify(This,pCardReader)	\
    ( (This)->lpVtbl -> Modify(This,pCardReader) ) 

#define IPorts_Purge(This,lIndex)	\
    ( (This)->lpVtbl -> Purge(This,lIndex) ) 

#define IPorts_GetIndex(This,bstrID,pIndex)	\
    ( (This)->lpVtbl -> GetIndex(This,bstrID,pIndex) ) 

#define IPorts_Get(This,lIndex,ppCardReader)	\
    ( (This)->lpVtbl -> Get(This,lIndex,ppCardReader) ) 

#define IPorts_EnumeratePortIDs(This,pIDs)	\
    ( (This)->lpVtbl -> EnumeratePortIDs(This,pIDs) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPorts_INTERFACE_DEFINED__ */



#ifndef __ScullyOPCServerLib_LIBRARY_DEFINED__
#define __ScullyOPCServerLib_LIBRARY_DEFINED__

/* library ScullyOPCServerLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_ScullyOPCServerLib;

EXTERN_C const CLSID CLSID_Scullys;

#ifdef __cplusplus

class DECLSPEC_UUID("948DA86B-A687-494c-9B93-569B65499B36")
Scullys;
#endif

EXTERN_C const CLSID CLSID_DataAccess;

#ifdef __cplusplus

class DECLSPEC_UUID("1F341EE6-E351-4fae-BEDD-30A86A804B4E")
DataAccess;
#endif

EXTERN_C const CLSID CLSID_Ports;

#ifdef __cplusplus

class DECLSPEC_UUID("BF99140E-F916-49c2-9541-61BDD75E4531")
Ports;
#endif

EXTERN_C const CLSID CLSID_OPCServer;

#ifdef __cplusplus

class DECLSPEC_UUID("206D99CF-6189-4440-AB4C-74DAEBCFC8FE")
OPCServer;
#endif
#endif /* __ScullyOPCServerLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

unsigned long             __RPC_USER  BSTR_UserSize64(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal64(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal64(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree64(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  VARIANT_UserSize64(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal64(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal64(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree64(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


