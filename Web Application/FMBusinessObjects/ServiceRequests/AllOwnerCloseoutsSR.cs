using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class AllOwnerCloseoutsSR : AccountingServiceRequest
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the All Owner Closeouts Service Request class.
		/// </summary>
		public AllOwnerCloseoutsSR ( )
		{

		}
		#endregion
	}
}
