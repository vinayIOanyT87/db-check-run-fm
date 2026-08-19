using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class SecondaryDefuelDO : BaseTransactionDO
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the secondary defuel data
		/// object class.
		/// </summary>
		public SecondaryDefuelDO()
		{
		}
		#endregion

		#region Properties
		public PaymentInfoDO PaymentInfo
		{
			get { return paymentInfo; }
			set { paymentInfo = value; }
		}
		#endregion
	}
}
