

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for DanielOPCServer.idl:
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

MIDL_DEFINE_GUID(IID, IID_IDataAccess,0x1218B83B,0x2946,0x41C8,0xBE,0x64,0x7A,0xF6,0x87,0x98,0x91,0x04);


MIDL_DEFINE_GUID(IID, IID_IDanLoads,0xC836DAF6,0x6EF0,0x492D,0x9D,0xB8,0x4E,0x1C,0xFD,0xE5,0x80,0x03);


MIDL_DEFINE_GUID(IID, IID_IPorts,0x6F92560C,0x9F70,0x4176,0x93,0x11,0x6A,0x0C,0x33,0x87,0xFE,0xB7);


MIDL_DEFINE_GUID(IID, LIBID_DanielOPCServerLib,0x4B9EC677,0xDB8A,0x4A49,0xB6,0x44,0x02,0xBB,0x18,0xFA,0x08,0x09);


MIDL_DEFINE_GUID(CLSID, CLSID_DataAccess,0x6FFB871D,0xD415,0x48BA,0x87,0x65,0xC6,0x7C,0x60,0x02,0x50,0x28);


MIDL_DEFINE_GUID(CLSID, CLSID_DanLoads,0x54F57ECB,0x6111,0x4A9A,0xAF,0xA6,0xAB,0xC5,0xB3,0xC4,0xFF,0x59);


MIDL_DEFINE_GUID(CLSID, CLSID_Ports,0x265331A0,0x40D0,0x4DEC,0xB6,0x14,0x1A,0x21,0xCD,0xC5,0xCC,0x1F);


MIDL_DEFINE_GUID(CLSID, CLSID_OPCServer,0x9E033B12,0x2D79,0x41db,0x8A,0xC0,0xD8,0xEB,0xF4,0x5A,0x5B,0x6A);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



