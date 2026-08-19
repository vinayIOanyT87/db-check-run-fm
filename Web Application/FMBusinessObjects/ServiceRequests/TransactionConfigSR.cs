using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class TransactionConfigSR : AccountingServiceRequest
	{
		#region Attributes
		//private bool newButtonSelected;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the transaction configuration service
		/// request class.
		/// </summary>
		public TransactionConfigSR ( )
		{
		}
		#endregion

		#region Methods
		public void setButtonSelected ( )
		{
		}
		public bool getButtonSelected ( )
		{
			return false;
		}
		public void clearButtonSelected ( )
		{
		}
		#endregion
	}
}
