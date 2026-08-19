///***************************************************************************
/// Module Name:  BaseMapDAC
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data.SqlClient;
    using System.Data;

    using FMCore;

    /// <summary>
    /// Configuration info used by MapDAC
    /// </summary>
    public class MapDACConfig
	{
		private const string MapSuffix = "map.";

		public MapDACConfig(string newAssigneeName, string newAssignedName, string newAssignedIDColumnName)
		{
			AssignedName = newAssignedName;
			AssigneeName = newAssigneeName;
			AssignedIDColumnName = newAssignedIDColumnName;
			StoredProcSuffix = "usp_";
			GenerateNames();
		}

		public string MapName { get; private set; }

		public string AssignedName { get; private set; }
		public string AssigneeName { get; private set; }

		public string StoredProcSuffix { get; set; }
		public string StoredProcInsert { get; set; }
		public string StoredProcDelete { get; set; }
		public string StoredProcDeleteByAssignee { get; set; }
		public string StoredProcDeleteByAssigned { get; set; }
		public string StoredProcUpdate { get; set; }
		public string StoredProcSelect { get; set; }
		public string StoredProcSelectAssigned { get; set; }

		// variable names used in stored procedures
		public string MapGuidVariableName { get; set; }
		public string AssigneeGuidVariableName { get; set; }
		public string AssignedGuidVariableName { get; set; }

		// column names in the map table
		public string MapGuidColumnName { get; set; }
		public string AssigneeGuidColumnName { get; set; }
		public string AssignedGuidColumnName { get; set; }

		// column name in the AssignedTable
		public string AssignedIDColumnName { get; set; }

		public void GenerateNames()
		{
			CreateDefaultMapName();
			CreateDefaultStoredProcedureNames();
			CreateDefaultVariableNames();
		}

		public void CreateDefaultMapName()
		{
			MapName = AssignedName + "To" + AssigneeName;
		}

		public void CreateDefaultStoredProcedureNames()
		{
			string fullStoredProcSuffix = MapSuffix + StoredProcSuffix + MapName;
			StoredProcInsert = fullStoredProcSuffix + "InsertByRowGuid";
			StoredProcUpdate = fullStoredProcSuffix + "UpdateByRowGuid";
			StoredProcDelete = fullStoredProcSuffix + "DeleteByRowGuid";
			StoredProcSelect = fullStoredProcSuffix + "Select";
			StoredProcSelectAssigned = fullStoredProcSuffix + "Select" + AssignedName;
		}

		public void CreateDefaultVariableNames()
		{
			MapGuidColumnName = MapName + "Guid";
			MapGuidVariableName = "@" + MapGuidColumnName;
			AssigneeGuidColumnName = AssigneeName + "Guid";
			AssigneeGuidVariableName = "@" + AssigneeGuidColumnName;
			AssignedGuidColumnName = AssignedName + "Guid";
			AssignedGuidVariableName = "@" + AssignedGuidColumnName;
		}
	}

	/// <summary>
	/// This is the Data Access Helper class for a generic Map.
	/// A map maps an Assigned to Assignee.
	/// When we assign like a product to a site.  Product is the Assigned and Site is the Assignee
	/// This shouldn't really contain any data and all data should be stored in the DO class.
	/// </summary>
	public class BaseMapDAC
	{

		public BaseMapDAC(MapDACConfig info)
		{
			MyMapConfig = info;
		}

		public MapDACConfig MyMapConfig { get; private set; }

		public void Load(DataRow srcRow, BaseMapDO destObject)
		{
            srcRow.ThrowIfNull("srcRow");
            destObject.ThrowIfNull("destObject");

			AutoDistributionReasonCodeClass reasonCode = new AutoDistributionReasonCodeClass();
			destObject.IdentityGuid = DataObject.getValue<Guid>(srcRow[MyMapConfig.MapGuidColumnName], Guid.Empty);
			destObject.AssignedGuid = DataObject.getValue<Guid>(srcRow[MyMapConfig.AssignedGuidColumnName], Guid.Empty);
			destObject.AssigneeGuid = DataObject.getValue<Guid>(srcRow[MyMapConfig.AssigneeGuidColumnName], Guid.Empty);
		}

		/// <summary>
		/// This is used to load just the Assigned Entity's ID
		/// </summary>
		/// <typeparam name="EntityType"></typeparam>
		/// <param name="srcRow"></param>
		/// <param name="dataObject"></param>
		public void LoadAssignedInfo<EntityType>(DataRow srcRow, EntityType dataObject)
			where EntityType : BaseMapAssignedInfoDO
		{
            srcRow.ThrowIfNull("srcRow");
            dataObject.ThrowIfNull("dataObject");

			dataObject.ID = (string)srcRow[MyMapConfig.AssignedIDColumnName];
			dataObject.AssignedGuid = (Guid)srcRow[MyMapConfig.AssignedGuidColumnName];
		}

		#region Methods with SQLs
		/// <summary>
		/// Prepares a SqlCommand to select the record with the given Guid
		/// </summary>
		/// <param name="security"></param>
		/// <param name="bInTransaction"></param>
		/// <returns></returns>
		public void PrepareSelectByGuidSqlCommand(SqlCommand cmd, SecurityClass mySecurity, Guid mapGuid)
		{
            cmd.ThrowIfNull("cmd");
            mySecurity.ThrowIfNull("mySecurity");

            BaseMapDO theMap = new BaseMapDO();
			theMap.IdentityGuid = mapGuid;
			PrepareSelectSqlCommand(cmd, mySecurity, theMap);
		}

		/// <summary>
		/// Prepares a SqlCommand to select the record with the given Assignee
		/// </summary>
		/// <param name="security"></param>
		/// <param name="bInTransaction"></param>
		/// <returns></returns>
		public void PrepareSelectByAssigneeSqlCommand(SqlCommand cmd, SecurityClass mySecurity, Guid assigneeGuid)
		{
            cmd.ThrowIfNull("cmd");
            mySecurity.ThrowIfNull("mySecurity");

			BaseMapDO theMap = new BaseMapDO();
			theMap.AssigneeGuid = assigneeGuid;
			PrepareSelectSqlCommand(cmd, mySecurity, theMap);
		}
		/// <summary>
		/// Prepares a SqlCommand to select the record with the given Assigned
		/// </summary>
		/// <param name="security"></param>
		/// <param name="bInTransaction"></param>
		/// <returns></returns>
		public void PrepareSelectByAssignedSqlCommand(SqlCommand cmd, SecurityClass mySecurity, Guid assignedGuid)
		{
            cmd.ThrowIfNull("cmd");
            mySecurity.ThrowIfNull("mySecurity");

            BaseMapDO theMap = new BaseMapDO();
			theMap.AssignedGuid = assignedGuid;
			PrepareSelectSqlCommand(cmd, mySecurity, theMap);
		}

		/// <summary>
		/// Prepares a SqlCommand to select from the map table.
		/// Filtering criteria will be included if specified.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="bInTransaction"></param>
		/// <param name="byGuid">Select by Guid or by ReasonCode(ID)</param>
		/// <returns></returns>
		private void PrepareSelectSqlCommand(SqlCommand cmd, SecurityClass mySecurity, BaseMapDO theMap)
		{
            cmd.ThrowIfNull("cmd");
            mySecurity.ThrowIfNull("mySecurity");
            theMap.ThrowIfNull("theMap");

			cmd.CommandText = MyMapConfig.StoredProcSelect;
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Add(DataObject.NewGuidParameter("SelectedSiteGuid", mySecurity.SiteGuid, true));
			cmd.Parameters.Add(DataObject.NewGuidParameter("LoginSiteGuid", mySecurity.LoginSiteGuid, true));
			cmd.Parameters.Add(DataObject.NewGuidParameter(MyMapConfig.MapGuidColumnName, theMap.IdentityGuid, true));
			cmd.Parameters.Add(DataObject.NewGuidParameter(MyMapConfig.AssigneeGuidColumnName, theMap.AssigneeGuid, true));
			cmd.Parameters.Add(DataObject.NewGuidParameter(MyMapConfig.AssignedGuidColumnName, theMap.AssignedGuid, true));
		}

		/// <summary>
		/// Creates common parameters for Insert and Update operations
		/// </summary>
		/// <param name="cmd"></param>
		private void AddCommonParameters(SqlCommand cmd, BaseMapDO theMap)
		{
			cmd.Parameters.Add(DataObject.NewGuidParameter(MyMapConfig.AssigneeGuidColumnName, theMap.AssigneeGuid, true));
			cmd.Parameters.Add(DataObject.NewGuidParameter(MyMapConfig.AssignedGuidColumnName, theMap.AssignedGuid, true));
			cmd.Parameters.AddWithValue("@UpdatedDate", theMap.UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", theMap.UpdatedBy);
		}

		/// <summary>
		/// Prepares a SqlCommand for select all assigned for a given assignee
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public void PrepareSelectAssignedSqlCommand(SqlCommand cmd, Guid assigneeGuid)
		{
            cmd.ThrowIfNull("cmd");

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = MyMapConfig.StoredProcSelectAssigned;

			cmd.Parameters.AddWithValue(MyMapConfig.AssigneeGuidColumnName, assigneeGuid);
		}

		/// <summary>
		/// Prepares a SqlCommand for upate
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public void PrepareUpdateSqlCommand(SqlCommand cmd, BaseMapDO theMap)
		{
            cmd.ThrowIfNull("cmd");
            theMap.ThrowIfNull("theMap");

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = MyMapConfig.StoredProcUpdate;

			AddCommonParameters(cmd, theMap);
			cmd.Parameters.AddWithValue(MyMapConfig.MapGuidColumnName, theMap.IdentityGuid);
		}

		/// <summary>
		/// Prepares a SqlCommand for Insert
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public void PrepareInsertSqlCommand(SqlCommand cmd, BaseMapDO theMap)
		{
            cmd.ThrowIfNull("cmd");
            theMap.ThrowIfNull("theMap");

            cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = MyMapConfig.StoredProcInsert;

			AddCommonParameters(cmd, theMap);
			cmd.Parameters.AddWithValue("@CreatedDate", theMap.UpdatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", theMap.UpdatedBy);
			DataObject.AddGuidOutputParameter(cmd, MyMapConfig.MapGuidColumnName);
		}

		/// <summary>
		/// Prepares a SqlCommand for Delete
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public void PrepareDeleteSqlCommand(SqlCommand cmd, Guid mapGuid)
		{
            cmd.ThrowIfNull("cmd");

            cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = MyMapConfig.StoredProcDelete;

			cmd.Parameters.AddWithValue(MyMapConfig.MapGuidColumnName, mapGuid);
		}
		#endregion
	}
}
