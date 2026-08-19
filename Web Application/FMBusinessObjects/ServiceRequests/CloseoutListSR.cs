using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class CloseoutListSR : AccountingServiceRequest
	{
		#region Attributes

		[DataMember]
		protected DateTime? startDate;
		[DataMember]
		protected DateTime? endDate;
		[DataMember]
		protected bool convertUnits;
		[DataMember]
		protected ProductType productType;
		#endregion Attributes

		#region Properties

		public ProductType ProductType
		{
			get { return this.productType; }
			set { this.productType = value; }
		}

        [DataMember]
        public Guid ManagerGuid { get; set; }

        [DataMember]
        public Guid ProductGuid { get; set; }

		public DateTime? StartDate
		{
			get { return startDate; }
			set { startDate = value; }
		}

		public DateTime? EndDate
		{
			get { return endDate; }
			set { endDate = value; }
		}

		public bool ConvertUnits
		{
			get { return convertUnits; }
			set { convertUnits = value; }
		}

		[DataMember]
		public bool GetPreviousAndSubsequentCloseouts { get; set; }

		#endregion Properties

		public CloseoutListSR()
		{
			this.convertUnits = true;
			this.GetPreviousAndSubsequentCloseouts = true;
		}
	}
}
