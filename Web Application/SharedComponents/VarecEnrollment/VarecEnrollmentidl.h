

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for VarecEnrollment.idl:
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


#ifndef __VarecEnrollmentidl_h__
#define __VarecEnrollmentidl_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef ___DVarecEnrollment_FWD_DEFINED__
#define ___DVarecEnrollment_FWD_DEFINED__
typedef interface _DVarecEnrollment _DVarecEnrollment;

#endif 	/* ___DVarecEnrollment_FWD_DEFINED__ */


#ifndef ___DVarecEnrollmentEvents_FWD_DEFINED__
#define ___DVarecEnrollmentEvents_FWD_DEFINED__
typedef interface _DVarecEnrollmentEvents _DVarecEnrollmentEvents;

#endif 	/* ___DVarecEnrollmentEvents_FWD_DEFINED__ */


#ifndef __VarecEnrollment_FWD_DEFINED__
#define __VarecEnrollment_FWD_DEFINED__

#ifdef __cplusplus
typedef class VarecEnrollment VarecEnrollment;
#else
typedef struct VarecEnrollment VarecEnrollment;
#endif /* __cplusplus */

#endif 	/* __VarecEnrollment_FWD_DEFINED__ */


#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_VarecEnrollment_0000_0000 */
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


extern RPC_IF_HANDLE __MIDL_itf_VarecEnrollment_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_VarecEnrollment_0000_0000_v0_0_s_ifspec;


#ifndef __VarecEnrollmentLib_LIBRARY_DEFINED__
#define __VarecEnrollmentLib_LIBRARY_DEFINED__

/* library VarecEnrollmentLib */
/* [control][helpstring][helpfile][version][uuid] */ 


EXTERN_C const IID LIBID_VarecEnrollmentLib;

#ifndef ___DVarecEnrollment_DISPINTERFACE_DEFINED__
#define ___DVarecEnrollment_DISPINTERFACE_DEFINED__

/* dispinterface _DVarecEnrollment */
/* [helpstring][uuid] */ 


EXTERN_C const IID DIID__DVarecEnrollment;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("AF61B8BF-B180-49F8-AD3E-957D666C11E5")
    _DVarecEnrollment : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _DVarecEnrollmentVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _DVarecEnrollment * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _DVarecEnrollment * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _DVarecEnrollment * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _DVarecEnrollment * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _DVarecEnrollment * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _DVarecEnrollment * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _DVarecEnrollment * This,
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
        
        END_INTERFACE
    } _DVarecEnrollmentVtbl;

    interface _DVarecEnrollment
    {
        CONST_VTBL struct _DVarecEnrollmentVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _DVarecEnrollment_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _DVarecEnrollment_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _DVarecEnrollment_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _DVarecEnrollment_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _DVarecEnrollment_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _DVarecEnrollment_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _DVarecEnrollment_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___DVarecEnrollment_DISPINTERFACE_DEFINED__ */


#ifndef ___DVarecEnrollmentEvents_DISPINTERFACE_DEFINED__
#define ___DVarecEnrollmentEvents_DISPINTERFACE_DEFINED__

/* dispinterface _DVarecEnrollmentEvents */
/* [helpstring][uuid] */ 


EXTERN_C const IID DIID__DVarecEnrollmentEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("EDE76D49-783F-4AAD-953D-F4CFCF3C6F76")
    _DVarecEnrollmentEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _DVarecEnrollmentEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _DVarecEnrollmentEvents * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _DVarecEnrollmentEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _DVarecEnrollmentEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _DVarecEnrollmentEvents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _DVarecEnrollmentEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _DVarecEnrollmentEvents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _DVarecEnrollmentEvents * This,
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
        
        END_INTERFACE
    } _DVarecEnrollmentEventsVtbl;

    interface _DVarecEnrollmentEvents
    {
        CONST_VTBL struct _DVarecEnrollmentEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _DVarecEnrollmentEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _DVarecEnrollmentEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _DVarecEnrollmentEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _DVarecEnrollmentEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _DVarecEnrollmentEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _DVarecEnrollmentEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _DVarecEnrollmentEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___DVarecEnrollmentEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_VarecEnrollment;

#ifdef __cplusplus

class DECLSPEC_UUID("C6AD5C3A-DB26-450A-82C7-890D2D23A8D9")
VarecEnrollment;
#endif
#endif /* __VarecEnrollmentLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


