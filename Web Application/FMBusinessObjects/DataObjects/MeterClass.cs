// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeterClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// This class represents a meter, which can be assigned to a piece of equipment, a tank, or a load arm injector / component
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
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

	 [Serializable]
	 [CollectionDataContract]
	 [KnownType(typeof(MeterClass))]
	 public class MeterCollectionClass : List<MeterClass>
	 {
	 }

	 /// <summary>
	 /// This class represents a meter, which can be assigned to a piece of equipment, a tank, or a load arm injector / component
	 /// </summary>
	 [Serializable]
	[DataContract]
	public class MeterClass : BaseDataObject
	{
		/// <summary>
		/// Indicates if the meter rotates backwards or not
		/// </summary>
		[EntityImportExportAttribute("ROTATESBACKWARDS", 10, "RotatesBackwardsFlag")]
		[DataMember]
		public bool RotatesBackwardsFlag { get; set; }

		/// <summary>
		/// Indicates if the meter is a receipt meter
		/// </summary>
		[EntityImportExportAttribute("RECEIPTMETER", 10, "ReceiptMeterFlag")]
		[DataMember]
		public bool ReceiptMeterFlag { get; set; }

		/// <summary>
		/// The number of digits in the meter. This is used when calculating rollover
		/// </summary>
		[EntityImportExportAttribute("NUMBEROFDIGITS", 2, "NumberOfDigits")]
		[DataMember]
		public int NumberOfDigits { get; set; }

		/// <summary>
		/// Used to adjust meter readings to show the actual volume measured by the meter
		/// </summary>
		[EntityImportExportAttribute("METERFACTOR", 8, "MeterFactor")]
		[DataMember]
		public double? MeterFactor { get; set; }

		/// <summary>
		/// MeterCompression
		/// </summary>
		[EntityImportExportAttribute("FUELCOMPRESSIONFACTOR", 8, "FuelCompressionFactor")]
		[DataMember]
		public double? FuelCompressionFactor { get; set; }

		[DataMember]
		public string DcuID { get; set; }

		[DataMember]
		public double? DcuBatteryVoltage { get; set; }

		[DataMember]
		public double? DcuBatteryCurrent { get; set; }

		[DataMember]
		public double? DcuTemperature { get; set; }

		[DataMember]
		public int? DcuResets { get; set; }

		[DataMember]
		public DateTimeOffset? DcuUpdateDate { get; set; }

		[DataMember]
		public DateTimeOffset? DcuConfigurationDate { get; set; }

		[DataMember]
		public string DcuFirmwareVersion { get; set; }

		[DataMember]
		public string DcuBluetoothAddress { get; set; }

		  //deep copy constructor
		  public MeterClass(MeterClass meter) : base(meter)
		  {
				this.RotatesBackwardsFlag = meter.RotatesBackwardsFlag;
				this.ReceiptMeterFlag = meter.ReceiptMeterFlag;
				this.NumberOfDigits = meter.NumberOfDigits;
				this.MeterFactor = meter.MeterFactor;
				this.FuelCompressionFactor = meter.FuelCompressionFactor;
				this.DcuID = meter.DcuID;
				this.DcuBatteryVoltage = meter.DcuBatteryVoltage;
				this.DcuBatteryCurrent = meter.DcuBatteryCurrent;
				this.DcuTemperature = meter.DcuTemperature;
				this.DcuResets = meter.DcuResets;
				this.DcuUpdateDate = meter.DcuUpdateDate;
				this.DcuConfigurationDate = meter.DcuConfigurationDate;
				this.DcuFirmwareVersion = meter.DcuFirmwareVersion;
				this.DcuBluetoothAddress = meter.DcuBluetoothAddress;
				this.EntityType = meter.EntityType;
				this.ParentEntityType = meter.ParentEntityType;
		  }

		  public MeterClass()
		  {

		  }
		/// <summary>
		/// Return a string representation of the number of digits to allow grids to bind to this property instead of the number of digits.
		/// This eliminates the awkwardness of displaying 0 as the number of digits for things like the "all" or Empty selection on the meter select form
		/// </summary>
		public string NumberOfDigitsString
		{
			get
			{
				if (this.NumberOfDigits > 0)
				{
					return this.NumberOfDigits.ToString();
				}
				else
				{
					return string.Empty;
				}
			}
		}

		/// <summary>
		/// Return a string representation of the MeterFactor to allow grids to bind to this property instead of the number of digits.
		/// This eliminates the awkwardness of displaying 0 as the MeterFactor for things like the "all" or Empty selection on the meter select form
		/// </summary>
		public string MeterFactorString
		{
			get
			{
				if (this.MeterFactor.HasValue && this.MeterFactor.Value > 0)
				{
					double MeterFactor = this.MeterFactor.Value;
					return MeterFactor.ToString("F4", CultureInfo.InvariantCulture);
				}
				else
				{
					return string.Empty;
				}
			}
		}

		/// <summary>
		/// Return a string representation of the FuelCompressionFactor to allow grids to bind to this property instead of the number of digits.
		/// This eliminates the awkwardness of displaying 0 as the FuelCompressionFactor for things like the "all" or Empty selection on the meter select form
		/// </summary>
		public string FuelCompressionFactorString
		{
			get
			{
				if (this.FuelCompressionFactor.HasValue && this.FuelCompressionFactor.Value > 0)
				{
					double FuelCp = this.FuelCompressionFactor.Value;

					return FuelCp.ToString("F4", CultureInfo.InvariantCulture);
				}
				else
				{
					return string.Empty;
				}
			}
		}

		/// <summary>
		/// Override ID so that we can use SetString to validate the length of the input value
		/// </summary>
		[EntityImportExportAttribute("METERID*", 30, "ID")]
		override public string ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				SetString("Meter ID", 30, value, ref this._ID);
			}
		}

		/// <summary>
		/// The entity type. In this case, a meter.
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.METER;
			}
		}

		/// <summary>
		/// The parent entity type of a meter. 
		/// This isn't used, but we override to return NONE instead of the default UNDEFINED
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		/// <summary>
		/// Blanks out the data in the meter object
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			this.RotatesBackwardsFlag = false;
			this.ReceiptMeterFlag = false;
			this.NumberOfDigits = 0;
			this.MeterFactor = 1.0;
			this.FuelCompressionFactor = 1.0;
			this.DcuID = string.Empty;
			this.DcuBatteryVoltage = null;
			this.DcuBatteryCurrent = null;
			this.DcuTemperature = null;
			this.DcuResets = null;
			this.DcuUpdateDate = null;
			this.DcuConfigurationDate = null;
			this.DcuFirmwareVersion = string.Empty;
			this.DcuBluetoothAddress = string.Empty;
		}

		/// <summary>
		/// Read a meter object from a DataSet
		/// </summary>
		/// <param name="set">A DataSet to read meter information from</param>
		/// <returns>True if loading the meter from the data set was successful</returns>
		public bool Load(DataSet set)
		{
			if (set == null)
			{
				throw new ArgumentNullException("set");
			}

			this.Reset();

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return false;
			}

			DataRow row = table.Rows[0];

			this._IdentityGuid = DataObject.getValue<Guid>(row["MeterGuid"], Guid.Empty);
			this.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this._ID = DataObject.getValue<string>(row["MeterID"], string.Empty);
			this.NumberOfDigits = DataObject.getValue<byte>(row["NumberOfDigits"], 0);
			this.RotatesBackwardsFlag = DataObject.getValue<bool>(row["RotatesBackwardsFlag"], false);
			this.ReceiptMeterFlag = DataObject.getValue<bool>(row["ReceiptMeterFlag"], false);
			this.MeterFactor = DataObject.getValue<double?>(row["MeterFactor"], null);
			this.FuelCompressionFactor = DataObject.getValue<double?>(row["FuelCompressionFactor"], null);
			this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			this.DcuID = DataObject.getValue<string>(row["DcuID"], string.Empty);
			this.DcuBatteryVoltage = DataObject.getValue<double?>(row["DcuBatteryVoltage"], null);
			this.DcuBatteryCurrent = DataObject.getValue<double?>(row["DcuBatteryCurrent"], null);
			this.DcuTemperature = DataObject.getValue<double?>(row["DcuTemperature"], null);
			this.DcuResets = DataObject.getValue<int?>(row["DcuResets"], null);
			this.DcuUpdateDate = DataObject.getValue<DateTimeOffset?>(row["DcuUpdateDate"], null);
			this.DcuConfigurationDate = DataObject.getValue<DateTimeOffset?>(row["DcuConfigurationDate"], null);
			this.DcuFirmwareVersion = DataObject.getValue<string>(row["DcuFirmwareVersion"], string.Empty);
			this.DcuBluetoothAddress = DataObject.getValue<string>(row["DcuBluetoothAddress"], string.Empty);

			return true;
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to insert a meter into the database 
		/// </summary>
		/// <param name="cmd">a SqlCommand object to populate</param>
		public string InsertSQL(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterInsert";

			string identityGuidParameterName = "@MeterGuid";

			SqlParameter identityGuidParam = new SqlParameter(identityGuidParameterName, SqlDbType.UniqueIdentifier);
			identityGuidParam.Direction = ParameterDirection.Output;
			cmd.Parameters.Add(identityGuidParam);

			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100).Value = this._CreatedBy;
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset).Value = this._CreatedDate;

			this.AddCommonInsertUpdateParameters(cmd);

			return identityGuidParameterName;
		}

		  /// <summary>
		  /// Set up a SqlCommand object with the information necessary to update a meter record in the database
		  /// </summary>
		  /// <param name="cmd">A SqlCommand object to populate</param>
		  public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterUpdate";

			cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier).Value = this._IdentityGuid;

			this.AddCommonInsertUpdateParameters(cmd);
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to delete a meter record in the database
		/// </summary>
		/// <param name="cmd">a SqlCommand object to populate</param>
		public void PurgeSQL(SqlCommand cmd)
		{
				//why is this taking so long?  Triggers?
				cmd.CommandTimeout = 200;

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterDelete";

			cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@MeterGuid"].Value = this._IdentityGuid;
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to read a meter record from the database by its primary key
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate</param>
		public void SelectSQL(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterSelect";

			cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@MeterGuid"].Value = this._IdentityGuid;
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to read a meter record from the database by its ID
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate</param>
		public void SelectByIDSQL(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterSelect";

			cmd.Parameters.Add("@MeterID", SqlDbType.NVarChar, 30);
			cmd.Parameters["@MeterID"].Value = this.ID;

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to read a meter record from the database by its ID 
		/// and a specified load arm guid. Only load arm component meters will be returned. 
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate</param>
		/// <param name="loadArmGuid">The load arm to search for meters for.</param>
		public void SelectComponentMeterByIDAndLoadArmGuidSQL(SqlCommand cmd, Guid loadArmGuid)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterSelect";

			cmd.Parameters.Add("@MeterID", SqlDbType.NVarChar, 30);
			cmd.Parameters["@MeterID"].Value = this.ID;

			cmd.Parameters.Add("@LoadArmGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@LoadArmGuid"].Value = loadArmGuid;

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to read all meters belonging to a tank from the database.
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate.</param>
		/// <param name="tankGuid">The primary key of the tank we're looking for meters for.</param>
		public void EnumerateByTank(SqlCommand cmd, Guid tankGuid)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterSelect";

			cmd.Parameters.Add("@TankGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TankGuid"].Value = tankGuid;
		}

		  /// <summary>
		/// Set up a SqlCommand object with the information necessary to read all meters belonging to a piece of equipment from the database.
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate.</param>
		/// <param name="equipmentGuid">The primary key of the equipment we're looking for meters for.</param>
		public void EnumerateByEquipment(SqlCommand cmd, Guid equipmentGuid, Guid siteGuid)
		  {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_MeterSelect";

				cmd.Parameters.Add("@MeterAssetGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@MeterAssetGuid"].Value = equipmentGuid;
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = siteGuid;
		  }

		  /// <summary>
		  /// Set up a SqlCommand object with the information necessary to read all meters belonging to a specified
		  /// asset (equipment, tank, or load arm) from the database.
		  /// </summary>
		  /// <param name="cmd">A SqlCommand object to populate.</param>
		  /// <param name="siteGuid">Identifies the site we're looking for meters in. </param>
		  /// <param name="assetGuid">The primary key of the tank, equipment, or load arm we're looking for meters for</param>	
		  public void EnumerateByAssetGuid(SqlCommand cmd, Guid siteGuid, Guid assetGuid)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterSelect";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;
			cmd.Parameters.Add("@MeterAssetGuid", SqlDbType.UniqueIdentifier).Value = assetGuid;		
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to read all meters belonging to a tank, equipment, or load arm from the database
		/// and filter the results by a partial match on the provided ID parameter
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate.</param>
		/// <param name="siteGuid">Identifies the site we're looking for meters in. </param>
		/// <param name="assetGuid">The primary key of the tank, equipment, or load arm we're looking for meters for.</param>
		/// <param name="meterIDFilterValue">A value to filter the meters returned on. The filter is applied to the meter ID</param>	
		public void EnumerateByAssetGuidAndFilter(SqlCommand cmd, Guid siteGuid, Guid assetGuid, string meterIDFilterValue)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterSelect";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;
			cmd.Parameters.Add("@MeterAssetGuid", SqlDbType.UniqueIdentifier).Value = assetGuid;
			cmd.Parameters.Add("@MeterIDFilterValue", SqlDbType.NVarChar, 30).Value = meterIDFilterValue;		
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to read all meters belonging to a specific site
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate.</param>
		/// <param name="siteGuid">The site to list meters for</param>
		public void Enumerate(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterSelect";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = siteGuid;
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to read all meters belonging to a specific site 
		/// and filter the results on a provided ID value
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate.</param>
		/// <param name="siteGuid">The site to list meters for</param>
		/// <param name="meterIDFilterValue">A value to filter the meters returned on. The filter is applied to the meter ID.</param>
		public void EnumerateAndFilter(SqlCommand cmd, Guid siteGuid, string meterIDFilterValue)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterSelect";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = siteGuid;

			cmd.Parameters.Add("@MeterIDFilterValue", SqlDbType.NVarChar, 30);
			cmd.Parameters["@MeterIDFilterValue"].Value = meterIDFilterValue;
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to insert a tank -> meter map object into the database.
		/// This represents a relationship between a tank and a meter.
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate</param>
		/// <param name="tankGuid">The tank to associate with the meter</param>
		public void InsertTankMapSql(SqlCommand cmd, Guid tankGuid)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterToTankMapInsert";

			cmd.Parameters.Add("@TankGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@MeterGuid"].Value = this._IdentityGuid;
			cmd.Parameters["@TankGuid"].Value = tankGuid;
			cmd.Parameters["@CreatedBy"].Value = this._CreatedBy;
			cmd.Parameters["@CreatedDate"].Value = this._CreatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
		}

		/// <summary>
		/// Delete any tank meter map records that exist for this meter.
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate</param>
		public void DeleteTankMapForMeterSQL(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "map.gsp_MeterToTankDeleteByMeterGuid";

			cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@MeterGuid"].Value = this._IdentityGuid;
		}

		  public void InsertEquipmentMapSQL(SqlCommand cmd, Guid equipmentguid)
		  {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_MeterToEquipmentMapInsert";

				cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);

				cmd.Parameters["@MeterGuid"].Value = this._IdentityGuid;
				cmd.Parameters["@EquipmentGuid"].Value = equipmentguid;
				cmd.Parameters["@CreatedBy"].Value = this._CreatedBy;
				cmd.Parameters["@CreatedDate"].Value = this._CreatedDate;
				cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
				cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
		  }

		  /// <summary>
		/// Delete any equipment meter map records that exist for this meter.
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate</param>
		public void DeleteEquipmentMapForMeterSQL(SqlCommand cmd)
		  {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "map.gsp_MeterToEquipmentDeleteByMeterGuid";

				cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@MeterGuid"].Value = this._IdentityGuid;
		  }

		  /// <summary>
		  /// Validate the input of number of digits. The Number of digits must be present, numeric, and greater than zero.
		  /// This method will throw if an error is detected
		  /// </summary>
		  /// <param name="numberOfDigitsText">The text the user typed in for the number of digits</param>
		  /// <returns>If successful, the number of digits as an integer</returns>
		public static int ValidateNumberOfDigits(string numberOfDigitsText)
		{
			// Validate the input of number of digits. It must be present, numeric, and greater than zero.
			int numberOfDigits = 0;

			if (string.IsNullOrEmpty(numberOfDigitsText))
			{
				throw new ApplicationException("Meter number of digits is required");
			}
			else if (int.TryParse(numberOfDigitsText, out numberOfDigits))
			{
				if (numberOfDigits <= 0)
				{
					throw new ApplicationException("Meter number of digits must be greater than zero");
				}
			}
			else
			{
				throw new ApplicationException("Meter number of digits must be numeric");
			}

			return numberOfDigits;
		}

		  public static string ValidateMeterID(string meterID)
		  {
				if (string.IsNullOrEmpty(meterID))
				{
					 throw new ApplicationException("Meter ID is required.");
				}

				return meterID;
		  }

		/// <summary>
		/// Validate the input of Meter Factor. The Meter Factor must be present, numeric, and greater than zero.
		/// This method will throw if an error is detected
		/// </summary>
		/// <param name="meterFactor">The text the user typed in for the meter factor</param>
		/// <returns>If successful, the meter factor as a double</returns>
		public static double ValidateMeterFactor(string meterFactorText)
		{
			// Validate the input of MeterFactor. It must be present, numeric, and greater than zero.
			double meterFactor = 0.0;
			Regex isRightDecimalFmt = new Regex(@"^\s*(?=.*[1-9])\d{0,3}(?:\.\d{0,4})?\s*$");
			meterFactorText = meterFactorText.Trim();

			if (string.IsNullOrEmpty(meterFactorText))
			{
				throw new ApplicationException("Meter Factor is required");
			}
			else if (isRightDecimalFmt.IsMatch(meterFactorText))
			{
				bool _ = double.TryParse(meterFactorText, out meterFactor);
			}
			else
			{
				throw new ApplicationException("Meter Factor must be positive number in the range from 0.0001 to 999.9999");
			}

			return meterFactor;
		}

		/// <summary>
		/// Validate the input of Fuel Compression Factor. The Fuel Compression Factor must be present, numeric, and greater than zero.
		/// This method will throw if an error is detected
		/// </summary>
		/// <param name="fuelCompressionFactor">The text the user typed in for the Fuel Compression Factor</param>
		/// <returns>If successful, the Fuel Compression Factor as a double</returns>
		public static double ValidateFuelCompressionFactor(string fuelCompressionFactorText)
		{
			// Validate the input of Fuel Compression Factor. It must be present, numeric, and greater than zero.
			double fuelCompressionFactor = 0.0;
			Regex isRightDecimalFmt = new Regex(@"^\s*(?=.*[1-9])\d{0,3}(?:\.\d{0,4})?\s*$");
			fuelCompressionFactorText = fuelCompressionFactorText.Trim();

			if (string.IsNullOrEmpty(fuelCompressionFactorText))
			{
				throw new ApplicationException("Meter Fuel Compression Factor is required");
			}
			else if (isRightDecimalFmt.IsMatch(fuelCompressionFactorText))
			{
				bool _ = double.TryParse(fuelCompressionFactorText, out fuelCompressionFactor);
			}
			else
			{
				throw new ApplicationException("Meter Fuel Compression Factor must be a positive number in the range from 0.0001 to 999.9999");
			}

			return fuelCompressionFactor;
		}
		/// <summary>
		/// Add parameters that are used by both the insert and update stored procedures
		/// </summary>
		/// <param name="cmd">A SqlCommand to add parameters to</param>
		private void AddCommonInsertUpdateParameters(SqlCommand cmd)
		{
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = this.SiteGuid;
			cmd.Parameters.Add("@MeterID", SqlDbType.NVarChar, 30).Value = this._ID;
			cmd.Parameters.Add("@NumberOfDigits", SqlDbType.TinyInt).Value = this.NumberOfDigits;
			cmd.Parameters.Add("@RotatesBackwardsFlag", SqlDbType.Bit).Value = (this.RotatesBackwardsFlag ? 1 : 0);
			cmd.Parameters.Add("@ReceiptMeterFlag", SqlDbType.Bit).Value = (this.ReceiptMeterFlag ? 1 : 0);
			cmd.Parameters.Add("@MeterFactor", SqlDbType.Float).Value = this.MeterFactor == null ? DBNull.Value : (object)this.MeterFactor;
			cmd.Parameters.Add("@FuelCompressionFactor", SqlDbType.Float).Value = this.FuelCompressionFactor == null ? DBNull.Value : (object)this.FuelCompressionFactor;
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = this._UpdatedBy;
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset).Value = this._UpdatedDate;
			cmd.Parameters.Add("@DcuID", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(this.DcuID) ? DBNull.Value : (object)this.DcuID;
			cmd.Parameters.Add("@DcuBatteryVoltage", SqlDbType.Float).Value = this.DcuBatteryVoltage == null ? DBNull.Value : (object)this.DcuBatteryVoltage;
			cmd.Parameters.Add("@DcuBatteryCurrent", SqlDbType.Float).Value = this.DcuBatteryCurrent == null ? DBNull.Value : (object)this.DcuBatteryCurrent;
			cmd.Parameters.Add("@DcuTemperature", SqlDbType.Float).Value = this.DcuTemperature == null ? DBNull.Value : (object)this.DcuTemperature;
			cmd.Parameters.Add("@DcuResets", SqlDbType.Int).Value = this.DcuResets == null ? DBNull.Value : (object)this.DcuResets;
			cmd.Parameters.Add("@DcuUpdateDate", SqlDbType.DateTimeOffset).Value = this.DcuUpdateDate == null ? DBNull.Value : (object)this.DcuUpdateDate;
			cmd.Parameters.Add("@DcuConfigurationDate", SqlDbType.DateTimeOffset).Value = this.DcuConfigurationDate == null ? DBNull.Value : (object)this.DcuConfigurationDate;
			cmd.Parameters.Add("@DcuFirmwareVersion", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(this.DcuFirmwareVersion) ? DBNull.Value : (object)this.DcuFirmwareVersion;
			cmd.Parameters.Add("@DcuBluetoothAddress", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(this.DcuBluetoothAddress) ? DBNull.Value : (object)this.DcuBluetoothAddress;
		}

	}
}
