// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomToolbarCommands.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomToolbarCommands type.
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
	/// Definition of the CustomToolbarCommands service class.  Provides a database interface for
	/// the CustomToolbarCommandClass type.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class CustomToolbarCommands : ICustomToolbarCommands
	{
		/// <summary>
		/// The consolidatedDA object provides database access
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomToolbarCommands"/> class.
		/// </summary>
		public CustomToolbarCommands()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}

		/// <summary>
		/// Adds a CustomToolbarCommandClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarCommand">The object to add to the database</param>
		/// <returns>The identity Guid of the added record</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, CustomToolbarCommandClass customToolbarCommand)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (customToolbarCommand == null)
			{
				throw new ArgumentNullException("customToolbarCommand");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			customToolbarCommand.CreatedDate = DateTimeOffset.Now;
			customToolbarCommand.CreatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				customToolbarCommand.IdentityGuid = Guid.NewGuid();
				customToolbarCommand.InsertSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			return customToolbarCommand.IdentityGuid;
		}

		/// <summary>
		/// Modifies an existing CustomToolbarCommandClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarCommand">The object to modify in the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, CustomToolbarCommandClass customToolbarCommand)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (customToolbarCommand == null)
			{
				throw new ArgumentNullException("customToolbarCommand");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			CustomToolbarCommandClass toolbarCommand = this.Get(security, customToolbarCommand.IdentityGuid);

			if (toolbarCommand.IdentityGuid == Guid.Empty)
			{
				throw new Exception("customToolbarCommand Not Found");
			}

			customToolbarCommand.UpdatedDate = DateTimeOffset.Now;
			customToolbarCommand.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				customToolbarCommand.UpdateSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Deletes an existing CustomToolbarCommandClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarCommandGuid">The identity Guid of the object to delete from the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid customToolbarCommandGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			CustomToolbarCommandClass toolbarCommand = this.Get(security, customToolbarCommandGuid);

			if (toolbarCommand.IdentityGuid == Guid.Empty)
			{
				return;
			}

			using (var cmd = new SqlCommand())
			{
				toolbarCommand.PurgeSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Gets an existing CustomToolbarCommandClass object from the database given the identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarCommandGuid">The identity Guid of the object to get from the database</param>
		/// <returns>The specified CustomToolbarCommandClass object</returns>
		public CustomToolbarCommandClass Get(SecurityClass security, Guid customToolbarCommandGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var toolbarCommand = new CustomToolbarCommandClass { IdentityGuid = customToolbarCommandGuid };
			using (var cmd = new SqlCommand())
			{
				toolbarCommand.SelectSql(cmd, ContextUtil.IsInTransaction);
				toolbarCommand.Load(this.consolidatedDa.GetDataSet(cmd, security));
			}

			return toolbarCommand;
		}

		/// <summary>
		/// Gets a list of CustomToolbarCommandClass objects from the database given the CustomToolbar identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarGuid">The asscoiated CustomToolbar identity Guid</param>
		/// <returns>The specified list of CustomToolbarCommandClass objects</returns>
		public CustomToolbarCommandCollectionClass Enumerate(SecurityClass security, Guid customToolbarGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var toolbarCommand = new CustomToolbarCommandClass { CustomToolbarGuid = customToolbarGuid };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				toolbarCommand.EnumerateSql(cmd, ContextUtil.IsInTransaction);
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			var toolbarCommandCollection = new CustomToolbarCommandCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				toolbarCommand = new CustomToolbarCommandClass();
				toolbarCommand.Load(set);

				toolbarCommandCollection.Add(toolbarCommand);

				table.Rows.RemoveAt(0);
			}

			return toolbarCommandCollection;
		}

		/// <summary>
		/// Gets a list of CustomToolbarCommandType objects from the database given the CustomToolbar type.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="toolbarType">The CustomToolbar type</param>
		/// <returns>The specified list of CustomToolbarCommandType objects</returns>
		public CustomToolbarCommandTypeList EnumerateCommandTypes(SecurityClass security, int toolbarType)
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
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT CustomToolbarCommandTypeIndex, CustomToolbarCommandTypeName, [Default], [DefaultOrder], ImageSource" +
								" FROM lookup.tblCustomToolbarCommandType" +
								" WHERE LookupCustomToolbarTypeIndex = @ToolbarTypeIndex" +
								" ORDER BY CustomToolbarCommandTypeName";

				cmd.Parameters.Add(new SqlParameter("@ToolbarTypeIndex", SqlDbType.Int){ Value = toolbarType});

				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			var commandTypeList = new CustomToolbarCommandTypeList();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				DataRow row = table.Rows[0];
				var commandType = new CustomToolbarCommandType();
				commandType.LookupIndex = DataObject.getValue<int>(row["CustomToolbarCommandTypeIndex"], CustomToolbarCommandType.UnknownCommandType);
				commandType.Id = DataObject.getValue<string>(row["CustomToolbarCommandTypeName"], string.Empty);
				commandType.IsDefault = DataObject.getValue<bool>(row["Default"], false);
				commandType.DefaultOrder = (row["DefaultOrder"] == DBNull.Value)? null:(int?)row["DefaultOrder"];
				commandType.ImageSource = DataObject.getValue<string>(row["ImageSource"], null);
				commandTypeList.Add(commandType);
				table.Rows.RemoveAt(0);
			}

			return commandTypeList;
		}


		/// <summary>
		/// Gets a list of CustomToolbarCommandType objects from the database given the CustomToolbar type.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="toolbarTypeIndex">The CustomToolbar type index</param>
		/// <returns>The specified list of CustomToolbarCommandType objects</returns>
		public CustomToolbarCommandTypeList EnumerateDefaultCommandTypes(SecurityClass security, int toolbarTypeIndex)
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
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT CustomToolbarCommandTypeIndex, CustomToolbarCommandTypeName, [Default], [DefaultOrder], ImageSource" +
								" FROM lookup.tblCustomToolbarCommandType" +
								" WHERE LookupCustomToolbarTypeIndex = @ToolbarTypeIndex" +
								" AND [Default] = 1 " +
								" ORDER BY DefaultOrder";

				cmd.Parameters.Add(new SqlParameter("@ToolbarTypeIndex", SqlDbType.Int) {Value = toolbarTypeIndex});

				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			var commandTypeList = new CustomToolbarCommandTypeList();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				DataRow row = table.Rows[0];
				var commandType = new CustomToolbarCommandType();
				commandType.LookupIndex = DataObject.getValue<int>(row["CustomToolbarCommandTypeIndex"], CustomToolbarCommandType.UnknownCommandType);
				commandType.Id = DataObject.getValue<string>(row["CustomToolbarCommandTypeName"], string.Empty);
				commandType.IsDefault = DataObject.getValue<bool>(row["Default"], false);
				commandType.DefaultOrder = (row["DefaultOrder"] == DBNull.Value) ? null : (int?)row["DefaultOrder"];
				commandType.ImageSource = DataObject.getValue<string>(row["ImageSource"], null);
				commandTypeList.Add(commandType);
				table.Rows.RemoveAt(0);
			}

			return commandTypeList;
		}

		/// <summary>
		/// Modify the list of CustomToolbarCommand objects asscoiated with a given CustomToolbar object.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarGuid">The asscoiated CustomToolbar identity Guid</param>
		/// <param name="customToolbarId">The asscoiated CustomToolbar ID</param>
		/// <param name="newCollection">The new list of CustomToolbarCommand objects</param>
		/// <param name="oldCollection">The old list of CustomToolbarCommand objects</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(
			SecurityClass security,
			Guid customToolbarGuid,
			string customToolbarId,
			CustomToolbarCommandCollectionClass newCollection,
			CustomToolbarCommandCollectionClass oldCollection)
		{
			if (newCollection != null)
			{
				foreach (CustomToolbarCommandClass newToolbarCommand in newCollection)
				{
					newToolbarCommand.CustomToolbarGuid = customToolbarGuid;
					newToolbarCommand.CustomToolbarId = customToolbarId;

					if (oldCollection != null)
					{
						int index = 0;

						foreach (CustomToolbarCommandClass oldToolbarCommand in oldCollection)
						{
							if (oldToolbarCommand.ToolbarCommandType == newToolbarCommand.ToolbarCommandType &&
								oldToolbarCommand.TransactionAliasGuid == newToolbarCommand.TransactionAliasGuid)
							{
								if (oldToolbarCommand.ColumnOrder != newToolbarCommand.ColumnOrder)
								{
									newToolbarCommand.IdentityGuid = oldToolbarCommand.IdentityGuid;
									this.Modify(security, newToolbarCommand);
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
							this.Add(security, newToolbarCommand);
						}
					}
					else
					{
						this.Add(security, newToolbarCommand);
					}
				}
			}

			if (oldCollection != null)
			{
				foreach (CustomToolbarCommandClass oldToolbarCommand in oldCollection)
				{
					this.Purge(security, oldToolbarCommand.IdentityGuid);
				}
			}
		}
	}
}