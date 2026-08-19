using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class ImportExportSR : AccountingServiceRequest
	{
		#region Public enumerations
		public enum ImportExportType { IMPORT, EXPORT };
		public enum ImportExportButtons { BROWSE, OK, MANAGE_CONFIGURATIONS };
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the import export service
		/// request class.
		/// </summary>
		public ImportExportSR()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public ImportExportType Type
		{
			get;
			set;
		}

		[DataMember]
		public ImportExportButtons ButtonSelected
		{
			get;
			set;
		}

		[DataMember]
		public string ConfigurationName
		{
			get;
			set;
		}

		[DataMember]
		public string FileName
		{
			get;
			set;
		}

		[DataMember]
		public DateTimeOffset ToDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTimeOffset FromDate
		{
			get;
			set;
		}
		#endregion
	}
}
