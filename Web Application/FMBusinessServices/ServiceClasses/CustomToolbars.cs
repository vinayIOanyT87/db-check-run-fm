// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomToolbars.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomToolbars type.
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
	/// Definition of the CustomToolbars service class.  Provides a database interface for
	/// the CustomToolbarClass type.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class CustomToolbars : ICustomToolbars
	{
		/// <summary>
		/// The consolidatedDA object provides database access
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomToolbars"/> class.
		/// </summary>
		public CustomToolbars()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}

		/// <summary>
		/// Adds a CustomToolbarClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbar">The object to add to the database</param>
		/// <returns>The identity Guid of the added record</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, CustomToolbarClass customToolbar)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (customToolbar == null)
			{
				throw new ArgumentNullException("customToolbar");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			customToolbar.SiteGuid = security.SiteGuid;
			customToolbar.CreatedDate = DateTimeOffset.Now;
			customToolbar.CreatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				customToolbar.IdentityGuid = Guid.NewGuid();
				customToolbar.InsertSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			var customToolbarCommands = new CustomToolbarCommands();

			customToolbarCommands.ModifyCollection(security, customToolbar.IdentityGuid, customToolbar.ID, customToolbar.ToolbarCommandList, null);

			customToolbar = this.Get(security, customToolbar.IdentityGuid);

			return customToolbar.IdentityGuid;
		}

		/// <summary>
		///  Modifies an existing CustomToolbarClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbar">The object to modify in the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, CustomToolbarClass customToolbar)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (customToolbar == null)
			{
				throw new ArgumentNullException("customToolbar");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			Guid customToolbarGuid = this.GetIdentityGuidById(security, customToolbar.ID, customToolbar.DispatchConfigurationGuid);

			if (customToolbarGuid != Guid.Empty && customToolbarGuid != customToolbar.IdentityGuid)
			{
				throw new Exception("Custom Toolbar Exists");
			}

			CustomToolbarClass oldCustomToolbar = this.Get(security, customToolbar.IdentityGuid);

			if (oldCustomToolbar.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Custom Toolbar Not Found");
			}

			customToolbar.UpdatedDate = DateTimeOffset.Now;
			customToolbar.UpdatedBy = security.UserID;
			using (var cmd = new SqlCommand())
			{
				customToolbar.UpdateSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			var customToolbarCommands = new CustomToolbarCommands();
			customToolbarCommands.ModifyCollection(security, customToolbar.IdentityGuid, customToolbar.ID, customToolbar.ToolbarCommandList, oldCustomToolbar.ToolbarCommandList);
		}

		/// <summary>
		/// Deletes an existing CustomToolbarClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarGuid">The identity Guid of the object to delete from the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid customToolbarGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			CustomToolbarClass customToolbar = this.Get(security, customToolbarGuid);
			if (customToolbar.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Custom Toolbar Not Found");
			}

			var customToolbarCommands = new CustomToolbarCommands();
			customToolbarCommands.ModifyCollection(security, customToolbar.IdentityGuid, customToolbar.ID, null, customToolbar.ToolbarCommandList);

			using (var cmd = new SqlCommand())
			{
				customToolbar.PurgeSql(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Gets an existing CustomToolbarClass object from the database given the identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarGuid">The identity Guid of the object to get from the database</param>
		/// <returns>The specified CustomToolbarClass object</returns>
		public CustomToolbarClass Get(SecurityClass security, Guid customToolbarGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var customToolbar = new CustomToolbarClass { IdentityGuid = customToolbarGuid };

			using (var cmd = new SqlCommand())
			{
				customToolbar.SelectSql(cmd, ContextUtil.IsInTransaction);
				customToolbar.Load(this.consolidatedDa.GetDataSet(cmd, security));
			}

			var customToolbarCommands = new CustomToolbarCommands();
			customToolbar.ToolbarCommandList = customToolbarCommands.Enumerate(security, customToolbar.IdentityGuid);

			return customToolbar;
		}

		/// <summary>
		/// Gets the identity Guid of a CustomToolbarClass object from the database given the ID
		/// and associated dispatch configuration identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The ID of the CustomToolbarClass object</param>
		/// <param name="dispatchConfigurationGuid">The asscoiated dispatch configuration identity Guid</param>
		/// <returns>The identity Guid of the specified CustomToolbarClass object</returns>
		public Guid GetIdentityGuidById(SecurityClass security, string id, Guid dispatchConfigurationGuid)
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

			var customToolbar = new CustomToolbarClass { DispatchConfigurationGuid = dispatchConfigurationGuid, ID = id };
			using (var cmd = new SqlCommand())
			{
				customToolbar.SelectByIdsql(cmd, ContextUtil.IsInTransaction);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				customToolbar.Load(set);
			}

			return customToolbar.IdentityGuid;
		}

		/// <summary>
		/// Gets a list of CustomToolbarClass objects from the database given the associated
		/// dispatch configuration identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfigurationGuid">The asscoiated dispatch configuration identity Guid</param>
		/// <returns>The specified list of CustomToolbarClass objects</returns>
		public CustomToolbarCollectionClass Enumerate(SecurityClass security, Guid dispatchConfigurationGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var customToolbar = new CustomToolbarClass { DispatchConfigurationGuid = dispatchConfigurationGuid };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				customToolbar.EnumerateSql(security, cmd);
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			var customToolbarCollection = new CustomToolbarCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				customToolbar = new CustomToolbarClass();
				customToolbar.Load(set);
				customToolbarCollection.Add(customToolbar);
				table.Rows.RemoveAt(0);
			}

			return customToolbarCollection;
		}

		/// <summary>
		/// Gets a list of CustomToolbarType objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of CustomToolbarType objects</returns>
		public CustomToolbarTypeList EnumerateToolbarTypes(SecurityClass security)
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
				cmd.CommandText = "SELECT CustomToolbarTypeIndex, CustomToolbarTypeName" +
								" FROM lookup.tblCustomToolbarType" +
								" WHERE CustomToolbarTypeIndex != " + CustomToolbarType.UnknownToolbarType.ToString() +
								" ORDER BY CustomToolbarTypeName";

				set = this.consolidatedDa.GetDataSet(cmd, security);
			}

			var toolbarTypeList = new CustomToolbarTypeList();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				DataRow row = table.Rows[0];
				var toolbarType = new CustomToolbarType();
				toolbarType.LookupIndex = DataObject.getValue<int>(row["CustomToolbarTypeIndex"], CustomToolbarType.UnknownToolbarType);
				toolbarType.Id = DataObject.getValue<string>(row["CustomToolbarTypeName"], string.Empty);
				toolbarTypeList.Add(toolbarType);
				table.Rows.RemoveAt(0);
			}

			return toolbarTypeList;
		}


		/// <summary>
		/// Gets a list of CustomToolbarType objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The CustomToolbarTypeName to search for</param>
		/// <returns>The specified list of CustomToolbarType objects</returns>
		public CustomToolbarType EnumerateToolbarTypeById(SecurityClass security, string id)
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
				cmd.CommandText = "SELECT CustomToolbarTypeIndex, CustomToolbarTypeName" +
								" FROM lookup.tblCustomToolbarType" +
								" WHERE CustomToolbarTypeIndex != @CustomToolbarType " + 
								" AND CustomToolbarTypeName = @CustomToolbarTypeName "+
								" ORDER BY CustomToolbarTypeName";

				cmd.Parameters.Add(new SqlParameter("@CustomToolbarType", SqlDbType.Int) { Value = CustomToolbarType.UnknownToolbarType});
				cmd.Parameters.Add(new SqlParameter("@CustomToolbarTypeName", SqlDbType.NVarChar, 100) {Value = id});

				set = this.consolidatedDa.GetDataSet(cmd, security);
			}


			DataTable table = set.Tables[0];
			var toolbarType = new CustomToolbarType();
			if (table.Rows.Count != 0)
			{
				DataRow row = table.Rows[0];
				
				toolbarType.LookupIndex = DataObject.getValue<int>(row["CustomToolbarTypeIndex"], CustomToolbarType.UnknownToolbarType);
				toolbarType.Id = DataObject.getValue<string>(row["CustomToolbarTypeName"], string.Empty);
			}

			return toolbarType;
		}
	}
}