using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class GetTransactionTypeSR : AccountingServiceRequest
	{
		#region Protected data members
		[DataMember]
		protected string transID;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the get transaction type service
		/// request class.
		/// </summary>
		public GetTransactionTypeSR()
		{
		}
		#endregion

		#region Properties

		public string TransID
		{
			get { return transID; }
			set { transID = value; }
		}

		#endregion
	}
}
