

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


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


#ifndef __dmlink_h__
#define __dmlink_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

/* header files for imported files */
#include "fmrpc.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __dmlink_INTERFACE_DEFINED__
#define __dmlink_INTERFACE_DEFINED__

/* interface dmlink */
/* [unique][version][uuid] */ 

typedef /* [context_handle] */ void *PDM_LOCK_CTXT_HNDL;

typedef double DATE;

#define	NO_OF_FLOWMETER_PVS_76	( 7 )

#define	NO_OF_TANKPT_PVS_73	( 57 )

#define	NO_OF_TANKPT_PVS_72	( 56 )

#define	NO_OF_PVS_45	( 52 )

#define	NO_OF_TANKPT_PVS_78	( 58 )

#define	NO_OF_PIPELINE_PVS	( 2 )

#define	NO_OF_FLOWMETER_PVS	( 9 )

#define	NO_OF_TANKPT_PVS	( 64 )

#define	NO_OF_LOGICPNT_PVS	( 27 )

#define	NO_OF_TIMERPNT_PVS	( 3 )

#define	MAX_DESCRIPTION_LENGTH	( 30 )

#define	MAX_DETAIL_LENGTH	( 20 )

#define	MAX_ALARM_DESCRIPTION	( 30 )

#define	MAX_ALARM_NAME_LEN	( 15 )

#define	MAX_LINING_LENGTH	( 40 )

#define	MAX_CST_MANNAME_LENGTH	( 255 )

#define	MAX_FORMULA_LENGTH	( 255 )

#define	INVALID_HANDLE	( -1 )

#define	NO_OF_POLYNOMIALSPHEREENTRIES	( 4 )

#define	NO_OF_POLYNOMIALSPHEREEQUATIONS	( 8 )

#define	LEVEL_COLUMN	( 0 )

#define	CONSTANT_Q_COLUMN	( 1 )

#define	CONSTANT_R_COLUMN	( 2 )

#define	CONSTANT_S_COLUMN	( 3 )

#define	DB_SOURCE_NONE	( 0 )

#define	DB_SOURCE_COMM	( 1 )

#define	DB_SOURCE_MANUAL	( 2 )

#define	DB_SOURCE_LOCAL	( 3 )

#define	DB_SOURCE_APP	( 4 )

#define	DB_SOURCE_ALARM	( 5 )

#define	DB_SOURCE_CALC	( 6 )

#define	DB_SOURCE_REM	( 7 )

#define	DB_SOURCE_OPC	( 8 )

#define	DB_SOURCE_OPCUA	( 9 )

#define	DB_MON_NONE	( 0 )

#define	DB_MON_ADDPT	( 1 )

#define	DB_MON_EDITPT	( 2 )

#define	DB_MON_ALARM	( 3 )

#define	DB_MON_TANKCALC	( 4 )

#define	DB_ED_ERR_NONE	( 0 )

#define	DB_ED_ERR_RPCTIMEOUT	( -1 )

#define	DB_ED_ERR_RPCMUTEX	( -2 )

#define	DB_ED_ERR_LOCKED	( -3 )

#define	DB_ED_ERR_EXISTING	( -4 )

#define	DB_ED_ERR_LEV	( -5 )

#define	DB_ED_ERR_STRING	( -6 )

#define	DB_ED_ERR_NOTAGS	( -7 )

#define	DB_ED_ERR_NOTFOUND	( -8 )

#define	DB_ED_ERR_FULL	( -9 )

#define	DB_ED_ERR_NOMEM	( -10 )

#define	DB_ED_ERR_FILE	( -11 )

#define	DB_ED_ERR_NOUSER	( -12 )

#define	DB_ED_ERR_NOACCESS	( -13 )

#define	DB_ED_ERR_TAGFAIL	( -14 )

#define	DB_ED_ERR_PARAM	( -16 )

#define	DB_ED_ERR_NOTOWNER	( -17 )

#define	DB_ED_ERR_BADID	( -18 )

#define	DB_ED_ERR_SOURCE_CLEAR	( -19 )

#define	DB_ED_ERR_SOURCE_SET	( -20 )

#define	DP_MAX_SOURCES	( 32 )

#define	TRANS_ENTRY_MAX	( 50 )

#define	TRANS_TYPE_TABLE	( 0 )

#define	TRANS_TYPE_POLYNOMIAL	( 1 )

#define	TRANSLATION_TYPE_NONE	( -1 )

#define	POLYNOMIAL_TYPES_COUNT	( 4 )

#define	POLYNOMIAL_TYPE_1	( 1 )

#define	POLYNOMIAL_TYPE_2	( 2 )

#define	POLYNOMIAL_TYPE_3	( 3 )

#define	POLYNOMIAL_TYPE_4	( 4 )

#define	LD_ERR_NONE	( 0 )

#define	LD_ERR_INVALID_INDEX	( 0x1 )

#define	LD_ERR_LEAKRATE_TOO_HIGH	( 0x2 )

#define	LD_ERR_TEST_FAILED	( 0x4 )

#define	LD_ERR_CONNECTION_FAILED	( 0x8 )

#define	LD_ERR_SQL	( 0x10 )

#define	LD_ERR_NOT_ENOUGH_DATA	( 0x20 )

#define	LD_ERR_OVER_DELTA_TEMP	( 0x40 )

#define	LD_ERR_LVL_CONSTANT	( 0x80 )

#define	SCHEDULE_NUMBER	( 7 )

#define	SCHEDULE_WEEKLY	( 0 )

#define	SCHEDULE_DAILY	( 1 )

#define	SCHEDULE_MONTHLY	( 2 )

#define	SCHEDULE_ONETIME	( 3 )

#define	SCHEDULE_MONDAY	( 0 )

#define	SCHEDULE_TUESDAY	( 1 )

#define	SCHEDULE_WEDNESDAY	( 2 )

#define	SCHEDULE_THURSDAY	( 3 )

#define	SCHEDULE_FRIDAY	( 4 )

#define	SCHEDULE_SATURDAY	( 5 )

#define	SCHEDULE_SUNDAY	( 6 )

#define	SCHEDULE_ACTUALDAY	( 7 )

#define	PIPELINE_TANK_POINT	( 0 )

#define	PIPELINE_METER_POINT	( 1 )

#define	PIPELINE_SOURCE_ASSIGNMENET	( 2 )

typedef struct FM_DESC_STRUCT
    {
    wchar_t szDescription[ 31 ];
    } 	FM_DESC_STRUCT;

typedef struct FM_DESC_STRUCT *PFM_DESC_STRUCT;

typedef struct FM_TRANS_TO_FROM
    {
    int iFrom;
    int iFromType;
    int iTo;
    int iToType;
    } 	FM_TRANS_TO_FROM;

typedef struct FM_TRANS_TO_FROM *PFM_TRANS_TO_FROM;

typedef struct FM_TRANS_TABLE
    {
    int iNumberOfEntries;
    FM_TRANS_TO_FROM TransConv[ 50 ];
    } 	FM_TRANS_TABLE;

typedef struct FM_TRANS_TABLE *PFM_TRANS_TABLE;

typedef struct FM_TRANS_POLYNOMIAL
    {
    int nFormulaNumber;
    double dA;
    double dB;
    double dC;
    double dD;
    } 	FM_TRANS_POLYNOMIAL;

typedef struct FM_TRANS_POLYNOMIAL *PFM_TRANS_POLYNOMIAL;

typedef struct FM_TRANSLATION
    {
    union 
        {
        FM_TRANS_TABLE Table;
        FM_TRANS_POLYNOMIAL Poly;
        } 	Translation;
    unsigned char bTransType;
    long nTranslationID;
    wchar_t wcTranslationName[ 64 ];
    unsigned char bInUse;
    unsigned char bLocked;
    long tLastChanged;
    } 	FM_TRANSLATION;

typedef struct FM_TRANSLATION *PFM_TRANSLATION;

typedef struct FM_TRANSLATIONNAME
    {
    unsigned char bTransType;
    long nTranslationID;
    wchar_t wcTranslationName[ 64 ];
    } 	FM_TRANSLATIONNAME;

typedef struct FM_TRANSLATIONNAME *PFM_TRANSLATIONNAME;

typedef struct SOURCEDATA
    {
    unsigned long dwPointID;
    unsigned long dwInterfaceID;
    unsigned long dwDeviceID;
    unsigned short wSystemID;
    unsigned short wVariable;
    unsigned char bSystemType;
    } 	SOURCEDATA;

typedef struct SOURCEDATA *PSOURCEDATA;


#pragma pack(1)
typedef /* [public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0002
    {
    unsigned long dwInterface;
    unsigned long dwDeviceCode;
    unsigned char SourceIndex;
    } 	COMM;

typedef /* [public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0003
    {
    unsigned long dwPointID;
    unsigned short wPointIndex;
    unsigned char bPointType;
    unsigned short bVariable;
    } 	LOCAL;

typedef /* [public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0004
    {
    unsigned long dwPointID;
    unsigned short wPointIndex;
    unsigned char bPointType;
    unsigned short bVariable;
    unsigned char bSourceIndex;
    } 	REMOTE;

typedef /* [public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0005
    {
    unsigned long dwTagID;
    unsigned short wUpdateTime;
    unsigned char bOutput;
    } 	OPC;

typedef /* [public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0006
    {
    unsigned long dwPointID;
    unsigned short wPointIndex;
    unsigned short wAllocatedType;
    unsigned char bPointType;
    unsigned short bVariable;
    unsigned char bSourceIndex;
    } 	OPCUA;

typedef /* [public] */ struct __MIDL_dmlink_0007
    {
    unsigned char bPointType;
    unsigned long dwPointid;
    unsigned short wPointIndex;
    unsigned char bPointFormat;
    unsigned short bVariable;
    unsigned short wAllocatedType;
    int iSourceIndex;
    wchar_t szSelectedMode[ 20 ];
    wchar_t szDomain[ 260 ];
    wchar_t szServer[ 260 ];
    wchar_t szOpcUaServer[ 260 ];
    wchar_t szOpcUaServerSecurity[ 260 ];
    wchar_t szOpcUaServerEndpoint[ 260 ];
    int SecurityMode;
    int lSecurityPolicy;
    wchar_t szMessageEncoding[ 260 ];
    wchar_t szUserIdentity[ 260 ];
    wchar_t szUserName[ 260 ];
    wchar_t szUserPassword[ 260 ];
    wchar_t szOpcUaBrowsePath[ 260 ];
    wchar_t szOpcUaNodeId[ 260 ];
    int lPublishingInterval;
    int lWritePeriodUpdate;
    int lWriteHoldoff;
    int lOpcServerDataType;
    wchar_t szLastGoodValue[ 260 ];
    long lLastStatus;
    } 	OPCUA_COMMUNICATIONS;

typedef struct __MIDL_dmlink_0007 *POPCUA_COMMUNICATIONS;

typedef /* [public][public][public][public][public][public][public][public][public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0008
    {
    union 
        {
        LOCAL Local;
        REMOTE Remote;
        OPC Opc;
        COMM Comm;
        } 	Param;
    unsigned char bSourceType;
    unsigned char bSourceUnits;
    long nTranslationIndex;
    short nStartBit;
    short nNumberBits;
    short nStartSourceBit;
    unsigned char bIndex;
    } 	FMSOURCE_75;

typedef struct __MIDL_dmlink_0008 *PFMSOURCE_75;

typedef /* [public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0010
    {
    union 
        {
        LOCAL Local;
        REMOTE Remote;
        OPC Opc;
        COMM Comm;
        } 	Param;
    unsigned char bSourceType;
    unsigned char bSourceUnits;
    long nTranslationIndex;
    short nStartBit;
    short nNumberBits;
    short nStartSourceBit;
    unsigned char bIndex;
    double dDeadBand;
    } 	FMSOURCE_76;

typedef struct __MIDL_dmlink_0010 *PFMSOURCE_76;

typedef /* [public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0012
    {
    union 
        {
        LOCAL Local;
        REMOTE Remote;
        OPC Opc;
        OPCUA OpcUA;
        COMM Comm;
        } 	Param;
    unsigned char bSourceType;
    unsigned char bSourceUnits;
    long nTranslationIndex;
    short nStartBit;
    short nNumberBits;
    short nStartSourceBit;
    unsigned char bIndex;
    double dDeadBand;
    } 	FMSOURCE;

typedef struct __MIDL_dmlink_0012 *PFMSOURCE;


#pragma pack()
typedef /* [public][public][public][public][public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0014
    {
    unsigned long dwValue;
    unsigned char bInValid;
    } 	INPUTBITFIELDVALUES;

typedef /* [public] */ struct __MIDL_dmlink_0015
    {
    unsigned long DataFormat	: 8;
    unsigned long bCategory	: 8;
    unsigned long bModifierEnable	: 8;
    unsigned long Modifier	: 8;
    unsigned long EngrUnits	: 8;
    unsigned long RevCode	: 8;
    unsigned long AlarmDefined	: 1;
    unsigned long EventDefined	: 1;
    unsigned long OverRide	: 1;
    unsigned long Disabled	: 1;
    unsigned long bSpare1	: 1;
    unsigned long OutputPoint	: 1;
    unsigned long RecordUsed	: 1;
    unsigned long PointLocked	: 1;
    unsigned long FormChange	: 1;
    unsigned long bSpare2	: 1;
    unsigned long bSpare3	: 1;
    unsigned long bSpare4	: 1;
    unsigned long bQuality	: 1;
    unsigned long bCommandDisabled	: 1;
    unsigned long bSpare	: 1;
    unsigned long bitReserved	: 1;
    } 	DATAPOINTFLAGS;

typedef struct __MIDL_dmlink_0015 *PDATAPOINTFLAGS;

typedef struct DATAPOINT71
    {
    unsigned long dwDataPtFlags[ 2 ];
    FMDATA8 cvMaximum;
    FMDATA8 cvMinimum;
    unsigned long dwPointID;
    unsigned long dwProcessIndex;
    unsigned long dwAlarmIndex;
    unsigned long dwAlarmID;
    FMSOURCE_75 DataSource[ 32 ];
    unsigned char bStyle;
    unsigned char bMomentaryOutput;
    double cvMomentaryDefaultValue;
    unsigned short wMomentaryTimeDelay;
    unsigned short wMomentaryTimeLeft;
    FMSOURCE_75 DataOutput[ 32 ];
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    unsigned char bTriggered;
    unsigned char bBitfield;
    int nMaxRepeatInterval;
    int nMinRepeatInterval;
    long tLastChanged;
    long lInputTranslationIndex;
    long lOutputTranslationIndex;
    INPUTBITFIELDVALUES InputBitFieldData[ 32 ];
    } 	DATAPOINT71;

typedef struct DATAPOINT71 *PDATAPOINT71;

typedef struct DATAPOINT_75
    {
    unsigned long dwDataPtFlags[ 2 ];
    FMDATA8 cvMaximum;
    FMDATA8 cvMinimum;
    unsigned long dwPointID;
    unsigned long dwProcessIndex;
    unsigned long dwAlarmIndex;
    unsigned long dwAlarmID;
    FMSOURCE_75 DataSource[ 32 ];
    unsigned char bStyle;
    unsigned char bMomentaryOutput;
    FMDATA512 cvMomentaryDefaultValue;
    unsigned short wMomentaryTimeDelay;
    unsigned short wMomentaryTimeLeft;
    FMSOURCE_75 DataOutput[ 32 ];
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    unsigned char bTriggered;
    unsigned char bBitfield;
    int nMaxRepeatInterval;
    int nMinRepeatInterval;
    long tLastChanged;
    long lInputTranslationIndex;
    long lOutputTranslationIndex;
    INPUTBITFIELDVALUES InputBitFieldData[ 32 ];
    } 	DATAPOINT_75;

typedef struct DATAPOINT_75 *PDATAPOINT_75;

typedef struct DATAPOINT_76
    {
    unsigned long dwDataPtFlags[ 2 ];
    FMDATA8 cvMaximum;
    FMDATA8 cvMinimum;
    unsigned long dwPointID;
    unsigned long dwProcessIndex;
    unsigned long dwAlarmIndex;
    unsigned long dwAlarmID;
    FMSOURCE_76 DataSource[ 32 ];
    unsigned char bStyle;
    unsigned char bMomentaryOutput;
    FMDATA512 cvMomentaryDefaultValue;
    unsigned short wMomentaryTimeDelay;
    unsigned short wMomentaryTimeLeft;
    FMSOURCE_76 DataOutput[ 32 ];
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    unsigned char bTriggered;
    unsigned char bBitfield;
    int nMaxRepeatInterval;
    int nMinRepeatInterval;
    long tLastChanged;
    long lInputTranslationIndex;
    long lOutputTranslationIndex;
    INPUTBITFIELDVALUES InputBitFieldData[ 32 ];
    } 	DATAPOINT_76;

typedef struct DATAPOINT_76 *PDATAPOINT_76;

typedef struct DATAPOINT
    {
    unsigned long dwDataPtFlags[ 2 ];
    FMDATA8 cvMaximum;
    FMDATA8 cvMinimum;
    unsigned long dwPointID;
    unsigned long dwProcessIndex;
    unsigned long dwAlarmIndex;
    unsigned long dwAlarmID;
    FMSOURCE DataSource[ 32 ];
    unsigned char bStyle;
    unsigned char bMomentaryOutput;
    FMDATA512 cvMomentaryDefaultValue;
    unsigned short wMomentaryTimeDelay;
    unsigned short wMomentaryTimeLeft;
    FMSOURCE DataOutput[ 32 ];
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    unsigned char bTriggered;
    unsigned char bBitfield;
    int nMaxRepeatInterval;
    int nMinRepeatInterval;
    long tLastChanged;
    long lInputTranslationIndex;
    long lOutputTranslationIndex;
    INPUTBITFIELDVALUES InputBitFieldData[ 32 ];
    } 	DATAPOINT;

typedef struct DATAPOINT *PDATAPOINT;

typedef /* [public] */ struct __MIDL_dmlink_0016
    {
    unsigned long bModifierEnable	: 8;
    unsigned long EngrUnits	: 8;
    unsigned long bStyle	: 8;
    unsigned long OverRide	: 1;
    unsigned long Disabled	: 1;
    unsigned long AlarmDefined	: 1;
    unsigned long bAllocated	: 1;
    unsigned long bAlarmChange	: 1;
    unsigned long bUnused2	: 1;
    unsigned long bCommandDisabled	: 1;
    unsigned long bUnused3	: 1;
    } 	PVINPUTFLAGS;

typedef struct __MIDL_dmlink_0016 *PPVINPUTFLAGS;

typedef struct PVINPUT_75
    {
    unsigned long dwProcessIndex;
    unsigned long dwAlarmID;
    double dDataMax;
    double dDataMin;
    unsigned long dwTankInputFlags;
    unsigned long dwAlarmValue;
    FMSOURCE_75 DataSource;
    } 	PVINPUT_75;

typedef struct PVINPUT_75 *PPVINPUT_75;

typedef struct PVINPUT_76
    {
    unsigned long dwProcessIndex;
    unsigned long dwAlarmID;
    double dDataMax;
    double dDataMin;
    unsigned long dwTankInputFlags;
    unsigned long dwAlarmValue;
    FMSOURCE_76 DataSource;
    } 	PVINPUT_76;

typedef struct PVINPUT_76 *PPVINPUT_76;

typedef struct PVINPUT
    {
    unsigned long dwProcessIndex;
    unsigned long dwAlarmID;
    double dDataMax;
    double dDataMin;
    unsigned long dwTankInputFlags;
    unsigned long dwAlarmValue;
    FMSOURCE DataSource;
    } 	PVINPUT;

typedef struct PVINPUT *PPVINPUT;

typedef struct VOLCORRECT
    {
    double dStandardTemp;
    double dStandardDens;
    double dK_Factor[ 5 ];
    double reserved;
    double dHyDroMeterReading;
    double dMoleConstant;
    unsigned char bCorrectionTypeMajor;
    unsigned char bCorrectionTypeMinor;
    unsigned char byVCFRoundingMethod;
    unsigned char byNetWeightMethod;
    unsigned char byWaterSubtractMethod;
    unsigned char byVSWSubtractMethod;
    unsigned char byAPIVolumeCalcType;
    unsigned char byReserv;
    double dDensityPressure;
    double dAlternateTemperature;
    } 	VOLCORRECT;

typedef struct VOLCORRECT *PVOLCORRECT;

typedef /* [public] */ struct __MIDL_dmlink_0017
    {
    unsigned long DataFormat	: 8;
    unsigned long bCategory	: 8;
    unsigned long bModifierEnable	: 8;
    unsigned long Modifier	: 8;
    unsigned long EngrUnits	: 8;
    unsigned long RevCode	: 8;
    unsigned long AlarmDefined	: 1;
    unsigned long EventDefined	: 1;
    unsigned long OverRide	: 1;
    unsigned long Disabled	: 1;
    unsigned long CalcNewData	: 1;
    unsigned long bSpare5	: 1;
    unsigned long RecordUsed	: 1;
    unsigned long PointLocked	: 1;
    unsigned long FormChange	: 1;
    unsigned long bSpare2	: 1;
    unsigned long bSpare3	: 1;
    unsigned long bSpare4	: 1;
    unsigned long bQuality	: 1;
    unsigned long bCommandDisabled	: 1;
    unsigned long bLastEbtryWasRollOver	: 1;
    unsigned long bitReserved	: 1;
    } 	FLOWMETERFLAGS;

typedef struct __MIDL_dmlink_0017 *PFLOWMETERFLAGS;

typedef struct FLOWMETERCALCDATA
    {
    double dLastMeterValue;
    int iNumberofRollOvers;
    } 	FLOWMETERCALCDATA;

typedef struct FLOWMETERCALCDATA *PFLOWMETERCALCDATA;

typedef struct FLOWMETERPOINT_75
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT_75 ProcessVariable[ 7 ];
    VOLCORRECT VolCorrection;
    unsigned long pVolCalculations;
    unsigned char bStyle;
    unsigned char bMeterType;
    double dPulseMeterVolAmount;
    unsigned char bPulseMeterEngrUnits;
    unsigned char bMeterTempEngrUnits;
    unsigned char bMeterPressEngrUnits;
    unsigned char bMeterDensityEngrUnits;
    unsigned char bMeterNetVolEngrUnits;
    unsigned char bMeterStdDensityUnits;
    unsigned char bMeterMassEngrUnits;
    double dRollOverAmount;
    double dCalcVolumeAmount;
    double dNetVolume;
    double dMass;
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    long tLastChanged;
    FLOWMETERCALCDATA stLastCalcData;
    } 	FLOWMETERPOINT_75;

typedef struct FLOWMETERPOINT_75 *PFLOWMETERPOINT_75;

typedef struct FLOWMETERPOINT_76
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT ProcessVariable[ 7 ];
    VOLCORRECT VolCorrection;
    unsigned long pVolCalculations;
    unsigned char bStyle;
    unsigned char bMeterType;
    double dPulseMeterVolAmount;
    unsigned char bPulseMeterEngrUnits;
    unsigned char bMeterTempEngrUnits;
    unsigned char bMeterPressEngrUnits;
    unsigned char bMeterDensityEngrUnits;
    unsigned char bMeterNetVolEngrUnits;
    unsigned char bMeterStdDensityUnits;
    unsigned char bMeterMassEngrUnits;
    double dRollOverAmount;
    double dCalcVolumeAmount;
    double dNetVolume;
    double dMass;
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    long tLastChanged;
    FLOWMETERCALCDATA stLastCalcData;
    } 	FLOWMETERPOINT_76;

typedef struct FLOWMETERPOINT_76 *PFLOWMETERPOINT_76;

typedef struct FLOWMETERPOINT_77
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT_76 ProcessVariable[ 9 ];
    VOLCORRECT VolCorrection;
    unsigned long pVolCalculations;
    unsigned char bStyle;
    unsigned char bMeterType;
    double dPulseMeterVolAmount;
    unsigned char bPulseMeterEngrUnits;
    unsigned char bMeterTempEngrUnits;
    unsigned char bMeterPressEngrUnits;
    unsigned char bMeterDensityEngrUnits;
    unsigned char bMeterNetVolEngrUnits;
    unsigned char bMeterStdDensityUnits;
    unsigned char bMeterMassEngrUnits;
    double dRollOverAmount;
    double dCalcVolumeAmount;
    double dNetVolume;
    double dMass;
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    long tLastChanged;
    FLOWMETERCALCDATA stLastCalcData;
    double dGrossTotalizerAmount;
    double dNetTotalizerAmount;
    } 	FLOWMETERPOINT_77;

typedef struct FLOWMETERPOINT_77 *PFLOWMETERPOINT_77;

typedef struct FLOWMETERPOINT
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT ProcessVariable[ 9 ];
    VOLCORRECT VolCorrection;
    unsigned long pVolCalculations;
    unsigned char bStyle;
    unsigned char bMeterType;
    double dPulseMeterVolAmount;
    unsigned char bPulseMeterEngrUnits;
    unsigned char bMeterTempEngrUnits;
    unsigned char bMeterPressEngrUnits;
    unsigned char bMeterDensityEngrUnits;
    unsigned char bMeterNetVolEngrUnits;
    unsigned char bMeterStdDensityUnits;
    unsigned char bMeterMassEngrUnits;
    double dRollOverAmount;
    double dCalcVolumeAmount;
    double dNetVolume;
    double dMass;
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    long tLastChanged;
    FLOWMETERCALCDATA stLastCalcData;
    double dGrossTotalizerAmount;
    double dNetTotalizerAmount;
    } 	FLOWMETERPOINT;

typedef struct FLOWMETERPOINT *PFLOWMETERPOINT;

typedef /* [public] */ struct __MIDL_dmlink_0018
    {
    unsigned long DataFormat	: 8;
    unsigned long bCategory	: 8;
    unsigned long RevCode	: 8;
    unsigned long bModifierEnable	: 8;
    unsigned long Modifier	: 8;
    unsigned long bSpare10	: 8;
    unsigned long OverRide	: 1;
    unsigned long Disabled	: 1;
    unsigned long CalcNewData	: 1;
    unsigned long RecordUsed	: 1;
    unsigned long PointLocked	: 1;
    unsigned long FormChange	: 1;
    unsigned long bQuality	: 1;
    unsigned long bCommandDisabled	: 1;
    unsigned long DigAlrmAlloc	: 1;
    unsigned long bPipeAlwaysFull	: 1;
    unsigned long bStartVolChanged	: 1;
    unsigned long bSpare4	: 1;
    unsigned long bSpare5	: 1;
    unsigned long bSpare6	: 1;
    unsigned long bSpare7	: 1;
    unsigned long bitReserved	: 1;
    } 	PIPELINEPNTFLAGS;

typedef struct __MIDL_dmlink_0018 *PPIPELINEPNTFLAGS;

typedef struct PIPELINEPNTCALCDATA
    {
    double dCalcDiscrepancy;
    double dCalcDiscrepancyinPercent;
    double dPipelineVolume;
    unsigned char bDiscrepancyUnits;
    unsigned short wMonitoringStatus;
    long tTimeMonitoringStarted;
    } 	PIPELINEPNTCALCDATA;

typedef struct PIPELINEPNTCALCDATA *PPIPELINEPNTCALCDATA;

typedef struct PIPELINEPNTALARMDATA
    {
    int iShortTermVariance;
    int iShortTermMinutes;
    int iMediumTermVariance;
    double dMediumTermHours;
    int iLongTermVariance;
    double dLongTermHours;
    int iHardwareTermVariance;
    unsigned short bShortTemEnabled;
    unsigned short bMediumTemEnabled;
    unsigned short bLongTemEnabled;
    unsigned short bHardwareEnabled;
    } 	PIPELINEPNTALARMDATA;

typedef struct PIPELINEPNTALARMDATA *PPIPELINEPNTALARMDATA;

typedef struct PIPELINEPNTLINEDATA
    {
    double dLineVolAmount;
    double dLineStartVolume;
    double dLineLastStartVolume;
    double dLineCalcVolume;
    unsigned long ulLineCalcVolumeStatus1;
    unsigned long ulLineCalcVolumeStatus2;
    long tLastTimeLineCalcVolume;
    unsigned short tLastTimemillisecsLineCalcVolume;
    } 	PIPELINEPNTLINEDATA;

typedef struct PIPELINEPNTLINEDATA *PPIPELINEPNTLINEDATA;

typedef struct PIPELINEPNTLINEDATA_75
    {
    double dLineVolAmount;
    double dLineStartVolume;
    double dLineLastStartVolume;
    double dLineCalcVolume;
    unsigned long ulLineCalcVolumeStatus;
    long tLastTimeLineCalcVolume;
    unsigned short tLastTimemillisecsLineCalcVolume;
    } 	PIPELINEPNTLINEDATA_75;

typedef struct PIPELINEPNTLINEDATA_75 *PPIPELINEPNTLINEDATA_75;

typedef struct PIPELINEPNTSTARTMONITORING
    {
    double dStartupThreshold;
    double dLastDeliveryValue;
    int iStartupDelayinMinutes;
    int iStartupDelayNumSeconds;
    int iStartupDelayMinutesLeft;
    int iShutdownDelayMinutesLeft;
    } 	PIPELINEPNTSTARTMONITORING;

typedef struct PIPELINEPNTSTARTMONITORING *PPIPELINEPNTSTARTMONITORING;

typedef struct PIPELINEPNTMETERTANKDATA
    {
    double dReceiptValue;
    double dReceiptValueConverted;
    unsigned long ulReceiptValueStatus1;
    unsigned long ulReceiptValueStatus2;
    unsigned char bReceiptUnits;
    double dDeliveryValue;
    double dDeliveryValueConverted;
    unsigned long ulDeliveryValueStatus1;
    unsigned long ulDeliveryValueStatus2;
    unsigned char bDeliveryUnits;
    double dReceiptStartValue;
    double dDeliveryStartValue;
    double dLastReceiptValue;
    double dLastDeliveryValue;
    long tLastTimeReceiptValue;
    unsigned short tLastTimemillisecsReceiptValue;
    long tLastTimeDeliveryValue;
    unsigned short tLastTimemillisecsDeliveryValue;
    } 	PIPELINEPNTMETERTANKDATA;

typedef struct PIPELINEPNTMETERTANKDATA *PPIPELINEPNTMETERTANKDATA;

typedef struct PIPELINEPNTMETERTANKDATA_75
    {
    double dReceiptValue;
    double dReceiptValueConverted;
    unsigned long ulReceiptValueStatus;
    unsigned char bReceiptUnits;
    double dDeliveryValue;
    double dDeliveryValueConverted;
    unsigned long ulDeliveryValueStatus;
    unsigned char bDeliveryUnits;
    double dReceiptStartValue;
    double dDeliveryStartValue;
    double dLastReceiptValue;
    double dLastDeliveryValue;
    long tLastTimeReceiptValue;
    unsigned short tLastTimemillisecsReceiptValue;
    long tLastTimeDeliveryValue;
    unsigned short tLastTimemillisecsDeliveryValue;
    } 	PIPELINEPNTMETERTANKDATA_75;

typedef struct PIPELINEPNTMETERTANKDATA_75 *PPIPELINEPNTMETERTANKDATA_75;

typedef struct PIPELINEPNT_75
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT_75 ProcessVariable[ 2 ];
    unsigned char bStyle;
    unsigned char bLeakDetectionType;
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    wchar_t szDeliveryPoint[ 101 ];
    unsigned char bDeliveryPointType;
    wchar_t szReceiptPoint[ 101 ];
    unsigned char bReceiptPointType;
    unsigned char bSelectedSourceType;
    unsigned char bSelectedMeterType;
    unsigned long dwStandardAlarms;
    unsigned long dwAlarmIDStd;
    PIPELINEPNTCALCDATA stCalcData;
    PIPELINEPNTLINEDATA_75 stLineVolData;
    PIPELINEPNTSTARTMONITORING stStartupParams;
    PIPELINEPNTALARMDATA stAlarmData;
    PIPELINEPNTMETERTANKDATA_75 stMeterTankData;
    long tLastChanged;
    } 	PIPELINEPNT_75;

typedef struct PIPELINEPNT_75 *PPIPELINEPNT_75;

typedef struct PIPELINEPNT_76
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT_76 ProcessVariable[ 2 ];
    unsigned char bStyle;
    unsigned char bLeakDetectionType;
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    wchar_t szDeliveryPoint[ 101 ];
    unsigned char bDeliveryPointType;
    wchar_t szReceiptPoint[ 101 ];
    unsigned char bReceiptPointType;
    unsigned char bSelectedSourceType;
    unsigned char bSelectedMeterType;
    unsigned long dwStandardAlarms;
    unsigned long dwAlarmIDStd;
    PIPELINEPNTCALCDATA stCalcData;
    PIPELINEPNTLINEDATA stLineVolData;
    PIPELINEPNTSTARTMONITORING stStartupParams;
    PIPELINEPNTALARMDATA stAlarmData;
    PIPELINEPNTMETERTANKDATA stMeterTankData;
    long tLastChanged;
    } 	PIPELINEPNT_76;

typedef struct PIPELINEPNT_76 *PPIPELINEPNT_76;

typedef struct PIPELINEPNT
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT ProcessVariable[ 2 ];
    unsigned char bStyle;
    unsigned char bLeakDetectionType;
    unsigned char bReservBlock[ 4 ];
    wchar_t szDescription[ 31 ];
    wchar_t szDeliveryPoint[ 101 ];
    unsigned char bDeliveryPointType;
    wchar_t szReceiptPoint[ 101 ];
    unsigned char bReceiptPointType;
    unsigned char bSelectedSourceType;
    unsigned char bSelectedMeterType;
    unsigned long dwStandardAlarms;
    unsigned long dwAlarmIDStd;
    PIPELINEPNTCALCDATA stCalcData;
    PIPELINEPNTLINEDATA stLineVolData;
    PIPELINEPNTSTARTMONITORING stStartupParams;
    PIPELINEPNTALARMDATA stAlarmData;
    PIPELINEPNTMETERTANKDATA stMeterTankData;
    long tLastChanged;
    } 	PIPELINEPNT;

typedef struct PIPELINEPNT *PPIPELINEPNT;

typedef /* [public] */ struct __MIDL_dmlink_0019
    {
    unsigned long DataFormat	: 8;
    unsigned long bCategory	: 8;
    unsigned long Spare2	: 8;
    unsigned long Spare3	: 8;
    unsigned long Spare4	: 8;
    unsigned long RevCode	: 8;
    unsigned long Spare5	: 1;
    unsigned long Spare6	: 1;
    unsigned long OverRide	: 1;
    unsigned long Disabled	: 1;
    unsigned long CalcNewData	: 1;
    unsigned long bSpare7	: 1;
    unsigned long RecordUsed	: 1;
    unsigned long PointLocked	: 1;
    unsigned long FormChange	: 1;
    unsigned long bSpare8	: 1;
    unsigned long bSpare9	: 1;
    unsigned long bSpare10	: 1;
    unsigned long bQuality	: 1;
    unsigned long bCommandDisabled	: 1;
    unsigned long Spare11	: 1;
    unsigned long bitReserved	: 1;
    } 	LOGICPNTFLAGS;

typedef struct __MIDL_dmlink_0019 *PLOGICPNTFLAGS;

typedef struct LOGICPNT
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT ProcessVariable[ 27 ];
    unsigned char bStyle;
    wchar_t szDescription[ 31 ];
    wchar_t szFormula[ 256 ];
    long tLastChanged;
    } 	LOGICPNT;

typedef struct LOGICPNT *PLOGICPNT;

typedef struct LOGICPNT_76
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT_76 ProcessVariable[ 27 ];
    unsigned char bStyle;
    wchar_t szDescription[ 31 ];
    wchar_t szFormula[ 256 ];
    long tLastChanged;
    } 	LOGICPNT_76;

typedef struct LOGICPNT_76 *PLOGICPNT_76;

typedef /* [public] */ struct __MIDL_dmlink_0020
    {
    unsigned long DataFormat	: 8;
    unsigned long bCategory	: 8;
    unsigned long bAlarmCategory	: 8;
    unsigned long Spare3	: 8;
    unsigned long Spare4	: 8;
    unsigned long RevCode	: 8;
    unsigned long DigAlrmAlloc	: 1;
    unsigned long Spare6	: 1;
    unsigned long OverRide	: 1;
    unsigned long Disabled	: 1;
    unsigned long CalcNewData	: 1;
    unsigned long bSpare7	: 1;
    unsigned long RecordUsed	: 1;
    unsigned long PointLocked	: 1;
    unsigned long FormChange	: 1;
    unsigned long bSpare8	: 1;
    unsigned long bSpare9	: 1;
    unsigned long bSpare10	: 1;
    unsigned long bQuality	: 1;
    unsigned long bCommandDisabled	: 1;
    unsigned long Spare11	: 1;
    unsigned long bitReserved	: 1;
    } 	TIMERPNTFLAGS;

typedef struct __MIDL_dmlink_0020 *PTIMERPNTFLAGS;

typedef struct TIMERPNTALARMDATA
    {
    long lAdvisortSetpoint;
    unsigned short bAdvisortAlarmEnabled;
    unsigned short bCompleteAlarmEnabled;
    } 	TIMERPNTALARMDATA;

typedef struct TIMERPNTALARMDATA *PTIMERPNTALARMDATA;

typedef struct TIMERPNT_76
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT_76 ProcessVariable[ 3 ];
    wchar_t szDescription[ 31 ];
    long lTimerSetAmount;
    long lTimerAmountLeft;
    long lTimerAutoResetAmount;
    long lTimerAutoResetAmountLeft;
    long lTimerAdvisoryETA;
    long lTimerCompleteETA;
    int iETACalcsValid;
    long tLastChanged;
    unsigned long dwStandardAlarms;
    unsigned long dwAlarmIDStd;
    TIMERPNTALARMDATA stAlarmData;
    } 	TIMERPNT_76;

typedef struct TIMERPNT_76 *PTIMERPNT_76;

typedef struct TIMERPNT
    {
    unsigned long dwDataPtFlags[ 2 ];
    unsigned long dwPointID;
    PVINPUT ProcessVariable[ 3 ];
    wchar_t szDescription[ 31 ];
    long lTimerSetAmount;
    long lTimerAmountLeft;
    long lTimerAutoResetAmount;
    long lTimerAutoResetAmountLeft;
    long lTimerAdvisoryETA;
    long lTimerCompleteETA;
    int iETACalcsValid;
    long tLastChanged;
    unsigned long dwStandardAlarms;
    unsigned long dwAlarmIDStd;
    TIMERPNTALARMDATA stAlarmData;
    } 	TIMERPNT;

typedef struct TIMERPNT *PTIMERPNT;

typedef /* [public][public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0021
    {
    unsigned char bTankCalibrated;
    unsigned char bProcessVariableCalibrated[ 64 ];
    unsigned long dwID;
    } 	SEALDATA;

typedef struct SEALDATA_75
    {
    unsigned char bTankCalibrated;
    unsigned char bProcessVariableCalibrated[ 57 ];
    unsigned long dwID;
    } 	SEALDATA_75;

typedef struct LPGDATA
    {
    double dTankRadius;
    double dTotalTnkVol;
    double dTotalTnkHeight;
    double dTankThickness;
    double dMassOfFloat;
    double dDiaOfFloat;
    double dDensityOfFloat;
    unsigned char byHeadType;
    unsigned char bySpare1;
    unsigned char bySpare2;
    unsigned char bySpare3;
    } 	LPGDATA;

typedef struct LPGDATA *PLPGDATA;

typedef /* [public] */ struct __MIDL_dmlink_0022
    {
    unsigned long TankGeometry	: 4;
    unsigned long bFlowState	: 3;
    unsigned long bMaxFillState	: 2;
    unsigned long bMinEmptyState	: 2;
    unsigned long bStopLevelDone	: 1;
    unsigned long bCaptureStopLevel	: 1;
    unsigned long bTankMaterial	: 3;
    unsigned long bInsulated	: 1;
    unsigned long bLeakDataArlmState	: 1;
    unsigned long Spare	: 14;
    } 	TANKVESSELFLAGS;

typedef struct __MIDL_dmlink_0022 *PTANKVESSELFLAGS;

typedef struct TANKVESSEL
    {
    double dStrapTemp;
    double dStrapDensity;
    double dRoofMass;
    double dPinHeight;
    double dCriticalZone;
    double dMaxFillLevel;
    double dMinEmptyLevel;
    double dMovementStopPoint;
    double dMovementAlarmTripPoint;
    double dMovementDeadband;
    double dTankMaterialCoef;
    double dTankExpansionAreaCoef;
    unsigned long pTransferCalc;
    unsigned long dwTankVesselFlags;
    unsigned long dwSpare;
    unsigned int bStrapTableAlloc;
    unsigned long tMovementAlarmTime;
    unsigned long tReverseFlowAlarmTime;
    unsigned char byTankRoofType;
    LPGDATA LPGData;
    double dHydroPressureVolume;
    double dUnroundedNetVolume;
    double dLevelDisableEnableAlarmCommand;
    unsigned short wLevelDisabledTimeRemaining;
    unsigned short wSpare;
    double dHydroDeformation;
    } 	TANKVESSEL;

typedef struct TANKVESSEL *PTANKVESSEL;

typedef struct TANKXFR
    {
    double dAdviseSetpoint;
    double dShutdownSP;
    double dShutdownLevelSP;
    double dShutdownVolSP;
    double dShutdownDiffVolSP;
    double dShutdownMassSP;
    double dShutdownDiffMassSP;
    double dStartLevel;
    double dStartVolume;
    double dStartNetVolume;
    double dStartMass;
    double dStartTemp;
    double dStartDensity;
    double dStartStdDensity;
    double dStartWaterLevel;
    double dStartWaterVolume;
    double dStartSolidLevel;
    double dStartSolidVolume;
    double dStartBSW;
    long StartTime;
    long CompletionTime;
    } 	TANKXFR;

typedef struct TANKXFR *PTANKXFR;

typedef struct FILTERSAMPLE
    {
    double dStdSample;
    double dStdSampleAvgDevSq;
    } 	FILTERSAMPLE;

typedef struct FILTERSAMPLE *PFILTERSAMPLE;

typedef struct FILTERDYNAMICDATA
    {
    unsigned long tLastSampleTime;
    double dTimeofNextSample;
    double dStdSampleTotal;
    double dStdSampleAvgDevSqTotal;
    unsigned char bNextSample;
    unsigned char bSamples;
    FILTERSAMPLE FilterSample[ 1 ];
    } 	FILTERDYNAMICDATA;

typedef struct FILTERDYNAMICDATA *PFILTERDYNAMICDATA;

#define	NUM_RATE_CALCS	( 5 )

#define	FLOW_CALCS	( 0 )

#define	LEVELRATE_CALCS	( 1 )

#define	MASSRATE_CALCS	( 2 )

#define	NETRATE_CALCS	( 3 )

#define	PRESSURERATE_CALCS	( 4 )

typedef struct FILTERCALCFLAGS
    {
    unsigned int bFilterEnabled;
    unsigned int bInitialized;
    unsigned long dwSpare[ 2 ];
    } 	FILTERCALCFLAGS;

typedef struct FILTERCALCFLAGS *PFILTERCALCFLAGS;

typedef struct FILTERCALC
    {
    double dLastSample[ 5 ];
    double dDeadBand[ 5 ];
    unsigned long pDynamicData[ 5 ];
    FILTERCALCFLAGS FilterCalcFlags[ 5 ];
    unsigned char byFilterSampleRate;
    unsigned char byFilterSampleSize;
    unsigned char bySampleCount;
    unsigned int bSetup;
    short wDensityCalcDelay;
    short wDensityCalcForceUpdate;
    float fCalculatedLeakRate;
    unsigned short wLeakRateInitialized;
    unsigned short wSpare[ 3 ];
    } 	FILTERCALC;

typedef struct FILTERCALC *PFILTERCALC;

typedef struct FILTERCALC_75
    {
    double dLastSample[ 4 ];
    double dDeadBand[ 4 ];
    unsigned long pDynamicData[ 4 ];
    FILTERCALCFLAGS FilterCalcFlags[ 4 ];
    unsigned char byFilterSampleRate;
    unsigned char byFilterSampleSize;
    unsigned char bySampleCount;
    unsigned int bSetup;
    short wDensityCalcDelay;
    short wDensityCalcForceUpdate;
    float fCalculatedLeakRate;
    unsigned short wLeakRateInitialized;
    unsigned short wSpare[ 3 ];
    } 	FILTERCALC_75;

typedef struct FILTERCALC_75 *PFILTERCALC_75;

typedef struct TANKDATAFLAGS
    {
    unsigned long RevCode	: 8;
    unsigned long TankLocked	: 1;
    unsigned long bQuality	: 1;
    unsigned long bEnableLeakDataAlrm	: 1;
    unsigned long EnableNoFlowAlrm	: 1;
    unsigned long EnableMovementAlrm	: 1;
    unsigned long EnableRevFlow	: 1;
    unsigned long RecordUsed	: 1;
    unsigned long bTankGaugingMode	: 3;
    unsigned long bUnused7	: 5;
    unsigned long bFlowDetect	: 1;
    unsigned long UseMeasuredDens	: 1;
    unsigned long CalcNewData	: 1;
    unsigned long FlowActive	: 1;
    unsigned long UseGlobalProduct	: 1;
    unsigned long ScaleFactorValid	: 1;
    unsigned long bStdAlarmAlloc	: 1;
    unsigned long TankDigAlrmAlloc	: 1;
    unsigned long TankDevAlrmAlloc	: 1;
    unsigned long bModeAlrIndAlloc	: 1;
    unsigned long bModeAlarmAlloc	: 1;
    unsigned long bAdvUnitsTime	: 1;
    unsigned long bUnused3	: 2;
    unsigned long bTestModeAlarm	: 1;
    unsigned long bUnused4	: 1;
    unsigned long bEnableEtaAlarms	: 1;
    unsigned long bSpare	: 1;
    unsigned long bTransferSetpoint	: 1;
    unsigned long bTransferComplete	: 1;
    unsigned long bUseCTSh	: 1;
    unsigned long bOutageTank	: 1;
    unsigned long bStrapTableMode	: 1;
    unsigned long bUnused5	: 1;
    unsigned long bUnused6	: 8;
    unsigned long bUseNetVolMvmntAlrm	: 1;
    unsigned long dwReservFlag	: 6;
    unsigned long bCalculatorFlag	: 1;
    unsigned long bCommitted	: 1;
    } 	TANKDATAFLAGS;

typedef struct TANKDATAFLAGS *PTANKDATAFLAGS;

typedef struct TANKETADATA
    {
    long EtaHiHiLevel;
    long EtaHighLevel;
    long EtaLowLevel;
    long EtaLoLoLevel;
    double dEtaHiHiLevelM;
    double dEtaHighLevelM;
    double dEtaLowLevelM;
    double dEtaLoLoLevelM;
    } 	TANKETADATA;

typedef struct TANKETADATA *PTANKETADATA;

typedef struct DEVICEFLAGS
    {
    unsigned long DeviceType	: 10;
    unsigned long DeviceCmdModifier	: 8;
    unsigned long bReserv	: 14;
    } 	DEVICEFLAGS;

typedef struct DEVICEFLAGS *PDEVICEFLAGS;

typedef struct tagPOLYNOMIALSPHERE
    {
    double dCoefficients[ 4 ][ 8 ];
    double dCorrectionLevel;
    double dPConstant;
    } 	POLYNOMIALSPHERE;

typedef struct tagPOLYNOMIALSPHERE *PPOLYNOMIALSPHERE;

typedef struct tagLEAKDETECTION
    {
    unsigned char byCathodicProtection;
    unsigned char byOverfillProtection;
    unsigned char bySpillProtection;
    wchar_t tszLiningMaterial[ 40 ];
    DATE dtDateInstalled;
    unsigned short sAnalysisMethod;
    unsigned short sAnalysisType;
    unsigned char byAutoPrinting;
    DATE dtPrintTime;
    unsigned short sNumDaysBeforeEOM;
    } 	LEAKDETECTION;

typedef struct tagLEAKDETECTION *PLEAKDETECTION;

typedef struct tagLATITUDE
    {
    unsigned char Deg;
    unsigned char Min;
    unsigned char Sec;
    } 	LATITUDE;

typedef struct tagLATITUDE *PLATITUDE;

typedef struct tagPRESSURETRANSMITTER
    {
    unsigned char bUsePT;
    double dOffSet;
    double dHeight;
    } 	PRESSURETRANSMITTER;

typedef struct tagPRESSURETRANSMITTER *PPRESSURETRANSMITTER;

typedef struct tagHYDROSTATICCONFIG
    {
    PRESSURETRANSMITTER pTopTransmitter;
    PRESSURETRANSMITTER pMiddleTransmitter;
    PRESSURETRANSMITTER pBottomTransmitter;
    double dMinimumLevel;
    double dDensityDeadband;
    unsigned char bUseLocalGravity;
    double dLocalGravity;
    LATITUDE pLatitude;
    double dElevation;
    double dInstallTemp;
    double dLevelOffSet;
    } 	HYDROSTATICCONFIG;

typedef struct tagHYDROSTATICCONFIG *PHYDROSTATICCONFIG;

typedef struct TANKSCHEDULE
    {
    unsigned short wSelectedCommand;
    unsigned char bCommandIssued;
    unsigned char bSchedulingType;
    unsigned char bSelectedDay;
    short stScheduledTimeHour;
    short stScheduledTimeMin;
    unsigned char bQuietTime;
    } 	TANKSCHEDULE;

typedef struct TANKSCHEDULE *PTANKSCHEDULE;

typedef struct OPERATIONALALARM
    {
    unsigned short bOperatorAlarmEnabled;
    unsigned char bOperatorAlarmDirection;
    double dOperatorAlarmSetPoint;
    } 	OPERATIONALALARM;

typedef struct OPERATIONALALARM *POPERATIONALALARM;

typedef struct CST_STRUCTURE
    {
    wchar_t szManufacturerName[ 255 ];
    long dtManufacturerDate;
    long dtWetDate;
    double dManufacturerVolume;
    wchar_t szCSTSerialNumber[ 255 ];
    wchar_t szCSTLocation[ 255 ];
    wchar_t szCSTLongitude[ 255 ];
    wchar_t szCSTLatitude[ 255 ];
    } 	CST_STRUCTURE;

typedef struct CST_STRUCTURE *PCST_STRUCTURE;

typedef struct TANKDATA
    {
    unsigned long dwPointID;
    PVINPUT ProcessVariable[ 64 ];
    TANKVESSEL TankVessel;
    VOLCORRECT VolCorrection;
    TANKXFR TransferData;
    POLYNOMIALSPHERE PolynomialSphere;
    unsigned long pTankEtaData;
    unsigned long pTankCalculations;
    FILTERCALC FilterCalculation;
    unsigned long dwTankDataFlags[ 2 ];
    unsigned long dwProductIndex;
    unsigned long dwStandardAlarms;
    unsigned long dwAlarmIDStd;
    unsigned long dwAlarmIDSensor;
    unsigned long dwModeAlarmID;
    unsigned long dwModeAlarmStorage;
    unsigned long dwMovementID;
    unsigned long dwDeviceFlags;
    unsigned short wNoFlowHoldOffTime;
    unsigned short wMovementHoldOffTime;
    unsigned short wReverseFlowHoldOffTime;
    unsigned char byLevelRoundingMethod;
    unsigned char byTemperatureRoundingMethod;
    unsigned char bTankCategory;
    unsigned char bAlarmCategory;
    wchar_t szDetail[ 21 ];
    wchar_t szDescription[ 31 ];
    wchar_t szComment[ 256 ];
    SEALDATA SealData;
    LEAKDETECTION LeakData;
    HYDROSTATICCONFIG HydrostaticConfig;
    long tLastChanged;
    TANKSCHEDULE Schedule_Data[ 7 ];
    OPERATIONALALARM OperationalAlarmData;
    double dVcfCalcGauge;
    double dAverageBottomPressure;
    CST_STRUCTURE stCSTData;
    } 	TANKDATA;

typedef struct TANKDATA *PTANKDATA;

typedef struct TANKDATA_79
    {
    unsigned long dwPointID;
    PVINPUT_76 ProcessVariable[ 64 ];
    TANKVESSEL TankVessel;
    VOLCORRECT VolCorrection;
    TANKXFR TransferData;
    POLYNOMIALSPHERE PolynomialSphere;
    unsigned long pTankEtaData;
    unsigned long pTankCalculations;
    FILTERCALC FilterCalculation;
    unsigned long dwTankDataFlags[ 2 ];
    unsigned long dwProductIndex;
    unsigned long dwStandardAlarms;
    unsigned long dwAlarmIDStd;
    unsigned long dwAlarmIDSensor;
    unsigned long dwModeAlarmID;
    unsigned long dwModeAlarmStorage;
    unsigned long dwMovementID;
    unsigned long dwDeviceFlags;
    unsigned short wNoFlowHoldOffTime;
    unsigned short wMovementHoldOffTime;
    unsigned short wReverseFlowHoldOffTime;
    unsigned char byLevelRoundingMethod;
    unsigned char byTemperatureRoundingMethod;
    unsigned char bTankCategory;
    unsigned char bAlarmCategory;
    wchar_t szDetail[ 21 ];
    wchar_t szDescription[ 31 ];
    wchar_t szComment[ 256 ];
    SEALDATA SealData;
    LEAKDETECTION LeakData;
    HYDROSTATICCONFIG HydrostaticConfig;
    long tLastChanged;
    TANKSCHEDULE Schedule_Data[ 7 ];
    OPERATIONALALARM OperationalAlarmData;
    double dVcfCalcGauge;
    double dAverageBottomPressure;
    CST_STRUCTURE stCSTData;
    } 	TANKDATA_79;

typedef struct TANKDATA_79 *PTANKDATA_79;

typedef struct VOLCORRECT_78
    {
    double dStandardTemp;
    double dStandardDens;
    double dK_Factor[ 5 ];
    double reserved;
    double dHyDroMeterReading;
    double dMoleConstant;
    unsigned char bCorrectionTypeMajor;
    unsigned char bCorrectionTypeMinor;
    unsigned char byVCFRoundingMethod;
    unsigned char byNetWeightMethod;
    unsigned char byWaterSubtractMethod;
    unsigned char byVSWSubtractMethod;
    unsigned short byReserv;
    double dDensityPressure;
    double dAlternateTemperature;
    } 	VOLCORRECT_78;

typedef struct VOLCORRECT_78 *PVOLCORRECT_78;

typedef /* [public][public][public] */ struct __MIDL_dmlink_0023
    {
    unsigned char bTankCalibrated;
    unsigned char bProcessVariableCalibrated[ 58 ];
    unsigned long dwID;
    } 	SEALDATA_78;

typedef struct TANKDATA_78
    {
    unsigned long dwPointID;
    PVINPUT ProcessVariable[ 58 ];
    TANKVESSEL TankVessel;
    VOLCORRECT_78 VolCorrection;
    TANKXFR TransferData;
    POLYNOMIALSPHERE PolynomialSphere;
    unsigned long pTankEtaData;
    unsigned long pTankCalculations;
    FILTERCALC FilterCalculation;
    unsigned long dwTankDataFlags[ 2 ];
    unsigned long dwProductIndex;
    unsigned long dwStandardAlarms;
    unsigned long dwAlarmIDStd;
    unsigned long dwAlarmIDSensor;
    unsigned long dwModeAlarmID;
    unsigned long dwModeAlarmStorage;
    unsigned long dwMovementID;
    unsigned long dwDeviceFlags;
    unsigned short wNoFlowHoldOffTime;
    unsigned short wMovementHoldOffTime;
    unsigned short wReverseFlowHoldOffTime;
    unsigned char byLevelRoundingMethod;
    unsigned char byTemperatureRoundingMethod;
    unsigned char bTankCategory;
    unsigned char bAlarmCategory;
    wchar_t szDetail[ 21 ];
    wchar_t szDescription[ 31 ];
    wchar_t szComment[ 256 ];
    SEALDATA_78 SealData;
    LEAKDETECTION LeakData;
    HYDROSTATICCONFIG HydrostaticConfig;
    long tLastChanged;
    TANKSCHEDULE Schedule_Data[ 7 ];
    OPERATIONALALARM OperationalAlarmData;
    double dVcfCalcGauge;
    double dAverageBottomPressure;
    CST_STRUCTURE stCSTData;
    } 	TANKDATA_78;

typedef struct TANKDATA_78 *PTANKDATA_78;

typedef /* [public] */ struct __MIDL_dmlink_0024
    {
    FMDATA8 cvMaximum;
    FMDATA8 cvMinimum;
    FMDATA512 cvCurrent;
    unsigned long dwPointIndex;
    LEVELNAMES Names;
    wchar_t szDescription[ 31 ];
    wchar_t szStringOutput[ 81 ];
    unsigned short RunStatus;
    unsigned char bPointType;
    unsigned short bVariable;
    unsigned char bDataFormat;
    unsigned char bModifier;
    unsigned char bSourceType;
    unsigned char bEngrUnits;
    unsigned char bStyle;
    unsigned char bManAvailable;
    unsigned char bOverRideAvail;
    unsigned char bModifierEnable;
    unsigned char bTagCategory;
    } 	PNTCOMMAND;

typedef struct __MIDL_dmlink_0024 *PPNTCOMMAND;

typedef struct POINTALLOC
    {
    unsigned long dwPointId;
    unsigned long dwInterfaceID;
    unsigned long dwDeviceID;
    unsigned short wSystemID;
    unsigned short bVariable;
    unsigned char bSystemType;
    } 	POINTALLOC;

typedef struct POINTALLOC *PPOINTALLOC;

typedef struct SYSIDATA
    {
    wchar_t szNewSystem[ 18 ];
    unsigned short wSysID;
    unsigned char bSysType;
    unsigned char bpad;
    } 	SYSIDATA;

typedef struct SYSIDATA *PSYSIDATA;

typedef /* [public][public][public][public] */ struct __MIDL_dmlink_0025
    {
    double dLevel;
    double dVolume;
    } 	STRAPENTRY;

typedef struct __MIDL_dmlink_0025 *PSTRAPENTRY;

typedef /* [public] */ struct __MIDL_dmlink_0026
    {
    unsigned long dwPointID;
    unsigned short wTankIndex;
    unsigned long dwProductID;
    unsigned short wSystemID;
    unsigned char bRevCode;
    wchar_t szDetail[ 21 ];
    } 	TANKDETAIL;

typedef struct __MIDL_dmlink_0026 *PTANKDETAIL;

typedef /* [public] */ struct __MIDL_dmlink_0027
    {
    unsigned long bRename	: 1;
    unsigned long bCopyAlarm	: 1;
    unsigned long bCopyDesc	: 1;
    unsigned long bCopyStrap	: 1;
    unsigned long bCopyProd	: 1;
    unsigned long bReserv	: 24;
    unsigned long bTagLevels	: 3;
    } 	POINTCOPYFLAGS;

typedef struct __MIDL_dmlink_0027 *PPOINTCOPYFLAGS;

typedef /* [public] */ struct __MIDL_dmlink_0028
    {
    LEVELNAMES lvlSource;
    LEVELNAMES lvlDestination;
    unsigned long dwPointCopyFlags;
    } 	POINTCOPY;

typedef struct __MIDL_dmlink_0028 *PPOINTCOPY;

typedef /* [public] */ struct __MIDL_dmlink_0029
    {
    unsigned short TestType	: 8;
    unsigned short bPriority	: 2;
    unsigned short WriteAlarmToLog	: 1;
    unsigned short reserv	: 5;
    } 	ALARMTESTFLAGS;

typedef struct __MIDL_dmlink_0029 *PALARMTESTFLAGS;

typedef struct ALARMTEST_75
    {
    FMDATA512 TestValue;
    unsigned short Result;
    unsigned short wAlarmTestFlags;
    } 	ALARMTEST_75;

typedef struct ALARMTEST_75 *PALARMTEST_75;

typedef struct ALARMTEST
    {
    FMDATA512 TestValue;
    unsigned short Result;
    unsigned short wAlarmTestFlags;
    wchar_t szAlarmUserText[ 255 ];
    wchar_t szAlarmUserComment[ 255 ];
    } 	ALARMTEST;

typedef struct ALARMTEST *PALARMTEST;

typedef /* [public] */ struct __MIDL_dmlink_0030
    {
    unsigned long NumberLimits	: 5;
    unsigned long DataType	: 8;
    unsigned long DisplayStyle	: 4;
    unsigned long UseHyst	: 1;
    unsigned long Reserved	: 12;
    unsigned long Locked	: 1;
    unsigned long SharedDef	: 1;
    } 	ALARMDEFFLAGS;

typedef struct __MIDL_dmlink_0030 *PALARMDEFFLAGS;

typedef struct ALARMDEF
    {
    FMDATA512 fmHysterisis;
    unsigned long dwFirstTest;
    unsigned long dwAlarmDefFlags;
    } 	ALARMDEF;

typedef struct ALARMDEF *PALARMDEF;

typedef /* [public] */ struct __MIDL_dmlink_0031
    {
    unsigned long bVariable	: 16;
    unsigned long Composite	: 4;
    unsigned long FirstPass	: 1;
    unsigned long bNoComposite	: 1;
    unsigned long bUseAbsolute	: 1;
    unsigned long bDisabled	: 1;
    unsigned long Reserved	: 2;
    unsigned long NewAck	: 1;
    unsigned long ProcessDef	: 1;
    unsigned long GlobalDef	: 1;
    unsigned long Template	: 1;
    unsigned long NewAlarm	: 1;
    unsigned long AlarmLocked	: 1;
    } 	ALARMINDEXFLAGS;

typedef struct __MIDL_dmlink_0031 *PALARMINDEXFLAGS;

typedef struct ALARMINDEX
    {
    unsigned long dwAlarmID;
    unsigned long dwPointID;
    unsigned long dwAlarmProcess;
    unsigned long dwAlarmLink;
    unsigned char bCategory;
    unsigned long dwProcessIndex;
    unsigned long dwCompositeIndex;
    unsigned short wAlarmActive;
    unsigned short wAlarmAcknowleged;
    unsigned short wAlarmDisabled;
    unsigned short AlarmGraphic;
    unsigned short AlarmHelp;
    unsigned short wAlarmCommanded;
    long lEventHandle;
    unsigned long dwAlarmIndexFlags;
    unsigned char bAutoRun;
    wchar_t szGraphicName[ 80 ];
    wchar_t szTempAlias[ 50 ];
    } 	ALARMINDEX;

typedef struct ALARMINDEX *PALARMINDEX;

typedef /* [public] */ struct __MIDL_dmlink_0032
    {
    ALARMINDEX AlarmIndex;
    ALARMDEF AlarmDefinition;
    ALARMTEST TestEntries[ 16 ];
    } 	ALARMEDIT;

typedef struct __MIDL_dmlink_0032 *PALARMEDIT;

typedef /* [public] */ struct __MIDL_dmlink_0033
    {
    unsigned short bTemplate	: 1;
    unsigned short bSystemAlarm	: 1;
    unsigned short bHidden	: 1;
    unsigned short reserved	: 5;
    unsigned short bDataType	: 8;
    } 	GLOBALALRMIDXFLAGS;

typedef struct __MIDL_dmlink_0033 *PGLOBALALRMIDXFLAGS;

typedef /* [public][public][public][public][public][public][public] */ struct __MIDL_dmlink_0034
    {
    unsigned long dwAlarmDefinition;
    unsigned long dwUsedCounter;
    unsigned short wGlobalAlrmIndexFlags;
    wchar_t szAlarmName[ 16 ];
    wchar_t szAlarmDescription[ 31 ];
    } 	GLOBALALARMINDEX;

typedef struct __MIDL_dmlink_0034 *PGLOBALALARMINDEX;

typedef /* [public] */ struct __MIDL_dmlink_0035
    {
    ALARMDEF StdDef;
    ALARMTEST StdTest[ 16 ];
    GLOBALALARMINDEX StdIndex;
    } 	STDALARM;

typedef struct __MIDL_dmlink_0035 *PSTDALARM;

typedef /* [public] */ struct __MIDL_dmlink_0036
    {
    double dLevel;
    double dTemp;
    double dStdDensity;
    double dProdDensity;
    double dH2OLevel;
    double dBSW;
    double dGrossVolume;
    double dNetVolume;
    double dRemainVol;
    double dAvailVol;
    double dH2OVolume;
    double dSolidLevel;
    double dStrapVolume;
    double dVolumeCorrect;
    double dRoofVolume;
    double dSolidVolume;
    double dProdMass;
    double dRemainMass;
    double dAvailMass;
    double dBottomVolume;
    double dAmbientTemp;
    double dShellCorrection;
    double dGrossRemaining;
    double dGrossAvailable;
    double dVaporTemp;
    double dVaporPress;
    double dCorrectionVolume;
    double dGasDensity;
    double dVaporMass;
    double dVaporNetVolume;
    double dLiquidMass;
    double dTotalNetVolume;
    double dHydrometerValue;
    double dDensityTempValue;
    double dGaugeStdDensity;
    unsigned char bUseGaugeStdDensity;
    double dGrossStandardVolume;
    double dTotalCalculatedVolume;
    double dProdDensityInAir;
    double dStdDensityInAir;
    } 	TANKCALCULATE;

typedef struct __MIDL_dmlink_0036 *PTANKCALCULATE;

typedef /* [public] */ struct __MIDL_dmlink_0037
    {
    double dLevMin;
    double dLevMax;
    double dTempMin;
    double dTempMax;
    double dDensMin;
    double dDensMax;
    double dFillLimit;
    double dEmptyLimit;
    double dPressMin;
    double dPressMax;
    double dGasPressMin;
    double dGasPressMax;
    double dGaugeStdDensity;
    unsigned char bLevelUnits;
    wchar_t szLevelUnits[ 11 ];
    unsigned char bTempUnits;
    wchar_t szTempUnits[ 11 ];
    unsigned char bVolumeUnits;
    wchar_t szVolumeUnits[ 11 ];
    unsigned char bDensityUnits;
    wchar_t szDensityUnits[ 11 ];
    unsigned char bStdDensUnits;
    wchar_t szStdDensUnits[ 11 ];
    unsigned char bLevStyle;
    unsigned char bMassUnits;
    wchar_t szMassUnits[ 11 ];
    unsigned char bPressUnits;
    wchar_t szPressUnits[ 11 ];
    unsigned char bCorrectionTypeMajor;
    unsigned char bCorrectionTypeMinor;
    unsigned char bGasDensityUnits;
    wchar_t szGasDensityUnits[ 11 ];
    unsigned char bUseMeasuredDensity;
    } 	SETCALC;

typedef struct __MIDL_dmlink_0037 *PSETCALC;

typedef /* [public] */ struct __MIDL_dmlink_0038
    {
    unsigned long bLevelDefined	: 1;
    unsigned long bTempDefined	: 1;
    unsigned long bDensityDefined	: 1;
    unsigned long bFlowDefined	: 1;
    unsigned long bLevelRateDefined	: 1;
    unsigned long bPressureDefined	: 1;
    unsigned long bLeakRateDefined	: 1;
    unsigned long bOperationalAlarmDef	: 1;
    unsigned long bLevelChange	: 1;
    unsigned long bTempChange	: 1;
    unsigned long bDensityChange	: 1;
    unsigned long bFlowChange	: 1;
    unsigned long bLevelRateChange	: 1;
    unsigned long bPressureChange	: 1;
    unsigned long bLeakRateChange	: 1;
    unsigned long bGrossVolDefined	: 1;
    unsigned long bLevelDisabled	: 1;
    unsigned long bTempDisabled	: 1;
    unsigned long bDensityDisabled	: 1;
    unsigned long bFlowDisabled	: 1;
    unsigned long bLevelRateDisabled	: 1;
    unsigned long bPressureDisabled	: 1;
    unsigned long bLevelMaxOperatingDisabled	: 1;
    unsigned long bLevelMinOperatingDisabled	: 1;
    unsigned long bLeakRateDisabled	: 1;
    unsigned long bLevelAlarmCommanded	: 1;
    unsigned long bCategory	: 8;
    unsigned long bGrossVolChanged	: 1;
    unsigned long bGrossVolDisabled	: 1;
    unsigned long bNetVolDefined	: 1;
    unsigned long bNetVolChanged	: 1;
    unsigned long bNetVolDisabled	: 1;
    unsigned long bMassDefined	: 1;
    unsigned long bMassChanged	: 1;
    unsigned long bMassDisabled	: 1;
    unsigned long bGrossStdVolDefined	: 1;
    unsigned long bGrossStdVolChanged	: 1;
    unsigned long bGrossStdVolDisabled	: 1;
    unsigned long bWeightGrossStdDefined	: 1;
    unsigned long bWeightGrossStdChanged	: 1;
    unsigned long bWeightGrossStdDisabled	: 1;
    unsigned long bWeightNetStdDefined	: 1;
    unsigned long bWeightNetStdChanged	: 1;
    unsigned long bWeightNetStdDisabled	: 1;
    } 	TANKALARMEDITFLAGS;

typedef struct __MIDL_dmlink_0038 *PTANKALARMEDITFLAGS;

typedef /* [public] */ struct __MIDL_dmlink_0039
    {
    double dLevelHiHi;
    double dLevelHigh;
    double dLevelLow;
    double dLevelLoLo;
    double dTempHiHi;
    double dTempHigh;
    double dTempLow;
    double dTempLoLo;
    double dDensityHigh;
    double dDensityLow;
    double dFlowHigh;
    double dFlowLow;
    double dLevelRateHigh;
    double dLevelRateLow;
    double dPressureHigh;
    double dPressureLow;
    double dLeakRateHigh;
    double dLeakRateLow;
    double dLevelHyst;
    double dTempHyst;
    double dDensityHyst;
    double dFlowHyst;
    double dLevelRateHyst;
    double dPressureHyst;
    double dLeakRateHyst;
    double dLevelMaxOperating;
    double dLevelMinOperating;
    unsigned char bLvlHiHiPriority;
    unsigned char bLvlHiPriority;
    unsigned char bLvlLoLoPriority;
    unsigned char bLvlLoPriority;
    unsigned char bTempHiHiPriority;
    unsigned char bTempHiPriority;
    unsigned char bTempLoLoPriority;
    unsigned char bTempLoPriority;
    unsigned short wLvlHiHiCodedVar;
    unsigned short wLvlHiCodedVar;
    unsigned short wLvlLoLoCodedVar;
    unsigned short wLvlLoCodedVar;
    unsigned short wTempHiHiCodedVar;
    unsigned short wTempHiCodedVar;
    unsigned short wTempLoLoCodedVar;
    unsigned short wTempLoCodedVar;
    unsigned long dwTankAlrmEditFlags[ 2 ];
    wchar_t szAlarmName[ 16 ];
    wchar_t szLevelHiHi[ 255 ];
    wchar_t szLevelHigh[ 255 ];
    wchar_t szLevelLow[ 255 ];
    wchar_t szLevelLoLo[ 255 ];
    wchar_t szTempHiHi[ 255 ];
    wchar_t szTempHigh[ 255 ];
    wchar_t szTempLow[ 255 ];
    wchar_t szTempLoLo[ 255 ];
    wchar_t szDensityHigh[ 255 ];
    wchar_t szDensityLow[ 255 ];
    wchar_t szFlowHigh[ 255 ];
    wchar_t szFlowLow[ 255 ];
    wchar_t szLevelRateHigh[ 255 ];
    wchar_t szLevelRateLow[ 255 ];
    wchar_t szPressureHigh[ 255 ];
    wchar_t szPressureLow[ 255 ];
    wchar_t szLeakRateHigh[ 255 ];
    wchar_t szLeakRateLow[ 255 ];
    wchar_t szLevelMaxOperating[ 255 ];
    wchar_t szLevelMinOperating[ 255 ];
    double dGrossVolHiHi;
    double dGrossVolHigh;
    double dGrossVolLow;
    double dGrossVolLoLo;
    double dNetVolHiHi;
    double dNetVolHigh;
    double dNetVolLow;
    double dNetVolLoLo;
    double dMassHiHi;
    double dMassHigh;
    double dMassLow;
    double dMassLoLo;
    double dGrossVolHyst;
    double dNetVolHyst;
    double dMassHyst;
    unsigned char bGrossVolHiHiPriority;
    unsigned char bGrossVolHiPriority;
    unsigned char bGrossVolLoLoPriority;
    unsigned char bGrossVolLoPriority;
    unsigned char bNetVolHiHiPriority;
    unsigned char bNetVolHiPriority;
    unsigned char bNetVolLoLoPriority;
    unsigned char bNetVolLoPriority;
    unsigned char bMassHiHiPriority;
    unsigned char bMassHiPriority;
    unsigned char bMassLoLoPriority;
    unsigned char bMassLoPriority;
    double dGrossStdVolHiHi;
    double dGrossStdVolHigh;
    double dGrossStdVolLow;
    double dGrossStdVolLoLo;
    double dGrossStdVolHyst;
    unsigned char bGrossStdVolHiHiPriority;
    unsigned char bGrossStdVolHiPriority;
    unsigned char bGrossStdVolLoLoPriority;
    unsigned char bGrossStdVolLoPriority;
    double dWeightGrossStdHiHi;
    double dWeightGrossStdHigh;
    double dWeightGrossStdLow;
    double dWeightGrossStdLoLo;
    double dWeightGrossStdHyst;
    unsigned char bWeightGrossStdHiHiPriority;
    unsigned char bWeightGrossStdHiPriority;
    unsigned char bWeightGrossStdLoLoPriority;
    unsigned char bWeightGrossStdLoPriority;
    double dWeightNetStdHiHi;
    double dWeightNetStdHigh;
    double dWeightNetStdLow;
    double dWeightNetStdLoLo;
    double dWeightNetStdHyst;
    unsigned char bWeightNetStdHiHiPriority;
    unsigned char bWeightNetStdHiPriority;
    unsigned char bWeightNetStdLoLoPriority;
    unsigned char bWeightNetStdLoPriority;
    } 	TANKALARMEDIT;

typedef struct __MIDL_dmlink_0039 *PTANKALARMEDIT;

typedef /* [public] */ struct __MIDL_dmlink_0040
    {
    unsigned long TestCode	: 4;
    unsigned long reserved	: 20;
    unsigned long bMatchTagName	: 1;
    unsigned long bMatchPointType	: 1;
    unsigned long bMatchDataType	: 1;
    unsigned long bMatchVarTest	: 1;
    unsigned long bresrvflag	: 4;
    } 	POINTFILTERFLAGS;

typedef struct __MIDL_dmlink_0040 *PPOINTFILTERFLAGS;

typedef /* [public] */ struct __MIDL_dmlink_0041
    {
    FMDATA512 cvValue;
    LEVELNAMES TagMask;
    unsigned short wDataMask;
    unsigned char bPointType;
    unsigned char bVariable;
    unsigned long dwPointFilterFlags;
    } 	POINTFILTER;

typedef struct __MIDL_dmlink_0041 *PPOINTFILTER;

typedef /* [public] */ struct __MIDL_dmlink_0042
    {
    unsigned long dwInterfaceID;
    unsigned long dwDeviceID;
    wchar_t szCMSystem[ 18 ];
    unsigned char bSpare1;
    unsigned char bSpare2;
    unsigned char bSpare3;
    unsigned char bSpare4;
    } 	DEVICESOURCE;

typedef struct __MIDL_dmlink_0042 *PDEVICESOURCE;

typedef /* [public][public][public] */ struct __MIDL_dmlink_0043
    {
    wchar_t szName[ 16 ];
    } 	TAGNAME;

typedef struct __MIDL_dmlink_0043 *PTAGNAME;

typedef /* [public] */ struct __MIDL_dmlink_0044
    {
    wchar_t szGroups[ 5 ];
    wchar_t szTimeOccurred[ 20 ];
    wchar_t szTag[ 65 ];
    wchar_t szDescription[ 31 ];
    wchar_t szCurrentState[ 16 ];
    wchar_t szAlarmPriority[ 3 ];
    wchar_t szVariable[ 21 ];
    wchar_t szValue[ 81 ];
    wchar_t szUnits[ 11 ];
    wchar_t szAction[ 20 ];
    wchar_t szUser[ 21 ];
    wchar_t szSeqNumber[ 20 ];
    } 	DMLOG;

typedef struct __MIDL_dmlink_0044 *PDMLOG;

typedef /* [public] */ struct __MIDL_dmlink_0045
    {
    unsigned long dwStringCode;
    unsigned short PointIndex;
    unsigned char PointType;
    unsigned char bReserved;
    } 	TAG_ENTRY;

typedef struct __MIDL_dmlink_0045 *PTAGENTRY;

unsigned long ValidateDMStatus( 
    /* [in] */ handle_t hBinding);

unsigned char Get_DM_Parameters( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned char *blevels,
    /* [out] */ unsigned short *pID);

short GetLevelTags( 
    /* [in] */ handle_t hBinding,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ TAGNAME **ppTagName,
    /* [size_is][size_is][out] */ TAGEXTEND **ppTagExtend);

short GetPointStrings( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwPointID,
    /* [out] */ LEVELNAMES *Names);

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
    /* [size_is][size_is][out] */ TAGEXTEND **ppTagExtend);

short AddDatabasePoint( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ POINTSPEC *pReturn);

short GetPointItems( 
    /* [in] */ handle_t hBinding,
    /* [in] */ LEVELNAMES *Names,
    /* [out] */ POINTSPEC *pReturn);

unsigned long GetPointVariable( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short wPointIndex,
    /* [in] */ unsigned char bPointType,
    /* [in] */ unsigned short bVariable,
    /* [out] */ VARDETAIL *pReturn);

short EditDatabaseStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PDATAPOINT pReturn);

short EditDatabaseCancel( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ unsigned char bType);

short EditDatabaseDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PDATAPOINT pNewPoint);

short EditTankDataStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PTANKDATA pReturn);

short EditTankDataDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PTANKDATA pNewPoint);

short PurgeDataPoint( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned long dwPntID);

short QueryLockoutUser( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lAction,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ short sNameLen,
    /* [in] */ short sSysLen,
    /* [size_is][out] */ wchar_t pUserName[  ],
    /* [size_is][out] */ wchar_t pSystem[  ]);

unsigned long RPC_SaveDatabase( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName);

unsigned long RPC_ValidateBlock( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lsize,
    /* [size_is][in] */ PBLOCKREQENTRY pBlock,
    /* [size_is][out] */ unsigned char pResult[  ]);

unsigned long RPC_CommandData( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [in] */ unsigned long dwPointId,
    /* [in] */ unsigned short bVarType,
    /* [out] */ PPNTCOMMAND pOutput);

unsigned long RPC_OperatorCommand( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short bVarType,
    /* [in] */ unsigned char bFormat,
    /* [in] */ unsigned char bOverride,
    /* [in] */ PFMDATA512 pDataIn);

unsigned long RPC_AllocateSource( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned char bAllocate,
    /* [in] */ PPOINTALLOC pInput,
    /* [in] */ long nSourceIndex);

unsigned long RPC_GetSystemString( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bSysIndex,
    /* [size_is][out] */ wchar_t pSysName[  ]);

unsigned long RPC_GetSystemIndex( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pUserSystem,
    /* [in] */ PSYSIDATA pSysParam,
    /* [in] */ unsigned char cAdd,
    /* [out] */ unsigned char *pIndex);

unsigned long RPC_EnumerateVariables( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PVARMASK pInput,
    /* [out] */ unsigned short *pbArraySize,
    /* [size_is][size_is][out] */ unsigned short **ppResult);

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
    /* [size_is][size_is][out] */ STRAPENTRY **pHydroOut);

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
    /* [size_is][in] */ PSTRAPENTRY pHydroOut);

short EnumerateTanks( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PFM_DESC_STRUCT *stDescription,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppTankTags);

unsigned long GetTankDetail( 
    /* [in] */ handle_t hBinding,
    /* [in] */ LEVELNAMES *Names,
    /* [out] */ PTANKDETAIL pTankDetail);

unsigned long RPC_AlarmAck( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [in] */ unsigned long dwAlarmID,
    /* [in] */ unsigned char bOffSet);

unsigned long RPC_SilenceAlarm( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwAlarmAccess);

unsigned long RPC_SetProduct( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [in] */ unsigned long dwTankID,
    /* [in] */ unsigned long dwTankIndex,
    /* [in] */ unsigned long dwProductId);

unsigned long RPC_EditPointAlarm( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned char bVariable,
    /* [out] */ PALARMEDIT pAlarmData);

unsigned long RPC_EditPointAlarmDone( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned char bVariable,
    /* [out][in] */ PALARMEDIT pNewAlarm);

unsigned long RPC_DeletePointAlarm( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned char bVariable);

unsigned long RPC_EnumGlobalAlarms( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bTemplate,
    /* [in] */ unsigned char bDataType,
    /* [out] */ long *plAccessTime,
    /* [out] */ unsigned short *pOutNum,
    /* [size_is][size_is][out] */ GLOBALALARMINDEX **ppGlobalAlarmIndex);

unsigned long RPC_EditGlobalAlarm( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwAlarmId,
    /* [in] */ long lAccessTime,
    /* [string][in] */ wchar_t *pAlarmName,
    /* [out] */ PSTDALARM pAlarmData);

unsigned long RPC_EditGlobalAlarmDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwAlarmId,
    /* [in] */ PSTDALARM pNewAlarm);

unsigned long RPC_AddGlobalAlarm( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ PSTDALARM pNewAlarm,
    /* [out] */ unsigned long *pDwAlarmId);

unsigned long RPC_DeleteGlobalAlarm( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [in] */ unsigned long dwAlarmId,
    /* [in] */ long lAccessTime,
    /* [string][in] */ wchar_t *pAlarmName);

unsigned long RPC_CancelAlarmEdit( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwAlarmID);

unsigned long RPC_GetGlobalAlarmData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwAlarmdef,
    /* [in] */ long lAccessTime,
    /* [string][in] */ wchar_t *pAlarmName,
    /* [out] */ PSTDALARM pReturn);

unsigned long RPC_GetGlobalAlarmName( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwAlarmDef,
    /* [size_is][out] */ wchar_t pName[  ]);

unsigned long RPC_GetExtendedAlarmData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwAlarmID,
    /* [out] */ unsigned short *pGraphic,
    /* [out] */ unsigned short *pHelp,
    /* [out] */ unsigned char *bAutoLoad,
    /* [size_is][out] */ wchar_t szGraphic[  ],
    /* [size_is][out] */ wchar_t szTemplate[  ]);

unsigned long RPC_AllocateTankCalc( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ PLEVELNAMES pTankName,
    /* [out] */ unsigned long *pCalcTank,
    /* [out] */ PSETCALC pSet);

unsigned long RPC_QuitTankCalc( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwTankIndex);

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
    /* [out] */ PTANKCALCULATE pReturn);

unsigned long RPC_SearchTags( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PPOINTFILTER pFilter,
    /* [out] */ unsigned short *pFound,
    /* [size_is][size_is][out] */ POINTSPEC **ppPointSpec);

unsigned long RPC_EditTankAlarm( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [out] */ PTANKALARMEDIT pTankAlarm);

unsigned long RPC_EditTankAlarmDone( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned char bOutageTank,
    /* [in] */ PTANKALARMEDIT pNewAlarm);

unsigned long RPC_GetDeviceSource( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwPntID,
    /* [out] */ PDEVICESOURCE pGaugeData);

unsigned long RPC_GetDeviceSourceEx( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned short wSelectedVariable,
    /* [in] */ unsigned char bDoNotLockMutex,
    /* [out] */ PDEVICESOURCE pGaugeData);

unsigned long RPC_GetSystemIndexData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bIndex,
    /* [out] */ PSYSIDATA pSysParam);

unsigned long RPC_AllocPointSource( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned short bVariable,
    /* [in] */ unsigned char bClear,
    /* [in] */ PFMSOURCE pSource,
    /* [in] */ long nSourceIndex);

unsigned long RPC_AddEventRecord( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ unsigned long bAdvise,
    /* [in] */ FMDATA512 prDeadBand,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned long wDataType,
    /* [in] */ unsigned char bCategory,
    /* [out] */ long *plNewHandle);

unsigned long RPC_GetEventAttributes( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ long lHandle,
    /* [out] */ unsigned long *pwDataType,
    /* [out] */ unsigned long *pbAdvise,
    /* [out] */ unsigned char *pbCategory,
    /* [out] */ PFMDATA512 prDeadBand);

unsigned long RPC_SetEventAttributes( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ long lHandle,
    /* [in] */ unsigned long wDataType,
    /* [in] */ unsigned long bAdvise,
    /* [in] */ unsigned char bCategory,
    /* [in] */ FMDATA512 prDeadBand);

unsigned long RPC_RemoveEventRecord( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ long lHandle);

unsigned long RPC_ChangeDataType( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ long lHandle,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned long dwNewDataType);

unsigned long RPC_GetLogPath( 
    /* [in] */ handle_t hBinding,
    /* [size_is][out][in] */ wchar_t szUniversalName[  ]);

unsigned long RPC_GetData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PBLOCKREQENTRY pBlockReq,
    /* [out][in] */ unsigned long *ulStatus1,
    /* [out][in] */ unsigned long *ulStatus2,
    /* [out][in] */ unsigned char *byChanged,
    /* [size_is][out][in] */ unsigned char *pData,
    /* [in] */ unsigned long ulDataSize);

unsigned long RPC_SetData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PBLOCKREQENTRY pBlockReq,
    /* [in] */ PFMDATA512 pDataIn);

unsigned long RPC_AddComment( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PDMLOG pLogEntry,
    /* [string][in] */ wchar_t *pszFileName);

unsigned long RPC_AddOpcFileEntry( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t szCLSID[ 256 ],
    /* [string][in] */ wchar_t szSystem[ 18 ],
    /* [string][in] */ wchar_t szTag[ 256 ],
    /* [string][in] */ wchar_t szServer[ 256 ],
    /* [out][in] */ unsigned long *dwTagID);

unsigned long RPC_GetOpcSourceStrings( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwTagID,
    /* [size_is][out] */ wchar_t szSystem[  ],
    /* [size_is][out] */ wchar_t szTag[  ],
    /* [size_is][out] */ wchar_t szServer[  ],
    /* [size_is][out] */ wchar_t szCLSID[  ]);

unsigned long RPC_DeleteOpcEntry( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwTagID);

unsigned long RPC_GetOpcUAEntryData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bPointType,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short wPointIndex,
    /* [in] */ unsigned short bVariable,
    /* [in] */ unsigned short wAllocatedType,
    /* [in] */ int iSourceIndex,
    /* [size_is][out][in] */ unsigned char *pData,
    /* [in] */ unsigned long ulDataSize);

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
    /* [out] */ unsigned long *dwDataIndex);

unsigned long RPC_DeleteOpcUAEntry( 
    /* [in] */ handle_t hBinding,
    /* [in] */ OPCUA OpcData);

unsigned long RPC_GetOpcUASourceString( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bPointType,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short wPointIndex,
    /* [in] */ unsigned short bVariable,
    /* [in] */ unsigned short wAllocatedType,
    /* [in] */ int iSourceIndex,
    /* [size_is][out][in] */ wchar_t szServer[  ],
    /* [size_is][out][in] */ wchar_t szSelectedOpcTag[  ]);

short RPC_EnumerateCopyPoints( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystemName,
    /* [in] */ PPOINTCOPY pntCopy,
    /* [out] */ unsigned long *pNumberPoints,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppNewTagArray,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppOldTagArray,
    /* [size_is][size_is][out] */ PTAGENTRY *ppTagEntry);

unsigned long RPC_ExecutePointCopy( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystemName,
    /* [in] */ PPOINTCOPY pntCopy,
    /* [in] */ PLEVELNAMES pNewNamesArray,
    /* [in] */ PTAGENTRY pOldTagArray);

unsigned long RPC_GetFormattedData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PBLOCKREQENTRY pBlockReq,
    /* [size_is][out] */ wchar_t *szOutput,
    /* [size_is][out] */ wchar_t *szEngUnits,
    /* [size_is][out] */ wchar_t *szQuality);

unsigned long RPC_SetDataFormatted( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PBLOCKREQENTRY pBlockReq,
    /* [size_is][in] */ wchar_t *szInput);

short EnumerateInputPoints( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppInputTags);

short EnumerateOutputPoints( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppOutputTags);

unsigned long RPC_SetSealData( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ SEALDATA *pSealData);

unsigned long RPC_GetSealID( 
    /* [in] */ handle_t hBinding,
    /* [in] */ LEVELNAMES *Name,
    /* [out] */ unsigned long *pdwSealID);

unsigned long RPC_EnumerateTranslations( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned short *pbArraySize,
    /* [size_is][size_is][out] */ PFM_TRANSLATIONNAME *ppResult);

unsigned long RPC_GetNewTranslation( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned char bType,
    /* [out] */ PFM_TRANSLATIONNAME pName);

unsigned long RPC_DeleteTranslation( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lIndex);

unsigned long RPC_EditTranslation( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lIndex,
    /* [out] */ PFM_TRANSLATION pTranslation);

unsigned long RPC_EditTranslationCancel( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lIndex);

unsigned long RPC_EditTranslationDone( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lIndex,
    /* [in] */ PFM_TRANSLATION pTranslation);

unsigned long RPC_IsValidTranslation( 
    /* [in] */ handle_t hBinding,
    /* [in] */ long lIndex,
    /* [out] */ unsigned char *pfValid);

unsigned long RPC_FindTranslationByName( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pszName,
    /* [out] */ long *plIndex);

unsigned int RPC_CalculateLeakRate( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pszSystem,
    /* [string][in] */ wchar_t *pszPointTag,
    /* [in] */ unsigned short sAnalysisMethod,
    /* [in] */ unsigned short sAnalysisType,
    /* [in] */ long tmStartTime,
    /* [in] */ long tmEndTime,
    /* [out][in] */ LPLEAKANALYSISRESULT lpAnalysisResult);

short EnumerateRealTimeLeakTanks( 
    /* [in] */ handle_t hBinding,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppTankTags);

unsigned long SetLeakRateValue( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned short wPointIndex,
    /* [in] */ double dLeakRate,
    /* [in] */ unsigned char bNotEnoughDataAlarm);

unsigned int RPC_DeleteReportData( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pszLeakRecordID);

unsigned long RPC_GetRawData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ PBLOCKREQENTRY pBlockReq,
    /* [size_is][out] */ wchar_t *szOutput,
    /* [size_is][out] */ wchar_t *szEngUnits,
    /* [size_is][out] */ wchar_t *szQuality);

unsigned long RPC_PostGroupData( 
    /* [in] */ handle_t hBinding,
    /* [in] */ unsigned long dwNumberBlockReq,
    /* [in] */ unsigned long dwStatusOffset,
    /* [size_is][in] */ PBLOCKREQENTRY pBlockReq,
    /* [out] */ unsigned long *pdwSize,
    /* [size_is][size_is][out] */ unsigned char **pbData);

unsigned long RPC_OperatorCommandEX( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [string][in] */ wchar_t *pPointDescription,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned short bVarType,
    /* [in] */ unsigned char bFormat,
    /* [in] */ unsigned char bOverride,
    /* [in] */ PFMDATA512 pDataIn);

short EditFlowMeterStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PFLOWMETERPOINT pReturn);

short EditFlowMeterCancel( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex);

short EditFlowMeterDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PFLOWMETERPOINT pNewPoint);

short EditPipeLineStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PPIPELINEPNT pReturn);

short EditPipeLineCancel( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex);

short EditPipeLineDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PPIPELINEPNT pNewPoint);

short EditLogicPntStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PLOGICPNT pReturn);

short EditLogicPntCancel( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex);

short EditLogicPntDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PLOGICPNT pNewPoint);

short EditTimerPntStart( 
    /* [in] */ handle_t hBinding,
    /* [out] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ LEVELNAMES *Names,
    /* [in] */ unsigned char bType,
    /* [out] */ PTIMERPNT pReturn);

short EditTimerPntCancel( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex);

short EditTimerPntDone( 
    /* [out][in] */ PDM_LOCK_CTXT_HNDL *pphContext,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pSystem,
    /* [in] */ unsigned long dwPntID,
    /* [in] */ unsigned long dwIndex,
    /* [in] */ PTIMERPNT pNewPoint);

short EnumerateMeterPoints( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PFM_DESC_STRUCT *stDescription,
    /* [size_is][size_is][out] */ unsigned char **ppMeterTypes,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppMeterTags);

short EnumeratePipelinePoints( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [out] */ unsigned short *pwArraySize,
    /* [size_is][size_is][out] */ PFM_DESC_STRUCT *stDescription,
    /* [size_is][size_is][out] */ PLEVELNAMES *ppTankTags);

unsigned long RPC_UpdateAlarmComment( 
    /* [in] */ handle_t hBinding,
    /* [string][in] */ wchar_t *pUserName,
    /* [string][in] */ wchar_t *pComment,
    /* [in] */ unsigned long dwAlarmID,
    /* [in] */ unsigned long dwPointID,
    /* [in] */ unsigned char bOffSet);

short RPC_IsPointInActiveMovement( 
    /* [in] */ PDM_LOCK_CTXT_HNDL phContext,
    /* [in] */ unsigned long dwPointID);



extern RPC_IF_HANDLE dmlink_ClientIfHandle;
extern RPC_IF_HANDLE dmlink_ServerIfHandle;
#endif /* __dmlink_INTERFACE_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

void __RPC_USER PDM_LOCK_CTXT_HNDL_rundown( PDM_LOCK_CTXT_HNDL );

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


