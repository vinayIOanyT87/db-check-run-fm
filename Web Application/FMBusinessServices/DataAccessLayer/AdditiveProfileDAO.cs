namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;

	internal static class AdditiveProfileDAO
	{
		internal static void LoadProfile( this AdditiveProfileClass profile, DataSet set )
		{
			if (set == null)
			{
				throw new ArgumentNullException( "set" );
			}

			profile.Reset();

			DataTable table = set.Tables[0];
			if ( table.Rows.Count == 0 )
			{
				profile.SiteGuid = Guid.Empty;
				return;
			}

			DataRow row = table.Rows[0];

			profile.IdentityGuid = DataObject.getValue( row["AdditiveProfileGuid"], Guid.Empty );
			profile.SiteGuid = DataObject.getValue( row["SiteGuid"], Guid.Empty );
			profile.ID = DataObject.getValue( row["ID"], "" );
			profile.Description = DataObject.getValue( row["Description"], "" );
			profile.CreatedDate = DataObject.getValue( row["CreatedDate"], DateTimeOffset.Now );
			profile.CreatedBy = DataObject.getValue( row["CreatedBy"], BaseDataObject.ADMIN );
			profile.UpdatedDate = DataObject.getValue( row["UpdatedDate"], profile.CreatedDate );
			profile.UpdatedBy = DataObject.getValue( row["UpdatedBy"], BaseDataObject.ADMIN );
		}

		internal static void InsertSQL( this AdditiveProfileClass profile, SqlCommand cmd )
		{
			cmd.CommandText = "INSERT INTO tblAdditiveProfiles " +
				"(SiteGuid," +
				"ID," +
				"Description," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"AdditiveProfileGuid" +
				") VALUES (@SiteGuid, @ID, @Description, @CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy, @AdditiveProfileGuid)";

			cmd.Parameters.Add( "@SiteGuid", SqlDbType.UniqueIdentifier );
			cmd.Parameters.Add( "@ID", SqlDbType.NVarChar, 30 );
			cmd.Parameters.Add( "@Description", SqlDbType.NVarChar, 50 );
			cmd.Parameters.Add( "@CreatedDate", SqlDbType.DateTimeOffset );
			cmd.Parameters.Add( "@CreatedBy", SqlDbType.NVarChar, 100 );
			cmd.Parameters.Add( "@UpdatedDate", SqlDbType.DateTimeOffset );
			cmd.Parameters.Add( "@UpdatedBy", SqlDbType.NVarChar, 100 );
			cmd.Parameters.Add( "@AdditiveProfileGuid", SqlDbType.UniqueIdentifier );

			cmd.Parameters["@SiteGuid"].Value = profile.SiteGuid;
			cmd.Parameters["@ID"].Value = profile.ID;

			if ( !string.IsNullOrEmpty( profile.Description ) )
			{
				cmd.Parameters["@Description"].Value = profile.Description;
			}
			else
			{
				cmd.Parameters["@Description"].Value = DBNull.Value;
			}

			cmd.Parameters["@CreatedDate"].Value = profile.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = profile.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = profile.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = profile.UpdatedBy;
			cmd.Parameters["@AdditiveProfileGuid"].Value = profile.IdentityGuid;
		}

		internal static void UpdateSQL( this AdditiveProfileClass profile, SqlCommand cmd )
		{

			cmd.CommandText = "UPDATE tblAdditiveProfiles " +
				"SET ID = @ID," +
				"SiteGuid = @SiteGuid," +
				"Description = @Description," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy" +
				" WHERE AdditiveProfileGuid = @AdditiveProfileGuid";

			cmd.Parameters.Add( "@SiteGuid", SqlDbType.UniqueIdentifier );
			cmd.Parameters.Add( "@ID", SqlDbType.NVarChar, 30 );
			cmd.Parameters.Add( "@Description", SqlDbType.NVarChar, 50 );
			cmd.Parameters.Add( "@UpdatedDate", SqlDbType.DateTimeOffset );
			cmd.Parameters.Add( "@UpdatedBy", SqlDbType.NVarChar, 100 );
			cmd.Parameters.Add( "@AdditiveProfileGuid", SqlDbType.UniqueIdentifier );

			cmd.Parameters["@SiteGuid"].Value = profile.SiteGuid;
			cmd.Parameters["@ID"].Value = profile.ID;

			if ( !string.IsNullOrEmpty( profile.Description ) )
			{
				cmd.Parameters["@Description"].Value = profile.Description;
			}
			else
			{
				cmd.Parameters["@Description"].Value = DBNull.Value;
			}

			cmd.Parameters["@UpdatedDate"].Value = profile.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = profile.UpdatedBy;
			cmd.Parameters["@AdditiveProfileGuid"].Value = profile.IdentityGuid;
		}

		internal static void PurgeSQL( this AdditiveProfileClass profile, SqlCommand cmd )
		{
			cmd.CommandText = "DELETE FROM tblAdditiveProfiles" +
				" WHERE AdditiveProfileGuid = @AdditiveProfileGuid";

			cmd.Parameters.Add( "@AdditiveProfileGuid", SqlDbType.UniqueIdentifier );
			cmd.Parameters["@AdditiveProfileGuid"].Value = profile.IdentityGuid;
		}

		internal static void SelectSQL( this AdditiveProfileClass profile, SqlCommand cmd, bool bInTransaction )
		{
			cmd.CommandText = "SELECT *" +
				" FROM tblAdditiveProfiles " + BaseDAO.SQLUpdateLock( bInTransaction ) +
				" WHERE AdditiveProfileGuid = @AdditiveProfileGuid";

			cmd.Parameters.Add( "@AdditiveProfileGuid", SqlDbType.UniqueIdentifier );
			cmd.Parameters["@AdditiveProfileGuid"].Value = profile.IdentityGuid;
		}

		internal static void SelectByIdSql( this AdditiveProfileClass profile, SqlCommand cmd, SecurityClass security, bool bInTransaction )
		{
			cmd.CommandText = "SELECT *" +
				" FROM tblAdditiveProfiles " + BaseDAO.SQLUpdateLock( bInTransaction ) +
                " WHERE" + profile.AppendSiteWhereClause(cmd, security, "tblAdditiveProfiles", "AdditiveProfileGuid") +
                " AND ID = @ID";

			cmd.Parameters.Add( "@ID", SqlDbType.NVarChar, 30 );
			cmd.Parameters["@ID"].Value = profile.ID;
		}

		internal static void EnumerateSQL( this AdditiveProfileClass profile, SqlCommand cmd, SecurityClass security )
		{
			cmd.CommandText = "SELECT *" +
				" FROM tblAdditiveProfiles" +
                " WHERE" + profile.AppendSiteWhereClause(cmd, security, "tblAdditiveProfiles", "AdditiveProfileGuid") +
                " ORDER BY ID";
		}

		internal static void EnumerateAdditiveProfilesAllSitesSql(SqlCommand cmd)
		{
			cmd.CommandText =
				"SELECT AP.ID, AP.AdditiveProfileGuid, EAPTS.SiteGuid"
				+ " FROM tblAdditiveProfiles AP WITH(NOLOCK)"
				+ " LEFT JOIN map.tblEntityAdditiveProfileToSite EAPTS ON EAPTS.AdditiveProfileGuid = AP.AdditiveProfileGuid AND EAPTS.AssignedFromSiteGuid = AP.SiteGuid";
		}
	}
}
