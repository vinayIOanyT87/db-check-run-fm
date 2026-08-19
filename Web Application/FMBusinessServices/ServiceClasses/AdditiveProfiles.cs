// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdditiveProfiles.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for AdditiveProfiles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Implmentation of the additive profiles service class.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public sealed class AdditiveProfiles : IDependency, IAdditiveProfiles
	{
		#region Constants and Fields

		/// <summary>
		/// Provides access to the database.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Adds the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="additiveProfile">The additive profile.</param>
		/// <returns>
		/// The Guid of the newly added profile.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AdditiveProfileClass additiveProfile)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (additiveProfile == null)
			{
				throw new ArgumentNullException("additiveProfile");
			}

			if (!security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			if (this.GetIdentityGuid(security, additiveProfile.ID) != Guid.Empty)
			{
				throw new Exception("Additive Profile Exists");
			}

			additiveProfile.SiteGuid = security.SiteGuid;
			additiveProfile.CreatedDate = DateTimeOffset.UtcNow;
			additiveProfile.CreatedBy = security.UserID;
			additiveProfile.UpdatedDate = additiveProfile.CreatedDate;
			additiveProfile.UpdatedBy = security.UserID;
			additiveProfile.Deleted = false;
			additiveProfile.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				additiveProfile.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(additiveProfile);
			entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

			var productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(
				security, additiveProfile.IdentityGuid, additiveProfile.ID, false, additiveProfile.AdditiveCollection, null);

			return additiveProfile.IdentityGuid;
		}

		/// <summary>
		/// Enumerates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>
		/// A collection of additive profiles.
		/// </returns>
		public AdditiveProfileCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS)
				&& !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var additiveProfile = new AdditiveProfileClass();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				additiveProfile.EnumerateSQL(cmd, security);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var additiveProfileCollection = new AdditiveProfileCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				additiveProfile = new AdditiveProfileClass();
				additiveProfile.LoadProfile(set);
				additiveProfileCollection.Add(additiveProfile);
				table.Rows.RemoveAt(0);
			}

			return additiveProfileCollection;
		}

	    /// <summary>
	    /// Gets the specified security.
	    /// </summary>
	    /// <param name="security">The security.</param>
	    /// <param name="identityGuid">The identity GUID.</param>
	    /// <param name="hideHiddenProducts">If true, only products that are not inactive will be returned in the additve collection</param>
	    /// <returns>
	    /// An additive profile class.
	    /// </returns>
	    public AdditiveProfileClass Get(SecurityClass security, Guid identityGuid, bool hideHiddenProducts = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			var additiveProfile = new AdditiveProfileClass { IdentityGuid = identityGuid };

			using (var cmd = new SqlCommand())
			{
				additiveProfile.SelectSQL(cmd, ContextUtil.IsInTransaction);
				additiveProfile.LoadProfile(this.consolidatedDA.GetDataSet(cmd, security));
			}

			var productMaps = new ProductMapsClass();
			additiveProfile.AdditiveCollection = productMaps.EnumerateByAssignedToGuidAndType(
				security, additiveProfile.IdentityGuid, PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP, hideHiddenProducts);

			return additiveProfile;
		}

		/// <summary>
		/// This method
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public DataSet EnumerateAdditiveProfilesAllSites(SecurityClass security)
        {
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			var additiveProfile = new AdditiveProfileClass();
			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				AdditiveProfileDAO.EnumerateAdditiveProfilesAllSitesSql(cmd);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			return dataSet;
		}

		/// <summary>
		/// Gets the identity GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="id">The ID.</param>
		/// <returns>
		/// The identity Guid of the additive profile with ID.
		/// </returns>
		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			var additiveProfile = new AdditiveProfileClass { ID = id, SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				additiveProfile.SelectByIdSql(cmd, security, ContextUtil.IsInTransaction);
				additiveProfile.LoadProfile(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return additiveProfile.IdentityGuid;
		}

		/// <summary>
		/// Modifies the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="additiveProfile">The additive profile.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AdditiveProfileClass additiveProfile)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (additiveProfile == null)
			{
				throw new ArgumentNullException("additiveProfile");
			}

			if (!security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			Guid identityGuid = this.GetIdentityGuid(security, additiveProfile.ID);
			if (identityGuid != Guid.Empty && identityGuid != additiveProfile.IdentityGuid)
			{
				throw new Exception("Additive Profile Exists");
			}

			AdditiveProfileClass oldAdditiveProfile = this.Get(security, additiveProfile.IdentityGuid);

 
         if (oldAdditiveProfile.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Additive Profile Not Found");
			}

			additiveProfile.UpdatedDate = DateTimeOffset.Now;
			additiveProfile.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				additiveProfile.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
				security, additiveProfile.EntityType, additiveProfile.IdentityGuid);
			if (additiveProfile.SiteGuid != oldAdditiveProfile.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = additiveProfile.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(additiveProfile);
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
			}

			var productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(
				security, 
				additiveProfile.IdentityGuid, 
				additiveProfile.ID, 
				false, 
				additiveProfile.AdditiveCollection, 
				oldAdditiveProfile.AdditiveCollection);
		}

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="identityGuid">The identity GUID.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			AdditiveProfileClass additiveProfile = this.Get(security, identityGuid);
			if (additiveProfile.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Additive Profile Not Found");
			}

			var productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(
				security, additiveProfile.IdentityGuid, additiveProfile.ID, false, null, additiveProfile.AdditiveCollection);

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, additiveProfile);

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();

			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
				security, additiveProfile.EntityType, identityGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = additiveProfile.ID;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			using (var cmd = new SqlCommand())
			{
				additiveProfile.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		#endregion

		#region Explicit Interface Methods

		/// <summary>
		/// Inserts the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="baseDataObject">The object.</param>
		void IDependency.Insert(SecurityClass security, BaseDataObject baseDataObject, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (baseDataObject == null)
			{
				throw new ArgumentNullException("baseDataObject");
			}
		}

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="baseDataObject">The object.</param>
		void IDependency.Purge(SecurityClass security, BaseDataObject baseDataObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (baseDataObject == null)
			{
				throw new ArgumentNullException("baseDataObject");
			}

			if (baseDataObject is SiteClass)
			{
				var site = (SiteClass)baseDataObject;
				AdditiveProfileCollectionClass additiveProfileCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (AdditiveProfileClass additiveProfile in additiveProfileCollection)
				{
					if (site.SiteGuid == additiveProfile.SiteGuid)
					{
						this.Purge(security, additiveProfile.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass
							{ TypeID = additiveProfile.EntityType, SiteGuid = site.SiteGuid, IdentityGuid = additiveProfile.IdentityGuid };

						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
		}

		/// <summary>
		/// Updates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="baseDataObject">The object.</param>
		void IDependency.Update(SecurityClass security, BaseDataObject baseDataObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (baseDataObject == null)
			{
				throw new ArgumentNullException("baseDataObject");
			}

			if (baseDataObject is SiteClass)
			{
				var site = (SiteClass)baseDataObject;
				AdditiveProfileCollectionClass additiveProfileCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (AdditiveProfileClass additiveProfile in additiveProfileCollection)
				{
					if (site.SiteGuid == additiveProfile.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
							security, additiveProfile.EntityType, additiveProfile.IdentityGuid);

						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = additiveProfile.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
		}

		#endregion
	}
}