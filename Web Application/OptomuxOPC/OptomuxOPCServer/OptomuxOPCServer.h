

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for OptomuxOPCServer.idl:
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

#ifndef __OptomuxOPCServer_h__
#define __OptomuxOPCServer_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDataAccess_FWD_DEFINED__
#define __IDataAccess_FWD_DEFINED__
typedef interface IDataAccess IDataAccess;

#endif 	/* __IDataAccess_FWD_DEFINED__ */


#ifndef __IOptomuxControllers_FWD_DEFINED__
#define __IOptomuxControllers_FWD_DEFINED__
typedef interface IOptomuxControllers IOptomuxControllers;

#endif 	/* __IOptomuxControllers_FWD_DEFINED__ */


#ifndef __IPorts_FWD_DEFINED__
#define __IPorts_FWD_DEFINED__
typedef interface IPorts IPorts;

#endif 	/* __IPorts_FWD_DEFINED__ */


#ifndef __DataAccess_FWD_DEFINED__
#define __DataAccess_FWD_DEFINED__

#ifdef __cplusplus
typedef class DataAccess DataAccess;
#else
typedef struct DataAccess DataAccess;
#endif /* __cplusplus */

#endif 	/* __DataAccess_FWD_DEFINED__ */


#ifndef __OptomuxControllers_FWD_DEFINED__
#define __OptomuxControllers_FWD_DEFINED__

#ifdef __cplusplus
typedef class OptomuxControllers OptomuxControllers;
#else
typedef struct OptomuxControllers OptomuxControllers;
#endif /* __cplusplus */

#endif 	/* __OptomuxControllers_FWD_DEFINED__ */


#ifndef __OPCServer_FWD_DEFINED__
#define __OPCServer_FWD_DEFINED__

#ifdef __cplusplus
typedef class OPCServer OPCServer;
#else
typedef struct OPCServer OPCServer;
#endif /* __cplusplus */

#endif 	/* __OPCServer_FWD_DEFINED__ */


#ifndef __Ports_FWD_DEFINED__
#define __Ports_FWD_DEFINED__

#ifdef __cplusplus
typedef class Ports Ports;
#else
typedef struct Ports Ports;
#endif /* __cplusplus */

#endif 	/* __Ports_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "opcda.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_OptomuxOPCServer_0000_0000 */
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


extern RPC_IF_HANDLE __MIDL_itf_OptomuxOPCServer_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_OptomuxOPCServer_0000_0000_v0_0_s_ifspec;

#ifndef __IDataAccess_INTERFACE_DEFINED__
#define __IDataAccess_INTERFACE_DEFINED__

/* interface IDataAccess */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IDataAccess;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4ECBB0F4-FFAF-4B3A-8566-439D4C236CF0")
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


#ifndef __IOptomuxControllers_INTERFACE_DEFINED__
#define __IOptomuxControllers_INTERFACE_DEFINED__

/* interface IOptomuxControllers */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IOptomuxControllers;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BFB066AF-2312-4C35-9251-580B45AACB95")
    IOptomuxControllers : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Enumerate( 
            /* [retval][out] */ IDispatch **ppOptomuxControllerCollection) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ IDispatch *pOptomuxController,
            /* [retval][out] */ LONG *pIndex) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Modify( 
            /* [in] */ IDispatch *pOptomuxController) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Purge( 
            /* [in] */ LONG lIndex) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetIndex( 
            /* [in] */ BSTR bstrID,
            /* [retval][out] */ LONG *pIndex) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Get( 
            /* [in] */ LONG lIndex,
            /* [retval][out] */ IDispatch **ppOptomuxController) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IOptomuxControllersVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOptomuxControllers * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOptomuxControllers * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOptomuxControllers * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IOptomuxControllers * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IOptomuxControllers * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IOptomuxControllers * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IOptomuxControllers * This,
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
            IOptomuxControllers * This,
            /* [retval][out] */ IDispatch **ppOptomuxControllerCollection);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            IOptomuxControllers * This,
            /* [in] */ IDispatch *pOptomuxController,
            /* [retval][out] */ LONG *pIndex);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Modify )( 
            IOptomuxControllers * This,
            /* [in] */ IDispatch *pOptomuxController);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Purge )( 
            IOptomuxControllers * This,
            /* [in] */ LONG lIndex);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetIndex )( 
            IOptomuxControllers * This,
            /* [in] */ BSTR bstrID,
            /* [retval][out] */ LONG *pIndex);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Get )( 
            IOptomuxControllers * This,
            /* [in] */ LONG lIndex,
            /* [retval][out] */ IDispatch **ppOptomuxController);
        
        END_INTERFACE
    } IOptomuxControllersVtbl;

    interface IOptomuxControllers
    {
        CONST_VTBL struct IOptomuxControllersVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOptomuxControllers_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOptomuxControllers_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOptomuxControllers_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOptomuxControllers_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IOptomuxControllers_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IOptomuxControllers_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IOptomuxControllers_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IOptomuxControllers_Enumerate(This,ppOptomuxControllerCollection)	\
    ( (This)->lpVtbl -> Enumerate(This,ppOptomuxControllerCollection) ) 

#define IOptomuxControllers_Add(This,pOptomuxController,pIndex)	\
    ( (This)->lpVtbl -> Add(This,pOptomuxController,pIndex) ) 

#define IOptomuxControllers_Modify(This,pOptomuxController)	\
    ( (This)->lpVtbl -> Modify(This,pOptomuxController) ) 

#define IOptomuxControllers_Purge(This,lIndex)	\
    ( (This)->lpVtbl -> Purge(This,lIndex) ) 

#define IOptomuxControllers_GetIndex(This,bstrID,pIndex)	\
    ( (This)->lpVtbl -> GetIndex(This,bstrID,pIndex) ) 

#define IOptomuxControllers_Get(This,lIndex,ppOptomuxController)	\
    ( (This)->lpVtbl -> Get(This,lIndex,ppOptomuxController) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOptomuxControllers_INTERFACE_DEFINED__ */


#ifndef __IPorts_INTERFACE_DEFINED__
#define __IPorts_INTERFACE_DEFINED__

/* interface IPorts */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPorts;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F0FDD2C4-A437-41F2-B113-E4B2EFF9B5E8")
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



#ifndef __OptomuxOPCServerLib_LIBRARY_DEFINED__
#define __OptomuxOPCServerLib_LIBRARY_DEFINED__

/* library OptomuxOPCServerLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_OptomuxOPCServerLib;

EXTERN_C const CLSID CLSID_DataAccess;

#ifdef __cplusplus

class DECLSPEC_UUID("6CAE3271-EFA1-4CB6-AFE9-2417144D2E8B")
DataAccess;
#endif

EXTERN_C const CLSID CLSID_OptomuxControllers;

#ifdef __cplusplus

class DECLSPEC_UUID("DD940B4F-C212-4361-8FDE-D4061584E4D0")
OptomuxControllers;
#endif

EXTERN_C const CLSID CLSID_OPCServer;

#ifdef __cplusplus

class DECLSPEC_UUID("DC07462E-EDCD-4186-9FD0-B8B7F990B490")
OPCServer;
#endif

EXTERN_C const CLSID CLSID_Ports;

#ifdef __cplusplus

class DECLSPEC_UUID("D1CAA238-8AB9-4E70-A628-49AB61EC5BD1")
Ports;
#endif
#endif /* __OptomuxOPCServerLib_LIBRARY_DEFINED__ */

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


