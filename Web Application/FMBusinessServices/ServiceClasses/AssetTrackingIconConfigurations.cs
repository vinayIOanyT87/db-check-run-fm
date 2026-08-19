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
	public class AssetTrackingIconConfigurations : IAssetTrackingIconConfigurations
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDa;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetTrackingIconConfigurations()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will add an asset tracking icon configuration record to the database.
		/// </summary>
		/// <param name="security">The secuity object.</param>
		/// <param name="assetTrackingIconConfiguration">The asset tracking icon configuration record to save.</param>
		/// <returns>Returns the record GUID</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AssetTrackingIconConfigurationClass assetTrackingIconConfiguration)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingIconConfiguration == null)
			{
				throw new ArgumentNullException("assetTrackingIconConfiguration");
			}

			if (this.CanModify(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			if (this.GetIdentityGuid(security, assetTrackingIconConfiguration.IconConfigurationId) != Guid.Empty)
			{
				throw (new Exception("Asset Tracking Icon Configuration exists."));
			}

			using (var sqlCommand = new SqlCommand())
			{
				assetTrackingIconConfiguration.InsertSql(sqlCommand, security);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);

				return assetTrackingIconConfiguration.AssetTrackingIconConfigurationGuid;
			}
		}

		/// <summary>
		/// This method will update an asset tracking icon configuration record to the database.
		/// </summary>
		/// <param name="security">The secuity object.</param>
		/// <param name="assetTrackingIconConfiguration">The asset tracking icon configuration record to save.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AssetTrackingIconConfigurationClass assetTrackingIconConfiguration)
		{
			if (security.Equals(null))
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingIconConfiguration == null)
			{
				throw new ArgumentNullException("assetTrackingIconConfiguration");
			}

			if (this.CanModify(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			Guid identityGuid = this.GetIdentityGuid(security, assetTrackingIconConfiguration.IconConfigurationId);

			if (identityGuid != Guid.Empty && identityGuid != assetTrackingIconConfiguration.AssetTrackingIconConfigurationGuid)
			{
				throw (new Exception("Wrong Asset Tracking Icon Configuration."));
			}

			AssetTrackingIconConfigurationClass oldAssetTrackingIconConfig = this.Get(security, assetTrackingIconConfiguration.AssetTrackingIconConfigurationGuid);

			if (oldAssetTrackingIconConfig.AssetTrackingIconConfigurationGuid == Guid.Empty)
			{
				throw (new Exception("Asset Tracking Icon Configuration ID Not Found"));
			}

			using (var sqlCommand = new SqlCommand())
			{
				assetTrackingIconConfiguration.UpdateSql(sqlCommand, security);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will delete an asset tracking Icon configuration record from the database.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingIconConfigurationGuid">The GUID of the record to delete.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid assetTrackingIconConfigurationGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingIconConfigurationGuid == null)
			{
				throw new ArgumentNullException("assetTrackingIconConfigurationGuid");
			}

			if (this.CanModify(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			AssetTrackingIconConfigurationClass oldIconConfig = this.Get(security, assetTrackingIconConfigurationGuid);

			if (oldIconConfig.AssetTrackingIconConfigurationGuid.Equals(Guid.Empty))
			{
				throw (new Exception("Asset Tracking Icon Configuration Not Found"));
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingIconConfigurationClass = new AssetTrackingIconConfigurationClass();
				assetTrackingIconConfigurationClass.DeleteSql(sqlCommand, assetTrackingIconConfigurationGuid);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will return the GUID for the asset tracking map name.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="iconConfigurationId">The asset tracking icon configuration ID to get the GUID.</param>
		/// <returns>Return the GUID or an empty GUID.</returns>
		public Guid GetIdentityGuid(SecurityClass security, string iconConfigurationId)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(iconConfigurationId))
			{
				throw new ArgumentNullException("iconConfigurationId");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand sqlCommand = new SqlCommand())
			{
				var iconConfiguration = new AssetTrackingIconConfigurationClass();
				iconConfiguration.GetIdentityGuidSql(sqlCommand, iconConfigurationId, security);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return Guid.Empty;
				}

				DataRow row = dataSet.Tables[0].Rows[0];
				Guid mapConfigurationGuid = row.IsNull("AssetTrackingIconConfigurationGuid") ? Guid.Empty : (Guid)row["AssetTrackingIconConfigurationGuid"];

				return mapConfigurationGuid;
			}
		}

		/// <summary>
		/// This method will retrieve an asset tracking map configuration record from the database
		/// based on an asset tracking icon configuration GUID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingIconConfigurationGuid">The GUID of the record to retrieve.</param>
		/// <returns>Returns either an asset tracking icon configuration record if found, else returns null.</returns>
		public AssetTrackingIconConfigurationClass Get(SecurityClass security, Guid assetTrackingIconConfigurationGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingIconConfigurationGuid == null)
			{
				throw new ArgumentNullException("assetTrackingIconConfigurationGuid");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingIconConfiguration = new AssetTrackingIconConfigurationClass();
				assetTrackingIconConfiguration.GetSql(sqlCommand, assetTrackingIconConfigurationGuid);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				assetTrackingIconConfiguration.Load(dataSet.Tables[0].Rows[0]);
				return assetTrackingIconConfiguration;
			}
		}

		/// <summary>
		/// This method will retrieve all the asset tracking map configuration records.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a list of asset tracking icon configuration records.</returns>
		public List<AssetTrackingIconConfigurationClass> Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (this.ViewBasic(security) == false && this.ViewExpanded(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingIconConfiguration = new AssetTrackingIconConfigurationClass();
				assetTrackingIconConfiguration.EnumerateSql(sqlCommand, security);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				var assetTrackingIconConfigList = new List<AssetTrackingIconConfigurationClass>();

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					assetTrackingIconConfiguration = new AssetTrackingIconConfigurationClass();
					assetTrackingIconConfiguration.Load(row);

					assetTrackingIconConfigList.Add(assetTrackingIconConfiguration);
				}

				return assetTrackingIconConfigList;
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
			return security.HasRight(RIGHT.MODIFY_ICON_CONFIGURATION);
		}

		/// <summary>
		/// This method will return true if the user has view or modify permissions.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns true if the user has view or modify rights.</returns>
		private bool ViewBasic(SecurityClass security)
		{
			return security.HasRight(RIGHT.MODIFY_ICON_CONFIGURATION) || security.HasRight(RIGHT.VIEW_ICON_CONFIGURATION);
		}

		/// <summary>
		/// This method will return true if the user has expanded view or modify permissions.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns true if the user has expanded view or modify rights.</returns>
		private bool ViewExpanded(SecurityClass security)
		{
			bool canView = security.HasRight(RIGHT.VIEW_MAPS) 
						   || security.HasRight(RIGHT.VIEW_MAP_CONFIGURATION)
			               || security.HasRight(RIGHT.MODIFY_MAP_CONFIGURATION)
			               || security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			               || security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
						   || security.HasRight(RIGHT.VIEW_TICKETING_DATA)
						   || security.HasRight(RIGHT.MODIFY_TICKETING_DATA);

			return canView;
		}
		#endregion
	}
}