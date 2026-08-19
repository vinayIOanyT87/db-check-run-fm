

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for WeightScaleOPCServer.idl:
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

MIDL_DEFINE_GUID(IID, IID_IWeightScales,0x1A13A3B1,0x18B4,0x45AC,0x8B,0x3A,0x98,0x6A,0x3D,0x55,0x0E,0x2F);


MIDL_DEFINE_GUID(IID, IID_IDataAccess,0x0245CB0C,0xA46E,0x423E,0x8D,0x5F,0x9B,0x94,0x45,0x7F,0x14,0x84);


MIDL_DEFINE_GUID(IID, IID_IPorts,0x6F92560C,0x9F70,0x4176,0x93,0x11,0x7A,0x0C,0x33,0x87,0xFE,0xB8);


MIDL_DEFINE_GUID(IID, LIBID_WeightScaleOPCServerLib,0x85CD7BF4,0xE995,0x4E4A,0xAB,0xE2,0xD9,0xFD,0x90,0xCF,0x60,0x27);


MIDL_DEFINE_GUID(CLSID, CLSID_WeightScales,0xFB4C3029,0xD5C9,0x4BB8,0xAC,0x5A,0x19,0x14,0x85,0x8D,0x79,0xD5);


MIDL_DEFINE_GUID(CLSID, CLSID_DataAccess,0x5485F6AE,0xF03B,0x4FEB,0xA5,0xE8,0x9B,0x83,0xD9,0xE2,0x65,0x16);


MIDL_DEFINE_GUID(CLSID, CLSID_Ports,0x265331A0,0x40D0,0x4DEC,0xB6,0x14,0x2A,0x21,0xCD,0xC5,0xCC,0x1F);


MIDL_DEFINE_GUID(CLSID, CLSID_OPCServer,0xAF5C3633,0x019F,0x4446,0x9E,0x11,0xB0,0x35,0x6D,0x0E,0x11,0xA8);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



