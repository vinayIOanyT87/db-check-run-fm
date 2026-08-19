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

    using IsolationLevel = System.Transactions.IsolationLevel;

    [SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class ListViewsClass : IDependency, IListViews
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public ListViewsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ListViewClass listView)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (listView == null)
			{
				throw new ArgumentNullException(nameof(listView));
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			// If this is a ledger view, verify that a list view with the same ID does not already exist 
			// Otherwise, make sure that a list view of the same type does not already exist.
			Guid listViewGuid;

			if (listView.Type == LISTVIEW_TYPE.STANDARD && listView.ListViewStandardType == LISTVIEW_STANDARD_TYPE.LEDGER)
			{
				listViewGuid = this.GetIdentityGuidByID(security, listView.Type, listView.TypeGuid, listView.ID);

				if (listViewGuid != Guid.Empty)
				{
					throw new Exception("A ledger view with the same name already exists");
				}
			}
			else
			{
				listViewGuid = this.GetIdentityGuid(security, listView.Type, listView.TypeGuid);

				if (listViewGuid != Guid.Empty)
				{
					throw new Exception("A list view of the same type already exists");
				}
			}

			listView.SiteGuid = security.SiteGuid;
			listView.CreatedDate = DateTimeOffset.Now;
			listView.CreatedBy = security.UserID;
			listView.UpdatedDate = listView.CreatedDate;
			listView.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				listView.IdentityGuid = Guid.NewGuid();
				listView.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(listView);
			entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

			ListViewFieldsClass listViewFields = new ListViewFieldsClass();

			listViewFields.ModifyCollection(security, listView.IdentityGuid, listView.ID, listView.ListViewFieldCollection, null);

			ProductMapsClass productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(security, listView.IdentityGuid, listView.ID, false, listView.ProductMapCollection, null);

			GroupLedgerViewMapsClass groupMaps = new GroupLedgerViewMapsClass();
			groupMaps.ModifyCollection(security, listView.IdentityGuid, listView.GroupMapCollection, null);

			return listView.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ListViewClass listView)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (listView == null)
			{
				throw new ArgumentNullException(nameof(listView));
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			Guid listViewGuid;

			if (listView.Type == LISTVIEW_TYPE.STANDARD && listView.ListViewStandardType == LISTVIEW_STANDARD_TYPE.LEDGER)
			{
				listViewGuid = this.GetIdentityGuidByID(security, listView.Type, listView.TypeGuid, listView.ID);
			}
			else
			{
				listViewGuid = this.GetIdentityGuid(security, listView.Type, listView.TypeGuid);
			}

			if (listViewGuid != Guid.Empty && listViewGuid != listView.IdentityGuid)
			{
				throw new Exception("ListView Exists");
			}

			ListViewClass oldListView = this.Get(security, listView.Type, listView.IdentityGuid);

         if (oldListView.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("ListView Not Found"));
			}

			listView.UpdatedDate = DateTimeOffset.Now;
			listView.UpdatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				listView.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}


			EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, listView.EntityType, listView.IdentityGuid);

			if (listView.SiteGuid != oldListView.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = listView.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				EntityToSiteMapClass newEntityToSiteMap = new EntityToSiteMapClass(listView);
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
			}

			ListViewFieldsClass listViewFields = new ListViewFieldsClass();
			listViewFields.ModifyCollection(security, listView.IdentityGuid, listView.ID, listView.ListViewFieldCollection, oldListView.ListViewFieldCollection);

			// ProductMaps
			foreach (ProductMapClass productMap in listView.ProductMapCollection)
			{
				productMap.AssignedToGuid = listView.IdentityGuid;
			}

			ProductMapsClass productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(security, listView.IdentityGuid, listView.ID, false, listView.ProductMapCollection, oldListView.ProductMapCollection);

			GroupLedgerViewMapsClass groupMaps = new GroupLedgerViewMapsClass();
			groupMaps.ModifyCollection(security, listView.IdentityGuid, listView.GroupMapCollection, oldListView.GroupMapCollection);
		}

		public ListViewClass GetWithProductsAndGroups(	SecurityClass security, 
														LISTVIEW_TYPE listViewType, 
														Guid listViewGuid,
														bool includeProductsAndGroups)
		{
			return this.Get(security, listViewType, listViewGuid, includeProductsAndGroups);
		}

		public ListViewClass Get(SecurityClass security, LISTVIEW_TYPE listViewType, Guid listViewGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.VIEW_BILLS_OF_LADING) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.PERFORM_CLOSEOUT) &&
				!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
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

		    ListViewClass listView = new ListViewClass { IdentityGuid = listViewGuid, Type = listViewType };

		    using (SqlCommand cmd = new SqlCommand())
			{
				listView.SelectSQL(cmd, ContextUtil.IsInTransaction);
				listView.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			ListViewFieldsClass listViewFields = new ListViewFieldsClass();
			listView.ListViewFieldCollection = listViewFields.Enumerate(security, listView.IdentityGuid);

			ProductMapsClass productMaps = new ProductMapsClass();
			listView.ProductMapCollection = productMaps.EnumerateByAssignedToGuidAndTypeInstr(security,
																								listView.IdentityGuid,
																								PRODUCT_MAP_TYPE.LEDGER_VIEW_MAP,
																								false);

			GroupLedgerViewMapsClass groupMaps = new GroupLedgerViewMapsClass();
			listView.GroupMapCollection = groupMaps.EnumerateByListViewGuid(security, listView.IdentityGuid);

			return listView;
		}

		public Guid GetIdentityGuidByID(SecurityClass security, LISTVIEW_TYPE listViewType, Guid typeGuid, string id)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.VIEW_BILLS_OF_LADING) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.PERFORM_CLOSEOUT) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

		    ListViewClass listView = new ListViewClass
		                             {
		                                 SiteGuid = security.SiteGuid,
		                                 Type = listViewType,
		                                 TypeGuid = typeGuid,
		                                 ID = id
		                             };
		    using (SqlCommand cmd = new SqlCommand())
			{
				listView.SelectByLedgerIDSQL(ContextUtil.IsInTransaction, cmd);
				listView.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}


			return listView.IdentityGuid;
		}

		public Guid GetIdentityGuid(SecurityClass security, LISTVIEW_TYPE listViewType, Guid typeGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
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

		    ListViewClass listView = new ListViewClass
		                             {
		                                 SiteGuid = security.SiteGuid,
		                                 Type = listViewType,
		                                 TypeGuid = typeGuid
		                             };
		    using (SqlCommand cmd = new SqlCommand())
			{
				listView.SelectByTypeAndForeignKeySQL(cmd, ContextUtil.IsInTransaction);
				listView.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return listView.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, LISTVIEW_TYPE listViewType, Guid listViewGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			ListViewClass listView = this.Get(security, listViewType, listViewGuid);
			if (listView.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("ListView Not Found"));
			}

			// Purge from EntityToSiteMap
			EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, listView.EntityType, listViewGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = listView.ID;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			ProductMapsClass productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(security, listView.IdentityGuid, listView.ID, false, null, listView.ProductMapCollection);

			GroupLedgerViewMapsClass groupMaps = new GroupLedgerViewMapsClass();
			groupMaps.ModifyCollection(security, listView.IdentityGuid, null, listView.GroupMapCollection);

			ListViewFieldsClass listViewFields = new ListViewFieldsClass();
			listViewFields.ModifyCollection(security, listView.IdentityGuid, listView.ID, null, listView.ListViewFieldCollection);

			using (SqlCommand cmd = new SqlCommand())
			{
				listView.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public ListViewCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

		    ListViewClass listView = new ListViewClass { SiteGuid = security.SiteGuid };

		    DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				listView.EnumerateSQL(security, cmd);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			ListViewCollectionClass listViewCollection = new ListViewCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				listView = new ListViewClass();
				listView.Load(set);
				listViewCollection.Add(listView);
				table.Rows.RemoveAt(0);
			}

			return listViewCollection;
		}

		public string CreateDefaultListViews(SecurityClass security)
		{
			string returnMsg1 = String.Empty;
			string returnMsg2 = String.Empty;
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			ListViewClass listView = new ListViewClass { SiteGuid = security.SiteGuid };

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "[dbo].[usp_AddStandardListViews]";
				cmd.CommandTimeout = 0;
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@ReturnMsg", SqlDbType.NVarChar, 2000);
				cmd.Parameters["@ReturnMsg"].Value = String.Empty;
				cmd.Parameters["@ReturnMsg"].Direction = ParameterDirection.Output;

				consolidatedDA.ExecuteQuery(security, cmd);
                var Msg = Convert.ToString(cmd.Parameters["@ReturnMsg"].Value);
				returnMsg1 = "--- Standard List Views ---" + Environment.NewLine;
				returnMsg1 += string.Join(Environment.NewLine, Msg.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
				returnMsg1 += Environment.NewLine;
            }

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "[dbo].[usp_AddTransactionListViews]";
				cmd.CommandTimeout = 0;
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@ReturnMsg", SqlDbType.NVarChar, 2000);
				cmd.Parameters["@ReturnMsg"].Value = String.Empty;
				cmd.Parameters["@ReturnMsg"].Direction = ParameterDirection.Output;

				consolidatedDA.ExecuteQuery(security, cmd);
				var Msg = Convert.ToString(cmd.Parameters["@ReturnMsg"].Value);
				returnMsg2 = "--- Transaction List Views ---" + Environment.NewLine;
				returnMsg2 += string.Join(Environment.NewLine, Msg.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
				returnMsg2 += Environment.NewLine;
			}

			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				listView.EnumerateSQL(security, cmd);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			ListViewCollectionClass listViewCollection = new ListViewCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				listView = new ListViewClass();
				listView.Load(set);
				listViewCollection.Add(listView);
				table.Rows.RemoveAt(0);
			}

			return returnMsg1 + Environment.NewLine + returnMsg2;
		}

		public string CreateDefaultLedgerView(SecurityClass security)
		{
			string returnMsg = String.Empty;
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			ListViewClass listView = new ListViewClass { SiteGuid = security.SiteGuid };

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "[dbo].[usp_AddLedgerListView]";
				cmd.CommandTimeout = 0;
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@ReturnMsg", SqlDbType.NVarChar, 2000);
				cmd.Parameters["@ReturnMsg"].Value = String.Empty;
				cmd.Parameters["@ReturnMsg"].Direction = ParameterDirection.Output;

				consolidatedDA.ExecuteQuery(security, cmd);

				var Msg = Convert.ToString(cmd.Parameters["@ReturnMsg"].Value);
				returnMsg = "--- Standard Ledger View ---" + Environment.NewLine;
				returnMsg += string.Join(Environment.NewLine, Msg.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
				returnMsg += Environment.NewLine;
			}
			return returnMsg;
		}

		public ListViewCollectionClass EnumerateByTypeAndTypeGuid(SecurityClass security, LISTVIEW_TYPE listViewType, Guid typeGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

		    ListViewClass listView = new ListViewClass { Type = listViewType, TypeGuid = typeGuid };

		    DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				listView.EnumerateByTypeAndForeignKeySQL(security, cmd);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			ListViewCollectionClass listViewCollection = new ListViewCollectionClass();
			ProductMapsClass productMaps = new ProductMapsClass();
			GroupLedgerViewMapsClass groupMaps = new GroupLedgerViewMapsClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				listView = new ListViewClass();
				listView.Load(set);
				listViewCollection.Add(listView);
				table.Rows.RemoveAt(0);

				if (listViewType == LISTVIEW_TYPE.STANDARD && listView.ListViewStandardType == LISTVIEW_STANDARD_TYPE.LEDGER)
				{
					listView.ProductMapCollection = productMaps.EnumerateByAssignedToGuidAndTypeInstr(security,
																										listView.IdentityGuid,
																										PRODUCT_MAP_TYPE.LEDGER_VIEW_MAP,
																										false);
					listView.GroupMapCollection = groupMaps.EnumerateByListViewGuid(security, listView.IdentityGuid);
				}

			}

			return listViewCollection;
		}

		public ListViewCollectionClass EnumerateAggregatesByAliasGuid(SecurityClass security, Guid aliasGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
			&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
			&& !security.HasRight(RIGHT.VIEW_DISPATCH))
				throw new FMInsufficientRightsException();

			ListViewClass listView = new ListViewClass();
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				listView.EnumerateAggregatesByAliasGuidSQL(security, cmd, aliasGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			ListViewCollectionClass listViewCollection = new ListViewCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				listView = new ListViewClass();
				listView.Load(set);
				listViewCollection.Add(listView);
				table.Rows.RemoveAt(0);
			}

			return listViewCollection;
		}

		/// <summary>
		/// This method will return ledger views based on the product and user.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="productGuid">The product GUID</param>
		/// <returns>Returns List View collection.</returns>
		public ListViewCollectionClass EnumerateLedgerViewsByProductAndUser(SecurityClass security, Guid productGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			    && !security.HasRight(RIGHT.MODIFY_DISPATCH) 
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw (new Exception("Access Denied"));
			}

			var listViewCollection = new ListViewCollectionClass();

			using (var command = new SqlCommand())
			{
				var listView = new ListViewClass
				               {
					               Type = LISTVIEW_TYPE.STANDARD,
					               ListViewStandardType = LISTVIEW_STANDARD_TYPE.LEDGER
				               };

				listView.EnumerateByProductAndUserSQL(security, command, productGuid);
				DataSet set = this.consolidatedDA.GetDataSet(command, security);

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					listView = new ListViewClass();
					listView.Load(set);
					listViewCollection.Add(listView);
					table.Rows.RemoveAt(0);
				}
			}

			return listViewCollection;
		}
		#endregion

		#region Dependency methods
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
            TransactionAliasesClass transactionAliases = new TransactionAliasesClass();
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}


			//check if TransactionAliasFieldClass is default to be assigned
			if (preOperation && Object is TransactionAliasFieldClass)
			{
				TransactionAliasFieldClass transactionAliasField = (TransactionAliasFieldClass)Object;

				if (transactionAliasField.DefaultAssigned) //only deal with the field need to be assined as default
				{
                    Guid transactionAliasGuid = Guid.Empty;
                    TransactionAliasClass transactionAlias = transactionAliases.GetBasicInfo(security, transactionAliasField.TransactionAliasGuid, security.SiteGuid);
                    if (transactionAlias != null)
                        transactionAliasGuid = transactionAlias.MasterRecordGuid;
				    ListViewClass listView = new ListViewClass(LISTVIEW_TYPE.TRANSACTION_LIST, transactionAliasGuid)
				                             {
				                                 ID = transactionAliasField.AliasName,
				                                 IdentityGuid = this.GetIdentityGuid(security,
                                                                                        LISTVIEW_TYPE.TRANSACTION_LIST,
                                                                                        transactionAliasGuid)
				                             };

				    if (listView.IdentityGuid == Guid.Empty) //no list view is found; create a new entry in tblListView table first
					{
						listView.IdentityGuid = this.Add(security, listView);
					}

					listView = this.Get(security, LISTVIEW_TYPE.TRANSACTION_LIST, listView.IdentityGuid);

					//add the AliasFields in the tblListViewFields table
					ListViewFieldsClass listViewFields = new ListViewFieldsClass();
					listView.ListViewFieldCollection = listViewFields.Enumerate(security, listView.IdentityGuid);

					//ListViewFieldCollectionClass ListViewFieldCollection=new ListViewFieldCollectionClass();
					int columnOrder = 0;
					bool found = false;

					foreach (ListViewFieldClass listViewField in listView.ListViewFieldCollection)
					{
						columnOrder++;

						if (listViewField.Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD && listViewField.TypeGuid == transactionAliasField.IdentityGuid)
						{
							found = true;
							break;
						}
					}

					if (!found) //add the field to the tblListViewFields table
					{
					    ListViewFieldClass listViewField = new ListViewFieldClass(
					        LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD,
					        transactionAliasField.IdentityGuid,
					        columnOrder,
					        transactionAliasField.DisplayName) { ListViewGuid = listView.IdentityGuid, ListViewID = listView.ID };
					    listViewFields.Add(security, listViewField);
					}
				}
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject obj)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

		    var site = obj as SiteClass;
		    if (site != null)
			{
				ListViewCollectionClass listViewCollection = this.Enumerate(security);
				EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();

				foreach (ListViewClass listView in listViewCollection)
				{
					if (site.SiteGuid == listView.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, listView.EntityType, listView.IdentityGuid);

						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = listView.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject obj)
		{
            TransactionAliasesClass transactionAliases = new TransactionAliasesClass();
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			// Purge ListViews
		    var site = obj as SiteClass;
		    if (site != null)
			{
				ListViewCollectionClass listViewCollection = this.Enumerate(security);
				EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();

				foreach (ListViewClass listView in listViewCollection)
				{
					if (site.SiteGuid == listView.SiteGuid)
					{
					    this.Purge(security, listView.Type, listView.IdentityGuid);
					}
					else
					{
					    EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(listView) { SiteGuid = site.SiteGuid };
					    entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}

				listViewCollection = EnumerateByTypeAndTypeGuid(security, LISTVIEW_TYPE.STANDARD, ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.LEDGER));

				foreach (ListViewClass listView in listViewCollection)
				{
					if (site.SiteGuid == listView.SiteGuid)
					{
						this.Purge(security, listView.Type, listView.IdentityGuid);
					}
					else
					{
						EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(listView) { SiteGuid = site.SiteGuid };
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}

				return;
			}

		    var alias = obj as TransactionAliasClass;
		    if (alias != null)
			{
				ListViewCollectionClass listViewCollection = this.EnumerateByTypeAndTypeGuid(security, LISTVIEW_TYPE.TRANSACTION_LIST, alias.MasterRecordGuid);

				foreach (ListViewClass listView in listViewCollection)
				{
				    this.Purge(security, listView.Type, listView.IdentityGuid);
				}

				ListViewFieldsClass listViewFields = new ListViewFieldsClass();
				listViewCollection = this.EnumerateByTypeAndTypeGuid(security, LISTVIEW_TYPE.STANDARD,
																				ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.LEDGER));

				foreach (ListViewClass listView in listViewCollection)
				{
					listView.ListViewFieldCollection = listViewFields.Enumerate(security, listView.IdentityGuid);
					ListViewFieldCollectionClass listViewFieldCollection = new ListViewFieldCollectionClass();
					int columnOrder = 0;
					bool found = false;

					foreach (ListViewFieldClass listViewField in listView.ListViewFieldCollection)
					{
						if (listViewField.Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS && listViewField.TypeGuid == alias.MasterRecordGuid)
						{
							found = true;
							continue;
						}

						listViewField.ColumnOrder = columnOrder++;
						listViewFieldCollection.Add(listViewField);
					}

					if (found)
					{
						listView.ListViewFieldCollection = listViewFieldCollection;
					    this.Modify(security, listView);
					}
				}

				return;
			}

		    var transactionAliasField = obj as TransactionAliasFieldClass;
		    if (transactionAliasField != null)
			{
                Guid transactionAliasGuid = Guid.Empty;
                TransactionAliasClass transactionAlias = transactionAliases.GetBasicInfo(security, transactionAliasField.TransactionAliasGuid, security.SiteGuid);
                if (transactionAlias != null)
                    transactionAliasGuid = transactionAlias.MasterRecordGuid;
				ListViewCollectionClass listViewCollection = this.EnumerateByTypeAndTypeGuid(security, LISTVIEW_TYPE.TRANSACTION_LIST, transactionAliasField.TransactionAliasGuid);
				ListViewFieldsClass listViewFields = new ListViewFieldsClass();

				foreach (ListViewClass listView in listViewCollection)
				{
					listView.ListViewFieldCollection = listViewFields.Enumerate(security, listView.IdentityGuid);
					ListViewFieldCollectionClass listViewFieldCollection = new ListViewFieldCollectionClass();
					int columnOrder = 0;
					bool found = false;

					foreach (ListViewFieldClass listViewField in listView.ListViewFieldCollection)
					{
						if (listViewField.Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD && listViewField.TypeGuid == transactionAliasField.IdentityGuid)
						{
							found = true;
							continue;
						}

						listViewField.ColumnOrder = columnOrder++;
						listViewFieldCollection.Add(listViewField);
					}

					if (found)
					{
						listView.ListViewFieldCollection = listViewFieldCollection;
					    this.Modify(security, listView);
					}
				}

				listViewCollection = this.EnumerateAggregatesByAliasGuid(security, transactionAliasGuid);
				listViewFields = new ListViewFieldsClass();
				foreach (ListViewClass listView in listViewCollection)
				{
					listView.ListViewFieldCollection = listViewFields.Enumerate(security, listView.IdentityGuid);
					ListViewFieldCollectionClass listViewFieldCollection = new ListViewFieldCollectionClass();
					int columnOrder = 0;
					bool found = false;
					foreach (ListViewFieldClass listViewField in listView.ListViewFieldCollection)
					{
						if (listViewField.Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD
						&& listViewField.DataPath == transactionAliasField.DbName)
						{
							found = true;
							continue;
						}

						listViewField.ColumnOrder = columnOrder++;
						listViewFieldCollection.Add(listViewField);
					}

					if (found)
					{
						listView.ListViewFieldCollection = listViewFieldCollection;
					    this.Modify(security, listView);
					}
				}

				return;
			}

		    var userDataField = obj as UserDataFieldClass;
		    if (userDataField != null)
			{
                Guid transactionAliasGuid = Guid.Empty;
                TransactionAliasClass transactionAlias = transactionAliases.GetBasicInfo(security, userDataField.TransactionAliasGuid, security.SiteGuid);
                if (transactionAlias != null)
                    transactionAliasGuid = transactionAlias.MasterRecordGuid;

                ListViewCollectionClass listViewCollection = this.EnumerateByTypeAndTypeGuid(security, LISTVIEW_TYPE.TRANSACTION_LIST, transactionAliasGuid);
				ListViewFieldsClass listViewFields = new ListViewFieldsClass();

				foreach (ListViewClass listView in listViewCollection)
				{
					listView.ListViewFieldCollection = listViewFields.Enumerate(security, listView.IdentityGuid);
					ListViewFieldCollectionClass listViewFieldCollection = new ListViewFieldCollectionClass();
					int columnOrder = 0;
					bool found = false;

					foreach (ListViewFieldClass listViewField in listView.ListViewFieldCollection)
					{
						if (listViewField.Type == LISTVIEW_FIELD_TYPE.USER_DATA_FIELD && listViewField.TypeGuid == userDataField.IdentityGuid)
						{
							found = true;
							continue;
						}

						listViewField.ColumnOrder = columnOrder++;
						listViewFieldCollection.Add(listViewField);
					}

					if (found)
					{
						Guid siteGuid = security.SiteGuid;
						security.SiteGuid = listView.SiteGuid;
						listView.ListViewFieldCollection = listViewFieldCollection;

						try
						{
						    this.Modify(security, listView);
						}
						finally
						{
							security.SiteGuid = siteGuid;
						}
					}
				}

				listViewCollection = this.EnumerateAggregatesByAliasGuid(security, transactionAliasGuid);
				listViewFields = new ListViewFieldsClass();
				foreach (ListViewClass listView in listViewCollection)
				{
					listView.ListViewFieldCollection = listViewFields.Enumerate(security, listView.IdentityGuid);
					ListViewFieldCollectionClass listViewFieldCollection = new ListViewFieldCollectionClass();
					int columnOrder = 0;
					bool found = false;
					foreach (ListViewFieldClass listViewField in listView.ListViewFieldCollection)
					{
						if (listViewField.Type == LISTVIEW_FIELD_TYPE.USER_DATA_FIELD
						&& listViewField.DataPath == userDataField.DbName)
						{
							found = true;
							continue;
						}

						listViewField.ColumnOrder = columnOrder++;
						listViewFieldCollection.Add(listViewField);
					}

					if (found)
					{
						Guid siteGuid = security.SiteGuid;
						security.SiteGuid = listView.SiteGuid;
						listView.ListViewFieldCollection = listViewFieldCollection;

						try
						{
						    this.Modify(security, listView);
						}
						finally
						{
							security.SiteGuid = siteGuid;
						}
					}
				}
			}
		}
		#endregion

		#region Private methods
		private ListViewClass Get(SecurityClass security, LISTVIEW_TYPE listViewType, Guid listViewGuid, bool includeProductsAndGroups)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) &&
				!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.VIEW_BILLS_OF_LADING) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.PERFORM_CLOSEOUT) &&
				!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION) &&
				!security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION))
			{
				throw new FMInsufficientRightsException();
			}

			var listView = new ListViewClass
			{
				IdentityGuid = listViewGuid,
				Type = listViewType
			};

			using (var cmd = new SqlCommand())
			{
				listView.SelectSQL(cmd, ContextUtil.IsInTransaction);
				listView.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			var listViewFields = new ListViewFieldsClass();
			listView.ListViewFieldCollection = listViewFields.Enumerate(security, listView.IdentityGuid);

			if (includeProductsAndGroups)
			{
				var productMaps = new ProductMapsClass();
				listView.ProductMapCollection = productMaps.EnumerateByAssignedToGuidAndTypeInstr(
																									security,
																									listView.IdentityGuid,
																									PRODUCT_MAP_TYPE.LEDGER_VIEW_MAP,
																									false);

				var groupMaps = new GroupLedgerViewMapsClass();
				listView.GroupMapCollection = groupMaps.EnumerateByListViewGuid(security, listView.IdentityGuid);
			}

			return listView;
		}
		#endregion
	}
}