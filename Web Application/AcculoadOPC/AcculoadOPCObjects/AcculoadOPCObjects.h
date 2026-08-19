

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for AcculoadOPCObjects.idl:
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

#ifndef __AcculoadOPCObjects_h__
#define __AcculoadOPCObjects_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IAccuload_FWD_DEFINED__
#define __IAccuload_FWD_DEFINED__
typedef interface IAccuload IAccuload;

#endif 	/* __IAccuload_FWD_DEFINED__ */


#ifndef __IAcculoadCollection_FWD_DEFINED__
#define __IAcculoadCollection_FWD_DEFINED__
typedef interface IAcculoadCollection IAcculoadCollection;

#endif 	/* __IAcculoadCollection_FWD_DEFINED__ */


#ifndef __IArm_FWD_DEFINED__
#define __IArm_FWD_DEFINED__
typedef interface IArm IArm;

#endif 	/* __IArm_FWD_DEFINED__ */


#ifndef __IArmCollection_FWD_DEFINED__
#define __IArmCollection_FWD_DEFINED__
typedef interface IArmCollection IArmCollection;

#endif 	/* __IArmCollection_FWD_DEFINED__ */


#ifndef __ICardReader_FWD_DEFINED__
#define __ICardReader_FWD_DEFINED__
typedef interface ICardReader ICardReader;

#endif 	/* __ICardReader_FWD_DEFINED__ */


#ifndef __ICardReaderCollection_FWD_DEFINED__
#define __ICardReaderCollection_FWD_DEFINED__
typedef interface ICardReaderCollection ICardReaderCollection;

#endif 	/* __ICardReaderCollection_FWD_DEFINED__ */


#ifndef __IPort_FWD_DEFINED__
#define __IPort_FWD_DEFINED__
typedef interface IPort IPort;

#endif 	/* __IPort_FWD_DEFINED__ */


#ifndef __IPortCollection_FWD_DEFINED__
#define __IPortCollection_FWD_DEFINED__
typedef interface IPortCollection IPortCollection;

#endif 	/* __IPortCollection_FWD_DEFINED__ */


#ifndef __Accuload_FWD_DEFINED__
#define __Accuload_FWD_DEFINED__

#ifdef __cplusplus
typedef class Accuload Accuload;
#else
typedef struct Accuload Accuload;
#endif /* __cplusplus */

#endif 	/* __Accuload_FWD_DEFINED__ */


#ifndef __AcculoadCollection_FWD_DEFINED__
#define __AcculoadCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class AcculoadCollection AcculoadCollection;
#else
typedef struct AcculoadCollection AcculoadCollection;
#endif /* __cplusplus */

#endif 	/* __AcculoadCollection_FWD_DEFINED__ */


#ifndef __Arm_FWD_DEFINED__
#define __Arm_FWD_DEFINED__

#ifdef __cplusplus
typedef class Arm Arm;
#else
typedef struct Arm Arm;
#endif /* __cplusplus */

#endif 	/* __Arm_FWD_DEFINED__ */


#ifndef __ArmCollection_FWD_DEFINED__
#define __ArmCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class ArmCollection ArmCollection;
#else
typedef struct ArmCollection ArmCollection;
#endif /* __cplusplus */

#endif 	/* __ArmCollection_FWD_DEFINED__ */


#ifndef __CardReader_FWD_DEFINED__
#define __CardReader_FWD_DEFINED__

#ifdef __cplusplus
typedef class CardReader CardReader;
#else
typedef struct CardReader CardReader;
#endif /* __cplusplus */

#endif 	/* __CardReader_FWD_DEFINED__ */


#ifndef __CardReaderCollection_FWD_DEFINED__
#define __CardReaderCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class CardReaderCollection CardReaderCollection;
#else
typedef struct CardReaderCollection CardReaderCollection;
#endif /* __cplusplus */

#endif 	/* __CardReaderCollection_FWD_DEFINED__ */


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


/* interface __MIDL_itf_AcculoadOPCObjects_0000_0000 */
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
enum ACCULOAD_TYPE
    {
        ACCULOAD_2_STD	= 0,
        ACCULOAD_2_SEQ	= 1,
        ACCULOAD_2_RBM	= 2,
        ACCULOAD_III_Q	= 3,
        MICROLOAD_NET	= 4,
        MULTILOAD_II_SMP	= 5,
        ACCULOAD_III_SA	= 6,
        MULTILOAD_II	= 7,
        MAX_ACCULOAD_TYPE	= 8
    } 	ACCULOAD_TYPE;

typedef 
enum ACCULOAD_BAUD
    {
        ACCULOAD_BAUD_1200	= 0,
        ACCULOAD_BAUD_2400	= 1,
        ACCULOAD_BAUD_4800	= 2,
        ACCULOAD_BAUD_9600	= 3,
        ACCULOAD_BAUD_19200	= 4,
        ACCULOAD_BAUD_38400	= 5,
        MAX_ACCULOAD_BAUD	= 6
    } 	ACCULOAD_BAUD;

typedef 
enum ACCULOAD_PARITY
    {
        ACCULOAD_PARITY_NONE	= 0,
        ACCULOAD_PARITY_EVEN	= 1,
        ACCULOAD_PARITY_ODD	= 2,
        MAX_ACCULOAD_PARITY	= 3
    } 	ACCULOAD_PARITY;

typedef 
enum ACCULOAD_DATA_BITS
    {
        DATA_BITS_7	= 0,
        DATA_BITS_8	= 1,
        MAX_ACCULOAD_DATA_BITS	= 2
    } 	ACCULOAD_DATA_BITS;

typedef 
enum ACCULOAD_STOP_BITS
    {
        STOP_BITS_1	= 0,
        STOP_BITS_2	= 1,
        MAX_ACCULOAD_STOP_BITS	= 2
    } 	ACCULOAD_STOP_BITS;

typedef 
enum ACCULOAD_ARM_TYPE
    {
        STRAIGHT	= 0,
        SEQUENTIAL	= 1,
        RATIO	= 2,
        SIDE_STREAM	= 3,
        UNLOADING	= 4,
        MAX_ACCULOAD_ARM_TYPE	= 5
    } 	ACCULOAD_ARM_TYPE;



extern RPC_IF_HANDLE __MIDL_itf_AcculoadOPCObjects_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_AcculoadOPCObjects_0000_0000_v0_0_s_ifspec;

#ifndef __IAccuload_INTERFACE_DEFINED__
#define __IAccuload_INTERFACE_DEFINED__

/* interface IAccuload */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IAccuload;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BF426B41-95C8-4133-8D29-F87FAA245631")
    IAccuload : public IDispatch
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
            /* [retval][out] */ ACCULOAD_TYPE *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Type( 
            /* [in] */ ACCULOAD_TYPE newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PortIndex( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PortIndex( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Arms( 
            /* [retval][out] */ IDispatch **pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Arms( 
            /* [in] */ IDispatch *newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PortID( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PortID( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_NetworkCommunications( 
            /* [retval][out] */ VARIANT_BOOL *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_NetworkCommunications( 
            /* [in] */ VARIANT_BOOL newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_IPAddress( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_IPAddress( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE TypeID( 
            /* [in] */ ACCULOAD_TYPE Type,
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

    typedef struct IAcculoadVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IAccuload * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IAccuload * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IAccuload * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IAccuload * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IAccuload * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IAccuload * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IAccuload * This,
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
            IAccuload * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Index )( 
            IAccuload * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            IAccuload * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ID )( 
            IAccuload * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            IAccuload * This,
            /* [retval][out] */ ACCULOAD_TYPE *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            IAccuload * This,
            /* [in] */ ACCULOAD_TYPE newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PortIndex )( 
            IAccuload * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PortIndex )( 
            IAccuload * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Arms )( 
            IAccuload * This,
            /* [retval][out] */ IDispatch **pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Arms )( 
            IAccuload * This,
            /* [in] */ IDispatch *newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PortID )( 
            IAccuload * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PortID )( 
            IAccuload * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_NetworkCommunications )( 
            IAccuload * This,
            /* [retval][out] */ VARIANT_BOOL *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_NetworkCommunications )( 
            IAccuload * This,
            /* [in] */ VARIANT_BOOL newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IPAddress )( 
            IAccuload * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_IPAddress )( 
            IAccuload * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *TypeID )( 
            IAccuload * This,
            /* [in] */ ACCULOAD_TYPE Type,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Load )( 
            IAccuload * This,
            /* [in] */ IDispatch *pDispatch);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IAccuload * This);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_InsertSQL )( 
            IAccuload * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UpdateSQL )( 
            IAccuload * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PurgeSQL )( 
            IAccuload * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectSQL )( 
            IAccuload * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectByIDSQL )( 
            IAccuload * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnumerateSQL )( 
            IAccuload * This,
            /* [retval][out] */ BSTR *pVal);
        
        END_INTERFACE
    } IAcculoadVtbl;

    interface IAccuload
    {
        CONST_VTBL struct IAcculoadVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAccuload_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IAccuload_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IAccuload_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IAccuload_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IAccuload_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IAccuload_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IAccuload_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IAccuload_get_Index(This,pVal)	\
    ( (This)->lpVtbl -> get_Index(This,pVal) ) 

#define IAccuload_put_Index(This,newVal)	\
    ( (This)->lpVtbl -> put_Index(This,newVal) ) 

#define IAccuload_get_ID(This,pVal)	\
    ( (This)->lpVtbl -> get_ID(This,pVal) ) 

#define IAccuload_put_ID(This,newVal)	\
    ( (This)->lpVtbl -> put_ID(This,newVal) ) 

#define IAccuload_get_Type(This,pVal)	\
    ( (This)->lpVtbl -> get_Type(This,pVal) ) 

#define IAccuload_put_Type(This,newVal)	\
    ( (This)->lpVtbl -> put_Type(This,newVal) ) 

#define IAccuload_get_PortIndex(This,pVal)	\
    ( (This)->lpVtbl -> get_PortIndex(This,pVal) ) 

#define IAccuload_put_PortIndex(This,newVal)	\
    ( (This)->lpVtbl -> put_PortIndex(This,newVal) ) 

#define IAccuload_get_Arms(This,pVal)	\
    ( (This)->lpVtbl -> get_Arms(This,pVal) ) 

#define IAccuload_put_Arms(This,newVal)	\
    ( (This)->lpVtbl -> put_Arms(This,newVal) ) 

#define IAccuload_get_PortID(This,pVal)	\
    ( (This)->lpVtbl -> get_PortID(This,pVal) ) 

#define IAccuload_put_PortID(This,newVal)	\
    ( (This)->lpVtbl -> put_PortID(This,newVal) ) 

#define IAccuload_get_NetworkCommunications(This,pVal)	\
    ( (This)->lpVtbl -> get_NetworkCommunications(This,pVal) ) 

#define IAccuload_put_NetworkCommunications(This,newVal)	\
    ( (This)->lpVtbl -> put_NetworkCommunications(This,newVal) ) 

#define IAccuload_get_IPAddress(This,pVal)	\
    ( (This)->lpVtbl -> get_IPAddress(This,pVal) ) 

#define IAccuload_put_IPAddress(This,newVal)	\
    ( (This)->lpVtbl -> put_IPAddress(This,newVal) ) 

#define IAccuload_TypeID(This,Type,pVal)	\
    ( (This)->lpVtbl -> TypeID(This,Type,pVal) ) 

#define IAccuload_Load(This,pDispatch)	\
    ( (This)->lpVtbl -> Load(This,pDispatch) ) 

#define IAccuload_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IAccuload_get_InsertSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_InsertSQL(This,pVal) ) 

#define IAccuload_get_UpdateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_UpdateSQL(This,pVal) ) 

#define IAccuload_get_PurgeSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_PurgeSQL(This,pVal) ) 

#define IAccuload_get_SelectSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectSQL(This,pVal) ) 

#define IAccuload_get_SelectByIDSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectByIDSQL(This,pVal) ) 

#define IAccuload_get_EnumerateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_EnumerateSQL(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IAccuload_INTERFACE_DEFINED__ */


#ifndef __IAcculoadCollection_INTERFACE_DEFINED__
#define __IAcculoadCollection_INTERFACE_DEFINED__

/* interface IAcculoadCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IAcculoadCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D8D7DC94-F668-44A1-B378-014B96D9D872")
    IAcculoadCollection : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ int Index,
            /* [retval][out] */ IAccuload **pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ IAccuload *pNewVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ long *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ int Index) = 0;
        
        virtual /* [restricted][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ IUnknown **pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IAcculoadCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IAcculoadCollection * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IAcculoadCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IAcculoadCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IAcculoadCollection * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IAcculoadCollection * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IAcculoadCollection * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IAcculoadCollection * This,
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
            IAcculoadCollection * This,
            /* [in] */ int Index,
            /* [retval][out] */ IAccuload **pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            IAcculoadCollection * This,
            /* [in] */ IAccuload *pNewVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            IAcculoadCollection * This,
            /* [retval][out] */ long *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            IAcculoadCollection * This,
            /* [in] */ int Index);
        
        /* [restricted][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            IAcculoadCollection * This,
            /* [retval][out] */ IUnknown **pVal);
        
        END_INTERFACE
    } IAcculoadCollectionVtbl;

    interface IAcculoadCollection
    {
        CONST_VTBL struct IAcculoadCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAcculoadCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IAcculoadCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IAcculoadCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IAcculoadCollection_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IAcculoadCollection_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IAcculoadCollection_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IAcculoadCollection_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IAcculoadCollection_Item(This,Index,pVal)	\
    ( (This)->lpVtbl -> Item(This,Index,pVal) ) 

#define IAcculoadCollection_Add(This,pNewVal)	\
    ( (This)->lpVtbl -> Add(This,pNewVal) ) 

#define IAcculoadCollection_get_Count(This,pVal)	\
    ( (This)->lpVtbl -> get_Count(This,pVal) ) 

#define IAcculoadCollection_Remove(This,Index)	\
    ( (This)->lpVtbl -> Remove(This,Index) ) 

#define IAcculoadCollection_get__NewEnum(This,pVal)	\
    ( (This)->lpVtbl -> get__NewEnum(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IAcculoadCollection_INTERFACE_DEFINED__ */


#ifndef __IArm_INTERFACE_DEFINED__
#define __IArm_INTERFACE_DEFINED__

/* interface IArm */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IArm;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E745F9FB-6EA9-45A9-BEF3-1158BF525CA9")
    IArm : public IDispatch
    {
    public:
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Index( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Index( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_AcculoadIndex( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_AcculoadIndex( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Number( 
            /* [retval][out] */ unsigned char *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Number( 
            /* [in] */ unsigned char newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Address( 
            /* [retval][out] */ unsigned char *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Address( 
            /* [in] */ unsigned char newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Type( 
            /* [retval][out] */ ACCULOAD_ARM_TYPE *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Type( 
            /* [in] */ ACCULOAD_ARM_TYPE newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Products( 
            /* [retval][out] */ unsigned char *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Products( 
            /* [in] */ unsigned char newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE TypeID( 
            /* [in] */ ACCULOAD_ARM_TYPE Type,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Load( 
            /* [in] */ IDispatch *pRecordset) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Reset( void) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_InsertSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_UpdateSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PurgeSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SelectSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_SelectByAcculoadIndexAndNumberSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_EnumerateByAcculoadIndexSQL( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IArmVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IArm * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IArm * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IArm * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IArm * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IArm * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IArm * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IArm * This,
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
            IArm * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Index )( 
            IArm * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AcculoadIndex )( 
            IArm * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_AcculoadIndex )( 
            IArm * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Number )( 
            IArm * This,
            /* [retval][out] */ unsigned char *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Number )( 
            IArm * This,
            /* [in] */ unsigned char newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Address )( 
            IArm * This,
            /* [retval][out] */ unsigned char *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Address )( 
            IArm * This,
            /* [in] */ unsigned char newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            IArm * This,
            /* [retval][out] */ ACCULOAD_ARM_TYPE *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            IArm * This,
            /* [in] */ ACCULOAD_ARM_TYPE newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Products )( 
            IArm * This,
            /* [retval][out] */ unsigned char *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Products )( 
            IArm * This,
            /* [in] */ unsigned char newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *TypeID )( 
            IArm * This,
            /* [in] */ ACCULOAD_ARM_TYPE Type,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Load )( 
            IArm * This,
            /* [in] */ IDispatch *pRecordset);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IArm * This);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_InsertSQL )( 
            IArm * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UpdateSQL )( 
            IArm * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PurgeSQL )( 
            IArm * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectSQL )( 
            IArm * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectByAcculoadIndexAndNumberSQL )( 
            IArm * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnumerateByAcculoadIndexSQL )( 
            IArm * This,
            /* [retval][out] */ BSTR *pVal);
        
        END_INTERFACE
    } IArmVtbl;

    interface IArm
    {
        CONST_VTBL struct IArmVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IArm_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IArm_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IArm_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IArm_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IArm_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IArm_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IArm_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IArm_get_Index(This,pVal)	\
    ( (This)->lpVtbl -> get_Index(This,pVal) ) 

#define IArm_put_Index(This,newVal)	\
    ( (This)->lpVtbl -> put_Index(This,newVal) ) 

#define IArm_get_AcculoadIndex(This,pVal)	\
    ( (This)->lpVtbl -> get_AcculoadIndex(This,pVal) ) 

#define IArm_put_AcculoadIndex(This,newVal)	\
    ( (This)->lpVtbl -> put_AcculoadIndex(This,newVal) ) 

#define IArm_get_Number(This,pVal)	\
    ( (This)->lpVtbl -> get_Number(This,pVal) ) 

#define IArm_put_Number(This,newVal)	\
    ( (This)->lpVtbl -> put_Number(This,newVal) ) 

#define IArm_get_Address(This,pVal)	\
    ( (This)->lpVtbl -> get_Address(This,pVal) ) 

#define IArm_put_Address(This,newVal)	\
    ( (This)->lpVtbl -> put_Address(This,newVal) ) 

#define IArm_get_Type(This,pVal)	\
    ( (This)->lpVtbl -> get_Type(This,pVal) ) 

#define IArm_put_Type(This,newVal)	\
    ( (This)->lpVtbl -> put_Type(This,newVal) ) 

#define IArm_get_Products(This,pVal)	\
    ( (This)->lpVtbl -> get_Products(This,pVal) ) 

#define IArm_put_Products(This,newVal)	\
    ( (This)->lpVtbl -> put_Products(This,newVal) ) 

#define IArm_TypeID(This,Type,pVal)	\
    ( (This)->lpVtbl -> TypeID(This,Type,pVal) ) 

#define IArm_Load(This,pRecordset)	\
    ( (This)->lpVtbl -> Load(This,pRecordset) ) 

#define IArm_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IArm_get_InsertSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_InsertSQL(This,pVal) ) 

#define IArm_get_UpdateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_UpdateSQL(This,pVal) ) 

#define IArm_get_PurgeSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_PurgeSQL(This,pVal) ) 

#define IArm_get_SelectSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectSQL(This,pVal) ) 

#define IArm_get_SelectByAcculoadIndexAndNumberSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectByAcculoadIndexAndNumberSQL(This,pVal) ) 

#define IArm_get_EnumerateByAcculoadIndexSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_EnumerateByAcculoadIndexSQL(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IArm_INTERFACE_DEFINED__ */


#ifndef __IArmCollection_INTERFACE_DEFINED__
#define __IArmCollection_INTERFACE_DEFINED__

/* interface IArmCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IArmCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4F575503-5589-4230-88E3-D5393BDFCF57")
    IArmCollection : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ int Index,
            /* [retval][out] */ IArm **pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ IArm *pNewVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ long *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ int Index) = 0;
        
        virtual /* [restricted][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ IUnknown **pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IArmCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IArmCollection * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IArmCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IArmCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IArmCollection * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IArmCollection * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IArmCollection * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IArmCollection * This,
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
            IArmCollection * This,
            /* [in] */ int Index,
            /* [retval][out] */ IArm **pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            IArmCollection * This,
            /* [in] */ IArm *pNewVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            IArmCollection * This,
            /* [retval][out] */ long *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            IArmCollection * This,
            /* [in] */ int Index);
        
        /* [restricted][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            IArmCollection * This,
            /* [retval][out] */ IUnknown **pVal);
        
        END_INTERFACE
    } IArmCollectionVtbl;

    interface IArmCollection
    {
        CONST_VTBL struct IArmCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IArmCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IArmCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IArmCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IArmCollection_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IArmCollection_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IArmCollection_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IArmCollection_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IArmCollection_Item(This,Index,pVal)	\
    ( (This)->lpVtbl -> Item(This,Index,pVal) ) 

#define IArmCollection_Add(This,pNewVal)	\
    ( (This)->lpVtbl -> Add(This,pNewVal) ) 

#define IArmCollection_get_Count(This,pVal)	\
    ( (This)->lpVtbl -> get_Count(This,pVal) ) 

#define IArmCollection_Remove(This,Index)	\
    ( (This)->lpVtbl -> Remove(This,Index) ) 

#define IArmCollection_get__NewEnum(This,pVal)	\
    ( (This)->lpVtbl -> get__NewEnum(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IArmCollection_INTERFACE_DEFINED__ */


#ifndef __ICardReader_INTERFACE_DEFINED__
#define __ICardReader_INTERFACE_DEFINED__

/* interface ICardReader */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_ICardReader;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D37415A8-A938-4A16-9A56-1A77DAB8D0E0")
    ICardReader : public IDispatch
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
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Address( 
            /* [retval][out] */ unsigned char *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Address( 
            /* [in] */ unsigned char newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PortID( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PortID( 
            /* [in] */ BSTR newVal) = 0;
        
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

    typedef struct ICardReaderVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICardReader * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICardReader * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICardReader * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ICardReader * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ICardReader * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ICardReader * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ICardReader * This,
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
            ICardReader * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Index )( 
            ICardReader * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            ICardReader * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ID )( 
            ICardReader * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PortIndex )( 
            ICardReader * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PortIndex )( 
            ICardReader * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Address )( 
            ICardReader * This,
            /* [retval][out] */ unsigned char *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Address )( 
            ICardReader * This,
            /* [in] */ unsigned char newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PortID )( 
            ICardReader * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PortID )( 
            ICardReader * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Load )( 
            ICardReader * This,
            /* [in] */ IDispatch *pDispatch);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Reset )( 
            ICardReader * This);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_InsertSQL )( 
            ICardReader * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UpdateSQL )( 
            ICardReader * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PurgeSQL )( 
            ICardReader * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectSQL )( 
            ICardReader * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectByIDSQL )( 
            ICardReader * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnumerateSQL )( 
            ICardReader * This,
            /* [retval][out] */ BSTR *pVal);
        
        END_INTERFACE
    } ICardReaderVtbl;

    interface ICardReader
    {
        CONST_VTBL struct ICardReaderVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICardReader_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICardReader_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICardReader_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICardReader_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ICardReader_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ICardReader_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ICardReader_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ICardReader_get_Index(This,pVal)	\
    ( (This)->lpVtbl -> get_Index(This,pVal) ) 

#define ICardReader_put_Index(This,newVal)	\
    ( (This)->lpVtbl -> put_Index(This,newVal) ) 

#define ICardReader_get_ID(This,pVal)	\
    ( (This)->lpVtbl -> get_ID(This,pVal) ) 

#define ICardReader_put_ID(This,newVal)	\
    ( (This)->lpVtbl -> put_ID(This,newVal) ) 

#define ICardReader_get_PortIndex(This,pVal)	\
    ( (This)->lpVtbl -> get_PortIndex(This,pVal) ) 

#define ICardReader_put_PortIndex(This,newVal)	\
    ( (This)->lpVtbl -> put_PortIndex(This,newVal) ) 

#define ICardReader_get_Address(This,pVal)	\
    ( (This)->lpVtbl -> get_Address(This,pVal) ) 

#define ICardReader_put_Address(This,newVal)	\
    ( (This)->lpVtbl -> put_Address(This,newVal) ) 

#define ICardReader_get_PortID(This,pVal)	\
    ( (This)->lpVtbl -> get_PortID(This,pVal) ) 

#define ICardReader_put_PortID(This,newVal)	\
    ( (This)->lpVtbl -> put_PortID(This,newVal) ) 

#define ICardReader_Load(This,pDispatch)	\
    ( (This)->lpVtbl -> Load(This,pDispatch) ) 

#define ICardReader_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define ICardReader_get_InsertSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_InsertSQL(This,pVal) ) 

#define ICardReader_get_UpdateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_UpdateSQL(This,pVal) ) 

#define ICardReader_get_PurgeSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_PurgeSQL(This,pVal) ) 

#define ICardReader_get_SelectSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectSQL(This,pVal) ) 

#define ICardReader_get_SelectByIDSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectByIDSQL(This,pVal) ) 

#define ICardReader_get_EnumerateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_EnumerateSQL(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICardReader_INTERFACE_DEFINED__ */


#ifndef __ICardReaderCollection_INTERFACE_DEFINED__
#define __ICardReaderCollection_INTERFACE_DEFINED__

/* interface ICardReaderCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_ICardReaderCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5F78587F-57CD-46C6-B1B8-73502025D983")
    ICardReaderCollection : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ int Index,
            /* [retval][out] */ ICardReader **pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ ICardReader *pNewVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ long *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ int Index) = 0;
        
        virtual /* [restricted][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ IUnknown **pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ICardReaderCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICardReaderCollection * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICardReaderCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICardReaderCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ICardReaderCollection * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ICardReaderCollection * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ICardReaderCollection * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ICardReaderCollection * This,
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
            ICardReaderCollection * This,
            /* [in] */ int Index,
            /* [retval][out] */ ICardReader **pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            ICardReaderCollection * This,
            /* [in] */ ICardReader *pNewVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            ICardReaderCollection * This,
            /* [retval][out] */ long *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            ICardReaderCollection * This,
            /* [in] */ int Index);
        
        /* [restricted][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            ICardReaderCollection * This,
            /* [retval][out] */ IUnknown **pVal);
        
        END_INTERFACE
    } ICardReaderCollectionVtbl;

    interface ICardReaderCollection
    {
        CONST_VTBL struct ICardReaderCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICardReaderCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICardReaderCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICardReaderCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICardReaderCollection_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ICardReaderCollection_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ICardReaderCollection_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ICardReaderCollection_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ICardReaderCollection_Item(This,Index,pVal)	\
    ( (This)->lpVtbl -> Item(This,Index,pVal) ) 

#define ICardReaderCollection_Add(This,pNewVal)	\
    ( (This)->lpVtbl -> Add(This,pNewVal) ) 

#define ICardReaderCollection_get_Count(This,pVal)	\
    ( (This)->lpVtbl -> get_Count(This,pVal) ) 

#define ICardReaderCollection_Remove(This,Index)	\
    ( (This)->lpVtbl -> Remove(This,Index) ) 

#define ICardReaderCollection_get__NewEnum(This,pVal)	\
    ( (This)->lpVtbl -> get__NewEnum(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICardReaderCollection_INTERFACE_DEFINED__ */


#ifndef __IPort_INTERFACE_DEFINED__
#define __IPort_INTERFACE_DEFINED__

/* interface IPort */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPort;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2C637431-EC5C-4B67-8B78-ABC946AE671A")
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
            /* [retval][out] */ ACCULOAD_BAUD *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Baud( 
            /* [in] */ ACCULOAD_BAUD newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DataBits( 
            /* [retval][out] */ ACCULOAD_DATA_BITS *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DataBits( 
            /* [in] */ ACCULOAD_DATA_BITS newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Parity( 
            /* [retval][out] */ ACCULOAD_PARITY *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Parity( 
            /* [in] */ ACCULOAD_PARITY newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StopBits( 
            /* [retval][out] */ ACCULOAD_STOP_BITS *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StopBits( 
            /* [in] */ ACCULOAD_STOP_BITS newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE BaudID( 
            /* [in] */ ACCULOAD_BAUD Baud,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE DataBitsID( 
            /* [in] */ ACCULOAD_DATA_BITS DataBits,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ParityID( 
            /* [in] */ ACCULOAD_PARITY Parity,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE StopBitsID( 
            /* [in] */ ACCULOAD_STOP_BITS StopBits,
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
            /* [retval][out] */ ACCULOAD_BAUD *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Baud )( 
            IPort * This,
            /* [in] */ ACCULOAD_BAUD newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DataBits )( 
            IPort * This,
            /* [retval][out] */ ACCULOAD_DATA_BITS *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DataBits )( 
            IPort * This,
            /* [in] */ ACCULOAD_DATA_BITS newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Parity )( 
            IPort * This,
            /* [retval][out] */ ACCULOAD_PARITY *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Parity )( 
            IPort * This,
            /* [in] */ ACCULOAD_PARITY newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StopBits )( 
            IPort * This,
            /* [retval][out] */ ACCULOAD_STOP_BITS *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StopBits )( 
            IPort * This,
            /* [in] */ ACCULOAD_STOP_BITS newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *BaudID )( 
            IPort * This,
            /* [in] */ ACCULOAD_BAUD Baud,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *DataBitsID )( 
            IPort * This,
            /* [in] */ ACCULOAD_DATA_BITS DataBits,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *ParityID )( 
            IPort * This,
            /* [in] */ ACCULOAD_PARITY Parity,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *StopBitsID )( 
            IPort * This,
            /* [in] */ ACCULOAD_STOP_BITS StopBits,
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
    
    MIDL_INTERFACE("D3ECA1A0-D582-40EA-AD38-BA6516F6DE1D")
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



#ifndef __AcculoadOPCObjectsLib_LIBRARY_DEFINED__
#define __AcculoadOPCObjectsLib_LIBRARY_DEFINED__

/* library AcculoadOPCObjectsLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_AcculoadOPCObjectsLib;

EXTERN_C const CLSID CLSID_Accuload;

#ifdef __cplusplus

class DECLSPEC_UUID("6C1646B1-E5C0-4EF8-A454-8828BAD2CD07")
Accuload;
#endif

EXTERN_C const CLSID CLSID_AcculoadCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("BB618EBC-0E8F-4797-BFFB-46A2C1473D3E")
AcculoadCollection;
#endif

EXTERN_C const CLSID CLSID_Arm;

#ifdef __cplusplus

class DECLSPEC_UUID("29EFEBEE-2CB2-4D09-8A2F-8225796DBAB7")
Arm;
#endif

EXTERN_C const CLSID CLSID_ArmCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("FBFF08AB-D838-41FB-ACB3-6CA87D4BF5FD")
ArmCollection;
#endif

EXTERN_C const CLSID CLSID_CardReader;

#ifdef __cplusplus

class DECLSPEC_UUID("EF6E300C-FCF7-46E9-92D2-C4033F1747A9")
CardReader;
#endif

EXTERN_C const CLSID CLSID_CardReaderCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("1C116B95-F1E9-4D4A-AF69-4D6B05763FA7")
CardReaderCollection;
#endif

EXTERN_C const CLSID CLSID_Port;

#ifdef __cplusplus

class DECLSPEC_UUID("71C97AF2-71F4-424B-914E-035C066E0975")
Port;
#endif

EXTERN_C const CLSID CLSID_PortCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("37D1D842-8F5C-4314-ACBE-AC540626B9C9")
PortCollection;
#endif
#endif /* __AcculoadOPCObjectsLib_LIBRARY_DEFINED__ */

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


