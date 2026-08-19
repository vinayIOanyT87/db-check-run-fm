namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;

	internal static class GeneralConfigAliasDAO
	{
		/// <summary>
		/// This method will accept a data set that contains the general configuration
		/// alias data retrieved from the database and load the object members with the data.
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="row"></param>
		/// <returns></returns>
		internal static void LoadGeneralConfigAliasSQL( this GeneralConfigAlias alias, System.Data.DataRow row )
		{
			alias.GeneralConfigurationAliasGuid = DataObject.getValue( row["GeneralConfigurationAliasGuid"], Guid.Empty );
			alias.GeneralConfigurationGuid = DataObject.getValue( row["GeneralConfigurationGuid"], Guid.Empty );
			alias.TransactionAliasGuid = DataObject.getValue( row["TransactionAliasGuid"], Guid.Empty );
			alias.AliasName = DataObject.getValue( row["AliasName"], "" );

			alias.CreatedDate = DataObject.getValue( row["CreatedDate"], DateTimeOffset.Now );
			alias.CreatedBy = DataObject.getValue( row["CreatedBy"], BaseDataObject.ADMIN );
			alias.UpdatedDate = DataObject.getValue( row["UpdatedDate"], alias.CreatedDate );
			alias.UpdatedBy = DataObject.getValue( row["UpdatedBy"], BaseDataObject.ADMIN );
		}

		/// <summary>
		/// This method will construct a SQL command that retrieves the assigned aliases for a 
		/// given site.
		/// </summary>
		/// <returns></returns>
		internal static void GetGeneralConfigAliasSQL( this GeneralConfigAlias alias, SqlCommand cmd, Guid generalConfigurationGuid )
		{
			const string Select = "SELECT gc.GeneralConfigurationAliasGuid, gc.GeneralConfigurationGuid, gc.TransactionAliasGuid, " +
			                       "gc.CreatedBy, gc.CreatedDate, gc.UpdatedBy, gc.UpdatedDate, ta.AliasName ";
			const string From = "FROM tblGeneralConfigurationAliases gc, tblTransactionAliases ta ";
			const string Where = "WHERE GeneralConfigurationGuid = @GeneralConfigurationGuid " +
			                      "AND gc.TransactionAliasGuid = ta.TransactionAliasGuid";

			cmd.CommandText = Select + From + Where;

			cmd.Parameters.AddWithValue( "@GeneralConfigurationGuid", generalConfigurationGuid );
		}

		/// <summary>
		/// This method will construct a SQL command that will allow an insert of the general configuration
		/// assigned aliases.
		/// </summary>
		/// <returns></returns>
		internal static void InsertGeneralConfigAliasSQL( this GeneralConfigAlias alias, SqlCommand cmd )
		{
			cmd.CommandText = "INSERT INTO tblGeneralConfigurationAliases (" +
						"GeneralConfigurationGuid, TransactionAliasGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)" +
						"VALUES (@GeneralConfigurationGuid, @TransactionAliasGuid, " +
									"@CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate )";

			cmd.Parameters.AddWithValue( "@GeneralConfigurationGuid", alias.GeneralConfigurationGuid );
			cmd.Parameters.AddWithValue( "@TransactionAliasGuid", alias.TransactionAliasGuid );
			cmd.Parameters.AddWithValue( "@CreatedDate", alias.CreatedDate );
			cmd.Parameters.AddWithValue( "@CreatedBy", alias.CreatedBy );
			cmd.Parameters.AddWithValue( "@UpdatedDate", alias.UpdatedDate );
			cmd.Parameters.AddWithValue( "@UpdatedBy", alias.UpdatedBy );
		}

		/// <summary>
		/// This method will return a SQL string that will allow an update of the general configuration
		/// assigned aliases.
		/// </summary>
		/// <returns></returns>
		internal static void UpdateGeneralConfigAliasSQL( this GeneralConfigAlias alias, SqlCommand cmd )
		{
			cmd.CommandText = "UPDATE tblGeneralConfigurationAliases SET TransactionAliasGuid = @TransactionAliasGuid, " +
												"UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate " +
									"WHERE GeneralConfigurationAliasGuid = @GeneralConfigurationAliasGuid " +
										"AND GeneralConfigurationGuid = @GeneralConfigurationGuid ";

			cmd.Parameters.AddWithValue( "@GeneralConfigurationGuid", alias.GeneralConfigurationGuid );
			cmd.Parameters.AddWithValue( "@GeneralConfigurationAliasGuid", alias.GeneralConfigurationAliasGuid );
			cmd.Parameters.AddWithValue( "@TransactionAliasGuid", alias.TransactionAliasGuid );
			cmd.Parameters.AddWithValue( "@UpdatedDate", alias.UpdatedDate );
			cmd.Parameters.AddWithValue( "@UpdatedBy", alias.UpdatedBy );
		}

		/// <summary>
		/// This method will return a SQL string that will allow a delete of the general configuration
		/// assigned alias.
		/// </summary>
		/// <returns></returns>
		internal static void DeleteGeneralConfigAliasSQL( this GeneralConfigAlias alias, SqlCommand cmd )
		{
			cmd.CommandText = "DELETE FROM tblGeneralConfigurationAliases " +
									"WHERE GeneralConfigurationAliasGuid = @GeneralConfigurationAliasGuid " +
										"AND GeneralConfigurationGuid = @GeneralConfigurationGuid";

			cmd.Parameters.AddWithValue( "@GeneralConfigurationGuid", alias.GeneralConfigurationGuid );
			cmd.Parameters.AddWithValue( "@GeneralConfigurationAliasGuid", alias.GeneralConfigurationAliasGuid );
		}
	}
}