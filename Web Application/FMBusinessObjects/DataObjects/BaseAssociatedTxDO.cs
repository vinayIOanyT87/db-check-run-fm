using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public abstract class BaseAssociatedTxDO : BaseLineItemDO
	{
		#region Private data members
		[DataMember]
		private bool associated;
		[DataMember]
		private DateTimeOffset transDate;
		[DataMember]
		private DateTime inventoryDate;
		[DataMember]
		private string alias;
		[DataMember]
		private string fuelType;
		[DataMember]
		private string supplier;
		[DataMember]
		private string manager;
		[DataMember]
		private string owner;
		[DataMember]
		private string shipTo;
		[DataMember]
		private string billTo;
		[DataMember]
		private string transID;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Base Associated Transaction Data Object class.
		/// </summary>
		public BaseAssociatedTxDO()
		{
		}
		#endregion

		#region Properties

		public bool Associated
		{
			set { associated = value; }
			get { return associated; }
		}

		public DateTimeOffset TransactionDate
		{
			set { transDate = value; }
			get { return transDate; }
		}

		public DateTime InventoryDate
		{
			set { inventoryDate = value; }
			get { return inventoryDate; }
		}

		public string TransactionAlias
		{
			set { alias = value; }
			get { return alias; }
		}

		public string Product
		{
			set { fuelType = value; }
			get { return fuelType; }
		}

		public string Supplier
		{
			set { supplier = value; }
			get { return supplier; }
		}

		public string Manager
		{
			set { manager = value; }
			get { return manager; }
		}

		public string Owner
		{
			set { owner = value; }
			get { return owner; }
		}

		public string ShipToID
		{
			set { shipTo = value; }
			get { return shipTo; }
		}

		public string BillToID
		{
			set { billTo = value; }
			get { return billTo; }
		}

		public string TransactionID
		{
			set { transID = value; }
			get { return transID; }
		}
		#endregion
	}
}
