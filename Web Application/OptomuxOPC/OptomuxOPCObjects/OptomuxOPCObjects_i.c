

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for OptomuxOPCObjects.idl:
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

MIDL_DEFINE_GUID(IID, IID_IOptomuxController,0xB76EC008,0x879B,0x40C8,0x80,0x19,0x71,0x21,0x32,0x0E,0x11,0x67);


MIDL_DEFINE_GUID(IID, IID_IOptomuxControllerCollection,0x4CD8FFE8,0x912A,0x4434,0xBA,0x90,0xBC,0xF1,0x16,0x10,0x55,0x6E);


MIDL_DEFINE_GUID(IID, IID_IPort,0xD7B410E5,0xCA3D,0x4EA1,0x9E,0x4A,0xAE,0x42,0xF0,0x84,0x84,0x0F);


MIDL_DEFINE_GUID(IID, IID_IPortCollection,0xC6A5E076,0xAFE9,0x4995,0xB0,0xC2,0x3B,0x19,0x6E,0x0C,0x13,0xA7);


MIDL_DEFINE_GUID(IID, LIBID_OptomuxOPCObjectsLib,0xF6F29101,0x4C9B,0x4480,0xBF,0xF1,0x68,0x33,0x97,0xE8,0xAD,0xBA);


MIDL_DEFINE_GUID(CLSID, CLSID_OptomuxController,0xE7E70A48,0x7A25,0x4E15,0xA8,0xB9,0xBD,0xCD,0x55,0x25,0x4A,0x92);


MIDL_DEFINE_GUID(CLSID, CLSID_OptomuxControllerCollection,0x60E40DAA,0x9AB0,0x47D9,0x97,0x3A,0x09,0x55,0x42,0xC2,0x15,0xA1);


MIDL_DEFINE_GUID(CLSID, CLSID_Port,0x78ECB7AE,0x2D26,0x4635,0xA8,0x0E,0x5F,0x71,0x84,0x47,0x5C,0xF9);


MIDL_DEFINE_GUID(CLSID, CLSID_PortCollection,0x579CD2E7,0x95F6,0x48F0,0x93,0xEF,0xAA,0x61,0x2F,0x00,0xB3,0x10);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



