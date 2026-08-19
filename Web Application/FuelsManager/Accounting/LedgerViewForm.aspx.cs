// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedgerViewForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Accounting
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	public partial class LedgerViewForm : AccountingAutoSubmitWebFormViewAjax, IEntityDiscovery
	{
		#region Constants and Fields
		private ListViewClass view;
		#endregion

		#region Explicit Interface Properties
		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IListViews);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.LEDGER_VIEW;
			}
		}
		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass inSecurity, ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			Guid ledgerGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.LEDGER);

			ListViewCollectionClass listViewCollection =
				FMChannelHelper.MakeCall<IListViews, ListViewCollectionClass>(
					x => x.EnumerateByTypeAndTypeGuid(inSecurity, LISTVIEW_TYPE.STANDARD, ledgerGuid));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (ListViewClass listView in listViewCollection)
			{
				// Only Ledger views here
				if (listView.Type != LISTVIEW_TYPE.STANDARD || listView.TypeGuid != ledgerGuid)
				{
					continue;
				}

				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (inSecurity.SiteGuid == listView.SiteGuid)
					{
						continue;
					}

					if (inSecurity.LoginSiteGuid != listView.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (inSecurity.SiteGuid != listView.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(listView);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass inSecurity, string id)
		{
			return
				FMChannelHelper.MakeCall<IListViews, Guid>(
					x =>
					x.GetIdentityGuidByID(
						inSecurity, LISTVIEW_TYPE.STANDARD, ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.LEDGER), id));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass inSecurity, Guid guid, Guid siteGuid)
		{
			ListViewClass listView =
				FMChannelHelper.MakeCall<IListViews, ListViewClass>(x => x.Get(inSecurity, LISTVIEW_TYPE.STANDARD, guid));

			listView.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IListViews>(x => x.Modify(inSecurity, listView));
		}
		#endregion

		#region Methods
		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			this.Initialize();
			base.OnInit(e);
			this.InitializeComponents();
		}

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.NullReferenceException">Expected session to contain view object.</exception>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.view = (ListViewClass)this.Session[PageSessionKeyConstants.LEDGER_VIEW_OBJECT];

				this.SetControlStates();

				if (this.view == null)
				{
					throw new NullReferenceException("Expected session to contain view object.");
				}

				if (this.IsPostBack == false)
				{
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the AssignFieldButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void AssignFieldButtonClick(object sender, EventArgs e)
		{
			try
			{
				ListItem item;
				while ((item = this.AvailableFieldsList.SelectedItem) != null)
				{
					item.Selected = false;
					this.AvailableFieldsList.Items.Remove(item);
					this.SelectedFieldsList.Items.Add(item);

					var listViewField = new ListViewFieldClass { ID = item.Text };
					char[] seperators = { ' ' };
					string[] strings = item.Value.Split(seperators);
					listViewField.Type = (LISTVIEW_FIELD_TYPE)Convert.ToInt32(strings[0]);
					listViewField.TypeGuid = Guid.Parse(strings[1]);
					listViewField.StandardFieldType = ListViewFieldClass.GetStandardFieldTypeFromGuid(listViewField.TypeGuid);

					listViewField.ColumnOrder = this.view.ListViewFieldCollection.Count;
					this.view.ListViewFieldCollection.Add(listViewField);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the AssignGroupButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void AssignGroupButtonClick(object sender, EventArgs e)
		{
			try
			{
				ListItem item;
				while ((item = this.AvailableGroupsList.SelectedItem) != null)
				{
					this.AvailableGroupsList.Items.Remove(item);
					item.Selected = false;
					this.SelectedGroupsList.Items.Add(item);

					Guid groupGuid = Guid.Parse(item.Value);

					var groupMap = new GroupLedgerViewMapClass { GroupGuid = groupGuid };
					this.view.GroupMapCollection.Add(groupMap);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the AssignProductButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void AssignProductButtonClick(object sender, EventArgs e)
		{
			try
			{
				ListItem item;
				while ((item = this.AvailableProductsList.SelectedItem) != null)
				{
					this.AvailableProductsList.Items.Remove(item);
					item.Selected = false;
					this.SelectedProductsList.Items.Add(item);

					var productMap = new ProductMapClass
						{
							AssignedID = item.Text,
							AssignedGuid = Guid.Parse(item.Value),
							Type = PRODUCT_MAP_TYPE.LEDGER_VIEW_MAP
						};

					this.view.ProductMapCollection.Add(productMap);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the CancelButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void CancelButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove(PageSessionKeyConstants.LEDGER_VIEW_OBJECT);
				
				this.Redirect("LedgerViewsForm.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Initializes the components.
		/// </summary>
		private void InitializeComponents()
		{
			this.NewButton.Click			+= this.NewButtonClick;
			this.OKButton.Click				+= this.OkButtonClick;
			this.CancelButton.Click			+= this.CancelButtonClick;
			this.AssignFieldButton.Click	+= this.AssignFieldButtonClick;
			this.RemoveFieldButton.Click	+= this.RemoveFieldButtonClick;
			this.MoveUpButton.Click			+= this.MoveUpButtonClick;
			this.MoveDownButton.Click		+= this.MoveDownButtonClick;
			this.AssignProductButton.Click	+= this.AssignProductButtonClick;
			this.RemoveProductButton.Click	+= this.RemoveProductButtonClick;
			this.AssignGroupButton.Click	+= this.AssignGroupButtonClick;
			this.RemoveGroupButton.Click	+= this.RemoveGroupButtonClick;
		}

		/// <summary>
		/// Inserts the unassigned column item.
		/// </summary>
		/// <param name="item">The item.</param>
		private void InsertUnassignedColumnItem(ListItem item)
		{
			foreach (ListItem unassignedColumnItem in this.AvailableFieldsList.Items)
			{
				if (string.Compare(unassignedColumnItem.Text, item.Text, StringComparison.Ordinal) > 0)
				{
					int index = this.AvailableFieldsList.Items.IndexOf(unassignedColumnItem);
					this.AvailableFieldsList.Items.Insert(index, item);
					item = null;
					break;
				}
			}

			if (item != null)
			{
				this.AvailableFieldsList.Items.Add(item);
			}
		}

		/// <summary>
		/// Handles the Click event of the MoveDownButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void MoveDownButtonClick(object sender, EventArgs e)
		{
			try
			{
				int itemIndex = this.SelectedFieldsList.Items.Count - 1;
				int beginIndex = 0;
				int endIndex = 0;
				bool beginFound = false;
				bool endFound = false;
				while (itemIndex >= 0)
				{
					if (!beginFound && this.SelectedFieldsList.Items[itemIndex].Selected)
					{
						beginIndex = itemIndex;
						beginFound = true;
					}

					if (!endFound && beginFound)
					{
						if (!this.SelectedFieldsList.Items[itemIndex].Selected)
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
						if (beginIndex < this.SelectedFieldsList.Items.Count - 1)
						{
							var endItem = new ListItem(
								this.SelectedFieldsList.Items[beginIndex + 1].Text, this.SelectedFieldsList.Items[beginIndex + 1].Value);

							var endListViewField = new ListViewFieldClass();
							endListViewField.Load(this.view.ListViewFieldCollection[beginIndex + 1]);

							for (int index = beginIndex; index >= endIndex; index--)
							{
								this.view.ListViewFieldCollection[index + 1].Load(this.view.ListViewFieldCollection[index]);
								this.view.ListViewFieldCollection[index + 1].ColumnOrder++;
								this.SelectedFieldsList.Items[index + 1].Text = this.SelectedFieldsList.Items[index].Text;
								this.SelectedFieldsList.Items[index + 1].Value = this.SelectedFieldsList.Items[index].Value;
								this.SelectedFieldsList.Items[index + 1].Selected = this.SelectedFieldsList.Items[index].Selected;
							}

							this.view.ListViewFieldCollection[endIndex].Load(endListViewField);
							this.view.ListViewFieldCollection[endIndex].ColumnOrder = endIndex;
							this.SelectedFieldsList.Items[endIndex].Text = endItem.Text;
							this.SelectedFieldsList.Items[endIndex].Value = endItem.Value;
							this.SelectedFieldsList.Items[endIndex].Selected = endItem.Selected;
						}

						beginFound = false;
						endFound = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the MoveUpButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void MoveUpButtonClick(object sender, EventArgs e)
		{
			try
			{
				int itemIndex = 0;
				int beginIndex = 0;
				int endIndex = 0;
				bool beginFound = false;
				bool endFound = false;
				while (itemIndex < this.SelectedFieldsList.Items.Count)
				{
					if (!beginFound && this.SelectedFieldsList.Items[itemIndex].Selected)
					{
						beginIndex = itemIndex;
						beginFound = true;
					}

					if (!endFound && beginFound)
					{
						if (!this.SelectedFieldsList.Items[itemIndex].Selected)
						{
							endIndex = itemIndex - 1;
							endFound = true;
						}
						else if (itemIndex == this.SelectedFieldsList.Items.Count - 1)
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
							var endItem = new ListItem(
								this.SelectedFieldsList.Items[beginIndex - 1].Text, this.SelectedFieldsList.Items[beginIndex - 1].Value);

							var endListViewField = new ListViewFieldClass();

							endListViewField.Load(this.view.ListViewFieldCollection[beginIndex - 1]);

							for (int index = beginIndex; index <= endIndex; index++)
							{
								this.view.ListViewFieldCollection[index - 1].Load(this.view.ListViewFieldCollection[index]);
								this.view.ListViewFieldCollection[index - 1].ColumnOrder--;
								this.SelectedFieldsList.Items[index - 1].Text = this.SelectedFieldsList.Items[index].Text;
								this.SelectedFieldsList.Items[index - 1].Value = this.SelectedFieldsList.Items[index].Value;
								this.SelectedFieldsList.Items[index - 1].Selected = this.SelectedFieldsList.Items[index].Selected;
							}

							this.view.ListViewFieldCollection[endIndex].Load(endListViewField);
							this.view.ListViewFieldCollection[endIndex].ColumnOrder = endIndex;
							this.SelectedFieldsList.Items[endIndex].Text = endItem.Text;
							this.SelectedFieldsList.Items[endIndex].Value = endItem.Value;
							this.SelectedFieldsList.Items[endIndex].Selected = endItem.Selected;
						}

						beginFound = false;
						endFound = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the NewButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void NewButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Save();

				var listViewClass = new ListViewClass
					{
						Type = LISTVIEW_TYPE.STANDARD,
						TypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.LEDGER),
						ListViewStandardType = LISTVIEW_STANDARD_TYPE.LEDGER
					};

				this.Session[PageSessionKeyConstants.LEDGER_VIEW_OBJECT] = listViewClass;
				
				this.Redirect("LedgerViewForm.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the OKButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void OkButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Save();
				
				this.Redirect("LedgerViewsForm.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Populates the aggregate fields.
		/// </summary>
		private void PopulateAggregateFields()
		{
			LedgerAggregateColumnCollectionClass columns =
				FMChannelHelper.MakeCall<ILedgerAggregateColumns, LedgerAggregateColumnCollectionClass>(
					x => x.Enumerate(this.security));

			foreach (LedgerAggregateColumnClass column in columns)
			{
				var item = new ListItem(
					column.ID, ((int)LISTVIEW_FIELD_TYPE.AGGREGATE_FIELD).ToString(CultureInfo.InvariantCulture) + " " + column.IdentityGuid);

				if (this.SelectedFieldsList.Items.FindByText(item.Text) == null)
				{
					this.InsertUnassignedColumnItem(item);
				}
			}
		}

		/// <summary>
		/// Populates the alias fields.
		/// </summary>
		private void PopulateAliasFields()
		{
			var transactionAliasNameCollection =
				FMChannelHelper.MakeCall<ITransactionAliases, List<TransactionAliasNameClass>>(
							transactionAliases => transactionAliases.EnumerateNamesOnly(this.security, false));

			foreach (TransactionAliasNameClass transactionAliasName in transactionAliasNameCollection)
			{
				var item = new ListItem(
					transactionAliasName.AliasName, 
					((int)LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS).ToString(CultureInfo.InvariantCulture) + " " + transactionAliasName.MasterRecordGuid);

				if (this.SelectedFieldsList.Items.FindByText(item.Text) == null)
				{
					this.InsertUnassignedColumnItem(item);
				}
			}
		}

		/// <summary>
		/// Populates the fields.
		/// </summary>
		private void PopulateFields()
		{
			this.SelectedFieldsList.Items.Clear();
			this.AvailableFieldsList.Items.Clear();

			this.PopulateSelectedFields();
			this.PopulateStandardFields();

			this.PopulateAliasFields();
			this.PopulateAggregateFields();
		}

		/// <summary>
		/// Populates the products.
		/// </summary>
		private void PopulateProducts()
		{
			this.SelectedProductsList.DataTextField = "AssignedID";
			this.SelectedProductsList.DataValueField = "AssignedGuid";
			this.SelectedProductsList.DataSource = this.view.ProductMapCollection;
			this.SelectedProductsList.DataBind();

			ProductCollectionClass productCollection =
				FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.security));

			var dataCollection = new ProductCollectionClass();

			foreach (ProductClass product in productCollection)
			{
				if (this.SelectedProductsList.Items.FindByValue(product.MasterRecordGuid.ToString()) == null)
				{
					dataCollection.Add(product);
				}
			}

			this.AvailableProductsList.DataTextField = "ID";
			this.AvailableProductsList.DataValueField = "MasterRecordGuid";
			this.AvailableProductsList.DataSource = dataCollection;
			this.AvailableProductsList.DataBind();
		}

		/// <summary>
		/// The populate selected fields.
		/// </summary>
		/// <param name="dictionaries">
		/// The data dictionaries.
		/// </param>
		private void PopulateSelectedFields()
		{
			foreach (ListViewFieldClass listViewField in this.view.ListViewFieldCollection)
			{
				string text = listViewField.ID;

				if (listViewField.Type == LISTVIEW_FIELD_TYPE.STANDARD_FIELD)
				{
					if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
					{
						text = GetDataDictionaryValueByKey(this.security.LoginSiteGuid, listViewField.ID);
					}
				}

				// There are some fields that have the same name in both the Transaction table and
				// Transaction Line Item table. For those fields the word "Item" prefixes the field
				// name. Check for the same name.
				if (listViewField.DataPath.StartsWith("Item"))
				{
					text = "Item" + text;
				}

				var item = new ListItem(text, ((int)listViewField.Type).ToString(CultureInfo.InvariantCulture) + " " + listViewField.TypeGuid);
				this.SelectedFieldsList.Items.Add(item);
			}
		}

		/// <summary>
		/// Populates the standard fields.
		/// </summary>
		/// <param name="dictionaries">The data dictionaries.</param>
		private void PopulateStandardFields()
		{
			STANDARD_FIELD_TYPE[] fields = ListViewClass.GetStandardViewFields(LISTVIEW_STANDARD_TYPE.LEDGER);

			foreach (STANDARD_FIELD_TYPE field in fields)
			{
				string text = ListViewFieldClass.StandardFieldTypeID(field, true);

				if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
				{
					text = GetDataDictionaryValueByKey(this.security.LoginSiteGuid, ListViewFieldClass.StandardFieldTypeID(field, true));
				}

				var item = new ListItem(
					text,
					((int)LISTVIEW_FIELD_TYPE.STANDARD_FIELD).ToString(CultureInfo.InvariantCulture) + " " + ListViewFieldClass.GetGuidFromStandardFieldType(field));

				if (this.SelectedFieldsList.Items.FindByText(item.Text) == null)
				{
					this.InsertUnassignedColumnItem(item);
				}
			}
		}

		/// <summary>
		/// Populates the user groups.
		/// </summary>
		private void PopulateUserGroups()
		{
			GroupCollectionClass groupCollection =
				FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(x => x.Enumerate(this.security));

			var dataCollection = new GroupCollectionClass();
			var selectedCollection = new GroupCollectionClass();

			foreach (GroupClass group in groupCollection)
			{
				if (this.view.GroupMapCollection.FindByGroupGuid(group.IdentityGuid) == null)
				{
					dataCollection.Add(group);
				}
				else
				{
					selectedCollection.Add(group);
				}
			}

			this.AvailableGroupsList.DataTextField = "ID";
			this.AvailableGroupsList.DataValueField = "IdentityGuid";
			this.AvailableGroupsList.DataSource = dataCollection;
			this.AvailableGroupsList.DataBind();

			this.SelectedGroupsList.DataTextField = "ID";
			this.SelectedGroupsList.DataValueField = "IdentityGuid";
			this.SelectedGroupsList.DataSource = selectedCollection;
			this.SelectedGroupsList.DataBind();
		}

		/// <summary>
		/// Handles the Click event of the RemoveFieldButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void RemoveFieldButtonClick(object sender, EventArgs e)
		{
			try
			{
				ListItem item;
				while ((item = this.SelectedFieldsList.SelectedItem) != null)
				{
					item.Selected = false;
					this.SelectedFieldsList.Items.Remove(item);
					this.AvailableFieldsList.Items.Add(item);

					char[] seperators = { ' ' };
					string[] strings = item.Value.Split(seperators);
					var listViewType = (LISTVIEW_FIELD_TYPE)Convert.ToInt32(strings[0]);
					Guid typeGuid = Guid.Parse(strings[1]);

					var listViewFieldCollection = new ListViewFieldCollectionClass();
					int columnOrder = 0;

					foreach (ListViewFieldClass listViewField in this.view.ListViewFieldCollection)
					{
						if (listViewField.Type == listViewType && listViewField.TypeGuid == typeGuid)
						{
							continue;
						}

						listViewField.ColumnOrder = columnOrder++;
						listViewFieldCollection.Add(listViewField);
					}

					this.view.ListViewFieldCollection = listViewFieldCollection;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Removes the group button click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void RemoveGroupButtonClick(object sender, EventArgs e)
		{
			try
			{
				ListItem item;
				while ((item = this.SelectedGroupsList.SelectedItem) != null)
				{
					this.SelectedGroupsList.Items.Remove(item);
					item.Selected = false;
					this.AvailableGroupsList.Items.Add(item);

					Guid selectedGuid = Guid.Parse(item.Value);

					for (int index = 0; index < this.view.GroupMapCollection.Count; ++index)
					{
						GroupLedgerViewMapClass groupMap = this.view.GroupMapCollection[index];

						if (groupMap.GroupGuid == selectedGuid)
						{
							this.view.GroupMapCollection.RemoveAt(index);
							break;
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the RemoveProductButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void RemoveProductButtonClick(object sender, EventArgs e)
		{
			try
			{
				ListItem item;
				while ((item = this.SelectedProductsList.SelectedItem) != null)
				{
					this.SelectedProductsList.Items.Remove(item);
					item.Selected = false;
					this.AvailableProductsList.Items.Add(item);

					Guid selectedGuid = Guid.Parse(item.Value);

					for (int index = 0; index < this.view.ProductMapCollection.Count; ++index)
					{
						ProductMapClass productMap = this.view.ProductMapCollection[index];

						if (productMap.AssignedGuid == selectedGuid)
						{
							this.view.ProductMapCollection.RemoveAt(index);
							break;
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Saves the view.
		/// </summary>
		/// <exception cref="System.ApplicationException">Name field is required.</exception>
		private void Save()
		{
			if (string.IsNullOrEmpty(this.NameTextBox.Text))
			{
				throw new ApplicationException("Name field is required.");
			}

			this.view.ID = this.NameTextBox.Text;

			if (this.view.IdentityGuid != Guid.Empty)
			{
				FMChannelHelper.MakeCall<IListViews>(x => x.Modify(this.security, this.view));
			}
			else
			{
				FMChannelHelper.MakeCall<IListViews, Guid>(x => x.Add(this.security, this.view));
			}
		}

		/// <summary>
		/// Sets the control states.
		/// </summary>
		private void SetControlStates()
		{
			if (this.view.SiteGuid != this.security.SiteGuid && this.view.IdentityGuid != Guid.Empty)
			{
				this.AssignFieldButton.Enabled = false;
				this.RemoveFieldButton.Enabled = false;

				this.AssignGroupButton.Enabled = false;
				this.RemoveGroupButton.Enabled = false;

				this.AssignProductButton.Enabled = false;
				this.RemoveProductButton.Enabled = false;

				this.MoveDownButton.Enabled = false;
				this.MoveUpButton.Enabled = false;

				this.OKButton.Enabled = false;
				this.NewButton.Enabled = false;
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			this.NameTextBox.Text = this.view.ID;

			// Set the title label with a key field from the bound object appended
			if (this.view != null)
			{
				this.TitleLabel.Text = this.GetTitleLabelText(this.TitleLabel.Text, this.view.ID);
			}

			this.PopulateFields();
			this.PopulateProducts();
			this.PopulateUserGroups();
		}
		#endregion
	}
}