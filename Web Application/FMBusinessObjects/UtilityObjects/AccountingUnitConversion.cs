 #pragma warning disable 1587
/// <summary> =================================================================
///
///	FILE NAME:	AccountingUnitConversion.cs
///
///	PURPOSE:	Declaration of the AccountingUnitConversion class
///
///	Copyright	(C) 1999-2009	 Varec, Inc.  Norcross, GA, USA        
///				All Rights Reserved
///
///	This file shall not be copied or reproduced in any form without the
///	express written consent of Varec.
///
///	DATE			BY							VERSION		REASON
///	==========		============				========	============================
///	2009-01-01		Unknown									Initial Creation.
///
/// </summary> ================================================================
#pragma warning restore 1587
namespace FMBusinessObjects.UtilityObjects
{
    using System;
    using System.Globalization;
    using System.Security;

    using FMBusinessObjects.DataObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class AccountingUnitConversion
	{
		#region Protected data members
		protected SiteClass site;
		protected double volumeConversionFactor;
		protected double massConversionFactor;
		protected int volumePrecision;
		protected int massPrecision;
		protected NumberFormatInfo volumeFormat;
		protected NumberFormatInfo massFormat;
		#endregion

		#region Constructors
		public AccountingUnitConversion ( SiteClass site, ProductClass product )
		{
			this.site = site;
			this.Init ( product );
		}
		#endregion

		#region Properties
		public double VolumeConversionFactorFromSI 
		{ 
			get { return this.volumeConversionFactor; } 
		}

		public double VolumeConversionFactorToSI 
		{ 
			get { return 1.0 / this.volumeConversionFactor; } 
		}

		public double MassConversionFactorFromSI 
		{ 
			get { return this.massConversionFactor; } 
		}

		public double MassConversionFactorToSI 
		{ 
			get { return 1.0 / this.massConversionFactor; } 
		}

		public int VolumePrecision 
		{ 
			get { return this.volumePrecision; } 
		}

		public int MassPrecision 
		{ 
			get { return this.massPrecision; } 
		}
		#endregion

		#region Public methods
		public double ConvertVolumeToSI ( double x )
		{
			return x * this.VolumeConversionFactorToSI;
		}

		public double ConvertVolumeFromSI ( double x )
		{
			return Math.Round ( x * this.VolumeConversionFactorFromSI, this.volumePrecision, MidpointRounding.AwayFromZero );
		}

		public double ConvertMassToSI ( double x )
		{
			return x * this.MassConversionFactorToSI;
		}

		public double ConvertMassFromSI ( double x )
		{
			return Math.Round ( x * this.MassConversionFactorFromSI, this.massPrecision, MidpointRounding.AwayFromZero );
		}
		#endregion

		[SecurityCritical]
		protected void Init(ProductClass product)
		{
			double dSpecial = 0;

			if (product.ProductType == ProductType.AdditiveProduct)
			{
				this.volumeFormat = this.site.GetNumberFormatInfo ( SITE_VARIABLE_TYPE.ADDITIVE_VOLUME );
				EngineeringUnits.Convert(1, EngineeringUnit.FmvMeter3, ref this.volumeConversionFactor, (product.VolumeUnits == 0) ? this.site.AdditiveVolumeUnits : product.VolumeUnits, dSpecial);
				this.volumePrecision = (product.VolumeUnits == 0) ? this.site._AdditiveVolumeDecimalPlaces : product.VolumeDecimalPlaces;

				this.massFormat = this.site.GetNumberFormatInfo ( SITE_VARIABLE_TYPE.ADDITIVE_MASS );
				EngineeringUnits.Convert(1, EngineeringUnit.FmmKg, ref this.massConversionFactor, (product.MassUnits == 0) ? this.site.MassUnits : product.MassUnits, dSpecial);
				this.massPrecision = (product.MassUnits == 0) ? this.site._MassDecimalPlaces : product.MassDecimalPlaces;
			}
			else
			{
			    this.volumeFormat = this.site.GetNumberFormatInfo ( SITE_VARIABLE_TYPE.VOLUME );
				EngineeringUnits.Convert(1, EngineeringUnit.FmvMeter3, ref this.volumeConversionFactor, (product.VolumeUnits == 0) ? this.site.VolumeUnits : product.VolumeUnits, dSpecial);
			    this.volumePrecision = (product.VolumeUnits == 0) ? this.site._VolumeDecimalPlaces : product.VolumeDecimalPlaces;

			    this.massFormat = this.site.GetNumberFormatInfo ( SITE_VARIABLE_TYPE.MASS );
				EngineeringUnits.Convert(1, EngineeringUnit.FmmKg, ref this.massConversionFactor, (product.MassUnits == 0) ? this.site.MassUnits : product.MassUnits, dSpecial);
			    this.massPrecision = (product.MassUnits == 0) ? this.site._MassDecimalPlaces : product.MassDecimalPlaces;
			}
		}
	}
}