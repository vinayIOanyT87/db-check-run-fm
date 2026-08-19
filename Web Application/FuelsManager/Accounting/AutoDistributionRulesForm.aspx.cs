namespace FuelsManager.Accounting
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;
    using FMBusinessObjects.UtilityObjects;
    using FMCore;
    public partial class AutoDistributionRulesForm : AccountingWebFormView, IEntityDiscovery
	{
		#region constants and fields
		public const string PageName = "AutoDistributionRulesForm";
		public const string PageUrl = PageName + ".aspx";

		/* constants referenced in the html/menu */
		public const string PageTitle = "Automatic Distribution Rules Summary";
		public const string MenuName = "Rules";

		public const string DefaultEOM = "DefaultEOM";
		public const string Description = "Description";
		public const string Enabled = "Enabled";
		public const string ManagerList = "Managers";
		public const string OwnerList = "Owners";
		public const string ProductList = "Products";
		public const string ReasonCode = "ReasonCode";
		public const string RuleID = "RuleID";

		public const string ManagerLabelText = "Manager";
		public const string ProductLabelText = "Product";
		public const string FindStringLabelText = "Find String";

		public const string DefaultEOMColumnName = "Default EOM";
		public const string DescriptionColumnName = "Description";
		public const string RuleGuidColumnName = "IdentityGuid";
		public const string SiteGuidColumnName = "SiteGuid";
		public const string EnabledColumnName = "Enabled";
		public const string ManagerListColumnName = "Managers";
		public const string OwnerListColumnName = "Owners";
		public const string ProductListColumnName = "Products";
		public const string ReasonCodeColumnName = "Reason Code";
		public const string RuleIDColumnName = "Rule ID";

		public const string PostBackRefreshDataArgument = "RefreshData";

		/* constants defined in html and used in the class*/
		public const string DefaultEOMControlID = "DefaultEOMTextBox";
		private const string GuidControlID = "identityGuidLabel";
		private const string DeleteButtonTagID = "DeleteButton";

		/* The following are referenced in the class only */

		private bool hasModifyRight = false;
		private AccountingSite accountingSite = null;
		private ListViewDataSet lvDataSet = null;
		private HorizontalAlign[] columnAlignmentInfo = null;

		private const string AllText = "{All}";
		#endregion constants and fields

		#region Properties
		/// <summary>
		/// Sets and returns Session[AutoDistributionRuleList] as AutoDistributionRuleCollectionClass
		/// </summary>
		private AutoDistributionRuleListViewDOCollection MySessionDataList
		{
			get
			{
				return this.Session[PageSessionKeyConstants.AutoDistributionRulesFormDataList] as AutoDistributionRuleListViewDOCollection;
			}
			set
			{
				this.Session[PageSessionKeyConstants.AutoDistributionRulesFormDataList] = value;
			}
		}

		/// <summary>
		/// Sets and returns Session[IsSortAscendingSessionKey] as bool
		/// </summary>
		private bool MySessionIsSortAscending
		{
			get
			{
				bool isSortAscending = true;

				if (this.Session[PageSessionKeyConstants.AutoDistributionRulesFormIsSortAscending] != null)
				{
					try
					{
						isSortAscending = (bool)this.Session[PageSessionKeyConstants.AutoDistributionRulesFormIsSortAscending];
					}
					catch
					{
					}
				}

				return isSortAscending;
			}
			set
			{
				this.Session[PageSessionKeyConstants.AutoDistributionRulesFormIsSortAscending] = value;
			}
		}

		/// <summary>
		/// Sets and returns Session[SortDisplayNameSessionKey] as string
		/// </summary>
		private String MySessionSortDisplayName
		{
			get
			{
				return this.Session[PageSessionKeyConstants.AutoDistributionRulesFormSortDisplayName] as string;
			}
			set
			{
				this.Session[PageSessionKeyConstants.AutoDistributionRulesFormSortDisplayName] = value;
			}
		}
		#endregion

		#region Page Events
		protected void Page_Load(object sender, EventArgs eventArgList)
		{
			try
			{
				this.GetSecurity();
				// initialize class variables
				this.hasModifyRight = this.Security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION);
				this.accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
					x =>
					x.LoadSiteInfoNoCompanies(base.Security, base.Security.SiteGuid)
				);

				this.lvDataSet = new ListViewDataSet(base.Security, LISTVIEW_TYPE.STANDARD,
							ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.AUTO_DISTRIBUTION_RULE), this.accountingSite);
				//this.SetColumnAlignmentInfo();


				if (this.Page.IsPostBack)
				{
					string argument = this.Request.GetQueryOrFormValue("__EVENTARGUMENT");
					if (string.IsNullOrWhiteSpace(argument) == false &&
						string.Compare(argument, PostBackRefreshDataArgument, true) == 0)
					{
						this.RefreshData();
					}

				}
				else
				{
					// initialize session variables
					this.MySessionSortDisplayName = ListViewFieldClass.StandardFieldTypeID(STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_RULE_ID, false);  // false  = use datapth
					this.MySessionIsSortAscending = true;

					this.lvDataSet.SortDirection = this.MySessionIsSortAscending;
					this.lvDataSet.Sort = this.MySessionSortDisplayName;

					this.EnableControls(true);
					this.ClearControls();
					this.RefreshData();

                }

			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		override protected void OnInit(EventArgs eventArgList)
		{
			base.OnInit(eventArgList);
		}

		protected void PageSizeDropDown_SelectedIndexChanged(object source, System.EventArgs eventArgList)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}
		#endregion Page Events

		#region Find buttons events
		/// <summary>
		/// This method is called when the find button is pressed. It will retrieve data from the find text box and update the grid.
		/// </summary>
		/// <param name="sender">Find Button</param>
		/// <param name="eventArgList">Event Arguments</param>
		protected void FindBtn_OnClick(object sender, System.EventArgs eventArgList)
		{
			try
			{
				this.RefreshData();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// This method is called when the find all button is pressed. 
		/// </summary>
		/// <param name="sender">ShowAllButton</param>
		/// <param name="eventArgList">Event Arguments</param>
		protected void ShowAllButton_OnClick(object sender, System.EventArgs eventArgList)
		{
			try
			{
				this.ClearControls();
				this.RefreshData();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}
		#endregion

		#region grid events
		protected void AddButton_Click(object sender, EventArgs eventArgs)
		{
			try
			{
				AutoDistributionRuleForm.AddARule(this);
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void DataGrid_EditCommand(object source, DataGridCommandEventArgs eventArgs)
		{

			bool hasErrors = false;
			try
			{
				// Clean up
				this.Session.Remove(PageSessionKeyConstants.AutoDistributionRule);

				Guid ruleGuid;

				if (this.FindCurrentGuid(eventArgs.Item, out ruleGuid))
				{
					// Retrieve the rule and pass it through session variable to the detail page
					AutoDistributionRuleDO theRule = FMChannelHelper.MakeCall<IAutoDistributionRules, AutoDistributionRuleDO>(
						x =>
						x.Get(this.Security, ruleGuid)
					);

					this.Session[PageSessionKeyConstants.AutoDistributionRule] = theRule;

					// save the page index for later use
					this.Session[PageSessionKeyConstants.AutoDistributionRulesFormPageIndex] = this.mainDataGrid.CurrentPageIndex;
				}

			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
				hasErrors = true;
			}

			if (hasErrors == false)
			{
				this.Redirect(AutoDistributionRuleForm.PageUrl);
			}
		}

		protected void DataGrid_DeleteCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				AutoDistributionRuleListViewDO currentRuleDO;
				if (this.FindCurrentRule(eventArgs.Item, out currentRuleDO))
				{

                    // remove from our data list
                    AutoDistributionRuleListViewDOCollection ruleList = this.MySessionDataList;
                    ruleList.Remove(ruleList[currentRuleDO.IdentityGuid]);

                    // Remove the rule from the database
                    FMChannelHelper.MakeCall<IAutoDistributionRules>(x => x.Purge(this.Security, currentRuleDO.IdentityGuid));

                    this.MySessionDataList = ruleList;

                    if (this.mainDataGrid.Items.Count == 1
                        && this.mainDataGrid.CurrentPageIndex > 0)
                    {
                        this.mainDataGrid.CurrentPageIndex--;
                    }

                    this.UpdateView();
				}

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void DataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs eventArgs)
		{
			try
			{
				this.mainDataGrid.CurrentPageIndex = eventArgs.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void DataGrid_ItemDataBound(object sender, DataGridItemEventArgs eventArgs)
		{
			try
			{
				// Need to disable the edit and delete buttons when the user does not
				// have the appropriate rights.
				if (eventArgs.Item.ItemIndex != -1)
				{
					Guid siteGuid = (Guid)((DataRowView)eventArgs.Item.DataItem).Row[SiteGuidColumnName];
					bool toEnable = this.hasModifyRight && (siteGuid == this.Security.SiteGuid);
					this.FindAndDisableLinkButton(eventArgs, DeleteButtonTagID, toEnable);

					// our list view doesn't have a good way to set alignment.  This is a hack to do it.
					//for (int cellIndex = 0; cellIndex < eventArgs.Item.Cells.Count; ++cellIndex)
					//{
					//	TableCell cell = eventArgs.Item.Cells[cellIndex];
					//	cell.Style.Add("text-align", this.columnAlignmentInfo[cellIndex].ToString());

					//}
				}

			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void DataGrid_SortCommand(object source, DataGridSortCommandEventArgs eventArg)
		{
			try
			{
				string sortDisplayName = this.MySessionSortDisplayName;
				bool isSortAscending = this.MySessionIsSortAscending;

				// save session variables
				if (eventArg.SortExpression != this.MySessionSortDisplayName)
				{
					sortDisplayName = eventArg.SortExpression;
					isSortAscending = true;
				}
				else
				{
					isSortAscending = !isSortAscending;
				}

				// sort our data using lambda expression
				string sortFieldName = this.lvDataSet.GetDataPath(sortDisplayName);

				this.MySessionSortDisplayName = sortDisplayName;
				this.MySessionIsSortAscending = isSortAscending;
				this.lvDataSet.SortDirection = this.MySessionIsSortAscending;
				this.lvDataSet.Sort = sortFieldName;

				ParameterExpression fieldParam = Expression.Parameter(typeof(AutoDistributionRuleListViewDO), sortFieldName);
				MemberExpression fieldExpression = Expression.Property(fieldParam, sortFieldName);
				Expression convertedFieldExpression = Expression.Convert(fieldExpression, typeof(object));
				Expression<Func<AutoDistributionRuleListViewDO, object>> sortLambdaExpression = Expression.Lambda<Func<AutoDistributionRuleListViewDO, object>>(convertedFieldExpression, fieldParam);
				List<AutoDistributionRuleListViewDO> tempList = this.MySessionDataList.Cast<AutoDistributionRuleListViewDO>().ToList();

				IEnumerable<AutoDistributionRuleListViewDO> sortedList = null;

				if (isSortAscending)
				{
					sortedList = tempList.AsQueryable<AutoDistributionRuleListViewDO>().OrderBy(sortLambdaExpression);
				}
				else
				{
					sortedList = tempList.AsQueryable<AutoDistributionRuleListViewDO>().OrderByDescending(sortLambdaExpression);
				}

				this.MySessionDataList = new AutoDistributionRuleListViewDOCollection(sortedList);

				// refresh
				this.mainDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion grid events
		#region public methods
		/// <summary>
		/// Used by the html to bind data column
		/// </summary>
		/// <param name="container">Data Grid containter/row</param>
		/// <param name="columnName">Name of the column</param>
		/// <returns>The column value</returns>
		public object BindColumn(object container, string columnName)
		{
			return System.Web.UI.DataBinder.Eval(container, "DataItem." + columnName);
		}
		#endregion public methods

		#region Private Methods

		/// <summary>
		/// Clear control status
		/// </summary>
		private void ClearControls()
		{
			// Just clear all criteria.  RefreshData will take care of the rest
			this.findTextBox.Text = string.Empty;
			this.managerTextBox.Text = this.GetTranslatedText(AllText);
			this.productTextBox.Text = this.managerTextBox.Text;
		}

		/// <summary>
		/// The populate my session data list.
		/// </summary>
		private void PopulateMySessionDataList()
		{
			var managerGuid = Guid.Empty;
			var productGuid = Guid.Empty;
			var searchText = this.findTextBox.Text.Trim();
			var managerID = this.managerTextBox.Text;
			var productID = this.productTextBox.Text;

			if (string.IsNullOrWhiteSpace(managerID) == false)
			{
				managerGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(theService => theService.GetIdentityGuid(this.Security, managerID));
			}

			if (string.IsNullOrWhiteSpace(productID) == false)
			{
				productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(theService => theService.GetIdentityGuid(this.Security, productID));
			}

			// get data
			var ruleList = FMChannelHelper.MakeCall<IAutoDistributionRules, AutoDistributionRuleDOCollection>(
				x => x.Enumerate(this.Security, managerGuid, productGuid, searchText));

			// Update session
			this.MySessionDataList = new AutoDistributionRuleListViewDOCollection(ruleList);
		}

		private void RefreshData()
		{
			this.PopulateMySessionDataList();
			this.UpdateView();
		}

		/// <summary>
		/// Enables/Disables Add buttons based on rights, Enable/Disable Show ... dropdown
		/// </summary>
		/// <param name="toEnable">Enable or Disable the controls</param>
		private void EnableControls(bool toEnable)
		{
			bool actualEnable = toEnable && this.hasModifyRight;
			this.topAddButton.Enabled = actualEnable;
			this.bottomAddButton.Enabled = actualEnable;
			this.pageSizeDropDown.Enabled = toEnable;	// indenpendt of Modify Rights
		}

		/// <summary>
		/// No parameter wrapper to the UpdateView method
		/// </summary>
		private void UpdateView()
		{
			this.UpdateView(this.pageSizeDropDown);
		}

		/// <summary>
		/// Updates the grid
		/// </summary>
		/// <param name="pageSizeDropDown">Page size control</param>
		private void UpdateView(FMControls.FMPageSizeDropDown pageSizeDropDown)
		{
			ICollection applicationStrings = this.EnumerateData();

			pageSizeDropDown?.SetPageSize(this.mainDataGrid, applicationStrings.Count);

			this.mainDataGrid.DataSource = applicationStrings;
			this.mainDataGrid.DataBind();

            if (pageSizeDropDown != null)
            {
                pageSizeDropDown.SetPageSize(this.mainDataGrid, this.MySessionDataList.Count);
            }

            //this.lvDataSet.SetDataGrid(this.mainDataGrid);

			// Take care of text alignment 
			this.mainDataGrid.Style.Remove("text-align");
			this.mainDataGrid.Style.Add("text-align", "left");
			this.mainDataGrid.Width = 0;

			//this.lvDataSet.BindData(this.MySessionDataList, QuantityDisplay.NET, this.accountingSite.CurrentSite._VolumeDecimalPlaces, this.accountingSite.CurrentSite._MassDecimalPlaces, false);
		}

		/// <summary>
		/// Prepare datasource for the data grid
		/// </summary>
		/// <returns>Returns a list of reason codes</returns>
		private ICollection EnumerateData()
		{
			AutoDistributionRuleListViewDOCollection dataList = this.MySessionDataList;

			DataTable mapDataTable = new DataTable();
			DataColumnCollection dataColumnList = mapDataTable.Columns;
			dataColumnList.Add(RuleGuidColumnName, typeof(Guid));
			dataColumnList.Add(DefaultEOMColumnName, typeof(bool));
			dataColumnList.Add(DescriptionColumnName, typeof(string));
			dataColumnList.Add(SiteGuidColumnName, typeof(Guid));
			dataColumnList.Add(EnabledColumnName, typeof(bool));
			dataColumnList.Add(ManagerListColumnName, typeof(string));
			dataColumnList.Add(OwnerListColumnName, typeof(string));
			dataColumnList.Add(ProductListColumnName, typeof(string));
			dataColumnList.Add(ReasonCodeColumnName, typeof(string));
			dataColumnList.Add(RuleIDColumnName, typeof(string));
			foreach (AutoDistributionRuleListViewDO t in dataList)
			{
				DataRow mapDataRow = mapDataTable.NewRow();
				AutoDistributionRuleListViewDO ruleDO = t;
				mapDataRow[RuleGuidColumnName] = ruleDO.IdentityGuid;
				mapDataRow[DefaultEOMColumnName] = ruleDO.DefaultEOM;
				mapDataRow[DescriptionColumnName] = ruleDO.Description;
				mapDataRow[SiteGuidColumnName] = ruleDO.SiteGuid;
				mapDataRow[EnabledColumnName] = ruleDO.Enabled;
				mapDataRow[ManagerListColumnName] = ruleDO.ManagerList;
				mapDataRow[OwnerListColumnName] = ruleDO.OwnerList;
				mapDataRow[ProductListColumnName] = ruleDO.ProductList;
				mapDataRow[ReasonCodeColumnName] = ruleDO.DefaultReasonCodeString;
				mapDataRow[RuleIDColumnName] = ruleDO.RuleID;
				mapDataTable.Rows.Add(mapDataRow);
			}
			DataView newDataView = new DataView(mapDataTable);
			return newDataView;
		}

		/// <summary>
		/// Disables Edit/Delete icons based on the given flag
		/// </summary>
		/// <param name="eventArgs">Data Grid Event Arguments</param>
		/// <param name="targetID">Target control to be find</param>
		/// <param name="toEnable">Enable or disable the control</param>
		private void FindAndDisableLinkButton(DataGridItemEventArgs eventArgs, string targetID, bool toEnable)
		{
			LinkButton targetButton = (LinkButton)eventArgs.Item.FindControl(targetID);

			if (targetButton != null)
			{
				targetButton.Enabled = toEnable;
			}

		}

		/// <summary>
		/// Finds the current item and returns its Guid
		/// </summary>
		/// <param name="currentItem">Current Data Guid Row</param>
		/// <param name="currentItemGuid">returns the Guid of the current row</param>
		/// <returns>True if the Guid of the current item is found</returns>
		private bool FindCurrentGuid(DataGridItem currentItem, out Guid currentItemGuid)
		{
			Label guidLabel = (Label)currentItem.FindControl(GuidControlID);
			currentItemGuid = Guid.Empty;
			bool found = guidLabel != null;

			if (found)
			{
				currentItemGuid = new Guid(guidLabel.Text);
			}

			return found;
		}

		/// <summary>
		/// Finds the current item and returns the object from the session list
		/// </summary>
		/// <param name="currentItem">Current DataGrid row</param>
		/// <param name="ruleDO">The corresponding ruleDO object</param>
		/// <returns>True if found</returns>
		private bool FindCurrentRule(DataGridItem currentItem, out AutoDistributionRuleListViewDO ruleDO)
		{
			Guid currentGuid;
			ruleDO = null;
			bool found = this.FindCurrentGuid(currentItem, out currentGuid);

			if (found)
			{
				ruleDO = this.MySessionDataList[currentGuid];
			}

			return found;
		}

		/// <summary>
		/// This is called
		/// </summary>
		private void SetColumnAlignmentInfo()
		{
			// Prepare an array of datapaths, this is to help looking up the column from the header
			Dictionary<string, STANDARD_FIELD_TYPE> fieldDataPathList = new Dictionary<string, STANDARD_FIELD_TYPE>();
			STANDARD_FIELD_TYPE[] fieldList = ListViewClass.GetStandardViewFields(LISTVIEW_STANDARD_TYPE.AUTO_DISTRIBUTION_RULE);
			foreach (STANDARD_FIELD_TYPE currrentField in fieldList)
			{
				fieldDataPathList.Add(ListViewFieldClass.StandardFieldTypeID(currrentField, false), currrentField);
			}

			// assuming all VISIBLE columns are centered
			this.columnAlignmentInfo = new HorizontalAlign[this.lvDataSet.listViewDO.ColumnCount + this.mainDataGrid.Columns.Count];
			for (int idx = 0; idx < this.mainDataGrid.Columns.Count; idx++)
			{
				this.columnAlignmentInfo[idx] = HorizontalAlign.Center;
			}

			// going through generated columns
			for (int idx = 0; idx < this.lvDataSet.listViewDO.ColumnCount; idx++)
			{
				// find the corresponding internal column
				ListViewColumnDO lvColumn = this.lvDataSet.listViewDO[idx];
				STANDARD_FIELD_TYPE currrentField = STANDARD_FIELD_TYPE.BEGIN_INVENTORY;

				if (fieldDataPathList.Keys.Any(dataPath => string.Compare(dataPath, lvColumn.DataPath, StringComparison.OrdinalIgnoreCase) == 0))
				{
					currrentField = fieldDataPathList[lvColumn.DataPath];
				}

				HorizontalAlign currentAlignnment = HorizontalAlign.Left;
				switch (currrentField)
				{
					case STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_DEFAULT_EOM:
					case STANDARD_FIELD_TYPE.ENABLED:
						currentAlignnment = HorizontalAlign.Center;
						break;
				}
				this.columnAlignmentInfo[idx + this.mainDataGrid.Columns.Count] = currentAlignnment;
			}
		}
		#endregion

		#region IEntityDiscovery interface
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.AUTODISTRIBUTION_RULE;
			}
		}

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			AutoDistributionRuleDOCollection ruleList =
				FMChannelHelper.MakeCall<IAutoDistributionRules, AutoDistributionRuleDOCollection>(
					x =>
					x.Enumerate(security, Guid.Empty, Guid.Empty, null)
				);

			EntityToSiteMapCollectionClass ruleToSiteMapList = new EntityToSiteMapCollectionClass();
			foreach (AutoDistributionRuleDO theRule in ruleList)
			{

				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if ((security.SiteGuid == theRule.SiteGuid)
						|| (security.LoginSiteGuid != theRule.SiteGuid))
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != theRule.SiteGuid)
					{
						continue;
					}
				}

				ruleToSiteMapList.Add(new EntityToSiteMapClass(theRule));
			}
			return ruleToSiteMapList;
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IAutoDistributionRules);
			}
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid ruleGuid, Guid siteGuid)
		{
			AutoDistributionRuleDO theRule = FMChannelHelper.MakeCall<IAutoDistributionRules, AutoDistributionRuleDO>(
				x =>
				x.Get(security, ruleGuid)
			);

			theRule.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IAutoDistributionRules>(x => x.Modify(security, theRule));
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ruleID)
		{
			return FMChannelHelper.MakeCall<IAutoDistributionRules, Guid>(x => x.GetIdentityGuid(security, ruleID));
		}

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}
		#endregion
	}
}