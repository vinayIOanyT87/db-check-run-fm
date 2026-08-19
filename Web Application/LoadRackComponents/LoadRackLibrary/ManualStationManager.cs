/******************************************************************************

	FILE NAME:		ManualStationManager.cs


	PURPOSE:			ManualStationManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/
using System;
using System.Diagnostics;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

namespace LoadRackLibrary
{
	/// <summary>
	/// Summary description for ManualStationManagerClass.
	/// </summary>
	public class ManualStationManagerClass :	StationManagerClass
	{
		public ManualStationManagerClass(	EventLog					EventLog,
														LoadRackManagerClass LoadRackManager,
														StationClass			Station,
														SiteManagerClass		SiteManager,
														SecurityClass			Security)
		: base(EventLog,LoadRackManager,Station,SiteManager,Security)
		{
		}
	}
}
