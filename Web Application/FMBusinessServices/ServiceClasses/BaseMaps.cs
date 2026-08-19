// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseMaps.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;

    using FMCore;

	/// <summary>
	/// This is the business class for the BaseMap.
	///     It uses MaPDAC to prepare sql statements.
	///     Map data is stored in MapDO class
	/// </summary>
	/// <typeparam name="MapDOType">
	/// </typeparam>
	/// <typeparam name="MapDOCollectionType">
	/// </typeparam>
	internal class BaseMaps<MapDOType, MapDOCollectionType>
		where MapDOType : BaseMapDO, new() where MapDOCollectionType : BaseMapDOCollection<MapDOType>, new()
	{
		// error messages
		#region Constants and Fields

		private const string MessageInvalidEntity = "Invalid Map";

		private const string MessageInvalidEntityGuid = "Invalid Guid";

		private const string MessageInvalidEntityList = "Invalid Map List";

		private const string MessageInvalidSecurity = "Invalid Security";

		// DAL object
		private readonly ConsolidatedDAClass consolidatedDA;

		private readonly BaseMapDAC dataAccessHelper;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseMaps{MapDOType,MapDOCollectionType}"/> class. 
		/// Default constructor
		/// </summary>
		/// <param name="dacHelper">
		/// The dac Helper.
		/// </param>
		public BaseMaps(BaseMapDAC dacHelper)
		{
            dacHelper.ThrowIfNull("dacHelper");

			this.consolidatedDA = new ConsolidatedDAClass();
			this.dataAccessHelper = dacHelper;
		}

		#endregion

		#region Enums

		private enum EnumerateByTypes
		{
			MapGuid, 

			AssigneeGuid, 

			AssignedGuid
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds the given Map
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="theMap">
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		internal Guid Add(SecurityClass security, MapDOType theMap)
		{
			this.Validate(security, theMap);

			theMap.CreatedDate = DateTimeOffset.Now;
			theMap.CreatedBy = security.UserID;
			theMap.UpdatedDate = theMap.CreatedDate;
			theMap.UpdatedBy = theMap.CreatedBy;

			using (var cmd = new SqlCommand())
			{
				this.dataAccessHelper.PrepareInsertSqlCommand(cmd, theMap);
				this.consolidatedDA.ExecuteQuery(security, cmd);
				theMap.IdentityGuid = (Guid)cmd.Parameters[this.dataAccessHelper.MyMapConfig.MapGuidColumnName].Value;
			}

			return theMap.IdentityGuid;
		}

		/// <summary>
		/// Add the given map list
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="mapList">
		/// The map List.
		/// </param>
		internal void AddList(SecurityClass security, IEnumerable mapList)
		{
			foreach (object currentObject in mapList)
			{
				var currentMap = currentObject as MapDOType;
				this.Validate(currentMap);
				this.Add(security, currentMap);
			}
		}

		/// <summary>
		/// Selects all assigned based on the given assignee
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="assigneeGuid">
		/// The assignee Guid.
		/// </param>
		/// <returns>
		/// A collection of entity types
		/// </returns>
		internal List<EntityType> EnumerateAssigned<EntityType>(SecurityClass security, Guid assigneeGuid, SqlCommand cmd)
			where EntityType : BaseMapAssignedInfoDO, new()
		{
			this.Validate(security, assigneeGuid);

			DataTable resultTable = this.consolidatedDA.GetDataTable(cmd, security);

			var assignedList = new List<EntityType>();
			if (resultTable != null)
			{
				foreach (DataRow currentRow in resultTable.Rows)
				{
					var newAssigned = new EntityType();
					this.dataAccessHelper.LoadAssignedInfo(currentRow, newAssigned);
					assignedList.Add(newAssigned);
				}
			}

			return assignedList;
		}

		/// <summary>
		/// Selects all assigned based on the given assignee
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="assigneeGuid">
		/// The assignee Guid.
		/// </param>
		/// <returns>
		/// A collection of entity types
		/// </returns>
		internal List<EntityType> EnumerateAssigned<EntityType>(SecurityClass security, Guid assigneeGuid)
			where EntityType : BaseMapAssignedInfoDO, new()
		{
			this.Validate(security, assigneeGuid);

			using (var cmd = new SqlCommand())
			{
				this.dataAccessHelper.PrepareSelectAssignedSqlCommand(cmd, assigneeGuid);
				DataTable resultTable = this.consolidatedDA.GetDataTable(cmd, security);

				var assignedList = new List<EntityType>();
				if (resultTable != null)
				{
					foreach (DataRow currentRow in resultTable.Rows)
					{
						var newAssigned = new EntityType();
						this.dataAccessHelper.LoadAssignedInfo(currentRow, newAssigned);
						assignedList.Add(newAssigned);
					}
				}

				return assignedList;
			}
		}

		/// <summary>
		/// Selects all maps by Assigned
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="assignedGuid">
		/// The assigned Guid.
		/// </param>
		/// <returns>
		/// The <see cref="MapDOCollectionType"/>.
		/// </returns>
		internal MapDOCollectionType EnumerateByAssigned(SecurityClass security, Guid assignedGuid)
		{
			return this.Enumerate(security, assignedGuid, EnumerateByTypes.AssignedGuid);
		}

		/// <summary>
		/// Selects all maps by Assignee
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="assigneeGuid">
		/// The assignee Guid.
		/// </param>
		/// <returns>
		/// The <see cref="MapDOCollectionType"/>.
		/// </returns>
		internal MapDOCollectionType EnumerateByAssignee(SecurityClass security, Guid assigneeGuid)
		{
			return this.Enumerate(security, assigneeGuid, EnumerateByTypes.AssigneeGuid);
		}

		/// <summary>
		/// Load a MapDOType object by the given key
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="mapGuid">
		/// </param>
		/// <returns>
		/// The <see cref="MapDOType"/>.
		/// </returns>
		internal MapDOType Get(SecurityClass security, Guid mapGuid)
		{
			this.Validate(security);
			MapDOCollectionType mapList = this.Enumerate(security, mapGuid, EnumerateByTypes.MapGuid);

			if (mapList.Count > 0)
			{
				return mapList[0];
			}

			return null;
		}

		/// <summary>
		/// Updates the given Map
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="theMap">
		/// </param>
		internal void Modify(SecurityClass security, MapDOType theMap)
		{
			this.Validate(security, theMap);

			theMap.UpdatedDate = DateTimeOffset.Now;
			theMap.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				this.dataAccessHelper.PrepareUpdateSqlCommand(cmd, theMap);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Modify the given map list
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="deleteList">
		/// The delete List.
		/// </param>
		/// <param name="updateList">
		/// The update List.
		/// </param>
		/// <param name="insertList">
		/// The insert List.
		/// </param>
		internal void ModifyList(
			SecurityClass security, IEnumerable deleteList, IEnumerable updateList, IEnumerable insertList)
		{
			this.PurgeList(security, deleteList);
			foreach (object currentObject in updateList)
			{
				var currentMap = currentObject as MapDOType;
				this.Validate(currentMap);
				this.Modify(security, currentMap);
			}

			this.AddList(security, insertList);
		}

		/// <summary>
		/// Deletes the map with the given Guid
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="mapGuid">
		/// </param>
		internal void Purge(SecurityClass security, Guid mapGuid)
		{
			this.Validate(security, mapGuid);
			using (var cmd = new SqlCommand())
			{
				this.dataAccessHelper.PrepareDeleteSqlCommand(cmd, mapGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Delete the given map list
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="mapList">
		/// The map List.
		/// </param>
		internal void PurgeList(SecurityClass security, IEnumerable mapList)
		{
			foreach (object currentObject in mapList)
			{
				var currentMap = currentObject as MapDOType;
				if (currentMap != null)
				{
					this.Purge(security, currentMap.IdentityGuid);
				}
			}
		}

		/// <summary>
		/// Selects all maps based on the criteria
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="srcGuid">
		/// The src Guid.
		/// </param>
		/// <param name="enumerateBy">
		/// The enumerate By.
		/// </param>
		/// <returns>
		/// The <see cref="MapDOCollectionType"/>.
		/// </returns>
		private MapDOCollectionType Enumerate(SecurityClass security, Guid srcGuid, EnumerateByTypes enumerateBy)
		{
			Validate(security, srcGuid);
			using (var cmd = new SqlCommand())
			{
				switch (enumerateBy)
				{
					case EnumerateByTypes.MapGuid:
						this.dataAccessHelper.PrepareSelectByGuidSqlCommand(cmd, security, srcGuid);
						break;
					case EnumerateByTypes.AssignedGuid:
						this.dataAccessHelper.PrepareSelectByAssignedSqlCommand(cmd, security, srcGuid);
						break;
					case EnumerateByTypes.AssigneeGuid:
						this.dataAccessHelper.PrepareSelectByAssigneeSqlCommand(cmd, security, srcGuid);
						break;
				}

				DataTable resultTable = this.consolidatedDA.GetDataTable(cmd, security);

				var mapList = new MapDOCollectionType();
				if (resultTable != null)
				{
					foreach (DataRow currentRow in resultTable.Rows)
					{
						var newMap = new MapDOType();
						newMap.Load(this.dataAccessHelper, currentRow);
						mapList.Add(newMap);
					}
				}

				return mapList;
			}
		}

		/// <summary>
		/// Validates the given security
		///     customers for a given product.
		/// </summary>
		/// <param name="security">
		/// </param>
		private void Validate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentException(MessageInvalidSecurity);
			}
		}

		/// <summary>
		/// Validates the given security
		/// </summary>
		/// <param name="mapGuid">
		/// The map Guid.
		/// </param>
		private void Validate(Guid mapGuid)
		{
			if (mapGuid.IsEmpty())
			{
				throw new ArgumentException(MessageInvalidEntityGuid);
			}
		}

		/// <summary>
		/// Validates the given map
		/// </summary>
		/// <param name="theMap">
		/// The the Map.
		/// </param>
		private void Validate(MapDOType theMap)
		{
			if (theMap == null)
			{
				throw new ArgumentException(MessageInvalidEntity);
			}
		}

		/// <summary>
		/// Validates the given object
		/// </summary>
		/// <param name="mapList">
		/// The map List.
		/// </param>
		private void Validate(MapDOCollectionType mapList)
		{
			if (mapList == null)
			{
				throw new ArgumentException(MessageInvalidEntityList);
			}
		}

		/// <summary>
		/// Validates the given security and map
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="theMap">
		/// The the Map.
		/// </param>
		private void Validate(SecurityClass security, MapDOType theMap)
		{
			this.Validate(security);
			this.Validate(theMap);
		}

		/// <summary>
		/// Validates the given security and guid
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="srcGuid">
		/// The src Guid.
		/// </param>
		private void Validate(SecurityClass security, Guid srcGuid)
		{
			this.Validate(security);
			this.Validate(srcGuid);
		}

		#endregion
	}
}