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

	[ServiceBehavior]
	public class AssetTrackingDevices : IAssetTrackingDevices
	{
		#region
		private readonly ConsolidatedDAClass consolidatedDa;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetTrackingDevices()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// This method will enumerate asset tracking devices for entities
		/// and is used by the entity infrastructure.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a list of asset tracking devices.</returns>
		public List<AssetTrackingDeviceClass> Enumerate(SecurityClass security)
		{
			var assetTrackingDeviceList = new List<AssetTrackingDeviceClass>();

			//if (security == null)
			//{
			//	throw new ArgumentNullException("security");
			//}

			//if (this.ViewBasic(security) == false)
			//{
			//	throw new FMInsufficientRightsException();
			//}

			//using (var sqlCommand = new SqlCommand())
			//{
			//	var assetTrackingDevice = new AssetTrackingDeviceClass();
			//	assetTrackingDevice.EnumerateSql(sqlCommand, security);

			//	DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

			//	if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			//	{
			//		return assetTrackingDeviceList;
			//	}

			//	foreach (DataRow row in dataSet.Tables[0].Rows)
			//	{
			//		assetTrackingDevice = new AssetTrackingDeviceClass();
			//		assetTrackingDevice.Load(row);

			//		assetTrackingDeviceList.Add(assetTrackingDevice);
			//	}

				return assetTrackingDeviceList;
			//}
		}

		/// <summary>
		/// This method will return all active asset tracking devices.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns only active asset tracking devices.</returns>
		public List<AssetTrackingDeviceClass> EnumerateActiveDevices(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES))
			{
				throw new FMInsufficientRightsException();
			}

			return this.GetDevices(true, security);
		}

		/// <summary>
		/// This method will return all asset tracking devices.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns all asset tracking devices.</returns>
		public List<AssetTrackingDeviceClass> EnumerateAllDevices(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			return this.GetDevices(false, security);		
		}

		/// <summary>
		/// This method will return all asset tracking devices in data set object.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="filter">Filter on Device ID.</param>
		/// <returns>Returns all asset tracking devices in a data set.</returns>
		public DataSet EnumerateAllDeviceInDataSet(SecurityClass security, string filter)
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
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.EnumerateAllWithFilterSql(sqlCommand, filter, security);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				return dataSet;
			}
		}

		/// <summary>
		/// This method will return a list of asset tracking device that have not yet
		/// been assigned to a piece of equipment.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a list of asset tracking devices.</returns>
		public List<AssetTrackingDeviceClass> EnumerateAllUnassignedActiveDevices(SecurityClass security)
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
				var deviceList = new List<AssetTrackingDeviceClass>();
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.EnumerateAllUnassignedActiveDevicesSql(sqlCommand, security);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return deviceList;
				}

				foreach(DataRow row in dataSet.Tables[0].Rows)
				{
					assetTrackingDevice = new AssetTrackingDeviceClass();
					assetTrackingDevice.Load(row);

					deviceList.Add(assetTrackingDevice);
				}

				return deviceList;
			}
		}

		/// <summary>
		/// This method will return all asset tracking deivces that are
		/// associated with equipment.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns all asset tracking devices associated to equipment.</returns>
		public List<AssetTrackingDeviceClass> EnumerateAllDevicesLinkedToEquipment(SecurityClass security)
		{
			var assetTrackingDeviceList = new List<AssetTrackingDeviceClass>();

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
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.EnumerateOnlyDevicesLinkedToEquipmentSql(sqlCommand, security);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					assetTrackingDevice = new AssetTrackingDeviceClass();
					assetTrackingDevice.LoadWithEquipment(row);

					assetTrackingDeviceList.Add(assetTrackingDevice);
				}

				return assetTrackingDeviceList;
			}
		}

		/// <summary>
		/// This method will get a list of equipment that have not been associated
		/// to a device.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Return a data set containing the equipment.</returns>
		public DataSet EnumerateAllEquipmentNotAssociateToDevices(SecurityClass security)
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
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.EnumerateAllEquipmentNotAssociateToDevicesSql(sqlCommand, security.SiteGuid);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				return dataSet;
			}
		}

		/// <summary>
		/// This method will get a list of tanks that are associated 
		/// to a device.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDeviceGuid">The asset device GUID that the tanks are associated.</param>
		/// <returns>Returns a list of tank IDs.</returns>
		public List<string> EnumerateAssociatedTanks(SecurityClass security, Guid assetTrackingDeviceGuid)
		{
			var tankList = new List<string>();

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
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.EnumerateAssociatedTanksSql(sqlCommand, assetTrackingDeviceGuid);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					string tankId = row.IsNull("TankID") ? string.Empty : (string)row["TankID"];
					tankList.Add(tankId);
				}

				return tankList;
			}
		}

		/// <summary>
		/// This method will get a list of tanks that are associated 
		/// to a device.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a dataset of tanks that have associated devices.</returns>
		public DataSet EnumerateAllAssociatedTanks(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (this.ViewBasic(security) == false && this.ViewAddition(security) == false && security.HasRight(RIGHT.VIEW_MAPS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.EnumerateAllAssociatedTanksSql(sqlCommand, security.SiteGuid);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				return dataSet;
			}
		}

		/// <summary>
		/// This method will return a list of asset tracking devices that are not associated to
		/// a tank. It will return the current associated tank device.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public List<AssetTrackingDeviceClass> EnumerateAllSatelliteDevices(SecurityClass security)
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
				var devices = new List<AssetTrackingDeviceClass>();
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.EnumerateAllSatelliateDevicesSql(sqlCommand, security);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						assetTrackingDevice = new AssetTrackingDeviceClass();
						assetTrackingDevice.Load(row);
						devices.Add(assetTrackingDevice);
					}
				}

				return devices;
			}
		}

		/// <summary>
		/// This method will return the GUID for the asset tracking device ID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDeviceId">The asset tracking device ID to get the GUID.</param>
		/// <returns>Return the GUID or an empty GUID.</returns>
		public Guid GetIdentityGuid(SecurityClass security, string assetTrackingDeviceId)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(assetTrackingDeviceId))
			{
				throw new ArgumentNullException("assetTrackingDeviceId");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand sqlCommand = new SqlCommand())
			{
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.GetIdentityGuidSql(sqlCommand, assetTrackingDeviceId, security);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return Guid.Empty;
				}

				DataRow row = dataSet.Tables[0].Rows[0];
				Guid deviceGuid = row.IsNull("AssetTrackingDeviceGuid") ? Guid.Empty : (Guid)row["AssetTrackingDeviceGuid"];

				return deviceGuid;
			}
		}

		/// <summary>
		/// This method will return the GUID for the asset tracking device ID without the site criterion.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDeviceId">The asset tracking device ID to get the GUID.</param>
		/// <returns>Return the GUID or an empty GUID.</returns>
		public Guid GetIdentityGuidWithoutSite(SecurityClass security, string assetTrackingDeviceId)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(assetTrackingDeviceId))
			{
				throw new ArgumentNullException("assetTrackingDeviceId");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand sqlCommand = new SqlCommand())
			{
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.GetIdentityGuidWithoutSiteSql(sqlCommand, assetTrackingDeviceId, security);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return Guid.Empty;
				}

				DataRow row = dataSet.Tables[0].Rows[0];
				Guid deviceGuid = row.IsNull("AssetTrackingDeviceGuid") ? Guid.Empty : (Guid) row["AssetTrackingDeviceGuid"];

				return deviceGuid;
			}
		}

		/// <summary>
		/// This method will return the asset tracking device object.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDeviceGuid">The asset tracking device object.</param>
		/// <returns>Return the asset tracking device object or null.</returns>
		public AssetTrackingDeviceClass GetByIdentityGuid(SecurityClass security, Guid assetTrackingDeviceGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDeviceGuid == Guid.Empty)
			{
				throw new ArgumentNullException("assetTrackingDeviceGuid");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand sqlCommand = new SqlCommand())
			{
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.GetByIdentityGuidSql(sqlCommand, assetTrackingDeviceGuid);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				DataRow row = dataSet.Tables[0].Rows[0];
				assetTrackingDevice.Load(row);

				return assetTrackingDevice;
			}
		}

		/// <summary>
		/// This method will return the associated equipment ID and product ID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDeviceGuid">The asset tracking device GUID.</param>
		/// <returns>Return the asset tracking device object with the equipment and product IDs.</returns>
		public AssetTrackingDeviceClass GetAssociatedEquipmentIdAndProduct(SecurityClass security, Guid assetTrackingDeviceGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDeviceGuid == Guid.Empty)
			{
				throw new ArgumentNullException("assetTrackingDeviceGuid");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand sqlCommand = new SqlCommand())
			{
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.GetAssociatedEquipmentIdAndProductSql(sqlCommand, assetTrackingDeviceGuid);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				DataRow row = dataSet.Tables[0].Rows[0];
				assetTrackingDevice.EquipmentId					= row.IsNull("EquipmentID") ? string.Empty : (string)row["EquipmentID"];
				assetTrackingDevice.ProductId					= row.IsNull("ProductID") ? string.Empty : (string)row["ProductID"];
				assetTrackingDevice.ProductDensity				= row.IsNull("StandardDensity") ? (double?)null : (double)row["StandardDensity"];
				assetTrackingDevice.ProductDielectricTolerance	= row.IsNull("DielectricTolerance") ? (double?)null : (double)row["DielectricTolerance"];
				assetTrackingDevice.EquipmentSiteGuid			= row.IsNull("EquipmentSiteGuid") ? Guid.Empty : (Guid) row["EquipmentSiteGuid"];

				return assetTrackingDevice;
			}
		}

		/// <summary>
		/// This method will retrieve the associated equipment's site GUID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDeviceGuid">The asset tracking device GUID.</param>
		/// <returns>Returns the associated equipment's site GUID.</returns>
		public Guid GetEquipmentSiteGuid(SecurityClass security, Guid assetTrackingDeviceGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDeviceGuid == Guid.Empty)
			{
				throw new ArgumentNullException("assetTrackingDeviceGuid");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand sqlCommand = new SqlCommand())
			{
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.GetEquipmentSiteGuidSql(sqlCommand, assetTrackingDeviceGuid);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return Guid.Empty;
				}

				DataRow row = dataSet.Tables[0].Rows[0];
				Guid equipmentSiteGuid = row.IsNull("EquipmentSiteGuid") ? Guid.Empty : (Guid) row["EquipmentSiteGuid"];

				return equipmentSiteGuid;
			}
		}

		/// <summary>
		/// This method will return the equipment GUID associated to the asset tracking device object.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDeviceGuid">The asset tracking device GUID.</param>
		/// <returns>Return the equipment GUID for the associated asset tracking device (Guid.Empty if not found).</returns>
		public Guid GetAssociatedEquipmentGuid(SecurityClass security, Guid assetTrackingDeviceGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDeviceGuid == Guid.Empty)
			{
				throw new ArgumentNullException("assetTrackingDeviceGuid");
			}

			if (this.ViewBasic(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand sqlCommand = new SqlCommand())
			{
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				assetTrackingDevice.GetAssociatedEquipmentGuidSql(sqlCommand, assetTrackingDeviceGuid);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return Guid.Empty;
				}

				DataRow row = dataSet.Tables[0].Rows[0];
				Guid equipmentGuid = row.IsNull("EquipmentGuid") ? Guid.Empty : (Guid)row["EquipmentGuid"];

				return equipmentGuid;
			}
		}

		/// <summary>
		/// This method will purge an asset tracking device based on the GUID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDeviceGuid">The GUID to the asset tracking device to purge.</param>
		public void Purge(SecurityClass security, Guid assetTrackingDeviceGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDeviceGuid == Guid.Empty)
			{
				throw new ArgumentNullException("assetTrackingDeviceGuid");
			}

			if (this.CanModify(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			AssetTrackingDeviceClass oldAssetTrackingDevice = this.GetByIdentityGuid(security, assetTrackingDeviceGuid);

			if (oldAssetTrackingDevice.AssetTrackingDeviceGuid.Equals(Guid.Empty))
			{
				throw (new Exception("Asset Tracking Device Not Found"));
			}

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = 
								entityToSiteMaps.EnumerateByTypeIDAndGuid(security, oldAssetTrackingDevice.EntityType, assetTrackingDeviceGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = oldAssetTrackingDevice.DeviceId;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingDevice = new AssetTrackingDeviceClass();
				Guid equipmentGuid = this.GetAssociatedEquipmentGuid(security, assetTrackingDeviceGuid);

				if (equipmentGuid != Guid.Empty)
				{
					assetTrackingDevice.AssetTrackingDeviceGuid = Guid.Empty;
					assetTrackingDevice.EquipmentGuid = equipmentGuid;
					assetTrackingDevice.UpdateEquipmentDeviceReference(sqlCommand, security);

					this.consolidatedDa.ExecuteQuery(security, sqlCommand);
				}

				// Remove the device reference in the tank table for this device.
				sqlCommand.Parameters.Clear();
				assetTrackingDevice.RemoveTankDeviceReference(sqlCommand, security, assetTrackingDeviceGuid);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);

				// Remove the device.
				sqlCommand.Parameters.Clear();
				assetTrackingDevice.PurgeSql(sqlCommand, assetTrackingDeviceGuid);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will insert a new asset tracking device record into the database.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDevice">The asset tracking device to save.</param>
		/// <returns>Returns the new GUID of the inserted record.</returns>
		public Guid Add(SecurityClass security, AssetTrackingDeviceClass assetTrackingDevice)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDevice == null)
			{
				throw new ArgumentNullException("assetTrackingDevice");
			}

			if (this.CanModify(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			if (this.GetIdentityGuid(security, assetTrackingDevice.DeviceId) != Guid.Empty)
			{
				throw (new Exception("Asset Tracking Device exists."));
			}

			using (var sqlCommand = new SqlCommand())
			{
				assetTrackingDevice.InsertSql(sqlCommand, security);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);

				// Only update the asset tracking device reference to equipment
				// if there was an association.
				if (string.IsNullOrEmpty(assetTrackingDevice.EquipmentId) == false
				    && assetTrackingDevice.EquipmentGuid != Guid.Empty)
				{
					sqlCommand.Parameters.Clear();
					assetTrackingDevice.UpdateEquipmentDeviceReference(sqlCommand, security);
					this.consolidatedDa.ExecuteQuery(security, sqlCommand);
				}

				// Create Entity to Site Map
				var entityToSiteMaps = new EntityToSiteMaps();
				var entityToSiteMap = new EntityToSiteMapClass(assetTrackingDevice);
				entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

				return assetTrackingDevice.AssetTrackingDeviceGuid;
			}
		}

		/// <summary>
		/// This method will update an existing asset tracking device record into the database.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDevice">The asset tracking device to save.</param>
		public void Modify(SecurityClass security, AssetTrackingDeviceClass assetTrackingDevice)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDevice == null)
			{
				throw new ArgumentNullException("assetTrackingDevice");
			}

			if (this.CanModify(security) == false)
			{
				throw new FMInsufficientRightsException();
			}

			Guid identityGuid = this.GetIdentityGuid(security, assetTrackingDevice.DeviceId);

			if (identityGuid != Guid.Empty && identityGuid != assetTrackingDevice.AssetTrackingDeviceGuid)
			{
				throw (new Exception("Asset Tracking Device exists."));
			}

			AssetTrackingDeviceClass oldAssetTrackingDevice = this.GetByIdentityGuid(security, assetTrackingDevice.AssetTrackingDeviceGuid);

			if (oldAssetTrackingDevice.AssetTrackingDeviceGuid == Guid.Empty)
			{
				throw (new Exception("Asset Tracking Device Not Found"));
			}

			using (var sqlCommand = new SqlCommand())
			{
				assetTrackingDevice.UpdateSql(sqlCommand, security);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);

				// Update or remove the device reference in the Equipment table.
				// If the equipement GUID is empty, then remove the reference.
				sqlCommand.Parameters.Clear();

				if (assetTrackingDevice.EquipmentGuid == Guid.Empty)
				{
					assetTrackingDevice.RemoveEquipmentDeviceReference(sqlCommand, security);
				}
				else
				{
					assetTrackingDevice.UpdateEquipmentDeviceReference(sqlCommand, security);
				}
				
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);
			}

			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(	security, 
																						assetTrackingDevice.EntityType, 
																						assetTrackingDevice.AssetTrackingDeviceGuid);

			if (assetTrackingDevice.SiteGuid != oldAssetTrackingDevice.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = assetTrackingDevice.DeviceId;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(assetTrackingDevice);
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
			}
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// This method will return true if the user has modify rights.
		/// </summary>
		/// <param name="security">The secruity object.</param>
		/// <returns>Returns true if editable.</returns>
		private bool CanModify(SecurityClass security)
		{
			return security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES);
		}

		/// <summary>
		/// This method will return true if the user has modify rights.
		/// </summary>
		/// <param name="security">The secruity object.</param>
		/// <returns>Returns true if editable.</returns>
		private bool ViewBasic(SecurityClass security)
		{
			bool canView = (security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES) || security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES));

			return canView;
		}

		/// <summary>
		/// This method will return true if the user has the rights.
		/// </summary>
		/// <param name="security">The secruity object.</param>
		/// <returns>Returns true if user has the rights.</returns>
		private bool ViewAddition(SecurityClass security)
		{
			bool canView = security.HasRight(RIGHT.MODIFY_MAP_CONFIGURATION) 
							|| security.HasRight(RIGHT.VIEW_MAP_CONFIGURATION)
							|| security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
							|| security.HasRight(RIGHT.VIEW_TICKETING_DATA)
							|| security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
							|| security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);

			return canView;
		}

		/// <summary>
		/// This method will retrieve the asset tracking devices from the database.
		/// </summary>
		/// <param name="getActive">If true, only gets active asset tracking devices. Otherwise, returns all devices.</param>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a list asset tracking devices.</returns>
		private List<AssetTrackingDeviceClass> GetDevices(bool getActive, SecurityClass security)
		{
			var assetTrackingDeviceList = new List<AssetTrackingDeviceClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingDevice = new AssetTrackingDeviceClass();

				if (getActive)
				{
					assetTrackingDevice.EnumerateActiveSql(sqlCommand, security);
				}
				else
				{
					// Only get the devices of type Standard or WRDCU.
					assetTrackingDevice.EnumerateAllSql(sqlCommand, security);
				}

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					assetTrackingDevice = new AssetTrackingDeviceClass();
					assetTrackingDevice.Load(row);

					assetTrackingDeviceList.Add(assetTrackingDevice);
				}

				return assetTrackingDeviceList;
			}
		}
		#endregion
	}
}