

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


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



#ifdef __cplusplus
extern "C"{
#endif 


#include <rpc.h>
#include <rpcndr.h>

#ifdef _MIDL_USE_GUIDDEF_

#ifndef INITGUID
#define INITGUID
#include <guiddef.h>
#undef INITGUID
#else
#include <guiddef.h>
#endif

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        DEFINE_GUID(name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8)

#else // !_MIDL_USE_GUIDDEF_

#ifndef __IID_DEFINED__
#define __IID_DEFINED__

typedef struct _IID
{
    unsigned long x;
    unsigned short s1;
    unsigned short s2;
    unsigned char  c[8];
} IID;

#endif // __IID_DEFINED__

#ifndef CLSID_DEFINED
#define CLSID_DEFINED
typedef IID CLSID;
#endif // CLSID_DEFINED

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        EXTERN_C __declspec(selectany) const type name = {l,w1,w2,{b1,b2,b3,b4,b5,b6,b7,b8}}

#endif // !_MIDL_USE_GUIDDEF_

MIDL_DEFINE_GUID(IID, IID_IPort,0xA936832E,0x3F54,0x4645,0x8D,0x8F,0xFA,0xBF,0xD6,0x6D,0xA0,0xB8);


MIDL_DEFINE_GUID(IID, IID_IContrec,0x467BEDE3,0x088B,0x4D08,0x98,0x5F,0xF5,0xEA,0x54,0x56,0xC2,0x21);


MIDL_DEFINE_GUID(IID, IID_IPortCollection,0x8A5434A0,0xF101,0x4070,0x92,0x63,0x1D,0x31,0x16,0xE3,0x9E,0xBA);


MIDL_DEFINE_GUID(IID, IID_IContrecCollection,0xC09BAFF5,0xA840,0x4023,0x8E,0x96,0x23,0xB4,0x4E,0x0F,0xA9,0x6B);


MIDL_DEFINE_GUID(IID, LIBID_ContrecOPCObjectsLib,0xC31FF61C,0x5215,0x4A9F,0xA4,0x38,0xA4,0x26,0x22,0x7D,0x15,0xA0);


MIDL_DEFINE_GUID(CLSID, CLSID_Port,0x3C30C23A,0x8C48,0x4CC2,0xB9,0x92,0x9A,0xFA,0xD5,0x39,0x4D,0x8E);


MIDL_DEFINE_GUID(CLSID, CLSID_Contrec,0x8A6F9478,0x08B3,0x47FC,0xB0,0x16,0x4B,0xCC,0xF7,0x09,0xEE,0x8F);


MIDL_DEFINE_GUID(CLSID, CLSID_PortCollection,0x3789DC85,0x8F6C,0x47B4,0xA0,0x1E,0x30,0x29,0x75,0x5F,0x82,0xE6);


MIDL_DEFINE_GUID(CLSID, CLSID_ContrecCollection,0x8185E935,0x2D3F,0x4CD4,0xB7,0x9A,0xF6,0x2A,0x62,0x79,0x4C,0xAD);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



