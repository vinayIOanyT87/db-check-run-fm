// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCardLimit.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Defines maximum quantities allowed to be disbursed in a particular period of time.
// A Fuel Card Limit is associated with zero to many fuel cards. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines maximum quantities allowed to be disbursed in a particular period of time.
    /// A Fuel Card Limit is associated with zero to many fuel cards. 
    /// </summary>
    [Serializable]
    [DataContract]
    public class FuelCardLimit : BaseDataObject
    {
        /// <summary>
        /// Get the entity type, which is used for entity assignments
        /// </summary>
        public override ENTITY_TYPE EntityType
        {
            get { return ENTITY_TYPE.FUEL_CARD_LIMIT; }
        }

        /// <summary>
        /// Get the parent entity type
        /// </summary>
        public override ENTITY_TYPE ParentEntityType
        {
            get { return ENTITY_TYPE.NONE; }
        }

	    [EntityImportExportAttribute("FUELCARDLIMITID*", 50, "ID")]
	    public override string ID
	    {
		    get { return this._ID; }
			set { this._ID = value; }
	    }

	    /// <summary>
        /// The Line Items associated with this Fuel Card Limit
        /// </summary>
        [DataMember]
        public List<FuelCardLimitLineItem> LineItems = new List<FuelCardLimitLineItem>();

        /// <summary>
        /// The fuel cards associated with this fuel card limit
        /// </summary>
        [DataMember]
        public List<FuelCardClass> AssignedFuelCards = new List<FuelCardClass>();

        /// <summary>
        /// Return the values in the Fuel Card Limit to their original values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.LineItems = new List<FuelCardLimitLineItem>();
            this.AssignedFuelCards = new List<FuelCardClass>();
        }

        /// <summary>
        /// Read a Fuel Card Limit object from a DataSet
        /// </summary>
        /// <param name="set">A DataSet to read Fuel Card Limit  information from</param>
        /// <returns>True if loading the Fuel Card Limit from the data set was successful</returns>
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

			this.IdentityGuid = DataObject.getValue(row["FuelCardLimitGuid"], Guid.Empty);
			this.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
			this.ID = DataObject.getValue(row["ID"], string.Empty);

			if (table.Columns.Contains("CreatedDate"))
			{
				this.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			}

			if (table.Columns.Contains("CreatedBy"))
			{
				this.CreatedBy = DataObject.getValue(row["CreatedBy"], ADMIN);
			}

			if (table.Columns.Contains("UpdatedDate"))
			{
				this.UpdatedDate = DataObject.getValue(row["UpdatedDate"], this._CreatedDate);
			}

			if (table.Columns.Contains("UpdatedBy"))
			{
				this.UpdatedBy = DataObject.getValue(row["UpdatedBy"], ADMIN);
			}

			return true;
		}

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to insert a fuel card limit into the database 
        /// </summary>
        /// <param name="cmd">a SqlCommand object to populate</param>
        public void InsertSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitInsert";

            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = this.IdentityGuid;
            cmd.Parameters.Add("@CreatedUpdatedBy", SqlDbType.NVarChar, 100).Value = this.CreatedBy;

            this.AddCommonInsertUpdateParameters(cmd);
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to update a fuel card limit in the database
        /// </summary>
        /// <param name="cmd">A SqlCommand object to populate</param>
        public void UpdateSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitUpdate";

            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = this.IdentityGuid;
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = this.UpdatedBy;

            this.AddCommonInsertUpdateParameters(cmd);
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to delete a fuel card limit in the database
        /// </summary>
        /// <param name="cmd">a SqlCommand object to populate</param>
        public void PurgeSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitDelete";

            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = this.IdentityGuid;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to read a fuel card limit record from the database by its primary key
        /// </summary>
        /// <param name="cmd">A SqlCommand object to populate</param>
        public void SelectSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitGet";

            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = this.IdentityGuid;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to read a fuel card limit record from the database by its Id
        /// </summary>
        /// <param name="cmd">A SqlCommand object to populate</param>
        public void SelectByIdSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitGet";

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = this.SiteGuid;
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = this.ID;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to read all fuel card limits assigned to a site
        /// </summary>
        /// <param name="cmd">A SqlCommand object to populate</param>
        public void EnumerateSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitEnumerate";

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = this.SiteGuid;
        }

		/// <summary>
		/// This method will setup the SQL command to retrieve the fuel card limit based on a fuel card.
		/// </summary>
		/// <param name="sqlCommand">SQL command to populate.</param>
		/// <param name="siteGuid">The site GUID filter.</param>
		/// <param name="fuelCardGuid">The fuel card GUID filter.</param>
		public static void EnumerateFuelCardLimitMappingsByFuelCardGuidSql(SqlCommand sqlCommand, Guid siteGuid, Guid fuelCardGuid)
		{
			sqlCommand.CommandType = CommandType.StoredProcedure;
			sqlCommand.CommandText = "usp_EnumerateFuelCardLimitMappingsByFuelCardGuid";

			sqlCommand.Parameters.Add("@FuelCardGuid", SqlDbType.UniqueIdentifier).Value = fuelCardGuid;
			sqlCommand.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;
		}

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to read all fuel card limits assigned to a site,
        /// filtering the matches to only those that partially match the ID provided
        /// </summary>
        /// <param name="cmd">A SqlCommand object to populate</param>
        public void EnumerateAndFilterSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitEnumerate";

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = this.SiteGuid;
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = this.ID;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to read all fuel cards assigned to a fuel card limit
        /// </summary>
        /// <param name="cmd">A SqlCommand object to populate</param>
        /// <param name="siteGuid">The site the user is currently accessing to retrieve fuel card mappings for</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit to get fuel card mappings for</param>
        public static void EnumerateFuelCardMappingsSQL(SqlCommand cmd, Guid siteGuid, Guid fuelCardLimitGuid)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FuelCardLimitToFuelCardGet";

            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = fuelCardLimitGuid;
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to read a fuel card limit to fuel card mapping 
        /// </summary>
        /// <param name="cmd">A SqlCommand object to populate</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit to get the mapping for</param>
        /// <param name="fuelCardGuid">Identifies the fuel card to get the mapping for</param>
        public static void GetFuelCardMappingSQL(SqlCommand cmd, Guid fuelCardLimitGuid, Guid fuelCardGuid)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FuelCardLimitToFuelCardGet";

            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = fuelCardLimitGuid;
            cmd.Parameters.Add("@FuelCardGuid", SqlDbType.UniqueIdentifier).Value = fuelCardGuid;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to create a new mapping between a fuel card and a fuel card limit
        /// </summary>
        /// <param name="cmd">A SqlCommand object to populate</param>
        /// <param name="fuelCardLimitToFuelCardGuid">Identifies the limit to fuel card mapping</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit</param>
        /// <param name="fuelCardGuid">Identifies the fuel card </param>
        /// <param name="userID">Identifies the user creating the mapping</param>
        public static void AddFuelCardMappingSQL(SqlCommand cmd, Guid fuelCardLimitToFuelCardGuid, Guid fuelCardLimitGuid, Guid fuelCardGuid, string userID)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FuelCardLimitToFuelCardInsert";

            cmd.Parameters.Add("@FuelCardLimitToFuelCardGuid", SqlDbType.UniqueIdentifier).Value = fuelCardLimitToFuelCardGuid;
            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = fuelCardLimitGuid;
            cmd.Parameters.Add("@FuelCardGuid", SqlDbType.UniqueIdentifier).Value = fuelCardGuid;
            cmd.Parameters.Add("@CreatedUpdatedBy", SqlDbType.NVarChar, 100).Value = userID;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to delete a fuel card limit to fuel card mapping
        /// </summary>
        /// <param name="cmd">A SqlCommand object to populate</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit to fuel card mapping to delete</param>
        /// <param name="fuelCardGuid">Identifies the fuel card limit to fuel card mapping to delet</param>
        public static void DeleteFuelCardMappingSQL(SqlCommand cmd, Guid fuelCardLimitGuid, Guid fuelCardGuid)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FuelCardLimitToFuelCardDelete";

            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = fuelCardLimitGuid;
            cmd.Parameters.Add("@FuelCardGuid", SqlDbType.UniqueIdentifier).Value = fuelCardGuid;
        }

        /// <summary>
        /// Add parameters that are used by both the insert and update stored procedures
        /// </summary>
        /// <param name="cmd">A SqlCommand to add parameters to</param>
        private void AddCommonInsertUpdateParameters(SqlCommand cmd)
        {
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = this.ID;
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = this.SiteGuid;
        }

        public void EnumerateForMobileSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitEnumerateForMobile";

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = this.SiteGuid;
        }
    }
}
