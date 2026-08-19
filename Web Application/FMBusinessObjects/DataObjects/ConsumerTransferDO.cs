using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Summary description for ConsumerTransferDO.
	/// </summary>
	[DataContract]
   [Serializable]
	public class ConsumerTransferDO : TransactionDO
	{
		#region Attributes
		[DataMember]
		protected string toBillToCode;
		[DataMember]
		protected string toBillToID;
		[DataMember]
		protected Guid toBillToCompanyGuid;
		[DataMember]
		protected string toShipToCode;
		[DataMember]
		protected string toShipToID;
		[DataMember]
		protected Guid toShipToCompanyGuid;
		#endregion Attributes

		#region Properties

		public string ToBillToCode
		{
			get { return toBillToCode; }
			set { toBillToCode = value; }
		}

		public string ToBillToID
		{
			get { return toBillToID; }
			set { toBillToID = value; }
		}

		public Guid ToBillToCompanyGuid
		{
			get { return toBillToCompanyGuid; }
			set { toBillToCompanyGuid = value; }
		}

		public string ToShipToCode
		{
			get { return toShipToCode; }
			set { toShipToCode = value; }
		}

		public string ToShipToID
		{
			get { return toShipToID; }
			set { toShipToID = value; }
		}

		public Guid ToShipToCompanyGuid
		{
			get { return toShipToCompanyGuid; }
			set { toShipToCompanyGuid = value; }
		}

		#endregion Properties

		public ConsumerTransferDO()
		{

		}
	}
}
