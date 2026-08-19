// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TFMSDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TFMSDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	/// <summary>
	/// The TFMS data object.
	/// </summary>
	[Serializable]
	[DataContract]
	public class TFMSDO
	{
		#region Private data members
		[DataMember] private string siteNumber;
		[DataMember] private string location;
		[DataMember] private string purchaseDate;
		[DataMember] private string product;
		[DataMember] private string purchaseNumber;
		[DataMember] private string supplier;
		[DataMember] private string customer;
		[DataMember] private string country;
		[DataMember] private string foreignCurrencyUnit;
		[DataMember] private string defenseAssetId;
		[DataMember] private string notes;
		[DataMember] private string uom;
		[DataMember] private double? uomQuantity;
		[DataMember] private double? quantity;
		[DataMember] private double? fuelPriceAud;
		[DataMember] private double? totalPriceAud;
		[DataMember] private double? excise;
		[DataMember] private double? gst;
		[DataMember] private double? foreignCurrencyPrice;
		[DataMember] private double? totalForeignCurrencyPrice;
		[DataMember] private DateTimeOffset? purchaseDateTime;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="TFMSDO"/> class.
		/// </summary>
		public TFMSDO()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will set and get the defense asset ID data member.
		/// </summary>
		public string DefenseAssetID
		{
			get { return this.defenseAssetId; }
			set { this.defenseAssetId = value; }
		}

		/// <summary>
		/// This property will set and get the Notes data member.
		/// </summary>
		public string Notes
		{
			get { return this.notes; }
			set { this.notes = value; }
		}

		/// <summary>
		/// This property will set and get the UOM data member.
		/// </summary>
		public string UOM
		{
			get { return this.uom; }
			set { this.uom = value; }
		}

		/// <summary>
		/// This property will set and get the UOM Quantity data member.
		/// </summary>
		public double? UOMQuantity
		{
			get { return this.uomQuantity; }
			set { this.uomQuantity = value; }
		}

		/// <summary>
		/// This property will set and get the site number data member.
		/// </summary>
		public string SiteNumber
		{
			get { return this.siteNumber; }
			set { this.siteNumber = value; }
		}

		/// <summary>
		/// This property will set and get the location data member.
		/// </summary>
		public string Location
		{
			get { return this.location; }
			set { this.location = value; }
		}

		/// <summary>
		/// This property will set and get the purchaseDate data member
		/// </summary>
		public string PurchaseDate
		{
			get { return this.purchaseDate; }
			set { this.purchaseDate = value; }
		}

		/// <summary>
		/// This property will set and get the purchaseDateTime data member
		/// </summary>
		public DateTimeOffset? PurchaseDateTime
		{
			get { return this.purchaseDateTime; }
			set { this.purchaseDateTime = value; }
		}

		/// <summary>
		/// This property will set and get the direct fuel purchase number data member.
		/// </summary>
		public string PurchaseNumber
		{
			get { return this.purchaseNumber; }
			set { this.purchaseNumber = value; }
		}

		/// <summary>
		/// This property will set and get the customer data member.
		/// </summary>
		public string Customer
		{
			get { return this.customer; }
			set { this.customer = value; }
		}

		/// <summary>
		/// This property will set and get the supplier data member.
		/// </summary>
		public string Supplier
		{
			get { return this.supplier; }
			set { this.supplier = value; }
		}

		/// <summary>
		/// This property will set and get the country data member.
		/// </summary>
		public string Country
		{
			get { return this.country; }
			set { this.country = value; }
		}

		/// <summary>
		/// This property will set and get the product data member.
		/// </summary>
		public string Product
		{
			get { return this.product; }
			set { this.product = value; }
		}

		/// <summary>
		/// This property will set and get the quantity data member.
		/// </summary>
		public double? Quantity
		{
			get { return this.quantity; }
			set { this.quantity = value; }
		}

		/// <summary>
		/// This property will set and get the fuel price AUD data member.
		/// </summary>
		public double? FuelPriceAUD
		{
			get { return this.fuelPriceAud; }
			set { this.fuelPriceAud = value; }
		}

		/// <summary>
		/// This property will set and get the total fuel price AUD data member.
		/// </summary>
		public double? TotalPriceAUD
		{
			get { return this.totalPriceAud; }
			set { this.totalPriceAud = value; }
		}

		/// <summary>
		/// This property will set and get the excise tax data member.
		/// </summary>
		public double? Excise
		{
			get { return this.excise; }
			set { this.excise = value; }
		}

		/// <summary>
		/// This property will set and get the GST data member.
		/// </summary>
		public double? GST
		{
			get { return this.gst; }
			set { this.gst = value; }
		}

		/// <summary>
		/// This property will set and get the foreign currency price data member.
		/// </summary>
		public double? ForeignCurrencyPrice
		{
			get { return this.foreignCurrencyPrice; }
			set { this.foreignCurrencyPrice = value; }
		}

		/// <summary>
		/// This property will set and get the total foreign currency price data member.
		/// </summary>
		public double? TotalForeignCurrencyPrice
		{
			get { return this.totalForeignCurrencyPrice; }
			set { this.totalForeignCurrencyPrice = value; }
		}

		/// <summary>
		/// This property will set and get the foreign currency units data member.
		/// </summary>
		public string ForeignCurrencyUnit
		{
			get { return this.foreignCurrencyUnit; }
			set { this.foreignCurrencyUnit = value; }
		}

		public string FuelCardNumber
		{
			get;
			set;
		}
		#endregion

		#region SQL methods
		/// <summary>
		/// Retrieves all the configured Excise Taxes from the database
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="purchaseNum">
		/// The purchase Number.
		/// </param>
		public void GetPurchaseNumberCount(SqlCommand cmd, string purchaseNum)
		{
			cmd.CommandText =	"SELECT COUNT(PONumber) AS PurchaseNumberCount " +
								"FROM tblTransactions " +
								"WHERE PONumber = @PurchaseNumber AND LookupTransTypeIndex = 12";

			cmd.Parameters.Add("@PurchaseNumber", SqlDbType.NVarChar, 50);
			cmd.Parameters[0].Value = purchaseNum;

			// Have to convert any null values to DbNull
			foreach (SqlParameter parm in cmd.Parameters)
			{
				if (parm.Value == null)
				{
					parm.Value = DBNull.Value;
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the TFMS data object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.location					= string.Empty;
			this.purchaseNumber				= string.Empty;
			this.purchaseDate				= string.Empty;
			this.product					= string.Empty;
			this.supplier					= string.Empty;
			this.customer					= string.Empty;
			this.country					= string.Empty;
			this.foreignCurrencyUnit		= string.Empty;
			this.defenseAssetId				= string.Empty;
			this.notes						= string.Empty;
			this.uom						= string.Empty;
			this.uomQuantity				= null;
			this.quantity					= null;
			this.fuelPriceAud				= null;
			this.totalPriceAud				= null;
			this.excise						= null;
			this.gst						= null;
			this.foreignCurrencyPrice		= null;
			this.totalForeignCurrencyPrice	= null;
			this.purchaseDateTime			= null;
			this.FuelCardNumber				= null;
		}
		#endregion
	}
}
