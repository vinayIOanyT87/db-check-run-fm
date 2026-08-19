// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchGridColumns.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchGridColumns type.
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
	/// Definition of the DispatchGridColumns service class.  Provides a database interface for
	/// the DispatchGridColumnClass type.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class DispatchGridColumns : IDispatchGridColumns
	{
		/// <summary>
		/// The consolidatedDA object provides database access
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa;

		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchGridColumns"/> class.
		/// </summary>
		public DispatchGridColumns()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}

		/// <summary>
		/// Adds a DispatchGridColumnClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridColumn">The object to add to the database</param>
		/// <returns>The identity Guid of the added record</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, DispatchGridColumnClass dispatchGridColumn)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dispatchGridColumn == null)
			{
				throw new ArgumentNullException("dispatchGridColumn");
			}

			if (!security.HasRight(RIGHT.MODIFY_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			dispatchGridColumn.CreatedDate = DateTimeOffset.Now;
			dispatchGridColumn.CreatedBy = security.UserID;
			dispatchGridColumn.UserGuid = security.UserGuid;

			using (var cmd = new SqlCommand())
			{
				dispatchGridColumn.IdentityGuid = Guid.NewGuid();
				dispatchGridColumn.InsertSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			return dispatchGridColumn.IdentityGuid;
		}

		/// <summary>
		/// Modifies an existing DispatchGridColumnClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridColumn">The object to modify in the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, DispatchGridColumnClass dispatchGridColumn)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dispatchGridColumn == null)
			{
				throw new ArgumentNullException("dispatchGridColumn");
			}

			if (!security.HasRight(RIGHT.MODIFY_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			DispatchGridColumnClass gridColumn = this.Get(security, dispatchGridColumn.IdentityGuid);

			if (gridColumn.IdentityGuid == Guid.Empty)
			{
				throw new Exception("dispatchGridColumn Not Found");
			}

			dispatchGridColumn.UpdatedDate = DateTimeOffset.Now;
			dispatchGridColumn.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				dispatchGridColumn.UpdateSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Deletes an existing DispatchGridColumnClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridColumnGuid">The identity Guid of the object to delete from the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid dispatchGridColumnGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			DispatchGridColumnClass dispatchGridColumn = this.Get(security, dispatchGridColumnGuid);

			if (dispatchGridColumn.IdentityGuid == Guid.Empty)
			{
				return;
			}

			using (var cmd = new SqlCommand())
			{
				dispatchGridColumn.PurgeSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByUser(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_USERS))
			{
				throw new FMInsufficientRightsException();
			}

			using (var cmd = new SqlCommand())
			{
				var dispatchColumn = new DispatchGridColumnClass();
				dispatchColumn.UserGuid = userGuid;
				dispatchColumn.PurgeSqlByUser(cmd);

				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Gets an existing DispatchGridColumnClass object from the database given the identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridColumnGuid">The identity Guid of the object to get from the database</param>
		/// <returns>The specified DispatchGridColumnClass object</returns>
		public DispatchGridColumnClass Get(SecurityClass security, Guid dispatchGridColumnGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var dispatchGridColumn = new DispatchGridColumnClass { IdentityGuid = dispatchGridColumnGuid };
			using (var cmd = new SqlCommand())
			{
				dispatchGridColumn.SelectSql(cmd, ContextUtil.IsInTransaction);
				dispatchGridColumn.Load(this.consolidatedDa.GetDataSet(cmd, security));
			}

			return dispatchGridColumn;
		}

		/// <summary>
		/// Gets a list of DispatchGridColumnClass objects from the database given the dispatch grid identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridGuid">The asscoiated dispatch grid identity Guid</param>
		/// <returns>The specified list of DispatchGridColumnClass objects</returns>
		public DispatchGridColumnCollectionClass Enumerate(SecurityClass security, Guid dispatchGridGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var dispatchGridColumn = new DispatchGridColumnClass { DispatchGridGuid = dispatchGridGuid, UserGuid = security.UserGuid };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				dispatchGridColumn.EnumerateSql(cmd, ContextUtil.IsInTransaction);
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			var dispatchGridColumnCollection = new DispatchGridColumnCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				dispatchGridColumn = new DispatchGridColumnClass();
				dispatchGridColumn.Load(set);

				dispatchGridColumnCollection.Add(dispatchGridColumn);

				table.Rows.RemoveAt(0);
			}

			return dispatchGridColumnCollection;
		}

		/// <summary>
		/// Gets the list of DispatchGridColumnType objects from the database given the dispatch grid type
		/// and the default order flag.  If the default order flag is true the columns are retrieved in
		/// default order.  Otherwise the columns are retrieved in alphabetical order.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="gridType">The dispatch grid type</param>
		/// <param name="defaultOrder">The default order flag</param>
		/// <returns>The list of DispatchGridColumnType objects</returns>
		public DispatchGridColumnTypeList EnumerateColumnTypes(SecurityClass security, int gridType, bool defaultOrder)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				string orderByColumn = defaultOrder ? "DefaultColumnOrder" : "DisplayName";
				cmd.CommandText = "SELECT DispatchGridColumnTypeIndex, ID, DisplayName, DataField, Width, DefaultColumnOrder" +
								" FROM lookup.tblDispatchGridColumnType" +
								" WHERE LookupDispatchGridTypeIndex = " + gridType.ToString() +
								" ORDER BY " + orderByColumn;

				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			var columnTypeList = new DispatchGridColumnTypeList();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				DataRow row = table.Rows[0];
				var columnType = new DispatchGridColumnType();
				columnType.LookupIndex = DataObject.getValue<int>(row["DispatchGridColumnTypeIndex"], DispatchGridColumnType.UnknownColumnType);
				columnType.Id = DataObject.getValue<string>(row["ID"], string.Empty);
				columnType.DisplayName = DataObject.getValue<string>(row["DisplayName"], string.Empty);
				columnType.DataField = DataObject.getValue<string>(row["DataField"], string.Empty);
				columnType.Width = DataObject.getValue<int>(row["Width"], 60);
				columnType.DefaultColumnOrder = DataObject.getValue<int>(row["DefaultColumnOrder"], -1);
				columnTypeList.Add(columnType);
				table.Rows.RemoveAt(0);
			}

			return columnTypeList;
		}

		/// <summary>
		/// Modify the list of DispatchGridColumnClass objects asscoiated with a given dispatch grid object.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridGuid">The asscoiated dispatch grid identity Guid</param>
		/// <param name="dispatchGridId">The asscoiated dispatch grid ID</param>
		/// <param name="newCollection">The new list of DispatchGridColumnClass objects</param>
		/// <param name="oldCollection">The old list of DispatchGridColumnClass objects</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(
			SecurityClass security,
			Guid dispatchGridGuid,
			string dispatchGridId,
			DispatchGridColumnCollectionClass newCollection,
			DispatchGridColumnCollectionClass oldCollection)
		{
			if (newCollection != null)
			{
				foreach (DispatchGridColumnClass newColumn in newCollection)
				{
					newColumn.DispatchGridGuid = dispatchGridGuid;
					newColumn.UserGuid = security.UserGuid;
					newColumn.DispatchGridId = dispatchGridId;

					if (oldCollection != null)
					{
						int index = 0;

						foreach (DispatchGridColumnClass oldColumn in oldCollection)
						{
							if (oldColumn.GridColumnType == newColumn.GridColumnType &&
								oldColumn.UserDataFieldTransactionAliasGuid == newColumn.UserDataFieldTransactionAliasGuid &&
								oldColumn.UserDataFieldTransactionAliasLineItemGuid == newColumn.UserDataFieldTransactionAliasLineItemGuid)
							{
								if (oldColumn.ColumnOrder != newColumn.ColumnOrder)
								{
									newColumn.IdentityGuid = oldColumn.IdentityGuid;
									this.Modify(security, newColumn);
								}

								break;
							}

							index++;
						}

						if (index < oldCollection.Count)
						{
							oldCollection.RemoveAt(index);
						}
						else
						{
							this.Add(security, newColumn);
						}
					}
					else
					{
						this.Add(security, newColumn);
					}
				}
			}

			if (oldCollection != null)
			{
				foreach (DispatchGridColumnClass oldColumn in oldCollection)
				{
					this.Purge(security, oldColumn.IdentityGuid);
				}
			}
		}
	}
}