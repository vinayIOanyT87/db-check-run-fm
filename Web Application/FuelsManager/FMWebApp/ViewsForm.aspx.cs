// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ViewsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for ViewsForm.
	/// </summary>
	public partial class ViewsForm : FMAutoSubmitFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields
		private LISTVIEW_STANDARD_TYPE selectedListViewStandardType = LISTVIEW_STANDARD_TYPE.TYPE_MAX;
		private LISTVIEW_TYPE selectedListViewType = LISTVIEW_TYPE.TYPE_MAX;
		private ArrayList sameNamesInTwoTables;
		#endregion

		#region Explicit Interface Properties
		bool IEntityDiscovery.EntityAssignable
		{
			get { return true; }
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get { return typeof(IListViews); }
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				if (this.selectedListViewType == LISTVIEW_TYPE.STANDARD
				    && this.selectedListViewStandardType == LISTVIEW_STANDARD_TYPE.LEDGER)
				{
					return ENTITY_TYPE.LEDGER_VIEW;
				}

				return ENTITY_TYPE.LIST_VIEW;
			}
		}
		#endregion

		#region Public Methods and Operators
		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_SYSTEM_VIEWS,
						RootMenuName = "Configuration",
						CategoryName = "System",
						ItemName = "Views",
						NavigateUrl = "ViewsForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}
		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, 
																			ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			ListViewCollectionClass listViewCollection = FMChannelHelper.MakeCall<IListViews, ListViewCollectionClass>(
																x => x.Enumerate(security)
																);

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (ListViewClass listView in listViewCollection)
			{
				// Ledger views included elsewhere
				if (listView.Type == LISTVIEW_TYPE.STANDARD
				    && listView.TypeGuid == ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.LEDGER))
				{
					continue;
				}

				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == listView.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != listView.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != listView.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(listView);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string entityId)
		{
			LISTVIEW_STANDARD_TYPE listViewStandardType = ListViewClass.GetListViewStandardType(entityId);
			LISTVIEW_TYPE listViewType;
			Guid typeGuid;

			if (listViewStandardType != LISTVIEW_STANDARD_TYPE.TYPE_MAX)
			{
				listViewType = LISTVIEW_TYPE.STANDARD;
				typeGuid = ListViewClass.GetGuidFromStandardType(listViewStandardType);
			}
			else
			{
				listViewType = LISTVIEW_TYPE.TRANSACTION_LIST;
				typeGuid = FMChannelHelper.MakeCall<ITransactionAliases, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security,entityId)
																);
			}

            if (typeGuid == Guid.Empty)
            {
                //Extends the search to tblLedgerAggregateColumns
                typeGuid = FMChannelHelper.MakeCall<ILedgerAggregateColumns, Guid>(
                                                                     x =>
                                                                     x.GetIdentityGuid(security, entityId)
                                                                );
                if (typeGuid != Guid.Empty)
                    listViewType = LISTVIEW_TYPE.AGGREGATE;
            }

			return FMChannelHelper.MakeCall<IListViews, Guid>(
							x =>
							x.GetIdentityGuidByID(security, listViewType, typeGuid, entityId)
					);

		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			ListViewClass listView = FMChannelHelper.MakeCall<IListViews, ListViewClass>(
													x =>
													x.Get(security, this.selectedListViewType, guid)
											);

			listView.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IListViews>(x =>x.Modify(security,listView));
		}
		#endregion

		#region Methods
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				// A list of fields named the same in tables tblTransactions
				// and tblTransactionLineItems. We need this list assure we have
				// unique names in the GUI lists.
				this.sameNamesInTwoTables = new ArrayList
				                            {
					                            "Date01",
					                            "Date02",
					                            "Date03",
					                            "Date04",
					                            "Flag01",
					                            "Flag02",
					                            "Flag03",
					                            "Flag04",
					                            "Flag05",
					                            "Flag06",
					                            "Number01",
					                            "Number02",
					                            "Number03",
					                            "Number04",
					                            "Number05",
					                            "Number06",
					                            "LookupTransactionStatusIndex",
					                            "DeleteFlag"
				                            };

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					    || this.Security.SiteGuid != this.Security.LoginSiteGuid) 
					{
						this.DownButton.Enabled = false;
						this.UpButton.Enabled = false;
						this.AssignColumnsButton.Enabled = false;
						this.UnassignColumnsButton.Enabled = false;
						this.SaveButton.Enabled = false;
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
                    {
						this.CreateDefaultViewsButton.Enabled = false;
					}
					
					// Populate the EntityDropdownList
					for (var listViewType = LISTVIEW_TYPE.TRANSACTION_LIST; listViewType < LISTVIEW_TYPE.TYPE_MAX; listViewType++)
					{
						var item = new ListItem(ListViewClass.ListViewTypeID(listViewType), ((int)listViewType).ToString());
						this.TypeDropDownList.Items.Add(item);

						if (this.Session["DataDictionaryViewsType"] != null
						    && (string)this.Session["DataDictionaryViewsType"] == item.Value)
						{
							this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
						}
					}

					this.TypeDropDownListSelectedIndexChanged(null, null);
                    this.cbeApply.Enabled = (this.IsViewTiedToChildTransactionAliasRv() && (this.AssignedColumnsListBox.Items.Count > 0));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method handles the functionality of when the type is changed in the type
		///    dropdown. It will retrieve the views for the selected type.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void TypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
            this.cbeApply.Enabled = false;

			if ((this.Session["DataDictionaryViewsType"] == null)
			    || ((string)this.Session["DataDictionaryViewsType"] != this.TypeDropDownList.SelectedValue))
			{
				this.Session.Remove("DataDictionaryViewsTypeGuid");
			}

			this.Session["DataDictionaryViewsType"] = this.TypeDropDownList.SelectedValue;
			var listViewType = (LISTVIEW_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);
			this.selectedListViewType = listViewType;

			this.ViewsDropDownList.Items.Clear();
			this.ViewsDropDownList.Translate = false;
			
			if (listViewType == LISTVIEW_TYPE.STANDARD)
			{
				bool bOrderEntry = this.CheckHardwareKey();

				// Skip the ledger type - it is handled by a dedicated summary page
				for (var standardType = LISTVIEW_STANDARD_TYPE.METER_RECONCILIATION_SUMMARY;
				     standardType < LISTVIEW_STANDARD_TYPE.TYPE_MAX;
				     standardType++)
				{
					// If Order Entry key is not present, do not allow for the configuration of views.
					if (bOrderEntry == false)
					{
						if (standardType == LISTVIEW_STANDARD_TYPE.ORDER || standardType == LISTVIEW_STANDARD_TYPE.ORDER_ASSOCIATED_TX)
						{
							continue;
						}
					}

					string typeID = ListViewClass.ListViewStandardTypeID(standardType);

					if (typeID != "Undefined")
					{
						string text = typeID;

						if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
						{
							text = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, typeID);
						}

						var item = new ListItem(text, ListViewClass.GetGuidFromStandardType(standardType).ToString());
						this.ViewsDropDownList.Items.Add(item);

						if ((this.Session["DataDictionaryViewsTypeGuid"] != null)
						    && ((string)this.Session["DataDictionaryViewsTypeGuid"] == item.Value))
						{
							this.ViewsDropDownList.SelectedIndex = this.ViewsDropDownList.Items.Count - 1;
						}
					}
				}
			}

			else if (listViewType == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
				ListViewCollectionClass listViewCollection = FMChannelHelper.MakeCall<IListViews, ListViewCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

				foreach (ListViewClass listView in listViewCollection)
				{
					if (listView.Type == LISTVIEW_TYPE.STANDARD || listView.Type == LISTVIEW_TYPE.AGGREGATE)
					{
						continue;
					}

					var item = new ListItem(listView.ID, listView.TypeGuid.ToString());
					this.ViewsDropDownList.Items.Add(item);

					if ((this.Session["DataDictionaryViewsTypeGuid"] != null)
					    && ((string)this.Session["DataDictionaryViewsTypeGuid"] == item.Value))
					{
						this.ViewsDropDownList.SelectedIndex = this.ViewsDropDownList.Items.Count - 1;
					}
				}

				TransactionAliasNameCollectionClass transactionAliasNameCollection =
					FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
							x =>
							x.EnumerateNamesOnly(this.Security, byUser: false)
					);

				foreach (TransactionAliasNameClass transactionAliasName in transactionAliasNameCollection)
				{
					if (this.ViewsDropDownList.Items.FindByText(transactionAliasName.AliasName) != null)
					{
						continue;
					}

					var item = new ListItem(transactionAliasName.AliasName, transactionAliasName.MasterRecordGuid.ToString());
					this.ViewsDropDownList.Items.Add(item);

					if ((this.Session["DataDictionaryViewsTypeIndex"] != null)
						&& ((string) this.Session["DataDictionaryViewsTypeIndex"] == item.Value))
					{
						this.ViewsDropDownList.SelectedIndex = this.ViewsDropDownList.Items.Count - 1;
					}
				}
			}
			else
			{
				// Add aggregate columns
				LedgerAggregateColumnCollectionClass aggregateCollection = 
					FMChannelHelper.MakeCall<ILedgerAggregateColumns, LedgerAggregateColumnCollectionClass>(
							x =>
							x.Enumerate(this.Security)
					);

				foreach (LedgerAggregateColumnClass column in aggregateCollection)
				{
					if (this.ViewsDropDownList.Items.FindByText(column.ID) != null)
					{
						continue;
					}

					var item = new ListItem(column.ID, column.IdentityGuid.ToString());
					this.ViewsDropDownList.Items.Add(item);

					if ((this.Session["DataDictionaryViewsTypeGuid"] != null)
					    && ((string)this.Session["DataDictionaryViewsTypeGuid"] == item.Value))
					{
						this.ViewsDropDownList.SelectedIndex = this.ViewsDropDownList.Items.Count - 1;
					}
				}
			}

			this.ViewsDropDownListSelectedIndexChanged(null, null);
            this.cbeApply.Enabled = (this.IsViewTiedToChildTransactionAliasRv() && (this.AssignedColumnsListBox.Items.Count > 0));
		}


        private bool IsViewTiedToChildTransactionAliasRv()
        {
            bool result = false;
	        if (this.TypeDropDownList.SelectedIndex < 0)
	        {
		        return false;
	        }

            var listViewType = (LISTVIEW_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);
            
			if (listViewType == LISTVIEW_TYPE.TRANSACTION_LIST)
            {
                if (this.ViewsDropDownList.SelectedIndex >= 0)
                {
                    Guid typeGuid = Guid.Parse(this.ViewsDropDownList.SelectedValue);
	                
					if (!this.IsMasterTransactionAliasRecord(typeGuid))
	                {
		                result = true;
	                }
                }
            }
            return result;
        }

		protected void UnassignColumnsButtonClick(object sender, EventArgs e)
		{
			ListItem assignedColumnItem;

			while ((assignedColumnItem = this.AssignedColumnsListBox.SelectedItem) != null)
			{
				this.AssignedColumnsListBox.Items.Remove(assignedColumnItem);
				assignedColumnItem.Selected = false;
				this.InsertUnassignedColumnItem(assignedColumnItem);

				char[] seperators = { ' ' };
				string[] strings = assignedColumnItem.Value.Split(seperators);
				var type = (LISTVIEW_FIELD_TYPE)Convert.ToInt32(strings[0]);
				Guid typeGuid = Guid.Parse(strings[1]);
				var listView = (ListViewClass)this.Session["ListView"];

				var listViewFieldCollection = new ListViewFieldCollectionClass();
				int columnOrder = 0;

				foreach (ListViewFieldClass listViewField in listView.ListViewFieldCollection)
				{
					if (listViewField.Type == type && listViewField.TypeGuid == typeGuid)
					{
						continue;
					}

					listViewField.ColumnOrder = columnOrder++;
					listViewFieldCollection.Add(listViewField);
				}

				listView.ListViewFieldCollection = listViewFieldCollection;
			}
            this.cbeApply.Enabled = (this.IsViewTiedToChildTransactionAliasRv() && (this.AssignedColumnsListBox.Items.Count > 0));
		}

		/// <summary>
		///    This method will retrieve the assigned and unassigned fields for the selected view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ViewsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.AssignedColumnsListBox.Items.Clear();
				this.UnassignedColumnsListBox.Items.Clear();

				if (this.ViewsDropDownList.SelectedIndex == -1)
				{
					return;
				}

				var listViewType = (LISTVIEW_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);
				this.Session["DataDictionaryViewsTypeGuid"] = this.ViewsDropDownList.SelectedValue;

				Guid typeGuid = Guid.Parse(this.ViewsDropDownList.SelectedValue);                

				Guid listViewGuid = FMChannelHelper.MakeCall<IListViews, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, listViewType, typeGuid)
																);

				ListViewClass listView;

				if (listViewGuid != Guid.Empty)
				{
					listView = FMChannelHelper.MakeCall<IListViews, ListViewClass>(
																	 x =>
																	 x.Get(this.Security, this.selectedListViewType, listViewGuid)
																);

					this.selectedListViewStandardType = LISTVIEW_STANDARD_TYPE.TYPE_MAX;
				}
				else
				{
					listView = new ListViewClass
					           {
						           ID = this.ViewsDropDownList.SelectedItem.Text,
						           Type = listViewType,
						           TypeGuid = typeGuid,
						           ListViewStandardType = ListViewClass.GetStandardTypeFromGuid(typeGuid)
					           };
					this.selectedListViewStandardType = listView.ListViewStandardType;
				}

				this.Session["ListView"] = listView;
				bool enable = !(listView.SiteGuid != this.Security.SiteGuid && listView.IdentityGuid != Guid.Empty)
				              || !this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);

				this.DownButton.Enabled = enable;
				this.UpButton.Enabled = enable;
				this.AssignColumnsButton.Enabled = enable;
				this.UnassignColumnsButton.Enabled = enable;
				this.SaveButton.Enabled = enable;

				TransactionAliasClass transAlias = null;

				if (listViewType == LISTVIEW_TYPE.TRANSACTION_LIST)
				{
					transAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
														x =>
														x.Get(this.Security,listView.TypeGuid, false)
												);
				}

				foreach (ListViewFieldClass listViewField in listView.ListViewFieldCollection)
				{
					string text = listViewField.ID;

					if (listViewField.Type == LISTVIEW_FIELD_TYPE.STANDARD_FIELD)
					{
						if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
						{
							text = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, listViewField.ID);
						}
					}

					// There are some fields that have the same name in both the Transaction table and
					// Transaction Line Item table. For those fields the word "Item" prefixes the field
					// name. Check for the same name.
					if (listViewField.DataPath.StartsWith("Item") && this.SameNamedField(text))
					{
						text = "Item" + text;
					}

					ListItem item = null;

					if ((listViewField.DataPath.StartsWith("From") || listViewField.DataPath.StartsWith("To"))
					    && (listViewField.DataPath.Contains("ManagerID") || listViewField.DataPath.Contains("OwnerID")
					        || listViewField.DataPath.Contains("CarrierID") || listViewField.DataPath.Contains("BillToID")
					        || listViewField.DataPath.Contains("ShipToID") || listViewField.DataPath.Contains("Product")
					        || listViewField.DataPath.Contains("StorageLocationID")))
					{
						string conjoinedDataPath;

						if (listViewField.DataPath.StartsWith("From"))
						{
							conjoinedDataPath = listViewField.DataPath.Replace("From", "To");
						}
						else
						{
							conjoinedDataPath = listViewField.DataPath.Replace("To", "From");
						}

						FieldClass[] fields;

						if (transAlias != null)
						{

							if ((conjoinedDataPath.Contains("Product") || conjoinedDataPath.Contains("StorageLocationID"))
							    && transAlias.MultipleLineItems)
							{
								fields = transAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.LINE_ITEMS);
							}
							else
							{
								fields = transAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);
							}

							foreach (FieldClass field in fields)
							{
								if (field.DbName == conjoinedDataPath)
								{
									if (listViewField.DataPath.StartsWith("From"))
									{
										item = new ListItem(
											listViewField.ID + "/" + field.DisplayName,
											((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD) + " " + listViewField.TypeGuid);
									}
									else
									{
										item = new ListItem(
											field.DisplayName + "/" + listViewField.ID,
											((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD) + " " + listViewField.TypeGuid);
									}
									break;
								}
							}
						}

						if (item == null)
						{
							item = new ListItem(
								listViewField.ID, ((int)listViewField.Type) + " " + listViewField.TypeGuid);
						}
					}
					else
					{

						if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
						{
							text = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, text);
						}
						item = new ListItem(text, ((int)listViewField.Type) + " " + listViewField.TypeGuid);
					}

					this.AssignedColumnsListBox.Items.Add(item);
				}

				if (listViewType == LISTVIEW_TYPE.STANDARD)
				{
					STANDARD_FIELD_TYPE[] fields =
						ListViewClass.GetStandardViewFields(ListViewClass.GetStandardTypeFromGuid(listView.TypeGuid));

					if (FMChannelHelper.MakeCall<IHardwareKey, Boolean>(x =>x.IsADFKey()))
					{
						// For associated tx, also add location
						var adfFields = new STANDARD_FIELD_TYPE[fields.Length + 2];
						adfFields[fields.Length] = STANDARD_FIELD_TYPE.SITE;

						// Add alternative net volume for receipt association to invoices (payment assoc)
						adfFields[fields.Length + 1] = STANDARD_FIELD_TYPE.ALTERNATIVE_NET_VOLUME;

						for (int i = 0; i < fields.Length; adfFields[i] = fields[i++])
						{
							;
						}

						fields = adfFields;
					}

					foreach (STANDARD_FIELD_TYPE field in fields)
					{
						if (!ListViewFieldClass.IsFieldHidden(field))
						{
							string text = ListViewFieldClass.StandardFieldTypeID(field, true);

							if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
							{
								text = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, ListViewFieldClass.StandardFieldTypeID(field, true));
							}

							var item = new ListItem(text,
													((int)LISTVIEW_FIELD_TYPE.STANDARD_FIELD) + " "
													+ ListViewFieldClass.GetGuidFromStandardFieldType(field));

							if (this.AssignedColumnsListBox.Items.FindByValue(item.Value) == null)
							{
								this.InsertUnassignedColumnItem(item);
							}
						}
					}

					if (typeGuid == ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.INVENTORY_RECONCILIATION))
					{
						TransactionAliasNameCollectionClass transactionAliasNameCollection =
							FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
									x =>
									x.EnumerateNamesOnly(this.Security, byUser: false)
							);

						foreach (TransactionAliasNameClass transactionAliasName in transactionAliasNameCollection)
						{
							var item = new ListItem(transactionAliasName.AliasName, 
													((int) LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS) + " " + transactionAliasName.MasterRecordGuid);

							if (this.AssignedColumnsListBox.Items.FindByText(item.Text) == null)
							{
								this.InsertUnassignedColumnItem(item);
							}
						}
					}
				}
				else if (listViewType == LISTVIEW_TYPE.TRANSACTION_LIST)
				{
					this.AddTransactionAliasFields(transAlias);
				}
				else if (listViewType == LISTVIEW_TYPE.AGGREGATE)
				{
					LedgerAggregateColumnClass aggregateColumn = FMChannelHelper.MakeCall<ILedgerAggregateColumns, LedgerAggregateColumnClass>(
																					 x =>
																					 x.GetByColumnGuid(this.Security, listView.TypeGuid)
																				);

					foreach (LedgerAggregateColumnMapClass columnMap in aggregateColumn.Aliases)
					{
						TransactionAliasClass alias = this.GetAlias(this.Security, columnMap.TransactionAliasGuid, false);
						this.AddTransactionAliasFields(alias);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private TransactionAliasClass GetAlias(SecurityClass securityClass, Guid guid, bool param)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(securityClass, guid, param)
																);
		}

		private void AddTransactionAliasFields(TransactionAliasClass transactionAlias)
		{
			FieldClass[] fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);
			
			foreach (FieldClass field in fields)
			{
				// Exclude Virtual Fields
				if (field.VirtualField && !this.SupportedVirtualField(field))
				{
					continue;
				}

				ListItem item = null;

				var transAliasField = field as TransactionAliasFieldClass;

				if (transAliasField != null)
				{
					var transactionAliasField = transAliasField;

					if (transactionAliasField.Type == TransactionFieldType.WeightReading)
					{
						continue;
					}

					if ((transactionAliasField.DbName.StartsWith("From") || transactionAliasField.DbName.StartsWith("To"))
					    && (transactionAliasField.DbName.Contains("ManagerID") || transactionAliasField.DbName.Contains("OwnerID")
					        || transactionAliasField.DbName.Contains("CarrierID") || transactionAliasField.DbName.Contains("BillToID")
					        || transactionAliasField.DbName.Contains("ShipToID") || transactionAliasField.DbName.Contains("Product")
					        || transactionAliasField.DbName.Contains("StorageLocationID")))
					{
						string conjoinedDbName;

						if (transactionAliasField.DbName.StartsWith("From"))
						{
							conjoinedDbName = "To" + transactionAliasField.DbName.Substring(4);
						}
						else
						{
							conjoinedDbName = "From" + transactionAliasField.DbName.Substring(2);
						}

						foreach (FieldClass conjoinedField in fields)
						{
							if (conjoinedField.DbName == conjoinedDbName)
							{
								if (transactionAliasField.DbName.StartsWith("From"))
								{
									item = new ListItem(
										transactionAliasField.DisplayName + "/" + conjoinedField.DisplayName,
										((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD) + " " + transactionAliasField.IdentityGuid);
								}
								else
								{
									item = new ListItem(
										conjoinedField.DisplayName + "/" + transactionAliasField.DisplayName,
										((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD) + " " + transactionAliasField.IdentityGuid);
								}
								break;
							}
						}

						if (item == null)
						{
							item = new ListItem(
								transactionAliasField.DisplayName,
								((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD) + " " + transactionAliasField.IdentityGuid);
						}
					}
					else
					{
						item = new ListItem(
							transactionAliasField.DisplayName,
							((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD) + " " + transactionAliasField.IdentityGuid);
					}
				}
				else
				{
					if (field.ID.ToUpper().Contains(BaseTransactionLineItemDO.UserDataLineItemKeyPrefix))
					{
						item = new ListItem(
							field.DisplayName,
							((int)LISTVIEW_FIELD_TYPE.LINE_ITEM_USER_DATA_FIELD) + " " + field.IdentityGuid);
					}

					else
					{
						item = new ListItem(
							field.DisplayName, ((int)LISTVIEW_FIELD_TYPE.USER_DATA_FIELD) + " " + field.IdentityGuid);
					}
				}

				if ((this.AssignedColumnsListBox.Items.FindByValue(item.Value) == null))
				{
					this.InsertUnassignedColumnItem(item);
				}
			}

			if (transactionAlias.MultipleLineItems)
			{
				fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.LINE_ITEMS);

				foreach (FieldClass field in fields)
				{
					if (field.VirtualField && !this.SupportedVirtualField(field))
					{
						continue;
					}

					ListItem item = null;

					if (field is UserDataFieldClass)
					{
						if (field.ID.ToUpper().Contains(BaseTransactionLineItemDO.UserDataLineItemKeyPrefix))
						{
							item = new ListItem(
								field.DisplayName,
								((int)LISTVIEW_FIELD_TYPE.LINE_ITEM_USER_DATA_FIELD) + " " + field.IdentityGuid);
						}
						else
						{
							item = new ListItem(
								field.DisplayName, ((int)LISTVIEW_FIELD_TYPE.USER_DATA_FIELD) + " " + field.IdentityGuid);
						}
					}
					else
					{
						if ((field.DbName.StartsWith("From") || field.DbName.StartsWith("To"))
						    && (field.DbName.Contains("Product") || field.DbName.Contains("StorageLocationID")))
						{
							string conjoinedDbName;

							if (field.DbName.StartsWith("From"))
							{
								conjoinedDbName = "To" + field.DbName.Substring(4);
							}
							else
							{
								conjoinedDbName = "From" + field.DbName.Substring(2);
							}

							foreach (FieldClass conjoinedField in fields)
							{
								if (conjoinedField.DbName == conjoinedDbName)
								{
									if (field.DbName.StartsWith("From"))
									{
										item = new ListItem(
											field.DisplayName + "/" + conjoinedField.DisplayName,
											((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD) + " " + field.IdentityGuid);
									}
									else
									{
										item = new ListItem(
											conjoinedField.DisplayName + "/" + field.DisplayName,
											((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD) + " " + field.IdentityGuid);
									}
									break;
								}
							}

							if (item == null)
							{
								item = new ListItem(
									field.DisplayName,
									((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD) + " " + field.IdentityGuid);
							}
						}
						else
						{
							string displayName = field.DisplayName;

							if (this.SameNamedField(displayName))
							{
								displayName = "Item" + displayName;
							}

							item = new ListItem(
								displayName, ((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD) + " " + field.IdentityGuid);
						}
					}

					if (this.AssignedColumnsListBox.Items.FindByValue(item.Value) == null)
					{
						this.InsertUnassignedColumnItem(item);
					}
				}
			}
		}

		private void AssignColumnsButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedColumnItem;

			while ((unassignedColumnItem = this.UnassignedColumnsListBox.SelectedItem) != null)
			{
				this.UnassignedColumnsListBox.Items.Remove(unassignedColumnItem);
				unassignedColumnItem.Selected = false;
				this.AssignedColumnsListBox.Items.Add(unassignedColumnItem);

				var listViewField = new ListViewFieldClass { ID = unassignedColumnItem.Text };

				char[] seperators = { ' ' };
				string[] strings = unassignedColumnItem.Value.Split(seperators);

				listViewField.Type = (LISTVIEW_FIELD_TYPE)Convert.ToInt32(strings[0]);
				listViewField.TypeGuid = Guid.Parse(strings[1]);
				listViewField.StandardFieldType = ListViewFieldClass.GetStandardFieldTypeFromGuid(listViewField.TypeGuid);

				var listView = (ListViewClass)this.Session["ListView"];
				listViewField.ColumnOrder = listView.ListViewFieldCollection.Count;
				listView.ListViewFieldCollection.Add(listViewField);
			}
            this.cbeApply.Enabled = (this.IsViewTiedToChildTransactionAliasRv() && (this.AssignedColumnsListBox.Items.Count > 0));
		}

		private bool CheckHardwareKey()
		{
			// Check the hardware key
			return FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsAnOrderEntryKey());
		}

		private void DownButtonCommand(object sender, CommandEventArgs e)
		{
			var listView = (ListViewClass)this.Session["ListView"];
			int itemIndex = this.AssignedColumnsListBox.Items.Count - 1;
			int beginIndex = 0;
			int endIndex = 0;
			bool beginFound = false;
			bool endFound = false;

			while (itemIndex >= 0)
			{
				if (!beginFound && this.AssignedColumnsListBox.Items[itemIndex].Selected)
				{
					beginIndex = itemIndex;
					beginFound = true;
				}

				if (beginFound)
				{
					if (!this.AssignedColumnsListBox.Items[itemIndex].Selected)
					{
						endIndex = itemIndex + 1;
						endFound = true;
					}

					else if (itemIndex == 0)
					{
						endIndex = itemIndex;
						endFound = true;
					}
				}

				itemIndex--;

				if (beginFound && endFound)
				{
					if (beginIndex < this.AssignedColumnsListBox.Items.Count - 1)
					{
						var endItem = new ListItem(	this.AssignedColumnsListBox.Items[beginIndex + 1].Text, 
													this.AssignedColumnsListBox.Items[beginIndex + 1].Value);
						var endListViewField = new ListViewFieldClass();
						endListViewField.Load(listView.ListViewFieldCollection[beginIndex + 1]);
						
						for (int index = beginIndex; index >= endIndex; index--)
						{
							listView.ListViewFieldCollection[index + 1].Load(listView.ListViewFieldCollection[index]);
							listView.ListViewFieldCollection[index + 1].ColumnOrder++;
							this.AssignedColumnsListBox.Items[index + 1].Text = this.AssignedColumnsListBox.Items[index].Text;
							this.AssignedColumnsListBox.Items[index + 1].Value = this.AssignedColumnsListBox.Items[index].Value;
							this.AssignedColumnsListBox.Items[index + 1].Selected = this.AssignedColumnsListBox.Items[index].Selected;
						}

						listView.ListViewFieldCollection[endIndex].Load(endListViewField);
						listView.ListViewFieldCollection[endIndex].ColumnOrder = endIndex;
						this.AssignedColumnsListBox.Items[endIndex].Text = endItem.Text;
						this.AssignedColumnsListBox.Items[endIndex].Value = endItem.Value;
						this.AssignedColumnsListBox.Items[endIndex].Selected = endItem.Selected;
					}

					beginFound = false;
					endFound = false;
				}
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AssignColumnsButton.Command += this.AssignColumnsButtonCommand;
			this.UpButton.Command			 += this.UpButtonCommand;
			this.DownButton.Command			 += this.DownButtonCommand;
			this.SaveButton.Command			 += this.SaveButtonCommand;
            this.CreateDefaultViewsButton.Command += this.CreateDefaultViewsButtonCommand;
		}

		private void InsertUnassignedColumnItem(ListItem item)
		{
			// Only insert if it is unique
			if (this.UnassignedColumnsListBox.Items.FindByText(item.Text) == null)
			{
				foreach (ListItem unassignedColumnItem in this.UnassignedColumnsListBox.Items)
				{
					if (String.Compare(unassignedColumnItem.Text, item.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.UnassignedColumnsListBox.Items.IndexOf(unassignedColumnItem);
						this.UnassignedColumnsListBox.Items.Insert(index, item);
						item = null;
						break;
					}
				}

				if (item != null)
				{
					this.UnassignedColumnsListBox.Items.Add(item);
				}
			}
		}

		private bool SameNamedField(string displayName)
		{
			bool theSame = false;

			foreach (string sameFieldName in this.sameNamesInTwoTables)
			{
				if (displayName.Equals(sameFieldName))
				{
					theSame = true;
					break;
				}
			}

			return theSame;
		}

		private void SaveButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				var listView = (ListViewClass)this.Session["ListView"];

				if (listView.ListViewFieldCollection.Count == 0)
				{
					if (listView.IdentityGuid != Guid.Empty)
					{
						FMChannelHelper.MakeCall<IListViews>(
																	 x =>
																	 x.Purge(this.Security, listView.Type, listView.IdentityGuid)
																);

						listView.IdentityGuid = Guid.Empty;
					}
				}
				else
				{
					if (listView.IdentityGuid == Guid.Empty)
					{
						listView.IdentityGuid = FMChannelHelper.MakeCall<IListViews, Guid>(
																	 x =>
																	 x.Add(this.Security,listView)
																);
					}
					else
					{
						FMChannelHelper.MakeCall<IListViews>(
																	 x =>
																	 x.Modify(this.Security, listView)
																);
					}

					this.Session["ListView"] = FMChannelHelper.MakeCall<IListViews, ListViewClass>(
																	 x =>
																	 x.Get(this.Security, listView.Type, listView.IdentityGuid)
																);

				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		public void PopupAlert(string message)
		{
			string alertString = "<script type=\"text/javascript\">\r\n<!--\r\n";
			alertString += "alert(\"" + HttpUtility.JavaScriptStringEncode(message) + "\");";
			alertString += "\r\n--></script>";

			ScriptManager.RegisterClientScriptBlock(
				this.Page,
				this.GetType(),
				"DefaultViews",
				alertString,
				false);
		}

		private void CreateDefaultViewsButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				string Msg = FMChannelHelper.MakeCall<IListViews, string>(
																x =>
																x.CreateDefaultListViews(this.Security)
													);
				PopupAlert(Msg);

				this.TypeDropDownListSelectedIndexChanged(null, null);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private bool SupportedVirtualField(FieldClass field)
		{
			if (field.DbName == "VolumeUnit" || field.DbName == "TemperatureUnit" || field.DbName == "DensityUnit"
			    || field.DbName == "MassUnit" || field.DbName == "LevelUnit" || field.DbName == "FlowUnit"
			    || field.DbName == "PressureUnit" || field.DbName == "MassPackageSize" || field.DbName == "VolumePackageSize"
			    || field.DbName == "PackageQuantity")
			{
				return true;
			}

			return false;
		}

		private void UpButtonCommand(object sender, CommandEventArgs e)
		{
			var listView = (ListViewClass)this.Session["ListView"];
			int itemIndex = 0;
			int beginIndex = 0;
			int endIndex = 0;
			bool beginFound = false;
			bool endFound = false;
			
			while (itemIndex < this.AssignedColumnsListBox.Items.Count)
			{
				if (!beginFound && this.AssignedColumnsListBox.Items[itemIndex].Selected)
				{
					beginIndex = itemIndex;
					beginFound = true;
				}

				if (beginFound)
				{
					if (!this.AssignedColumnsListBox.Items[itemIndex].Selected)
					{
						endIndex = itemIndex - 1;
						endFound = true;
					}

					else if (itemIndex == this.AssignedColumnsListBox.Items.Count - 1)
					{
						endIndex = itemIndex;
						endFound = true;
					}
				}

				itemIndex++;

				if (beginFound && endFound)
				{
					if (beginIndex > 0)
					{
						var endItem = new ListItem(	this.AssignedColumnsListBox.Items[beginIndex - 1].Text, 
													this.AssignedColumnsListBox.Items[beginIndex - 1].Value);
						var endListViewField = new ListViewFieldClass();
						endListViewField.Load(listView.ListViewFieldCollection[beginIndex - 1]);
						
						for (int index = beginIndex; index <= endIndex; index++)
						{
							listView.ListViewFieldCollection[index - 1].Load(listView.ListViewFieldCollection[index]);
							listView.ListViewFieldCollection[index - 1].ColumnOrder--;
							this.AssignedColumnsListBox.Items[index - 1].Text = this.AssignedColumnsListBox.Items[index].Text;
							this.AssignedColumnsListBox.Items[index - 1].Value = this.AssignedColumnsListBox.Items[index].Value;
							this.AssignedColumnsListBox.Items[index - 1].Selected = this.AssignedColumnsListBox.Items[index].Selected;
						}

						listView.ListViewFieldCollection[endIndex].Load(endListViewField);
						listView.ListViewFieldCollection[endIndex].ColumnOrder = endIndex;
						this.AssignedColumnsListBox.Items[endIndex].Text = endItem.Text;
						this.AssignedColumnsListBox.Items[endIndex].Value = endItem.Value;
						this.AssignedColumnsListBox.Items[endIndex].Selected = endItem.Selected;
					}

					beginFound = false;
					endFound = false;
				}
			}
		}


        /// <summary>
        /// Verifies if the Transaction Alias record for a given Transaction Alias, for the current site, corresponds to a Master Record Version or not.
        /// </summary>
        /// <param name="transAliasGuid">Guid of the Transaction Alias to be tested. This can be the exact Transaction Alias Guid or the Master Record Version Guid.</param>
        /// <returns></returns>
        private bool IsMasterTransactionAliasRecord(Guid transAliasGuid)
        {
            bool result = false;
            TransactionAliasClass transAlias =
                    FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
                            x =>
                            x.GetBasicInfo(this.Security, transAliasGuid, this.Security.SiteGuid)
                    );

	        if (transAlias != null)
	        {
		        result = (transAlias.IdentityGuid == transAlias.MasterRecordGuid);
	        }

            return result;
        }
        #endregion
    }
}