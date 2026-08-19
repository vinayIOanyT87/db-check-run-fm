using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class TransactionFilterSR : AccountingServiceRequest
	{
		#region Public enumerations
		public enum DateType { INVENTORYDATE, TRANSACTIONDATETIME }
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction filter service
		/// request class.
		/// </summary>
		public TransactionFilterSR()
			: base()
		{
			this.Site = "";
			this.SupplierID = "";
			this.TransTypeID = 0;
			this.StartDateInventory = DateTimeOffset.Now.AddDays(-1.0);
			this.EndDateInventory = DateTimeOffset.Now;
			this.DocumentNumber = "";
			this.InvoiceQuery = 0;
			this.UpdatedBy = "";
			this.UseDate = DateType.TRANSACTIONDATETIME;
		}
		#endregion

		#region Properties
		[DataMember]
		public string SupplierID
		{
			get;
			set;
		}

		[DataMember]
		public TransactionTypes TransTypeID
		{
			get;
			set;
		}

		[DataMember]
		public DateTimeOffset StartDateInventory
		{
			get;
			set;
		}

		[DataMember]
		public DateTimeOffset EndDateInventory
		{
			get;
			set;
		}

		[DataMember]
		public string DocumentNumber
		{
			get;
			set;
		}

		[DataMember]
		public int InvoiceQuery
		{
			get;
			set;
		}

		[DataMember]
		public string UpdatedBy
		{
			get;
			set;
		}

		[DataMember]
		public DateType UseDate
		{
			get;
			set;
		}
		#endregion // Properties
	}
}
