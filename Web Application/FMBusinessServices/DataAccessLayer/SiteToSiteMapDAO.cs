namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;

	internal static class SiteToSiteMapDAO
	{
		internal static void SelectSQL( this SiteToSiteMapClass map, SqlCommand cmd, bool bInTransaction )
		{
			cmd.CommandText =
				"SELECT *, tblParent.ID AS ParentSiteID, tblChild.ID AS ChildSiteID, tblChild.SiteGroupFlag AS ChildGroup  "
				+ " FROM map.tblSiteToSite " + BaseDAO.SQLUpdateLock( bInTransaction ) + " JOIN dbo.tblSites AS tblParent"
				+ BaseDAO.SQLUpdateLock( bInTransaction ) + " ON tblParent.SiteGuid = map.tblSiteToSite.ParentSiteGuid"
				+ " JOIN dbo.tblSites AS tblChild" + BaseDAO.SQLUpdateLock( bInTransaction )
				+ " ON tblChild.SiteGuid = map.tblSiteToSite.ChildSiteGuid"
				+ " WHERE ParentSiteGuid = @ParentSiteGuid AND ChildSiteGuid = @ChildSiteGuid";

			cmd.Parameters.AddWithValue( "@ParentSiteGuid", map.ParentSiteGuid );
			cmd.Parameters.AddWithValue( "@ChildSiteGuid", map.ChildSiteGuid );
		}

		internal static void PurgeSQL( this SiteToSiteMapClass map, SqlCommand cmd )
		{
			cmd.CommandText =
				"DELETE FROM map.tblSiteToSite WHERE ParentSiteGuid = @ParentSiteGuid AND ChildSiteGuid = @ChildSiteGuid";

			cmd.Parameters.AddWithValue( "@ParentSiteGuid", map.ParentSiteGuid );
			cmd.Parameters.AddWithValue( "@ChildSiteGuid", map.ChildSiteGuid );
		}

		internal static void EnumerateByChildSiteSQL( this SiteToSiteMapClass map, SqlCommand cmd )
		{
			cmd.CommandText =
				"SELECT *, tblParent.ID AS ParentSiteID, tblChild.ID AS ChildSiteID, tblChild.SiteGroupFlag AS ChildGroup  "
				+ " FROM map.tblSiteToSite"
				+ " JOIN dbo.tblSites AS tblParent ON tblParent.SiteGuid = map.tblSiteToSite.ParentSiteGuid"
				+ " JOIN dbo.tblSites AS tblChild ON tblChild.SiteGuid = map.tblSiteToSite.ChildSiteGuid"
				+ " WHERE ChildSiteGuid = @ChildSiteGuid";

			cmd.Parameters.AddWithValue( "@ChildSiteGuid", map.ChildSiteGuid );
		}

		internal static void EnumerateByParentSiteSQL( this SiteToSiteMapClass map, SqlCommand cmd )
		{
			cmd.CommandText =
				"SELECT *, tblParent.ID AS ParentSiteID, tblChild.ID AS ChildSiteID, tblChild.SiteGroupFlag AS ChildGroup  "
				+ " FROM map.tblSiteToSite"
				+ " JOIN dbo.tblSites AS tblParent ON tblParent.SiteGuid = map.tblSiteToSite.ParentSiteGuid"
				+ " JOIN dbo.tblSites AS tblChild ON tblChild.SiteGuid = map.tblSiteToSite.ChildSiteGuid"
				+ " WHERE ParentSiteGuid = @ParentSiteGuid";

			cmd.Parameters.AddWithValue( "@ParentSiteGuid", map.ParentSiteGuid );
		}

		internal static void EnumerateSQL( this SiteToSiteMapClass map, SqlCommand cmd )
		{
			cmd.CommandText =
				"SELECT *, tblParent.ID AS ParentSiteID, tblChild.ID AS ChildSiteID, tblChild.SiteGroupFlag AS ChildGroup "
				+ " FROM map.tblSiteToSite"
				+ " JOIN dbo.tblSites AS tblParent ON tblParent.SiteGuid = map.tblSiteToSite.ParentSiteGuid"
				+ " JOIN dbo.tblSites AS tblChild ON tblChild.SiteGuid = map.tblSiteToSite.ChildSiteGuid";
		}

		internal static void InsertSQL( this SiteToSiteMapClass map, SqlCommand cmd )
		{
			cmd.CommandText = "INSERT INTO map.tblSiteToSite " + "(ParentSiteGuid," + "ChildSiteGuid," + "CreatedDate,"
							  + "CreatedBy" + ") VALUES (" + "@ParentSiteGuid," + "@ChildSiteGuid," + "@CreatedDate,"
							  + "@CreatedBy)";

			cmd.Parameters.AddWithValue( "@ParentSiteGuid", map.ParentSiteGuid );
			cmd.Parameters.AddWithValue( "@ChildSiteGuid", map.ChildSiteGuid );
			cmd.Parameters.AddWithValue( "@CreatedDate", map.CreatedDate );
			cmd.Parameters.AddWithValue( "@CreatedBy", map.CreatedBy );
		}

		internal static void LoadObject( this SiteToSiteMapClass map, DataSet set )
		{
			if ( set == null )
			{
				throw new ArgumentNullException( "set" );
			}

			map.Reset();

			DataTable table = set.Tables[0];
			if ( table.Rows.Count == 0 )
			{
				return;
			}

			DataRow row = table.Rows[0];

			map.LoadObject( row );
		}

		internal static void LoadObject( this SiteToSiteMapClass map, DataRow row )
		{
			map.ParentSiteGuid = DataObject.getValue( row["ParentSiteGuid"], Guid.Empty );
			map.ChildSiteGuid = DataObject.getValue( row["ChildSiteGuid"], Guid.Empty );
			map.CreatedDate = DataObject.getValue( row["CreatedDate"], DateTimeOffset.Now );
			map.CreatedBy = DataObject.getValue( row["CreatedBy"], BaseDataObject.ADMIN );
			map.ParentSiteID = DataObject.getValue( row["ParentSiteID"], "" );
			map.ChildSiteID = DataObject.getValue( row["ChildSiteID"], "" );
			map.ChildGroup = DataObject.getValue( row["ChildGroup"], false );
		}
	}
}