/// <summary>
/// File name:	ReportUrlDirSR.cs
/// Purpose:	Contains the report URL/directory service request parameters to retrieve
///				report system information.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:					Reason:
///		----------	------------------	-------------------------------------------
///		2006-09-22	Richard Panachida	Initial version. Used to request the report URL
///										and directory information.
/// </summary>
/// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class ReportUrlDirSR : AccountingServiceRequest
	{
		[DataMember]
		private Guid reportGuid;

		public Guid ReportGuid
		{
			get { return reportGuid; }
			set { reportGuid = value; }
		}


		#region Constructor
		/// <summary>
		/// This is the default constructor for the report url/path service 
		/// request class.
		/// </summary>
		public ReportUrlDirSR ( )
		{
			this.Initialize ( );
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will set this object to its initial state.
		/// </summary>
		private void Initialize ( )
		{
		}
		#endregion
	}
}
