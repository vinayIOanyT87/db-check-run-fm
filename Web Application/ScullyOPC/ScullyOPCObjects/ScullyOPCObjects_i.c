

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


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

MIDL_DEFINE_GUID(IID, IID_IPort,0x36992C1C,0x5543,0x4574,0xB9,0xFD,0x42,0x6E,0xC2,0xAF,0xA1,0x32);


MIDL_DEFINE_GUID(IID, IID_IScully,0xD3A59F43,0xE032,0x4bd9,0xA5,0x29,0x81,0x69,0xAC,0xE6,0xF6,0x4B);


MIDL_DEFINE_GUID(IID, IID_IPortCollection,0x1F6D57D0,0x075C,0x4de2,0xB7,0xA6,0x12,0x45,0xFC,0x16,0x16,0xC1);


MIDL_DEFINE_GUID(IID, IID_IScullyCollection,0x5746EEC1,0x7306,0x420a,0x93,0x05,0x20,0x9F,0x39,0xC2,0xA9,0xD8);


MIDL_DEFINE_GUID(IID, LIBID_ScullyOPCObjectsLib,0x1EE60832,0x5723,0x4c9d,0x8D,0x2A,0x0C,0xD8,0x8A,0x62,0x81,0x5B);


MIDL_DEFINE_GUID(CLSID, CLSID_Port,0x465A30A8,0x5978,0x4948,0xA5,0xDB,0x31,0xEB,0x09,0xAA,0xCC,0xD1);


MIDL_DEFINE_GUID(CLSID, CLSID_Scully,0x33772EBA,0xF1EF,0x49f5,0xBD,0x16,0xA0,0xAE,0x9E,0x60,0xA4,0xDB);


MIDL_DEFINE_GUID(CLSID, CLSID_PortCollection,0x9C08C25E,0x50C2,0x451a,0x9E,0xA3,0xE8,0x4C,0x58,0x9B,0x80,0x74);


MIDL_DEFINE_GUID(CLSID, CLSID_ScullyCollection,0xC48047CC,0xEA3B,0x464a,0x93,0x06,0x89,0x78,0x58,0x73,0x39,0x4F);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



