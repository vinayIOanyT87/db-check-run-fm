#pragma once
// error defines from legacy system
#define	FM_ERROR_NONE						0x20000000	// No Error

// defines for tank calculator
// // Define Tank Calculator Special Processing Flags
#define	NO_SPECIAL_PROCESSING	0
#define	GROSSVOL_LEVEL				1
#define	STRAPVOL_LEVEL				2
#define	NETVOL_LEVEL				3
#define	SOLIDVOL_LEVEL				4
#define	WATERVOL_LEVEL				5
#define	MASS_LEVEL					6

// define tank calculator parameter override flags
#define	FMTANK_CALC_LEVEL				0x0001
#define	FMTANK_CALC_TEMP				0x0002
#define	FMTANK_CALC_AMBIENTTEMP			0x0004
#define	FMTANK_CALC_DENSITY				0x0008
#define	FMTANK_CALC_WTRLEVEL			0x0010
#define	FMTANK_CALC_BSW					0x0020
#define	FMTANK_CALC_SOLIDSLEVEL			0x0040
#define	FMTANK_CALC_VAPORTEMP			0x0080
#define	FMTANK_CALC_VAPORPRESS			0x0100
#define	FMTANK_CALC_GASDENSITY			0x0200
#define	FMTANK_CALC_CORRVOLUME			0x0400
#define	FMTANK_CALC_STDDENSITY			0x0800
#define	FMTANK_GROSSVOL_LEVEL			0x1000
#define	FMTANK_NETVOL_LEVEL				0x2000

//structures for various functions
typedef	struct
{
	double 	dTestLevel;
	double 	dTestTemp;
	double 	dTestDens;
	double 	dTestH2O;
	double 	dTestBSW;
	double	dTestSolid;
	double 	dXfrValue;
	double	dMinGrossVol;
	double	dMaxGrossVol;
	double	dTestAmbientTemp;
	double	dTestVaporTemp;
	double	dTestVaporPress;
	double	dTestCorrectionVolume;
	double	dTestGasDensity;
	double	dTestDensityTemp;
	double	dGaugeStdDensity;
	DWORD		dwCalcID;
	DWORD		dwXfrBaseField;
}CALCDATA, * PCALCDATA;

BOOL RunCalculator(BOOL	bUpDate,
	PSETCALC	pSetup,
	PDM_LOCK_CTXT_HNDL hDmLockCtxt,
	PTANKCALCULATE 		pTankCalcData,
	PCALCDATA 				pTankSetData);

typedef struct _MYMOVEMENTGROUP
{
    wchar_t* szName;
}MYMOVEMENTGROUP;

typedef struct _MYPRINTERDATA
{
	byte byLen;
	byte byDefault;
	char* szName;
}MYPRINTERDATA;

typedef struct _MYDELIVERYTICKETNAME
{
	wchar_t* szName;
}MYDELIVERYTICKETNAME;

typedef struct _MYNODEINSTANCEDATA
{
    wchar_t* szName;// [37] ;
    wchar_t* szNameOld;// [37] ;
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
    wchar_t* szTankDataBaseReference;// [129] ;
    wchar_t* szMeterGrossReference;// [129] ;
    wchar_t* szMeterNetReference;// [129] ;
    wchar_t* szMeterMassReference;// [129] ;
}MYNODEINSTANCEDATA;

typedef struct _MYMOVEINSTANCEDATA
{
    wchar_t* szName;// [21] ;
    wchar_t* szOrder;//[21];
    wchar_t* szComment;//[201];
    wchar_t** szUserDef;//[10][31];
    wchar_t* szGroup;//[21];
    wchar_t* szReportName;//[81];
    wchar_t* szPrinterName;//[81];
    wchar_t* szInputPoint;//[81];
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
    MYNODEINSTANCEDATA* pNodeInstanceData;
}MYMOVEINSTANCEDATA;


typedef struct _Person
{
    wchar_t* FirstName;
    wchar_t* LastName;
    wchar_t Grades[4][2];
    int Age;
}Person;