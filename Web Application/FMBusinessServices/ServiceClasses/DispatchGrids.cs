// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchGrids.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchGrids type.
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
	/// Definition of the DispatchGrids service class.  Provides a database interface for
	/// the DispatchGridClass type.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class DispatchGrids : IDispatchGrids
	{
		/// <summary>
		/// The consolidatedDA object provides database access
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa;

		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchGrids"/> class.
		/// </summary>
		public DispatchGrids()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}

		/// <summary>
		/// Adds a DispatchGridClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGrid">The object to add to the database</param>
		/// <returns>The identity Guid of the added record</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, DispatchGridClass dispatchGrid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dispatchGrid == null)
			{
				throw new ArgumentNullException("dispatchGrid");
			}

			if (!security.HasRight(RIGHT.MODIFY_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			dispatchGrid.SiteGuid = security.SiteGuid;
			dispatchGrid.CreatedDate = DateTimeOffset.Now;
			dispatchGrid.CreatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				dispatchGrid.IdentityGuid = Guid.NewGuid();
				dispatchGrid.InsertSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			var dispatchGridCommands = new DispatchGridColumns();

			dispatchGridCommands.ModifyCollection(security, dispatchGrid.IdentityGuid, dispatchGrid.ID, dispatchGrid.GridColumnList, null);

			dispatchGrid = this.Get(security, dispatchGrid.IdentityGuid);

			return dispatchGrid.IdentityGuid;
		}

		/// <summary>
		///  Modifies an existing DispatchGridClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGrid">The object to modify in the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, DispatchGridClass dispatchGrid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dispatchGrid == null)
			{
				throw new ArgumentNullException("dispatchGrid");
			}

			if (!security.HasRight(RIGHT.MODIFY_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			Guid dispatchGridGuid = this.GetIdentityGuidById(security, dispatchGrid.ID, dispatchGrid.DispatchConfigurationGuid);

			if (dispatchGridGuid != Guid.Empty && dispatchGridGuid != dispatchGrid.IdentityGuid)
			{
				throw new Exception("Dispatch Grid Exists");
			}

			DispatchGridClass oldDispatchGrid = this.Get(security, dispatchGrid.IdentityGuid);

			if (oldDispatchGrid.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Dispatch Grid Not Found");
			}

			dispatchGrid.UpdatedDate = DateTimeOffset.Now;
			dispatchGrid.UpdatedBy = security.UserID;
			using (var cmd = new SqlCommand())
			{
				dispatchGrid.UpdateSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			var dispatchGridCommands = new DispatchGridColumns();
			dispatchGridCommands.ModifyCollection(security, dispatchGrid.IdentityGuid, dispatchGrid.ID, dispatchGrid.GridColumnList, oldDispatchGrid.GridColumnList);
		}

		/// <summary>
		/// Deletes an existing DispatchGridClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridGuid">The identity Guid of the object to delete from the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid dispatchGridGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			DispatchGridClass dispatchGrid = this.Get(security, dispatchGridGuid);
			if (dispatchGrid.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Dispatch Grid Not Found");
			}

			var dispatchGridCommands = new DispatchGridColumns();
			dispatchGridCommands.ModifyCollection(security, dispatchGrid.IdentityGuid, dispatchGrid.ID, null, dispatchGrid.GridColumnList);

			using (var cmd = new SqlCommand())
			{
				dispatchGrid.PurgeSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Gets an existing DispatchGridClass object from the database given the identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridGuid">The identity Guid of the object to get from the database</param>
		/// <returns>The specified DispatchGridClass object</returns>
		public DispatchGridClass Get(SecurityClass security, Guid dispatchGridGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var dispatchGrid = new DispatchGridClass { IdentityGuid = dispatchGridGuid };

			using (var cmd = new SqlCommand())
			{
				dispatchGrid.SelectSql(cmd, ContextUtil.IsInTransaction);
				dispatchGrid.Load(this.consolidatedDa.GetDataSet(cmd, security));
			}

			var dispatchGridCommands = new DispatchGridColumns();
			dispatchGrid.GridColumnList = dispatchGridCommands.Enumerate(security, dispatchGrid.IdentityGuid);

			return dispatchGrid;
		}

		/// <summary>
		/// Gets the identity Guid of a DispatchGridClass object from the database given the ID
		/// and associated dispatch configuration identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The ID of the DispatchGridClass object</param>
		/// <param name="dispatchConfigurationGuid">The asscoiated dispatch configuration identity Guid</param>
		/// <returns>The identity Guid of the specified DispatchGridClass object</returns>
		public Guid GetIdentityGuidById(SecurityClass security, string id, Guid dispatchConfigurationGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var dispatchGrid = new DispatchGridClass { DispatchConfigurationGuid = dispatchConfigurationGuid, ID = id };
			using (var cmd = new SqlCommand())
			{
				dispatchGrid.SelectByIdSql(cmd, ContextUtil.IsInTransaction);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				dispatchGrid.Load(set);
			}

			return dispatchGrid.IdentityGuid;
		}

		/// <summary>
		/// Gets a list of DispatchGridClass objects from the database given the associated
		/// dispatch configuration identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfigurationGuid">The asscoiated dispatch configuration identity Guid</param>
		/// <returns>The specified list of DispatchGridClass objects</returns>
		public DispatchGridCollectionClass Enumerate(SecurityClass security, Guid dispatchConfigurationGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var dispatchGrid = new DispatchGridClass { DispatchConfigurationGuid = dispatchConfigurationGuid };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				dispatchGrid.EnumerateSql(security, cmd);
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			var dispatchGridCollection = new DispatchGridCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				dispatchGrid = new DispatchGridClass();
				dispatchGrid.Load(set);
				dispatchGridCollection.Add(dispatchGrid);
				table.Rows.RemoveAt(0);
			}

			return dispatchGridCollection;
		}

		/// <summary>
		/// Gets a list of DispatchGridType objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of DispatchGridType objects</returns>
		public DispatchGridTypeList EnumerateGridTypes(SecurityClass security)
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
				cmd.CommandText = "SELECT DispatchGridTypeIndex, DispatchGridTypeName" +
								" FROM lookup.tblDispatchGridType" +
								" WHERE DispatchGridTypeIndex != " + DispatchGridType.UnknownGridType.ToString() +
								" ORDER BY DispatchGridTypeName";

				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			var toolbarTypeList = new DispatchGridTypeList();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				DataRow row = table.Rows[0];
				var toolbarType = new DispatchGridType();
				toolbarType.LookupIndex = DataObject.getValue<int>(row["DispatchGridTypeIndex"], DispatchGridType.UnknownGridType);
				toolbarType.Id = DataObject.getValue<string>(row["DispatchGridTypeName"], string.Empty);
				toolbarTypeList.Add(toolbarType);
				table.Rows.RemoveAt(0);
			}

			return toolbarTypeList;
		}
	}
}