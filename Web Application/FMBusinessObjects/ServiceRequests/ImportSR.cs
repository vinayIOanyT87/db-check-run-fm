using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class ImportSR : AccountingServiceRequest
	{
		#region Protected data members
		[DataMember]
		protected string importName;
		[DataMember]
		protected string filePath;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Import service request class.
		/// </summary>
		public ImportSR()
		{
		}
		#endregion

		#region Properties

		public string ImportName
		{
			get { return this.importName; }
			set { this.importName = value; }
		}

		public string FilePath
		{
			get { return this.filePath; }
			set { this.filePath = value; }
		}
		#endregion
	}
}
