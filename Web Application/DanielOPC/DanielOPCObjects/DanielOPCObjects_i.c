

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for DanielOPCObjects.idl:
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

MIDL_DEFINE_GUID(IID, IID_IPort,0x24DEED9D,0x30F2,0x489E,0x87,0xB3,0x96,0x4D,0x42,0x8E,0x42,0xE9);


MIDL_DEFINE_GUID(IID, IID_IDanLoad,0x4C6547E4,0xDFA5,0x4355,0xAF,0x4D,0xA9,0x7E,0x64,0x1B,0x7E,0xD8);


MIDL_DEFINE_GUID(IID, IID_IPortCollection,0x2AEBF73A,0x01C7,0x4A92,0xBB,0x3F,0x0C,0x36,0x6F,0xA0,0x29,0xB0);


MIDL_DEFINE_GUID(IID, IID_IDanLoadCollection,0x1F782045,0x9027,0x4D15,0xBD,0xEC,0x99,0x94,0x98,0x5F,0x2A,0xEE);


MIDL_DEFINE_GUID(IID, LIBID_DanielOPCObjectsLib,0xD783BBFF,0x74D7,0x4466,0xA2,0xA8,0xA8,0x97,0xAE,0x45,0x80,0x13);


MIDL_DEFINE_GUID(CLSID, CLSID_Port,0xB140A6E9,0xE0DD,0x4C4F,0xB9,0x73,0x63,0x0C,0xCA,0x43,0x13,0x6D);


MIDL_DEFINE_GUID(CLSID, CLSID_DanLoad,0xB32780E5,0x1D56,0x4A71,0x8C,0x4C,0xFF,0xB8,0xC7,0x42,0x88,0x88);


MIDL_DEFINE_GUID(CLSID, CLSID_PortCollection,0x93ED554D,0xDE90,0x4090,0xA9,0xE6,0x8C,0xA8,0x9F,0x97,0xEA,0xD1);


MIDL_DEFINE_GUID(CLSID, CLSID_DanLoadCollection,0x0CD850DB,0x6A02,0x4AF3,0xAA,0xED,0x8F,0xE6,0xF7,0xCF,0xFA,0x31);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



