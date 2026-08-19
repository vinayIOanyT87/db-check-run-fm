 #pragma warning disable 1587
/// <summary>
///	FILE NAME:	UnitsHelperClass.cs
///	PURPOSE:		This class returns engineering units based on site, transaction alias
///	and product information. If a ProductClass object is provided in its constructor,
///	it will return	units configured for that product in the product's configuration. 
///	Otherwise, if a transaction alias is provided in its constructor, it will
///	return the units configured for that transaction alias. Otherwise,
///	site units will be returned.
///
///	COMMENTS:
///		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
///		This file shall not be copied or reproduced in any form without
///		the express written consent of Endress+Hauser.
///
///	AUTHOR(S):	Thomas Beckum
///	VERSION:		1.0.0  Current version
///
///	MODIFICATION HISTORY:
///		Date:			By:					Reason:
///		---------	-----------------	-------------------------------------------
/// </summary>
#pragma warning restore 1587

namespace FMBusinessObjects.UtilityObjects
{
    using System.Collections;
    using System.Security;

    using DataObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    [SecuritySafeCritical]
	public class UnitsHelperClass
	{
		#region Attributes
		Hashtable _productsCache = new Hashtable();

		SecurityClass _security = null;

		protected EngineeringUnit _volumeUnits = 0;
		protected byte _volumeDecimalPlaces = 0;
		protected EngineeringUnit _additiveVolumeUnits = 0;
		protected byte _additiveVolumeDecimalPlaces = 0;
		protected EngineeringUnit _levelUnits = 0;
		protected byte _levelDecimalPlaces = 2;
		protected EngineeringUnit _temperatureUnits = 0;
		protected byte _temperatureDecimalPlaces = 0;
		protected EngineeringUnit _densityUnits = 0;
		protected byte _densityDecimalPlaces = 1;
		protected EngineeringUnit _massUnits = 0;
		protected byte _massDecimalPlaces = 0;
		protected EngineeringUnit _flowUnits = 0;
		protected byte _flowDecimalPlaces = 1;
		protected EngineeringUnit _pressureUnits = 0;
		protected byte _pressureDecimalPlaces = 2;
		protected SiteClass _site = null;
		protected TransactionAliasClass _alias = null;
		protected ProductClass _product = null;
		protected double _massPackageSize = 0.0;
		protected double _volumePackageSize = 0.0;

		#endregion Attributes

		public UnitsHelperClass(SecurityClass security, SiteClass site, TransactionAliasClass alias, ProductClass product)
		{
			_security = security;
			init(site, alias, product);

		}
		#region Properties
		public SiteClass Site
		{
			set
			{
				init(value, _alias, _product);
			}
			get
			{
				return _site;
			}
		}

		public TransactionAliasClass Alias
		{
			set
			{
				init(_site, value, _product);
			}

			get
			{
				return _alias;
			}
		}

		public ProductClass Product
		{
			set
			{
				init(_site, _alias, value);
			}
			get
			{
				return _product;
			}
		}


		public EngineeringUnit VolumeUnits { get { return _volumeUnits; } }
		public byte VolumeDecimalPlaces { get { return _volumeDecimalPlaces; } }
		public EngineeringUnit AdditiveVolumeUnits { get { return _additiveVolumeUnits; } }
		public byte AdditiveVolumeDecimalPlaces { get { return _additiveVolumeDecimalPlaces; } }
		public EngineeringUnit LevelUnits { get { return _levelUnits; } }
		public byte LevelDecimalPlaces { get { return _levelDecimalPlaces; } }
		public EngineeringUnit TemperatureUnits { get { return _temperatureUnits; } }
		public byte TemperatureDecimalPlaces { get { return _temperatureDecimalPlaces; } }
		public EngineeringUnit DensityUnits { get { return _densityUnits; } }
		public byte DensityDecimalPlaces { get { return _densityDecimalPlaces; } }
		public EngineeringUnit MassUnits { get { return _massUnits; } }
		public byte MassDecimalPlaces { get { return _massDecimalPlaces; } }
		public EngineeringUnit FlowUnits { get { return _flowUnits; } }
		public byte FlowDecimalPlaces { get { return _flowDecimalPlaces; } }
		public EngineeringUnit PressureUnits { get { return _pressureUnits; } }
		public byte PressureDecimalPlaces { get { return _pressureDecimalPlaces; } }
		public double VolumePackageSize { get { return _volumePackageSize; } }
		public double MassPackageSize { get { return _massPackageSize; } }

		#endregion Properties

		#region Public Methods
		/// <summary>
		/// Sets unit properties of TransactionDO object based on transaction alias
		/// and site.
		/// </summary>
		/// <param name="trans">TransactionDO object that will have its units set.</param>
		/// <param name="defaultProductType">If set to PRODUCT_TYPE.ADDITIVE_PRODUCT, then Additive Volume Units is used.</param>
		public void SetUnits(TransactionDO trans, ProductType defaultProductType)
		{
			//Set products to null since transactionDO units are based on transaction alias or site units.
			ProductClass tmpProduct = _product;
			//Setting product to null will force units and decimal places to be initialized with their new values
			//based on alias and site units.
			this.Product = null;
			if (defaultProductType == ProductType.AdditiveProduct)
			{
				trans.VolumeUnits = this.AdditiveVolumeUnits;
				trans.VolumeDecimalPlaces = this.AdditiveVolumeDecimalPlaces;
			}
			else
			{
				trans.VolumeUnits = this.VolumeUnits;
				trans.VolumeDecimalPlaces = this.VolumeDecimalPlaces;
			}
			trans.LevelUnits = this.LevelUnits;
			trans.LevelDecimalPlaces = this.LevelDecimalPlaces;
			trans.TemperatureUnits = this.TemperatureUnits;
			trans.TemperatureDecimalPlaces = this.TemperatureDecimalPlaces;
			trans.DensityUnits = this.DensityUnits;
			trans.DensityDecimalPlaces = this.DensityDecimalPlaces;
			trans.MassUnits = this.MassUnits;
			trans.MassDecimalPlaces = this.MassDecimalPlaces;
			trans.FlowUnits = this.FlowUnits;
			trans.FlowDecimalPlaces = this.FlowDecimalPlaces;
			trans.PressureUnits = this.PressureUnits;
			trans.PressureDecimalPlaces = this.PressureDecimalPlaces;
			//restore product.
			this.Product = tmpProduct;
		}
		/// <summary>
		/// Sets unit properties of LineItemDO object based on configured product units
		/// where product is specified by TransactionDO.ProductIndex. If product is null, then
		/// it is set to transaction alias units. If transaction alias is null, it is set to site units.
		/// </summary>
		/// <param name="lineItemDO">LineItemDO object that will have its units set.</param>
		/// <param name="defaultProductType">If set to PRODUCT_TYPE.ADDITIVE_PRODUCT, then Additive Volume Units is used.</param>
		public void SetUnits(LineItemDO lineItemDO, ProductType defaultProductType, ProductClass product)
		{
			this.Product = product;

			if (_product == null && defaultProductType == ProductType.AdditiveProduct)
			{
				lineItemDO.VolumeUnits = this.AdditiveVolumeUnits;
				lineItemDO.VolumeDecimalPlaces = this.AdditiveVolumeDecimalPlaces;
			}
			else
			{
				lineItemDO.VolumeUnits = this.VolumeUnits;
				lineItemDO.VolumeDecimalPlaces = this.VolumeDecimalPlaces;
			}
			lineItemDO.LevelUnits = this.LevelUnits;
			lineItemDO.LevelDecimalPlaces = this.LevelDecimalPlaces;
			lineItemDO.TemperatureUnits = this.TemperatureUnits;
			lineItemDO.TemperatureDecimalPlaces = this.TemperatureDecimalPlaces;
			lineItemDO.DensityUnits = this.DensityUnits;
			lineItemDO.DensityDecimalPlaces = this.DensityDecimalPlaces;
			lineItemDO.MassUnits = this.MassUnits;
			lineItemDO.MassDecimalPlaces = this.MassDecimalPlaces;
			lineItemDO.FlowUnits = this.FlowUnits;
			lineItemDO.FlowDecimalPlaces = this.FlowDecimalPlaces;
			lineItemDO.PressureUnits = this.PressureUnits;
			lineItemDO.PressureDecimalPlaces = this.PressureDecimalPlaces;
			lineItemDO.MassPackageSize = this.MassPackageSize;
			lineItemDO.VolumePackageSize = this.VolumePackageSize;
		}
		/// <summary>
		/// Sets unit properties of SubLineItemDO object based on configured product units
		/// where product is specified by TransactionDO.ProductIndex. If product is null, then
		/// it is set to transaction alias units. If transaction alias is null, it is set to site units.
		/// </summary>
		/// <param name="subLineItemDO">SubLineItemDO object that will have its units set.</param>
		/// <param name="defaultProductType">If set to PRODUCT_TYPE.ADDITIVE_PRODUCT, then Additive Volume Units is used.</param>
		public void SetUnits(SubLineItemDO subLineItemDO, ProductType defaultProductType, ProductClass product)
		{
			Product = product;

			if (_product == null && defaultProductType == ProductType.AdditiveProduct)
			{
				subLineItemDO.VolumeUnits = this.AdditiveVolumeUnits;
				subLineItemDO.VolumeDecimalPlaces = this.AdditiveVolumeDecimalPlaces;
			}
			else
			{
				subLineItemDO.VolumeUnits = this.VolumeUnits;
				subLineItemDO.VolumeDecimalPlaces = this.VolumeDecimalPlaces;
			}
			subLineItemDO.LevelUnits = this.LevelUnits;
			subLineItemDO.LevelDecimalPlaces = this.LevelDecimalPlaces;
			subLineItemDO.TemperatureUnits = this.TemperatureUnits;
			subLineItemDO.TemperatureDecimalPlaces = this.TemperatureDecimalPlaces;
			subLineItemDO.DensityUnits = this.DensityUnits;
			subLineItemDO.DensityDecimalPlaces = this.DensityDecimalPlaces;
			subLineItemDO.MassUnits = this.MassUnits;
			subLineItemDO.MassDecimalPlaces = this.MassDecimalPlaces;
			subLineItemDO.FlowUnits = this.FlowUnits;
			subLineItemDO.FlowDecimalPlaces = this.FlowDecimalPlaces;
			subLineItemDO.PressureUnits = this.PressureUnits;
			subLineItemDO.PressureDecimalPlaces = this.PressureDecimalPlaces;
			subLineItemDO.MassPackageSize = this.MassPackageSize;
			subLineItemDO.VolumePackageSize = this.VolumePackageSize;
		}
		#endregion Public Methods

		#region Protected Methods

		protected void ResetUnits()
		{
			_additiveVolumeUnits = 0;
			_additiveVolumeDecimalPlaces = 0;
			_volumeUnits = 0;
			_volumeDecimalPlaces = 0;
			_levelUnits = 0;
			_levelDecimalPlaces = 2;
			_temperatureUnits = 0;
			_temperatureDecimalPlaces = 0;
			_densityUnits = 0;
			_densityDecimalPlaces = 0;
			_massUnits = 0;
			_massDecimalPlaces = 0;
			_flowUnits = 0;
			_flowDecimalPlaces = 1;
			_pressureUnits = 0;
			_pressureDecimalPlaces = 2;
			_volumePackageSize = 0;
			_massPackageSize = 0;
		}

		protected void init(SiteClass site, TransactionAliasClass alias, ProductClass product)
		{
			ResetUnits();

			_site = site;
			_alias = alias;
			_product = product;
			if ((product != null) && (!_productsCache.ContainsKey(_product.IdentityGuid)))
			{
				_productsCache.Add(_product.IdentityGuid, _product);
			}

			try
			{
				if (_product == null)
				{
					if (_alias == null)
					{
						_volumeUnits = _site.VolumeUnits;
						_volumeDecimalPlaces = _site._VolumeDecimalPlaces;
						_additiveVolumeUnits = _site.AdditiveVolumeUnits;
						_additiveVolumeDecimalPlaces = _site._AdditiveVolumeDecimalPlaces;
						_levelUnits = _site.LevelUnits;
						_levelDecimalPlaces = _site._LevelDecimalPlaces;
						_temperatureUnits = _site.TemperatureUnits;
						_temperatureDecimalPlaces = _site._TemperatureDecimalPlaces;
						_densityUnits = _site.DensityUnits;
						_densityDecimalPlaces = _site._DensityDecimalPlaces;
						_massUnits = _site.MassUnits;
						_massDecimalPlaces = _site._MassDecimalPlaces;
						_flowUnits = _site.FlowUnits;
						_flowDecimalPlaces = site._FlowDecimalPlaces;
						_pressureUnits = _site.PressureUnits;
						_pressureDecimalPlaces = _site._PressureDecimalPlaces;
					}
					else
					{
						if (_alias.VolumeUnits == 0)
						{
							_volumeUnits = _site.VolumeUnits;
							_volumeDecimalPlaces = _site._VolumeDecimalPlaces;
						}
						else
						{
							_volumeUnits = _alias.VolumeUnits;
							_volumeDecimalPlaces = _alias._VolumeDecimalPlaces;
						}

						if (_alias.LevelUnits == 0)
						{
							_levelUnits = _site.LevelUnits;
							_levelDecimalPlaces = _site._LevelDecimalPlaces;
						}
						else
						{
							_levelUnits = _alias.LevelUnits;
							_levelDecimalPlaces = _alias._LevelDecimalPlaces;
						}
						if (_alias.TemperatureUnits == 0)
						{
							_temperatureUnits = _site.TemperatureUnits;
							_temperatureDecimalPlaces = _site._TemperatureDecimalPlaces;
						}
						else
						{
							_temperatureUnits = _alias.TemperatureUnits;
							_temperatureDecimalPlaces = _alias._TemperatureDecimalPlaces;
						}
						if (_alias.DensityUnits == 0)
						{
							_densityUnits = _site.DensityUnits;
							_densityDecimalPlaces = _site._DensityDecimalPlaces;
						}
						else
						{
							_densityUnits = _alias.DensityUnits;
							_densityDecimalPlaces = _alias._DensityDecimalPlaces;
						}
						if (_alias.MassUnits == 0)
						{
							_massUnits = _site.MassUnits;
							_massDecimalPlaces = _site._MassDecimalPlaces;
						}
						else
						{
							_massUnits = _alias.MassUnits;
							_massDecimalPlaces = _alias._MassDecimalPlaces;
						}
						if (_alias.FlowUnits == 0)
						{
							_flowUnits = _site.FlowUnits;
							_flowDecimalPlaces = _site._FlowDecimalPlaces;

						}
						else
						{
							_flowUnits = _alias.FlowUnits;
							_flowDecimalPlaces = _alias._FlowDecimalPlaces;
						}
						if (_alias.PressureUnits == 0)
						{
							_pressureUnits = _site.PressureUnits;
							_pressureDecimalPlaces = _site._PressureDecimalPlaces;
						}
						else
						{
							_pressureUnits = _alias.PressureUnits;
							_pressureDecimalPlaces = _alias._PressureDecimalPlaces;
						}
						if (_alias.AdditiveVolumeUnits == 0)
						{
							_additiveVolumeUnits = _site.AdditiveVolumeUnits;
							_additiveVolumeDecimalPlaces = _site._AdditiveVolumeDecimalPlaces;
						}
						else
						{
							_additiveVolumeUnits = _alias.AdditiveVolumeUnits;
							_additiveVolumeDecimalPlaces = _alias._AdditiveVolumeDecimalPlaces;
						}
					}
				}
				else
				{
					if (_product.VolumeUnits == 0)
					{
						if (_alias == null || _alias.VolumeUnits == 0)
						{
							if (_product.ProductType == ProductType.AdditiveProduct)
							{
								_volumeUnits = _site.AdditiveVolumeUnits;
								_volumeDecimalPlaces = _site._AdditiveVolumeDecimalPlaces;
							}
							else
							{
								_volumeUnits = _site.VolumeUnits;
								_volumeDecimalPlaces = _site._VolumeDecimalPlaces;
							}
						}
						else
						{
							if (_product.ProductType == ProductType.AdditiveProduct)
							{
								_volumeUnits = _alias.AdditiveVolumeUnits;
								_volumeDecimalPlaces = _alias._AdditiveVolumeDecimalPlaces;
							}
							else
							{
								_volumeUnits = _alias.VolumeUnits;
								_volumeDecimalPlaces = _alias._VolumeDecimalPlaces;
							}
						}
						_additiveVolumeUnits = _volumeUnits;
						_additiveVolumeDecimalPlaces = _volumeDecimalPlaces;
					}
					else
					{
						_volumeUnits = _product.VolumeUnits;
						_volumeDecimalPlaces = _product.VolumeDecimalPlaces;
						_additiveVolumeUnits = _product.VolumeUnits;
						_additiveVolumeDecimalPlaces = _product.VolumeDecimalPlaces;
					}


					if (_product.LevelUnits == 0)
					{
						if (_alias == null || _alias.LevelUnits == 0)
						{

							_levelUnits = _site.LevelUnits;
							_levelDecimalPlaces = _site._LevelDecimalPlaces;
						}
						else
						{
							_levelUnits = _alias.LevelUnits;
							_levelDecimalPlaces = _alias._LevelDecimalPlaces;
						}
					}
					else
					{
						_levelUnits = _product.LevelUnits;
						_levelDecimalPlaces = _product.LevelDecimalPlaces;
					}

					if (_product.TemperatureUnits == 0)
					{
						if (_alias == null || _alias.TemperatureUnits == 0)
						{
							_temperatureUnits = _site.TemperatureUnits;
							_temperatureDecimalPlaces = _site._TemperatureDecimalPlaces;
						}
						else
						{
							_temperatureUnits = _alias.TemperatureUnits;
							_temperatureDecimalPlaces = _alias._TemperatureDecimalPlaces;
						}
					}
					else
					{
						_temperatureUnits = _product.TemperatureUnits;
						_temperatureDecimalPlaces = _product.TemperatureDecimalPlaces;
					}


					if (_product.DensityUnits == 0)
					{
						if (_alias == null || _alias.DensityUnits == 0)
						{
							_densityUnits = _site.DensityUnits;
							_densityDecimalPlaces = _site._DensityDecimalPlaces;
						}
						else
						{
							_densityUnits = _alias.DensityUnits;
							_densityDecimalPlaces = _alias._DensityDecimalPlaces;
						}
					}
					else
					{
						_densityUnits = _product.DensityUnits;
						_densityDecimalPlaces = _product.DensityDecimalPlaces;
					}


					if (_product.MassUnits == 0)
					{
						if (_alias == null || _alias.MassUnits == 0)
						{
							_massUnits = _site.MassUnits;
							_massDecimalPlaces = _site._MassDecimalPlaces;
						}
						else
						{
							_massUnits = _alias.MassUnits;
							_massDecimalPlaces = _alias._MassDecimalPlaces;
						}
					}
					else
					{
						_massUnits = _product.MassUnits;
						_massDecimalPlaces = _product.MassDecimalPlaces;
					}


					if (_product.FlowUnits == 0)
					{
						if (_alias == null || _alias.FlowUnits == 0)
						{
							_flowUnits = _site.FlowUnits;
							_flowDecimalPlaces = _site._FlowDecimalPlaces;
						}
						else
						{
							_flowUnits = _alias.FlowUnits;
							_flowDecimalPlaces = _alias._FlowDecimalPlaces;
						}
					}
					else
					{
						_flowUnits = _product.FlowUnits;
						_flowDecimalPlaces = _product.FlowDecimalPlaces;
					}


					if (_product.PressureUnits == 0)
					{
						if (_alias == null || _alias.PressureUnits == 0)
						{
							_pressureUnits = _site.PressureUnits;
							_pressureDecimalPlaces = _site._PressureDecimalPlaces;
						}
						else
						{
							_pressureUnits = _alias.PressureUnits;
							_pressureDecimalPlaces = _alias._PressureDecimalPlaces;
						}
					}
					else
					{
						_pressureUnits = _product.PressureUnits;
						_pressureDecimalPlaces = _product.PressureDecimalPlaces;
					}

					_massPackageSize = product._MassPackageSize.Value;
					_volumePackageSize = product._VolumePackageSize.Value;
				}
			}
			catch
			{
				ResetUnits();
			}
		}
		#endregion Protected Methods

	}
}
