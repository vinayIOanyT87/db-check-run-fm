// this file contains system functions like data objects
#include <stdlib.h>
#include "EngineeringUnits.h"
#include <link.h>
#include <corecrt_malloc.h>
#include "utility.hpp"
#include <winnt.h>
#include "FMDefines.hpp"
#include "SystemFunctions.hpp"

#define MAX_PATH	260

// load the tank gauge data used by leak detection
bool getTankGaugeData(TCHAR * szgaugename,
							short * pstype,
							DOUBLE * pdthreshold,
							DOUBLE * pdcertification,
							DOUBLE * pddeltatemp,
							short * psmintime)
{
	// this rountine will read the tank gauge data file from the projevt directory. If it does not exist it is a failure
	// get the project directory from the registry
	TCHAR	szprojectdir[MAX_PATH + 1] = TEXT("");
	TCHAR szfilespec[MAX_PATH + 1] = TEXT("");
	HANDLE hfile = INVALID_HANDLE_VALUE;
	GAUGETYPEHEADER header;
	DWORD dwsize = 0;
	long larraysize = 0;
	LPTANKGAUGETYPES m_lpTankGaugeTypes;

	if (!bGetRegistryStringValue((LPTSTR)TEXT("Software\\Varec\\SCADA"),
		(LPTSTR)TEXT("Project"),
		szprojectdir))
	{
		return false;
	}

	// Construct File Spec
	lstrcpy(szfilespec, szprojectdir);
	lstrcat(szfilespec, TEXT("\\FM_GaugeData.dat"));

	// Attempt to Open File
	hfile = CreateFile(szfilespec, GENERIC_READ | GENERIC_WRITE,
		0,
		(LPSECURITY_ATTRIBUTES)NULL,
		OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);

	if (INVALID_HANDLE_VALUE == hfile)
	{
		return false;
	}

	// Read Header to Determine Array Parameters
	if (!ReadFile(hfile, (LPVOID)&header, sizeof(GAUGETYPEHEADER), &dwsize, NULL))
	{
		CloseHandle(hfile);
		return false;
	}

	// verify that this is the correct version
	if (header.wMajorRev != FM_MAJORREV ||
		header.wMinorRev != FM_MINORREV)
	{
		CloseHandle(hfile);
		return false;
	}

	// read the hand gauge data from the file
	larraysize = header.wNumGaugeTypes * sizeof(TANKGAUGETYPES);
	m_lpTankGaugeTypes = (LPTANKGAUGETYPES)calloc(header.wNumGaugeTypes, sizeof(TANKGAUGETYPES));
	if (!m_lpTankGaugeTypes)
	{
		CloseHandle(hfile);
		return false;
	}
	if (!ReadFile(hfile, (LPVOID)m_lpTankGaugeTypes, larraysize, &dwsize, NULL))
	{
		if (m_lpTankGaugeTypes)
			free(m_lpTankGaugeTypes);
		m_lpTankGaugeTypes = NULL;
		CloseHandle(hfile);
		return false;
	}

	int iIndex = -1;
	if (m_lpTankGaugeTypes && header.wNumGaugeTypes > 0)
	{
		// find the guage and return the leak settings
		for (int loop = 0; loop < header.wNumGaugeTypes;loop++)
		{
			int yy = 0;
			++yy;
			if (!lstrcmpi((LPCWSTR)szgaugename, (LPCWSTR)m_lpTankGaugeTypes[loop].szGaugeName))
			{
				// set the data for being returned
				iIndex = loop;
				break;
			}
		 }
		//szgaugename
	}

	if (iIndex < 0)
	{
		if (m_lpTankGaugeTypes)
			free(m_lpTankGaugeTypes);
		m_lpTankGaugeTypes = NULL;
		CloseHandle(hfile);
		return false;
	}

	// populate the leak datection data
	LPTANKGAUGETYPES pgaugeentry;

	pgaugeentry = &m_lpTankGaugeTypes[iIndex];
	//lstrcpyW(szTGstring, pgaugeentry->szGaugeName);
	*pstype = pgaugeentry->byGaugeType;
	*pdthreshold = pgaugeentry->dThresholdRate;
	*pdcertification = pgaugeentry->dCertificationRate;
	*pddeltatemp = pgaugeentry->dDeltaTemp;
	*psmintime = pgaugeentry->nMinTestTime;


	if (m_lpTankGaugeTypes)
		free(m_lpTankGaugeTypes);
	m_lpTankGaugeTypes = NULL;
	CloseHandle(hfile);
	return true;
}

