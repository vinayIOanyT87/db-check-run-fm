using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class StandingOffersSR : AccountingServiceRequest
	{
		#region Private data members
		[DataMember] private Guid standingOfferGuid = Guid.Empty;
		#endregion

		#region Constructors
 		/// <summary>
 		/// This is the default constructor for the standing offer (aka price list) service
		/// request class.
 		/// </summary>
		public StandingOffersSR ( )
		{
		}
		#endregion

		public Guid StandingOfferGuid
		{
			get { return standingOfferGuid; }
			set { standingOfferGuid = value; }
		}
	}
}
