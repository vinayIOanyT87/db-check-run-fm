

/* this ALWAYS GENERATED file contains the proxy stub code */


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

#if !defined(_M_IA64) && !defined(_M_AMD64) && !defined(_ARM_)


#if _MSC_VER >= 1200
#pragma warning(push)
#endif

#pragma warning( disable: 4211 )  /* redefine extern to static */
#pragma warning( disable: 4232 )  /* dllimport identity*/
#pragma warning( disable: 4024 )  /* array to pointer mapping*/
#pragma warning( disable: 4152 )  /* function/data pointer conversion in expression */
#pragma warning( disable: 4100 ) /* unreferenced arguments in x86 call */

#pragma optimize("", off ) 

#define USE_STUBLESS_PROXY


/* verify that the <rpcproxy.h> version is high enough to compile this file*/
#ifndef __REDQ_RPCPROXY_H_VERSION__
#define __REQUIRED_RPCPROXY_H_VERSION__ 475
#endif


#include "rpcproxy.h"
#ifndef __RPCPROXY_H_VERSION__
#error this stub requires an updated version of <rpcproxy.h>
#endif /* __RPCPROXY_H_VERSION__ */


#include "ContrecOPCServer.h"

#define TYPE_FORMAT_STRING_SIZE   1211                              
#define PROC_FORMAT_STRING_SIZE   349                               
#define EXPR_FORMAT_STRING_SIZE   1                                 
#define TRANSMIT_AS_TABLE_SIZE    0            
#define WIRE_MARSHAL_TABLE_SIZE   2            

typedef struct _ContrecOPCServer_MIDL_TYPE_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ TYPE_FORMAT_STRING_SIZE ];
    } ContrecOPCServer_MIDL_TYPE_FORMAT_STRING;

typedef struct _ContrecOPCServer_MIDL_PROC_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ PROC_FORMAT_STRING_SIZE ];
    } ContrecOPCServer_MIDL_PROC_FORMAT_STRING;

typedef struct _ContrecOPCServer_MIDL_EXPR_FORMAT_STRING
    {
    long          Pad;
    unsigned char  Format[ EXPR_FORMAT_STRING_SIZE ];
    } ContrecOPCServer_MIDL_EXPR_FORMAT_STRING;


static const RPC_SYNTAX_IDENTIFIER  _RpcTransferSyntax = 
{{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}};


extern const ContrecOPCServer_MIDL_TYPE_FORMAT_STRING ContrecOPCServer__MIDL_TypeFormatString;
extern const ContrecOPCServer_MIDL_PROC_FORMAT_STRING ContrecOPCServer__MIDL_ProcFormatString;
extern const ContrecOPCServer_MIDL_EXPR_FORMAT_STRING ContrecOPCServer__MIDL_ExprFormatString;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO IDataAccess_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO IDataAccess_ProxyInfo;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO IContrecs_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO IContrecs_ProxyInfo;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO IPorts_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO IPorts_ProxyInfo;


extern const USER_MARSHAL_ROUTINE_QUADRUPLE UserMarshalRoutines[ WIRE_MARSHAL_TABLE_SIZE ];

#if !defined(__RPC_WIN32__)
#error  Invalid build platform for this stub.
#endif
#if !(TARGET_IS_NT60_OR_LATER)
#error You need Windows Vista or later to run this stub because it uses these features:
#error   forced complex structure or array, new range semantics, compiled for Windows Vista.
#error However, your C/C++ compilation flags indicate you intend to run this app on earlier systems.
#error This app will fail with the RPC_X_WRONG_STUB_VERSION error.
#endif


static const ContrecOPCServer_MIDL_PROC_FORMAT_STRING ContrecOPCServer__MIDL_ProcFormatString =
    {
        0,
        {

	/* Procedure GetRecordSet */

			0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/*  2 */	NdrFcLong( 0x0 ),	/* 0 */
/*  6 */	NdrFcShort( 0x7 ),	/* 7 */
/*  8 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 10 */	NdrFcShort( 0x0 ),	/* 0 */
/* 12 */	NdrFcShort( 0x8 ),	/* 8 */
/* 14 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 16 */	0x8,		/* 8 */
			0x45,		/* Ext Flags:  new corr desc, srv corr check, has range on conformance */
/* 18 */	NdrFcShort( 0x0 ),	/* 0 */
/* 20 */	NdrFcShort( 0x1 ),	/* 1 */
/* 22 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter bstrSQL */

/* 24 */	NdrFcShort( 0x8b ),	/* Flags:  must size, must free, in, by val, */
/* 26 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 28 */	NdrFcShort( 0x26 ),	/* Type Offset=38 */

	/* Parameter ppRecordSet */

/* 30 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 32 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 34 */	NdrFcShort( 0x30 ),	/* Type Offset=48 */

	/* Return value */

/* 36 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 38 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 40 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure ExecuteQuery */

/* 42 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 44 */	NdrFcLong( 0x0 ),	/* 0 */
/* 48 */	NdrFcShort( 0x8 ),	/* 8 */
/* 50 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 52 */	NdrFcShort( 0x0 ),	/* 0 */
/* 54 */	NdrFcShort( 0x8 ),	/* 8 */
/* 56 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 58 */	0x8,		/* 8 */
			0x45,		/* Ext Flags:  new corr desc, srv corr check, has range on conformance */
/* 60 */	NdrFcShort( 0x0 ),	/* 0 */
/* 62 */	NdrFcShort( 0x1 ),	/* 1 */
/* 64 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter bstrSQL */

/* 66 */	NdrFcShort( 0x8b ),	/* Flags:  must size, must free, in, by val, */
/* 68 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 70 */	NdrFcShort( 0x26 ),	/* Type Offset=38 */

	/* Return value */

/* 72 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 74 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 76 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Enumerate */


	/* Procedure Enumerate */

/* 78 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 80 */	NdrFcLong( 0x0 ),	/* 0 */
/* 84 */	NdrFcShort( 0x7 ),	/* 7 */
/* 86 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 88 */	NdrFcShort( 0x0 ),	/* 0 */
/* 90 */	NdrFcShort( 0x8 ),	/* 8 */
/* 92 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 94 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 96 */	NdrFcShort( 0x0 ),	/* 0 */
/* 98 */	NdrFcShort( 0x0 ),	/* 0 */
/* 100 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter ppCardReaderCollection */


	/* Parameter ppContrecCollection */

/* 102 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 104 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 106 */	NdrFcShort( 0x30 ),	/* Type Offset=48 */

	/* Return value */


	/* Return value */

/* 108 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 110 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 112 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Add */


	/* Procedure Add */

/* 114 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 116 */	NdrFcLong( 0x0 ),	/* 0 */
/* 120 */	NdrFcShort( 0x8 ),	/* 8 */
/* 122 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 124 */	NdrFcShort( 0x0 ),	/* 0 */
/* 126 */	NdrFcShort( 0x24 ),	/* 36 */
/* 128 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 130 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 132 */	NdrFcShort( 0x0 ),	/* 0 */
/* 134 */	NdrFcShort( 0x0 ),	/* 0 */
/* 136 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pCardReader */


	/* Parameter pContrec */

/* 138 */	NdrFcShort( 0xb ),	/* Flags:  must size, must free, in, */
/* 140 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 142 */	NdrFcShort( 0x34 ),	/* Type Offset=52 */

	/* Parameter pIndex */


	/* Parameter pIndex */

/* 144 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 146 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 148 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */


	/* Return value */

/* 150 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 152 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 154 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Modify */


	/* Procedure Modify */

/* 156 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 158 */	NdrFcLong( 0x0 ),	/* 0 */
/* 162 */	NdrFcShort( 0x9 ),	/* 9 */
/* 164 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 166 */	NdrFcShort( 0x0 ),	/* 0 */
/* 168 */	NdrFcShort( 0x8 ),	/* 8 */
/* 170 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 172 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 174 */	NdrFcShort( 0x0 ),	/* 0 */
/* 176 */	NdrFcShort( 0x0 ),	/* 0 */
/* 178 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pCardReader */


	/* Parameter pContrec */

/* 180 */	NdrFcShort( 0xb ),	/* Flags:  must size, must free, in, */
/* 182 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 184 */	NdrFcShort( 0x34 ),	/* Type Offset=52 */

	/* Return value */


	/* Return value */

/* 186 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 188 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 190 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Purge */


	/* Procedure Purge */

/* 192 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 194 */	NdrFcLong( 0x0 ),	/* 0 */
/* 198 */	NdrFcShort( 0xa ),	/* 10 */
/* 200 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 202 */	NdrFcShort( 0x8 ),	/* 8 */
/* 204 */	NdrFcShort( 0x8 ),	/* 8 */
/* 206 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 208 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 210 */	NdrFcShort( 0x0 ),	/* 0 */
/* 212 */	NdrFcShort( 0x0 ),	/* 0 */
/* 214 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter lIndex */


	/* Parameter lIndex */

/* 216 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 218 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 220 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */


	/* Return value */

/* 222 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 224 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 226 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure GetIndex */


	/* Procedure GetIndex */

/* 228 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 230 */	NdrFcLong( 0x0 ),	/* 0 */
/* 234 */	NdrFcShort( 0xb ),	/* 11 */
/* 236 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 238 */	NdrFcShort( 0x0 ),	/* 0 */
/* 240 */	NdrFcShort( 0x24 ),	/* 36 */
/* 242 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 244 */	0x8,		/* 8 */
			0x45,		/* Ext Flags:  new corr desc, srv corr check, has range on conformance */
/* 246 */	NdrFcShort( 0x0 ),	/* 0 */
/* 248 */	NdrFcShort( 0x1 ),	/* 1 */
/* 250 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter bstrID */


	/* Parameter bstrID */

/* 252 */	NdrFcShort( 0x8b ),	/* Flags:  must size, must free, in, by val, */
/* 254 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 256 */	NdrFcShort( 0x26 ),	/* Type Offset=38 */

	/* Parameter pIndex */


	/* Parameter pIndex */

/* 258 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 260 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 262 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */


	/* Return value */

/* 264 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 266 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 268 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Get */


	/* Procedure Get */

/* 270 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 272 */	NdrFcLong( 0x0 ),	/* 0 */
/* 276 */	NdrFcShort( 0xc ),	/* 12 */
/* 278 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 280 */	NdrFcShort( 0x8 ),	/* 8 */
/* 282 */	NdrFcShort( 0x8 ),	/* 8 */
/* 284 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 286 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 288 */	NdrFcShort( 0x0 ),	/* 0 */
/* 290 */	NdrFcShort( 0x0 ),	/* 0 */
/* 292 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter lIndex */


	/* Parameter lIndex */

/* 294 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 296 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 298 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter ppCardReader */


	/* Parameter ppContrec */

/* 300 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 302 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 304 */	NdrFcShort( 0x30 ),	/* Type Offset=48 */

	/* Return value */


	/* Return value */

/* 306 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 308 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 310 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure EnumeratePortIDs */

/* 312 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 314 */	NdrFcLong( 0x0 ),	/* 0 */
/* 318 */	NdrFcShort( 0xd ),	/* 13 */
/* 320 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 322 */	NdrFcShort( 0x0 ),	/* 0 */
/* 324 */	NdrFcShort( 0x8 ),	/* 8 */
/* 326 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 328 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 330 */	NdrFcShort( 0x1 ),	/* 1 */
/* 332 */	NdrFcShort( 0x0 ),	/* 0 */
/* 334 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pIDs */

/* 336 */	NdrFcShort( 0x4113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=16 */
/* 338 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 340 */	NdrFcShort( 0x4b0 ),	/* Type Offset=1200 */

	/* Return value */

/* 342 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 344 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 346 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

			0x0
        }
    };

static const ContrecOPCServer_MIDL_TYPE_FORMAT_STRING ContrecOPCServer__MIDL_TypeFormatString =
    {
        0,
        {
			NdrFcShort( 0x0 ),	/* 0 */
/*  2 */	
			0x12, 0x0,	/* FC_UP */
/*  4 */	NdrFcShort( 0x18 ),	/* Offset= 24 (28) */
/*  6 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/*  8 */	NdrFcShort( 0x2 ),	/* 2 */
/* 10 */	0x9,		/* Corr desc: FC_ULONG */
			0x0,		/*  */
/* 12 */	NdrFcShort( 0xfffc ),	/* -4 */
/* 14 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 16 */	0x0 , 
			0x0,		/* 0 */
/* 18 */	NdrFcLong( 0x0 ),	/* 0 */
/* 22 */	NdrFcLong( 0x0 ),	/* 0 */
/* 26 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 28 */	
			0x17,		/* FC_CSTRUCT */
			0x3,		/* 3 */
/* 30 */	NdrFcShort( 0x8 ),	/* 8 */
/* 32 */	NdrFcShort( 0xffe6 ),	/* Offset= -26 (6) */
/* 34 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 36 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 38 */	0xb4,		/* FC_USER_MARSHAL */
			0x83,		/* 131 */
/* 40 */	NdrFcShort( 0x0 ),	/* 0 */
/* 42 */	NdrFcShort( 0x4 ),	/* 4 */
/* 44 */	NdrFcShort( 0x0 ),	/* 0 */
/* 46 */	NdrFcShort( 0xffd4 ),	/* Offset= -44 (2) */
/* 48 */	
			0x11, 0x10,	/* FC_RP [pointer_deref] */
/* 50 */	NdrFcShort( 0x2 ),	/* Offset= 2 (52) */
/* 52 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 54 */	NdrFcLong( 0x20400 ),	/* 132096 */
/* 58 */	NdrFcShort( 0x0 ),	/* 0 */
/* 60 */	NdrFcShort( 0x0 ),	/* 0 */
/* 62 */	0xc0,		/* 192 */
			0x0,		/* 0 */
/* 64 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 66 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 68 */	0x0,		/* 0 */
			0x46,		/* 70 */
/* 70 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/* 72 */	0x8,		/* FC_LONG */
			0x5c,		/* FC_PAD */
/* 74 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/* 76 */	NdrFcShort( 0x464 ),	/* Offset= 1124 (1200) */
/* 78 */	
			0x13, 0x0,	/* FC_OP */
/* 80 */	NdrFcShort( 0x44c ),	/* Offset= 1100 (1180) */
/* 82 */	
			0x2b,		/* FC_NON_ENCAPSULATED_UNION */
			0x9,		/* FC_ULONG */
/* 84 */	0x7,		/* Corr desc: FC_USHORT */
			0x0,		/*  */
/* 86 */	NdrFcShort( 0xfff8 ),	/* -8 */
/* 88 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 90 */	0x0 , 
			0x0,		/* 0 */
/* 92 */	NdrFcLong( 0x0 ),	/* 0 */
/* 96 */	NdrFcLong( 0x0 ),	/* 0 */
/* 100 */	NdrFcShort( 0x2 ),	/* Offset= 2 (102) */
/* 102 */	NdrFcShort( 0x10 ),	/* 16 */
/* 104 */	NdrFcShort( 0x2f ),	/* 47 */
/* 106 */	NdrFcLong( 0x14 ),	/* 20 */
/* 110 */	NdrFcShort( 0x800b ),	/* Simple arm type: FC_HYPER */
/* 112 */	NdrFcLong( 0x3 ),	/* 3 */
/* 116 */	NdrFcShort( 0x8008 ),	/* Simple arm type: FC_LONG */
/* 118 */	NdrFcLong( 0x11 ),	/* 17 */
/* 122 */	NdrFcShort( 0x8001 ),	/* Simple arm type: FC_BYTE */
/* 124 */	NdrFcLong( 0x2 ),	/* 2 */
/* 128 */	NdrFcShort( 0x8006 ),	/* Simple arm type: FC_SHORT */
/* 130 */	NdrFcLong( 0x4 ),	/* 4 */
/* 134 */	NdrFcShort( 0x800a ),	/* Simple arm type: FC_FLOAT */
/* 136 */	NdrFcLong( 0x5 ),	/* 5 */
/* 140 */	NdrFcShort( 0x800c ),	/* Simple arm type: FC_DOUBLE */
/* 142 */	NdrFcLong( 0xb ),	/* 11 */
/* 146 */	NdrFcShort( 0x8006 ),	/* Simple arm type: FC_SHORT */
/* 148 */	NdrFcLong( 0xa ),	/* 10 */
/* 152 */	NdrFcShort( 0x8008 ),	/* Simple arm type: FC_LONG */
/* 154 */	NdrFcLong( 0x6 ),	/* 6 */
/* 158 */	NdrFcShort( 0xe8 ),	/* Offset= 232 (390) */
/* 160 */	NdrFcLong( 0x7 ),	/* 7 */
/* 164 */	NdrFcShort( 0x800c ),	/* Simple arm type: FC_DOUBLE */
/* 166 */	NdrFcLong( 0x8 ),	/* 8 */
/* 170 */	NdrFcShort( 0xe2 ),	/* Offset= 226 (396) */
/* 172 */	NdrFcLong( 0xd ),	/* 13 */
/* 176 */	NdrFcShort( 0xe0 ),	/* Offset= 224 (400) */
/* 178 */	NdrFcLong( 0x9 ),	/* 9 */
/* 182 */	NdrFcShort( 0xff7e ),	/* Offset= -130 (52) */
/* 184 */	NdrFcLong( 0x2000 ),	/* 8192 */
/* 188 */	NdrFcShort( 0xe6 ),	/* Offset= 230 (418) */
/* 190 */	NdrFcLong( 0x24 ),	/* 36 */
/* 194 */	NdrFcShort( 0x390 ),	/* Offset= 912 (1106) */
/* 196 */	NdrFcLong( 0x4024 ),	/* 16420 */
/* 200 */	NdrFcShort( 0x38a ),	/* Offset= 906 (1106) */
/* 202 */	NdrFcLong( 0x4011 ),	/* 16401 */
/* 206 */	NdrFcShort( 0x388 ),	/* Offset= 904 (1110) */
/* 208 */	NdrFcLong( 0x4002 ),	/* 16386 */
/* 212 */	NdrFcShort( 0x386 ),	/* Offset= 902 (1114) */
/* 214 */	NdrFcLong( 0x4003 ),	/* 16387 */
/* 218 */	NdrFcShort( 0x384 ),	/* Offset= 900 (1118) */
/* 220 */	NdrFcLong( 0x4014 ),	/* 16404 */
/* 224 */	NdrFcShort( 0x382 ),	/* Offset= 898 (1122) */
/* 226 */	NdrFcLong( 0x4004 ),	/* 16388 */
/* 230 */	NdrFcShort( 0x380 ),	/* Offset= 896 (1126) */
/* 232 */	NdrFcLong( 0x4005 ),	/* 16389 */
/* 236 */	NdrFcShort( 0x37e ),	/* Offset= 894 (1130) */
/* 238 */	NdrFcLong( 0x400b ),	/* 16395 */
/* 242 */	NdrFcShort( 0x368 ),	/* Offset= 872 (1114) */
/* 244 */	NdrFcLong( 0x400a ),	/* 16394 */
/* 248 */	NdrFcShort( 0x366 ),	/* Offset= 870 (1118) */
/* 250 */	NdrFcLong( 0x4006 ),	/* 16390 */
/* 254 */	NdrFcShort( 0x370 ),	/* Offset= 880 (1134) */
/* 256 */	NdrFcLong( 0x4007 ),	/* 16391 */
/* 260 */	NdrFcShort( 0x366 ),	/* Offset= 870 (1130) */
/* 262 */	NdrFcLong( 0x4008 ),	/* 16392 */
/* 266 */	NdrFcShort( 0x368 ),	/* Offset= 872 (1138) */
/* 268 */	NdrFcLong( 0x400d ),	/* 16397 */
/* 272 */	NdrFcShort( 0x366 ),	/* Offset= 870 (1142) */
/* 274 */	NdrFcLong( 0x4009 ),	/* 16393 */
/* 278 */	NdrFcShort( 0x364 ),	/* Offset= 868 (1146) */
/* 280 */	NdrFcLong( 0x6000 ),	/* 24576 */
/* 284 */	NdrFcShort( 0x362 ),	/* Offset= 866 (1150) */
/* 286 */	NdrFcLong( 0x400c ),	/* 16396 */
/* 290 */	NdrFcShort( 0x360 ),	/* Offset= 864 (1154) */
/* 292 */	NdrFcLong( 0x10 ),	/* 16 */
/* 296 */	NdrFcShort( 0x8002 ),	/* Simple arm type: FC_CHAR */
/* 298 */	NdrFcLong( 0x12 ),	/* 18 */
/* 302 */	NdrFcShort( 0x8006 ),	/* Simple arm type: FC_SHORT */
/* 304 */	NdrFcLong( 0x13 ),	/* 19 */
/* 308 */	NdrFcShort( 0x8008 ),	/* Simple arm type: FC_LONG */
/* 310 */	NdrFcLong( 0x15 ),	/* 21 */
/* 314 */	NdrFcShort( 0x800b ),	/* Simple arm type: FC_HYPER */
/* 316 */	NdrFcLong( 0x16 ),	/* 22 */
/* 320 */	NdrFcShort( 0x8008 ),	/* Simple arm type: FC_LONG */
/* 322 */	NdrFcLong( 0x17 ),	/* 23 */
/* 326 */	NdrFcShort( 0x8008 ),	/* Simple arm type: FC_LONG */
/* 328 */	NdrFcLong( 0xe ),	/* 14 */
/* 332 */	NdrFcShort( 0x33e ),	/* Offset= 830 (1162) */
/* 334 */	NdrFcLong( 0x400e ),	/* 16398 */
/* 338 */	NdrFcShort( 0x342 ),	/* Offset= 834 (1172) */
/* 340 */	NdrFcLong( 0x4010 ),	/* 16400 */
/* 344 */	NdrFcShort( 0x340 ),	/* Offset= 832 (1176) */
/* 346 */	NdrFcLong( 0x4012 ),	/* 16402 */
/* 350 */	NdrFcShort( 0x2fc ),	/* Offset= 764 (1114) */
/* 352 */	NdrFcLong( 0x4013 ),	/* 16403 */
/* 356 */	NdrFcShort( 0x2fa ),	/* Offset= 762 (1118) */
/* 358 */	NdrFcLong( 0x4015 ),	/* 16405 */
/* 362 */	NdrFcShort( 0x2f8 ),	/* Offset= 760 (1122) */
/* 364 */	NdrFcLong( 0x4016 ),	/* 16406 */
/* 368 */	NdrFcShort( 0x2ee ),	/* Offset= 750 (1118) */
/* 370 */	NdrFcLong( 0x4017 ),	/* 16407 */
/* 374 */	NdrFcShort( 0x2e8 ),	/* Offset= 744 (1118) */
/* 376 */	NdrFcLong( 0x0 ),	/* 0 */
/* 380 */	NdrFcShort( 0x0 ),	/* Offset= 0 (380) */
/* 382 */	NdrFcLong( 0x1 ),	/* 1 */
/* 386 */	NdrFcShort( 0x0 ),	/* Offset= 0 (386) */
/* 388 */	NdrFcShort( 0xffff ),	/* Offset= -1 (387) */
/* 390 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 392 */	NdrFcShort( 0x8 ),	/* 8 */
/* 394 */	0xb,		/* FC_HYPER */
			0x5b,		/* FC_END */
/* 396 */	
			0x13, 0x0,	/* FC_OP */
/* 398 */	NdrFcShort( 0xfe8e ),	/* Offset= -370 (28) */
/* 400 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 402 */	NdrFcLong( 0x0 ),	/* 0 */
/* 406 */	NdrFcShort( 0x0 ),	/* 0 */
/* 408 */	NdrFcShort( 0x0 ),	/* 0 */
/* 410 */	0xc0,		/* 192 */
			0x0,		/* 0 */
/* 412 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 414 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 416 */	0x0,		/* 0 */
			0x46,		/* 70 */
/* 418 */	
			0x13, 0x10,	/* FC_OP [pointer_deref] */
/* 420 */	NdrFcShort( 0x2 ),	/* Offset= 2 (422) */
/* 422 */	
			0x13, 0x0,	/* FC_OP */
/* 424 */	NdrFcShort( 0x298 ),	/* Offset= 664 (1088) */
/* 426 */	
			0x2a,		/* FC_ENCAPSULATED_UNION */
			0x49,		/* 73 */
/* 428 */	NdrFcShort( 0x18 ),	/* 24 */
/* 430 */	NdrFcShort( 0xa ),	/* 10 */
/* 432 */	NdrFcLong( 0x8 ),	/* 8 */
/* 436 */	NdrFcShort( 0x64 ),	/* Offset= 100 (536) */
/* 438 */	NdrFcLong( 0xd ),	/* 13 */
/* 442 */	NdrFcShort( 0x9c ),	/* Offset= 156 (598) */
/* 444 */	NdrFcLong( 0x9 ),	/* 9 */
/* 448 */	NdrFcShort( 0xd0 ),	/* Offset= 208 (656) */
/* 450 */	NdrFcLong( 0xc ),	/* 12 */
/* 454 */	NdrFcShort( 0x104 ),	/* Offset= 260 (714) */
/* 456 */	NdrFcLong( 0x24 ),	/* 36 */
/* 460 */	NdrFcShort( 0x174 ),	/* Offset= 372 (832) */
/* 462 */	NdrFcLong( 0x800d ),	/* 32781 */
/* 466 */	NdrFcShort( 0x190 ),	/* Offset= 400 (866) */
/* 468 */	NdrFcLong( 0x10 ),	/* 16 */
/* 472 */	NdrFcShort( 0x1b4 ),	/* Offset= 436 (908) */
/* 474 */	NdrFcLong( 0x2 ),	/* 2 */
/* 478 */	NdrFcShort( 0x1d8 ),	/* Offset= 472 (950) */
/* 480 */	NdrFcLong( 0x3 ),	/* 3 */
/* 484 */	NdrFcShort( 0x1fc ),	/* Offset= 508 (992) */
/* 486 */	NdrFcLong( 0x14 ),	/* 20 */
/* 490 */	NdrFcShort( 0x220 ),	/* Offset= 544 (1034) */
/* 492 */	NdrFcShort( 0xffff ),	/* Offset= -1 (491) */
/* 494 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 496 */	NdrFcShort( 0x4 ),	/* 4 */
/* 498 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 500 */	NdrFcShort( 0x0 ),	/* 0 */
/* 502 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 504 */	0x0 , 
			0x0,		/* 0 */
/* 506 */	NdrFcLong( 0x0 ),	/* 0 */
/* 510 */	NdrFcLong( 0x0 ),	/* 0 */
/* 514 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 516 */	
			0x48,		/* FC_VARIABLE_REPEAT */
			0x49,		/* FC_FIXED_OFFSET */
/* 518 */	NdrFcShort( 0x4 ),	/* 4 */
/* 520 */	NdrFcShort( 0x0 ),	/* 0 */
/* 522 */	NdrFcShort( 0x1 ),	/* 1 */
/* 524 */	NdrFcShort( 0x0 ),	/* 0 */
/* 526 */	NdrFcShort( 0x0 ),	/* 0 */
/* 528 */	0x13, 0x0,	/* FC_OP */
/* 530 */	NdrFcShort( 0xfe0a ),	/* Offset= -502 (28) */
/* 532 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 534 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 536 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 538 */	NdrFcShort( 0x8 ),	/* 8 */
/* 540 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 542 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 544 */	NdrFcShort( 0x4 ),	/* 4 */
/* 546 */	NdrFcShort( 0x4 ),	/* 4 */
/* 548 */	0x11, 0x0,	/* FC_RP */
/* 550 */	NdrFcShort( 0xffc8 ),	/* Offset= -56 (494) */
/* 552 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 554 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 556 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x3,		/* 3 */
/* 558 */	NdrFcShort( 0x0 ),	/* 0 */
/* 560 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 562 */	NdrFcShort( 0x0 ),	/* 0 */
/* 564 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 566 */	0x0 , 
			0x0,		/* 0 */
/* 568 */	NdrFcLong( 0x0 ),	/* 0 */
/* 572 */	NdrFcLong( 0x0 ),	/* 0 */
/* 576 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 580 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 582 */	0x0 , 
			0x0,		/* 0 */
/* 584 */	NdrFcLong( 0x0 ),	/* 0 */
/* 588 */	NdrFcLong( 0x0 ),	/* 0 */
/* 592 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 594 */	NdrFcShort( 0xff3e ),	/* Offset= -194 (400) */
/* 596 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 598 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 600 */	NdrFcShort( 0x8 ),	/* 8 */
/* 602 */	NdrFcShort( 0x0 ),	/* 0 */
/* 604 */	NdrFcShort( 0x6 ),	/* Offset= 6 (610) */
/* 606 */	0x8,		/* FC_LONG */
			0x36,		/* FC_POINTER */
/* 608 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 610 */	
			0x11, 0x0,	/* FC_RP */
/* 612 */	NdrFcShort( 0xffc8 ),	/* Offset= -56 (556) */
/* 614 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x3,		/* 3 */
/* 616 */	NdrFcShort( 0x0 ),	/* 0 */
/* 618 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 620 */	NdrFcShort( 0x0 ),	/* 0 */
/* 622 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 624 */	0x0 , 
			0x0,		/* 0 */
/* 626 */	NdrFcLong( 0x0 ),	/* 0 */
/* 630 */	NdrFcLong( 0x0 ),	/* 0 */
/* 634 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 638 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 640 */	0x0 , 
			0x0,		/* 0 */
/* 642 */	NdrFcLong( 0x0 ),	/* 0 */
/* 646 */	NdrFcLong( 0x0 ),	/* 0 */
/* 650 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 652 */	NdrFcShort( 0xfda8 ),	/* Offset= -600 (52) */
/* 654 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 656 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 658 */	NdrFcShort( 0x8 ),	/* 8 */
/* 660 */	NdrFcShort( 0x0 ),	/* 0 */
/* 662 */	NdrFcShort( 0x6 ),	/* Offset= 6 (668) */
/* 664 */	0x8,		/* FC_LONG */
			0x36,		/* FC_POINTER */
/* 666 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 668 */	
			0x11, 0x0,	/* FC_RP */
/* 670 */	NdrFcShort( 0xffc8 ),	/* Offset= -56 (614) */
/* 672 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 674 */	NdrFcShort( 0x4 ),	/* 4 */
/* 676 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 678 */	NdrFcShort( 0x0 ),	/* 0 */
/* 680 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 682 */	0x0 , 
			0x0,		/* 0 */
/* 684 */	NdrFcLong( 0x0 ),	/* 0 */
/* 688 */	NdrFcLong( 0x0 ),	/* 0 */
/* 692 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 694 */	
			0x48,		/* FC_VARIABLE_REPEAT */
			0x49,		/* FC_FIXED_OFFSET */
/* 696 */	NdrFcShort( 0x4 ),	/* 4 */
/* 698 */	NdrFcShort( 0x0 ),	/* 0 */
/* 700 */	NdrFcShort( 0x1 ),	/* 1 */
/* 702 */	NdrFcShort( 0x0 ),	/* 0 */
/* 704 */	NdrFcShort( 0x0 ),	/* 0 */
/* 706 */	0x13, 0x0,	/* FC_OP */
/* 708 */	NdrFcShort( 0x1d8 ),	/* Offset= 472 (1180) */
/* 710 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 712 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 714 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 716 */	NdrFcShort( 0x8 ),	/* 8 */
/* 718 */	NdrFcShort( 0x0 ),	/* 0 */
/* 720 */	NdrFcShort( 0x6 ),	/* Offset= 6 (726) */
/* 722 */	0x8,		/* FC_LONG */
			0x36,		/* FC_POINTER */
/* 724 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 726 */	
			0x11, 0x0,	/* FC_RP */
/* 728 */	NdrFcShort( 0xffc8 ),	/* Offset= -56 (672) */
/* 730 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 732 */	NdrFcLong( 0x2f ),	/* 47 */
/* 736 */	NdrFcShort( 0x0 ),	/* 0 */
/* 738 */	NdrFcShort( 0x0 ),	/* 0 */
/* 740 */	0xc0,		/* 192 */
			0x0,		/* 0 */
/* 742 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 744 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 746 */	0x0,		/* 0 */
			0x46,		/* 70 */
/* 748 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 750 */	NdrFcShort( 0x1 ),	/* 1 */
/* 752 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 754 */	NdrFcShort( 0x4 ),	/* 4 */
/* 756 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 758 */	0x0 , 
			0x0,		/* 0 */
/* 760 */	NdrFcLong( 0x0 ),	/* 0 */
/* 764 */	NdrFcLong( 0x0 ),	/* 0 */
/* 768 */	0x1,		/* FC_BYTE */
			0x5b,		/* FC_END */
/* 770 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 772 */	NdrFcShort( 0x10 ),	/* 16 */
/* 774 */	NdrFcShort( 0x0 ),	/* 0 */
/* 776 */	NdrFcShort( 0xa ),	/* Offset= 10 (786) */
/* 778 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 780 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 782 */	NdrFcShort( 0xffcc ),	/* Offset= -52 (730) */
/* 784 */	0x36,		/* FC_POINTER */
			0x5b,		/* FC_END */
/* 786 */	
			0x13, 0x20,	/* FC_OP [maybenull_sizeis] */
/* 788 */	NdrFcShort( 0xffd8 ),	/* Offset= -40 (748) */
/* 790 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 792 */	NdrFcShort( 0x4 ),	/* 4 */
/* 794 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 796 */	NdrFcShort( 0x0 ),	/* 0 */
/* 798 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 800 */	0x0 , 
			0x0,		/* 0 */
/* 802 */	NdrFcLong( 0x0 ),	/* 0 */
/* 806 */	NdrFcLong( 0x0 ),	/* 0 */
/* 810 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 812 */	
			0x48,		/* FC_VARIABLE_REPEAT */
			0x49,		/* FC_FIXED_OFFSET */
/* 814 */	NdrFcShort( 0x4 ),	/* 4 */
/* 816 */	NdrFcShort( 0x0 ),	/* 0 */
/* 818 */	NdrFcShort( 0x1 ),	/* 1 */
/* 820 */	NdrFcShort( 0x0 ),	/* 0 */
/* 822 */	NdrFcShort( 0x0 ),	/* 0 */
/* 824 */	0x13, 0x0,	/* FC_OP */
/* 826 */	NdrFcShort( 0xffc8 ),	/* Offset= -56 (770) */
/* 828 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 830 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 832 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 834 */	NdrFcShort( 0x8 ),	/* 8 */
/* 836 */	NdrFcShort( 0x0 ),	/* 0 */
/* 838 */	NdrFcShort( 0x6 ),	/* Offset= 6 (844) */
/* 840 */	0x8,		/* FC_LONG */
			0x36,		/* FC_POINTER */
/* 842 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 844 */	
			0x11, 0x0,	/* FC_RP */
/* 846 */	NdrFcShort( 0xffc8 ),	/* Offset= -56 (790) */
/* 848 */	
			0x1d,		/* FC_SMFARRAY */
			0x0,		/* 0 */
/* 850 */	NdrFcShort( 0x8 ),	/* 8 */
/* 852 */	0x1,		/* FC_BYTE */
			0x5b,		/* FC_END */
/* 854 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 856 */	NdrFcShort( 0x10 ),	/* 16 */
/* 858 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 860 */	0x6,		/* FC_SHORT */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 862 */	0x0,		/* 0 */
			NdrFcShort( 0xfff1 ),	/* Offset= -15 (848) */
			0x5b,		/* FC_END */
/* 866 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 868 */	NdrFcShort( 0x18 ),	/* 24 */
/* 870 */	NdrFcShort( 0x0 ),	/* 0 */
/* 872 */	NdrFcShort( 0xa ),	/* Offset= 10 (882) */
/* 874 */	0x8,		/* FC_LONG */
			0x36,		/* FC_POINTER */
/* 876 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 878 */	NdrFcShort( 0xffe8 ),	/* Offset= -24 (854) */
/* 880 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 882 */	
			0x11, 0x0,	/* FC_RP */
/* 884 */	NdrFcShort( 0xfeb8 ),	/* Offset= -328 (556) */
/* 886 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 888 */	NdrFcShort( 0x1 ),	/* 1 */
/* 890 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 892 */	NdrFcShort( 0x0 ),	/* 0 */
/* 894 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 896 */	0x0 , 
			0x0,		/* 0 */
/* 898 */	NdrFcLong( 0x0 ),	/* 0 */
/* 902 */	NdrFcLong( 0x0 ),	/* 0 */
/* 906 */	0x1,		/* FC_BYTE */
			0x5b,		/* FC_END */
/* 908 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 910 */	NdrFcShort( 0x8 ),	/* 8 */
/* 912 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 914 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 916 */	NdrFcShort( 0x4 ),	/* 4 */
/* 918 */	NdrFcShort( 0x4 ),	/* 4 */
/* 920 */	0x13, 0x20,	/* FC_OP [maybenull_sizeis] */
/* 922 */	NdrFcShort( 0xffdc ),	/* Offset= -36 (886) */
/* 924 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 926 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 928 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 930 */	NdrFcShort( 0x2 ),	/* 2 */
/* 932 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 934 */	NdrFcShort( 0x0 ),	/* 0 */
/* 936 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 938 */	0x0 , 
			0x0,		/* 0 */
/* 940 */	NdrFcLong( 0x0 ),	/* 0 */
/* 944 */	NdrFcLong( 0x0 ),	/* 0 */
/* 948 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 950 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 952 */	NdrFcShort( 0x8 ),	/* 8 */
/* 954 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 956 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 958 */	NdrFcShort( 0x4 ),	/* 4 */
/* 960 */	NdrFcShort( 0x4 ),	/* 4 */
/* 962 */	0x13, 0x20,	/* FC_OP [maybenull_sizeis] */
/* 964 */	NdrFcShort( 0xffdc ),	/* Offset= -36 (928) */
/* 966 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 968 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 970 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 972 */	NdrFcShort( 0x4 ),	/* 4 */
/* 974 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 976 */	NdrFcShort( 0x0 ),	/* 0 */
/* 978 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 980 */	0x0 , 
			0x0,		/* 0 */
/* 982 */	NdrFcLong( 0x0 ),	/* 0 */
/* 986 */	NdrFcLong( 0x0 ),	/* 0 */
/* 990 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 992 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 994 */	NdrFcShort( 0x8 ),	/* 8 */
/* 996 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 998 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 1000 */	NdrFcShort( 0x4 ),	/* 4 */
/* 1002 */	NdrFcShort( 0x4 ),	/* 4 */
/* 1004 */	0x13, 0x20,	/* FC_OP [maybenull_sizeis] */
/* 1006 */	NdrFcShort( 0xffdc ),	/* Offset= -36 (970) */
/* 1008 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 1010 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 1012 */	
			0x1b,		/* FC_CARRAY */
			0x7,		/* 7 */
/* 1014 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1016 */	0x19,		/* Corr desc:  field pointer, FC_ULONG */
			0x0,		/*  */
/* 1018 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1020 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1022 */	0x0 , 
			0x0,		/* 0 */
/* 1024 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1028 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1032 */	0xb,		/* FC_HYPER */
			0x5b,		/* FC_END */
/* 1034 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 1036 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1038 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 1040 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 1042 */	NdrFcShort( 0x4 ),	/* 4 */
/* 1044 */	NdrFcShort( 0x4 ),	/* 4 */
/* 1046 */	0x13, 0x20,	/* FC_OP [maybenull_sizeis] */
/* 1048 */	NdrFcShort( 0xffdc ),	/* Offset= -36 (1012) */
/* 1050 */	
			0x5b,		/* FC_END */

			0x8,		/* FC_LONG */
/* 1052 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 1054 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 1056 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1058 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 1060 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1062 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 1064 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1066 */	0x7,		/* Corr desc: FC_USHORT */
			0x0,		/*  */
/* 1068 */	NdrFcShort( 0xffd8 ),	/* -40 */
/* 1070 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1072 */	0x0 , 
			0x0,		/* 0 */
/* 1074 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1078 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1082 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1084 */	NdrFcShort( 0xffe2 ),	/* Offset= -30 (1054) */
/* 1086 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1088 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 1090 */	NdrFcShort( 0x28 ),	/* 40 */
/* 1092 */	NdrFcShort( 0xffe2 ),	/* Offset= -30 (1062) */
/* 1094 */	NdrFcShort( 0x0 ),	/* Offset= 0 (1094) */
/* 1096 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1098 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 1100 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1102 */	NdrFcShort( 0xfd5c ),	/* Offset= -676 (426) */
/* 1104 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1106 */	
			0x13, 0x0,	/* FC_OP */
/* 1108 */	NdrFcShort( 0xfeae ),	/* Offset= -338 (770) */
/* 1110 */	
			0x13, 0x8,	/* FC_OP [simple_pointer] */
/* 1112 */	0x1,		/* FC_BYTE */
			0x5c,		/* FC_PAD */
/* 1114 */	
			0x13, 0x8,	/* FC_OP [simple_pointer] */
/* 1116 */	0x6,		/* FC_SHORT */
			0x5c,		/* FC_PAD */
/* 1118 */	
			0x13, 0x8,	/* FC_OP [simple_pointer] */
/* 1120 */	0x8,		/* FC_LONG */
			0x5c,		/* FC_PAD */
/* 1122 */	
			0x13, 0x8,	/* FC_OP [simple_pointer] */
/* 1124 */	0xb,		/* FC_HYPER */
			0x5c,		/* FC_PAD */
/* 1126 */	
			0x13, 0x8,	/* FC_OP [simple_pointer] */
/* 1128 */	0xa,		/* FC_FLOAT */
			0x5c,		/* FC_PAD */
/* 1130 */	
			0x13, 0x8,	/* FC_OP [simple_pointer] */
/* 1132 */	0xc,		/* FC_DOUBLE */
			0x5c,		/* FC_PAD */
/* 1134 */	
			0x13, 0x0,	/* FC_OP */
/* 1136 */	NdrFcShort( 0xfd16 ),	/* Offset= -746 (390) */
/* 1138 */	
			0x13, 0x10,	/* FC_OP [pointer_deref] */
/* 1140 */	NdrFcShort( 0xfd18 ),	/* Offset= -744 (396) */
/* 1142 */	
			0x13, 0x10,	/* FC_OP [pointer_deref] */
/* 1144 */	NdrFcShort( 0xfd18 ),	/* Offset= -744 (400) */
/* 1146 */	
			0x13, 0x10,	/* FC_OP [pointer_deref] */
/* 1148 */	NdrFcShort( 0xfbb8 ),	/* Offset= -1096 (52) */
/* 1150 */	
			0x13, 0x10,	/* FC_OP [pointer_deref] */
/* 1152 */	NdrFcShort( 0xfd22 ),	/* Offset= -734 (418) */
/* 1154 */	
			0x13, 0x10,	/* FC_OP [pointer_deref] */
/* 1156 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1158) */
/* 1158 */	
			0x13, 0x0,	/* FC_OP */
/* 1160 */	NdrFcShort( 0x14 ),	/* Offset= 20 (1180) */
/* 1162 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 1164 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1166 */	0x6,		/* FC_SHORT */
			0x1,		/* FC_BYTE */
/* 1168 */	0x1,		/* FC_BYTE */
			0x8,		/* FC_LONG */
/* 1170 */	0xb,		/* FC_HYPER */
			0x5b,		/* FC_END */
/* 1172 */	
			0x13, 0x0,	/* FC_OP */
/* 1174 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (1162) */
/* 1176 */	
			0x13, 0x8,	/* FC_OP [simple_pointer] */
/* 1178 */	0x2,		/* FC_CHAR */
			0x5c,		/* FC_PAD */
/* 1180 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 1182 */	NdrFcShort( 0x20 ),	/* 32 */
/* 1184 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1186 */	NdrFcShort( 0x0 ),	/* Offset= 0 (1186) */
/* 1188 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 1190 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1192 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1194 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1196 */	NdrFcShort( 0xfba6 ),	/* Offset= -1114 (82) */
/* 1198 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1200 */	0xb4,		/* FC_USER_MARSHAL */
			0x83,		/* 131 */
/* 1202 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1204 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1206 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1208 */	NdrFcShort( 0xfb96 ),	/* Offset= -1130 (78) */

			0x0
        }
    };

static const USER_MARSHAL_ROUTINE_QUADRUPLE UserMarshalRoutines[ WIRE_MARSHAL_TABLE_SIZE ] = 
        {
            
            {
            BSTR_UserSize
            ,BSTR_UserMarshal
            ,BSTR_UserUnmarshal
            ,BSTR_UserFree
            },
            {
            VARIANT_UserSize
            ,VARIANT_UserMarshal
            ,VARIANT_UserUnmarshal
            ,VARIANT_UserFree
            }

        };



/* Standard interface: __MIDL_itf_ContrecOPCServer_0000_0000, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} */


/* Object interface: IUnknown, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}} */


/* Object interface: IDispatch, ver. 0.0,
   GUID={0x00020400,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}} */


/* Object interface: IDataAccess, ver. 0.0,
   GUID={0xA3B7EB56,0x6763,0x426A,{0x8B,0xC5,0x9E,0x7C,0xA2,0x7F,0x77,0xCB}} */

#pragma code_seg(".orpc")
static const unsigned short IDataAccess_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    0,
    42
    };

static const MIDL_STUBLESS_PROXY_INFO IDataAccess_ProxyInfo =
    {
    &Object_StubDesc,
    ContrecOPCServer__MIDL_ProcFormatString.Format,
    &IDataAccess_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO IDataAccess_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    ContrecOPCServer__MIDL_ProcFormatString.Format,
    &IDataAccess_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(9) _IDataAccessProxyVtbl = 
{
    &IDataAccess_ProxyInfo,
    &IID_IDataAccess,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IDispatch::GetTypeInfoCount */ ,
    0 /* IDispatch::GetTypeInfo */ ,
    0 /* IDispatch::GetIDsOfNames */ ,
    0 /* IDispatch_Invoke_Proxy */ ,
    (void *) (INT_PTR) -1 /* IDataAccess::GetRecordSet */ ,
    (void *) (INT_PTR) -1 /* IDataAccess::ExecuteQuery */
};


static const PRPC_STUB_FUNCTION IDataAccess_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    NdrStubCall2,
    NdrStubCall2
};

CInterfaceStubVtbl _IDataAccessStubVtbl =
{
    &IID_IDataAccess,
    &IDataAccess_ServerInfo,
    9,
    &IDataAccess_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Object interface: IContrecs, ver. 0.0,
   GUID={0xC412CB25,0x436D,0x4536,{0x8E,0x3E,0x4B,0xDD,0x0F,0xBD,0xE8,0x69}} */

#pragma code_seg(".orpc")
static const unsigned short IContrecs_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    78,
    114,
    156,
    192,
    228,
    270
    };

static const MIDL_STUBLESS_PROXY_INFO IContrecs_ProxyInfo =
    {
    &Object_StubDesc,
    ContrecOPCServer__MIDL_ProcFormatString.Format,
    &IContrecs_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO IContrecs_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    ContrecOPCServer__MIDL_ProcFormatString.Format,
    &IContrecs_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(13) _IContrecsProxyVtbl = 
{
    &IContrecs_ProxyInfo,
    &IID_IContrecs,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IDispatch::GetTypeInfoCount */ ,
    0 /* IDispatch::GetTypeInfo */ ,
    0 /* IDispatch::GetIDsOfNames */ ,
    0 /* IDispatch_Invoke_Proxy */ ,
    (void *) (INT_PTR) -1 /* IContrecs::Enumerate */ ,
    (void *) (INT_PTR) -1 /* IContrecs::Add */ ,
    (void *) (INT_PTR) -1 /* IContrecs::Modify */ ,
    (void *) (INT_PTR) -1 /* IContrecs::Purge */ ,
    (void *) (INT_PTR) -1 /* IContrecs::GetIndex */ ,
    (void *) (INT_PTR) -1 /* IContrecs::Get */
};


static const PRPC_STUB_FUNCTION IContrecs_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2
};

CInterfaceStubVtbl _IContrecsStubVtbl =
{
    &IID_IContrecs,
    &IContrecs_ServerInfo,
    13,
    &IContrecs_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Object interface: IPorts, ver. 0.0,
   GUID={0xD7A227C0,0xD386,0x408A,{0x8B,0x2D,0xE0,0x0E,0x2F,0x27,0x0D,0x32}} */

#pragma code_seg(".orpc")
static const unsigned short IPorts_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    78,
    114,
    156,
    192,
    228,
    270,
    312
    };

static const MIDL_STUBLESS_PROXY_INFO IPorts_ProxyInfo =
    {
    &Object_StubDesc,
    ContrecOPCServer__MIDL_ProcFormatString.Format,
    &IPorts_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO IPorts_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    ContrecOPCServer__MIDL_ProcFormatString.Format,
    &IPorts_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(14) _IPortsProxyVtbl = 
{
    &IPorts_ProxyInfo,
    &IID_IPorts,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IDispatch::GetTypeInfoCount */ ,
    0 /* IDispatch::GetTypeInfo */ ,
    0 /* IDispatch::GetIDsOfNames */ ,
    0 /* IDispatch_Invoke_Proxy */ ,
    (void *) (INT_PTR) -1 /* IPorts::Enumerate */ ,
    (void *) (INT_PTR) -1 /* IPorts::Add */ ,
    (void *) (INT_PTR) -1 /* IPorts::Modify */ ,
    (void *) (INT_PTR) -1 /* IPorts::Purge */ ,
    (void *) (INT_PTR) -1 /* IPorts::GetIndex */ ,
    (void *) (INT_PTR) -1 /* IPorts::Get */ ,
    (void *) (INT_PTR) -1 /* IPorts::EnumeratePortIDs */
};


static const PRPC_STUB_FUNCTION IPorts_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2
};

CInterfaceStubVtbl _IPortsStubVtbl =
{
    &IID_IPorts,
    &IPorts_ServerInfo,
    14,
    &IPorts_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};

static const MIDL_STUB_DESC Object_StubDesc = 
    {
    0,
    NdrOleAllocate,
    NdrOleFree,
    0,
    0,
    0,
    0,
    0,
    ContrecOPCServer__MIDL_TypeFormatString.Format,
    1, /* -error bounds_check flag */
    0x60001, /* Ndr library version */
    0,
    0x801026e, /* MIDL Version 8.1.622 */
    0,
    UserMarshalRoutines,
    0,  /* notify & notify_flag routine table */
    0x1, /* MIDL flag */
    0, /* cs routines */
    0,   /* proxy/server info */
    0
    };

const CInterfaceProxyVtbl * const _ContrecOPCServer_ProxyVtblList[] = 
{
    ( CInterfaceProxyVtbl *) &_IContrecsProxyVtbl,
    ( CInterfaceProxyVtbl *) &_IDataAccessProxyVtbl,
    ( CInterfaceProxyVtbl *) &_IPortsProxyVtbl,
    0
};

const CInterfaceStubVtbl * const _ContrecOPCServer_StubVtblList[] = 
{
    ( CInterfaceStubVtbl *) &_IContrecsStubVtbl,
    ( CInterfaceStubVtbl *) &_IDataAccessStubVtbl,
    ( CInterfaceStubVtbl *) &_IPortsStubVtbl,
    0
};

PCInterfaceName const _ContrecOPCServer_InterfaceNamesList[] = 
{
    "IContrecs",
    "IDataAccess",
    "IPorts",
    0
};

const IID *  const _ContrecOPCServer_BaseIIDList[] = 
{
    &IID_IDispatch,
    &IID_IDispatch,
    &IID_IDispatch,
    0
};


#define _ContrecOPCServer_CHECK_IID(n)	IID_GENERIC_CHECK_IID( _ContrecOPCServer, pIID, n)

int __stdcall _ContrecOPCServer_IID_Lookup( const IID * pIID, int * pIndex )
{
    IID_BS_LOOKUP_SETUP

    IID_BS_LOOKUP_INITIAL_TEST( _ContrecOPCServer, 3, 2 )
    IID_BS_LOOKUP_NEXT_TEST( _ContrecOPCServer, 1 )
    IID_BS_LOOKUP_RETURN_RESULT( _ContrecOPCServer, 3, *pIndex )
    
}

const ExtendedProxyFileInfo ContrecOPCServer_ProxyFileInfo = 
{
    (PCInterfaceProxyVtblList *) & _ContrecOPCServer_ProxyVtblList,
    (PCInterfaceStubVtblList *) & _ContrecOPCServer_StubVtblList,
    (const PCInterfaceName * ) & _ContrecOPCServer_InterfaceNamesList,
    (const IID ** ) & _ContrecOPCServer_BaseIIDList,
    & _ContrecOPCServer_IID_Lookup, 
    3,
    2,
    0, /* table of [async_uuid] interfaces */
    0, /* Filler1 */
    0, /* Filler2 */
    0  /* Filler3 */
};
#pragma optimize("", on )
#if _MSC_VER >= 1200
#pragma warning(pop)
#endif


#endif /* !defined(_M_IA64) && !defined(_M_AMD64) && !defined(_ARM_) */

