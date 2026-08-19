

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


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

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__


#ifndef __link_h__
#define __link_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

/* header files for imported files */
#include "fmrpc.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __mvmntlink_INTERFACE_DEFINED__
#define __mvmntlink_INTERFACE_DEFINED__

/* interface mvmntlink */
/* [unique][version][uuid] */ 

#define	MVMNT_SERV_PIPE_NAME	( L"\\pipe\\mvmntserv" )

#define	MVMNT_SERV_SOCK_NAME	( L"10120" )

#define	NODE_NAME_LENGTH_SMALL	( 20 )

#define	NODE_NAME_LENGTH	( 36 )

#define	MOVE_NAME_LENGTH	( 20 )

#define	MOVE_GROUP_LENGTH	( 20 )

#define	MOVE_DELIVERYTICKET_NAME	( 80 )

#define	MOVE_PRINTER_NAME	( 80 )

#define	ORDER_LENGTH	( 20 )

#define	COMMENT_LENGTH	( 200 )

#define	THIRTY_LENGTH	( 30 )

#define	MAX_MVMNT_HISTORY_VARIABLES	( 152 )

#define	MAX_MVMNT_SUMMARY_VARIABLES	( 102 )

#define	MAX_MVMNT_VARIABLES	( 152 )

#define	MAX_EMPLOYEEID_LENGTH	( 50 )

#define	MAX_STRING_LENGTH	( 201 )

typedef /* [context_handle] */ void *MVMNT_DATA_CONTEXT_HANDLE;

typedef /* [public] */ struct __MIDL_mvmntlink_0001
    {
    wchar_t szUserName[ 21 ];
    } 	MVMNT_DATA_CONTEXT;

typedef /* [public] */ struct __MIDL_mvmntlink_0002
    {
    unsigned short SRC;
    unsigned short DEST;
    } 	MVMNT_CTRL_BLK;

typedef /* [allocate] */ struct __MIDL_mvmntlink_0002 *PMVMNT_CTRL_BLK;

#define	NODE_TYPE_NONE	( 0 )

#define	NODE_TYPE_TANK	( 1 )

#define	NODE_TYPE_USER	( 2 )

#define	NODE_TYPE_METER	( 3 )

#define	NODE_TYPE_HANDGAUGE	( 4 )

#define	MOVE_TANK_ID	( 0 )

#define	MOVE_XFR_GROSS_VOL	( 0 )

#define	MOVE_XFR_NET_VOL	( 1 )

#define	MOVE_XFR_FLOW	( 2 )

#define	MOVE_XFR_TIME_REM	( 3 )

#define	MOVE_XFR_MASS	( 4 )

#define	MOVE_XFR_PRODUCT	( 5 )

#define	MOVE_XFR_TEMP	( 6 )

#define	MOVE_XFR_DENSITY	( 7 )

#define	MOVE_XFR_LEVEL	( 8 )

#define	MOVE_XFR_STATUS	( 9 )

#define	MOVE_XFR_WATER_LEVEL	( 10 )

#define	MOVE_XFR_WATER_VOL	( 11 )

#define	MOVE_XFR_MODE	( 12 )

#define	MOVE_XFR_SP	( 13 )

#define	MOVE_XFR_VCF	( 14 )

#define	MOVE_XFR_CTSH	( 15 )

#define	MOVE_XFR_ROOFVOLUME	( 16 )

#define	MOVE_XFR_STDDENSITY	( 17 )

#define	MOVE_XFR_STRAPVOLUME	( 18 )

#define	MOVE_XFR_AMBIENTTEMP	( 19 )

#define	MOVE_XFR_DENSITYTEMP	( 20 )

#define	MOVE_XFR_ROOFMASS	( 21 )

#define	MOVE_XFR_BSW	( 22 )

#define	MOVE_XFR_BSW_VOL	( 23 )

#define	MOVE_XFR_PRODAIR_DENSITY	( 24 )

#define	MOVE_XFR_AIR_STDDENSITY	( 25 )

#define	MOVE_XFR_GSV	( 26 )

#define	MOVE_NUM_VARS	( 27 )

#define	METER_DATA_SOURCES	( 5 )

#define	METER_GROSS_POINT	( 0 )

#define	METER_NET_POINT	( 1 )

#define	METER_MASS_POINT	( 2 )

#define	METER_FLOW_POINT	( 3 )

#define	METER_RESET_POINT	( 4 )

typedef /* [public] */ struct __MIDL_mvmntlink_0003
    {
    unsigned long NodeType	: 10;
    unsigned long bGrossAssigned	: 1;
    unsigned long bNetAssigned	: 1;
    unsigned long bMassAssigned	: 1;
    unsigned long bFlowAssigned	: 1;
    unsigned long bResetAssigned	: 1;
    unsigned long bResetable	: 1;
    unsigned long bReserved	: 4;
    unsigned long wUsedCount	: 12;
    } 	ENDNODEFLAGS;

typedef struct __MIDL_mvmntlink_0003 *PENDNODEFLAGS;

typedef /* [public][public][public][public][public] */ struct __MIDL_mvmntlink_0004
    {
    unsigned long dwPointID[ 5 ];
    unsigned long dwEndNodeFlags;
    wchar_t szEndNodeName[ 37 ];
    unsigned long dwVolumeRollover;
    unsigned long dwMassRollover;
    unsigned short wNodeID;
    } 	ENDNODE;

typedef struct __MIDL_mvmntlink_0004 *PENDNODE;

typedef /* [public] */ struct __MIDL_mvmntlink_0005
    {
    unsigned char bNumberVariables;
    unsigned char bVariable[ 152 ];
    } 	MOVELINEFORMAT;

typedef struct __MIDL_mvmntlink_0005 *PMOVELINEFORMAT;

typedef /* [public] */ struct __MIDL_mvmntlink_0006
    {
    wchar_t szAlias[ 152 ][ 31 ];
    } 	MOVELINEALIASFORMAT_V70;

typedef struct __MIDL_mvmntlink_0006 *PMOVELINEALIASFORMAT_V70;

typedef /* [public] */ struct __MIDL_mvmntlink_0007
    {
    wchar_t szAlias[ 152 ][ 31 ];
    unsigned char bSelectedUnit1[ 152 ];
    wchar_t szUnit1[ 152 ][ 21 ];
    unsigned char bSelectedUnit2[ 152 ];
    wchar_t szUnit2[ 152 ][ 21 ];
    unsigned char bFieldWidth[ 152 ];
    unsigned char bFieldPrecision[ 152 ];
    unsigned char bUseRegionalSettings[ 152 ];
    } 	MOVELINEALIASFORMAT;

typedef struct __MIDL_mvmntlink_0007 *PMOVELINEALIASFORMAT;

typedef /* [public] */ struct __MIDL_mvmntlink_0008
    {
    unsigned long dwActionID;
    unsigned short wInstCount;
    unsigned short wSourceID;
    unsigned short wDestID;
    unsigned short wSrcTankMode;
    unsigned short wSrcXfrMode;
    double dSrcXfrSetpoint;
    unsigned char bSrcXfrSetpointUnits;
    unsigned char bSrcXfrSetpointStyle;
    unsigned char bSrcXfrSetpointValid;
    unsigned short wDestTankMode;
    unsigned short wDestXfrMode;
    double dDestXfrSetpoint;
    unsigned char bDestXfrSetpointUnits;
    unsigned char bDestXfrSetpointStyle;
    unsigned char bDestXfrSetpointValid;
    } 	MOVEMENT;

typedef struct __MIDL_mvmntlink_0008 *PMOVEMENT;

typedef /* [public][public][public][public][public] */ struct __MIDL_mvmntlink_0009
    {
    wchar_t szName[ 37 ];
    wchar_t szNameOld[ 37 ];
    unsigned short wNodeID;
    unsigned char bType;
    unsigned char bSource;
    unsigned char bSetNamePerm;
    unsigned char bSetXfrModePerm;
    unsigned char bSetXfrModeInactivePerm;
    unsigned char bSetSetpointPerm;
    unsigned char bSetSetpointPercentPerm;
    unsigned char bCombined;
    unsigned char bSetpointDataValid;
    unsigned char bRangeDataValid;
    unsigned char bPercentDataValid;
    unsigned char bReferenceGrossValid;
    unsigned char bReferenceMassValid;
    unsigned short wXfrMode;
    unsigned short wXfrModeOld;
    double dXfrSetpoint;
    double dXfrSetpointOld;
    double dXfrSetpointInPercent;
    double dXfrSetpointInPercentOld;
    unsigned char bXfrSetpointUnits;
    unsigned char bXfrSetpointStyle;
    double dXfrSetpointMax;
    double dXfrSetpointMin;
    double dXfrSetpointInPercentMax;
    double dXfrSetpointInPercentMin;
    double dXfrReferenceGross;
    double dXfrReferenceMass;
    unsigned char bStatus;
    wchar_t szTankDataBaseReference[ 129 ];
    wchar_t szMeterGrossReference[ 129 ];
    wchar_t szMeterNetReference[ 129 ];
    wchar_t szMeterMassReference[ 129 ];
    } 	NODEINSTANCEDATA;

typedef struct __MIDL_mvmntlink_0009 *PNODEINSTANCEDATA;

typedef /* [public] */ struct __MIDL_mvmntlink_0010
    {
    wchar_t szName[ 21 ];
    wchar_t szOrder[ 21 ];
    wchar_t szComment[ 201 ];
    wchar_t szUserDef[ 10 ][ 31 ];
    wchar_t szGroup[ 21 ];
    wchar_t szReportName[ 81 ];
    wchar_t szPrinterName[ 81 ];
    wchar_t szInputPoint[ 81 ];
    long lPlannedStartTime;
    unsigned char bPlannedStartTimeOperational;
    int wPlannedStartTimeStatus;
    long lAutoStartTime;
    unsigned char bAutoStartTimeActive;
    int wAutoStartTimeStatus;
    long lAutoStopTime;
    unsigned char bAutoStopTimeActive;
    int wAutoStopTimeStatus;
    unsigned short wZeroFlowHoldOffMinutes;
    unsigned char bType;
    unsigned char bCommit;
    unsigned char bOkPerm;
    unsigned char bOrderPerm;
    unsigned char bSourceSetpointsInPercentPerm;
    unsigned char bSourceSetpointsInPercent;
    unsigned char bSourceSetpointsInPercentOld;
    unsigned char bAutoDelete;
    unsigned char bStartOnNonZeroFlow;
    unsigned char bStopOnZeroFlow;
    unsigned char bInterlockSetpoints;
    unsigned char bIncludeHandValues;
    unsigned char bLineupActionSequence;
    unsigned char bLineupActionSequencePerm;
    unsigned char bHaltOnCompletion;
    unsigned char bInhibitSetpointOverrange;
    unsigned char bInhibitMovementType;
    unsigned char bIndividualNodeControl;
    unsigned char bUsePendingOperation;
    unsigned char bUseInputPoint;
    unsigned char bSendMvmntToSnapIn;
    unsigned char bMvmntToSnapInAvailable;
    unsigned long dwInitiationCount;
    long tInitiationTime;
    unsigned short wNumberOfNodes;
    NODEINSTANCEDATA *pNodeInstanceData;
    } 	MOVEINSTANCEDATA;

typedef struct __MIDL_mvmntlink_0010 *PMOVEINSTANCEDATA;

typedef /* [public] */ struct __MIDL_mvmntlink_0011
    {
    long lStartTime;
    double dStartLevel;
    double dStartLevelMax;
    double dStartLevelMin;
    double dStartGross;
    double dStartGrossMax;
    double dStartGrossMin;
    double dStartNet;
    double dStartNetMax;
    double dStartNetMin;
    double dStartMass;
    double dStartMassMax;
    double dStartMassMin;
    double dStartWaterLevel;
    double dStartWaterLevelMax;
    double dStartWaterLevelMin;
    double dStartWaterVol;
    double dStartWaterVolMax;
    double dStartWaterVolMin;
    double dStartTemp;
    double dStartTempMax;
    double dStartTempMin;
    double dStartDensity;
    double dStartDensityMax;
    double dStartDensityMin;
    double dStartDensityTemp;
    double dStartDensityTempMax;
    double dStartDensityTempMin;
    double dStartStrapVol;
    double dStartStrapVolMax;
    double dStartStrapVolMin;
    double dStartStdDensity;
    double dStartStdDensityMax;
    double dStartStdDensityMin;
    double dStartVCF;
    double dStartVCFMax;
    double dStartVCFMin;
    double dStartCTSh;
    double dStartCTShMax;
    double dStartCTShMin;
    double dStartAmbTemp;
    double dStartAmbTempMax;
    double dStartAmbTempMin;
    double dStartRoofMass;
    double dStartBSW;
    double dStartBSWMax;
    double dStartBSWMin;
    unsigned char bStartLevelUnit;
    unsigned char bStartGrossUnit;
    unsigned char bStartNetUnit;
    unsigned char bStartMassUnit;
    unsigned char bStartWaterLevelUnit;
    unsigned char bStartWaterVolUnit;
    unsigned char bStartTempUnit;
    unsigned char bStartDensityUnit;
    unsigned char bStartDensityTempUnit;
    unsigned char bStartStrapVolUnit;
    unsigned char bStartStdDensityUnit;
    unsigned char bStartVCFUnit;
    unsigned char bStartCTShUnit;
    unsigned char bStartAmbTempUnit;
    unsigned char bStartBSWUnit;
    LEVELNAMES lvlTagName;
    ENDNODE Node;
    } 	STARTDATAGET;

typedef struct __MIDL_mvmntlink_0011 *PSTARTDATAGET;

typedef /* [public] */ struct __MIDL_mvmntlink_0012
    {
    long lStartTime;
    double dStartLevel;
    double dStartGross;
    double dStartNet;
    double dStartMass;
    double dStartWaterLevel;
    double dStartWaterVol;
    double dStartTemperature;
    double dStartDensity;
    double dStartStrapVol;
    double dStartStdDensity;
    double dStartVCF;
    double dStartCtsh;
    double dStartAmbTemp;
    double dStartBSW;
    long lStartDataMod;
    wchar_t szDataModName[ 20 ];
    } 	STARTDATAPUT;

typedef struct __MIDL_mvmntlink_0012 *PSTARTDATAPUT;

typedef /* [public] */ struct __MIDL_mvmntlink_0013
    {
    wchar_t szName[ 21 ];
    } 	MOVEMENTGROUP;

typedef struct __MIDL_mvmntlink_0013 *PMOVEMENTGROUP;

typedef /* [public] */ struct __MIDL_mvmntlink_0014
    {
    wchar_t szName[ 21 ];
    } 	MOVEMENTNAME;

typedef struct __MIDL_mvmntlink_0014 *PMOVEMENTNAME;

typedef /* [public] */ struct __MIDL_mvmntlink_0015
    {
    wchar_t szName[ 37 ];
    unsigned long dwProductIndex;
    } 	NODEPRODUCTDATA;

typedef struct __MIDL_mvmntlink_0015 *PNODEPRODUCTDATA;

typedef /* [public] */ struct __MIDL_mvmntlink_0016
    {
    unsigned char bLen;
    wchar_t *szName;
    } 	MOVEMENTTYPEDATA;

typedef struct __MIDL_mvmntlink_0016 *PMOVEMENTTYPEDATA;

typedef /* [public][public] */ struct __MIDL_mvmntlink_0017
    {
    wchar_t szReportName[ 261 ];
    } 	DELIVERYTICKETNAME;

typedef struct __MIDL_mvmntlink_0017 *PDELIVERYTICKETNAME;

typedef /* [public][public] */ struct __MIDL_mvmntlink_0018
    {
    unsigned char byLen;
    unsigned char byDefault;
    wchar_t *szName;
    } 	PRINTERDATA;

typedef struct __MIDL_mvmntlink_0018 *PPRINTERDATA;

typedef /* [public] */ struct __MIDL_mvmntlink_0019
    {
    unsigned long dwLevelBlockStatus;
    unsigned long dwGrossVolumeBlockStatus;
    unsigned long dwNetVolumeBlockStatus;
    unsigned long dwMassBlockStatus;
    unsigned long dwWaterLevelBlockStatus;
    unsigned long dwWaterVolumeBlockStatus;
    unsigned long dwTemperatureBlockStatus;
    unsigned long dwDensityBlockStatus;
    unsigned long dwVCFBlockStatus;
    unsigned long dwCTShBlockStatus;
    unsigned long dwRoofVolumeBlockStatus;
    unsigned long dwStdDensityBlockStatus;
    unsigned long dwStrapVolumeBlockStatus;
    unsigned long dwAmbientTempBlockStatus;
    unsigned long dwDensityTempBlockStatus;
    unsigned long dwRoofMassBlockStatus;
    } 	MVMNTBLOCKSTATUS;

typedef struct __MIDL_mvmntlink_0019 *PMVMNTBLOCKSTATUS;

typedef /* [public] */ struct __MIDL_mvmntlink_0020
    {
    double dLevel;
    long LevelTime;
    double dTemperature;
    long TemperatureTime;
    double dDensity;
    long DensityTime;
    double dDensityTemp;
    long DensityTempTime;
    double dAmbientTemp;
    long AmbientTempTime;
    double dRefHeight;
    long RefHeightTime;
    double dWaterLevel;
    long WaterLevelTime;
    double dStrapVolume;
    double dWaterVolume;
    double dGrossVolume;
    double dStdDensity;
    double dVCF;
    double dCTSh;
    double dNetVolume;
    double dMass;
    double dRoofMass;
    wchar_t szEmployeeID[ 50 ];
    LEVELNAMES TagName;
    } 	HANDGAUGEDATA;

typedef struct __MIDL_mvmntlink_0020 *PHANDGAUGEDATA;

typedef /* [public] */ struct __MIDL_mvmntlink_0021
    {
    double dLevel;
    long LevelTime;
    double dTemperature;
    long TemperatureTime;
    double dDensity;
    long DensityTime;
    double dDensityTemp;
    long DensityTempTime;
    double dAmbientTemp;
    long AmbientTempTime;
    double dRefHeight;
    long RefHeightTime;
    double dWaterLevel;
    long WaterLevelTime;
    double dStrapVolume;
    double dWaterVolume;
    double dGrossVolume;
    double dStdDensity;
    double dVCF;
    double dCTSh;
    double dNetVolume;
    double dMass;
    wchar_t szEmployeeID[ 50 ];
    LEVELNAMES TagName;
    } 	HANDGAUGEDATA_432;

typedef struct __MIDL_mvmntlink_0021 *PHANDGAUGEDATA_432;

unsigned long MvmntEditStart( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [out] */ MVMNT_DATA_CONTEXT_HANDLE *phContext,
    /* [out] */ unsigned char *pbCheckProducts,
    /* [out] */ unsigned char *pbClosingHour,
    /* [out] */ unsigned char *pbClosingMinute,
    /* [out] */ unsigned short *pwHistoryLogPeriod,
    /* [size_is][out] */ wchar_t pUser[  ]);

unsigned long MvmntEditDone( 
    /* [out][in] */ MVMNT_DATA_CONTEXT_HANDLE *phContext,
    /* [in] */ unsigned char bCheckProducts,
    /* [in] */ unsigned char bClosingHour,
    /* [in] */ unsigned char bClosingMinute,
    /* [in] */ unsigned short wHistoryLogPeriod);

unsigned long MvmntGetGroups( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumGroups,
    /* [size_is][size_is][out] */ PMOVEMENTGROUP *ppMovementGroup);

unsigned long MvmntAddGroup( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PMOVEMENTGROUP pMovementGroup);

unsigned long MvmntUpdateGroup( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PMOVEMENTGROUP pOldMovementGroup,
    /* [in] */ PMOVEMENTGROUP pNewMovementGroup);

unsigned long MvmntDeleteGroup( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PMOVEMENTGROUP pMovementGroup);

unsigned long MvmntGetEndNodeArray( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumPnt,
    /* [size_is][size_is][out] */ ENDNODE **pEndNode);

unsigned long MvmntGetNodeData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned short wNodeId,
    /* [out] */ PENDNODE pNodeData);

unsigned long MvmntAddEndNode( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ PENDNODE pNewNode,
    /* [out] */ unsigned short *pNewId);

unsigned long MvmntDeleteEndNode( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ unsigned short wNodeId);

unsigned long MvmntGetNodeName( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned short wNodeId,
    /* [size_is][out] */ wchar_t pName[  ],
    /* [out] */ unsigned short *pwNodeType);

unsigned long MvmntUpdateEndNode( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ PENDNODE pNewPoint);

unsigned long MvmntGetNodeID( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pNodeName,
    /* [out] */ unsigned short *pNodeId);

unsigned long MvmntAddMovement( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ PMOVEMENT pNewMove);

unsigned long MvmntDeleteMovement( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ PMOVEMENT pDelMove);

unsigned long MvmntReplaceMove( 
    /* [in] */ MVMNT_DATA_CONTEXT_HANDLE hContext,
    /* [in] */ PMOVEMENT pOldMove,
    /* [in] */ PMOVEMENT pNewMove);

unsigned long MvmntGetMoveSources( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumSrc,
    /* [size_is][size_is][out] */ PENDNODE *ppEndNodeSrc,
    /* [size_is][size_is][out] */ unsigned short **ppwDestCount);

unsigned long MvmntGetMoveDestinations( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumDest,
    /* [size_is][size_is][out] */ PENDNODE *ppEndNodeDest,
    /* [size_is][size_is][out] */ unsigned short **ppwSrcCount);

unsigned long MvmntGetMoveDestinationsForSource( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned short wSrc,
    /* [out] */ unsigned long *pdwNumDest,
    /* [size_is][size_is][out] */ PENDNODE *ppEndNodeDest,
    /* [size_is][size_is][out] */ unsigned short **ppwInstCount);

unsigned long MvmntGetMoveSourcesForDestination( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned short wDest,
    /* [out] */ unsigned long *pdwNumSrc,
    /* [size_is][size_is][out] */ PENDNODE *ppEndNodeSrc,
    /* [size_is][size_is][out] */ unsigned short **ppwInstCount);

unsigned long MvmntAddMoveInst( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUser,
    /* [in] */ unsigned char bType,
    /* [out] */ unsigned long *pdwMoveInstID);

unsigned long MvmntDeleteMoveInst( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID);

unsigned long MvmntGetMoveInst( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [out] */ PMOVEINSTANCEDATA *ppMoveInstanceData);

unsigned long MvmntSetMoveInst( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pSystem,
    /* [string][in] */ wchar_t *pUser,
    /* [in] */ unsigned long dwMoveInstID,
    /* [out][in] */ PMOVEINSTANCEDATA *ppMoveInstanceData);

unsigned long MvmntGetMovementTypeData( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumTypes,
    /* [size_is][size_is][out] */ PMOVEMENTTYPEDATA *ppMovementTypeData);

unsigned long MvmntMoveCommand( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pSystem,
    /* [string][in] */ wchar_t *pUser,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wMoveNodeID,
    /* [in] */ unsigned short wCommand,
    /* [out] */ long *lMoveTimeStamp);

/* [callback] */ unsigned long MvmntCombinationWarning( 
    /* [in] */ PMOVEMENTNAME pMovementName,
    /* [in] */ unsigned short wNameCount,
    /* [size_is][in] */ PMOVEMENTNAME pCombineName);

/* [callback] */ unsigned long MvmntContaminateWarning( 
    /* [in] */ PMOVEMENTNAME pMovementName,
    /* [in] */ PNODEPRODUCTDATA pNodeProductData,
    /* [in] */ unsigned short wNodeCount,
    /* [size_is][in] */ PNODEPRODUCTDATA pContaminateData);

/* [callback] */ unsigned long MvmntAddUnitNodeWarning( 
    /* [in] */ PMOVEMENTNAME pMovementName,
    /* [string][in] */ wchar_t *szNodeName);

/* [callback] */ unsigned long MvmntLockoutWarning( 
    /* [in] */ PMOVEMENTNAME pMovementName);

/* [callback] */ unsigned long MvmntSetupWarning( 
    /* [in] */ PMOVEMENTNAME pMovementName);

/* [callback] */ unsigned long MvmntChangeTransferWarning( 
    /* [string][in] */ wchar_t *szNodeName,
    /* [in] */ unsigned short wNewTankMode,
    /* [in] */ unsigned short wOldTankMode,
    /* [in] */ unsigned short wNewXfrMode,
    /* [in] */ unsigned short wOldXfrMode,
    /* [in] */ double dNewXfrSetpoint,
    /* [in] */ unsigned char bNewXfrSetpointStyle,
    /* [in] */ double dOldXfrSetpoint,
    /* [in] */ unsigned char bOldXfrSetpointStyle);

unsigned long MvmntGetStartData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wNodeInstID,
    /* [out] */ PSTARTDATAGET pStartDataGet);

unsigned long MvmntPutStartData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wNodeInstID,
    /* [in] */ PSTARTDATAPUT pStartDataPut);

unsigned long MvmntGetCloseOutTime( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned short *wHour,
    /* [out] */ unsigned short *wMinute);

unsigned long MvmntGetServerDeliveryTicketNames( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumReports,
    /* [size_is][size_is][out] */ DELIVERYTICKETNAME **pReportStrings);

unsigned long MvmntGetServerPrinterNames( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned long *pdwNumPrinters,
    /* [size_is][size_is][out] */ PRINTERDATA **pPrinterNames);

unsigned long MvmntGetStartDataBlockStatus( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wNodeInstID,
    /* [out] */ PMVMNTBLOCKSTATUS pStartDataBlockStatus);

unsigned long MvmntGetHandGaugeData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wNodeInstID,
    /* [in] */ unsigned char bStartData,
    /* [out] */ PHANDGAUGEDATA pHandGaugeData);

unsigned long MvmntPutHandGaugeData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwMoveInstID,
    /* [in] */ unsigned short wNodeInstID,
    /* [in] */ unsigned char bStartData,
    /* [in] */ PHANDGAUGEDATA pHandGaugeData);

unsigned long MvmntSetClearDescrencyInPercent( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bEnable);

unsigned long MvmntGetDescrencyInPercent( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned char *bEnable);

unsigned long MvmntHistoryDataModified( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PMOVEMENTNAME pMovementName,
    /* [in] */ unsigned long dwMoveInstID);



extern RPC_IF_HANDLE mvmntlink_ClientIfHandle;
extern RPC_IF_HANDLE mvmntlink_ServerIfHandle;
#endif /* __mvmntlink_INTERFACE_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

void __RPC_USER MVMNT_DATA_CONTEXT_HANDLE_rundown( MVMNT_DATA_CONTEXT_HANDLE );

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


