/******************************************************************************

	FILE NAME:		MultiloadDevice.h


	PURPOSE:			Declaration of the CMultiloadDevice


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
#include "Io.h"
#include "Device.h"

class CMultiloadDevice : CDevice
{
public:	
	CTag*				m_pRcuStatusTag;
	CTag*				m_pCardStatusTag;
	CTag*				m_pStatusTag;
	CTag*				m_pCardNumberTag;
public:
	CMultiloadDevice(ACCULOAD_TYPE Type) : CDevice(Type)
	{
	}

	HRESULT PrepareRequest(CTag* pTag,BOOL bWrite);
	HRESULT PerformNetworkIO(CTag* pTag);
	HRESULT ProcessResponse(CTag* pTag,HRESULT hr);
	HRESULT PerformSerialIO(CTag* pTag);
};


