

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for OptomuxOPCServer.idl:
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

MIDL_DEFINE_GUID(IID, IID_IDataAccess,0x4ECBB0F4,0xFFAF,0x4B3A,0x85,0x66,0x43,0x9D,0x4C,0x23,0x6C,0xF0);


MIDL_DEFINE_GUID(IID, IID_IOptomuxControllers,0xBFB066AF,0x2312,0x4C35,0x92,0x51,0x58,0x0B,0x45,0xAA,0xCB,0x95);


MIDL_DEFINE_GUID(IID, IID_IPorts,0xF0FDD2C4,0xA437,0x41F2,0xB1,0x13,0xE4,0xB2,0xEF,0xF9,0xB5,0xE8);


MIDL_DEFINE_GUID(IID, LIBID_OptomuxOPCServerLib,0x1EBA63BD,0xE098,0x42BF,0xB1,0x66,0xF9,0x8D,0xA4,0xBC,0x99,0xE6);


MIDL_DEFINE_GUID(CLSID, CLSID_DataAccess,0x6CAE3271,0xEFA1,0x4CB6,0xAF,0xE9,0x24,0x17,0x14,0x4D,0x2E,0x8B);


MIDL_DEFINE_GUID(CLSID, CLSID_OptomuxControllers,0xDD940B4F,0xC212,0x4361,0x8F,0xDE,0xD4,0x06,0x15,0x84,0xE4,0xD0);


MIDL_DEFINE_GUID(CLSID, CLSID_OPCServer,0xDC07462E,0xEDCD,0x4186,0x9F,0xD0,0xB8,0xB7,0xF9,0x90,0xB4,0x90);


MIDL_DEFINE_GUID(CLSID, CLSID_Ports,0xD1CAA238,0x8AB9,0x4E70,0xA6,0x28,0x49,0xAB,0x61,0xEC,0x5B,0xD1);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



