

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* at Fri Jul 24 08:12:53 2020
 */
/* Compiler settings for FMInterfaces.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 8.00.0603 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __FMInterfaces_i_h__
#define __FMInterfaces_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __ICACComponents_FWD_DEFINED__
#define __ICACComponents_FWD_DEFINED__
typedef interface ICACComponents ICACComponents;

#endif 	/* __ICACComponents_FWD_DEFINED__ */


#ifndef __CACComponents_FWD_DEFINED__
#define __CACComponents_FWD_DEFINED__

#ifdef __cplusplus
typedef class CACComponents CACComponents;
#else
typedef struct CACComponents CACComponents;
#endif /* __cplusplus */

#endif 	/* __CACComponents_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __ICACComponents_INTERFACE_DEFINED__
#define __ICACComponents_INTERFACE_DEFINED__

/* interface ICACComponents */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_ICACComponents;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5DA37292-E616-4C44-A702-920934174258")
    ICACComponents : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE CACEnable( 
            /* [out] */ BSTR *szUserID,
            /* [retval][out] */ VARIANT_BOOL *bEnabled) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ICACComponentsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICACComponents * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICACComponents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICACComponents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ICACComponents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ICACComponents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ICACComponents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ICACComponents * This,
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
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *CACEnable )( 
            ICACComponents * This,
            /* [out] */ BSTR *szUserID,
            /* [retval][out] */ VARIANT_BOOL *bEnabled);
        
        END_INTERFACE
    } ICACComponentsVtbl;

    interface ICACComponents
    {
        CONST_VTBL struct ICACComponentsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICACComponents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICACComponents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICACComponents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICACComponents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ICACComponents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ICACComponents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ICACComponents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ICACComponents_CACEnable(This,szUserID,bEnabled)	\
    ( (This)->lpVtbl -> CACEnable(This,szUserID,bEnabled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICACComponents_INTERFACE_DEFINED__ */



#ifndef __FMInterfacesLib_LIBRARY_DEFINED__
#define __FMInterfacesLib_LIBRARY_DEFINED__

/* library FMInterfacesLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_FMInterfacesLib;

EXTERN_C const CLSID CLSID_CACComponents;

#ifdef __cplusplus

class DECLSPEC_UUID("4A1A2E0A-FD23-4384-90AB-EBCACB569B13")
CACComponents;
#endif
#endif /* __FMInterfacesLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


