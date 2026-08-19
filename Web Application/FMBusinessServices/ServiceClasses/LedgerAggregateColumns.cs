using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class LedgerAggregateColumnsClass : IDependency, ILedgerAggregateColumns
	{
		#region private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public LedgerAggregateColumnsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Private methods
		private void Validate(SecurityClass security, LedgerAggregateColumnClass AggregateColumn)
		{
			if (AggregateColumn.ID == "")
			{
				throw new ApplicationException("ID Required");
			}

			Guid guid = GetIdentityGuid(security, AggregateColumn.ID);

			if (guid != Guid.Empty && guid != AggregateColumn.IdentityGuid)
			{
				throw (new ApplicationException("Aggregate Column Exists"));
			}

			// There cannot be an aggregate column and alias with the same ID, or else errors
			// occur displaying ledger
			TransactionAliasesClass aliases = new TransactionAliasesClass();
			guid = aliases.GetIdentityGuid(security, AggregateColumn.ID);

			if (guid != Guid.Empty)
			{
				throw (new ApplicationException("ID matches existing Transaction Alias ID"));
			}

			if (AggregateColumn.AggregateField == LedgerAggregateColumnClass.AggregateType.CustomFunction &&
				string.IsNullOrEmpty(AggregateColumn.CustomFunctionName))
			{
				throw new ApplicationException("Custom function name cannot be empty");
			}
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, LedgerAggregateColumnClass AggregateColumn)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (AggregateColumn == null)
			{
				throw new ArgumentNullException("AggregateColumn");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, AggregateColumn);

			AggregateColumn.SiteGuid = security.SiteGuid;
			AggregateColumn.CreatedDate = DateTimeOffset.Now;
			AggregateColumn.CreatedBy = security.UserID;
			AggregateColumn.UpdatedDate = AggregateColumn.CreatedDate;
			AggregateColumn.UpdatedBy = security.UserID;
			AggregateColumn.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				AggregateColumn.InsertSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}

			LedgerAggregateColumnMapsClass columnMaps = new LedgerAggregateColumnMapsClass();
			columnMaps.ModifyCollection(security, AggregateColumn.IdentityGuid, AggregateColumn.Aliases);

			// Create Entity to Site Map
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(AggregateColumn);
			EntityToSiteMaps.Add(security, EntityToSiteMap, GetType().GUID);

			return AggregateColumn.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, LedgerAggregateColumnClass AggregateColumn)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (AggregateColumn == null)
			{
				throw new ArgumentNullException("AggregateColumn");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, AggregateColumn);
			LedgerAggregateColumnClass oldColumn = this.GetByColumnGuid(security, AggregateColumn.IdentityGuid);

			if (oldColumn.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Aggregate Column Not Found"));
			}

			AggregateColumn.UpdatedDate = DateTimeOffset.Now;
			AggregateColumn.UpdatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				AggregateColumn.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			LedgerAggregateColumnMapsClass columnMaps = new LedgerAggregateColumnMapsClass();
			columnMaps.ModifyCollection(security, AggregateColumn.IdentityGuid, AggregateColumn.Aliases);

			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, AggregateColumn.EntityType, AggregateColumn.IdentityGuid);

			if (AggregateColumn.SiteGuid != oldColumn.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
				{
					EntityToSiteMaps.Purge(security, EntityToSiteMap);
				}

				// Create Entity to Site Map
				EntityToSiteMapClass NewEntityToSiteMap = new EntityToSiteMapClass(AggregateColumn);
				EntityToSiteMaps.Add(security, NewEntityToSiteMap, GetType().GUID);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid columnGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			LedgerAggregateColumnClass column = this.GetByColumnGuid(security, columnGuid);

			if (column.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Aggregate Column Not Found"));
			}

			// Purge from EntityToSiteMap
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, column.EntityType, columnGuid);

			foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
			{
				EntityToSiteMaps.Purge(security, EntityToSiteMap);
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				column.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			LedgerAggregateColumnMapsClass columnMaps = new LedgerAggregateColumnMapsClass();
			columnMaps.PurgeCollection(security, columnGuid);
		}

		public LedgerAggregateColumnClass GetByColumnID(SecurityClass security, string columnID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			LedgerAggregateColumnClass column = new LedgerAggregateColumnClass();
			column.ID = columnID;
			column.SiteGuid = security.SiteGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				column.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				column.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}
			return column;
		}

		public LedgerAggregateColumnClass GetByColumnGuid(SecurityClass security, Guid columnGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			LedgerAggregateColumnClass column = new LedgerAggregateColumnClass();
			column.IdentityGuid = columnGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				column.SelectSQL(cmd, security, ContextUtil.IsInTransaction);
				column.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}
			LedgerAggregateColumnMapsClass columnMaps = new LedgerAggregateColumnMapsClass();
			column.Aliases = columnMaps.Enumerate(security, column.IdentityGuid);

			return column;
		}

		public Guid GetIdentityGuid(SecurityClass security, string ID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			LedgerAggregateColumnClass column = new LedgerAggregateColumnClass();
			column.ID = ID;
			column.SiteGuid = security.SiteGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				column.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				column.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return column.IdentityGuid;
		}

		public LedgerAggregateColumnCollectionClass Enumerate(SecurityClass security)
		{
			return this.EnumerateByFindText(security, string.Empty);
		}

		public LedgerAggregateColumnCollectionClass EnumerateByFindText(SecurityClass security, string findText)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			LedgerAggregateColumnClass column = new LedgerAggregateColumnClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				column.EnumerateSQL(cmd, security, findText);
				DataSet set = this.consolidatedDA.GetDataSet(cmd, security);

				LedgerAggregateColumnCollectionClass columnCollection = new LedgerAggregateColumnCollectionClass();

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					column = new LedgerAggregateColumnClass();
					column.Load(set);
					columnCollection.Add(column);
					table.Rows.RemoveAt(0);
				}

				return columnCollection;
			}
		}

		#region Dependency methods
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
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				LedgerAggregateColumnCollectionClass columnCollection = Enumerate(security);
				EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();

				foreach (LedgerAggregateColumnClass Column in columnCollection)
				{
					if (Site.SiteGuid == Column.SiteGuid)
					{
						EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, Column.EntityType, Column.IdentityGuid);

						foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
						{
							if (EntityToSiteMap.SiteGuid != Site.SiteGuid)
								EntityToSiteMaps.Purge(security, EntityToSiteMap);
						}
					}
				}
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}


			// Purge Equipment Deleted/Undeleted
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				LedgerAggregateColumnCollectionClass columnCollection = Enumerate(security);
				EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();

				foreach (LedgerAggregateColumnClass Column in columnCollection)
				{
					if (Site.IdentityGuid == Column.SiteGuid)
					{
						Purge(security, Column.IdentityGuid);
					}
					else
					{
						EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(Column);
						EntityToSiteMap.SiteGuid = Site.SiteGuid;
						EntityToSiteMaps.Purge(security, EntityToSiteMap);
					}
				}
			}
		}
		#endregion

	}
}