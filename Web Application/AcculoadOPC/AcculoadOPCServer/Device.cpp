/******************************************************************************

	FILE NAME:		Device.cpp


	PURPOSE:			Implementation of the CDevice


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


#include "StdAfx.h"
#include "Device.h"
#include "DeviceManager.h"

extern CDeviceManager*		g_pDeviceManager;

CDevice::CDevice(ACCULOAD_TYPE Type)
{
	m_Type=Type;
}

CDevice::~CDevice(void)
{
}

