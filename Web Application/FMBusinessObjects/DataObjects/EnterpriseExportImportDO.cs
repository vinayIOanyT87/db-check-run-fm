using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class EnterpriseExportImportDO : BaseDataObject
	{
		#region Private data members
		private string alarmAndEventSourceName;
		private String strExportArchiveDir;											// set from Database tblSettings
		private String strImportArchiveDir;											// set from Database tblSettings
		private String strURLofEnterpriseDataWebService;						// set from Database tblSettings
		private String strExportingSiteGuid;										// set from Database tblSettings
		private int enterpriseDataIntervalBetweenSendAttemptsInSeconds;	// set from Database tblSettings
		private int enterpriseDataSentAttempts;									// set from Database tblSettings
		private bool logImportProcessRunInformation;								// set from Database tblSettings
		#endregion

		#region Constructors
		public EnterpriseExportImportDO ()
		{
			this.Init ( );
		}
		#endregion

		#region Properties
		[DataMember]
		public String AlarmAndEventSourceName
		{
			get { return this.alarmAndEventSourceName; }
			set { this.alarmAndEventSourceName = value; }
		}

		[DataMember]
		public String ExportArchiveDir
		{
			get { return this.strExportArchiveDir; }
			set { this.strExportArchiveDir = value; }
		}

		[DataMember]
		public String ImportArchiveDir
		{
			get { return this.strImportArchiveDir; }
			set { this.strImportArchiveDir = value; }
		}
		
		[DataMember]
		public String URLofEnterpriseDataWebService
		{
			get { return this.strURLofEnterpriseDataWebService; }
			set { this.strURLofEnterpriseDataWebService = value; }
		}

		[DataMember]
		public String ExportingSiteGuid
		{
			get { return this.strExportingSiteGuid; }
			set { this.strExportingSiteGuid = value; }
		}

		[DataMember]
		public int EnterpriseDataIntervalBetweenSendAttemptsInSeconds
		{
			get { return this.enterpriseDataIntervalBetweenSendAttemptsInSeconds; }
			set { this.enterpriseDataIntervalBetweenSendAttemptsInSeconds = value; }
		}

		[DataMember]
		public int EnterpriseDataSendAttempts
		{
			get { return this.enterpriseDataSentAttempts; }
			set { this.enterpriseDataSentAttempts = value; }
		}

		[DataMember]
		public bool LogImportProcessInformation
		{
			get { return this.logImportProcessRunInformation; }
			set { this.logImportProcessRunInformation = value; }
		}
		#endregion

		#region Private methods
		private void Init()
		{
			this.alarmAndEventSourceName										= "";
			this.strURLofEnterpriseDataWebService							= "";
			this.strImportArchiveDir											= "";
			this.strExportArchiveDir											= "";
			this.enterpriseDataSentAttempts									= 1;		// default is to run once;
			this.enterpriseDataIntervalBetweenSendAttemptsInSeconds	= 3600;	// Default is to wait one hour between attempts. 
			this.logImportProcessRunInformation								= false;
			this.ExportingSiteGuid												= Guid.Empty.ToString();
		}
		#endregion
	}
}
