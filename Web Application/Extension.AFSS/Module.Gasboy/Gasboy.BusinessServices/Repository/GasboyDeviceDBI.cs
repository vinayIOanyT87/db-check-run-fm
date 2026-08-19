// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyDeviceDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements database interaction for the gasboy device functionality
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessServices.Repository
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;

	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

	using Microsoft.SqlServer.Server;

	/// <summary>
	/// Implements database interaction for the gasboy device functionality
	/// </summary>
	public class GasboyDeviceDBI : DataAccessLayerDBI
	{
		#region Attributes
		#endregion Attributes

		/// <summary>
		/// Construct a new GasboyDeviceDBI object and set the user to the provided parameter
		/// </summary>
		/// <param name="user">
		/// The user executing this operation
		/// </param>
		public GasboyDeviceDBI(string user)
			: base(user)
		{
		}

		#region Standard Data Access Methods

		/// <summary>
		/// Gets a list of all the Gasboy devices associated with the specified Site.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="siteGuid">Identifies the site to retrieve the records for</param>
		/// <param name="deviceID">The device ID to search for matches on</param>
		/// <param name="deviceName">The device name to search for matches on</param>
		/// <param name="returnDeleted">Returns Gasboy Devices that were soft deleted (default is false)</param>
		/// <param name="returnAirCards">Returns AirCards in addition to Gasboy devices (default is true)</param>
		/// <returns>All the Gasboy devices associated with the specified Site, filtered by ID if it was provided.</returns>
		public List<GasboyDevice> GetList(SecurityClass security, Guid siteGuid, long? deviceID, string deviceName, bool returnDeleted = false, bool returnAirCards = true)
		{
			var dataObjects = new List<GasboyDevice>();

			DataSet dataSet = this.Load(security, null, siteGuid, deviceName, deviceID, false, returnDeleted, returnAirCards);

			if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return dataObjects;
			}

			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				dataObjects.Add(this.GetDataObjectFromDataRow(row));
			}

			return dataObjects;
		}

		/// <summary>
		/// Gets a specific Gasboy device that has the specified ID for the specified site.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="siteGuid">Identifies the site to retrieve the records for</param>
		/// <param name="deviceID">The device ID to search for matches on</param>
		/// <param name="deviceName">The device name to search for matches on</param>
		/// <returns>The Gasboy Station identified by the provided ID.</returns>
		public GasboyDevice GetByID(SecurityClass security, Guid siteGuid, long? deviceID, string deviceName)
		{
			if (siteGuid == Guid.Empty)
			{
				throw new Exception("SiteGuid parameter cannot be empty.");
			}

			if (string.IsNullOrEmpty(deviceName))
			{
				throw new ArgumentException("deviceName");
			}

			DataSet dataSet = this.Load(security, null, siteGuid, deviceName, deviceID, true);

			if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			var dataObject = this.GetDataObjectFromDataRow(dataSet.Tables[0].Rows[0]);

			return dataObject;
		}

		/// <summary>
		/// Gets a specific Gasboy device that has the specified ID for the specified site.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="siteGuid">Identifies the site to retrieve the records for</param>
		/// <param name="deviceID">The device ID to search for matches on</param>
		/// <param name="deviceName">The device name to search for matches on</param>
		/// <returns>The Gasboy Station identified by the provided ID.</returns>
		public GasboyDevice GetByCardNumber(SecurityClass security, Guid siteGuid, string cardNumber)
		{
			if (siteGuid == Guid.Empty)
			{
				throw new Exception("SiteGuid parameter cannot be empty.");
			}

			if (string.IsNullOrEmpty(cardNumber))
			{
				throw new ArgumentException("cardNumber");
			}

			DataSet dataSet = this.LoadByCardNumber(security, null, siteGuid, cardNumber);

			if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			var dataObject = this.GetDataObjectFromDataRow(dataSet.Tables[0].Rows[0]);

			return dataObject;
		}

		/// <summary>
		/// Gets a specific Gasboy device that has the specified ID for the specified site.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="siteGuid">Identifies the site to retrieve the records for</param>
		/// <param name="deviceName">The device name to search for matches on</param>
		/// <returns>The Gasboy Station identified by the provided ID.</returns>
		public List<GasboyDevice> GetByDepartment(SecurityClass security,Guid? siteGuid, Guid departmentGuid)
		{

			if (departmentGuid == Guid.Empty)
			{
				throw new ArgumentException("departmentguid is empty");
			}



			DataSet dataSet = this.LoadByDepartment(security, siteGuid, departmentGuid);

			if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			var dataObjects = new List<GasboyDevice>();

			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				dataObjects.Add(this.GetDataObjectFromDataRow(row));
			}

			return dataObjects;
		}

		/// <summary>
		/// Gets the current <seealso cref="GasboyDevice"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="identityGuid">Identifies the record to retrieve</param>
		/// <returns>The Gasboy Station identified by the provided identityGuid</returns>
		public GasboyDevice Get(SecurityClass security, Guid identityGuid)
		{
			if (identityGuid == Guid.Empty)
			{
				throw new Exception("IdentityGuid parameter cannot be empty.");
			}

			DataSet dataSet = this.Load(security, identityGuid, null, null, null, false);

			if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			var dataObject = this.GetDataObjectFromDataRow(dataSet.Tables[0].Rows[0]);

			return dataObject;
		}

		/// <summary>
		/// Saves the passed in <seealso cref="GasboyDevice"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The Gasboy Station business object to save.</param>
		public void Insert(SecurityClass security, GasboyDevice dataObject)
		{
			using (SqlCommand cmd = this.PrepareInsertStatement())
			{
				this.BindCommonInsertUpdateParameterValues(cmd, dataObject);

				cmd.Parameters["@CreatedUpdatedBy"].Value = this.CreatedBy;

				this.ConsolidatedDA.ExecuteQuery(security, cmd);   
			}
		}

		/// <summary>
		/// Saves the passed in <seealso cref="GasboyDevice"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The Gasboy Station object to modify in the database</param>
		public void Update(SecurityClass security, GasboyDevice dataObject)
		{
			using (SqlCommand cmd = this.PrepareUpdateStatement())
			{
				this.BindCommonInsertUpdateParameterValues(cmd, dataObject);

				cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;

				this.ConsolidatedDA.ExecuteQuery(security, cmd); 
			}          
		}

		/// <summary>
		/// Deletes the passed in <seealso cref="GasboyDevice"/> record
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The Gasboy Station object to delete</param>
		public void Delete(SecurityClass security, GasboyDevice dataObject)
		{
			using (var cmd = this.PrepareDeleteStatement())
			{
				cmd.Parameters["@IdentityGuid"].Value = dataObject.IdentityGuid;

				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		#endregion Standard Data Access Methods

		#region Private Persistence Methods

		/// <summary>
		/// Gets the <seealso cref="GasboyDevice"/> object(s) from the database identified by the provided parameters
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="identityGuid">Contains the identity guid of a specific record to load.</param>
		/// <param name="siteGuid">Contains the SiteGuid to restrict the results to.</param>
		/// <param name="deviceID">Contains the device ID to load, must be combined with the SiteGuid.</param>
		/// <param name="deviceName">Contains the device name to load, must be combined with the SiteGuid.</param>
		/// <param name="exactIdMatch">If true, only devices that match the site guid AND exact id value will be returned</param>
		/// <param name="returnDeleted">If true, devices that are soft deleted will be included in the enumerate</param>
		/// <param name="ReturnAirCards">If true, AirCards will be included in the enumerate</param>
		/// <returns>A dataset populated with matching gasboy devices or null if no matches exist.</returns>
		private DataSet Load(SecurityClass security, Guid? identityGuid, Guid? siteGuid, string deviceName, long? deviceID, bool exactIdMatch = true, bool returnDeleted = false, bool returnAirCards = true)
		{
			DataSet dataSet = null;

			if (identityGuid.HasValue || exactIdMatch)
			{
				using (var cmd = this.PrepareSelectStatement())
				{
					if (exactIdMatch)
					{
						cmd.Parameters["@SiteGuid"].Value = this.SetOptionalValue<Guid>(siteGuid);
						cmd.Parameters["@DeviceID"].Value = this.SetOptionalValue<long>(deviceID);
						cmd.Parameters["@DeviceName"].Value = this.SetOptionalValue<string>(deviceName);
					}
					else
					{
						cmd.Parameters["@GasboyDeviceGuid"].Value = this.SetOptionalValue<Guid>(identityGuid);
					}

					dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}
			else
			{
				using (var cmd = this.PrepareEnumerateStatement())
				{
					cmd.Parameters["@SiteGuid"].Value = this.SetOptionalValue<Guid>(siteGuid);
					cmd.Parameters["@DeviceName"].Value = this.SetOptionalValue<string>(deviceName);
					cmd.Parameters["@ReturnDeleted"].Value = (returnDeleted);
					cmd.Parameters["@ReturnAirCards"].Value = (returnAirCards);

					dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			return dataSet;
		}

		/// <summary>
		/// Gets the <seealso cref="GasboyDevice"/> object(s) from the database identified by the provided parameters
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="siteGuid">Contains the SiteGuid to restrict the results to.</param>
		/// <param name="departmentGuid">Contains the department to restrict the results to</param>
		/// <returns>A dataset populated with matching gasboy devices or null if no matches exist.</returns>
		private DataSet LoadByDepartment(SecurityClass security, Guid? siteGuid, Guid? departmentGuid)
		{
			DataSet dataSet = null;

			if (departmentGuid.HasValue)
			{
				using (var cmd = this.PrepareSelectStatement())
				{
						cmd.Parameters["@SiteGuid"].Value = this.SetOptionalValue<Guid>(siteGuid);
						cmd.Parameters["@GasboyDepartmentGuid"].Value = this.SetOptionalValue<Guid>(departmentGuid);

					dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}
			else
			{
				throw new Exception("Gasboy Department Guid cannot be empty.");
			}

			return dataSet;
		}

		/// <summary>
		/// Gets the <seealso cref="GasboyDevice"/> object(s) from the database identified by the provided parameters
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="identityGuid">Contains the identity guid of a specific record to load.</param>
		/// <param name="siteGuid">Contains the SiteGuid to restrict the results to.</param>
		/// <param name="cardNumber">Contains the card number to load, must be combined with the SiteGuid.</param>
		/// <param name="exactIdMatch">If true, only devices that match the site guid AND exact id value will be returned</param>
		/// <returns>A dataset populated with matching gasboy devices or null if no matches exist.</returns>
		private DataSet LoadByCardNumber(SecurityClass security, Guid? identityGuid, Guid? siteGuid, string cardNumber, bool exactIdMatch = true)
		{
			DataSet dataSet = null;

			if (identityGuid.HasValue || exactIdMatch)
			{
				using (var cmd = this.PrepareSelectByCardNumberStatement())
				{
					if (exactIdMatch)
					{
						cmd.Parameters["@SiteGuid"].Value = this.SetOptionalValue<Guid>(siteGuid);
						cmd.Parameters["@CardNumber"].Value = this.SetOptionalValue<string>(cardNumber);
					}
					else
					{
						cmd.Parameters["@GasboyDeviceGuid"].Value = this.SetOptionalValue<Guid>(identityGuid);
					}

					dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}
			else
			{
				using (var cmd = this.PrepareEnumerateStatement())
				{
					cmd.Parameters["@SiteGuid"].Value = this.SetOptionalValue<Guid>(siteGuid);
					cmd.Parameters["@CardNumber"].Value = this.SetOptionalValue<string>(cardNumber);

					dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			return dataSet;
		}

		#endregion Private Persistence Methods

		#region Override Implementations for Prepare Methods

		protected override SqlCommand PrepareSelectStatement()
		{
			var cmd = new SqlCommand
						  {
							  CommandType = CommandType.StoredProcedure,
							  CommandText = "usp_GasboyDeviceGet"
						  };

			cmd.Parameters.Add("@GasboyDeviceGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@DeviceID", SqlDbType.BigInt);
			cmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@GasboyDepartmentGuid", SqlDbType.UniqueIdentifier);

			return cmd;
		}

		protected override SqlCommand PrepareInsertStatement()
		{
			var cmd = new SqlCommand
						  {
							  CommandType = CommandType.StoredProcedure,
							  CommandText = "usp_GasboyDeviceInsert"
						  };

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@CreatedUpdatedBy", SqlDbType.NVarChar, 100);

			this.AddCommonInsertUpdateParameters(cmd);

			return cmd;
		}

		protected override SqlCommand PrepareUpdateStatement()
		{
			var cmd = new SqlCommand
						  {
							  CommandType = CommandType.StoredProcedure,
							  CommandText = "usp_GasboyDeviceUpdate"
						  };

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			this.AddCommonInsertUpdateParameters(cmd);

			return cmd;
		}

		protected override SqlCommand PrepareDeleteStatement()
		{
			var cmd = new SqlCommand
						  {
							  CommandType = CommandType.StoredProcedure,
							  CommandText = "usp_GasboyDeviceDelete"
						  };

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			return cmd;
		}

		#endregion Override Implementations for Prepare Methods

		#region Protected Custom Methods

		protected SqlCommand PrepareSelectByCardNumberStatement()
		{
			var cmd = new SqlCommand
			{
				CommandType = CommandType.StoredProcedure,
				CommandText = "usp_GasboyDeviceGetByCardNumber"
			};

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CardNumber", SqlDbType.NVarChar, 50);

			return cmd;
		}

		protected SqlCommand PrepareEnumerateStatement()
		{
			var cmd = new SqlCommand
						  {
							  CommandType = CommandType.StoredProcedure,
							  CommandText = "usp_GasboyDeviceEnumerate"
						  };

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@ReturnDeleted", SqlDbType.Bit);
			cmd.Parameters.Add("@ReturnAirCards", SqlDbType.Bit);

			return cmd;
		}

		protected SqlCommand PrepareEnumerateByCardNumberStatement()
		{
			var cmd = new SqlCommand
			{
				CommandType = CommandType.StoredProcedure,
				CommandText = "usp_GasboyDeviceEnumerateByCardNumber"
			};

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CardNumber", SqlDbType.NVarChar, 50);

			return cmd;
		}

		#endregion Protected Custom Methods

		#region Private Support Methods

		/// <summary>
		/// Add parameters that are used by both the insert and update stored procedures
		/// </summary>
		/// <param name="cmd">A SqlCommand to add parameters to</param>
		private void AddCommonInsertUpdateParameters(SqlCommand cmd)
		{
			cmd.Parameters.Add("@GasboyDeviceGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@GasboyDepartmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@DeviceCode", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@CardNumber", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@GroupRuleName", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@LookupGasboyDeviceTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@LookupGasboyRecordStatusIndex", SqlDbType.Int);
			cmd.Parameters.Add("@LookupGasboyHardwareTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@LookupGasboyAuthorizationTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@LookupGasboyEmployeeTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@LookupGasboyTwoStageDriverValidationTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@UsePINCodeFlag", SqlDbType.Bit);
			cmd.Parameters.Add("@PINCode", SqlDbType.VarBinary, 256);
			cmd.Parameters.Add("@AuthPINFrom", SqlDbType.TinyInt);
			cmd.Parameters.Add("@VehiclePlate", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@PromptForVehiclePlateFlag", SqlDbType.Bit);
			cmd.Parameters.Add("@LookupGasboyVehiclePlateCheckTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@AlwaysPromptForAdditionalValidationFlag", SqlDbType.Bit);
		}

		/// <summary>
		/// Bind parameter values that are used by both the insert and update stored procedures
		/// </summary>
		/// <param name="cmd">A SqlCommand to add parameters to</param>
		/// <param name="dataObject">The Gasboy Station that needs to be persisted.</param>
		private void BindCommonInsertUpdateParameterValues(SqlCommand cmd, GasboyDevice dataObject)
		{
			cmd.Parameters["@GasboyDeviceGuid"].Value = dataObject.IdentityGuid;
			cmd.Parameters["@SiteGuid"].Value = dataObject.SiteGuid;
			cmd.Parameters["@GasboyDepartmentGuid"].Value = dataObject.DepartmentIdentityGuid;
			cmd.Parameters["@DeviceCode"].Value = dataObject.DeviceCode;
			cmd.Parameters["@DeviceName"].Value = dataObject.DeviceName;
			cmd.Parameters["@CardNumber"].Value = dataObject.CardNumber;
			cmd.Parameters["@GroupRuleName"].Value = dataObject.GroupRuleName;
			cmd.Parameters["@LookupGasboyDeviceTypeIndex"].Value = (int)dataObject.DeviceType;
			cmd.Parameters["@LookupGasboyRecordStatusIndex"].Value = (int)dataObject.RecordStatus;
			cmd.Parameters["@LookupGasboyHardwareTypeIndex"].Value = (int)dataObject.HardwareType;
			cmd.Parameters["@LookupGasboyAuthorizationTypeIndex"].Value = (int)dataObject.AuthorizationType;
			cmd.Parameters["@LookupGasboyEmployeeTypeIndex"].Value = (int)dataObject.EmployeeType;
			cmd.Parameters["@LookupGasboyTwoStageDriverValidationTypeIndex"].Value = (int)dataObject.DriverValidationType;

			cmd.Parameters["@UsePINCodeFlag"].Value = dataObject.UsePINCode;
			cmd.Parameters["@PINCode"].Value = !string.IsNullOrEmpty(dataObject.PINCode) ? UserClass.encode(dataObject.PINCode, dataObject.SiteGuid) : (object)DBNull.Value;
			cmd.Parameters["@AuthPINFrom"].Value = dataObject.AuthorizationPINSource;
			cmd.Parameters["@VehiclePlate"].Value = dataObject.VehiclePlate;
			cmd.Parameters["@PromptForVehiclePlateFlag"].Value = dataObject.PromptForVehiclePlate;
			cmd.Parameters["@LookupGasboyVehiclePlateCheckTypeIndex"].Value = (int)dataObject.VehiclePlateCheckType;
			cmd.Parameters["@AlwaysPromptForAdditionalValidationFlag"].Value = dataObject.AlwaysPromptForAdditionalValidation;
		}

		/// <summary>
		/// The get data object from data row.
		/// </summary>
		/// <param name="row">
		/// The row containing the device record
		/// </param>
		/// <returns>
		/// The <see cref="GasboyDevice"/>.
		/// </returns>
		private GasboyDevice GetDataObjectFromDataRow(DataRow row)
		{
			var dataObject = new GasboyDevice
			{
				IdentityGuid = DataObject.getValue(row["GasboyDeviceGuid"], Guid.Empty),
				SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
				DepartmentIdentityGuid = DataObject.getValue(row["GasboyDepartmentGuid"], Guid.Empty),
				DeviceID = DataObject.getValue<long>(row["DeviceID"], 900000001),
				DeviceCode = DataObject.getValue<long?>(row["DeviceCode"], null),
				DeviceName = DataObject.getValue(row["DeviceName"], string.Empty),
				CardNumber = DataObject.getValue(row["CardNumber"], string.Empty),
				GroupRuleName = DataObject.getValue<string>(row["GroupRuleName"], string.Empty),

				//Currently, Fleets and Departments are not managed by FMD so these constants are stored in GasboySpecialConstants
				//When we want FMD to manage these, we need to implement entity mapping and summary detail pages for each. 
				//FleetID = DataObject.getOptionalInt(row.SafeValue("FleetID")),
				//FleetCode = DataObject.getOptionalInt(row.SafeValue("FleetCode")),
				//DepartmentID = DataObject.getOptionalInt(row.SafeValue("DepartmentID")),
				//DepartmentCode = DataObject.getOptionalInt(row.SafeValue("DepartmentCode")),
			};

			if (dataObject.DepartmentIdentityGuid == GasboySpecialConstants.DefaultDepartmentGuid)
			{
				dataObject.DepartmentID = GasboySpecialConstants.DefaultDepartmentID;
				dataObject.DepartmentCode = GasboySpecialConstants.DefaultDepartmentCode;
			}

			if (dataObject.DepartmentIdentityGuid == GasboySpecialConstants.BlacklistDepartmentGuid)
			{
				dataObject.DepartmentID = GasboySpecialConstants.DefaultBlackListDepartmentID;
				dataObject.DepartmentCode = GasboySpecialConstants.DefaultBlackListDepartmentCode;
			}

			dataObject.FleetID = GasboySpecialConstants.DefaultFleetID;
			dataObject.FleetCode = GasboySpecialConstants.DefaultFleetCode;


			int indexValue = DataObject.getValue<int>(row["LookupGasboyDeviceTypeIndex"], (int)GasboyDeviceType.Vehicle);
			GasboyDeviceType deviceType;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out deviceType))
			{
				dataObject.DeviceType = deviceType;
			}

			indexValue = DataObject.getValue<int>(row["LookupGasboyRecordStatusIndex"], (int)GasboyRecordStatus.Active);
			GasboyRecordStatus recordStatus;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out recordStatus))
			{
				dataObject.RecordStatus = recordStatus;
			}

			indexValue = DataObject.getValue<int>(row["LookupGasboyHardwareTypeIndex"], (int)GasboyHardwareType.Tag);
			GasboyHardwareType hardwareType;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out hardwareType))
			{
				dataObject.HardwareType = hardwareType;
			}

			indexValue = DataObject.getValue<int>(row["LookupGasboyAuthTypeIndex"], (int)GasboyAuthorizationType.FuelCard);
			GasboyAuthorizationType authorizationType;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out authorizationType))
			{
				dataObject.AuthorizationType = authorizationType;
			}

			indexValue = DataObject.getValue<int>(row["LookupGasboyEmployeeTypeIndex"], (int)GasboyEmployeeType.Attendant);
			GasboyEmployeeType employeeType;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out employeeType))
			{
				dataObject.EmployeeType = employeeType;
			}

			indexValue = DataObject.getValue<int>(row["LookupGasboyTwoStageDriverValidationTypeIndex"], (int)GasboyTwoStageDriverValidationType.NotSelected);
			GasboyTwoStageDriverValidationType driverValidationType;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out driverValidationType))
			{
				dataObject.DriverValidationType = driverValidationType;
			}

			dataObject.UsePINCode = DataObject.getValue<bool>(row["UsePINCodeFlag"], false);
			dataObject.PINCode = DataObject.GetEncryptedValue<string>(row["PINCode"], dataObject.SiteGuid, string.Empty);
			dataObject.VehiclePlate = DataObject.getValue<string>(row["VehiclePlate"], string.Empty);
			dataObject.PromptForVehiclePlate = DataObject.getValue<bool>(row["PromptForVehiclePlateFlag"], false);

			indexValue = DataObject.getValue<int>(row["LookupGasboyVehiclePlateCheckTypeIndex"], (int)GasboyVehiclePlateCheckType.ValidVehicleNoForCurrentDevice);
			GasboyVehiclePlateCheckType plateCheckType;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out plateCheckType))
			{
				dataObject.VehiclePlateCheckType = plateCheckType;
			}

			dataObject.AlwaysPromptForAdditionalValidation =
				Convert.ToBoolean(DataObject.getValue<byte>(row["AlwaysPromptForAdditionalValidationFlag"], 0));

			dataObject.CreatedDate = DataObject.getValue(row["CreatedDate"], this.CreatedDateTime);
			dataObject.CreatedBy = DataObject.getValue(row["CreatedBy"], this.CreatedBy);
			dataObject.UpdatedDate = DataObject.getValue(row["UpdatedDate"], this.UpdatedDateTime);
			dataObject.UpdatedBy = DataObject.getValue(row["UpdatedBy"], this.UpdatedBy);
		   
			return dataObject;
		}

		#endregion Private Support Methods
	}
}
