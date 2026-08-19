

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for OptomuxOPCObjects.idl:
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

#ifndef __OptomuxOPCObjects_h__
#define __OptomuxOPCObjects_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IOptomuxController_FWD_DEFINED__
#define __IOptomuxController_FWD_DEFINED__
typedef interface IOptomuxController IOptomuxController;

#endif 	/* __IOptomuxController_FWD_DEFINED__ */


#ifndef __IOptomuxControllerCollection_FWD_DEFINED__
#define __IOptomuxControllerCollection_FWD_DEFINED__
typedef interface IOptomuxControllerCollection IOptomuxControllerCollection;

#endif 	/* __IOptomuxControllerCollection_FWD_DEFINED__ */


#ifndef __IPort_FWD_DEFINED__
#define __IPort_FWD_DEFINED__
typedef interface IPort IPort;

#endif 	/* __IPort_FWD_DEFINED__ */


#ifndef __IPortCollection_FWD_DEFINED__
#define __IPortCollection_FWD_DEFINED__
typedef interface IPortCollection IPortCollection;

#endif 	/* __IPortCollection_FWD_DEFINED__ */


#ifndef __OptomuxController_FWD_DEFINED__
#define __OptomuxController_FWD_DEFINED__

#ifdef __cplusplus
typedef class OptomuxController OptomuxController;
#else
typedef struct OptomuxController OptomuxController;
#endif /* __cplusplus */

#endif 	/* __OptomuxController_FWD_DEFINED__ */


#ifndef __OptomuxControllerCollection_FWD_DEFINED__
#define __OptomuxControllerCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class OptomuxControllerCollection OptomuxControllerCollection;
#else
typedef struct OptomuxControllerCollection OptomuxControllerCollection;
#endif /* __cplusplus */

#endif 	/* __OptomuxControllerCollection_FWD_DEFINED__ */


#ifndef __Port_FWD_DEFINED__
#define __Port_FWD_DEFINED__

#ifdef __cplusplus
typedef class Port Port;
#else
typedef struct Port Port;
#endif /* __cplusplus */

#endif 	/* __Port_FWD_DEFINED__ */


#ifndef __PortCollection_FWD_DEFINED__
#define __PortCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class PortCollection PortCollection;
#else
typedef struct PortCollection PortCollection;
#endif /* __cplusplus */

#endif 	/* __PortCollection_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_OptomuxOPCObjects_0000_0000 */
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
typedef 
enum OPTOMUX_TYPE
    {
        PASSCONTROLLER_HC05	= 0,
        PASSCONTROLLER_HC12	= 1,
        VAREC_DET	= 2,
        MAX_OPTOMUX_TYPE	= 3
    } 	OPTOMUX_TYPE;

typedef 
enum OPTOMUX_BAUD
    {
        OPTOMUX_BAUD_1200	= 0,
        OPTOMUX_BAUD_2400	= 1,
        OPTOMUX_BAUD_4800	= 2,
        OPTOMUX_BAUD_9600	= 3,
        OPTOMUX_BAUD_19200	= 4,
        OPTOMUX_BAUD_38400	= 5,
        MAX_OPTOMUX_BAUD	= 6
    } 	OPTOMUX_BAUD;

typedef 
enum OPTOMUX_PARITY
    {
        OPTOMUX_PARITY_NONE	= 0,
        OPTOMUX_PARITY_EVEN	= 1,
        OPTOMUX_PARITY_ODD	= 2,
        MAX_OPTOMUX_PARITY	= 3
    } 	OPTOMUX_PARITY;

typedef 
enum OPTOMUX_DATA_BITS
    {
        DATA_BITS_7	= 0,
        DATA_BITS_8	= 1,
        MAX_OPTOMUX_DATA_BITS	= 2
    } 	OPTOMUX_DATA_BITS;

typedef 
enum OPTOMUX_STOP_BITS
    {
        STOP_BITS_1	= 0,
        STOP_BITS_2	= 1,
        MAX_OPTOMUX_STOP_BITS	= 2
    } 	OPTOMUX_STOP_BITS;



extern RPC_IF_HANDLE __MIDL_itf_OptomuxOPCObjects_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_OptomuxOPCObjects_0000_0000_v0_0_s_ifspec;

#ifndef __IOptomuxController_INTERFACE_DEFINED__
#define __IOptomuxController_INTERFACE_DEFINED__

/* interface IOptomuxController */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IOptomuxController;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B76EC008-879B-40C8-8019-7121320E1167")
    IOptomuxController : public IDispatch
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Index( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Index( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ID( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ID( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Type( 
            /* [retval][out] */ OPTOMUX_TYPE *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Type( 
            /* [in] */ OPTOMUX_TYPE newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Address( 
            /* [retval][out] */ BYTE *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Address( 
            /* [in] */ BYTE newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PortIndex( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PortIndex( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ModuleInputOutputMap( 
            /* [retval][out] */ BYTE *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ModuleInputOutputMap( 
            /* [in] */ BYTE newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_NetworkCommunications( 
            /* [retval][out] */ VARIANT_BOOL *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_NetworkCommunications( 
            /* [in] */ VARIANT_BOOL newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_IPAddress( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_IPAddress( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Port( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Port( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PortID( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PortID( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE TypeID( 
            /* [in] */ OPTOMUX_TYPE Type,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Load( 
            /* [in] */ IDispatch *pDispatch) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_InsertSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_UpdateSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PurgeSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SelectSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SelectByIDSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnumerateSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IOptomuxControllerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOptomuxController * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOptomuxController * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOptomuxController * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IOptomuxController * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IOptomuxController * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IOptomuxController * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IOptomuxController * This,
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
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Index )( 
            IOptomuxController * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Index )( 
            IOptomuxController * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            IOptomuxController * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ID )( 
            IOptomuxController * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            IOptomuxController * This,
            /* [retval][out] */ OPTOMUX_TYPE *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            IOptomuxController * This,
            /* [in] */ OPTOMUX_TYPE newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Address )( 
            IOptomuxController * This,
            /* [retval][out] */ BYTE *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Address )( 
            IOptomuxController * This,
            /* [in] */ BYTE newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PortIndex )( 
            IOptomuxController * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PortIndex )( 
            IOptomuxController * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ModuleInputOutputMap )( 
            IOptomuxController * This,
            /* [retval][out] */ BYTE *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ModuleInputOutputMap )( 
            IOptomuxController * This,
            /* [in] */ BYTE newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_NetworkCommunications )( 
            IOptomuxController * This,
            /* [retval][out] */ VARIANT_BOOL *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_NetworkCommunications )( 
            IOptomuxController * This,
            /* [in] */ VARIANT_BOOL newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IPAddress )( 
            IOptomuxController * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_IPAddress )( 
            IOptomuxController * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Port )( 
            IOptomuxController * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Port )( 
            IOptomuxController * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PortID )( 
            IOptomuxController * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PortID )( 
            IOptomuxController * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *TypeID )( 
            IOptomuxController * This,
            /* [in] */ OPTOMUX_TYPE Type,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Load )( 
            IOptomuxController * This,
            /* [in] */ IDispatch *pDispatch);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IOptomuxController * This);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_InsertSQL )( 
            IOptomuxController * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UpdateSQL )( 
            IOptomuxController * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PurgeSQL )( 
            IOptomuxController * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectSQL )( 
            IOptomuxController * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectByIDSQL )( 
            IOptomuxController * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnumerateSQL )( 
            IOptomuxController * This,
            /* [retval][out] */ BSTR *pVal);
        
        END_INTERFACE
    } IOptomuxControllerVtbl;

    interface IOptomuxController
    {
        CONST_VTBL struct IOptomuxControllerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOptomuxController_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOptomuxController_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOptomuxController_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOptomuxController_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IOptomuxController_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IOptomuxController_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IOptomuxController_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IOptomuxController_get_Index(This,pVal)	\
    ( (This)->lpVtbl -> get_Index(This,pVal) ) 

#define IOptomuxController_put_Index(This,newVal)	\
    ( (This)->lpVtbl -> put_Index(This,newVal) ) 

#define IOptomuxController_get_ID(This,pVal)	\
    ( (This)->lpVtbl -> get_ID(This,pVal) ) 

#define IOptomuxController_put_ID(This,newVal)	\
    ( (This)->lpVtbl -> put_ID(This,newVal) ) 

#define IOptomuxController_get_Type(This,pVal)	\
    ( (This)->lpVtbl -> get_Type(This,pVal) ) 

#define IOptomuxController_put_Type(This,newVal)	\
    ( (This)->lpVtbl -> put_Type(This,newVal) ) 

#define IOptomuxController_get_Address(This,pVal)	\
    ( (This)->lpVtbl -> get_Address(This,pVal) ) 

#define IOptomuxController_put_Address(This,newVal)	\
    ( (This)->lpVtbl -> put_Address(This,newVal) ) 

#define IOptomuxController_get_PortIndex(This,pVal)	\
    ( (This)->lpVtbl -> get_PortIndex(This,pVal) ) 

#define IOptomuxController_put_PortIndex(This,newVal)	\
    ( (This)->lpVtbl -> put_PortIndex(This,newVal) ) 

#define IOptomuxController_get_ModuleInputOutputMap(This,pVal)	\
    ( (This)->lpVtbl -> get_ModuleInputOutputMap(This,pVal) ) 

#define IOptomuxController_put_ModuleInputOutputMap(This,newVal)	\
    ( (This)->lpVtbl -> put_ModuleInputOutputMap(This,newVal) ) 

#define IOptomuxController_get_NetworkCommunications(This,pVal)	\
    ( (This)->lpVtbl -> get_NetworkCommunications(This,pVal) ) 

#define IOptomuxController_put_NetworkCommunications(This,newVal)	\
    ( (This)->lpVtbl -> put_NetworkCommunications(This,newVal) ) 

#define IOptomuxController_get_IPAddress(This,pVal)	\
    ( (This)->lpVtbl -> get_IPAddress(This,pVal) ) 

#define IOptomuxController_put_IPAddress(This,newVal)	\
    ( (This)->lpVtbl -> put_IPAddress(This,newVal) ) 

#define IOptomuxController_get_Port(This,pVal)	\
    ( (This)->lpVtbl -> get_Port(This,pVal) ) 

#define IOptomuxController_put_Port(This,newVal)	\
    ( (This)->lpVtbl -> put_Port(This,newVal) ) 

#define IOptomuxController_get_PortID(This,pVal)	\
    ( (This)->lpVtbl -> get_PortID(This,pVal) ) 

#define IOptomuxController_put_PortID(This,newVal)	\
    ( (This)->lpVtbl -> put_PortID(This,newVal) ) 

#define IOptomuxController_TypeID(This,Type,pVal)	\
    ( (This)->lpVtbl -> TypeID(This,Type,pVal) ) 

#define IOptomuxController_Load(This,pDispatch)	\
    ( (This)->lpVtbl -> Load(This,pDispatch) ) 

#define IOptomuxController_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IOptomuxController_get_InsertSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_InsertSQL(This,pVal) ) 

#define IOptomuxController_get_UpdateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_UpdateSQL(This,pVal) ) 

#define IOptomuxController_get_PurgeSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_PurgeSQL(This,pVal) ) 

#define IOptomuxController_get_SelectSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectSQL(This,pVal) ) 

#define IOptomuxController_get_SelectByIDSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectByIDSQL(This,pVal) ) 

#define IOptomuxController_get_EnumerateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_EnumerateSQL(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOptomuxController_INTERFACE_DEFINED__ */


#ifndef __IOptomuxControllerCollection_INTERFACE_DEFINED__
#define __IOptomuxControllerCollection_INTERFACE_DEFINED__

/* interface IOptomuxControllerCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IOptomuxControllerCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4CD8FFE8-912A-4434-BA90-BCF11610556E")
    IOptomuxControllerCollection : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ int Index,
            /* [retval][out] */ IOptomuxController **pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ IOptomuxController *pNewVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ long *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ int Index) = 0;
        
        virtual /* [restricted][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ IUnknown **pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IOptomuxControllerCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOptomuxControllerCollection * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOptomuxControllerCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOptomuxControllerCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IOptomuxControllerCollection * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IOptomuxControllerCollection * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IOptomuxControllerCollection * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IOptomuxControllerCollection * This,
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
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            IOptomuxControllerCollection * This,
            /* [in] */ int Index,
            /* [retval][out] */ IOptomuxController **pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            IOptomuxControllerCollection * This,
            /* [in] */ IOptomuxController *pNewVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            IOptomuxControllerCollection * This,
            /* [retval][out] */ long *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            IOptomuxControllerCollection * This,
            /* [in] */ int Index);
        
        /* [restricted][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            IOptomuxControllerCollection * This,
            /* [retval][out] */ IUnknown **pVal);
        
        END_INTERFACE
    } IOptomuxControllerCollectionVtbl;

    interface IOptomuxControllerCollection
    {
        CONST_VTBL struct IOptomuxControllerCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOptomuxControllerCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOptomuxControllerCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOptomuxControllerCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOptomuxControllerCollection_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IOptomuxControllerCollection_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IOptomuxControllerCollection_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IOptomuxControllerCollection_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IOptomuxControllerCollection_Item(This,Index,pVal)	\
    ( (This)->lpVtbl -> Item(This,Index,pVal) ) 

#define IOptomuxControllerCollection_Add(This,pNewVal)	\
    ( (This)->lpVtbl -> Add(This,pNewVal) ) 

#define IOptomuxControllerCollection_get_Count(This,pVal)	\
    ( (This)->lpVtbl -> get_Count(This,pVal) ) 

#define IOptomuxControllerCollection_Remove(This,Index)	\
    ( (This)->lpVtbl -> Remove(This,Index) ) 

#define IOptomuxControllerCollection_get__NewEnum(This,pVal)	\
    ( (This)->lpVtbl -> get__NewEnum(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOptomuxControllerCollection_INTERFACE_DEFINED__ */


#ifndef __IPort_INTERFACE_DEFINED__
#define __IPort_INTERFACE_DEFINED__

/* interface IPort */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPort;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D7B410E5-CA3D-4EA1-9E4A-AE42F084840F")
    IPort : public IDispatch
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Index( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Index( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_ID( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_ID( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Baud( 
            /* [retval][out] */ OPTOMUX_BAUD *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Baud( 
            /* [in] */ OPTOMUX_BAUD newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DataBits( 
            /* [retval][out] */ OPTOMUX_DATA_BITS *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DataBits( 
            /* [in] */ OPTOMUX_DATA_BITS newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Parity( 
            /* [retval][out] */ OPTOMUX_PARITY *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Parity( 
            /* [in] */ OPTOMUX_PARITY newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StopBits( 
            /* [retval][out] */ OPTOMUX_STOP_BITS *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StopBits( 
            /* [in] */ OPTOMUX_STOP_BITS newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE BaudID( 
            /* [in] */ OPTOMUX_BAUD Baud,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE DataBitsID( 
            /* [in] */ OPTOMUX_DATA_BITS DataBits,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ParityID( 
            /* [in] */ OPTOMUX_PARITY Parity,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE StopBitsID( 
            /* [in] */ OPTOMUX_STOP_BITS StopBits,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Load( 
            /* [in] */ IDispatch *pDispatch) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_InsertSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_UpdateSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PurgeSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SelectSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SelectByIDSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnumerateSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IPortVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPort * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPort * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPort * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IPort * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IPort * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IPort * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IPort * This,
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
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Index )( 
            IPort * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Index )( 
            IPort * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            IPort * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ID )( 
            IPort * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Baud )( 
            IPort * This,
            /* [retval][out] */ OPTOMUX_BAUD *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Baud )( 
            IPort * This,
            /* [in] */ OPTOMUX_BAUD newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DataBits )( 
            IPort * This,
            /* [retval][out] */ OPTOMUX_DATA_BITS *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DataBits )( 
            IPort * This,
            /* [in] */ OPTOMUX_DATA_BITS newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Parity )( 
            IPort * This,
            /* [retval][out] */ OPTOMUX_PARITY *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Parity )( 
            IPort * This,
            /* [in] */ OPTOMUX_PARITY newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StopBits )( 
            IPort * This,
            /* [retval][out] */ OPTOMUX_STOP_BITS *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StopBits )( 
            IPort * This,
            /* [in] */ OPTOMUX_STOP_BITS newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *BaudID )( 
            IPort * This,
            /* [in] */ OPTOMUX_BAUD Baud,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *DataBitsID )( 
            IPort * This,
            /* [in] */ OPTOMUX_DATA_BITS DataBits,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *ParityID )( 
            IPort * This,
            /* [in] */ OPTOMUX_PARITY Parity,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *StopBitsID )( 
            IPort * This,
            /* [in] */ OPTOMUX_STOP_BITS StopBits,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Load )( 
            IPort * This,
            /* [in] */ IDispatch *pDispatch);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IPort * This);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_InsertSQL )( 
            IPort * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UpdateSQL )( 
            IPort * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PurgeSQL )( 
            IPort * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectSQL )( 
            IPort * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectByIDSQL )( 
            IPort * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnumerateSQL )( 
            IPort * This,
            /* [retval][out] */ BSTR *pVal);
        
        END_INTERFACE
    } IPortVtbl;

    interface IPort
    {
        CONST_VTBL struct IPortVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPort_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPort_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPort_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPort_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IPort_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IPort_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IPort_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IPort_get_Index(This,pVal)	\
    ( (This)->lpVtbl -> get_Index(This,pVal) ) 

#define IPort_put_Index(This,newVal)	\
    ( (This)->lpVtbl -> put_Index(This,newVal) ) 

#define IPort_get_ID(This,pVal)	\
    ( (This)->lpVtbl -> get_ID(This,pVal) ) 

#define IPort_put_ID(This,newVal)	\
    ( (This)->lpVtbl -> put_ID(This,newVal) ) 

#define IPort_get_Baud(This,pVal)	\
    ( (This)->lpVtbl -> get_Baud(This,pVal) ) 

#define IPort_put_Baud(This,newVal)	\
    ( (This)->lpVtbl -> put_Baud(This,newVal) ) 

#define IPort_get_DataBits(This,pVal)	\
    ( (This)->lpVtbl -> get_DataBits(This,pVal) ) 

#define IPort_put_DataBits(This,newVal)	\
    ( (This)->lpVtbl -> put_DataBits(This,newVal) ) 

#define IPort_get_Parity(This,pVal)	\
    ( (This)->lpVtbl -> get_Parity(This,pVal) ) 

#define IPort_put_Parity(This,newVal)	\
    ( (This)->lpVtbl -> put_Parity(This,newVal) ) 

#define IPort_get_StopBits(This,pVal)	\
    ( (This)->lpVtbl -> get_StopBits(This,pVal) ) 

#define IPort_put_StopBits(This,newVal)	\
    ( (This)->lpVtbl -> put_StopBits(This,newVal) ) 

#define IPort_BaudID(This,Baud,pVal)	\
    ( (This)->lpVtbl -> BaudID(This,Baud,pVal) ) 

#define IPort_DataBitsID(This,DataBits,pVal)	\
    ( (This)->lpVtbl -> DataBitsID(This,DataBits,pVal) ) 

#define IPort_ParityID(This,Parity,pVal)	\
    ( (This)->lpVtbl -> ParityID(This,Parity,pVal) ) 

#define IPort_StopBitsID(This,StopBits,pVal)	\
    ( (This)->lpVtbl -> StopBitsID(This,StopBits,pVal) ) 

#define IPort_Load(This,pDispatch)	\
    ( (This)->lpVtbl -> Load(This,pDispatch) ) 

#define IPort_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IPort_get_InsertSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_InsertSQL(This,pVal) ) 

#define IPort_get_UpdateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_UpdateSQL(This,pVal) ) 

#define IPort_get_PurgeSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_PurgeSQL(This,pVal) ) 

#define IPort_get_SelectSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectSQL(This,pVal) ) 

#define IPort_get_SelectByIDSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectByIDSQL(This,pVal) ) 

#define IPort_get_EnumerateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_EnumerateSQL(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPort_INTERFACE_DEFINED__ */


#ifndef __IPortCollection_INTERFACE_DEFINED__
#define __IPortCollection_INTERFACE_DEFINED__

/* interface IPortCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPortCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C6A5E076-AFE9-4995-B0C2-3B196E0C13A7")
    IPortCollection : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ int Index,
            /* [retval][out] */ IPort **pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ IPort *pNewVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ long *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ int Index) = 0;
        
        virtual /* [restricted][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ IUnknown **pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IPortCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPortCollection * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPortCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPortCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IPortCollection * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IPortCollection * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IPortCollection * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IPortCollection * This,
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
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Item )( 
            IPortCollection * This,
            /* [in] */ int Index,
            /* [retval][out] */ IPort **pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            IPortCollection * This,
            /* [in] */ IPort *pNewVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            IPortCollection * This,
            /* [retval][out] */ long *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            IPortCollection * This,
            /* [in] */ int Index);
        
        /* [restricted][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            IPortCollection * This,
            /* [retval][out] */ IUnknown **pVal);
        
        END_INTERFACE
    } IPortCollectionVtbl;

    interface IPortCollection
    {
        CONST_VTBL struct IPortCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPortCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPortCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPortCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPortCollection_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IPortCollection_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IPortCollection_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IPortCollection_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IPortCollection_Item(This,Index,pVal)	\
    ( (This)->lpVtbl -> Item(This,Index,pVal) ) 

#define IPortCollection_Add(This,pNewVal)	\
    ( (This)->lpVtbl -> Add(This,pNewVal) ) 

#define IPortCollection_get_Count(This,pVal)	\
    ( (This)->lpVtbl -> get_Count(This,pVal) ) 

#define IPortCollection_Remove(This,Index)	\
    ( (This)->lpVtbl -> Remove(This,Index) ) 

#define IPortCollection_get__NewEnum(This,pVal)	\
    ( (This)->lpVtbl -> get__NewEnum(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPortCollection_INTERFACE_DEFINED__ */



#ifndef __OptomuxOPCObjectsLib_LIBRARY_DEFINED__
#define __OptomuxOPCObjectsLib_LIBRARY_DEFINED__

/* library OptomuxOPCObjectsLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_OptomuxOPCObjectsLib;

EXTERN_C const CLSID CLSID_OptomuxController;

#ifdef __cplusplus

class DECLSPEC_UUID("E7E70A48-7A25-4E15-A8B9-BDCD55254A92")
OptomuxController;
#endif

EXTERN_C const CLSID CLSID_OptomuxControllerCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("60E40DAA-9AB0-47D9-973A-095542C215A1")
OptomuxControllerCollection;
#endif

EXTERN_C const CLSID CLSID_Port;

#ifdef __cplusplus

class DECLSPEC_UUID("78ECB7AE-2D26-4635-A80E-5F7184475CF9")
Port;
#endif

EXTERN_C const CLSID CLSID_PortCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("579CD2E7-95F6-48F0-93EF-AA612F00B310")
PortCollection;
#endif
#endif /* __OptomuxOPCObjectsLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  BSTR_UserSize64(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal64(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal64(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree64(     unsigned long *, BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


