// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseTransactionLineItemDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the BaseTransactionLineItemDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    #region Operational Base Line Item Data Object class.
	/// <summary>
	/// The operational base line item data object.
	/// </summary>
	[DataContract]
	[Serializable]
	[KnownType(typeof(BaseTransactionLineItemDO))]
	public abstract class OperationalBaseLineItemDO : DataObject
	{
		#region Load Rack Operational Values
		[DataMember]
		public bool Density_BadQualityLogged { get; set; }
		[DataMember]
		public bool PresetAmount_BadQualityLogged { get; set; }
		[DataMember]
		public bool Temperature_BadQualityLogged { get; set; }
		[DataMember]
		public bool VCF_BadQualityLogged { get; set; }
		[DataMember]
		public bool Pressure_BadQualityLogged { get; set; }

		public override string getSelectCommand() { return string.Empty; }

		public override string getInsertCommand() { return string.Empty; }

		public override string getDeleteCommand() { return string.Empty; }

		public override string getUpdateCommand() { return string.Empty; }

		#endregion
	}
	#endregion

	/// <summary>
	/// The base transaction line item data object.
	/// </summary>
	[XmlType("BaseTransactionLineItem")]
	[DataContract]
	[Serializable]
	[KnownType(typeof(QuantityDO))]
	[KnownType(typeof(MeterReadingDO))]
	[KnownType(typeof(EquipmentDO))]
	[KnownType(typeof(NoteClass))]
	[KnownType(typeof(ProductMapClass))]
	[KnownType(typeof(LineItemDO))]
	[KnownType(typeof(EngineeringUnit))]
	[KnownType(typeof(ProductClass))]
	public class BaseTransactionLineItemDO : OperationalBaseLineItemDO
	{
		#region Constants
		/// <summary>
		/// The time format.
		/// </summary>
		protected const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		public const string UserDataLineItemKeyPrefix = "TALUD";
		public const string USER_DATA_LINE_ITEM_KEY_01 = UserDataLineItemKeyPrefix + "1";
		public const string USER_DATA_LINE_ITEM_KEY_02 = UserDataLineItemKeyPrefix + "2";
		public const string USER_DATA_LINE_ITEM_KEY_03 = UserDataLineItemKeyPrefix + "3";
		public const string USER_DATA_LINE_ITEM_KEY_04 = UserDataLineItemKeyPrefix + "4";
		public const string USER_DATA_LINE_ITEM_KEY_05 = UserDataLineItemKeyPrefix + "5";
		public const string USER_DATA_LINE_ITEM_KEY_06 = UserDataLineItemKeyPrefix + "6";
		public const string USER_DATA_LINE_ITEM_KEY_07 = UserDataLineItemKeyPrefix + "7";
		public const string USER_DATA_LINE_ITEM_KEY_08 = UserDataLineItemKeyPrefix + "8";
		public const string USER_DATA_LINE_ITEM_KEY_09 = UserDataLineItemKeyPrefix + "9";
		public const string USER_DATA_LINE_ITEM_KEY_10 = UserDataLineItemKeyPrefix + "10";
		public const string USER_DATA_LINE_ITEM_KEY_11 = UserDataLineItemKeyPrefix + "11";
		public const string USER_DATA_LINE_ITEM_KEY_12 = UserDataLineItemKeyPrefix + "12";
		public const string USER_DATA_LINE_ITEM_KEY_13 = UserDataLineItemKeyPrefix + "13";
		public const string USER_DATA_LINE_ITEM_KEY_14 = UserDataLineItemKeyPrefix + "14";
		public const string USER_DATA_LINE_ITEM_KEY_15 = UserDataLineItemKeyPrefix + "15";
		public const string USER_DATA_LINE_ITEM_KEY_16 = UserDataLineItemKeyPrefix + "16";
		public const string USER_DATA_LINE_ITEM_KEY_17 = UserDataLineItemKeyPrefix + "17";
		public const string USER_DATA_LINE_ITEM_KEY_18 = UserDataLineItemKeyPrefix + "18";
		public const string USER_DATA_LINE_ITEM_KEY_19 = UserDataLineItemKeyPrefix + "19";
		public const string USER_DATA_LINE_ITEM_KEY_20 = UserDataLineItemKeyPrefix + "20";
		public const string USER_DATA_LINE_ITEM_KEY_21 = UserDataLineItemKeyPrefix + "21";
		public const string USER_DATA_LINE_ITEM_KEY_22 = UserDataLineItemKeyPrefix + "22";
		public const string USER_DATA_LINE_ITEM_KEY_23 = UserDataLineItemKeyPrefix + "23";
		public const string USER_DATA_LINE_ITEM_KEY_24 = UserDataLineItemKeyPrefix + "24";
		#endregion

		#region Attributes

		[DataMember]
		protected Guid transactionLineItemGuid;

		/// <summary>
		/// This is the line item conjoined GUID for transfer type transactions.
		/// </summary>
		[DataMember]
		protected Guid conjoinedTransactionLineItemGuid;

		/// <summary>
		/// The sequence ID.  Identification of line items.
		/// </summary>
		[DataMember]
		private int? sequenceId;

		[DataMember]
		protected string productCode;
		[DataMember]
		protected string product;
		[DataMember]
		protected string productType = ProductClass.ProductTypeID(DataObjects.ProductType.ComponentProduct);
		[DataMember]
		protected double? productPrice;
		[DataMember]
		protected Guid productGuid;
		[DataMember]
		protected string customs;
		[DataMember]
		protected QuantityDO quantity;
		[DataMember]
		protected double? vcf;
		[DataMember]
		protected double? pressure;
		[DataMember]
		protected double? temperature;
		[DataMember]
		protected double? density;

		[DataMember]
		protected string contractNumber;
		[DataMember]
		protected string clin;

		[DataMember]
		protected int? armNumber;
		[DataMember]
		protected int? lineNumber;
		[DataMember]
		protected string operatorID;
		[DataMember]
		protected Guid operatorPersonnelGuid;

		[DataMember]
		protected string batchNumber;
		[DataMember]
		protected string documentNumber;
		[DataMember]
		protected double? lineFill;
		[DataMember]
		protected double? bottomVolume;
		[DataMember]
		protected double? netCapacity;
		[DataMember]
		protected string tankStatus;

		[DataMember]
		protected string pit;
		[DataMember]
		protected DateTimeOffset? requestedDateTime;
		[DataMember]
		protected DateTimeOffset? dispatchedDateTime;
		[DataMember]
		protected DateTimeOffset? acknowledgedDateTime;
		[DataMember]
		protected DateTimeOffset? onLocationTime;
		[DataMember]
		protected DateTimeOffset? validationDateTime;
		[DataMember]
		protected DateTimeOffset? completionDateTime;

		[DataMember]
		protected double? receiptVariance;
		[DataMember]
		protected double? differentialPressure;
		[DataMember]
		protected double? loadRackVariance;

		[DataMember]
		protected string requestedBy;
		[DataMember]
		protected double? freezePoint;

		[DataMember]
		protected EquipmentDO destinationEQ;
		[DataMember]
		protected string destinationCompartmentID;
		[DataMember]
		protected Guid destinationCompartmentEquipmentGuid;
		[DataMember]
		protected EquipmentDO sourceEQ;
		[DataMember]
		protected string sourceCompartmentID;
		[DataMember]
		protected Guid sourceCompartmentEquipmentGuid;
		[DataMember]
		protected MeterReadingDO meterReading;

		[DataMember]
		protected double? presetAmount;
		[DataMember]
		protected string additiveProfileID;
		[DataMember]
		protected Guid additiveProfileGuid;
		[DataMember]
		protected string storageLocationID;
		[DataMember]
		protected Guid storageLocationTankGuid;
		[DataMember]
		protected string meterID;
		[DataMember]
		protected Guid meterGuid;

		[DataMember]
		protected TransactionStatus status;
		[DataMember]
		protected bool deleteFlag;

		[DataMember]
		protected string sCustomerProductName;
		[DataMember]
		protected string sCustomerProduceCode;
		[DataMember]
		protected EngineeringUnit cuEngineeringUnitsIndex;

		[DataMember]
		protected double GrossQtyReceived;
		[DataMember]
		protected double GrossQtyRemaining;
		[DataMember]
		protected double NetQtyReceived;
		[DataMember]
		protected double NetQtyRemaining;
		[DataMember]
		protected double MassQtyReceived;
		[DataMember]
		protected double MassQtyRemaining;

		[DataMember]
		protected double totalValue;
		[DataMember]
		protected double totalPriceWithTax;
		[DataMember]
		protected double valueRemaining;

		[DataMember]
		protected EngineeringUnit volumeUnits =  EngineeringUnit.FmvMeter3;
		[DataMember]
		protected EngineeringUnit levelUnits = EngineeringUnit.FmlMeter;
		[DataMember]
		protected EngineeringUnit densityUnits = EngineeringUnit.FmdKgM3;
		[DataMember]
		protected EngineeringUnit temperatureUnits = EngineeringUnit.FmtDegC;
		[DataMember]
		protected EngineeringUnit massUnits = EngineeringUnit.FmmKg;
		[DataMember]
		protected EngineeringUnit flowUnits = EngineeringUnit.FmvfM3Sec;
		[DataMember]
		protected EngineeringUnit pressureUnits = EngineeringUnit.FmpPa;

		[DataMember]
		protected byte volumeDecimalPlaces = 0;
		[DataMember]
		protected byte levelDecimalPlaces = 0;
		[DataMember]
		protected byte densityDecimalPlaces = 0;
		[DataMember]
		protected byte temperatureDecimalPlaces = 0;
		[DataMember]
		protected byte massDecimalPlaces = 0;
		[DataMember]
		protected byte flowDecimalPlaces = 0;
		[DataMember]
		protected byte pressureDecimalPlaces = 0;

		[DataMember]
		protected Guid orderReferenceTransactionLineItemGuid;

		[DataMember]
		protected bool coaWaiver;
		[DataMember]
		protected string coaNote;
		[DataMember]
		protected string coaID;
		[DataMember]
		protected TransactionQuality quality = TransactionQuality.Usable;

		[DataMember]
		protected double? tax1;
		[DataMember]
		protected double? tax2;
		[DataMember]
		protected double? tax3;
		[DataMember]
		protected double? tax4;
		[DataMember]
		protected double? tax5;

		[DataMember]
		protected string loadingLocationID;
		[DataMember]
		protected Guid loadingLocationStationGuid;

		[DataMember]
		protected string specialInstructionsNote;

		[DataMember]
		protected Guid specialInstructionsNoteIdentityGuid;

		[DataMember]
		protected PRODUCT_MAP_TYPE specialInstructionsNoteProductMapType;

		[DataMember]
		protected ProductMapClass _SplashBlendingMap = null;

		[DataMember]
		protected List<SubLineItemDO> subLineItems;

		[DataMember]
		protected DateTime? closeoutDate;

		[DataMember]
		private bool? improperAdditization;
		[DataMember]
		private bool? brokenBlend;

		[DataMember]
		private bool? contaminatePrompt;
		[DataMember]
		private bool? compartmentsPreviouslyLoaded;
		[DataMember]
		private bool? compartmentsEmpty;

		// vthompson 05-21-2008
		[DataMember]
		protected bool flag01;
		[DataMember]
		protected bool flag02;
		[DataMember]
		protected bool flag03;
		[DataMember]
		protected bool flag04;
		[DataMember]
		protected bool flag05;
		[DataMember]
		protected bool flag06;

		[DataMember]
		protected double? number01;
		[DataMember]
		protected double? number02;
		[DataMember]
		protected double? number03;
		[DataMember]
		protected double? number04;
		[DataMember]
		protected double? number05;
		[DataMember]
		protected double? number06;

		[DataMember]
		protected double? odometerHours;
		[DataMember]
		protected DateTimeOffset? endDeliveryDate;
		[DataMember]
		protected DateTimeOffset? requestedDeliveryDate;

		// vthompson 05-22-2008
		[DataMember]
		protected string invoiceNumber;
		[DataMember]
		protected string invoiceLineNumber;
		[DataMember]
		protected double? alternativeGrossVolume;
		[DataMember]
		protected double? alternativeNetVolume;
		[DataMember]
		protected double? tankLevel;
		[DataMember]
		protected int? tankLevelUnits;
		[DataMember]
		protected int? alternativeUnits;

		// 07-09-2008 vt: Adding generic date fields for ADF
		[DataMember]
		protected DateTimeOffset? date01;
		[DataMember]
		protected DateTimeOffset? date02;
		[DataMember]
		protected DateTimeOffset? date03;
		[DataMember]
		protected DateTimeOffset? date04;

		// 07-10-2008 vt: Adding NonDomesticPrice, CurrencyUnit, ExchangeRate, and QualityTestNumber
		[DataMember]
		protected double? nonDomesticPrice;
		[DataMember]
		protected double? exchangeRate;
		[DataMember]
		protected double? odometer;
		[DataMember]
		protected Guid currencyGuid;
		[DataMember]
		protected string qualityTestNumber;

		// 9/22/2008
		// Adding DeliveryLocation
		[DataMember]
		protected string deliveryLocation;

		// 9/16/2009
		// Added for BSME Dispatch
		[DataMember]
		protected double? variance;
		[DataMember]
		protected bool? partialFill;

		//8/10/2010
		//Added for Missile fuels
		[DataMember]
		protected double? massPackageSize;
		[DataMember]
		protected double? volumePackageSize;

		[DataMember]
		protected bool cleanLineProduct;
		[DataMember]
		protected bool cleanLineDeductProduct;
		[DataMember]
		protected double? cleanLineDeductQuantity;
		[DataMember]
		protected double? cleanLinePackQuantity;

		// START 2014-Apr-04 p carpenter added to support expanded FSR fields.  Initially the following fields are only supported within
		// secondaryDefault, primaryDistribution and fillstand

		[DataMember]
		protected bool? meterStartObtainedAutomaticallyFlag;
		[DataMember]
		protected bool? meterStopObtainedAutomaticallyFlag;
		[DataMember]
		protected bool? dualFuelingModeFlag ;
		[DataMember]
		protected double? flowRate;
		[DataMember]
		protected double? engineRunTime;
		[DataMember]
		protected double? fuelCompressionFactor;
		[DataMember]
		protected double? hydrantPressure;
		[DataMember]
		protected string mobileDeviceID;
		[DataMember]
		protected Guid? mobileDeviceGuid;
		[DataMember]
		protected bool? dualFuelingPrimaryFlag;
		[DataMember]
		protected string temperatureQualityStatus;
		[DataMember]
		protected bool? netVolumeIndicator;


		// END   2014-Apr-04 p carpenter added to support expanded FSR fields.

		#endregion Attributes

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseTransactionLineItemDO"/> class. 
		/// This is the default constructor for the Base Transaction Line Item DO class.
		/// </summary>
		public BaseTransactionLineItemDO()
		{
			this.subLineItems				= new List<SubLineItemDO>();
			this.destinationEQ				= new EquipmentDO();
			this.sourceEQ					= new EquipmentDO();
			this.meterReading				= new MeterReadingDO();
			this.quantity					= new QuantityDO();
			this.massPackageSize			= new double?();
			this.volumePackageSize			= new double?();
			this.WacCalculated				= false;
			this.cleanLineDeductQuantity	= new double?();
			this.cleanLinePackQuantity		= new double?();
			this.VCF						= 1;
            this.armNumber                  = new int?();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseTransactionLineItemDO"/> class. 
		/// This is a copy constructor for the Base Transaction Line Item DO class.
		/// NOTE: It is incomplete!
		/// </summary>
		/// <param name="lineItemDO">
		/// Line Item data object.
		/// </param>
		public BaseTransactionLineItemDO(BaseTransactionLineItemDO lineItemDO)
		{
            if (lineItemDO == null)
            {
                throw new ArgumentNullException(nameof(lineItemDO));
            }

            this.transactionLineItemGuid=lineItemDO.transactionLineItemGuid;
            this.conjoinedTransactionLineItemGuid = lineItemDO.conjoinedTransactionLineItemGuid;
            this.sequenceId = lineItemDO.sequenceId;
            this.productCode = lineItemDO.productCode;
            this.product = lineItemDO.product;
            this.productType = lineItemDO.productType;
            this.productPrice = lineItemDO.productPrice;
            this.productGuid = lineItemDO.productGuid;
            this.customs = lineItemDO.customs;
            this.quantity = new QuantityDO(lineItemDO.quantity);
            this.vcf = lineItemDO.vcf;
            this.temperature = lineItemDO.temperature;
            this.density = lineItemDO.density;
            this.contractNumber = lineItemDO.contractNumber;
            this.clin = lineItemDO.clin;
            this.armNumber = lineItemDO.armNumber;
            this.lineNumber = lineItemDO.lineNumber;
            this.operatorID = lineItemDO.operatorID;
            this.operatorPersonnelGuid = lineItemDO.operatorPersonnelGuid;
            this.batchNumber = lineItemDO.batchNumber;
            this.documentNumber = lineItemDO.documentNumber;
            this.lineFill = lineItemDO.lineFill;
            this.bottomVolume = lineItemDO.bottomVolume;
            this.netCapacity = lineItemDO.netCapacity;
            this.tankStatus = lineItemDO.tankStatus;
            this.pit = lineItemDO.pit;
            this.requestedDateTime = lineItemDO.requestedDateTime;
            this.dispatchedDateTime = lineItemDO.dispatchedDateTime;
            this.acknowledgedDateTime = lineItemDO.acknowledgedDateTime;
            this.onLocationTime = lineItemDO.onLocationTime;
            this.validationDateTime = lineItemDO.validationDateTime;
            this.completionDateTime = lineItemDO.completionDateTime;
            this.receiptVariance = lineItemDO.receiptVariance;
            this.differentialPressure = lineItemDO.differentialPressure;
            this.loadRackVariance = lineItemDO.loadRackVariance;
            this.requestedBy = lineItemDO.requestedBy;
            this.freezePoint = lineItemDO.freezePoint;
            this.destinationEQ=new EquipmentDO(lineItemDO.destinationEQ);
            this.destinationCompartmentID = lineItemDO.destinationCompartmentID;
            this.destinationCompartmentEquipmentGuid = lineItemDO.destinationCompartmentEquipmentGuid;
            this.sourceEQ=new EquipmentDO(lineItemDO.sourceEQ);
            this.sourceCompartmentID = lineItemDO.sourceCompartmentID;
            this.sourceCompartmentEquipmentGuid = lineItemDO.sourceCompartmentEquipmentGuid;
            this.meterReading=new MeterReadingDO(lineItemDO.meterReading);
            this.presetAmount = lineItemDO.presetAmount;
            this.additiveProfileID = lineItemDO.additiveProfileID;
            this.additiveProfileGuid = lineItemDO.additiveProfileGuid;
            this.storageLocationID = lineItemDO.storageLocationID;
            this.storageLocationTankGuid = lineItemDO.storageLocationTankGuid;
            this.meterID = lineItemDO.meterID;
            this.meterGuid = lineItemDO.meterGuid;
            this.status = lineItemDO.status;
            this.deleteFlag = lineItemDO.deleteFlag;
            this.sCustomerProductName = lineItemDO.sCustomerProductName;
            this.sCustomerProduceCode = lineItemDO.sCustomerProduceCode;
            this.cuEngineeringUnitsIndex = lineItemDO.cuEngineeringUnitsIndex;
            this.GrossQtyReceived = lineItemDO.GrossQtyReceived;
            this.GrossQtyRemaining = lineItemDO.GrossQtyRemaining;
            this.NetQtyReceived = lineItemDO.NetQtyReceived;
            this.NetQtyRemaining = lineItemDO.NetQtyRemaining;
            this.MassQtyReceived = lineItemDO.MassQtyReceived;
            this.MassQtyRemaining = lineItemDO.MassQtyRemaining;
            this.totalValue = lineItemDO.totalValue;
            this.totalPriceWithTax = lineItemDO.totalPriceWithTax;
            this.valueRemaining = lineItemDO.valueRemaining;
            this.volumeUnits = lineItemDO.volumeUnits;
            this.levelUnits = lineItemDO.levelUnits;
            this.densityUnits = lineItemDO.densityUnits;
            this.temperatureUnits = lineItemDO.temperatureUnits;
            this.massUnits = lineItemDO.massUnits;
            this.flowUnits = lineItemDO.flowUnits;
            this.pressureUnits = lineItemDO.pressureUnits;
            this.volumeDecimalPlaces = lineItemDO.volumeDecimalPlaces;
            this.levelDecimalPlaces = lineItemDO.levelDecimalPlaces;
            this.densityDecimalPlaces = lineItemDO.densityDecimalPlaces;
            this.temperatureDecimalPlaces = lineItemDO.temperatureDecimalPlaces;
            this.massDecimalPlaces = lineItemDO.massDecimalPlaces;
            this.flowDecimalPlaces = lineItemDO.flowDecimalPlaces;
            this.pressureDecimalPlaces = lineItemDO.pressureDecimalPlaces;
            this.orderReferenceTransactionLineItemGuid = lineItemDO.orderReferenceTransactionLineItemGuid;
            this.coaWaiver = lineItemDO.coaWaiver;
            this.coaNote = lineItemDO.coaNote;
            this.coaID = lineItemDO.coaID;
            this.quality = lineItemDO.quality;
            this.tax1 = lineItemDO.tax1;
            this.tax2 = lineItemDO.tax2;
            this.tax3 = lineItemDO.tax3;
            this.tax4 = lineItemDO.tax4;
            this.tax5 = lineItemDO.tax5;
            this.loadingLocationID = lineItemDO.loadingLocationID;
            this.loadingLocationStationGuid = lineItemDO.loadingLocationStationGuid;
            this.specialInstructionsNote = lineItemDO.specialInstructionsNote;
            this.specialInstructionsNoteIdentityGuid = lineItemDO.specialInstructionsNoteIdentityGuid;
            this.specialInstructionsNoteProductMapType = lineItemDO.specialInstructionsNoteProductMapType;
            this._SplashBlendingMap = lineItemDO._SplashBlendingMap;
            this.subLineItems=new List<SubLineItemDO>(lineItemDO.subLineItems);
            this.closeoutDate = lineItemDO.closeoutDate;
            this.improperAdditization = lineItemDO.improperAdditization;
            this.brokenBlend = lineItemDO.brokenBlend;
            this.contaminatePrompt = lineItemDO.contaminatePrompt;
            this.compartmentsPreviouslyLoaded = lineItemDO.compartmentsPreviouslyLoaded;
            this.compartmentsEmpty = lineItemDO.compartmentsEmpty;
            this.flag01 = lineItemDO.flag01;
            this.flag02 = lineItemDO.flag02;
            this.flag03 = lineItemDO.flag03;
            this.flag04 = lineItemDO.flag04;
            this.flag05 = lineItemDO.flag05;
            this.flag06 = lineItemDO.flag06;
            this.number01 = lineItemDO.number01;
            this.number02 = lineItemDO.number02;
            this.number03 = lineItemDO.number03;
            this.number04 = lineItemDO.number04;
            this.number05 = lineItemDO.number05;
            this.number06 = lineItemDO.number06;
            this.odometerHours = lineItemDO.odometerHours;
            this.endDeliveryDate = lineItemDO.endDeliveryDate;
            this.requestedDeliveryDate = lineItemDO.requestedDeliveryDate;
            this.invoiceNumber = lineItemDO.invoiceNumber;
            this.invoiceLineNumber = lineItemDO.invoiceLineNumber;
            this.alternativeGrossVolume = lineItemDO.alternativeGrossVolume;
            this.alternativeNetVolume = lineItemDO.alternativeNetVolume;
            this.tankLevel = lineItemDO.tankLevel;
            this.tankLevelUnits = lineItemDO.tankLevelUnits;
            this.alternativeUnits = lineItemDO.alternativeUnits;
            this.date01 = lineItemDO.date01;
            this.date02 = lineItemDO.date02;
            this.date03 = lineItemDO.date03;
            this.date04 = lineItemDO.date04;
            this.nonDomesticPrice = lineItemDO.nonDomesticPrice;
            this.exchangeRate = lineItemDO.exchangeRate;
            this.odometer = lineItemDO.odometer;
            this.currencyGuid = lineItemDO.currencyGuid;
            this.qualityTestNumber = lineItemDO.qualityTestNumber;
            this.deliveryLocation = lineItemDO.deliveryLocation;
            this.variance = lineItemDO.variance;
            this.partialFill = lineItemDO.partialFill;
            this.massPackageSize = lineItemDO.massPackageSize;
            this.volumePackageSize = lineItemDO.volumePackageSize;
            this.cleanLineProduct = lineItemDO.cleanLineProduct;
            this.cleanLineDeductProduct = lineItemDO.cleanLineDeductProduct;
            this.cleanLineDeductQuantity = lineItemDO.cleanLineDeductQuantity;
            this.cleanLinePackQuantity = lineItemDO.cleanLinePackQuantity;
            this.meterStartObtainedAutomaticallyFlag = lineItemDO.meterStartObtainedAutomaticallyFlag;
            this.meterStopObtainedAutomaticallyFlag = lineItemDO.meterStopObtainedAutomaticallyFlag;
            this.dualFuelingModeFlag = lineItemDO.dualFuelingModeFlag;
            this.flowRate = lineItemDO.flowRate;
            this.engineRunTime = lineItemDO.engineRunTime;
            this.fuelCompressionFactor = lineItemDO.fuelCompressionFactor;
            this.hydrantPressure = lineItemDO.hydrantPressure;
            this.mobileDeviceID = lineItemDO.mobileDeviceID;
            this.mobileDeviceGuid = lineItemDO.mobileDeviceGuid;
            this.dualFuelingPrimaryFlag = lineItemDO.dualFuelingPrimaryFlag;
            this.temperatureQualityStatus = lineItemDO.temperatureQualityStatus;
			this.netVolumeIndicator = lineItemDO.netVolumeIndicator;
    }
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether (Weighted Average Calculation) WAC calculated.
		/// </summary>
		[XmlIgnore]
		[DataMember]
		public bool WacCalculated { get; set; }

		/// <summary>
		/// Gets or sets the sequence ID.
		/// </summary>
		[QueryWriterField("Line Sequence Number", "tblTransactionLineItems.SequenceID")]
		public int? SequenceId
		{
			get { return this.sequenceId; }
			set { this.sequenceId = value; }
		}

		/// <summary>
		/// Gets or sets the product.
		/// </summary>
		[QueryWriterField("Product", "tblTransactionLineItems.Product")]
		public string Product
		{
			get { return this.product; }
			set { this.product = value; }
		}

		/// <summary>
		/// Gets or sets the product code.
		/// </summary>
		[QueryWriterField("Product Code", "tblTransactionLineItems.ProductCode")]
		public string ProductCode
		{
			get { return this.productCode; }
			set { this.productCode = value; }
		}

		/// <summary>
		/// Gets or sets the product type.
		/// </summary>
		[QueryWriterField("Product Type", "tblTransactionLineItems.ProductType")]
		public string ProductType
		{
			get { return this.productType; }
			set { this.productType = value; }
		}

		/// <summary>
		/// Represents the unit price of a product
		/// </summary>
		[QueryWriterField("Product Price", "tblTransactionLineItems.ProductPrice")]
		public double? ProductPrice
		{
			get { return this.productPrice; }
			set { this.productPrice = value; }
		}

		/// <summary>
		/// This method causes the Product Price property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeProductPrice()
		{
			return this.productPrice.HasValue;
		}

		[XmlIgnore]
		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		[QueryWriterField("Customs", "tblTransactionLineItems.Customs")]
		public string Customs
		{
			get { return this.customs; }
			set { this.customs = value; }
		}

		public QuantityDO Quantity
		{
			get { return this.quantity; }
			set { this.quantity = value; }
		}

      [XmlIgnore]
		[QueryWriterField("Net Quantity", "tblTransactionLineItems.NetQuantity", false)]
		public double NetInventoryChange
		{
			get { return this.Quantity.NetInventoryChange; }
			set { this.Quantity.NetInventoryChange = value; }
		}

		[XmlIgnore]
		[QueryWriterField("Delivered Net Quantity", "tblTransactionLineItems.DeliveredNetQuantity", false)]
		public double DeliveredNetInventoryChange
		{
			get { return this.Quantity.DeliveredNetInventoryChange; }
			set { this.Quantity.DeliveredNetInventoryChange = value; }
		}


		[XmlIgnore]
      [QueryWriterField("Mass Quantity", "tblTransactionLineItems.MassQuantity", false)]
		public double MassQuantityChange
		{
			get { return this.Quantity.MassInventoryChange; }
			set { this.Quantity.MassInventoryChange = value; }
		}

      [XmlIgnore]
      [QueryWriterField("Gross Quantity", "tblTransactionLineItems.GrossQuantity", false)]
		public double GrossInventoryChange
		{ 
			get { return this.Quantity.GrossInventoryChange; } 
			set { this.Quantity.GrossInventoryChange = value; }
		}

		[XmlIgnore]
		[QueryWriterField("Delivered Gross Quantity", "tblTransactionLineItems.DeliveredGrossQuantity", false)]
		public double DeliveredGrossInventoryChange
		{
			get { return this.Quantity.DeliveredGrossInventoryChange; }
			set { this.Quantity.DeliveredGrossInventoryChange = value; }
		}

		[QueryWriterField("VCF", "tblTransactionLineItems.VCF")]
		public double? VCF
		{
			get { return this.vcf; }
			set { this.vcf = value; }
		}

		[QueryWriterField("Pressure", "tblTransactionLineItems.Pressure")]
		public double? Pressure
		{
			get { return this.pressure; }
			set { this.pressure = value; }
		}


		/// <summary>
		/// This method causes the VCF property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeVCF( )
		{
			return this.vcf.HasValue;
		}

		/// <summary>
		/// This method causes the Pressure property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializePressure()
		{
			return this.pressure.HasValue;
		}



		[QueryWriterField("Temperature", "tblTransactionLineItems.Temperature", false)]
		public double? Temperature
		{
			get { return this.temperature; }
			set { this.temperature = value; }
		}

		/// <summary>
		/// This method causes the Temperature property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeTemperature( )
		{
			return this.temperature.HasValue;
		}

		[QueryWriterField("Density", "tblTransactionLineItems.Density", false)]
		public double? Density
		{
			get { return this.density; }
			set { this.density = value; }
		}

		/// <summary>
		/// This method causes the Density property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeDensity( )
		{
			return this.density.HasValue;
		}

		[QueryWriterField("Requested By", "tblTransactionLineItems.RequestedBy")]
		public string RequestedBy
		{
			get { return this.requestedBy; }
			set { this.requestedBy = value; }
		}

		[QueryWriterField("Line Document Number", "tblTransactionLineItems.DocumentNumber")]
		public string DocumentNumber
		{
			get { return this.documentNumber; }
			set { this.documentNumber = value; }
		}

		[QueryWriterField("Preset Amount", "tblTransactionLineItems.PresetAmount", false)]
		public double? PresetAmount
		{
			get { return this.presetAmount; }
			set { this.presetAmount = value; }
		}

		/// <summary>
		/// This method causes the Preset Amount property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializePresetAmount( )
		{
			return this.presetAmount.HasValue;
		}

		[QueryWriterField("Line Item Status", "tblTransactionLineItems.LookupTransactionStatusIndex")]
		public TransactionStatus Status
		{
			get { return this.status; }
			set { this.status = value; }
		}

		[QueryWriterField("Line Item Guid", "tblTransactionLineItems.TransactionLineItemGuid")]
		[XmlIgnore]
		public Guid TransactionLineItemGuid
		{
			get { return this.transactionLineItemGuid; }
			set { this.transactionLineItemGuid = value; }
		}

		/// <summary>
		/// Gets or sets the line item conjoined GUID that is used for transfer type
		/// transactions.
		/// </summary>
		[XmlIgnore]
		public Guid ConjoinedTransactionLineItemGuid
		{
			get { return this.conjoinedTransactionLineItemGuid; }
			set { this.conjoinedTransactionLineItemGuid = value; }
        }

        /// <summary>
        /// Represents the conjoined line item's user data record
        /// This is used when saving conjoined transactions to ensure that the 
        /// conjoined transaction line item's user data record is updated.
        /// </summary>
        [XmlIgnore]
        [DataMember]
		public Guid ConjoinedTransactionLineItemUserDataGuid { get; set; }

		[QueryWriterField("Customer Product Name", "tblTransactionLineItems.CustomerProductName")]
		public string CustomerProductName
		{
			get { return this.sCustomerProductName; }
			set { this.sCustomerProductName = value; }
		}

		[QueryWriterField("Customer Product Code", "tblTransactionLineItems.CustomerProductCode")]
		public string CustomerProductCode
		{
			get { return this.sCustomerProduceCode; }
			set { this.sCustomerProduceCode = value; }
		}

		[XmlIgnore]
		public EngineeringUnit EngineeringUnitsIndex
		{
			get { return this.cuEngineeringUnitsIndex; }
			set { this.cuEngineeringUnitsIndex = value; }
		}

		public double GrossQuantityReceived
		{
			get { return this.GrossQtyReceived; }
			set { this.GrossQtyReceived = value; }
		}

		public double GrossQuantityRemaining
		{
			get { return this.GrossQtyRemaining; }
			set { this.GrossQtyRemaining = value; }
		}

		public double NetQuantityReceived
		{
			get { return this.NetQtyReceived; }
			set { this.NetQtyReceived = value; }
		}

		public double NetQuantityRemaining
		{
			get { return this.NetQtyRemaining; }
			set { this.NetQtyRemaining = value; }
		}

		public double MassQuantityReceived
		{
			get { return this.MassQtyReceived; }
			set { this.MassQtyReceived = value; }
		}

		public double MassQuantityRemaining
		{
			get { return this.MassQtyRemaining; }
			set { this.MassQtyRemaining = value; }
		}

		public double TotalValue
		{
			get { return this.totalValue; }
			set { this.totalValue = value; }
		}

		public double ValueRemaining
		{
			get { return this.valueRemaining; }
			set { this.valueRemaining = value; }
		}

		[XmlIgnore]
		public Guid OrderReferenceTransactionLineItemGuid
		{
			get { return this.orderReferenceTransactionLineItemGuid; }
			set { this.orderReferenceTransactionLineItemGuid = value; }
		}

		[QueryWriterField("COA Waiver", "tblTransactionLineItems.COAWaiver")]
		public bool COAWaiver
		{
			get { return this.coaWaiver; }
			set { this.coaWaiver = value; }
		}

		[QueryWriterField("COA Note", "tblTransactionLineItems.COANote")]
		public string COANote
		{
			get { return this.coaNote; }
			set { this.coaNote = value; }
		}

		[QueryWriterField("COA ID", "tblTransactionLineItems.COAID")]
		public string COAID
		{
			get { return this.coaID; }
			set { this.coaID = value; }
		}

		[QueryWriterField("Quality", "tblTransactionLineItems.LookupQualityIndex")]
		public TransactionQuality Quality
		{
			get { return this.quality; }
			set { this.quality = value; }
		}

		[QueryWriterField("Line Item Tax 1", "tblTransactionLineItems.Tax1")]
		public double? Tax1
		{
			get { return this.tax1; }
			set { this.tax1 = value; }
		}

		/// <summary>
		/// This method causes the Tax 1 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeTax1( )
		{
			return this.tax1.HasValue;
		}

		[QueryWriterField("Line Item Tax 2", "tblTransactionLineItems.Tax2")]
		public double? Tax2
		{
			get { return this.tax2; }
			set { this.tax2 = value; }
		}

		/// <summary>
		/// This method causes the Tax 2 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeTax2( )
		{
			return this.tax2.HasValue;
		}

		[QueryWriterField("Line Item Tax 3", "tblTransactionLineItems.Tax3")]
		public double? Tax3
		{
			get { return this.tax3; }
			set { this.tax3 = value; }
		}

		/// <summary>
		/// This method causes the Tax 3 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeTax3( )
		{
			return this.tax3.HasValue;
		}

		[QueryWriterField("Line Item Tax 4", "tblTransactionLineItems.Tax4")]
		public double? Tax4
		{
			get { return this.tax4; }
			set { this.tax4 = value; }
		}

		/// <summary>
		/// This method causes the Tax 4 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeTax4( )
		{
			return this.tax4.HasValue;
		}

		[QueryWriterField("Line Item Tax 5", "tblTransactionLineItems.Tax5")]
		public double? Tax5
		{
			get { return this.tax5; }
			set { this.tax5 = value; }
		}

		/// <summary>
		/// This method causes the Tax 5 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeTax5( )
		{
			return this.tax5.HasValue;
		}

		[XmlIgnore]
		public string SpecialInstructions
		{
			get
			{
				if (this.specialInstructionsNote != null)
				{
					return this.specialInstructionsNote;
				}

				return "";
			}
		}

		[XmlIgnore]
		public string SpecialInstructionsNote
		{
			get { return this.specialInstructionsNote; }
			set { this.specialInstructionsNote = value; }
		}

		[XmlIgnore]
		public Guid SpecialInstructionsNoteGuid
		{
			get
			{
				if (this.specialInstructionsNoteIdentityGuid != null)
				{
					return this.specialInstructionsNoteIdentityGuid;
				}

				return Guid.Empty;
			}

			set
			{
				if (value != null)
				{
					this.specialInstructionsNoteIdentityGuid = value;
				}
				else
				{
					this.specialInstructionsNoteIdentityGuid = Guid.Empty;
				}
			}
		}

		[XmlIgnore]
		public PRODUCT_MAP_TYPE SpecialInstructionsNoteProductMapType
		{
			get { return this.specialInstructionsNoteProductMapType; }
			set { this.specialInstructionsNoteProductMapType = value; }
		}

		[XmlElement("CloseoutDateString")]
		public string CloseoutDateString
		{
			get
			{
				return this.closeoutDate == null ? string.Empty : ((DateTime)this.closeoutDate).ToString(TimeFormat);
			}

			set
			{
				this.closeoutDate = (value == string.Empty) ? (DateTime?)null : DateTime.ParseExact(value, TimeFormat, null).Date;
			}
		}


		[XmlIgnore]
		public DateTime? CloseoutDate
		{
			get { return this.closeoutDate; }
			set { this.closeoutDate = value; }
		}

		[QueryWriterField("Line Improper Additization", "tblTransactionLineItems.ImproperAdditization")]
		protected bool? ImproperAdditization
		{
			get { return this.improperAdditization; }
			set { this.improperAdditization = value; }
		}

		/// <summary>
		/// This method causes the Improper Additization property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeImproperAdditization( )
		{
			return this.improperAdditization.HasValue;
		}

		[QueryWriterField("Line Broken Blend", "tblTransactionLineItems.BrokenBlend")]
		public bool? BrokenBlend
		{
			get { return this.brokenBlend; }
			set { this.brokenBlend = value; }
		}

		/// <summary>
		/// This method causes the Broken Blend property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeBrokenBlend( )
		{
			return this.brokenBlend.HasValue;
		}

		[QueryWriterField("Contaminate Prompt", "tblTransactionLineItems.ContimiatePrompt")]
		protected bool? ContaminatePrompt
		{
			get { return this.contaminatePrompt; }
			set { this.contaminatePrompt = value; }
		}

		/// <summary>
		/// This method causes the Contaminate Prompt property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeContaminatePrompt( )
		{
			return this.contaminatePrompt.HasValue;
		}

		[QueryWriterField("Line Compartments Previously Loaded", "tblTransactionLineItems.CompartmentsPreviouslyLoaded")]
		public bool? CompartmentsPreviouslyLoaded
		{
			get { return this.compartmentsPreviouslyLoaded; }
			set { this.compartmentsPreviouslyLoaded = value; }
		}

		/// <summary>
		/// This method causes the Compartments Previously Loaded property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeCompartmentsPreviouslyLoaded( )
		{
			return this.compartmentsPreviouslyLoaded.HasValue;
		}

		[QueryWriterField("Line Compartments Empty", "tblTransactionLineItems.CompartmentsEmpty")]
		public bool? CompartmentsEmpty
		{
			get { return this.compartmentsEmpty; }
			set { this.compartmentsEmpty = value; }
		}

		/// <summary>
		/// This method causes the Compartments Empty property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeCompartmentsEmpty( )
		{
			return this.compartmentsEmpty.HasValue;
		}

		// vthompson 05-21-2008
		[QueryWriterField("Line Flag 1", "tblTransactionLineItems.Flag01")]
		public bool Flag01
		{
			get { return this.flag01; }
			set { this.flag01 = value; }
		}

		[QueryWriterField("Line Flag 2", "tblTransactionLineItems.Flag02")]
		public bool Flag02
		{
			get { return this.flag02; }
			set { this.flag02 = value; }
		}

		[QueryWriterField("Line Flag 3", "tblTransactionLineItems.Flag03")]
		public bool Flag03
		{
			get { return this.flag03; }
			set { this.flag03 = value; }
		}

		[QueryWriterField("Line Flag 4", "tblTransactionLineItems.Flag04")]
		public bool Flag04
		{
			get { return this.flag04; }
			set { this.flag04 = value; }
		}

		[QueryWriterField("Line Flag 5", "tblTransactionLineItems.Flag05")]
		public bool Flag05
		{
			get { return this.flag05; }
			set { this.flag05 = value; }
		}

		[QueryWriterField("Line Flag 6", "tblTransactionLineItems.Flag06")]
		public bool Flag06
		{
			get { return this.flag06; }
			set { this.flag06 = value; }
		}

		[QueryWriterField("Line Number 1", "tblTransactionLineItems.Number01")]
		public double? Number01
		{
			get { return this.number01; }
			set { this.number01 = value; }
		}

		/// <summary>
		/// This method causes the Number 1 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber01( )
		{
			return this.number01.HasValue;
		}

		[QueryWriterField("Line Number 2", "tblTransactionLineItems.Number02")]
		public double? Number02
		{
			get { return this.number02; }
			set { this.number02 = value; }
		}

		/// <summary>
		/// This method causes the Number 2 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber02( )
		{
			return this.number02.HasValue;
		}

		[QueryWriterField("Line Number 3", "tblTransactionLineItems.Number03")]
		public double? Number03
		{
			get { return this.number03; }
			set { this.number03 = value; }
		}

		/// <summary>
		/// This method causes the Number 3 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber03( )
		{
			return this.number03.HasValue;
		}

		[QueryWriterField("Line Number 4", "tblTransactionLineItems.Number04")]
		public double? Number04
		{
			get { return this.number04; }
			set { this.number04 = value; }
		}

		/// <summary>
		/// This method causes the Number 4 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber04( )
		{
			return this.number04.HasValue;
		}

		[QueryWriterField("Line Number 5", "tblTransactionLineItems.Number05")]
		public double? Number05
		{
			get { return this.number05; }
			set { this.number05 = value; }
		}

		/// <summary>
		/// This method causes the Number 5 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber05( )
		{
			return this.number05.HasValue;
		}

		[QueryWriterField("Line Number 6", "tblTransactionLineItems.Number06")]
		public double? Number06
		{
			get { return this.number06; }
			set { this.number06 = value; }
		}

		/// <summary>
		/// This method causes the Number 6 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber06( )
		{
			return this.number06.HasValue;
		}

		[QueryWriterField("Odometer Hours", "tblTransactionLineItems.OdometerHours")]
		public double? OdometerHours
		{
			get { return this.odometerHours; }
			set { this.odometerHours = value; }
		}

		/// <summary>
		/// This method causes the Odometer Hours property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeOdometerHours( )
		{
			return this.odometerHours.HasValue;
		}

		[XmlElement("EndDeliveryDateString")]
		public string EndDeliveryDateString
		{
			get
			{
				return this.endDeliveryDate == null ? string.Empty : ((DateTimeOffset)this.endDeliveryDate).ToString(TimeFormat);
			}

			set
			{
				this.endDeliveryDate = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}


		[QueryWriterField("Line Delivery Date", "tblTransactionLineItems.EndDeliveryDate")]
		[XmlIgnore]
		public DateTimeOffset? EndDeliveryDate
		{
			get { return this.endDeliveryDate; }
			set { this.endDeliveryDate = value; }
		}

		[XmlElement("RequestedDeliveryDateString")]
		public string RequestedDeliveryDateString
		{
			get
			{
				return this.requestedDeliveryDate == null ? string.Empty : ((DateTimeOffset)this.requestedDeliveryDate).ToString(TimeFormat);
			}

			set
			{
				this.requestedDeliveryDate = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Line Requested Date", "tblTransactionLineItems.RequestedDeliveryDate")]
		[XmlIgnore]
		public DateTimeOffset? RequestedDeliveryDate
		{
			get { return this.requestedDeliveryDate; }
			set { this.requestedDeliveryDate = value; }
		}

		// vthompson 05-22-2008
		[QueryWriterField("Line Invoice Number", "tblTransactionLineItems.InvoiceNumber")]
		public string InvoiceNumber
		{
			get { return this.invoiceNumber; }
			set { this.invoiceNumber = value; }
		}

		public string InvoiceLineNumber
		{
			get { return this.invoiceLineNumber; }
			set { this.invoiceLineNumber = value; }
		}

		[QueryWriterField("Alternative Gross Volume", "tblTransactionLineItems.AlternativeGrossVolume")]
		public double? AlternativeGrossVolume
		{
			get { return this.alternativeGrossVolume; }
			set { this.alternativeGrossVolume = value; }
		}

		/// <summary>
		/// This method causes the Alternative Gross Volume property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeAlternativeGrossVolume( )
		{
			return this.alternativeGrossVolume.HasValue;
		}

		[QueryWriterField("Alternative Net Volume", "tblTransactionLineItems.AlternativeNetVolume")]
		public double? AlternativeNetVolume
		{
			get { return this.alternativeNetVolume; }
			set { this.alternativeNetVolume = value; }
		}

		/// <summary>
		/// This method causes the Alternative Net Volume property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeAlternativeNetVolume( )
		{
			return this.alternativeNetVolume.HasValue;
		}

		[QueryWriterField("Alternative Units", "tblTransactionLineItems.AlternativeUnits")]
		public int? AlternativeUnits
		{
			get { return this.alternativeUnits; }
			set { this.alternativeUnits = value; }
		}

		/// <summary>
		/// This method causes the Alternative Units property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeAlternativeUnits( )
		{
			return this.alternativeUnits.HasValue;
		}

		[QueryWriterField("Tank Level", "tblTransactionLineItems.TankLevel")]
		public double? TankLevel
		{
			get { return this.tankLevel; }
			set { this.tankLevel = value; }
		}

		/// <summary>
		/// This method causes the Tank Level property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeTankLevel( )
		{
			return this.tankLevel.HasValue;
		}

		[QueryWriterField("Tank Level Units", "tblTransactionLineItems.TankLevelUnits")]
		public int? TankLevelUnits
		{
			get { return this.tankLevelUnits; }
			set { this.tankLevelUnits = value; }
		}

		/// <summary>
		/// This method causes the Tank Level Units property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeTankLevelUnits( )
		{
			return this.tankLevelUnits.HasValue;
		}

		[XmlElement("Date01String")]
		public string Date01String
		{
			get
			{
				return this.date01 == null ? string.Empty : ((DateTimeOffset)this.date01).ToString(TimeFormat);
			}

			set
			{
				this.date01 = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Line Date 1", "tblTransactionLineItems.Date01")]
		[XmlIgnore]
		public DateTimeOffset? Date01
		{
			get { return this.date01; }
			set { this.date01 = value; }
		}

		[XmlElement("Date02String")]
		public string Date02String
		{
			get
			{
				return this.date02 == null ? string.Empty : ((DateTimeOffset)this.date02).ToString(TimeFormat);
			}

			set
			{
				this.date02 = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Line Date 2", "tblTransactionLineItems.Date02")]
		[XmlIgnore]
		public DateTimeOffset? Date02
		{
			get { return this.date02; }
			set { this.date02 = value; }
		}

		[XmlElement("Date03String")]
		public string Date03String
		{
			get
			{
				return this.date03 == null ? string.Empty : ((DateTimeOffset)this.date03).ToString(TimeFormat);
			}

			set
			{
				this.date03 = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Line Date 3", "tblTransactionLineItems.Date03")]
		[XmlIgnore]
		public DateTimeOffset? Date03
		{
			get { return this.date03; }
			set { this.date03 = value; }
		}

		[XmlElement("Date04String")]
		public string Date04String
		{
			get
			{
				return this.date04 == null ? string.Empty : ((DateTimeOffset)this.date04).ToString(TimeFormat);
			}

			set
			{
				this.date04 = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Line Date 4", "tblTransactionLineItems.Date04")]
		[XmlIgnore]
		public DateTimeOffset? Date04
		{
			get { return this.date04; }
			set { this.date04 = value; }
		}

		[QueryWriterField("Non Domestic Price", "tblTransactionLineItems.NonDomesticPrice")]
		public double? NonDomesticPrice
		{
			get { return this.nonDomesticPrice; }
			set { this.nonDomesticPrice = value; }
		}

		/// <summary>
		/// This method causes the Non Domestic Price property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNonDomesticPrice( )
		{
			return this.nonDomesticPrice.HasValue;
		}

		[QueryWriterField("Exchange Rate", "tblTransactionLineItems.ExchangeRate")]
		public double? ExchangeRate
		{
			get { return this.exchangeRate; }
			set { this.exchangeRate = value; }
		}

		/// <summary>
		/// This method causes the Exchange Rate property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeExchangeRate( )
		{
			return this.exchangeRate.HasValue;
		}

		[QueryWriterField("Currency Unit", "tblTransactionLineItems.CurrencyGuid")]
		[XmlIgnore] public Guid CurrencyGuid
		{
			get { return this.currencyGuid; }
			set { this.currencyGuid = value; }
		}

		[QueryWriterField("Quality Test Number", "tblTransactionLineItems.QualityTestNumber")]
		public string QualityTestNumber
		{
			get { return this.qualityTestNumber; }
			set { this.qualityTestNumber = value; }
		}

		[QueryWriterField("Odometer", "tblTransactionLineItems.Odometer")]
		public double? Odometer
		{
			get { return this.odometer; }
			set { this.odometer = value; }
		}

		/// <summary>
		/// This method causes the Odometer property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeOdometer( )
		{
			return this.odometer.HasValue;
		}

		// vthompson 9/22/2008
		[QueryWriterField("Line Delivery Location", "tblTransactionLineItems.DeliveryLocation")]
		public string DeliveryLocation
		{
			get { return this.deliveryLocation; }
			set { this.deliveryLocation = value; }
		}

		/// <summary>
		/// Returns the unit price * quanity plus taxes
		/// </summary>
		public double TotalPriceWithTax
		{
			get { return this.totalPriceWithTax; }
			set { this.totalPriceWithTax = value; }
		}

		/// <summary>
		/// Returns the volume unit 
		/// </summary>
		[QueryWriterField("Volume Unit", "tblTransactionLineItems.VolumeUnit", GenerateSelect = false)]
		public EngineeringUnit VolumeUnits
		{
			get { return this.volumeUnits; }
			set { this.volumeUnits = value; }
		}

		public EngineeringUnit LevelUnits
		{
			get { return this.levelUnits; }
			set { this.levelUnits = value; }
		}

		public EngineeringUnit TemperatureUnits
		{
			get { return this.temperatureUnits; }
			set { this.temperatureUnits = value; }
		}

		public EngineeringUnit DensityUnits
		{
			get { return this.densityUnits; }
			set { this.densityUnits = value; }
		}

		public EngineeringUnit MassUnits
		{
			get { return this.massUnits; }
			set { this.massUnits = value; }
		}

		public EngineeringUnit FlowUnits
		{
			get { return this.flowUnits; }
			set { this.flowUnits = value; }
		}

		public EngineeringUnit PressureUnits
		{
			get { return this.pressureUnits; }
			set { this.pressureUnits = value; }
		}

		[XmlIgnore]
		public byte VolumeDecimalPlaces
		{
			get { return this.volumeDecimalPlaces; }
			set { this.volumeDecimalPlaces = value; }
		}

		[XmlIgnore]
		public byte LevelDecimalPlaces
		{
			get { return this.levelDecimalPlaces; }
			set { this.levelDecimalPlaces = value; }
		}

		[XmlIgnore]
		public byte TemperatureDecimalPlaces
		{
			get { return this.temperatureDecimalPlaces; }
			set { this.temperatureDecimalPlaces = value; }
		}

		[XmlIgnore]
		public byte DensityDecimalPlaces
		{
			get { return this.densityDecimalPlaces; }
			set { this.densityDecimalPlaces = value; }
		}

		[XmlIgnore]
		public byte MassDecimalPlaces
		{
			get { return this.massDecimalPlaces; }
			set { this.massDecimalPlaces = value; }
		}

		[XmlIgnore]
		public byte FlowDecimalPlaces
		{
			get { return this.flowDecimalPlaces; }
			set { this.flowDecimalPlaces = value; }
		}

		[XmlIgnore]
		public byte PressureDecimalPlaces
		{
			get { return this.pressureDecimalPlaces; }
			set { this.pressureDecimalPlaces = value; }
		}

		[QueryWriterField("Variance", "tblTransactionLineItems.Variance")]
		public double? Variance
		{
			get { return this.variance; }
			set { this.variance = value; }
		}

		/// <summary>
		/// This method causes the Variance property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeVariance( )
		{
			return this.variance.HasValue;
		}

		[QueryWriterField("Partial Fill", "tblTransactionLineItems.PartialFill")]
		public bool? PartialFill
		{
			get { return this.partialFill; }
			set { this.partialFill = value; }
		}

		/// <summary>
		/// This method causes the Partial Fill property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializePartialFill( )
		{
			return this.partialFill.HasValue;
		}

		public double? MassPackageSize
		{
			get { return this.massPackageSize; }
			set { this.massPackageSize = value; }
		}

		/// <summary>
		/// This method causes the MassPackageSize property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeMassPackageSize( )
		{
			return this.massPackageSize.HasValue;
		}

		[DataMember]
		public double? VolumePackageSize
		{
			get { return this.volumePackageSize; }
			set { this.volumePackageSize = value; }
		}

		/// <summary>
		/// This method causes the Volume Package Size property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeVolumePackageSize( )
		{
			return this.volumePackageSize.HasValue;
		}

		public bool CleanLineProduct
		{
			get { return this.cleanLineProduct; }
			set { this.cleanLineProduct = value; }
		}

		public bool CleanLineDeductProduct
		{
			get { return this.cleanLineDeductProduct; }
			set { this.cleanLineDeductProduct = value; }
		}

		public double? CleanLinePackQuantity
		{
			get { return this.cleanLinePackQuantity; }
			set { this.cleanLinePackQuantity = value; }
		}

		/// <summary>
		/// This method causes the Clean Line Pack Quantity property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeCleanLinePackQuantity( )
		{
			return this.cleanLinePackQuantity.HasValue;
		}

		public double? CleanLineDeductQuantity
		{
			get { return this.cleanLineDeductQuantity; }
			set { this.cleanLineDeductQuantity = value; }
		}

		/// <summary>
		/// This method causes the Clean Line Deduct Quantity property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeCleanLineDeductQuantity( )
		{
			return this.cleanLineDeductQuantity.HasValue;
		}
		#endregion Properties
	}
}
