// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchConfigurations.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchConfigurations type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Definition of the DispatchConfigurations service class.  Provides a database interface for
	/// the DispatchConfigurationClass type.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class DispatchConfigurations : IDispatchConfigurations
	{
		/// <summary>
		/// The ConsolidatedDAClass object provides database access
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa;

		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchConfigurations"/> class.
		/// </summary>
		public DispatchConfigurations()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}

		/// <summary>
		/// Adds a DispatchConfigurationClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfig">The object to add to the database</param>
		/// <returns>The identity Guid of the added record</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, DispatchConfigurationClass dispatchConfig)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dispatchConfig == null)
			{
				throw new ArgumentNullException("dispatchConfig");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_DISPATCH_VALIDATIONS))
			{
				throw new FMInsufficientRightsException();
			}

			dispatchConfig.SiteGuid = security.SiteGuid;
			dispatchConfig.CreatedDate = DateTimeOffset.Now;
			dispatchConfig.CreatedBy = security.UserID;
			dispatchConfig.UpdatedDate = dispatchConfig.CreatedDate;
			dispatchConfig.UpdatedBy = security.UserID;
			dispatchConfig.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				dispatchConfig.InsertSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(dispatchConfig);
			entityToSiteMaps.Add(security, entityToSiteMap, GetType().GUID);

			dispatchConfig = this.Get(security, dispatchConfig.IdentityGuid);

			return dispatchConfig.IdentityGuid;
		}

		/// <summary>
		///  Modifies an existing DispatchConfigurationClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfig">The object to modify in the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, DispatchConfigurationClass dispatchConfig)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dispatchConfig == null)
			{
				throw new ArgumentNullException("dispatchConfig");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_DISPATCH_VALIDATIONS))
			{
				throw new FMInsufficientRightsException();
			}

			Guid dispatchConfigGuid = this.GetIdentityGuidBySiteAndId(security, dispatchConfig.SiteGuid, dispatchConfig.ID);

			if (dispatchConfigGuid != Guid.Empty && dispatchConfigGuid != dispatchConfig.IdentityGuid)
			{
				throw new Exception("Dispatch Configuration Exists");
			}

			DispatchConfigurationClass oldDispatchConfiguration = this.Get(security, dispatchConfig.IdentityGuid);

			if (oldDispatchConfiguration.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Dispatch Configuration Not Found");
			}

			dispatchConfig.UpdatedDate = DateTimeOffset.Now;
			dispatchConfig.UpdatedBy = security.UserID;
			using (var cmd = new SqlCommand())
			{
				dispatchConfig.UpdateSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, dispatchConfig.EntityType, dispatchConfig.IdentityGuid);

			if (dispatchConfig.SiteGuid != oldDispatchConfiguration.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (var entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = dispatchConfig.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(dispatchConfig);
				entityToSiteMaps.Add(security, newEntityToSiteMap, GetType().GUID);
			}
		}

		/// <summary>
		/// Deletes an existing DispatchConfigurationClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfigGuid">The identity Guid of the object to delete from the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid dispatchConfigGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_DISPATCH_VALIDATIONS))
			{
				throw new FMInsufficientRightsException();
			}

			DispatchConfigurationClass dispatchConfig = this.Get(security, dispatchConfigGuid);
			if (dispatchConfig.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Dispatch Configuration Not Found");
			}

			// Purge from EntityToSiteMap using delete stored procedure
			using (var cmd = new SqlCommand())
			{
				dispatchConfig.PurgeFromEntityToSiteMapSql(cmd);

				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			using (var cmd = new SqlCommand())
			{
				dispatchConfig.PurgeSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Gets an existing DispatchConfigurationClass object from the database given the identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfigGuid">The identity Guid of the object to get from the database</param>
		/// <returns>The specified DispatchConfigurationClass object</returns>
		public DispatchConfigurationClass Get(SecurityClass security, Guid dispatchConfigGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var dispatchConfig = new DispatchConfigurationClass { IdentityGuid = dispatchConfigGuid };

			using (var cmd = new SqlCommand())
			{
				dispatchConfig.SelectSql(cmd, ContextUtil.IsInTransaction);
				dispatchConfig.Load(this.consolidatedDa.GetDataSet(cmd, security));
			}

			return dispatchConfig;
		}

		/// <summary>
		/// Gets the identity Guid of a DispatchConfigurationClass object from the database given the ID.
		/// Assigned entities are given preference to owned entities when both exist with the specified ID.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The ID of the DispatchConfigurationClass object</param>
		/// <returns>The identity Guid of the specified DispatchConfigurationClass object</returns>
		public Guid GetIdentityGuidById(SecurityClass security, string id)
		{
			bool entityAssigned;
			return this.GetIdentityGuidBySiteIdAndAssigned(security, security.SiteGuid, id, true, out entityAssigned);
		}

		/// <summary>
		/// Gets the identity Guid of a DispatchConfigurationClass object from the database given the Site Guid and ID.
		/// Owned entities are given preference to assigned entities when both exist with the specified Site Guid and ID.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="siteGuid">The Site Guid of the DispatchConfigurationClass object</param>
		/// <param name="id">The ID of the DispatchConfigurationClass object</param>
		/// <returns>The identity Guid of the specified DispatchConfigurationClass object</returns>
		public Guid GetIdentityGuidBySiteAndId(SecurityClass security, Guid siteGuid, string id)
		{
			bool entityAssigned;
			return this.GetIdentityGuidBySiteIdAndAssigned(security, siteGuid, id, false, out entityAssigned);
		}

		/// <summary>
		/// Gets the identity Guid of a DispatchConfigurationClass object from the database given the Site Guid and ID.  The parameter
		/// getAssignedEntityFirst is used to determine how to select between an owned and assigned entity when both exist.  If set
		/// to true and both entities exist the Guid of the assigned entity is returned otherwise the Guid of the owned entity is
		/// returned.  If only one entity exists then its Guid is returned whether it is an assigned or owned entity.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="siteGuid">The Site Guid of the DispatchConfigurationClass object</param>
		/// <param name="id">The ID of the DispatchConfigurationClass object</param>
		/// <param name="getAssignedEntityFirst">If an assigned entity exists get its identity Guid</param>
		/// <param name="entityAssigned">True if the returned identity Guid is from an assigned entity</param>
		/// <returns>The identity Guid of the specified DispatchConfigurationClass object</returns>
		public Guid GetIdentityGuidBySiteIdAndAssigned(SecurityClass security, Guid siteGuid, string id, bool getAssignedEntityFirst, out bool entityAssigned)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet set;
			var dispatchConfig = new DispatchConfigurationClass { SiteGuid = siteGuid, ID = id };
			using (var cmd = new SqlCommand())
			{
				dispatchConfig.SelectByIdSql(cmd, ContextUtil.IsInTransaction);
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			entityAssigned = false;
			DataTable table = set.Tables[0];

			// If exactly one data row exits than use it
			if (table.Rows.Count == 1)
			{
				dispatchConfig = new DispatchConfigurationClass();
				dispatchConfig.Load(set);
				if (dispatchConfig.SiteGuid != security.SiteGuid)
				{
					entityAssigned = true;
				}
			}
			else
			{
				while (table.Rows.Count != 0)
				{
					dispatchConfig = new DispatchConfigurationClass();
					dispatchConfig.Load(set);
					if (getAssignedEntityFirst)
					{
						// Look for assigned entities first.  If the entity Site Guid
						// differs from the security Site Guid the entity is assigned.
						if (dispatchConfig.SiteGuid != security.SiteGuid)
						{
							entityAssigned = true;
							break;
						}
					}
					else
					{
						// Look for owned entities first.  If the entity Site Guid
						// equals the security Site Guid the entity is owned.
						if (dispatchConfig.SiteGuid == security.SiteGuid)
						{
							break;
						}
					}

					table.Rows.RemoveAt(0);
				}
			}

			return dispatchConfig.IdentityGuid;
		}

		/// <summary>
		/// Gets a list of DispatchConfigurationClass objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of DispatchConfigurationClass objects</returns>
		public DispatchConfigurationCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var dispatchConfig = new DispatchConfigurationClass { SiteGuid = security.SiteGuid };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				dispatchConfig.EnumerateSql(security, cmd);
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			var dispatchConfigCollection = new DispatchConfigurationCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				dispatchConfig = new DispatchConfigurationClass();
				dispatchConfig.Load(set);
				dispatchConfigCollection.Add(dispatchConfig);
				table.Rows.RemoveAt(0);
			}

			return dispatchConfigCollection;
		}
	}
}