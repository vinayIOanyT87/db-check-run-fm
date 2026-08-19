

/* this ALWAYS GENERATED file contains the RPC client stubs */


 /* File created by MIDL compiler version 7.00.0500 */
/* at Fri Oct 28 11:08:32 2016
 */
/* Compiler settings for link.idl, link.acf:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext, oldnames, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#if !defined(_M_IA64) && !defined(_M_AMD64)


#pragma warning( disable: 4049 )  /* more than 64k source lines */
#if _MSC_VER >= 1200
#pragma warning(push)
#endif

#pragma warning( disable: 4211 )  /* redefine extern to static */
#pragma warning( disable: 4232 )  /* dllimport identity*/
#pragma warning( disable: 4024 )  /* array to pointer mapping*/
#pragma warning( disable: 4100 ) /* unreferenced arguments in x86 call */

#pragma optimize("", off ) 

#include <string.h>

#include "link.h"

#define TYPE_FORMAT_STRING_SIZE   963                               
#define PROC_FORMAT_STRING_SIZE   2173                              
#define EXPR_FORMAT_STRING_SIZE   1                                 
#define TRANSMIT_AS_TABLE_SIZE    0            
#define WIRE_MARSHAL_TABLE_SIZE   0            

typedef struct _link_MIDL_TYPE_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ TYPE_FORMAT_STRING_SIZE ];
    } link_MIDL_TYPE_FORMAT_STRING;

typedef struct _link_MIDL_PROC_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ PROC_FORMAT_STRING_SIZE ];
    } link_MIDL_PROC_FORMAT_STRING;

typedef struct _link_MIDL_EXPR_FORMAT_STRING
    {
    long          Pad;
    unsigned char  Format[ EXPR_FORMAT_STRING_SIZE ];
    } link_MIDL_EXPR_FORMAT_STRING;


static RPC_SYNTAX_IDENTIFIER  _RpcTransferSyntax = 
{{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}};


extern const link_MIDL_TYPE_FORMAT_STRING link__MIDL_TypeFormatString;
extern const link_MIDL_PROC_FORMAT_STRING link__MIDL_ProcFormatString;
extern const link_MIDL_EXPR_FORMAT_STRING link__MIDL_ExprFormatString;

#define GENERIC_BINDING_TABLE_SIZE   0            


/* Standard interface: mvmntlink, ver. 1.0,
   GUID={0xF5E958F0,0xA00C,0x101B,{0xA9,0x58,0x08,0x00,0x2B,0x31,0x0E,0x80}} */


extern const MIDL_SERVER_INFO mvmntlink_ServerInfo;


extern RPC_DISPATCH_TABLE mvmntlink_DispatchTable;

static const RPC_CLIENT_INTERFACE mvmntlink___RpcClientInterface =
    {
    sizeof(RPC_CLIENT_INTERFACE),
    {{0xF5E958F0,0xA00C,0x101B,{0xA9,0x58,0x08,0x00,0x2B,0x31,0x0E,0x80}},{1,0}},
    {{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}},
    &mvmntlink_DispatchTable,
    0,
    0,
    0,
    &mvmntlink_ServerInfo,
    0x04000000
    };
RPC_IF_HANDLE mvmntlink_ClientIfHandle = (RPC_IF_HANDLE)& mvmntlink___RpcClientInterface;

extern const MIDL_STUB_DESC mvmntlink_StubDesc;

static RPC_BINDING_HANDLE mvmntlink__MIDL_AutoBindHandle;


unsigned long MvmntEditStart( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [out] */ MVMNT_DATA_CONTEXT_HANDLE *phContext,
    /* [out] */ unsigned char *pbCheckProducts,
    /* [out] */ unsigned char *pbClosingHour,
    /* [out] */ unsigned char *pbClosingMinute,
    /* [out] */ unsigned short *pwHistoryLogPeriod,
    /* [size_is][out] */ wchar_t pUser[  ])
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[0],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntEditDone( 
    /* [out][in] */ MVMNT_DATA_CONTEXT_HANDLE *phContext,
    /* [in] */ unsigned char bCheckProducts,
    /* [in] */ unsigned char bClosingHour,
    /* [in] */ unsigned char bClosingMinute,
    /* [in] */ unsigned short wHistoryLogPeriod)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[76],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetGroups( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumGroups,
    /* [size_is][size_is][out] */ PMOVEMENTGROUP *ppMovementGroup)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[142],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntAddGroup( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PMOVEMENTGROUP pMovementGroup)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[188],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntUpdateGroup( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PMOVEMENTGROUP pOldMovementGroup,
    /* [in] */ PMOVEMENTGROUP pNewMovementGroup)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[228],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntDeleteGroup( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PMOVEMENTGROUP pMovementGroup)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[274],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetEndNodeArray( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumPnt,
    /* [size_is][size_is][out] */ ENDNODE **pEndNode)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[314],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetNodeData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned short wNodeId,
    /* [out] */ PENDNODE pNodeData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[360],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntAddEndNode( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ PENDNODE pNewNode,
    /* [out] */ unsigned short *pNewId)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[406],
                  ( unsigned char * )&hContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntDeleteEndNode( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ unsigned short wNodeId)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[460],
                  ( unsigned char * )&hContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetNodeName( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned short wNodeId,
    /* [size_is][out] */ wchar_t pName[  ],
    /* [out] */ unsigned short *pwNodeType)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[508],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntUpdateEndNode( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ PENDNODE pNewPoint)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[560],
                  ( unsigned char * )&hContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetNodeID( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pNodeName,
    /* [out] */ unsigned short *pNodeId)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[608],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntAddMovement( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ PMOVEMENT pNewMove)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[654],
                  ( unsigned char * )&hContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntDeleteMovement( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ PMOVEMENT pDelMove)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[702],
                  ( unsigned char * )&hContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntReplaceMove( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ PMOVEMENT pOldMove,
    /* [in] */ PMOVEMENT pNewMove)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[750],
                  ( unsigned char * )&hContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetMoveSources( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumSrc,
    /* [size_is][size_is][out] */ PENDNODE *ppEndNodeSrc,
    /* [size_is][size_is][out] */ unsigned short **ppwDestCount)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[804],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetMoveDestinations( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumDest,
    /* [size_is][size_is][out] */ PENDNODE *ppEndNodeDest,
    /* [size_is][size_is][out] */ unsigned short **ppwSrcCount)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[856],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetMoveDestinationsForSource( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned short wSrc,
    /* [out] */ unsigned long *pdwNumDest,
    /* [size_is][size_is][out] */ PENDNODE *ppEndNodeDest,
    /* [size_is][size_is][out] */ unsigned short **ppwInstCount)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[908],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetMoveSourcesForDestination( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned short wDest,
    /* [out] */ unsigned long *pdwNumSrc,
    /* [size_is][size_is][out] */ PENDNODE *ppEndNodeSrc,
    /* [size_is][size_is][out] */ unsigned short **ppwInstCount)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[966],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntAddMoveInst( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUser,
    /* [in] */ unsigned char bType,
    /* [out] */ unsigned long *pdwMoveInstID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1024],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntDeleteMoveInst( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1076],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetMoveInst( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [out] */ PMOVEINSTANCEDATA *ppMoveInstanceData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1116],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntSetMoveInst( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pSystem,
    /* [string][in] */ wchar_t *pUser,
    /* [in] */ unsigned long dwMoveInstID,
    /* [out][in] */ PMOVEINSTANCEDATA *ppMoveInstanceData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1162],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetMovementTypeData( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumTypes,
    /* [size_is][size_is][out] */ PMOVEMENTTYPEDATA *ppMovementTypeData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1220],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntMoveCommand( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pSystem,
    /* [string][in] */ wchar_t *pUser,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wMoveNodeID,
    /* [in] */ unsigned short wCommand,
    /* [out] */ long *lMoveTimeStamp)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1266],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetStartData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wNodeInstID,
    /* [out] */ PSTARTDATAGET pStartDataGet)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1636],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntPutStartData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wNodeInstID,
    /* [in] */ PSTARTDATAPUT pStartDataPut)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1688],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetCloseOutTime( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned short *wHour,
    /* [out] */ unsigned short *wMinute)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1740],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetServerDeliveryTicketNames( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumReports,
    /* [size_is][size_is][out] */ DELIVERYTICKETNAME **pReportStrings)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1786],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetServerPrinterNames( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumPrinters,
    /* [size_is][size_is][out] */ PRINTERDATA **pPrinterNames)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1832],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetStartDataBlockStatus( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wNodeInstID,
    /* [out] */ PMVMNTBLOCKSTATUS pStartDataBlockStatus)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1878],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetHandGaugeData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wNodeInstID,
    /* [in] */ unsigned char bStartData,
    /* [out] */ PHANDGAUGEDATA pHandGaugeData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1930],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntPutHandGaugeData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wNodeInstID,
    /* [in] */ unsigned char bStartData,
    /* [in] */ PHANDGAUGEDATA pHandGaugeData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[1988],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntSetClearDescrencyInPercent( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bEnable)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[2046],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntGetDescrencyInPercent( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned char *bEnable)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[2086],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long MvmntHistoryDataModified( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PMOVEMENTNAME pMovementName,
    /* [in] */ unsigned long dwMoveInstID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&mvmntlink_StubDesc,
                  (PFORMAT_STRING) &link__MIDL_ProcFormatString.Format[2126],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


#if !defined(__RPC_WIN32__)
#error  Invalid build platform for this stub.
#endif

#if !(TARGET_IS_NT50_OR_LATER)
#error You need a Windows 2000 or later to run this stub because it uses these features:
#error   /robust command line switch.
#error However, your C/C++ compilation flags indicate you intend to run this app on earlier systems.
#error This app will fail with the RPC_X_WRONG_STUB_VERSION error.
#endif


static const link_MIDL_PROC_FORMAT_STRING link__MIDL_ProcFormatString =
    {
        0,
        {

	/* Procedure MvmntEditStart */

			0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/*  2 */	NdrFcLong( 0x0 ),	/* 0 */
/*  6 */	NdrFcShort( 0x0 ),	/* 0 */
/*  8 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 10 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 12 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 14 */	NdrFcShort( 0x0 ),	/* 0 */
/* 16 */	NdrFcShort( 0xa5 ),	/* 165 */
/* 18 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x8,		/* 8 */
/* 20 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 22 */	NdrFcShort( 0x1 ),	/* 1 */
/* 24 */	NdrFcShort( 0x0 ),	/* 0 */
/* 26 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 28 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 30 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 32 */	NdrFcShort( 0x4 ),	/* Type Offset=4 */

	/* Parameter pUserName */

/* 34 */	NdrFcShort( 0x110 ),	/* Flags:  out, simple ref, */
/* 36 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 38 */	NdrFcShort( 0xa ),	/* Type Offset=10 */

	/* Parameter phContext */

/* 40 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 42 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 44 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pbCheckProducts */

/* 46 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 48 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 50 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pbClosingHour */

/* 52 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 54 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 56 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pbClosingMinute */

/* 58 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 60 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 62 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pwHistoryLogPeriod */

/* 64 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 66 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 68 */	NdrFcShort( 0x16 ),	/* Type Offset=22 */

	/* Parameter pUser */

/* 70 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 72 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 74 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntEditDone */


	/* Return value */

/* 76 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 78 */	NdrFcLong( 0x0 ),	/* 0 */
/* 82 */	NdrFcShort( 0x1 ),	/* 1 */
/* 84 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 86 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 88 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 90 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 92 */	NdrFcShort( 0x4d ),	/* 77 */
/* 94 */	NdrFcShort( 0x40 ),	/* 64 */
/* 96 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x6,		/* 6 */
/* 98 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 100 */	NdrFcShort( 0x0 ),	/* 0 */
/* 102 */	NdrFcShort( 0x0 ),	/* 0 */
/* 104 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 106 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 108 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 110 */	NdrFcShort( 0x26 ),	/* Type Offset=38 */

	/* Parameter bCheckProducts */

/* 112 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 114 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 116 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bClosingHour */

/* 118 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 120 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 122 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bClosingMinute */

/* 124 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 126 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 128 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter wHistoryLogPeriod */

/* 130 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 132 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 134 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Return value */

/* 136 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 138 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 140 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetGroups */

/* 142 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 144 */	NdrFcLong( 0x0 ),	/* 0 */
/* 148 */	NdrFcShort( 0x2 ),	/* 2 */
/* 150 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 152 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 154 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 156 */	NdrFcShort( 0x0 ),	/* 0 */
/* 158 */	NdrFcShort( 0x24 ),	/* 36 */
/* 160 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 162 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 164 */	NdrFcShort( 0x1 ),	/* 1 */
/* 166 */	NdrFcShort( 0x0 ),	/* 0 */
/* 168 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 170 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 172 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 174 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwNumGroups */

/* 176 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 178 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 180 */	NdrFcShort( 0x2e ),	/* Type Offset=46 */

	/* Parameter ppMovementGroup */

/* 182 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 184 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 186 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntAddGroup */


	/* Return value */

/* 188 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 190 */	NdrFcLong( 0x0 ),	/* 0 */
/* 194 */	NdrFcShort( 0x3 ),	/* 3 */
/* 196 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 198 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 200 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 202 */	NdrFcShort( 0x5e ),	/* 94 */
/* 204 */	NdrFcShort( 0x8 ),	/* 8 */
/* 206 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 208 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 210 */	NdrFcShort( 0x0 ),	/* 0 */
/* 212 */	NdrFcShort( 0x0 ),	/* 0 */
/* 214 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 216 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 218 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 220 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Parameter pMovementGroup */

/* 222 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 224 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 226 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntUpdateGroup */


	/* Return value */

/* 228 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 230 */	NdrFcLong( 0x0 ),	/* 0 */
/* 234 */	NdrFcShort( 0x4 ),	/* 4 */
/* 236 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 238 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 240 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 242 */	NdrFcShort( 0xbc ),	/* 188 */
/* 244 */	NdrFcShort( 0x8 ),	/* 8 */
/* 246 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 248 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 250 */	NdrFcShort( 0x0 ),	/* 0 */
/* 252 */	NdrFcShort( 0x0 ),	/* 0 */
/* 254 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 256 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 258 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 260 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Parameter pOldMovementGroup */

/* 262 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 264 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 266 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Parameter pNewMovementGroup */

/* 268 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 270 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 272 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntDeleteGroup */


	/* Return value */

/* 274 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 276 */	NdrFcLong( 0x0 ),	/* 0 */
/* 280 */	NdrFcShort( 0x5 ),	/* 5 */
/* 282 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 284 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 286 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 288 */	NdrFcShort( 0x5e ),	/* 94 */
/* 290 */	NdrFcShort( 0x8 ),	/* 8 */
/* 292 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 294 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 296 */	NdrFcShort( 0x0 ),	/* 0 */
/* 298 */	NdrFcShort( 0x0 ),	/* 0 */
/* 300 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 302 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 304 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 306 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Parameter pMovementGroup */

/* 308 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 310 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 312 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetEndNodeArray */


	/* Return value */

/* 314 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 316 */	NdrFcLong( 0x0 ),	/* 0 */
/* 320 */	NdrFcShort( 0x6 ),	/* 6 */
/* 322 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 324 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 326 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 328 */	NdrFcShort( 0x0 ),	/* 0 */
/* 330 */	NdrFcShort( 0x24 ),	/* 36 */
/* 332 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 334 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 336 */	NdrFcShort( 0x1 ),	/* 1 */
/* 338 */	NdrFcShort( 0x0 ),	/* 0 */
/* 340 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 342 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 344 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 346 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwNumPnt */

/* 348 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 350 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 352 */	NdrFcShort( 0x5a ),	/* Type Offset=90 */

	/* Parameter pEndNode */

/* 354 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 356 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 358 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetNodeData */


	/* Return value */

/* 360 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 362 */	NdrFcLong( 0x0 ),	/* 0 */
/* 366 */	NdrFcShort( 0x7 ),	/* 7 */
/* 368 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 370 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 372 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 374 */	NdrFcShort( 0x6 ),	/* 6 */
/* 376 */	NdrFcShort( 0x8 ),	/* 8 */
/* 378 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 380 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 382 */	NdrFcShort( 0x0 ),	/* 0 */
/* 384 */	NdrFcShort( 0x0 ),	/* 0 */
/* 386 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 388 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 390 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 392 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wNodeId */

/* 394 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 396 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 398 */	NdrFcShort( 0x6e ),	/* Type Offset=110 */

	/* Parameter pNodeData */

/* 400 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 402 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 404 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntAddEndNode */


	/* Return value */

/* 406 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 408 */	NdrFcLong( 0x0 ),	/* 0 */
/* 412 */	NdrFcShort( 0x8 ),	/* 8 */
/* 414 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 416 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 418 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 420 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 422 */	NdrFcShort( 0x24 ),	/* 36 */
/* 424 */	NdrFcShort( 0x22 ),	/* 34 */
/* 426 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x4,		/* 4 */
/* 428 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 430 */	NdrFcShort( 0x0 ),	/* 0 */
/* 432 */	NdrFcShort( 0x0 ),	/* 0 */
/* 434 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hContext */

/* 436 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 438 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 440 */	NdrFcShort( 0xa0 ),	/* Type Offset=160 */

	/* Parameter pNewNode */

/* 442 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 444 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 446 */	NdrFcShort( 0x6e ),	/* Type Offset=110 */

	/* Parameter pNewId */

/* 448 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 450 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 452 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Return value */

/* 454 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 456 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 458 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntDeleteEndNode */

/* 460 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 462 */	NdrFcLong( 0x0 ),	/* 0 */
/* 466 */	NdrFcShort( 0x9 ),	/* 9 */
/* 468 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 470 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 472 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 474 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 476 */	NdrFcShort( 0x2a ),	/* 42 */
/* 478 */	NdrFcShort( 0x8 ),	/* 8 */
/* 480 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 482 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 484 */	NdrFcShort( 0x0 ),	/* 0 */
/* 486 */	NdrFcShort( 0x0 ),	/* 0 */
/* 488 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hContext */

/* 490 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 492 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 494 */	NdrFcShort( 0xa0 ),	/* Type Offset=160 */

	/* Parameter wNodeId */

/* 496 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 498 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 500 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Return value */

/* 502 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 504 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 506 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetNodeName */

/* 508 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 510 */	NdrFcLong( 0x0 ),	/* 0 */
/* 514 */	NdrFcShort( 0xa ),	/* 10 */
/* 516 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 518 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 520 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 522 */	NdrFcShort( 0x6 ),	/* 6 */
/* 524 */	NdrFcShort( 0x22 ),	/* 34 */
/* 526 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x4,		/* 4 */
/* 528 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 530 */	NdrFcShort( 0x1 ),	/* 1 */
/* 532 */	NdrFcShort( 0x0 ),	/* 0 */
/* 534 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 536 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 538 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 540 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wNodeId */

/* 542 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 544 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 546 */	NdrFcShort( 0x16 ),	/* Type Offset=22 */

	/* Parameter pName */

/* 548 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 550 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 552 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pwNodeType */

/* 554 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 556 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 558 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntUpdateEndNode */


	/* Return value */

/* 560 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 562 */	NdrFcLong( 0x0 ),	/* 0 */
/* 566 */	NdrFcShort( 0xb ),	/* 11 */
/* 568 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 570 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 572 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 574 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 576 */	NdrFcShort( 0x24 ),	/* 36 */
/* 578 */	NdrFcShort( 0x8 ),	/* 8 */
/* 580 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 582 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 584 */	NdrFcShort( 0x0 ),	/* 0 */
/* 586 */	NdrFcShort( 0x0 ),	/* 0 */
/* 588 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hContext */

/* 590 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 592 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 594 */	NdrFcShort( 0xa0 ),	/* Type Offset=160 */

	/* Parameter pNewPoint */

/* 596 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 598 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 600 */	NdrFcShort( 0x6e ),	/* Type Offset=110 */

	/* Return value */

/* 602 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 604 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 606 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetNodeID */

/* 608 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 610 */	NdrFcLong( 0x0 ),	/* 0 */
/* 614 */	NdrFcShort( 0xc ),	/* 12 */
/* 616 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 618 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 620 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 622 */	NdrFcShort( 0x0 ),	/* 0 */
/* 624 */	NdrFcShort( 0x22 ),	/* 34 */
/* 626 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 628 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 630 */	NdrFcShort( 0x0 ),	/* 0 */
/* 632 */	NdrFcShort( 0x0 ),	/* 0 */
/* 634 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 636 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 638 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 640 */	NdrFcShort( 0x4 ),	/* Type Offset=4 */

	/* Parameter pNodeName */

/* 642 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 644 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 646 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pNodeId */

/* 648 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 650 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 652 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntAddMovement */


	/* Return value */

/* 654 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 656 */	NdrFcLong( 0x0 ),	/* 0 */
/* 660 */	NdrFcShort( 0xd ),	/* 13 */
/* 662 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 664 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 666 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 668 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 670 */	NdrFcShort( 0x24 ),	/* 36 */
/* 672 */	NdrFcShort( 0x8 ),	/* 8 */
/* 674 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 676 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 678 */	NdrFcShort( 0x0 ),	/* 0 */
/* 680 */	NdrFcShort( 0x0 ),	/* 0 */
/* 682 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hContext */

/* 684 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 686 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 688 */	NdrFcShort( 0xa0 ),	/* Type Offset=160 */

	/* Parameter pNewMove */

/* 690 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 692 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 694 */	NdrFcShort( 0xa8 ),	/* Type Offset=168 */

	/* Return value */

/* 696 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 698 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 700 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntDeleteMovement */

/* 702 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 704 */	NdrFcLong( 0x0 ),	/* 0 */
/* 708 */	NdrFcShort( 0xe ),	/* 14 */
/* 710 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 712 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 714 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 716 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 718 */	NdrFcShort( 0x24 ),	/* 36 */
/* 720 */	NdrFcShort( 0x8 ),	/* 8 */
/* 722 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 724 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 726 */	NdrFcShort( 0x0 ),	/* 0 */
/* 728 */	NdrFcShort( 0x0 ),	/* 0 */
/* 730 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hContext */

/* 732 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 734 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 736 */	NdrFcShort( 0xa0 ),	/* Type Offset=160 */

	/* Parameter pDelMove */

/* 738 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 740 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 742 */	NdrFcShort( 0xa8 ),	/* Type Offset=168 */

	/* Return value */

/* 744 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 746 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 748 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntReplaceMove */

/* 750 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 752 */	NdrFcLong( 0x0 ),	/* 0 */
/* 756 */	NdrFcShort( 0xf ),	/* 15 */
/* 758 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 760 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 762 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 764 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 766 */	NdrFcShort( 0x24 ),	/* 36 */
/* 768 */	NdrFcShort( 0x8 ),	/* 8 */
/* 770 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x4,		/* 4 */
/* 772 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 774 */	NdrFcShort( 0x0 ),	/* 0 */
/* 776 */	NdrFcShort( 0x0 ),	/* 0 */
/* 778 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hContext */

/* 780 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 782 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 784 */	NdrFcShort( 0xa0 ),	/* Type Offset=160 */

	/* Parameter pOldMove */

/* 786 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 788 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 790 */	NdrFcShort( 0xa8 ),	/* Type Offset=168 */

	/* Parameter pNewMove */

/* 792 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 794 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 796 */	NdrFcShort( 0xa8 ),	/* Type Offset=168 */

	/* Return value */

/* 798 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 800 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 802 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetMoveSources */

/* 804 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 806 */	NdrFcLong( 0x0 ),	/* 0 */
/* 810 */	NdrFcShort( 0x10 ),	/* 16 */
/* 812 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 814 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 816 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 818 */	NdrFcShort( 0x0 ),	/* 0 */
/* 820 */	NdrFcShort( 0x24 ),	/* 36 */
/* 822 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x4,		/* 4 */
/* 824 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 826 */	NdrFcShort( 0x2 ),	/* 2 */
/* 828 */	NdrFcShort( 0x0 ),	/* 0 */
/* 830 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 832 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 834 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 836 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwNumSrc */

/* 838 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 840 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 842 */	NdrFcShort( 0x5a ),	/* Type Offset=90 */

	/* Parameter ppEndNodeSrc */

/* 844 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 846 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 848 */	NdrFcShort( 0xc4 ),	/* Type Offset=196 */

	/* Parameter ppwDestCount */

/* 850 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 852 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 854 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetMoveDestinations */


	/* Return value */

/* 856 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 858 */	NdrFcLong( 0x0 ),	/* 0 */
/* 862 */	NdrFcShort( 0x11 ),	/* 17 */
/* 864 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 866 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 868 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 870 */	NdrFcShort( 0x0 ),	/* 0 */
/* 872 */	NdrFcShort( 0x24 ),	/* 36 */
/* 874 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x4,		/* 4 */
/* 876 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 878 */	NdrFcShort( 0x2 ),	/* 2 */
/* 880 */	NdrFcShort( 0x0 ),	/* 0 */
/* 882 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 884 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 886 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 888 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwNumDest */

/* 890 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 892 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 894 */	NdrFcShort( 0x5a ),	/* Type Offset=90 */

	/* Parameter ppEndNodeDest */

/* 896 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 898 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 900 */	NdrFcShort( 0xc4 ),	/* Type Offset=196 */

	/* Parameter ppwSrcCount */

/* 902 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 904 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 906 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetMoveDestinationsForSource */


	/* Return value */

/* 908 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 910 */	NdrFcLong( 0x0 ),	/* 0 */
/* 914 */	NdrFcShort( 0x12 ),	/* 18 */
/* 916 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 918 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 920 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 922 */	NdrFcShort( 0x6 ),	/* 6 */
/* 924 */	NdrFcShort( 0x24 ),	/* 36 */
/* 926 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x5,		/* 5 */
/* 928 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 930 */	NdrFcShort( 0x2 ),	/* 2 */
/* 932 */	NdrFcShort( 0x0 ),	/* 0 */
/* 934 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 936 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 938 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 940 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wSrc */

/* 942 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 944 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 946 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwNumDest */

/* 948 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 950 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 952 */	NdrFcShort( 0xd8 ),	/* Type Offset=216 */

	/* Parameter ppEndNodeDest */

/* 954 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 956 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 958 */	NdrFcShort( 0xf6 ),	/* Type Offset=246 */

	/* Parameter ppwInstCount */

/* 960 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 962 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 964 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetMoveSourcesForDestination */


	/* Return value */

/* 966 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 968 */	NdrFcLong( 0x0 ),	/* 0 */
/* 972 */	NdrFcShort( 0x13 ),	/* 19 */
/* 974 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 976 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 978 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 980 */	NdrFcShort( 0x6 ),	/* 6 */
/* 982 */	NdrFcShort( 0x24 ),	/* 36 */
/* 984 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x5,		/* 5 */
/* 986 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 988 */	NdrFcShort( 0x2 ),	/* 2 */
/* 990 */	NdrFcShort( 0x0 ),	/* 0 */
/* 992 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 994 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 996 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 998 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wDest */

/* 1000 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1002 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1004 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwNumSrc */

/* 1006 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1008 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1010 */	NdrFcShort( 0xd8 ),	/* Type Offset=216 */

	/* Parameter ppEndNodeSrc */

/* 1012 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1014 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1016 */	NdrFcShort( 0xf6 ),	/* Type Offset=246 */

	/* Parameter ppwInstCount */

/* 1018 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1020 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1022 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntAddMoveInst */


	/* Return value */

/* 1024 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1026 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1030 */	NdrFcShort( 0x14 ),	/* 20 */
/* 1032 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1034 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1036 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1038 */	NdrFcShort( 0x5 ),	/* 5 */
/* 1040 */	NdrFcShort( 0x24 ),	/* 36 */
/* 1042 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x4,		/* 4 */
/* 1044 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1046 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1048 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1050 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1052 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1054 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1056 */	NdrFcShort( 0x4 ),	/* Type Offset=4 */

	/* Parameter pUser */

/* 1058 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1060 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1062 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bType */

/* 1064 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1066 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1068 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwMoveInstID */

/* 1070 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1072 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1074 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntDeleteMoveInst */


	/* Return value */

/* 1076 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1078 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1082 */	NdrFcShort( 0x15 ),	/* 21 */
/* 1084 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1086 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1088 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1090 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1092 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1094 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 1096 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1098 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1100 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1102 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1104 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1106 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1108 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwMoveInstID */

/* 1110 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1112 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1114 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetMoveInst */


	/* Return value */

/* 1116 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1118 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1122 */	NdrFcShort( 0x16 ),	/* 22 */
/* 1124 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1126 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1128 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1130 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1132 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1134 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 1136 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 1138 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1140 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1142 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1144 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1146 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1148 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwMoveInstID */

/* 1150 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1152 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1154 */	NdrFcShort( 0x10a ),	/* Type Offset=266 */

	/* Parameter ppMoveInstanceData */

/* 1156 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1158 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1160 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntSetMoveInst */


	/* Return value */

/* 1162 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1164 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1168 */	NdrFcShort( 0x17 ),	/* 23 */
/* 1170 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1172 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1174 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1176 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1178 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1180 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 1182 */	0x8,		/* 8 */
			0x7,		/* Ext Flags:  new corr desc, clt corr check, srv corr check, */
/* 1184 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1186 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1188 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1190 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1192 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1194 */	NdrFcShort( 0x4 ),	/* Type Offset=4 */

	/* Parameter pSystem */

/* 1196 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1198 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1200 */	NdrFcShort( 0x4 ),	/* Type Offset=4 */

	/* Parameter pUser */

/* 1202 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1204 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1206 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwMoveInstID */

/* 1208 */	NdrFcShort( 0x201b ),	/* Flags:  must size, must free, in, out, srv alloc size=8 */
/* 1210 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1212 */	NdrFcShort( 0x10a ),	/* Type Offset=266 */

	/* Parameter ppMoveInstanceData */

/* 1214 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1216 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1218 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetMovementTypeData */


	/* Return value */

/* 1220 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1222 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1226 */	NdrFcShort( 0x18 ),	/* 24 */
/* 1228 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1230 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1232 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1234 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1236 */	NdrFcShort( 0x24 ),	/* 36 */
/* 1238 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 1240 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 1242 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1244 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1246 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1248 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1250 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1252 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwNumTypes */

/* 1254 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1256 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1258 */	NdrFcShort( 0x1e8 ),	/* Type Offset=488 */

	/* Parameter ppMovementTypeData */

/* 1260 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1262 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1264 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntMoveCommand */


	/* Return value */

/* 1266 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1268 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1272 */	NdrFcShort( 0x19 ),	/* 25 */
/* 1274 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 1276 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1278 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1280 */	NdrFcShort( 0x14 ),	/* 20 */
/* 1282 */	NdrFcShort( 0x24 ),	/* 36 */
/* 1284 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 1286 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1288 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1290 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1292 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1294 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1296 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1298 */	NdrFcShort( 0x4 ),	/* Type Offset=4 */

	/* Parameter pSystem */

/* 1300 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1302 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1304 */	NdrFcShort( 0x4 ),	/* Type Offset=4 */

	/* Parameter pUser */

/* 1306 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1308 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1310 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwMoveInstID */

/* 1312 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1314 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1316 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wMoveNodeID */

/* 1318 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1320 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1322 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wCommand */

/* 1324 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1326 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1328 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lMoveTimeStamp */

/* 1330 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1332 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 1334 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntCombinationWarning */


	/* Return value */

/* 1336 */	0x34,		/* FC_CALLBACK_HANDLE */
			0x48,		/* Old Flags:  */
/* 1338 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1342 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1344 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1346 */	NdrFcShort( 0x64 ),	/* 100 */
/* 1348 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1350 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x4,		/* 4 */
/* 1352 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 1354 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1356 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1358 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pMovementName */

/* 1360 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 1362 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1364 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Parameter wNameCount */

/* 1366 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1368 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1370 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pCombineName */

/* 1372 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1374 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1376 */	NdrFcShort( 0x238 ),	/* Type Offset=568 */

	/* Return value */

/* 1378 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1380 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1382 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntContaminateWarning */

/* 1384 */	0x34,		/* FC_CALLBACK_HANDLE */
			0x48,		/* Old Flags:  */
/* 1386 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1390 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1392 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1394 */	NdrFcShort( 0xe8 ),	/* 232 */
/* 1396 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1398 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 1400 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 1402 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1404 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1406 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pMovementName */

/* 1408 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 1410 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1412 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Parameter pNodeProductData */

/* 1414 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 1416 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1418 */	NdrFcShort( 0x24c ),	/* Type Offset=588 */

	/* Parameter wNodeCount */

/* 1420 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1422 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1424 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pContaminateData */

/* 1426 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1428 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1430 */	NdrFcShort( 0x25c ),	/* Type Offset=604 */

	/* Return value */

/* 1432 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1434 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1436 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntAddUnitNodeWarning */

/* 1438 */	0x34,		/* FC_CALLBACK_HANDLE */
			0x48,		/* Old Flags:  */
/* 1440 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1444 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1446 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1448 */	NdrFcShort( 0x5e ),	/* 94 */
/* 1450 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1452 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 1454 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1456 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1458 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1460 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pMovementName */

/* 1462 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 1464 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1466 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Parameter szNodeName */

/* 1468 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1470 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1472 */	NdrFcShort( 0x4 ),	/* Type Offset=4 */

	/* Return value */

/* 1474 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1476 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1478 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntLockoutWarning */

/* 1480 */	0x34,		/* FC_CALLBACK_HANDLE */
			0x48,		/* Old Flags:  */
/* 1482 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1486 */	NdrFcShort( 0x3 ),	/* 3 */
/* 1488 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1490 */	NdrFcShort( 0x5e ),	/* 94 */
/* 1492 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1494 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 1496 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1498 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1500 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1502 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pMovementName */

/* 1504 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 1506 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1508 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Return value */

/* 1510 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1512 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1514 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntSetupWarning */

/* 1516 */	0x34,		/* FC_CALLBACK_HANDLE */
			0x48,		/* Old Flags:  */
/* 1518 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1522 */	NdrFcShort( 0x4 ),	/* 4 */
/* 1524 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1526 */	NdrFcShort( 0x5e ),	/* 94 */
/* 1528 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1530 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 1532 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1534 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1536 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1538 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pMovementName */

/* 1540 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 1542 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1544 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Return value */

/* 1546 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1548 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1550 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntChangeTransferWarning */

/* 1552 */	0x34,		/* FC_CALLBACK_HANDLE */
			0x48,		/* Old Flags:  */
/* 1554 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1558 */	NdrFcShort( 0x5 ),	/* 5 */
/* 1560 */	NdrFcShort( 0x30 ),	/* x86 Stack size/offset = 48 */
/* 1562 */	NdrFcShort( 0x42 ),	/* 66 */
/* 1564 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1566 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0xa,		/* 10 */
/* 1568 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1570 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1572 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1574 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter szNodeName */

/* 1576 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1578 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1580 */	NdrFcShort( 0x4 ),	/* Type Offset=4 */

	/* Parameter wNewTankMode */

/* 1582 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1584 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1586 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wOldTankMode */

/* 1588 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1590 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1592 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wNewXfrMode */

/* 1594 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1596 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1598 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wOldXfrMode */

/* 1600 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1602 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1604 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter dNewXfrSetpoint */

/* 1606 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1608 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1610 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter bNewXfrSetpointStyle */

/* 1612 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1614 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 1616 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter dOldXfrSetpoint */

/* 1618 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1620 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 1622 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter bOldXfrSetpointStyle */

/* 1624 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1626 */	NdrFcShort( 0x28 ),	/* x86 Stack size/offset = 40 */
/* 1628 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Return value */

/* 1630 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1632 */	NdrFcShort( 0x2c ),	/* x86 Stack size/offset = 44 */
/* 1634 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetStartData */

/* 1636 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1638 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1642 */	NdrFcShort( 0x1a ),	/* 26 */
/* 1644 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1646 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1648 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1650 */	NdrFcShort( 0xe ),	/* 14 */
/* 1652 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1654 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x4,		/* 4 */
/* 1656 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1658 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1660 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1662 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1664 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1666 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1668 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwMoveInstID */

/* 1670 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1672 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1674 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wNodeInstID */

/* 1676 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 1678 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1680 */	NdrFcShort( 0x28a ),	/* Type Offset=650 */

	/* Parameter pStartDataGet */

/* 1682 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1684 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1686 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntPutStartData */


	/* Return value */

/* 1688 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1690 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1694 */	NdrFcShort( 0x1b ),	/* 27 */
/* 1696 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1698 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1700 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1702 */	NdrFcShort( 0xe ),	/* 14 */
/* 1704 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1706 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x4,		/* 4 */
/* 1708 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1710 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1712 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1714 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1716 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1718 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1720 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwMoveInstID */

/* 1722 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1724 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1726 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wNodeInstID */

/* 1728 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1730 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1732 */	NdrFcShort( 0x2e6 ),	/* Type Offset=742 */

	/* Parameter pStartDataPut */

/* 1734 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1736 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1738 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetCloseOutTime */


	/* Return value */

/* 1740 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1742 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1746 */	NdrFcShort( 0x1c ),	/* 28 */
/* 1748 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1750 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1752 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1754 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1756 */	NdrFcShort( 0x3c ),	/* 60 */
/* 1758 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 1760 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1762 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1764 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1766 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1768 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1770 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1772 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wHour */

/* 1774 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1776 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1778 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wMinute */

/* 1780 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1782 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1784 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetServerDeliveryTicketNames */


	/* Return value */

/* 1786 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1788 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1792 */	NdrFcShort( 0x1d ),	/* 29 */
/* 1794 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1796 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1798 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1800 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1802 */	NdrFcShort( 0x24 ),	/* 36 */
/* 1804 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 1806 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 1808 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1810 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1812 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1814 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1816 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1818 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwNumReports */

/* 1820 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1822 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1824 */	NdrFcShort( 0x306 ),	/* Type Offset=774 */

	/* Parameter pReportStrings */

/* 1826 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1828 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1830 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetServerPrinterNames */


	/* Return value */

/* 1832 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1834 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1838 */	NdrFcShort( 0x1e ),	/* 30 */
/* 1840 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1842 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1844 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1846 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1848 */	NdrFcShort( 0x24 ),	/* 36 */
/* 1850 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 1852 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 1854 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1856 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1858 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1860 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1862 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1864 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwNumPrinters */

/* 1866 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1868 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1870 */	NdrFcShort( 0x32e ),	/* Type Offset=814 */

	/* Parameter pPrinterNames */

/* 1872 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1874 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1876 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetStartDataBlockStatus */


	/* Return value */

/* 1878 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1880 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1884 */	NdrFcShort( 0x1f ),	/* 31 */
/* 1886 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1888 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1890 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1892 */	NdrFcShort( 0xe ),	/* 14 */
/* 1894 */	NdrFcShort( 0x6c ),	/* 108 */
/* 1896 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x4,		/* 4 */
/* 1898 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1900 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1902 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1904 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1906 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1908 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1910 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwMoveInstID */

/* 1912 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1914 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1916 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wNodeInstID */

/* 1918 */	NdrFcShort( 0x112 ),	/* Flags:  must free, out, simple ref, */
/* 1920 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1922 */	NdrFcShort( 0x372 ),	/* Type Offset=882 */

	/* Parameter pStartDataBlockStatus */

/* 1924 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1926 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1928 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetHandGaugeData */


	/* Return value */

/* 1930 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1932 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1936 */	NdrFcShort( 0x20 ),	/* 32 */
/* 1938 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1940 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1942 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1944 */	NdrFcShort( 0x13 ),	/* 19 */
/* 1946 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1948 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x5,		/* 5 */
/* 1950 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1952 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1954 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1956 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1958 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1960 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1962 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwMoveInstID */

/* 1964 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1966 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1968 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wNodeInstID */

/* 1970 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1972 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1974 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bStartData */

/* 1976 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 1978 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1980 */	NdrFcShort( 0x392 ),	/* Type Offset=914 */

	/* Parameter pHandGaugeData */

/* 1982 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1984 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1986 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntPutHandGaugeData */


	/* Return value */

/* 1988 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1990 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1994 */	NdrFcShort( 0x21 ),	/* 33 */
/* 1996 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1998 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2000 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2002 */	NdrFcShort( 0x13 ),	/* 19 */
/* 2004 */	NdrFcShort( 0x8 ),	/* 8 */
/* 2006 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 2008 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2010 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2012 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2014 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2016 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2018 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2020 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwMoveInstID */

/* 2022 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2024 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2026 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wNodeInstID */

/* 2028 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2030 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2032 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bStartData */

/* 2034 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2036 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2038 */	NdrFcShort( 0x392 ),	/* Type Offset=914 */

	/* Parameter pHandGaugeData */

/* 2040 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2042 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2044 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntSetClearDescrencyInPercent */


	/* Return value */

/* 2046 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2048 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2052 */	NdrFcShort( 0x22 ),	/* 34 */
/* 2054 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2056 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2058 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2060 */	NdrFcShort( 0x5 ),	/* 5 */
/* 2062 */	NdrFcShort( 0x8 ),	/* 8 */
/* 2064 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 2066 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2068 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2070 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2072 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2074 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2076 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2078 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bEnable */

/* 2080 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2082 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2084 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntGetDescrencyInPercent */


	/* Return value */

/* 2086 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2088 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2092 */	NdrFcShort( 0x23 ),	/* 35 */
/* 2094 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2096 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2098 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2100 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2102 */	NdrFcShort( 0x21 ),	/* 33 */
/* 2104 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 2106 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2108 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2110 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2112 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2114 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 2116 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2118 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bEnable */

/* 2120 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2122 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2124 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure MvmntHistoryDataModified */


	/* Return value */

/* 2126 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2128 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2132 */	NdrFcShort( 0x24 ),	/* 36 */
/* 2134 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2136 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2138 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2140 */	NdrFcShort( 0x66 ),	/* 102 */
/* 2142 */	NdrFcShort( 0x8 ),	/* 8 */
/* 2144 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 2146 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2148 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2150 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2152 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2154 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 2156 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2158 */	NdrFcShort( 0x3c ),	/* Type Offset=60 */

	/* Parameter pMovementName */

/* 2160 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2162 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2164 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwMoveInstID */

/* 2166 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2168 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2170 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

			0x0
        }
    };

static const link_MIDL_TYPE_FORMAT_STRING link__MIDL_TypeFormatString =
    {
        0,
        {
			NdrFcShort( 0x0 ),	/* 0 */
/*  2 */	
			0x11, 0x8,	/* FC_RP [simple_pointer] */
/*  4 */	
			0x25,		/* FC_C_WSTRING */
			0x5c,		/* FC_PAD */
/*  6 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/*  8 */	NdrFcShort( 0x2 ),	/* Offset= 2 (10) */
/* 10 */	0x30,		/* FC_BIND_CONTEXT */
			0xa0,		/* Ctxt flags:  via ptr, out, */
/* 12 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 14 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/* 16 */	0x2,		/* FC_CHAR */
			0x5c,		/* FC_PAD */
/* 18 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/* 20 */	0x6,		/* FC_SHORT */
			0x5c,		/* FC_PAD */
/* 22 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 24 */	NdrFcShort( 0x2 ),	/* 2 */
/* 26 */	0x40,		/* Corr desc:  constant, val=21 */
			0x0,		/* 0 */
/* 28 */	NdrFcShort( 0x15 ),	/* 21 */
/* 30 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 32 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 34 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/* 36 */	NdrFcShort( 0x2 ),	/* Offset= 2 (38) */
/* 38 */	0x30,		/* FC_BIND_CONTEXT */
			0xe1,		/* Ctxt flags:  via ptr, in, out, can't be null */
/* 40 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 42 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/* 44 */	0x8,		/* FC_LONG */
			0x5c,		/* FC_PAD */
/* 46 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 48 */	NdrFcShort( 0x2 ),	/* Offset= 2 (50) */
/* 50 */	
			0x12, 0x0,	/* FC_UP */
/* 52 */	NdrFcShort( 0x12 ),	/* Offset= 18 (70) */
/* 54 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 56 */	NdrFcShort( 0x2a ),	/* 42 */
/* 58 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 60 */	
			0x15,		/* FC_STRUCT */
			0x1,		/* 1 */
/* 62 */	NdrFcShort( 0x2a ),	/* 42 */
/* 64 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 66 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (54) */
/* 68 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 70 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 72 */	NdrFcShort( 0x2a ),	/* 42 */
/* 74 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 76 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 78 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 80 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 82 */	NdrFcShort( 0xffea ),	/* Offset= -22 (60) */
/* 84 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 86 */	
			0x11, 0x0,	/* FC_RP */
/* 88 */	NdrFcShort( 0xffe4 ),	/* Offset= -28 (60) */
/* 90 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 92 */	NdrFcShort( 0x2 ),	/* Offset= 2 (94) */
/* 94 */	
			0x12, 0x0,	/* FC_UP */
/* 96 */	NdrFcShort( 0x26 ),	/* Offset= 38 (134) */
/* 98 */	
			0x1d,		/* FC_SMFARRAY */
			0x3,		/* 3 */
/* 100 */	NdrFcShort( 0x14 ),	/* 20 */
/* 102 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 104 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 106 */	NdrFcShort( 0x4a ),	/* 74 */
/* 108 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 110 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 112 */	NdrFcShort( 0x70 ),	/* 112 */
/* 114 */	NdrFcShort( 0x0 ),	/* 0 */
/* 116 */	NdrFcShort( 0x0 ),	/* Offset= 0 (116) */
/* 118 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 120 */	NdrFcShort( 0xffea ),	/* Offset= -22 (98) */
/* 122 */	0x8,		/* FC_LONG */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 124 */	0x0,		/* 0 */
			NdrFcShort( 0xffeb ),	/* Offset= -21 (104) */
			0x3e,		/* FC_STRUCTPAD2 */
/* 128 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 130 */	0x6,		/* FC_SHORT */
			0x3e,		/* FC_STRUCTPAD2 */
/* 132 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 134 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x3,		/* 3 */
/* 136 */	NdrFcShort( 0x0 ),	/* 0 */
/* 138 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 140 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 142 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 144 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 148 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 150 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 152 */	NdrFcShort( 0xffd6 ),	/* Offset= -42 (110) */
/* 154 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 156 */	
			0x11, 0x0,	/* FC_RP */
/* 158 */	NdrFcShort( 0xffd0 ),	/* Offset= -48 (110) */
/* 160 */	0x30,		/* FC_BIND_CONTEXT */
			0x41,		/* Ctxt flags:  in, can't be null */
/* 162 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 164 */	
			0x11, 0x0,	/* FC_RP */
/* 166 */	NdrFcShort( 0x2 ),	/* Offset= 2 (168) */
/* 168 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 170 */	NdrFcShort( 0x30 ),	/* 48 */
/* 172 */	NdrFcShort( 0x0 ),	/* 0 */
/* 174 */	NdrFcShort( 0x0 ),	/* Offset= 0 (174) */
/* 176 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 178 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 180 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 182 */	0x3e,		/* FC_STRUCTPAD2 */
			0xc,		/* FC_DOUBLE */
/* 184 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 186 */	0x2,		/* FC_CHAR */
			0x3d,		/* FC_STRUCTPAD1 */
/* 188 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 190 */	0xc,		/* FC_DOUBLE */
			0x2,		/* FC_CHAR */
/* 192 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 194 */	0x41,		/* FC_STRUCTPAD5 */
			0x5b,		/* FC_END */
/* 196 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 198 */	NdrFcShort( 0x2 ),	/* Offset= 2 (200) */
/* 200 */	
			0x12, 0x0,	/* FC_UP */
/* 202 */	NdrFcShort( 0x2 ),	/* Offset= 2 (204) */
/* 204 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 206 */	NdrFcShort( 0x2 ),	/* 2 */
/* 208 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 210 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 212 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 214 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 216 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 218 */	NdrFcShort( 0x2 ),	/* Offset= 2 (220) */
/* 220 */	
			0x12, 0x0,	/* FC_UP */
/* 222 */	NdrFcShort( 0x2 ),	/* Offset= 2 (224) */
/* 224 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x3,		/* 3 */
/* 226 */	NdrFcShort( 0x0 ),	/* 0 */
/* 228 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 230 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 232 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 234 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 238 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 240 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 242 */	NdrFcShort( 0xff7c ),	/* Offset= -132 (110) */
/* 244 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 246 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 248 */	NdrFcShort( 0x2 ),	/* Offset= 2 (250) */
/* 250 */	
			0x12, 0x0,	/* FC_UP */
/* 252 */	NdrFcShort( 0x2 ),	/* Offset= 2 (254) */
/* 254 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 256 */	NdrFcShort( 0x2 ),	/* 2 */
/* 258 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 260 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 262 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 264 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 266 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 268 */	NdrFcShort( 0x2 ),	/* Offset= 2 (270) */
/* 270 */	
			0x12, 0x0,	/* FC_UP */
/* 272 */	NdrFcShort( 0x7c ),	/* Offset= 124 (396) */
/* 274 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 276 */	NdrFcShort( 0x192 ),	/* 402 */
/* 278 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 280 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 282 */	NdrFcShort( 0x3e ),	/* 62 */
/* 284 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 286 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 288 */	NdrFcShort( 0x26c ),	/* 620 */
/* 290 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 292 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (280) */
/* 294 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 296 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 298 */	NdrFcShort( 0xa2 ),	/* 162 */
/* 300 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 302 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 304 */	NdrFcShort( 0x102 ),	/* 258 */
/* 306 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 308 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 310 */	NdrFcShort( 0x510 ),	/* 1296 */
/* 312 */	NdrFcShort( 0x0 ),	/* 0 */
/* 314 */	NdrFcShort( 0x0 ),	/* Offset= 0 (314) */
/* 316 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 318 */	NdrFcShort( 0xff2a ),	/* Offset= -214 (104) */
/* 320 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 322 */	NdrFcShort( 0xff26 ),	/* Offset= -218 (104) */
/* 324 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 326 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 328 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 330 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 332 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 334 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 336 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 338 */	0x3d,		/* FC_STRUCTPAD1 */
			0x6,		/* FC_SHORT */
/* 340 */	0x6,		/* FC_SHORT */
			0xc,		/* FC_DOUBLE */
/* 342 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 344 */	0xc,		/* FC_DOUBLE */
			0x2,		/* FC_CHAR */
/* 346 */	0x2,		/* FC_CHAR */
			0x42,		/* FC_STRUCTPAD6 */
/* 348 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 350 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 352 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 354 */	0x2,		/* FC_CHAR */
			0x3d,		/* FC_STRUCTPAD1 */
/* 356 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 358 */	NdrFcShort( 0xffc8 ),	/* Offset= -56 (302) */
/* 360 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 362 */	NdrFcShort( 0xffc4 ),	/* Offset= -60 (302) */
/* 364 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 366 */	NdrFcShort( 0xffc0 ),	/* Offset= -64 (302) */
/* 368 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 370 */	NdrFcShort( 0xffbc ),	/* Offset= -68 (302) */
/* 372 */	0x42,		/* FC_STRUCTPAD6 */
			0x5b,		/* FC_END */
/* 374 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x7,		/* 7 */
/* 376 */	NdrFcShort( 0x0 ),	/* 0 */
/* 378 */	0x17,		/* Corr desc:  field pointer, FC_USHORT */
			0x0,		/*  */
/* 380 */	NdrFcShort( 0x6a8 ),	/* 1704 */
/* 382 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 384 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 388 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 390 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 392 */	NdrFcShort( 0xffac ),	/* Offset= -84 (308) */
/* 394 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 396 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 398 */	NdrFcShort( 0x6b0 ),	/* 1712 */
/* 400 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 402 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 404 */	NdrFcShort( 0x6ac ),	/* 1708 */
/* 406 */	NdrFcShort( 0x6ac ),	/* 1708 */
/* 408 */	0x12, 0x0,	/* FC_UP */
/* 410 */	NdrFcShort( 0xffdc ),	/* Offset= -36 (374) */
/* 412 */	
			0x5b,		/* FC_END */

			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 414 */	0x0,		/* 0 */
			NdrFcShort( 0xfe97 ),	/* Offset= -361 (54) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 418 */	0x0,		/* 0 */
			NdrFcShort( 0xfe93 ),	/* Offset= -365 (54) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 422 */	0x0,		/* 0 */
			NdrFcShort( 0xff6b ),	/* Offset= -149 (274) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 426 */	0x0,		/* 0 */
			NdrFcShort( 0xff73 ),	/* Offset= -141 (286) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 430 */	0x0,		/* 0 */
			NdrFcShort( 0xfe87 ),	/* Offset= -377 (54) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 434 */	0x0,		/* 0 */
			NdrFcShort( 0xff75 ),	/* Offset= -139 (296) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 438 */	0x0,		/* 0 */
			NdrFcShort( 0xff71 ),	/* Offset= -143 (296) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 442 */	0x0,		/* 0 */
			NdrFcShort( 0xff6d ),	/* Offset= -147 (296) */
			0x3e,		/* FC_STRUCTPAD2 */
/* 446 */	0x8,		/* FC_LONG */
			0x2,		/* FC_CHAR */
/* 448 */	0x3f,		/* FC_STRUCTPAD3 */
			0x8,		/* FC_LONG */
/* 450 */	0x8,		/* FC_LONG */
			0x2,		/* FC_CHAR */
/* 452 */	0x3f,		/* FC_STRUCTPAD3 */
			0x8,		/* FC_LONG */
/* 454 */	0x8,		/* FC_LONG */
			0x2,		/* FC_CHAR */
/* 456 */	0x3f,		/* FC_STRUCTPAD3 */
			0x8,		/* FC_LONG */
/* 458 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 460 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 462 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 464 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 466 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 468 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 470 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 472 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 474 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 476 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 478 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 480 */	0x2,		/* FC_CHAR */
			0x8,		/* FC_LONG */
/* 482 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 484 */	0x3e,		/* FC_STRUCTPAD2 */
			0x8,		/* FC_LONG */
/* 486 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 488 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 490 */	NdrFcShort( 0x2 ),	/* Offset= 2 (492) */
/* 492 */	
			0x12, 0x0,	/* FC_UP */
/* 494 */	NdrFcShort( 0x24 ),	/* Offset= 36 (530) */
/* 496 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 498 */	NdrFcShort( 0x2 ),	/* 2 */
/* 500 */	0x14,		/* Corr desc:  field pointer, FC_USMALL */
			0x0,		/*  */
/* 502 */	NdrFcShort( 0x0 ),	/* 0 */
/* 504 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 506 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 508 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 510 */	NdrFcShort( 0x8 ),	/* 8 */
/* 512 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 514 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 516 */	NdrFcShort( 0x4 ),	/* 4 */
/* 518 */	NdrFcShort( 0x4 ),	/* 4 */
/* 520 */	0x12, 0x0,	/* FC_UP */
/* 522 */	NdrFcShort( 0xffe6 ),	/* Offset= -26 (496) */
/* 524 */	
			0x5b,		/* FC_END */

			0x2,		/* FC_CHAR */
/* 526 */	0x3f,		/* FC_STRUCTPAD3 */
			0x8,		/* FC_LONG */
/* 528 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 530 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 532 */	NdrFcShort( 0x8 ),	/* 8 */
/* 534 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 536 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 538 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 540 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 542 */	
			0x48,		/* FC_VARIABLE_REPEAT */
			0x49,		/* FC_FIXED_OFFSET */
/* 544 */	NdrFcShort( 0x8 ),	/* 8 */
/* 546 */	NdrFcShort( 0x0 ),	/* 0 */
/* 548 */	NdrFcShort( 0x1 ),	/* 1 */
/* 550 */	NdrFcShort( 0x4 ),	/* 4 */
/* 552 */	NdrFcShort( 0x4 ),	/* 4 */
/* 554 */	0x12, 0x0,	/* FC_UP */
/* 556 */	NdrFcShort( 0xffc4 ),	/* Offset= -60 (496) */
/* 558 */	
			0x5b,		/* FC_END */

			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 560 */	0x0,		/* 0 */
			NdrFcShort( 0xffcb ),	/* Offset= -53 (508) */
			0x5b,		/* FC_END */
/* 564 */	
			0x11, 0x0,	/* FC_RP */
/* 566 */	NdrFcShort( 0x2 ),	/* Offset= 2 (568) */
/* 568 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 570 */	NdrFcShort( 0x2a ),	/* 42 */
/* 572 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x0,		/*  */
/* 574 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 576 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 578 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 580 */	NdrFcShort( 0xfdf8 ),	/* Offset= -520 (60) */
/* 582 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 584 */	
			0x11, 0x0,	/* FC_RP */
/* 586 */	NdrFcShort( 0x2 ),	/* Offset= 2 (588) */
/* 588 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 590 */	NdrFcShort( 0x50 ),	/* 80 */
/* 592 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 594 */	NdrFcShort( 0xfe16 ),	/* Offset= -490 (104) */
/* 596 */	0x3e,		/* FC_STRUCTPAD2 */
			0x8,		/* FC_LONG */
/* 598 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 600 */	
			0x11, 0x0,	/* FC_RP */
/* 602 */	NdrFcShort( 0x2 ),	/* Offset= 2 (604) */
/* 604 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 606 */	NdrFcShort( 0x50 ),	/* 80 */
/* 608 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x0,		/*  */
/* 610 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 612 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 614 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 616 */	NdrFcShort( 0xffe4 ),	/* Offset= -28 (588) */
/* 618 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 620 */	
			0x11, 0x0,	/* FC_RP */
/* 622 */	NdrFcShort( 0x1c ),	/* Offset= 28 (650) */
/* 624 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 626 */	NdrFcShort( 0x20 ),	/* 32 */
/* 628 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 630 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 632 */	NdrFcShort( 0x80 ),	/* 128 */
/* 634 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 636 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (624) */
/* 638 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 640 */	
			0x15,		/* FC_STRUCT */
			0x1,		/* 1 */
/* 642 */	NdrFcShort( 0x80 ),	/* 128 */
/* 644 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 646 */	NdrFcShort( 0xfff0 ),	/* Offset= -16 (630) */
/* 648 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 650 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 652 */	NdrFcShort( 0x278 ),	/* 632 */
/* 654 */	NdrFcShort( 0x0 ),	/* 0 */
/* 656 */	NdrFcShort( 0x0 ),	/* Offset= 0 (656) */
/* 658 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 660 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 662 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 664 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 666 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 668 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 670 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 672 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 674 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 676 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 678 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 680 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 682 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 684 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 686 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 688 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 690 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 692 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 694 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 696 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 698 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 700 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 702 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 704 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 706 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 708 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 710 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 712 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 714 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 716 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 718 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 720 */	0x2,		/* FC_CHAR */
			0x3d,		/* FC_STRUCTPAD1 */
/* 722 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 724 */	NdrFcShort( 0xffac ),	/* Offset= -84 (640) */
/* 726 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 728 */	NdrFcShort( 0xfd96 ),	/* Offset= -618 (110) */
/* 730 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 732 */	
			0x11, 0x0,	/* FC_RP */
/* 734 */	NdrFcShort( 0x8 ),	/* Offset= 8 (742) */
/* 736 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 738 */	NdrFcShort( 0x28 ),	/* 40 */
/* 740 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 742 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 744 */	NdrFcShort( 0xa8 ),	/* 168 */
/* 746 */	NdrFcShort( 0x0 ),	/* 0 */
/* 748 */	NdrFcShort( 0x0 ),	/* Offset= 0 (748) */
/* 750 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 752 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 754 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 756 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 758 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 760 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 762 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 764 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 766 */	0x8,		/* FC_LONG */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 768 */	0x0,		/* 0 */
			NdrFcShort( 0xffdf ),	/* Offset= -33 (736) */
			0x40,		/* FC_STRUCTPAD4 */
/* 772 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 774 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 776 */	NdrFcShort( 0x2 ),	/* Offset= 2 (778) */
/* 778 */	
			0x12, 0x0,	/* FC_UP */
/* 780 */	NdrFcShort( 0x12 ),	/* Offset= 18 (798) */
/* 782 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 784 */	NdrFcShort( 0x20a ),	/* 522 */
/* 786 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 788 */	
			0x15,		/* FC_STRUCT */
			0x1,		/* 1 */
/* 790 */	NdrFcShort( 0x20a ),	/* 522 */
/* 792 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 794 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (782) */
/* 796 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 798 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 800 */	NdrFcShort( 0x20a ),	/* 522 */
/* 802 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 804 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 806 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 808 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 810 */	NdrFcShort( 0xffea ),	/* Offset= -22 (788) */
/* 812 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 814 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 816 */	NdrFcShort( 0x2 ),	/* Offset= 2 (818) */
/* 818 */	
			0x12, 0x0,	/* FC_UP */
/* 820 */	NdrFcShort( 0x18 ),	/* Offset= 24 (844) */
/* 822 */	
			0x16,		/* FC_PSTRUCT */
			0x3,		/* 3 */
/* 824 */	NdrFcShort( 0x8 ),	/* 8 */
/* 826 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 828 */	
			0x46,		/* FC_NO_REPEAT */
			0x5c,		/* FC_PAD */
/* 830 */	NdrFcShort( 0x4 ),	/* 4 */
/* 832 */	NdrFcShort( 0x4 ),	/* 4 */
/* 834 */	0x12, 0x0,	/* FC_UP */
/* 836 */	NdrFcShort( 0xfeac ),	/* Offset= -340 (496) */
/* 838 */	
			0x5b,		/* FC_END */

			0x2,		/* FC_CHAR */
/* 840 */	0x2,		/* FC_CHAR */
			0x3e,		/* FC_STRUCTPAD2 */
/* 842 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 844 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 846 */	NdrFcShort( 0x8 ),	/* 8 */
/* 848 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 850 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 852 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 854 */	
			0x4b,		/* FC_PP */
			0x5c,		/* FC_PAD */
/* 856 */	
			0x48,		/* FC_VARIABLE_REPEAT */
			0x49,		/* FC_FIXED_OFFSET */
/* 858 */	NdrFcShort( 0x8 ),	/* 8 */
/* 860 */	NdrFcShort( 0x0 ),	/* 0 */
/* 862 */	NdrFcShort( 0x1 ),	/* 1 */
/* 864 */	NdrFcShort( 0x4 ),	/* 4 */
/* 866 */	NdrFcShort( 0x4 ),	/* 4 */
/* 868 */	0x12, 0x0,	/* FC_UP */
/* 870 */	NdrFcShort( 0xfe8a ),	/* Offset= -374 (496) */
/* 872 */	
			0x5b,		/* FC_END */

			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 874 */	0x0,		/* 0 */
			NdrFcShort( 0xffcb ),	/* Offset= -53 (822) */
			0x5b,		/* FC_END */
/* 878 */	
			0x11, 0x0,	/* FC_RP */
/* 880 */	NdrFcShort( 0x2 ),	/* Offset= 2 (882) */
/* 882 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 884 */	NdrFcShort( 0x40 ),	/* 64 */
/* 886 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 888 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 890 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 892 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 894 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 896 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 898 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 900 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 902 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 904 */	
			0x11, 0x0,	/* FC_RP */
/* 906 */	NdrFcShort( 0x8 ),	/* Offset= 8 (914) */
/* 908 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 910 */	NdrFcShort( 0x64 ),	/* 100 */
/* 912 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 914 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 916 */	NdrFcShort( 0x1a0 ),	/* 416 */
/* 918 */	NdrFcShort( 0x0 ),	/* 0 */
/* 920 */	NdrFcShort( 0x0 ),	/* Offset= 0 (920) */
/* 922 */	0xc,		/* FC_DOUBLE */
			0x8,		/* FC_LONG */
/* 924 */	0x40,		/* FC_STRUCTPAD4 */
			0xc,		/* FC_DOUBLE */
/* 926 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 928 */	0xc,		/* FC_DOUBLE */
			0x8,		/* FC_LONG */
/* 930 */	0x40,		/* FC_STRUCTPAD4 */
			0xc,		/* FC_DOUBLE */
/* 932 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 934 */	0xc,		/* FC_DOUBLE */
			0x8,		/* FC_LONG */
/* 936 */	0x40,		/* FC_STRUCTPAD4 */
			0xc,		/* FC_DOUBLE */
/* 938 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 940 */	0xc,		/* FC_DOUBLE */
			0x8,		/* FC_LONG */
/* 942 */	0x40,		/* FC_STRUCTPAD4 */
			0xc,		/* FC_DOUBLE */
/* 944 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 946 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 948 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 950 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 952 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 954 */	NdrFcShort( 0xffd2 ),	/* Offset= -46 (908) */
/* 956 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 958 */	NdrFcShort( 0xfec2 ),	/* Offset= -318 (640) */
/* 960 */	0x40,		/* FC_STRUCTPAD4 */
			0x5b,		/* FC_END */

			0x0
        }
    };

static const unsigned short mvmntlink_FormatStringOffsetTable[] =
    {
    0,
    76,
    142,
    188,
    228,
    274,
    314,
    360,
    406,
    460,
    508,
    560,
    608,
    654,
    702,
    750,
    804,
    856,
    908,
    966,
    1024,
    1076,
    1116,
    1162,
    1220,
    1266,
    1636,
    1688,
    1740,
    1786,
    1832,
    1878,
    1930,
    1988,
    2046,
    2086,
    2126
    };


static const unsigned short _callbackmvmntlink_FormatStringOffsetTable[] =
    {
    1336,
    1384,
    1438,
    1480,
    1516,
    1552,
    };


static const MIDL_STUB_DESC mvmntlink_StubDesc = 
    {
    (void *)& mvmntlink___RpcClientInterface,
    MIDL_user_allocate,
    MIDL_user_free,
    &mvmntlink__MIDL_AutoBindHandle,
    0,
    0,
    0,
    0,
    link__MIDL_TypeFormatString.Format,
    1, /* -error bounds_check flag */
    0x50002, /* Ndr library version */
    0,
    0x70001f4, /* MIDL Version 7.0.500 */
    0,
    0,
    0,  /* notify & notify_flag routine table */
    0x1, /* MIDL flag */
    0, /* cs routines */
    0,   /* proxy/server info */
    0
    };

static RPC_DISPATCH_FUNCTION mvmntlink_table[] =
    {
    NdrServerCall2,
    NdrServerCall2,
    NdrServerCall2,
    NdrServerCall2,
    NdrServerCall2,
    NdrServerCall2,
    0
    };
RPC_DISPATCH_TABLE mvmntlink_DispatchTable = 
    {
    6,
    mvmntlink_table
    };

static const SERVER_ROUTINE mvmntlink_ServerRoutineTable[] = 
    {
    (SERVER_ROUTINE)MvmntCombinationWarning,
    (SERVER_ROUTINE)MvmntContaminateWarning,
    (SERVER_ROUTINE)MvmntAddUnitNodeWarning,
    (SERVER_ROUTINE)MvmntLockoutWarning,
    (SERVER_ROUTINE)MvmntSetupWarning,
    (SERVER_ROUTINE)MvmntChangeTransferWarning,
    };

static const MIDL_SERVER_INFO mvmntlink_ServerInfo = 
    {
    &mvmntlink_StubDesc,
    mvmntlink_ServerRoutineTable,
    link__MIDL_ProcFormatString.Format,
    _callbackmvmntlink_FormatStringOffsetTable,
    0,
    0,
    0,
    0};
#pragma optimize("", on )
#if _MSC_VER >= 1200
#pragma warning(pop)
#endif

#endif /* !defined(_M_IA64) && !defined(_M_AMD64)*/

