// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliases.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for TransactionAliasesClass.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
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
	/// Summary description for TransactionAliasesClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TransactionAliasesClass : ITransactionAliases, IDependency
	{
		/// <summary>
		/// The consolidated data layer.
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

	    /// <summary>
		/// The validate.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionAlias">
		/// The transaction alias.
		/// </param>
		/// <exception cref="Exception">
		/// </exception>
		/// <exception cref="ApplicationException">
		/// </exception>
		/// <exception cref="ArgumentException">
		/// </exception>
		private void Validate(SecurityClass security, TransactionAliasClass transactionAlias)
		{
			if (transactionAlias.ID == string.Empty)
			{
				throw new Exception("ID Required");
			}

			if (transactionAlias.ID == "{None}" 
				|| transactionAlias.ID == "{Unassigned}"
				|| transactionAlias.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + transactionAlias.ID);
			}

			// There cannot be an aggregate column and alias with the same ID, or else errors
			// occur displaying ledger
			var aggCols = new LedgerAggregateColumnsClass();
			Guid guid = aggCols.GetIdentityGuid(security, transactionAlias.ID);

			if (guid != Guid.Empty)
			{
				throw new ApplicationException("ID matches existing Ledger Aggregate Column ID");
			}

			// Don't let the user save a transaction with both associated transaction methods selected
			if (transactionAlias.AssociatedTransactionAliasGuid != Guid.Empty
				&& transactionAlias.AssociatedAliases.Count > 0)
			{
				throw new ArgumentException("Associated transactions and a general associated transaction cannot be saved in the same alias.");
			}
		}

		/// <summary>
		/// The update user data fields.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="newTransactionAlias">
		/// The new transaction alias.
		/// </param>
		/// <param name="oldTransactionAlias">
		/// The old transaction alias.
		/// </param>
		private void UpdateUserDataFields(SecurityClass security,
										TransactionAliasClass newTransactionAlias,
										TransactionAliasClass oldTransactionAlias)
		{
			const int UserDataType = 1;
			const int LineItemType = 2;
			const int DispatchUserDataType = 3;
			const int DispatchLineItemType = 4;

			IUserDataFields userDataFieldIfc = new UserDataFieldsClass();
			if (newTransactionAlias != null)
			{
				// Perform the same logic on each type of user data field collection
				for (int typeIndex = UserDataType; typeIndex <= DispatchLineItemType; typeIndex++)
				{
					UserDataFieldCollectionClass newCollection;
					if (typeIndex == UserDataType)
					{
						newCollection = newTransactionAlias.UserDataFieldCollection;
					}
					else if (typeIndex == LineItemType)
					{
						newCollection = newTransactionAlias.LineItemUserDataFieldCollection;
					}
					else if (typeIndex == DispatchUserDataType)
					{
						newCollection = newTransactionAlias.DispatchUserDataFields;
					}
					else
					{
						// (typeIndex == DispatchLineItemType)
						newCollection = newTransactionAlias.DispatchLineItemUserDataFields;
					}

					foreach (var fieldClass in newCollection)
					{
						var newField = (UserDataFieldClass)fieldClass;
						newField.TransactionAliasGuid = newTransactionAlias.IdentityGuid;
						newField.SiteGuid = newTransactionAlias.SiteGuid;

						if (oldTransactionAlias != null)
						{
							int collectionIndex = 0;

							// Do for both user and line item data fields collections
							UserDataFieldCollectionClass oldCollection;

							if (typeIndex == UserDataType)
							{
								oldCollection = oldTransactionAlias.UserDataFieldCollection;
							}
							else if (typeIndex == LineItemType)
							{
								oldCollection = oldTransactionAlias.LineItemUserDataFieldCollection;
							}
							else if (typeIndex == DispatchUserDataType)
							{
								oldCollection = oldTransactionAlias.DispatchUserDataFields;
							}
							else
							{
								// (typeIndex == DispatchLineItemType)
								oldCollection = oldTransactionAlias.DispatchLineItemUserDataFields;
							}

							foreach (var fieldClass1 in oldCollection)
							{
								var oldField = (UserDataFieldClass)fieldClass1;
								if (oldField.Number == newField.Number)
								{
									newField.IdentityGuid = oldField.IdentityGuid;
									userDataFieldIfc.Modify(security, newField);
									break;
								}

								collectionIndex++;
							}

							if (collectionIndex < oldCollection.Count)
							{
								oldCollection.Remove(collectionIndex);
							}
							else
							{
								userDataFieldIfc.Add(security, newField);
							}
						}
						else
						{
							userDataFieldIfc.Add(security, newField);
						}
					}
				}
			}

			if (oldTransactionAlias != null)
			{
				// Perform the same logic on each type of user data field collection
				foreach (var fieldClass in oldTransactionAlias.UserDataFieldCollection)
				{
					var oldField = (UserDataFieldClass)fieldClass;
					userDataFieldIfc.Purge(security, oldField.IdentityGuid, oldField.UserDataEntityType);
				}

				foreach (var fieldClass in oldTransactionAlias.LineItemUserDataFieldCollection)
				{
					var oldField = (UserDataFieldClass)fieldClass;
					userDataFieldIfc.Purge(security, oldField.IdentityGuid, oldField.UserDataEntityType);
				}

				foreach (var fieldClass in oldTransactionAlias.DispatchUserDataFields)
				{
					var oldField = (UserDataFieldClass)fieldClass;
					userDataFieldIfc.Purge(security, oldField.IdentityGuid, oldField.UserDataEntityType);
				}

				foreach (var fieldClass in oldTransactionAlias.DispatchLineItemUserDataFields)
				{
					var oldField = (UserDataFieldClass)fieldClass;
					userDataFieldIfc.Purge(security, oldField.IdentityGuid, oldField.UserDataEntityType);
				}
			}
		}

		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionAlias">
		/// The transaction alias.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		/// <exception cref="Exception">
		/// Access exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TransactionAliasClass transactionAlias)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (transactionAlias == null)
			{
				throw new ArgumentNullException(nameof(transactionAlias));
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, transactionAlias);
			this.ValidateDefaultStatus(transactionAlias);

			if (this.GetIdentityGuid(security, transactionAlias.ID) != Guid.Empty)
			{
				throw new Exception("TransactionAlias Exists");
			}

			transactionAlias.SiteGuid = security.SiteGuid;
			transactionAlias.CreatedDate = DateTimeOffset.Now;
			transactionAlias.CreatedBy = security.UserID;
			transactionAlias.UpdatedDate = transactionAlias.CreatedDate;
			transactionAlias.UpdatedBy = security.UserID;
			transactionAlias.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				transactionAlias.InsertSQL(cmd);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}

			// vt - Add the statuses assigned to the aliase
			foreach (object obj in transactionAlias.AssignedStatuses)
			{
				int status = (int)obj;
				using (var cmd = new SqlCommand())
				{
					transactionAlias.GetInsertAvailableStatusSQL(cmd, status, security);
					this.ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}

			// vthompson 8/19/2008
			// Add the associated aliases
			foreach (TransactionAliasClass alias in transactionAlias.AssociatedAliases)
			{
				using (var cmd = new SqlCommand())
				{
					transactionAlias.GetInsertAssociatedAliasesSQL(cmd, alias, security);
					this.ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(transactionAlias);
			entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

			var transactionAliasFields = new TransactionAliasFieldsClass();
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.TransactionFieldCollection, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.LineItemFieldCollection, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.WeightReadingFieldCollection, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.NoteFieldCollection, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.TransportLineItemFieldCollection, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.ExportResultDetailFieldCollection, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchTransactionFields, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchLineItemFields, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchWeightReadingFields, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchNoteFields, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchTransportLineItemFields, null);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchExportResultDetailFields, null);

			this.UpdateUserDataFields(security, transactionAlias, null);

			var productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, false, transactionAlias.ExcludedProductCollection, null);

			// Create Alias to Security Group Map (IGO 2009-Sep-09)
			var groupTransactionAliasMaps = new GroupTransactionAliasMapsClass();

			foreach (GroupTransactionAliasMapClass groupTransactionAliasMap in transactionAlias.GroupTransactionAliasMapCollection)
			{
				// Update the alias id to the newly created one (IGO 2009-Sep-17)
				groupTransactionAliasMap.TransactionAliasGuid = transactionAlias.IdentityGuid;
				groupTransactionAliasMaps.Add(security, groupTransactionAliasMap);
			}

			return transactionAlias.IdentityGuid;
		}

		/// <summary>
		/// The modify.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionAlias">
		/// The transaction alias.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TransactionAliasClass transactionAlias)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (transactionAlias == null)
			{
				throw new ArgumentNullException(nameof(transactionAlias));
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, transactionAlias);
			this.ValidateDefaultStatus(transactionAlias);

			Guid existingIdentityGuid = this.GetIdentityGuid(security, transactionAlias.ID);

			if (existingIdentityGuid != Guid.Empty && existingIdentityGuid != transactionAlias.IdentityGuid)
			{
				throw new Exception("TransactionAlias Exists");
			}

			TransactionAliasClass oldTransactionAlias = this.Get(security, transactionAlias.IdentityGuid, false);

			if (oldTransactionAlias.IdentityGuid == Guid.Empty)
			{
				throw new Exception("TransactionAlias Not Found");
			}

			transactionAlias.UpdatedDate = DateTimeOffset.Now;
			transactionAlias.UpdatedBy = security.UserID;

			var entityToSiteMaps = new EntityToSiteMaps();

			if (transactionAlias.SiteGuid != oldTransactionAlias.SiteGuid)
			{
				entityToSiteMaps.PurgeAll(security, ENTITY_TYPE.TRANSACTION_ALIAS, transactionAlias.MasterRecordGuid);
			}

			using (var cmd = new SqlCommand())
			{
				transactionAlias.UpdateSQL(cmd);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}

			// Iterate through the old assigned statuses and delete them
			foreach (object obj in oldTransactionAlias.AssignedStatuses)
			{
				if (transactionAlias.AssignedStatuses.Contains(obj))
				{
					continue;
				}

				var status = (int)obj;
				using (var cmd = new SqlCommand())
				{
					transactionAlias.GetDeleteAvailableStatusSQL(cmd, status, security);
					this.ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}

			// Iterate through the assigned statuses and add them
			foreach (object obj in transactionAlias.AssignedStatuses)
			{
				if (oldTransactionAlias.AssignedStatuses.Contains(obj))
				{
					continue;
				}

				var status = (int)obj;
				using (var cmd = new SqlCommand())
				{
					transactionAlias.GetInsertAvailableStatusSQL(cmd, status, security);
					this.ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}

			// vthompson 8/19/2008
			// remove the associated statuses
			using (var cmd = new SqlCommand())
			{
				transactionAlias.DeleteAssociatedAliasesSQL(cmd);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}

			// Add the associated statuses
			foreach (TransactionAliasClass alias in transactionAlias.AssociatedAliases)
			{
				using (var cmd = new SqlCommand())
				{
					transactionAlias.GetInsertAssociatedAliasesSQL(cmd, alias, security);
					this.ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}

			if (transactionAlias.SiteGuid != oldTransactionAlias.SiteGuid)
			{
				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(transactionAlias);
				Guid currentSiteContext = security.SiteGuid;

				// When changing ownership of an entity that supports Cascading Assignment, need to make sure that the base mapping is created with the AssignedFromSiteGuid being the same as the Owner Site Guid (and the AssignedToSiteGuid), and not be set with the Site Context Guid which in the case of a Change of Ownership would be different from the new Owner Site Guid.
				// The Security SiteGuid swap below effectively does so by supplying the EntityToSiteMaps.Add() operation with the correct SiteGuid to use to set the AssignedFromSiteGuid.
				security.SiteGuid = transactionAlias.SiteGuid;
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
				security.SiteGuid = currentSiteContext;
			}

			var transactionAliasFields = new TransactionAliasFieldsClass();
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.TransactionFieldCollection, oldTransactionAlias.TransactionFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.LineItemFieldCollection, oldTransactionAlias.LineItemFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.WeightReadingFieldCollection, oldTransactionAlias.WeightReadingFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.NoteFieldCollection, oldTransactionAlias.NoteFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.TransportLineItemFieldCollection, oldTransactionAlias.TransportLineItemFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.ExportResultDetailFieldCollection, oldTransactionAlias.ExportResultDetailFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchTransactionFields, oldTransactionAlias.DispatchTransactionFields);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchLineItemFields, oldTransactionAlias.DispatchLineItemFields);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchWeightReadingFields, oldTransactionAlias.DispatchWeightReadingFields);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchNoteFields, oldTransactionAlias.DispatchNoteFields);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchTransportLineItemFields, oldTransactionAlias.DispatchTransportLineItemFields);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, transactionAlias.DispatchExportResultDetailFields, oldTransactionAlias.DispatchExportResultDetailFields);

			this.UpdateUserDataFields(security, transactionAlias, oldTransactionAlias);

			var productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, false, transactionAlias.ExcludedProductCollection, oldTransactionAlias.ExcludedProductCollection);

			// Modify Alias to Security Group Map (IGO 2009-Sep-09)
			var groupTransactionAliasMaps = new GroupTransactionAliasMapsClass();
			groupTransactionAliasMaps.ModifyCollection(security, transactionAlias.GroupTransactionAliasMapCollection, oldTransactionAlias.GroupTransactionAliasMapCollection);

			this.PropagateUpdate(security, transactionAlias);
			ChangeQueueRecordsClass.ProcessChangeQueueRecords(security, ChangeQueueEventType.Modify, transactionAlias);
		}

		/// <summary>
		/// Propagates the latest updates made to a TransactionAlias record to its child record versions.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionAlias">
		/// The transaction alias.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		public void PropagateUpdate(SecurityClass security, TransactionAliasClass transactionAlias)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "erv.usp_PropagateTransactionAliasRevisionByEntityRecordChange";
				cmd.Parameters.Add("@SourceTransactionAliasGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SourceTransactionAliasGuid"].Value = transactionAlias.IdentityGuid;
				this.ConsolidatedDa.ExecuteQuery(security, cmd);

                // Next, enqueue a replication of global changes up to a master record version.
                // if the change was made to a child record.
                if (transactionAlias.IdentityGuid != transactionAlias.MasterRecordGuid)
                {
                    cmd.CommandText = "erv.usp_AddGlobalSpecificQueueRecord";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
                    cmd.Parameters["@EntityTypeId"].Value = TransactionAliasClass.ENTITY_TYPE_ID;
                    cmd.Parameters.Add("@EntityGuid", SqlDbType.UniqueIdentifier);
                    cmd.Parameters["@EntityGuid"].Value = transactionAlias.IdentityGuid;
                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 100);
                    cmd.Parameters["@UserId"].Value = security.UserID;
                    this.ConsolidatedDa.ExecuteQuery(security, cmd);
                }
            }
        }

		/// <summary>
		/// This function checks to see that the default status selection exists in the
		/// list of assigned statuses.  If not, it sets the default to the non-selection.
		/// </summary>
		/// <param name="alias">
		/// The alias.
		/// </param>
		private void ValidateDefaultStatus(TransactionAliasClass alias)
		{
			foreach (int value in alias.AssignedStatuses)
			{
				if (value == alias.LookupDefaultStatusIndex)
				{
					return;
				}
			}

			// Set to non-selection since the 
			alias.LookupDefaultStatusIndex = -1;
		}

		/// <summary>
		/// The get basic info.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionAliasGuid">
		/// The transaction alias GUID.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		public TransactionAliasClass GetBasicInfo(SecurityClass security, Guid transactionAliasGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetTransactionAliasByGuid";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@TransactionAliasGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = siteGuid;
				cmd.Parameters["@TransactionAliasGuid"].Value = transactionAliasGuid;
				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			if (set.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			DataRow row = set.Tables[0].Rows[0];
			var transAlias = new TransactionAliasClass
								 {
									 IdentityGuid = DataObject.getValue(row["TransactionAliasGuid"], Guid.Empty),
									 MasterRecordGuid = DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
									 ID = DataObject.getValue(row["AliasName"], string.Empty),
									 SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty)
								 };

			return transAlias;
		}

		public bool UserHasModifyPermissions(SecurityClass security, Guid transactionAliasGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			bool toRet = false;

			object result;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT dbo.udf_IsUserGroupAssignedToModifyTransactionAlias(@SiteGuid,@UserGuid, @AliasGuid)";
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@AliasGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@AliasGuid"].Value = transactionAliasGuid;
				cmd.Parameters["@UserGuid"].Value = security.UserGuid;
				result = this.ConsolidatedDa.ExecuteScalar(cmd, security);
			}

			if (result != null)
			{
				toRet = (bool)result;
			}

			return toRet;
		}

		/// <summary>
		/// The get without alias fields.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="aliasGuid">
		/// The alias GUID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public TransactionAliasClass GetWithoutAliasFields(SecurityClass security, Guid aliasGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_QUERIES) 
				&& !security.HasRight(RIGHT.MODIFY_QUERIES)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var transactionAlias = new TransactionAliasClass { IdentityGuid = aliasGuid };
			DataSet set;

			using ( var cmd = new SqlCommand( ) )
			{
				// Company.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetTransactionAliasByGuid";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@TransactionAliasGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@TransactionAliasGuid"].Value = aliasGuid;
				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			transactionAlias.Load(set);

			using ( var command = new SqlCommand( ) )
			{
				// Populate the transaction statuses associated with the transaction alias
				transactionAlias.SelectAssignedStatusesSQL(command, ContextUtil.IsInTransaction, security);
				transactionAlias.LoadAssignedStatuses(this.ConsolidatedDa.GetDataSet(command, security));
			}

			// vthompson 8/20/2008
			// Populate the associated aliases
			this.PopulateAssociatedAliases(security, ref transactionAlias);

			return transactionAlias;
		}

		/// <summary>
		/// The get.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="identityGuid">
		/// The identity GUID.
		/// </param>
		/// <param name="byUser">
		/// The by user.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public TransactionAliasClass Get(SecurityClass security, Guid identityGuid, bool byUser)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_QUERIES) 
				&& !security.HasRight(RIGHT.MODIFY_QUERIES)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION) 
				&& !security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION))
			{
				throw new FMInsufficientRightsException();
			}

			var transactionAlias = new TransactionAliasClass { IdentityGuid = identityGuid };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				// Company.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetTransactionAliasByGuid";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@TransactionAliasGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@TransactionAliasGuid"].Value = identityGuid;
				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			transactionAlias.Load(set);

			// vt - Populate the transaction statuses associated with the transaction alias
			using (var cmd = new SqlCommand())
			{
				transactionAlias.SelectAssignedStatusesSQL(cmd, ContextUtil.IsInTransaction, security);
				transactionAlias.LoadAssignedStatuses(this.ConsolidatedDa.GetDataSet(cmd, security));
			}

			// vthompson 8/20/2008
			// Populate the associated aliases
			this.PopulateAssociatedAliases(security, ref transactionAlias);

			var transactionAliasFields = new TransactionAliasFieldsClass();

			TransactionAliasFieldCollectionClass allFields = transactionAliasFields.EnumerateByAliasGuid(security, transactionAlias.IdentityGuid, byUser);

			foreach (var fieldClass in allFields)
			{
				var aliasField = (TransactionAliasFieldClass)fieldClass;

				switch (aliasField.Type)
				{
					case TransactionFieldType.Transaction:
						if (aliasField.DispatchField)
						{
							 transactionAlias.DispatchTransactionFields.Add(aliasField);
						}
						else
						{
							transactionAlias.TransactionFieldCollection.Add(aliasField);
						}
						break;
					case TransactionFieldType.LineItem:
						if (aliasField.DispatchField)
						{
							transactionAlias.DispatchLineItemFields.Add(aliasField);
						}
						else
						{
							transactionAlias.LineItemFieldCollection.Add(aliasField);
						}
						break;
					case TransactionFieldType.WeightReading:
						if (aliasField.DispatchField)
						{
							transactionAlias.DispatchWeightReadingFields.Add(aliasField);
						}
						else
						{
							transactionAlias.WeightReadingFieldCollection.Add(aliasField);							
						}
						break;
					case TransactionFieldType.Note:
						if (aliasField.DispatchField)
						{
							transactionAlias.DispatchNoteFields.Add(aliasField);
						}
						else
						{
							transactionAlias.NoteFieldCollection.Add(aliasField);
						}
						break;
					case TransactionFieldType.TransportInfo:
						if (aliasField.DispatchField)
						{
							transactionAlias.DispatchTransportLineItemFields.Add(aliasField);
						}
						else
						{
							transactionAlias.TransportLineItemFieldCollection.Add(aliasField);
						}
						break;
					case TransactionFieldType.ExportResult:
						if (aliasField.DispatchField)
						{
							transactionAlias.DispatchExportResultDetailFields.Add(aliasField);
						}
						else
						{
							transactionAlias.ExportResultDetailFieldCollection.Add(aliasField);
						}
						break;
				}
			}

			var userDataFields = new UserDataFieldsClass();
			transactionAlias.UserDataFieldCollection =
				userDataFields.EnumerateByEntityType(security, ENTITY_TYPE.TRANSACTION_ALIAS, transactionAlias.IdentityGuid, byUser, false);
			transactionAlias.LineItemUserDataFieldCollection =
				userDataFields.EnumerateByEntityType(security, ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM, transactionAlias.IdentityGuid, byUser, false);

			transactionAlias.DispatchUserDataFields =
				userDataFields.EnumerateByEntityType(security, ENTITY_TYPE.TRANSACTION_ALIAS, transactionAlias.IdentityGuid, byUser, true);
			transactionAlias.DispatchLineItemUserDataFields =
				userDataFields.EnumerateByEntityType(security, ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM, transactionAlias.IdentityGuid, byUser, true);

			if (transactionAlias.TransTypeID == TransactionTypes.T11_ConsumerTransfer)
			{
				bool loopAgain = true;

				while (loopAgain)
				{
					loopAgain = false;

					foreach (var fieldClass in transactionAlias.TransactionFieldCollection)
					{
						var field = (TransactionAliasFieldClass)fieldClass;
						if (field.DbName == "BillToID" || field.DbName == "ShipToID")
						{
							// Note : DisplayOrder resequences the order and is called here to
							// insure the order is sequential.  It was observed to be non-sequential
							// indicating a defect
							FieldClass[] fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);

							var toCompanyField = new TransactionAliasFieldClass { DbName = "To" + field.DbName };

							field.DbName = "From" + field.DbName;
							toCompanyField.DisplayName = "To " + field.DisplayName;
							field.DisplayName = "From " + field.DisplayName;
							transactionAliasFields.Modify(security, field);
							toCompanyField.Type = TransactionFieldType.Transaction;
							toCompanyField.TransactionAliasGuid = field.TransactionAliasGuid;
							toCompanyField.DisplayOrder = field.DisplayOrder + 1;

							for (int i = field.DisplayOrder + 1; i < fields.Length; i++)
							{
								fields[i].DisplayOrder++;
							    var transactionAliasField = fields[i] as TransactionAliasFieldClass;
							    if (transactionAliasField != null)
								{
									transactionAliasFields.Modify(security, transactionAliasField);
								}
								else
								{
									userDataFields.Modify(security, fields[i] as UserDataFieldClass);
								}
							}

							transactionAlias.TransactionFieldCollection.Add(toCompanyField);
							transactionAliasFields.Add(security, toCompanyField);

							loopAgain = true;
							break;
						}
					}
				}
			}

			if (transactionAlias.TransTypeID == TransactionTypes.T13_OwnerTransfer)
			{
				bool loopAgain = true;

				while (loopAgain)
				{
					loopAgain = false;

					foreach (var fieldClass in transactionAlias.TransactionFieldCollection)
					{
						var field = (TransactionAliasFieldClass)fieldClass;
						if (field.DbName == "ManagerID" || field.DbName == "OwnerID" || field.DbName == "CarrierID")
						{
							// Note : DisplayOrder resequences the order and is called here to
							// insure the order is sequential.  It was observed to be non-sequential
							// indicating a defect
							FieldClass[] fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);

							var toCompanyField = new TransactionAliasFieldClass { DbName = "To" + field.DbName };

							field.DbName = "From" + field.DbName;
							toCompanyField.DisplayName = "To " + field.DisplayName;
							field.DisplayName = "From " + field.DisplayName;
							transactionAliasFields.Modify(security, field);
							toCompanyField.Type = TransactionFieldType.Transaction;
							toCompanyField.TransactionAliasGuid = field.TransactionAliasGuid;
							toCompanyField.DisplayOrder = field.DisplayOrder + 1;

							for (int i = field.DisplayOrder + 1; i < fields.Length; i++)
							{
								fields[i].DisplayOrder++;
							    var transactionAliasField = fields[i] as TransactionAliasFieldClass;
							    if (transactionAliasField != null)
								{
									transactionAliasFields.Modify(security, transactionAliasField);
								}
								else
								{
									userDataFields.Modify(security, fields[i] as UserDataFieldClass);
								}
							}

							transactionAlias.TransactionFieldCollection.Add(toCompanyField);
						    try
						    {
						        transactionAliasFields.Modify(security, toCompanyField);
						    }
						    catch (Exception e)
						    {
						        System.Diagnostics.Debug.Write(e.Message);
						        transactionAliasFields.Add(security, toCompanyField);
						    }

						    loopAgain = true;
							break;
						}
					}
				}
			}


			if (transactionAlias.TransTypeID == TransactionTypes.T15_PrimaryRegrade
				|| transactionAlias.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
			{
				foreach (var fieldClass in transactionAlias.LineItemFieldCollection)
				{
					var field = (TransactionAliasFieldClass)fieldClass;
					if (field.DbName == "Product")
					{
						// Note : DisplayOrder resequences the order and is called here to
						// insure the order is sequential.  It was observed to be non-sequential
						// indicating a defect
						FieldClass[] fields = transactionAlias.DisplayOrder(transactionAlias.MultipleLineItems ? TRANSACTION_SECTION_TYPE.LINE_ITEMS : TRANSACTION_SECTION_TYPE.BODY);

						var toProductField = new TransactionAliasFieldClass { DbName = "To" + field.DbName };

						field.DbName = "From" + field.DbName;
						toProductField.DisplayName = "To " + field.DisplayName;
						field.DisplayName = "From " + field.DisplayName;
						transactionAliasFields.Modify(security, field);
						toProductField.Type = TransactionFieldType.LineItem;
						toProductField.TransactionAliasGuid = field.TransactionAliasGuid;
						toProductField.DisplayOrder = field.DisplayOrder + 1;

						for (int i = field.DisplayOrder + 1; i < fields.Length; i++)
						{
							fields[i].DisplayOrder++;
						    var transactionAliasField = fields[i] as TransactionAliasFieldClass;
						    if (transactionAliasField != null)
							{
								transactionAliasFields.Modify(security, transactionAliasField);
							}
							else
							{
								userDataFields.Modify(security, fields[i] as UserDataFieldClass);
							}
						}

						transactionAlias.LineItemFieldCollection.Add(toProductField);
						transactionAliasFields.Add(security, toProductField);
						break;
					}
				}
			}

			if (transactionAlias.TransTypeID == TransactionTypes.T23_StorageTransfer)
			{
				foreach (var fieldClass in transactionAlias.LineItemFieldCollection)
				{
					var field = (TransactionAliasFieldClass)fieldClass;
					if (field.DbName == "StorageLocationID")
					{
						// Note : DisplayOrder resequences the order and is called here to
						// insure the order is sequential.  It was observed to be non-sequential
						// indicating a defect
						FieldClass[] fields = transactionAlias.DisplayOrder(transactionAlias.MultipleLineItems ? TRANSACTION_SECTION_TYPE.LINE_ITEMS : TRANSACTION_SECTION_TYPE.BODY);

						var toProductField = new TransactionAliasFieldClass { DbName = "To" + field.DbName };

						field.DbName = "From" + field.DbName;
						toProductField.DisplayName = "To " + field.DisplayName;
						field.DisplayName = "From " + field.DisplayName;
						transactionAliasFields.Modify(security, field);
						toProductField.Type = TransactionFieldType.LineItem;
						toProductField.TransactionAliasGuid = field.TransactionAliasGuid;
						toProductField.DisplayOrder = field.DisplayOrder + 1;


						for (int i = field.DisplayOrder + 1; i < fields.Length; i++)
						{
							fields[i].DisplayOrder++;

						    var transactionAliasField = fields[i] as TransactionAliasFieldClass;
						    if (transactionAliasField != null)
							{
								transactionAliasFields.Modify(security, transactionAliasField);
							}
							else
							{
								userDataFields.Modify(security, fields[i] as UserDataFieldClass);
							}
						}

						transactionAlias.LineItemFieldCollection.Add(toProductField);
						transactionAliasFields.Add(security, toProductField);

						break;
					}
				}
			}

			var productMaps = new ProductMapsClass();
			transactionAlias.ExcludedProductCollection = productMaps.EnumerateByAssignedToGuidAndTypeInstr(security, transactionAlias.IdentityGuid, PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP, false);

			// Get Alias to Security Group Map (IGO 2009-Sep-09)
			var groupTransactionAliasMaps = new GroupTransactionAliasMapsClass();
			transactionAlias.GroupTransactionAliasMapCollection = groupTransactionAliasMaps.EnumerateByTransactionAliasGuid(security, transactionAlias.IdentityGuid);

			return transactionAlias;
		}

		/// <summary>
		/// The get master record GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="id">
		/// The ID.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		public Guid GetMasterRecordGuid(SecurityClass security, string id)
		{
			Guid result = Guid.Empty;
			TransactionAliasClass transactionAlias = this.GetById(security, id);

			if (transactionAlias != null)
			{
				result = transactionAlias.MasterRecordGuid;
			}

			return result;
		}

		/// <summary>
		/// The get identity GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="id">
		/// The ID.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.VIEW_QUERIES)
				&& !security.HasRight(RIGHT.MODIFY_QUERIES)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var transactionAlias = new TransactionAliasClass { SiteGuid = security.SiteGuid, ID = id };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				// transactionAlias.SelectIdentityGuidSQL(cmd, security, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetTransactionAliasesById";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@AliasName", SqlDbType.NVarChar, 32);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                if (id == "ReturnToBulk")
                    cmd.Parameters["@AliasName"].Value = "Return to Bulk";
                else if (id == "ShipmentTransfer")
                    cmd.Parameters["@AliasName"].Value = "Shipment - Transfer";
                else if (id == "PhysicalInventory")
                    cmd.Parameters["@AliasName"].Value = "Physical Inventory";
                else
                    cmd.Parameters["@AliasName"].Value = id;
                    

				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			transactionAlias.LoadIdentityGuid(set);
			return transactionAlias.IdentityGuid;
		}

		/// <summary>
		/// The get by ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="id">
		/// The ID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public TransactionAliasClass GetById(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.VIEW_QUERIES)
				&& !security.HasRight(RIGHT.MODIFY_QUERIES) 
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var transactionAlias = new TransactionAliasClass { SiteGuid = security.SiteGuid, ID = id };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetTransactionAliasesById";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@AliasName", SqlDbType.NVarChar, 32);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@AliasName"].Value = id;
				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			transactionAlias.Load(set);
			return transactionAlias;
		}

		/// <summary>
		/// The purge.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionAliasGuid">
		/// The transaction alias GUID.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid transactionAliasGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			TransactionAliasClass transactionAlias = this.Get(security, transactionAliasGuid, false);

			if (transactionAlias.IdentityGuid == Guid.Empty)
			{
				throw new Exception("TransactionAlias Not Found");
			}

			if (transactionAlias.IdentityGuid != transactionAlias.MasterRecordGuid)
			{
				throw new Exception("Cannot delete a Transaction Alias child record version directly");
			}

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, transactionAlias);

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();
			entityToSiteMaps.PurgeAll(security, ENTITY_TYPE.TRANSACTION_ALIAS, transactionAlias.MasterRecordGuid);


			var transactionAliasFields = new TransactionAliasFieldsClass();
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.TransactionFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.LineItemFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.WeightReadingFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.NoteFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.TransportLineItemFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.ExportResultDetailFieldCollection);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.DispatchTransactionFields);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.DispatchLineItemFields);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.DispatchWeightReadingFields);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.DispatchNoteFields);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.DispatchTransportLineItemFields);
			transactionAliasFields.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, null, transactionAlias.DispatchExportResultDetailFields);

			this.UpdateUserDataFields(security, null, transactionAlias);

			var productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(security, transactionAlias.IdentityGuid, transactionAlias.ID, false, null, transactionAlias.ExcludedProductCollection);

			// Purge Alias to Security Group Map (IGO 2009-Sep-09)
			var groupTransactionAliasMaps = new GroupTransactionAliasMapsClass();

			foreach (GroupTransactionAliasMapClass groupTransactionAliasMap in transactionAlias.GroupTransactionAliasMapCollection)
			{
				groupTransactionAliasMaps.Purge(security, groupTransactionAliasMap.GroupGuid, groupTransactionAliasMap.TransactionAliasGuid);
			}

			// Delete the assigned transaction statuses
			using (var cmd = new SqlCommand())
			{
				transactionAlias.DeleteAssignedStatusesSQL(cmd);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}

			// Delete the associated aliases
			using (var cmd = new SqlCommand())
			{
				transactionAlias.DeleteAssociatedAliasesSQL(cmd);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}

            using (var cmd = new SqlCommand())
            {
                transactionAlias.PurgeSQL(cmd);
                this.ConsolidatedDa.ExecuteQuery(security, cmd);
            }
		}

		/// <summary>
		/// The enumerate names only.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="byUser">
		/// The by user.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasNameCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		public TransactionAliasNameCollectionClass EnumerateNamesOnly(SecurityClass security, bool byUser)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var transactionAliasNames = new TransactionAliasNameCollectionClass();

			if (byUser)
			{
				// Remove aliases in which the user in not in the associated group collection (IGO 2009-Sep-10)
				var groups = new GroupsClass();
				var groupcollection = groups.EnumerateByUserBySite(security, security.UserGuid, security.SiteGuid);

				TransactionAliasCollectionClass aliascollection = this.Enumerate(security);

				var groupTransactionAliasMaps = new GroupTransactionAliasMapsClass();

				// loop through each alias
				foreach (TransactionAliasClass transactionalias in aliascollection)
				{
					transactionalias.GroupTransactionAliasMapCollection = groupTransactionAliasMaps.EnumerateByTransactionAliasGuid(security, transactionalias.IdentityGuid);

					// loop though each user group mapped to the current alias
					foreach (GroupTransactionAliasMapClass grouptransactionaliasmap in transactionalias.GroupTransactionAliasMapCollection)
					{
						bool found = false;

						// loop through each group associated with the user looking for a match
						foreach (GroupClass group in groupcollection)
						{
							if (grouptransactionaliasmap.GroupGuid == group.IdentityGuid)
							{
								found = true;
								break;
							}
						}

						// if the group is found add it the collection that is returned
						if (found)
						{
							var aliasname = new TransactionAliasNameClass
												{
													AliasName = transactionalias.ID,
													TransTypeID = transactionalias.TransTypeID,
													IdentityGuid = transactionalias.IdentityGuid,
													MasterRecordGuid = transactionalias.MasterRecordGuid
												};

							if (null == transactionAliasNames.Find(x => x.AliasName == aliasname.AliasName))
							{
								transactionAliasNames.Add(aliasname);
							}
						}
					}
				}
			}
			else
			{
				var transactionAliasName = new TransactionAliasNameClass();

				using (var cmd = new SqlCommand())
				{
					transactionAliasName.EnumerateSql(cmd, security);
					DataSet set = this.ConsolidatedDa.GetDataSet(cmd, security);
					DataTable table = set.Tables[0];

					while (table.Rows.Count != 0)
					{
						transactionAliasName = new TransactionAliasNameClass();
						transactionAliasName.Load(set);

						transactionAliasNames.Add(transactionAliasName);

						table.Rows.RemoveAt(0);
					}
				}
			}

			return transactionAliasNames;
		}

		/// <summary>
		/// Gets a list of TransactionAliasNameClass objects that are associated with
		/// transaction aliases that have the "IncludeInDispatch" flag set to true.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of TransactionAliasNameClass objects</returns>
		public TransactionAliasNameCollectionClass EnumerateDispatchAliasNames(SecurityClass security)
		{
			var transactionAliasNames = new TransactionAliasNameCollectionClass();
			var transactionAliasName = new TransactionAliasNameClass();
			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

			using (var cmd = new SqlCommand())
			{
				transactionAliasName.EnumerateForDispatchSql(cmd, site);
				DataSet set = this.ConsolidatedDa.GetDataSet(cmd, security);

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					transactionAliasName = new TransactionAliasNameClass();
					transactionAliasName.Load(set);

					transactionAliasNames.Add(transactionAliasName);

					table.Rows.RemoveAt(0);
				}
			}

			return transactionAliasNames;
		}

		/// <summary>
		/// Gets a list of transaction status codes that are associated with transaction
		/// aliases that have the "IncludeInDispatch" flag set to true.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of transaction status codes</returns>
		public List<string> EnumerateDispatchStatusCodes(SecurityClass security)
		{
			var transactionStatusCodes = new List<string>();
			using (var cmd = new SqlCommand())
			{
				var transactionAlias = new TransactionAliasClass();
				transactionAlias.EnumerateDispatchStatusesSQL(cmd, security);
				DataSet set = this.ConsolidatedDa.GetDataSet(cmd, security);

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					DataRow row = table.Rows[0];
					var statusCode = DataObject.getValue<string>(row["TransactionStatusCode"], string.Empty);
					if (statusCode != string.Empty)
					{
						transactionStatusCodes.Add(statusCode);
					}

					table.Rows.RemoveAt(0);
				}
			}

			return transactionStatusCodes;
		}

		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasCollectionClass"/>.
		/// </returns>
		public TransactionAliasCollectionClass Enumerate(SecurityClass security)
		{
			return this.Enumerate2(security, security.SiteGuid);
		}

		/// <summary>
		/// The enumerate 2.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetSiteGuid">
		/// The target site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		public TransactionAliasCollectionClass Enumerate2(SecurityClass security, Guid targetSiteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var transactionAliasCollection = new TransactionAliasCollectionClass( );

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.CONFIGURE_ACCOUNTING)
				&& !security.HasRight(RIGHT.VIEW_QUERIES)
				&& !security.HasRight(RIGHT.MODIFY_QUERIES)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				// Just return an empty collection if the user does not have rights, instead of throwing an 
				// exception.
				return transactionAliasCollection;
			}

			var transactionAlias = new TransactionAliasClass { SiteGuid = targetSiteGuid };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				// TransactionAlias.EnumerateSQL(cmd, security);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetTransactionAliasesById";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@AliasName", SqlDbType.NVarChar, 32);
				cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
				cmd.Parameters["@AliasName"].Value = DBNull.Value;
				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			transactionAlias.Load(set);
			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				transactionAlias = new TransactionAliasClass();
				transactionAlias.Load(set);
				transactionAliasCollection.Add(transactionAlias);
				table.Rows.RemoveAt(0);
			}

			return transactionAliasCollection;
		}

		/// <summary>
		/// This method will return a collection of transaction aliases based on
		/// on the alias transaction group map.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a collection of transaction aliases.</returns>
		public TransactionAliasCollectionClass EnumerateByGroupMapsOnly(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var transactionAliasCollection = new TransactionAliasCollectionClass();

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.CONFIGURE_ACCOUNTING)
				&& !security.HasRight(RIGHT.VIEW_QUERIES)
				&& !security.HasRight(RIGHT.MODIFY_QUERIES)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				// Just return an empty collection if the user does not have rights, instead of throwing an 
				// exception.
				return transactionAliasCollection;
			}

			SqlCommand command;

			using (command = new SqlCommand())
			{
				var transactionAlias = new TransactionAliasClass { SiteGuid = security.SiteGuid };
				transactionAlias.EnumerateSQL(command, security);
				DataSet set = this.ConsolidatedDa.GetDataSet(command, security);

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					transactionAlias = new TransactionAliasClass();
					transactionAlias.Load(set);

					// Get Alias to Security Group Map (IGO 2009-Sep-09)
					var groupTransactionAliasMaps = new GroupTransactionAliasMapsClass();
					transactionAlias.GroupTransactionAliasMapCollection = 
										groupTransactionAliasMaps.EnumerateByTransactionAliasGuid(security, transactionAlias.IdentityGuid);

					transactionAliasCollection.Add(transactionAlias);
					table.Rows.RemoveAt(0);
				}
			}

			return transactionAliasCollection;
		}

		/// <summary>
		/// The enumerate by trans type ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="TransTypeID">
		/// The trans type ID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public TransactionAliasCollectionClass EnumerateByTransTypeID(SecurityClass security, TransactionTypes TransTypeID)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) 
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.PERFORM_CLOSEOUT) 
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.INTERFACE_IMPORT)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_DISPATCH) 
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var transactionAlias = new TransactionAliasClass { SiteGuid = security.SiteGuid, TransTypeID = TransTypeID };

			using (var cmd = new SqlCommand())
			{
				transactionAlias.EnumerateByTransTypeIDSQL(cmd, security);
				DataSet set = this.ConsolidatedDa.GetDataSet(cmd, security);
				var transactionAliasCollection = new TransactionAliasCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					transactionAlias = new TransactionAliasClass();
					transactionAlias.Load(set);

					// vt - Add the transaction statuses
					using (var cmd2 = new SqlCommand())
					{
						transactionAlias.SelectAssignedStatusesSQL(cmd2, ContextUtil.IsInTransaction, security);
						transactionAlias.LoadAssignedStatuses(this.ConsolidatedDa.GetDataSet(cmd2, security));
					}

					// vthompson 8/20/2008
					// Populate the associated aliases
					this.PopulateAssociatedAliases(security, ref transactionAlias);
					transactionAliasCollection.Add(transactionAlias);
					table.Rows.RemoveAt(0);
				}

				return transactionAliasCollection;
			}
		}

		/// <summary>
		/// The enumerate undelegated.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument exception.
		/// </exception>
		public TransactionAliasCollectionClass EnumerateUndelegated(SecurityClass security)
		{
			var transactionAliasCollection = new TransactionAliasCollectionClass();

			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetUndelegatedTransactionAliases";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			if (set.Tables[0].Rows.Count > 0)
			{
				while (set.Tables[0].Rows.Count != 0)
				{
					DataRow row = set.Tables[0].Rows[0];
					var transactionAlias = new TransactionAliasClass
											   {
												   IdentityGuid = DataObject.getValue(row["TransactionAliasGuid"], Guid.Empty),
												   MasterRecordGuid = DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
												   ID = DataObject.getValue(row["Id"], string.Empty),
												   SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
												   AssignedToSiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
												   AssignedFromSiteGuid = DataObject.getValue(row["AssignedFromSiteGuid"], Guid.Empty),
												   AssignedFromSiteId = DataObject.getValue<string>(row["AssignedFromSiteId"], string.Empty)
											   };
					transactionAliasCollection.Add(transactionAlias);
					set.Tables[0].Rows.RemoveAt(0);
				}
			}

			return transactionAliasCollection;
		}

		/// <summary>
		/// The insert.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="Object">
		/// The object.
		/// </param>
		/// <param name="preOperation">
		/// The pre operation.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid arguement.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}
		}

		/// <summary>
		/// The import.
		/// </summary>
		/// <param name="inSecurity">
		/// The security.
		/// </param>
		/// <param name="alias">
		/// The alias.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		/// <exception cref="ApplicationException">
		/// Import exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass inSecurity, TransactionAliasClass alias)
		{
			if (inSecurity == null)
			{
				throw new ArgumentNullException("security");
			}

			if (alias == null)
			{
				throw new ArgumentNullException(nameof(alias));
			}

			SecurityClass security = inSecurity.Clone();

			var groups = new GroupsClass();
			var products = new ProductsClass();

			try
			{
				// Get this early since we may need to use it
				alias.IdentityGuid = this.GetIdentityGuid(security, alias.ID);

				// If the entity exists and is not owned by this site, do not update it.
				if (alias.IdentityGuid != Guid.Empty
					&& this.Get(security, alias.IdentityGuid, false).SiteGuid != security.SiteGuid)
				{
					return;
				}

				// Modify User Group Assignments
				foreach (GroupTransactionAliasMapClass groupMap in alias.GroupTransactionAliasMapCollection)
				{
					Guid groupGuid = groups.GetIdentityGuid(security, groupMap.ID);

					if (groupGuid == Guid.Empty)
					{
						var group = new GroupClass();
						group.ID = groupMap.ID;
						groupGuid = groups.Add(security, group);
					}

					groupMap.GroupGuid = groupGuid;
					groupMap.TransactionAliasGuid = alias.IdentityGuid;
				}

				// Look up indices for associated aliases - add if necessary
				foreach (TransactionAliasClass associatedAlias in alias.AssociatedAliases)
				{
					Guid associatedAliasGuid = this.GetIdentityGuid(security, associatedAlias.ID);

					if (associatedAliasGuid == Guid.Empty)
					{
						associatedAliasGuid = this.Add(security, associatedAlias);
					}

					associatedAlias.IdentityGuid = associatedAliasGuid;
				}

				// Excluded products
				foreach (ProductMapClass productMap in alias.ExcludedProductCollection)
				{
					Guid productGuid = products.GetIdentityGuid(security, productMap.AssignedID);
					if (productGuid == Guid.Empty)
					{
						var product = new ProductClass
										  {
											  ID = productMap.AssignedID,
											  ProductType = ProductType.ComponentProduct
										  };

						productGuid = products.Add(security, product);
					}

					productMap.AssignedGuid = productGuid;
					productMap.AssignedToGuid = alias.IdentityGuid;
				}

				if (alias.IdentityGuid == Guid.Empty)
				{
					this.Add(security, alias);
				}
				else
				{
					this.Modify(security, alias);
				}
			}
			catch (Exception except)
			{
				throw new ApplicationException("[Transaction Alias Import Error ID] : " + alias.ID + ", " + except.Message);
			}
		}

		/// <summary>
		/// The update.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="Object">
		/// The object.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}

			var siteObject = Object as SiteClass;

			if (siteObject != null)
			{
				var site = siteObject;
				TransactionAliasCollectionClass transactionAliasCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();

				foreach (TransactionAliasClass transactionAlias in transactionAliasCollection)
				{
					if (site.SiteGuid == transactionAlias.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, transactionAlias.EntityType, transactionAlias.IdentityGuid);
						
						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = transactionAlias.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// The purge.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="Object">
		/// The object.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}

			// Purge TransactionAliases
			var siteObject = Object as SiteClass;

			if (siteObject != null)
			{
				var site = siteObject;
				TransactionAliasCollectionClass transactionAliasCollection = this.Enumerate2(security, site.SiteGuid);
				var entityToSiteMaps = new EntityToSiteMaps();
				
				foreach (TransactionAliasClass transactionAlias in transactionAliasCollection)
				{
					if (site.SiteGuid == transactionAlias.SiteGuid && transactionAlias.MasterRecordGuid == transactionAlias.IdentityGuid)
					{
						this.Purge(security, transactionAlias.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass(transactionAlias) { SiteGuid = site.SiteGuid };
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
		}

		/// <summary>
		/// Populates the passed alias with its associated aliases
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="transactionAlias">The alias that needs to be populated</param>
		private void PopulateAssociatedAliases(SecurityClass security, ref TransactionAliasClass transactionAlias)
		{
			using (var cmd = new SqlCommand())
			{
				transactionAlias.SelectAssociatedAliasesSQL(cmd, security);
				DataSet ds = this.ConsolidatedDa.GetDataSet(cmd, security);
				DataTable dt = ds.Tables[0];

				while (dt.Rows.Count > 0)
				{
					var associated = new TransactionAliasClass();
					associated.Load(ds);
					transactionAlias.AssociatedAliases.Add(associated);
					dt.Rows.RemoveAt(0);
				}
			}
		}
	}
}
