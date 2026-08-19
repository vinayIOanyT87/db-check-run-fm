// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentTypeClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EQUIPMENT_TYPE type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Runtime.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    #region Public equipment type enumerations.
	/// <summary>
	/// The equipment types.
	/// </summary>
	public enum EQUIPMENT_TYPE
	{
		TRAILER_TYPE = 0,
		TRACTOR_TYPE = 1,
		AIRCRAFT_TYPE = 2,
		RAILCAR_TYPE = 3,
		BARGE_TYPE = 4,
		COMPARTMENT_TYPE = 5,
		SHIP_TYPE = 6,
		PIPELINE_TYPE = 7,
		HYDRANT_CART_TYPE = 8,
		TANKER_TYPE = 9,
		STATIONARY_CART_TYPE = 10,
		OTHER_TYPE = 11,
		SYSTEM_TYPE = 12,
		TANK_TYPE = 13,
		FILLSTAND_TYPE = 14,
		CONTAINER = 15,
		VEHICLE = 16,
		INFRASTRUCTURE = 17,
		MAX_EQUIPMENT_TYPE = 18
	};

	public static class EquipmentTypeExtensions
	{
		public static bool IsMultiCompartmentCapable(this EQUIPMENT_TYPE type)
		{
			return type == EQUIPMENT_TYPE.TRAILER_TYPE || type == EQUIPMENT_TYPE.BARGE_TYPE || type == EQUIPMENT_TYPE.SHIP_TYPE
			       || type == EQUIPMENT_TYPE.TANKER_TYPE || type == EQUIPMENT_TYPE.RAILCAR_TYPE
			       || type == EQUIPMENT_TYPE.AIRCRAFT_TYPE;
		}
	}

	/// <summary>
	/// The equipment type location.
	/// </summary>
	public enum EQUIPMENT_TYPE_LOCATION
	{
		Center = 0,
		Left = 1,
		Right = 2,
		MAX_EQUIPMENT_TYPE_LOCATION = 3
	};

	/// <summary>
	/// The tolerance type.
	/// </summary>
	public enum TOLERANCE_TYPE
	{
		Mass = 0,
		Volume = 1,
		Percentage = 2,
		MAX_TOLERANCE_TYPE = 3
	};
	#endregion

	#region Equipment Type Collection Class
	/// <summary>
	/// The equipment type collection class.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(EquipmentTypeClass))]
	public class EquipmentTypeCollectionClass : List<EquipmentTypeClass> { }
	#endregion

	/// <summary>
	/// The equipment type class.
	/// </summary>
	[EntityImportExportWorksheetAttribute("EQUIPMENT TYPE CLASSES")]
	[DataContract]
	[Serializable]
	public class EquipmentTypeClass : BaseDataObject, IComparable
	{
		#region Protected data members
		[DataMember]
		protected AirplaneTankCollectionClass _TankCollection = new AirplaneTankCollectionClass();

		[DataMember]
		protected string _CustomerDesignator;

		[DataMember]
		protected double _ServiceTime;

		[DataMember]
		protected Guid _Product;

		[DataMember]
		protected EngineeringUnit _MassUnits = EngineeringUnit.FmmLb;

		[DataMember]
		protected int _MassDecimalPlaces = NumberFormatInfo.CurrentInfo.CurrencyDecimalDigits;

		[DataMember]
		protected TOLERANCE_TYPE _WingToWingToleranceType;

		[DataMember]
		protected SIDouble _WingToWingToleranceValue;

		[DataMember]
		protected TOLERANCE_TYPE _TankToTankToleranceType;

		[DataMember]
		protected SIDouble _TankToTankToleranceValue;

		[DataMember]
		protected TOLERANCE_TYPE _FuelServiceToleranceType;

		[DataMember]
		protected SIDouble _FuelServiceToleranceValue;

		[DataMember]
		protected TOLERANCE_TYPE _FuelServiceToleranceMaxType;

		[DataMember]
		protected SIDouble _FuelServiceToleranceMaxValue;

		[DataMember]
		protected string _Description;

		[DataMember]
		protected string _Make;

		[DataMember]
		protected string _Model;

		[DataMember]
		protected string _Isspt;

		[DataMember]
		protected int _Year;

		[DataMember]
		protected SIDouble _Capacity;

		[DataMember]
		protected SIDouble _SafeFill;

		[DataMember]
		protected EQUIPMENT_TYPE _Attribute;

		[DataMember]
		protected bool _MultiCompartment;

		[DataMember]
		protected EngineeringUnit _VolumeUnits = EngineeringUnit.FmvUsGal;

		[DataMember]
		protected int _VolumeDecimalPlaces = NumberFormatInfo.CurrentInfo.CurrencyDecimalDigits;

		[DataMember]
		protected bool _AllowFuelingByWeight = true;

		[DataMember]
		protected COMPANY_ROLE _CompanyRoleAssignmentConstraint = COMPANY_ROLE.MAX_COMPANY_ROLE;

		/// <summary>
		/// The product ID.
		/// </summary>
		[DataMember] private string productId;
		#endregion

		#region Public data members
		[EntityImportExportWorksheetAttribute("TYPECLASSQUALIFICATIONS")]
		[EntityImportExportAttribute("QUALIFICATIONID*", 125, "ID", 1)]
		[EntityImportExportAttribute("TYPE", 110, "Type", 2)]
		[DataMember]
		public QualificationMapCollectionClass ReqQualificationsCollection;

		[EntityImportExportWorksheetAttribute("TYPECLASSTRAINING")]
		[EntityImportExportAttribute("TRAININGID*", 125, "ID", 1)]
		[EntityImportExportAttribute("TYPE", 110, "Type", 2)]
		[DataMember]
		public QualificationMapCollectionClass ReqTrainingCollection;
		#endregion

		#region Properties
		[EntityImportExportAttribute("SITE*", 105, "SITEGUID")]
		new public Guid SiteGuid { get { return this._SiteGuid; } set {
		    this._SiteGuid = value; } }

		[EntityImportExportAttribute("TYPECLASSID*", 100, "ID")]
		public override string ID { get { return this._ID; } set {
		    this.SetString("ID", 50, value, ref this._ID); } }

		public string Alias { get { return this._ID; } set {
		    this.SetString("ID", 50, value, ref this._ID); } }

		[EntityImportExportAttribute("DESCRIPTION", 100)]
		public string Description { get { return this._Description; } set {
		    this.SetString("Description", 50, value, ref this._Description); } }

		[EntityImportExportAttribute("ISSPT", 50)]
		public string Isspt { get { return this._Isspt; } set {
		    this.SetString("Isspt", 20, value, ref this._Isspt); } }

		[EntityImportExportAttribute("MAKE", 120)]
		public string Make { get { return this._Make; } set {
		    this.SetString("Make", 20, value, ref this._Make); } }

		[EntityImportExportAttribute("MODEL", 120)]
		public string Model { get { return this._Model; } set {
		    this.SetString("Model", 50, value, ref this._Model); } }

		[EntityImportExportAttribute("YEAR", 50)]
		public int Year { get { return this._Year; } set {
		    this._Year = value; } }

		[EntityImportExportAttribute("CAPACITY", 80)]
		public string Capacity
		{
			get
			{
				return this._Capacity.ToString();
			}
			set
			{
			    this.SetSIDouble("Capacity", value, ref this._Capacity);
			}
		}

		[EntityImportExportAttribute("SAFEFILL", 80)]
		public string SafeFill { get { return this._SafeFill.ToString(); } set {
		    this.SetSIDouble("Safe Fill", value, ref this._SafeFill); } }

		[EntityImportExportAttribute("CUSTOMERDESIGNATOR", 120)]
		public string CustomerDesignator { get { return this._CustomerDesignator; } set {
		    this._CustomerDesignator = value; } }

		[EntityImportExportAttribute("SERVICETIME", 80)]
		public string ServiceTime
		{
			get
			{
				return this._ServiceTime.ToString();
			} 

			set
			{
				if (!double.TryParse(value, out this._ServiceTime))
				{
					throw new FormatException("Service Time must be provided and must be a numeric value.");
				}
			}
		}

		public Guid Product { get { return this._Product; } set {
		    this._Product = value; } }

		[EntityImportExportAttribute("PRODUCTID", 30)]
		public string ProductId
		{
			get { return this.productId; }
			set { this.productId = value; }
		}

		[EntityImportExportAttribute("MASSUNITS", 50)]
		public EngineeringUnit MassUnits { get { return this._MassUnits; } set {
		    this._MassUnits = value;
		    this.UpdateTankCapacityParameters(); } }

		[EntityImportExportAttribute("MASSDECIMALPLACES", 50)]
		public string MassDecimalPlaces { 
			get
			{
				return this._MassDecimalPlaces.ToString();
			} 

			set
			{		
				if (!int.TryParse(value, out this._MassDecimalPlaces))
				{
					throw new FormatException("Mass Decimal Places must be provided and must be a numeric value.");
				}

				// The value must be between 0 and 15 or we get an error when we try to round.
				if (this._MassDecimalPlaces < 0 || this._MassDecimalPlaces > 15)
				{
					throw new FormatException("Mass Decimal Places must be between 0 and 15.");
				}

			    this.UpdateTankCapacityParameters();
			} 
		}

		[EntityImportExportAttribute("VOLUMEUNITS", 50)]
		public EngineeringUnit VolumeUnits { get { return this._VolumeUnits; } set {
		    this._VolumeUnits = value;
		    this.UpdateTankCapacityParameters(); } }

		[EntityImportExportAttribute("VOLUMEDECIMALPLACES", 50)]
		public string VolumeDecimalPlaces
		{
			get
			{
				return this._VolumeDecimalPlaces.ToString();
			} 

			set
			{
				if (!int.TryParse(value, out this._VolumeDecimalPlaces))
				{
					throw new FormatException("Volume Decimal Places must be provided and must be a numeric value.");
				}

				// The value must be between 0 and 15 or we get an error when we try to round.
				if (this._VolumeDecimalPlaces < 0 || this._VolumeDecimalPlaces > 15)
				{
					throw new FormatException("Volume Decimal Places must be between 0 and 15.");
				}

			    this.UpdateTankCapacityParameters();
			}
		}

		[EntityImportExportAttribute("ALLOWFUELINGBYWEIGHT", 30)]
		public bool AllowFuelingByWeight { get { return this._AllowFuelingByWeight; } set {
		    this._AllowFuelingByWeight = value; } }

		[EntityImportExportAttribute("COMPANYROLECONSTRAINT", 30)]
		public COMPANY_ROLE CompanyRoleAssignmentConstraint { get { return this._CompanyRoleAssignmentConstraint; } set {
		    this._CompanyRoleAssignmentConstraint = value; } }


		[EntityImportExportAttribute("WINGTOWINGTOLERANCETYPE", 50)]
		public TOLERANCE_TYPE WingToWingToleranceType
		{
			get
			{
				return this._WingToWingToleranceType;
			}
			set {
			    this._WingToWingToleranceType = value; }
		}

		[EntityImportExportAttribute("WINGTOWINGTOLERANCEVALUE", 80)]
		public string WingToWingToleranceValue
		{
			get
			{
				switch (this._WingToWingToleranceType)
				{
					case TOLERANCE_TYPE.Volume:
				        this._WingToWingToleranceValue.Units = this._VolumeUnits;
				        this._WingToWingToleranceValue.numberDecimalDigits = this._VolumeDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Mass:
				        this._WingToWingToleranceValue.Units = this._MassUnits;
				        this._WingToWingToleranceValue.numberDecimalDigits = this._MassDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Percentage:
				        this._WingToWingToleranceValue.Units = EngineeringUnit.FmSiteUnits;
				        this._WingToWingToleranceValue.numberDecimalDigits = 2;
						break;
				}
				return this._WingToWingToleranceValue.ToString();
			}

			set
			{
				double wingToWingToleranceValue;
				if (!double.TryParse(value, out wingToWingToleranceValue))
				{
					throw new FormatException("Wing to Wing Tolerance Value must be provided and must be a numeric value.");
				}

			    this.SetSIDouble("WingToWingToleranceValue", value, ref this._WingToWingToleranceValue);
			}
		}

		[EntityImportExportAttribute("TANKTOTANKTOLERANCETYPE", 50)]
		public TOLERANCE_TYPE TankToTankToleranceType { get { return this._TankToTankToleranceType; } set {
		    this._TankToTankToleranceType = value; } }

		[EntityImportExportAttribute("TANKTOTANKTOLERANCEVALUE", 80)]
		public string TankToTankToleranceValue
		{
			get
			{
				switch (this._TankToTankToleranceType)
				{
					case TOLERANCE_TYPE.Volume:
				        this._TankToTankToleranceValue.Units = this._VolumeUnits;
				        this._TankToTankToleranceValue.numberDecimalDigits = this._VolumeDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Mass:
				        this._TankToTankToleranceValue.Units = this._MassUnits;
				        this._TankToTankToleranceValue.numberDecimalDigits = this._MassDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Percentage:
				        this._TankToTankToleranceValue.Units = EngineeringUnit.FmSiteUnits;
				        this._TankToTankToleranceValue.numberDecimalDigits = 2;
						break;
				} return this._TankToTankToleranceValue.ToString();
			}

			set
			{
				double tankToTankToleranceValue;
				if (!double.TryParse(value, out tankToTankToleranceValue))
				{
					throw new FormatException("Tank to Tank Tolerance Value must be provided and must be a numeric value.");	
				}

			    this.SetSIDouble("TankToTankToleranceValue", value, ref this._TankToTankToleranceValue);
			}
		}

		[EntityImportExportAttribute("FUELSERVICETOLERANCETYPE", 50)]
		public TOLERANCE_TYPE FuelServiceToleranceType { get { return this._FuelServiceToleranceType; } set {
		    this._FuelServiceToleranceType = value; } }

		[EntityImportExportAttribute("FUELSERVICETOLERANCEVALUE", 80)]
		public string FuelServiceToleranceValue
		{
			get
			{
				switch (this._FuelServiceToleranceType)
				{
					case TOLERANCE_TYPE.Volume:
				        this._FuelServiceToleranceValue.Units = this._VolumeUnits;
				        this._FuelServiceToleranceValue.numberDecimalDigits = this._VolumeDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Mass:
				        this._FuelServiceToleranceValue.Units = this._MassUnits;
				        this._FuelServiceToleranceValue.numberDecimalDigits = this._MassDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Percentage:
				        this._FuelServiceToleranceValue.Units = EngineeringUnit.FmSiteUnits;
				        this._FuelServiceToleranceValue.numberDecimalDigits = 2;
						break;
				}
				return this._FuelServiceToleranceValue.ToString();
			}

			set
			{
				double fuelServiceToleranceValue;
				if (!double.TryParse(value, out fuelServiceToleranceValue))
				{
					throw new FormatException("Fuel Service Tolerance Value must be provided and must be a numeric value.");
				}

			    this.SetSIDouble("FuelServiceToleranceValue", value, ref this._FuelServiceToleranceValue);
			}
		}

		[EntityImportExportAttribute("FUELSERVICETOLERANCEMAXTYPE", 50)]
		public TOLERANCE_TYPE FuelServiceToleranceMaxType { get { return this._FuelServiceToleranceMaxType; } set {
		    this._FuelServiceToleranceMaxType = value;
		    this.UpdateTankCapacityParameters(); } }

		[EntityImportExportAttribute("FUELSERVICETOLERANCEMAXVALUE", 80)]
		public string FuelServiceToleranceMaxValue
		{
			get
			{
				switch (this._FuelServiceToleranceMaxType)
				{
					case TOLERANCE_TYPE.Volume:
				        this._FuelServiceToleranceMaxValue.Units = this._VolumeUnits;
				        this._FuelServiceToleranceMaxValue.numberDecimalDigits = this._VolumeDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Mass:
				        this._FuelServiceToleranceMaxValue.Units = this._MassUnits;
				        this._FuelServiceToleranceMaxValue.numberDecimalDigits = this._MassDecimalPlaces;
						break;
				}
				return this._FuelServiceToleranceMaxValue.ToString();
			}

			set
			{
				double fuelServiceToleranceValue;
				if (!double.TryParse(value, out fuelServiceToleranceValue))
				{
					throw new FormatException("Fuel Service Tolerance Max Value must be provided and must be a numeric value.");
				}

			    this.SetSIDouble("FuelServiceToleranceMaxValue", value, ref this._FuelServiceToleranceMaxValue);
			}
		}

		public SIDouble SICapacity { get { return this._Capacity; } set {
		    this._Capacity = value; } }

		public SIDouble SISafeFill { get { return this._SafeFill; } set {
		    this._SafeFill = value; } }


        [EntityImportExportWorksheetAttribute("TYPECLASSAIRPLANETANKS")]
        [EntityImportExportAttribute("ID*", 125, "ID", 1)]
        [EntityImportExportAttribute("CUSTOMERTANKID", 125, "CUSTOMERTANKID", 2)]
        [EntityImportExportAttribute("DESCRIPTION", 125, "DESCRIPTION", 3)]
        [EntityImportExportAttribute("CAPACITY", 80, "CAPACITY", 4)]
        [EntityImportExportAttribute("POSITION", 80, "POSITION", 5)]
        [EntityImportExportAttribute("LOCATION", 80, "LOCATION", 6)]
        [EntityImportExportAttribute("GUIORDER", 80, "GUIORDER", 7)]
        [DataMember]
		public AirplaneTankCollectionClass TankCollection { get {
		    this.UpdateTankCapacityParameters(); return this._TankCollection; } set {
		        this._TankCollection = value;
		        this.UpdateTankCapacityParameters(); } }

		[EntityImportExportAttribute("Attribute", 120)]
		public EQUIPMENT_TYPE Attribute
		{
			get { return this._Attribute; }
			set {
			    this._Attribute = value; }
		}

		[EntityImportExportAttribute("MULTICOMPARTMENT", 120, "IsMultiCompartment")]
		public bool IsMultiCompartment { get { return this._MultiCompartment; } set {
		    this._MultiCompartment = value; } }

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.EQUIPMENT_TYPE; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public void InsertSQL(SqlCommand cmd)
		{

			cmd.CommandText = "INSERT INTO dbo.tblEquipmentTypes " +
					"(SiteGuid," +
					"EqTypeName," +
					"EqTypeDescription," +
					"Capacity," +
					"SafeFill," +
					"Make," +
					"Model," +
					"Isspt," +
					"Year," +
					"CustomerDesignator," +
					"ServiceTime," +
					"ProductGuid," +
					"VolumeUnits," +
					"VolumeDecimalPlaces," +
					"MassUnits," +
					"MassDecimalPlaces," +
					"WingToWingToleranceType," +
					"WingToWingToleranceValue," +
					"TankToTankToleranceType," +
					"TankToTankToleranceValue," +
					"FuelServiceToleranceType," +
					"FuelServiceToleranceValue," +
					"FuelServiceToleranceMaxType," +
					"FuelServiceToleranceMaxValue," +
					"AllowFuelingByWeight," +
					"LookupEquipmentTypeIndex," +
					"MultiCompartment," +
					"LookupCompanyRoleIndex," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy," +
					"EquipmentTypeGuid" +
					") VALUES (" +
					"@SiteGuid," +
					"@ID," +
					"@Description," +
					"@Capacity," +
					"@SafeFill," +
					"@Make," +
					"@Model," +
					"@Isspt," +
					"@Year," +
					"@CustomerDesignator," +
					"@ServiceTime," +
					"@ProductGuid," +
					"@VolumeUnits," +
					"@VolumeDecimalPlaces," +
					"@MassUnits," +
					"@MassDecimalPlaces," +
					"@WingToWingToleranceType," +
					"@WingToWingToleranceValue," +
					"@TankToTankToleranceType," +
					"@TankToTankToleranceValue," +
					"@FuelServiceToleranceType," +
					"@FuelServiceToleranceValue," +
					"@FuelServiceToleranceMaxType," +
					"@FuelServiceToleranceMaxValue," +
					"@AllowFuelingByWeight," +
					"@LookupEquipmentTypeIndex, " +
					"@MultiCompartment, " +
					"@LookupCompanyRoleIndex," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy," +
					"@EquipmentTypeGuid)";


			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Capacity", SqlDbType.Float);
			cmd.Parameters.Add("@SafeFill", SqlDbType.Float);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Make", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@Model", SqlDbType.NVarChar, 32);
			cmd.Parameters.Add("@Isspt", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@Year", SqlDbType.SmallInt);
			cmd.Parameters.Add("@CustomerDesignator", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@ServiceTime", SqlDbType.Float);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@VolumeUnits", SqlDbType.Int);
			cmd.Parameters.Add("@VolumeDecimalPlaces", SqlDbType.SmallInt);
			cmd.Parameters.Add("@MassUnits", SqlDbType.Int);
			cmd.Parameters.Add("@MassDecimalPlaces", SqlDbType.Int);
			cmd.Parameters.Add("@WingToWingToleranceType", SqlDbType.SmallInt);
			cmd.Parameters.Add("@WingToWingToleranceValue", SqlDbType.Float);
			cmd.Parameters.Add("@TankToTankToleranceType", SqlDbType.SmallInt);
			cmd.Parameters.Add("@TankToTankToleranceValue", SqlDbType.Float);
			cmd.Parameters.Add("@FuelServiceToleranceType", SqlDbType.SmallInt);
			cmd.Parameters.Add("@FuelServiceToleranceValue", SqlDbType.Float);
			cmd.Parameters.Add("@FuelServiceToleranceMaxType", SqlDbType.SmallInt);
			cmd.Parameters.Add("@FuelServiceToleranceMaxValue", SqlDbType.Float);
			cmd.Parameters.Add("@AllowFuelingByWeight", SqlDbType.Bit);
			cmd.Parameters.Add("@LookupEquipmentTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@MultiCompartment", SqlDbType.Bit);
			cmd.Parameters.Add("@LookupCompanyRoleIndex", SqlDbType.Int);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@EquipmentTypeGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@Description"].Value = this._Description;
			cmd.Parameters["@Capacity"].Value = this._Capacity.SIValue;
			cmd.Parameters["@SafeFill"].Value = this._SafeFill.SIValue;
			cmd.Parameters["@Make"].Value = this._Make;
			cmd.Parameters["@Model"].Value = this._Model;
			cmd.Parameters["@Isspt"].Value = this._Isspt;

			if (this.Year == 0)
			{
				cmd.Parameters["@Year"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@Year"].Value = this.Year;
			}
			cmd.Parameters["@CustomerDesignator"].Value = this._CustomerDesignator;
			cmd.Parameters["@ServiceTime"].Value = this._ServiceTime;
			if (this._Product == Guid.Empty)
			{
				cmd.Parameters["@ProductGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ProductGuid"].Value = this._Product;
			}
			cmd.Parameters["@VolumeUnits"].Value = this._VolumeUnits;
			cmd.Parameters["@VolumeDecimalPlaces"].Value = this._VolumeDecimalPlaces;
			cmd.Parameters["@MassUnits"].Value = this._MassUnits;
			cmd.Parameters["@MassDecimalPlaces"].Value = this._MassDecimalPlaces;
			cmd.Parameters["@WingToWingToleranceType"].Value = this._WingToWingToleranceType;
			cmd.Parameters["@WingToWingToleranceValue"].Value = this._WingToWingToleranceValue.SIValue;
			cmd.Parameters["@TankToTankToleranceType"].Value = this._TankToTankToleranceType;
			cmd.Parameters["@TankToTankToleranceValue"].Value = this._TankToTankToleranceValue.SIValue;
			cmd.Parameters["@FuelServiceToleranceType"].Value = this._FuelServiceToleranceType;
			cmd.Parameters["@FuelServiceToleranceValue"].Value = this._FuelServiceToleranceValue.SIValue;
			cmd.Parameters["@FuelServiceToleranceMaxType"].Value = this._FuelServiceToleranceMaxType;
			cmd.Parameters["@FuelServiceToleranceMaxValue"].Value = this._FuelServiceToleranceMaxValue.SIValue;
			cmd.Parameters["@LookupEquipmentTypeIndex"].Value = (int)this.Attribute;

			if (this._AllowFuelingByWeight)
			{
				cmd.Parameters["@AllowFuelingByWeight"].Value = 1;
			}
			else
			{
				cmd.Parameters["@AllowFuelingByWeight"].Value = 0;
			}
			if (this._MultiCompartment)
			{
				cmd.Parameters["@MultiCompartment"].Value = 1;
			}
			else
			{
				cmd.Parameters["@MultiCompartment"].Value = 0;
			}

			cmd.Parameters["@LookupCompanyRoleIndex"].Value = (int)this._CompanyRoleAssignmentConstraint;
			cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@EquipmentTypeGuid"].Value = this._IdentityGuid;

		}

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText = "UPDATE dbo.tblEquipmentTypes " +
						"SET SiteGuid = @SiteGuid," +
						"EqTypeName = @ID," +
						"EqTypeDescription = @Description," +
						"Make = @Make," +
						"Model = @Model," +
						"Isspt = @Isspt," +
						"Year = @Year," +
						"CustomerDesignator = @CustomerDesignator," +
						"ServiceTime = @ServiceTime," +
						"ProductGuid = @ProductGuid," +
						"VolumeUnits = @VolumeUnits," +
						"VolumeDecimalPlaces = @VolumeDecimalPlaces," +
						"MassUnits = @MassUnits," +
						"MassDecimalPlaces = @MassDecimalPlaces," +
						"WingToWingToleranceType = @WingToWingToleranceType," +
						"WingToWingToleranceValue = @WingToWingToleranceValue," +
						"TankToTankToleranceType = @TankToTankToleranceType," +
						"TankToTankToleranceValue = @TankToTankToleranceValue," +
						"FuelServiceToleranceType = @FuelServiceToleranceType," +
						"FuelServiceToleranceValue = @FuelServiceToleranceValue," +
						"FuelServiceToleranceMaxType = @FuelServiceToleranceMaxType," +
						"FuelServiceToleranceMaxValue = @FuelServiceToleranceMaxValue," +
						"AllowFuelingByWeight = @AllowFuelingByWeight," +
						"Capacity = @Capacity," +
						"SafeFill = @SafeFill," +
						"LookupEquipmentTypeIndex = @LookupEquipmentTypeIndex," +
						"LookupCompanyRoleIndex = @LookupCompanyRoleIndex," +
						"MultiCompartment = @MultiCompartment," +
						"UpdatedDate = @UpdatedDate," +
						"UpdatedBy = @UpdatedBy " +
				  "WHERE EquipmentTypeGuid = @EquipmentTypeGuid";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Capacity", SqlDbType.Float);
			cmd.Parameters.Add("@SafeFill", SqlDbType.Float);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Make", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@Model", SqlDbType.NVarChar, 32);
			cmd.Parameters.Add("@Isspt", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@Year", SqlDbType.SmallInt);
			cmd.Parameters.Add("@CustomerDesignator", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@ServiceTime", SqlDbType.Float);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@VolumeUnits", SqlDbType.Int);
			cmd.Parameters.Add("@VolumeDecimalPlaces", SqlDbType.SmallInt);
			cmd.Parameters.Add("@MassUnits", SqlDbType.Int);
			cmd.Parameters.Add("@MassDecimalPlaces", SqlDbType.Int);
			cmd.Parameters.Add("@WingToWingToleranceType", SqlDbType.SmallInt);
			cmd.Parameters.Add("@WingToWingToleranceValue", SqlDbType.Float);
			cmd.Parameters.Add("@TankToTankToleranceType", SqlDbType.SmallInt);
			cmd.Parameters.Add("@TankToTankToleranceValue", SqlDbType.Float);
			cmd.Parameters.Add("@FuelServiceToleranceType", SqlDbType.SmallInt);
			cmd.Parameters.Add("@FuelServiceToleranceValue", SqlDbType.Float);
			cmd.Parameters.Add("@FuelServiceToleranceMaxType", SqlDbType.SmallInt);
			cmd.Parameters.Add("@FuelServiceToleranceMaxValue", SqlDbType.Float);
			cmd.Parameters.Add("@AllowFuelingByWeight", SqlDbType.Bit);
			cmd.Parameters.Add("@LookupEquipmentTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@LookupCompanyRoleIndex", SqlDbType.Int);
			cmd.Parameters.Add("@MultiCompartment", SqlDbType.Bit);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@EquipmentTypeGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@Description"].Value = this._Description;
			cmd.Parameters["@Capacity"].Value = this._Capacity.SIValue;
			cmd.Parameters["@SafeFill"].Value = this._SafeFill.SIValue;
			cmd.Parameters["@Make"].Value = this._Make;
			cmd.Parameters["@Model"].Value = this._Model;
			cmd.Parameters["@Isspt"].Value = this._Isspt;

			if (this.Year == 0)
			{
				cmd.Parameters["@Year"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@Year"].Value = this.Year;
			}
			cmd.Parameters["@CustomerDesignator"].Value = this._CustomerDesignator;
			cmd.Parameters["@ServiceTime"].Value = this._ServiceTime;
			if (this._Product == Guid.Empty)
			{
				cmd.Parameters["@ProductGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ProductGuid"].Value = this._Product;
			}
			cmd.Parameters["@VolumeUnits"].Value = this._VolumeUnits;
			cmd.Parameters["@VolumeDecimalPlaces"].Value = this._VolumeDecimalPlaces;
			cmd.Parameters["@MassUnits"].Value = this._MassUnits;
			cmd.Parameters["@MassDecimalPlaces"].Value = this._MassDecimalPlaces;
			cmd.Parameters["@WingToWingToleranceType"].Value = this._WingToWingToleranceType;
			cmd.Parameters["@WingToWingToleranceValue"].Value = this._WingToWingToleranceValue.SIValue;
			cmd.Parameters["@TankToTankToleranceType"].Value = this._TankToTankToleranceType;
			cmd.Parameters["@TankToTankToleranceValue"].Value = this._TankToTankToleranceValue.SIValue;
			cmd.Parameters["@FuelServiceToleranceType"].Value = this._FuelServiceToleranceType;
			cmd.Parameters["@FuelServiceToleranceValue"].Value = this._FuelServiceToleranceValue.SIValue;
			cmd.Parameters["@FuelServiceToleranceMaxType"].Value = this._FuelServiceToleranceMaxType;
			cmd.Parameters["@FuelServiceToleranceMaxValue"].Value = this._FuelServiceToleranceMaxValue.SIValue;
			cmd.Parameters["@LookupEquipmentTypeIndex"].Value = (int)this.Attribute;
			if (this._AllowFuelingByWeight)
			{
				cmd.Parameters["@AllowFuelingByWeight"].Value = 1;
			}
			else
			{
				cmd.Parameters["@AllowFuelingByWeight"].Value = 0;
			}
			if (this._MultiCompartment)
			{
				cmd.Parameters["@MultiCompartment"].Value = 1;
			}
			else
			{
				cmd.Parameters["@MultiCompartment"].Value = 0;
			}

			cmd.Parameters["@LookupCompanyRoleIndex"].Value = (int)this._CompanyRoleAssignmentConstraint;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@EquipmentTypeGuid"].Value = this.IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM dbo.tblEquipmentTypes WHERE EquipmentTypeGuid = @EquipmentTypeGuid";

			cmd.Parameters.Add("@EquipmentTypeGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@EquipmentTypeGuid"].Value = this.IdentityGuid;
		}

		class TankPositionCount
		{
			protected int position = 0;

			public int Position
			{
				get { return this.position; }
				set {
				    this.position = value; }
			}
			protected int count = 0;

			public int Count
			{
				get { return this.count; }
				set {
				    this.count = value; }
			}

			public TankPositionCount()
			{
			}

			public TankPositionCount(int aPosition)
			{
			    this.position = aPosition;
			    this.count = 1;
			}

			public void Increment()
			{
			    this.count++;
			}

			public bool IsPaired()
			{
				if (this.position != 0 && this.count != 2)
				{
					return false;
				}
				return true;
			}

			public bool IsPosition(int aPosition)
			{
				if (aPosition == this.position)
				{
					return true;
				}
				return false;
			}
		}

		class TankPositionList
		{
			protected List<TankPositionCount> tankPositionList = new List<TankPositionCount>();
			public TankPositionList()
			{
			}

			public void Add(int aPosition)
			{
				foreach (TankPositionCount posCnt in this.tankPositionList)
				{
					if (posCnt.IsPosition(aPosition))
					{
						posCnt.Increment();
						return;
					}
				}
				TankPositionCount tpc = new TankPositionCount(aPosition);
			    this.tankPositionList.Add(tpc);
			}

			public bool IsAllTanksPaired()
			{
				foreach (TankPositionCount posCnt in this.tankPositionList)
				{
					if (posCnt.IsPaired() == false)
					{
						return false;
					}
				}
				return true;
			}

		}

		public bool IsTanksPaired()
		{
			if (this._Attribute != EQUIPMENT_TYPE.AIRCRAFT_TYPE)
			{
				return true;
			}
			TankPositionList tpl = new TankPositionList();
			foreach (AirplaneTankClass tank in this._TankCollection)
			{
				tpl.Add(tank.Position);
			}
			return tpl.IsAllTanksPaired();
		}
		#endregion

		#region Comparable method
		/// <summary>
		/// The compare to.
		/// </summary>
		/// <param name="obj">
		/// The object.
		/// </param>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
		/// <exception cref="Exception">
		/// Invalid equipment type exception.
		/// </exception>
		int IComparable.CompareTo(object obj)
		{
			var equipmentType = obj as EquipmentTypeClass;

			if (equipmentType == null)
			{
				throw new Exception("Invalid EquipmentType");
			}

			return string.Compare(this.ID, equipmentType.ID, StringComparison.Ordinal);
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="EquipmentTypeClass"/> class. 
		/// This is the default constructor for the equipment type class.
		/// </summary>
		public EquipmentTypeClass() : this(null)
		{
			NumberFormatInfo currentInfo = NumberFormatInfo.CurrentInfo;
			this._Capacity = new SIDouble(EngineeringUnit.FmvUsGal, currentInfo, 0.0);
			this._SafeFill = new SIDouble(EngineeringUnit.FmvUsGal, currentInfo, 0.0);
			this._WingToWingToleranceValue = new SIDouble(EngineeringUnit.FmmLb, currentInfo, 0.0);
			this._TankToTankToleranceValue = new SIDouble(EngineeringUnit.FmmLb, currentInfo, 0.0);
			this._FuelServiceToleranceValue = new SIDouble(EngineeringUnit.FmmLb, currentInfo, 0.0);
			this._FuelServiceToleranceMaxValue = new SIDouble(EngineeringUnit.FmmLb, currentInfo, 0.0);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EquipmentTypeClass"/> class. 
		/// This constructor will initialize the equipment type class based on the site.
		/// </summary>
		/// <param name="Site">
		/// </param>
		public EquipmentTypeClass(SiteClass Site)
		{
            NumberFormatInfo currentInfo = NumberFormatInfo.CurrentInfo;
            if(Site != null)
            {
                currentInfo = Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);
            }
			var units = EngineeringUnit.FmvUsGal;
			this._WingToWingToleranceValue = new SIDouble(EngineeringUnit.FmmLb, currentInfo, 0.0);
			this._TankToTankToleranceValue = new SIDouble(EngineeringUnit.FmmLb, currentInfo, 0.0);
			this._FuelServiceToleranceValue = new SIDouble(EngineeringUnit.FmmLb, currentInfo, 0.0);
			this._FuelServiceToleranceMaxValue = new SIDouble(EngineeringUnit.FmmLb, currentInfo, 0.0);

			this.Initialize();

			if (Site != null)
			{
				currentInfo = Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
				this.SiteGuid = Site.IdentityGuid;
				units = Site.GetSiteUnits(SITE_VARIABLE_TYPE.VOLUME);
			}

			this._Capacity = new SIDouble(units, currentInfo, 0.0);
			this._SafeFill = new SIDouble(units, currentInfo, 0.0);
		}
		#endregion

		#region Public methods
		/// <summary>
		/// The type ID.
		/// </summary>
		/// <param name="equipmentType">
		/// The type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string TypeID(EQUIPMENT_TYPE equipmentType)
		{
			switch ( equipmentType )
			{
				case EQUIPMENT_TYPE.TRAILER_TYPE:
					return "Trailer";
				case EQUIPMENT_TYPE.TRACTOR_TYPE:
					return "Tractor";
				case EQUIPMENT_TYPE.AIRCRAFT_TYPE:
					return "Aircraft";
				case EQUIPMENT_TYPE.RAILCAR_TYPE:
					return "Railcar";
				case EQUIPMENT_TYPE.BARGE_TYPE:
					return "Barge";
				case EQUIPMENT_TYPE.COMPARTMENT_TYPE:
					return "Compartment";
				case EQUIPMENT_TYPE.SHIP_TYPE:
					return "Ship";
				case EQUIPMENT_TYPE.PIPELINE_TYPE:
					return "Pipeline";
				case EQUIPMENT_TYPE.HYDRANT_CART_TYPE:
					return "HydrantCart";
				case EQUIPMENT_TYPE.TANKER_TYPE:
					return "Tanker";
				case EQUIPMENT_TYPE.STATIONARY_CART_TYPE:
					return "StationaryCart";
				case EQUIPMENT_TYPE.OTHER_TYPE:
					return "Other";
				case EQUIPMENT_TYPE.SYSTEM_TYPE:
					return "System";
				case EQUIPMENT_TYPE.TANK_TYPE:
					return "Tank";
				case EQUIPMENT_TYPE.FILLSTAND_TYPE:
					return "Fillstand";
				case EQUIPMENT_TYPE.CONTAINER:
					return "Container";
				case EQUIPMENT_TYPE.VEHICLE:
					return "Vehicle";
				case EQUIPMENT_TYPE.INFRASTRUCTURE:
					return "Infrastructure";
				case EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE:
					return "{All}";
				default:
					return "Undefined";
			}
		}

		/// <summary>
		/// The type.
		/// </summary>
		/// <param name="typeId">
		/// The type ID.
		/// </param>
		/// <returns>
		/// The <see cref="EQUIPMENT_TYPE"/>.
		/// </returns>
		public static EQUIPMENT_TYPE Type(string typeId)
		{
			if (typeId == "Trailer")
			{
				return EQUIPMENT_TYPE.TRAILER_TYPE;
			}

			if (typeId == "Tractor")
			{
				return EQUIPMENT_TYPE.TRACTOR_TYPE;
			}

			if (typeId == "Aircraft")
			{
				return EQUIPMENT_TYPE.AIRCRAFT_TYPE;
			}

			if (typeId == "Railcar")
			{
				return EQUIPMENT_TYPE.RAILCAR_TYPE;
			}

			if (typeId == "Barge")
			{
				return EQUIPMENT_TYPE.BARGE_TYPE;
			}

			if (typeId == "Compartment")
			{
				return EQUIPMENT_TYPE.COMPARTMENT_TYPE;
			}

			if (typeId == "Ship")
			{
				return EQUIPMENT_TYPE.SHIP_TYPE;
			}

			if (typeId == "Pipeline")
			{
				return EQUIPMENT_TYPE.PIPELINE_TYPE;
			}

			if (typeId == "HydrantCart")
			{
				return EQUIPMENT_TYPE.HYDRANT_CART_TYPE;
			}

			if (typeId == "Tanker")
			{
				return EQUIPMENT_TYPE.TANKER_TYPE;
			}

			if (typeId == "StationaryCart")
			{
				return EQUIPMENT_TYPE.STATIONARY_CART_TYPE;
			}

			if (typeId == "Other")
			{
				return EQUIPMENT_TYPE.OTHER_TYPE;
			}

			if (typeId == "System")
			{	
				return EQUIPMENT_TYPE.SYSTEM_TYPE;
			}

			if (typeId == "Tank")
			{
				return EQUIPMENT_TYPE.TANK_TYPE;
			}

			if (typeId == "Fillstand")
			{
				return EQUIPMENT_TYPE.FILLSTAND_TYPE;
			}

			if (typeId == "Container")
			{
				return EQUIPMENT_TYPE.CONTAINER;
			}

			if (typeId == "Vehicle")
			{
				return EQUIPMENT_TYPE.VEHICLE;
			}

			if (typeId == "Infrastructure")
			{
				return EQUIPMENT_TYPE.INFRASTRUCTURE;				
			}

			return EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;
		}

		/// <summary>
		/// The type tolerance.
		/// </summary>
		/// <param name="toleranceType">
		/// The tolerance type.
		/// </param>
		/// <returns>
		/// The <see cref="TOLERANCE_TYPE"/>.
		/// </returns>
		public static TOLERANCE_TYPE TypeTolerance(string toleranceType)
		{
			if (toleranceType == "Mass")
			{
				return TOLERANCE_TYPE.Mass;
			}

			if (toleranceType == "Volume")
			{
				return TOLERANCE_TYPE.Volume;
			}

			if (toleranceType == "Percentage")
			{
				return TOLERANCE_TYPE.Percentage;
			}

			return TOLERANCE_TYPE.MAX_TOLERANCE_TYPE;
		}

		public static string TypeTolerance(TOLERANCE_TYPE toleranceType)
		{
			switch (toleranceType)
			{
				case TOLERANCE_TYPE.Mass:
					return "Mass";
				case TOLERANCE_TYPE.Volume:
					return "Volue";
				case TOLERANCE_TYPE.Percentage:
					return "Percentage";
				default:
					return "Undefined";
			}
		}

		/// <summary>
		/// The has compartments.
		/// </summary>
		/// <param name="Type">
		/// The type.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public static bool HasCompartments(EQUIPMENT_TYPE Type)
		{
			if (Type == EQUIPMENT_TYPE.BARGE_TYPE ||
				Type == EQUIPMENT_TYPE.RAILCAR_TYPE ||
				Type == EQUIPMENT_TYPE.SHIP_TYPE ||
				Type == EQUIPMENT_TYPE.TANKER_TYPE ||
				Type == EQUIPMENT_TYPE.TRAILER_TYPE ||
				Type == EQUIPMENT_TYPE.OTHER_TYPE)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="o">
		/// The object to load.
		/// </param>
		public override void Load(object o)
		{
			this.Reset();

			if ( typeof(DataSet).IsInstanceOfType(o) )
			{
				var set = (DataSet) o;
				DataTable table = set.Tables[0];

				if (table.Rows.Count == 0)
				{
					return;
				}

				DataRow row = table.Rows[0];

				this.IdentityGuid						= DataObject.getValue<Guid>(row["EquipmentTypeGuid"], Guid.Empty);
				this.SiteGuid							= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
				this.ID									= DataObject.getValue<string>(row["EqTypeName"], string.Empty);
				this._Description						= DataObject.getValue<string>(row["EqTypeDescription"], string.Empty);
				this._Make								= DataObject.getValue<string>(row["Make"], string.Empty);
				this._Model								= DataObject.getValue<string>(row["Model"], string.Empty);
				this._Isspt								= DataObject.getValue<string>(row["Isspt"], string.Empty);
				this._Year								= DataObject.getValue<short>(row["Year"], 0);
				this._CustomerDesignator				= DataObject.getValue<string>(row["CustomerDesignator"], string.Empty);
				this._ServiceTime						= DataObject.getValue<double>(row["ServiceTime"], 0.0);
				this._Product							= DataObject.getValue<Guid>(row["ProductGuid"], Guid.Empty);
				this._VolumeUnits						= (EngineeringUnit) DataObject.getValue<int>(row["VolumeUnits"], 0);
				this._VolumeDecimalPlaces				= DataObject.getValue<short>(row["VolumeDecimalPlaces"], 0);
				this._MassUnits							= (EngineeringUnit) DataObject.getValue<int>(row["MassUnits"], 0);
				this._MassDecimalPlaces					= DataObject.getValue<short>(row["MassDecimalPlaces"], 0);
				this._WingToWingToleranceType			= (TOLERANCE_TYPE) DataObject.getValue<short>(row["WingToWingToleranceType"], 0);
				this._TankToTankToleranceType			= (TOLERANCE_TYPE) DataObject.getValue<short>(row["TankToTankToleranceType"], 0);
				this._FuelServiceToleranceType			= (TOLERANCE_TYPE) DataObject.getValue<short>(row["FuelServiceToleranceType"], 0);
				this._FuelServiceToleranceMaxType		= (TOLERANCE_TYPE) DataObject.getValue<short>(row["FuelServiceToleranceMaxType"], 0);
				this._Attribute							= DataObject.getValue<EQUIPMENT_TYPE>(row["LookupEquipmentTypeIndex"], EQUIPMENT_TYPE.COMPARTMENT_TYPE);
				this._CompanyRoleAssignmentConstraint	= DataObject.getValue<COMPANY_ROLE>(row["LookupCompanyRoleIndex"], COMPANY_ROLE.MAX_COMPANY_ROLE);
				this._MultiCompartment					= DataObject.getValue<bool>(row["MultiCompartment"], false);
				this._AllowFuelingByWeight				= DataObject.getValue<bool>(row["AllowFuelingByWeight"], true);
				this._CreatedDate						= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				this._CreatedBy							= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
				this._UpdatedDate						= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
				this._UpdatedBy							= DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
				this.productId							= DataObject.getValue<string>(row["ProductID"], string.Empty);

				this._Capacity.Units					= this._VolumeUnits;
				this._SafeFill.Units					= this._VolumeUnits;
				this._Capacity.numberDecimalDigits		= this._VolumeDecimalPlaces;
				this._SafeFill.numberDecimalDigits		= this._VolumeDecimalPlaces;
				this._Capacity.SIValue					= DataObject.getValue<double>(row["Capacity"], 0.0);
				this._SafeFill.SIValue					= DataObject.getValue<double>(row["SafeFill"], 0.0);

				switch ( this._WingToWingToleranceType )
				{
					case TOLERANCE_TYPE.Volume:
						this._WingToWingToleranceValue.Units = this._VolumeUnits;
						this._WingToWingToleranceValue.numberDecimalDigits = this._VolumeDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Mass:
						this._WingToWingToleranceValue.Units = this._MassUnits;
						this._WingToWingToleranceValue.numberDecimalDigits = this._MassDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Percentage:
						this._WingToWingToleranceValue.Units = EngineeringUnit.FmSiteUnits;
						this._WingToWingToleranceValue.numberDecimalDigits = 2;
						break;
				}

				switch ( this._TankToTankToleranceType )
				{
					case TOLERANCE_TYPE.Volume:
						this._TankToTankToleranceValue.Units = this._VolumeUnits;
						this._TankToTankToleranceValue.numberDecimalDigits = this._VolumeDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Mass:
						this._TankToTankToleranceValue.Units = this._MassUnits;
						this._TankToTankToleranceValue.numberDecimalDigits = this._MassDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Percentage:
						this._TankToTankToleranceValue.Units = EngineeringUnit.FmSiteUnits;
						this._TankToTankToleranceValue.numberDecimalDigits = 2;
						break;
				}

				switch ( this._FuelServiceToleranceType )
				{
					case TOLERANCE_TYPE.Volume:
						this._FuelServiceToleranceValue.Units = this._VolumeUnits;
						this._FuelServiceToleranceValue.numberDecimalDigits = this._VolumeDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Mass:
						this._FuelServiceToleranceValue.Units = this._MassUnits;
						this._FuelServiceToleranceValue.numberDecimalDigits = this._MassDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Percentage:
						this._FuelServiceToleranceValue.Units = EngineeringUnit.FmSiteUnits;
						this._FuelServiceToleranceValue.numberDecimalDigits = 2;
						break;
				}

				switch ( this._FuelServiceToleranceMaxType )
				{
					case TOLERANCE_TYPE.Volume:
						this._FuelServiceToleranceMaxValue.Units = this._VolumeUnits;
						this._FuelServiceToleranceMaxValue.numberDecimalDigits = this._VolumeDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Mass:
						this._FuelServiceToleranceMaxValue.Units = this._MassUnits;
						this._FuelServiceToleranceMaxValue.numberDecimalDigits = this._MassDecimalPlaces;
						break;
				}

				this._WingToWingToleranceValue.SIValue = DataObject.getValue<double>(row["WingToWingToleranceValue"], 0.0);
				this._TankToTankToleranceValue.SIValue = DataObject.getValue<double>(row["TankToTankToleranceValue"], 0.0);
				this._FuelServiceToleranceValue.SIValue = DataObject.getValue<double>(row["FuelServiceToleranceValue"], 0.0);
				this._FuelServiceToleranceMaxValue.SIValue = DataObject.getValue<double>(row["FuelServiceToleranceMaxValue"], 0.0);

				if ( !this._Capacity.Format.IsReadOnly )
				{
					this._Capacity.Format.NumberDecimalDigits = this._VolumeDecimalPlaces;
				}

				if ( !this._SafeFill.Format.IsReadOnly )
				{
					this._SafeFill.Format.NumberDecimalDigits = this._VolumeDecimalPlaces;
				}
			}
		}

		/// <summary>
		/// The select SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL Command.
		/// </param>
		/// <param name="inTransaction">
		/// The transaction.
		/// </param>
		public void SelectSQL(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = "SELECT tblEquipmentTypes.*, tblProducts.ProductID " +
							  " FROM dbo.tblEquipmentTypes  " + SQLUpdateLock(inTransaction) +
							  " LEFT OUTER JOIN dbo.tblProducts ON tblEquipmentTypes.ProductGuid = tblProducts.ProductGuid " +
							  " WHERE dbo.tblEquipmentTypes.EquipmentTypeGuid = @EquipmentTypeGuid";

			cmd.Parameters.Add("@EquipmentTypeGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@EquipmentTypeGuid"].Value = this.IdentityGuid;
		}

		/// <summary>
		/// The select by ID SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="inTransaction">
		/// The transaction.
		/// </param>
		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool inTransaction)
		{
			cmd.CommandText = "SELECT tblEquipmentTypes.*, tblProducts.ProductID " +
							  " FROM dbo.tblEquipmentTypes " + SQLUpdateLock(inTransaction) +
							  " LEFT OUTER JOIN dbo.tblProducts ON tblEquipmentTypes.ProductGuid = tblProducts.ProductGuid " +
							  " WHERE " + this.AppendSiteWhereClause(cmd, security, "tblEquipmentTypes", "EquipmentTypeGuid") +
							  " AND dbo.tblEquipmentTypes.EqTypeName = @ID";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters["@ID"].Value = this.ID;
		}


		/// <summary>
		/// This method will return a SQL statement that retrieves a list of EquipmentType using the EquipmentType types and
		/// search filter as a criterion.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="filter">
		/// The filter.
		/// </param>
		/// <param name="order">
		/// The order.
		/// </param>
		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, string filter, string order)
		{
			var selectFrom = "DECLARE @VolumeUnitIndex int" +
								" SET @VolumeUnitIndex = (SELECT VolumeUnitIndex FROM tblSites WHERE tblSites.SiteGuid = '" + security.SiteGuid.ToString() + "')" +
								" DECLARE @VolumeDecimalPlaces int" +
								" SET @VolumeDecimalPlaces = (SELECT VolumeDecimalPlaces FROM tblSites WHERE tblSites.SiteGuid = '" + security.SiteGuid.ToString() + "')" +
								" SELECT tblEquipmentTypes.EquipmentTypeGuid, tblEquipmentTypes.SiteGuid, tblEquipmentTypes.EqTypeName, " +
								" tblEquipmentTypes.EqTypeDescription, " +
                                " CASE WHEN tblEquipmentTypes.LookUpEquipmentTypeIndex = 2 THEN " + 
                                " CASE WHEN tblEquipmentTypes.FuelServiceToleranceType = 0 THEN dbo.udf_ConvertFromSIUnits(tblEquipmentTypes.Capacity, tblEquipmentTypes.MassUnits, tblEquipmentTypes.MassDecimalPlaces) " +
                                " ELSE dbo.udf_ConvertFromSIUnits(tblEquipmentTypes.Capacity, tblEquipmentTypes.VolumeUnits, tblEquipmentTypes.VolumeDecimalPlaces) "+ 
                                " END " +
                                " ELSE dbo.udf_ConvertFromSIUnits(tblEquipmentTypes.Capacity, @VolumeUnitIndex, @VolumeDecimalPlaces) " +
                                " END as Capacity, " +
                                " CASE WHEN tblEquipmentTypes.LookUpEquipmentTypeIndex = 2 THEN " +
                                " CASE WHEN tblEquipmentTypes.FuelServiceToleranceType = 0 THEN dbo.udf_ConvertFromSIUnits(tblEquipmentTypes.SafeFill, tblEquipmentTypes.MassUnits, tblEquipmentTypes.MassDecimalPlaces) " +
                                " ELSE dbo.udf_ConvertFromSIUnits(tblEquipmentTypes.SafeFill, tblEquipmentTypes.VolumeUnits, tblEquipmentTypes.VolumeDecimalPlaces) " +
                                " END " +
                                " ELSE dbo.udf_ConvertFromSIUnits(tblEquipmentTypes.SafeFill, @VolumeUnitIndex, @VolumeDecimalPlaces) " +
                                " END as SafeFill, " +
								" tblEquipmentTypes.Make, tblEquipmentTypes.Model, tblEquipmentTypes.Year, tblEquipmentTypes.LookUpEquipmentTypeIndex, " +
								" tblEquipmentTypes.CreatedDate, tblEquipmentTypes.CreatedBy, tblEquipmentTypes.UpdatedDate, " +
								" tblEquipmentTypes.UpdatedBy, tblEquipmentTypes.CustomerDesignator, tblEquipmentTypes.ServiceTime, " +
								" tblEquipmentTypes.ProductGuid, tblEquipmentTypes.VolumeUnits, tblEquipmentTypes.VolumeDecimalPlaces, " +
								" tblEquipmentTypes.MassUnits, tblEquipmentTypes.MassDecimalPlaces, tblEquipmentTypes.WingToWingToleranceType, " +
								" tblEquipmentTypes.WingToWingToleranceValue, tblEquipmentTypes.TankToTankToleranceType, " +
								" tblEquipmentTypes.TankToTankToleranceValue, tblEquipmentTypes.FuelServiceToleranceType, " +
								" tblEquipmentTypes.FuelServiceToleranceValue, tblEquipmentTypes.FuelServiceToleranceMaxType, " +
								" tblEquipmentTypes.FuelServiceToleranceMaxValue, tblEquipmentTypes.AllowFuelingByWeight, " +
								" tblEquipmentTypes.DeleteFlag,Isspt, tblEquipmentTypes.MultiCompartment, tblEquipmentTypes.LookupCompanyRoleIndex, " +
								" p.ProductID " +
								" FROM dbo.tblEquipmentTypes LEFT OUTER JOIN dbo.tblProducts p ON tblEquipmentTypes.ProductGuid = p.ProductGuid ";

			var where = " WHERE " + this.AppendSiteWhereClause(cmd, security, "tblEquipmentTypes", "EquipmentTypeGuid") +
			" AND LookupEquipmentTypeIndex <> @CompartmentType ";

			cmd.Parameters.Add("@CompartmentType", SqlDbType.Int);
			cmd.Parameters["@CompartmentType"].Value = (int)EQUIPMENT_TYPE.COMPARTMENT_TYPE;

			var orderClause = "EqTypeName ASC";
			if (string.IsNullOrEmpty(order) == false)
			{
				orderClause = order;
			}

			var orderBy = " ORDER BY " + orderClause;

			if (!string.IsNullOrEmpty(filter))
			{
				var yearSearch = string.Empty;
				var attributeSearch = string.Empty;

				int filterYear;

				if (int.TryParse(filter, out filterYear))
				{
					yearSearch = " OR dbo.tblEquipmentTypes.Year = @FilterYear ";

					cmd.Parameters.Add("@FilterYear", SqlDbType.Int);
					cmd.Parameters["@FilterYear"].Value = filterYear;
				}

				for (EQUIPMENT_TYPE e = 0; e < EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; e++)
				{
					if (TypeID(e).ToUpper().Contains(filter))
					{
						attributeSearch = " OR dbo.tblEquipmentTypes.LookupEquipmentTypeIndex = @EquipmentType ";
						cmd.Parameters.Add("@EquipmentType", SqlDbType.Int);
						cmd.Parameters["@EquipmentType"].Value = (int)e;

						break;
					}
				}

				where +=
					" AND (tblEquipmentTypes.EqTypeName LIKE(@SearchFilter)" +
					" OR dbo.tblEquipmentTypes.EqTypeDescription LIKE(@SearchFilter)" +
					yearSearch +
					" OR dbo.tblEquipmentTypes.Isspt LIKE(@SearchFilter)" +
					" OR dbo.tblEquipmentTypes.Make LIKE(@SearchFilter)" +
					" OR dbo.tblEquipmentTypes.Model LIKE(@SearchFilter)" +
					attributeSearch +
					")";

				cmd.Parameters.Add("@SearchFilter", SqlDbType.NVarChar, 255);
				cmd.Parameters["@SearchFilter"].Value = "%" + filter + "%";
			}

			cmd.CommandText = selectFrom + where + orderBy;
		}

		/// <summary>
		/// The get airplane tank capacity unit.
		/// </summary>
		/// <returns>
		/// The <see cref="EngineeringUnit"/>.
		/// </returns>
		public EngineeringUnit GetAirplaneTankCapacityUnit()
		{
			switch ( this._FuelServiceToleranceMaxType )
			{
				case TOLERANCE_TYPE.Volume:
					return this._VolumeUnits;
				case TOLERANCE_TYPE.Mass:
					return this._MassUnits;
			}

			return this._VolumeUnits;
		}

		/// <summary>
		/// The get airplane tank capacity decimal places.
		/// </summary>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
		public int GetAirplaneTankCapacityDecimalPlaces()
		{
			switch ( this._FuelServiceToleranceMaxType )
			{
				case TOLERANCE_TYPE.Volume:
					return this._VolumeDecimalPlaces;
				case TOLERANCE_TYPE.Mass:
					return this._MassDecimalPlaces;
			}

			return this._VolumeDecimalPlaces;
		}
		#endregion

		#region Private and internal methods
		/// <summary>
		/// The update tank capacity parameters.
		/// </summary>
		protected void UpdateTankCapacityParameters()
		{
			EngineeringUnit units = this._VolumeUnits;
			int decimalPlaces = this._VolumeDecimalPlaces;

			if ( this._Attribute == EQUIPMENT_TYPE.AIRCRAFT_TYPE )
			{
				switch ( this._FuelServiceToleranceMaxType )
				{
					case TOLERANCE_TYPE.Volume:
						units = this._VolumeUnits;
						decimalPlaces = this._VolumeDecimalPlaces;
						break;
					case TOLERANCE_TYPE.Mass:
						units = this._MassUnits;
						decimalPlaces = this._MassDecimalPlaces;
						break;
				}

				double sum = 0;
				foreach ( AirplaneTankClass tank in this._TankCollection )
				{
					tank.SetCapacityParameters(units, decimalPlaces);
					sum += tank.CapacityValue;
				}

				this._Capacity.Units = units;
				this._Capacity.numberDecimalDigits = decimalPlaces;
				this._Capacity.Value = sum;
			}
		}

		/// <summary>
		/// The reset.
		/// </summary>
		public override void Reset()
		{
			base.Reset( );
			this.Initialize();
		}

		/// <summary>
		/// The initialize.
		/// </summary>
		private void Initialize()
		{
			this._Description = string.Empty;
			this._Make = string.Empty;
			this._Model = string.Empty;
			this._Isspt = string.Empty;
			this._Year = 0;
			this._CustomerDesignator = string.Empty;
			this._ServiceTime = 0;
			this._Product = Guid.Empty;
			this.productId = string.Empty;
			this._VolumeUnits = EngineeringUnit.FmvCm3;
			this._VolumeDecimalPlaces = 2;
			this._MassUnits = EngineeringUnit.FmmLb;
			this._MassDecimalPlaces = 2;
			this._WingToWingToleranceType = TOLERANCE_TYPE.Mass;
			this._WingToWingToleranceValue.SIValue = 0;
			this._WingToWingToleranceValue.numberDecimalDigits = this._MassDecimalPlaces;
			this._WingToWingToleranceValue.Units = this._MassUnits;
			this._TankToTankToleranceType = TOLERANCE_TYPE.Mass;
			this._TankToTankToleranceValue.SIValue = 0;
			this._TankToTankToleranceValue.numberDecimalDigits = this._MassDecimalPlaces;
			this._TankToTankToleranceValue.Units = this._MassUnits;
			this._FuelServiceToleranceType = TOLERANCE_TYPE.Mass;
			this._FuelServiceToleranceValue.SIValue = 0;
			this._FuelServiceToleranceValue.numberDecimalDigits = this._MassDecimalPlaces;
			this._FuelServiceToleranceValue.Units = this._MassUnits;
			this._FuelServiceToleranceMaxType = TOLERANCE_TYPE.Mass;
			this._FuelServiceToleranceMaxValue.SIValue = 0;
			this._FuelServiceToleranceMaxValue.numberDecimalDigits = this._MassDecimalPlaces;
			this._FuelServiceToleranceMaxValue.Units = this._MassUnits;
			this._Attribute = EQUIPMENT_TYPE.COMPARTMENT_TYPE;
			this._CompanyRoleAssignmentConstraint = COMPANY_ROLE.MAX_COMPANY_ROLE;
			this._MultiCompartment = false;
			this._AllowFuelingByWeight = true;
			this.ReqQualificationsCollection = new QualificationMapCollectionClass( );
			this.ReqTrainingCollection = new QualificationMapCollectionClass( );
		}
		#endregion
	}
}
