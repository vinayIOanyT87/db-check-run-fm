using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class EquipmentTransactionLineItemDO : BaseLineItemDO
	{
		#region Protected data members
		[DataMember]
		protected string equipmentID;
		[DataMember]
		protected string meterStart;
		[DataMember]
		protected string meterStop;
		[DataMember]
		protected QuantityDO quantity;
		[DataMember]
		protected string transactionAlias;
		[DataMember]
		protected DateTimeOffset inventoryDate;
		[DataMember]
		protected string transID;
		[DataMember]
		protected string consumerID;
		[DataMember]
		protected string differential;
		[DataMember]
		protected string variance;
		[DataMember]
		protected string serialNumber;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the defautl constructor for the equipment transaction line item data object class.
		/// </summary>
		public EquipmentTransactionLineItemDO()
		{
		}
		#endregion

		#region Begin properties.

		public string EquipmentID
		{
			get { return this.equipmentID; }
			set { this.equipmentID = value; }
		}

		public string MeterStart
		{
			get { return this.meterStart; }
			set { this.meterStart = value; }
		}

		public string MeterStop
		{
			get { return this.meterStop; }
			set { this.meterStop = value; }
		}

		public QuantityDO Quantity
		{
			get { return this.quantity; }
			set { this.quantity = value; }
		}

		public string TransactionAlias
		{
			get { return this.transactionAlias; }
			set { this.transactionAlias = value; }
		}

		public DateTimeOffset InventoryDate
		{
			get { return this.inventoryDate; }
			set { this.inventoryDate = value; }
		}

		public string TransactionID
		{
			get { return this.transID; }
			set { this.transID = value; }
		}

		public string ConsumerID
		{
			get { return this.consumerID; }
			set { this.consumerID = value; }
		}

		public string Differential
		{
			get { return this.differential; }
			set { this.differential = value; }
		}

		public string Variance
		{
			get { return this.variance; }
			set { this.variance = value; }
		}

		public string SerialNumber
		{
			get { return this.serialNumber; }
			set { this.serialNumber = value; }
		}

		#endregion

		#region Public override methods
		public override string getSelectCommand()
		{
			string sql =
				"SELECT a.TransID, b.MeterStart, b.MeterStop, a.InventoryDate, " +
				"a.LookupTransTypeIndex, a.AliasName, a.ShipToID, a.PartialCloseout, " +
				"b.GrossQuantity, " +
				"b.DestinationRegistrationID, " +
				"b.SourceRegistrationID, " +
				"b.DestinationSerialNumber, " +
				"b.SourceSerialNumber " +
				"FROM tblTransactions a, tblTransactionLineItems b " +
				"WHERE a.Site = @SiteID AND " +
				"a.InventoryDate = @InventoryDate AND " +
				"a.LookupTransTypeIndex != 12 AND " +
				"b.TransactionGuid = a.TransactionGuid AND " +
				"((@EquipmentID = '') OR " +
				" (@EquipmentID IN (b.DestinationRegistrationID, " +
				"b.SourceRegistrationID))) " +
				"ORDER BY b.MeterStart";

			return sql;
		}

		public override string getDeleteCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion
	}
}
