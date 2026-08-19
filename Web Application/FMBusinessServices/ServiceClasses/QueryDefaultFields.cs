using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;
using System.Xml.Serialization;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class QueryDefaultFieldsClass : IQueryDefaultFields
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public QueryDefaultFieldsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, QueryDefaultFieldClass QueryDefaultField)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (QueryDefaultField == null)
				throw new ArgumentNullException("QueryDefaultField");

			if (!security.HasRight(RIGHT.CONFIGURE_QUERIES))
				throw new FMInsufficientRightsException();

			// If EntityAssignmentMap exists do not allow addition
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, QueryDefaultField.EntityType, security.SiteGuid);
			if (EntityToSiteMapCollection.Count > 0)
				throw new Exception("Query Defaults Assigned");

			QueryDefaultField.SiteGuid = security.SiteGuid;
			QueryDefaultField.CreatedDate = DateTimeOffset.Now;
			QueryDefaultField.CreatedBy = security.UserID;
			QueryDefaultField.UpdatedDate = QueryDefaultField.CreatedDate;
			QueryDefaultField.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				QueryDefaultField.IdentityGuid = Guid.NewGuid();
				QueryDefaultField.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, QueryDefaultFieldClass QueryDefaultField)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (QueryDefaultField == null)
			{
				throw new ArgumentNullException("QueryDefaultField");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_QUERIES))
				throw new FMInsufficientRightsException();

			// If EntityAssignmentMap exists it do not allow modification
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, QueryDefaultField.EntityType, QueryDefaultField.SiteGuid);
			if (EntityToSiteMapCollection.Count > 0)
				throw new Exception("QueryDefaultField Assigned");

			QueryDefaultFieldClass OldQueryDefaultField = Get(security, QueryDefaultField.IdentityGuid);

			QueryDefaultField.UpdatedDate = DateTimeOffset.Now;
			QueryDefaultField.UpdatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				QueryDefaultField.UpdateSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			QueryDefaultFieldClass QueryDefaultField = new QueryDefaultFieldClass();
			QueryDefaultField.SiteGuid = security.SiteGuid;
			QueryDefaultField.IdentityGuid = identityGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				QueryDefaultField.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Update(SecurityClass security, QueryDefaultFieldCollectionClass fieldCollection)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (fieldCollection == null)
			{
				throw new ArgumentNullException("QueryDefaultFieldCollection");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			// Purge all existing
			QueryDefaultFieldCollectionClass ExistingCollection = Enumerate(security);
			foreach (QueryDefaultFieldClass ExistingField in ExistingCollection)
			{
				Purge(security, ExistingField.IdentityGuid);
			}

			// Add the new list to the database
			foreach (QueryDefaultFieldClass DefaultField in fieldCollection)
			{
				Add(security, DefaultField);
			}

		}

		public QueryDefaultFieldClass Get(SecurityClass Security, Guid identityGuid)
		{
			if (Security == null)
			{
				throw new ArgumentNullException("Security");
			}

			QueryDefaultFieldClass QueryDefaultField = new QueryDefaultFieldClass();
			QueryDefaultField.IdentityGuid = identityGuid;

			if (identityGuid != Guid.Empty)
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					QueryDefaultField.SelectSQL(cmd, ContextUtil.IsInTransaction);
					QueryDefaultField.Load(this.ConsolidatedDA.GetDataSet(cmd, Security));
				}

			}

			return QueryDefaultField;

		}

		public QueryDefaultFieldCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			QueryDefaultFieldClass QueryDefaultField = new QueryDefaultFieldClass();
			QueryDefaultField.SiteGuid = security.SiteGuid;

			DataSet Set;
			using (SqlCommand cmd = new SqlCommand())
			{
				QueryDefaultField.EnumerateSQL(cmd);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			QueryDefaultFieldCollectionClass dataCollection = new QueryDefaultFieldCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				QueryDefaultField = new QueryDefaultFieldClass();
				QueryDefaultField.Load(Set);
				dataCollection.Add(QueryDefaultField);
				Table.Rows.RemoveAt(0);
			}

			return dataCollection;
		}

		public QueryDefaultFieldCollectionClass EnumerateBySite(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			QueryDefaultFieldClass QueryDefaultField = new QueryDefaultFieldClass();
			QueryDefaultField.SiteGuid = security.SiteGuid;

			DataSet Set;
			using (SqlCommand cmd = new SqlCommand())
			{
				QueryDefaultField.EnumerateBySiteSQL(cmd);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			QueryDefaultFieldCollectionClass dataCollection = new QueryDefaultFieldCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				QueryDefaultField = new QueryDefaultFieldClass();
				QueryDefaultField.Load(Set);
				dataCollection.Add(QueryDefaultField);
				Table.Rows.RemoveAt(0);
			}

			return dataCollection;
		}
	}

}
