using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class ExistTransactionAssetSR : AccountingServiceRequest
	{
		#region Properties
		[DataMember]
		public Guid SiteGuid
		{
			get;
			set;
		}

		[DataMember]
		public string Product
		{
			get;
			set;
		}

		[DataMember]
		public string Tank
		{
			get;
			set;
		}

		[DataMember]
		public DateTime InventoryDate
		{
			get;
			set;
		}

		[DataMember]
		public string AliasName
		{
			get;
			set;
		}

		[DataMember]
		public Guid TransactionLineItemGuid
		{
			get;
			set;
		}
		#endregion // Properties

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Exist Transaction Asset Service Request class.
		/// </summary>
		public ExistTransactionAssetSR()
			: base()
		{
			SiteGuid = Guid.Empty;
			Product = "";
			Tank = "";
			InventoryDate = DateTime.Today;
			AliasName = "";
			TransactionLineItemGuid = Guid.Empty;
		}
		#endregion
	}
}
