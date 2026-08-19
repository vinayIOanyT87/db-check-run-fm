namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AssetTrackingMapConfigurations : IAssetTrackingMapConfigurations
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetTrackingMapConfigurations()
		{		
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will add an asset tracking map configuration record to the database.
		/// </summary>
		/// <param name="security">The secuity object.</param>
		/// <param name="assetTrackingMapConfigurationClass">The asset tracking map configuration record to save.</param>
		/// <returns>Returns the record GUID</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AssetTrackingMapConfigurationClass assetTrackingMapConfigurationClass)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingMapConfigurationClass == null)
			{
				throw new ArgumentNullException("assetTrackingMapConfigurationClass");
			}

			if (this.CanModify(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			if (this.GetIdentityGuid(security, assetTrackingMapConfigurationClass.MapName) != Guid.Empty)
			{
				throw (new Exception("Asset Tracking Map Configuration exists."));
			}

			using (var sqlCommand = new SqlCommand())
			{
				assetTrackingMapConfigurationClass.InsertSql(sqlCommand, security);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);

				// Create Entity to Site Map
				var entityToSiteMaps = new EntityToSiteMaps();
				var entityToSiteMap = new EntityToSiteMapClass(assetTrackingMapConfigurationClass);
				entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

				return assetTrackingMapConfigurationClass.AssetTrackingMapConfigurationGuid;
			}
		}

		/// <summary>
		/// This method will update an asset tracking map configuration record to the database.
		/// </summary>
		/// <param name="security">The secuity object.</param>
		/// <param name="assetTrackingMapConfigurationClass">The asset tracking map configuration record to save.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AssetTrackingMapConfigurationClass assetTrackingMapConfigurationClass)
		{
			if (security.Equals(null))
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingMapConfigurationClass == null)
			{
				throw new ArgumentNullException("assetTrackingMapConfigurationClass");
			}

			if (this.CanModify(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			Guid identityGuid = this.GetIdentityGuid(security, assetTrackingMapConfigurationClass.MapName);

			if (identityGuid != Guid.Empty && identityGuid != assetTrackingMapConfigurationClass.AssetTrackingMapConfigurationGuid)
			{
				throw (new Exception("Asset Tracking Map Configuration exists."));
			}

			AssetTrackingMapConfigurationClass oldAssetTrackingMapConfig = this.Get(security, assetTrackingMapConfigurationClass.AssetTrackingMapConfigurationGuid);

			if (oldAssetTrackingMapConfig.AssetTrackingMapConfigurationGuid == Guid.Empty)
			{
				throw (new Exception("Asset Tracking Map Name Not Found"));
			}

			using (var sqlCommand = new SqlCommand())
			{
				assetTrackingMapConfigurationClass.UpdateSql(sqlCommand, security);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}

			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security,
																						assetTrackingMapConfigurationClass.EntityType,
																						assetTrackingMapConfigurationClass.AssetTrackingMapConfigurationGuid);

			if (assetTrackingMapConfigurationClass.SiteGuid != oldAssetTrackingMapConfig.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = assetTrackingMapConfigurationClass.MapName;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(assetTrackingMapConfigurationClass);
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
			}
		}

		/// <summary>
		/// This method will delete an asset tracking map configuration record from the database.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingMapConfigurationGuid">The GUID of the record to delete.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid assetTrackingMapConfigurationGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingMapConfigurationGuid == null)
			{
				throw new ArgumentNullException("assetTrackingMapConfigurationGuid");
			}

			if (this.CanModify(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			AssetTrackingMapConfigurationClass oldMapConfig = this.Get(security, assetTrackingMapConfigurationGuid);

			if (oldMapConfig.AssetTrackingMapConfigurationGuid.Equals(Guid.Empty))
			{
				throw (new Exception("Asset Tracking Map Configuration Not Found"));
			}

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection =
								entityToSiteMaps.EnumerateByTypeIDAndGuid(security, oldMapConfig.EntityType, assetTrackingMapConfigurationGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = oldMapConfig.MapName;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingMapConfigurationClass = new AssetTrackingMapConfigurationClass();
				assetTrackingMapConfigurationClass.DeleteSql(sqlCommand, assetTrackingMapConfigurationGuid);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will return the GUID for the asset tracking map name.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="mapName">The asset tracking map name to get the GUID.</param>
		/// <returns>Return the GUID or an empty GUID.</returns>
		public Guid GetIdentityGuid(SecurityClass security, string mapName)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(mapName))
			{
				throw new ArgumentNullException("mapName");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand sqlCommand = new SqlCommand())
			{
				var mapConfiguration = new AssetTrackingMapConfigurationClass();
				mapConfiguration.GetIdentityGuidSql(sqlCommand, mapName, security);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return Guid.Empty;
				}

				DataRow row = dataSet.Tables[0].Rows[0];
				Guid mapConfigurationGuid = row.IsNull("AssetTrackingMapConfigurationGuid") ? Guid.Empty : (Guid)row["AssetTrackingMapConfigurationGuid"];

				return mapConfigurationGuid;
			}
		}

		/// <summary>
		/// This method will retrieve an asset tracking map configuration record from the database
		/// based on an asset tracking map configuration GUID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingMapConfigurationGuid">The GUID of the record to retrieve.</param>
		/// <returns>Returns either an asset tracking map configuration record if found, else returns null.</returns>
		public AssetTrackingMapConfigurationClass Get(SecurityClass security, Guid assetTrackingMapConfigurationGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingMapConfigurationGuid == null)
			{
				throw new ArgumentNullException("assetTrackingMapConfigurationGuid");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingMapConfiguration = new AssetTrackingMapConfigurationClass();
				assetTrackingMapConfiguration.GetSql(sqlCommand, assetTrackingMapConfigurationGuid);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				assetTrackingMapConfiguration.Load(dataSet.Tables[0].Rows[0]);
				return assetTrackingMapConfiguration;
			}			
		}

		/// <summary>
		/// This method will retrieve an asset tracking map configuration record from the database
		/// based on an asset tracking map configuration map name.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="mapName">The map name of the record to retrieve.</param>
		/// <returns>Returns either an asset tracking map configuration record if found, else returns null.</returns>
		public AssetTrackingMapConfigurationClass GetByMapName(SecurityClass security, string mapName)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(mapName))
			{
				throw new ArgumentNullException("mapName");
			}

			if (this.ViewBasic(security) == false && security.HasRight(RIGHT.VIEW_MAPS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingMapConfiguration = new AssetTrackingMapConfigurationClass();
				assetTrackingMapConfiguration.GetByMapNameSql(sqlCommand, security, mapName);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				assetTrackingMapConfiguration.Load(dataSet.Tables[0].Rows[0]);
				return assetTrackingMapConfiguration;
			}
		}

		/// <summary>
		/// This method will retrieve all the asset tracking map configuration records.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a list of asset tracking map configuration records.</returns>
		public List<AssetTrackingMapConfigurationClass> Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (this.ViewBasic(security) == false && security.HasRight(RIGHT.VIEW_MAPS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingMapConfiguration = new AssetTrackingMapConfigurationClass();
				assetTrackingMapConfiguration.EnumerateSql(sqlCommand, security);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				var assetTrackingMapConfigList = new List<AssetTrackingMapConfigurationClass>();

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					assetTrackingMapConfiguration = new AssetTrackingMapConfigurationClass();
					assetTrackingMapConfiguration.Load(row);

					assetTrackingMapConfigList.Add(assetTrackingMapConfiguration);
				}

				return assetTrackingMapConfigList;
			}			
		}

		/// <summary>
		/// This method will retrieve all the asset tracking map configuration records.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="filter">The filtering text</param>
		/// <returns>Returns a list of asset tracking map configuration records.</returns>
		public List<AssetTrackingMapConfigurationClass> EnumerateByFilter(SecurityClass security, string filter)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingMapConfiguration = new AssetTrackingMapConfigurationClass();

				if (string.IsNullOrEmpty(filter))
				{
					assetTrackingMapConfiguration.EnumerateSql(sqlCommand, security);
				}
				else
				{
					assetTrackingMapConfiguration.EnumerateByFilterSql(sqlCommand, security, filter);
				}

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				var assetTrackingMapConfigList = new List<AssetTrackingMapConfigurationClass>();

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					assetTrackingMapConfiguration = new AssetTrackingMapConfigurationClass();
					assetTrackingMapConfiguration.Load(row);

					assetTrackingMapConfigList.Add(assetTrackingMapConfiguration);
				}

				return assetTrackingMapConfigList;
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will return true if the user had modify permission.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns true if editable.</returns>
		private bool CanModify(SecurityClass security)
		{
			return security.HasRight(RIGHT.MODIFY_MAP_CONFIGURATION);
		}

		/// <summary>
		/// This method will return true if the user has view or modify permissions.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns true if the user has view or modify rights.</returns>
		private bool ViewBasic(SecurityClass security)
		{
			return security.HasRight(RIGHT.MODIFY_MAP_CONFIGURATION) || security.HasRight(RIGHT.VIEW_MAP_CONFIGURATION);
		}
		#endregion
	}
}