// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyDepartmentDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements database interaction for the gasboy department functionality
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessServices.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.DataAccessLayer;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    using Microsoft.SqlServer.Server;
    using System.Globalization;

    /// <summary>
    /// Implements database interaction for the gasboy department functionality
    /// </summary>
    public class GasboyDepartmentDBI : DataAccessLayerDBI
    {
        #region Attributes
        #endregion Attributes

        /// <summary>
        /// Construct a new GasboyDepartmentDBI object and set the user to the provided parameter
        /// </summary>
        /// <param name="user">
        /// The user executing this operation
        /// </param>
        public GasboyDepartmentDBI(string user)
            : base(user)
        {
        }

		#region Standard Data Access Methods

		/// <summary>
		/// Gets a list of all the Gasboy departments associated with the specified Site.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="siteGuid">Identifies the site to retrieve the records for</param>
		/// <param name="departmentName">The department name to search for matches on</param>
		/// <param name="departmentID">The department id to search for matches on</param>
		/// <returns>All the Gasboy departments associated with the specified Site, filtered by ID if it was provided.</returns>
		public GasboyDepartmentCollection GetList(SecurityClass security, Guid siteGuid, string departmentName, long? departmentID)
        {
            var dataObjects = new GasboyDepartmentCollection();

            DataSet dataSet = this.Load(security, null, siteGuid, departmentName, departmentID, false);

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
        /// Gets a specific Gasboy department that has the specified ID for the specified site.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="siteGuid">Identifies the site to retrieve the records for</param>
        /// <param name="departmentName">The department name to load.</param>
        /// <returns>The Gasboy Station identified by the provided ID.</returns>
        public GasboyDepartment GetByID(SecurityClass security, Guid siteGuid, string departmentName)
        {
            if (siteGuid == Guid.Empty)
            {
                throw new Exception("SiteGuid parameter cannot be empty.");
            }

            if (string.IsNullOrEmpty(departmentName))
            {
                throw new ArgumentException("departmentName");
            }

            DataSet dataSet = this.Load(security, null, siteGuid, departmentName, null, true);

            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            var dataObject = this.GetDataObjectFromDataRow(dataSet.Tables[0].Rows[0]);

            return dataObject;
        }

        /// <summary>
        /// Gets the current <seealso cref="GasboyDepartment"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="identityGuid">Identifies the record to retrieve</param>
        /// <returns>The Gasboy Station identified by the provided identityGuid</returns>
        public GasboyDepartment Get(SecurityClass security, Guid identityGuid)
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
        /// Saves the passed in <seealso cref="GasboyDepartment"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The Gasboy Station business object to save.</param>
        public void Insert(SecurityClass security, GasboyDepartment dataObject)
        {
            using (SqlCommand cmd = this.PrepareInsertStatement())
            {
                this.BindCommonInsertUpdateParameterValues(cmd, dataObject);

                cmd.Parameters["@CreatedUpdatedBy"].Value = this.CreatedBy;

                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Saves the passed in <seealso cref="GasboyDepartment"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The Gasboy Station object to modify in the database</param>
        public void Update(SecurityClass security, GasboyDepartment dataObject)
        {
            using (SqlCommand cmd = this.PrepareUpdateStatement())
            {
                this.BindCommonInsertUpdateParameterValues(cmd, dataObject);

                cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;

                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Deletes the passed in <seealso cref="GasboyDepartment"/> record
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The Gasboy Station object to delete</param>
        public void Delete(SecurityClass security, GasboyDepartment dataObject)
        {
            using (var cmd = this.PrepareDeleteStatement())
            {
                cmd.Parameters["@IdentityGuid"].Value = dataObject.IdentityGuid;

                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

		#endregion Standard Data Access Methods

		#region Custom Data Access Methods

		#endregion Custom Data Access Methods

		#region Private Persistence Methods

		/// <summary>
		/// Gets the <seealso cref="GasboyDepartment"/> object(s) from the database identified by the provided parameters
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="identityGuid">Contains the identity guid of a specific record to load.</param>
		/// <param name="siteGuid">Contains the SiteGuid to restrict the results to.</param>
		/// <param name="departmentName">Contains the department name to load, must be combined with the SiteGuid.</param>
		/// <param name="departmentID">Contains the department id to load, must be combined with the SiteGuid.</param>
		/// <param name="exactIdMatch">If true, only departments that match the site guid AND exact id value will be returned</param>
		/// <returns>A dataset populated with matching gasboy departments or null if no matches exist.</returns>
		private DataSet Load(SecurityClass security, Guid? identityGuid, Guid? siteGuid, string departmentName, long? departmentID, bool exactIdMatch = true)
        {
            DataSet dataSet = null;

            if (identityGuid.HasValue || exactIdMatch)
            {
                using (var cmd = this.PrepareSelectStatement())
                {
                    if (exactIdMatch)
                    {
                        cmd.Parameters["@SiteGuid"].Value = this.SetOptionalValue<Guid>(siteGuid);
						cmd.Parameters["@DepartmentID"].Value = this.SetOptionalValue<long>(departmentID);
						cmd.Parameters["@DepartmentName"].Value = this.SetOptionalValue<string>(departmentName);
                    }
                    else
                    {
                        cmd.Parameters["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(identityGuid);
                    }

                    dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
                }
            }
            else
            {
                using (var cmd = this.PrepareEnumerateStatement())
                {
                    cmd.Parameters["@SiteGuid"].Value = this.SetOptionalValue<Guid>(siteGuid);
                    cmd.Parameters["@DepartmentName"].Value = this.SetOptionalValue<string>(departmentName);

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
                CommandText = "usp_GasboyDepartmentGet"
            };

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@DepartmentID", SqlDbType.BigInt);
			cmd.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, 50);

            return cmd;
        }

        protected override SqlCommand PrepareInsertStatement()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "usp_GasboyDepartmentInsert"
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
                CommandText = "usp_GasboyDepartmentUpdate"
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
                CommandText = "usp_GasboyDepartmentDelete"
            };

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

            return cmd;
        }

        #endregion Override Implementations for Prepare Methods

        #region Private Static Support Methods
        #endregion Private Static Support Methods

        #region Private Support Methods

        protected SqlCommand PrepareEnumerateStatement()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "usp_GasboyDepartmentEnumerate"
            };

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, 50);

            return cmd;
        }

        /// <summary>
        /// Add parameters that are used by both the insert and update stored procedures
        /// </summary>
        /// <param name="cmd">A SqlCommand to add parameters to</param>
        private void AddCommonInsertUpdateParameters(SqlCommand cmd)
        {
            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@DepartmentCode", SqlDbType.BigInt);
            cmd.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@GroupRuleName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@PriceListName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@LookupGasboyRecordStatusIndex", SqlDbType.Int);
            cmd.Parameters.Add("@UsePINCodeFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@PINCode", SqlDbType.VarBinary, 256);
            cmd.Parameters.Add("@AuthPINFrom", SqlDbType.TinyInt);
            cmd.Parameters.Add("@PromptForVehiclePlateFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@LookupGasboyVehiclePlateCheckTypeIndex", SqlDbType.Int);
            cmd.Parameters.Add("@AlwaysPromptForAdditionalValidationFlag", SqlDbType.Bit);
        }

        /// <summary>
        /// Bind parameter values that are used by both the insert and update stored procedures
        /// </summary>
        /// <param name="cmd">A SqlCommand to add parameters to</param>
        /// <param name="dataObject">The Gasboy Station that needs to be persisted.</param>
        private void BindCommonInsertUpdateParameterValues(SqlCommand cmd, GasboyDepartment dataObject)
        {
            cmd.Parameters["@IdentityGuid"].Value = dataObject.IdentityGuid;
            cmd.Parameters["@SiteGuid"].Value = dataObject.SiteGuid;
			cmd.Parameters["@DepartmentCode"].Value = dataObject.DepartmentCode;
            cmd.Parameters["@DepartmentName"].Value = dataObject.DepartmentName;
            cmd.Parameters["@GroupRuleName"].Value = dataObject.GroupRuleName;
            cmd.Parameters["@PriceListName"].Value = dataObject.PriceListName;
            cmd.Parameters["@LookupGasboyRecordStatusIndex"].Value = (int)dataObject.RecordStatus;
            cmd.Parameters["@UsePINCodeFlag"].Value = dataObject.UsePINCode;
            cmd.Parameters["@PINCode"].Value = !string.IsNullOrEmpty(dataObject.PINCode) ? UserClass.encode(dataObject.PINCode, dataObject.SiteGuid) : (object)DBNull.Value;
            cmd.Parameters["@AuthPINFrom"].Value = dataObject.AuthorizationPINSource;
            cmd.Parameters["@PromptForVehiclePlateFlag"].Value = dataObject.PromptForVehiclePlate;
            cmd.Parameters["@LookupGasboyVehiclePlateCheckTypeIndex"].Value = (int)dataObject.VehiclePlateCheckType;
            cmd.Parameters["@AlwaysPromptForAdditionalValidationFlag"].Value = dataObject.AlwaysPromptForAdditionalValidation;
        }

        /// <summary>
        /// The get data object from data row.
        /// </summary>
        /// <param name="row">
        /// The row containing the department record
        /// </param>
        /// <returns>
        /// The <see cref="GasboyDepartment"/>.
        /// </returns>
        private GasboyDepartment GetDataObjectFromDataRow(DataRow row)
        {
            var dataObject = new GasboyDepartment();

            dataObject.IdentityGuid = DataObject.getValue<Guid>(row["GasboyDepartmentGuid"], Guid.Empty);
            dataObject.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			dataObject.DepartmentID = DataObject.getValue<long?>(row["DepartmentID"], null);
			dataObject.DepartmentCode = DataObject.getValue<long?>(row["DepartmentCode"], null);
            dataObject.DepartmentName = DataObject.getValue<string>(row["DepartmentName"], string.Empty);
            dataObject.GroupRuleName = DataObject.getValue<string>(row["GroupRuleName"], string.Empty);
            dataObject.PriceListName = DataObject.getValue<string>(row["PriceListName"], string.Empty);

            int indexValue = DataObject.getValue<int>(row["LookupGasboyRecordStatusIndex"], (int)GasboyRecordStatus.Active);
            GasboyRecordStatus recordStatus = GasboyRecordStatus.Active;

            if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out recordStatus))
            {
                dataObject.RecordStatus = recordStatus;
            }

            dataObject.UsePINCode = DataObject.getValue<bool>(row["UsePINCodeFlag"], false);
            dataObject.PINCode = DataObject.GetEncryptedValue<string>(row["PINCode"], dataObject.SiteGuid, string.Empty);
            dataObject.PromptForVehiclePlate = DataObject.getValue<bool>(row["PromptForVehiclePlateFlag"], false);

            indexValue = DataObject.getValue<int>(row["LookupGasboyVehiclePlateCheckTypeIndex"], (int)GasboyVehiclePlateCheckType.ValidVehicleNoForCurrentDevice);
            GasboyVehiclePlateCheckType plateCheckType = GasboyVehiclePlateCheckType.ValidVehicleNoForCurrentDevice;

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
