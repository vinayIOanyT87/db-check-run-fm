using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	public class PrimaryDefuelDO : BaseTransactionDO
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the primary defuel data 
		/// object class.
		/// </summary>
		public PrimaryDefuelDO()
		{
		}
		#endregion

		#region Properties

		public string LinkedDocumentNumber
		{
			get { return linkedDocumentNumber; }
			set { linkedDocumentNumber = value; }
		}

		public string CarrierID
		{
			get { return carrierID; }
			set { carrierID = value; }
		}

		public string CarrierCode
		{
			get { return carrierCode; }
			set { carrierCode = value; }
		}

		public PaymentInfoDO PaymentInfo
		{
			get { return paymentInfo; }
			set { paymentInfo = value; }
		}
		#endregion Properties
	}
}
