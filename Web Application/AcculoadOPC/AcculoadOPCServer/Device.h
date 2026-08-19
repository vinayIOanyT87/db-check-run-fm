/******************************************************************************

	FILE NAME:		Device.h


	PURPOSE:			Declaration of the CDevice


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
	Date:			By:			Reason:
	-----------	----------  -------------------------------------------
*******************************************************************************/
#pragma once
#include "IO.h"

class CDevice
{
public:
	ACCULOAD_TYPE	m_Type;

protected:


public:
	CDevice(ACCULOAD_TYPE Type);
	~CDevice(void);

	virtual HRESULT PrepareRequest(CTag* pTag,BOOL bWrite){return S_OK;}
	virtual HRESULT PerformNetworkIO(CTag* pTag){return S_OK;}
	virtual HRESULT ProcessResponse(CTag* pTag,HRESULT hr){return S_OK;}
	virtual HRESULT PerformSerialIO(CTag* pTag){return S_OK;}

protected:
};


