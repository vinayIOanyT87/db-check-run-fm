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
	/// Supports adding, modifying, deleting, and reading quality tag records
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class QualityTagsClass : IDependency, IQualityTags
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public QualityTagsClass()
		{
		}

		private void Validate(QualityTagClass qualityTag)
		{
			if (qualityTag.ID.Trim().Length == 0)
			{
				throw (new Exception("Name Required"));
			}

			if (qualityTag.ID.Length > 50)
			{
				throw (new Exception("Name must not contain more than 50 characters."));
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, QualityTagClass qualityTag)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (qualityTag == null)
			{
				throw new ArgumentNullException("qualityTag");
			}

			if (!security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			if (GetIdentityGuid(security, qualityTag.ID) != Guid.Empty)
			{
				throw new Exception("QualityTag Exists.");
			}

			this.Validate(qualityTag);

			qualityTag.CreatedDate = DateTimeOffset.Now;
			qualityTag.CreatedBy = security.UserID;
			qualityTag.UpdatedDate = qualityTag.CreatedDate;
			qualityTag.UpdatedBy = security.UserID;
			qualityTag.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				qualityTag.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// Add an entity to site mapping record to indicate that the quality tag is assigned to the site it was created in
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(qualityTag);
			entityToSiteMaps.Add(security, entityToSiteMap, GetType().GUID);

			return qualityTag.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, QualityTagClass qualityTag)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (qualityTag == null)
			{
				throw new ArgumentNullException("qualityTag");
			}

			this.Validate(qualityTag);

			QualityTagClass oldQualityTag = Get(security, qualityTag.IdentityGuid);

          if (oldQualityTag.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("QualityTag Not Found"));
			}

			Guid guid = this.GetIdentityGuid(security, qualityTag.ID);

			if (guid != Guid.Empty && guid != qualityTag.IdentityGuid)
			{
				throw (new Exception("QualityTag Exists by the same ID: " + qualityTag.ID));
			}

			if (qualityTag.SiteGuid != oldQualityTag.SiteGuid)
			{
				var entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = 
									entityToSiteMaps.EnumerateByTypeIDAndGuid(security, qualityTag.EntityType, qualityTag.IdentityGuid);

				// If the ownership of the entity is changing, purge any existing assignments and add a new one for the new site owner.
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = qualityTag.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				var newEntityToSiteMap = new EntityToSiteMapClass(qualityTag);
				entityToSiteMaps.Add(security, newEntityToSiteMap, GetType().GUID);
			}

			qualityTag.UpdatedDate = DateTimeOffset.Now;
			qualityTag.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				qualityTag.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public QualityTagClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var qualityTag = new QualityTagClass { IdentityGuid = identityGuid };

			if (identityGuid != Guid.Empty)
			{
				using (var cmd = new SqlCommand())
				{
					qualityTag.SelectSQL(cmd, ContextUtil.IsInTransaction);
					qualityTag.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			return qualityTag;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (id == "{All}" || id == "{Unassigned}" || id == "{None}")
			{
				return Guid.Empty;
			}

			var qualityTag = new QualityTagClass { ID = id };

			using (var cmd = new SqlCommand())
			{
				qualityTag.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				qualityTag.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return qualityTag.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid qualityTagGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			QualityTagClass qualityTag = Get(security, qualityTagGuid);

			if (qualityTag.IdentityGuid == Guid.Empty)
			{
				return;
			}

			// Delete any existing entity to site mapping records that are associated with this quality tag
			var entityToSiteMaps = new EntityToSiteMaps();

			EntityToSiteMapCollectionClass entityToSiteMapCollection = 
								entityToSiteMaps.EnumerateByTypeIDAndGuid(security, qualityTag.EntityType, qualityTagGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = qualityTag.ID;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			using (var cmd = new SqlCommand())
			{
				qualityTag.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, qualityTag);
		}

		/// <summary>
		/// When the user assigns or changes ownership of an entity, first check to make sure that a 
		/// duplicate quality tag record does not exist
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="Object">The object being inserted</param>
		/// <param name="preOperation">True if this method is being called before the insert of the entityToSiteMap record</param>
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		/// <summary>
		/// Implemented because we implement IDependency
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="Object">The object being updated</param>
		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
		}

		/// <summary>
		/// When a site is deleted, delete any quality tags it owns and also delete any entity to site
		/// records mapping the quality tag to the site
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="Object">The object being deleted.</param>
		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			if (Object is SiteClass)
			{
				var site = (SiteClass)Object;
				QualityTagCollectionClass qualityTags = this.Enumerate(security, null, null, false);
				
				foreach (QualityTagClass qualityTag in qualityTags)
				{
					if (site.SiteGuid == qualityTag.SiteGuid)
					{
						this.Purge(security, qualityTag.IdentityGuid);
					}
					else
					{
						var entityToSiteMaps = new EntityToSiteMaps();
						var entityToSiteMap = new EntityToSiteMapClass(qualityTag) { SiteGuid = site.SiteGuid };
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
		}

		public QualityTagCollectionClass Enumerate(SecurityClass security, string filter, string order, bool activeTagsOnly)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_LOGS)
				&& !security.HasRight(RIGHT.VIEW_TEST_ITEMS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var qualityTag = new QualityTagClass();

			using (var cmd = new SqlCommand())
			{
				qualityTag.EnumerateSQL(cmd, security, filter, order, activeTagsOnly);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var qualityTagCollection = new QualityTagCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					qualityTag = new QualityTagClass();
					qualityTag.Load(set);
					qualityTagCollection.Add(qualityTag);
					table.Rows.RemoveAt(0);
				}

				return qualityTagCollection;
			}
		}
	}
}
