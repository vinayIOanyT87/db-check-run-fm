#pragma once
// define the version this system will be compatible with
#define	FM_MAJORREV		7
#define	FM_MINORREV		6

#define FM_TankGauge_SIZE		26		// Length of Tank Gauge Data Strings (includes \0)
#define FM_TG_NUM_STRINGS

// structure definitions
typedef struct
{
	WORD	wMajorRev,
		wMinorRev,
		wCustomGaugeStart,
		wNumGaugeTypes;
}GAUGETYPEHEADER, * PGAUGETYPEHEADER;

typedef struct tagTANKGAUGETYPES
{
	WCHAR		szGaugeName[FM_TankGauge_SIZE + 1];
	BYTE		byGaugeType;									// 1 = Non-Cert, 2 = Cert
	DOUBLE	dThresholdRate;
	DOUBLE	dCertificationRate;
	DOUBLE	dDeltaTemp;
	short		nMinTestTime;
	BYTE		byIsCustom;
	int		nIndex;
} TANKGAUGETYPES, *LPTANKGAUGETYPES;

// function prototypes

bool getTankGaugeData(TCHAR* szgaugename,
							short* stype,
							DOUBLE* dthreshold,
							DOUBLE* dcertification,
							DOUBLE* ddeltatemp,
							short* smintime);

