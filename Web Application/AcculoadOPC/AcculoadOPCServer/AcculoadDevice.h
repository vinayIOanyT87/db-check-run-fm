/******************************************************************************

	FILE NAME:		AcculoadDevice.h


	PURPOSE:			Declaration of the CAcculoadDevice


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
	Date:			By:			Reason:
	-----------	----------  -------------------------------------------
*******************************************************************************/
#pragma once
#include "IO.h"
#include "Device.h"

class CAcculoadDevice :
	public CDevice
{
public:
	CAcculoadDevice(ACCULOAD_TYPE Type) : CDevice(Type)
	{
	}

	HRESULT PrepareRequest(CTag* pTag,BOOL bWrite);
	HRESULT PerformNetworkIO(CTag* pTag);
	HRESULT ProcessResponse(CTag* pTag,HRESULT hr);
	HRESULT PerformSerialIO(CTag* pTag);
	void ReportError(CTag* pTag);
};
