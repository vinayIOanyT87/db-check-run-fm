/************************************************************************************

	FILE NAME:  FmRpc.h


	FUNCTIONS:	


	PURPOSE:


	CALLING PARAMETERS:


   MESSAGES:


   COMMENTS:


   AUTHOR:	Warren Gray


   VERSION:	4.0.0.0  09-Mar-97   Initial Version for Windows NT 4.0
	   		1.0.0.0  09-Sep-93   

   
   MODIFICATION HISTORY:
		Date: 	  	By:			Reason:
		----------------------------------------------------------------------------
		13-Mar-97	B. Bias		4.0.0.0 - Changed DEVICEFLAGS structure bit mapping.
							      	bReserv	was 18 bits, making a total of 36 bits in
								      the ULONG.

		16-Oct-97	W.Gray		4.0.0.1 - Changed DataIndex to double word in BLOCKREQENTRY
										MOved BLOCKREQHDR from NTWMSG.HPP, Changed NumberBlockReq
										and StatusOffset to double word

		08-May-98	W.Gray		4.0.0.2 - Moved TAGEXTEND typedef from DMLINK.IDL
	
		10-Jul-00	W,Gray		4.3.0.0 - Redefined BLOCKREQENTRY removed BREQFLAGS
										and moved bPointType, bVariable, bFormat, and bChangedCode
										into BLOCKREQENTRY.  Added bUnits which allows the specification
										of units in which the value should be supplied.  When bUnits
										equal FMU_Source the value is supplied in the Source Units.

  		06-Nov-03	I.Orndorff	4.3.6.0 - Added LEAKANALYSISRESULT.

		18-Nov-03	I.Orndorff	4.3.6.1 - Added "szLeakRecordID" and "wMinGaugeTestTime"
													 to LEAKANALYSISRESULT.

		06-Apr-04	I.Orndorff	4.3.6.2 - Added "dGraphTempDelta" and "wGaugeType"
													 to LEAKANALYSISRESULT.
	  
	Coggins Systems, Inc
	5834 Peachtree Corners East
	Norcross, GA  30092

   Copyright (c) Coggins Systems, Inc, 1993


************************************************************************************/

#ifndef	FmRpc
#define	FmRpc

#define FM_MAXLEVEL                    4     // must match #define in FMCOMMON - fuelcomm.hpp

#define	FM_MAXSTRINGDATA_LENGTH			255	// Maximum data string length allowed to be stored in database

typedef double FMDATA8;
	
typedef FMDATA8 * PFMDATA8;

typedef unsigned short FMDATA512[255];

typedef FMDATA512 * PFMDATA512;


/*****************************************************************************
					Define Structures for Data Block
*****************************************************************************/

typedef	struct
{
	unsigned long	dwNumberBlockReq;		// Number of BlockReqEntries
	unsigned long	dwStatusOffset; 		// Word alligned Byte offset of status block
} BLOCKREQHDR, * PBLOCKREQHDR;


typedef struct
{
	unsigned long		dwPointID;							// Unique Coded Point Identifier
	unsigned short		wPointIndex;						// Point Structure Index
	unsigned char		bPointType;							// Type of Point
	unsigned short		bVariable;							// Variable Code
	unsigned char		bFormat;								// Requested Data Format
	unsigned char		bUnits;								// Requested Units
	unsigned long		dwDataIndex;						// Index into data block
} BLOCKREQENTRY, * PBLOCKREQENTRY;

//              define structure for Fuels Manager Time Storage
#pragma pack(1)
typedef struct
{
	long            time;                           // Standard Seconds since 00:00:00 Jan 1, 1970
	unsigned short  millisec;                       // Milliseconds ( resolution to 10 msec)

}FMTIMEDATA, * PFMTIMEDATA;

//              define structure for generic FM point storage
typedef struct
{
	unsigned long                   dwPointId;
	unsigned char                   bType;
	unsigned char                   bVariable;
	unsigned short                  wSystemId;
} FMPOINTDATA, *PFMPOINTDATA;
#pragma pack()
//              Define Tag struture to store single DM level tag.
typedef wchar_t  TAGSTRING[16];         // Unicode Symbol String

typedef TAGSTRING       *       PTAGSTRING; // Pointer to Symbol String

// Define Structure to Store up to Four Level Names
typedef struct
{
	TAGSTRING       szLevelString[FM_MAXLEVEL];     // Array of Level Symbols

} LEVELNAMES, *PLEVELNAMES;

// Define Structure for FM Data Storage Short
typedef union                                   
{
	signed char		cvalue;                //  char  +/- 127
	unsigned char  bvalue;                //  unsigned byte  0 - 255
	short          ivalue;                //  signed 16 bit
	unsigned short uvalue;                //  unsigned 16 bit value
	long           livalue;               //  signed 32 Bit Integer
	unsigned long  luvalue;               //  unsigned long
	float          fvalue;                //  Real Nos.
} FMDATASTORAGESHORT, *PFMDATASTORAGESHORT;


// Define Structure for FM Data Storage Large
typedef union                                   
{
	signed char		cvalue;                //  char  +/- 127
	unsigned char  bvalue;                //  unsigned byte  0 - 255
	short          ivalue;                //  signed 16 bit
	unsigned short uvalue;                //  unsigned 16 bit value
	long           livalue;               //  signed 32 Bit Integer
	unsigned long  luvalue;               //  unsigned long
	float          fvalue;                //  Real Nos.
	double         dvalue;                //  Double Precision Real Nos.
	FMTIMEDATA     fmTimeValue;           //  Fuels Manager Time Value
	FMPOINTDATA    fmPointData;           //       Fuels Manager Point Reference
} FMDATASTORAGE_OLD, *PFMDATASTORAGE_OLD;

typedef union                                   
{
	signed char		cvalue;                //  char  +/- 127
	unsigned char  bvalue;                //  unsigned byte  0 - 255
	short          ivalue;                //  signed 16 bit
	unsigned short uvalue;                //  unsigned 16 bit value
	long           livalue;               //  signed 32 Bit Integer
	unsigned long  luvalue;               //  unsigned long
	float          fvalue;                //  Real Nos.
	double         dvalue;                //  Double Precision Real Nos.
	unsigned short	szStringData[255];
	FMTIMEDATA     fmTimeValue;           //  Fuels Manager Time Value
	FMPOINTDATA    fmPointData;           //       Fuels Manager Point Reference
} FMDATASTORAGE, *PFMDATASTORAGE;

// Define basic point data structure
typedef struct
{
	unsigned long					dwPointID;
	unsigned short					wPointIndex;
	unsigned char					bPointType;
	unsigned char					bPointCategory;
	unsigned char					bReserv[2];
}POINTSPEC,*PPOINTSPEC;

	//              Define Structure for Point Variable Enumeration Function
typedef struct VARMASK
{
	unsigned long			   dwPoint;                                           // Point ID                   
	unsigned short          wFormatMask;                       // Format Mask                        
	unsigned short          wUnitsMask;                                // Engineering Units Mask             
	unsigned char           bPointType;                                     // Point Type         
	unsigned char           bSearchMode;                       // Masked Search Mode 
	unsigned char           bPad[2];
}       VARMASK,*PVARMASK;

	// Define Structure to Return Point Variable Details
	// Define Structure to Return Point Variable Details
typedef struct
{
	FMDATA8 cMaxScale;
	FMDATA8 cMinScale;
	unsigned char    bFormat;
	unsigned char    bExamine;
	unsigned char    bModPrior;
	unsigned char    bModifier;
	unsigned char    bEngrUnits;
	unsigned char    bChangeCode;
	unsigned char    bPVSource;
	unsigned char    bStyle;
	unsigned char    bQuality;
}VARDETAIL_42,*PVARDETAIL_42;

typedef struct
{
	FMDATA8 cMaxScale;
	FMDATA8 cMinScale;
	unsigned char    bFormat;
	unsigned char    bModifier;
	unsigned char    bEngrUnits;
	unsigned char    bChangeCode;
	unsigned char    bPVSource;
	unsigned char    bStyle;
	unsigned char    bQuality;
}VARDETAIL,*PVARDETAIL;

typedef struct
{
	wchar_t				szDataBaseName[18];
}	AMDATABASENAME,	*PAMDATABASENAME;

typedef struct
{
	unsigned char		bPointType;    // Associated Point Type             
	unsigned char		bConfig;       // Associated Configuration Status   
}TAGEXTEND,* PTAGEXTEND;

// Added for leak detection (IGO 06-Nov-2003)
typedef struct tagLEAKANALYSISRESULT
{
	double dLeakRate;
	double dCertRate;
	double dLeakThreshold;
	double dStdDev;
	double dMinValue;
	double dMaxValue;
	double dAverage;
	double dTempMinValue;
	double dTempMaxValue;
	double dTempDelta;
	unsigned short wAnalysisStatus;
	unsigned long dwNumSamples;
	long ReportTime,StartTime,EndTime;
	double dUsableSampleTime;
	double dGraphMinValue;
	double dGraphMaxValue;
	double dGraphTempDelta;
	double dLevelStart;
	double dLevelEnd;
	double dWaterLevelStart;
	double dWaterLevelEnd;
	unsigned short	wMinGaugeTestTime;
	unsigned short wGaugeType;
	wchar_t szLeakRecordID[50];
} LEAKANALYSISRESULT, *LPLEAKANALYSISRESULT;

#endif