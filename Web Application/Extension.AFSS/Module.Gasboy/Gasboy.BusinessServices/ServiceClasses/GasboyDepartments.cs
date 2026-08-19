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

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.BusinessServices.Repository;

	/// <summary>
	/// Implements operations to support database operations for Gasboy Devices
	/// like adding, modifying, or deleting a record.
	/// </summary>
	public class GasboyDepartments : IGasboyDepartments
	{
		/// <summary>
		/// Allows database access.
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();
		
		/// <summary>
		/// Get stations configured for the site, filtering by the filter text if it was provided
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <returns>Stations configured for the site, filtered by the filter text if it was provided</returns>
		public GasboyDepartmentCollection Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

GasboyDepartmentCollection departments = new GasboyDepartmentCollection();

			GasboyDepartment defaultDepartment = new GasboyDepartment();
			defaultDepartment.FleetIdentityGuid = GasboySpecialConstants.DefaultFleetGuid;
			defaultDepartment.IdentityGuid = GasboySpecialConstants.DefaultDepartmentGuid;
			defaultDepartment.DepartmentName = GasboySpecialConstants.DefaultDepartmentName;
			defaultDepartment.DepartmentID = GasboySpecialConstants.DefaultDepartmentID;
			defaultDepartment.DepartmentCode = GasboySpecialConstants.DefaultDepartmentCode;

			GasboyDepartment blacklistDepartment = new GasboyDepartment();
			blacklistDepartment.FleetIdentityGuid = GasboySpecialConstants.DefaultFleetGuid;
			blacklistDepartment.IdentityGuid = GasboySpecialConstants.BlacklistDepartmentGuid;
			blacklistDepartment.DepartmentName = GasboySpecialConstants.DefaultBlackListDepartmentName;
			blacklistDepartment.DepartmentID = GasboySpecialConstants.DefaultBlackListDepartmentID;
			blacklistDepartment.DepartmentCode = GasboySpecialConstants.DefaultBlackListDepartmentCode;

			departments.Add(defaultDepartment);
			departments.Add(blacklistDepartment);

			//Currently, Fleets and Departments are not managed by FMD so these constants are stored in GasboySpecialConstants
			//When we want FMD to manage these, we need to implement entity mapping and summary detail pages for each. 

			//GasboyDepartmentCollection departments = null;

			//using (var dbi = new GasboyDepartmentDBI(security.UserID))
			//{
			//	departments = dbi.GetList(security, security.SiteGuid, null, null);
			//}

			return departments;
		}

		public GasboyDepartment Get(SecurityClass security, Guid gasboyDepartmentGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			GasboyDepartment department = new GasboyDepartment();

			if (gasboyDepartmentGuid == GasboySpecialConstants.DefaultDepartmentGuid)
			{
				department.FleetIdentityGuid = GasboySpecialConstants.DefaultFleetGuid;
				department.IdentityGuid = GasboySpecialConstants.DefaultDepartmentGuid;
				department.DepartmentName = GasboySpecialConstants.DefaultDepartmentName;
				department.DepartmentID = GasboySpecialConstants.DefaultDepartmentID;
				department.DepartmentCode = GasboySpecialConstants.DefaultDepartmentCode;
			}
			if (gasboyDepartmentGuid == GasboySpecialConstants.BlacklistDepartmentGuid)
			{
				department.FleetIdentityGuid = GasboySpecialConstants.DefaultFleetGuid;
				department.IdentityGuid = GasboySpecialConstants.BlacklistDepartmentGuid;
				department.DepartmentName = GasboySpecialConstants.DefaultBlackListDepartmentName;
				department.DepartmentID = GasboySpecialConstants.DefaultBlackListDepartmentID;
				department.DepartmentCode = GasboySpecialConstants.DefaultBlackListDepartmentCode;
			}
			else
			{
				throw new Exception("Department was not found.");
			}

			//Currently, Fleets and Departments are not managed by FMD so these constants are stored in GasboySpecialConstants
			//When we want FMD to manage these, we need to implement entity mapping and summary detail pages for each. 
			//GasboyDepartment department = null;
			//using (var dbi = new GasboyDepartmentDBI(security.UserID))
			//{
			//	department = dbi.Get(security, gasboyDepartmentGuid);
			//}

			return department;
		}
	}
}
