/// <summary>
/// File name:	ReportTransactionSR.cs
/// Purpose:	Contains the report journal service request parameters and special request
///				to generate report data.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>
/// 
using System;

namespace ReportingServices
{
	[System.Serializable]
	public class ReportJournalSR : ReportServiceRequest
	{
		#region Public Attributes
		public enum SubReportTypes {JOURNAL_GROSS, JOURNAL_NET, JOURNAL_SUMMARY};
		#endregion

		#region Private Attributes
		private const int EMPTY_STRING = 0;
		private SubReportTypes subReportType;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the report journal service 
		/// request class.
		/// </summary>
		public ReportJournalSR()
		{
			base.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property gets and sets the journal sub report type (JOURNAL_GROSS,
		/// JOURNAL_NET, JOURNAL_SUMMARY).
		/// </summary>
		public SubReportTypes SubReportType
		{
			get { return this.subReportType; }
			set { this.subReportType = value; }
		}

		#endregion

		#region Private Methods
		#endregion
	}
}
