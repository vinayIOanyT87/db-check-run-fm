// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyDevices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements operations to support database operations for Gasboy Devices
// like adding, modifying, or deleting a record.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.ServiceModel;
	using System.Text;
	using System.Xml;
	using System.Xml.Serialization;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.ServiceClasses;

	using FuelsManager.Afss.BusinessObjects.Constants;
	using FuelsManager.Afss.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.BusinessServices.Repository;

	/// <summary>
	/// Implements operations to support database operations for Gasboy Devices
	/// like adding, modifying, or deleting a record.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class GasboyDevices : IGasboyDevices
	{
		/// <summary>
		/// Allows database access.
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		#region Gasboy Device Methods

		/// <summary>
		/// Get stations configured for the site, filtering by the filter text if it was provided
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <returns>Stations configured for the site, filtered by the filter text if it was provided</returns>
		public List<GasboyDevice> Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			List<GasboyDevice> devices = null;

			using (var dbi = new GasboyDeviceDBI(security.UserID))
			{
				devices = dbi.GetList(security, security.SiteGuid, null, null);
			}

			return devices;
		}

		/// <summary>
		/// Gets GasboyDevices for an associated site, includes soft-deleted devices but filters out AirCards. 
		/// This is what should be used when enumerating Gasboy Devices to push to a pedestal so that orphaned devices can be purged from the unit
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <returns>GasboyDevices configured for the site</returns>
		public List<GasboyDevice> EnumerateWithDeleted(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			List<GasboyDevice> devices = null;

			using (var dbi = new GasboyDeviceDBI(security.UserID))
			{
				devices = dbi.GetList(security, security.SiteGuid, null, null, true, false);
			}

			return devices;
		}

		/// <summary>
		/// Get all Gasboy Devices assigned or owned by the current site that partially match the ID provided
		/// </summary>
		/// <param name="security">Contains security information, like the site we're currently accessing to retrieve Gasboy Devices for</param>
		/// <param name="searchFilter">The ID to search for matches on</param>
		/// <returns>All Gasboy Devices assigned or owned by the current site that partially match the provided ID</returns>
		public List<GasboyDevice> EnumerateAndFilter(SecurityClass security, string searchFilter)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			List<GasboyDevice> stations;

			using (var dbi = new GasboyDeviceDBI(security.UserID))
			{
				stations = dbi.GetList(security, security.SiteGuid, null, searchFilter);
			}

			return stations;
		}

		/// <summary>
		/// Retrieve the Gasboy Device identified by the provided guid
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDeviceGuid">Identifies the Gasboy Device to retrieve</param>
		/// <returns>The Gasboy Device identified by the provided guid</returns>
		public GasboyDevice Get(SecurityClass security, Guid gasboyDeviceGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (gasboyDeviceGuid == Guid.Empty)
			{
				throw new ArgumentException("gasboyDeviceGuid");
			}

			GasboyDevice station;

			using (var dbi = new GasboyDeviceDBI(security.UserID))
			{
				station = dbi.Get(security, gasboyDeviceGuid);
			}

			return station;
		}

		/// <summary>
		/// Retrieve the Gasboy Device identified by the provided guid
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDepartmentGuid">Identifies the Gasboy department to retrieve</param>
		/// <returns>The Gasboy Device identified by the provided guid</returns>
		public List<GasboyDevice> GetByDepartment(SecurityClass security, Guid siteGuid, Guid gasboyDepartmentGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (gasboyDepartmentGuid == Guid.Empty)
			{
				throw new ArgumentException("gasboyDepartmentGuid");
			}

			List<GasboyDevice> devices;


			using (var dbi = new GasboyDeviceDBI(security.UserID))
			{
				devices = dbi.GetByDepartment(security,siteGuid, gasboyDepartmentGuid);
			}

			return devices;
		}

		/// <summary>
		/// Retrieve the Gasboy Device identified by the provided id
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDeviceID">Identifies the Gasboy Device to retrieve</param>
		/// <param name="gasboyDeviceName">Identifies the Gasboy Device to retrieve</param>
		/// <returns>The Gasboy Device identified by the provided id</returns>
		public GasboyDevice GetByName(SecurityClass security, long? gasboyDeviceID, string gasboyDeviceName)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (string.IsNullOrEmpty(gasboyDeviceName))
			{
				throw new ArgumentException("gasboyDeviceName");
			}

			GasboyDevice station;

			using (var dbi = new GasboyDeviceDBI(security.UserID))
			{
				station = dbi.GetByID(security, security.SiteGuid, gasboyDeviceID, gasboyDeviceName);
			}

			return station;
		}

		/// <summary>
		/// Retrieve the Gasboy Device identified by the specified Card Number
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="siteGuid">Identifies the site to retrieve the device from</param>
		/// <param name="gasboyCardNumber">Card Number of the Gasboy Device to retreive</param>
		/// <returns>The Gasboy Device identified by the provided id</returns>
		public GasboyDevice GetByCardNumber(SecurityClass security, Guid siteGuid, string gasboyCardNumber)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (string.IsNullOrEmpty(gasboyCardNumber))
			{
				throw new ArgumentException("gasboyCardNumber");
			}

			GasboyDevice station;

			using (var dbi = new GasboyDeviceDBI(security.UserID))
			{
				station = dbi.GetByCardNumber(security, siteGuid, gasboyCardNumber);
			}

			return station;
		}

		/// <summary>
		/// Get the Identity Guid (Primary Key) of the Gasboy Device record identified by the provided ID
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="gasboyDeviceID">Identifies the Gasboy Device record to retrieve.</param>
		/// <param name="gasboyDeviceName">Identifies the Gasboy Device record to retrieve.</param>
		/// <returns>The Identity Guid (Primary Key) Gasboy Device record identified by the provided Name. Will return an empty guid if no match is found</returns>
		public Guid GetIdentityGuid(SecurityClass security, long? gasboyDeviceID, string gasboyDeviceName)
		{
			GasboyDevice matchingExternalStation = this.GetByName(security, gasboyDeviceID, gasboyDeviceName);
			return matchingExternalStation == null ? Guid.Empty : matchingExternalStation.IdentityGuid;
		}

		/// <summary>
		/// Add a new Gasboy Device record to the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDevice">The Gasboy Device to add</param>
		/// <returns>The identity guid of the new Gasboy Device record</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, GasboyDevice gasboyDevice)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (gasboyDevice == null)
			{
				throw new ArgumentNullException("gasboyDevice");
			}

			this.Validate(gasboyDevice);

			// Make sure that there is not already a Gasboy Device assigned to or owned by this site
			// with the same Name 
			if (this.GetIdentityGuid(security, gasboyDevice.DeviceID, gasboyDevice.DeviceName) != Guid.Empty)
			{
				throw new Exception("An Gasboy Device with the same Name exists");
			}

			gasboyDevice.IdentityGuid = Guid.NewGuid();
			gasboyDevice.SiteGuid = security.SiteGuid;
			gasboyDevice.CreatedBy = security.UserID;

			using (var dbi = new GasboyDeviceDBI(security.UserID))
			{
				dbi.Insert(security, gasboyDevice);
			}

			// Create a record mapping the Gasboy Device to the current site
			//EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
			//EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(gasboyDevice);
			//entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);


			return gasboyDevice.IdentityGuid;
		}

		/// <summary>
		/// Modify the provided Gasboy Device in the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDevice">The Gasboy Device to modify</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, GasboyDevice gasboyDevice)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (gasboyDevice == null)
			{
				throw new ArgumentNullException("gasboyDevice");
			}

			this.Validate(gasboyDevice);

			// Modify the security object's site guid in case the Gasboy Device's site is changing from an entity ownership change.
			// We want to perform the check in the site the Gasboy Device will be owned by, not the site it's currently owned by.
			Guid siteGuid = security.SiteGuid;
			security.SiteGuid = gasboyDevice.SiteGuid;

			Guid existingGasboyDevice = this.GetIdentityGuid(security, gasboyDevice.DeviceID, gasboyDevice.DeviceName);

			// restore the site guid to its original value
			security.SiteGuid = siteGuid;

			if (existingGasboyDevice != Guid.Empty && existingGasboyDevice != gasboyDevice.IdentityGuid)
			{
				throw new Exception("An Gasboy Device with the same Name exists");
			}

			GasboyDevice oldGasboyDevice = this.Get(security, gasboyDevice.IdentityGuid);

			if (oldGasboyDevice == null || oldGasboyDevice.IdentityGuid == Guid.Empty)
			{
				throw new Exception("The Gasboy Device was not found");
			}

			// If the password is the dummy masked password text, 
			// it has not been modified by the user and the existing value should be preserved
			if (gasboyDevice.PINCode == GasboyStation.PasswordDefaultValue)
			{
				gasboyDevice.PINCode = oldGasboyDevice.PINCode;
			}

			gasboyDevice.UsePINCode = oldGasboyDevice.UsePINCode;
			gasboyDevice.UpdatedBy = security.UserID;

			using (var dbi = new GasboyDeviceDBI(security.UserID))
			{
				dbi.Update(security, gasboyDevice);
			}

			if (gasboyDevice.SiteGuid != oldGasboyDevice.SiteGuid)
			{
				EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
					security, gasboyDevice.EntityType, gasboyDevice.IdentityGuid);

				// If the site changed,
				// Purge any records mapping the Gasboy Device to a site
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = gasboyDevice.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create a new record mapping the Gasboy Device to the new site
				EntityToSiteMapClass newEntityToSiteMap = new EntityToSiteMapClass(gasboyDevice);
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
			}
		}

		/// <summary>
		/// Delete the Gasboy Device identified by the provided guid from the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDeviceGuid">Identifies the Gasboy Device to delete</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid gasboyDeviceGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			GasboyDevice gasboyDevice = this.Get(security, gasboyDeviceGuid);

			if (gasboyDevice.IdentityGuid != Guid.Empty)
			{
				// Delete any records mapping the Gasboy Device to a site
				EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
					security, gasboyDevice.EntityType, gasboyDevice.IdentityGuid);

				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = gasboyDevice.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Delete the Gasboy Device now that the mappings have been removed.
				using (var dbi = new GasboyDeviceDBI(security.UserID))
				{
					gasboyDevice.RecordStatus = GasboyRecordStatus.Deleted;
					dbi.Update(security, gasboyDevice);
				}
			}
			else
			{
				throw new Exception("The Gasboy Device to delete was not found");
			}
		}

		#endregion Gasboy Device Methods

		/// <summary>
		/// Check to make sure the Gasboy Station is valid
		/// </summary>
		/// <param name="gasboyDevice">The Gasboy Station to check</param>
		private void Validate(GasboyDevice gasboyDevice)
		{
			if (string.IsNullOrEmpty(gasboyDevice.DeviceName))
			{
				throw new Exception("Name must be provided for a Gasboy Station");
			}
		}
	}
}