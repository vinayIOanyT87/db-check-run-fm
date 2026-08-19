using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Summary description for OwnerTransferDO.
	/// </summary>
   [Serializable]
   [DataContract]
	public class OwnerTransferDO : TransactionDO
	{
		#region Attributes
		[DataMember]
		protected string toManagerCode;
		[DataMember]
		protected Guid toManagerCompanyGuid;
		[DataMember]
		protected string toOwnerCode;
		[DataMember]
		protected Guid toOwnerCompanyGuid;
		[DataMember]
		protected string toCarrierCode;
		[DataMember]
		protected Guid toCarrierCompanyGuid;
		#endregion Attributes

		#region Properties

		public string ToManagerCode
		{
			get { return toManagerCode; }
			set { toManagerCode = value; }
		}

		public Guid ToManagerCompanyGuid
		{
			get { return toManagerCompanyGuid; }
			set { toManagerCompanyGuid = value; }
		}

		public string ToOwnerCode
		{
			get { return toOwnerCode; }
			set { toOwnerCode = value; }
		}

		public Guid ToOwnerCompanyGuid
		{
			get { return toOwnerCompanyGuid; }
			set { toOwnerCompanyGuid = value; }
		}

		public string ToCarrierCode
		{
			get { return toCarrierCode; }
			set { toCarrierCode = value; }
		}

		public Guid ToCarrierCompanyGuid
		{
			get { return toCarrierCompanyGuid; }
			set { toCarrierCompanyGuid = value; }
		}

		#endregion Attributes

		public OwnerTransferDO()
		{

		}
	}
}
