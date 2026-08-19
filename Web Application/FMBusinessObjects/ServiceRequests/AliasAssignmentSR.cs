using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class AliasAssignmentSR : AccountingServiceRequest
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Alias Assignment Service Request class.
		/// </summary>
		public AliasAssignmentSR()
		{

		}
		#endregion
	}
}
