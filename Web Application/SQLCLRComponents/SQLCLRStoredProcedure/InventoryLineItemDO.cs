/// <summary>
///   File name:  InventoryLineItemDO.cs
///   Purpose:	   The purpoase of this class is to contain one ledger line item.
///				   Since the ledger columns are configurable (present or not present),
///				   there is a collection of gross/net values for each alias that will
///				   be displayed on the ledger.  The gross/net values shall be summed
///				   up prior to loading this class.
///				   
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				   2000.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///				   
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///   Date:			   By:						   Reason:
///   ----------		--------------------	   ----------------------------------
///   2010-04-23     W.Gray						Removed Support for Error Indication
/// </summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;

[Serializable]
public class InventoryLineItemDO : BaseInventoryLineItemDO
{
   #region Attributes
   private Hashtable		quantityListHshTbl;
   private Hashtable		aliasTypeHshTbl;
   private string			inventoryDate;
   private string			site;
   private QuantityDO	beginInventory;
	private QuantityDO	bookInventory;
	private QuantityDO	variance;
	private QuantityDO	totalVariance;
	private QuantityDO	totalActivity;
	private QuantityDO	totalPhysicalInventory;
	private QuantityDO	totalMovement;
   private DateTime		originalInventoryDate;
   private bool			hasPhysicalInventory;
   private string			owner;
   private QuantityDO	toleranceTestedQuantity;
   private QuantityDO	variancePercentage;
   private double			tolerance;
   private QuantityDO	allowableGainLoss;
	private Int64			maxTransVersion;
   #endregion

   #region Constructor
   /// <summary>
   /// This is the default constructor for the Generic Inventory Line Item DO.
   /// </summary>
   public InventoryLineItemDO()
   {
      this.Initialize();
   }
   #endregion

   #region Properties
   /// <summary>
   /// This property sets and get the inventory date for a given ledger
   /// line item.
   /// </summary>
   public string InventoryDate
   {
      get { return inventoryDate; }
      set { inventoryDate = value; }
   }

   /// <summary>
   /// This property sets and gets the original date retrieved from the
   /// ledger processor.
   /// </summary>
   public DateTime OriginalInventoryDate
   {
      get { return this.originalInventoryDate; }
      set { this.originalInventoryDate = value; }
   }

   /// <summary>
   /// This property sets and gets the gross/net/mass quantity/price collection.
   /// </summary>
   public Hashtable QuantityList
   {
      get { return this.quantityListHshTbl; }
      set { this.quantityListHshTbl = value; }
   }

   /// <summary>
   /// This property sets and gets the alias transaction type.
   /// </summary>
   public Hashtable AliasTypeList
   {
      get { return this.aliasTypeHshTbl; }
      set { this.aliasTypeHshTbl = value; }
   }

   /// <summary>
   /// This property sets and gets the site value.
   /// </summary>
   public string Site
   {
      get { return this.site; }
      set { this.site = value; }
   }

   /// <summary>
   /// This property sets and gets the gross beginning inventory attribute.
   /// </summary>
	public QuantityDO BeginInventory
   {
      get { return this.beginInventory; }
      set { this.beginInventory = value; }
   }

   /// <summary>
   /// This property sets and gets the gross book inventory attribute.
   /// </summary>
	public QuantityDO BookInventory
   {
      get { return this.bookInventory; }
      set { this.bookInventory = value; }
   }

   /// <summary>
   /// Daily variance is the variance on a daily bases. It is the difference
   /// between the book inventory value and the measured physical inventory.
   /// </summary>
	public QuantityDO Variance
   {
      get { return this.variance; }
      set { this.variance = value; }
   }

   /// <summary>
   /// Total variance is the running total of all the daily variances.
   /// </summary>
	public QuantityDO TotalVariance
   {
      get { return this.totalVariance; }
      set { this.totalVariance = value; }
   }

   /// <summary>
   /// Total activity is the running total of all the daily activities.
   /// </summary>
	public QuantityDO TotalActivity
   {
      get { return this.totalActivity; }
      set { this.totalActivity = value; }
   }

   /// <summary>
   /// Total movement is the running total of all the issue type inventories.
   /// </summary>
	public QuantityDO TotalMovement
   {
      get { return this.totalMovement; }
      set { this.totalMovement = value; }
   }

   /// <summary>
   /// Total physical is the running total of all the physical inventory.
   /// </summary>
	public QuantityDO TotalPhysicalInventory
   {
      get { return this.totalPhysicalInventory; }
      set { this.totalPhysicalInventory = value; }
   }

   /// <summary>
   /// Returns true if the line item has a physical inventory. Otherwise,
   /// it returns false.
   /// </summary>
   public bool HasPhysicalInventory
   {
      get { return this.hasPhysicalInventory; }
      set { this.hasPhysicalInventory = value; }
   }

   /// <summary>
   /// Returns the Owner data member if populated, else
   /// it returns an empty string.
   /// </summary>
   public string Owner
   {
      get { return this.owner; }
      set
      {
         this.owner = value;

         if (this.owner == null)
         {
            this.owner = "";
         }
      }
   }
   public QuantityDO VariancePercentage
   {
       get { return variancePercentage; }
       set { variancePercentage = value; }
   }
   public double Tolerance
   {
       get { return tolerance; }
       set { tolerance = value; }
   }
   public QuantityDO AllowableGainLoss
   {
       get { return allowableGainLoss; }
       set { allowableGainLoss = value; }
   }
   public QuantityDO ToleranceTestedQuantity
   {
	   get { return toleranceTestedQuantity; }
	   set { toleranceTestedQuantity = value; }
   }

	/// <summary>
	/// This property sets and get the maxTransVersion
	/// line item.
	/// </summary>
	public Int64 MaxTransVersion
	{
		get { return maxTransVersion; }
		set { maxTransVersion = value; }
	}


   #endregion

   #region Public methods
   /// <summary>
   /// This method adds the gross, net, and mass quantities to the to the 
   /// quantity DO class, which is stored into a hashtable with the 
   /// alias name as a key. 
   /// </summary>
   /// <param name="gross"></param>
   /// <param name="net"></param>
	/// <param name="mass"></param>
	public void AddQuantity(string quantityAlias,double gross,double net,double mass)
   {
		this.quantityListHshTbl.Add(quantityAlias,new QuantityDO(gross,net,mass));
   }

   /// <summary>
   /// This method adds the gross, net, and mass quantities along with price to the 
   /// quantity DO class, which is stored into a hashtable with the 
   /// alias name as a key. 
   /// </summary>
   /// <param name="quantityAlias"></param>
   /// <param name="gross"></param>
   /// <param name="net"></param>
   /// <param name="price"></param>
   public void AddQuantity(string quantityAlias, double gross, double net, double mass, double package, double grossPrice, double netPrice, double massPrice)
   {
      this.quantityListHshTbl.Add(quantityAlias, new QuantityDO(gross, net, mass, package, grossPrice, netPrice, massPrice));
   }

   /// <summary>
   /// This method adds the gross, net, and mass quantities along with price and number fields to the 
   /// quantity DO class, which is stored into a hashtable with the alias name as a key.
   /// </summary>
   /// <param name="quantityAlias"></param>
   /// <param name="gross"></param>
   /// <param name="net"></param>
	/// <param name="mass"></param>
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
      QuantityDO newQuantity = new QuantityDO(gross,
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
      this.quantityListHshTbl.Add(quantityAlias, newQuantity);
   }

   /// <summary>
   /// This method will build a hash table of alias name keys and their corresponding
   /// transaction type. This hash table will be used in order to set the URL link in the 
   /// ledger grid.
   /// </summary>
   /// <param name="aliasName"></param>
   /// <param name="transType"></param>
   public void AddAliasType(string aliasName, TransactionAliases.TransactionTypes transType)
   {
      if (this.aliasTypeHshTbl == null)
      {
         this.aliasTypeHshTbl = new Hashtable();
      }

      if ((aliasName != null) && (aliasName.Length > 0) && (this.aliasTypeHshTbl.Contains(aliasName) == false))
      {
         this.aliasTypeHshTbl.Add(aliasName, transType);
      }
   }

   #endregion

   #region Private Methods

   /// <summary>
   /// This method initializes the current object.
   /// </summary>
   private void Initialize()
   {
      this.quantityListHshTbl     = new Hashtable();
      this.owner                  = "";
      this.inventoryDate          = "";
      this.beginInventory         = new QuantityDO();
		this.bookInventory          = new QuantityDO();
		this.variance               = new QuantityDO();
		this.totalVariance          = new QuantityDO();
		this.totalActivity          = new QuantityDO();
		this.TotalPhysicalInventory = new QuantityDO();
		this.totalMovement          = new QuantityDO();
      this.hasPhysicalInventory   = false;
		this.maxTransVersion			 = 0;

      this.variance.AffectsInventory               = false;
      this.totalVariance.AffectsInventory          = false;
      this.totalActivity.AffectsInventory          = false;
      this.totalPhysicalInventory.AffectsInventory = false;
      this.totalMovement.AffectsInventory          = false;

      this.variancePercentage = new QuantityDO();
      this.variancePercentage.AffectsInventory = false;
      this.tolerance = 0.0;
      this.allowableGainLoss = new QuantityDO();
      this.allowableGainLoss.AffectsInventory = false;
		this.toleranceTestedQuantity = new QuantityDO();
		this.toleranceTestedQuantity.AffectsInventory = false;
   }
   #endregion
}