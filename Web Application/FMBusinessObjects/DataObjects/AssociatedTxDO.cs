using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Summary description for AssociatedTxDO.
	/// </summary>
	[DataContract]
	[Serializable]
	public class AssociatedTxDO : BaseLineItemDO
	{
		DateTimeFormatInfo dtFormat = null;

		public AssociatedTxDO()
		{
			Init();
		}

		public AssociatedTxDO(DateTimeFormatInfo dateTimeFormat)
		{
			this.dtFormat = dateTimeFormat;

			Init();
		}

		protected void Init()
		{
			TransactionLineItemGuid = Guid.Empty;
		}

		public void Load(DataRow dr)
		{
			try
			{
				this.BillToID = DataObject.getValue<string>(dr["BillToID"], "");
				this.DocumentNumber = DataObject.getValue<string>(dr["DocumentNumber"], "");
				this.InventoryDateTime = DataObject.getValue<DateTime>(dr["InventoryDate"], DateTime.Today);
				this.TransactionLineItemGuid = DataObject.getValue<Guid>(dr["TransactionLineItemGuid"], Guid.Empty);
				this.Manager = DataObject.getValue<string>(dr["ManagerID"], "");
				this.Owner = DataObject.getValue<string>(dr["OwnerID"], "");
				this.PONumber = DataObject.getValue<string>(dr["PONumber"], "");
				this.ShipToID = DataObject.getValue<string>(dr["ShipToID"], "");
				this.SupplierID = DataObject.getValue<string>(dr["SupplierID"], "");
				this.TransactionDateTime = DataObject.getValue<DateTimeOffset>(dr["TransDateTime"], TimeConverter.Today());
				this.TransID = DataObject.getValue<string>(dr["TransID"], "");
				this.TransactionAlias = DataObject.getValue<string>(dr["AliasName"], "");
				this.Product = DataObject.getValue<string>(dr["Product"], "");
				this.GrossQuantity = DataObject.getValue<double>(dr["GrossQuantity"], 0.0);
				this.Excise = DataObject.getValue<double>(dr["Excise"], 0.0);
				this.GST = DataObject.getValue<double>(dr["GST"], 0.0);
				this.Markup = DataObject.getValue<double>(dr["Markup"], 0.0);
				this.DeliveryLocation = DataObject.getValue<string>(dr["DeliveryLocation"], "");
				this.Site = DataObject.getValue<string>(dr["Site"], "");
				this.TransTypeID = DataObject.getValue<TransactionTypes>(dr["LookupTransTypeIndex"], TransactionTypes.T_Maximum);
				this.TransStatus = DataObject.getValue<TransactionStatus>(dr["LookupTransactionStatusIndex"], TransactionStatus.InProgress);

				if (this.dtFormat != null)
				{
					this.InventoryDate = this.InventoryDateTime.ToString("d", dtFormat);
					this.TransactionDate = this.TransactionDateTime.ToString("d", dtFormat);
				}

				if (dr.Table != null && dr.Table.Columns != null)
				{
					if (dr.Table.Columns.Contains("ProductPrice"))
					{
						this.ProductPrice = DataObject.getValue<double>(dr["ProductPrice"], 0.0);
					}
					else
					{
						this.ProductPrice = 0.0;
					}

					if (dr.Table.Columns.Contains("LineItemStatus"))
					{
						this.LineItemStatus = DataObject.getValue<TransactionStatus>(dr["LineItemStatus"], TransactionStatus.InProgress);
					}
					else
					{
						this.LineItemStatus = TransactionStatus.InProgress;
					}

					if (dr.Table.Columns.Contains("Associated"))
					{
						this.Associated = DataObject.getValue<int>(dr["Associated"], 0);
					}
					else
					{
						this.Associated = 0;
					}

					if (dr.Table.Columns.Contains("LinkedTransactionLineItemGuid"))
					{
						this.LinkedTransactionLineItemGuid = DataObject.getValue<Guid>(dr["LinkedTransactionLineItemGuid"], Guid.Empty);
					}
					else
					{
						this.LinkedTransactionLineItemGuid = Guid.Empty;
					}

					if (dr.Table.Columns.Contains("CurrencyGuid") && !dr.IsNull("CurrencyGuid"))
					{
						this.CurrencyGuid = (Guid)dr["CurrencyGuid"];
					}
					else
					{
						this.CurrencyGuid = Guid.Empty;
					}

					if (dr.Table.Columns.Contains("LineItemRequestedDateTime") && !dr.IsNull("LineItemRequestedDateTime"))
					{
						this.LineItemRequestedDateTime = (DateTimeOffset)dr["LineItemRequestedDateTime"];
					}
					else
					{
						this.LineItemRequestedDateTime = null;
					}


					if (dr.Table.Columns.Contains("AlternativeNetVolume") && !dr.IsNull("AlternativeNetVolume"))
					{
						this.AlternativeNetVolume = new double?((double)dr["AlternativeNetVolume"]);
					}
					else
					{
						this.AlternativeNetVolume = null;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		[DataMember]
		public string TransID { get; set; }

		[DataMember]
		public TransactionStatus TransStatus { get; set; }

		[DataMember]
		public TransactionStatus LineItemStatus { get; set; }

		[DataMember]
		public double ProductPrice { get; set; }

		[DataMember]
		public TransactionTypes TransTypeID { get; set; }

		[DataMember]
		public Guid TransactionLineItemGuid { get; set; }

		[DataMember]
		public Guid LinkedTransactionLineItemGuid { get; set; }

		[DataMember]
		public string ShipToID { get; set; }

		[DataMember]
		public string SupplierID { get; set; }

		[DataMember]
		public string BillToID { get; set; }

		[DataMember]
		public string Manager { get; set; }

		[DataMember]
		public string Owner { get; set; }

		[DataMember]
		public DateTime InventoryDateTime { get; set; }

		[DataMember]
		public string InventoryDate { get; set; }

		[DataMember]
		public DateTimeOffset TransactionDateTime { get; set; }

		[DataMember]
		public string TransactionDate { get; set; }

		[DataMember]
		public string DocumentNumber { get; set; }

		[DataMember]
		public string PONumber { get; set; }

		[DataMember]
		public string TransactionAlias { get; set; }

		[DataMember]
		public string Product { get; set; }

		[DataMember]
		public double GrossQuantity { get; set; }

		[DataMember]
		public double GrossQuantityReceived { get; set; }

		[DataMember]
		public double Excise { get; set; }

		[DataMember]
		public double GST { get; set; }

		[DataMember]
		public double Markup { get; set; }

		[DataMember]
		public double TotalValue { get; set; }

		[DataMember]
		public double TotalPriceWithTax { get; set; }

		[DataMember]
		public int Associated { get; set; }

		[DataMember]
		public string DeliveryLocation { get; set; }

		[DataMember]
		public string Site { get; set; }

		[DataMember]
		public Guid CurrencyGuid { get; set; }

		[DataMember]
		public DateTimeOffset? LineItemRequestedDateTime { get; set; }

		[DataMember]
		public double? AlternativeNetVolume { get; set; }

		public override string getDeleteCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getSelectCommand()
		{
			return null;
		}

		public override string getUpdateCommand()
		{
			return null;
		}

	}
}
