

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for WeightScaleOPCObjects.idl:
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

#ifndef __WeightScaleOPCObjects_h__
#define __WeightScaleOPCObjects_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IPort_FWD_DEFINED__
#define __IPort_FWD_DEFINED__
typedef interface IPort IPort;

#endif 	/* __IPort_FWD_DEFINED__ */


#ifndef __IWeightScale_FWD_DEFINED__
#define __IWeightScale_FWD_DEFINED__
typedef interface IWeightScale IWeightScale;

#endif 	/* __IWeightScale_FWD_DEFINED__ */


#ifndef __IPortCollection_FWD_DEFINED__
#define __IPortCollection_FWD_DEFINED__
typedef interface IPortCollection IPortCollection;

#endif 	/* __IPortCollection_FWD_DEFINED__ */


#ifndef __IWeightScaleCollection_FWD_DEFINED__
#define __IWeightScaleCollection_FWD_DEFINED__
typedef interface IWeightScaleCollection IWeightScaleCollection;

#endif 	/* __IWeightScaleCollection_FWD_DEFINED__ */


#ifndef __Port_FWD_DEFINED__
#define __Port_FWD_DEFINED__

#ifdef __cplusplus
typedef class Port Port;
#else
typedef struct Port Port;
#endif /* __cplusplus */

#endif 	/* __Port_FWD_DEFINED__ */


#ifndef __WeightScale_FWD_DEFINED__
#define __WeightScale_FWD_DEFINED__

#ifdef __cplusplus
typedef class WeightScale WeightScale;
#else
typedef struct WeightScale WeightScale;
#endif /* __cplusplus */

#endif 	/* __WeightScale_FWD_DEFINED__ */


#ifndef __PortCollection_FWD_DEFINED__
#define __PortCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class PortCollection PortCollection;
#else
typedef struct PortCollection PortCollection;
#endif /* __cplusplus */

#endif 	/* __PortCollection_FWD_DEFINED__ */


#ifndef __WeightScaleCollection_FWD_DEFINED__
#define __WeightScaleCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class WeightScaleCollection WeightScaleCollection;
#else
typedef struct WeightScaleCollection WeightScaleCollection;
#endif /* __cplusplus */

#endif 	/* __WeightScaleCollection_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_WeightScaleOPCObjects_0000_0000 */
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
enum WEIGHTSCALE_TYPE
    {
        TOLEDO_8142	= 0,
        FAIRBANKS_90_164	= 1,
        BRECHBUHLER_UMC600	= 2,
        SIPELARIES_ASCII	= 3,
        METTLER_SICS	= 4,
        RICE_LAKE_720I	= 5,
        MAX_WEIGHTSCALE_TYPE	= 6
    } 	WEIGHTSCALE_TYPE;

typedef 
enum WEIGHTSCALE_BAUD
    {
        WEIGHTSCALE_BAUD_1200	= 0,
        WEIGHTSCALE_BAUD_2400	= 1,
        WEIGHTSCALE_BAUD_4800	= 2,
        WEIGHTSCALE_BAUD_9600	= 3,
        WEIGHTSCALE_BAUD_19200	= 4,
        WEIGHTSCALE_BAUD_38400	= 5,
        MAX_WEIGHTSCALE_BAUD	= 6
    } 	WEIGHTSCALE_BAUD;

typedef 
enum WEIGHTSCALE_PARITY
    {
        WEIGHTSCALE_PARITY_NONE	= 0,
        WEIGHTSCALE_PARITY_EVEN	= 1,
        WEIGHTSCALE_PARITY_ODD	= 2,
        MAX_WEIGHTSCALE_PARITY	= 3
    } 	WEIGHTSCALE_PARITY;

typedef 
enum WEIGHTSCALE_DATA_BITS
    {
        DATA_BITS_7	= 0,
        DATA_BITS_8	= 1,
        MAX_WEIGHTSCALE_DATA_BITS	= 2
    } 	WEIGHTSCALE_DATA_BITS;

typedef 
enum WEIGHTSCALE_STOP_BITS
    {
        STOP_BITS_1	= 0,
        STOP_BITS_2	= 1,
        MAX_WEIGHTSCALE_STOP_BITS	= 2
    } 	WEIGHTSCALE_STOP_BITS;



extern RPC_IF_HANDLE __MIDL_itf_WeightScaleOPCObjects_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_WeightScaleOPCObjects_0000_0000_v0_0_s_ifspec;

#ifndef __IPort_INTERFACE_DEFINED__
#define __IPort_INTERFACE_DEFINED__

/* interface IPort */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPort;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("24DEED9D-30F2-489E-87B3-984D428E42E9")
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
            /* [retval][out] */ WEIGHTSCALE_BAUD *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Baud( 
            /* [in] */ WEIGHTSCALE_BAUD newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DataBits( 
            /* [retval][out] */ WEIGHTSCALE_DATA_BITS *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DataBits( 
            /* [in] */ WEIGHTSCALE_DATA_BITS newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Parity( 
            /* [retval][out] */ WEIGHTSCALE_PARITY *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Parity( 
            /* [in] */ WEIGHTSCALE_PARITY newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StopBits( 
            /* [retval][out] */ WEIGHTSCALE_STOP_BITS *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StopBits( 
            /* [in] */ WEIGHTSCALE_STOP_BITS newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE BaudID( 
            /* [in] */ WEIGHTSCALE_BAUD Baud,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE DataBitsID( 
            /* [in] */ WEIGHTSCALE_DATA_BITS DataBits,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ParityID( 
            /* [in] */ WEIGHTSCALE_PARITY Parity,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE StopBitsID( 
            /* [in] */ WEIGHTSCALE_STOP_BITS StopBits,
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
            /* [retval][out] */ WEIGHTSCALE_BAUD *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Baud )( 
            IPort * This,
            /* [in] */ WEIGHTSCALE_BAUD newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DataBits )( 
            IPort * This,
            /* [retval][out] */ WEIGHTSCALE_DATA_BITS *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DataBits )( 
            IPort * This,
            /* [in] */ WEIGHTSCALE_DATA_BITS newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Parity )( 
            IPort * This,
            /* [retval][out] */ WEIGHTSCALE_PARITY *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Parity )( 
            IPort * This,
            /* [in] */ WEIGHTSCALE_PARITY newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StopBits )( 
            IPort * This,
            /* [retval][out] */ WEIGHTSCALE_STOP_BITS *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StopBits )( 
            IPort * This,
            /* [in] */ WEIGHTSCALE_STOP_BITS newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *BaudID )( 
            IPort * This,
            /* [in] */ WEIGHTSCALE_BAUD Baud,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *DataBitsID )( 
            IPort * This,
            /* [in] */ WEIGHTSCALE_DATA_BITS DataBits,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *ParityID )( 
            IPort * This,
            /* [in] */ WEIGHTSCALE_PARITY Parity,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *StopBitsID )( 
            IPort * This,
            /* [in] */ WEIGHTSCALE_STOP_BITS StopBits,
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


#ifndef __IWeightScale_INTERFACE_DEFINED__
#define __IWeightScale_INTERFACE_DEFINED__

/* interface IWeightScale */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IWeightScale;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D18CE9A3-9851-4EDD-9964-68938A5D6CFB")
    IWeightScale : public IDispatch
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
            /* [retval][out] */ WEIGHTSCALE_TYPE *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Type( 
            /* [in] */ WEIGHTSCALE_TYPE newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PortIndex( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PortIndex( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Port( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Port( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DeviceID( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DeviceID( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE TypeID( 
            /* [in] */ WEIGHTSCALE_TYPE Type,
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

    typedef struct IWeightScaleVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IWeightScale * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IWeightScale * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IWeightScale * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IWeightScale * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IWeightScale * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IWeightScale * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IWeightScale * This,
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
            IWeightScale * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Index )( 
            IWeightScale * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            IWeightScale * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ID )( 
            IWeightScale * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            IWeightScale * This,
            /* [retval][out] */ WEIGHTSCALE_TYPE *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            IWeightScale * This,
            /* [in] */ WEIGHTSCALE_TYPE newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PortIndex )( 
            IWeightScale * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PortIndex )( 
            IWeightScale * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Port )( 
            IWeightScale * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Port )( 
            IWeightScale * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DeviceID )( 
            IWeightScale * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DeviceID )( 
            IWeightScale * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *TypeID )( 
            IWeightScale * This,
            /* [in] */ WEIGHTSCALE_TYPE Type,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Load )( 
            IWeightScale * This,
            /* [in] */ IDispatch *pDispatch);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IWeightScale * This);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_InsertSQL )( 
            IWeightScale * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UpdateSQL )( 
            IWeightScale * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PurgeSQL )( 
            IWeightScale * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectSQL )( 
            IWeightScale * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectByIDSQL )( 
            IWeightScale * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnumerateSQL )( 
            IWeightScale * This,
            /* [retval][out] */ BSTR *pVal);
        
        END_INTERFACE
    } IWeightScaleVtbl;

    interface IWeightScale
    {
        CONST_VTBL struct IWeightScaleVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IWeightScale_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IWeightScale_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IWeightScale_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IWeightScale_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IWeightScale_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IWeightScale_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IWeightScale_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IWeightScale_get_Index(This,pVal)	\
    ( (This)->lpVtbl -> get_Index(This,pVal) ) 

#define IWeightScale_put_Index(This,newVal)	\
    ( (This)->lpVtbl -> put_Index(This,newVal) ) 

#define IWeightScale_get_ID(This,pVal)	\
    ( (This)->lpVtbl -> get_ID(This,pVal) ) 

#define IWeightScale_put_ID(This,newVal)	\
    ( (This)->lpVtbl -> put_ID(This,newVal) ) 

#define IWeightScale_get_Type(This,pVal)	\
    ( (This)->lpVtbl -> get_Type(This,pVal) ) 

#define IWeightScale_put_Type(This,newVal)	\
    ( (This)->lpVtbl -> put_Type(This,newVal) ) 

#define IWeightScale_get_PortIndex(This,pVal)	\
    ( (This)->lpVtbl -> get_PortIndex(This,pVal) ) 

#define IWeightScale_put_PortIndex(This,newVal)	\
    ( (This)->lpVtbl -> put_PortIndex(This,newVal) ) 

#define IWeightScale_get_Port(This,pVal)	\
    ( (This)->lpVtbl -> get_Port(This,pVal) ) 

#define IWeightScale_put_Port(This,newVal)	\
    ( (This)->lpVtbl -> put_Port(This,newVal) ) 

#define IWeightScale_get_DeviceID(This,pVal)	\
    ( (This)->lpVtbl -> get_DeviceID(This,pVal) ) 

#define IWeightScale_put_DeviceID(This,newVal)	\
    ( (This)->lpVtbl -> put_DeviceID(This,newVal) ) 

#define IWeightScale_TypeID(This,Type,pVal)	\
    ( (This)->lpVtbl -> TypeID(This,Type,pVal) ) 

#define IWeightScale_Load(This,pDispatch)	\
    ( (This)->lpVtbl -> Load(This,pDispatch) ) 

#define IWeightScale_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IWeightScale_get_InsertSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_InsertSQL(This,pVal) ) 

#define IWeightScale_get_UpdateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_UpdateSQL(This,pVal) ) 

#define IWeightScale_get_PurgeSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_PurgeSQL(This,pVal) ) 

#define IWeightScale_get_SelectSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectSQL(This,pVal) ) 

#define IWeightScale_get_SelectByIDSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectByIDSQL(This,pVal) ) 

#define IWeightScale_get_EnumerateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_EnumerateSQL(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IWeightScale_INTERFACE_DEFINED__ */


#ifndef __IPortCollection_INTERFACE_DEFINED__
#define __IPortCollection_INTERFACE_DEFINED__

/* interface IPortCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPortCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2AEBF73A-01C7-4A92-BB3F-0D466FA029B0")
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


#ifndef __IWeightScaleCollection_INTERFACE_DEFINED__
#define __IWeightScaleCollection_INTERFACE_DEFINED__

/* interface IWeightScaleCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IWeightScaleCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5B148EFF-E2D2-4F6B-A2E8-C16C5519F64B")
    IWeightScaleCollection : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ int Index,
            /* [retval][out] */ IWeightScale **pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ IWeightScale *pNewVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ long *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ int Index) = 0;
        
        virtual /* [restricted][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ IUnknown **pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IWeightScaleCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IWeightScaleCollection * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IWeightScaleCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IWeightScaleCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IWeightScaleCollection * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IWeightScaleCollection * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IWeightScaleCollection * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IWeightScaleCollection * This,
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
            IWeightScaleCollection * This,
            /* [in] */ int Index,
            /* [retval][out] */ IWeightScale **pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            IWeightScaleCollection * This,
            /* [in] */ IWeightScale *pNewVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            IWeightScaleCollection * This,
            /* [retval][out] */ long *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            IWeightScaleCollection * This,
            /* [in] */ int Index);
        
        /* [restricted][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            IWeightScaleCollection * This,
            /* [retval][out] */ IUnknown **pVal);
        
        END_INTERFACE
    } IWeightScaleCollectionVtbl;

    interface IWeightScaleCollection
    {
        CONST_VTBL struct IWeightScaleCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IWeightScaleCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IWeightScaleCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IWeightScaleCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IWeightScaleCollection_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IWeightScaleCollection_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IWeightScaleCollection_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IWeightScaleCollection_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IWeightScaleCollection_Item(This,Index,pVal)	\
    ( (This)->lpVtbl -> Item(This,Index,pVal) ) 

#define IWeightScaleCollection_Add(This,pNewVal)	\
    ( (This)->lpVtbl -> Add(This,pNewVal) ) 

#define IWeightScaleCollection_get_Count(This,pVal)	\
    ( (This)->lpVtbl -> get_Count(This,pVal) ) 

#define IWeightScaleCollection_Remove(This,Index)	\
    ( (This)->lpVtbl -> Remove(This,Index) ) 

#define IWeightScaleCollection_get__NewEnum(This,pVal)	\
    ( (This)->lpVtbl -> get__NewEnum(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IWeightScaleCollection_INTERFACE_DEFINED__ */



#ifndef __WeightScaleOPCObjectsLib_LIBRARY_DEFINED__
#define __WeightScaleOPCObjectsLib_LIBRARY_DEFINED__

/* library WeightScaleOPCObjectsLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_WeightScaleOPCObjectsLib;

EXTERN_C const CLSID CLSID_Port;

#ifdef __cplusplus

class DECLSPEC_UUID("B140A6E9-E0DD-4C4F-B973-730CCA43136D")
Port;
#endif

EXTERN_C const CLSID CLSID_WeightScale;

#ifdef __cplusplus

class DECLSPEC_UUID("C20933A0-BBE4-42D9-9AAC-0532B2FD8807")
WeightScale;
#endif

EXTERN_C const CLSID CLSID_PortCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("93ED554D-DE90-4090-A9E6-9CA89F97EAD1")
PortCollection;
#endif

EXTERN_C const CLSID CLSID_WeightScaleCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("B47EA5BF-06E8-441C-BADE-43E0D6CE347D")
WeightScaleCollection;
#endif
#endif /* __WeightScaleOPCObjectsLib_LIBRARY_DEFINED__ */

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


