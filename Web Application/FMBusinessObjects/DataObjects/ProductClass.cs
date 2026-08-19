using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Text;

using Varec.CommonComponents.EngineeringUnitsLibrary;
using Varec.CommonComponents.VolumeCorrection;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	using FMBusinessObjects.Exceptions;
	using System.IO;
	using CodedVariables;
    using System.Web.UI;
    using System.Web;


    #region Public enumerations
    public enum ProductType
    {
        ComponentProduct = 0,
        BlendProduct = 1,
        AdditiveProduct = 2,
        AdditizedProduct = 3,
        MaxProduct = 4
	};
	#endregion

	[Serializable]
	[CollectionDataContract]
	public class ProductCollectionClass : List<ProductClass>
	{
	}

	[Serializable]
	[CollectionDataContract]
	public class ProductByTaxCodeCollectionClass : Dictionary<string, ProductClass>
	{
		public void Add(ProductClass product )
		{
			if (!this.ContainsKey(product.TaxCode))
			{
				this.Add( product.TaxCode, product);
			}

		}
	}

	[Serializable]
	[DataContract]
	[QueryWriterTopic(typeof(ProductClass), "Products")]
	[QueryWriterTopicSecurity(RIGHT.VIEW_PRODUCTS)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_PRODUCTS)]
	[EntityImportExportWorksheetAttribute("PRODUCTS")]
	public class ProductClass : FMBaseDataObjectWithUserData, IDataDictionary
	{
		#region Public data members
		[DataMember]
		public const string ENTITY_TYPE_ID = "Products";

		[DataMember]
		public Date _StockResetDate;
		[DataMember]
		public SIDouble _DensityHighLimit;
		[DataMember]
		public SIDouble _DensityLowLimit;
		[DataMember]
		public SIDouble _TemperatureHiHiLimit;
		[DataMember]
		public SIDouble _TemperatureHighLimit;
		[DataMember]
		public SIDouble _TemperatureLowLimit;
		[DataMember]
		public SIDouble _TemperatureLoLoLimit;
		[DataMember]
		public SIDifferential _TemperatureDeadband;
		[DataMember]
		public SIDouble _LowStockWarning;
		[DataMember]
		public FMDecimal _Price;
		[DataMember]
		public SIDouble _StandardDensity;
		[DataMember]
		public VcfModuleSettings _VcfModuleSettings;
		[DataMember]
		public FMDecimal _OctaneNumber;
		[DataMember]
		public SIDouble _ReidVaporPressure;
		[DataMember]
		public bool _HazardousMaterial;
		[DataMember]
		public FMDecimal _ComponentTolerance;
		[DataMember]
		public Date _LockedOutDate;					// Excluded from Property Map
		[DataMember]
		public SIDouble _VolumePackageSize;
		[DataMember]
		public SIDouble _MassPackageSize;
		[DataMember]
		public Guid TrackingProductGuid;

		[EntityImportExportWorksheetAttribute("PRODUCT MESSAGES")]
		[EntityImportExportAttribute("ID*", 125, "ID", 1)]
		[DataMember]
		public ApplicationStringMapCollectionClass ProductMessageCollection;

		[EntityImportExportWorksheetAttribute("HAZARDOUS MATERIAL MESSAGES","ID*")]
		[EntityImportExportAttribute("ID*", 125, "ID", 1)]
		[DataMember]
		public ApplicationStringMapCollectionClass HazardousMaterialMessageCollection;

		[EntityImportExportWorksheetAttribute("BLEND COMPONENTS")]
		[EntityImportExportAttribute("ID*", 125, "AssignedID", 1)]
		[EntityImportExportAttribute("PERCENTAGE", 100, "BlendPercentage", 2)]
		[DataMember]
		public ProductMapCollectionClass ComponentCollection;

		[EntityImportExportWorksheetAttribute("AUTHORIZED CUSTOMERS")]
		[EntityImportExportAttribute("ID*", 125, "AssignedToID", 1)]
		[EntityImportExportAttribute("ADDITIVEPROFILEID*", 130, "AdditiveProfileID", 2)]
		[EntityImportExportAttribute("SHIPTOPRODUCTID", 130, "ShipToProductID", 3)]
		[EntityImportExportAttribute("SHIPTOPRODUCTCODE", 130, "ShipToProductCode", 4)]
		[EntityImportExportAttribute("SHIPTOLOADRACKTEXT", 130, "ShipToLoadRackDisplayText", 5)]
		[EntityImportExportAttribute("INSTRUCTIONS", 1000, "Note", 6)]
		[DataMember]
		public ProductMapCollectionClass AuthorizedCustomerCollection;

		[EntityImportExportWorksheetAttribute("AUTHORIZED CUSTOMER GROUPS")]
		[EntityImportExportAttribute("ID*", 125, "AssignedToID", 1)]
		[EntityImportExportAttribute("ADDITIVEPROFILEID*", 130, "AdditiveProfileID", 2)]
		[EntityImportExportAttribute("SHIPTOPRODUCTID", 130, "ShipToProductID", 3)]
		[EntityImportExportAttribute("SHIPTOPRODUCTCODE", 130, "ShipToProductCode", 4)]
		[EntityImportExportAttribute("SHIPTOLOADRACKTEXT", 130, "ShipToLoadRackDisplayText", 5)]
		[EntityImportExportAttribute("INSTRUCTIONS", 1000, "Note", 6)]
		[DataMember]
		public ProductMapCollectionClass AuthorizedCustomerGroupCollection;

        [EntityImportExportWorksheetAttribute("AUTHORIZED SUPPLIERS")]
        [EntityImportExportAttribute("ID*", 125, "AssignedToID", 1)]
        [EntityImportExportAttribute("SUPPLIERPRODUCTID", 130, "ShipToProductID", 3)]
        [EntityImportExportAttribute("SUPPLIERPRODUCTCODE", 130, "ShipToProductCode", 4)]
        [EntityImportExportAttribute("SUPPLIERLOADRACKTEXT", 130, "ShipToLoadRackDisplayText", 5)]
        [EntityImportExportAttribute("INSTRUCTIONS", 1000, "Note", 6)]
        [DataMember]
        public ProductMapCollectionClass AuthorizedSupplierCollection;

        public bool TrackedByIrs { get { return this.TaxCode != null && this.TaxCode.Length > 0;}   }
	
		#endregion

		#region Protected data members
		[DataMember]
		protected string _Description;
		[DataMember]
		protected string _GenericType;
		[DataMember]
		protected ProductType _ProductType;
		[DataMember]
		protected bool _StockTrack;

		[DataMember]
		protected EngineeringUnit _SiteVolumeUnits;
		[DataMember]
		protected EngineeringUnit _SiteDensityUnits;
		[DataMember]
		protected EngineeringUnit _SiteTemperatureUnits;
		[DataMember]
		protected EngineeringUnit _SitePressureUnits;
		[DataMember]
		protected EngineeringUnit _SiteMassUnits;
		[DataMember]
		protected EngineeringUnit _SiteFlowUnits;
		[DataMember]
		protected EngineeringUnit _SiteLevelUnits;
		[DataMember]
		protected EngineeringUnit _VolumeUnits;
		[DataMember]
		protected EngineeringUnit _TemperatureUnits;
		[DataMember]
		protected EngineeringUnit _DensityUnits;
		[DataMember]
		protected EngineeringUnit _MassUnits;
		[DataMember]
		protected EngineeringUnit _LevelUnits;
		[DataMember]
		protected EngineeringUnit _FlowUnits;
		[DataMember]
		protected EngineeringUnit _PressureUnits;
		[DataMember]
		public SIDifferential _DensityDeadband;
		[DataMember]
		protected bool _ApplyDensityLimits;
		[DataMember]
		protected bool _ApplyStandardDensity;
		[DataMember]
		protected bool _ApplyTemperatureLimits;
		[DataMember]
		protected bool _ApplyVolumeCorrection;
		[DataMember]
		protected bool _Bonded;
		[DataMember]
		protected bool _GroundFuel;
		[DataMember]
		protected string _Code;
		[DataMember]
		protected bool _AviationFuel;
		[DataMember]
		protected byte _VolumeDecimalPlaces;
		[DataMember]
		protected byte _TemperatureDecimalPlaces;
		[DataMember]
		protected byte _DensityDecimalPlaces;
		[DataMember]
		protected byte _LevelDecimalPlaces;
		[DataMember]
		protected byte _MassDecimalPlaces;
		[DataMember]
		protected byte _FlowDecimalPlaces;
		[DataMember]
		protected byte _PressureDecimalPlaces;
		[DataMember]
		protected bool _Capitalize;
		[DataMember]
		protected int _RegulatoryClass;
		[DataMember]
		protected string _LoadRackDisplayText;
		[DataMember]
		protected bool _VaporRecovery;
		[DataMember]
		protected bool _LockedOut;
		[DataMember]
		protected string _LockedOutReason;
		[DataMember]
		protected double _VarianceTolerance;
		[DataMember]
		protected double dielectricTolerance;
		[DataMember]
		protected bool _LoadByWeight;
		[DataMember]
		protected string _PIDXCode;
		[DataMember]
		protected string _PIDXFamilyCode;
		[DataMember]
		protected bool _IsEthanol;
		[DataMember]
		protected string _ContaminationPromptLoadRackText;
		[DataMember]
		protected bool _InhibitAccounting;
		[DataMember]
		protected string _TaxCode;
		[DataMember]
		protected Guid _MasterRecordGuid;
		[DataMember]
		protected Guid _AssignedToSiteGuid;
		[DataMember]
		protected Guid _AssignedFromSiteGuid;
		[DataMember]
		protected string _AssignedFromSiteId;

		// The following items are not part of tblProducts
		[DataMember]
		protected string _TrackingProductID;
		#endregion

		#region Private data members
		static private System.Text.RegularExpressions.Regex irsProduceCodesRegex;

        private EngineeringUnit ConvertFromStringToEngineeringUnits(string value)
        {
            EngineeringUnit units = EngineeringUnit.FmSiteUnits;

            if ( Enum.TryParse(value, true, out units) == false)
            {
                units = EngineeringUnit.FmSiteUnits;

            }
            return units;
        }

        [DataMember]
		protected string productColor;
		[DataMember]
		protected string patternColor;
		[DataMember]
		protected int patternNumber;
		#endregion

		#region Properties

		[EntityImportExportAttribute("SITE*", 105, "SITEGUID")]
		new public Guid SiteGuid { get { return this._SiteGuid; } set {
			this._SiteGuid = value; } }

		[QueryWriterField("ID", "ProductID")]
		[EntityImportExportAttribute("PRODUCTID*", 105, "ID")]
		public override string ID
        {
            get { return this._ID; }
            set
            {
                this.SetString("ID", 30, value, ref this._ID);
            }
        }

        [QueryWriterField("Description")]
		[EntityImportExportAttribute("DESCRIPTION", 105, "Description")]
		public string Description { get { return this._Description; } set {
			this.SetString("Description", 50, value, ref this._Description); } }

		[QueryWriterField("Generic Type")]
		[EntityImportExportAttribute("GENERICTYPE", 80, "GenericType")]
		public string GenericType { get { return this._GenericType; } set {
			this.SetString("GenericType", 10, value, ref this._GenericType); } }

		[QueryWriterField("Product Type", "LookupProductTypeIndex")]
		[EntityImportExportAttribute("PRODUCTTYPE", 125, "ProductType")]
		public ProductType ProductType { get { return this._ProductType; } set {
			this._ProductType = value; } }

		[QueryWriterField("Stock Reset Date", "StockResetDate")]
		public Date StockResetDateObject { get { return this._StockResetDate; } set {
			this._StockResetDate = value; } }

		[EntityImportExportAttribute("STOCKRESETDATE", 99, "StockResetDate")]
		[XmlIgnore]
		public string StockResetDate { get { return this._StockResetDate.ToString( ); } set {
			this.SetDate("Stock Reset Date", value, ref this._StockResetDate); } }

		[QueryWriterField("Stock Track")]
		[EntityImportExportAttribute("STOCKTRACK", 75, "StockTrack")]
		public bool StockTrack { get { return this._StockTrack; } set {
			this._StockTrack = value; } }

		public string DensityDeadband { get { return this._DensityDeadband.ToString(); } set {
			this.SetSIDifferential("Density Deadband", value, ref this._DensityDeadband); } }

		[QueryWriterField("Density High Limit")]
		[EntityImportExportAttribute("DENSITYHIGHLIMIT", 115, "DensityHighLimit")]
		[XmlIgnore]
		public string DensityHighLimit
		{
			get { return this.GetSIDouble("Density High Limit", this._DensityHighLimit); }
			set
			{
				//Perform the value assignment on a temp variable in case there is 
				//a conversion exception and want to keep the original value of the Limit object
				var d = new SIDouble() { Format = this._DensityHighLimit.Format, Units = this._DensityHighLimit.Units };
				this.SetSIDouble("Density High Limit", value, ref d);
				this._DensityHighLimit.SIValue = d.SIValue;
			}
		}

		[QueryWriterField("Density Low Limit")]
		[EntityImportExportAttribute("DENSITYLOWLIMIT", 115, "DensityLowLimit")]
		[XmlIgnore]
		public string DensityLowLimit
		{
			get { return this.GetSIDouble("Density Low Limit", this._DensityLowLimit); }
			set
			{
				//Perform the value assignment on a temp variable in case there is 
				//a conversion exception and want to keep the original value of the Limit object
				var d = new SIDouble() { Format = this._DensityLowLimit.Format, Units = this._DensityLowLimit.Units };
				this.SetSIDouble("Density Low Limit", value, ref d);
				this._DensityLowLimit.SIValue = d.SIValue;
			}
		}


        [QueryWriterField("Density High Limit Minus Deadband", false)]
        [EntityImportExportAttribute("DENSITYHIGHLIMITMINUSDEADBAND", 115, "DensityHighMinusDeadband")]
        public string DensityHighMinusDeadband
        {
            get
            {
                var _DensityHighMinusDeadband = new SIDouble() { Format = this._DensityHighLimit.Format, Units = this._DensityHighLimit.Units };
                _DensityHighMinusDeadband.SIValue = this._DensityHighLimit.SIValue - this._DensityDeadband.SIValue;
                return this.GetSIDouble("Density High Limit Minus Deadband", _DensityHighMinusDeadband);
            }
        }

        [QueryWriterField("Density Low Limit Plus Deadband", false)]
        [EntityImportExportAttribute("DENSITYLOWLIMITPLUSDEADBAND", 115, "DensityLowPlusDeadband")]
        public string DensityLowPlusDeadband
        {
            get
            {
                var _DensityLowPlusDeadband = new SIDouble() { Format = this._DensityLowLimit.Format, Units = this._DensityLowLimit.Units };
                _DensityLowPlusDeadband.SIValue = this._DensityLowLimit.SIValue + this._DensityDeadband.SIValue;
                return this.GetSIDouble("Density Low Limit Plus Deadband", _DensityLowPlusDeadband);
            }
        }

        [QueryWriterField("Apply Density Limits")]
		[EntityImportExportAttribute("APPLYDENSITYLIMITS", 120, "ApplyDensityLimits")]
		public bool ApplyDensityLimits { get { return this._ApplyDensityLimits; } set {
			this._ApplyDensityLimits = value; } }

		[QueryWriterField("Temperature HiHi Limit")]
		[EntityImportExportAttribute("TEMPERATUREHIHILIMIT", 130, "TemperatureHiHiLimit")]
		[XmlIgnore]
		public string TemperatureHiHiLimit { get { return this._TemperatureHiHiLimit.ToString( ); } set {
			this.SetSIDouble("Temperature HiHi Limit", value, ref this._TemperatureHiHiLimit); } }

		[QueryWriterField("Temperature High Limit")]
		[EntityImportExportAttribute("TEMPERATUREHIGHLIMIT", 135, "TemperatureHighLimit")]
		[XmlIgnore]
		public string TemperatureHighLimit { get { return this._TemperatureHighLimit.ToString( ); } set {
			this.SetSIDouble("Temperature High Limit", value, ref this._TemperatureHighLimit); } }

		[QueryWriterField("Temperature Low Limit")]
		[EntityImportExportAttribute("TEMPERATURELOWLIMIT", 135, "TemperatureLowLimit")]
		[XmlIgnore]
		public string TemperatureLowLimit { get { return this._TemperatureLowLimit.ToString( ); } set {
			this.SetSIDouble("Temperature Low Limit", value, ref this._TemperatureLowLimit); } }

		[QueryWriterField("Temperature LoLo Limit")]
		[EntityImportExportAttribute("TEMPERATURELOLOLIMIT", 135, "TemperatureLoLoLimit")]
		[XmlIgnore]
		public string TemperatureLoLoLimit { get { return this._TemperatureLoLoLimit.ToString( ); } set {
			this.SetSIDouble("Temperature LoLo Limit", value, ref this._TemperatureLoLoLimit); } }

		[QueryWriterField("Temperature Deadband")]
		[EntityImportExportAttribute("TEMPERATUREDEADBAND", 135, "TemperatureDeadband")]
		[XmlIgnore]
		public string TemperatureDeadband { get { return this._TemperatureDeadband.ToString( ); } set {
			this.SetSIDifferential("Temperature Deadband", value, ref this._TemperatureDeadband); } }

		[QueryWriterField("Apply Temperature Limits")]
		[EntityImportExportAttribute("APPLYTEMPERATURELIMITS", 145, "ApplyTemperatureLimits")]
		public bool ApplyTemperatureLimits { get { return this._ApplyTemperatureLimits; } set {
			this._ApplyTemperatureLimits = value; } }

		[QueryWriterField("Bonded")]
		[EntityImportExportAttribute("BONDED", 50, "Bonded")]
		public bool Bonded { get { return this._Bonded; } set {
			this._Bonded = value; } }

		[QueryWriterField("Low Stock Warning")]
		[EntityImportExportAttribute("LOWSTOCKWARNING", 115, "LowStockWarning")]
		public string LowStockWarning { get { return this._LowStockWarning.ToString( ); } set {
			this.SetSIDouble("Low Stock Warning", value, ref this._LowStockWarning); } }

		[QueryWriterField("Ground Fuel")]
		[EntityImportExportAttribute("GROUNDFUEL", 75, "GroundFuel")]
		public bool GroundFuel { get { return this._GroundFuel; } set {
			this._GroundFuel = value; } }

		[QueryWriterField("Code", "ProductCode")]
		[EntityImportExportAttribute("PRODUCTCODE", 96, "Code")]
		public string Code { get { return this._Code; } set {
			this.SetString("Code", 15, value, ref this._Code); } }

		[QueryWriterField("Price")]
		[EntityImportExportAttribute("PRICE", 60, "Price")]
		public string Price { get { return this._Price.ToString( ); } set {
			this.SetDecimal("Price", value, ref this._Price); } }

		[QueryWriterField("Aviation Fuel", "AviationFuelFlag")]
		[EntityImportExportAttribute("AVIATIONFUEL", 90, "AviationFuel")]
		public bool AviationFuel { get { return this._AviationFuel; } set {
			this._AviationFuel = value; } }

		[QueryWriterField("Standard Density")]
		[EntityImportExportAttribute("STANDARDDENSITY", 115, "StandardDensity")]
		public string StandardDensity
		{
			get { return this.GetSIDouble("Standard Density", this._StandardDensity); }
			set {
				this.SetSIDouble("Standard Density", value, ref this._StandardDensity); }
		}

		[QueryWriterField("Apply Standard Density")]
		[EntityImportExportAttribute("APPLYSTANDARDDENSITY", 149, "ApplyStandardDensity")]
		public bool ApplyStandardDensity { get { return this._ApplyStandardDensity; } set {
			this._ApplyStandardDensity = value; } }

		[QueryWriterField("Correction Method Type", "CorrectionMethodType", false)]
		[EntityImportExportAttribute("CORRECTIONMETHODTYPE", 149, "CorrectionMethodType")]
		public string CorrectionMethodType
		{
			get
			{
				return MajorCorrectionMethodID(this._VcfModuleSettings.CorrectionMethodType);
			}
			set
			{
				for (ECorrectionTypeMajor Type = ECorrectionTypeMajor.CORR_NONE; Type <= ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009; Type++)
				{
					if (value == MajorCorrectionMethodID(Type))
					{
						this._VcfModuleSettings.CorrectionMethodType = Type;
						break;
					}
				}
			}
		}

		[QueryWriterField("Correction Method Specific", "CorrectionMethodSpecific", false)]
		[EntityImportExportAttribute("CORRECTIONMETHODSPECIFIC", 149, "CorrectionMethodSpecific")]
		public string CorrectionMethodSpecific
		{
			get
			{
				return MinorCorrectionMethodID(this._VcfModuleSettings.CorrectionMethodType, this._VcfModuleSettings.CorrectionMethodSpecific);
			}
			set
			{
				int minimum = 0;
				int maximum = 0;

				MinorRange(this._VcfModuleSettings.CorrectionMethodType, ref minimum, ref maximum);

				for (int minorType = minimum; minorType < maximum; minorType++)
				{
					if (value == MinorCorrectionMethodID(this._VcfModuleSettings.CorrectionMethodType, (ECorrectionTypeMinor) minorType))
					{
						this._VcfModuleSettings.CorrectionMethodSpecific = (ECorrectionTypeMinor) minorType;
						break;
					}
				}
			}
		}

		[QueryWriterField("Standard Temperature", false)]
		[EntityImportExportAttribute("STANDARDTEMPERATURE", 135, "StandardTemperature")]
		public string StandardTemperature { get { return this._VcfModuleSettings.BaseTemperature.Value.ToString(); } set {
			this._VcfModuleSettings.BaseTemperature.Value = Convert.ToDouble(value); } }

		[QueryWriterField("Alternate Temperature", false)]
		[EntityImportExportAttribute("ALTERNATETEMPERATURE", 140, "AlternateTemperature")]
		public string AlternateTemperature { get { return this._VcfModuleSettings.AlternateTemperature.Value.ToString(); } set {
			this._VcfModuleSettings.AlternateTemperature.Value = Convert.ToDouble(value); } }

		[QueryWriterField("Alternate Pressure", false)]
		[EntityImportExportAttribute("ALTERNATEPRESSURE", 125, "AlternatePressure")]
		public string AlternateBasePressure { get { return this._VcfModuleSettings.AlternateBasePressure.Value.ToString(); } set {
			this._VcfModuleSettings.AlternateBasePressure.Value = Convert.ToDouble(value); } }



		[EntityImportExportAttribute("K0", 120, "K0")]
		public double CorrectionFactor0 { get { return this._VcfModuleSettings.K[0]; } set {
			this._VcfModuleSettings.K[0] = value; } }

		[EntityImportExportAttribute("K1", 120, "K1")]
		public double CorrectionFactor1 { get { return this._VcfModuleSettings.K[1]; } set {
			this._VcfModuleSettings.K[1] = value; } }

		[EntityImportExportAttribute("K2", 120, "K2")]
		public double CorrectionFactor2 { get { return this._VcfModuleSettings.K[2]; } set {
			this._VcfModuleSettings.K[2] = value; } }

		[EntityImportExportAttribute("K3", 120, "K3")]
		public double CorrectionFactor3 { get { return this._VcfModuleSettings.K[3]; } set {
			this._VcfModuleSettings.K[3] = value; } }

		[EntityImportExportAttribute("K4", 120, "K4")]
		public double CorrectionFactor4 { get { return this._VcfModuleSettings.K[4]; } set {
			this._VcfModuleSettings.K[4] = value; } }



		[QueryWriterField("Apply Volume Correction")]
		[EntityImportExportAttribute("APPLYVOLUMECORRECTION", 149, "ApplyVolumeCorrection")]
		public bool ApplyVolumeCorrection { get { return this._ApplyVolumeCorrection; } set {
			this._ApplyVolumeCorrection = value; } }

		//[QueryWriterField("Volume Units")]
		[XmlIgnoreAttribute]
		public EngineeringUnit VolumeUnits
		{
			get { return this._VolumeUnits; }
			set
			{
				this._VolumeUnits = value;
				this._VolumePackageSize.Units = value;

				if ( value == 0 )
				{
					this._LowStockWarning.Units = this._SiteVolumeUnits;
				}
				else
				{
					this._LowStockWarning.Units = value;
				}
			}
		}

        [EntityImportExportAttribute("VOLUMEUNITS", 95, "VolumeUnitsString")]
		public string VolumeUnitsString
		{
			get { return (this.VolumeUnits == 0) ? "<Site>" : this.VolumeUnits.ToString(); }
			set { this.VolumeUnits = ConvertFromStringToEngineeringUnits(value); }
		}

        [XmlIgnoreAttribute]
		public EngineeringUnit TemperatureUnits
		{
			get { return this._TemperatureUnits; }
			set
			{
				this._TemperatureUnits = value;

				if ( value == 0 )
				{
					this._TemperatureDeadband.Units = this._SiteTemperatureUnits;
					this._TemperatureHighLimit.Units = this._SiteTemperatureUnits;
					this._TemperatureLowLimit.Units = this._SiteTemperatureUnits;
					this._TemperatureHiHiLimit.Units = this._SiteTemperatureUnits;
					this._TemperatureLoLoLimit.Units = this._SiteTemperatureUnits;
				}
				else
				{
					this._TemperatureDeadband.Units = value;
					this._TemperatureHighLimit.Units = value;
					this._TemperatureLowLimit.Units = value;
					this._TemperatureHiHiLimit.Units = value;
					this._TemperatureLoLoLimit.Units = value;
				}
			}
		}

		[EntityImportExportAttribute("TEMPERATUREUNITS", 120, "TemperatureUnitsString")]
		public string TemperatureUnitsString
		{
			get { return (this._TemperatureUnits == 0) ? "<Site>" : this._TemperatureUnits.ToString(); }
			set { this.TemperatureUnits = ConvertFromStringToEngineeringUnits(value); }
		}

            [XmlIgnoreAttribute]
		public EngineeringUnit DensityUnits
		{
			get { return this._DensityUnits; }
			set
			{
				this._DensityUnits = value;

				if ( value == 0 )
				{
					this._DensityHighLimit.Units = this._SiteDensityUnits;
					this._DensityLowLimit.Units = this._SiteDensityUnits;
					this._DensityDeadband.Units = this._SiteDensityUnits;
					this._StandardDensity.Units = this._SiteDensityUnits;
				}
				else
				{
					this._DensityHighLimit.Units = value;
					this._DensityLowLimit.Units = value;
					this._DensityDeadband.Units = value;
					this._StandardDensity.Units = value;
				}
			}
		}

		[EntityImportExportAttribute("DENSITYUNITS", 100, "DensityUnitsString")]
		public string DensityUnitsString 
		{ 
			get { return (this._DensityUnits == 0) ? "<Site>" : this._DensityUnits.ToString( ); }
            set { this.DensityUnits = ConvertFromStringToEngineeringUnits(value); }
        }

		[XmlIgnoreAttribute]
		public EngineeringUnit LevelUnits { get { return this._LevelUnits; } set {
			this._LevelUnits = value; } }

		[EntityImportExportAttribute("LEVELUNITS", 95, "LevelUnitsString")]
		public string LevelUnitsString
		{
			get { return (this._LevelUnits == 0) ? "<Site>" : this._LevelUnits.ToString(); }
			set { this.LevelUnits = ConvertFromStringToEngineeringUnits(value); }
		}

        [XmlIgnoreAttribute]
		public EngineeringUnit MassUnits { get { return this._MassUnits; } set {
			this._MassUnits = value;
			this._MassPackageSize.Units = value; } }

		[EntityImportExportAttribute("MASSUNITS", 95, "MassUnitsString")]
		public string MassUnitsString 
		{ 
			get { return (this._MassUnits == 0) ? "<Site>" : this._MassUnits.ToString( ); }
            set { this.MassUnits = ConvertFromStringToEngineeringUnits(value); }
        }

		[XmlIgnoreAttribute]
		public EngineeringUnit FlowUnits { get { return this._FlowUnits; } set {
			this._FlowUnits = value; } }

		[EntityImportExportAttribute("FLOWUNITS", 95, "FlowUnitsString")]
		public string FlowUnitsString 
		{ 
			get { return (this._FlowUnits == 0) ? "<Site>" : this._FlowUnits.ToString( ); }
            set { this.FlowUnits = ConvertFromStringToEngineeringUnits(value); }
		}

		public EngineeringUnit SiteDensityUnits
		{
			get { return this._SiteDensityUnits; }
			set {
				this._SiteDensityUnits = value; }
		}

		public EngineeringUnit SiteTemperatureUnits
		{
			get { return this._SiteTemperatureUnits; }
			set {
				this._SiteTemperatureUnits = value; }
		}

		public EngineeringUnit SiteVolumeUnits
		{
			get { return this._SiteVolumeUnits; }
			set {
				this._SiteVolumeUnits = value; }
		}

		public EngineeringUnit SitePressureUnits
		{
			get { return this._SitePressureUnits; }
			set {
				this._SitePressureUnits = value; }
		}

		public EngineeringUnit SiteMassUnits
		{
			get { return this._SiteMassUnits; }
			set {
				this._SiteMassUnits = value; }
		}

		public EngineeringUnit SiteFlowUnits
		{
			get { return this._SiteFlowUnits; }
			set {
				this._SiteFlowUnits = value; }
		}

		[XmlIgnoreAttribute]
		public EngineeringUnit PressureUnits
		{
			get { return this._PressureUnits; }
			set
			{
				this._PressureUnits = value;

				if ( value == 0 )
				{
					this._ReidVaporPressure.Units = this._SitePressureUnits;
				}
				else
				{
					this._ReidVaporPressure.Units = value;
				}
			}
		}

		[EntityImportExportAttribute("PRESUREUNITS", 95, "PressureUnitsString")]
		public string PressureUnitsString
		{
			get { return (this._PressureUnits == 0) ? "<Site>" : this._PressureUnits.ToString( ); }
            set { this.PressureUnits = ConvertFromStringToEngineeringUnits(value); }
        }

		[QueryWriterField("Volume Unit Decimal Places")]
		[EntityImportExportAttribute("VOLUMEDECIMALPLACES", 135, "VolumeDecimalPlaces")]
		public byte VolumeDecimalPlaces { get { return this._VolumeDecimalPlaces; } set {
			this._VolumeDecimalPlaces = value;
			this._VolumePackageSize.Format.NumberDecimalDigits = value; } }

		[QueryWriterField("Temperature Decimal Places")]
		[EntityImportExportAttribute("TEMPERATUREDECIMALPLACES", 165, "TemperatureDecimalPlaces")]
		public byte TemperatureDecimalPlaces { get { return this._TemperatureDecimalPlaces; } set {
			this._TemperatureDecimalPlaces = value; } }

		[QueryWriterField("Density Decimal Places")]
		[EntityImportExportAttribute("DENSITYDECIMALPLACES", 135, "DensityDecimalPlaces")]
		public byte DensityDecimalPlaces { get { return this._DensityDecimalPlaces; } set {
			this._DensityDecimalPlaces = value; } }

		[QueryWriterField("Mass Decimal Places")]
		[EntityImportExportAttribute("MASSDECIMALPLACES", 135, "MassDecimalPlaces")]
		public byte MassDecimalPlaces { get { return this._MassDecimalPlaces; } set {
			this._MassDecimalPlaces = value;
			this._MassPackageSize.Format.NumberDecimalDigits = value; } }

		[QueryWriterField("Level Decimal Places")]
		public byte LevelDecimalPlaces { get { return this._LevelDecimalPlaces; } set {
			this._LevelDecimalPlaces = value; } }

		[QueryWriterField("Flow Decimal Places")]
		public byte FlowDecimalPlaces { get { return this._FlowDecimalPlaces; } set {
			this._FlowDecimalPlaces = value; } }

		[QueryWriterField("Pressure Decimal Places")]
		public byte PressureDecimalPlaces { get { return this._PressureDecimalPlaces; } set {
			this._PressureDecimalPlaces = value; } }

		[QueryWriterField("Capitalize")]
		[EntityImportExportAttribute("CAPITALIZE", 75, "Capitalize")]
		public bool Capitalize { get { return this._Capitalize; } set {
			this._Capitalize = value; } }

		[QueryWriterField("Octane Number")]
		[EntityImportExportAttribute("OCTANENUMBER", 95, "OctaneNumber")]
		public string OctaneNumber { get { return this._OctaneNumber.ToString( ); } set {
			this.SetDecimal("Octane Number", value, ref this._OctaneNumber); } }

		[QueryWriterField("Reid Vapor Pressure")]
		[EntityImportExportAttribute("REIDVAPORPRESSURE", 125, "ReidVaporPressure")]
		public string ReidVaporPressure
		{
			get { return this._ReidVaporPressure.ToString( ); }
			set {
				this.SetSIDouble("Reid Vapor Pressure", value, ref this._ReidVaporPressure); }
		}

		[QueryWriterField("Hazardous Material")]
		[EntityImportExportAttribute("HAZARDOUSMATERIAL", 125, "HazardousMaterial")]
		public bool HazardousMaterial { get { return this._HazardousMaterial; } set {
			this._HazardousMaterial = value; } }

		[QueryWriterField("Regulatory Class")]
		[EntityImportExportAttribute("REGULATORYCLASS", 120, "RegulatoryClass")]
		public int RegulatoryClass { get { return this._RegulatoryClass; } set {
			this._RegulatoryClass = value; } }

		[QueryWriterField("Load Rack Display Text")]
		[EntityImportExportAttribute("LOADRACKDISPLAYTEXT", 135, "LoadRackDisplayText")]
		public string LoadRackDisplayText { get { return this._LoadRackDisplayText; } set {
			this.SetString("Load Rack Display Text", 10, value, ref this._LoadRackDisplayText); } }

		[QueryWriterField("Component Tolerance")]
		[EntityImportExportAttribute("COMPONENTTOLERANCE", 130, "ComponentTolerance")]
		public string ComponentTolerance { get { return this._ComponentTolerance.ToString( ); } set {
			this.SetDecimal("Component Tolerance", value, ref this._ComponentTolerance); } }

		[QueryWriterField("Vapor Recovery")]
		[EntityImportExportAttribute("VAPORRECOVERY", 100, "VAPORRECOVERY")]
		public bool VaporRecovery { get { return this._VaporRecovery; } set {
			this._VaporRecovery = value; } }

		[QueryWriterField("Locked Out")]
		[EntityImportExportAttribute("LOCKEDOUT", 70, "LockedOut")]
		public bool LockedOut { get { return this._LockedOut; } set {
			this._LockedOut = value; } }

		[QueryWriterField("Locked Out Reason")]
		[EntityImportExportAttribute("LOCKEDOUTREASON", 107, "LOCKEDOUTREASON")]
		public string LockedOutReason { get { return this._LockedOutReason; } set {
			this.SetString("Locked Out Reason", 80, value, ref this._LockedOutReason); } }

		[QueryWriterField("Locked Out Date", "LockedOutDate")]
		public Date LockedOutDateObject { get { return this._LockedOutDate; } set {
			this._LockedOutDate = value; } }

		[XmlIgnore]
		public string LockedOutDate { get { return this._LockedOutDate.ToString( ); } set {
			this.SetDate("Locked Out Date", value, ref this._LockedOutDate); } }

		[QueryWriterField("Variance Tolerance")]
		[EntityImportExportAttribute("VARIANCETOLERANCE", 120, "VarianceTolerance")]
		public double VarianceTolerance { get { return this._VarianceTolerance; } set {
			this._VarianceTolerance = value; } }

        [QueryWriterField("Dielectric Tolerance")]
        [EntityImportExportAttribute("DIELECTRICTOLERANCE", 120, "DielectricTolerance")]
        public double DielectricTolerance
        {
            get { return this.dielectricTolerance; }
            set { this.dielectricTolerance = value; }
        }

        [QueryWriterField("Load By Weight")]
		[EntityImportExportAttribute("LOADBYWEIGHT", 95, "LoadByWeight")]
		public bool LoadByWeight { get { return this._LoadByWeight; } set {
			this._LoadByWeight = value; } }

		[QueryWriterField("PIDXCode")]
		[EntityImportExportAttribute("PIDXCODE", 65, "PIDXCode")]
		public string PIDXCode { get { return this._PIDXCode; } set {
			this.SetString("PIDXCode", 3, value, ref this._PIDXCode); } }

		[QueryWriterField("PIDXFamilyCode")]
		[EntityImportExportAttribute("PIDXFAMILYCODE", 65, "PIDXFamilyCode")]
		public string PIDXFamilyCode
		{
			get { return this._PIDXFamilyCode; }
			set
			{
					this.SetString("PIDXFamilyCode", 3, value, ref this._PIDXFamilyCode);
			}
		}

		[QueryWriterField("IsEthanol")]
		[EntityImportExportAttribute("ISETHANOL", 65, "IsEthanol")]
		public bool IsEthanol
		{
			get { return this._IsEthanol; }
			set
			{
				this._IsEthanol = value;
			}
		}


		[QueryWriterField("Contamination Prompt Text")]
		[EntityImportExportAttribute("CONTAMINATIONPROMPT", 95, "ContaminationPromptLoadRackText")]
		public string ContaminationPromptLoadRackText { get { return this._ContaminationPromptLoadRackText; } set {
			this.SetString("Contamination Prompt Load Rack Text", 10, value, ref this._ContaminationPromptLoadRackText); } }

		[QueryWriterField("Inhibit Accounting")]
		[EntityImportExportAttribute("INHIBITACCOUNTING", 95, "InhibitAccounting")]
		public bool InhibitAccounting { get { return this._InhibitAccounting; } set {
			this._InhibitAccounting = value; } }

        /// <summary>
        /// Represents the date + time that this product was hidden
        /// A null value indicates the product is not hidden.
        /// Although this field is represented as a datetime it is represented to users
        /// as a checkbox. 
        /// </summary>
        [DataMember]
        public DateTimeOffset? HiddenDate { get; set; }

        /// <summary>
        /// This property is here to support entity import + export of the hidden date.
        /// The Entity import + export functionality doesn't play nice with nullable DateTimeOffsets
        /// </summary>
        [EntityImportExportAttribute("HIDDENDATE", 70, "HIDDENDATE")]
        public string HiddenDateAsString
        {
            get
            {
                if (this.HiddenDate.HasValue)
                {
                    return this.HiddenDate.Value.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.HiddenDate = null;
                }
                else
                {
                    this.HiddenDate = DateTimeOffset.Parse(value);
                }
            }
        }

        [QueryWriterField("Tracking Product ID", false)]
		[EntityImportExportAttribute("TRACKINGPRODUCTID*", 95, "TrackingProductID")]
		public string TrackingProductID { get { return this._TrackingProductID; } set {
			this.SetString("Tracking Product", 30, value, ref this._TrackingProductID); } }

        [DataMember]
        public bool AutomaticCloseout { get; set; }


        [QueryWriterField("User Data 1", "tblProducts.UserData1")]
		[EntityImportExportAttribute("USERDATA1", 70, "UserData1")]
		public string UserData1 { get { return this.UserData[0]; } set {
			this.UserData[0] = value; } }

		[QueryWriterField("User Data 2", "tblProducts.UserData2")]
		[EntityImportExportAttribute("USERDATA2", 70, "UserData2")]
		public string UserData2 { get { return this.UserData[1]; } set {
			this.UserData[1] = value; } }

		[QueryWriterField("User Data 3", "tblProducts.UserData3")]
		[EntityImportExportAttribute("USERDATA3", 70, "UserData3")]
		public string UserData3 { get { return this.UserData[2]; } set {
			this.UserData[2] = value; } }

		[QueryWriterField("User Data 4", "tblProducts.UserData4")]
		[EntityImportExportAttribute("USERDATA4", 70, "UserData4")]
		public string UserData4 { get { return this.UserData[3]; } set {
			this.UserData[3] = value; } }

		[QueryWriterField("User Data 5", "tblProducts.UserData5")]
		[EntityImportExportAttribute("USERDATA5", 70, "UserData5")]
		public string UserData5 { get { return this.UserData[4]; } set {
			this.UserData[4] = value; } }

		[QueryWriterField("User Data 6", "tblProducts.UserData6")]
		[EntityImportExportAttribute("USERDATA6", 70, "USERDATA6")]
		public string UserData6 { get { return this.UserData[5]; } set {
			this.UserData[5] = value; } }

		[QueryWriterField("User Data 7", "tblProducts.UserData7")]
		[EntityImportExportAttribute("USERDATA7", 70, "USERDATA7")]
		public string UserData7 { get { return this.UserData[6]; } set {
			this.UserData[6] = value; } }

		[QueryWriterField("User Data 8", "tblProducts.UserData8")]
		[EntityImportExportAttribute("USERDATA8", 70, "USERDATA8")]
		public string UserData8 { get { return this.UserData[7]; } set {
			this.UserData[7] = value; } }

		[QueryWriterField("Volume Package Size", "tblProducts.VolumePackageSize")]
		public string VolumePackageSize
		{
			get
			{
				if (this._VolumePackageSize.Value == 0 )
				{
					return "";
				}

				return this.GetSIDouble("Volume Package Size", this._VolumePackageSize);
			}
			set
			{
				string processedValue = string.IsNullOrEmpty(value) ? "0" : value;
				this.SetSIDouble("Volume Package Size", processedValue, ref this._VolumePackageSize);
			}
		}

		[QueryWriterField("Mass Package Size", "tblProducts.MassPackageSize")]
		public string MassPackageSize
		{
			get
			{
				if (this._MassPackageSize.Value == 0 )
				{
					return "";
				}
				return this.GetSIDouble("Mass Package Size", this._MassPackageSize);
			}
			set
			{
				string processedValue = string.IsNullOrEmpty(value) ? "0" : value;
				this.SetSIDouble("Mass Package Size", processedValue, ref this._MassPackageSize);
			}
		}

		[QueryWriterField("Tax Code", "tblProducts.TaxCode")]
		[EntityImportExportAttribute("TAXCODE", 70, "TaxCode")]
		public string TaxCode
		{
			get
			{
				return this._TaxCode;
			}
			set
			{
				this.SetString("Tax Code", 10, value, ref this._TaxCode);
			}
		}
		[QueryWriterField("Product Color", "tblProducts.ProductColor")]
		[EntityImportExportAttribute("PRODUCTCOLOR", 90, "ProductColor")]
		public string ProductColor
		{
			get { return this.productColor; }
			set
			{
				this.productColor = "#99ccff";

				if (string.IsNullOrEmpty(value) == false && value.Length == 7)
				{
					this.productColor = value;
				}
			}
		}

		[QueryWriterField("Pattern Color", "tblProducts.PatternColor")]
		[EntityImportExportAttribute("PATTERNCOLOR", 90, "PatternColor")]
		public string PatternColor
		{
			get
			{
				return this.patternColor;
			}
			set
			{
				this.patternColor = "#ffffff";

				if (string.IsNullOrEmpty(value) == false && value.Length == 7)
				{
					this.patternColor = value;
				}
			}
		}

		public int PatternNumber
		{
			get { return this.patternNumber; }
			set { this.patternNumber = value; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType => ENTITY_TYPE.PRODUCT;

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;

		public Guid MasterRecordGuid { get { return this._MasterRecordGuid; } set {
			this._MasterRecordGuid = value; } }

		public Guid AssignedToSiteGuid { get { return this._AssignedToSiteGuid; } set {
			this._AssignedToSiteGuid = value; } }

		public Guid AssignedFromSiteGuid { get { return this._AssignedFromSiteGuid; } set {
			this._AssignedFromSiteGuid = value; } }

		public string AssignedFromSiteId { get { return this._AssignedFromSiteId; } set {
			this._AssignedFromSiteId = value; } }

		public string ProductToolTip
		{
			get
			{
				string toolTip;

				if (this._Code != "" )
				{
					toolTip = this._Code;
				}
				else
				{
					toolTip = this._ID;
				}

				if (this._Description != "" )
				{
					toolTip += ", " + this._Description;
				}

				return toolTip;
			}
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblProducts " +
					"(SiteGuid," +
					"ProductID," +
					"Description," +
					"GenericType," +
					"LookupProductTypeIndex," +
					"ProductColor," +
					"PatternColor," +
					"PatternNumber," +
					"StockResetDate," +
					"StockTrack," +
					"DensityHighLimit," +
					"DensityLowLimit," +
					"DensityDeadband," +
					"ApplyDensityLimits," +
					"TemperatureHiHiLimit," +
					"TemperatureHighLimit," +
					"TemperatureLowLimit," +
					"TemperatureLoLoLimit," +
					"TemperatureDeadband," +
					"ApplyTemperatureLimits," +
					"Bonded," +
					"LowStockWarning," +
					"GroundFuel," +
					"ProductCode," +
					"Price," +
					"AviationFuelFlag," +
					"StandardDensity," +
					"ApplyStandardDensity," +
					"VcfModuleSettings," +
					"ApplyVolumeCorrection," +
					"VolumeUnitIndex," +
					"TemperatureUnitIndex," +
					"DensityUnitIndex," +
					"LevelUnitIndex," +
					"MassUnitIndex," +
					"FlowUnitIndex," +
					"PressureUnitIndex," +
					"VolumeDecimalPlaces," +
					"TemperatureDecimalPlaces," +
					"DensityDecimalPlaces," +
					"LevelDecimalPlaces," +
					"MassDecimalPlaces," +
					"FlowDecimalPlaces," +
					"PressureDecimalPlaces," +
					"VolumePackageSize," +
					"MassPackageSize," +
					"Capitalize," +
					"OctaneNumber," +
					"ReidVaporPressure," +
					"HazardousMaterial," +
					"RegulatoryClass," +
					"LoadRackDisplayText," +
					"ComponentTolerance," +
					"VaporRecovery," +
					"LockedOut," +
					"LockedOutReason," +
					"LockedOutDate," +
					"VarianceTolerance," +
               "DielectricTolerance," +
               "LoadByWeight," +
					"PIDXCode," +
					"ContaminationPromptLoadRackText," +
					"InhibitAccounting," +
               "HiddenDate," +
               "TrackingProductGuid," +
               "AutomaticCloseout," +
               "UserData1," +
					"UserData2," +
					"UserData3," +
					"UserData4," +
					"UserData5," +
					"UserData6," +
					"UserData7," +
					"UserData8," +
					"TaxCode," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy," +
					"ProductGuid," +
					"_MasterRecordGuid," +
               "PIDXFamilyCode," +
					"IsEthanol" +
               ") VALUES (" +
					"@SiteGuid," +
					"@ID," +
					"@Description," +
					"@GenericType," +
					"@ProductType," +
					"@ProductColor," +
					"@PatternColor," +
					"@PatternNumber," +
					"@StockResetDate," +
					"@StockTrack," +
					"@DensityHighLimit," +
					"@DensityLowLimit," +
					"@DensityDeadband," +
					"@ApplyDensityLimits," +
					"@TemperatureHiHiLimit," +
					"@TemperatureHighLimit," +
					"@TemperatureLowLimit," +
					"@TemperatureLoLoLimit," +
					"@TemperatureDeadband," +
					"@ApplyTemperatureLimits," +
					"@Bonded," +
					"@LowStockWarning," +
					"@GroundFuel," +
					"@Code," +
					"@Price," +
					"@AviationFuel," +
					"@StandardDensity," +
					"@ApplyStandardDensity," +
					"@VcfModuleSettings," +
					"@ApplyVolumeCorrection," +
					"@VolumeUnits," +
					"@TemperatureUnits," +
					"@DensityUnits," +
					"@LevelUnits," +
					"@MassUnits," +
					"@FlowUnits," +
					"@PressureUnits," +
					"@VolumeDecimalPlaces," +
					"@TemperatureDecimalPlaces," +
					"@DensityDecimalPlaces," +
					"@LevelDecimalPlaces," +
					"@MassDecimalPlaces," +
					"@FlowDecimalPlaces," +
					"@PressureDecimalPlaces," +
					"@VolumePackageSize," +
					"@MassPackageSize," +
					"@Capitalize," +
					"@OctaneNumber," +
					"@ReidVaporPressure," +
					"@HazardousMaterial," +
					"@RegulatoryClass," +
					"@LoadRackDisplayText," +
					"@ComponentTolerance," +
					"@VaporRecovery," +
					"@LockedOut," +
					"@LockedOutReason," +
					"@LockedOutDate," +
					"@VarianceTolerance," +
               "@DielectricTolerance," +
               "@LoadByWeight," +
					"@PIDXCode," +
					"@ContaminationPromptLoadRackText," +
					"@InhibitAccounting," +
               "@HiddenDate," +
               "@TrackingProductGuid," +
               "@AutomaticCloseout," +
               "@UserData0," +
					"@UserData1," +
					"@UserData2," +
					"@UserData3," +
					"@UserData4," +
					"@UserData5," +
					"@UserData6," +
					"@UserData7," +
					"@TaxCode," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy," +
					"@ProductGuid," +
					"@MasterRecordGuid," +
               "@PIDXFamilyCode," +
					"@IsEthanol" +
               ") ";

			this.CreateBaseCommand(cmd);

			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters["@CreatedBy"].Value = this._CreatedBy;

			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters["@CreatedDate"].Value = this._CreatedDate;

			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ProductGuid"].Value = this._IdentityGuid;

			cmd.Parameters.Add("@MasterRecordGuid", SqlDbType.UniqueIdentifier);
			//This query can only be used to create master record versions.
			this.MasterRecordGuid = this.IdentityGuid;
			cmd.Parameters["@MasterRecordGuid"].Value = this.MasterRecordGuid;

		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblProducts " +
					"SET SiteGuid = @SiteGuid," +
					"ProductID = @ID," +
					"Description = @Description," +
					"GenericType = @GenericType," +
					"LookupProductTypeIndex = @ProductType," +
					"ProductColor = @ProductColor," +
					"PatternColor = @PatternColor," +
					"PatternNumber = @PatternNumber," +
					"StockResetDate = @StockResetDate," +
					"StockTrack = @StockTrack," +
					"DensityHighLimit = @DensityHighLimit," +
					"DensityLowLimit = @DensityLowLimit," +
					"DensityDeadband = @DensityDeadband," +
					"ApplyDensityLimits = @ApplyDensityLimits," +
					"TemperatureHiHiLimit = @TemperatureHiHiLimit," +
					"TemperatureHighLimit = @TemperatureHighLimit," +
					"TemperatureLowLimit = @TemperatureLowLimit," +
					"TemperatureLoLoLimit = @TemperatureLoLoLimit," +
					"TemperatureDeadband = @TemperatureDeadband," +
					"ApplyTemperatureLimits = @ApplyTemperatureLimits," +
					"Bonded = @Bonded," +
					"LowStockWarning = @LowStockWarning," +
					"GroundFuel = @GroundFuel," +
					"ProductCode = @Code," +
					"Price = @Price," +
					"AviationFuelFlag = @AviationFuel," +
					"StandardDensity = @StandardDensity," +
					"ApplyStandardDensity = @ApplyStandardDensity," +
					"VcfModuleSettings = @VcfModuleSettings," +
					"ApplyVolumeCorrection = @ApplyVolumeCorrection," +
					"VolumeUnitIndex = @VolumeUnits," +
					"TemperatureUnitIndex = @TemperatureUnits," +
					"DensityUnitIndex = @DensityUnits," +
					"LevelUnitIndex = @LevelUnits," +
					"MassUnitIndex = @MassUnits," +
					"FlowUnitIndex = @FlowUnits," +
					"PressureUnitIndex = @PressureUnits," +
					"VolumeDecimalPlaces = @VolumeDecimalPlaces," +
					"TemperatureDecimalPlaces = @TemperatureDecimalPlaces," +
					"DensityDecimalPlaces = @DensityDecimalPlaces," +
					"LevelDecimalPlaces = @LevelDecimalPlaces," +
					"MassDecimalPlaces = @MassDecimalPlaces," +
					"FlowDecimalPlaces = @FlowDecimalPlaces," +
					"PressureDecimalPlaces = @PressureDecimalPlaces," +
					"VolumePackageSize = @VolumePackageSize," +
					"MassPackageSize = @MassPackageSize," +
					"Capitalize = @Capitalize," +
					"OctaneNumber = @OctaneNumber," +
					"ReidVaporPressure = @ReidVaporPressure," +
					"HazardousMaterial = @HazardousMaterial," +
					"RegulatoryClass = @RegulatoryClass," +
					"LoadRackDisplayText = @LoadRackDisplayText," +
					"ComponentTolerance = @ComponentTolerance," +
					"VaporRecovery = @VaporRecovery," +
					"LockedOut = @LockedOut," +
					"LockedOutReason = @LockedOutReason," +
					"LockedOutDate = @LockedOutDate," +
					"VarianceTolerance = @VarianceTolerance," +
               "DielectricTolerance = @DielectricTolerance," +
               "LoadByWeight = @LoadByWeight," +
					"PIDXCode = @PIDXCode," +
					"ContaminationPromptLoadRackText = @ContaminationPromptLoadRackText," +
					"InhibitAccounting = @InhibitAccounting," +
               "HiddenDate = @HiddenDate," +
               "TrackingProductGuid = @TrackingProductGuid," +
               "AutomaticCloseout = @AutomaticCloseout," +
               "UserData1 = @UserData0," +
					"UserData2 = @UserData1," +
					"UserData3 = @UserData2," +
					"UserData4 = @UserData3," +
					"UserData5 = @UserData4," +
					"UserData6 = @UserData5," +
					"UserData7 = @UserData6," +
					"UserData8 = @UserData7," +
					"TaxCode = @TaxCode," +
					"UpdatedDate = @UpdatedDate," +
					"UpdatedBy = @UpdatedBy, " +
               "PIDXFamilyCode = @PIDXFamilyCode, " +
					"IsEthanol = @IsEthanol " +
               "WHERE ProductGuid = @ProductGuid";

			this.CreateBaseCommand(cmd);

			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ProductGuid"].Value = this._IdentityGuid;
		}

		//public string PurgeSql
		//{
		//   get
		//   {
		//      string SQL;

		//      SQL = "DELETE FROM tblProducts WHERE ProductGuid = '" + _IdentityGuid.ToString() + "'";

		//      return SQL;
		//   }
		//}
		#endregion

		#region Data Dictionary Keys
		string[ ] IDataDictionary.Keys(SecurityClass security)
		{
			string[ ] keys ={	"Product Exists",
												"Product Not Found",
												"Products",
												"Component",
												"Blend",
												"Additive",
												"Additized",
												"Assigned to Company",
												"and Company Group"
										  };

			return keys;
		}
		#endregion

		#region Constructors

		static ProductClass()
		{
			string s = AppSettingsHelper.GetKeyValue<string>("IrsProductCodesRegEx", "");
			irsProduceCodesRegex = new System.Text.RegularExpressions.Regex(s);
		}

		/// <summary>
		/// This is the default construct for the product class.
		/// </summary>
		public ProductClass( )
		{
			this._StockResetDate = new Date( );
			this._LockedOutDate = new Date( );
			this._DensityHighLimit = new SIDouble(EngineeringUnit.FmdKgM3, NumberFormatInfo.CurrentInfo, 1035);
			this._DensityLowLimit = new SIDouble(EngineeringUnit.FmdKgM3, NumberFormatInfo.CurrentInfo, 624);
			this._DensityDeadband = new SIDifferential(EngineeringUnit.FmdKgM3, NumberFormatInfo.CurrentInfo, 0.0);
			this._TemperatureHiHiLimit = new SIDouble(EngineeringUnit.FmtDegC, NumberFormatInfo.CurrentInfo, 132.222);
			this._TemperatureHighLimit = new SIDouble(EngineeringUnit.FmtDegC, NumberFormatInfo.CurrentInfo, 115.556);
			this._TemperatureLowLimit = new SIDouble(EngineeringUnit.FmtDegC, NumberFormatInfo.CurrentInfo, -151.111);
			this._TemperatureLoLoLimit = new SIDouble(EngineeringUnit.FmtDegC, NumberFormatInfo.CurrentInfo, -167.778);
			this._TemperatureDeadband = new SIDifferential(EngineeringUnit.FmtDegC, NumberFormatInfo.CurrentInfo, 0.0);
			this._LowStockWarning = new SIDouble(EngineeringUnit.FmvMeter3, NumberFormatInfo.CurrentInfo, 0);
			this._StandardDensity = new SIDouble(EngineeringUnit.FmdKgM3, NumberFormatInfo.CurrentInfo, 810);
			this._OctaneNumber = new FMDecimal(NumberFormatInfo.CurrentInfo);
			this._ReidVaporPressure = new SIDouble(EngineeringUnit.FmpKgCm2, NumberFormatInfo.CurrentInfo, 0.0);
			this._ComponentTolerance = new FMDecimal(NumberFormatInfo.CurrentInfo);
			this._Price = new FMDecimal(NumberFormatInfo.CurrentInfo);
			NumberFormatInfo numberFormat = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone( );
			numberFormat.NumberDecimalDigits = this.MassDecimalPlaces;
			this._MassPackageSize = new SIDouble(EngineeringUnit.FmmKg, numberFormat, 0.0);
			numberFormat.NumberDecimalDigits = this.VolumeDecimalPlaces;
			this._VolumePackageSize = new SIDouble(EngineeringUnit.FmvMeter3, numberFormat, 0.0);

			this.Reset( );

			this._TemperatureUnits = EngineeringUnit.FmtDegC;
			this._DensityUnits = EngineeringUnit.FmdKgM3;
			this._PressureUnits = EngineeringUnit.FmpKgCm2;
			this._VolumeUnits = EngineeringUnit.FmvMeter3;

			this._SiteDensityUnits = EngineeringUnit.FmdKgM3;
			this._SiteTemperatureUnits = EngineeringUnit.FmtDegC;
			this._SiteVolumeUnits = EngineeringUnit.FmvMeter3;
			this._SitePressureUnits = EngineeringUnit.FmpKgCm2;

		}

		/// <summary>
		/// This construct untilizes the Site object to initialize site sensitive objects.
		/// </summary>
		/// <param name="Site"></param>
		public ProductClass(SiteClass Site)
		{
			this._StockResetDate = new Date(Site);
			this._LockedOutDate = new Date(Site);
			this._DensityHighLimit = new SIDouble(Site.DensityUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DENSITY), 1035);
			this._DensityLowLimit = new SIDouble(Site.DensityUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DENSITY), 624);
			this._DensityDeadband = new SIDifferential(Site.DensityUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DENSITY), 0.0);
			this._TemperatureHiHiLimit = new SIDouble(Site.TemperatureUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE), 132.222);
			this._TemperatureHighLimit = new SIDouble(Site.TemperatureUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE), 115.556);
			this._TemperatureLowLimit = new SIDouble(Site.TemperatureUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE), -151.111);
			this._TemperatureLoLoLimit = new SIDouble(Site.TemperatureUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE), -167.778);
			this._TemperatureDeadband = new SIDifferential(Site.TemperatureUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE), 0.0);
			this._StandardDensity = new SIDouble(Site.DensityUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DENSITY), 810);
			this._LowStockWarning = new SIDouble(Site.VolumeUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), 0);
			this._OctaneNumber = new FMDecimal(Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
			this._ReidVaporPressure = new SIDouble(Site.PressureUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.PRESSURE), 0.0);
			this._ComponentTolerance = new FMDecimal(Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
			this._Price = new FMDecimal(Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
			NumberFormatInfo numberFormat = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone( );
			numberFormat.NumberDecimalDigits = this.MassDecimalPlaces;
			this._MassPackageSize = new SIDouble(EngineeringUnit.FmmKg, numberFormat, 0.0);
			numberFormat.NumberDecimalDigits = this.VolumeDecimalPlaces;
			this._VolumePackageSize = new SIDouble(EngineeringUnit.FmvMeter3, numberFormat, 0.0);

			this.Reset( );
			this.SetSiteUnits(Site);

		}
		#endregion

		#region Public methods

		public bool ValidateIrsProductCode()
		{
			if (!this.TrackedByIrs)
			{
				return true;
			}
			if (this.TaxCode.Length == 0)
			{
				throw new ProductClassException(string.Format("ProductID:{0} is missing the IRS product code", this.ID));
			}
			if (!irsProduceCodesRegex.IsMatch(this.TaxCode))
			{
				throw new ProductClassException(string.Format("ProductID:{0} is using an invalid IRS product code(\"{1}\")", this.ID, this.TaxCode));
			}
			return true;
		}

		public void SetSiteUnits(SiteClass Site)
		{
			this._SiteDensityUnits = Site.DensityUnits;
			this._SiteTemperatureUnits = Site.TemperatureUnits;
			this._SiteVolumeUnits = Site.VolumeUnits;
			this._SitePressureUnits = Site.PressureUnits;
			this._SiteMassUnits = Site.MassUnits;
			this._SiteFlowUnits = Site.FlowUnits;
			this._SiteLevelUnits = Site.LevelUnits;
		}

		public static ProductCollectionClass LoadForSite(SiteClass siteClass)
		{
			ProductCollectionClass retVal = new ProductCollectionClass();
			ProductClass product;

			product = new ProductClass(siteClass);

			return retVal;
		}

		public static string ProductTypeID(ProductType Type)
		{
			switch ( Type )
			{
				case ProductType.ComponentProduct:
					return "Component";
				case ProductType.BlendProduct:
					return "Blend";
				case ProductType.AdditiveProduct:
					return "Additive";
				case ProductType.AdditizedProduct:
					return "Additized";
				case ProductType.MaxProduct:
					return "{All}";
				default:
					return "Undefined";
			}
		}

		public static void MinorRange(ECorrectionTypeMajor Type, ref int MinimumInt, ref int MaximumInt)
		{

			switch (Type)
			{
				case ECorrectionTypeMajor.CORR_API_C:
				case ECorrectionTypeMajor.CORR_API_C_1980:
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2249:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_API54A;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_API60D;
						break;
					}

				case ECorrectionTypeMajor.CORR_API_F:
				case ECorrectionTypeMajor.CORR_API_F_1980:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_API6A;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_API24E;
						break;
					}

				case ECorrectionTypeMajor.CORR_POLYNOMIAL_F:
				case ECorrectionTypeMajor.CORR_POLYNOMIAL_F_1980:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_POLYNOMIAL;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_POLYNOMIAL;
						break;
					}

				case ECorrectionTypeMajor.CORR_LPG_C:
				case ECorrectionTypeMajor.CORR_LPG_C_1980:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_LPG;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_LPG;
						break;
					}

				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_2004:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_1980:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_C_2004:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_C_1980:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009:
				case ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1555:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_BENZENE;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_350_AROMATIC;
						break;
					}

				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2250:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_ASTM_TABLE55;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_ASTM_TABLE2;
						break;
					}

				case ECorrectionTypeMajor.CORR_JAPAN_CHEMICAL:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_JIS_CHEMICAL1;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_JIS_CHEMICAL2;
						break;
					}

				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2249_TABLE:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_API54A_TABLE;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_API54D_TABLE;
						break;
					}

				case ECorrectionTypeMajor.CORR_GBT:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_APIGBT60A;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_APIGBT60D;
						break;
					}

				case ECorrectionTypeMajor.CORR_GOST:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_3900_85_20C;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_3900_85_20C;
						break;
					}

				case ECorrectionTypeMajor.CORR_ASPHALT:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_D4311DEGC_2004;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_TABLE7;
						break;
					}

				case ECorrectionTypeMajor.CORR_ASTM_D1250_1952:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_D125020DEGC;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_D125020DEGC;
						break;
					}

				case ECorrectionTypeMajor.CORR_ASTM_COMM_2004:
					{
						MinimumInt = (int)ECorrectionTypeMinor.CORR_ALPHA60_SUPPLIED;
						MaximumInt = (int)ECorrectionTypeMinor.CORR_LUBRICATION_OIL;
						break;
					}


				default:
					MinimumInt = 0;
					MaximumInt = 0;
					break;
			}

			return;
		}


		public static string MajorCorrectionMethodID(ECorrectionTypeMajor Type)
		{
			switch (Type)
			{
				case ECorrectionTypeMajor.CORR_NONE:
					return "No VCF";
				case ECorrectionTypeMajor.CORR_NONE_1980:
					return "No VCF 1980";
				case ECorrectionTypeMajor.CORR_API_C:
					return "API °C 1952";
				case ECorrectionTypeMajor.CORR_API_C_1980:
					return "API °C 1980";
				case ECorrectionTypeMajor.CORR_API_F:
					return "API °F 1952";
				case ECorrectionTypeMajor.CORR_API_F_1980:
					return "API °F 1980";
				case ECorrectionTypeMajor.CORR_POLYNOMIAL_F:
					return "Polynomial °F";
				case ECorrectionTypeMajor.CORR_POLYNOMIAL_F_1980:
					return "Polynomial °F 1980";
				case ECorrectionTypeMajor.CORR_LPG_C:
					return "LPG °C";
				case ECorrectionTypeMajor.CORR_LPG_C_1980:
					return "LPG °C 1980";
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_2004:
					return "ASTM D1555 °F 2004";
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_1980:
					return "ASTM D1555 °F 1980";
				case ECorrectionTypeMajor.CORR_ASTM_D1555_C_2004:
					return "ASTM D1555M °C 2004";
				case ECorrectionTypeMajor.CORR_ASTM_D1555_C_1980:
					return "ASTM D1555M °C 1980";
				case ECorrectionTypeMajor.CORR_JAPAN_NONE:
					return "Japan No VCF";
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2249:
					return "Japan JIS 2249 °C 1980";
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2250:
					return "Japan JIS 2250 °C 1967";
				case ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1555:
					return "Japan ASTM D1555 °C";
				case ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1250:
					return "Japan ASTM D1250 °C";
				case ECorrectionTypeMajor.CORR_JAPAN_CHEMICAL:
					return "Japan Chemical °C";
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2249_TABLE:
					return "JIS ASTM 2249 °C 1980 Table";
				case ECorrectionTypeMajor.CORR_GBT:
					return "GB/T";
				case ECorrectionTypeMajor.CORR_GOST:
					return "GOST";
				case ECorrectionTypeMajor.CORR_ASPHALT:
					return "Asphalt";
				case ECorrectionTypeMajor.CORR_ASTM_D1250_1952:
					return "ASTM D1250-1952";
				case ECorrectionTypeMajor.CORR_ASTM_COMM_2004:
					return "Commodity";
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009:
					return "ASTM D1555 °F 2009";
				default:
					return "No VCF";
			}
		}

		public static string MinorCorrectionMethodID(ECorrectionTypeMajor MajorMethod, ECorrectionTypeMinor MinorMethod)
		{
			switch (MajorMethod)
			{
				case ECorrectionTypeMajor.CORR_NONE:
					return "";

				case ECorrectionTypeMajor.CORR_NONE_1980:
					return "";

				case ECorrectionTypeMajor.CORR_API_C:
				case ECorrectionTypeMajor.CORR_API_C_1980:
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2249:
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2249_TABLE:
					{
						switch (MinorMethod)
						{
							case ECorrectionTypeMinor.CORR_API54A:
								return "Table 54A/53A";
							case ECorrectionTypeMinor.CORR_API54B:
								return "Table 54B/53B";
							case ECorrectionTypeMinor.CORR_API54C:
								return "Table 54C";
							case ECorrectionTypeMinor.CORR_API54D:
								return "Table 54D";
							case ECorrectionTypeMinor.CORR_API54A_30:
								return "Table 54A/53A 30°C";
							case ECorrectionTypeMinor.CORR_API54B_30:
								return "Table 54B/53B 30°C";
							case ECorrectionTypeMinor.CORR_API54C_30:
								return "Table 54C 30°C";
							case ECorrectionTypeMinor.CORR_API54D_30:
								return "Table 54D 30°C";
							case ECorrectionTypeMinor.CORR_API60A:
								return "Table 60A";
							case ECorrectionTypeMinor.CORR_API60B:
								return "Table 60B";
							case ECorrectionTypeMinor.CORR_API60D:
								return "Table 60D";
							default:
								return "";
						}
					}

				case ECorrectionTypeMajor.CORR_API_F:
				case ECorrectionTypeMajor.CORR_API_F_1980:
					{
						switch (MinorMethod)
						{
							case ECorrectionTypeMinor.CORR_API6A:
								return "Table 6A/5A";
							case ECorrectionTypeMinor.CORR_API6B:
								return "Table 6B/5B";
							case ECorrectionTypeMinor.CORR_API6C:
								return "Table 6C";
							case ECorrectionTypeMinor.CORR_API6D:
								return "Table 6D";
							case ECorrectionTypeMinor.CORR_API24E:
								return "Table 24E / Table 23E";
							default:
								return "";
						}
					}

				case ECorrectionTypeMajor.CORR_POLYNOMIAL_F:
					return "Polynomial";

				case ECorrectionTypeMajor.CORR_POLYNOMIAL_F_1980:
					return "Polynomial";

				case ECorrectionTypeMajor.CORR_LPG_C:
					return "LPG";

				case ECorrectionTypeMajor.CORR_LPG_C_1980:
					return "LPG";

				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_2004:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_1980:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_C_2004:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_C_1980:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009:
				case ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1555:
					{
						switch (MinorMethod)
						{
							case ECorrectionTypeMinor.CORR_BENZENE:
								return "Benzene";
							case ECorrectionTypeMinor.CORR_TOLUENE:
								return "Toluene";
							case ECorrectionTypeMinor.CORR_M_XYLENE:
								return "Mixed Xylene";
							case ECorrectionTypeMinor.CORR_STYRENE:
								return "Styrene";
							case ECorrectionTypeMinor.CORR_O_XYLENE:
								return "o-Xylene";
							case ECorrectionTypeMinor.CORR_P_XYLENE:
								return "p-Xylene";
							case ECorrectionTypeMinor.CORR_CYCLO_HEXANE:
								return "Cyclo-hexane";
							case ECorrectionTypeMinor.CORR_ETHYL_BENZENE:
								return "Ethyl-benzene";
							case ECorrectionTypeMinor.CORR_CUMENE:
								return "Cumene";
							case ECorrectionTypeMinor.CORR_300_AROMATIC:
								return "300°F/148.9°C Aromatic";
							case ECorrectionTypeMinor.CORR_350_AROMATIC:
								return "350°F/176.7°C Aromatic";
							default:
								return "";
						}
					}

				case ECorrectionTypeMajor.CORR_JAPAN_NONE:
					return "";

				case ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1250:
					{
						switch (MinorMethod)
						{
							case ECorrectionTypeMinor.CORR_ASTM_TABLE55:
								return "Table 55";
							case ECorrectionTypeMinor.CORR_ASTM_TABLE6X_54A:
								return "Table 54A (6X)";
							case ECorrectionTypeMinor.CORR_ASTM_TABLE6X_54B:
								return "Table 54B (6X)";
							case ECorrectionTypeMinor.CORR_ASTM_TABLE2:
								return "Table 2 (54)";
							default:
								return "";
						}
					}

				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2250:
					return "Table 2 (54)";

				case ECorrectionTypeMajor.CORR_JAPAN_CHEMICAL:
					{
						switch (MinorMethod)
						{
							case ECorrectionTypeMinor.CORR_JIS_CHEMICAL1:
								return "Chemical 1";
							case ECorrectionTypeMinor.CORR_JIS_CHEMICAL2:
								return "Chemical 2";
							default:
								return "";
						}
					}

				case ECorrectionTypeMajor.CORR_GBT:
					{
						switch (MinorMethod)
						{
							case ECorrectionTypeMinor.CORR_API6A:
								return "Table 6A/5A";
							case ECorrectionTypeMinor.CORR_API6B:
								return "Table 6B/5B";
							case ECorrectionTypeMinor.CORR_API6D:
								return "Table 6D";
							default:
								return "";
						}
					}

				case ECorrectionTypeMajor.CORR_GOST:
					{
						switch ((ECorrectionTypeMinor)MinorMethod)
						{
							case ECorrectionTypeMinor.CORR_3900_85_20C:
								return "ASTM D1250-1952";
							default:
								return "";
						}
					}

				case ECorrectionTypeMajor.CORR_ASPHALT:
					{
						switch ((ECorrectionTypeMinor)MinorMethod)
						{
							case ECorrectionTypeMinor.CORR_D4311DEGC_2004:
								return "ASTM D4311-04 Deg C";
							case ECorrectionTypeMinor.CORR_D4311DEGF_2004:
								return "ASTM D4311-04 Deg F";
							case ECorrectionTypeMinor.CORR_TABLE7:
								return "ASTM-IP Table 7";
							default:
								return "";
						}
					}

				case ECorrectionTypeMajor.CORR_ASTM_D1250_1952:
					{
						switch (MinorMethod)
						{
							case ECorrectionTypeMinor.CORR_D125020DEGC:
								return "LPG 20C";
							default:
								return "";
						}
					}

				case ECorrectionTypeMajor.CORR_ASTM_COMM_2004:
					{
						switch (MinorMethod)
						{
							case ECorrectionTypeMinor.CORR_ALPHA60_SUPPLIED:
								return "Alpha 60 Supplied";
							case ECorrectionTypeMinor.CORR_CRUDE_OIL:
								return "Crude Oils";
							case ECorrectionTypeMinor.CORR_REFINED_PRODUCTS:
								return "Refined Products";
							case ECorrectionTypeMinor.CORR_LUBRICATION_OIL:
								return "Lubrication Oils";
							default:
								return "";
						}
					}


				default:
					return "";
			}
		}



		public override void Load(Object O)
		{
			this.Reset( );

			if ( typeof(DataSet).IsInstanceOfType(O) )
			{
				DataSet Set = (DataSet) O;
				DataTable Table = Set.Tables[0];

				if ( Table.Rows.Count == 0 )
					return;

				DataRow Row = Table.Rows[0];

				this._IdentityGuid = DataObject.getValue<Guid>(Row["ProductGuid"], Guid.Empty);
				this._MasterRecordGuid = DataObject.getValue<Guid>(Row["_MasterRecordGuid"], Guid.Empty);
				this._SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				this._ID = DataObject.getValue<string>(Row["ProductID"], "");
				this._Description = DataObject.getValue<string>(Row["Description"], "");
				this._GenericType = DataObject.getValue<string>(Row["GenericType"], "");
				this._ProductType = DataObject.getValue<ProductType>(Row["LookupProductTypeIndex"], ProductType.ComponentProduct);
				this.productColor = DataObject.getValue<string>(Row["ProductColor"], "#99ccff");
				this.patternColor = DataObject.getValue<string>(Row["PatternColor"], "#ffffff");
				this.patternNumber = DataObject.getValue<int>(Row["PatternNumber"], 1);
				this._StockResetDate.Value = DataObject.getValue<DateTimeOffset>(Row["StockResetDate"], TimeConverter.Today(this._StockResetDate.StandardName));
				this._StockTrack = DataObject.getValue<bool>(Row["StockTrack"], false);
				this._DensityHighLimit.SIValue = DataObject.getValue<double>(Row["DensityHighLimit"], 1035.0);
				this._DensityLowLimit.SIValue = DataObject.getValue<double>(Row["DensityLowLimit"], 624.0);
				this._DensityDeadband.SIValue = DataObject.getValue<double>(Row["DensityDeadband"], 0.0);
				this._ApplyDensityLimits = DataObject.getValue<bool>(Row["ApplyDensityLimits"], false);
				this._TemperatureHiHiLimit.SIValue = DataObject.getValue<double>(Row["TemperatureHiHiLimit"], 132.222);
				this._TemperatureHighLimit.SIValue = DataObject.getValue<double>(Row["TemperatureHighLimit"], 115.556);
				this._TemperatureLowLimit.SIValue = DataObject.getValue<double>(Row["TemperatureLowLimit"], -151.111);
				this._TemperatureLoLoLimit.SIValue = DataObject.getValue<double>(Row["TemperatureLoLoLimit"], -167.778);
				this._TemperatureDeadband.SIValue = DataObject.getValue<double>(Row["TemperatureDeadband"], 0.0);
				this._ApplyTemperatureLimits = DataObject.getValue<bool>(Row["ApplyTemperatureLimits"], false);
				this._Bonded = DataObject.getValue<bool>(Row["Bonded"], false);
				this._LowStockWarning.SIValue = DataObject.getValue<double>(Row["LowStockWarning"], 0.0);
				this._GroundFuel = DataObject.getValue<bool>(Row["GroundFuel"], false);
				this._Code = DataObject.getValue<string>(Row["ProductCode"], "");
				this._Price.Value = DataObject.getValue<Decimal>(Row["Price"], 0);
				this._AviationFuel = DataObject.getValue<bool>(Row["AviationFuelFlag"], false);
				this._StandardDensity.SIValue = DataObject.getValue<double>(Row["StandardDensity"], 810.0);
				this._ApplyStandardDensity = DataObject.getValue<bool>(Row["ApplyStandardDensity"], false);

				if (!string.IsNullOrEmpty(Row["VcfModuleSettings"] as string))
				{
					try
					{
                        using (MemoryStream memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(Row["VcfModuleSettings"] as string)))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(VcfModuleSettings));
                            this._VcfModuleSettings = serializer.ReadObject(memoryStream) as VcfModuleSettings;
                        }
                    }
					catch
					{
                        // Try catch can be removed after next release after FM12 SP3 as it will be fixed on first start after upgrade to SP3
                        // All products will be resaved with new serializer on first start after upgrade.
                        var serializer = CachingXmlSerializerFactory.Create(typeof(VcfModuleSettings));
                        var stringReader = new StringReader(DataObject.getValue<string>(Row["VcfModuleSettings"], null));
                        this._VcfModuleSettings = (VcfModuleSettings)serializer.Deserialize(stringReader);
                    }
				}

				this._ApplyVolumeCorrection = DataObject.getValue<bool>(Row["ApplyVolumeCorrection"], false);
				this._VolumeUnits = DataObject.getValue<EngineeringUnit>(Row["VolumeUnitIndex"], EngineeringUnit.FmSiteUnits);
				this._TemperatureUnits = DataObject.getValue<EngineeringUnit>(Row["TemperatureUnitIndex"], EngineeringUnit.FmSiteUnits);
				this._DensityUnits = DataObject.getValue<EngineeringUnit>(Row["DensityUnitIndex"], EngineeringUnit.FmSiteUnits);
				this._VolumeDecimalPlaces = DataObject.getValue<byte>(Row["VolumeDecimalPlaces"], 0);
				this._TemperatureDecimalPlaces = DataObject.getValue<byte>(Row["TemperatureDecimalPlaces"], 0);
				this._DensityDecimalPlaces = DataObject.getValue<byte>(Row["DensityDecimalPlaces"], 1);
				this._Capitalize = DataObject.getValue<bool>(Row["Capitalize"], false);
				this._OctaneNumber.Value = (Decimal) DataObject.getValue<double>(Row["OctaneNumber"], 0.0);
				this._ReidVaporPressure.SIValue = DataObject.getValue<double>(Row["ReidVaporPressure"], 0.0);
				this._HazardousMaterial = DataObject.getValue<bool>(Row["HazardousMaterial"], false);
				this._RegulatoryClass = DataObject.getValue<int>(Row["RegulatoryClass"], 0);
				this._LoadRackDisplayText = DataObject.getValue<string>(Row["LoadRackDisplayText"], "");
				this._ComponentTolerance.Value = (Decimal) DataObject.getValue<double>(Row["ComponentTolerance"], 0.0);
				this._VaporRecovery = DataObject.getValue<bool>(Row["VaporRecovery"], false);
				this._LockedOut = DataObject.getValue<bool>(Row["LockedOut"], false);
				this._LockedOutReason = DataObject.getValue<string>(Row["LockedOutReason"], "");
				this._LockedOutDate.Value = DataObject.getValue<DateTimeOffset>(Row["LockedOutDate"], TimeConverter.Today(this._LockedOutDate.StandardName));
				this._VarianceTolerance = DataObject.getValue<double>(Row["VarianceTolerance"], 0.0);
            this.dielectricTolerance = DataObject.getValue<double>(Row["DielectricTolerance"], 0.0);
            this._LoadByWeight = DataObject.getValue<bool>(Row["LoadByWeight"], false);
				this._PIDXCode = DataObject.getValue<string>(Row["PIDXCode"], "");
            this._PIDXFamilyCode = DataObject.getValue<string>(Row["PIDXFamilyCode"], "");
				this._IsEthanol = DataObject.getValue<bool>(Row["IsEthanol"], false);
				this._ContaminationPromptLoadRackText = DataObject.getValue<string>(Row["ContaminationPromptLoadRackText"], "");
				this._InhibitAccounting = DataObject.getValue<bool>(Row["InhibitAccounting"], false);
            this.HiddenDate = DataObject.getValue<DateTimeOffset?>(Row["HiddenDate"], null);
            this.TrackingProductGuid = DataObject.getValue<Guid>(Row["TrackingProductGuid"], Guid.Empty);
				this.UserData[0] = DataObject.getValue<string>(Row["UserData1"], "");
				this.UserData[1] = DataObject.getValue<string>(Row["UserData2"], "");
				this.UserData[2] = DataObject.getValue<string>(Row["UserData3"], "");
				this.UserData[3] = DataObject.getValue<string>(Row["UserData4"], "");
				this.UserData[4] = DataObject.getValue<string>(Row["UserData5"], "");
				this.UserData[5] = DataObject.getValue<string>(Row["UserData6"], "");
				this.UserData[6] = DataObject.getValue<string>(Row["UserData7"], "");
				this.UserData[7] = DataObject.getValue<string>(Row["UserData8"], "");
				this._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				this._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				this._UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], this._CreatedDate);
				this._UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
				this._MassUnits = DataObject.getValue<EngineeringUnit>(Row["MassUnitIndex"], EngineeringUnit.FmSiteUnits);
				this._LevelUnits = DataObject.getValue<EngineeringUnit>(Row["LevelUnitIndex"], EngineeringUnit.FmSiteUnits);
				this._FlowUnits = DataObject.getValue<EngineeringUnit>(Row["FlowUnitIndex"], EngineeringUnit.FmSiteUnits);
				this._PressureUnits = DataObject.getValue<EngineeringUnit>(Row["PressureUnitIndex"], EngineeringUnit.FmSiteUnits);
				this._MassDecimalPlaces = DataObject.getValue<byte>(Row["MassDecimalPlaces"], 0);
				this._LevelDecimalPlaces = DataObject.getValue<byte>(Row["LevelDecimalPlaces"], 2);
				this._FlowDecimalPlaces = DataObject.getValue<byte>(Row["FlowDecimalPlaces"], 1);
				this._PressureDecimalPlaces = DataObject.getValue<byte>(Row["PressureDecimalPlaces"], 2);
				this._VolumePackageSize.SIValue = DataObject.getValue<double>(Row["VolumePackageSize"], 0.0);
				this._MassPackageSize.SIValue = DataObject.getValue<double>(Row["MassPackageSize"], 0.0);
				this._TrackingProductID = DataObject.getValue<string>(Row["TrackingProductID"], "{None}");
				this._TaxCode = DataObject.getValue<string>(Row["TaxCode"], string.Empty);
            this.AutomaticCloseout = DataObject.getValue<bool>(Row["AutomaticCloseout"], false);

            if ( Table.Columns.IndexOf("ASSIGNEDTOSITEGUID") >= 0 ) this.AssignedToSiteGuid = DataObject.getValue<Guid>(Row["ASSIGNEDTOSITEGUID"], Guid.Empty);
				if ( Table.Columns.IndexOf("ASSIGNEDFROMSITEGUID") >= 0 ) this.AssignedFromSiteGuid = DataObject.getValue<Guid>(Row["ASSIGNEDFROMSITEGUID"], Guid.Empty);
				if ( Table.Columns.IndexOf("ASSIGNEDFROMSITEID") >= 0 ) this.AssignedFromSiteId = DataObject.getValue<string>(Row["ASSIGNEDFROMSITEID"], "");

				this._TemperatureDeadband.numberDecimalDigits = this.TemperatureDecimalPlaces;
				this._TemperatureHighLimit.numberDecimalDigits = this.TemperatureDecimalPlaces;
				this._TemperatureHiHiLimit.numberDecimalDigits = this.TemperatureDecimalPlaces;
				this._TemperatureLoLoLimit.numberDecimalDigits = this.TemperatureDecimalPlaces;
				this._TemperatureLowLimit.numberDecimalDigits = this.TemperatureDecimalPlaces;

				if(this._TemperatureUnits != EngineeringUnit.FmSiteUnits)
				{
					this._TemperatureDeadband.Units = this._TemperatureUnits;
					this._TemperatureHighLimit.Units = this._TemperatureUnits;
					this._TemperatureHiHiLimit.Units = this._TemperatureUnits;
					this._TemperatureLoLoLimit.Units = this._TemperatureUnits;
					this._TemperatureLowLimit.Units = this._TemperatureUnits;
				}

				this._DensityHighLimit.numberDecimalDigits = this.DensityDecimalPlaces;
				this._DensityDeadband.numberDecimalDigits = this.DensityDecimalPlaces;
				this._DensityLowLimit.numberDecimalDigits = this.DensityDecimalPlaces;
				this._StandardDensity.numberDecimalDigits = this.DensityDecimalPlaces;

				if (this._DensityUnits != EngineeringUnit.FmSiteUnits)
				{
					this._DensityHighLimit.Units = this._DensityUnits;
					this._DensityHighLimit.Units = this._DensityUnits;
					this._StandardDensity.Units = this._DensityUnits;
				}

			}

			else
			{
				base.Load(O);

				if ( typeof(XmlNode).IsInstanceOfType(O) )
				{
					XmlNode productNode = (XmlNode) O;

					foreach ( XmlNode node in productNode )
					{
						if ( node.Name == "BlendComponents" )
						{
							int sequence = 0;
							foreach ( XmlNode componentNode in node )
							{
								ProductMapClass blendComponent = new ProductMapClass( );
								blendComponent.Load(componentNode);
								blendComponent.Sequence = sequence++;
								this.ComponentCollection.Add(blendComponent);
							}
						}

						else if ( node.Name == "ProductMessages" )
						{
							foreach ( XmlNode productMessageNode in node )
							{
								ApplicationStringMapClass productMessage = new ApplicationStringMapClass( );
								productMessage.Load(productMessageNode);
								this.ProductMessageCollection.Add(productMessage);
							}
						}

						else if ( node.Name == "HazardousMaterialMessages" )
						{
							foreach ( XmlNode hazardousMaterialMessageNode in node )
							{
								ApplicationStringMapClass hazardousMaterialMessage = new ApplicationStringMapClass( );
								hazardousMaterialMessage.Load(hazardousMaterialMessageNode);
								this.HazardousMaterialMessageCollection.Add(hazardousMaterialMessage);
							}
						}


						else if ( node.Name == "AuthorizedCustomers" )
						{
							foreach ( XmlNode authorizedCustomerNode in node )
							{
								ProductMapClass authorizedCustomer = new ProductMapClass( );
								authorizedCustomer.Load(authorizedCustomerNode);

								// Ensure that the authorized customer has an ID. If not, do not
								// add to the collection.
								if ( !string.IsNullOrEmpty(authorizedCustomer.ID) ) this.AuthorizedCustomerCollection.Add(authorizedCustomer);
							}
						}

						else if ( node.Name == "AuthorizedCustomerGroups" )
						{
							foreach ( XmlNode authorizedCustomerGroupNode in node )
							{
								ProductMapClass authorizedCustomerGroup = new ProductMapClass( );
								authorizedCustomerGroup.Load(authorizedCustomerGroupNode);

								// Ensure that the authorized customer group has an ID. If not, do not
								// add to the collection.
								if ( !string.IsNullOrEmpty(authorizedCustomerGroup.ID) ) this.AuthorizedCustomerGroupCollection.Add(authorizedCustomerGroup);
							}
						}

					}
				}
			}

			if (this._DensityUnits != 0 )
			{
				this._DensityHighLimit.Units = this._DensityUnits;
				this._DensityLowLimit.Units = this._DensityUnits;
				this._StandardDensity.Units = this._DensityUnits;
			}
			if (this._TemperatureUnits != 0 )
			{
				this._TemperatureHighLimit.Units = this._TemperatureUnits;
				this._TemperatureHiHiLimit.Units = this._TemperatureUnits;
				this._TemperatureLowLimit.Units = this._TemperatureUnits;
				this._TemperatureLoLoLimit.Units = this._TemperatureUnits;
				this._TemperatureDeadband.Units = this._TemperatureUnits;
			}
			if (this._PressureUnits != 0 )
			{
			}
			if (this._VolumeUnits != 0 ) this._VolumePackageSize.Units = this._VolumeUnits;
			if (this._MassUnits != 0 ) this._MassPackageSize.Units = this._MassUnits;
			if ( !this._MassPackageSize.Format.IsReadOnly ) this._MassPackageSize.Format.NumberDecimalDigits = this._MassDecimalPlaces;
			if ( !this._VolumePackageSize.Format.IsReadOnly ) this._VolumePackageSize.Format.NumberDecimalDigits = this._VolumeDecimalPlaces;
		}

		public override void Store(Object O)
		{
			if ( typeof(XmlNode).IsInstanceOfType(O) )
			{
				base.Store(O);

				XmlNode productNode = (XmlNode) O;

				if (this._ProductType == ProductType.BlendProduct)
				{
					XmlNode BlendComponentsNode = (XmlNode) productNode.OwnerDocument.CreateNode(XmlNodeType.Element, "BlendComponents", null);
					productNode.AppendChild(BlendComponentsNode);
					foreach ( ProductMapClass blendComponent in this.ComponentCollection )
					{
						XmlNode BlendComponentNode = (XmlNode) BlendComponentsNode.OwnerDocument.CreateNode(XmlNodeType.Element, "BlendComponent", null);
						blendComponent.Store(BlendComponentNode);
						BlendComponentsNode.AppendChild(BlendComponentNode);
					}
				}

				XmlNode ProductMessagesNode = (XmlNode) productNode.OwnerDocument.CreateNode(XmlNodeType.Element, "ProductMessages", null);
				productNode.AppendChild(ProductMessagesNode);
				foreach ( ApplicationStringMapClass ProductMessage in this.ProductMessageCollection )
				{
					XmlNode ProductMessageNode = (XmlNode) ProductMessagesNode.OwnerDocument.CreateNode(XmlNodeType.Element, "ProductMessages", null);
					ProductMessage.Store(ProductMessageNode);
					ProductMessagesNode.AppendChild(ProductMessageNode);
				}

				XmlNode HazardousMaterialMessagesNode = (XmlNode) productNode.OwnerDocument.CreateNode(XmlNodeType.Element, "HazardousMaterialMessages", null);
				productNode.AppendChild(HazardousMaterialMessagesNode);
				foreach ( ApplicationStringMapClass HazardousMaterialMessage in this.HazardousMaterialMessageCollection )
				{
					XmlNode HazardousMaterialMessageNode = (XmlNode) HazardousMaterialMessagesNode.OwnerDocument.CreateNode(XmlNodeType.Element, "HazardousMaterialMessages", null);
					HazardousMaterialMessage.Store(HazardousMaterialMessageNode);
					HazardousMaterialMessagesNode.AppendChild(HazardousMaterialMessageNode);
				}


				XmlNode AuthorizedCustomersNode = (XmlNode) productNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AuthorizedCustomers", null);
				productNode.AppendChild(AuthorizedCustomersNode);
				foreach ( ProductMapClass AuthorizedCustomer in this.AuthorizedCustomerCollection )
				{
					XmlNode AuthorizedCustomerNode = (XmlNode) AuthorizedCustomersNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AuthorizedCustomer", null);
					AuthorizedCustomer.Store(AuthorizedCustomerNode);
					AuthorizedCustomersNode.AppendChild(AuthorizedCustomerNode);
				}

				XmlNode AuthorizedCustomerGroupsNode = (XmlNode) productNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AuthorizedCustomerGroups", null);
				productNode.AppendChild(AuthorizedCustomerGroupsNode);
				foreach ( ProductMapClass AuthorizedCustomerGroup in this.AuthorizedCustomerGroupCollection )
				{
					XmlNode AuthorizedCustomerGroupNode = (XmlNode) AuthorizedCustomerGroupsNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AuthorizedCustomerGroup", null);
					AuthorizedCustomerGroup.Store(AuthorizedCustomerGroupNode);
					AuthorizedCustomerGroupsNode.AppendChild(AuthorizedCustomerGroupNode);
				}
			}
		}


		public void QueryWriterSQL(SqlCommand cmd, SecurityClass Security, string selectClause)
		{
			// Construct the query writer sql. The WHERE 1 = 1 is required because the query writer functionality relies on a WHERE being present in the query already
			// If you don't have the WHERE it will just tack on ANDs for each field in the criteria
			cmd.CommandText = "SELECT * FROM (" + selectClause + " ,tblProducts.ProductGuid AS EntityGuid,"
                                    + " VcfModuleSettings.value('(/VcfModuleSettings/AlternateTemperature/Value)[1]', 'float') as AlternateTemperature,"
                                    + " VcfModuleSettings.value('(/VcfModuleSettings/AlternateBasePressure/Value)[1]', 'float') as AlternateBasePressure,"
                                    + " VcfModuleSettings.value('(/VcfModuleSettings/CorrectionMethodSpecific/Value)[1]', 'float') as CorrectionMethodSpecific,"
                                    + " VcfModuleSettings.value('(/VcfModuleSettings/CorrectionMethodType/Value)[1]', 'float') as CorrectionMethodType,"
                                    + " VcfModuleSettings.value('(/VcfModuleSettings/StandardTemperature/Value)[1]', 'float') as StandardTemperature,"
                                    + " dbo.udf_ConvertFromSIUnits((DensityHighLimit - DensityDeadband), DensityUnitIndex, DensityDecimalPlaces) AS DensityHighMinusDeadband,"
			                  + " dbo.udf_ConvertFromSIUnits((DensityLowLimit + DensityDeadband), DensityUnitIndex, DensityDecimalPlaces) AS DensityLowPlusDeadband,"
			                  + " (SELECT ProductID FROM tblProducts P WHERE P.ProductGuid = tblProducts.TrackingProductGuid) AS TrackingProductID"
			                  + " FROM tblProducts"  
			                  + " WHERE tblProducts.ProductGuid IN (SELECT ProductGuid FROM [erv].[udf_GetProductRecordVersions] (@SiteGuid))) QueryWriterProducts "
			                  + " WHERE 1 = 1";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = Security.SiteGuid;
		}


		#endregion

		#region Private and internal methods
		public override void Reset( )
		{
			base.Reset( );

			this._Description = "";
			this._GenericType = "";
			this._ProductType = ProductType.ComponentProduct;
			this.productColor = "#99ccff";
			this.patternColor = "#ffffff";
			this.patternNumber = 1;
			this._StockResetDate.Value = TimeConverter.Today(this._StockResetDate.StandardName);
			this._StockTrack = false;
			this._DensityHighLimit.SIValue = 1035.0;
			this._DensityLowLimit.SIValue = 624.0;
			this._DensityDeadband.SIValue = 0.0;
			this._ApplyDensityLimits = false;
			this._TemperatureHiHiLimit.SIValue = 132.222;
			this._TemperatureHighLimit.SIValue = 115.556;
			this._TemperatureLowLimit.SIValue = -151.111;
			this._TemperatureLoLoLimit.SIValue = -167.778;
			this._TemperatureDeadband.SIValue = 0.0;
			this._ApplyTemperatureLimits = false;
			this._Bonded = false;
			this._LowStockWarning.SIValue = 0.0;
			this._GroundFuel = false;
			this._Code = "";
			this._Price.Value = new Decimal(0.0);
			this._AviationFuel = false;
			this._StandardDensity.SIValue = 810.0;
			this._ApplyStandardDensity = false;
			this._VcfModuleSettings = new VcfModuleSettings();
			this._ApplyVolumeCorrection = false;

			this._VolumeUnits = 0;
			this._DensityUnits = 0;
			this._TemperatureUnits = 0;
			this._LevelUnits = 0;
			this._MassUnits = 0;
			this._FlowUnits = 0;
			this._PressureUnits = 0;
			this._VolumeDecimalPlaces = 0;
			this._TemperatureDecimalPlaces = 0;
			this._DensityDecimalPlaces = 1;
			this._LevelDecimalPlaces = 2;
			this._MassDecimalPlaces = 0;
			this._FlowDecimalPlaces = 1;
			this._PressureDecimalPlaces = 2;
			this._Capitalize = false;
			this._OctaneNumber.Value = new Decimal(0.0);
			this._ReidVaporPressure.SIValue = 0.0;
			this._HazardousMaterial = false;
			this._RegulatoryClass = 0;
			this._LoadRackDisplayText = "";
			this._ComponentTolerance.Value = new Decimal(0.0);
			this._VaporRecovery = false;
			this._LockedOut = false;
			this._LockedOutReason = "";
			this._LockedOutDate.Value = TimeConverter.Today(this._LockedOutDate.StandardName);
			this._VarianceTolerance = 0.0;
         this.dielectricTolerance = 0.0;
         this._LoadByWeight = false;
			this._PIDXCode = "";
         this._PIDXFamilyCode = string.Empty;
			this._IsEthanol = false;
         this._ContaminationPromptLoadRackText = "";
			this._InhibitAccounting = false;
         this.HiddenDate = null;
         this.TrackingProductGuid = Guid.Empty;
			this._TrackingProductID = "{None}";
			this._VolumePackageSize.SIValue = 0.0;
			this._MassPackageSize.SIValue = 0.0;
			this.UserData = new UserDataClass( );
			this._TaxCode = string.Empty;
         this.AutomaticCloseout = false;

         this.ProductMessageCollection = new ApplicationStringMapCollectionClass( );
			this.HazardousMaterialMessageCollection = new ApplicationStringMapCollectionClass( );
			this.ComponentCollection = new ProductMapCollectionClass( );
			this.AuthorizedCustomerCollection = new ProductMapCollectionClass( );
			this.AuthorizedCustomerGroupCollection = new ProductMapCollectionClass( );
		}

		private void CreateBaseCommand(SqlCommand cmd)
		{
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@GenericType", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@ProductType", SqlDbType.Int);
			cmd.Parameters.Add("@ProductColor", SqlDbType.NVarChar, 7);
			cmd.Parameters.Add("@PatternColor", SqlDbType.NVarChar, 7);
			cmd.Parameters.Add("@PatternNumber", SqlDbType.Int);
			cmd.Parameters.Add("@StockResetDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@StockTrack", SqlDbType.Bit);
			cmd.Parameters.Add("@DensityHighLimit", SqlDbType.Float);
			cmd.Parameters.Add("@DensityLowLimit", SqlDbType.Float);
			cmd.Parameters.Add("@DensityDeadband", SqlDbType.Float);
			cmd.Parameters.Add("@ApplyDensityLimits", SqlDbType.Bit);
			cmd.Parameters.Add("@TemperatureHiHiLimit", SqlDbType.Float);
			cmd.Parameters.Add("@TemperatureHighLimit", SqlDbType.Float);
			cmd.Parameters.Add("@TemperatureLowLimit", SqlDbType.Float);
			cmd.Parameters.Add("@TemperatureLoLoLimit", SqlDbType.Float);
			cmd.Parameters.Add("@TemperatureDeadband", SqlDbType.Float);
			cmd.Parameters.Add("@ApplyTemperatureLimits", SqlDbType.Bit);
			cmd.Parameters.Add("@Bonded", SqlDbType.Bit);
			cmd.Parameters.Add("@LowStockWarning", SqlDbType.Float);
			cmd.Parameters.Add("@GroundFuel", SqlDbType.Bit);
			cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 15);
			cmd.Parameters.Add("@Price", SqlDbType.Money);
			cmd.Parameters.Add("@AviationFuel", SqlDbType.Bit);
			cmd.Parameters.Add("@StandardDensity", SqlDbType.Float);
			cmd.Parameters.Add("@ApplyStandardDensity", SqlDbType.Bit);
			cmd.Parameters.Add("@VcfModuleSettings", SqlDbType.Xml);
			cmd.Parameters.Add("@ApplyVolumeCorrection", SqlDbType.Bit);
			cmd.Parameters.Add("@VolumeUnits", SqlDbType.Int);
			cmd.Parameters.Add("@TemperatureUnits", SqlDbType.Int);
			cmd.Parameters.Add("@DensityUnits", SqlDbType.Int);
			cmd.Parameters.Add("@LevelUnits", SqlDbType.Int);
			cmd.Parameters.Add("@MassUnits", SqlDbType.Int);
			cmd.Parameters.Add("@FlowUnits", SqlDbType.Int);
			cmd.Parameters.Add("@PressureUnits", SqlDbType.Int);
			cmd.Parameters.Add("@VolumeDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@TemperatureDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@DensityDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@LevelDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@MassDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@FlowDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@PressureDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@VolumePackageSize", SqlDbType.Float);
			cmd.Parameters.Add("@MassPackageSize", SqlDbType.Float);
			cmd.Parameters.Add("@Capitalize", SqlDbType.Bit);
			cmd.Parameters.Add("@OctaneNumber", SqlDbType.Float);
			cmd.Parameters.Add("@ReidVaporPressure", SqlDbType.Float);
			cmd.Parameters.Add("@HazardousMaterial", SqlDbType.Bit);
			cmd.Parameters.Add("@RegulatoryClass", SqlDbType.Int);
			cmd.Parameters.Add("@LoadRackDisplayText", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@ComponentTolerance", SqlDbType.Float);
			cmd.Parameters.Add("@VaporRecovery", SqlDbType.Bit);
			cmd.Parameters.Add("@LockedOut", SqlDbType.Bit);
			cmd.Parameters.Add("@LockedOutReason", SqlDbType.NVarChar, 80);
			cmd.Parameters.Add("@LockedOutDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@VarianceTolerance", SqlDbType.Float);
         cmd.Parameters.Add("@DielectricTolerance", SqlDbType.Float);
         cmd.Parameters.Add("@LoadByWeight", SqlDbType.Bit);
			cmd.Parameters.Add("@PIDXCode", SqlDbType.NVarChar, 3);
         cmd.Parameters.Add("@PIDXFamilyCode", SqlDbType.NVarChar, 3);
			cmd.Parameters.Add("@IsEthanol", SqlDbType.Bit);
         cmd.Parameters.Add("@ContaminationPromptLoadRackText", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@InhibitAccounting", SqlDbType.Bit);
         cmd.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset);
         cmd.Parameters.Add("@TrackingProductGuid", SqlDbType.UniqueIdentifier);
         cmd.Parameters.Add("@AutomaticCloseout", SqlDbType.Bit);
         cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UserData0", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData1", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData2", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData3", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData4", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData5", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData6", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData7", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@TaxCode", SqlDbType.NVarChar, 10);

			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
			cmd.Parameters["@ID"].Value = this._ID;
			cmd.Parameters["@Description"].Value = this._Description;
			cmd.Parameters["@GenericType"].Value = this._GenericType;
			cmd.Parameters["@ProductType"].Value = ((int)this._ProductType);
			cmd.Parameters["@ProductColor"].Value = this.productColor;
			cmd.Parameters["@PatternColor"].Value = this.patternColor;
			cmd.Parameters["@PatternNumber"].Value = this.patternNumber;
			cmd.Parameters["@StockResetDate"].Value = this._StockResetDate.Value;
			cmd.Parameters["@StockTrack"].Value = this._StockTrack ? 1 : 0;
			cmd.Parameters["@DensityHighLimit"].Value = this._DensityHighLimit.SIValue;
			cmd.Parameters["@DensityLowLimit"].Value = this._DensityLowLimit.SIValue;
			cmd.Parameters["@DensityDeadband"].Value = this._DensityDeadband.SIValue;
			cmd.Parameters["@ApplyDensityLimits"].Value = this._ApplyDensityLimits ? 1 : 0;
			cmd.Parameters["@TemperatureHiHiLimit"].Value = this._TemperatureHiHiLimit.SIValue;
			cmd.Parameters["@TemperatureHighLimit"].Value = this._TemperatureHighLimit.SIValue;
			cmd.Parameters["@TemperatureLowLimit"].Value = this._TemperatureLowLimit.SIValue;
			cmd.Parameters["@TemperatureLoLoLimit"].Value = this._TemperatureLoLoLimit.SIValue;
			cmd.Parameters["@TemperatureDeadband"].Value = this._TemperatureDeadband.SIValue;
			cmd.Parameters["@ApplyTemperatureLimits"].Value = this._ApplyTemperatureLimits ? 1 : 0;
			cmd.Parameters["@Bonded"].Value = this._Bonded ? 1 : 0;
			cmd.Parameters["@LowStockWarning"].Value = this._LowStockWarning.SIValue;
			cmd.Parameters["@GroundFuel"].Value = this._GroundFuel ? 1 : 0;
			cmd.Parameters["@Code"].Value = this._Code;
			cmd.Parameters["@Price"].Value = this._Price.Value;
			cmd.Parameters["@AviationFuel"].Value = this._AviationFuel ? 1 : 0;
			cmd.Parameters["@StandardDensity"].Value = this._StandardDensity.SIValue;
			cmd.Parameters["@ApplyStandardDensity"].Value = this._ApplyStandardDensity ? 1 : 0;

         using (MemoryStream stream = new MemoryStream())
         {
            DataContractSerializer serializer = new DataContractSerializer(typeof(VcfModuleSettings));
            serializer.WriteObject(stream, this._VcfModuleSettings);
            cmd.Parameters["@VcfModuleSettings"].Value = new UTF8Encoding().GetString(stream.ToArray());
         }

			cmd.Parameters["@ApplyVolumeCorrection"].Value = this._ApplyVolumeCorrection ? 1 : 0;
			cmd.Parameters["@VolumeUnits"].Value = ((this._VolumeUnits == 0) ? (object) DBNull.Value : (int)this._VolumeUnits);
			cmd.Parameters["@TemperatureUnits"].Value = ((this._TemperatureUnits == 0) ? (object) DBNull.Value : (int)this._TemperatureUnits);
			cmd.Parameters["@DensityUnits"].Value = ((this._DensityUnits == 0) ? (object) DBNull.Value : (int)this._DensityUnits);
			cmd.Parameters["@LevelUnits"].Value = ((this._LevelUnits == 0) ? (object) DBNull.Value : (int)this._LevelUnits);
			cmd.Parameters["@MassUnits"].Value = ((this._MassUnits == 0) ? (object) DBNull.Value : (int)this._MassUnits);
			cmd.Parameters["@FlowUnits"].Value = ((this._FlowUnits == 0) ? (object) DBNull.Value : (int)this._FlowUnits);
			cmd.Parameters["@PressureUnits"].Value = ((this._PressureUnits == 0) ? (object) DBNull.Value : (int)this._PressureUnits);
			cmd.Parameters["@VolumeDecimalPlaces"].Value = this._VolumeDecimalPlaces;
			cmd.Parameters["@TemperatureDecimalPlaces"].Value = this._TemperatureDecimalPlaces;
			cmd.Parameters["@DensityDecimalPlaces"].Value = this._DensityDecimalPlaces;
			cmd.Parameters["@LevelDecimalPlaces"].Value = this._LevelDecimalPlaces;
			cmd.Parameters["@MassDecimalPlaces"].Value = this._MassDecimalPlaces;
			cmd.Parameters["@FlowDecimalPlaces"].Value = this._FlowDecimalPlaces;
			cmd.Parameters["@PressureDecimalPlaces"].Value = this._PressureDecimalPlaces;
			cmd.Parameters["@VolumePackageSize"].Value = this._VolumePackageSize.SIValue;
			cmd.Parameters["@MassPackageSize"].Value = this._MassPackageSize.SIValue;
			cmd.Parameters["@Capitalize"].Value = (this._Capitalize ? 1 : 0);
			cmd.Parameters["@OctaneNumber"].Value = this._OctaneNumber.Value;
			cmd.Parameters["@ReidVaporPressure"].Value = this._ReidVaporPressure.SIValue;
			cmd.Parameters["@HazardousMaterial"].Value = (this.HazardousMaterial ? 1 : 0);
			cmd.Parameters["@RegulatoryClass"].Value = this._RegulatoryClass;
			cmd.Parameters["@LoadRackDisplayText"].Value = this._LoadRackDisplayText;
			cmd.Parameters["@ComponentTolerance"].Value = this._ComponentTolerance.Value;
			cmd.Parameters["@VaporRecovery"].Value = (this._VaporRecovery ? 1 : 0);
			cmd.Parameters["@LockedOut"].Value = (this._LockedOut ? 1 : 0);
			cmd.Parameters["@LockedOutReason"].Value = this._LockedOutReason;
			cmd.Parameters["@LockedOutDate"].Value = this._LockedOutDate.Value;
			cmd.Parameters["@VarianceTolerance"].Value = this._VarianceTolerance;
         cmd.Parameters["@DielectricTolerance"].Value = this.dielectricTolerance;
         cmd.Parameters["@LoadByWeight"].Value = (this._LoadByWeight ? 1 : 0);
			cmd.Parameters["@PIDXCode"].Value = this._PIDXCode;
         cmd.Parameters["@PIDXFamilyCode"].Value = this._PIDXFamilyCode;
			cmd.Parameters["@IsEthanol"].Value = this._IsEthanol;
			cmd.Parameters["@ContaminationPromptLoadRackText"].Value = this._ContaminationPromptLoadRackText;
			cmd.Parameters["@InhibitAccounting"].Value = (this._InhibitAccounting ? 1 : 0);
         cmd.Parameters["@HiddenDate"].Value = this.HiddenDate ?? (object)DBNull.Value;
         cmd.Parameters["@TrackingProductGuid"].Value = ((this.TrackingProductGuid != Guid.Empty) ? this.TrackingProductGuid : (object) DBNull.Value);
         cmd.Parameters["@AutomaticCloseout"].Value = this.AutomaticCloseout ? 1 : 0;
         cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
			cmd.Parameters["@UserData0"].Value = this.UserData[0];
			cmd.Parameters["@UserData1"].Value = this.UserData[1];
			cmd.Parameters["@UserData2"].Value = this.UserData[2];
			cmd.Parameters["@UserData3"].Value = this.UserData[3];
			cmd.Parameters["@UserData4"].Value = this.UserData[4];
			cmd.Parameters["@UserData5"].Value = this.UserData[5];
			cmd.Parameters["@UserData6"].Value = this.UserData[6];
			cmd.Parameters["@UserData7"].Value = this.UserData[7];

			if ( string.IsNullOrEmpty(this._TaxCode) )
			{
				cmd.Parameters["@TaxCode"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@TaxCode"].Value = this._TaxCode;
			}
		}

		#endregion

		public QueryWriterFieldCollection QueryAliasFields(SecurityClass Security, QueryWriterFieldCollection Fields)
		{
			var userDataFieldCollection =
				FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
					x => x.EnumerateByEntityType(Security, ENTITY_TYPE.PRODUCT, Guid.Empty, false, false));

			QueryWriterFieldCollection newCollection = new QueryWriterFieldCollection(Fields);

			var userFields = from F in newCollection
							 where F.DisplayName.StartsWith("User Data")
							 select F;

			foreach ( var userField in userFields )
			{
				if (this.UpdateFieldName(userField, userDataFieldCollection) == false )
				{
					userField.DisplayName = string.Empty;
				}

			}

			// Remove any blanked out fields.  Wish we could do it above but
			// it disrupts the enumeration.
			for ( int index = newCollection.Count - 1; index >= 0; --index )
			{
				if ( string.IsNullOrEmpty(newCollection[index].DisplayName) )
				{
					newCollection.RemoveAt(index);
				}
			}

			QueryClass.ApplyDataDictionary(Security, newCollection);

			return newCollection;

		}


		public string DetailPageReference( )
		{
			return "FMWebApp\\ProductForm.aspx";
		}

		#region "Parameterized SQL"



		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblProducts WHERE ProductGuid = @ProductGuid";
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ProductGuid"].Value = this.IdentityGuid;
		}


		#endregion
	}
}
