

/* this ALWAYS GENERATED file contains the proxy stub code */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Mon Jan 18 22:14:07 2038
 */
/* Compiler settings for ContrecOPCObjects.idl:
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


#include "ContrecOPCObjects.h"

#define TYPE_FORMAT_STRING_SIZE   185                               
#define PROC_FORMAT_STRING_SIZE   1573                              
#define EXPR_FORMAT_STRING_SIZE   1                                 
#define TRANSMIT_AS_TABLE_SIZE    0            
#define WIRE_MARSHAL_TABLE_SIZE   1            

typedef struct _ContrecOPCObjects_MIDL_TYPE_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ TYPE_FORMAT_STRING_SIZE ];
    } ContrecOPCObjects_MIDL_TYPE_FORMAT_STRING;

typedef struct _ContrecOPCObjects_MIDL_PROC_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ PROC_FORMAT_STRING_SIZE ];
    } ContrecOPCObjects_MIDL_PROC_FORMAT_STRING;

typedef struct _ContrecOPCObjects_MIDL_EXPR_FORMAT_STRING
    {
    long          Pad;
    unsigned char  Format[ EXPR_FORMAT_STRING_SIZE ];
    } ContrecOPCObjects_MIDL_EXPR_FORMAT_STRING;


static const RPC_SYNTAX_IDENTIFIER  _RpcTransferSyntax = 
{{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}};


extern const ContrecOPCObjects_MIDL_TYPE_FORMAT_STRING ContrecOPCObjects__MIDL_TypeFormatString;
extern const ContrecOPCObjects_MIDL_PROC_FORMAT_STRING ContrecOPCObjects__MIDL_ProcFormatString;
extern const ContrecOPCObjects_MIDL_EXPR_FORMAT_STRING ContrecOPCObjects__MIDL_ExprFormatString;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO IPort_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO IPort_ProxyInfo;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO IContrec_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO IContrec_ProxyInfo;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO IPortCollection_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO IPortCollection_ProxyInfo;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO IContrecCollection_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO IContrecCollection_ProxyInfo;


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


static const ContrecOPCObjects_MIDL_PROC_FORMAT_STRING ContrecOPCObjects__MIDL_ProcFormatString =
    {
        0,
        {

	/* Procedure get_Index */


	/* Procedure get_Index */

			0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/*  2 */	NdrFcLong( 0x0 ),	/* 0 */
/*  6 */	NdrFcShort( 0x7 ),	/* 7 */
/*  8 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 10 */	NdrFcShort( 0x0 ),	/* 0 */
/* 12 */	NdrFcShort( 0x24 ),	/* 36 */
/* 14 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 16 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 18 */	NdrFcShort( 0x0 ),	/* 0 */
/* 20 */	NdrFcShort( 0x0 ),	/* 0 */
/* 22 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */


	/* Parameter pVal */

/* 24 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 26 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 28 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */


	/* Return value */

/* 30 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 32 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 34 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure put_Index */


	/* Procedure put_Index */

/* 36 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 38 */	NdrFcLong( 0x0 ),	/* 0 */
/* 42 */	NdrFcShort( 0x8 ),	/* 8 */
/* 44 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 46 */	NdrFcShort( 0x8 ),	/* 8 */
/* 48 */	NdrFcShort( 0x8 ),	/* 8 */
/* 50 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 52 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 54 */	NdrFcShort( 0x0 ),	/* 0 */
/* 56 */	NdrFcShort( 0x0 ),	/* 0 */
/* 58 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter newVal */


	/* Parameter newVal */

/* 60 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 62 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 64 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */


	/* Return value */

/* 66 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 68 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 70 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_ID */


	/* Procedure get_ID */

/* 72 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 74 */	NdrFcLong( 0x0 ),	/* 0 */
/* 78 */	NdrFcShort( 0x9 ),	/* 9 */
/* 80 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 82 */	NdrFcShort( 0x0 ),	/* 0 */
/* 84 */	NdrFcShort( 0x8 ),	/* 8 */
/* 86 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 88 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 90 */	NdrFcShort( 0x1 ),	/* 1 */
/* 92 */	NdrFcShort( 0x0 ),	/* 0 */
/* 94 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */


	/* Parameter pVal */

/* 96 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 98 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 100 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */


	/* Return value */

/* 102 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 104 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 106 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure put_ID */


	/* Procedure put_ID */

/* 108 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 110 */	NdrFcLong( 0x0 ),	/* 0 */
/* 114 */	NdrFcShort( 0xa ),	/* 10 */
/* 116 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 118 */	NdrFcShort( 0x0 ),	/* 0 */
/* 120 */	NdrFcShort( 0x8 ),	/* 8 */
/* 122 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 124 */	0x8,		/* 8 */
			0x45,		/* Ext Flags:  new corr desc, srv corr check, has range on conformance */
/* 126 */	NdrFcShort( 0x0 ),	/* 0 */
/* 128 */	NdrFcShort( 0x1 ),	/* 1 */
/* 130 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter newVal */


	/* Parameter newVal */

/* 132 */	NdrFcShort( 0x8b ),	/* Flags:  must size, must free, in, by val, */
/* 134 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 136 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Return value */


	/* Return value */

/* 138 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 140 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 142 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_Type */


	/* Procedure get_Baud */

/* 144 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 146 */	NdrFcLong( 0x0 ),	/* 0 */
/* 150 */	NdrFcShort( 0xb ),	/* 11 */
/* 152 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 154 */	NdrFcShort( 0x0 ),	/* 0 */
/* 156 */	NdrFcShort( 0x22 ),	/* 34 */
/* 158 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 160 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 162 */	NdrFcShort( 0x0 ),	/* 0 */
/* 164 */	NdrFcShort( 0x0 ),	/* 0 */
/* 166 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */


	/* Parameter pVal */

/* 168 */	NdrFcShort( 0x2010 ),	/* Flags:  out, srv alloc size=8 */
/* 170 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 172 */	NdrFcShort( 0x46 ),	/* Type Offset=70 */

	/* Return value */


	/* Return value */

/* 174 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 176 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 178 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure put_Type */


	/* Procedure put_Baud */

/* 180 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 182 */	NdrFcLong( 0x0 ),	/* 0 */
/* 186 */	NdrFcShort( 0xc ),	/* 12 */
/* 188 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 190 */	NdrFcShort( 0x6 ),	/* 6 */
/* 192 */	NdrFcShort( 0x8 ),	/* 8 */
/* 194 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 196 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 198 */	NdrFcShort( 0x0 ),	/* 0 */
/* 200 */	NdrFcShort( 0x0 ),	/* 0 */
/* 202 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter newVal */


	/* Parameter newVal */

/* 204 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 206 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 208 */	0xd,		/* FC_ENUM16 */
			0x0,		/* 0 */

	/* Return value */


	/* Return value */

/* 210 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 212 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 214 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_DataBits */

/* 216 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 218 */	NdrFcLong( 0x0 ),	/* 0 */
/* 222 */	NdrFcShort( 0xd ),	/* 13 */
/* 224 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 226 */	NdrFcShort( 0x0 ),	/* 0 */
/* 228 */	NdrFcShort( 0x22 ),	/* 34 */
/* 230 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 232 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 234 */	NdrFcShort( 0x0 ),	/* 0 */
/* 236 */	NdrFcShort( 0x0 ),	/* 0 */
/* 238 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 240 */	NdrFcShort( 0x2010 ),	/* Flags:  out, srv alloc size=8 */
/* 242 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 244 */	NdrFcShort( 0x46 ),	/* Type Offset=70 */

	/* Return value */

/* 246 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 248 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 250 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure put_DataBits */

/* 252 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 254 */	NdrFcLong( 0x0 ),	/* 0 */
/* 258 */	NdrFcShort( 0xe ),	/* 14 */
/* 260 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 262 */	NdrFcShort( 0x6 ),	/* 6 */
/* 264 */	NdrFcShort( 0x8 ),	/* 8 */
/* 266 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 268 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 270 */	NdrFcShort( 0x0 ),	/* 0 */
/* 272 */	NdrFcShort( 0x0 ),	/* 0 */
/* 274 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter newVal */

/* 276 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 278 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 280 */	0xd,		/* FC_ENUM16 */
			0x0,		/* 0 */

	/* Return value */

/* 282 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 284 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 286 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_Parity */

/* 288 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 290 */	NdrFcLong( 0x0 ),	/* 0 */
/* 294 */	NdrFcShort( 0xf ),	/* 15 */
/* 296 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 298 */	NdrFcShort( 0x0 ),	/* 0 */
/* 300 */	NdrFcShort( 0x22 ),	/* 34 */
/* 302 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 304 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 306 */	NdrFcShort( 0x0 ),	/* 0 */
/* 308 */	NdrFcShort( 0x0 ),	/* 0 */
/* 310 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 312 */	NdrFcShort( 0x2010 ),	/* Flags:  out, srv alloc size=8 */
/* 314 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 316 */	NdrFcShort( 0x46 ),	/* Type Offset=70 */

	/* Return value */

/* 318 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 320 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 322 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure put_Parity */

/* 324 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 326 */	NdrFcLong( 0x0 ),	/* 0 */
/* 330 */	NdrFcShort( 0x10 ),	/* 16 */
/* 332 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 334 */	NdrFcShort( 0x6 ),	/* 6 */
/* 336 */	NdrFcShort( 0x8 ),	/* 8 */
/* 338 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 340 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 342 */	NdrFcShort( 0x0 ),	/* 0 */
/* 344 */	NdrFcShort( 0x0 ),	/* 0 */
/* 346 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter newVal */

/* 348 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 350 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 352 */	0xd,		/* FC_ENUM16 */
			0x0,		/* 0 */

	/* Return value */

/* 354 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 356 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 358 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_StopBits */

/* 360 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 362 */	NdrFcLong( 0x0 ),	/* 0 */
/* 366 */	NdrFcShort( 0x11 ),	/* 17 */
/* 368 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 370 */	NdrFcShort( 0x0 ),	/* 0 */
/* 372 */	NdrFcShort( 0x22 ),	/* 34 */
/* 374 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 376 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 378 */	NdrFcShort( 0x0 ),	/* 0 */
/* 380 */	NdrFcShort( 0x0 ),	/* 0 */
/* 382 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 384 */	NdrFcShort( 0x2010 ),	/* Flags:  out, srv alloc size=8 */
/* 386 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 388 */	NdrFcShort( 0x46 ),	/* Type Offset=70 */

	/* Return value */

/* 390 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 392 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 394 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure put_StopBits */

/* 396 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 398 */	NdrFcLong( 0x0 ),	/* 0 */
/* 402 */	NdrFcShort( 0x12 ),	/* 18 */
/* 404 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 406 */	NdrFcShort( 0x6 ),	/* 6 */
/* 408 */	NdrFcShort( 0x8 ),	/* 8 */
/* 410 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 412 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 414 */	NdrFcShort( 0x0 ),	/* 0 */
/* 416 */	NdrFcShort( 0x0 ),	/* 0 */
/* 418 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter newVal */

/* 420 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 422 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 424 */	0xd,		/* FC_ENUM16 */
			0x0,		/* 0 */

	/* Return value */

/* 426 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 428 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 430 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure TypeID */


	/* Procedure BaudID */

/* 432 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 434 */	NdrFcLong( 0x0 ),	/* 0 */
/* 438 */	NdrFcShort( 0x13 ),	/* 19 */
/* 440 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 442 */	NdrFcShort( 0x6 ),	/* 6 */
/* 444 */	NdrFcShort( 0x8 ),	/* 8 */
/* 446 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 448 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 450 */	NdrFcShort( 0x1 ),	/* 1 */
/* 452 */	NdrFcShort( 0x0 ),	/* 0 */
/* 454 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter Type */


	/* Parameter Baud */

/* 456 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 458 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 460 */	0xd,		/* FC_ENUM16 */
			0x0,		/* 0 */

	/* Parameter pVal */


	/* Parameter pVal */

/* 462 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 464 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 466 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */


	/* Return value */

/* 468 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 470 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 472 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure DataBitsID */

/* 474 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 476 */	NdrFcLong( 0x0 ),	/* 0 */
/* 480 */	NdrFcShort( 0x14 ),	/* 20 */
/* 482 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 484 */	NdrFcShort( 0x6 ),	/* 6 */
/* 486 */	NdrFcShort( 0x8 ),	/* 8 */
/* 488 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 490 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 492 */	NdrFcShort( 0x1 ),	/* 1 */
/* 494 */	NdrFcShort( 0x0 ),	/* 0 */
/* 496 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter DataBits */

/* 498 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 500 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 502 */	0xd,		/* FC_ENUM16 */
			0x0,		/* 0 */

	/* Parameter pVal */

/* 504 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 506 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 508 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */

/* 510 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 512 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 514 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure ParityID */

/* 516 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 518 */	NdrFcLong( 0x0 ),	/* 0 */
/* 522 */	NdrFcShort( 0x15 ),	/* 21 */
/* 524 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 526 */	NdrFcShort( 0x6 ),	/* 6 */
/* 528 */	NdrFcShort( 0x8 ),	/* 8 */
/* 530 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 532 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 534 */	NdrFcShort( 0x1 ),	/* 1 */
/* 536 */	NdrFcShort( 0x0 ),	/* 0 */
/* 538 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter Parity */

/* 540 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 542 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 544 */	0xd,		/* FC_ENUM16 */
			0x0,		/* 0 */

	/* Parameter pVal */

/* 546 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 548 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 550 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */

/* 552 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 554 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 556 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure StopBitsID */

/* 558 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 560 */	NdrFcLong( 0x0 ),	/* 0 */
/* 564 */	NdrFcShort( 0x16 ),	/* 22 */
/* 566 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 568 */	NdrFcShort( 0x6 ),	/* 6 */
/* 570 */	NdrFcShort( 0x8 ),	/* 8 */
/* 572 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 574 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 576 */	NdrFcShort( 0x1 ),	/* 1 */
/* 578 */	NdrFcShort( 0x0 ),	/* 0 */
/* 580 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter StopBits */

/* 582 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 584 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 586 */	0xd,		/* FC_ENUM16 */
			0x0,		/* 0 */

	/* Parameter pVal */

/* 588 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 590 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 592 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */

/* 594 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 596 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 598 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Load */

/* 600 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 602 */	NdrFcLong( 0x0 ),	/* 0 */
/* 606 */	NdrFcShort( 0x17 ),	/* 23 */
/* 608 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 610 */	NdrFcShort( 0x0 ),	/* 0 */
/* 612 */	NdrFcShort( 0x8 ),	/* 8 */
/* 614 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 616 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 618 */	NdrFcShort( 0x0 ),	/* 0 */
/* 620 */	NdrFcShort( 0x0 ),	/* 0 */
/* 622 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pDispatch */

/* 624 */	NdrFcShort( 0xb ),	/* Flags:  must size, must free, in, */
/* 626 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 628 */	NdrFcShort( 0x4a ),	/* Type Offset=74 */

	/* Return value */

/* 630 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 632 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 634 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Reset */

/* 636 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 638 */	NdrFcLong( 0x0 ),	/* 0 */
/* 642 */	NdrFcShort( 0x18 ),	/* 24 */
/* 644 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 646 */	NdrFcShort( 0x0 ),	/* 0 */
/* 648 */	NdrFcShort( 0x8 ),	/* 8 */
/* 650 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x1,		/* 1 */
/* 652 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 654 */	NdrFcShort( 0x0 ),	/* 0 */
/* 656 */	NdrFcShort( 0x0 ),	/* 0 */
/* 658 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Return value */

/* 660 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 662 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 664 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_SelectSQL */


	/* Procedure get_InsertSQL */

/* 666 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 668 */	NdrFcLong( 0x0 ),	/* 0 */
/* 672 */	NdrFcShort( 0x19 ),	/* 25 */
/* 674 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 676 */	NdrFcShort( 0x0 ),	/* 0 */
/* 678 */	NdrFcShort( 0x8 ),	/* 8 */
/* 680 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 682 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 684 */	NdrFcShort( 0x1 ),	/* 1 */
/* 686 */	NdrFcShort( 0x0 ),	/* 0 */
/* 688 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */


	/* Parameter pVal */

/* 690 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 692 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 694 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */


	/* Return value */

/* 696 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 698 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 700 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_SelectByIDSQL */


	/* Procedure get_UpdateSQL */

/* 702 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 704 */	NdrFcLong( 0x0 ),	/* 0 */
/* 708 */	NdrFcShort( 0x1a ),	/* 26 */
/* 710 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 712 */	NdrFcShort( 0x0 ),	/* 0 */
/* 714 */	NdrFcShort( 0x8 ),	/* 8 */
/* 716 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 718 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 720 */	NdrFcShort( 0x1 ),	/* 1 */
/* 722 */	NdrFcShort( 0x0 ),	/* 0 */
/* 724 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */


	/* Parameter pVal */

/* 726 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 728 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 730 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */


	/* Return value */

/* 732 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 734 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 736 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_EnumerateSQL */


	/* Procedure get_PurgeSQL */

/* 738 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 740 */	NdrFcLong( 0x0 ),	/* 0 */
/* 744 */	NdrFcShort( 0x1b ),	/* 27 */
/* 746 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 748 */	NdrFcShort( 0x0 ),	/* 0 */
/* 750 */	NdrFcShort( 0x8 ),	/* 8 */
/* 752 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 754 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 756 */	NdrFcShort( 0x1 ),	/* 1 */
/* 758 */	NdrFcShort( 0x0 ),	/* 0 */
/* 760 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */


	/* Parameter pVal */

/* 762 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 764 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 766 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */


	/* Return value */

/* 768 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 770 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 772 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_SelectSQL */

/* 774 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 776 */	NdrFcLong( 0x0 ),	/* 0 */
/* 780 */	NdrFcShort( 0x1c ),	/* 28 */
/* 782 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 784 */	NdrFcShort( 0x0 ),	/* 0 */
/* 786 */	NdrFcShort( 0x8 ),	/* 8 */
/* 788 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 790 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 792 */	NdrFcShort( 0x1 ),	/* 1 */
/* 794 */	NdrFcShort( 0x0 ),	/* 0 */
/* 796 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 798 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 800 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 802 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */

/* 804 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 806 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 808 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_SelectByIDSQL */

/* 810 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 812 */	NdrFcLong( 0x0 ),	/* 0 */
/* 816 */	NdrFcShort( 0x1d ),	/* 29 */
/* 818 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 820 */	NdrFcShort( 0x0 ),	/* 0 */
/* 822 */	NdrFcShort( 0x8 ),	/* 8 */
/* 824 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 826 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 828 */	NdrFcShort( 0x1 ),	/* 1 */
/* 830 */	NdrFcShort( 0x0 ),	/* 0 */
/* 832 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 834 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 836 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 838 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */

/* 840 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 842 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 844 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_EnumerateSQL */

/* 846 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 848 */	NdrFcLong( 0x0 ),	/* 0 */
/* 852 */	NdrFcShort( 0x1e ),	/* 30 */
/* 854 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 856 */	NdrFcShort( 0x0 ),	/* 0 */
/* 858 */	NdrFcShort( 0x8 ),	/* 8 */
/* 860 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 862 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 864 */	NdrFcShort( 0x1 ),	/* 1 */
/* 866 */	NdrFcShort( 0x0 ),	/* 0 */
/* 868 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 870 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 872 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 874 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */

/* 876 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 878 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 880 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_PortIndex */

/* 882 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 884 */	NdrFcLong( 0x0 ),	/* 0 */
/* 888 */	NdrFcShort( 0xd ),	/* 13 */
/* 890 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 892 */	NdrFcShort( 0x0 ),	/* 0 */
/* 894 */	NdrFcShort( 0x24 ),	/* 36 */
/* 896 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 898 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 900 */	NdrFcShort( 0x0 ),	/* 0 */
/* 902 */	NdrFcShort( 0x0 ),	/* 0 */
/* 904 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 906 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 908 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 910 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 912 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 914 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 916 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure put_PortIndex */

/* 918 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 920 */	NdrFcLong( 0x0 ),	/* 0 */
/* 924 */	NdrFcShort( 0xe ),	/* 14 */
/* 926 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 928 */	NdrFcShort( 0x8 ),	/* 8 */
/* 930 */	NdrFcShort( 0x8 ),	/* 8 */
/* 932 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 934 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 936 */	NdrFcShort( 0x0 ),	/* 0 */
/* 938 */	NdrFcShort( 0x0 ),	/* 0 */
/* 940 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter newVal */

/* 942 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 944 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 946 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 948 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 950 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 952 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_PortID */

/* 954 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 956 */	NdrFcLong( 0x0 ),	/* 0 */
/* 960 */	NdrFcShort( 0xf ),	/* 15 */
/* 962 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 964 */	NdrFcShort( 0x0 ),	/* 0 */
/* 966 */	NdrFcShort( 0x8 ),	/* 8 */
/* 968 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 970 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 972 */	NdrFcShort( 0x1 ),	/* 1 */
/* 974 */	NdrFcShort( 0x0 ),	/* 0 */
/* 976 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 978 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 980 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 982 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */

/* 984 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 986 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 988 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure put_PortID */

/* 990 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 992 */	NdrFcLong( 0x0 ),	/* 0 */
/* 996 */	NdrFcShort( 0x10 ),	/* 16 */
/* 998 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1000 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1002 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1004 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 1006 */	0x8,		/* 8 */
			0x45,		/* Ext Flags:  new corr desc, srv corr check, has range on conformance */
/* 1008 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1010 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1012 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter newVal */

/* 1014 */	NdrFcShort( 0x8b ),	/* Flags:  must size, must free, in, by val, */
/* 1016 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1018 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Return value */

/* 1020 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1022 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1024 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_Address */

/* 1026 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1028 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1032 */	NdrFcShort( 0x11 ),	/* 17 */
/* 1034 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1036 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1038 */	NdrFcShort( 0x21 ),	/* 33 */
/* 1040 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 1042 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1044 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1046 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1048 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 1050 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1052 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1054 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Return value */

/* 1056 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1058 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1060 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure put_Address */

/* 1062 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1064 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1068 */	NdrFcShort( 0x12 ),	/* 18 */
/* 1070 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1072 */	NdrFcShort( 0x5 ),	/* 5 */
/* 1074 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1076 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 1078 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1080 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1082 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1084 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter newVal */

/* 1086 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1088 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1090 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Return value */

/* 1092 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1094 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1096 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Load */

/* 1098 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1100 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1104 */	NdrFcShort( 0x14 ),	/* 20 */
/* 1106 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1108 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1110 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1112 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 1114 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1116 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1118 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1120 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pDispatch */

/* 1122 */	NdrFcShort( 0xb ),	/* Flags:  must size, must free, in, */
/* 1124 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1126 */	NdrFcShort( 0x4a ),	/* Type Offset=74 */

	/* Return value */

/* 1128 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1130 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1132 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Reset */

/* 1134 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1136 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1140 */	NdrFcShort( 0x15 ),	/* 21 */
/* 1142 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1144 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1146 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1148 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x1,		/* 1 */
/* 1150 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1152 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1154 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1156 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Return value */

/* 1158 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1160 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1162 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_InsertSQL */

/* 1164 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1166 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1170 */	NdrFcShort( 0x16 ),	/* 22 */
/* 1172 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1174 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1176 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1178 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 1180 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 1182 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1184 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1186 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 1188 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 1190 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1192 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */

/* 1194 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1196 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1198 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_UpdateSQL */

/* 1200 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1202 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1206 */	NdrFcShort( 0x17 ),	/* 23 */
/* 1208 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1210 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1212 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1214 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 1216 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 1218 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1220 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1222 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 1224 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 1226 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1228 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */

/* 1230 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1232 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1234 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_PurgeSQL */

/* 1236 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1238 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1242 */	NdrFcShort( 0x18 ),	/* 24 */
/* 1244 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1246 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1248 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1250 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 1252 */	0x8,		/* 8 */
			0x43,		/* Ext Flags:  new corr desc, clt corr check, has range on conformance */
/* 1254 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1256 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1258 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 1260 */	NdrFcShort( 0x2113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=8 */
/* 1262 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1264 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Return value */

/* 1266 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1268 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1270 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Item */

/* 1272 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1274 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1278 */	NdrFcShort( 0x7 ),	/* 7 */
/* 1280 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1282 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1284 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1286 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 1288 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1290 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1292 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1294 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter Index */

/* 1296 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1298 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1300 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pVal */

/* 1302 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 1304 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1306 */	NdrFcShort( 0x60 ),	/* Type Offset=96 */

	/* Return value */

/* 1308 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1310 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1312 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Add */

/* 1314 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1316 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1320 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1322 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1324 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1326 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1328 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 1330 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1332 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1334 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1336 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pNewVal */

/* 1338 */	NdrFcShort( 0xb ),	/* Flags:  must size, must free, in, */
/* 1340 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1342 */	NdrFcShort( 0x64 ),	/* Type Offset=100 */

	/* Return value */

/* 1344 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1346 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1348 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get_Count */


	/* Procedure get_Count */

/* 1350 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1352 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1356 */	NdrFcShort( 0x9 ),	/* 9 */
/* 1358 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1360 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1362 */	NdrFcShort( 0x24 ),	/* 36 */
/* 1364 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 1366 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1368 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1370 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1372 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */


	/* Parameter pVal */

/* 1374 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1376 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1378 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */


	/* Return value */

/* 1380 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1382 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1384 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Remove */


	/* Procedure Remove */

/* 1386 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1388 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1392 */	NdrFcShort( 0xa ),	/* 10 */
/* 1394 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1396 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1398 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1400 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 1402 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1404 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1406 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1408 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter Index */


	/* Parameter Index */

/* 1410 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1412 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1414 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */


	/* Return value */

/* 1416 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1418 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1420 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get__NewEnum */

/* 1422 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1424 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1428 */	NdrFcShort( 0xb ),	/* 11 */
/* 1430 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1432 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1434 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1436 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 1438 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1440 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1442 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1444 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 1446 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 1448 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1450 */	NdrFcShort( 0x76 ),	/* Type Offset=118 */

	/* Return value */

/* 1452 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1454 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1456 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Item */

/* 1458 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1460 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1464 */	NdrFcShort( 0x7 ),	/* 7 */
/* 1466 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1468 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1470 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1472 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 1474 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1476 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1478 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1480 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter Index */

/* 1482 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1484 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1486 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pVal */

/* 1488 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 1490 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1492 */	NdrFcShort( 0x8c ),	/* Type Offset=140 */

	/* Return value */

/* 1494 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1496 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1498 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Add */

/* 1500 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1502 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1506 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1508 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1510 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1512 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1514 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 1516 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1518 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1520 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1522 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pNewVal */

/* 1524 */	NdrFcShort( 0xb ),	/* Flags:  must size, must free, in, */
/* 1526 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1528 */	NdrFcShort( 0x90 ),	/* Type Offset=144 */

	/* Return value */

/* 1530 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1532 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1534 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure get__NewEnum */

/* 1536 */	0x33,		/* FC_AUTO_HANDLE */
			0x6c,		/* Old Flags:  object, Oi2 */
/* 1538 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1542 */	NdrFcShort( 0xb ),	/* 11 */
/* 1544 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1546 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1548 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1550 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x2,		/* 2 */
/* 1552 */	0x8,		/* 8 */
			0x41,		/* Ext Flags:  new corr desc, has range on conformance */
/* 1554 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1556 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1558 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pVal */

/* 1560 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 1562 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1564 */	NdrFcShort( 0xa2 ),	/* Type Offset=162 */

	/* Return value */

/* 1566 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1568 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1570 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

			0x0
        }
    };

static const ContrecOPCObjects_MIDL_TYPE_FORMAT_STRING ContrecOPCObjects__MIDL_TypeFormatString =
    {
        0,
        {
			NdrFcShort( 0x0 ),	/* 0 */
/*  2 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/*  4 */	0x8,		/* FC_LONG */
			0x5c,		/* FC_PAD */
/*  6 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/*  8 */	NdrFcShort( 0x26 ),	/* Offset= 38 (46) */
/* 10 */	
			0x13, 0x0,	/* FC_OP */
/* 12 */	NdrFcShort( 0x18 ),	/* Offset= 24 (36) */
/* 14 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 16 */	NdrFcShort( 0x2 ),	/* 2 */
/* 18 */	0x9,		/* Corr desc: FC_ULONG */
			0x0,		/*  */
/* 20 */	NdrFcShort( 0xfffc ),	/* -4 */
/* 22 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 24 */	0x0 , 
			0x0,		/* 0 */
/* 26 */	NdrFcLong( 0x0 ),	/* 0 */
/* 30 */	NdrFcLong( 0x0 ),	/* 0 */
/* 34 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 36 */	
			0x17,		/* FC_CSTRUCT */
			0x3,		/* 3 */
/* 38 */	NdrFcShort( 0x8 ),	/* 8 */
/* 40 */	NdrFcShort( 0xffe6 ),	/* Offset= -26 (14) */
/* 42 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 44 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 46 */	0xb4,		/* FC_USER_MARSHAL */
			0x83,		/* 131 */
/* 48 */	NdrFcShort( 0x0 ),	/* 0 */
/* 50 */	NdrFcShort( 0x4 ),	/* 4 */
/* 52 */	NdrFcShort( 0x0 ),	/* 0 */
/* 54 */	NdrFcShort( 0xffd4 ),	/* Offset= -44 (10) */
/* 56 */	
			0x12, 0x0,	/* FC_UP */
/* 58 */	NdrFcShort( 0xffea ),	/* Offset= -22 (36) */
/* 60 */	0xb4,		/* FC_USER_MARSHAL */
			0x83,		/* 131 */
/* 62 */	NdrFcShort( 0x0 ),	/* 0 */
/* 64 */	NdrFcShort( 0x4 ),	/* 4 */
/* 66 */	NdrFcShort( 0x0 ),	/* 0 */
/* 68 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (56) */
/* 70 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/* 72 */	0xd,		/* FC_ENUM16 */
			0x5c,		/* FC_PAD */
/* 74 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 76 */	NdrFcLong( 0x20400 ),	/* 132096 */
/* 80 */	NdrFcShort( 0x0 ),	/* 0 */
/* 82 */	NdrFcShort( 0x0 ),	/* 0 */
/* 84 */	0xc0,		/* 192 */
			0x0,		/* 0 */
/* 86 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 88 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 90 */	0x0,		/* 0 */
			0x46,		/* 70 */
/* 92 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/* 94 */	0x2,		/* FC_CHAR */
			0x5c,		/* FC_PAD */
/* 96 */	
			0x11, 0x10,	/* FC_RP [pointer_deref] */
/* 98 */	NdrFcShort( 0x2 ),	/* Offset= 2 (100) */
/* 100 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 102 */	NdrFcLong( 0xa936832e ),	/* -1456045266 */
/* 106 */	NdrFcShort( 0x3f54 ),	/* 16212 */
/* 108 */	NdrFcShort( 0x4645 ),	/* 17989 */
/* 110 */	0x8d,		/* 141 */
			0x8f,		/* 143 */
/* 112 */	0xfa,		/* 250 */
			0xbf,		/* 191 */
/* 114 */	0xd6,		/* 214 */
			0x6d,		/* 109 */
/* 116 */	0xa0,		/* 160 */
			0xb8,		/* 184 */
/* 118 */	
			0x11, 0x10,	/* FC_RP [pointer_deref] */
/* 120 */	NdrFcShort( 0x2 ),	/* Offset= 2 (122) */
/* 122 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 124 */	NdrFcLong( 0x0 ),	/* 0 */
/* 128 */	NdrFcShort( 0x0 ),	/* 0 */
/* 130 */	NdrFcShort( 0x0 ),	/* 0 */
/* 132 */	0xc0,		/* 192 */
			0x0,		/* 0 */
/* 134 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 136 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 138 */	0x0,		/* 0 */
			0x46,		/* 70 */
/* 140 */	
			0x11, 0x10,	/* FC_RP [pointer_deref] */
/* 142 */	NdrFcShort( 0x2 ),	/* Offset= 2 (144) */
/* 144 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 146 */	NdrFcLong( 0x467bede3 ),	/* 1182526947 */
/* 150 */	NdrFcShort( 0x88b ),	/* 2187 */
/* 152 */	NdrFcShort( 0x4d08 ),	/* 19720 */
/* 154 */	0x98,		/* 152 */
			0x5f,		/* 95 */
/* 156 */	0xf5,		/* 245 */
			0xea,		/* 234 */
/* 158 */	0x54,		/* 84 */
			0x56,		/* 86 */
/* 160 */	0xc2,		/* 194 */
			0x21,		/* 33 */
/* 162 */	
			0x11, 0x10,	/* FC_RP [pointer_deref] */
/* 164 */	NdrFcShort( 0x2 ),	/* Offset= 2 (166) */
/* 166 */	
			0x2f,		/* FC_IP */
			0x5a,		/* FC_CONSTANT_IID */
/* 168 */	NdrFcLong( 0x0 ),	/* 0 */
/* 172 */	NdrFcShort( 0x0 ),	/* 0 */
/* 174 */	NdrFcShort( 0x0 ),	/* 0 */
/* 176 */	0xc0,		/* 192 */
			0x0,		/* 0 */
/* 178 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 180 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 182 */	0x0,		/* 0 */
			0x46,		/* 70 */

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
            }

        };



/* Standard interface: __MIDL_itf_ContrecOPCObjects_0000_0000, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} */


/* Object interface: IUnknown, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}} */


/* Object interface: IDispatch, ver. 0.0,
   GUID={0x00020400,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}} */


/* Object interface: IPort, ver. 0.0,
   GUID={0xA936832E,0x3F54,0x4645,{0x8D,0x8F,0xFA,0xBF,0xD6,0x6D,0xA0,0xB8}} */

#pragma code_seg(".orpc")
static const unsigned short IPort_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    0,
    36,
    72,
    108,
    144,
    180,
    216,
    252,
    288,
    324,
    360,
    396,
    432,
    474,
    516,
    558,
    600,
    636,
    666,
    702,
    738,
    774,
    810,
    846
    };

static const MIDL_STUBLESS_PROXY_INFO IPort_ProxyInfo =
    {
    &Object_StubDesc,
    ContrecOPCObjects__MIDL_ProcFormatString.Format,
    &IPort_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO IPort_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    ContrecOPCObjects__MIDL_ProcFormatString.Format,
    &IPort_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(31) _IPortProxyVtbl = 
{
    &IPort_ProxyInfo,
    &IID_IPort,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IDispatch::GetTypeInfoCount */ ,
    0 /* IDispatch::GetTypeInfo */ ,
    0 /* IDispatch::GetIDsOfNames */ ,
    0 /* IDispatch_Invoke_Proxy */ ,
    (void *) (INT_PTR) -1 /* IPort::get_Index */ ,
    (void *) (INT_PTR) -1 /* IPort::put_Index */ ,
    (void *) (INT_PTR) -1 /* IPort::get_ID */ ,
    (void *) (INT_PTR) -1 /* IPort::put_ID */ ,
    (void *) (INT_PTR) -1 /* IPort::get_Baud */ ,
    (void *) (INT_PTR) -1 /* IPort::put_Baud */ ,
    (void *) (INT_PTR) -1 /* IPort::get_DataBits */ ,
    (void *) (INT_PTR) -1 /* IPort::put_DataBits */ ,
    (void *) (INT_PTR) -1 /* IPort::get_Parity */ ,
    (void *) (INT_PTR) -1 /* IPort::put_Parity */ ,
    (void *) (INT_PTR) -1 /* IPort::get_StopBits */ ,
    (void *) (INT_PTR) -1 /* IPort::put_StopBits */ ,
    (void *) (INT_PTR) -1 /* IPort::BaudID */ ,
    (void *) (INT_PTR) -1 /* IPort::DataBitsID */ ,
    (void *) (INT_PTR) -1 /* IPort::ParityID */ ,
    (void *) (INT_PTR) -1 /* IPort::StopBitsID */ ,
    (void *) (INT_PTR) -1 /* IPort::Load */ ,
    (void *) (INT_PTR) -1 /* IPort::Reset */ ,
    (void *) (INT_PTR) -1 /* IPort::get_InsertSQL */ ,
    (void *) (INT_PTR) -1 /* IPort::get_UpdateSQL */ ,
    (void *) (INT_PTR) -1 /* IPort::get_PurgeSQL */ ,
    (void *) (INT_PTR) -1 /* IPort::get_SelectSQL */ ,
    (void *) (INT_PTR) -1 /* IPort::get_SelectByIDSQL */ ,
    (void *) (INT_PTR) -1 /* IPort::get_EnumerateSQL */
};


static const PRPC_STUB_FUNCTION IPort_table[] =
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
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2
};

CInterfaceStubVtbl _IPortStubVtbl =
{
    &IID_IPort,
    &IPort_ServerInfo,
    31,
    &IPort_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Object interface: IContrec, ver. 0.0,
   GUID={0x467BEDE3,0x088B,0x4D08,{0x98,0x5F,0xF5,0xEA,0x54,0x56,0xC2,0x21}} */

#pragma code_seg(".orpc")
static const unsigned short IContrec_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    0,
    36,
    72,
    108,
    144,
    180,
    882,
    918,
    954,
    990,
    1026,
    1062,
    432,
    1098,
    1134,
    1164,
    1200,
    1236,
    666,
    702,
    738
    };

static const MIDL_STUBLESS_PROXY_INFO IContrec_ProxyInfo =
    {
    &Object_StubDesc,
    ContrecOPCObjects__MIDL_ProcFormatString.Format,
    &IContrec_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO IContrec_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    ContrecOPCObjects__MIDL_ProcFormatString.Format,
    &IContrec_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(28) _IContrecProxyVtbl = 
{
    &IContrec_ProxyInfo,
    &IID_IContrec,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IDispatch::GetTypeInfoCount */ ,
    0 /* IDispatch::GetTypeInfo */ ,
    0 /* IDispatch::GetIDsOfNames */ ,
    0 /* IDispatch_Invoke_Proxy */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_Index */ ,
    (void *) (INT_PTR) -1 /* IContrec::put_Index */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_ID */ ,
    (void *) (INT_PTR) -1 /* IContrec::put_ID */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_Type */ ,
    (void *) (INT_PTR) -1 /* IContrec::put_Type */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_PortIndex */ ,
    (void *) (INT_PTR) -1 /* IContrec::put_PortIndex */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_PortID */ ,
    (void *) (INT_PTR) -1 /* IContrec::put_PortID */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_Address */ ,
    (void *) (INT_PTR) -1 /* IContrec::put_Address */ ,
    (void *) (INT_PTR) -1 /* IContrec::TypeID */ ,
    (void *) (INT_PTR) -1 /* IContrec::Load */ ,
    (void *) (INT_PTR) -1 /* IContrec::Reset */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_InsertSQL */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_UpdateSQL */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_PurgeSQL */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_SelectSQL */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_SelectByIDSQL */ ,
    (void *) (INT_PTR) -1 /* IContrec::get_EnumerateSQL */
};


static const PRPC_STUB_FUNCTION IContrec_table[] =
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
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2
};

CInterfaceStubVtbl _IContrecStubVtbl =
{
    &IID_IContrec,
    &IContrec_ServerInfo,
    28,
    &IContrec_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Object interface: IPortCollection, ver. 0.0,
   GUID={0x8A5434A0,0xF101,0x4070,{0x92,0x63,0x1D,0x31,0x16,0xE3,0x9E,0xBA}} */

#pragma code_seg(".orpc")
static const unsigned short IPortCollection_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    1272,
    1314,
    1350,
    1386,
    1422
    };

static const MIDL_STUBLESS_PROXY_INFO IPortCollection_ProxyInfo =
    {
    &Object_StubDesc,
    ContrecOPCObjects__MIDL_ProcFormatString.Format,
    &IPortCollection_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO IPortCollection_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    ContrecOPCObjects__MIDL_ProcFormatString.Format,
    &IPortCollection_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(12) _IPortCollectionProxyVtbl = 
{
    &IPortCollection_ProxyInfo,
    &IID_IPortCollection,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IDispatch::GetTypeInfoCount */ ,
    0 /* IDispatch::GetTypeInfo */ ,
    0 /* IDispatch::GetIDsOfNames */ ,
    0 /* IDispatch_Invoke_Proxy */ ,
    (void *) (INT_PTR) -1 /* IPortCollection::Item */ ,
    (void *) (INT_PTR) -1 /* IPortCollection::Add */ ,
    (void *) (INT_PTR) -1 /* IPortCollection::get_Count */ ,
    (void *) (INT_PTR) -1 /* IPortCollection::Remove */ ,
    (void *) (INT_PTR) -1 /* IPortCollection::get__NewEnum */
};


static const PRPC_STUB_FUNCTION IPortCollection_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2
};

CInterfaceStubVtbl _IPortCollectionStubVtbl =
{
    &IID_IPortCollection,
    &IPortCollection_ServerInfo,
    12,
    &IPortCollection_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};


/* Object interface: IContrecCollection, ver. 0.0,
   GUID={0xC09BAFF5,0xA840,0x4023,{0x8E,0x96,0x23,0xB4,0x4E,0x0F,0xA9,0x6B}} */

#pragma code_seg(".orpc")
static const unsigned short IContrecCollection_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    1458,
    1500,
    1350,
    1386,
    1536
    };

static const MIDL_STUBLESS_PROXY_INFO IContrecCollection_ProxyInfo =
    {
    &Object_StubDesc,
    ContrecOPCObjects__MIDL_ProcFormatString.Format,
    &IContrecCollection_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO IContrecCollection_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    ContrecOPCObjects__MIDL_ProcFormatString.Format,
    &IContrecCollection_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(12) _IContrecCollectionProxyVtbl = 
{
    &IContrecCollection_ProxyInfo,
    &IID_IContrecCollection,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* IDispatch::GetTypeInfoCount */ ,
    0 /* IDispatch::GetTypeInfo */ ,
    0 /* IDispatch::GetIDsOfNames */ ,
    0 /* IDispatch_Invoke_Proxy */ ,
    (void *) (INT_PTR) -1 /* IContrecCollection::Item */ ,
    (void *) (INT_PTR) -1 /* IContrecCollection::Add */ ,
    (void *) (INT_PTR) -1 /* IContrecCollection::get_Count */ ,
    (void *) (INT_PTR) -1 /* IContrecCollection::Remove */ ,
    (void *) (INT_PTR) -1 /* IContrecCollection::get__NewEnum */
};


static const PRPC_STUB_FUNCTION IContrecCollection_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2,
    NdrStubCall2
};

CInterfaceStubVtbl _IContrecCollectionStubVtbl =
{
    &IID_IContrecCollection,
    &IContrecCollection_ServerInfo,
    12,
    &IContrecCollection_table[-3],
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
    ContrecOPCObjects__MIDL_TypeFormatString.Format,
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

const CInterfaceProxyVtbl * const _ContrecOPCObjects_ProxyVtblList[] = 
{
    ( CInterfaceProxyVtbl *) &_IPortProxyVtbl,
    ( CInterfaceProxyVtbl *) &_IPortCollectionProxyVtbl,
    ( CInterfaceProxyVtbl *) &_IContrecProxyVtbl,
    ( CInterfaceProxyVtbl *) &_IContrecCollectionProxyVtbl,
    0
};

const CInterfaceStubVtbl * const _ContrecOPCObjects_StubVtblList[] = 
{
    ( CInterfaceStubVtbl *) &_IPortStubVtbl,
    ( CInterfaceStubVtbl *) &_IPortCollectionStubVtbl,
    ( CInterfaceStubVtbl *) &_IContrecStubVtbl,
    ( CInterfaceStubVtbl *) &_IContrecCollectionStubVtbl,
    0
};

PCInterfaceName const _ContrecOPCObjects_InterfaceNamesList[] = 
{
    "IPort",
    "IPortCollection",
    "IContrec",
    "IContrecCollection",
    0
};

const IID *  const _ContrecOPCObjects_BaseIIDList[] = 
{
    &IID_IDispatch,
    &IID_IDispatch,
    &IID_IDispatch,
    &IID_IDispatch,
    0
};


#define _ContrecOPCObjects_CHECK_IID(n)	IID_GENERIC_CHECK_IID( _ContrecOPCObjects, pIID, n)

int __stdcall _ContrecOPCObjects_IID_Lookup( const IID * pIID, int * pIndex )
{
    IID_BS_LOOKUP_SETUP

    IID_BS_LOOKUP_INITIAL_TEST( _ContrecOPCObjects, 4, 2 )
    IID_BS_LOOKUP_NEXT_TEST( _ContrecOPCObjects, 1 )
    IID_BS_LOOKUP_RETURN_RESULT( _ContrecOPCObjects, 4, *pIndex )
    
}

const ExtendedProxyFileInfo ContrecOPCObjects_ProxyFileInfo = 
{
    (PCInterfaceProxyVtblList *) & _ContrecOPCObjects_ProxyVtblList,
    (PCInterfaceStubVtblList *) & _ContrecOPCObjects_StubVtblList,
    (const PCInterfaceName * ) & _ContrecOPCObjects_InterfaceNamesList,
    (const IID ** ) & _ContrecOPCObjects_BaseIIDList,
    & _ContrecOPCObjects_IID_Lookup, 
    4,
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

