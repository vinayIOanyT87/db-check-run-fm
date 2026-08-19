namespace LedgerCore
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class LRInventoryLineItemDO : LRBaseInventoryLineItemDO
	{
		#region Attributes
		private DateTimeOffset originalInventoryDate;
		private string owner;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the Generic Inventory Line Item DO.
		/// </summary>
		public LRInventoryLineItemDO()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and get the inventory date for a given ledger
		/// line item.
		/// </summary>
		public string InventoryDate { get; set; }

		/// <summary>
		/// This property sets and gets the original date retrieved from the
		/// ledger processor.
		/// </summary>
		public DateTimeOffset OriginalInventoryDate
		{
			get { return this.originalInventoryDate; }
			set { this.originalInventoryDate = value; }
		}

		/// <summary>
		/// This property sets and gets the gross/net/mass quantity/price collection.
		/// </summary>
		public Dictionary<string, LRQuantityDO> QuantityList { get; set; }

		/// <summary>
		/// This property sets and gets the alias transaction type.
		/// </summary>
		public Hashtable AliasTypeList { get; set; }

		/// <summary>
		/// This property sets and gets the site value.
		/// </summary>
		public string Site { get; set; }

		/// <summary>
		/// This property sets and gets the gross beginning inventory attribute.
		/// </summary>
		public LRQuantityDO BeginInventory { get; set; }

		/// <summary>
		/// This property sets and gets the gross book inventory attribute.
		/// </summary>
		public LRQuantityDO BookInventory { get; set; }

		/// <summary>
		/// Daily variance is the variance on a daily bases. It is the difference
		/// between the book inventory value and the measured physical inventory.
		/// </summary>
		public LRQuantityDO Variance { get; set; }

		/// <summary>
		/// Total variance is the running total of all the daily variances.
		/// </summary>
		public LRQuantityDO TotalVariance { get; set; }

		/// <summary>
		/// Total activity is the running total of all the daily activities.
		/// </summary>
		public LRQuantityDO TotalActivity { get; set; }

		/// <summary>
		/// Total movement is the running total of all the issue type inventories.
		/// </summary>
		public LRQuantityDO TotalMovement { get; set; }

		/// <summary>
		/// Total physical is the running total of all the physical inventory.
		/// </summary>
		public LRQuantityDO TotalPhysicalInventory { get; set; }

		/// <summary>
		/// Returns true if the line item has a physical inventory. Otherwise,
		/// it returns false.
		/// </summary>
		public bool HasPhysicalInventory { get; set; }

		/// <summary>
		/// Returns the Owner data member if populated, else
		/// it returns an empty string.
		/// </summary>
		public string Owner
		{
			get { return this.owner; }
			set
			{
				this.owner = value ?? string.Empty;
			}
		}
		public LRQuantityDO VariancePercentage { get; set; }
		public double Tolerance { get; set; }
		public LRQuantityDO AllowableGainLoss { get; set; }
		public LRQuantityDO ToleranceTestedQuantity { get; set; }

		/// <summary>
		/// This property sets and get the maxTransVersion
		/// line item.
		/// </summary>
		public long MaxTransVersion { get; set; }
		#endregion

		#region Public methods
		/// <summary>
		/// This method adds the gross, net, and mass quantities to the to the 
		/// quantity DO class, which is stored into a hashtable with the 
		/// alias name as a key. 
		/// </summary>
		/// <param name="quantityAlias"></param>
		/// <param name="gross"></param>
		/// <param name="net"></param>
		/// <param name="mass"></param>
		public void AddQuantity(string quantityAlias, double gross, double net, double mass)
		{
			this.QuantityList.Add(quantityAlias, new LRQuantityDO(gross, net, mass));
		}

		/// <summary>
		/// This method adds the gross, net, and mass quantities along with price to the 
		/// quantity DO class, which is stored into a hashtable with the 
		/// alias name as a key. 
		/// </summary>
		/// <param name="quantityAlias"></param>
		/// <param name="gross"></param>
		/// <param name="net"></param>
		/// <param name="mass"></param>
		/// <param name="package"></param>
		/// <param name="grossPrice"></param>
		/// <param name="netPrice"></param>
		/// <param name="massPrice"></param>
		public void AddQuantity(string quantityAlias, 
								double gross, 
								double net, 
								double mass, 
								double package, 
								double grossPrice, 
								double netPrice, 
								double massPrice)
		{
			this.QuantityList.Add(quantityAlias, new LRQuantityDO(gross, net, mass, package, grossPrice, netPrice, massPrice));
		}

		/// <summary>
		/// This method adds the gross, net, and mass quantities along with price and number fields to the 
		/// quantity DO class, which is stored into a hashtable with the alias name as a key.
		/// </summary>
		/// <param name="quantityAlias"></param>
		/// <param name="gross"></param>
		/// <param name="net"></param>
		/// <param name="mass"></param>
		/// <param name="package"></param>
		/// <param name="grossPrice"></param>
		/// <param name="netPrice"></param>
		/// <param name="massPrice"></param>
		/// <param name="number01"></param>
		/// <param name="number02"></param>
		/// <param name="number03"></param>
		/// <param name="number04"></param>
		/// <param name="number05"></param>
		/// <param name="number06"></param>
		public void AddQuantity(string quantityAlias,
								double gross,
								double net,
								double mass,
								double package,
								double grossPrice,
								double netPrice,
								double massPrice,
								double number01,
								double number02,
								double number03,
								double number04,
								double number05,
								double number06)
		{
			var newQuantity = new LRQuantityDO(	gross,
												net,
												mass,
												package,
												grossPrice,
												netPrice,
												massPrice,
												number01,
												number02,
												number03,
												number04,
												number05,
												number06);
			this.QuantityList.Add(quantityAlias, newQuantity);
		}

		/// <summary>
		/// This method will build a hash table of alias name keys and their corresponding
		/// transaction type. This hash table will be used in order to set the URL link in the 
		/// ledger grid.
		/// </summary>
		/// <param name="aliasName"></param>
		/// <param name="transType"></param>
		public void AddAliasType(string aliasName, LRTransactionAliases.TransactionTypes transType)
		{
			if (this.AliasTypeList == null)
			{
				this.AliasTypeList = new Hashtable();
			}

			if (!string.IsNullOrEmpty(aliasName) && this.AliasTypeList.Contains(aliasName) == false)
			{
				this.AliasTypeList.Add(aliasName, transType);
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the current object.
		/// </summary>
		private void Initialize()
		{
			this.QuantityList		= new Dictionary<string, LRQuantityDO>(StringComparer.InvariantCultureIgnoreCase);
			this.owner					= string.Empty;
			this.InventoryDate			= string.Empty;
			this.BeginInventory			= new LRQuantityDO();
			this.BookInventory			= new LRQuantityDO();
			this.Variance				= new LRQuantityDO();
			this.TotalVariance			= new LRQuantityDO();
			this.TotalActivity			= new LRQuantityDO();
			this.TotalPhysicalInventory = new LRQuantityDO();
			this.TotalMovement			= new LRQuantityDO();
			this.HasPhysicalInventory	= false;
			this.MaxTransVersion		= 0;

			this.Variance.AffectsInventory					= false;
			this.TotalVariance.AffectsInventory				= false;
			this.TotalActivity.AffectsInventory				= false;
			this.TotalPhysicalInventory.AffectsInventory	= false;
			this.TotalMovement.AffectsInventory				= false;

			this.Tolerance = 0.0;
			this.VariancePercentage = new LRQuantityDO { AffectsInventory = false };
			this.AllowableGainLoss = new LRQuantityDO { AffectsInventory = false };
			this.ToleranceTestedQuantity = new LRQuantityDO { AffectsInventory = false };
		}
		#endregion
	}
}