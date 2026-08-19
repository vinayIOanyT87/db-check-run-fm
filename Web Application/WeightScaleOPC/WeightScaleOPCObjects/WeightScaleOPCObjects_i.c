

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


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

MIDL_DEFINE_GUID(IID, IID_IPort,0x24DEED9D,0x30F2,0x489E,0x87,0xB3,0x98,0x4D,0x42,0x8E,0x42,0xE9);


MIDL_DEFINE_GUID(IID, IID_IWeightScale,0xD18CE9A3,0x9851,0x4EDD,0x99,0x64,0x68,0x93,0x8A,0x5D,0x6C,0xFB);


MIDL_DEFINE_GUID(IID, IID_IPortCollection,0x2AEBF73A,0x01C7,0x4A92,0xBB,0x3F,0x0D,0x46,0x6F,0xA0,0x29,0xB0);


MIDL_DEFINE_GUID(IID, IID_IWeightScaleCollection,0x5B148EFF,0xE2D2,0x4F6B,0xA2,0xE8,0xC1,0x6C,0x55,0x19,0xF6,0x4B);


MIDL_DEFINE_GUID(IID, LIBID_WeightScaleOPCObjectsLib,0xC65A4132,0xE6B2,0x46B4,0x8E,0xA3,0xE6,0xBB,0xCC,0xB1,0x0B,0x58);


MIDL_DEFINE_GUID(CLSID, CLSID_Port,0xB140A6E9,0xE0DD,0x4C4F,0xB9,0x73,0x73,0x0C,0xCA,0x43,0x13,0x6D);


MIDL_DEFINE_GUID(CLSID, CLSID_WeightScale,0xC20933A0,0xBBE4,0x42D9,0x9A,0xAC,0x05,0x32,0xB2,0xFD,0x88,0x07);


MIDL_DEFINE_GUID(CLSID, CLSID_PortCollection,0x93ED554D,0xDE90,0x4090,0xA9,0xE6,0x9C,0xA8,0x9F,0x97,0xEA,0xD1);


MIDL_DEFINE_GUID(CLSID, CLSID_WeightScaleCollection,0xB47EA5BF,0x06E8,0x441C,0xBA,0xDE,0x43,0xE0,0xD6,0xCE,0x34,0x7D);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



