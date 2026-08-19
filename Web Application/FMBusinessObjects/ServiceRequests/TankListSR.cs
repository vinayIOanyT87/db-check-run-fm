/// <summary>
/// File name:	TankListSR.cs
/// Purpose:	The purpose of the tanklist service request is to request a list
///				of tanks that are in the transaction line items table.
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA. 
///				This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Chris Knight
///	Version:	8.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		02-Spt-2010     C. Knight               Initial Creation
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
	public class TankListSR : AccountingServiceRequest
	{
		#region Private data members
		[DataMember]
		private string productId;
		[DataMember]
		private string managerId;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the tank list service
		/// request class.
		/// </summary>
		public TankListSR ( )
		{
		}
		#endregion

		#region Properties

		public string ManagerId
		{
			get { return managerId; }
			set { managerId = value; }
		}

		public string ProductId
		{
			get { return productId; }
			set { productId = value; }
		}
		#endregion
	}
}
