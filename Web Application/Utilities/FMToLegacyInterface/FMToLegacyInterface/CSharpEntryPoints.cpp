// this file is where we will add the C# entry points.
// FM will call these and we will make the RPC calls, 
// reformat the data and then return it to the calling app
#include "pch.h"
#include <stdlib.h>
#include <link.h>
#include <DMLINK.H>
#include "utility.hpp"
#include "EngineeringUnits.h"
#include <winnt.h>
#include <WTypesbase.h>
#include "FMDefines.hpp"
#include "SystemFunctions.hpp"
#include <math.h>
#include "RPCFunc.hpp"
#include <Objbase.h>
#include <wchar.h> 
//#include <strsafe.h>

BOOL RunCalculator(BOOL	bUpDate,
	PSETCALC	pSetup,
	PDM_LOCK_CTXT_HNDL hDmLockCtxt,
	PTANKCALCULATE 		pTankCalcData,
	PCALCDATA 				pTankSetData);

BOOL	RunCalculatorEnteredData(PSETCALC pSetup,
	PDM_LOCK_CTXT_HNDL	hDmLockCtxt,
	PTANKCALCULATE 		pTankCalcData,
	PCALCDATA 			pTankSetData);


/////////////////////////////////////////////////////////////////////////////
/////////// Leak Detection Read Configuration Data
////////////////////////////////////////////////////////////////////////////
int GetLeakDetectionConfigurationData(TCHAR tankName[255], 
												TCHAR	*szgaugename, 
												TCHAR* szanalyismethod, 
												TCHAR* sztankvol, 
												TCHAR* szleakunits, 
												TCHAR* szvolunits, 
												TCHAR* sztempunits, 
												TCHAR* szquality,
												short* pstype,
												double* pdthreshold,
												double* pdcertification,
												double* pddeltatemp,
												short* psmintime)
{
	TCHAR	szengunits[256] = TEXT("");
	RPC_BINDING_HANDLE hRPCbindingHandle = NULL;

	*pdthreshold = 0;

	RPC_STATUS rpcstatus = FuelManagerBind(&hRPCbindingHandle,
		&dmlink_ClientIfHandle,
		FM_SYSTEM_DB,
		NULL);

	if (RPC_S_OK != rpcstatus)
	{
		return -1;
	}

	RpcTryExcept
	{
		DWORD dwStatus = ValidateDMStatus(hRPCbindingHandle);

		if (dwStatus != 2)	// 2 is in run mode
		{
			RpcBindingFree(&hRPCbindingHandle);
			return -1;
		}

		POINTSPEC pntspec;
		LEVELNAMES lvlNames;

		// parse the tank name and populate the levelnames structure
		if (!ConvertStringToLevelNames(&lvlNames, tankName))
		{
			RpcBindingFree(&hRPCbindingHandle);
			return -1;
		}

		if (0 > GetPointItems(hRPCbindingHandle,
			&lvlNames,
			&pntspec))
		{
			RpcBindingFree(&hRPCbindingHandle);
			return -1;
		}

		DWORD dwerror;
		BLOCKREQENTRY blockreq;
		
		double dtankvol = 0.0;
		int	nindex = 0, ngaugeindex = -1;

		HANDLE htankgauge = NULL;
		//SYSTEMTIME systimestart, systimeend;

		// Gauge Type
		blockreq.dwPointID = pntspec.dwPointID;
		blockreq.wPointIndex = LOWORD(pntspec.wPointIndex);
		blockreq.bPointType = FM_POINT_TANK;
		blockreq.bVariable = FMTANK_GAUGETYPE;
		blockreq.bUnits = FMU_Undefined;
		blockreq.bFormat = DSA_GAUGETYPE;
		blockreq.dwDataIndex = 0; 		// Index into data block

		dwerror = RPC_GetFormattedData(hRPCbindingHandle,
			&blockreq,
			szgaugename,
			szengunits,
			szquality);

		// Tank Analysis Method
		blockreq.bVariable = FMTANK_LEAK_ANALYSIS_METHOD;
		blockreq.bUnits = FMU_Undefined;
		blockreq.bFormat = DSA_WORD;
		blockreq.dwDataIndex = 0; 		// Index into data block


		dwerror = RPC_GetFormattedData(hRPCbindingHandle,
			&blockreq,
			szanalyismethod,
			szengunits,
			szquality);

		// Leak Rate Units
		blockreq.bVariable = FMTANK_LEAK_UNIT;
		blockreq.bUnits = FMU_Undefined;
		blockreq.bFormat = DSA_UNITS;
		blockreq.dwDataIndex = 0; 		// Index into data block

		dwerror = RPC_GetFormattedData(hRPCbindingHandle,
			&blockreq,
			szleakunits,
			szengunits,
			szquality);

		// Volume Units
		blockreq.bVariable = FMTANK_VOLUME_UNIT;
		blockreq.bUnits = FMU_Undefined;
		blockreq.bFormat = DSA_UNITS;
		blockreq.dwDataIndex = 0; 		// Index into data block

		dwerror = RPC_GetFormattedData(hRPCbindingHandle,
			&blockreq,
			szvolunits,
			szengunits,
			szquality);

		// Temperature Units
		blockreq.bVariable = FMTANK_TEMP_UNIT;
		blockreq.bUnits = FMU_Undefined;
		blockreq.bFormat = DSA_UNITS;
		blockreq.dwDataIndex = 0; 		// Index into data block

		dwerror = RPC_GetFormattedData(hRPCbindingHandle,
			&blockreq,
			sztempunits,
			szengunits,
			szquality);

		// Tank Volume (IGO 29-Aug-05)
		blockreq.bVariable = FMTANK_TANK_VOLUME;
		blockreq.bUnits = FMU_Undefined;
		blockreq.bFormat = DSA_DOUBLE;
		blockreq.dwDataIndex = 0; 		// Index into data block

		dwerror = RPC_GetRawData(hRPCbindingHandle,
			&blockreq,
			sztankvol,
			szengunits,
			szquality);
		dtankvol = wcstod(sztankvol, NULL);

		// get the gauge data from the project
		if (!getTankGaugeData(szgaugename,
										pstype,
										pdthreshold,
										pdcertification,
										pddeltatemp,
										psmintime))
		{
			RpcBindingFree(&hRPCbindingHandle);
			return -1;
		}
	}
	RpcExcept(1)
	{
		RpcBindingFree(&hRPCbindingHandle);
		return -1;
	}
	RpcEndExcept;

	RpcBindingFree(&hRPCbindingHandle);

	return 1;
}

/////////////////////////////////////////////////////////////////////////////
/////////// Leak Detection Delete Report Data from Database
////////////////////////////////////////////////////////////////////////////
int DeleteLeakDectionReportData(TCHAR * szLeakRecordID)
{
	RPC_BINDING_HANDLE hRPCbindingHandle = NULL;

	if (szLeakRecordID == NULL || szLeakRecordID == TEXT(""))
	{
		return -1;
	}

	RPC_STATUS rpcstatus = FuelManagerBind(&hRPCbindingHandle,
		&dmlink_ClientIfHandle,
		FM_SYSTEM_DB,
		NULL);

	if (RPC_S_OK != rpcstatus)
	{
		return -1;
	}

	RpcTryExcept
	{
		RPC_DeleteReportData(hRPCbindingHandle, szLeakRecordID);
	}
	RpcExcept(1)
	{
		// does not matter
	}
	RpcEndExcept;

	RpcBindingFree(&hRPCbindingHandle);

	return 1;
}

/////////////////////////////////////////////////////////////////////////////
/////////// Leak Detection Run Leak Analysis
////////////////////////////////////////////////////////////////////////////
int AnalyzeLeakDetectionData(TCHAR szTemp[255],
	short stype,
	short enanalysismethod,
	short entype,
	long ttstart,
	long ttend,
	double dthreshold,
	double dcertification,
	double ddeltatemp,
	double dtankvol,
	short smintime,
	double * RT_dLeakRate,
	double * RT_dMinValue,
	double * RT_dMaxValue ,
	long* RT_elapseTime,
	double* RT_dTempMinValue,
	double* RT_dTempMaxValue,
	double* RT_dGraphTempDelta,
	TCHAR * szLeakResult)
{
	RPC_BINDING_HANDLE hRPCbindingHandle = NULL;
	UINT uierror = LD_ERR_NONE;
	LEAKANALYSISRESULT leakresult;
	TCHAR szSystem[MAX_COMPUTERNAME_LENGTH + 10] = TEXT("");
	DWORD dwSize = 0;

	RPC_STATUS rpcstatus = FuelManagerBind(&hRPCbindingHandle,
		&dmlink_ClientIfHandle,
		FM_SYSTEM_DB,
		NULL);

	if (RPC_S_OK != rpcstatus)
	{
		return -1;
	}

	memset(&leakresult, 0, sizeof(LEAKANALYSISRESULT));

	leakresult.dCertRate = dcertification;
	leakresult.dLeakThreshold = dthreshold;
	leakresult.dMinValue = 0;
	leakresult.dMaxValue = dtankvol;
	leakresult.dTempMinValue = -25;
	leakresult.dTempMaxValue = 200;
	leakresult.dTempDelta = ddeltatemp;
	leakresult.wGaugeType = stype;
	leakresult.wMinGaugeTestTime = smintime * 60; // time in minutes

	leakresult.wAnalysisStatus = 0;

	dwSize = MAX_COMPUTERNAME_LENGTH + 3;

	lstrcpy(szSystem, TEXT("\\\\"));	// this forces the use of named pipes locally.
	GetComputerName(&szSystem[2], &dwSize);

	RpcTryExcept
	{
		uierror = RPC_CalculateLeakRate(hRPCbindingHandle,
			szSystem,
			szTemp,
			(SHORT)enanalysismethod,
			(SHORT)entype,
			(long)ttstart,
			(long)ttend,
			&leakresult);
	}
		RpcExcept(1)
	{
		RpcBindingFree(&hRPCbindingHandle);
		return -1;
	}
	RpcEndExcept;

	RpcBindingFree(&hRPCbindingHandle);

	if (uierror == LD_ERR_NONE)
	{
		szLeakResult[0] = 0x00;
		time_t ttelapsed, ttmintime = smintime * 3600;
		ttelapsed = leakresult.EndTime - leakresult.StartTime;
		*RT_dLeakRate = leakresult.dLeakRate;
		*RT_dMinValue = leakresult.dMinValue;
		*RT_dMaxValue = leakresult.dMaxValue;
		*RT_elapseTime = leakresult.EndTime - leakresult.StartTime;
		*RT_dTempMinValue = leakresult.dTempMinValue;
		*RT_dTempMaxValue = leakresult.dTempMaxValue;
		*RT_dGraphTempDelta = leakresult.dGraphTempDelta;

		if (ttmintime > ttelapsed)
		{
			lstrcpy(szLeakResult, TEXT("Test period to short for the specified gauge."));
		}
		if (fabs(leakresult.dLeakRate) >= fabs(dthreshold))
		{
			lstrcpy(szLeakResult, TEXT("Leak Rate is greater than the Certified Leak Rate."));
		}
		if (fabs(leakresult.dLeakRate) >= fabs(dcertification))
		{
			lstrcpy(szLeakResult, TEXT("Leak Rate is greater than the Certified Leak Rate."));
		}
		
		if (LD_ERR_OVER_DELTA_TEMP & leakresult.wAnalysisStatus)
		{
			lstrcpy(szLeakResult, TEXT("Temperature change is greater than the allowable temperature change."));
		}
		if (LD_ERR_LVL_CONSTANT & leakresult.wAnalysisStatus)
		{
			lstrcpy(szLeakResult, TEXT("Leak Rate Remained Constant."));
		}
		// Non-certified gauges are always not applicable for status (IGO 06-Apr-2004)
		if (1 == leakresult.wGaugeType &&
			0 == leakresult.wMinGaugeTestTime &&
			0 == leakresult.dCertRate &&
			0 == leakresult.dLeakThreshold)
		{
			lstrcpy(szLeakResult, TEXT("Leak Test result is N/A for non-certified gauges."));
		}
		else
		{
			if (LD_ERR_TEST_FAILED & leakresult.wAnalysisStatus)
			{
				lstrcpy(szLeakResult, TEXT("Leak Test Failed."));
			}
			else
			{
				lstrcpy(szLeakResult, TEXT("Leak Test Passed."));
			}
		}

		return 1;
	}
	return -1;
}

/////////////////////////////////////////////////////////////////////////////
/////////// Tank Calculator create calculator
////////////////////////////////////////////////////////////////////////////
int CreateRunDestroyTankCalculator(TCHAR szTankname[255],
	TCHAR szUserName[255],
	double dLevel,
	double dTemperature,
	double dDensity,
	double dStdDensity,
	double dDensityTemp,
	double dAmbientTemp,
	double dWaterLevel,
	double* RT_dStrapVolume,
	double* RT_dWaterVolume,
	double* RT_dGrossVolume,
	double* RT_dDensity,
	double* RT_dStdDensity,
	double* RT_dVCF,
	double* RT_dCTSh,
	double* RT_dNetVolume,
	double* RT_dMass)
{
	int returnInt = 1;
	RPC_BINDING_HANDLE	hRPCbindingHandle = NULL;
	PDM_LOCK_CTXT_HNDL	hDmLockCtxt;
	LEVELNAMES				lvlNames;
	TCHAR						szSystem[MAX_COMPUTERNAME_LENGTH + 10] = TEXT("");
	DWORD						dwSize = 0, dwError = 0;
	SETCALC					setupCalc;
	CALCDATA 				tankSetData;
	TANKCALCULATE			tankCalcData;

	dwSize = MAX_COMPUTERNAME_LENGTH + 3;

	lstrcpy(szSystem, TEXT("\\\\"));
	GetComputerName(&szSystem[2], &dwSize);


	RPC_STATUS rpcstatus = FuelManagerBind(&hRPCbindingHandle,
		&dmlink_ClientIfHandle,
		FM_SYSTEM_DB,
		NULL);

	if (RPC_S_OK != rpcstatus)
	{
		return -1;
	}

	// parse the tank name and populate the levelnames structure
	if (!ConvertStringToLevelNames(&lvlNames, szTankname))
	{
		RpcBindingFree(&hRPCbindingHandle);
		return -1;
	}

	tankSetData.dwXfrBaseField = NO_SPECIAL_PROCESSING;

	RpcTryExcept
	{
		dwError = RPC_AllocateTankCalc(hRPCbindingHandle,
													&hDmLockCtxt,
													szUserName,
													szSystem,
													&lvlNames,
													&tankSetData.dwCalcID,
													&setupCalc);
	}
	RpcExcept(1)
	{
		RpcBindingFree(&hRPCbindingHandle);
		return - 1;
	}
	RpcEndExcept;

		if (!RunCalculator(FALSE, &setupCalc, hDmLockCtxt, &tankCalcData, &tankSetData))
	{
		RpcBindingFree(&hRPCbindingHandle);
		return - 1;
	}

	// run the pased in data and get the values

	tankCalcData.dLevel = dLevel;
	tankCalcData.dTemp = dTemperature;
	tankCalcData.dProdDensity = dDensity;
	tankCalcData.dStdDensity = dStdDensity;
	tankCalcData.dDensityTempValue = dDensityTemp;
	tankCalcData.dAmbientTemp = dAmbientTemp;
	tankCalcData.dH2OLevel = dWaterLevel;

	if (!RunCalculatorEnteredData(&setupCalc,
		hDmLockCtxt,
		&tankCalcData,
		&tankSetData))
	{
		returnInt = -1;
	}
	else
	{
		// set the returned data
		*RT_dStrapVolume = tankCalcData.dStrapVolume;
		*RT_dWaterVolume = tankCalcData.dH2OVolume;
		*RT_dGrossVolume = tankCalcData.dGrossVolume;
		*RT_dDensity = tankCalcData.dProdDensity;
		*RT_dStdDensity = tankCalcData.dStdDensity;
		*RT_dVCF = tankCalcData.dVolumeCorrect;
		*RT_dCTSh = tankCalcData.dShellCorrection;
		*RT_dNetVolume = tankCalcData.dNetVolume;
		*RT_dMass = tankCalcData.dProdMass;
	}


	// destroy the tank created for the calculator
	RpcTryExcept
	{
		RPC_QuitTankCalc(&hDmLockCtxt,
							  szUserName,
							  szSystem,
							  tankSetData.dwCalcID);
	}
		RpcExcept(1)
	{
		RpcBindingFree(&hRPCbindingHandle);
		return -1;
	}
	RpcEndExcept;

	return returnInt;
}

/////////////////////////////////////////////////////////////////////////////
/////////// Run Tank Calculator
////////////////////////////////////////////////////////////////////////////
BOOL RunCalculator(BOOL	bUpDate,
	PSETCALC	pSetup,
	PDM_LOCK_CTXT_HNDL hDmLockCtxt,
	PTANKCALCULATE 		pTankCalcData,
	PCALCDATA 				pTankSetData)
{
	DWORD						dwError =  FM_ERROR_NONE;

	RpcTryExcept
	{
		dwError = RPC_DoTankCalculate(hDmLockCtxt,
												pTankSetData->dTestLevel,
												pTankSetData->dTestTemp,
												pTankSetData->dTestDens,
												pTankSetData->dTestH2O,
												pTankSetData->dTestBSW,
												pTankSetData->dTestSolid,
												pTankSetData->dTestAmbientTemp,
												pTankSetData->dTestVaporTemp,
												pTankSetData->dTestVaporPress,
												pTankSetData->dTestCorrectionVolume,
												pTankSetData->dTestGasDensity,
												pTankSetData->dXfrValue,
												pTankSetData->dTestDensityTemp,
												pTankSetData->dGaugeStdDensity,
												pTankSetData->dwCalcID,
												pTankSetData->dwXfrBaseField,
												(unsigned char)bUpDate,
												pSetup->bUseMeasuredDensity,
												pTankCalcData);

	}
		RpcExcept(1)
	{
		return(FALSE);
	}
	RpcEndExcept;

	if (dwError != FM_ERROR_NONE)
		return(FALSE);

	return(TRUE);
}

/////////////////////////////////////////////////////////////////////////////
/////////// Run Tank Calculator entered data
////////////////////////////////////////////////////////////////////////////
BOOL	RunCalculatorEnteredData(PSETCALC pSetup,
	PDM_LOCK_CTXT_HNDL	hDmLockCtxt,
	PTANKCALCULATE 		pTankCalcData,
	PCALCDATA 			pTankSetData)
{
	pTankSetData->dTestLevel = pTankCalcData->dLevel;
	pTankSetData->dTestTemp = pTankCalcData->dTemp;
	if (pSetup->bUseMeasuredDensity)
	{
		pTankSetData->dTestDens = pTankCalcData->dProdDensity;
	}
	else
	{
		pTankSetData->dTestDens = pTankCalcData->dStdDensity;
	}

	pTankSetData->dTestH2O = pTankCalcData->dH2OLevel;
	pTankSetData->dTestBSW = pTankCalcData->dBSW;
	pTankSetData->dTestSolid = pTankCalcData->dSolidLevel;
	pTankSetData->dTestAmbientTemp = pTankCalcData->dAmbientTemp;
	pTankSetData->dTestVaporTemp = pTankCalcData->dVaporTemp;
	pTankSetData->dTestVaporPress = pTankCalcData->dVaporPress;

	pTankSetData->dTestCorrectionVolume = pTankCalcData->dCorrectionVolume;
	pTankSetData->dTestGasDensity = pTankCalcData->dGasDensity;
	pTankSetData->dTestDensityTemp = pTankCalcData->dDensityTempValue;

	if (!RunCalculator(TRUE,
		pSetup,
		hDmLockCtxt,
		pTankCalcData,
		pTankSetData))
		return(FALSE);

	return(TRUE);
}


/////////////////////////////////////////////////////////////////////////////
/////////// Get Handgauge start data
////////////////////////////////////////////////////////////////////////////
int GetHandGaugeDataStartEnd(int dwMoveID,
	int wNodeID,
	int getStartData,
	double* RT_dLevel,
	long* RT_LevelTime,
	double* RT_dTemperature,
	long* RT_TemperatureTime,
	double* RT_dDensity,
	long* RT_DensityTime,
	double* RT_dDensityTemp,
	long* RT_DensityTempTime,
	double* RT_dAmbientTemp,
	long* RT_AmbientTempTime,
	double* RT_dRefHeight,
	long* RT_RefHeightTime,
	double* RT_dWaterLevel,
	long* RT_WaterLevelTime,
	double* RT_dStrapVolume,
	double* RT_dWaterVolume,
	double* RT_dGrossVolume,
	double* RT_dStdDensity,
	double* RT_dVCF,
	double* RT_dCTSh,
	double* RT_dNetVolume,
	double* RT_dMass,
	double* RT_dRoofMass,
	TCHAR * szEmployeeID)
{
	int iReturn = 1;
	RPC_BINDING_HANDLE	hMvmntBinding = NULL;
	TCHAR						szSystem[MAX_COMPUTERNAME_LENGTH + 10] = TEXT("");
	DWORD						dwSize = 0, dwError = FM_ERROR_NONE;
	PSTARTDATAGET			pHGDataGet = NULL;

	dwSize = MAX_COMPUTERNAME_LENGTH + 3;

	lstrcpy(szSystem, TEXT("\\\\"));
	GetComputerName(&szSystem[2], &dwSize);

	if (!MvmntBind(&hMvmntBinding, szSystem))
	{
		return - 1;
	}

	PHANDGAUGEDATA pHandGaugeData = NULL;

	pHandGaugeData = new HANDGAUGEDATA;

	if (!pHandGaugeData)
	{
		RpcBindingFree(&hMvmntBinding);
		return -1;
	}

	RpcTryExcept;
	{
		dwError = MvmntGetHandGaugeData(hMvmntBinding,
			dwMoveID,
			wNodeID,
			getStartData,
			pHandGaugeData);
	}
	RpcExcept(1)
	{
		delete(pHandGaugeData);
		pHandGaugeData = NULL;
		RpcBindingFree(&hMvmntBinding);
		return -1;
	}
	RpcEndExcept;

	if (dwError != FM_ERROR_NONE)
	{
		delete(pHandGaugeData);
		pHandGaugeData = NULL;
		RpcBindingFree(&hMvmntBinding);
		return -1;
	}

	// copy the values into the data above to be returned
	*RT_dLevel = pHandGaugeData->dLevel;
	*RT_LevelTime = pHandGaugeData->LevelTime;
	*RT_dTemperature = pHandGaugeData->dTemperature;
	*RT_TemperatureTime = pHandGaugeData->TemperatureTime;
	*RT_dDensity = pHandGaugeData->dDensity;
	*RT_DensityTime = pHandGaugeData->DensityTime;
	*RT_dDensityTemp = pHandGaugeData->dDensityTemp;
	*RT_DensityTempTime = pHandGaugeData->DensityTempTime;
	*RT_dAmbientTemp = pHandGaugeData->dAmbientTemp;
	*RT_AmbientTempTime = pHandGaugeData->AmbientTempTime;
	*RT_dRefHeight = pHandGaugeData->dRefHeight;
	*RT_RefHeightTime = pHandGaugeData->RefHeightTime;
	*RT_dWaterLevel = pHandGaugeData->dWaterLevel;
	*RT_WaterLevelTime = pHandGaugeData->WaterLevelTime;
	*RT_dStrapVolume = pHandGaugeData->dStrapVolume;
	*RT_dWaterVolume = pHandGaugeData->dWaterVolume;
	*RT_dGrossVolume = pHandGaugeData->dGrossVolume;
	*RT_dStdDensity = pHandGaugeData->dStdDensity;
	*RT_dVCF = pHandGaugeData->dVCF;
	*RT_dCTSh = pHandGaugeData->dCTSh;
	*RT_dNetVolume = pHandGaugeData->dNetVolume;
	*RT_dMass = pHandGaugeData->dMass;
	*RT_dRoofMass = pHandGaugeData->dRoofMass;
	lstrcpy(szEmployeeID, pHandGaugeData->szEmployeeID);


	delete(pHandGaugeData);
	pHandGaugeData = NULL;
	RpcBindingFree(&hMvmntBinding);
	
	return iReturn;
}

int PutHandGaugeDataStartEnd(int dwMoveID,
	int wNodeID,
	int putStartData,
	double RT_dLevel,
	long RT_LevelTime,
	double RT_dTemperature,
	long RT_TemperatureTime,
	double RT_dDensity,
	long RT_DensityTime,
	double RT_dDensityTemp,
	long RT_DensityTempTime,
	double RT_dAmbientTemp,
	long RT_AmbientTempTime,
	double RT_dRefHeight,
	long RT_RefHeightTime,
	double RT_dWaterLevel,
	long RT_WaterLevelTime,
	double RT_dStrapVolume,
	double RT_dWaterVolume,
	double RT_dGrossVolume,
	double RT_dStdDensity,
	double RT_dVCF,
	double RT_dCTSh,
	double RT_dNetVolume,
	double RT_dMass,
	double RT_dRoofMass,
	TCHAR szEmployeeID[50])
{
	int iReturn = 1;
	RPC_BINDING_HANDLE	hMvmntBinding = NULL;
	TCHAR						szSystem[MAX_COMPUTERNAME_LENGTH + 10] = TEXT("");
	DWORD						dwSize = 0, dwError = FM_ERROR_NONE;
	PSTARTDATAGET			pHGDataGet = NULL;

	dwSize = MAX_COMPUTERNAME_LENGTH + 3;

	lstrcpy(szSystem, TEXT("\\\\"));
	GetComputerName(&szSystem[2], &dwSize);

	if (!MvmntBind(&hMvmntBinding, szSystem))
	{
		return -1;
	}

	PHANDGAUGEDATA pHandGaugeData = NULL;

	pHandGaugeData = new HANDGAUGEDATA;

	if (!pHandGaugeData)
	{
		RpcBindingFree(&hMvmntBinding);
		return -1;
	}

	pHandGaugeData->dLevel = RT_dLevel;
	pHandGaugeData->LevelTime = RT_LevelTime;
	pHandGaugeData->dTemperature = RT_dTemperature;
	pHandGaugeData->TemperatureTime = RT_TemperatureTime;
	pHandGaugeData->dDensity = RT_dDensity;
	pHandGaugeData->DensityTime = RT_DensityTime;
	pHandGaugeData->dDensityTemp = RT_dDensityTemp;
	pHandGaugeData->DensityTempTime = RT_DensityTempTime;
	pHandGaugeData->dAmbientTemp = RT_dAmbientTemp;
	pHandGaugeData->AmbientTempTime = RT_AmbientTempTime;
	pHandGaugeData->dRefHeight = RT_dRefHeight;
	pHandGaugeData->RefHeightTime = RT_RefHeightTime;
	pHandGaugeData->dWaterLevel = RT_dWaterLevel;
	pHandGaugeData->WaterLevelTime = RT_WaterLevelTime;
	pHandGaugeData->dStrapVolume = RT_dStrapVolume;
	pHandGaugeData->dWaterVolume = RT_dWaterVolume;
	pHandGaugeData->dGrossVolume = RT_dGrossVolume;
	pHandGaugeData->dStdDensity = RT_dStdDensity;
	pHandGaugeData->dVCF = RT_dVCF;
	pHandGaugeData->dCTSh = RT_dCTSh;
	pHandGaugeData->dNetVolume = RT_dNetVolume;
	pHandGaugeData->dMass = RT_dMass;
	pHandGaugeData->dRoofMass = RT_dRoofMass;
	lstrcpy(pHandGaugeData->szEmployeeID,szEmployeeID);

	RpcTryExcept;
	{
		dwError = MvmntPutHandGaugeData(hMvmntBinding,
			dwMoveID,
			wNodeID,
			putStartData,
			pHandGaugeData);
	}
	RpcExcept(1)
	{
		delete(pHandGaugeData);
		pHandGaugeData = NULL;
		RpcBindingFree(&hMvmntBinding);
		return -1;
	}
	RpcEndExcept;

	if (dwError != FM_ERROR_NONE)
	{
		delete(pHandGaugeData);
		pHandGaugeData = NULL;
		RpcBindingFree(&hMvmntBinding);
		return -1;
	}


	return iReturn;
}

BOOL ExecuteMvmntCmd(DWORD dwMoveInstID, WORD wMoveNodeID, WORD wCommand)
{
	RPC_BINDING_HANDLE	hMvmntBinding;
	DWORD						dwSize, dwError;
	WCHAR						szSystemName[MAX_COMPUTERNAME_LENGTH + 3];
	TCHAR						szComputerName[MAX_COMPUTERNAME_LENGTH + 3];
	WCHAR						szUserName[MAX_USERNAME_LENGTH + 1];
	long						lTimePointer = 0;

	dwSize = MAX_COMPUTERNAME_LENGTH + 3;
	lstrcpy(szSystemName, TEXT("\\\\"));	
	if (!GetComputerName(szComputerName, &dwSize))
		return(FALSE);

	lstrcat(szSystemName, szComputerName);	// this forces the use of named pipes locally.

	dwSize = MAX_USERNAME_LENGTH + 1;
	if (!GetUserName(szUserName, &dwSize))
		return(FALSE);

	if (!MvmntBind(&hMvmntBinding, szSystemName))
		return(FALSE);

	RpcTryExcept
	{
		dwError = MvmntMoveCommand(hMvmntBinding,
											szSystemName,
											szUserName,
											dwMoveInstID,
											wMoveNodeID,
											wCommand,
											&lTimePointer);
	}
	RpcExcept(1)
	{
		RPC_STATUS	RpcStatus;

		RpcStatus = RpcExceptionCode();
		RpcBindingFree(&hMvmntBinding);
		return(FALSE);
	}
	RpcEndExcept;

	RpcBindingFree(&hMvmntBinding);

	if (dwError != FM_ERROR_NONE
		&& dwError != FM_ERROR_CANCEL)
	{
		return(FALSE);
	}
	return(TRUE);
}

BOOL GetGroups(DWORD *dwNumGroups, MYMOVEMENTGROUP** ppGroup)
{
	RPC_BINDING_HANDLE	hMvmntBinding;
	DWORD				dwSize, dwError, dwCount;
	TCHAR				szComputerName[MAX_COMPUTERNAME_LENGTH + 3];
	WCHAR				szSystemName[MAX_COMPUTERNAME_LENGTH + 3];
	WCHAR				szUserName[MAX_USERNAME_LENGTH + 1];
	PMOVEMENTGROUP		pMovementGroup = 0;

	dwSize = MAX_COMPUTERNAME_LENGTH + 3;
	lstrcpy(szSystemName, TEXT("\\\\"));	
	if (!GetComputerName(szComputerName, &dwSize))
		return(FALSE);

	lstrcat(szSystemName, szComputerName);	// this forces the use of named pipes locally.

	dwSize = MAX_USERNAME_LENGTH + 1;
	if (!GetUserName(szUserName, &dwSize))
		return(FALSE);

	if (!MvmntBind(&hMvmntBinding, szSystemName))
		return(FALSE);

	RpcTryExcept
	{
		dwError = MvmntGetGroups(hMvmntBinding, &dwCount, &pMovementGroup);
	}
	RpcExcept(1)
	{
		RPC_STATUS	RpcStatus;

		RpcStatus = RpcExceptionCode();
		RpcBindingFree(&hMvmntBinding);
		return(FALSE);
	}
	RpcEndExcept;

	RpcBindingFree(&hMvmntBinding);

	if (dwError != FM_ERROR_NONE
		&& dwError != FM_ERROR_CANCEL)
	{
		return(FALSE);
	}

	// Free any received memory
	CoTaskMemFree(*ppGroup);
	*ppGroup = NULL;

	*dwNumGroups = dwCount;
	*ppGroup = (MYMOVEMENTGROUP*)CoTaskMemAlloc(dwCount * sizeof(MYMOVEMENTGROUP));
	MYMOVEMENTGROUP* pCurrent = *ppGroup;
	wchar_t* buffer;
	if (pMovementGroup)
	{
		for (DWORD i = 0; i < dwCount; i++, pCurrent++)
		{
			buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 21);
			if (buffer)
			{
				::ZeroMemory(buffer, sizeof(wchar_t) * 21);
				wcsncpy_s(buffer, 21, pMovementGroup[i].szName, 21);
				if (pCurrent)
				{
					pCurrent->szName = buffer;
				}
			}
		}
	}

	free(pMovementGroup);
	pMovementGroup = 0;

	return(TRUE);
}

///////////////////////////////////////////////////////////////////////////////////////
//	Method:			GetServerPrinters()
//
//	Parameters:		pPrinterList - The CStringList containing the printer names.
//
//	Description:	Makes an RPC call to get the list of printers available on the server.
//
//	Returns:			TRUE/FALSE 
//
///////////////////////////////////////////////////////////////////////////////////////
BOOL GetPrinters(DWORD* dwNumPrinters, PRINTERDATA** ppPrinter)
{
	PPRINTERDATA				pPrinters = NULL;
	DWORD						dwCount = 0;
	BOOL						bReturn = TRUE;
	RPC_BINDING_HANDLE	hMvmntBinding = NULL;
	DWORD						dwError, dwSize;
	TCHAR						szComputerName[MAX_COMPUTERNAME_LENGTH + 3];
	TCHAR						szSystemName[MAX_COMPUTERNAME_LENGTH + 3];

	dwSize = MAX_COMPUTERNAME_LENGTH + 3;
	lstrcpy(szSystemName, TEXT("\\\\"));	
	if (!GetComputerName(szComputerName, &dwSize))
		return(FALSE);

	lstrcat(szSystemName, szComputerName);	// this forces the use of named pipes locally.

	if (!MvmntBind(&hMvmntBinding, szSystemName))
		return FALSE;

	RpcTryExcept
	{
		dwError = MvmntGetServerPrinterNames(hMvmntBinding, &dwCount, &pPrinters);
	}
	RpcExcept(1)
	{
		RPC_STATUS	RpcStatus;

		RpcStatus = RpcExceptionCode();
		RpcBindingFree(&hMvmntBinding);
		return FALSE;
	}
	RpcEndExcept;

	RpcBindingFree(&hMvmntBinding);

	if (FM_ERROR_UNAVAIL == dwError)
	{
		bReturn = FALSE;
	}
	else if (dwError != FM_ERROR_NONE)
	{
		return FALSE;
	}
	else
	{
		// Free any received memory
		CoTaskMemFree(*ppPrinter);
		*ppPrinter = NULL;

		*dwNumPrinters = dwCount;

		*ppPrinter = (PRINTERDATA*)CoTaskMemAlloc(dwCount * sizeof(PRINTERDATA));
		PRINTERDATA* pCurrent = *ppPrinter;
		wchar_t* buffer;

		if (pPrinters)
		{
			for (DWORD i = 0; i < dwCount; i++, pCurrent++)
			{
				buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * pPrinters[i].byLen);
				if (buffer)
				{
					::ZeroMemory(buffer, sizeof(wchar_t) * pPrinters[i].byLen);
					pPrinters[i].szName[pPrinters[i].byLen - 1] = '\0';
					lstrcpy(buffer, pPrinters[i].szName);
					if (pCurrent)
					{
						pCurrent->szName = buffer;
						pCurrent->byLen = pPrinters[i].byLen;
						pCurrent->byDefault = pPrinters[i].byDefault;
					}
				}
			}
		}
		bReturn = TRUE;
	}

	delete[] pPrinters;
	pPrinters = 0;

	return bReturn;

}

BOOL GetDeliveryTickets(DWORD* dwNumReports, MYDELIVERYTICKETNAME** ppReports)
{
	DELIVERYTICKETNAME* pReports = NULL;
	DWORD						dwCount = 0;
	BOOL						bReturn = TRUE;

	RPC_BINDING_HANDLE	hMvmntBinding = NULL;
	DWORD						dwError, dwSize;
	TCHAR						szComputerName[MAX_COMPUTERNAME_LENGTH + 3];
	TCHAR						szSystemName[MAX_COMPUTERNAME_LENGTH + 3];

	dwSize = MAX_COMPUTERNAME_LENGTH + 3;
	lstrcpy(szSystemName, TEXT("\\\\"));
	if (!GetComputerName(szComputerName, &dwSize))
		return(FALSE);

	lstrcat(szSystemName, szComputerName);	// this forces the use of named pipes locally.

	if (!MvmntBind(&hMvmntBinding, szSystemName))
		return FALSE;

	RpcTryExcept
	{
		dwError = MvmntGetServerDeliveryTicketNames(hMvmntBinding,&dwCount,&pReports);
	}
		RpcExcept(1)
	{
		RPC_STATUS	RpcStatus;
		RpcStatus = RpcExceptionCode();
		RpcBindingFree(&hMvmntBinding);
		return FALSE;
	}
	RpcEndExcept;

	RpcBindingFree(&hMvmntBinding);

	if (dwError != FM_ERROR_NONE)
	{
		return FALSE;
	}

	// Free any received memory
	CoTaskMemFree(*ppReports);
	*ppReports = NULL;

	*dwNumReports = dwCount;

	*ppReports = (MYDELIVERYTICKETNAME*)CoTaskMemAlloc(dwCount * sizeof(MYDELIVERYTICKETNAME));
	MYDELIVERYTICKETNAME* pCurrent = *ppReports;
	wchar_t* buffer;

	if (pReports)
	{
		for (DWORD i = 0; i < dwCount; i++, pCurrent++)
		{
			buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 261);
			if (buffer)
			{
				::ZeroMemory(buffer, sizeof(wchar_t) * 261);
				wcsncpy_s(buffer, 261, pReports[i].szReportName, 261);
				if (pCurrent)
				{
					pCurrent->szName = buffer;
				}
			}
		}
	}
	bReturn = TRUE;

	delete[] pReports;
	pReports = 0;


	return bReturn;

} 

BOOL GetMovementInstance(DWORD dwMoveID, MYMOVEINSTANCEDATA** ppMoveInstanceData)
{
	MOVEINSTANCEDATA* pMovementData = NULL;
	RPC_BINDING_HANDLE	hMvmntBinding = NULL;
	DWORD dwError, dwSize;
	TCHAR szComputerName[MAX_COMPUTERNAME_LENGTH + 3];
	TCHAR szSystemName[MAX_COMPUTERNAME_LENGTH + 3];

	dwSize = MAX_COMPUTERNAME_LENGTH + 3;
	lstrcpy(szSystemName, TEXT("\\\\"));
	if (!GetComputerName(szComputerName, &dwSize))
		return(FALSE);

	lstrcat(szSystemName, szComputerName);	// this forces the use of named pipes locally.

	if (!MvmntBind(&hMvmntBinding, szSystemName))
		return FALSE;

	RpcTryExcept;
	{
		dwError = MvmntGetMoveInst(hMvmntBinding, dwMoveID, &pMovementData);
	}
	RpcExcept(1)										// 1 = Always evaluate expression
	{
		RPC_STATUS	RpcStatus;
		RpcBindingFree(&hMvmntBinding);
		RpcStatus = RpcExceptionCode();
		return(FALSE);
	}
	RpcEndExcept;
	RpcBindingFree(&hMvmntBinding);

	// Free any received memory
	CoTaskMemFree(*ppMoveInstanceData);
	*ppMoveInstanceData = NULL;

	if (dwError != FM_ERROR_NONE)
	{
		return(FALSE);
	}

	if (pMovementData)
	{
		*ppMoveInstanceData = (MYMOVEINSTANCEDATA*)CoTaskMemAlloc(sizeof(MYMOVEINSTANCEDATA));
		MYMOVEINSTANCEDATA* pCurrent = *ppMoveInstanceData;
		wchar_t* buffer;

		if (pCurrent)
		{
			buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 21);
			if (buffer)
			{
				::ZeroMemory(buffer, sizeof(wchar_t) * 21);
				lstrcpy(buffer, pMovementData->szName);
				pCurrent->szName = buffer;
			}

			buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 21);
			if (buffer)
			{
				::ZeroMemory(buffer, sizeof(wchar_t) * 21);
				lstrcpy(buffer, pMovementData->szOrder);
				pCurrent->szOrder = buffer;
			}

			buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 201);
			if (buffer)
			{
				::ZeroMemory(buffer, sizeof(wchar_t) * 201);
				lstrcpy(buffer, pMovementData->szComment);
				pCurrent->szComment = buffer;
			}

			pCurrent->szUserDef = (wchar_t **)CoTaskMemAlloc(sizeof(wchar_t *) * 10);
			for (int i = 0; i < 10; i++)
			{
				buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 31);
				if (buffer)
				{
					::ZeroMemory(buffer, sizeof(wchar_t) * 31);
					wmemcpy(buffer, &pMovementData->szUserDef[i][0], 31);
					(pCurrent->szUserDef)[i] = buffer;
				}
			}

			buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 21);
			if (buffer)
			{
				::ZeroMemory(buffer, sizeof(wchar_t) * 21);
				lstrcpy(buffer, pMovementData->szGroup);
				pCurrent->szGroup = buffer;
			}

			buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 81);
			if (buffer)
			{
				::ZeroMemory(buffer, sizeof(wchar_t) * 81);
				lstrcpy(buffer, pMovementData->szReportName);
				pCurrent->szReportName = buffer;
			}

			buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 81);
			if (buffer)
			{
				::ZeroMemory(buffer, sizeof(wchar_t) * 81);
				lstrcpy(buffer, pMovementData->szPrinterName);
				pCurrent->szPrinterName = buffer;
			}

			buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 81);
			if (buffer)
			{
				::ZeroMemory(buffer, sizeof(wchar_t) * 81);
				lstrcpy(buffer, pMovementData->szInputPoint);
				pCurrent->szInputPoint = buffer;
			}

			pCurrent->lPlannedStartTime = pMovementData->lPlannedStartTime;
			pCurrent->bPlannedStartTimeOperational = pMovementData->bPlannedStartTimeOperational;
			pCurrent->wPlannedStartTimeStatus = pMovementData->wPlannedStartTimeStatus;
			pCurrent->lAutoStartTime = pMovementData->lAutoStartTime;
			pCurrent->bAutoStartTimeActive = pMovementData->bAutoStartTimeActive;
			pCurrent->wAutoStartTimeStatus = pMovementData->wAutoStartTimeStatus;
			pCurrent->lAutoStopTime = pMovementData->lAutoStopTime;
			pCurrent->bAutoStopTimeActive = pMovementData->bAutoStopTimeActive;
			pCurrent->wAutoStopTimeStatus = pMovementData->wAutoStopTimeStatus;
			pCurrent->wZeroFlowHoldOffMinutes = pMovementData->wZeroFlowHoldOffMinutes;
			pCurrent->bType = pMovementData->bType;
			pCurrent->bCommit = pMovementData->bCommit;
			pCurrent->bOkPerm = pMovementData->bOkPerm;
			pCurrent->bOrderPerm = pMovementData->bOrderPerm;
			pCurrent->bSourceSetpointsInPercentPerm = pMovementData->bSourceSetpointsInPercentPerm;
			pCurrent->bSourceSetpointsInPercent = pMovementData->bSourceSetpointsInPercent;
			pCurrent->bSourceSetpointsInPercentOld = pMovementData->bSourceSetpointsInPercentOld;
			pCurrent->bAutoDelete = pMovementData->bAutoDelete;
			pCurrent->bStartOnNonZeroFlow = pMovementData->bStartOnNonZeroFlow;
			pCurrent->bStopOnZeroFlow = pMovementData->bStopOnZeroFlow;
			pCurrent->bInterlockSetpoints = pMovementData->bInterlockSetpoints;
			pCurrent->bIncludeHandValues = pMovementData->bIncludeHandValues;
			pCurrent->bLineupActionSequence = pMovementData->bLineupActionSequence;
			pCurrent->bLineupActionSequencePerm = pMovementData->bLineupActionSequencePerm;
			pCurrent->bHaltOnCompletion = pMovementData->bHaltOnCompletion;
			pCurrent->bInhibitSetpointOverrange = pMovementData->bInhibitSetpointOverrange;
			pCurrent->bInhibitMovementType = pMovementData->bInhibitMovementType;
			pCurrent->bIndividualNodeControl = pMovementData->bIndividualNodeControl;
			pCurrent->bUsePendingOperation = pMovementData->bUsePendingOperation;
			pCurrent->bUseInputPoint = pMovementData->bUseInputPoint;
			pCurrent->bSendMvmntToSnapIn = pMovementData->bSendMvmntToSnapIn;
			pCurrent->bMvmntToSnapInAvailable = pMovementData->bMvmntToSnapInAvailable;
			pCurrent->dwInitiationCount = pMovementData->dwInitiationCount;
			pCurrent->tInitiationTime = pMovementData->tInitiationTime;
			pCurrent->wNumberOfNodes = pMovementData->wNumberOfNodes;


			MYNODEINSTANCEDATA* ppNodeData = (MYNODEINSTANCEDATA*)CoTaskMemAlloc(sizeof(MYNODEINSTANCEDATA) * pMovementData->wNumberOfNodes);

			pCurrent->pNodeInstanceData = ppNodeData;

			if (ppNodeData)
			{
				for (int i = 0, j = 1; i < pMovementData->wNumberOfNodes; i++, j++)
				{
					MYNODEINSTANCEDATA* pCurrentNode = &pCurrent->pNodeInstanceData[i];
					NODEINSTANCEDATA* pReceivedNodeData = &pMovementData->pNodeInstanceData[i];

					buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 37);
					if (buffer)
					{
						::ZeroMemory(buffer, sizeof(wchar_t) * 37);
						lstrcpy(buffer, pReceivedNodeData->szName);
						pCurrentNode->szName = buffer;
					}

					buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 37);
					if (buffer)
					{
						::ZeroMemory(buffer, sizeof(wchar_t) * 37);
						lstrcpy(buffer, pReceivedNodeData->szNameOld);
						pCurrentNode->szNameOld = buffer;
					}

					pCurrentNode->wNodeID = pReceivedNodeData->wNodeID;
					pCurrentNode->bType = pReceivedNodeData->bType;
					pCurrentNode->bSource = pReceivedNodeData->bSource;
					pCurrentNode->bSetNamePerm = pReceivedNodeData->bSetNamePerm;
					pCurrentNode->bSetXfrModePerm = pReceivedNodeData->bSetXfrModePerm;
					pCurrentNode->bSetXfrModeInactivePerm = pReceivedNodeData->bSetXfrModeInactivePerm;
					pCurrentNode->bSetSetpointPerm = pReceivedNodeData->bSetSetpointPerm;
					pCurrentNode->bSetSetpointPercentPerm = pReceivedNodeData->bSetSetpointPercentPerm;
					pCurrentNode->bCombined = pReceivedNodeData->bCombined;
					pCurrentNode->bSetpointDataValid = pReceivedNodeData->bSetpointDataValid;
					pCurrentNode->bRangeDataValid = pReceivedNodeData->bRangeDataValid;
					pCurrentNode->bPercentDataValid = pReceivedNodeData->bPercentDataValid;
					pCurrentNode->bReferenceGrossValid = pReceivedNodeData->bReferenceGrossValid;
					pCurrentNode->bReferenceMassValid = pReceivedNodeData->bReferenceMassValid;
					pCurrentNode->wXfrMode = pReceivedNodeData->wXfrMode;
					pCurrentNode->wXfrModeOld = pReceivedNodeData->wXfrModeOld;
					pCurrentNode->dXfrSetpoint = pReceivedNodeData->dXfrSetpoint;
					pCurrentNode->dXfrSetpointOld = pReceivedNodeData->dXfrSetpointOld;
					pCurrentNode->dXfrSetpointInPercent = pReceivedNodeData->dXfrSetpointInPercent;
					pCurrentNode->dXfrSetpointInPercentOld = pReceivedNodeData->dXfrSetpointInPercentOld;
					pCurrentNode->bXfrSetpointUnits = pReceivedNodeData->bXfrSetpointUnits;
					pCurrentNode->bXfrSetpointStyle = pReceivedNodeData->bXfrSetpointStyle;
					pCurrentNode->dXfrSetpointMax = pReceivedNodeData->dXfrSetpointMax;
					pCurrentNode->dXfrSetpointMin = pReceivedNodeData->dXfrSetpointMin;
					pCurrentNode->dXfrSetpointInPercentMax = pReceivedNodeData->dXfrSetpointInPercentMax;
					pCurrentNode->dXfrSetpointInPercentMin = pReceivedNodeData->dXfrSetpointInPercentMin;
					pCurrentNode->dXfrReferenceGross = pReceivedNodeData->dXfrReferenceGross;
					pCurrentNode->dXfrReferenceMass = pReceivedNodeData->dXfrReferenceMass;
					pCurrentNode->bStatus = pReceivedNodeData->bStatus;

					buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 129);
					if (buffer)
					{
						::ZeroMemory(buffer, sizeof(wchar_t) * 129);
						lstrcpy(buffer, pReceivedNodeData->szTankDataBaseReference);
						pCurrentNode->szTankDataBaseReference = buffer;
					}

					buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 129);
					if (buffer)
					{
						::ZeroMemory(buffer, sizeof(wchar_t) * 129);
						lstrcpy(buffer, pReceivedNodeData->szMeterGrossReference);
						pCurrentNode->szMeterGrossReference = buffer;
					}

					buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 129);
					if (buffer)
					{
						::ZeroMemory(buffer, sizeof(wchar_t) * 129);
						lstrcpy(buffer, pReceivedNodeData->szMeterNetReference);
						pCurrentNode->szMeterNetReference = buffer;
					}

					buffer = (wchar_t*)CoTaskMemAlloc(sizeof(wchar_t) * 129);
					if (buffer)
					{
						::ZeroMemory(buffer, sizeof(wchar_t) * 129);
						lstrcpy(buffer, pReceivedNodeData->szMeterMassReference);
						pCurrentNode->szMeterMassReference = buffer;
					}
				}

			}
		}
	}

	if (pMovementData)
	{
		if (pMovementData->pNodeInstanceData)
		{
			free(pMovementData->pNodeInstanceData);
			pMovementData->pNodeInstanceData = 0;
		}
		free(pMovementData);
		pMovementData = 0;
	}

	return(TRUE);
}

DWORD SetMovementInstance(MYMOVEINSTANCEDATA *pMovementData, DWORD dwMoveID)
{
	RPC_BINDING_HANDLE	hMvmntBinding = NULL;
	DWORD dwError = 0, dwSize;
	TCHAR szSystemName[MAX_COMPUTERNAME_LENGTH + 3];
	TCHAR szComputerName[MAX_COMPUTERNAME_LENGTH + 3];
	WCHAR szUserName[MAX_USERNAME_LENGTH + 1];

	dwSize = MAX_COMPUTERNAME_LENGTH + 3;
	lstrcpy(szSystemName, TEXT("\\\\"));
	if (!GetComputerName(szComputerName, &dwSize))
		return(FALSE);

	lstrcat(szSystemName, szComputerName);	// this forces the use of named pipes locally.

	dwSize = MAX_USERNAME_LENGTH + 1;
	if (!GetUserName(szUserName, &dwSize))
		return(FALSE);

	if (!MvmntBind(&hMvmntBinding, szSystemName))
		return FALSE;

	MOVEINSTANCEDATA MoveInstanceData;
	MOVEINSTANCEDATA* pMoveInstanceData = &MoveInstanceData;
	
	if (pMoveInstanceData && pMovementData)
	{
		lstrcpy(pMoveInstanceData->szName, pMovementData->szName);
		lstrcpy(pMoveInstanceData->szOrder, pMovementData->szOrder);
		lstrcpy(pMoveInstanceData->szComment, pMovementData->szComment);

		::ZeroMemory(&pMoveInstanceData->szUserDef[0][0], sizeof(wchar_t) * 10 * 31);
		//wmemcpy(&pCurrent->szUserDef[0][0], pMovementData->szUserDef, 310);

		if (pMovementData->szUserDef)
		{
			for (int j = 0, len = 0; j < 10; j++)
			{
				lstrcpy(pMoveInstanceData->szUserDef[j], (pMovementData->szUserDef)[j]);
				CoTaskMemFree((pMovementData->szUserDef)[j]);
			}
			CoTaskMemFree(pMovementData->szUserDef);
		}

		lstrcpy(pMoveInstanceData->szGroup, pMovementData->szGroup);
		lstrcpy(pMoveInstanceData->szReportName, pMovementData->szReportName);
		lstrcpy(pMoveInstanceData->szPrinterName, pMovementData->szPrinterName);
		lstrcpy(pMoveInstanceData->szInputPoint, pMovementData->szInputPoint);

		pMoveInstanceData->lPlannedStartTime = pMovementData->lPlannedStartTime;
		pMoveInstanceData->bPlannedStartTimeOperational = (pMovementData->bPlannedStartTimeOperational & 0xff);
		pMoveInstanceData->wPlannedStartTimeStatus = (pMovementData->wPlannedStartTimeStatus & 0xffff);
		pMoveInstanceData->lAutoStartTime = pMovementData->lAutoStartTime;
		pMoveInstanceData->bAutoStartTimeActive = (pMovementData->bAutoStartTimeActive & 0xff);
		pMoveInstanceData->wAutoStartTimeStatus = (pMovementData->wAutoStartTimeStatus & 0xffff);
		pMoveInstanceData->lAutoStopTime = pMovementData->lAutoStopTime;
		pMoveInstanceData->bAutoStopTimeActive = (pMovementData->bAutoStopTimeActive & 0xff);
		pMoveInstanceData->wAutoStopTimeStatus = (pMovementData->wAutoStopTimeStatus & 0xffff);
		pMoveInstanceData->wZeroFlowHoldOffMinutes = pMovementData->wZeroFlowHoldOffMinutes;
		pMoveInstanceData->bType = pMovementData->bType;
		pMoveInstanceData->bCommit = pMovementData->bCommit;
		pMoveInstanceData->bOkPerm = pMovementData->bOkPerm;
		pMoveInstanceData->bOrderPerm = pMovementData->bOrderPerm;
		pMoveInstanceData->bSourceSetpointsInPercentPerm = pMovementData->bSourceSetpointsInPercentPerm;
		pMoveInstanceData->bSourceSetpointsInPercent = pMovementData->bSourceSetpointsInPercent;
		pMoveInstanceData->bSourceSetpointsInPercentOld = pMovementData->bSourceSetpointsInPercentOld;
		pMoveInstanceData->bAutoDelete = pMovementData->bAutoDelete;
		pMoveInstanceData->bStartOnNonZeroFlow = pMovementData->bStartOnNonZeroFlow;
		pMoveInstanceData->bStopOnZeroFlow = pMovementData->bStopOnZeroFlow;
		pMoveInstanceData->bInterlockSetpoints = pMovementData->bInterlockSetpoints;
		pMoveInstanceData->bIncludeHandValues = pMovementData->bIncludeHandValues;
		pMoveInstanceData->bLineupActionSequence = pMovementData->bLineupActionSequence;
		pMoveInstanceData->bLineupActionSequencePerm = pMovementData->bLineupActionSequencePerm;
		pMoveInstanceData->bHaltOnCompletion = pMovementData->bHaltOnCompletion;
		pMoveInstanceData->bInhibitSetpointOverrange = pMovementData->bInhibitSetpointOverrange;
		pMoveInstanceData->bInhibitMovementType = pMovementData->bInhibitMovementType;
		pMoveInstanceData->bIndividualNodeControl = pMovementData->bIndividualNodeControl;
		pMoveInstanceData->bUsePendingOperation = pMovementData->bUsePendingOperation;
		pMoveInstanceData->bUseInputPoint = pMovementData->bUseInputPoint;
		pMoveInstanceData->bSendMvmntToSnapIn = pMovementData->bSendMvmntToSnapIn;
		pMoveInstanceData->bMvmntToSnapInAvailable = pMovementData->bMvmntToSnapInAvailable;
		pMoveInstanceData->dwInitiationCount = pMovementData->dwInitiationCount;
		pMoveInstanceData->tInitiationTime = pMovementData->tInitiationTime;
		pMoveInstanceData->wNumberOfNodes = pMovementData->wNumberOfNodes;

		NODEINSTANCEDATA* pNodeData = (NODEINSTANCEDATA*) calloc (pMovementData->wNumberOfNodes, sizeof(NODEINSTANCEDATA));
		pMoveInstanceData->pNodeInstanceData = pNodeData;
		
		if (pNodeData)
		{
			for (int i = 0, j = 1; i < pMovementData->wNumberOfNodes; i++, j++)
			{
				NODEINSTANCEDATA* pCurrentNode = &pMoveInstanceData->pNodeInstanceData[i];
				MYNODEINSTANCEDATA* pReceivedNodeData = &pMovementData->pNodeInstanceData[i];

				lstrcpy(pCurrentNode->szName, pReceivedNodeData->szName);
				lstrcpy(pCurrentNode->szNameOld, pReceivedNodeData->szNameOld);
			
				pCurrentNode->wNodeID = pReceivedNodeData->wNodeID;
				pCurrentNode->bType = pReceivedNodeData->bType;
				pCurrentNode->bSource = pReceivedNodeData->bSource;
				pCurrentNode->bSetNamePerm = pReceivedNodeData->bSetNamePerm;
				pCurrentNode->bSetXfrModePerm = pReceivedNodeData->bSetXfrModePerm;
				pCurrentNode->bSetXfrModeInactivePerm = pReceivedNodeData->bSetXfrModeInactivePerm;
				pCurrentNode->bSetSetpointPerm = pReceivedNodeData->bSetSetpointPerm;
				pCurrentNode->bSetSetpointPercentPerm = pReceivedNodeData->bSetSetpointPercentPerm;
				pCurrentNode->bCombined = pReceivedNodeData->bCombined;
				pCurrentNode->bSetpointDataValid = pReceivedNodeData->bSetpointDataValid;
				pCurrentNode->bRangeDataValid = pReceivedNodeData->bRangeDataValid;
				pCurrentNode->bPercentDataValid = pReceivedNodeData->bPercentDataValid;
				pCurrentNode->bReferenceGrossValid = pReceivedNodeData->bReferenceGrossValid;
				pCurrentNode->bReferenceMassValid = pReceivedNodeData->bReferenceMassValid;
				pCurrentNode->wXfrMode = pReceivedNodeData->wXfrMode;
				pCurrentNode->wXfrModeOld = pReceivedNodeData->wXfrModeOld;
				pCurrentNode->dXfrSetpoint = pReceivedNodeData->dXfrSetpoint;
				pCurrentNode->dXfrSetpointOld = pReceivedNodeData->dXfrSetpointOld;
				pCurrentNode->dXfrSetpointInPercent = pReceivedNodeData->dXfrSetpointInPercent;
				pCurrentNode->dXfrSetpointInPercentOld = pReceivedNodeData->dXfrSetpointInPercentOld;
				pCurrentNode->bXfrSetpointUnits = pReceivedNodeData->bXfrSetpointUnits;
				pCurrentNode->bXfrSetpointStyle = pReceivedNodeData->bXfrSetpointStyle;
				pCurrentNode->dXfrSetpointMax = pReceivedNodeData->dXfrSetpointMax;
				pCurrentNode->dXfrSetpointMin = pReceivedNodeData->dXfrSetpointMin;
				pCurrentNode->dXfrSetpointInPercentMax = pReceivedNodeData->dXfrSetpointInPercentMax;
				pCurrentNode->dXfrSetpointInPercentMin = pReceivedNodeData->dXfrSetpointInPercentMin;
				pCurrentNode->dXfrReferenceGross = pReceivedNodeData->dXfrReferenceGross;
				pCurrentNode->dXfrReferenceMass = pReceivedNodeData->dXfrReferenceMass;
				pCurrentNode->bStatus = pReceivedNodeData->bStatus;
			
				lstrcpy(pCurrentNode->szTankDataBaseReference, pReceivedNodeData->szTankDataBaseReference);
				lstrcpy(pCurrentNode->szMeterGrossReference, pReceivedNodeData->szMeterGrossReference);
				lstrcpy(pCurrentNode->szMeterNetReference, pReceivedNodeData->szMeterNetReference);
				lstrcpy(pCurrentNode->szMeterMassReference, pReceivedNodeData->szMeterMassReference);
			}
		}

		// Free any received memory
		if (pMovementData)
		{
			if (pMovementData->pNodeInstanceData)
			{
				CoTaskMemFree(pMovementData->pNodeInstanceData);
				pMovementData->pNodeInstanceData = NULL;
			}
		}

		RpcTryExcept;
		{
			dwError = MvmntSetMoveInst(hMvmntBinding,
				szSystemName,
				szUserName,
				dwMoveID,
				&pMoveInstanceData);
		}
		RpcExcept(1)
		{
			RPC_STATUS	RpcStatus;

			RpcStatus = RpcExceptionCode();
			RpcBindingFree(&hMvmntBinding);

			// release any memory used to pass the reference
			if (pMoveInstanceData->pNodeInstanceData)
			{
				free(pMoveInstanceData->pNodeInstanceData);
				pMoveInstanceData->pNodeInstanceData = NULL;
			}
			return FM_ERROR_RPC;
		}
		RpcEndExcept;
	}
	RpcBindingFree(&hMvmntBinding);

	// release any memory used to pass the reference
	if (pMoveInstanceData->pNodeInstanceData)
	{
		free(pMoveInstanceData->pNodeInstanceData);
		pMoveInstanceData->pNodeInstanceData = NULL;
	}

	if (dwError == FM_ERROR_NONE)
	{
		return(TRUE);
	}
	else
	{
		return (FALSE);
	}
}

