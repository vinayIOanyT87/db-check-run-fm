

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for ScullyOPCServer.idl:
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

MIDL_DEFINE_GUID(IID, IID_IScullys,0x3922F2C9,0xA5DB,0x4f67,0xA6,0xA0,0xA8,0x40,0xB7,0x33,0x30,0x9C);


MIDL_DEFINE_GUID(IID, IID_IDataAccess,0xC4C45BB0,0x49E0,0x4813,0xA8,0x7D,0xF6,0xD2,0xDE,0x1D,0x46,0x1C);


MIDL_DEFINE_GUID(IID, IID_IPorts,0xD0E7E3F3,0xEEE7,0x440e,0xBA,0x80,0x14,0xD1,0x83,0x50,0x90,0xA2);


MIDL_DEFINE_GUID(IID, LIBID_ScullyOPCServerLib,0xB93AB8CF,0xD487,0x4d1c,0x92,0xB3,0x28,0x0A,0x77,0x3C,0xC3,0x6C);


MIDL_DEFINE_GUID(CLSID, CLSID_Scullys,0x948DA86B,0xA687,0x494c,0x9B,0x93,0x56,0x9B,0x65,0x49,0x9B,0x36);


MIDL_DEFINE_GUID(CLSID, CLSID_DataAccess,0x1F341EE6,0xE351,0x4fae,0xBE,0xDD,0x30,0xA8,0x6A,0x80,0x4B,0x4E);


MIDL_DEFINE_GUID(CLSID, CLSID_Ports,0xBF99140E,0xF916,0x49c2,0x95,0x41,0x61,0xBD,0xD7,0x5E,0x45,0x31);


MIDL_DEFINE_GUID(CLSID, CLSID_OPCServer,0x206D99CF,0x6189,0x4440,0xAB,0x4C,0x74,0xDA,0xEB,0xCF,0xC8,0xFE);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



