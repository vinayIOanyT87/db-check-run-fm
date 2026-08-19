// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubLineItemDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SubLineItemDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using System.Xml.Schema;
	using System.Xml.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using Varec.CommonComponents.VolumeCorrection;


	/// <summary>
	/// The sub-line item data object.
	/// </summary>
	[XmlType("SubLineItem")]
	[Serializable]
	[DataContract]
	public class SubLineItemDO : OperationalBaseLineItemDO
	{
		#region Attributes
		[DataMember]
		public Guid TransactionSubLineItemGuid { get; set; }
		[DataMember]
		public Guid ConjoinedTransactionSubLineItemGuid { get; set; }
		[DataMember]
		public int? SequenceId { get; set; }
		[DataMember]
		protected string productCode;
		[DataMember]
		protected string product;
		[DataMember]
		protected string productType;
		[DataMember]
		protected Guid productGuid;
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
		protected string customs;
		[DataMember]
		protected TransactionStatus status;
		[DataMember]
		protected int? armNumber;
		[DataMember]
		protected int? lineNumber;
		[DataMember]
		protected string batchNumber;
		[DataMember]
		protected double? lineFill;
		[DataMember]
		protected double? bottomVolume;
		[DataMember]
		protected double? netCapacity;
		[DataMember]
		protected string tankStatus;
		[DataMember]
		protected MeterReadingDO meterReading;
		[DataMember]
		protected double? freezePoint;
		[DataMember]
		protected double? differentialPressure;
		[DataMember]
		protected double? dosageRate;
		[DataMember]
		protected double? presetAmount;
		[DataMember]
		protected string storageLocationID;
		[DataMember]
		protected Guid storageLocationTankGuid;
		[DataMember]
		protected string meterID;
		[DataMember]
		protected Guid meterGuid;
		[DataMember]
		protected bool deleteFlag;
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
		protected DateTime? closeoutDate;

		[DataMember]
		protected string specialInstructionsNote;

		[DataMember]
		protected Guid specialInstructionsNoteIdentityGuid;

		[DataMember]
		protected PRODUCT_MAP_TYPE specialInstructionsNoteProductMapType;

		[DataMember]
		private bool? improperAdditization;
		[DataMember]
		private bool? brokenBlend;

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

		// vthompson 07-10-2008
		[DataMember]
		protected DateTimeOffset? date01;
		[DataMember]
		protected DateTimeOffset? date02;
		[DataMember]
		protected DateTimeOffset? date03;
		[DataMember]
		protected DateTimeOffset? date04;

		[DataMember]
		protected EngineeringUnit volumeUnits = EngineeringUnit.FmvMeter3;
		[DataMember]
		protected EngineeringUnit levelUnit = EngineeringUnit.FmlMeter;
		[DataMember]
		protected EngineeringUnit densityUnit = EngineeringUnit.FmdKgM3;
		[DataMember]
		protected EngineeringUnit temperatureUnit = EngineeringUnit.FmtDegC;
		[DataMember]
		protected EngineeringUnit massUnit = EngineeringUnit.FmmKg;
		[DataMember]
		protected EngineeringUnit flowUnit = EngineeringUnit.FmvfM3Sec;
		[DataMember]
		protected EngineeringUnit pressureUnit = EngineeringUnit.FmpPa;
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
		[DataMember]
		protected bool isEthanol;
		[DataMember]
		protected VcfModuleSettings _VcfModuleSettings;

		private const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		#endregion Attributes

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SubLineItemDO"/> class.
		/// </summary>
		public SubLineItemDO( )
		{
			this.quantity = new QuantityDO( );
			this.meterReading = new MeterReadingDO( );
			this.massPackageSize = new double?( );
			this.volumePackageSize = new double?( );
			this.VCF = 1;
			this.isEthanol = false;
			this._VcfModuleSettings = new VcfModuleSettings();
		}

		public SubLineItemDO(SubLineItemDO subLineItemDO)
		{
			if (subLineItemDO == null)
			{
					throw new ArgumentNullException(nameof(subLineItemDO));
			}

			this.TransactionSubLineItemGuid = subLineItemDO.TransactionSubLineItemGuid;
			this.ConjoinedTransactionSubLineItemGuid = subLineItemDO.ConjoinedTransactionSubLineItemGuid;
			this.SequenceId = subLineItemDO.SequenceId;
			this.productCode = subLineItemDO.productCode;
			this.product = subLineItemDO.product;
			this.productType = subLineItemDO.productType;
			this.productGuid = subLineItemDO.productGuid;
			this.quantity = new QuantityDO(subLineItemDO.quantity);
			this.vcf = subLineItemDO.vcf;
			this.pressure = subLineItemDO.pressure;
			this.temperature = subLineItemDO.temperature;
			this.density = subLineItemDO.density;
			this.customs = subLineItemDO.customs;
			this.status = subLineItemDO.status;
			this.armNumber = subLineItemDO.armNumber;
			this.lineNumber = subLineItemDO.lineNumber;
			this.batchNumber = subLineItemDO.batchNumber;
			this.lineFill = subLineItemDO.lineFill;
			this.bottomVolume = subLineItemDO.bottomVolume;
			this.netCapacity = subLineItemDO.netCapacity;
			this.tankStatus = subLineItemDO.tankStatus;
			this.meterReading = new MeterReadingDO(subLineItemDO.meterReading);
			this.freezePoint = subLineItemDO.freezePoint;
			this.differentialPressure = subLineItemDO.differentialPressure;
			this.dosageRate = subLineItemDO.dosageRate;
			this.presetAmount = subLineItemDO.presetAmount;
			this.storageLocationID = subLineItemDO.storageLocationID;
			this.storageLocationTankGuid = subLineItemDO.storageLocationTankGuid;
			this.meterID = subLineItemDO.meterID;
			this.meterGuid = subLineItemDO.meterGuid;
			this.deleteFlag = subLineItemDO.deleteFlag;
			this.coaID = subLineItemDO.coaID;
			this.quality = subLineItemDO.quality;
			this.tax1 = subLineItemDO.tax1;
			this.tax2 = subLineItemDO.tax2;
			this.tax3 = subLineItemDO.tax3;
			this.tax4 = subLineItemDO.tax4;
			this.tax5 = subLineItemDO.tax5;
			this.closeoutDate = subLineItemDO.closeoutDate;
			this.specialInstructionsNote = subLineItemDO.specialInstructionsNote;
			this.specialInstructionsNoteIdentityGuid = subLineItemDO.specialInstructionsNoteIdentityGuid;
			this.specialInstructionsNoteProductMapType = subLineItemDO.specialInstructionsNoteProductMapType;
			this.improperAdditization = subLineItemDO.improperAdditization;
			this.brokenBlend = subLineItemDO.brokenBlend;
			this.flag01 = subLineItemDO.flag01;
			this.flag02 = subLineItemDO.flag02;
			this.flag03 = subLineItemDO.flag03;
			this.flag04 = subLineItemDO.flag04;
			this.flag05 = subLineItemDO.flag05;
			this.flag06 = subLineItemDO.flag06;
			this.number01 = subLineItemDO.number01;
			this.number02 = subLineItemDO.number02;
			this.number03 = subLineItemDO.number03;
			this.number04 = subLineItemDO.number04;
			this.number05 = subLineItemDO.number05;
			this.number06 = subLineItemDO.number06;
			this.date01 = subLineItemDO.date01;
			this.date02 = subLineItemDO.date02;
			this.date03 = subLineItemDO.date03;
			this.date04 = subLineItemDO.date04;
			this.volumeUnits = subLineItemDO.volumeUnits;
			this.levelUnit = subLineItemDO.levelUnit;
			this.densityUnit = subLineItemDO.densityUnit;
			this.temperatureUnit = subLineItemDO.temperatureUnit;
			this.massUnit = subLineItemDO.massUnit;
			this.flowUnit = subLineItemDO.flowUnit;
			this.pressureUnit = subLineItemDO.pressureUnit;
			this.volumeDecimalPlaces = subLineItemDO.volumeDecimalPlaces;
			this.levelDecimalPlaces = subLineItemDO.levelDecimalPlaces;
			this.densityDecimalPlaces = subLineItemDO.densityDecimalPlaces;
			this.temperatureDecimalPlaces = subLineItemDO.temperatureDecimalPlaces;
			this.massDecimalPlaces = subLineItemDO.massDecimalPlaces;
			this.flowDecimalPlaces = subLineItemDO.flowDecimalPlaces;
			this.pressureDecimalPlaces = subLineItemDO.pressureDecimalPlaces;
			this.massPackageSize = subLineItemDO.massPackageSize;
			this.volumePackageSize = subLineItemDO.volumePackageSize;
			this.cleanLineProduct = subLineItemDO.cleanLineProduct;
			this.cleanLineDeductProduct = subLineItemDO.cleanLineDeductProduct;
			this.cleanLineDeductQuantity = subLineItemDO.cleanLineDeductQuantity;
			this.cleanLinePackQuantity = subLineItemDO.cleanLinePackQuantity;
			this.isEthanol = subLineItemDO.isEthanol;
			this.VcfModuleSettings = subLineItemDO.VcfModuleSettings;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the product code.
		/// </summary>
		public string ProductCode
		{
			get { return this.productCode; }
			set { this.productCode = value; }
		}

		public string Product
		{
			get { return product; }
			set { product = value; }
		}

		public string ProductType
		{
			get { return productType; }
			set { productType = value; }
		}

		[XmlIgnoreAttribute]
		public Guid ProductGuid
		{
			get { return productGuid; }
			set { productGuid = value; }
		}

		public QuantityDO Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}

		/// <summary>
		/// Gets or sets the VCF.
		/// </summary>
		public double? VCF
		{
			get { return this.vcf; }
			set { this.vcf = value; }
		}

		/// <summary>
		/// Gets or sets the pressure.
		/// </summary>
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



		/// <summary>
		/// Gets or sets the temperature.
		/// </summary>
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

		public double? Density
		{
			get { return density; }
			set { density = value; }
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

		public string Customs
		{
			get { return customs; }
			set { customs = value; }
		}

		public TransactionStatus Status
		{
			get { return status; }
			set { status = value; }
		}

		public int? ArmNumber
		{
			get { return armNumber; }
			set { armNumber = value; }
		}

		/// <summary>
		/// This method causes the Arm Number property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeArmNumber( )
		{
			return this.armNumber.HasValue;
		}

		public int? LineNumber
		{
			get { return lineNumber; }
			set { lineNumber = value; }
		}

		/// <summary>
		/// This method causes the Line Number property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeLineNumber( )
		{
			return this.lineNumber.HasValue;
		}

		public string BatchNumber
		{
			get { return batchNumber; }
			set { batchNumber = value; }
		}

		public double? LineFill
		{
			get { return lineFill; }
			set { lineFill = value; }
		}

		/// <summary>
		/// This method causes the Line Fill property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeLineFill( )
		{
			return this.lineFill.HasValue;
		}

		public double? BottomVolume
		{
			get { return bottomVolume; }
			set { bottomVolume = value; }
		}

		/// <summary>
		/// This method causes the Bottom Volume property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeBottomVolume( )
		{
			return this.bottomVolume.HasValue;
		}

		public double? NetCapacity
		{
			get { return netCapacity; }
			set { netCapacity = value; }
		}

		/// <summary>
		/// This method causes the Net Capacity property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNetCapacity( )
		{
			return this.netCapacity.HasValue;
		}

		public string TankStatus
		{
			get { return tankStatus; }
			set { tankStatus = value; }
		}

		public MeterReadingDO MeterReading
		{
			get { return meterReading; }
			set { meterReading = value; }
		}

		public double? FreezePoint
		{
			get { return freezePoint; }
			set { freezePoint = value; }
		}

		/// <summary>
		/// This method causes the Freeze Point property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeFreezePoint( )
		{
			return this.freezePoint.HasValue;
		}

		public double? DifferentialPressure
		{
			get { return differentialPressure; }
			set { differentialPressure = value; }
		}

		/// <summary>
		/// This method causes the Differential Pressure property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeDifferentialPressure( )
		{
			return this.differentialPressure.HasValue;
		}

		public double? DosageRate
		{
			get { return dosageRate; }
			set { dosageRate = value; }
		}

		/// <summary>
		/// This method causes the Dosage Rate property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeDosageRate( )
		{
			return this.dosageRate.HasValue;
		}

		public double? PresetAmount
		{
			get { return presetAmount; }
			set { presetAmount = value; }
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

		public string StorageLocationID
		{
			get { return storageLocationID; }
			set { storageLocationID = value; }
		}

		[XmlIgnoreAttribute]
		public Guid StorageLocationTankGuid
		{
			get { return storageLocationTankGuid; }
			set { storageLocationTankGuid = value; }
		}

		public string MeterID
		{
			get { return meterID; }
			set { meterID = value; }
		}

		[XmlIgnore]
		public Guid MeterGuid
		{
			get { return meterGuid; }
			set { meterGuid = value; }
		}

		public string COAID
		{
			get { return this.coaID; }
			set { this.coaID = value; }
		}

		public TransactionQuality Quality
		{
			get { return this.quality; }
			set { this.quality = value; }
		}

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

		public bool DeleteFlag
		{
			get { return deleteFlag; }
			set { deleteFlag = value; }
		}

		[XmlIgnoreAttribute]
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

		[XmlIgnoreAttribute]
		public string SpecialInstructionsNote
		{
			get { return this.specialInstructionsNote; }
			set { this.specialInstructionsNote = value; }
		}

		[XmlIgnoreAttribute]
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

		[XmlIgnoreAttribute]
		public PRODUCT_MAP_TYPE SpecialInstructionsNoteProductMapType
		{
			get { return this.specialInstructionsNoteProductMapType; }
			set { this.specialInstructionsNoteProductMapType = value; }
		}

		[XmlIgnore]
		public DateTime? CloseoutDate
		{
			get { return this.closeoutDate; }
			set { this.closeoutDate = value; }
		}

		[XmlElementAttribute(Form = XmlSchemaForm.Unqualified)]
		public string CloseoutDateString
		{
			get
			{
				return this.closeoutDate == null ? string.Empty : ((DateTime) this.closeoutDate).ToString(TimeFormat);
			}

			set
			{
				this.closeoutDate = string.IsNullOrEmpty(value) ? (DateTime?) null : DateTime.ParseExact(value, TimeFormat, null).Date;
			}
		}

		/// <summary>
		/// Gets or sets the improper additization.
		/// </summary>
		public bool? ImproperAdditization
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

		/// <summary>
		/// Gets or sets the broken blend.
		/// </summary>
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
			return this.BrokenBlend.HasValue;
		}

		public bool Flag01
		{
			get { return this.flag01; }
			set { this.flag01 = value; }
		}

		public bool Flag02
		{
			get { return this.flag02; }
			set { this.flag02 = value; }
		}

		public bool Flag03
		{
			get { return this.flag03; }
			set { this.flag03 = value; }
		}

		public bool Flag04
		{
			get { return this.flag04; }
			set { this.flag04 = value; }
		}

		public bool Flag05
		{
			get { return this.flag05; }
			set { this.flag05 = value; }
		}

		public bool Flag06
		{
			get { return this.flag06; }
			set { this.flag06 = value; }
		}

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

		[XmlIgnore]
		public DateTimeOffset? Date01
		{
			get { return date01; }
			set { date01 = value; }
		}

		[XmlElementAttribute(Form = XmlSchemaForm.Unqualified)]
		public string Date01String
		{
			get
			{
				return this.date01 == null ? string.Empty : ((DateTimeOffset) this.date01).ToString(TimeFormat);
			}

			set
			{
				this.date01 = string.IsNullOrEmpty(value) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[XmlIgnore]
		public DateTimeOffset? Date02
		{
			get { return date02; }
			set { date02 = value; }
		}

		[XmlElementAttribute(Form = XmlSchemaForm.Unqualified)]
		public string Date02String
		{
			get
			{
				return this.date02 == null ? string.Empty : ((DateTimeOffset) this.date02).ToString(TimeFormat);
			}

			set
			{
				this.date02 = string.IsNullOrEmpty(value) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[XmlIgnore]
		public DateTimeOffset? Date03
		{
			get { return date03; }
			set { date03 = value; }
		}

		[XmlElementAttribute(Form = XmlSchemaForm.Unqualified)]
		public string Date03String
		{
			get
			{
				return this.date03 == null ? string.Empty : ((DateTimeOffset) this.date03).ToString(TimeFormat);
			}

			set
			{
				this.date03 = string.IsNullOrEmpty(value) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[XmlIgnore]
		public DateTimeOffset? Date04
		{
			get { return date04; }
			set { date04 = value; }
		}

		[XmlElementAttribute(Form = XmlSchemaForm.Unqualified)]
		public string Date04String
		{
			get
			{
				return this.date04 == null ? string.Empty : ((DateTimeOffset) this.date04).ToString(TimeFormat);
			}

			set
			{
				this.date04 = string.IsNullOrEmpty(value) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		public EngineeringUnit VolumeUnits
		{
			get { return volumeUnits; }
			set { volumeUnits = value; }
		}

		public EngineeringUnit LevelUnits
		{
			get { return levelUnit; }
			set { levelUnit = value; }
		}

		public EngineeringUnit TemperatureUnits
		{
			get { return temperatureUnit; }
			set { temperatureUnit = value; }
		}

		public EngineeringUnit DensityUnits
		{
			get { return densityUnit; }
			set { densityUnit = value; }
		}

		public EngineeringUnit MassUnits
		{
			get { return massUnit; }
			set { massUnit = value; }
		}

		public EngineeringUnit FlowUnits
		{
			get { return flowUnit; }
			set { flowUnit = value; }
		}

		public EngineeringUnit PressureUnits
		{
			get { return pressureUnit; }
			set { pressureUnit = value; }
		}

		[XmlIgnoreAttribute]
		public byte VolumeDecimalPlaces
		{
			get { return volumeDecimalPlaces; }
			set { volumeDecimalPlaces = value; }
		}

		[XmlIgnoreAttribute]
		public byte LevelDecimalPlaces
		{
			get { return levelDecimalPlaces; }
			set { levelDecimalPlaces = value; }
		}

		[XmlIgnoreAttribute]
		public byte TemperatureDecimalPlaces
		{
			get { return temperatureDecimalPlaces; }
			set { temperatureDecimalPlaces = value; }
		}

		[XmlIgnoreAttribute]
		public byte DensityDecimalPlaces
		{
			get { return densityDecimalPlaces; }
			set { densityDecimalPlaces = value; }
		}

		[XmlIgnoreAttribute]
		public byte MassDecimalPlaces
		{
			get { return massDecimalPlaces; }
			set { massDecimalPlaces = value; }
		}

		[XmlIgnoreAttribute]
		public byte FlowDecimalPlaces
		{
			get { return flowDecimalPlaces; }
			set { flowDecimalPlaces = value; }
		}

		[XmlIgnoreAttribute]
		public byte PressureDecimalPlaces
		{
			get { return pressureDecimalPlaces; }
			set { pressureDecimalPlaces = value; }
		}

		public double? MassPackageSize
		{
			get { return massPackageSize; }
			set { massPackageSize = value; }
		}

		/// <summary>
		/// This method causes the Mass Package Size property to not be serialized if it
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

		public double? VolumePackageSize
		{
			get { return volumePackageSize; }
			set { volumePackageSize = value; }
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

		public bool IsEthanol
		{
			get { return this.isEthanol; }
			set { this.isEthanol = value; }
		}

		public VcfModuleSettings VcfModuleSettings
		{
			get { return this._VcfModuleSettings; }
			set { this._VcfModuleSettings = value; }
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

		#region DataObject overrides
		public override string getSelectCommand()
		{
			return null;
		}
		public override string getDeleteCommand()
		{
			return null;
		}
		public override string getInsertCommand()
		{
			return null;
		}
		public override string getUpdateCommand()
		{
			return null;
		}


		#endregion DataObject overrides
	}
}
