/******************************************************************************

	FILE NAME:		SignatureStationManager.cs


	PURPOSE:			SignatureStationManagerClass


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA.

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec, Inc.


	AUTHOR(S):	W. Gray


	VERSION:		7.4.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		16-Apr-2008	C. Knight	7.4.0.0 - CSI 5503 - Initial creation to support stored signature
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
	/// Class for handling signature capture at designated signature stations
	/// </summary>
	public class SignatureStationManagerClass : StationManagerClass
	{
		public SignatureStationManagerClass(EventLog				eventLog,
											LoadRackManagerClass	loadRackManager,
											StationClass			station,
											SiteManagerClass		siteManager,
											SecurityClass			security)
			: base(eventLog, loadRackManager, station, siteManager, security)
		{
		}

		/// <summary>
		/// new method specifically for grabbing signature from signature device
		/// </summary>
		/// <returns>signature retreived from the signature pad as a byte array</returns>
		public override byte[] GetSignature()
		{
			SignatureCaptureClass signatureCapture = new SignatureCaptureClass(this.eventLog);

			return signatureCapture.Get(base.Station.SignatureDevice,base.Station.SignatureDevicePort,base.Station.SignatureDeviceBaudRate);
		}
	}
}
