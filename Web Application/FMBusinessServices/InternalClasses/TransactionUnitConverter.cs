 #pragma warning disable 1587
///***************************************************************************
/// Module Name:  TransactionUnitConverter.cs
/// Author:       
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
#pragma warning restore 1587

namespace FMBusinessServices.InternalClasses
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.ServiceClasses;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// Converts values in a transaction record to and from standard values (e.g. UTC time, SI).
	/// For the most part, we rely on the unit values provided for the line items or sub line items
	/// However, in some cases, we must use the values defined for the transaction alias or site associated with the transaction record.
	/// Sometimes the site or transaction alias units are present on the transaction header, but if they're not, this class will get them for you
	/// if it needs them
	/// </summary>
	public class TransactionUnitConverter
	{
		#region Constants and Fields

		/// <summary>
		/// Contains security information. Used to get the site and transaction alias records if we have to.
		/// </summary>
		private SecurityClass Security;

		/// <summary>
		/// Identifies the site we're processing the transaction for.
		/// We may get the site record to retrieve the units defined for the site
		/// </summary>
		private Guid SiteGuid = Guid.Empty;

		/// <summary>
		/// Identifies the transaction alias associated with the transaction we're converting.
		/// We may get the transaction alias record to retrieve the units defined for the transaction alias
		/// </summary>
		private Guid TransactionAliasGuid = Guid.Empty;

		private SiteClass site = null;

		/// <summary>
		/// The site we're processing the transaction for. We only load it if we need to.
		/// </summary>
		private SiteClass Site
		{
			get
			{
				if (this.site == null)
				{
					SitesClass sites = new SitesClass();
					this.site = sites.Get(this.Security, this.SiteGuid, false, false, false);

					if (this.site == null || this.site.IdentityGuid == Guid.Empty)
					{
						throw new Exception("TransactionUnitConverter tried to get the site to get its units or time zone but could not find a site matching the guid " + this.SiteGuid);
					}
				}

				return this.site;
			}
		}

		private TransactionAliasClass transactionAlias = null;

		/// <summary>
		/// The transaction alias associated with the transaction. We only load it if we need to.
		/// </summary>
		private TransactionAliasClass TransactionAlias
		{
			get
			{
				if (this.transactionAlias == null && this.TransactionAliasGuid != Guid.Empty)
				{
					TransactionAliasesClass aliases = new TransactionAliasesClass();
					this.transactionAlias = aliases.Get(this.Security, this.TransactionAliasGuid, false);
				}

				return this.transactionAlias;
			}
		}

		#endregion

		#region Constructors and Destructors

		public TransactionUnitConverter(SecurityClass security, Guid siteGuid)
		{
			this.Security = security;
			this.SiteGuid = siteGuid;
		}

		#endregion

		#region Enums

		public enum ConversionDirections
		{
			ToSI, 
			FromSI
		}

		#endregion

		#region Public Methods and Operators

		public void ConvertFromSI(TransactionDO transaction)
		{
			this.ConvertTransaction(transaction, ConversionDirections.FromSI);
		}

		public void ConvertToSI(TransactionDO transaction)
		{
			this.ConvertTransaction(transaction, ConversionDirections.ToSI);
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method will examine a unit value on the transaction header, which should be populated with 
		/// either the units defined for the transaction alias or the site. If the value on the header is not set,
		/// this method will load the site and alias if they are not already available and determine which units to use.
		/// </summary>
		/// <param name="transactionUnitsValue">The value for the unit from the transaction header. E.g., transaction.MassUnits</param>
		/// <param name="unitType">The type of unit we're looking for</param>
		/// <returns>If present, the header's unit value. Otherwise, the transaction alias units if they are defined, or the site's units.</returns>
		private EngineeringUnit GetTransactionAliasOrSiteUnits(EngineeringUnit transactionUnitsValue, SITE_VARIABLE_TYPE unitType)
		{
			if (transactionUnitsValue != 0)
			{
				return transactionUnitsValue;
			}
			else
			{
				if (this.TransactionAlias != null)
				{
					EngineeringUnit aliasUnits = this.TransactionAlias.GetUnits(unitType);

					if (aliasUnits != 0)
					{
						return aliasUnits;
					}
				}

				EngineeringUnit siteUnits = this.Site.GetSiteUnits(unitType);

				return siteUnits;
			}
		}

        /// <summary>
        /// This method will examine a unit value on the transaction header, which should be populated with 
        /// either the units defined for the transaction alias or the site. If the value on the header is not set,
        /// this method will load the site and alias if they are not already available and determine which units to use.
        /// </summary>
        /// <param name="transactionUnitsValue">The value for the unit from the transaction header. E.g., transaction.MassUnits</param>
        /// <param name="transactionDecimalPlaces"></param>
        /// <param name="unitType">The type of unit we're looking for</param>
        /// <returns>If present, the header's unit value. Otherwise, the transaction alias units if they are defined, or the site's units.</returns>
        private byte GetTransactionAliasOrSiteDecimalPlaces(EngineeringUnit transactionUnitsValue, byte transactionDecimalPlaces, SITE_VARIABLE_TYPE unitType)
		{
			if (transactionUnitsValue != 0)
			{
				return transactionDecimalPlaces;
			}
			else
			{
				if (this.TransactionAlias != null)
				{
					EngineeringUnit aliasUnits = this.TransactionAlias.GetUnits(unitType);

					if (aliasUnits != 0)
					{
						return this.TransactionAlias.GetDecimalPlaces(unitType);
					}
				}

				return this.Site.GetSiteDecimalPlaces(unitType);
			}
		}


		private void ConvertTransaction(TransactionDO transaction, ConversionDirections direction)
		{
			this.TransactionAliasGuid = transaction.TransactionAliasGuid;

			foreach (WeightReadingDO reading in transaction.WeightReadings)
			{
				// Since weight readings don't have a product, 
				// Use the mass units that are defined for either the transaction alias or site, 
				// which should be present on the transaction header. If they aren't present, we'll get them for you.
				reading.BeginQuantity = this.ConvertUnit(reading.BeginQuantity, direction,
					this.GetTransactionAliasOrSiteUnits(transaction.MassUnits, SITE_VARIABLE_TYPE.MASS),
					GetTransactionAliasOrSiteDecimalPlaces(transaction.MassUnits, transaction.MassDecimalPlaces, SITE_VARIABLE_TYPE.MASS));

				reading.RequestedQuantity = this.ConvertUnit(reading.RequestedQuantity, direction,
					this.GetTransactionAliasOrSiteUnits(transaction.MassUnits, SITE_VARIABLE_TYPE.MASS),
					GetTransactionAliasOrSiteDecimalPlaces(transaction.MassUnits, transaction.MassDecimalPlaces, SITE_VARIABLE_TYPE.MASS));

				reading.FinalQuantity = this.ConvertUnit(reading.FinalQuantity, direction,
					this.GetTransactionAliasOrSiteUnits(transaction.MassUnits, SITE_VARIABLE_TYPE.MASS),
					GetTransactionAliasOrSiteDecimalPlaces(transaction.MassUnits, transaction.MassDecimalPlaces, SITE_VARIABLE_TYPE.MASS));
			}

			foreach (LineItemDO lineItem in transaction.LineItems)
			{
				lineItem.BottomVolume = this.ConvertUnit(lineItem.BottomVolume, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);

				try
				{
					lineItem.Density = this.ConvertUnit(lineItem.Density, direction, lineItem.DensityUnits, lineItem.DensityDecimalPlaces);
				}
				catch
				{
					lineItem.Density = 0.0;
				}

				lineItem.DifferentialPressure = this.ConvertUnit(lineItem.DifferentialPressure, direction, lineItem.PressureUnits, lineItem.PressureDecimalPlaces);
				lineItem.FreezePoint = this.ConvertUnit(lineItem.FreezePoint, direction, lineItem.TemperatureUnits, lineItem.TemperatureDecimalPlaces);
				lineItem.LineFill = this.ConvertUnit(lineItem.LineFill, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
				lineItem.LoadRackVariance = this.ConvertUnit(lineItem.LoadRackVariance, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
				lineItem.NetCapacity = this.ConvertUnit(lineItem.NetCapacity, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
				lineItem.ReceiptVariance = this.ConvertUnit(lineItem.ReceiptVariance, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
				lineItem.Temperature = this.ConvertUnit(lineItem.Temperature, direction, lineItem.TemperatureUnits, lineItem.TemperatureDecimalPlaces);
				lineItem.Variance = this.ConvertUnit(lineItem.Variance, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
				lineItem.Pressure = this.ConvertUnit(lineItem.Pressure, direction, lineItem.PressureUnits, lineItem.PressureDecimalPlaces);

				// For Order type transactions, honor the Engineering Units on the line items.
				if ((transaction.TransTypeID == TransactionTypes.T17_Order) || (transaction.TransTypeID == TransactionTypes.T18_SupplyOrder))
				{
					this.ConvertOrderItems(transaction, lineItem, direction);
				}
				else if (lineItem.Quantity != null)
				{
					lineItem.Quantity.GrossInventoryChange = this.ConvertUnit(lineItem.Quantity.GrossInventoryChange, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
					lineItem.Quantity.DeliveredGrossInventoryChange = this.ConvertUnit(lineItem.Quantity.DeliveredGrossInventoryChange, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
					lineItem.Quantity.NetInventoryChange = this.ConvertUnit(lineItem.Quantity.NetInventoryChange, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
					lineItem.Quantity.DeliveredNetInventoryChange = this.ConvertUnit(lineItem.Quantity.DeliveredNetInventoryChange, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
					lineItem.Quantity.MassInventoryChange = this.ConvertUnit(lineItem.Quantity.MassInventoryChange, direction, lineItem.MassUnits, lineItem.MassDecimalPlaces);
				}

				lineItem.PresetAmount = this.ConvertUnit(lineItem.PresetAmount, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);

				lineItem.CleanLineDeductQuantity = this.ConvertUnit(lineItem.CleanLineDeductQuantity, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
				lineItem.CleanLinePackQuantity = this.ConvertUnit(lineItem.CleanLinePackQuantity, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
				

				if (lineItem.SubLineItems != null)
				{
					foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
					{
						subLineItem.BottomVolume = this.ConvertUnit(subLineItem.BottomVolume, direction, subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces);

						try
						{
							subLineItem.Density = this.ConvertUnit(subLineItem.Density, direction, subLineItem.DensityUnits, subLineItem.DensityDecimalPlaces);
						}
						catch
						{
							subLineItem.Density = 0.0;
						}

						subLineItem.DifferentialPressure = this.ConvertUnit(subLineItem.DifferentialPressure, direction, subLineItem.PressureUnits, subLineItem.PressureDecimalPlaces);
						subLineItem.FreezePoint = this.ConvertUnit(subLineItem.FreezePoint, direction, subLineItem.TemperatureUnits, subLineItem.TemperatureDecimalPlaces);
						subLineItem.LineFill = this.ConvertUnit(subLineItem.LineFill, direction, subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces);
						subLineItem.NetCapacity = this.ConvertUnit(subLineItem.NetCapacity, direction, subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces);
						subLineItem.Temperature = this.ConvertUnit(subLineItem.Temperature, direction, subLineItem.TemperatureUnits, subLineItem.TemperatureDecimalPlaces);
						subLineItem.Pressure = this.ConvertUnit(subLineItem.Pressure, direction, subLineItem.PressureUnits, subLineItem.PressureDecimalPlaces);

						if (subLineItem.Quantity != null)
						{
							subLineItem.Quantity.GrossInventoryChange = this.ConvertUnit(subLineItem.Quantity.GrossInventoryChange, direction, subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces);
							subLineItem.Quantity.DeliveredGrossInventoryChange = this.ConvertUnit(subLineItem.Quantity.DeliveredGrossInventoryChange, direction, subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces);
							subLineItem.Quantity.NetInventoryChange = this.ConvertUnit(subLineItem.Quantity.NetInventoryChange, direction, subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces);
							subLineItem.Quantity.DeliveredNetInventoryChange = this.ConvertUnit(subLineItem.Quantity.DeliveredNetInventoryChange, direction, subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces);
							subLineItem.Quantity.MassInventoryChange = this.ConvertUnit(subLineItem.Quantity.MassInventoryChange, direction, subLineItem.MassUnits, subLineItem.MassDecimalPlaces);
						}

						subLineItem.PresetAmount = this.ConvertUnit(subLineItem.PresetAmount, direction, subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces);


						subLineItem.CleanLineDeductQuantity = this.ConvertUnit(subLineItem.CleanLineDeductQuantity, direction, subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces);
						subLineItem.CleanLinePackQuantity = this.ConvertUnit(subLineItem.CleanLinePackQuantity, direction, subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces);
					}
				}
			}
		}

		private void ConvertOrderItems(TransactionDO transaction, LineItemDO lineItem, ConversionDirections direction)
		{
			lineItem.Quantity.NetInventoryChange = this.ConvertUnit(lineItem.Quantity.NetInventoryChange, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
			lineItem.NetQuantityReceived = this.ConvertUnit(lineItem.NetQuantityReceived, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
			lineItem.NetQuantityRemaining = this.ConvertUnit(lineItem.NetQuantityRemaining, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
			lineItem.Quantity.GrossInventoryChange = this.ConvertUnit(lineItem.Quantity.GrossInventoryChange, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
			lineItem.GrossQuantityReceived = this.ConvertUnit(lineItem.GrossQuantityReceived, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
			lineItem.GrossQuantityRemaining = this.ConvertUnit(lineItem.GrossQuantityRemaining, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
			lineItem.Quantity.MassInventoryChange = this.ConvertUnit(lineItem.Quantity.MassInventoryChange, direction, lineItem.MassUnits, lineItem.MassDecimalPlaces);
			lineItem.MassQuantityReceived = this.ConvertUnit(lineItem.MassQuantityReceived, direction, lineItem.MassUnits, lineItem.MassDecimalPlaces);
			lineItem.MassQuantityRemaining = this.ConvertUnit(lineItem.MassQuantityRemaining, direction, lineItem.MassUnits, lineItem.MassDecimalPlaces);

			if (TransactionTypes.T18_SupplyOrder == transaction.TransTypeID)
			{
				lineItem.TotalValue = this.ConvertUnit(lineItem.TotalValue, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
				lineItem.ValueRemaining = this.ConvertUnit(lineItem.ValueRemaining, direction, lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces);
			}
		}


		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ConvertUnit")]
		private double? ConvertUnit(double? inValue, ConversionDirections direction, EngineeringUnit unitType, byte decimalDigits)
		{
			if (inValue.HasValue)
			{
				return this.ConvertUnit(inValue.Value, direction, unitType, decimalDigits);
			}

			return null;
		}

		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ConvertUnit")]
		private double ConvertUnit(double inValue, ConversionDirections direction, EngineeringUnit unitType, byte decimalDigits)
		{
			SIDouble siDouble = new SIDouble();
			siDouble.Units = unitType;
			siDouble.numberDecimalDigits = decimalDigits;

			if (direction == ConversionDirections.FromSI)
			{
				siDouble.SIValue = inValue;
				return siDouble.Value;
			}
			else if (direction == ConversionDirections.ToSI)
			{
				siDouble.Value = inValue;
				return siDouble.SIValue;
			}
			else
			{
				throw new Exception("ConvertUnit() got unknown ConversionDirection");
			}
		}

		#endregion
	}
}