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
	public class ListViewFieldsClass : IListViewFields
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public ListViewFieldsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Private methods
		private void Validate(SecurityClass security, ListViewFieldClass listViewField)
		{
			if (listViewField.Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS)
			{
				TransactionAliasesClass TransactionAliases = new TransactionAliasesClass();
				TransactionAliasClass TransactionAlias = TransactionAliases.Get(security, listViewField.TypeGuid, false);

				if (TransactionAlias.IdentityGuid == Guid.Empty)
				{
					throw new Exception("Transaction Alias Not Found: " + listViewField.ID);
				}

			}
			else if (listViewField.Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD)
			{
				TransactionAliasFieldsClass TransactionAliaseFields = new TransactionAliasFieldsClass();
				TransactionAliasFieldClass TransactionAliaseField = TransactionAliaseFields.Get(security, listViewField.TypeGuid);

				if (TransactionAliaseField.IdentityGuid == Guid.Empty)
				{
					throw new Exception("Transaction Alias Field Not Found: " + listViewField.ID);
				}
			}
			else if (listViewField.Type == LISTVIEW_FIELD_TYPE.USER_DATA_FIELD)
			{
				UserDataFieldsClass UserDataFields = new UserDataFieldsClass();

				UserDataFieldClass UserDataField = UserDataFields.Get(security, listViewField.TypeGuid, ENTITY_TYPE.TRANSACTION_ALIAS);

				if (UserDataField.IdentityGuid == Guid.Empty)
				{
					throw new Exception("User Data Field Not Found: " + listViewField.ID);
				}
			}
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ListViewFieldClass listViewField)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (listViewField == null)
			{
				throw new ArgumentNullException("listViewField");
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, listViewField);

			listViewField.CreatedDate = DateTimeOffset.Now;
			listViewField.CreatedBy = security.UserID;
			listViewField.UpdatedDate = listViewField.CreatedDate;
			listViewField.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				listViewField.IdentityGuid = Guid.NewGuid();
				listViewField.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}



			return listViewField.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ListViewFieldClass listViewField)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (listViewField == null)
			{
				throw new ArgumentNullException("listViewField");
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, listViewField);

			ListViewFieldClass OldListViewField = Get(security, listViewField.Type, listViewField.IdentityGuid);

			if (OldListViewField.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("ListViewField Not Found"));
			}

			listViewField.CreatedDate = DateTimeOffset.Now;
			listViewField.CreatedBy = security.UserID;
			listViewField.UpdatedDate = listViewField.CreatedDate;
			listViewField.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				listViewField.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, LISTVIEW_FIELD_TYPE listViewFieldType, Guid listViewFieldGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			ListViewFieldClass ListViewField = Get(security, listViewFieldType, listViewFieldGuid);

			if (ListViewField.IdentityGuid == Guid.Empty)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				ListViewField.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		public ListViewFieldClass Get(SecurityClass security, LISTVIEW_FIELD_TYPE listViewFieldType, Guid listViewFieldGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) && !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			ListViewFieldClass ListViewField = new ListViewFieldClass();
			ListViewField.IdentityGuid = listViewFieldGuid;
			ListViewField.Type = listViewFieldType;

            DataSet Set;
            using (SqlCommand cmd = new SqlCommand())
            {
                //ListViewField.SelectSQL(cmd, ContextUtil.IsInTransaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetListViewFieldByGuid";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@ListViewFieldGuid", SqlDbType.UniqueIdentifier);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@ListViewFieldGuid"].Value = listViewFieldGuid;
                Set = consolidatedDA.GetDataSet(cmd, security);
            }
            ListViewField.Load(Set);
			return ListViewField;
		}

		public ListViewFieldCollectionClass Enumerate(SecurityClass security, Guid listViewGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.VIEW_BILLS_OF_LADING) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.PERFORM_CLOSEOUT) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH) &&
                !security.HasRight(RIGHT.CREATE_ORDERS) &&
                !security.HasRight(RIGHT.VIEW_ORDERS) &&
                !security.HasRight(RIGHT.MODIFY_ORDERS) &&
                !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS) &&
                !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS) &&
                !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS) &&
                !security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION) &&
				!security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION))
			{
				throw new FMInsufficientRightsException();
			}

			ListViewFieldClass ListViewField = new ListViewFieldClass();
			ListViewField.ListViewGuid = listViewGuid;

            DataSet Set;
            using (SqlCommand cmd = new SqlCommand())
            {
                //ListViewField.EnumerateSQL(cmd, ContextUtil.IsInTransaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetListViewFieldsByListView";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@ListViewGuid", SqlDbType.UniqueIdentifier);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@ListViewGuid"].Value = listViewGuid;
                Set = consolidatedDA.GetDataSet(cmd, security);
            }

			ListViewFieldCollectionClass ListViewFieldCollection = new ListViewFieldCollectionClass();

			LedgerAggregateColumnsClass aggregateColumns = new LedgerAggregateColumnsClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				ListViewField = new ListViewFieldClass();
				ListViewField.Load(Set);

				if (ListViewField.Type == LISTVIEW_FIELD_TYPE.AGGREGATE_FIELD)
				{
					LedgerAggregateColumnClass aggregateColumn = aggregateColumns.GetByColumnID(security, ListViewField.ID);
					if (aggregateColumn.IdentityGuid != Guid.Empty)
					{
						ListViewField.AggregateType = aggregateColumn.AggregateField;
					}

				}

				ListViewFieldCollection.Add(ListViewField);

				Table.Rows.RemoveAt(0);
			}

			return ListViewFieldCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(SecurityClass security,
										Guid listViewGuid,
										string listViewID,
										ListViewFieldCollectionClass newListViewFieldCollection,
										ListViewFieldCollectionClass oldListViewFieldCollection)
		{
			if (newListViewFieldCollection != null)
			{
				foreach (ListViewFieldClass NewListViewField in newListViewFieldCollection)
				{
					NewListViewField.ListViewGuid = listViewGuid;
					NewListViewField.ListViewID = listViewID;

					if (oldListViewFieldCollection != null)
					{
						int index = 0;

						foreach (ListViewFieldClass OldListViewField in oldListViewFieldCollection)
						{
							if (OldListViewField.Type == NewListViewField.Type && OldListViewField.TypeGuid == NewListViewField.TypeGuid)
							{
								NewListViewField.IdentityGuid = OldListViewField.IdentityGuid;

								if (OldListViewField.ColumnOrder != NewListViewField.ColumnOrder ||
									OldListViewField.ListViewID != NewListViewField.ListViewID)
								{
									this.Modify(security, NewListViewField);
								}
								break;
							}

							index++;
						}

						if (index < oldListViewFieldCollection.Count)
						{
							oldListViewFieldCollection.RemoveAt(index);
						}
						else
						{
							this.Add(security, NewListViewField);
						}
					}
					else
					{
						this.Add(security, NewListViewField);
					}
				}
			}

			if (oldListViewFieldCollection != null)
			{
				foreach (ListViewFieldClass oldListViewField in oldListViewFieldCollection)
				{
					this.Purge(security, oldListViewField.Type, oldListViewField.IdentityGuid);
				}
			}
		}
	}
}