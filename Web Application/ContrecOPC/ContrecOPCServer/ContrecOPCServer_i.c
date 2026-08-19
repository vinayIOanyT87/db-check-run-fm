

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for ContrecOPCServer.idl:
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

MIDL_DEFINE_GUID(IID, IID_IDataAccess,0xA3B7EB56,0x6763,0x426A,0x8B,0xC5,0x9E,0x7C,0xA2,0x7F,0x77,0xCB);


MIDL_DEFINE_GUID(IID, IID_IContrecs,0xC412CB25,0x436D,0x4536,0x8E,0x3E,0x4B,0xDD,0x0F,0xBD,0xE8,0x69);


MIDL_DEFINE_GUID(IID, IID_IPorts,0xD7A227C0,0xD386,0x408A,0x8B,0x2D,0xE0,0x0E,0x2F,0x27,0x0D,0x32);


MIDL_DEFINE_GUID(IID, LIBID_ContrecOPCServerLib,0x827959BA,0x0A20,0x493C,0x9E,0xE1,0x8D,0x00,0x7B,0xF8,0xC7,0x3B);


MIDL_DEFINE_GUID(CLSID, CLSID_Contrecs,0x59DB8E98,0xD175,0x49A8,0x99,0x7B,0x8D,0x34,0x21,0x54,0xB9,0xD7);


MIDL_DEFINE_GUID(CLSID, CLSID_DataAccess,0x2089945A,0x98ED,0x4FFB,0xB4,0x75,0x6C,0x53,0x58,0xBE,0x74,0x66);


MIDL_DEFINE_GUID(CLSID, CLSID_Ports,0x2B2CCFD9,0x9EF7,0x48BB,0xBE,0xF4,0xC5,0x8C,0x0C,0x43,0x40,0x9D);


MIDL_DEFINE_GUID(CLSID, CLSID_OPCServer,0xAF670D94,0x703A,0x4993,0xB1,0xDF,0x1A,0xBE,0xA1,0x95,0xA2,0x94);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



