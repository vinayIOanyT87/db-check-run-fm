

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for ScullyOPCObjects.idl:
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

#ifndef __ScullyOPCObjects_h__
#define __ScullyOPCObjects_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IPort_FWD_DEFINED__
#define __IPort_FWD_DEFINED__
typedef interface IPort IPort;

#endif 	/* __IPort_FWD_DEFINED__ */


#ifndef __IScully_FWD_DEFINED__
#define __IScully_FWD_DEFINED__
typedef interface IScully IScully;

#endif 	/* __IScully_FWD_DEFINED__ */


#ifndef __IPortCollection_FWD_DEFINED__
#define __IPortCollection_FWD_DEFINED__
typedef interface IPortCollection IPortCollection;

#endif 	/* __IPortCollection_FWD_DEFINED__ */


#ifndef __IScullyCollection_FWD_DEFINED__
#define __IScullyCollection_FWD_DEFINED__
typedef interface IScullyCollection IScullyCollection;

#endif 	/* __IScullyCollection_FWD_DEFINED__ */


#ifndef __Port_FWD_DEFINED__
#define __Port_FWD_DEFINED__

#ifdef __cplusplus
typedef class Port Port;
#else
typedef struct Port Port;
#endif /* __cplusplus */

#endif 	/* __Port_FWD_DEFINED__ */


#ifndef __Scully_FWD_DEFINED__
#define __Scully_FWD_DEFINED__

#ifdef __cplusplus
typedef class Scully Scully;
#else
typedef struct Scully Scully;
#endif /* __cplusplus */

#endif 	/* __Scully_FWD_DEFINED__ */


#ifndef __PortCollection_FWD_DEFINED__
#define __PortCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class PortCollection PortCollection;
#else
typedef struct PortCollection PortCollection;
#endif /* __cplusplus */

#endif 	/* __PortCollection_FWD_DEFINED__ */


#ifndef __ScullyCollection_FWD_DEFINED__
#define __ScullyCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class ScullyCollection ScullyCollection;
#else
typedef struct ScullyCollection ScullyCollection;
#endif /* __cplusplus */

#endif 	/* __ScullyCollection_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_ScullyOPCObjects_0000_0000 */
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
enum SCULLY_BAUD
    {
        SCULLY_BAUD_1200	= 0,
        SCULLY_BAUD_2400	= 1,
        SCULLY_BAUD_4800	= 2,
        SCULLY_BAUD_9600	= 3,
        SCULLY_BAUD_19200	= 4,
        SCULLY_BAUD_38400	= 5,
        MAX_SCULLY_BAUD	= 6
    } 	SCULLY_BAUD;

typedef 
enum SCULLY_PARITY
    {
        SCULLY_PARITY_NONE	= 0,
        SCULLY_PARITY_EVEN	= 1,
        SCULLY_PARITY_ODD	= 2,
        MAX_SCULLY_PARITY	= 3
    } 	SCULLY_PARITY;

typedef 
enum SCULLY_DATA_BITS
    {
        DATA_BITS_7	= 0,
        DATA_BITS_8	= 1,
        MAX_SCULLY_DATA_BITS	= 2
    } 	SCULLY_DATA_BITS;

typedef 
enum SCULLY_STOP_BITS
    {
        STOP_BITS_1	= 0,
        STOP_BITS_2	= 1,
        MAX_SCULLY_STOP_BITS	= 2
    } 	SCULLY_STOP_BITS;



extern RPC_IF_HANDLE __MIDL_itf_ScullyOPCObjects_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ScullyOPCObjects_0000_0000_v0_0_s_ifspec;

#ifndef __IPort_INTERFACE_DEFINED__
#define __IPort_INTERFACE_DEFINED__

/* interface IPort */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPort;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("36992C1C-5543-4574-B9FD-426EC2AFA132")
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
            /* [retval][out] */ SCULLY_BAUD *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Baud( 
            /* [in] */ SCULLY_BAUD newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DataBits( 
            /* [retval][out] */ SCULLY_DATA_BITS *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DataBits( 
            /* [in] */ SCULLY_DATA_BITS newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Parity( 
            /* [retval][out] */ SCULLY_PARITY *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Parity( 
            /* [in] */ SCULLY_PARITY newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StopBits( 
            /* [retval][out] */ SCULLY_STOP_BITS *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StopBits( 
            /* [in] */ SCULLY_STOP_BITS newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE BaudID( 
            /* [in] */ SCULLY_BAUD Baud,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE DataBitsID( 
            /* [in] */ SCULLY_DATA_BITS DataBits,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ParityID( 
            /* [in] */ SCULLY_PARITY Parity,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE StopBitsID( 
            /* [in] */ SCULLY_STOP_BITS StopBits,
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
            /* [retval][out] */ SCULLY_BAUD *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Baud )( 
            IPort * This,
            /* [in] */ SCULLY_BAUD newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DataBits )( 
            IPort * This,
            /* [retval][out] */ SCULLY_DATA_BITS *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DataBits )( 
            IPort * This,
            /* [in] */ SCULLY_DATA_BITS newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Parity )( 
            IPort * This,
            /* [retval][out] */ SCULLY_PARITY *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Parity )( 
            IPort * This,
            /* [in] */ SCULLY_PARITY newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StopBits )( 
            IPort * This,
            /* [retval][out] */ SCULLY_STOP_BITS *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StopBits )( 
            IPort * This,
            /* [in] */ SCULLY_STOP_BITS newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *BaudID )( 
            IPort * This,
            /* [in] */ SCULLY_BAUD Baud,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *DataBitsID )( 
            IPort * This,
            /* [in] */ SCULLY_DATA_BITS DataBits,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *ParityID )( 
            IPort * This,
            /* [in] */ SCULLY_PARITY Parity,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *StopBitsID )( 
            IPort * This,
            /* [in] */ SCULLY_STOP_BITS StopBits,
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


#ifndef __IScully_INTERFACE_DEFINED__
#define __IScully_INTERFACE_DEFINED__

/* interface IScully */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IScully;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D3A59F43-E032-4bd9-A529-8169ACE6F64B")
    IScully : public IDispatch
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
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PortIndex( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PortIndex( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DeviceID( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DeviceID( 
            /* [in] */ LONG newVal) = 0;
        
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

    typedef struct IScullyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IScully * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IScully * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IScully * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IScully * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IScully * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IScully * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IScully * This,
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
            IScully * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Index )( 
            IScully * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            IScully * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ID )( 
            IScully * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PortIndex )( 
            IScully * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PortIndex )( 
            IScully * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DeviceID )( 
            IScully * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DeviceID )( 
            IScully * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Load )( 
            IScully * This,
            /* [in] */ IDispatch *pDispatch);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IScully * This);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_InsertSQL )( 
            IScully * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UpdateSQL )( 
            IScully * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PurgeSQL )( 
            IScully * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectSQL )( 
            IScully * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectByIDSQL )( 
            IScully * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnumerateSQL )( 
            IScully * This,
            /* [retval][out] */ BSTR *pVal);
        
        END_INTERFACE
    } IScullyVtbl;

    interface IScully
    {
        CONST_VTBL struct IScullyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IScully_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IScully_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IScully_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IScully_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IScully_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IScully_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IScully_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IScully_get_Index(This,pVal)	\
    ( (This)->lpVtbl -> get_Index(This,pVal) ) 

#define IScully_put_Index(This,newVal)	\
    ( (This)->lpVtbl -> put_Index(This,newVal) ) 

#define IScully_get_ID(This,pVal)	\
    ( (This)->lpVtbl -> get_ID(This,pVal) ) 

#define IScully_put_ID(This,newVal)	\
    ( (This)->lpVtbl -> put_ID(This,newVal) ) 

#define IScully_get_PortIndex(This,pVal)	\
    ( (This)->lpVtbl -> get_PortIndex(This,pVal) ) 

#define IScully_put_PortIndex(This,newVal)	\
    ( (This)->lpVtbl -> put_PortIndex(This,newVal) ) 

#define IScully_get_DeviceID(This,pVal)	\
    ( (This)->lpVtbl -> get_DeviceID(This,pVal) ) 

#define IScully_put_DeviceID(This,newVal)	\
    ( (This)->lpVtbl -> put_DeviceID(This,newVal) ) 

#define IScully_Load(This,pDispatch)	\
    ( (This)->lpVtbl -> Load(This,pDispatch) ) 

#define IScully_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IScully_get_InsertSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_InsertSQL(This,pVal) ) 

#define IScully_get_UpdateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_UpdateSQL(This,pVal) ) 

#define IScully_get_PurgeSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_PurgeSQL(This,pVal) ) 

#define IScully_get_SelectSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectSQL(This,pVal) ) 

#define IScully_get_SelectByIDSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectByIDSQL(This,pVal) ) 

#define IScully_get_EnumerateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_EnumerateSQL(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IScully_INTERFACE_DEFINED__ */


#ifndef __IPortCollection_INTERFACE_DEFINED__
#define __IPortCollection_INTERFACE_DEFINED__

/* interface IPortCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPortCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1F6D57D0-075C-4de2-B7A6-1245FC1616C1")
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


#ifndef __IScullyCollection_INTERFACE_DEFINED__
#define __IScullyCollection_INTERFACE_DEFINED__

/* interface IScullyCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IScullyCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5746EEC1-7306-420a-9305-209F39C2A9D8")
    IScullyCollection : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ int Index,
            /* [retval][out] */ IScully **pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ IScully *pNewVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ long *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ int Index) = 0;
        
        virtual /* [restricted][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ IUnknown **pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IScullyCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IScullyCollection * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IScullyCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IScullyCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IScullyCollection * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IScullyCollection * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IScullyCollection * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IScullyCollection * This,
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
            IScullyCollection * This,
            /* [in] */ int Index,
            /* [retval][out] */ IScully **pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            IScullyCollection * This,
            /* [in] */ IScully *pNewVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            IScullyCollection * This,
            /* [retval][out] */ long *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            IScullyCollection * This,
            /* [in] */ int Index);
        
        /* [restricted][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            IScullyCollection * This,
            /* [retval][out] */ IUnknown **pVal);
        
        END_INTERFACE
    } IScullyCollectionVtbl;

    interface IScullyCollection
    {
        CONST_VTBL struct IScullyCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IScullyCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IScullyCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IScullyCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IScullyCollection_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IScullyCollection_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IScullyCollection_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IScullyCollection_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IScullyCollection_Item(This,Index,pVal)	\
    ( (This)->lpVtbl -> Item(This,Index,pVal) ) 

#define IScullyCollection_Add(This,pNewVal)	\
    ( (This)->lpVtbl -> Add(This,pNewVal) ) 

#define IScullyCollection_get_Count(This,pVal)	\
    ( (This)->lpVtbl -> get_Count(This,pVal) ) 

#define IScullyCollection_Remove(This,Index)	\
    ( (This)->lpVtbl -> Remove(This,Index) ) 

#define IScullyCollection_get__NewEnum(This,pVal)	\
    ( (This)->lpVtbl -> get__NewEnum(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IScullyCollection_INTERFACE_DEFINED__ */



#ifndef __ScullyOPCObjectsLib_LIBRARY_DEFINED__
#define __ScullyOPCObjectsLib_LIBRARY_DEFINED__

/* library ScullyOPCObjectsLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_ScullyOPCObjectsLib;

EXTERN_C const CLSID CLSID_Port;

#ifdef __cplusplus

class DECLSPEC_UUID("465A30A8-5978-4948-A5DB-31EB09AACCD1")
Port;
#endif

EXTERN_C const CLSID CLSID_Scully;

#ifdef __cplusplus

class DECLSPEC_UUID("33772EBA-F1EF-49f5-BD16-A0AE9E60A4DB")
Scully;
#endif

EXTERN_C const CLSID CLSID_PortCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("9C08C25E-50C2-451a-9EA3-E84C589B8074")
PortCollection;
#endif

EXTERN_C const CLSID CLSID_ScullyCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("C48047CC-EA3B-464a-9306-89785873394F")
ScullyCollection;
#endif
#endif /* __ScullyOPCObjectsLib_LIBRARY_DEFINED__ */

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


