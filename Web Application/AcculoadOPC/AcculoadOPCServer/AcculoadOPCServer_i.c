

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for AcculoadOPCServer.idl:
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

MIDL_DEFINE_GUID(IID, IID_IDataAccess,0x2415FC12,0xF24B,0x48C6,0x9B,0x36,0x13,0xBD,0x03,0x03,0x18,0x9C);


MIDL_DEFINE_GUID(IID, IID_IAcculoads,0xD1EBB062,0x7933,0x45BE,0x8F,0x3E,0x04,0x40,0x52,0x15,0x45,0x58);


MIDL_DEFINE_GUID(IID, IID_IArms,0xBD2E1B2F,0x73DF,0x41E7,0x82,0x6E,0xFC,0x7B,0x2E,0x28,0xF8,0x47);


MIDL_DEFINE_GUID(IID, IID_ICardReaders,0x0345F55F,0x7273,0x4178,0xBD,0xD8,0x9C,0x80,0x7D,0xA9,0xF1,0x04);


MIDL_DEFINE_GUID(IID, IID_IPorts,0x993CDAB0,0xB262,0x426D,0x85,0x5B,0x84,0x40,0xB6,0xCB,0xFF,0x2C);


MIDL_DEFINE_GUID(IID, LIBID_AcculoadOPCServerLib,0x4BDC9779,0x8FEB,0x4C9A,0xB2,0x7F,0x9B,0xF8,0x23,0x49,0x74,0x10);


MIDL_DEFINE_GUID(CLSID, CLSID_DataAccess,0xEAA93515,0xF290,0x45DC,0x92,0x8E,0xCC,0xBD,0xE6,0x74,0xA2,0x88);


MIDL_DEFINE_GUID(CLSID, CLSID_Acculoads,0x41D54854,0x8705,0x400A,0x9B,0x22,0xF5,0x8B,0x58,0x08,0x8B,0xE7);


MIDL_DEFINE_GUID(CLSID, CLSID_Arms,0x91AA0986,0xE49B,0x4683,0x99,0xF9,0x41,0x2B,0xCA,0xBC,0x37,0x66);


MIDL_DEFINE_GUID(CLSID, CLSID_OPCServer,0xE70484C1,0x08F4,0x4E58,0x99,0xF8,0xFD,0x68,0xD1,0xF5,0x00,0xCE);


MIDL_DEFINE_GUID(CLSID, CLSID_CardReaders,0x0AB8E5B2,0x986C,0x4B03,0xA0,0xC7,0x24,0x3F,0xC6,0x96,0x33,0x28);


MIDL_DEFINE_GUID(CLSID, CLSID_Ports,0x2070F4BA,0x651D,0x4268,0x9F,0x5A,0x1E,0xBE,0x0A,0x13,0x71,0x41);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



