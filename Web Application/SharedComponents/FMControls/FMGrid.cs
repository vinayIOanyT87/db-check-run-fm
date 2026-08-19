// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMGrid.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMGrid type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	using FMBusinessObjects.UtilityObjects;
	using System.Data;
	using System.Globalization;
	using System.Threading;

	/// <summary>
	/// The FuelsManager grid.
	/// </summary>
	public class FMGrid : DataGrid
	{
		#region Private Attributes
		private bool editColumn;
		private bool selectColumn;
		private bool printColumn;
		private string deleteBtnName = "DeleteButton";
		private SecurityClass securityObj;
		private System.Data.DataSet dataSet;
		private FMPageSizeDropDown associatedDropdown;
		#endregion

		#region Protected Attributes
		protected SiteClass loginSite;
		protected const string DATA_DICTIONARY_KEY = "DataDictionaryKey";
		protected const string TOOL_TIP_KEYS = "ToolTipKeys";
		#endregion

		#region Public Attributes
		public const string SORT_EXPRESSION = "SortExpression";
		public const string SORT_DIRECTION = "SortDirection";
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="FMGrid"/> class.
		/// </summary>
		public FMGrid()
		{
			this.UseAccessibleHeader = true;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will get and set the delete button name. The
		/// name is used with displaying the delete column and disabling it.
		/// </summary>
		public string DeleteButtonName
		{
			get
			{
				return this.deleteBtnName;
			}

			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.deleteBtnName = value;
				}
			}
		}

		/// <summary>
		/// This property sets whether or not the edit column
		/// should appear.
		/// </summary>
		public bool EditColumn
		{
			get { return this.editColumn; }
			set { this.editColumn = value; }
		}


		/// <summary>
		/// This property sets whether or not the select column
		/// should appear.
		/// </summary>
		public bool SelectColumn
		{
			get { return this.selectColumn; }
			set { this.selectColumn = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether print column.
		/// </summary>
		public bool PrintColumn
		{
			get { return this.printColumn; }
			set { this.printColumn = value; }
		}

		/// <summary>
		/// This property sets the security object. If it is null,
		/// then an new empty class is created.
		/// </summary>
		public SecurityClass SecurityObject
		{
			set
			{
				this.securityObj = value ?? new SecurityClass();

				const bool GetSchedulesFlag = false;
				const bool GetMemberSites = false;
				const bool GetAssociatedAliases = false;

				this.loginSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
					x =>
					x.Get(this.securityObj, this.securityObj.LoginSiteGuid, GetMemberSites, GetSchedulesFlag, GetAssociatedAliases));
			}
		}

		/// <summary>
		/// This property will set the data set attribute with a 
		/// valid data set object.
		/// </summary>
		public DataSet DataSet
		{
			set
			{
				this.dataSet = value ?? new DataSet();
			}
		}

		/// <summary>
		/// This propert will set the associated page size dropdown control
		/// ID. It will be used to save the page size.
		/// </summary>
		public FMPageSizeDropDown AssociatedDropdown
		{
			set { this.associatedDropdown = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will create the columns for the grid. It uses the ListView classes
		/// to get the columns to create and will create the edit type columns if requested.
		/// It is implemented by the derived class.
		/// </summary>
		/// <param name="listViewType"></param>
		/// <param name="aliasGuid"></param>
		/// <param name="productName"></param>
		public virtual void InitializeGridColumns(LISTVIEW_TYPE listViewType, Guid aliasGuid, string productName)
		{
		}

		/// <summary>
		/// This method will update the data view and bind to the grid.
		/// </summary>
		public void UpdateView()
		{
			var view = new DataView(this.dataSet.Tables[0]);

			if ((this.Page.Session[this.ID + SORT_EXPRESSION] != null) && (this.Page.Session[this.ID + SORT_DIRECTION] != null))
			{
				view.Sort = (string)this.Page.Session[this.ID + SORT_EXPRESSION] + " " + (string)this.Page.Session[this.ID + SORT_DIRECTION];
			}

			CultureInfo oldCulture = Thread.CurrentThread.CurrentCulture;
			var culture = new CultureInfo(CultureInfo.CurrentCulture.LCID)
				              {
					              NumberFormat =
						              {
							              NumberDecimalSeparator = this.loginSite.NumberDecimalSeparator,
							              NumberGroupSeparator = this.loginSite.NumberGroupSeparator,
							              NumberGroupSizes = this.loginSite.GetNumberGroupSizes(),
							              NumberNegativePattern = 0,
							              NegativeSign = string.Empty
						              },
					              DateTimeFormat = this.loginSite.GetDateTimeFormatInfo()
				              };

			Thread.CurrentThread.CurrentCulture = culture;

			this.DataSource = view;

			if (((view.Table.Rows.Count - 1) / this.PageSize) < this.CurrentPageIndex)
			{
                int calculationResult = (view.Table.Rows.Count - 1) / this.PageSize;

                this.CurrentPageIndex = calculationResult < 0 ? 0 : calculationResult;
            }

			// Save the page size in session.
			if (this.associatedDropdown != null)
			{
				this.associatedDropdown.SetPageSize(this, view.Count);
			}

			this.DataBind();
			Thread.CurrentThread.CurrentCulture = oldCulture;
		}

		#endregion

		#region Private Methods
		/// <summary>
		/// This method will return a tool tip for any company column.
		/// </summary>
		/// <param name="index">
		/// </param>
		/// <param name="view">
		/// The view.
		/// </param>
		/// <param name="role">
		/// The role.
		/// </param>
		/// <returns>
		/// </returns>
		private string GetToolTip(int index, DataView view, COMPANY_ROLE role)
		{
			string companyRole = null;
			string toolTip = string.Empty;

			switch (role)
			{
				case COMPANY_ROLE.MANAGER:
					companyRole = "Manager";
					break;
				case COMPANY_ROLE.OWNER:
					companyRole = "Owner";
					break;
				case COMPANY_ROLE.SHIPPER:
					companyRole = "Shipper";
					break;
				case COMPANY_ROLE.CUSTOMER_BILLTO:
					companyRole = "BillTo";
					break;
				case COMPANY_ROLE.CUSTOMER_SHIPTO:
					companyRole = "ShipTo";
					break;
				case COMPANY_ROLE.CARRIER:
					companyRole = "Carrier";
					break;
				case COMPANY_ROLE.SUPPLIER:
					companyRole = "Supplier";
					break;
			}

			var name = view[index][companyRole + "Name"] as string;
			if (!string.IsNullOrEmpty(name))
			{
				toolTip += name;
			}

			var address = view[index][companyRole + "Address"] as string;
			if (!string.IsNullOrEmpty(address))
			{
				toolTip += ", " + address;
			}

			var city = view[index][companyRole + "City"] as string;
			if (!string.IsNullOrEmpty(city))
			{
				toolTip += ", " + city;
			}

			var state = view[index][companyRole + "State"] as string;
			if (!string.IsNullOrEmpty(state))
			{
				toolTip += ", " + state;
			}

			return toolTip;
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// This method will remove all the columns with the exception of the
		/// edit, delete, or select columns if they exist.
		/// </summary>
		protected void RemoveColumns()
		{
			int count = this.Columns.Count - 1;

			// Remove all columns with the exception of edit, delete, 
			// select or print bol. Check to see if the edit, delete, or select columns exist.
			// get the strings from the datadictionary for the column headers
			Guid siteGuid = (Guid)this.Page.Session["SiteGuid"];
			string editText = this.GetDataDictionaryValueByKey(siteGuid, "Edit");
			string selectText = this.GetDataDictionaryValueByKey(siteGuid, "Select");
			string printBolText = this.GetDataDictionaryValueByKey(siteGuid, "Multiple Select");

			for (int index = count; index >= 0; index--)
			{
				DataGridColumn column = this.Columns[index];

				if (column.HeaderText.ToUpper() == "EDIT" ||
					column.HeaderText == editText)
				{
					if (this.editColumn == false)
					{
						this.Columns.RemoveAt(index);
					}
					else
					{
						if (column.HeaderText.ToUpper() == "EDIT" &&
							editText.CompareTo("Edit") != 0)
						{
							column.HeaderText = editText;
						}
					}
				}
				else if (column.HeaderText.ToUpper() == "MULTIPLE SELECT" ||
					column.HeaderText == printBolText)
				{
					if (this.printColumn == false)
					{
						this.Columns.RemoveAt(index);
					}
					else
					{
						if (column.HeaderText.ToUpper() == "MULTIPLE SELECT" &&
							printBolText.CompareTo("Multiple Select") != 0)
						{
							column.HeaderText = printBolText;
						}
					}
				}
				else if (column.HeaderText.ToUpper() == "SELECT" ||
					column.HeaderText == selectText)
				{
					if (this.selectColumn == false)
					{
						this.Columns.RemoveAt(index);
					}
					else
					{
						if (column.HeaderText.ToUpper() == "SELECT" &&
							selectText.CompareTo("Select") != 0)
						{
							column.HeaderText = selectText;
						}
					}
				}
				else
				{
					this.Columns.RemoveAt(index);
				}
			}
		}

		/// <summary>
		/// The get data dictionary value by key.
		/// </summary>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		private string GetDataDictionaryValueByKey(Guid siteGuid, string key)
		{
			return DataDictionarySingleton.Get(siteGuid, key);
		}

		/// <summary>
		/// This method will return the ListView class that contains a collection
		/// of ListViewFields base on teh list view type (transaction list, ...) and 
		/// the selected alias (issue, bulk issue, ...). It will return null if there
		/// are not matches.
		/// </summary>
		/// <param name="listViewType">
		/// </param>
		/// <param name="aliasGuid">
		/// The alias Guid.
		/// </param>
		/// <returns>
		/// </returns>
		protected ListViewClass GetListViews(LISTVIEW_TYPE listViewType, Guid aliasGuid)
		{
			ListViewClass listView = null;

			Guid listViewGuid = FMChannelHelper.MakeCall<IListViews, Guid>(x => x.GetIdentityGuid(this.securityObj, listViewType, aliasGuid));

			if (listViewGuid != Guid.Empty)
			{
				listView = FMChannelHelper.MakeCall<IListViews, ListViewClass>(x => x.Get(this.securityObj, listViewType, listViewGuid));
			}

			return listView;
		}

		/// <summary>
		/// This method will return the product type for the given product (product name). It
		/// will return max product type if the product was not found.
		/// </summary>
		/// <param name="productText"></param>
		/// <returns></returns>
		protected ProductType GetProductType(string productText)
		{
			var type = ProductType.MaxProduct;

			if (!string.IsNullOrEmpty(productText))
			{
				Guid productIdentityGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x => x.GetIdentityGuid(this.securityObj, productText));

				ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x => x.GetByProductAuthorizedCompanies(this.securityObj, productIdentityGuid, false));

				type = product.ProductType;
			}

			return type;
		}

		/// <summary>
		/// This method will setup the grid column keys in order to
		/// later create the tool tips for manager, owner, ...
		/// </summary>
		/// <param name="key"></param>
		protected void SetToolTipKeys(string key)
		{
			Hashtable toolTipKeys;

			if (this.Page.Session[this.ID + TOOL_TIP_KEYS] == null)
			{
				toolTipKeys = new Hashtable();
				this.Page.Session.Add(this.ID + TOOL_TIP_KEYS, toolTipKeys);
			}
			else
			{
				toolTipKeys = (Hashtable)this.Page.Session[this.ID + TOOL_TIP_KEYS];
			}

			if (key.ToUpper().StartsWith("MANAGER"))
			{
				if (toolTipKeys.Contains(key) == false)
				{
					toolTipKeys.Add(key, COMPANY_ROLE.MANAGER);
				}
			}

			if (key.ToUpper().StartsWith("OWNER"))
			{
				if (toolTipKeys.Contains(key) == false)
				{
					toolTipKeys.Add(key, COMPANY_ROLE.OWNER);
				}
			}

			if (key.ToUpper().StartsWith("BILLTO"))
			{
				if (toolTipKeys.Contains(key) == false)
				{
					toolTipKeys.Add(key, COMPANY_ROLE.CUSTOMER_BILLTO);
				}
			}

			if (key.ToUpper().StartsWith("SHIPTO"))
			{
				if (toolTipKeys.Contains(key) == false)
				{
					toolTipKeys.Add(key, COMPANY_ROLE.CUSTOMER_SHIPTO);
				}
			}

			if (key.ToUpper().StartsWith("CARRIER"))
			{
				if (toolTipKeys.Contains(key) == false)
				{
					toolTipKeys.Add(key, COMPANY_ROLE.CARRIER);
				}
			}

			if (key.ToUpper().StartsWith("SHIPPER"))
			{
				if (toolTipKeys.Contains(key) == false)
				{
					toolTipKeys.Add(key, COMPANY_ROLE.SHIPPER);
				}
			}

			if (key.ToUpper().StartsWith("SUPPLIER"))
			{
				if (toolTipKeys.Contains(key) == false)
				{
					toolTipKeys.Add(key, COMPANY_ROLE.SUPPLIER);
				}
			}
		}

		/// <summary>
		/// This method will set the initial sort column and direction.
		/// </summary>
		/// <param name="columnName"></param>
		protected void SetSortColumn(string columnName)
		{
			// Set the sorting expression and direction.
			if (this.Page.Session[this.ID + SORT_EXPRESSION] == null)
			{
				this.Page.Session[this.ID + SORT_EXPRESSION] = columnName;
				this.Page.Session[this.ID + SORT_DIRECTION] = "ASC";
			}
		}
		#endregion

		#region Event methods
		/// <summary>
		/// This method is called during the page binding. It will data dictionary the
		/// column headers.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void Page_DataBinding(object sender, EventArgs e)
		{

			// Try catch is necessary for designer
			try
			{
				if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
				{
					var siteGuid = (Guid)this.Page.Session["SiteGuid"];

					// Apply the data dictionary to the column headers if the dictionary
					// exists.


					// Apply the data dictionary to the column headers.
					foreach (DataGridColumn column in this.Columns)
					{
						column.HeaderText = this.GetDataDictionaryValueByKey(siteGuid, column.HeaderText);
					}
				}
				else
				{
					// Remove the all characters with the exception of the column name.
					foreach (DataGridColumn column in this.Columns)
					{
						column.HeaderText = column.HeaderText.Substring(column.HeaderText.IndexOf("|") + 1);
					}
				}
			}
			catch
			{
			}
			
		}

		/// <summary>
		/// This method handles the data grid sort event. It will set the sort expression and
		/// direction into session for the view to be updated.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		public void DataGrid_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (this.Page.Session[this.ID + SORT_EXPRESSION] != null)
			{
				if ((string)this.Page.Session[this.ID + SORT_EXPRESSION] != e.SortExpression)
				{
					this.Page.Session[this.ID + SORT_EXPRESSION] = e.SortExpression;
				}
				else
				{
					if ((this.Page.Session[this.ID + SORT_DIRECTION] != null) &&
						((string)this.Page.Session[this.ID + SORT_DIRECTION] == "ASC"))
					{
						this.Page.Session[this.ID + SORT_DIRECTION] = "DESC";
					}
					else
					{
						this.Page.Session[this.ID + SORT_DIRECTION] = "ASC";
					}
				}
			}
			else
			{
				this.Page.Session[this.ID + SORT_EXPRESSION] = e.SortExpression;
				this.Page.Session[this.ID + SORT_DIRECTION] = "ASC";
			}

			this.UpdateView();
		}

		/// <summary>
		/// This method handles the data grid item data bound event. This event is called when
		/// the items are getting bound to the grid. During this process the tool tips for the
		/// company role columns are created.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public virtual void DataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex >= 0)
			{
				var view = (DataView)this.DataSource;

				if (this.Page.Session[this.ID + TOOL_TIP_KEYS] != null)
				{
					var toolTipKeys = (Hashtable)this.Page.Session[this.ID + TOOL_TIP_KEYS];
					IDictionaryEnumerator enumerator = toolTipKeys.GetEnumerator();

					while (enumerator.MoveNext())
					{
						string id = enumerator.Key.ToString();
						var role = (COMPANY_ROLE)enumerator.Value;
						var companyLabel = e.Item.FindControl(id) as Label;

						if (companyLabel != null)
						{
							companyLabel.Text = view[e.Item.DataSetIndex][id] as string;
							companyLabel.ToolTip = this.GetToolTip(e.Item.DataSetIndex, view, role);
						}
					}
				}
			}
		}

		/// <summary>
		/// This method handles the item create event. It will add the sort direction direction
		/// for the column that is sorted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void DataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
		{
			ListItemType elemType = e.Item.ItemType;

			if (elemType == ListItemType.Header)
			{
				var sortExpression = (string)this.Page.Session[this.ID + SORT_EXPRESSION];
				var sortDirection = (string)this.Page.Session[this.ID + SORT_DIRECTION];

				if (sortExpression != null)
				{
					int index = 0;

					foreach (DataGridColumn column in this.Columns)
					{
						if (column.SortExpression == sortExpression)
						{
							TableCell cell = e.Item.Cells[index];
							var sortedLabel = new Label();
							sortedLabel.Font.Name = "webdings";
							sortedLabel.Font.Size = FontUnit.XSmall;

							if (string.IsNullOrEmpty(sortDirection) || (sortDirection == "ASC"))
							{
								sortedLabel.Text = "6";
							}
							else
							{
								sortedLabel.Text = "5";
							}

							cell.Controls.Add(sortedLabel);
							break;
						}

						index++;
					}
				}
			}
		}

		/// <summary>
		/// This method will handle the paging event. It will set the current page index to the
		/// selected page and update the view.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		public void DataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.EditItemIndex > -1)
			{
				return;
			}

			this.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}
		#endregion
	}
}
