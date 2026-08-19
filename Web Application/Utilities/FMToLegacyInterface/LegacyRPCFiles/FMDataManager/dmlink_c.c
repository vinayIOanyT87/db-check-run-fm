

/* this ALWAYS GENERATED file contains the RPC client stubs */


 /* File created by MIDL compiler version 7.00.0500 */
/* at Thu Jul 23 14:56:09 2020
 */
/* Compiler settings for dmlink.idl:
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

#include "dmlink.h"

#define TYPE_FORMAT_STRING_SIZE   3067                              
#define PROC_FORMAT_STRING_SIZE   6621                              
#define EXPR_FORMAT_STRING_SIZE   1                                 
#define TRANSMIT_AS_TABLE_SIZE    0            
#define WIRE_MARSHAL_TABLE_SIZE   0            

typedef struct _dmlink_MIDL_TYPE_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ TYPE_FORMAT_STRING_SIZE ];
    } dmlink_MIDL_TYPE_FORMAT_STRING;

typedef struct _dmlink_MIDL_PROC_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ PROC_FORMAT_STRING_SIZE ];
    } dmlink_MIDL_PROC_FORMAT_STRING;

typedef struct _dmlink_MIDL_EXPR_FORMAT_STRING
    {
    long          Pad;
    unsigned char  Format[ EXPR_FORMAT_STRING_SIZE ];
    } dmlink_MIDL_EXPR_FORMAT_STRING;


static RPC_SYNTAX_IDENTIFIER  _RpcTransferSyntax = 
{{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}};


extern const dmlink_MIDL_TYPE_FORMAT_STRING dmlink__MIDL_TypeFormatString;
extern const dmlink_MIDL_PROC_FORMAT_STRING dmlink__MIDL_ProcFormatString;
extern const dmlink_MIDL_EXPR_FORMAT_STRING dmlink__MIDL_ExprFormatString;

#define GENERIC_BINDING_TABLE_SIZE   0            


/* Standard interface: dmlink, ver. 1.0,
   GUID={0xCFCFDB01,0x0793,0x0001,{0xDB,0x01,0x00,0xA0,0x0B,0x00,0xC0,0x0D}} */



static const RPC_CLIENT_INTERFACE dmlink___RpcClientInterface =
    {
    sizeof(RPC_CLIENT_INTERFACE),
    {{0xCFCFDB01,0x0793,0x0001,{0xDB,0x01,0x00,0xA0,0x0B,0x00,0xC0,0x0D}},{1,0}},
    {{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}},
    0,
    0,
    0,
    0,
    0,
    0x00000000
    };
RPC_IF_HANDLE dmlink_ClientIfHandle = (RPC_IF_HANDLE)& dmlink___RpcClientInterface;

extern const MIDL_STUB_DESC dmlink_StubDesc;

static RPC_BINDING_HANDLE dmlink__MIDL_AutoBindHandle;


unsigned long ValidateDMStatus( 
    /* [in] */ handle_t hBinding)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[0],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned char Get_DM_Parameters( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned char *blevels,
    /* [out] */ unsigned short *pID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[34],
                  ( unsigned char * )&hBinding);
    return ( unsigned char  )_RetVal.Simple;
    
}


short GetLevelTags( 
    /* [in] */ handle_t hBinding,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ TAGNAME **ppTagName,
    /* [size_is][size_is][out] */ TAGEXTEND **ppTagExtend)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[80],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short GetPointStrings( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwPointID,
    /* [out] */ LEVELNAMES *Names)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[144],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short GetLevelTagsMasked( 
    /* [in] */ handle_t hBinding,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned short wFormat,
    /* [in] */ unsigned short wUnits,
    /* [in] */ unsigned char bPointType,
    /* [in] */ unsigned char bVariable,
    /* [in] */ unsigned char bSearchMode,
    /* [string][in] */ wchar_t *pUserName,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ TAGNAME **ppTagName,
    /* [size_is][size_is][out] */ TAGEXTEND **ppTagExtend)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[190],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short AddDatabasePoint( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ POINTSPEC *pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[284],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short GetPointItems( 
    /* [in] */ handle_t hBinding,
    /* [in] */ LEVELNAMES *Names,
    /* [out] */ POINTSPEC *pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[348],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


unsigned long GetPointVariable( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short wPointIndex,
    /* [in] */ unsigned char bPointType,
    /* [in] */ unsigned short bVariable,
    /* [out] */ VARDETAIL *pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[394],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


short EditDatabaseStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PDATAPOINT pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[458],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short EditDatabaseCancel( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ unsigned char bType)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[528],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short EditDatabaseDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PDATAPOINT pNewPoint)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[600],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short EditTankDataStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PTANKDATA pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[672],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short EditTankDataDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PTANKDATA pNewPoint)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[742],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short PurgeDataPoint( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned long dwPntID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[814],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short QueryLockoutUser( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lAction,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ short sNameLen,
    /* [in] */ short sSysLen,
    /* [size_is][out] */ wchar_t pUserName[  ],
    /* [size_is][out] */ wchar_t pSystem[  ])
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[872],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


unsigned long RPC_SaveDatabase( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[942],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_ValidateBlock( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lsize,
    /* [size_is][in] */ PBLOCKREQENTRY pBlock,
    /* [size_is][out] */ unsigned char pResult[  ])
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[982],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_CommandData( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [in] */ unsigned long dwPointId,
    /* [in] */ unsigned short bVarType,
    /* [out] */ PPNTCOMMAND pOutput)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1034],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_OperatorCommand( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short bVarType,
    /* [in] */ unsigned char bFormat,
    /* [in] */ unsigned char bOverride,
    /* [in] */ PFMDATA512 pDataIn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1092],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_AllocateSource( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned char bAllocate,
    /* [in] */ PPOINTALLOC pInput,
    /* [in] */ long nSourceIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1168],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetSystemString( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bSysIndex,
    /* [size_is][out] */ wchar_t pSysName[  ])
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1232],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetSystemIndex( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pUserSystem,
    /* [in] */ PSYSIDATA pSysParam,
    /* [in] */ unsigned char cAdd,
    /* [out] */ unsigned char *pIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1278],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EnumerateVariables( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PVARMASK pInput,
    /* [out] */ unsigned short *pbArraySize,
    /* [size_is][size_is][out] */ unsigned short **ppResult)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1342],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetStrapTable( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [out] */ unsigned char *pLvlUnits,
    /* [out] */ unsigned char *pVolUnits,
    /* [out] */ unsigned char *pPressUnits,
    /* [out] */ unsigned long *pdwSize,
    /* [size_is][size_is][out] */ STRAPENTRY **pOut,
    /* [out] */ unsigned long *pdwWTSize,
    /* [size_is][size_is][out] */ STRAPENTRY **pWTOut,
    /* [out] */ unsigned long *pdwHydroSize,
    /* [size_is][size_is][out] */ STRAPENTRY **pHydroOut)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1394],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_ReplaceStrapTable( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ unsigned char bIndUnits,
    /* [in] */ unsigned char bDepUnits,
    /* [in] */ unsigned long dwSize,
    /* [size_is][in] */ PSTRAPENTRY pNew,
    /* [in] */ unsigned long dwWTSize,
    /* [size_is][in] */ PSTRAPENTRY pWTNew,
    /* [in] */ unsigned long pdwHydroSize,
    /* [size_is][in] */ PSTRAPENTRY pHydroOut)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1514],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


short EnumerateTanks( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PFM_DESC_STRUCT *stDescription,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppTankTags)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1628],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


unsigned long GetTankDetail( 
    /* [in] */ handle_t hBinding,
    /* [in] */ LEVELNAMES *Names,
    /* [out] */ PTANKDETAIL pTankDetail)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1686],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_AlarmAck( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [in] */ unsigned long dwAlarmID,
    /* [in] */ unsigned char bOffSet)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1732],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_SilenceAlarm( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwAlarmAccess)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1784],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_SetProduct( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [in] */ unsigned long dwTankID,
    /* [in] */ unsigned long dwTankIndex,
    /* [in] */ unsigned long dwProductId)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1824],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EditPointAlarm( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned char bVariable,
    /* [out] */ PALARMEDIT pAlarmData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1882],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EditPointAlarmDone( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned char bVariable,
    /* [out][in] */ PALARMEDIT pNewAlarm)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[1954],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_DeletePointAlarm( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned char bVariable)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2026],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EnumGlobalAlarms( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bTemplate,
    /* [in] */ unsigned char bDataType,
    /* [out] */ long *plAccessTime,
    /* [out] */ unsigned short *pOutNum,
    /* [size_is][size_is][out] */ GLOBALALARMINDEX **ppGlobalAlarmIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2092],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EditGlobalAlarm( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwAlarmId,
    /* [in] */ long lAccessTime,
    /* [string][in] */ wchar_t *pAlarmName,
    /* [out] */ PSTDALARM pAlarmData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2156],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EditGlobalAlarmDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwAlarmId,
    /* [in] */ PSTDALARM pNewAlarm)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2232],
                  ( unsigned char * )&pphContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_AddGlobalAlarm( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ PSTDALARM pNewAlarm,
    /* [out] */ unsigned long *pDwAlarmId)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2298],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_DeleteGlobalAlarm( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [in] */ unsigned long dwAlarmId,
    /* [in] */ long lAccessTime,
    /* [string][in] */ wchar_t *pAlarmName)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2356],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_CancelAlarmEdit( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwAlarmID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2414],
                  ( unsigned char * )&pphContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetGlobalAlarmData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwAlarmdef,
    /* [in] */ long lAccessTime,
    /* [string][in] */ wchar_t *pAlarmName,
    /* [out] */ PSTDALARM pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2474],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetGlobalAlarmName( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwAlarmDef,
    /* [size_is][out] */ wchar_t pName[  ])
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2532],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetExtendedAlarmData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwAlarmID,
    /* [out] */ unsigned short *pGraphic,
    /* [out] */ unsigned short *pHelp,
    /* [out] */ unsigned char *bAutoLoad,
    /* [size_is][out] */ wchar_t szGraphic[  ],
    /* [size_is][out] */ wchar_t szTemplate[  ])
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2578],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_AllocateTankCalc( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ PLEVELNAMES pTankName,
    /* [out] */ unsigned long *pCalcTank,
    /* [out] */ PSETCALC pSet)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2648],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_QuitTankCalc( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwTankIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2718],
                  ( unsigned char * )&pphContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_DoTankCalculate( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ double dLevel,
    /* [in] */ double dTemp,
    /* [in] */ double dStdDens,
    /* [in] */ double dH2OLev,
    /* [in] */ double dBSW,
    /* [in] */ double dSolidLev,
    /* [in] */ double dAmbientTemp,
    /* [in] */ double dVaporTemp,
    /* [in] */ double dVaporPress,
    /* [in] */ double dCorrectionVolume,
    /* [in] */ double dGasDensity,
    /* [in] */ double dXfrValue,
    /* [in] */ double dDensityTempValue,
    /* [in] */ double dGaugeStdDensity,
    /* [in] */ unsigned long dwTankIndex,
    /* [in] */ unsigned long dwXfrBaseMode,
    /* [in] */ unsigned char bNewInput,
    /* [in] */ unsigned char bUseMeasuredDensity,
    /* [out] */ PTANKCALCULATE pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2778],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_SearchTags( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PPOINTFILTER pFilter,
    /* [out] */ unsigned short *pFound,
    /* [size_is][size_is][out] */ POINTSPEC **ppPointSpec)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2934],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EditTankAlarm( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [out] */ PTANKALARMEDIT pTankAlarm)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[2986],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EditTankAlarmDone( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned char bOutageTank,
    /* [in] */ PTANKALARMEDIT pNewAlarm)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3052],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetDeviceSource( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwPntID,
    /* [out] */ PDEVICESOURCE pGaugeData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3124],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetDeviceSourceEx( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned short wSelectedVariable,
    /* [in] */ unsigned char bDoNotLockMutex,
    /* [out] */ PDEVICESOURCE pGaugeData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3170],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetSystemIndexData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bIndex,
    /* [out] */ PSYSIDATA pSysParam)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3228],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_AllocPointSource( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned short bVariable,
    /* [in] */ unsigned char bClear,
    /* [in] */ PFMSOURCE pSource,
    /* [in] */ long nSourceIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3274],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_AddEventRecord( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ unsigned long bAdvise,
    /* [in] */ FMDATA512 prDeadBand,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned long wDataType,
    /* [in] */ unsigned char bCategory,
    /* [out] */ long *plNewHandle)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3358],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetEventAttributes( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ long lHandle,
    /* [out] */ unsigned long *pwDataType,
    /* [out] */ unsigned long *pbAdvise,
    /* [out] */ unsigned char *pbCategory,
    /* [out] */ PFMDATA512 prDeadBand)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3436],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_SetEventAttributes( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ long lHandle,
    /* [in] */ unsigned long wDataType,
    /* [in] */ unsigned long bAdvise,
    /* [in] */ unsigned char bCategory,
    /* [in] */ FMDATA512 prDeadBand)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3508],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_RemoveEventRecord( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ long lHandle)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3580],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_ChangeDataType( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ long lHandle,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned long dwNewDataType)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3628],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetLogPath( 
    /* [in] */ handle_t hBinding,
    /* [size_is][out][in] */ wchar_t szUniversalName[  ])
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3688],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PBLOCKREQENTRY pBlockReq,
    /* [out][in] */ unsigned long *ulStatus1,
    /* [out][in] */ unsigned long *ulStatus2,
    /* [out][in] */ unsigned char *byChanged,
    /* [size_is][out][in] */ unsigned char *pData,
    /* [in] */ unsigned long ulDataSize)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3728],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_SetData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PBLOCKREQENTRY pBlockReq,
    /* [in] */ PFMDATA512 pDataIn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3798],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_AddComment( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PDMLOG pLogEntry,
    /* [string][in] */ wchar_t *pszFileName)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3844],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_AddOpcFileEntry( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t szCLSID[ 256 ],
    /* [string][in] */ wchar_t szSystem[ 18 ],
    /* [string][in] */ wchar_t szTag[ 256 ],
    /* [string][in] */ wchar_t szServer[ 256 ],
    /* [out][in] */ unsigned long *dwTagID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3890],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetOpcSourceStrings( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwTagID,
    /* [size_is][out] */ wchar_t szSystem[  ],
    /* [size_is][out] */ wchar_t szTag[  ],
    /* [size_is][out] */ wchar_t szServer[  ],
    /* [size_is][out] */ wchar_t szCLSID[  ])
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[3954],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_DeleteOpcEntry( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwTagID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4018],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetOpcUAEntryData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bPointType,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short wPointIndex,
    /* [in] */ unsigned short bVariable,
    /* [in] */ unsigned short wAllocatedType,
    /* [in] */ int iSourceIndex,
    /* [size_is][out][in] */ unsigned char *pData,
    /* [in] */ unsigned long ulDataSize)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4058],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_SetOpcUAEntryData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bPointType,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short wPointIndex,
    /* [in] */ unsigned short bVariable,
    /* [in] */ unsigned short wAllocatedType,
    /* [in] */ int iSourceIndex,
    /* [size_is][out][in] */ unsigned char *pData,
    /* [in] */ unsigned long ulDataSize,
    /* [out] */ unsigned long *dwDataIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4140],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_DeleteOpcUAEntry( 
    /* [in] */ handle_t hBinding,
    /* [in] */ OPCUA OpcData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4228],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetOpcUASourceString( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bPointType,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short wPointIndex,
    /* [in] */ unsigned short bVariable,
    /* [in] */ unsigned short wAllocatedType,
    /* [in] */ int iSourceIndex,
    /* [size_is][out][in] */ wchar_t szServer[  ],
    /* [size_is][out][in] */ wchar_t szSelectedOpcTag[  ])
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4268],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


short RPC_EnumerateCopyPoints( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystemName,
    /* [in] */ PPOINTCOPY pntCopy,
    /* [out] */ unsigned long *pNumberPoints,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppNewTagArray,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppOldTagArray,
    /* [size_is][size_is][out] */ PTAGENTRY *ppTagEntry)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4350],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


unsigned long RPC_ExecutePointCopy( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystemName,
    /* [in] */ PPOINTCOPY pntCopy,
    /* [in] */ PLEVELNAMES pNewNamesArray,
    /* [in] */ PTAGENTRY pOldTagArray)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4426],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetFormattedData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PBLOCKREQENTRY pBlockReq,
    /* [size_is][out] */ wchar_t *szOutput,
    /* [size_is][out] */ wchar_t *szEngUnits,
    /* [size_is][out] */ wchar_t *szQuality)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4490],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_SetDataFormatted( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PBLOCKREQENTRY pBlockReq,
    /* [size_is][in] */ wchar_t *szInput)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4548],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


short EnumerateInputPoints( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppInputTags)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4594],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short EnumerateOutputPoints( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppOutputTags)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4640],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


unsigned long RPC_SetSealData( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ SEALDATA *pSealData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4686],
                  ( unsigned char * )&phContext);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetSealID( 
    /* [in] */ handle_t hBinding,
    /* [in] */ LEVELNAMES *Name,
    /* [out] */ unsigned long *pdwSealID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4734],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EnumerateTranslations( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned short *pbArraySize,
    /* [size_is][size_is][out] */ PFM_TRANSLATIONNAME *ppResult)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4780],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_GetNewTranslation( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bType,
    /* [out] */ PFM_TRANSLATIONNAME pName)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4826],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_DeleteTranslation( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4872],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EditTranslation( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lIndex,
    /* [out] */ PFM_TRANSLATION pTranslation)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4912],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EditTranslationCancel( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4958],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_EditTranslationDone( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lIndex,
    /* [in] */ PFM_TRANSLATION pTranslation)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[4998],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_IsValidTranslation( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lIndex,
    /* [out] */ unsigned char *pfValid)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5044],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_FindTranslationByName( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pszName,
    /* [out] */ long *plIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5090],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned int RPC_CalculateLeakRate( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pszSystem,
    /* [string][in] */ wchar_t *pszPointTag,
    /* [in] */ unsigned short sAnalysisMethod,
    /* [in] */ unsigned short sAnalysisType,
    /* [in] */ long tmStartTime,
    /* [in] */ long tmEndTime,
    /* [out][in] */ LPLEAKANALYSISRESULT lpAnalysisResult)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5136],
                  ( unsigned char * )&hBinding);
    return ( unsigned int  )_RetVal.Simple;
    
}


short EnumerateRealTimeLeakTanks( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppTankTags)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5212],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


unsigned long SetLeakRateValue( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned short wPointIndex,
    /* [in] */ double dLeakRate,
    /* [in] */ unsigned char bNotEnoughDataAlarm)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5258],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned int RPC_DeleteReportData( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pszLeakRecordID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5310],
                  ( unsigned char * )&hBinding);
    return ( unsigned int  )_RetVal.Simple;
    
}


unsigned long RPC_GetRawData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PBLOCKREQENTRY pBlockReq,
    /* [size_is][out] */ wchar_t *szOutput,
    /* [size_is][out] */ wchar_t *szEngUnits,
    /* [size_is][out] */ wchar_t *szQuality)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5350],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_PostGroupData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwNumberBlockReq,
    /* [in] */ unsigned long dwStatusOffset,
    /* [size_is][in] */ PBLOCKREQENTRY pBlockReq,
    /* [out] */ unsigned long *pdwSize,
    /* [size_is][size_is][out] */ unsigned char **pbData)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5408],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


unsigned long RPC_OperatorCommandEX( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [string][in] */ wchar_t *pPointDescription,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short bVarType,
    /* [in] */ unsigned char bFormat,
    /* [in] */ unsigned char bOverride,
    /* [in] */ PFMDATA512 pDataIn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5472],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


short EditFlowMeterStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PFLOWMETERPOINT pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5554],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short EditFlowMeterCancel( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5624],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short EditFlowMeterDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PFLOWMETERPOINT pNewPoint)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5690],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short EditPipeLineStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PPIPELINEPNT pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5762],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short EditPipeLineCancel( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5832],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short EditPipeLineDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PPIPELINEPNT pNewPoint)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5898],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short EditLogicPntStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PLOGICPNT pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[5970],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short EditLogicPntCancel( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[6040],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short EditLogicPntDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PLOGICPNT pNewPoint)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[6106],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short EditTimerPntStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PTIMERPNT pReturn)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[6178],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short EditTimerPntCancel( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[6248],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short EditTimerPntDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PTIMERPNT pNewPoint)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[6314],
                  ( unsigned char * )&pphContext);
    return ( short  )_RetVal.Simple;
    
}


short EnumerateMeterPoints( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PFM_DESC_STRUCT *stDescription,
    /* [size_is][size_is][out] */ unsigned char **ppMeterTypes,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppMeterTags)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[6386],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


short EnumeratePipelinePoints( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PFM_DESC_STRUCT *stDescription,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppTankTags)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[6450],
                  ( unsigned char * )&hBinding);
    return ( short  )_RetVal.Simple;
    
}


unsigned long RPC_UpdateAlarmComment( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pComment,
    /* [in] */ unsigned long dwAlarmID,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned char bOffSet)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[6508],
                  ( unsigned char * )&hBinding);
    return ( unsigned long  )_RetVal.Simple;
    
}


short RPC_IsPointInActiveMovement( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ unsigned long dwPointID)
{

    CLIENT_CALL_RETURN _RetVal;

    _RetVal = NdrClientCall2(
                  ( PMIDL_STUB_DESC  )&dmlink_StubDesc,
                  (PFORMAT_STRING) &dmlink__MIDL_ProcFormatString.Format[6572],
                  ( unsigned char * )&phContext);
    return ( short  )_RetVal.Simple;
    
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


static const dmlink_MIDL_PROC_FORMAT_STRING dmlink__MIDL_ProcFormatString =
    {
        0,
        {

	/* Procedure ValidateDMStatus */

			0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/*  2 */	NdrFcLong( 0x0 ),	/* 0 */
/*  6 */	NdrFcShort( 0x0 ),	/* 0 */
/*  8 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 10 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 12 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 14 */	NdrFcShort( 0x0 ),	/* 0 */
/* 16 */	NdrFcShort( 0x8 ),	/* 8 */
/* 18 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x1,		/* 1 */
/* 20 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 22 */	NdrFcShort( 0x0 ),	/* 0 */
/* 24 */	NdrFcShort( 0x0 ),	/* 0 */
/* 26 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 28 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 30 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 32 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure Get_DM_Parameters */


	/* Return value */

/* 34 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 36 */	NdrFcLong( 0x0 ),	/* 0 */
/* 40 */	NdrFcShort( 0x1 ),	/* 1 */
/* 42 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 44 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 46 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 48 */	NdrFcShort( 0x0 ),	/* 0 */
/* 50 */	NdrFcShort( 0x38 ),	/* 56 */
/* 52 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 54 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 56 */	NdrFcShort( 0x0 ),	/* 0 */
/* 58 */	NdrFcShort( 0x0 ),	/* 0 */
/* 60 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 62 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 64 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 66 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter blevels */

/* 68 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 70 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 72 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pID */

/* 74 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 76 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 78 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Procedure GetLevelTags */


	/* Return value */

/* 80 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 82 */	NdrFcLong( 0x0 ),	/* 0 */
/* 86 */	NdrFcShort( 0x2 ),	/* 2 */
/* 88 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 90 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 92 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 94 */	NdrFcShort( 0xe9 ),	/* 233 */
/* 96 */	NdrFcShort( 0x20 ),	/* 32 */
/* 98 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x6,		/* 6 */
/* 100 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 102 */	NdrFcShort( 0x2 ),	/* 2 */
/* 104 */	NdrFcShort( 0x0 ),	/* 0 */
/* 106 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 108 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 110 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 112 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 114 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 116 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 118 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bType */

/* 120 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 122 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 124 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pwArraySize */

/* 126 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 128 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 130 */	NdrFcShort( 0x28 ),	/* Type Offset=40 */

	/* Parameter ppTagName */

/* 132 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 134 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 136 */	NdrFcShort( 0x4a ),	/* Type Offset=74 */

	/* Parameter ppTagExtend */

/* 138 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 140 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 142 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure GetPointStrings */


	/* Return value */

/* 144 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 146 */	NdrFcLong( 0x0 ),	/* 0 */
/* 150 */	NdrFcShort( 0x3 ),	/* 3 */
/* 152 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 154 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 156 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 158 */	NdrFcShort( 0x8 ),	/* 8 */
/* 160 */	NdrFcShort( 0xea ),	/* 234 */
/* 162 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 164 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 166 */	NdrFcShort( 0x0 ),	/* 0 */
/* 168 */	NdrFcShort( 0x0 ),	/* 0 */
/* 170 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 172 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 174 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 176 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPointID */

/* 178 */	NdrFcShort( 0x112 ),	/* Flags:  must free, out, simple ref, */
/* 180 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 182 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 184 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 186 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 188 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure GetLevelTagsMasked */


	/* Return value */

/* 190 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 192 */	NdrFcLong( 0x0 ),	/* 0 */
/* 196 */	NdrFcShort( 0x4 ),	/* 4 */
/* 198 */	NdrFcShort( 0x30 ),	/* x86 Stack size/offset = 48 */
/* 200 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 202 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 204 */	NdrFcShort( 0xff ),	/* 255 */
/* 206 */	NdrFcShort( 0x20 ),	/* 32 */
/* 208 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0xb,		/* 11 */
/* 210 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 212 */	NdrFcShort( 0x2 ),	/* 2 */
/* 214 */	NdrFcShort( 0x0 ),	/* 0 */
/* 216 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 218 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 220 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 222 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 224 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 226 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 228 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wFormat */

/* 230 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 232 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 234 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wUnits */

/* 236 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 238 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 240 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bPointType */

/* 242 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 244 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 246 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bVariable */

/* 248 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 250 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 252 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bSearchMode */

/* 254 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 256 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 258 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 260 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 262 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 264 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pwArraySize */

/* 266 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 268 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 270 */	NdrFcShort( 0x6e ),	/* Type Offset=110 */

	/* Parameter ppTagName */

/* 272 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 274 */	NdrFcShort( 0x28 ),	/* x86 Stack size/offset = 40 */
/* 276 */	NdrFcShort( 0x86 ),	/* Type Offset=134 */

	/* Parameter ppTagExtend */

/* 278 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 280 */	NdrFcShort( 0x2c ),	/* x86 Stack size/offset = 44 */
/* 282 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure AddDatabasePoint */


	/* Return value */

/* 284 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 286 */	NdrFcLong( 0x0 ),	/* 0 */
/* 290 */	NdrFcShort( 0x5 ),	/* 5 */
/* 292 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 294 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 296 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 298 */	NdrFcShort( 0xe9 ),	/* 233 */
/* 300 */	NdrFcShort( 0x6 ),	/* 6 */
/* 302 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 304 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 306 */	NdrFcShort( 0x0 ),	/* 0 */
/* 308 */	NdrFcShort( 0x0 ),	/* 0 */
/* 310 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 312 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 314 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 316 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 318 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 320 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 322 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 324 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 326 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 328 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 330 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 332 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 334 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bType */

/* 336 */	NdrFcShort( 0x4113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=16 */
/* 338 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 340 */	NdrFcShort( 0xa8 ),	/* Type Offset=168 */

	/* Parameter pReturn */

/* 342 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 344 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 346 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure GetPointItems */


	/* Return value */

/* 348 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 350 */	NdrFcLong( 0x0 ),	/* 0 */
/* 354 */	NdrFcShort( 0x6 ),	/* 6 */
/* 356 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 358 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 360 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 362 */	NdrFcShort( 0xe4 ),	/* 228 */
/* 364 */	NdrFcShort( 0x6 ),	/* 6 */
/* 366 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 368 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 370 */	NdrFcShort( 0x0 ),	/* 0 */
/* 372 */	NdrFcShort( 0x0 ),	/* 0 */
/* 374 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 376 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 378 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 380 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 382 */	NdrFcShort( 0x4113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=16 */
/* 384 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 386 */	NdrFcShort( 0xa8 ),	/* Type Offset=168 */

	/* Parameter pReturn */

/* 388 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 390 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 392 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure GetPointVariable */


	/* Return value */

/* 394 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 396 */	NdrFcLong( 0x0 ),	/* 0 */
/* 400 */	NdrFcShort( 0x7 ),	/* 7 */
/* 402 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 404 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 406 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 408 */	NdrFcShort( 0x19 ),	/* 25 */
/* 410 */	NdrFcShort( 0x8 ),	/* 8 */
/* 412 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x6,		/* 6 */
/* 414 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 416 */	NdrFcShort( 0x0 ),	/* 0 */
/* 418 */	NdrFcShort( 0x0 ),	/* 0 */
/* 420 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 422 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 424 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 426 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPointID */

/* 428 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 430 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 432 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wPointIndex */

/* 434 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 436 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 438 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bPointType */

/* 440 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 442 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 444 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter bVariable */

/* 446 */	NdrFcShort( 0x6113 ),	/* Flags:  must size, must free, out, simple ref, srv alloc size=24 */
/* 448 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 450 */	NdrFcShort( 0xbe ),	/* Type Offset=190 */

	/* Parameter pReturn */

/* 452 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 454 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 456 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure EditDatabaseStart */


	/* Return value */

/* 458 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 460 */	NdrFcLong( 0x0 ),	/* 0 */
/* 464 */	NdrFcShort( 0x8 ),	/* 8 */
/* 466 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 468 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 470 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 472 */	NdrFcShort( 0xe9 ),	/* 233 */
/* 474 */	NdrFcShort( 0x3e ),	/* 62 */
/* 476 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 478 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 480 */	NdrFcShort( 0x16 ),	/* 22 */
/* 482 */	NdrFcShort( 0x0 ),	/* 0 */
/* 484 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 486 */	NdrFcShort( 0x110 ),	/* Flags:  out, simple ref, */
/* 488 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 490 */	NdrFcShort( 0xd6 ),	/* Type Offset=214 */

	/* Parameter pphContext */

/* 492 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 494 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 496 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 498 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 500 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 502 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 504 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 506 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 508 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 510 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 512 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 514 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bType */

/* 516 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 518 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 520 */	NdrFcShort( 0x1a8 ),	/* Type Offset=424 */

	/* Parameter pReturn */

/* 522 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 524 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 526 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditDatabaseCancel */


	/* Return value */

/* 528 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 530 */	NdrFcLong( 0x0 ),	/* 0 */
/* 534 */	NdrFcShort( 0x9 ),	/* 9 */
/* 536 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 538 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 540 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 542 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 544 */	NdrFcShort( 0x4d ),	/* 77 */
/* 546 */	NdrFcShort( 0x3e ),	/* 62 */
/* 548 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 550 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 552 */	NdrFcShort( 0x0 ),	/* 0 */
/* 554 */	NdrFcShort( 0x0 ),	/* 0 */
/* 556 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 558 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 560 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 562 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 564 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 566 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 568 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 570 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 572 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 574 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 576 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 578 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 580 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 582 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 584 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 586 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bType */

/* 588 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 590 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 592 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Return value */

/* 594 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 596 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 598 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditDatabaseDone */

/* 600 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 602 */	NdrFcLong( 0x0 ),	/* 0 */
/* 606 */	NdrFcShort( 0xa ),	/* 10 */
/* 608 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 610 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 612 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 614 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 616 */	NdrFcShort( 0x48 ),	/* 72 */
/* 618 */	NdrFcShort( 0x3e ),	/* 62 */
/* 620 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 622 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 624 */	NdrFcShort( 0x0 ),	/* 0 */
/* 626 */	NdrFcShort( 0x17 ),	/* 23 */
/* 628 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 630 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 632 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 634 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 636 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 638 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 640 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 642 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 644 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 646 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 648 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 650 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 652 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 654 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 656 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 658 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pNewPoint */

/* 660 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 662 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 664 */	NdrFcShort( 0x1a8 ),	/* Type Offset=424 */

	/* Return value */

/* 666 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 668 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 670 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditTankDataStart */

/* 672 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 674 */	NdrFcLong( 0x0 ),	/* 0 */
/* 678 */	NdrFcShort( 0xb ),	/* 11 */
/* 680 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 682 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 684 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 686 */	NdrFcShort( 0xe9 ),	/* 233 */
/* 688 */	NdrFcShort( 0x3e ),	/* 62 */
/* 690 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 692 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 694 */	NdrFcShort( 0x1 ),	/* 1 */
/* 696 */	NdrFcShort( 0x0 ),	/* 0 */
/* 698 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 700 */	NdrFcShort( 0x110 ),	/* Flags:  out, simple ref, */
/* 702 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 704 */	NdrFcShort( 0xd6 ),	/* Type Offset=214 */

	/* Parameter pphContext */

/* 706 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 708 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 710 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 712 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 714 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 716 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 718 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 720 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 722 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 724 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 726 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 728 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bType */

/* 730 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 732 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 734 */	NdrFcShort( 0x3b6 ),	/* Type Offset=950 */

	/* Parameter pReturn */

/* 736 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 738 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 740 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditTankDataDone */


	/* Return value */

/* 742 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 744 */	NdrFcLong( 0x0 ),	/* 0 */
/* 748 */	NdrFcShort( 0xc ),	/* 12 */
/* 750 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 752 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 754 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 756 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 758 */	NdrFcShort( 0x48 ),	/* 72 */
/* 760 */	NdrFcShort( 0x3e ),	/* 62 */
/* 762 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 764 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 766 */	NdrFcShort( 0x0 ),	/* 0 */
/* 768 */	NdrFcShort( 0x1 ),	/* 1 */
/* 770 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 772 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 774 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 776 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 778 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 780 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 782 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 784 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 786 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 788 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 790 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 792 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 794 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 796 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 798 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 800 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pNewPoint */

/* 802 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 804 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 806 */	NdrFcShort( 0x3b6 ),	/* Type Offset=950 */

	/* Return value */

/* 808 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 810 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 812 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure PurgeDataPoint */

/* 814 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 816 */	NdrFcLong( 0x0 ),	/* 0 */
/* 820 */	NdrFcShort( 0xd ),	/* 13 */
/* 822 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 824 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 826 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 828 */	NdrFcShort( 0xec ),	/* 236 */
/* 830 */	NdrFcShort( 0x6 ),	/* 6 */
/* 832 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 834 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 836 */	NdrFcShort( 0x0 ),	/* 0 */
/* 838 */	NdrFcShort( 0x0 ),	/* 0 */
/* 840 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 842 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 844 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 846 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 848 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 850 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 852 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 854 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 856 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 858 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 860 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 862 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 864 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPntID */

/* 866 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 868 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 870 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure QueryLockoutUser */


	/* Return value */

/* 872 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 874 */	NdrFcLong( 0x0 ),	/* 0 */
/* 878 */	NdrFcShort( 0xe ),	/* 14 */
/* 880 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 882 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 884 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 886 */	NdrFcShort( 0x1c ),	/* 28 */
/* 888 */	NdrFcShort( 0x6 ),	/* 6 */
/* 890 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x7,		/* 7 */
/* 892 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 894 */	NdrFcShort( 0x2 ),	/* 2 */
/* 896 */	NdrFcShort( 0x0 ),	/* 0 */
/* 898 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 900 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 902 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 904 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lAction */

/* 906 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 908 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 910 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPntID */

/* 912 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 914 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 916 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter sNameLen */

/* 918 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 920 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 922 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter sSysLen */

/* 924 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 926 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 928 */	NdrFcShort( 0x418 ),	/* Type Offset=1048 */

	/* Parameter pUserName */

/* 930 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 932 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 934 */	NdrFcShort( 0x424 ),	/* Type Offset=1060 */

	/* Parameter pSystem */

/* 936 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 938 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 940 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure RPC_SaveDatabase */


	/* Return value */

/* 942 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 944 */	NdrFcLong( 0x0 ),	/* 0 */
/* 948 */	NdrFcShort( 0xf ),	/* 15 */
/* 950 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 952 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 954 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 956 */	NdrFcShort( 0x0 ),	/* 0 */
/* 958 */	NdrFcShort( 0x8 ),	/* 8 */
/* 960 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 962 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 964 */	NdrFcShort( 0x0 ),	/* 0 */
/* 966 */	NdrFcShort( 0x0 ),	/* 0 */
/* 968 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 970 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 972 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 974 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 976 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 978 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 980 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_ValidateBlock */


	/* Return value */

/* 982 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 984 */	NdrFcLong( 0x0 ),	/* 0 */
/* 988 */	NdrFcShort( 0x10 ),	/* 16 */
/* 990 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 992 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 994 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 996 */	NdrFcShort( 0x8 ),	/* 8 */
/* 998 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1000 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x4,		/* 4 */
/* 1002 */	0x8,		/* 8 */
			0x7,		/* Ext Flags:  new corr desc, clt corr check, srv corr check, */
/* 1004 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1006 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1008 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1010 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1012 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1014 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lsize */

/* 1016 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1018 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1020 */	NdrFcShort( 0x442 ),	/* Type Offset=1090 */

	/* Parameter pBlock */

/* 1022 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 1024 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1026 */	NdrFcShort( 0x452 ),	/* Type Offset=1106 */

	/* Parameter pResult */

/* 1028 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1030 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1032 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_CommandData */


	/* Return value */

/* 1034 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1036 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1040 */	NdrFcShort( 0x11 ),	/* 17 */
/* 1042 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1044 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1046 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1048 */	NdrFcShort( 0xe ),	/* 14 */
/* 1050 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1052 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 1054 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1056 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1058 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1060 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1062 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1064 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1066 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 1068 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1070 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1072 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPointId */

/* 1074 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1076 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1078 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter bVarType */

/* 1080 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 1082 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1084 */	NdrFcShort( 0x468 ),	/* Type Offset=1128 */

	/* Parameter pOutput */

/* 1086 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1088 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1090 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_OperatorCommand */


	/* Return value */

/* 1092 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1094 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1098 */	NdrFcShort( 0x12 ),	/* 18 */
/* 1100 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 1102 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1104 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1106 */	NdrFcShort( 0x23a ),	/* 570 */
/* 1108 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1110 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x8,		/* 8 */
/* 1112 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1114 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1116 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1118 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1120 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1122 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1124 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 1126 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1128 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1130 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 1132 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1134 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1136 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPointID */

/* 1138 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1140 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1142 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter bVarType */

/* 1144 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1146 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1148 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bFormat */

/* 1150 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1152 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1154 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bOverride */

/* 1156 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 1158 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 1160 */	NdrFcShort( 0x174 ),	/* Type Offset=372 */

	/* Parameter pDataIn */

/* 1162 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1164 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 1166 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_AllocateSource */


	/* Return value */

/* 1168 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1170 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1174 */	NdrFcShort( 0x13 ),	/* 19 */
/* 1176 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 1178 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1180 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1182 */	NdrFcShort( 0xd ),	/* 13 */
/* 1184 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1186 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 1188 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1190 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1192 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1194 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1196 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1198 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1200 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 1202 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1204 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1206 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 1208 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1210 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1212 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bAllocate */

/* 1214 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1216 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1218 */	NdrFcShort( 0x49c ),	/* Type Offset=1180 */

	/* Parameter pInput */

/* 1220 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1222 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1224 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter nSourceIndex */

/* 1226 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1228 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1230 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetSystemString */


	/* Return value */

/* 1232 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1234 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1238 */	NdrFcShort( 0x14 ),	/* 20 */
/* 1240 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1242 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1244 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1246 */	NdrFcShort( 0x5 ),	/* 5 */
/* 1248 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1250 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 1252 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 1254 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1256 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1258 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1260 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1262 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1264 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bSysIndex */

/* 1266 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 1268 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1270 */	NdrFcShort( 0x4ac ),	/* Type Offset=1196 */

	/* Parameter pSysName */

/* 1272 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1274 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1276 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetSystemIndex */


	/* Return value */

/* 1278 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1280 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1284 */	NdrFcShort( 0x15 ),	/* 21 */
/* 1286 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 1288 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1290 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1292 */	NdrFcShort( 0x61 ),	/* 97 */
/* 1294 */	NdrFcShort( 0x21 ),	/* 33 */
/* 1296 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 1298 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1300 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1302 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1304 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1306 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1308 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1310 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 1312 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1314 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1316 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserSystem */

/* 1318 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 1320 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1322 */	NdrFcShort( 0x4c2 ),	/* Type Offset=1218 */

	/* Parameter pSysParam */

/* 1324 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1326 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1328 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter cAdd */

/* 1330 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1332 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1334 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pIndex */

/* 1336 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1338 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1340 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EnumerateVariables */


	/* Return value */

/* 1342 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1344 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1348 */	NdrFcShort( 0x16 ),	/* 22 */
/* 1350 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1352 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1354 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1356 */	NdrFcShort( 0x40 ),	/* 64 */
/* 1358 */	NdrFcShort( 0x22 ),	/* 34 */
/* 1360 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x4,		/* 4 */
/* 1362 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 1364 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1366 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1368 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1370 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 1372 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1374 */	NdrFcShort( 0x4d2 ),	/* Type Offset=1234 */

	/* Parameter pInput */

/* 1376 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1378 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1380 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pbArraySize */

/* 1382 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1384 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1386 */	NdrFcShort( 0x4e0 ),	/* Type Offset=1248 */

	/* Parameter ppResult */

/* 1388 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1390 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1392 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetStrapTable */


	/* Return value */

/* 1394 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1396 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1400 */	NdrFcShort( 0x17 ),	/* 23 */
/* 1402 */	NdrFcShort( 0x3c ),	/* x86 Stack size/offset = 60 */
/* 1404 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 1406 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1408 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 1410 */	NdrFcShort( 0x34 ),	/* 52 */
/* 1412 */	NdrFcShort( 0xa7 ),	/* 167 */
/* 1414 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0xf,		/* 15 */
/* 1416 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 1418 */	NdrFcShort( 0x3 ),	/* 3 */
/* 1420 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1422 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 1424 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 1426 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1428 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter pUserName */

/* 1430 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1432 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1434 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 1436 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1438 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1440 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 1442 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1444 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1446 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 1448 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1450 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1452 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pLvlUnits */

/* 1454 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1456 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1458 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pVolUnits */

/* 1460 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1462 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1464 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pPressUnits */

/* 1466 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1468 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 1470 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pdwSize */

/* 1472 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1474 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 1476 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pOut */

/* 1478 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1480 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 1482 */	NdrFcShort( 0x4fc ),	/* Type Offset=1276 */

	/* Parameter pdwWTSize */

/* 1484 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1486 */	NdrFcShort( 0x28 ),	/* x86 Stack size/offset = 40 */
/* 1488 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pWTOut */

/* 1490 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1492 */	NdrFcShort( 0x2c ),	/* x86 Stack size/offset = 44 */
/* 1494 */	NdrFcShort( 0x51c ),	/* Type Offset=1308 */

	/* Parameter pdwHydroSize */

/* 1496 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1498 */	NdrFcShort( 0x30 ),	/* x86 Stack size/offset = 48 */
/* 1500 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pHydroOut */

/* 1502 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1504 */	NdrFcShort( 0x34 ),	/* x86 Stack size/offset = 52 */
/* 1506 */	NdrFcShort( 0x534 ),	/* Type Offset=1332 */

	/* Return value */

/* 1508 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1510 */	NdrFcShort( 0x38 ),	/* x86 Stack size/offset = 56 */
/* 1512 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_ReplaceStrapTable */

/* 1514 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1516 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1520 */	NdrFcShort( 0x18 ),	/* 24 */
/* 1522 */	NdrFcShort( 0x38 ),	/* x86 Stack size/offset = 56 */
/* 1524 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 1526 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1528 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 1530 */	NdrFcShort( 0x56 ),	/* 86 */
/* 1532 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1534 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0xe,		/* 14 */
/* 1536 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 1538 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1540 */	NdrFcShort( 0x3 ),	/* 3 */
/* 1542 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 1544 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 1546 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1548 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter pUserName */

/* 1550 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1552 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1554 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 1556 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1558 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1560 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 1562 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1564 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1566 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 1568 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1570 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1572 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bIndUnits */

/* 1574 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1576 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1578 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bDepUnits */

/* 1580 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1582 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1584 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter dwSize */

/* 1586 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1588 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 1590 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pNew */

/* 1592 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1594 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 1596 */	NdrFcShort( 0x550 ),	/* Type Offset=1360 */

	/* Parameter dwWTSize */

/* 1598 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1600 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 1602 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pWTNew */

/* 1604 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1606 */	NdrFcShort( 0x28 ),	/* x86 Stack size/offset = 40 */
/* 1608 */	NdrFcShort( 0x564 ),	/* Type Offset=1380 */

	/* Parameter pdwHydroSize */

/* 1610 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1612 */	NdrFcShort( 0x2c ),	/* x86 Stack size/offset = 44 */
/* 1614 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pHydroOut */

/* 1616 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1618 */	NdrFcShort( 0x30 ),	/* x86 Stack size/offset = 48 */
/* 1620 */	NdrFcShort( 0x578 ),	/* Type Offset=1400 */

	/* Return value */

/* 1622 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1624 */	NdrFcShort( 0x34 ),	/* x86 Stack size/offset = 52 */
/* 1626 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure EnumerateTanks */

/* 1628 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1630 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1634 */	NdrFcShort( 0x19 ),	/* 25 */
/* 1636 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1638 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1640 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1642 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1644 */	NdrFcShort( 0x20 ),	/* 32 */
/* 1646 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 1648 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 1650 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1652 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1654 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1656 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1658 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1660 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 1662 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 1664 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1666 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pwArraySize */

/* 1668 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1670 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1672 */	NdrFcShort( 0x588 ),	/* Type Offset=1416 */

	/* Parameter stDescription */

/* 1674 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 1676 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1678 */	NdrFcShort( 0x5aa ),	/* Type Offset=1450 */

	/* Parameter ppTankTags */

/* 1680 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1682 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1684 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure GetTankDetail */


	/* Return value */

/* 1686 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1688 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1692 */	NdrFcShort( 0x1a ),	/* 26 */
/* 1694 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1696 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1698 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1700 */	NdrFcShort( 0xe4 ),	/* 228 */
/* 1702 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1704 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 1706 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1708 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1710 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1712 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1714 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 1716 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1718 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 1720 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 1722 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1724 */	NdrFcShort( 0x5c6 ),	/* Type Offset=1478 */

	/* Parameter pTankDetail */

/* 1726 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1728 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1730 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_AlarmAck */


	/* Return value */

/* 1732 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1734 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1738 */	NdrFcShort( 0x1b ),	/* 27 */
/* 1740 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1742 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1744 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1746 */	NdrFcShort( 0xd ),	/* 13 */
/* 1748 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1750 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x4,		/* 4 */
/* 1752 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1754 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1756 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1758 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1760 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1762 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1764 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 1766 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1768 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1770 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwAlarmID */

/* 1772 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1774 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1776 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bOffSet */

/* 1778 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1780 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1782 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_SilenceAlarm */


	/* Return value */

/* 1784 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1786 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1790 */	NdrFcShort( 0x1c ),	/* 28 */
/* 1792 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1794 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1796 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1798 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1800 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1802 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 1804 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1806 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1808 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1810 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1812 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1814 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1816 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwAlarmAccess */

/* 1818 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1820 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1822 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_SetProduct */


	/* Return value */

/* 1824 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1826 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1830 */	NdrFcShort( 0x1d ),	/* 29 */
/* 1832 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1834 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 1836 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1838 */	NdrFcShort( 0x18 ),	/* 24 */
/* 1840 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1842 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 1844 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1846 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1848 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1850 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 1852 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1854 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1856 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 1858 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1860 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1862 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwTankID */

/* 1864 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1866 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1868 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwTankIndex */

/* 1870 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1872 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1874 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwProductId */

/* 1876 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1878 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1880 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EditPointAlarm */


	/* Return value */

/* 1882 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1884 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1888 */	NdrFcShort( 0x1e ),	/* 30 */
/* 1890 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 1892 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 1894 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1896 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 1898 */	NdrFcShort( 0x31 ),	/* 49 */
/* 1900 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1902 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 1904 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1906 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1908 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1910 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 1912 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 1914 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1916 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter pUserName */

/* 1918 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1920 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1922 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 1924 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1926 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1928 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 1930 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1932 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1934 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bVariable */

/* 1936 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 1938 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1940 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pAlarmData */

/* 1942 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 1944 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 1946 */	NdrFcShort( 0x63a ),	/* Type Offset=1594 */

	/* Return value */

/* 1948 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 1950 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 1952 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EditPointAlarmDone */

/* 1954 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 1956 */	NdrFcLong( 0x0 ),	/* 0 */
/* 1960 */	NdrFcShort( 0x1f ),	/* 31 */
/* 1962 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 1964 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 1966 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1968 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 1970 */	NdrFcShort( 0x31 ),	/* 49 */
/* 1972 */	NdrFcShort( 0x8 ),	/* 8 */
/* 1974 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 1976 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 1978 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1980 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1982 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 1984 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 1986 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 1988 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter pUserName */

/* 1990 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1992 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1994 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 1996 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 1998 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2000 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 2002 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2004 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2006 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bVariable */

/* 2008 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2010 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2012 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pNewAlarm */

/* 2014 */	NdrFcShort( 0x11b ),	/* Flags:  must size, must free, in, out, simple ref, */
/* 2016 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2018 */	NdrFcShort( 0x63a ),	/* Type Offset=1594 */

	/* Return value */

/* 2020 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2022 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2024 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_DeletePointAlarm */

/* 2026 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2028 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2032 */	NdrFcShort( 0x20 ),	/* 32 */
/* 2034 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2036 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 2038 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2040 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 2042 */	NdrFcShort( 0x31 ),	/* 49 */
/* 2044 */	NdrFcShort( 0x8 ),	/* 8 */
/* 2046 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 2048 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2050 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2052 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2054 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 2056 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 2058 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2060 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter pUserName */

/* 2062 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2064 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2066 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 2068 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2070 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2072 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 2074 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2076 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2078 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bVariable */

/* 2080 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2082 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2084 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Return value */

/* 2086 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2088 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2090 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EnumGlobalAlarms */

/* 2092 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2094 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2098 */	NdrFcShort( 0x21 ),	/* 33 */
/* 2100 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 2102 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2104 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2106 */	NdrFcShort( 0xa ),	/* 10 */
/* 2108 */	NdrFcShort( 0x3e ),	/* 62 */
/* 2110 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x6,		/* 6 */
/* 2112 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 2114 */	NdrFcShort( 0x1 ),	/* 1 */
/* 2116 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2118 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2120 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2122 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2124 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bTemplate */

/* 2126 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2128 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2130 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bDataType */

/* 2132 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 2134 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2136 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter plAccessTime */

/* 2138 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 2140 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2142 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pOutNum */

/* 2144 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 2146 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2148 */	NdrFcShort( 0x650 ),	/* Type Offset=1616 */

	/* Parameter ppGlobalAlarmIndex */

/* 2150 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2152 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2154 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EditGlobalAlarm */


	/* Return value */

/* 2156 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2158 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2162 */	NdrFcShort( 0x22 ),	/* 34 */
/* 2164 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 2166 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2168 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2170 */	NdrFcShort( 0x10 ),	/* 16 */
/* 2172 */	NdrFcShort( 0x6704 ),	/* 26372 */
/* 2174 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x8,		/* 8 */
/* 2176 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2178 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2180 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2182 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2184 */	NdrFcShort( 0x110 ),	/* Flags:  out, simple ref, */
/* 2186 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2188 */	NdrFcShort( 0xd6 ),	/* Type Offset=214 */

	/* Parameter pphContext */

/* 2190 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2192 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2194 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 2196 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2198 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2200 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 2202 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2204 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2206 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwAlarmId */

/* 2208 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2210 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2212 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lAccessTime */

/* 2214 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2216 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2218 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pAlarmName */

/* 2220 */	NdrFcShort( 0x112 ),	/* Flags:  must free, out, simple ref, */
/* 2222 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 2224 */	NdrFcShort( 0x67c ),	/* Type Offset=1660 */

	/* Parameter pAlarmData */

/* 2226 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2228 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 2230 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EditGlobalAlarmDone */


	/* Return value */

/* 2232 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2234 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2238 */	NdrFcShort( 0x23 ),	/* 35 */
/* 2240 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2242 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 2244 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2246 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 2248 */	NdrFcShort( 0x6704 ),	/* 26372 */
/* 2250 */	NdrFcShort( 0x40 ),	/* 64 */
/* 2252 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 2254 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2256 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2258 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2260 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 2262 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 2264 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2266 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 2268 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2270 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2272 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 2274 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2276 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2278 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwAlarmId */

/* 2280 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2282 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2284 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pNewAlarm */

/* 2286 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 2288 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2290 */	NdrFcShort( 0x67c ),	/* Type Offset=1660 */

	/* Return value */

/* 2292 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2294 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2296 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_AddGlobalAlarm */

/* 2298 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2300 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2304 */	NdrFcShort( 0x24 ),	/* 36 */
/* 2306 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2308 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2310 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2312 */	NdrFcShort( 0x66c4 ),	/* 26308 */
/* 2314 */	NdrFcShort( 0x24 ),	/* 36 */
/* 2316 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 2318 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2320 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2322 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2324 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2326 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2328 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2330 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 2332 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2334 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2336 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 2338 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 2340 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2342 */	NdrFcShort( 0x67c ),	/* Type Offset=1660 */

	/* Parameter pNewAlarm */

/* 2344 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 2346 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2348 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pDwAlarmId */

/* 2350 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2352 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2354 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_DeleteGlobalAlarm */


	/* Return value */

/* 2356 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2358 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2362 */	NdrFcShort( 0x25 ),	/* 37 */
/* 2364 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2366 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2368 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2370 */	NdrFcShort( 0x10 ),	/* 16 */
/* 2372 */	NdrFcShort( 0x8 ),	/* 8 */
/* 2374 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 2376 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2378 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2380 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2382 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2384 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2386 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2388 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 2390 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2392 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2394 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwAlarmId */

/* 2396 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2398 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2400 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lAccessTime */

/* 2402 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2404 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2406 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pAlarmName */

/* 2408 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2410 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2412 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_CancelAlarmEdit */


	/* Return value */

/* 2414 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2416 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2420 */	NdrFcShort( 0x26 ),	/* 38 */
/* 2422 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2424 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 2426 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2428 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 2430 */	NdrFcShort( 0x40 ),	/* 64 */
/* 2432 */	NdrFcShort( 0x40 ),	/* 64 */
/* 2434 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 2436 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2438 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2440 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2442 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 2444 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 2446 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2448 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 2450 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2452 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2454 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 2456 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2458 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2460 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwAlarmID */

/* 2462 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2464 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2466 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 2468 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2470 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2472 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetGlobalAlarmData */

/* 2474 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2476 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2480 */	NdrFcShort( 0x27 ),	/* 39 */
/* 2482 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2484 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2486 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2488 */	NdrFcShort( 0x10 ),	/* 16 */
/* 2490 */	NdrFcShort( 0x66cc ),	/* 26316 */
/* 2492 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 2494 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2496 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2498 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2500 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2502 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2504 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2506 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwAlarmdef */

/* 2508 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2510 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2512 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lAccessTime */

/* 2514 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2516 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2518 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pAlarmName */

/* 2520 */	NdrFcShort( 0x112 ),	/* Flags:  must free, out, simple ref, */
/* 2522 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2524 */	NdrFcShort( 0x67c ),	/* Type Offset=1660 */

	/* Parameter pReturn */

/* 2526 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2528 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2530 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetGlobalAlarmName */


	/* Return value */

/* 2532 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2534 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2538 */	NdrFcShort( 0x28 ),	/* 40 */
/* 2540 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2542 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2544 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2546 */	NdrFcShort( 0x8 ),	/* 8 */
/* 2548 */	NdrFcShort( 0x8 ),	/* 8 */
/* 2550 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 2552 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 2554 */	NdrFcShort( 0x1 ),	/* 1 */
/* 2556 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2558 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2560 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2562 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2564 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwAlarmDef */

/* 2566 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 2568 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2570 */	NdrFcShort( 0x68e ),	/* Type Offset=1678 */

	/* Parameter pName */

/* 2572 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2574 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2576 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetExtendedAlarmData */


	/* Return value */

/* 2578 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2580 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2584 */	NdrFcShort( 0x29 ),	/* 41 */
/* 2586 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 2588 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2590 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2592 */	NdrFcShort( 0x8 ),	/* 8 */
/* 2594 */	NdrFcShort( 0x55 ),	/* 85 */
/* 2596 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x7,		/* 7 */
/* 2598 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 2600 */	NdrFcShort( 0x2 ),	/* 2 */
/* 2602 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2604 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2606 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2608 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2610 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwAlarmID */

/* 2612 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 2614 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2616 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pGraphic */

/* 2618 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 2620 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2622 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pHelp */

/* 2624 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 2626 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2628 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bAutoLoad */

/* 2630 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 2632 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2634 */	NdrFcShort( 0x69a ),	/* Type Offset=1690 */

	/* Parameter szGraphic */

/* 2636 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 2638 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2640 */	NdrFcShort( 0x6a6 ),	/* Type Offset=1702 */

	/* Parameter szTemplate */

/* 2642 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2644 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 2646 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_AllocateTankCalc */


	/* Return value */

/* 2648 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2650 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2654 */	NdrFcShort( 0x2a ),	/* 42 */
/* 2656 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 2658 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2660 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2662 */	NdrFcShort( 0xe4 ),	/* 228 */
/* 2664 */	NdrFcShort( 0x5c ),	/* 92 */
/* 2666 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 2668 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2670 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2672 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2674 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2676 */	NdrFcShort( 0x110 ),	/* Flags:  out, simple ref, */
/* 2678 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2680 */	NdrFcShort( 0xd6 ),	/* Type Offset=214 */

	/* Parameter pphContext */

/* 2682 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2684 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2686 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 2688 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2690 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2692 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 2694 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 2696 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2698 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter pTankName */

/* 2700 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 2702 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2704 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pCalcTank */

/* 2706 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 2708 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2710 */	NdrFcShort( 0x6bc ),	/* Type Offset=1724 */

	/* Parameter pSet */

/* 2712 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2714 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 2716 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_QuitTankCalc */


	/* Return value */

/* 2718 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2720 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2724 */	NdrFcShort( 0x2b ),	/* 43 */
/* 2726 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2728 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 2730 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2732 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 2734 */	NdrFcShort( 0x40 ),	/* 64 */
/* 2736 */	NdrFcShort( 0x40 ),	/* 64 */
/* 2738 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 2740 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2742 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2744 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2746 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 2748 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 2750 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2752 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 2754 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2756 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2758 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 2760 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 2762 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2764 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwTankIndex */

/* 2766 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2768 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2770 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 2772 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2774 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2776 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_DoTankCalculate */

/* 2778 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2780 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2784 */	NdrFcShort( 0x2c ),	/* 44 */
/* 2786 */	NdrFcShort( 0x8c ),	/* x86 Stack size/offset = 140 */
/* 2788 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 2790 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2792 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 2794 */	NdrFcShort( 0x11e ),	/* 286 */
/* 2796 */	NdrFcShort( 0x16c ),	/* 364 */
/* 2798 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x15,		/* 21 */
/* 2800 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 2802 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2804 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2806 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 2808 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 2810 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2812 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter dLevel */

/* 2814 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2816 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2818 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dTemp */

/* 2820 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2822 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2824 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dStdDens */

/* 2826 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2828 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2830 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dH2OLev */

/* 2832 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2834 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 2836 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dBSW */

/* 2838 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2840 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 2842 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dSolidLev */

/* 2844 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2846 */	NdrFcShort( 0x2c ),	/* x86 Stack size/offset = 44 */
/* 2848 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dAmbientTemp */

/* 2850 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2852 */	NdrFcShort( 0x34 ),	/* x86 Stack size/offset = 52 */
/* 2854 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dVaporTemp */

/* 2856 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2858 */	NdrFcShort( 0x3c ),	/* x86 Stack size/offset = 60 */
/* 2860 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dVaporPress */

/* 2862 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2864 */	NdrFcShort( 0x44 ),	/* x86 Stack size/offset = 68 */
/* 2866 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dCorrectionVolume */

/* 2868 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2870 */	NdrFcShort( 0x4c ),	/* x86 Stack size/offset = 76 */
/* 2872 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dGasDensity */

/* 2874 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2876 */	NdrFcShort( 0x54 ),	/* x86 Stack size/offset = 84 */
/* 2878 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dXfrValue */

/* 2880 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2882 */	NdrFcShort( 0x5c ),	/* x86 Stack size/offset = 92 */
/* 2884 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dDensityTempValue */

/* 2886 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2888 */	NdrFcShort( 0x64 ),	/* x86 Stack size/offset = 100 */
/* 2890 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dGaugeStdDensity */

/* 2892 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2894 */	NdrFcShort( 0x6c ),	/* x86 Stack size/offset = 108 */
/* 2896 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dwTankIndex */

/* 2898 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2900 */	NdrFcShort( 0x74 ),	/* x86 Stack size/offset = 116 */
/* 2902 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwXfrBaseMode */

/* 2904 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2906 */	NdrFcShort( 0x78 ),	/* x86 Stack size/offset = 120 */
/* 2908 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bNewInput */

/* 2910 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2912 */	NdrFcShort( 0x7c ),	/* x86 Stack size/offset = 124 */
/* 2914 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bUseMeasuredDensity */

/* 2916 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 2918 */	NdrFcShort( 0x80 ),	/* x86 Stack size/offset = 128 */
/* 2920 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pReturn */

/* 2922 */	NdrFcShort( 0x112 ),	/* Flags:  must free, out, simple ref, */
/* 2924 */	NdrFcShort( 0x84 ),	/* x86 Stack size/offset = 132 */
/* 2926 */	NdrFcShort( 0x70a ),	/* Type Offset=1802 */

	/* Return value */

/* 2928 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2930 */	NdrFcShort( 0x88 ),	/* x86 Stack size/offset = 136 */
/* 2932 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_SearchTags */

/* 2934 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2936 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2940 */	NdrFcShort( 0x2d ),	/* 45 */
/* 2942 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 2944 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 2946 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 2948 */	NdrFcShort( 0x30c ),	/* 780 */
/* 2950 */	NdrFcShort( 0x22 ),	/* 34 */
/* 2952 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x4,		/* 4 */
/* 2954 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 2956 */	NdrFcShort( 0x1 ),	/* 1 */
/* 2958 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2960 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 2962 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 2964 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2966 */	NdrFcShort( 0x73c ),	/* Type Offset=1852 */

	/* Parameter pFilter */

/* 2968 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 2970 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 2972 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pFound */

/* 2974 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 2976 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 2978 */	NdrFcShort( 0x74e ),	/* Type Offset=1870 */

	/* Parameter ppPointSpec */

/* 2980 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 2982 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2984 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EditTankAlarm */


	/* Return value */

/* 2986 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 2988 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2992 */	NdrFcShort( 0x2e ),	/* 46 */
/* 2994 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2996 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 2998 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3000 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 3002 */	NdrFcShort( 0x2c ),	/* 44 */
/* 3004 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3006 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 3008 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3010 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3012 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3014 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 3016 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 3018 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3020 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter pUserName */

/* 3022 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 3024 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3026 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 3028 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 3030 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3032 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 3034 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3036 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3038 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pTankAlarm */

/* 3040 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 3042 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3044 */	NdrFcShort( 0x770 ),	/* Type Offset=1904 */

	/* Return value */

/* 3046 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3048 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 3050 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EditTankAlarmDone */

/* 3052 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3054 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3058 */	NdrFcShort( 0x2f ),	/* 47 */
/* 3060 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 3062 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 3064 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3066 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 3068 */	NdrFcShort( 0x31 ),	/* 49 */
/* 3070 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3072 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 3074 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3076 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3078 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3080 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 3082 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 3084 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3086 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter pUserName */

/* 3088 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 3090 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3092 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 3094 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 3096 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3098 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 3100 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3102 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3104 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bOutageTank */

/* 3106 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3108 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3110 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pNewAlarm */

/* 3112 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 3114 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 3116 */	NdrFcShort( 0x770 ),	/* Type Offset=1904 */

	/* Return value */

/* 3118 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3120 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 3122 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetDeviceSource */

/* 3124 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3126 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3130 */	NdrFcShort( 0x30 ),	/* 48 */
/* 3132 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3134 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 3136 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3138 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3140 */	NdrFcShort( 0x6c ),	/* 108 */
/* 3142 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 3144 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3146 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3148 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3150 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 3152 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3154 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3156 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPntID */

/* 3158 */	NdrFcShort( 0xc112 ),	/* Flags:  must free, out, simple ref, srv alloc size=48 */
/* 3160 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3162 */	NdrFcShort( 0x83a ),	/* Type Offset=2106 */

	/* Parameter pGaugeData */

/* 3164 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3166 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3168 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetDeviceSourceEx */


	/* Return value */

/* 3170 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3172 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3176 */	NdrFcShort( 0x31 ),	/* 49 */
/* 3178 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 3180 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 3182 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3184 */	NdrFcShort( 0x13 ),	/* 19 */
/* 3186 */	NdrFcShort( 0x6c ),	/* 108 */
/* 3188 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x5,		/* 5 */
/* 3190 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3192 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3194 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3196 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 3198 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3200 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3202 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPntID */

/* 3204 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3206 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3208 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wSelectedVariable */

/* 3210 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3212 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3214 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bDoNotLockMutex */

/* 3216 */	NdrFcShort( 0xc112 ),	/* Flags:  must free, out, simple ref, srv alloc size=48 */
/* 3218 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3220 */	NdrFcShort( 0x83a ),	/* Type Offset=2106 */

	/* Parameter pGaugeData */

/* 3222 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3224 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 3226 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetSystemIndexData */


	/* Return value */

/* 3228 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3230 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3234 */	NdrFcShort( 0x32 ),	/* 50 */
/* 3236 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3238 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 3240 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3242 */	NdrFcShort( 0x5 ),	/* 5 */
/* 3244 */	NdrFcShort( 0x64 ),	/* 100 */
/* 3246 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 3248 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3250 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3252 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3254 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 3256 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3258 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3260 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bIndex */

/* 3262 */	NdrFcShort( 0xa112 ),	/* Flags:  must free, out, simple ref, srv alloc size=40 */
/* 3264 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3266 */	NdrFcShort( 0x4c2 ),	/* Type Offset=1218 */

	/* Parameter pSysParam */

/* 3268 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3270 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3272 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_AllocPointSource */


	/* Return value */

/* 3274 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3276 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3280 */	NdrFcShort( 0x33 ),	/* 51 */
/* 3282 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 3284 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 3286 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3288 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 3290 */	NdrFcShort( 0x3f ),	/* 63 */
/* 3292 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3294 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x9,		/* 9 */
/* 3296 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 3298 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3300 */	NdrFcShort( 0x1 ),	/* 1 */
/* 3302 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 3304 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 3306 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3308 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter pUserName */

/* 3310 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 3312 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3314 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 3316 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 3318 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3320 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 3322 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3324 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3326 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bVariable */

/* 3328 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3330 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3332 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter bClear */

/* 3334 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3336 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 3338 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pSource */

/* 3340 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 3342 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 3344 */	NdrFcShort( 0x148 ),	/* Type Offset=328 */

	/* Parameter nSourceIndex */

/* 3346 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3348 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 3350 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 3352 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3354 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 3356 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_AddEventRecord */

/* 3358 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3360 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3364 */	NdrFcShort( 0x34 ),	/* 52 */
/* 3366 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 3368 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 3370 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3372 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 3374 */	NdrFcShort( 0x24f ),	/* 591 */
/* 3376 */	NdrFcShort( 0x24 ),	/* 36 */
/* 3378 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x8,		/* 8 */
/* 3380 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3382 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3384 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3386 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 3388 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 3390 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3392 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter bAdvise */

/* 3394 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3396 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3398 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter prDeadBand */

/* 3400 */	NdrFcShort( 0xa ),	/* Flags:  must free, in, */
/* 3402 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3404 */	NdrFcShort( 0x174 ),	/* Type Offset=372 */

	/* Parameter dwPointID */

/* 3406 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3408 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3410 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter wDataType */

/* 3412 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3414 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3416 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bCategory */

/* 3418 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3420 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 3422 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter plNewHandle */

/* 3424 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 3426 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 3428 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 3430 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3432 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 3434 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetEventAttributes */

/* 3436 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3438 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3442 */	NdrFcShort( 0x35 ),	/* 53 */
/* 3444 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 3446 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 3448 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3450 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 3452 */	NdrFcShort( 0x2c ),	/* 44 */
/* 3454 */	NdrFcShort( 0x27b ),	/* 635 */
/* 3456 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x7,		/* 7 */
/* 3458 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3460 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3462 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3464 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 3466 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 3468 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3470 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter lHandle */

/* 3472 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3474 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3476 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pwDataType */

/* 3478 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 3480 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3482 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pbAdvise */

/* 3484 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 3486 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3488 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pbCategory */

/* 3490 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 3492 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3494 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter prDeadBand */

/* 3496 */	NdrFcShort( 0x112 ),	/* Flags:  must free, out, simple ref, */
/* 3498 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 3500 */	NdrFcShort( 0x174 ),	/* Type Offset=372 */

	/* Return value */

/* 3502 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3504 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 3506 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_SetEventAttributes */

/* 3508 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3510 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3514 */	NdrFcShort( 0x36 ),	/* 54 */
/* 3516 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 3518 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 3520 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3522 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 3524 */	NdrFcShort( 0x24f ),	/* 591 */
/* 3526 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3528 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x7,		/* 7 */
/* 3530 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3532 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3534 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3536 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 3538 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 3540 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3542 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter lHandle */

/* 3544 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3546 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3548 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter wDataType */

/* 3550 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3552 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3554 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bAdvise */

/* 3556 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3558 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3560 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter bCategory */

/* 3562 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3564 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3566 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter prDeadBand */

/* 3568 */	NdrFcShort( 0xa ),	/* Flags:  must free, in, */
/* 3570 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 3572 */	NdrFcShort( 0x174 ),	/* Type Offset=372 */

	/* Return value */

/* 3574 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3576 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 3578 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_RemoveEventRecord */

/* 3580 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3582 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3586 */	NdrFcShort( 0x37 ),	/* 55 */
/* 3588 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3590 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 3592 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3594 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 3596 */	NdrFcShort( 0x2c ),	/* 44 */
/* 3598 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3600 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 3602 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3604 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3606 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3608 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 3610 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 3612 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3614 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter lHandle */

/* 3616 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3618 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3620 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 3622 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3624 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3626 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_ChangeDataType */

/* 3628 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3630 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3634 */	NdrFcShort( 0x38 ),	/* 56 */
/* 3636 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 3638 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 3640 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3642 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 3644 */	NdrFcShort( 0x3c ),	/* 60 */
/* 3646 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3648 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x5,		/* 5 */
/* 3650 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3652 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3654 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3656 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 3658 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 3660 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3662 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter lHandle */

/* 3664 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3666 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3668 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPointID */

/* 3670 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3672 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3674 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwNewDataType */

/* 3676 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3678 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3680 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 3682 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3684 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3686 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetLogPath */

/* 3688 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3690 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3694 */	NdrFcShort( 0x39 ),	/* 57 */
/* 3696 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3698 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 3700 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3702 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3704 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3706 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 3708 */	0x8,		/* 8 */
			0x7,		/* Ext Flags:  new corr desc, clt corr check, srv corr check, */
/* 3710 */	NdrFcShort( 0x1 ),	/* 1 */
/* 3712 */	NdrFcShort( 0x1 ),	/* 1 */
/* 3714 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 3716 */	NdrFcShort( 0x1b ),	/* Flags:  must size, must free, in, out, */
/* 3718 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3720 */	NdrFcShort( 0x852 ),	/* Type Offset=2130 */

	/* Parameter szUniversalName */

/* 3722 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3724 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3726 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetData */


	/* Return value */

/* 3728 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3730 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3734 */	NdrFcShort( 0x3a ),	/* 58 */
/* 3736 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 3738 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 3740 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3742 */	NdrFcShort( 0x8d ),	/* 141 */
/* 3744 */	NdrFcShort( 0x59 ),	/* 89 */
/* 3746 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 3748 */	0x8,		/* 8 */
			0x7,		/* Ext Flags:  new corr desc, clt corr check, srv corr check, */
/* 3750 */	NdrFcShort( 0x1 ),	/* 1 */
/* 3752 */	NdrFcShort( 0x1 ),	/* 1 */
/* 3754 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 3756 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 3758 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3760 */	NdrFcShort( 0x434 ),	/* Type Offset=1076 */

	/* Parameter pBlockReq */

/* 3762 */	NdrFcShort( 0x158 ),	/* Flags:  in, out, base type, simple ref, */
/* 3764 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3766 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter ulStatus1 */

/* 3768 */	NdrFcShort( 0x158 ),	/* Flags:  in, out, base type, simple ref, */
/* 3770 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3772 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter ulStatus2 */

/* 3774 */	NdrFcShort( 0x158 ),	/* Flags:  in, out, base type, simple ref, */
/* 3776 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3778 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter byChanged */

/* 3780 */	NdrFcShort( 0x11b ),	/* Flags:  must size, must free, in, out, simple ref, */
/* 3782 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 3784 */	NdrFcShort( 0x86e ),	/* Type Offset=2158 */

	/* Parameter pData */

/* 3786 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3788 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 3790 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter ulDataSize */

/* 3792 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3794 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 3796 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_SetData */


	/* Return value */

/* 3798 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3800 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3804 */	NdrFcShort( 0x3b ),	/* 59 */
/* 3806 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3808 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 3810 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3812 */	NdrFcShort( 0x256 ),	/* 598 */
/* 3814 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3816 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 3818 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3820 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3822 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3824 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 3826 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 3828 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3830 */	NdrFcShort( 0x434 ),	/* Type Offset=1076 */

	/* Parameter pBlockReq */

/* 3832 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 3834 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3836 */	NdrFcShort( 0x174 ),	/* Type Offset=372 */

	/* Parameter pDataIn */

/* 3838 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3840 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3842 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_AddComment */


	/* Return value */

/* 3844 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3846 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3850 */	NdrFcShort( 0x3c ),	/* 60 */
/* 3852 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3854 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 3856 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3858 */	NdrFcShort( 0x358 ),	/* 856 */
/* 3860 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3862 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 3864 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3866 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3868 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3870 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 3872 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 3874 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3876 */	NdrFcShort( 0x896 ),	/* Type Offset=2198 */

	/* Parameter pLogEntry */

/* 3878 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 3880 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3882 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pszFileName */

/* 3884 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3886 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3888 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_AddOpcFileEntry */


	/* Return value */

/* 3890 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3892 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3896 */	NdrFcShort( 0x3d ),	/* 61 */
/* 3898 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 3900 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 3902 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3904 */	NdrFcShort( 0x1c ),	/* 28 */
/* 3906 */	NdrFcShort( 0x24 ),	/* 36 */
/* 3908 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 3910 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 3912 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3914 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3916 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 3918 */	NdrFcShort( 0xb ),	/* Flags:  must size, must free, in, */
/* 3920 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3922 */	NdrFcShort( 0x8cc ),	/* Type Offset=2252 */

	/* Parameter szCLSID */

/* 3924 */	NdrFcShort( 0xb ),	/* Flags:  must size, must free, in, */
/* 3926 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3928 */	NdrFcShort( 0x8d0 ),	/* Type Offset=2256 */

	/* Parameter szSystem */

/* 3930 */	NdrFcShort( 0xb ),	/* Flags:  must size, must free, in, */
/* 3932 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3934 */	NdrFcShort( 0x8cc ),	/* Type Offset=2252 */

	/* Parameter szTag */

/* 3936 */	NdrFcShort( 0xb ),	/* Flags:  must size, must free, in, */
/* 3938 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 3940 */	NdrFcShort( 0x8cc ),	/* Type Offset=2252 */

	/* Parameter szServer */

/* 3942 */	NdrFcShort( 0x158 ),	/* Flags:  in, out, base type, simple ref, */
/* 3944 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 3946 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwTagID */

/* 3948 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 3950 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 3952 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetOpcSourceStrings */


	/* Return value */

/* 3954 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 3956 */	NdrFcLong( 0x0 ),	/* 0 */
/* 3960 */	NdrFcShort( 0x3e ),	/* 62 */
/* 3962 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 3964 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 3966 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 3968 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3970 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3972 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x6,		/* 6 */
/* 3974 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 3976 */	NdrFcShort( 0x4 ),	/* 4 */
/* 3978 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3980 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 3982 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 3984 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 3986 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwTagID */

/* 3988 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 3990 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3992 */	NdrFcShort( 0x4ac ),	/* Type Offset=1196 */

	/* Parameter szSystem */

/* 3994 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 3996 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 3998 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szTag */

/* 4000 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 4002 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4004 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szServer */

/* 4006 */	NdrFcShort( 0x13 ),	/* Flags:  must size, must free, out, */
/* 4008 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 4010 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szCLSID */

/* 4012 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4014 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 4016 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_DeleteOpcEntry */


	/* Return value */

/* 4018 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4020 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4024 */	NdrFcShort( 0x3f ),	/* 63 */
/* 4026 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4028 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4030 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4032 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4034 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4036 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 4038 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 4040 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4042 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4044 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4046 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4048 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4050 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwTagID */

/* 4052 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4054 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4056 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetOpcUAEntryData */


	/* Return value */

/* 4058 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4060 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4064 */	NdrFcShort( 0x40 ),	/* 64 */
/* 4066 */	NdrFcShort( 0x28 ),	/* x86 Stack size/offset = 40 */
/* 4068 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4070 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4072 */	NdrFcShort( 0x2f ),	/* 47 */
/* 4074 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4076 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x9,		/* 9 */
/* 4078 */	0x8,		/* 8 */
			0x7,		/* Ext Flags:  new corr desc, clt corr check, srv corr check, */
/* 4080 */	NdrFcShort( 0x1 ),	/* 1 */
/* 4082 */	NdrFcShort( 0x1 ),	/* 1 */
/* 4084 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4086 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4088 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4090 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bPointType */

/* 4092 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4094 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4096 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPointID */

/* 4098 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4100 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4102 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wPointIndex */

/* 4104 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4106 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4108 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter bVariable */

/* 4110 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4112 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 4114 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wAllocatedType */

/* 4116 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4118 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 4120 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter iSourceIndex */

/* 4122 */	NdrFcShort( 0x11b ),	/* Flags:  must size, must free, in, out, simple ref, */
/* 4124 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 4126 */	NdrFcShort( 0x8e4 ),	/* Type Offset=2276 */

	/* Parameter pData */

/* 4128 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4130 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 4132 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter ulDataSize */

/* 4134 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4136 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 4138 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_SetOpcUAEntryData */


	/* Return value */

/* 4140 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4142 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4146 */	NdrFcShort( 0x41 ),	/* 65 */
/* 4148 */	NdrFcShort( 0x2c ),	/* x86 Stack size/offset = 44 */
/* 4150 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4152 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4154 */	NdrFcShort( 0x2f ),	/* 47 */
/* 4156 */	NdrFcShort( 0x24 ),	/* 36 */
/* 4158 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0xa,		/* 10 */
/* 4160 */	0x8,		/* 8 */
			0x7,		/* Ext Flags:  new corr desc, clt corr check, srv corr check, */
/* 4162 */	NdrFcShort( 0x1 ),	/* 1 */
/* 4164 */	NdrFcShort( 0x1 ),	/* 1 */
/* 4166 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4168 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4170 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4172 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bPointType */

/* 4174 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4176 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4178 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPointID */

/* 4180 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4182 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4184 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wPointIndex */

/* 4186 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4188 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4190 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter bVariable */

/* 4192 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4194 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 4196 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wAllocatedType */

/* 4198 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4200 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 4202 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter iSourceIndex */

/* 4204 */	NdrFcShort( 0x11b ),	/* Flags:  must size, must free, in, out, simple ref, */
/* 4206 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 4208 */	NdrFcShort( 0x8e4 ),	/* Type Offset=2276 */

	/* Parameter pData */

/* 4210 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4212 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 4214 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter ulDataSize */

/* 4216 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 4218 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 4220 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwDataIndex */

/* 4222 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4224 */	NdrFcShort( 0x28 ),	/* x86 Stack size/offset = 40 */
/* 4226 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_DeleteOpcUAEntry */


	/* Return value */

/* 4228 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4230 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4234 */	NdrFcShort( 0x42 ),	/* 66 */
/* 4236 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 4238 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4240 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4242 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4244 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4246 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 4248 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 4250 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4252 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4254 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4256 */	NdrFcShort( 0x8b ),	/* Flags:  must size, must free, in, by val, */
/* 4258 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4260 */	NdrFcShort( 0x130 ),	/* Type Offset=304 */

	/* Parameter OpcData */

/* 4262 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4264 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4266 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetOpcUASourceString */


	/* Return value */

/* 4268 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4270 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4274 */	NdrFcShort( 0x43 ),	/* 67 */
/* 4276 */	NdrFcShort( 0x28 ),	/* x86 Stack size/offset = 40 */
/* 4278 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4280 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4282 */	NdrFcShort( 0x27 ),	/* 39 */
/* 4284 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4286 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x9,		/* 9 */
/* 4288 */	0x8,		/* 8 */
			0x7,		/* Ext Flags:  new corr desc, clt corr check, srv corr check, */
/* 4290 */	NdrFcShort( 0x2 ),	/* 2 */
/* 4292 */	NdrFcShort( 0x2 ),	/* 2 */
/* 4294 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4296 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4298 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4300 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bPointType */

/* 4302 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4304 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4306 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPointID */

/* 4308 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4310 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4312 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wPointIndex */

/* 4314 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4316 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4318 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter bVariable */

/* 4320 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4322 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 4324 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wAllocatedType */

/* 4326 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4328 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 4330 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter iSourceIndex */

/* 4332 */	NdrFcShort( 0x1b ),	/* Flags:  must size, must free, in, out, */
/* 4334 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 4336 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szServer */

/* 4338 */	NdrFcShort( 0x1b ),	/* Flags:  must size, must free, in, out, */
/* 4340 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 4342 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szSelectedOpcTag */

/* 4344 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4346 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 4348 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EnumerateCopyPoints */


	/* Return value */

/* 4350 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4352 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4356 */	NdrFcShort( 0x44 ),	/* 68 */
/* 4358 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 4360 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4362 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4364 */	NdrFcShort( 0x1c8 ),	/* 456 */
/* 4366 */	NdrFcShort( 0x22 ),	/* 34 */
/* 4368 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x8,		/* 8 */
/* 4370 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 4372 */	NdrFcShort( 0x3 ),	/* 3 */
/* 4374 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4376 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4378 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 4380 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4382 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 4384 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 4386 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4388 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystemName */

/* 4390 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 4392 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4394 */	NdrFcShort( 0x8f4 ),	/* Type Offset=2292 */

	/* Parameter pntCopy */

/* 4396 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 4398 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4400 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pNumberPoints */

/* 4402 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 4404 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 4406 */	NdrFcShort( 0x902 ),	/* Type Offset=2306 */

	/* Parameter ppNewTagArray */

/* 4408 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 4410 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 4412 */	NdrFcShort( 0x902 ),	/* Type Offset=2306 */

	/* Parameter ppOldTagArray */

/* 4414 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 4416 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 4418 */	NdrFcShort( 0x91a ),	/* Type Offset=2330 */

	/* Parameter ppTagEntry */

/* 4420 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4422 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 4424 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure RPC_ExecutePointCopy */


	/* Return value */

/* 4426 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4428 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4432 */	NdrFcShort( 0x45 ),	/* 69 */
/* 4434 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 4436 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4438 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4440 */	NdrFcShort( 0x2d8 ),	/* 728 */
/* 4442 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4444 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 4446 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 4448 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4450 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4452 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4454 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 4456 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4458 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 4460 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 4462 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4464 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystemName */

/* 4466 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 4468 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4470 */	NdrFcShort( 0x8f4 ),	/* Type Offset=2292 */

	/* Parameter pntCopy */

/* 4472 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 4474 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4476 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter pNewNamesArray */

/* 4478 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 4480 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 4482 */	NdrFcShort( 0x922 ),	/* Type Offset=2338 */

	/* Parameter pOldTagArray */

/* 4484 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4486 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 4488 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetFormattedData */


	/* Return value */

/* 4490 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4492 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4496 */	NdrFcShort( 0x46 ),	/* 70 */
/* 4498 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 4500 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4502 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4504 */	NdrFcShort( 0x34 ),	/* 52 */
/* 4506 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4508 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x5,		/* 5 */
/* 4510 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 4512 */	NdrFcShort( 0x3 ),	/* 3 */
/* 4514 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4516 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4518 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 4520 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4522 */	NdrFcShort( 0x434 ),	/* Type Offset=1076 */

	/* Parameter pBlockReq */

/* 4524 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 4526 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4528 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szOutput */

/* 4530 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 4532 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4534 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szEngUnits */

/* 4536 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 4538 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4540 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szQuality */

/* 4542 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4544 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 4546 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_SetDataFormatted */


	/* Return value */

/* 4548 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4550 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4554 */	NdrFcShort( 0x47 ),	/* 71 */
/* 4556 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4558 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4560 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4562 */	NdrFcShort( 0x34 ),	/* 52 */
/* 4564 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4566 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 4568 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 4570 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4572 */	NdrFcShort( 0x1 ),	/* 1 */
/* 4574 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4576 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 4578 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4580 */	NdrFcShort( 0x434 ),	/* Type Offset=1076 */

	/* Parameter pBlockReq */

/* 4582 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 4584 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4586 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szInput */

/* 4588 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4590 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4592 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure EnumerateInputPoints */


	/* Return value */

/* 4594 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4596 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4600 */	NdrFcShort( 0x48 ),	/* 72 */
/* 4602 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4604 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4606 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4608 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4610 */	NdrFcShort( 0x20 ),	/* 32 */
/* 4612 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 4614 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 4616 */	NdrFcShort( 0x1 ),	/* 1 */
/* 4618 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4620 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4622 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 4624 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4626 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pwArraySize */

/* 4628 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 4630 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4632 */	NdrFcShort( 0x944 ),	/* Type Offset=2372 */

	/* Parameter ppInputTags */

/* 4634 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4636 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4638 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EnumerateOutputPoints */


	/* Return value */

/* 4640 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4642 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4646 */	NdrFcShort( 0x49 ),	/* 73 */
/* 4648 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4650 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4652 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4654 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4656 */	NdrFcShort( 0x20 ),	/* 32 */
/* 4658 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 4660 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 4662 */	NdrFcShort( 0x1 ),	/* 1 */
/* 4664 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4666 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4668 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 4670 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4672 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pwArraySize */

/* 4674 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 4676 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4678 */	NdrFcShort( 0x944 ),	/* Type Offset=2372 */

	/* Parameter ppOutputTags */

/* 4680 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4682 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4684 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure RPC_SetSealData */


	/* Return value */

/* 4686 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4688 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4692 */	NdrFcShort( 0x4a ),	/* 74 */
/* 4694 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4696 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 4698 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4700 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 4702 */	NdrFcShort( 0xa0 ),	/* 160 */
/* 4704 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4706 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 4708 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 4710 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4712 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4714 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 4716 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 4718 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4720 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter pSealData */

/* 4722 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 4724 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4726 */	NdrFcShort( 0x304 ),	/* Type Offset=772 */

	/* Return value */

/* 4728 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4730 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4732 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetSealID */

/* 4734 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4736 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4740 */	NdrFcShort( 0x4b ),	/* 75 */
/* 4742 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4744 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4746 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4748 */	NdrFcShort( 0xe4 ),	/* 228 */
/* 4750 */	NdrFcShort( 0x24 ),	/* 36 */
/* 4752 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 4754 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 4756 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4758 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4760 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4762 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 4764 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4766 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Name */

/* 4768 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 4770 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4772 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwSealID */

/* 4774 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4776 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4778 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EnumerateTranslations */


	/* Return value */

/* 4780 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4782 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4786 */	NdrFcShort( 0x4c ),	/* 76 */
/* 4788 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4790 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4792 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4794 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4796 */	NdrFcShort( 0x22 ),	/* 34 */
/* 4798 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 4800 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 4802 */	NdrFcShort( 0x1 ),	/* 1 */
/* 4804 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4806 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4808 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 4810 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4812 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pbArraySize */

/* 4814 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 4816 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4818 */	NdrFcShort( 0x960 ),	/* Type Offset=2400 */

	/* Parameter ppResult */

/* 4820 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4822 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4824 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetNewTranslation */


	/* Return value */

/* 4826 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4828 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4832 */	NdrFcShort( 0x4d ),	/* 77 */
/* 4834 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4836 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4838 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4840 */	NdrFcShort( 0x5 ),	/* 5 */
/* 4842 */	NdrFcShort( 0xc4 ),	/* 196 */
/* 4844 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 4846 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 4848 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4850 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4852 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4854 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4856 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4858 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bType */

/* 4860 */	NdrFcShort( 0x112 ),	/* Flags:  must free, out, simple ref, */
/* 4862 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4864 */	NdrFcShort( 0x96e ),	/* Type Offset=2414 */

	/* Parameter pName */

/* 4866 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4868 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4870 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_DeleteTranslation */


	/* Return value */

/* 4872 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4874 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4878 */	NdrFcShort( 0x4e ),	/* 78 */
/* 4880 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4882 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4884 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4886 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4888 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4890 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 4892 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 4894 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4896 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4898 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4900 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4902 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4904 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lIndex */

/* 4906 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4908 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4910 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EditTranslation */


	/* Return value */

/* 4912 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4914 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4918 */	NdrFcShort( 0x4f ),	/* 79 */
/* 4920 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 4922 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4924 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4926 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4928 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4930 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 4932 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 4934 */	NdrFcShort( 0x1 ),	/* 1 */
/* 4936 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4938 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4940 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4942 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4944 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lIndex */

/* 4946 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 4948 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4950 */	NdrFcShort( 0x9d8 ),	/* Type Offset=2520 */

	/* Parameter pTranslation */

/* 4952 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4954 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4956 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EditTranslationCancel */


	/* Return value */

/* 4958 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 4960 */	NdrFcLong( 0x0 ),	/* 0 */
/* 4964 */	NdrFcShort( 0x50 ),	/* 80 */
/* 4966 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 4968 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 4970 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 4972 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4974 */	NdrFcShort( 0x8 ),	/* 8 */
/* 4976 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x2,		/* 2 */
/* 4978 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 4980 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4982 */	NdrFcShort( 0x0 ),	/* 0 */
/* 4984 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 4986 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 4988 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 4990 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lIndex */

/* 4992 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 4994 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 4996 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_EditTranslationDone */


	/* Return value */

/* 4998 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5000 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5004 */	NdrFcShort( 0x51 ),	/* 81 */
/* 5006 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5008 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5010 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5012 */	NdrFcShort( 0x8 ),	/* 8 */
/* 5014 */	NdrFcShort( 0x8 ),	/* 8 */
/* 5016 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 5018 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 5020 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5022 */	NdrFcShort( 0x1 ),	/* 1 */
/* 5024 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5026 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5028 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5030 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lIndex */

/* 5032 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5034 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5036 */	NdrFcShort( 0x9d8 ),	/* Type Offset=2520 */

	/* Parameter pTranslation */

/* 5038 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5040 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5042 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_IsValidTranslation */


	/* Return value */

/* 5044 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5046 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5050 */	NdrFcShort( 0x52 ),	/* 82 */
/* 5052 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5054 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5056 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5058 */	NdrFcShort( 0x8 ),	/* 8 */
/* 5060 */	NdrFcShort( 0x21 ),	/* 33 */
/* 5062 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 5064 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 5066 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5068 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5070 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5072 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5074 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5076 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter lIndex */

/* 5078 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 5080 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5082 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter pfValid */

/* 5084 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5086 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5088 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_FindTranslationByName */


	/* Return value */

/* 5090 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5092 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5096 */	NdrFcShort( 0x53 ),	/* 83 */
/* 5098 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5100 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5102 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5104 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5106 */	NdrFcShort( 0x24 ),	/* 36 */
/* 5108 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x3,		/* 3 */
/* 5110 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 5112 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5114 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5116 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5118 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5120 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5122 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pszName */

/* 5124 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 5126 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5128 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter plIndex */

/* 5130 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5132 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5134 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_CalculateLeakRate */


	/* Return value */

/* 5136 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5138 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5142 */	NdrFcShort( 0x54 ),	/* 84 */
/* 5144 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 5146 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5148 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5150 */	NdrFcShort( 0x160 ),	/* 352 */
/* 5152 */	NdrFcShort( 0x14c ),	/* 332 */
/* 5154 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x8,		/* 8 */
/* 5156 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 5158 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5160 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5162 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5164 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5166 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5168 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pszSystem */

/* 5170 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5172 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5174 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pszPointTag */

/* 5176 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5178 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5180 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter sAnalysisMethod */

/* 5182 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5184 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5186 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter sAnalysisType */

/* 5188 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5190 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5192 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter tmStartTime */

/* 5194 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5196 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5198 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter tmEndTime */

/* 5200 */	NdrFcShort( 0x11a ),	/* Flags:  must free, in, out, simple ref, */
/* 5202 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 5204 */	NdrFcShort( 0x9f4 ),	/* Type Offset=2548 */

	/* Parameter lpAnalysisResult */

/* 5206 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5208 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 5210 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure EnumerateRealTimeLeakTanks */


	/* Return value */

/* 5212 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5214 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5218 */	NdrFcShort( 0x55 ),	/* 85 */
/* 5220 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5222 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5224 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5226 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5228 */	NdrFcShort( 0x20 ),	/* 32 */
/* 5230 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x3,		/* 3 */
/* 5232 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 5234 */	NdrFcShort( 0x1 ),	/* 1 */
/* 5236 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5238 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5240 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 5242 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5244 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pwArraySize */

/* 5246 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 5248 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5250 */	NdrFcShort( 0x944 ),	/* Type Offset=2372 */

	/* Parameter ppTankTags */

/* 5252 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5254 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5256 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure SetLeakRateValue */


	/* Return value */

/* 5258 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5260 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5264 */	NdrFcShort( 0x56 ),	/* 86 */
/* 5266 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5268 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5270 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5272 */	NdrFcShort( 0x1b ),	/* 27 */
/* 5274 */	NdrFcShort( 0x8 ),	/* 8 */
/* 5276 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x4,		/* 4 */
/* 5278 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 5280 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5282 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5284 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5286 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5288 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5290 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter wPointIndex */

/* 5292 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5294 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5296 */	0xc,		/* FC_DOUBLE */
			0x0,		/* 0 */

	/* Parameter dLeakRate */

/* 5298 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5300 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5302 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bNotEnoughDataAlarm */

/* 5304 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5306 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5308 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_DeleteReportData */


	/* Return value */

/* 5310 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5312 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5316 */	NdrFcShort( 0x57 ),	/* 87 */
/* 5318 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5320 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5322 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5324 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5326 */	NdrFcShort( 0x8 ),	/* 8 */
/* 5328 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x2,		/* 2 */
/* 5330 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 5332 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5334 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5336 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5338 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5340 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5342 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pszLeakRecordID */

/* 5344 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5346 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5348 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_GetRawData */


	/* Return value */

/* 5350 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5352 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5356 */	NdrFcShort( 0x58 ),	/* 88 */
/* 5358 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5360 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5362 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5364 */	NdrFcShort( 0x34 ),	/* 52 */
/* 5366 */	NdrFcShort( 0x8 ),	/* 8 */
/* 5368 */	0x45,		/* Oi2 Flags:  srv must size, has return, has ext, */
			0x5,		/* 5 */
/* 5370 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 5372 */	NdrFcShort( 0x3 ),	/* 3 */
/* 5374 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5376 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5378 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 5380 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5382 */	NdrFcShort( 0x434 ),	/* Type Offset=1076 */

	/* Parameter pBlockReq */

/* 5384 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 5386 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5388 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szOutput */

/* 5390 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 5392 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5394 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szEngUnits */

/* 5396 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 5398 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5400 */	NdrFcShort( 0x8d4 ),	/* Type Offset=2260 */

	/* Parameter szQuality */

/* 5402 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5404 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5406 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_PostGroupData */


	/* Return value */

/* 5408 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5410 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5414 */	NdrFcShort( 0x59 ),	/* 89 */
/* 5416 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 5418 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5420 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5422 */	NdrFcShort( 0x10 ),	/* 16 */
/* 5424 */	NdrFcShort( 0x24 ),	/* 36 */
/* 5426 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 5428 */	0x8,		/* 8 */
			0x7,		/* Ext Flags:  new corr desc, clt corr check, srv corr check, */
/* 5430 */	NdrFcShort( 0x1 ),	/* 1 */
/* 5432 */	NdrFcShort( 0x1 ),	/* 1 */
/* 5434 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5436 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5438 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5440 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwNumberBlockReq */

/* 5442 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5444 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5446 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwStatusOffset */

/* 5448 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5450 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5452 */	NdrFcShort( 0xa1c ),	/* Type Offset=2588 */

	/* Parameter pBlockReq */

/* 5454 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 5456 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5458 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pdwSize */

/* 5460 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 5462 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5464 */	NdrFcShort( 0xa2c ),	/* Type Offset=2604 */

	/* Parameter pbData */

/* 5466 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5468 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5470 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_OperatorCommandEX */


	/* Return value */

/* 5472 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5474 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5478 */	NdrFcShort( 0x5a ),	/* 90 */
/* 5480 */	NdrFcShort( 0x28 ),	/* x86 Stack size/offset = 40 */
/* 5482 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5484 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5486 */	NdrFcShort( 0x23a ),	/* 570 */
/* 5488 */	NdrFcShort( 0x8 ),	/* 8 */
/* 5490 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x9,		/* 9 */
/* 5492 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 5494 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5496 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5498 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5500 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5502 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5504 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 5506 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5508 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5510 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 5512 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5514 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5516 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pPointDescription */

/* 5518 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5520 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5522 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPointID */

/* 5524 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5526 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5528 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter bVarType */

/* 5530 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5532 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5534 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bFormat */

/* 5536 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5538 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 5540 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bOverride */

/* 5542 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 5544 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 5546 */	NdrFcShort( 0x174 ),	/* Type Offset=372 */

	/* Parameter pDataIn */

/* 5548 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5550 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 5552 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure EditFlowMeterStart */


	/* Return value */

/* 5554 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5556 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5560 */	NdrFcShort( 0x5b ),	/* 91 */
/* 5562 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 5564 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5566 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5568 */	NdrFcShort( 0xe9 ),	/* 233 */
/* 5570 */	NdrFcShort( 0x3e ),	/* 62 */
/* 5572 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 5574 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 5576 */	NdrFcShort( 0x1 ),	/* 1 */
/* 5578 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5580 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5582 */	NdrFcShort( 0x110 ),	/* Flags:  out, simple ref, */
/* 5584 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5586 */	NdrFcShort( 0xd6 ),	/* Type Offset=214 */

	/* Parameter pphContext */

/* 5588 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5590 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5592 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 5594 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5596 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5598 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 5600 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 5602 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5604 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 5606 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5608 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5610 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bType */

/* 5612 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 5614 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5616 */	NdrFcShort( 0xa66 ),	/* Type Offset=2662 */

	/* Parameter pReturn */

/* 5618 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5620 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 5622 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditFlowMeterCancel */


	/* Return value */

/* 5624 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5626 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5630 */	NdrFcShort( 0x5c ),	/* 92 */
/* 5632 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5634 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 5636 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5638 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 5640 */	NdrFcShort( 0x48 ),	/* 72 */
/* 5642 */	NdrFcShort( 0x3e ),	/* 62 */
/* 5644 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 5646 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 5648 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5650 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5652 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 5654 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 5656 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5658 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 5660 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5662 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5664 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 5666 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5668 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5670 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 5672 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5674 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5676 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 5678 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5680 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5682 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 5684 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5686 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5688 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditFlowMeterDone */

/* 5690 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5692 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5696 */	NdrFcShort( 0x5d ),	/* 93 */
/* 5698 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 5700 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 5702 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5704 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 5706 */	NdrFcShort( 0x48 ),	/* 72 */
/* 5708 */	NdrFcShort( 0x3e ),	/* 62 */
/* 5710 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 5712 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 5714 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5716 */	NdrFcShort( 0x1 ),	/* 1 */
/* 5718 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 5720 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 5722 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5724 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 5726 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5728 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5730 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 5732 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5734 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5736 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 5738 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5740 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5742 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 5744 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5746 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5748 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pNewPoint */

/* 5750 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5752 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5754 */	NdrFcShort( 0xa66 ),	/* Type Offset=2662 */

	/* Return value */

/* 5756 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5758 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5760 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditPipeLineStart */

/* 5762 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5764 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5768 */	NdrFcShort( 0x5e ),	/* 94 */
/* 5770 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 5772 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5774 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5776 */	NdrFcShort( 0xe9 ),	/* 233 */
/* 5778 */	NdrFcShort( 0x3e ),	/* 62 */
/* 5780 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 5782 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 5784 */	NdrFcShort( 0x1 ),	/* 1 */
/* 5786 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5788 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5790 */	NdrFcShort( 0x110 ),	/* Flags:  out, simple ref, */
/* 5792 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5794 */	NdrFcShort( 0xd6 ),	/* Type Offset=214 */

	/* Parameter pphContext */

/* 5796 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5798 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5800 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 5802 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5804 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5806 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 5808 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 5810 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5812 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 5814 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5816 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5818 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bType */

/* 5820 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 5822 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5824 */	NdrFcShort( 0xb20 ),	/* Type Offset=2848 */

	/* Parameter pReturn */

/* 5826 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5828 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 5830 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditPipeLineCancel */


	/* Return value */

/* 5832 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5834 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5838 */	NdrFcShort( 0x5f ),	/* 95 */
/* 5840 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5842 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 5844 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5846 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 5848 */	NdrFcShort( 0x48 ),	/* 72 */
/* 5850 */	NdrFcShort( 0x3e ),	/* 62 */
/* 5852 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 5854 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 5856 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5858 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5860 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 5862 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 5864 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5866 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 5868 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5870 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5872 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 5874 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5876 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5878 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 5880 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5882 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5884 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 5886 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5888 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5890 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 5892 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5894 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5896 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditPipeLineDone */

/* 5898 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5900 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5904 */	NdrFcShort( 0x60 ),	/* 96 */
/* 5906 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 5908 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 5910 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5912 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 5914 */	NdrFcShort( 0x48 ),	/* 72 */
/* 5916 */	NdrFcShort( 0x3e ),	/* 62 */
/* 5918 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 5920 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 5922 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5924 */	NdrFcShort( 0x1 ),	/* 1 */
/* 5926 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 5928 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 5930 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5932 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 5934 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5936 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 5938 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 5940 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5942 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 5944 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 5946 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5948 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 5950 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 5952 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 5954 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 5956 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pNewPoint */

/* 5958 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 5960 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 5962 */	NdrFcShort( 0xb20 ),	/* Type Offset=2848 */

	/* Return value */

/* 5964 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 5966 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 5968 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditLogicPntStart */

/* 5970 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 5972 */	NdrFcLong( 0x0 ),	/* 0 */
/* 5976 */	NdrFcShort( 0x61 ),	/* 97 */
/* 5978 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 5980 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 5982 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 5984 */	NdrFcShort( 0xe9 ),	/* 233 */
/* 5986 */	NdrFcShort( 0x3e ),	/* 62 */
/* 5988 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 5990 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 5992 */	NdrFcShort( 0x1 ),	/* 1 */
/* 5994 */	NdrFcShort( 0x0 ),	/* 0 */
/* 5996 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 5998 */	NdrFcShort( 0x110 ),	/* Flags:  out, simple ref, */
/* 6000 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 6002 */	NdrFcShort( 0xd6 ),	/* Type Offset=214 */

	/* Parameter pphContext */

/* 6004 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6006 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 6008 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 6010 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6012 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 6014 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 6016 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 6018 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 6020 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 6022 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6024 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 6026 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bType */

/* 6028 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 6030 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 6032 */	NdrFcShort( 0xb7e ),	/* Type Offset=2942 */

	/* Parameter pReturn */

/* 6034 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 6036 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 6038 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditLogicPntCancel */


	/* Return value */

/* 6040 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 6042 */	NdrFcLong( 0x0 ),	/* 0 */
/* 6046 */	NdrFcShort( 0x62 ),	/* 98 */
/* 6048 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 6050 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 6052 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6054 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 6056 */	NdrFcShort( 0x48 ),	/* 72 */
/* 6058 */	NdrFcShort( 0x3e ),	/* 62 */
/* 6060 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 6062 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 6064 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6066 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6068 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 6070 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 6072 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6074 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 6076 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6078 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 6080 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 6082 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6084 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 6086 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 6088 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6090 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 6092 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 6094 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6096 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 6098 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 6100 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 6102 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 6104 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditLogicPntDone */

/* 6106 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 6108 */	NdrFcLong( 0x0 ),	/* 0 */
/* 6112 */	NdrFcShort( 0x63 ),	/* 99 */
/* 6114 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 6116 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 6118 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6120 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 6122 */	NdrFcShort( 0x48 ),	/* 72 */
/* 6124 */	NdrFcShort( 0x3e ),	/* 62 */
/* 6126 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 6128 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 6130 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6132 */	NdrFcShort( 0x1 ),	/* 1 */
/* 6134 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 6136 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 6138 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6140 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 6142 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6144 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 6146 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 6148 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6150 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 6152 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 6154 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6156 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 6158 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 6160 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6162 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 6164 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pNewPoint */

/* 6166 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6168 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 6170 */	NdrFcShort( 0xb7e ),	/* Type Offset=2942 */

	/* Return value */

/* 6172 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 6174 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 6176 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditTimerPntStart */

/* 6178 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 6180 */	NdrFcLong( 0x0 ),	/* 0 */
/* 6184 */	NdrFcShort( 0x64 ),	/* 100 */
/* 6186 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 6188 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 6190 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6192 */	NdrFcShort( 0xe9 ),	/* 233 */
/* 6194 */	NdrFcShort( 0x3e ),	/* 62 */
/* 6196 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 6198 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 6200 */	NdrFcShort( 0x1 ),	/* 1 */
/* 6202 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6204 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 6206 */	NdrFcShort( 0x110 ),	/* Flags:  out, simple ref, */
/* 6208 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 6210 */	NdrFcShort( 0xd6 ),	/* Type Offset=214 */

	/* Parameter pphContext */

/* 6212 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6214 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 6216 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 6218 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6220 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 6222 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 6224 */	NdrFcShort( 0x10a ),	/* Flags:  must free, in, simple ref, */
/* 6226 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 6228 */	NdrFcShort( 0x1e ),	/* Type Offset=30 */

	/* Parameter Names */

/* 6230 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6232 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 6234 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bType */

/* 6236 */	NdrFcShort( 0x113 ),	/* Flags:  must size, must free, out, simple ref, */
/* 6238 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 6240 */	NdrFcShort( 0xbc0 ),	/* Type Offset=3008 */

	/* Parameter pReturn */

/* 6242 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 6244 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 6246 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditTimerPntCancel */


	/* Return value */

/* 6248 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 6250 */	NdrFcLong( 0x0 ),	/* 0 */
/* 6254 */	NdrFcShort( 0x65 ),	/* 101 */
/* 6256 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 6258 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 6260 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6262 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 6264 */	NdrFcShort( 0x48 ),	/* 72 */
/* 6266 */	NdrFcShort( 0x3e ),	/* 62 */
/* 6268 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 6270 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 6272 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6274 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6276 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 6278 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 6280 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6282 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 6284 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6286 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 6288 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 6290 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6292 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 6294 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 6296 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6298 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 6300 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 6302 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6304 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 6306 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 6308 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 6310 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 6312 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EditTimerPntDone */

/* 6314 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 6316 */	NdrFcLong( 0x0 ),	/* 0 */
/* 6320 */	NdrFcShort( 0x66 ),	/* 102 */
/* 6322 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 6324 */	0x30,		/* FC_BIND_CONTEXT */
			0xe0,		/* Ctxt flags:  via ptr, in, out, */
/* 6326 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6328 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 6330 */	NdrFcShort( 0x48 ),	/* 72 */
/* 6332 */	NdrFcShort( 0x3e ),	/* 62 */
/* 6334 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x7,		/* 7 */
/* 6336 */	0x8,		/* 8 */
			0x5,		/* Ext Flags:  new corr desc, srv corr check, */
/* 6338 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6340 */	NdrFcShort( 0x1 ),	/* 1 */
/* 6342 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter pphContext */

/* 6344 */	NdrFcShort( 0x118 ),	/* Flags:  in, out, simple ref, */
/* 6346 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6348 */	NdrFcShort( 0x1e4 ),	/* Type Offset=484 */

	/* Parameter pUserName */

/* 6350 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6352 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 6354 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pSystem */

/* 6356 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6358 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 6360 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter dwPntID */

/* 6362 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6364 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 6366 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwIndex */

/* 6368 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6370 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 6372 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter pNewPoint */

/* 6374 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6376 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 6378 */	NdrFcShort( 0xbc0 ),	/* Type Offset=3008 */

	/* Return value */

/* 6380 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 6382 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 6384 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EnumerateMeterPoints */

/* 6386 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 6388 */	NdrFcLong( 0x0 ),	/* 0 */
/* 6392 */	NdrFcShort( 0x67 ),	/* 103 */
/* 6394 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 6396 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 6398 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6400 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6402 */	NdrFcShort( 0x20 ),	/* 32 */
/* 6404 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 6406 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 6408 */	NdrFcShort( 0x3 ),	/* 3 */
/* 6410 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6412 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 6414 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6416 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 6418 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 6420 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 6422 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 6424 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pwArraySize */

/* 6426 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 6428 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 6430 */	NdrFcShort( 0x588 ),	/* Type Offset=1416 */

	/* Parameter stDescription */

/* 6432 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 6434 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 6436 */	NdrFcShort( 0xbe6 ),	/* Type Offset=3046 */

	/* Parameter ppMeterTypes */

/* 6438 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 6440 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 6442 */	NdrFcShort( 0x5aa ),	/* Type Offset=1450 */

	/* Parameter ppMeterTags */

/* 6444 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 6446 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 6448 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure EnumeratePipelinePoints */


	/* Return value */

/* 6450 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 6452 */	NdrFcLong( 0x0 ),	/* 0 */
/* 6456 */	NdrFcShort( 0x68 ),	/* 104 */
/* 6458 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 6460 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 6462 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6464 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6466 */	NdrFcShort( 0x20 ),	/* 32 */
/* 6468 */	0x47,		/* Oi2 Flags:  srv must size, clt must size, has return, has ext, */
			0x5,		/* 5 */
/* 6470 */	0x8,		/* 8 */
			0x3,		/* Ext Flags:  new corr desc, clt corr check, */
/* 6472 */	NdrFcShort( 0x2 ),	/* 2 */
/* 6474 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6476 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 6478 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6480 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 6482 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 6484 */	NdrFcShort( 0x2150 ),	/* Flags:  out, base type, simple ref, srv alloc size=8 */
/* 6486 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 6488 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Parameter pwArraySize */

/* 6490 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 6492 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 6494 */	NdrFcShort( 0x588 ),	/* Type Offset=1416 */

	/* Parameter stDescription */

/* 6496 */	NdrFcShort( 0x2013 ),	/* Flags:  must size, must free, out, srv alloc size=8 */
/* 6498 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 6500 */	NdrFcShort( 0x5aa ),	/* Type Offset=1450 */

	/* Parameter ppTankTags */

/* 6502 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 6504 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 6506 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

	/* Procedure RPC_UpdateAlarmComment */


	/* Return value */

/* 6508 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 6510 */	NdrFcLong( 0x0 ),	/* 0 */
/* 6514 */	NdrFcShort( 0x69 ),	/* 105 */
/* 6516 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 6518 */	0x32,		/* FC_BIND_PRIMITIVE */
			0x0,		/* 0 */
/* 6520 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6522 */	NdrFcShort( 0x15 ),	/* 21 */
/* 6524 */	NdrFcShort( 0x8 ),	/* 8 */
/* 6526 */	0x46,		/* Oi2 Flags:  clt must size, has return, has ext, */
			0x6,		/* 6 */
/* 6528 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 6530 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6532 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6534 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter hBinding */

/* 6536 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6538 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 6540 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pUserName */

/* 6542 */	NdrFcShort( 0x10b ),	/* Flags:  must size, must free, in, simple ref, */
/* 6544 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 6546 */	NdrFcShort( 0x6c ),	/* Type Offset=108 */

	/* Parameter pComment */

/* 6548 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6550 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 6552 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwAlarmID */

/* 6554 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6556 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 6558 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Parameter dwPointID */

/* 6560 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6562 */	NdrFcShort( 0x14 ),	/* x86 Stack size/offset = 20 */
/* 6564 */	0x2,		/* FC_CHAR */
			0x0,		/* 0 */

	/* Parameter bOffSet */

/* 6566 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 6568 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 6570 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Procedure RPC_IsPointInActiveMovement */


	/* Return value */

/* 6572 */	0x0,		/* 0 */
			0x48,		/* Old Flags:  */
/* 6574 */	NdrFcLong( 0x0 ),	/* 0 */
/* 6578 */	NdrFcShort( 0x6a ),	/* 106 */
/* 6580 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 6582 */	0x30,		/* FC_BIND_CONTEXT */
			0x40,		/* Ctxt flags:  in, */
/* 6584 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6586 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 6588 */	NdrFcShort( 0x2c ),	/* 44 */
/* 6590 */	NdrFcShort( 0x6 ),	/* 6 */
/* 6592 */	0x44,		/* Oi2 Flags:  has return, has ext, */
			0x3,		/* 3 */
/* 6594 */	0x8,		/* 8 */
			0x1,		/* Ext Flags:  new corr desc, */
/* 6596 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6598 */	NdrFcShort( 0x0 ),	/* 0 */
/* 6600 */	NdrFcShort( 0x0 ),	/* 0 */

	/* Parameter phContext */

/* 6602 */	NdrFcShort( 0x8 ),	/* Flags:  in, */
/* 6604 */	NdrFcShort( 0x0 ),	/* x86 Stack size/offset = 0 */
/* 6606 */	NdrFcShort( 0x4f4 ),	/* Type Offset=1268 */

	/* Parameter dwPointID */

/* 6608 */	NdrFcShort( 0x48 ),	/* Flags:  in, base type, */
/* 6610 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 6612 */	0x8,		/* FC_LONG */
			0x0,		/* 0 */

	/* Return value */

/* 6614 */	NdrFcShort( 0x70 ),	/* Flags:  out, return, base type, */
/* 6616 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 6618 */	0x6,		/* FC_SHORT */
			0x0,		/* 0 */

			0x0
        }
    };

static const dmlink_MIDL_TYPE_FORMAT_STRING dmlink__MIDL_TypeFormatString =
    {
        0,
        {
			NdrFcShort( 0x0 ),	/* 0 */
/*  2 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/*  4 */	0x2,		/* FC_CHAR */
			0x5c,		/* FC_PAD */
/*  6 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/*  8 */	0x6,		/* FC_SHORT */
			0x5c,		/* FC_PAD */
/* 10 */	
			0x11, 0x0,	/* FC_RP */
/* 12 */	NdrFcShort( 0x12 ),	/* Offset= 18 (30) */
/* 14 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 16 */	NdrFcShort( 0x20 ),	/* 32 */
/* 18 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 20 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 22 */	NdrFcShort( 0x80 ),	/* 128 */
/* 24 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 26 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (14) */
/* 28 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 30 */	
			0x15,		/* FC_STRUCT */
			0x1,		/* 1 */
/* 32 */	NdrFcShort( 0x80 ),	/* 128 */
/* 34 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 36 */	NdrFcShort( 0xfff0 ),	/* Offset= -16 (20) */
/* 38 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 40 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 42 */	NdrFcShort( 0x2 ),	/* Offset= 2 (44) */
/* 44 */	
			0x12, 0x0,	/* FC_UP */
/* 46 */	NdrFcShort( 0xc ),	/* Offset= 12 (58) */
/* 48 */	
			0x15,		/* FC_STRUCT */
			0x1,		/* 1 */
/* 50 */	NdrFcShort( 0x20 ),	/* 32 */
/* 52 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 54 */	NdrFcShort( 0xffd8 ),	/* Offset= -40 (14) */
/* 56 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 58 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 60 */	NdrFcShort( 0x20 ),	/* 32 */
/* 62 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 64 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 66 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 68 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 70 */	NdrFcShort( 0xffea ),	/* Offset= -22 (48) */
/* 72 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 74 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 76 */	NdrFcShort( 0x2 ),	/* Offset= 2 (78) */
/* 78 */	
			0x12, 0x0,	/* FC_UP */
/* 80 */	NdrFcShort( 0xa ),	/* Offset= 10 (90) */
/* 82 */	
			0x15,		/* FC_STRUCT */
			0x0,		/* 0 */
/* 84 */	NdrFcShort( 0x2 ),	/* 2 */
/* 86 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 88 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 90 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 92 */	NdrFcShort( 0x2 ),	/* 2 */
/* 94 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 96 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 98 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 100 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 102 */	NdrFcShort( 0xffec ),	/* Offset= -20 (82) */
/* 104 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 106 */	
			0x11, 0x8,	/* FC_RP [simple_pointer] */
/* 108 */	
			0x25,		/* FC_C_WSTRING */
			0x5c,		/* FC_PAD */
/* 110 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 112 */	NdrFcShort( 0x2 ),	/* Offset= 2 (114) */
/* 114 */	
			0x12, 0x0,	/* FC_UP */
/* 116 */	NdrFcShort( 0x2 ),	/* Offset= 2 (118) */
/* 118 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 120 */	NdrFcShort( 0x20 ),	/* 32 */
/* 122 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 124 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 126 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 128 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 130 */	NdrFcShort( 0xffae ),	/* Offset= -82 (48) */
/* 132 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 134 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 136 */	NdrFcShort( 0x2 ),	/* Offset= 2 (138) */
/* 138 */	
			0x12, 0x0,	/* FC_UP */
/* 140 */	NdrFcShort( 0x2 ),	/* Offset= 2 (142) */
/* 142 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 144 */	NdrFcShort( 0x2 ),	/* 2 */
/* 146 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 148 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 150 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 152 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 154 */	NdrFcShort( 0xffb8 ),	/* Offset= -72 (82) */
/* 156 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 158 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/* 160 */	NdrFcShort( 0x8 ),	/* Offset= 8 (168) */
/* 162 */	
			0x1d,		/* FC_SMFARRAY */
			0x0,		/* 0 */
/* 164 */	NdrFcShort( 0x2 ),	/* 2 */
/* 166 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 168 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 170 */	NdrFcShort( 0xc ),	/* 12 */
/* 172 */	NdrFcShort( 0x0 ),	/* 0 */
/* 174 */	NdrFcShort( 0x0 ),	/* Offset= 0 (174) */
/* 176 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 178 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 180 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 182 */	NdrFcShort( 0xffec ),	/* Offset= -20 (162) */
/* 184 */	0x3e,		/* FC_STRUCTPAD2 */
			0x5b,		/* FC_END */
/* 186 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/* 188 */	NdrFcShort( 0x2 ),	/* Offset= 2 (190) */
/* 190 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 192 */	NdrFcShort( 0x18 ),	/* 24 */
/* 194 */	NdrFcShort( 0x0 ),	/* 0 */
/* 196 */	NdrFcShort( 0x0 ),	/* Offset= 0 (196) */
/* 198 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 200 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 202 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 204 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 206 */	0x2,		/* FC_CHAR */
			0x3d,		/* FC_STRUCTPAD1 */
/* 208 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 210 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/* 212 */	NdrFcShort( 0x2 ),	/* Offset= 2 (214) */
/* 214 */	0x30,		/* FC_BIND_CONTEXT */
			0xa0,		/* Ctxt flags:  via ptr, out, */
/* 216 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 218 */	
			0x11, 0x0,	/* FC_RP */
/* 220 */	NdrFcShort( 0xcc ),	/* Offset= 204 (424) */
/* 222 */	
			0x1d,		/* FC_SMFARRAY */
			0x3,		/* 3 */
/* 224 */	NdrFcShort( 0x8 ),	/* 8 */
/* 226 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 228 */	
			0x2b,		/* FC_NON_ENCAPSULATED_UNION */
			0x4,		/* FC_USMALL */
/* 230 */	0x4,		/* Corr desc: FC_USMALL */
			0x0,		/*  */
/* 232 */	NdrFcShort( 0xc ),	/* 12 */
/* 234 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 236 */	NdrFcShort( 0x2 ),	/* Offset= 2 (238) */
/* 238 */	NdrFcShort( 0xc ),	/* 12 */
/* 240 */	NdrFcShort( 0x4 ),	/* 4 */
/* 242 */	NdrFcLong( 0x3 ),	/* 3 */
/* 246 */	NdrFcShort( 0x16 ),	/* Offset= 22 (268) */
/* 248 */	NdrFcLong( 0x7 ),	/* 7 */
/* 252 */	NdrFcShort( 0x1e ),	/* Offset= 30 (282) */
/* 254 */	NdrFcLong( 0x8 ),	/* 8 */
/* 258 */	NdrFcShort( 0x26 ),	/* Offset= 38 (296) */
/* 260 */	NdrFcLong( 0x9 ),	/* 9 */
/* 264 */	NdrFcShort( 0x28 ),	/* Offset= 40 (304) */
/* 266 */	NdrFcShort( 0x36 ),	/* Offset= 54 (320) */
/* 268 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 270 */	NdrFcShort( 0x9 ),	/* 9 */
/* 272 */	NdrFcShort( 0x0 ),	/* 0 */
/* 274 */	NdrFcShort( 0x0 ),	/* Offset= 0 (274) */
/* 276 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 278 */	0x2,		/* FC_CHAR */
			0x6,		/* FC_SHORT */
/* 280 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 282 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 284 */	NdrFcShort( 0xa ),	/* 10 */
/* 286 */	NdrFcShort( 0x0 ),	/* 0 */
/* 288 */	NdrFcShort( 0x0 ),	/* Offset= 0 (288) */
/* 290 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 292 */	0x2,		/* FC_CHAR */
			0x6,		/* FC_SHORT */
/* 294 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 296 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 298 */	NdrFcShort( 0x7 ),	/* 7 */
/* 300 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 302 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 304 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 306 */	NdrFcShort( 0xc ),	/* 12 */
/* 308 */	NdrFcShort( 0x0 ),	/* 0 */
/* 310 */	NdrFcShort( 0x0 ),	/* Offset= 0 (310) */
/* 312 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 314 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 316 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 318 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 320 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 322 */	NdrFcShort( 0x9 ),	/* 9 */
/* 324 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 326 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 328 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 330 */	NdrFcShort( 0x21 ),	/* 33 */
/* 332 */	NdrFcShort( 0x0 ),	/* 0 */
/* 334 */	NdrFcShort( 0x0 ),	/* Offset= 0 (334) */
/* 336 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 338 */	NdrFcShort( 0xff92 ),	/* Offset= -110 (228) */
/* 340 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 342 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 344 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 346 */	0x2,		/* FC_CHAR */
			0xc,		/* FC_DOUBLE */
/* 348 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 350 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x7,		/* 7 */
/* 352 */	NdrFcShort( 0x20 ),	/* 32 */
/* 354 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 358 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 360 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 364 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 366 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 368 */	NdrFcShort( 0xffd8 ),	/* Offset= -40 (328) */
/* 370 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 372 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 374 */	NdrFcShort( 0x1fe ),	/* 510 */
/* 376 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 378 */	
			0x1d,		/* FC_SMFARRAY */
			0x0,		/* 0 */
/* 380 */	NdrFcShort( 0x4 ),	/* 4 */
/* 382 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 384 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 386 */	NdrFcShort( 0x3e ),	/* 62 */
/* 388 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 390 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 392 */	NdrFcShort( 0x8 ),	/* 8 */
/* 394 */	NdrFcShort( 0x0 ),	/* 0 */
/* 396 */	NdrFcShort( 0x0 ),	/* Offset= 0 (396) */
/* 398 */	0x8,		/* FC_LONG */
			0x2,		/* FC_CHAR */
/* 400 */	0x3f,		/* FC_STRUCTPAD3 */
			0x5b,		/* FC_END */
/* 402 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x3,		/* 3 */
/* 404 */	NdrFcShort( 0x20 ),	/* 32 */
/* 406 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 410 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 412 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 416 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 418 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 420 */	NdrFcShort( 0xffe2 ),	/* Offset= -30 (390) */
/* 422 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 424 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 426 */	NdrFcShort( 0xbc8 ),	/* 3016 */
/* 428 */	NdrFcShort( 0x0 ),	/* 0 */
/* 430 */	NdrFcShort( 0x0 ),	/* Offset= 0 (430) */
/* 432 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 434 */	NdrFcShort( 0xff2c ),	/* Offset= -212 (222) */
/* 436 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 438 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 440 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 442 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 444 */	NdrFcShort( 0xffa2 ),	/* Offset= -94 (350) */
/* 446 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 448 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 450 */	NdrFcShort( 0xffb2 ),	/* Offset= -78 (372) */
/* 452 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 454 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 456 */	NdrFcShort( 0xff96 ),	/* Offset= -106 (350) */
/* 458 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 460 */	NdrFcShort( 0xffae ),	/* Offset= -82 (378) */
/* 462 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 464 */	NdrFcShort( 0xffb0 ),	/* Offset= -80 (384) */
/* 466 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 468 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 470 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 472 */	0x8,		/* FC_LONG */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 474 */	0x0,		/* 0 */
			NdrFcShort( 0xffb7 ),	/* Offset= -73 (402) */
			0x40,		/* FC_STRUCTPAD4 */
/* 478 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 480 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/* 482 */	NdrFcShort( 0x2 ),	/* Offset= 2 (484) */
/* 484 */	0x30,		/* FC_BIND_CONTEXT */
			0xe1,		/* Ctxt flags:  via ptr, in, out, can't be null */
/* 486 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 488 */	
			0x11, 0x0,	/* FC_RP */
/* 490 */	NdrFcShort( 0x1cc ),	/* Offset= 460 (950) */
/* 492 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 494 */	NdrFcShort( 0x48 ),	/* 72 */
/* 496 */	NdrFcShort( 0x0 ),	/* 0 */
/* 498 */	NdrFcShort( 0x0 ),	/* Offset= 0 (498) */
/* 500 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 502 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 504 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 506 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 508 */	NdrFcShort( 0xff4c ),	/* Offset= -180 (328) */
/* 510 */	0x43,		/* FC_STRUCTPAD7 */
			0x5b,		/* FC_END */
/* 512 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x7,		/* 7 */
/* 514 */	NdrFcShort( 0x40 ),	/* 64 */
/* 516 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 520 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 522 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 526 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 528 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 530 */	NdrFcShort( 0xffda ),	/* Offset= -38 (492) */
/* 532 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 534 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 536 */	NdrFcShort( 0x40 ),	/* 64 */
/* 538 */	NdrFcShort( 0x0 ),	/* 0 */
/* 540 */	NdrFcShort( 0x0 ),	/* Offset= 0 (540) */
/* 542 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 544 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 546 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 548 */	0xc,		/* FC_DOUBLE */
			0x2,		/* FC_CHAR */
/* 550 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 552 */	0x2,		/* FC_CHAR */
			0x40,		/* FC_STRUCTPAD4 */
/* 554 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 556 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 558 */	NdrFcShort( 0xe8 ),	/* 232 */
/* 560 */	NdrFcShort( 0x0 ),	/* 0 */
/* 562 */	NdrFcShort( 0x0 ),	/* Offset= 0 (562) */
/* 564 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 566 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 568 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 570 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 572 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 574 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 576 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 578 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 580 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 582 */	0x2,		/* FC_CHAR */
			0x43,		/* FC_STRUCTPAD7 */
/* 584 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 586 */	NdrFcShort( 0xffcc ),	/* Offset= -52 (534) */
/* 588 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 590 */	0xc,		/* FC_DOUBLE */
			0x6,		/* FC_SHORT */
/* 592 */	0x6,		/* FC_SHORT */
			0x40,		/* FC_STRUCTPAD4 */
/* 594 */	0xc,		/* FC_DOUBLE */
			0x5b,		/* FC_END */
/* 596 */	
			0x1d,		/* FC_SMFARRAY */
			0x7,		/* 7 */
/* 598 */	NdrFcShort( 0x28 ),	/* 40 */
/* 600 */	0xc,		/* FC_DOUBLE */
			0x5b,		/* FC_END */
/* 602 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 604 */	NdrFcShort( 0x68 ),	/* 104 */
/* 606 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 608 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 610 */	NdrFcShort( 0xfff2 ),	/* Offset= -14 (596) */
/* 612 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 614 */	0xc,		/* FC_DOUBLE */
			0x2,		/* FC_CHAR */
/* 616 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 618 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 620 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 622 */	0x2,		/* FC_CHAR */
			0xc,		/* FC_DOUBLE */
/* 624 */	0xc,		/* FC_DOUBLE */
			0x5b,		/* FC_END */
/* 626 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 628 */	NdrFcShort( 0xa0 ),	/* 160 */
/* 630 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 632 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 634 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 636 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 638 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 640 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 642 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 644 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 646 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 648 */	0xc,		/* FC_DOUBLE */
			0x8,		/* FC_LONG */
/* 650 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 652 */	
			0x1d,		/* FC_SMFARRAY */
			0x7,		/* 7 */
/* 654 */	NdrFcShort( 0x40 ),	/* 64 */
/* 656 */	0xc,		/* FC_DOUBLE */
			0x5b,		/* FC_END */
/* 658 */	
			0x1d,		/* FC_SMFARRAY */
			0x7,		/* 7 */
/* 660 */	NdrFcShort( 0x100 ),	/* 256 */
/* 662 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 664 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (652) */
/* 666 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 668 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 670 */	NdrFcShort( 0x110 ),	/* 272 */
/* 672 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 674 */	NdrFcShort( 0xfff0 ),	/* Offset= -16 (658) */
/* 676 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 678 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 680 */	
			0x1d,		/* FC_SMFARRAY */
			0x3,		/* 3 */
/* 682 */	NdrFcShort( 0x14 ),	/* 20 */
/* 684 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 686 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 688 */	NdrFcShort( 0x10 ),	/* 16 */
/* 690 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 692 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 694 */	NdrFcShort( 0xfe28 ),	/* Offset= -472 (222) */
/* 696 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 698 */	
			0x1d,		/* FC_SMFARRAY */
			0x3,		/* 3 */
/* 700 */	NdrFcShort( 0x50 ),	/* 80 */
/* 702 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 704 */	NdrFcShort( 0xffee ),	/* Offset= -18 (686) */
/* 706 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 708 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 710 */	NdrFcShort( 0x6 ),	/* 6 */
/* 712 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 714 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 716 */	NdrFcShort( 0xd0 ),	/* 208 */
/* 718 */	NdrFcShort( 0x0 ),	/* 0 */
/* 720 */	NdrFcShort( 0x0 ),	/* Offset= 0 (720) */
/* 722 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 724 */	NdrFcShort( 0xff80 ),	/* Offset= -128 (596) */
/* 726 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 728 */	NdrFcShort( 0xff7c ),	/* Offset= -132 (596) */
/* 730 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 732 */	NdrFcShort( 0xffcc ),	/* Offset= -52 (680) */
/* 734 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 736 */	NdrFcShort( 0xffda ),	/* Offset= -38 (698) */
/* 738 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 740 */	0x2,		/* FC_CHAR */
			0x3d,		/* FC_STRUCTPAD1 */
/* 742 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 744 */	0x6,		/* FC_SHORT */
			0xa,		/* FC_FLOAT */
/* 746 */	0x6,		/* FC_SHORT */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 748 */	0x0,		/* 0 */
			NdrFcShort( 0xffd7 ),	/* Offset= -41 (708) */
			0x40,		/* FC_STRUCTPAD4 */
/* 752 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 754 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 756 */	NdrFcShort( 0x2a ),	/* 42 */
/* 758 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 760 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 762 */	NdrFcShort( 0x200 ),	/* 512 */
/* 764 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 766 */	
			0x1d,		/* FC_SMFARRAY */
			0x0,		/* 0 */
/* 768 */	NdrFcShort( 0x40 ),	/* 64 */
/* 770 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 772 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 774 */	NdrFcShort( 0x48 ),	/* 72 */
/* 776 */	0x2,		/* FC_CHAR */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 778 */	0x0,		/* 0 */
			NdrFcShort( 0xfff3 ),	/* Offset= -13 (766) */
			0x3f,		/* FC_STRUCTPAD3 */
/* 782 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 784 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 786 */	NdrFcShort( 0x50 ),	/* 80 */
/* 788 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 790 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 792 */	NdrFcShort( 0x78 ),	/* 120 */
/* 794 */	NdrFcShort( 0x0 ),	/* 0 */
/* 796 */	NdrFcShort( 0x0 ),	/* Offset= 0 (796) */
/* 798 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 800 */	0x2,		/* FC_CHAR */
			0x3d,		/* FC_STRUCTPAD1 */
/* 802 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 804 */	NdrFcShort( 0xffec ),	/* Offset= -20 (784) */
/* 806 */	0x40,		/* FC_STRUCTPAD4 */
			0xc,		/* FC_DOUBLE */
/* 808 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 810 */	0x2,		/* FC_CHAR */
			0x3f,		/* FC_STRUCTPAD3 */
/* 812 */	0xc,		/* FC_DOUBLE */
			0x6,		/* FC_SHORT */
/* 814 */	0x42,		/* FC_STRUCTPAD6 */
			0x5b,		/* FC_END */
/* 816 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 818 */	NdrFcShort( 0x18 ),	/* 24 */
/* 820 */	0x2,		/* FC_CHAR */
			0x43,		/* FC_STRUCTPAD7 */
/* 822 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 824 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 826 */	
			0x15,		/* FC_STRUCT */
			0x0,		/* 0 */
/* 828 */	NdrFcShort( 0x3 ),	/* 3 */
/* 830 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 832 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 834 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 836 */	NdrFcShort( 0x88 ),	/* 136 */
/* 838 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 840 */	NdrFcShort( 0xffe8 ),	/* Offset= -24 (816) */
/* 842 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 844 */	NdrFcShort( 0xffe4 ),	/* Offset= -28 (816) */
/* 846 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 848 */	NdrFcShort( 0xffe0 ),	/* Offset= -32 (816) */
/* 850 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 852 */	0x2,		/* FC_CHAR */
			0x43,		/* FC_STRUCTPAD7 */
/* 854 */	0xc,		/* FC_DOUBLE */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 856 */	0x0,		/* 0 */
			NdrFcShort( 0xffe1 ),	/* Offset= -31 (826) */
			0x41,		/* FC_STRUCTPAD5 */
/* 860 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 862 */	0xc,		/* FC_DOUBLE */
			0x5b,		/* FC_END */
/* 864 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x1,		/* 1 */
/* 866 */	NdrFcShort( 0xc ),	/* 12 */
/* 868 */	NdrFcShort( 0x0 ),	/* 0 */
/* 870 */	NdrFcShort( 0x0 ),	/* Offset= 0 (870) */
/* 872 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 874 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 876 */	0x3d,		/* FC_STRUCTPAD1 */
			0x6,		/* FC_SHORT */
/* 878 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 880 */	0x3d,		/* FC_STRUCTPAD1 */
			0x5b,		/* FC_END */
/* 882 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x1,		/* 1 */
/* 884 */	NdrFcShort( 0x7 ),	/* 7 */
/* 886 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 890 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 892 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 896 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 898 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 900 */	NdrFcShort( 0xffdc ),	/* Offset= -36 (864) */
/* 902 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 904 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 906 */	NdrFcShort( 0x10 ),	/* 16 */
/* 908 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 910 */	0x41,		/* FC_STRUCTPAD5 */
			0xc,		/* FC_DOUBLE */
/* 912 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 914 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 916 */	NdrFcShort( 0x1fe ),	/* 510 */
/* 918 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 920 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 922 */	NdrFcShort( 0xa08 ),	/* 2568 */
/* 924 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 926 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (914) */
/* 928 */	0x3e,		/* FC_STRUCTPAD2 */
			0x8,		/* FC_LONG */
/* 930 */	0x8,		/* FC_LONG */
			0xc,		/* FC_DOUBLE */
/* 932 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 934 */	NdrFcShort( 0xffec ),	/* Offset= -20 (914) */
/* 936 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 938 */	NdrFcShort( 0xffe8 ),	/* Offset= -24 (914) */
/* 940 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 942 */	NdrFcShort( 0xffe4 ),	/* Offset= -28 (914) */
/* 944 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 946 */	NdrFcShort( 0xffe0 ),	/* Offset= -32 (914) */
/* 948 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 950 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 952 */	NdrFcShort( 0x2448 ),	/* 9288 */
/* 954 */	NdrFcShort( 0x0 ),	/* 0 */
/* 956 */	NdrFcShort( 0x0 ),	/* Offset= 0 (956) */
/* 958 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 960 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 962 */	NdrFcShort( 0xfe3e ),	/* Offset= -450 (512) */
/* 964 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 966 */	NdrFcShort( 0xfe66 ),	/* Offset= -410 (556) */
/* 968 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 970 */	NdrFcShort( 0xfe90 ),	/* Offset= -368 (602) */
/* 972 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 974 */	NdrFcShort( 0xfea4 ),	/* Offset= -348 (626) */
/* 976 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 978 */	NdrFcShort( 0xfeca ),	/* Offset= -310 (668) */
/* 980 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 982 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 984 */	NdrFcShort( 0xfef2 ),	/* Offset= -270 (714) */
/* 986 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 988 */	NdrFcShort( 0xfd02 ),	/* Offset= -766 (222) */
/* 990 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 992 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 994 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 996 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 998 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1000 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 1002 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 1004 */	0x2,		/* FC_CHAR */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1006 */	0x0,		/* 0 */
			NdrFcShort( 0xff03 ),	/* Offset= -253 (754) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1010 */	0x0,		/* 0 */
			NdrFcShort( 0xfd8d ),	/* Offset= -627 (384) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1014 */	0x0,		/* 0 */
			NdrFcShort( 0xff01 ),	/* Offset= -255 (760) */
			0x3e,		/* FC_STRUCTPAD2 */
/* 1018 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1020 */	NdrFcShort( 0xff08 ),	/* Offset= -248 (772) */
/* 1022 */	0x40,		/* FC_STRUCTPAD4 */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1024 */	0x0,		/* 0 */
			NdrFcShort( 0xff15 ),	/* Offset= -235 (790) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1028 */	0x0,		/* 0 */
			NdrFcShort( 0xff3d ),	/* Offset= -195 (834) */
			0x8,		/* FC_LONG */
/* 1032 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1034 */	NdrFcShort( 0xff68 ),	/* Offset= -152 (882) */
/* 1036 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1038 */	NdrFcShort( 0xff7a ),	/* Offset= -134 (904) */
/* 1040 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1042 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1044 */	NdrFcShort( 0xff84 ),	/* Offset= -124 (920) */
/* 1046 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1048 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 1050 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1052 */	0x26,		/* Corr desc:  parameter, FC_SHORT */
			0x0,		/*  */
/* 1054 */	NdrFcShort( 0xc ),	/* x86 Stack size/offset = 12 */
/* 1056 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1058 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1060 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 1062 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1064 */	0x26,		/* Corr desc:  parameter, FC_SHORT */
			0x0,		/*  */
/* 1066 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1068 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1070 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1072 */	
			0x11, 0x0,	/* FC_RP */
/* 1074 */	NdrFcShort( 0x10 ),	/* Offset= 16 (1090) */
/* 1076 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 1078 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1080 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 1082 */	0x2,		/* FC_CHAR */
			0x3d,		/* FC_STRUCTPAD1 */
/* 1084 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 1086 */	0x2,		/* FC_CHAR */
			0x8,		/* FC_LONG */
/* 1088 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1090 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 1092 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1094 */	0x28,		/* Corr desc:  parameter, FC_LONG */
			0x0,		/*  */
/* 1096 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1098 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1100 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1102 */	NdrFcShort( 0xffe6 ),	/* Offset= -26 (1076) */
/* 1104 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1106 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 1108 */	NdrFcShort( 0x1 ),	/* 1 */
/* 1110 */	0x28,		/* Corr desc:  parameter, FC_LONG */
			0x0,		/*  */
/* 1112 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 1114 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1116 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 1118 */	
			0x11, 0x0,	/* FC_RP */
/* 1120 */	NdrFcShort( 0x8 ),	/* Offset= 8 (1128) */
/* 1122 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 1124 */	NdrFcShort( 0xa2 ),	/* 162 */
/* 1126 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1128 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 1130 */	NdrFcShort( 0x388 ),	/* 904 */
/* 1132 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1134 */	NdrFcShort( 0x0 ),	/* Offset= 0 (1134) */
/* 1136 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1138 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1140 */	NdrFcShort( 0xfd00 ),	/* Offset= -768 (372) */
/* 1142 */	0x3e,		/* FC_STRUCTPAD2 */
			0x8,		/* FC_LONG */
/* 1144 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1146 */	NdrFcShort( 0xfba4 ),	/* Offset= -1116 (30) */
/* 1148 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1150 */	NdrFcShort( 0xfd02 ),	/* Offset= -766 (384) */
/* 1152 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1154 */	NdrFcShort( 0xffe0 ),	/* Offset= -32 (1122) */
/* 1156 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 1158 */	0x3d,		/* FC_STRUCTPAD1 */
			0x6,		/* FC_SHORT */
/* 1160 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 1162 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 1164 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 1166 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 1168 */	0x2,		/* FC_CHAR */
			0x41,		/* FC_STRUCTPAD5 */
/* 1170 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1172 */	
			0x11, 0x0,	/* FC_RP */
/* 1174 */	NdrFcShort( 0xfcde ),	/* Offset= -802 (372) */
/* 1176 */	
			0x11, 0x0,	/* FC_RP */
/* 1178 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1180) */
/* 1180 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 1182 */	NdrFcShort( 0x14 ),	/* 20 */
/* 1184 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1186 */	NdrFcShort( 0x0 ),	/* Offset= 0 (1186) */
/* 1188 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 1190 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 1192 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 1194 */	0x3f,		/* FC_STRUCTPAD3 */
			0x5b,		/* FC_END */
/* 1196 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 1198 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1200 */	0x40,		/* Corr desc:  constant, val=18 */
			0x0,		/* 0 */
/* 1202 */	NdrFcShort( 0x12 ),	/* 18 */
/* 1204 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1206 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1208 */	
			0x11, 0x0,	/* FC_RP */
/* 1210 */	NdrFcShort( 0x8 ),	/* Offset= 8 (1218) */
/* 1212 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 1214 */	NdrFcShort( 0x24 ),	/* 36 */
/* 1216 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1218 */	
			0x15,		/* FC_STRUCT */
			0x1,		/* 1 */
/* 1220 */	NdrFcShort( 0x28 ),	/* 40 */
/* 1222 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1224 */	NdrFcShort( 0xfff4 ),	/* Offset= -12 (1212) */
/* 1226 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 1228 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 1230 */	
			0x11, 0x0,	/* FC_RP */
/* 1232 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1234) */
/* 1234 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 1236 */	NdrFcShort( 0xc ),	/* 12 */
/* 1238 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 1240 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 1242 */	0x2,		/* FC_CHAR */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1244 */	0x0,		/* 0 */
			NdrFcShort( 0xfbc5 ),	/* Offset= -1083 (162) */
			0x5b,		/* FC_END */
/* 1248 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 1250 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1252) */
/* 1252 */	
			0x12, 0x0,	/* FC_UP */
/* 1254 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1256) */
/* 1256 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 1258 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1260 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 1262 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1264 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1266 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 1268 */	0x30,		/* FC_BIND_CONTEXT */
			0x41,		/* Ctxt flags:  in, can't be null */
/* 1270 */	0x0,		/* 0 */
			0x0,		/* 0 */
/* 1272 */	
			0x11, 0xc,	/* FC_RP [alloced_on_stack] [simple_pointer] */
/* 1274 */	0x8,		/* FC_LONG */
			0x5c,		/* FC_PAD */
/* 1276 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 1278 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1280) */
/* 1280 */	
			0x12, 0x0,	/* FC_UP */
/* 1282 */	NdrFcShort( 0xa ),	/* Offset= 10 (1292) */
/* 1284 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 1286 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1288 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1290 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1292 */	
			0x1b,		/* FC_CARRAY */
			0x7,		/* 7 */
/* 1294 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1296 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 1298 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 1300 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1302 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1304 */	NdrFcShort( 0xffec ),	/* Offset= -20 (1284) */
/* 1306 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1308 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 1310 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1312) */
/* 1312 */	
			0x12, 0x0,	/* FC_UP */
/* 1314 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1316) */
/* 1316 */	
			0x1b,		/* FC_CARRAY */
			0x7,		/* 7 */
/* 1318 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1320 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 1322 */	NdrFcShort( 0x28 ),	/* x86 Stack size/offset = 40 */
/* 1324 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1326 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1328 */	NdrFcShort( 0xffd4 ),	/* Offset= -44 (1284) */
/* 1330 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1332 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 1334 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1336) */
/* 1336 */	
			0x12, 0x0,	/* FC_UP */
/* 1338 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1340) */
/* 1340 */	
			0x1b,		/* FC_CARRAY */
			0x7,		/* 7 */
/* 1342 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1344 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 1346 */	NdrFcShort( 0x30 ),	/* x86 Stack size/offset = 48 */
/* 1348 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1350 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1352 */	NdrFcShort( 0xffbc ),	/* Offset= -68 (1284) */
/* 1354 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1356 */	
			0x11, 0x0,	/* FC_RP */
/* 1358 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1360) */
/* 1360 */	
			0x1b,		/* FC_CARRAY */
			0x7,		/* 7 */
/* 1362 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1364 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x0,		/*  */
/* 1366 */	NdrFcShort( 0x1c ),	/* x86 Stack size/offset = 28 */
/* 1368 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1370 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1372 */	NdrFcShort( 0xffa8 ),	/* Offset= -88 (1284) */
/* 1374 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1376 */	
			0x11, 0x0,	/* FC_RP */
/* 1378 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1380) */
/* 1380 */	
			0x1b,		/* FC_CARRAY */
			0x7,		/* 7 */
/* 1382 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1384 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x0,		/*  */
/* 1386 */	NdrFcShort( 0x24 ),	/* x86 Stack size/offset = 36 */
/* 1388 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1390 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1392 */	NdrFcShort( 0xff94 ),	/* Offset= -108 (1284) */
/* 1394 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1396 */	
			0x11, 0x0,	/* FC_RP */
/* 1398 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1400) */
/* 1400 */	
			0x1b,		/* FC_CARRAY */
			0x7,		/* 7 */
/* 1402 */	NdrFcShort( 0x10 ),	/* 16 */
/* 1404 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x0,		/*  */
/* 1406 */	NdrFcShort( 0x2c ),	/* x86 Stack size/offset = 44 */
/* 1408 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1410 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1412 */	NdrFcShort( 0xff80 ),	/* Offset= -128 (1284) */
/* 1414 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1416 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 1418 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1420) */
/* 1420 */	
			0x12, 0x0,	/* FC_UP */
/* 1422 */	NdrFcShort( 0xc ),	/* Offset= 12 (1434) */
/* 1424 */	
			0x15,		/* FC_STRUCT */
			0x1,		/* 1 */
/* 1426 */	NdrFcShort( 0x3e ),	/* 62 */
/* 1428 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1430 */	NdrFcShort( 0xfbea ),	/* Offset= -1046 (384) */
/* 1432 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1434 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 1436 */	NdrFcShort( 0x3e ),	/* 62 */
/* 1438 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 1440 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1442 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1444 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1446 */	NdrFcShort( 0xffea ),	/* Offset= -22 (1424) */
/* 1448 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1450 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 1452 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1454) */
/* 1454 */	
			0x12, 0x0,	/* FC_UP */
/* 1456 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1458) */
/* 1458 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 1460 */	NdrFcShort( 0x80 ),	/* 128 */
/* 1462 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 1464 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1466 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1468 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1470 */	NdrFcShort( 0xfa60 ),	/* Offset= -1440 (30) */
/* 1472 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1474 */	
			0x11, 0x0,	/* FC_RP */
/* 1476 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1478) */
/* 1478 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 1480 */	NdrFcShort( 0x3c ),	/* 60 */
/* 1482 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1484 */	NdrFcShort( 0x0 ),	/* Offset= 0 (1484) */
/* 1486 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 1488 */	0x3e,		/* FC_STRUCTPAD2 */
			0x8,		/* FC_LONG */
/* 1490 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 1492 */	0x3d,		/* FC_STRUCTPAD1 */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1494 */	0x0,		/* 0 */
			NdrFcShort( 0xfd1b ),	/* Offset= -741 (754) */
			0x3e,		/* FC_STRUCTPAD2 */
/* 1498 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1500 */	
			0x11, 0x0,	/* FC_RP */
/* 1502 */	NdrFcShort( 0x5c ),	/* Offset= 92 (1594) */
/* 1504 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 1506 */	NdrFcShort( 0xa0 ),	/* 160 */
/* 1508 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1510 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 1512 */	NdrFcShort( 0x64 ),	/* 100 */
/* 1514 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1516 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 1518 */	NdrFcShort( 0x138 ),	/* 312 */
/* 1520 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1522 */	NdrFcShort( 0x0 ),	/* Offset= 0 (1522) */
/* 1524 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 1526 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 1528 */	0x2,		/* FC_CHAR */
			0x3f,		/* FC_STRUCTPAD3 */
/* 1530 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 1532 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1534 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1536 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1538 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 1540 */	0x2,		/* FC_CHAR */
			0x3d,		/* FC_STRUCTPAD1 */
/* 1542 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1544 */	NdrFcShort( 0xffd8 ),	/* Offset= -40 (1504) */
/* 1546 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1548 */	NdrFcShort( 0xffda ),	/* Offset= -38 (1510) */
/* 1550 */	0x3e,		/* FC_STRUCTPAD2 */
			0x5b,		/* FC_END */
/* 1552 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 1554 */	NdrFcShort( 0x208 ),	/* 520 */
/* 1556 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1558 */	NdrFcShort( 0xfb5e ),	/* Offset= -1186 (372) */
/* 1560 */	0x3e,		/* FC_STRUCTPAD2 */
			0x8,		/* FC_LONG */
/* 1562 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 1564 */	
			0x15,		/* FC_STRUCT */
			0x1,		/* 1 */
/* 1566 */	NdrFcShort( 0x5fe ),	/* 1534 */
/* 1568 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1570 */	NdrFcShort( 0xfb52 ),	/* Offset= -1198 (372) */
/* 1572 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1574 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1576 */	NdrFcShort( 0xfd6a ),	/* Offset= -662 (914) */
/* 1578 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1580 */	NdrFcShort( 0xfd66 ),	/* Offset= -666 (914) */
/* 1582 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1584 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 1586 */	NdrFcShort( 0x5fe0 ),	/* 24544 */
/* 1588 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1590 */	NdrFcShort( 0xffe6 ),	/* Offset= -26 (1564) */
/* 1592 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1594 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x3,		/* 3 */
/* 1596 */	NdrFcShort( 0x6320 ),	/* 25376 */
/* 1598 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1600 */	NdrFcShort( 0x0 ),	/* Offset= 0 (1600) */
/* 1602 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1604 */	NdrFcShort( 0xffa8 ),	/* Offset= -88 (1516) */
/* 1606 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1608 */	NdrFcShort( 0xffc8 ),	/* Offset= -56 (1552) */
/* 1610 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1612 */	NdrFcShort( 0xffe4 ),	/* Offset= -28 (1584) */
/* 1614 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1616 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 1618 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1620) */
/* 1620 */	
			0x12, 0x0,	/* FC_UP */
/* 1622 */	NdrFcShort( 0x12 ),	/* Offset= 18 (1640) */
/* 1624 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 1626 */	NdrFcShort( 0x68 ),	/* 104 */
/* 1628 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 1630 */	0x6,		/* FC_SHORT */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1632 */	0x0,		/* 0 */
			NdrFcShort( 0xf9ad ),	/* Offset= -1619 (14) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1636 */	0x0,		/* 0 */
			NdrFcShort( 0xfb1b ),	/* Offset= -1253 (384) */
			0x5b,		/* FC_END */
/* 1640 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 1642 */	NdrFcShort( 0x68 ),	/* 104 */
/* 1644 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 1646 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 1648 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1650 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1652 */	NdrFcShort( 0xffe4 ),	/* Offset= -28 (1624) */
/* 1654 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1656 */	
			0x11, 0x0,	/* FC_RP */
/* 1658 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1660) */
/* 1660 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 1662 */	NdrFcShort( 0x6250 ),	/* 25168 */
/* 1664 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1666 */	NdrFcShort( 0xff8e ),	/* Offset= -114 (1552) */
/* 1668 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1670 */	NdrFcShort( 0xffaa ),	/* Offset= -86 (1584) */
/* 1672 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1674 */	NdrFcShort( 0xffce ),	/* Offset= -50 (1624) */
/* 1676 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1678 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 1680 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1682 */	0x40,		/* Corr desc:  constant, val=20 */
			0x0,		/* 0 */
/* 1684 */	NdrFcShort( 0x14 ),	/* 20 */
/* 1686 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1688 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1690 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 1692 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1694 */	0x40,		/* Corr desc:  constant, val=80 */
			0x0,		/* 0 */
/* 1696 */	NdrFcShort( 0x50 ),	/* 80 */
/* 1698 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1700 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1702 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 1704 */	NdrFcShort( 0x2 ),	/* 2 */
/* 1706 */	0x40,		/* Corr desc:  constant, val=50 */
			0x0,		/* 0 */
/* 1708 */	NdrFcShort( 0x32 ),	/* 50 */
/* 1710 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1712 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1714 */	
			0x11, 0x0,	/* FC_RP */
/* 1716 */	NdrFcShort( 0x8 ),	/* Offset= 8 (1724) */
/* 1718 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 1720 */	NdrFcShort( 0x16 ),	/* 22 */
/* 1722 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 1724 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 1726 */	NdrFcShort( 0x130 ),	/* 304 */
/* 1728 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1730 */	NdrFcShort( 0x0 ),	/* Offset= 0 (1730) */
/* 1732 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1734 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1736 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1738 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1740 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1742 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1744 */	0xc,		/* FC_DOUBLE */
			0x2,		/* FC_CHAR */
/* 1746 */	0x3d,		/* FC_STRUCTPAD1 */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1748 */	0x0,		/* 0 */
			NdrFcShort( 0xffe1 ),	/* Offset= -31 (1718) */
			0x2,		/* FC_CHAR */
/* 1752 */	0x3d,		/* FC_STRUCTPAD1 */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1754 */	0x0,		/* 0 */
			NdrFcShort( 0xffdb ),	/* Offset= -37 (1718) */
			0x2,		/* FC_CHAR */
/* 1758 */	0x3d,		/* FC_STRUCTPAD1 */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1760 */	0x0,		/* 0 */
			NdrFcShort( 0xffd5 ),	/* Offset= -43 (1718) */
			0x2,		/* FC_CHAR */
/* 1764 */	0x3d,		/* FC_STRUCTPAD1 */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1766 */	0x0,		/* 0 */
			NdrFcShort( 0xffcf ),	/* Offset= -49 (1718) */
			0x2,		/* FC_CHAR */
/* 1770 */	0x3d,		/* FC_STRUCTPAD1 */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1772 */	0x0,		/* 0 */
			NdrFcShort( 0xffc9 ),	/* Offset= -55 (1718) */
			0x2,		/* FC_CHAR */
/* 1776 */	0x2,		/* FC_CHAR */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1778 */	0x0,		/* 0 */
			NdrFcShort( 0xffc3 ),	/* Offset= -61 (1718) */
			0x2,		/* FC_CHAR */
/* 1782 */	0x3d,		/* FC_STRUCTPAD1 */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1784 */	0x0,		/* 0 */
			NdrFcShort( 0xffbd ),	/* Offset= -67 (1718) */
			0x2,		/* FC_CHAR */
/* 1788 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 1790 */	0x3d,		/* FC_STRUCTPAD1 */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1792 */	0x0,		/* 0 */
			NdrFcShort( 0xffb5 ),	/* Offset= -75 (1718) */
			0x2,		/* FC_CHAR */
/* 1796 */	0x41,		/* FC_STRUCTPAD5 */
			0x5b,		/* FC_END */
/* 1798 */	
			0x11, 0x0,	/* FC_RP */
/* 1800 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1802) */
/* 1802 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 1804 */	NdrFcShort( 0x140 ),	/* 320 */
/* 1806 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1808 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1810 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1812 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1814 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1816 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1818 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1820 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1822 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1824 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1826 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1828 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1830 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1832 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1834 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1836 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1838 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1840 */	0xc,		/* FC_DOUBLE */
			0x2,		/* FC_CHAR */
/* 1842 */	0x43,		/* FC_STRUCTPAD7 */
			0xc,		/* FC_DOUBLE */
/* 1844 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1846 */	0xc,		/* FC_DOUBLE */
			0x5b,		/* FC_END */
/* 1848 */	
			0x11, 0x0,	/* FC_RP */
/* 1850 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1852) */
/* 1852 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 1854 */	NdrFcShort( 0x288 ),	/* 648 */
/* 1856 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1858 */	NdrFcShort( 0xfa32 ),	/* Offset= -1486 (372) */
/* 1860 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1862 */	NdrFcShort( 0xf8d8 ),	/* Offset= -1832 (30) */
/* 1864 */	0x6,		/* FC_SHORT */
			0x2,		/* FC_CHAR */
/* 1866 */	0x2,		/* FC_CHAR */
			0x3e,		/* FC_STRUCTPAD2 */
/* 1868 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 1870 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 1872 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1874) */
/* 1874 */	
			0x12, 0x0,	/* FC_UP */
/* 1876 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1878) */
/* 1878 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x3,		/* 3 */
/* 1880 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1882 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 1884 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 1886 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 1888 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 1892 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 1894 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 1896 */	NdrFcShort( 0xf940 ),	/* Offset= -1728 (168) */
/* 1898 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 1900 */	
			0x11, 0x0,	/* FC_RP */
/* 1902 */	NdrFcShort( 0x2 ),	/* Offset= 2 (1904) */
/* 1904 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 1906 */	NdrFcShort( 0x2a08 ),	/* 10760 */
/* 1908 */	NdrFcShort( 0x0 ),	/* 0 */
/* 1910 */	NdrFcShort( 0x0 ),	/* Offset= 0 (1910) */
/* 1912 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1914 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1916 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1918 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1920 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1922 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1924 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1926 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1928 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1930 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1932 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1934 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1936 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 1938 */	0xc,		/* FC_DOUBLE */
			0x2,		/* FC_CHAR */
/* 1940 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 1942 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 1944 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 1946 */	0x2,		/* FC_CHAR */
			0x6,		/* FC_SHORT */
/* 1948 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1950 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1952 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 1954 */	0x6,		/* FC_SHORT */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1956 */	0x0,		/* 0 */
			NdrFcShort( 0xf939 ),	/* Offset= -1735 (222) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1960 */	0x0,		/* 0 */
			NdrFcShort( 0xf865 ),	/* Offset= -1947 (14) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1964 */	0x0,		/* 0 */
			NdrFcShort( 0xfbe5 ),	/* Offset= -1051 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1968 */	0x0,		/* 0 */
			NdrFcShort( 0xfbe1 ),	/* Offset= -1055 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1972 */	0x0,		/* 0 */
			NdrFcShort( 0xfbdd ),	/* Offset= -1059 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1976 */	0x0,		/* 0 */
			NdrFcShort( 0xfbd9 ),	/* Offset= -1063 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1980 */	0x0,		/* 0 */
			NdrFcShort( 0xfbd5 ),	/* Offset= -1067 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1984 */	0x0,		/* 0 */
			NdrFcShort( 0xfbd1 ),	/* Offset= -1071 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1988 */	0x0,		/* 0 */
			NdrFcShort( 0xfbcd ),	/* Offset= -1075 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1992 */	0x0,		/* 0 */
			NdrFcShort( 0xfbc9 ),	/* Offset= -1079 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 1996 */	0x0,		/* 0 */
			NdrFcShort( 0xfbc5 ),	/* Offset= -1083 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2000 */	0x0,		/* 0 */
			NdrFcShort( 0xfbc1 ),	/* Offset= -1087 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2004 */	0x0,		/* 0 */
			NdrFcShort( 0xfbbd ),	/* Offset= -1091 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2008 */	0x0,		/* 0 */
			NdrFcShort( 0xfbb9 ),	/* Offset= -1095 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2012 */	0x0,		/* 0 */
			NdrFcShort( 0xfbb5 ),	/* Offset= -1099 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2016 */	0x0,		/* 0 */
			NdrFcShort( 0xfbb1 ),	/* Offset= -1103 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2020 */	0x0,		/* 0 */
			NdrFcShort( 0xfbad ),	/* Offset= -1107 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2024 */	0x0,		/* 0 */
			NdrFcShort( 0xfba9 ),	/* Offset= -1111 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2028 */	0x0,		/* 0 */
			NdrFcShort( 0xfba5 ),	/* Offset= -1115 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2032 */	0x0,		/* 0 */
			NdrFcShort( 0xfba1 ),	/* Offset= -1119 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2036 */	0x0,		/* 0 */
			NdrFcShort( 0xfb9d ),	/* Offset= -1123 (914) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2040 */	0x0,		/* 0 */
			NdrFcShort( 0xfb99 ),	/* Offset= -1127 (914) */
			0xc,		/* FC_DOUBLE */
/* 2044 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2046 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2048 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2050 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2052 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2054 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2056 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2058 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2060 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2062 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2064 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2066 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2068 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2070 */	0x40,		/* FC_STRUCTPAD4 */
			0xc,		/* FC_DOUBLE */
/* 2072 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2074 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2076 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2078 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2080 */	0x40,		/* FC_STRUCTPAD4 */
			0xc,		/* FC_DOUBLE */
/* 2082 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2084 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2086 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2088 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2090 */	0x40,		/* FC_STRUCTPAD4 */
			0xc,		/* FC_DOUBLE */
/* 2092 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2094 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2096 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2098 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2100 */	0x40,		/* FC_STRUCTPAD4 */
			0x5b,		/* FC_END */
/* 2102 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/* 2104 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2106) */
/* 2106 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 2108 */	NdrFcShort( 0x30 ),	/* 48 */
/* 2110 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2112 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2114 */	NdrFcShort( 0xfc7a ),	/* Offset= -902 (1212) */
/* 2116 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2118 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2120 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2122 */	
			0x11, 0x4,	/* FC_RP [alloced_on_stack] */
/* 2124 */	NdrFcShort( 0xfc76 ),	/* Offset= -906 (1218) */
/* 2126 */	
			0x11, 0x0,	/* FC_RP */
/* 2128 */	NdrFcShort( 0xf8f8 ),	/* Offset= -1800 (328) */
/* 2130 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 2132 */	NdrFcShort( 0x2 ),	/* 2 */
/* 2134 */	0x40,		/* Corr desc:  constant, val=261 */
			0x0,		/* 0 */
/* 2136 */	NdrFcShort( 0x105 ),	/* 261 */
/* 2138 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 2140 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 2142 */	
			0x11, 0x0,	/* FC_RP */
/* 2144 */	NdrFcShort( 0xfbd4 ),	/* Offset= -1068 (1076) */
/* 2146 */	
			0x11, 0x8,	/* FC_RP [simple_pointer] */
/* 2148 */	0x8,		/* FC_LONG */
			0x5c,		/* FC_PAD */
/* 2150 */	
			0x11, 0x8,	/* FC_RP [simple_pointer] */
/* 2152 */	0x2,		/* FC_CHAR */
			0x5c,		/* FC_PAD */
/* 2154 */	
			0x11, 0x0,	/* FC_RP */
/* 2156 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2158) */
/* 2158 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 2160 */	NdrFcShort( 0x1 ),	/* 1 */
/* 2162 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x0,		/*  */
/* 2164 */	NdrFcShort( 0x18 ),	/* x86 Stack size/offset = 24 */
/* 2166 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2168 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 2170 */	
			0x11, 0x0,	/* FC_RP */
/* 2172 */	NdrFcShort( 0x1a ),	/* Offset= 26 (2198) */
/* 2174 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 2176 */	NdrFcShort( 0xa ),	/* 10 */
/* 2178 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 2180 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 2182 */	NdrFcShort( 0x28 ),	/* 40 */
/* 2184 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 2186 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 2188 */	NdrFcShort( 0x82 ),	/* 130 */
/* 2190 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 2192 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 2194 */	NdrFcShort( 0x6 ),	/* 6 */
/* 2196 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 2198 */	
			0x15,		/* FC_STRUCT */
			0x1,		/* 1 */
/* 2200 */	NdrFcShort( 0x274 ),	/* 628 */
/* 2202 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2204 */	NdrFcShort( 0xffe2 ),	/* Offset= -30 (2174) */
/* 2206 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2208 */	NdrFcShort( 0xffe4 ),	/* Offset= -28 (2180) */
/* 2210 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2212 */	NdrFcShort( 0xffe6 ),	/* Offset= -26 (2186) */
/* 2214 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2216 */	NdrFcShort( 0xf8d8 ),	/* Offset= -1832 (384) */
/* 2218 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2220 */	NdrFcShort( 0xf762 ),	/* Offset= -2206 (14) */
/* 2222 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2224 */	NdrFcShort( 0xffe0 ),	/* Offset= -32 (2192) */
/* 2226 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2228 */	NdrFcShort( 0xfa3e ),	/* Offset= -1474 (754) */
/* 2230 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2232 */	NdrFcShort( 0xfbaa ),	/* Offset= -1110 (1122) */
/* 2234 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2236 */	NdrFcShort( 0xfdfa ),	/* Offset= -518 (1718) */
/* 2238 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2240 */	NdrFcShort( 0xffc4 ),	/* Offset= -60 (2180) */
/* 2242 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2244 */	NdrFcShort( 0xfa2e ),	/* Offset= -1490 (754) */
/* 2246 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2248 */	NdrFcShort( 0xffbc ),	/* Offset= -68 (2180) */
/* 2250 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2252 */	
			0x29,		/* FC_WSTRING */
			0x5c,		/* FC_PAD */
/* 2254 */	NdrFcShort( 0x100 ),	/* 256 */
/* 2256 */	
			0x29,		/* FC_WSTRING */
			0x5c,		/* FC_PAD */
/* 2258 */	NdrFcShort( 0x12 ),	/* 18 */
/* 2260 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 2262 */	NdrFcShort( 0x2 ),	/* 2 */
/* 2264 */	0x40,		/* Corr desc:  constant, val=256 */
			0x0,		/* 0 */
/* 2266 */	NdrFcShort( 0x100 ),	/* 256 */
/* 2268 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 2270 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 2272 */	
			0x11, 0x0,	/* FC_RP */
/* 2274 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2276) */
/* 2276 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 2278 */	NdrFcShort( 0x1 ),	/* 1 */
/* 2280 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x0,		/*  */
/* 2282 */	NdrFcShort( 0x20 ),	/* x86 Stack size/offset = 32 */
/* 2284 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2286 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 2288 */	
			0x11, 0x0,	/* FC_RP */
/* 2290 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2292) */
/* 2292 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 2294 */	NdrFcShort( 0x104 ),	/* 260 */
/* 2296 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2298 */	NdrFcShort( 0xf724 ),	/* Offset= -2268 (30) */
/* 2300 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2302 */	NdrFcShort( 0xf720 ),	/* Offset= -2272 (30) */
/* 2304 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 2306 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 2308 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2310) */
/* 2310 */	
			0x12, 0x0,	/* FC_UP */
/* 2312 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2314) */
/* 2314 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 2316 */	NdrFcShort( 0x80 ),	/* 128 */
/* 2318 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 2320 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2322 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 2324 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2326 */	NdrFcShort( 0xf708 ),	/* Offset= -2296 (30) */
/* 2328 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2330 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 2332 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2334) */
/* 2334 */	
			0x12, 0x0,	/* FC_UP */
/* 2336 */	NdrFcShort( 0xc ),	/* Offset= 12 (2348) */
/* 2338 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 2340 */	NdrFcShort( 0x8 ),	/* 8 */
/* 2342 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 2344 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2346 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2348 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 2350 */	NdrFcShort( 0x8 ),	/* 8 */
/* 2352 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 2354 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2356 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 2358 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2360 */	NdrFcShort( 0xffea ),	/* Offset= -22 (2338) */
/* 2362 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2364 */	
			0x11, 0x0,	/* FC_RP */
/* 2366 */	NdrFcShort( 0xffe4 ),	/* Offset= -28 (2338) */
/* 2368 */	
			0x11, 0x0,	/* FC_RP */
/* 2370 */	NdrFcShort( 0xff92 ),	/* Offset= -110 (2260) */
/* 2372 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 2374 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2376) */
/* 2376 */	
			0x12, 0x0,	/* FC_UP */
/* 2378 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2380) */
/* 2380 */	
			0x1b,		/* FC_CARRAY */
			0x1,		/* 1 */
/* 2382 */	NdrFcShort( 0x80 ),	/* 128 */
/* 2384 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 2386 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2388 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 2390 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2392 */	NdrFcShort( 0xf6c6 ),	/* Offset= -2362 (30) */
/* 2394 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2396 */	
			0x11, 0x0,	/* FC_RP */
/* 2398 */	NdrFcShort( 0xf9a6 ),	/* Offset= -1626 (772) */
/* 2400 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 2402 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2404) */
/* 2404 */	
			0x12, 0x0,	/* FC_UP */
/* 2406 */	NdrFcShort( 0x14 ),	/* Offset= 20 (2426) */
/* 2408 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 2410 */	NdrFcShort( 0x80 ),	/* 128 */
/* 2412 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 2414 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 2416 */	NdrFcShort( 0x88 ),	/* 136 */
/* 2418 */	0x2,		/* FC_CHAR */
			0x3f,		/* FC_STRUCTPAD3 */
/* 2420 */	0x8,		/* FC_LONG */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2422 */	0x0,		/* 0 */
			NdrFcShort( 0xfff1 ),	/* Offset= -15 (2408) */
			0x5b,		/* FC_END */
/* 2426 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 2428 */	NdrFcShort( 0x88 ),	/* 136 */
/* 2430 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 2432 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2434 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 2436 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2438 */	NdrFcShort( 0xffe8 ),	/* Offset= -24 (2414) */
/* 2440 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2442 */	
			0x11, 0x0,	/* FC_RP */
/* 2444 */	NdrFcShort( 0xffe2 ),	/* Offset= -30 (2414) */
/* 2446 */	
			0x11, 0x0,	/* FC_RP */
/* 2448 */	NdrFcShort( 0x48 ),	/* Offset= 72 (2520) */
/* 2450 */	
			0x2b,		/* FC_NON_ENCAPSULATED_UNION */
			0x4,		/* FC_USMALL */
/* 2452 */	0x4,		/* Corr desc: FC_USMALL */
			0x0,		/*  */
/* 2454 */	NdrFcShort( 0x328 ),	/* 808 */
/* 2456 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2458 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2460) */
/* 2460 */	NdrFcShort( 0x328 ),	/* 808 */
/* 2462 */	NdrFcShort( 0x2 ),	/* 2 */
/* 2464 */	NdrFcLong( 0x0 ),	/* 0 */
/* 2468 */	NdrFcShort( 0x1e ),	/* Offset= 30 (2498) */
/* 2470 */	NdrFcLong( 0x1 ),	/* 1 */
/* 2474 */	NdrFcShort( 0x22 ),	/* Offset= 34 (2508) */
/* 2476 */	NdrFcShort( 0xffff ),	/* Offset= -1 (2475) */
/* 2478 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 2480 */	NdrFcShort( 0x10 ),	/* 16 */
/* 2482 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2484 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2486 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2488 */	
			0x1d,		/* FC_SMFARRAY */
			0x3,		/* 3 */
/* 2490 */	NdrFcShort( 0x320 ),	/* 800 */
/* 2492 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2494 */	NdrFcShort( 0xfff0 ),	/* Offset= -16 (2478) */
/* 2496 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2498 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 2500 */	NdrFcShort( 0x324 ),	/* 804 */
/* 2502 */	0x8,		/* FC_LONG */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2504 */	0x0,		/* 0 */
			NdrFcShort( 0xffef ),	/* Offset= -17 (2488) */
			0x5b,		/* FC_END */
/* 2508 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 2510 */	NdrFcShort( 0x28 ),	/* 40 */
/* 2512 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 2514 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2516 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2518 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2520 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 2522 */	NdrFcShort( 0x3b8 ),	/* 952 */
/* 2524 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2526 */	NdrFcShort( 0x0 ),	/* Offset= 0 (2526) */
/* 2528 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2530 */	NdrFcShort( 0xffb0 ),	/* Offset= -80 (2450) */
/* 2532 */	0x2,		/* FC_CHAR */
			0x3f,		/* FC_STRUCTPAD3 */
/* 2534 */	0x8,		/* FC_LONG */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2536 */	0x0,		/* 0 */
			NdrFcShort( 0xff7f ),	/* Offset= -129 (2408) */
			0x2,		/* FC_CHAR */
/* 2540 */	0x2,		/* FC_CHAR */
			0x3e,		/* FC_STRUCTPAD2 */
/* 2542 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 2544 */	
			0x11, 0x0,	/* FC_RP */
/* 2546 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2548) */
/* 2548 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 2550 */	NdrFcShort( 0x110 ),	/* 272 */
/* 2552 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2554 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2556 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2558 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2560 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2562 */	0x6,		/* FC_SHORT */
			0x3e,		/* FC_STRUCTPAD2 */
/* 2564 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2566 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2568 */	0x40,		/* FC_STRUCTPAD4 */
			0xc,		/* FC_DOUBLE */
/* 2570 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2572 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2574 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2576 */	0xc,		/* FC_DOUBLE */
			0x6,		/* FC_SHORT */
/* 2578 */	0x6,		/* FC_SHORT */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2580 */	0x0,		/* 0 */
			NdrFcShort( 0xfbd1 ),	/* Offset= -1071 (1510) */
			0x5b,		/* FC_END */
/* 2584 */	
			0x11, 0x0,	/* FC_RP */
/* 2586 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2588) */
/* 2588 */	
			0x1b,		/* FC_CARRAY */
			0x3,		/* 3 */
/* 2590 */	NdrFcShort( 0x10 ),	/* 16 */
/* 2592 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x0,		/*  */
/* 2594 */	NdrFcShort( 0x4 ),	/* x86 Stack size/offset = 4 */
/* 2596 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 2598 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2600 */	NdrFcShort( 0xfa0c ),	/* Offset= -1524 (1076) */
/* 2602 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2604 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 2606 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2608) */
/* 2608 */	
			0x12, 0x0,	/* FC_UP */
/* 2610 */	NdrFcShort( 0x2 ),	/* Offset= 2 (2612) */
/* 2612 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 2614 */	NdrFcShort( 0x1 ),	/* 1 */
/* 2616 */	0x29,		/* Corr desc:  parameter, FC_ULONG */
			0x54,		/* FC_DEREFERENCE */
/* 2618 */	NdrFcShort( 0x10 ),	/* x86 Stack size/offset = 16 */
/* 2620 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 2622 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */
/* 2624 */	
			0x11, 0x0,	/* FC_RP */
/* 2626 */	NdrFcShort( 0x24 ),	/* Offset= 36 (2662) */
/* 2628 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x7,		/* 7 */
/* 2630 */	NdrFcShort( 0x9 ),	/* 9 */
/* 2632 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 2636 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2638 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 2642 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2644 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2646 */	NdrFcShort( 0xf796 ),	/* Offset= -2154 (492) */
/* 2648 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2650 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 2652 */	NdrFcShort( 0x10 ),	/* 16 */
/* 2654 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2656 */	NdrFcShort( 0x0 ),	/* Offset= 0 (2656) */
/* 2658 */	0xc,		/* FC_DOUBLE */
			0x8,		/* FC_LONG */
/* 2660 */	0x40,		/* FC_STRUCTPAD4 */
			0x5b,		/* FC_END */
/* 2662 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 2664 */	NdrFcShort( 0x3a0 ),	/* 928 */
/* 2666 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2668 */	NdrFcShort( 0x0 ),	/* Offset= 0 (2668) */
/* 2670 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2672 */	NdrFcShort( 0xf66e ),	/* Offset= -2450 (222) */
/* 2674 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 2676 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2678 */	NdrFcShort( 0xffce ),	/* Offset= -50 (2628) */
/* 2680 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2682 */	NdrFcShort( 0xf7e0 ),	/* Offset= -2080 (602) */
/* 2684 */	0x8,		/* FC_LONG */
			0x2,		/* FC_CHAR */
/* 2686 */	0x2,		/* FC_CHAR */
			0x3e,		/* FC_STRUCTPAD2 */
/* 2688 */	0xc,		/* FC_DOUBLE */
			0x2,		/* FC_CHAR */
/* 2690 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2692 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2694 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2696 */	0x3d,		/* FC_STRUCTPAD1 */
			0xc,		/* FC_DOUBLE */
/* 2698 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2700 */	0xc,		/* FC_DOUBLE */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2702 */	0x0,		/* 0 */
			NdrFcShort( 0xf6eb ),	/* Offset= -2325 (378) */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2706 */	0x0,		/* 0 */
			NdrFcShort( 0xf6ed ),	/* Offset= -2323 (384) */
			0x3e,		/* FC_STRUCTPAD2 */
/* 2710 */	0x8,		/* FC_LONG */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 2712 */	0x0,		/* 0 */
			NdrFcShort( 0xffc1 ),	/* Offset= -63 (2650) */
			0xc,		/* FC_DOUBLE */
/* 2716 */	0xc,		/* FC_DOUBLE */
			0x5b,		/* FC_END */
/* 2718 */	
			0x11, 0x0,	/* FC_RP */
/* 2720 */	NdrFcShort( 0x80 ),	/* Offset= 128 (2848) */
/* 2722 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x7,		/* 7 */
/* 2724 */	NdrFcShort( 0x2 ),	/* 2 */
/* 2726 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 2730 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2732 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 2736 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2738 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2740 */	NdrFcShort( 0xf738 ),	/* Offset= -2248 (492) */
/* 2742 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2744 */	
			0x1d,		/* FC_SMFARRAY */
			0x1,		/* 1 */
/* 2746 */	NdrFcShort( 0xca ),	/* 202 */
/* 2748 */	0x5,		/* FC_WCHAR */
			0x5b,		/* FC_END */
/* 2750 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 2752 */	NdrFcShort( 0x20 ),	/* 32 */
/* 2754 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2756 */	0xc,		/* FC_DOUBLE */
			0x2,		/* FC_CHAR */
/* 2758 */	0x3d,		/* FC_STRUCTPAD1 */
			0x6,		/* FC_SHORT */
/* 2760 */	0x8,		/* FC_LONG */
			0x5b,		/* FC_END */
/* 2762 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 2764 */	NdrFcShort( 0x30 ),	/* 48 */
/* 2766 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2768 */	NdrFcShort( 0x0 ),	/* Offset= 0 (2768) */
/* 2770 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2772 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2774 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2776 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 2778 */	0x3e,		/* FC_STRUCTPAD2 */
			0x5b,		/* FC_END */
/* 2780 */	
			0x15,		/* FC_STRUCT */
			0x7,		/* 7 */
/* 2782 */	NdrFcShort( 0x20 ),	/* 32 */
/* 2784 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2786 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2788 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2790 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2792 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 2794 */	NdrFcShort( 0x38 ),	/* 56 */
/* 2796 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2798 */	NdrFcShort( 0x0 ),	/* Offset= 0 (2798) */
/* 2800 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2802 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 2804 */	0xc,		/* FC_DOUBLE */
			0x8,		/* FC_LONG */
/* 2806 */	0x40,		/* FC_STRUCTPAD4 */
			0xc,		/* FC_DOUBLE */
/* 2808 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 2810 */	0x6,		/* FC_SHORT */
			0x6,		/* FC_SHORT */
/* 2812 */	0x6,		/* FC_SHORT */
			0x40,		/* FC_STRUCTPAD4 */
/* 2814 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2816 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 2818 */	NdrFcShort( 0x70 ),	/* 112 */
/* 2820 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2822 */	NdrFcShort( 0x0 ),	/* Offset= 0 (2822) */
/* 2824 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2826 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2828 */	0x2,		/* FC_CHAR */
			0x43,		/* FC_STRUCTPAD7 */
/* 2830 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2832 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2834 */	0x2,		/* FC_CHAR */
			0x43,		/* FC_STRUCTPAD7 */
/* 2836 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2838 */	0xc,		/* FC_DOUBLE */
			0xc,		/* FC_DOUBLE */
/* 2840 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 2842 */	0x3e,		/* FC_STRUCTPAD2 */
			0x8,		/* FC_LONG */
/* 2844 */	0x6,		/* FC_SHORT */
			0x3e,		/* FC_STRUCTPAD2 */
/* 2846 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2848 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 2850 */	NdrFcShort( 0x3a8 ),	/* 936 */
/* 2852 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2854 */	NdrFcShort( 0x0 ),	/* Offset= 0 (2854) */
/* 2856 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2858 */	NdrFcShort( 0xf5b4 ),	/* Offset= -2636 (222) */
/* 2860 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 2862 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2864 */	NdrFcShort( 0xff72 ),	/* Offset= -142 (2722) */
/* 2866 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2868 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2870 */	NdrFcShort( 0xf644 ),	/* Offset= -2492 (378) */
/* 2872 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2874 */	NdrFcShort( 0xf646 ),	/* Offset= -2490 (384) */
/* 2876 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2878 */	NdrFcShort( 0xff7a ),	/* Offset= -134 (2744) */
/* 2880 */	0x2,		/* FC_CHAR */
			0x3d,		/* FC_STRUCTPAD1 */
/* 2882 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2884 */	NdrFcShort( 0xff74 ),	/* Offset= -140 (2744) */
/* 2886 */	0x2,		/* FC_CHAR */
			0x2,		/* FC_CHAR */
/* 2888 */	0x2,		/* FC_CHAR */
			0x3f,		/* FC_STRUCTPAD3 */
/* 2890 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 2892 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2894 */	NdrFcShort( 0xff70 ),	/* Offset= -144 (2750) */
/* 2896 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2898 */	NdrFcShort( 0xff78 ),	/* Offset= -136 (2762) */
/* 2900 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2902 */	NdrFcShort( 0xff86 ),	/* Offset= -122 (2780) */
/* 2904 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2906 */	NdrFcShort( 0xff8e ),	/* Offset= -114 (2792) */
/* 2908 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2910 */	NdrFcShort( 0xffa2 ),	/* Offset= -94 (2816) */
/* 2912 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 2914 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2916 */	
			0x11, 0x0,	/* FC_RP */
/* 2918 */	NdrFcShort( 0x18 ),	/* Offset= 24 (2942) */
/* 2920 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x7,		/* 7 */
/* 2922 */	NdrFcShort( 0x1b ),	/* 27 */
/* 2924 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 2928 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2930 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 2934 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2936 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2938 */	NdrFcShort( 0xf672 ),	/* Offset= -2446 (492) */
/* 2940 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2942 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 2944 */	NdrFcShort( 0x9f0 ),	/* 2544 */
/* 2946 */	NdrFcShort( 0x0 ),	/* 0 */
/* 2948 */	NdrFcShort( 0x0 ),	/* Offset= 0 (2948) */
/* 2950 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2952 */	NdrFcShort( 0xf556 ),	/* Offset= -2730 (222) */
/* 2954 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 2956 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2958 */	NdrFcShort( 0xffda ),	/* Offset= -38 (2920) */
/* 2960 */	0x2,		/* FC_CHAR */
			0x3d,		/* FC_STRUCTPAD1 */
/* 2962 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2964 */	NdrFcShort( 0xf5ec ),	/* Offset= -2580 (384) */
/* 2966 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2968 */	NdrFcShort( 0xf760 ),	/* Offset= -2208 (760) */
/* 2970 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 2972 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 2974 */	
			0x11, 0x0,	/* FC_RP */
/* 2976 */	NdrFcShort( 0x20 ),	/* Offset= 32 (3008) */
/* 2978 */	
			0x21,		/* FC_BOGUS_ARRAY */
			0x7,		/* 7 */
/* 2980 */	NdrFcShort( 0x3 ),	/* 3 */
/* 2982 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 2986 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2988 */	NdrFcLong( 0xffffffff ),	/* -1 */
/* 2992 */	NdrFcShort( 0x0 ),	/* Corr flags:  */
/* 2994 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 2996 */	NdrFcShort( 0xf638 ),	/* Offset= -2504 (492) */
/* 2998 */	0x5c,		/* FC_PAD */
			0x5b,		/* FC_END */
/* 3000 */	
			0x15,		/* FC_STRUCT */
			0x3,		/* 3 */
/* 3002 */	NdrFcShort( 0x8 ),	/* 8 */
/* 3004 */	0x8,		/* FC_LONG */
			0x6,		/* FC_SHORT */
/* 3006 */	0x6,		/* FC_SHORT */
			0x5b,		/* FC_END */
/* 3008 */	
			0x1a,		/* FC_BOGUS_STRUCT */
			0x7,		/* 7 */
/* 3010 */	NdrFcShort( 0x158 ),	/* 344 */
/* 3012 */	NdrFcShort( 0x0 ),	/* 0 */
/* 3014 */	NdrFcShort( 0x0 ),	/* Offset= 0 (3014) */
/* 3016 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 3018 */	NdrFcShort( 0xf514 ),	/* Offset= -2796 (222) */
/* 3020 */	0x8,		/* FC_LONG */
			0x40,		/* FC_STRUCTPAD4 */
/* 3022 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 3024 */	NdrFcShort( 0xffd2 ),	/* Offset= -46 (2978) */
/* 3026 */	0x4c,		/* FC_EMBEDDED_COMPLEX */
			0x0,		/* 0 */
/* 3028 */	NdrFcShort( 0xf5ac ),	/* Offset= -2644 (384) */
/* 3030 */	0x3e,		/* FC_STRUCTPAD2 */
			0x8,		/* FC_LONG */
/* 3032 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 3034 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 3036 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 3038 */	0x8,		/* FC_LONG */
			0x8,		/* FC_LONG */
/* 3040 */	0x8,		/* FC_LONG */
			0x4c,		/* FC_EMBEDDED_COMPLEX */
/* 3042 */	0x0,		/* 0 */
			NdrFcShort( 0xffd5 ),	/* Offset= -43 (3000) */
			0x5b,		/* FC_END */
/* 3046 */	
			0x11, 0x14,	/* FC_RP [alloced_on_stack] [pointer_deref] */
/* 3048 */	NdrFcShort( 0x2 ),	/* Offset= 2 (3050) */
/* 3050 */	
			0x12, 0x0,	/* FC_UP */
/* 3052 */	NdrFcShort( 0x2 ),	/* Offset= 2 (3054) */
/* 3054 */	
			0x1b,		/* FC_CARRAY */
			0x0,		/* 0 */
/* 3056 */	NdrFcShort( 0x1 ),	/* 1 */
/* 3058 */	0x27,		/* Corr desc:  parameter, FC_USHORT */
			0x54,		/* FC_DEREFERENCE */
/* 3060 */	NdrFcShort( 0x8 ),	/* x86 Stack size/offset = 8 */
/* 3062 */	NdrFcShort( 0x1 ),	/* Corr flags:  early, */
/* 3064 */	0x2,		/* FC_CHAR */
			0x5b,		/* FC_END */

			0x0
        }
    };

static const unsigned short dmlink_FormatStringOffsetTable[] =
    {
    0,
    34,
    80,
    144,
    190,
    284,
    348,
    394,
    458,
    528,
    600,
    672,
    742,
    814,
    872,
    942,
    982,
    1034,
    1092,
    1168,
    1232,
    1278,
    1342,
    1394,
    1514,
    1628,
    1686,
    1732,
    1784,
    1824,
    1882,
    1954,
    2026,
    2092,
    2156,
    2232,
    2298,
    2356,
    2414,
    2474,
    2532,
    2578,
    2648,
    2718,
    2778,
    2934,
    2986,
    3052,
    3124,
    3170,
    3228,
    3274,
    3358,
    3436,
    3508,
    3580,
    3628,
    3688,
    3728,
    3798,
    3844,
    3890,
    3954,
    4018,
    4058,
    4140,
    4228,
    4268,
    4350,
    4426,
    4490,
    4548,
    4594,
    4640,
    4686,
    4734,
    4780,
    4826,
    4872,
    4912,
    4958,
    4998,
    5044,
    5090,
    5136,
    5212,
    5258,
    5310,
    5350,
    5408,
    5472,
    5554,
    5624,
    5690,
    5762,
    5832,
    5898,
    5970,
    6040,
    6106,
    6178,
    6248,
    6314,
    6386,
    6450,
    6508,
    6572
    };


static const MIDL_STUB_DESC dmlink_StubDesc = 
    {
    (void *)& dmlink___RpcClientInterface,
    MIDL_user_allocate,
    MIDL_user_free,
    &dmlink__MIDL_AutoBindHandle,
    0,
    0,
    0,
    0,
    dmlink__MIDL_TypeFormatString.Format,
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
#pragma optimize("", on )
#if _MSC_VER >= 1200
#pragma warning(pop)
#endif


#endif /* !defined(_M_IA64) && !defined(_M_AMD64)*/

