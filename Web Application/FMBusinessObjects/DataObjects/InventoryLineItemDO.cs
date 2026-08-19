 #pragma warning disable 1587
/// <summary>
/// ******************************************************************************
/// FILE NAME:		InventoryLineItemDO.cs
/// PURPOSE:		Implementation of InventoryLineItemDO class
///
/// COMMENTS:
/// Copyright (C) Varec, Inc. Norcross, GA, USA, 2005
/// This file shall not be copied or reproduced in any form without
/// the express written consent of Varec.
///
/// AUTHOR(S):	I. Orndorff
///
/// VERSION:	1.0.0  Current version
/// 
/// MODIFICATION HISTORY:
///   Date:		   By:					Reason:
///   ----------	-----------------	-------------------------------------------
///	2007-03-26	Richard Panachida	Added the variance and total variance properties
///									      for inventory reconciliation (CSI 4077).
///	2007-10-03	Richard Panachida	Added the ability to handle pricing.
///	2007-11-23  E. Simmons			Added dtInventoryDate property to resolve CSI #5378
///									      This function could be potentially used to resolve
///									      CSI #5377.
///	2008-12-19  Richard Panachida Defect 784: Added a new data member called originalInventoryDate
///	                              to save the original format of the date (US format).
///	2009-08-25	W.Gray				Remove aliasTypeHshTbl as it is not used by the system
/// ******************************************************************************* 		
/// </summary>
#pragma warning restore 1587

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    [Serializable]
   [DataContract]
	[KnownType(typeof(QuantityDO))]
	[KnownType(typeof(EngineeringUnits))]
	public class InventoryLineItemDO : BaseLineItemDO
	{
		#region Attributes
		[DataMember]
		private Dictionary<string, QuantityDO> quantityListHshTbl;
		[DataMember]
		private string inventoryDate;
		[DataMember]
		private string site;
		[DataMember]
		private QuantityDO beginInventory;
		[DataMember]
		private QuantityDO bookInventory;
		[DataMember]
		private QuantityDO variance;
		[DataMember]
		private QuantityDO totalVariance;
		[DataMember]
		private QuantityDO totalActivity;
		[DataMember]
		private QuantityDO totalPhysicalInventory;
		[DataMember]
		private QuantityDO totalMovement;
		[DataMember]
		private DateTime originalInventoryDate;
		[DataMember]
		private bool isSIUnits;
		[DataMember]
		private bool hasPhysicalInventory;
		[DataMember]
		private EngineeringUnit? convEngUnits;
		[DataMember]
		private QuantityDO variancePercentage;
		[DataMember]
		private double tolerance;
		[DataMember]
		private QuantityDO allowableGainLoss;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the Generic Inventory Line Item DO.
		/// </summary>
		public InventoryLineItemDO()
		{
			this.Initialize(null);
		}

		/// <summary>
		/// This constructor sets the convert engineering units object.
		/// </summary>
		/// <param name="convEngUnits"></param>
		public InventoryLineItemDO(EngineeringUnit? convEngUnits)
		{
			this.Initialize(convEngUnits);
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and get the inventory date for a given ledger
		/// line item.
		/// </summary>
		public string InventoryDate
		{
			get { return this.inventoryDate; }
			set { this.inventoryDate = value; }
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

		//Eric Simmons - 11/23/2007 @ 1:30 PM
		//Added to support CSI #5378
		public DateTime DtInventoryDate
		{
			get { return DateEfficacy.convertMonthDayYearToDateTime(this.inventoryDate).Date; }
			set { this.inventoryDate = DateEfficacy.convertToMonthDayYear(value); }
		}

		/// <summary>
		/// This property sets and gets the gross/net volume/mass/package/price collection.
		/// </summary>
		public Dictionary<string, QuantityDO> QuantityList
		{
			get { return this.quantityListHshTbl; }
			set { this.quantityListHshTbl = value; }
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
		/// This property sets the convert engineering units attribute.
		/// </summary>
		public EngineeringUnit ConvertEngineeringUnits
		{
			get { return this.convEngUnits.Value; }
			set { this.convEngUnits = value; }
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

		public QuantityDO VariancePercentage
		{
			get { return this.variancePercentage; }
			set { this.variancePercentage = value; }
		}

		public double Tolerance
		{
			get { return this.tolerance; }
			set { this.tolerance = value; }
		}

		public QuantityDO AllowableGainLoss
		{
			get { return this.allowableGainLoss; }
			set { this.allowableGainLoss = value; }
		}
		#endregion

		#region Public override Methods
		override public string getSelectCommand()
		{
			return "";
		}
		override public string getInsertCommand()
		{
			return "";
		}
		override public string getUpdateCommand()
		{
			return "";
		}
		override public string getDeleteCommand()
		{
			return "";
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method adds the gross and net volumes to the to the 
		/// volume DO class, which is stored into a hashtable with the 
		/// alias name as a key. 
		/// </summary>
		/// <param name="volumeType"></param>
		/// <param name="gross"></param>
		/// <param name="net"></param>
		/// <param name="mass"></param>
		/// <param name="package"></param>
		public void AddQuantity(string quantityAlias, double gross, double net, double mass, double package)
		{
			double grossOut = this.Convert(gross);
			double netOut = this.Convert(net);
			double massOut = this.Convert(mass);
			double packageOut = this.Convert(package);
			this.quantityListHshTbl.Add(quantityAlias, new QuantityDO(grossOut, netOut, massOut, packageOut));
		}

		/// <summary>
		/// This method adds the gross and net volumes along with price to the 
		/// volume DO class, which is stored into a hashtable with the 
		/// alias name as a key. 
		/// </summary>
		/// <param name="volumeAlias"></param>
		/// <param name="gross"></param>
		/// <param name="net"></param>
		/// <param name="mass"></param>
		/// <param name="package"></param>
		/// <param name="grossPrice"></param>
		/// <param name="netPrice"></param>
		/// <param name="massPrice"></param>
		public void AddQuantity(string quantityAlias, double gross, double net, double mass, double package, double grossPrice, double netPrice, double massPrice)
		{
			double grossOut = this.Convert(gross);
			double netOut = this.Convert(net);
			double massOut = this.Convert(mass);
			double packageOut = this.Convert(package);
			double grossPriceOut = this.ConvertCurrency(grossPrice);
			double netPriceOut = this.ConvertCurrency(netPrice);
			double massPriceOut = this.ConvertCurrency(massPrice);
			this.quantityListHshTbl.Add(quantityAlias, new QuantityDO(grossOut, netOut, massOut, packageOut, grossPriceOut, netPriceOut, massPriceOut));
		}

		public void Initialize(EngineeringUnit? convEngUnits)
		{
			this.quantityListHshTbl = new Dictionary<string, QuantityDO>(StringComparer.InvariantCultureIgnoreCase);
			this.convEngUnits = convEngUnits;
			this.isSIUnits = true;
			this.inventoryDate = "";
			this.beginInventory = new QuantityDO();
			this.bookInventory = new QuantityDO();
			this.variance = new QuantityDO();
			this.totalVariance = new QuantityDO();
			this.totalActivity = new QuantityDO();
			this.TotalPhysicalInventory = new QuantityDO();
			this.totalMovement = new QuantityDO();
			this.hasPhysicalInventory = false;
			this.tolerance = 0.0;

			this.variance.AffectsInventory = false;
			this.totalVariance.AffectsInventory = false;
			this.totalActivity.AffectsInventory = false;
			this.totalPhysicalInventory.AffectsInventory = false;
			this.totalMovement.AffectsInventory = false;

			this.variancePercentage = new QuantityDO
			{
				AffectsInventory = false
			};
			this.allowableGainLoss = new QuantityDO
			{
				AffectsInventory = false
			};
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will convert the currency to a different unit.
		/// </summary>
		/// <param name="valueToConvert"></param>
		/// <returns></returns>
		private double ConvertCurrency(double valueToConvert)
		{
			// Future for currency conversion...
			return valueToConvert;
		}

		/// <summary>
		/// This method will convert the incoming value to the appropriate
		/// engineering units and return the new value.
		/// </summary>
		/// <param name="valueToConvert"></param>
		/// <returns></returns>
		private double Convert(double valueToConvert)
		{
			double convertedValue = valueToConvert;

			if (this.convEngUnits != null)
			{
				this.isSIUnits = this.isSIUnits != true;
			}

			return convertedValue;
		}
		#endregion
	}
}
