/// <summary>
///	File name:	TransactionBolPidxSR.cs
///	Purpose:		The purpose of this class is to encapsulate the transaction BOL PIDX 
///					service updates.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///					2000.  This file shall not be copied or reproduced in any form 
///					without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:		1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------	--------------------	----------------------------------
///		
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class TransactionBolPidxSR : AccountingServiceRequest
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction BOL PIDX service request.
		/// </summary>
		public TransactionBolPidxSR ( )
		{
		}
		#endregion
	}
}
