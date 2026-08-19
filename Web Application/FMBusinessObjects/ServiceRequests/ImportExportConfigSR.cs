using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class ImportExportConfigSR : AccountingServiceRequest
	{
		#region Private data members
		[DataMember]
		private ImportExportListDO importExportList = null;
		[DataMember]
		private string requestType;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the import export list service
		/// request class.
		/// </summary>
		public ImportExportConfigSR ( )
		{
		}
		#endregion

		#region Properties

		public string RequestType
		{
			get { return this.requestType; }
			set { this.requestType = value; }
		}

		public ImportExportListDO ImportExportList
		{
			get { return this.importExportList; }
			set { this.importExportList = value; }
		}
		#endregion
	}
}
