// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryDefaultsClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the QueryDefaultsClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// The QueryDefaultsClass provides access to user-defined query defaults.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class QueryDefaultsClass : IDependency, IQueryDefaults
	{
		#region Constants and Fields

		/// <summary>
		/// Object for database access.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryDefaultsClass"/> class.
		/// </summary>
		public QueryDefaultsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Adds a query default to the database.
		/// </summary>
		/// <param name="security">The FuelsManager security object.</param>
		/// <param name="queryDefault">The query default to add.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, QueryDefaultClass queryDefault)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (queryDefault == null)
			{
				throw new ArgumentNullException("queryDefault");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			// If EntityAssignmentMap exists do not allow addition
			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(
				security, queryDefault.EntityType, security.SiteGuid);

			if (entityToSiteMapCollection.Count > 0)
			{
				throw new Exception("Query Defaults Assigned");
			}

			queryDefault.SiteGuid = security.SiteGuid;
			queryDefault.CreatedDate = DateTimeOffset.Now;
			queryDefault.CreatedBy = security.UserID;
			queryDefault.UpdatedDate = queryDefault.CreatedDate;
			queryDefault.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				queryDefault.IdentityGuid = Guid.NewGuid();
				queryDefault.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Enumerates all query defaults in the specified security context.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A QueryDefaultClass containing the enumerated defaults.</returns>
		public QueryDefaultClass Enumerate(SecurityClass security)
		{
			var queryDefault = new QueryDefaultClass { SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				queryDefault.EnumerateSQL(cmd);
				return this.EnumerateBySql(security, cmd);
			}
		}

		/// <summary>
		/// Enumerates all query defaults in the specified security context filtered by the current site specified in the security object.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A QueryDefaultClass containing the enumerated defaults.</returns>
		public QueryDefaultClass EnumerateBySite(SecurityClass security)
		{
			var queryDefault = new QueryDefaultClass { SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				queryDefault.EnumerateBySiteSQL(cmd);
				return this.EnumerateBySql(security, cmd);
			}
		}

		public QueryDefaultClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var queryDefault = new QueryDefaultClass();
			queryDefault.IdentityGuid = identityGuid;

			if (identityGuid != Guid.Empty)
			{
				using (var cmd = new SqlCommand())
				{
					queryDefault.SelectSQL(cmd, ContextUtil.IsInTransaction);
					queryDefault.Load(this.consolidatedDA.GetDataSet(cmd, security));
				}
			}

			return queryDefault;
		}

		/// <summary>
		/// Modifies the specified query default.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="queryDefault">The query default to save.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, QueryDefaultClass queryDefault)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (queryDefault == null)
			{
				throw new ArgumentNullException("queryDefault");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			// If EntityAssignmentMap exists do not allow modification
			var entityToSiteMaps = new EntityToSiteMaps();

			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(
				security, queryDefault.EntityType, queryDefault.SiteGuid);

			if (entityToSiteMapCollection.Count > 0)
			{
				throw new Exception("QueryDefault Assigned");
			}

			queryDefault.SiteGuid = security.SiteGuid;
			queryDefault.UpdatedDate = DateTimeOffset.Now;
			queryDefault.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				queryDefault.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_QUERIES) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			// Get the old one we are purging
			QueryDefaultClass config = this.Get(security, identityGuid);

			// Purge the default fields as well
			var fields = new QueryDefaultFieldsClass();
			Guid oldSiteGuid = security.SiteGuid;
			security.SiteGuid = config.SiteGuid;
			QueryDefaultFieldCollectionClass fieldCollection = fields.Enumerate(security);
			security.SiteGuid = oldSiteGuid;

			foreach (QueryDefaultFieldClass field in fieldCollection)
			{
				fields.Purge(security, field.IdentityGuid);
			}

			var queryDefault = new QueryDefaultClass();
			queryDefault.SiteGuid = security.SiteGuid;
			queryDefault.IdentityGuid = identityGuid;
			using (var cmd = new SqlCommand())
			{
				queryDefault.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Update(SecurityClass security, QueryDefaultClass queryDefault)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (queryDefault == null)
			{
				throw new ArgumentNullException( "queryDefault" );
			}

			if (!security.HasRight(RIGHT.CONFIGURE_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			var existingDefault = this.Enumerate(security);

			if (existingDefault.IdentityGuid != Guid.Empty)
			{
				queryDefault.IdentityGuid = existingDefault.IdentityGuid;
				this.Modify ( security, queryDefault );
			}
			else
			{
				this.Add(security, queryDefault);
			}
		}

		#endregion

		#region Explicit Interface Methods

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			if (preOperation && Object is EntityToSiteMapClass)
			{
				var entityToSiteMap = (EntityToSiteMapClass)Object;

				var queryDefault = new QueryDefaultClass();
				if (entityToSiteMap.TypeID == queryDefault.EntityType)
				{
					QueryDefaultClass existingDefault = this.Enumerate(security);
					if (existingDefault.IdentityGuid != Guid.Empty)
					{
						this.Purge(security, existingDefault.IdentityGuid);
					}
				}
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject baseObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (baseObject == null)
			{
				throw new ArgumentNullException("baseObject");
			}

			if (baseObject is SiteClass)
			{
				var site = (SiteClass)baseObject;
				var entityToSiteMaps = new EntityToSiteMaps();

				var queryDefault = new QueryDefaultClass();

				var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(
					security, queryDefault.EntityType, site.IdentityGuid);

				foreach (var entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				queryDefault = new QueryDefaultClass { SiteGuid = security.SiteGuid };
				queryDefault = this.Enumerate(security);

				this.Purge(security, queryDefault.IdentityGuid);
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject baseObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (baseObject == null)
			{
				throw new ArgumentNullException("baseObject");
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Enumerates query defaults using the given SqlCommand object.
		/// </summary>
		/// <param name="security">The FuelsManager security object.</param>
		/// <param name="cmd">The SqlCommand object to use.</param>
		/// <returns>A QueryDefaultClass containing the defaults enumerated.</returns>
		private QueryDefaultClass EnumerateBySql(SecurityClass security, SqlCommand cmd)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var set = this.consolidatedDA.GetDataSet(cmd, security);

			var queryDefault = new QueryDefaultClass();

			var table = set.Tables[0];

			if (table.Rows.Count != 0)
			{
				queryDefault = new QueryDefaultClass();
				queryDefault.Load(set);
				table.Rows.RemoveAt(0);
			}

			return queryDefault;
		}

		#endregion
	}
}
