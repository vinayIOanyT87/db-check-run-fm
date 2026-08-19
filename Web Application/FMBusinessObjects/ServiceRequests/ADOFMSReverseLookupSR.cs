using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class ADOFMSReverseLookupSR : AccountingServiceRequest
	{
		#region Public data members
		// the same can be found in the creation script for fm_ADF_ADOFMSEntityLookup
		public enum EntityID : int
		{
			UNSPECIFIED = 0,
			SITE = 1,
			COMPANY = 2,
			PRODUCT = 3,
			EQUIPMENT = 4
		}
		#endregion

		#region Properties
		[DataMember]
		public EntityID EntityIdentifier
		{
			get;
			set;
		}

		[DataMember]
		public string EntityValue
		{
			get;
			set;
		}
		#endregion // Properties

		#region Construction
		public ADOFMSReverseLookupSR()
		{

		}
		#endregion // Construction
	}
}
