

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for ContrecOPCObjects.idl:
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

#ifndef __ContrecOPCObjects_h__
#define __ContrecOPCObjects_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IPort_FWD_DEFINED__
#define __IPort_FWD_DEFINED__
typedef interface IPort IPort;

#endif 	/* __IPort_FWD_DEFINED__ */


#ifndef __IContrec_FWD_DEFINED__
#define __IContrec_FWD_DEFINED__
typedef interface IContrec IContrec;

#endif 	/* __IContrec_FWD_DEFINED__ */


#ifndef __IPortCollection_FWD_DEFINED__
#define __IPortCollection_FWD_DEFINED__
typedef interface IPortCollection IPortCollection;

#endif 	/* __IPortCollection_FWD_DEFINED__ */


#ifndef __IContrecCollection_FWD_DEFINED__
#define __IContrecCollection_FWD_DEFINED__
typedef interface IContrecCollection IContrecCollection;

#endif 	/* __IContrecCollection_FWD_DEFINED__ */


#ifndef __Port_FWD_DEFINED__
#define __Port_FWD_DEFINED__

#ifdef __cplusplus
typedef class Port Port;
#else
typedef struct Port Port;
#endif /* __cplusplus */

#endif 	/* __Port_FWD_DEFINED__ */


#ifndef __Contrec_FWD_DEFINED__
#define __Contrec_FWD_DEFINED__

#ifdef __cplusplus
typedef class Contrec Contrec;
#else
typedef struct Contrec Contrec;
#endif /* __cplusplus */

#endif 	/* __Contrec_FWD_DEFINED__ */


#ifndef __PortCollection_FWD_DEFINED__
#define __PortCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class PortCollection PortCollection;
#else
typedef struct PortCollection PortCollection;
#endif /* __cplusplus */

#endif 	/* __PortCollection_FWD_DEFINED__ */


#ifndef __ContrecCollection_FWD_DEFINED__
#define __ContrecCollection_FWD_DEFINED__

#ifdef __cplusplus
typedef class ContrecCollection ContrecCollection;
#else
typedef struct ContrecCollection ContrecCollection;
#endif /* __cplusplus */

#endif 	/* __ContrecCollection_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_ContrecOPCObjects_0000_0000 */
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
enum CONTREC_TYPE
    {
        CONTREC1010	= 0,
        MAX_CONTREC_TYPE	= 1
    } 	CONTREC_TYPE;

typedef 
enum CONTREC_BAUD
    {
        CONTREC_BAUD_1200	= 0,
        CONTREC_BAUD_2400	= 1,
        CONTREC_BAUD_4800	= 2,
        CONTREC_BAUD_9600	= 3,
        CONTREC_BAUD_19200	= 4,
        CONTREC_BAUD_38400	= 5,
        MAX_CONTREC_BAUD	= 6
    } 	CONTREC_BAUD;

typedef 
enum CONTREC_PARITY
    {
        CONTREC_PARITY_NONE	= 0,
        CONTREC_PARITY_EVEN	= 1,
        CONTREC_PARITY_ODD	= 2,
        MAX_CONTREC_PARITY	= 3
    } 	CONTREC_PARITY;

typedef 
enum CONTREC_DATA_BITS
    {
        DATA_BITS_7	= 0,
        DATA_BITS_8	= 1,
        MAX_CONTREC_DATA_BITS	= 2
    } 	CONTREC_DATA_BITS;

typedef 
enum CONTREC_STOP_BITS
    {
        STOP_BITS_1	= 0,
        STOP_BITS_2	= 1,
        MAX_CONTREC_STOP_BITS	= 2
    } 	CONTREC_STOP_BITS;



extern RPC_IF_HANDLE __MIDL_itf_ContrecOPCObjects_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ContrecOPCObjects_0000_0000_v0_0_s_ifspec;

#ifndef __IPort_INTERFACE_DEFINED__
#define __IPort_INTERFACE_DEFINED__

/* interface IPort */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPort;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A936832E-3F54-4645-8D8F-FABFD66DA0B8")
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
            /* [retval][out] */ CONTREC_BAUD *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Baud( 
            /* [in] */ CONTREC_BAUD newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_DataBits( 
            /* [retval][out] */ CONTREC_DATA_BITS *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_DataBits( 
            /* [in] */ CONTREC_DATA_BITS newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Parity( 
            /* [retval][out] */ CONTREC_PARITY *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Parity( 
            /* [in] */ CONTREC_PARITY newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_StopBits( 
            /* [retval][out] */ CONTREC_STOP_BITS *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_StopBits( 
            /* [in] */ CONTREC_STOP_BITS newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE BaudID( 
            /* [in] */ CONTREC_BAUD Baud,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE DataBitsID( 
            /* [in] */ CONTREC_DATA_BITS DataBits,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ParityID( 
            /* [in] */ CONTREC_PARITY Parity,
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE StopBitsID( 
            /* [in] */ CONTREC_STOP_BITS StopBits,
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
            /* [retval][out] */ CONTREC_BAUD *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Baud )( 
            IPort * This,
            /* [in] */ CONTREC_BAUD newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_DataBits )( 
            IPort * This,
            /* [retval][out] */ CONTREC_DATA_BITS *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_DataBits )( 
            IPort * This,
            /* [in] */ CONTREC_DATA_BITS newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Parity )( 
            IPort * This,
            /* [retval][out] */ CONTREC_PARITY *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Parity )( 
            IPort * This,
            /* [in] */ CONTREC_PARITY newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_StopBits )( 
            IPort * This,
            /* [retval][out] */ CONTREC_STOP_BITS *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_StopBits )( 
            IPort * This,
            /* [in] */ CONTREC_STOP_BITS newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *BaudID )( 
            IPort * This,
            /* [in] */ CONTREC_BAUD Baud,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *DataBitsID )( 
            IPort * This,
            /* [in] */ CONTREC_DATA_BITS DataBits,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *ParityID )( 
            IPort * This,
            /* [in] */ CONTREC_PARITY Parity,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *StopBitsID )( 
            IPort * This,
            /* [in] */ CONTREC_STOP_BITS StopBits,
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


#ifndef __IContrec_INTERFACE_DEFINED__
#define __IContrec_INTERFACE_DEFINED__

/* interface IContrec */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IContrec;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("467BEDE3-088B-4D08-985F-F5EA5456C221")
    IContrec : public IDispatch
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
            /* [retval][out] */ CONTREC_TYPE *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Type( 
            /* [in] */ CONTREC_TYPE newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PortIndex( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PortIndex( 
            /* [in] */ LONG newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_PortID( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_PortID( 
            /* [in] */ BSTR newVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Address( 
            /* [retval][out] */ unsigned char *pVal) = 0;
        
        virtual /* [helpstring][id][propput] */ HRESULT STDMETHODCALLTYPE put_Address( 
            /* [in] */ unsigned char newVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE TypeID( 
            /* [in] */ CONTREC_TYPE Type,
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

    typedef struct IContrecVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IContrec * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IContrec * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IContrec * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IContrec * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IContrec * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IContrec * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IContrec * This,
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
            IContrec * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Index )( 
            IContrec * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ID )( 
            IContrec * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_ID )( 
            IContrec * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Type )( 
            IContrec * This,
            /* [retval][out] */ CONTREC_TYPE *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Type )( 
            IContrec * This,
            /* [in] */ CONTREC_TYPE newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PortIndex )( 
            IContrec * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PortIndex )( 
            IContrec * This,
            /* [in] */ LONG newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PortID )( 
            IContrec * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_PortID )( 
            IContrec * This,
            /* [in] */ BSTR newVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Address )( 
            IContrec * This,
            /* [retval][out] */ unsigned char *pVal);
        
        /* [helpstring][id][propput] */ HRESULT ( STDMETHODCALLTYPE *put_Address )( 
            IContrec * This,
            /* [in] */ unsigned char newVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *TypeID )( 
            IContrec * This,
            /* [in] */ CONTREC_TYPE Type,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Load )( 
            IContrec * This,
            /* [in] */ IDispatch *pDispatch);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Reset )( 
            IContrec * This);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_InsertSQL )( 
            IContrec * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_UpdateSQL )( 
            IContrec * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PurgeSQL )( 
            IContrec * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectSQL )( 
            IContrec * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_SelectByIDSQL )( 
            IContrec * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_EnumerateSQL )( 
            IContrec * This,
            /* [retval][out] */ BSTR *pVal);
        
        END_INTERFACE
    } IContrecVtbl;

    interface IContrec
    {
        CONST_VTBL struct IContrecVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IContrec_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IContrec_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IContrec_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IContrec_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IContrec_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IContrec_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IContrec_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IContrec_get_Index(This,pVal)	\
    ( (This)->lpVtbl -> get_Index(This,pVal) ) 

#define IContrec_put_Index(This,newVal)	\
    ( (This)->lpVtbl -> put_Index(This,newVal) ) 

#define IContrec_get_ID(This,pVal)	\
    ( (This)->lpVtbl -> get_ID(This,pVal) ) 

#define IContrec_put_ID(This,newVal)	\
    ( (This)->lpVtbl -> put_ID(This,newVal) ) 

#define IContrec_get_Type(This,pVal)	\
    ( (This)->lpVtbl -> get_Type(This,pVal) ) 

#define IContrec_put_Type(This,newVal)	\
    ( (This)->lpVtbl -> put_Type(This,newVal) ) 

#define IContrec_get_PortIndex(This,pVal)	\
    ( (This)->lpVtbl -> get_PortIndex(This,pVal) ) 

#define IContrec_put_PortIndex(This,newVal)	\
    ( (This)->lpVtbl -> put_PortIndex(This,newVal) ) 

#define IContrec_get_PortID(This,pVal)	\
    ( (This)->lpVtbl -> get_PortID(This,pVal) ) 

#define IContrec_put_PortID(This,newVal)	\
    ( (This)->lpVtbl -> put_PortID(This,newVal) ) 

#define IContrec_get_Address(This,pVal)	\
    ( (This)->lpVtbl -> get_Address(This,pVal) ) 

#define IContrec_put_Address(This,newVal)	\
    ( (This)->lpVtbl -> put_Address(This,newVal) ) 

#define IContrec_TypeID(This,Type,pVal)	\
    ( (This)->lpVtbl -> TypeID(This,Type,pVal) ) 

#define IContrec_Load(This,pDispatch)	\
    ( (This)->lpVtbl -> Load(This,pDispatch) ) 

#define IContrec_Reset(This)	\
    ( (This)->lpVtbl -> Reset(This) ) 

#define IContrec_get_InsertSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_InsertSQL(This,pVal) ) 

#define IContrec_get_UpdateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_UpdateSQL(This,pVal) ) 

#define IContrec_get_PurgeSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_PurgeSQL(This,pVal) ) 

#define IContrec_get_SelectSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectSQL(This,pVal) ) 

#define IContrec_get_SelectByIDSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_SelectByIDSQL(This,pVal) ) 

#define IContrec_get_EnumerateSQL(This,pVal)	\
    ( (This)->lpVtbl -> get_EnumerateSQL(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IContrec_INTERFACE_DEFINED__ */


#ifndef __IPortCollection_INTERFACE_DEFINED__
#define __IPortCollection_INTERFACE_DEFINED__

/* interface IPortCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPortCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("8A5434A0-F101-4070-9263-1D3116E39EBA")
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


#ifndef __IContrecCollection_INTERFACE_DEFINED__
#define __IContrecCollection_INTERFACE_DEFINED__

/* interface IContrecCollection */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IContrecCollection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C09BAFF5-A840-4023-8E96-23B44E0FA96B")
    IContrecCollection : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Item( 
            /* [in] */ int Index,
            /* [retval][out] */ IContrec **pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Add( 
            /* [in] */ IContrec *pNewVal) = 0;
        
        virtual /* [helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get_Count( 
            /* [retval][out] */ long *pVal) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Remove( 
            /* [in] */ int Index) = 0;
        
        virtual /* [restricted][helpstring][id][propget] */ HRESULT STDMETHODCALLTYPE get__NewEnum( 
            /* [retval][out] */ IUnknown **pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IContrecCollectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IContrecCollection * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IContrecCollection * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IContrecCollection * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IContrecCollection * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IContrecCollection * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IContrecCollection * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IContrecCollection * This,
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
            IContrecCollection * This,
            /* [in] */ int Index,
            /* [retval][out] */ IContrec **pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Add )( 
            IContrecCollection * This,
            /* [in] */ IContrec *pNewVal);
        
        /* [helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Count )( 
            IContrecCollection * This,
            /* [retval][out] */ long *pVal);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Remove )( 
            IContrecCollection * This,
            /* [in] */ int Index);
        
        /* [restricted][helpstring][id][propget] */ HRESULT ( STDMETHODCALLTYPE *get__NewEnum )( 
            IContrecCollection * This,
            /* [retval][out] */ IUnknown **pVal);
        
        END_INTERFACE
    } IContrecCollectionVtbl;

    interface IContrecCollection
    {
        CONST_VTBL struct IContrecCollectionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IContrecCollection_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IContrecCollection_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IContrecCollection_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IContrecCollection_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IContrecCollection_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IContrecCollection_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IContrecCollection_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IContrecCollection_Item(This,Index,pVal)	\
    ( (This)->lpVtbl -> Item(This,Index,pVal) ) 

#define IContrecCollection_Add(This,pNewVal)	\
    ( (This)->lpVtbl -> Add(This,pNewVal) ) 

#define IContrecCollection_get_Count(This,pVal)	\
    ( (This)->lpVtbl -> get_Count(This,pVal) ) 

#define IContrecCollection_Remove(This,Index)	\
    ( (This)->lpVtbl -> Remove(This,Index) ) 

#define IContrecCollection_get__NewEnum(This,pVal)	\
    ( (This)->lpVtbl -> get__NewEnum(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IContrecCollection_INTERFACE_DEFINED__ */



#ifndef __ContrecOPCObjectsLib_LIBRARY_DEFINED__
#define __ContrecOPCObjectsLib_LIBRARY_DEFINED__

/* library ContrecOPCObjectsLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_ContrecOPCObjectsLib;

EXTERN_C const CLSID CLSID_Port;

#ifdef __cplusplus

class DECLSPEC_UUID("3C30C23A-8C48-4CC2-B992-9AFAD5394D8E")
Port;
#endif

EXTERN_C const CLSID CLSID_Contrec;

#ifdef __cplusplus

class DECLSPEC_UUID("8A6F9478-08B3-47FC-B016-4BCCF709EE8F")
Contrec;
#endif

EXTERN_C const CLSID CLSID_PortCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("3789DC85-8F6C-47B4-A01E-3029755F82E6")
PortCollection;
#endif

EXTERN_C const CLSID CLSID_ContrecCollection;

#ifdef __cplusplus

class DECLSPEC_UUID("8185E935-2D3F-4CD4-B79A-F62A62794CAD")
ContrecCollection;
#endif
#endif /* __ContrecOPCObjectsLib_LIBRARY_DEFINED__ */

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


