// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMAETranslation.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Contains classes representing a type of translation that can be applied to legacy transactions imported through the 
// FMAE interface. 
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
	/// Defines the types of translations supported by the FMAE interface.
	/// If you add a value here, be sure to add the new translation class as a known type
	/// to the FMAETranslation class, and update the CreateTranslationObject method 
	/// defined in the FMAETranslation class. 
	/// </summary>
	public enum FMAETranslationType
	{
		Unknown = 0,
		Company = 1,
		Product = 2
	};

	/// <summary>
	/// Represents a type of translation that can be applied to legacy transactions imported through the 
	/// FMAE interface. All translations that can be applied should derive from this class.
	/// 
	/// The use of this base class simplifies the code in the service class methods and user interface by allowing 
	/// them to interact with just one class rather than one class for every type of translation.
	/// </summary>
	[DataContract]
	[Serializable]
	[KnownType(typeof(FMAECompanyTranslation))]
	[KnownType(typeof(FMAEProductTranslation))]
	public abstract class FMAETranslation : BaseDataObject
	{
		/// <summary>
		/// Identifies the entity the legacy record should be translated to. 
		/// </summary>
		[DataMember]
		public Guid EntityGuid { get; set; }

		/// <summary>
		/// The ID of the entity the legacy record should be translated to. 
		/// </summary>
		[DataMember]
		public string EntityID { get; set; }

		/// <summary>
		/// Specifies the type of translation this object represents,
		/// e.g. a company translation or a product translation
		/// </summary>
		[DataMember]
		public FMAETranslationType TranslationType { get; set; }

		/// <summary>
		/// Return the fields in the object to their default values
		/// </summary>
		public override void Reset()
		{
			base.Reset();

			this.EntityGuid = Guid.Empty;
			this.EntityID = string.Empty;
		}
		
		/// <summary>
		/// Use the specified type of translation to create the appropriate type of translation object
		/// </summary>
		/// <param name="translationType">The type of translation</param>
		/// <returns>A translation object of the type that corresponds to the type specified. This will throw 
		/// if the type is not recognized</returns>
		public static FMAETranslation CreateTranslationObject(FMAETranslationType translationType)
		{
			FMAETranslation fmaeTranslation = null;

			if (translationType == FMAETranslationType.Company)
			{
				fmaeTranslation = new FMAECompanyTranslation();
				fmaeTranslation.TranslationType = FMAETranslationType.Company;
			}
			else if (translationType == FMAETranslationType.Product)
			{
				fmaeTranslation = new FMAEProductTranslation();
				fmaeTranslation.TranslationType = FMAETranslationType.Product;
			}
			else
			{
				throw new Exception("Translation type is not defined");
			}

			return fmaeTranslation;
		}

		/// <summary>
		/// Populate an FMAE translation object with data from a DataSet
		/// </summary>
		/// <param name="set"></param>
		/// <returns>True if loading information from the DataSet was successful</returns>
		public abstract bool Load(DataSet set);

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to select a translation by its IdentityGuid
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
		public abstract void SelectSQL(SqlCommand cmd);

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to select a translation by its ID, 
		/// which is the FMAE value we want to translate
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
		public abstract void SelectByIDSQL(SqlCommand cmd);

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to enumerate all translations of a particular type
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
        /// <param name="searchFilter">if provided, match results with a legacy ID containing this string</param>
		public abstract void EnumerateSQL(SqlCommand cmd, string searchFilter);

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to insert a translation
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
		/// <returns>The name of the identity guid output parameter. The value of the parameter is generated by
		/// the stored procedure before inserting the record</returns>
		public abstract string InsertSQL(SqlCommand cmd);

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to update a translation
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
		public abstract void UpdateSQL(SqlCommand cmd);

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to delete a translation
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
		public abstract void PurgeSQL(SqlCommand cmd);

	    /// <summary>
	    /// Populate a SQLCommand object with the information necessary to import translations.
	    /// </summary>
	    /// <param name="cmd">A SQLCommand object to populate.</param>
	    /// <param name="security">Used to get the UserID.</param>
	    /// <param name="translations">Translations to Import. They'll get added to a table valued parameter.</param>
	    public abstract void ImportSql(SqlCommand cmd, SecurityClass security, List<FMAETranslation> translations);
	}

    /// <summary>
    /// Represents a translation of a company ID specified in the legacy aviation application
    /// to a company entity in FuelsManager
    /// </summary>
    [DataContract]
    [Serializable]
    public class FMAECompanyTranslation : FMAETranslation
    {
        /// <summary>
        /// Populate an FMAE company translation object with data from a DataSet
        /// </summary>
        /// <param name="set">Contains information usually retrieved from the database</param>
        /// <returns>True if loading information from the DataSet was successful</returns>
        public override bool Load(DataSet set)
        {
            if (set == null)
            {
                throw new ArgumentNullException("set");
            }

            Reset();

            DataTable Table = set.Tables[0];

            if (Table.Rows.Count == 0)
            {
                return false;
            }

            DataRow Row = Table.Rows[0];

            _IdentityGuid = DataObject.getValue<Guid>(Row["FMAECompanyIDMapGuid"], Guid.Empty);
            _ID = DataObject.getValue<string>(Row["FMAECompanyID"], string.Empty);
            EntityGuid = DataObject.getValue<Guid>(Row["CompanyGuid"], Guid.Empty);
            EntityID = DataObject.getValue<string>(Row["CompanyID"], string.Empty);
            _CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
            _CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
            _UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
            _UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);

            return true;
        }

        /// <summary>
        /// Populate a SQLCommand object with the information necessary to select a company translation by its IdentityGuid
        /// </summary>
        /// <param name="cmd">A SQLCommand object to populate</param>
        public override void SelectSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FMAECompanyIDSelect";

            cmd.Parameters.Add("@FMAECompanyIDMapGuid", SqlDbType.UniqueIdentifier).Value = _IdentityGuid;
        }

        /// <summary>
        /// Populate a SQLCommand object with the information necessary to select a company translation by its ID, 
        /// which is the FMAE value we want to translate
        /// </summary>
        /// <param name="cmd">A SQLCommand object to populate</param>
        public override void SelectByIDSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FMAECompanyIDSelect";

            cmd.Parameters.Add("@FMAECompanyID", SqlDbType.NVarChar, 100).Value = ID;
        }

        /// <summary>
        /// Populate a SQLCommand object with the information necessary to enumerate all company translations 
        /// </summary>
        /// <param name="cmd">A SQLCommand object to populate</param>
        /// <param name="searchFilter">If provided, results will be limited to translations with a legacy id that contain this value</param>
        public override void EnumerateSQL(SqlCommand cmd, string searchFilter)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FMAECompanyIDSelect";

            if (!string.IsNullOrWhiteSpace(searchFilter))
            {
                cmd.Parameters.Add("@FMAECompanyIDSearchFilter", SqlDbType.NVarChar, 25).Value = searchFilter;
            }
        }

        /// <summary>
        /// Populate a SQLCommand object with the information necessary to insert a company translation
        /// </summary>
        /// <param name="cmd">A SQLCommand object to populate</param>
        /// <returns>The name of the identity guid output parameter. The value of the parameter is generated by
        /// the stored procedure before inserting the record</returns>
        public override string InsertSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FMAECompanyIDInsert";

            string identityGuidParameterName = "@FMAECompanyIDMapGuid";

            SqlParameter identityGuidParam = new SqlParameter(identityGuidParameterName, SqlDbType.UniqueIdentifier);
            identityGuidParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(identityGuidParam);

            cmd.Parameters.Add("@FMAECompanyID", SqlDbType.NVarChar, 100).Value = _ID;
            cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier).Value = EntityGuid;
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100).Value = _CreatedBy;
            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset).Value = _CreatedDate;
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = _UpdatedBy;
            cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset).Value = _UpdatedDate;

            return identityGuidParameterName;
        }

        /// <summary>
        /// Populate a SQLCommand object with the information necessary to update a company translation
        /// </summary>
        /// <param name="cmd">A SQLCommand object to populate</param>
        public override void UpdateSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FMAECompanyIDUpdate";

            cmd.Parameters.Add("@FMAECompanyIDMapGuid", SqlDbType.UniqueIdentifier).Value = _IdentityGuid;
            cmd.Parameters.Add("@FMAECompanyID", SqlDbType.NVarChar, 100).Value = _ID;
            cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier).Value = EntityGuid;
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = _UpdatedBy;
            cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset).Value = _UpdatedDate;
        }

        /// <summary>
        /// Populate a SQLCommand object with the information necessary to delete a company translation
        /// </summary>
        /// <param name="cmd">A SQLCommand object to populate</param>
        public override void PurgeSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FMAECompanyIDDelete";

            cmd.Parameters.Add("@FMAECompanyIDMapGuid", SqlDbType.UniqueIdentifier).Value = _IdentityGuid;
        }

        /// <summary>
        /// Populate a SQLCommand object with the information necessary to import company translations.
        /// </summary>
        /// <param name="cmd">A SQLCommand object to populate.</param>
        /// <param name="security">Used to get the UserID.</param>
        /// <param name="translations">Translations to Import. They'll get added to a table valued parameter.</param>
        public override void ImportSql(SqlCommand cmd, SecurityClass security, List<FMAETranslation> translations)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FMAECompanyIDImport";

            // Add every translation record provided to a table that will be passed to the stored procedure
            DataTable parameterTable = new DataTable();
            parameterTable.Columns.Add("FMAECompanyID", typeof(string));
            parameterTable.Columns.Add("CompanyGuid", typeof(Guid));
            // The FMAECompanyIDMapGuid (Primary Key), Created date, and Updated date will be set by the stored procedure
            parameterTable.Columns.Add("UserID", typeof(string));

            foreach (FMAETranslation translation in translations)
            {
                parameterTable.Rows.Add(
                    translation.ID,
                    translation.EntityGuid,
                    security.UserID);
            }

            SqlParameter tableValuedParameter = cmd.Parameters.Add("@FMAETranslations", SqlDbType.Structured);
            tableValuedParameter.Value = parameterTable;
            tableValuedParameter.TypeName = "map.FMAECompanyIDType";
        }
    }

	/// <summary>
	/// Represents a translation of a product ID specified in the legacy aviation application
	/// to a product entity in FuelsManager
	/// </summary>
	[DataContract]
	[Serializable]
	public class FMAEProductTranslation : FMAETranslation
	{
		/// <summary>
		/// Populate an FMAE product translation object with data from a DataSet
		/// </summary>
		/// <param name="set">Contains information usually retrieved from the database</param>
		/// <returns>True if loading information from the DataSet was successful</returns>
		public override bool Load(DataSet set)
		{
			if (set == null)
			{
				throw new ArgumentNullException("set");
			}

			Reset();

			DataTable Table = set.Tables[0];

			if (Table.Rows.Count == 0)
			{
				return false;
			}

			DataRow Row = Table.Rows[0];

			_IdentityGuid = DataObject.getValue<Guid>(Row["FMAEProductIDMapGuid"], Guid.Empty);
			_ID = DataObject.getValue<string>(Row["FMAEProductID"], string.Empty);
			EntityGuid = DataObject.getValue<Guid>(Row["ProductGuid"], Guid.Empty);
			EntityID = DataObject.getValue<string>(Row["ProductID"], string.Empty);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);

			return true;
		}

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to select a product translation by its IdentityGuid
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
		public override void SelectSQL(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "map.usp_FMAEProductIDSelect";

			cmd.Parameters.Add("@FMAEProductIDMapGuid", SqlDbType.UniqueIdentifier).Value = _IdentityGuid;
		}

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to select a product translation by its ID, 
		/// which is the FMAE value we want to translate
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
		public override void SelectByIDSQL(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "map.usp_FMAEProductIDSelect";

			cmd.Parameters.Add("@FMAEProductID", SqlDbType.NVarChar, 30).Value = ID;
		}

	    /// <summary>
	    /// Populate a SQLCommand object with the information necessary to enumerate all defined product translations
	    /// </summary>
	    /// <param name="cmd">A SQLCommand object to populate</param>
	    /// <param name="searchFilter">If provided, results will be limited to translations with a legacy id that contain this value</param>
	    public override void EnumerateSQL(SqlCommand cmd, string searchFilter)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "map.usp_FMAEProductIDSelect";

            if (!string.IsNullOrWhiteSpace(searchFilter))
            {
                cmd.Parameters.Add("@FMAEProductIDSearchFilter", SqlDbType.NVarChar, 25).Value = searchFilter;
            }
		}

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to insert a product translation
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
		/// <returns>The name of the identity guid output parameter. The value of the parameter is generated by
		/// the stored procedure before inserting the record </returns>
		public override string InsertSQL(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "map.usp_FMAEProductIDInsert";

			string identityGuidParameterName = "@FMAEProductIDMapGuid";

			SqlParameter identityGuidParam = new SqlParameter(identityGuidParameterName, SqlDbType.UniqueIdentifier);
			identityGuidParam.Direction = ParameterDirection.Output;
			cmd.Parameters.Add(identityGuidParam);

			cmd.Parameters.Add("@FMAEProductID", SqlDbType.NVarChar, 30).Value = _ID;
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = EntityGuid;
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100).Value = _CreatedBy;
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset).Value = _CreatedDate;
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = _UpdatedBy;
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset).Value = _UpdatedDate;

			return identityGuidParameterName;
		}

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to update a product translation
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
		public override void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "map.usp_FMAEProductIDUpdate";

			cmd.Parameters.Add("@FMAEProductIDMapGuid", SqlDbType.UniqueIdentifier).Value = _IdentityGuid;
			cmd.Parameters.Add("@FMAEProductID", SqlDbType.NVarChar, 30).Value = _ID;
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = EntityGuid;
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = _UpdatedBy;
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset).Value = _UpdatedDate;
		}

		/// <summary>
		/// Populate a SQLCommand object with the information necessary to delete a product translation
		/// </summary>
		/// <param name="cmd">A SQLCommand object to populate</param>
		public override void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "map.usp_FMAEProductIDDelete";

			cmd.Parameters.Add("@FMAEProductIDMapGuid", SqlDbType.UniqueIdentifier).Value = _IdentityGuid;
		}

        /// <summary>
        /// Populate a SQLCommand object with the information necessary to import product translations
        /// </summary>
        /// <param name="cmd">A SQLCommand object to populate.</param>
        /// <param name="security">Used to get the UserID.</param>
        /// <param name="translations">Translations to Import. They'll get added to a table valued parameter</param>
        public override void ImportSql(SqlCommand cmd, SecurityClass security, List<FMAETranslation> translations)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_FMAEProductIDImport";

            // Add every translation record provided to a table that will be passed to the stored procedure
            DataTable parameterTable = new DataTable();
            parameterTable.Columns.Add("FMAEProductID", typeof(string));
            parameterTable.Columns.Add("ProductGuid", typeof(Guid));
            // The FMAEProductIDMapGuid (Primary Key), Created date, and Updated date will be set by the stored procedure
            parameterTable.Columns.Add("UserID", typeof(string));

            foreach (FMAETranslation translation in translations)
            {
                parameterTable.Rows.Add(
                    translation.ID,
                    translation.EntityGuid,
                    security.UserID);
            }

            SqlParameter tableValuedParameter = cmd.Parameters.Add("@FMAETranslations", SqlDbType.Structured);
            tableValuedParameter.Value = parameterTable;
            tableValuedParameter.TypeName = "map.FMAEProductIDType";
        }
	}
}
