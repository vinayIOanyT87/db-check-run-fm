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
	public class LedgerAggregateColumnMapsClass : ILedgerAggregateColumnMaps
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public LedgerAggregateColumnMapsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, LedgerAggregateColumnMapClass ledgerAggregateColumnMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (ledgerAggregateColumnMap == null)
			{
				throw new ArgumentNullException("LedgerAggregateColumnMap");
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				ledgerAggregateColumnMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
				DataSet Set = this.consolidatedDA.GetDataSet(cmd, security);

				if (Set.Tables[0].Rows.Count != 0)
				{
					return;
				}
			}

			ledgerAggregateColumnMap.CreatedDate = DateTimeOffset.Now;
			ledgerAggregateColumnMap.CreatedBy = security.UserID;
			ledgerAggregateColumnMap.UpdatedDate = ledgerAggregateColumnMap.CreatedDate;
			ledgerAggregateColumnMap.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				ledgerAggregateColumnMap.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, LedgerAggregateColumnMapClass ColumnMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (ColumnMap == null)
			{
				throw new ArgumentNullException("ColumnMap");
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				ColumnMap.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(SecurityClass security, Guid columnGuid, LedgerAggregateColumnMapCollectionClass columnMaps)
		{
			this.PurgeCollection(security, columnGuid);

			// Add new ones
			foreach (LedgerAggregateColumnMapClass columnMap in columnMaps)
			{
				columnMap.LedgerAggregateColumnGuid = columnGuid;
				this.Add(security, columnMap);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeCollection(SecurityClass security, Guid columnGuid)
		{
			// Purge existing
			LedgerAggregateColumnMapCollectionClass purgeCollection = this.Enumerate(security, columnGuid);

			foreach (LedgerAggregateColumnMapClass columnMap in purgeCollection)
			{
				this.Purge(security, columnMap);
			}
		}

		public LedgerAggregateColumnMapCollectionClass Enumerate(SecurityClass security, Guid ledgerAggregateColumnGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			LedgerAggregateColumnMapClass ColumnMap = new LedgerAggregateColumnMapClass();
			ColumnMap.LedgerAggregateColumnGuid = ledgerAggregateColumnGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				ColumnMap.Enumerate(cmd);
				DataSet Set = this.consolidatedDA.GetDataSet(cmd, security);

				LedgerAggregateColumnMapCollectionClass columnCollection = new LedgerAggregateColumnMapCollectionClass();

				DataTable Table = Set.Tables[0];

				while (Table.Rows.Count != 0)
				{
					ColumnMap = new LedgerAggregateColumnMapClass();
					ColumnMap.Load(Set);
					columnCollection.Add(ColumnMap);
					Table.Rows.RemoveAt(0);
				}

				return columnCollection;
			}
		}
	}
}
