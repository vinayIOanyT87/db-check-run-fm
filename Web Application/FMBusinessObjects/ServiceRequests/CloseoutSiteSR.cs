using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class CloseoutSiteSR : AccountingServiceRequest
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Closeout Site Service Request class.
		/// </summary>
		public CloseoutSiteSR ( )
		{

		}
		#endregion
	}
}
