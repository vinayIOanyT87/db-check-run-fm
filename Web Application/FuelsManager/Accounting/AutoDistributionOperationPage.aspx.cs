// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoDistributionOperationPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   This is to indicate whether the page is being called from Inv Recon or from FM menu.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

	using FMBusinessObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

    using FMCore;

    using FuelsManager.Accounting;

    #region Public Enumerations
	/// <summary>
	/// This is to indicate whether the page is being called from 
	/// Inventory Reconciliation or from FM menu.
	/// </summary>
	public enum AutoDistributionOperationTypes
	{
		Manual,
		InventoryReconcilliation
	}
	#endregion

	#region Inventory Reconciliation page information class
	/// <summary>
	/// This is just a data container class to hold information passing from 
	/// Inventory Reconciliation page.
	/// </summary>
	public class InventoryReconPageInfo
	{
		/// <summary>
		/// Gets or sets the rule GUID.
		/// </summary>
		public Guid RuleGuid { get; set; }

		/// <summary>
		/// Gets or sets the manager GUID.
		/// </summary>
		public Guid ManagerGuid { get; set; }

		/// <summary>
		/// Gets or sets the product GUID.
		/// </summary>
		public Guid ProductGuid { get; set; }

		/// <summary>
		/// Gets or sets the gross.
		/// </summary>
		public double Gross { get; set; }

		/// <summary>
		/// Gets or sets the net.
		/// </summary>
		public double Net { get; set; }

		/// <summary>
		/// Gets or sets the mass.
		/// </summary>
		public double Mass { get; set; }

		/// <summary>
		/// Gets or sets the inventory month.
		/// </summary>
		public string InventoryMonth { get; set; }
	}
	#endregion

	/// <summary>
	/// The auto distribution operation page.
	/// </summary>
	public partial class AutoDistributionOperationPage : AccountingWebFormView
	{
		#region constants and fields
		public const string PageName = "AutoDistributionOperationPage";
		public const string PageUrl = PageName + ".aspx";

		/* constants referenced in the html/menu */
		public const string PageTitle = "Automatic Distribution Operation";
		public const string MenuName = "Auto Distribution";

		// label text, if you add new ones, please add it to the DataDictionary below.
		public const string RuleIdLabelText = "Rule ID";
		public const string DescriptionLabelText = "Description";
		public const string ManagerLabelText = "Manager";
		public const string ProductLabelText = "Product";
		public const string TrxAliasLabelText = "Transaction Alias";
		public const string ThruputStartDateLabelText = "Throughput Start Date";
		public const string ThruputEndDateLabelText = "Throughput End Date";
		public const string InventoryMonthLabelText = "Inventory Month";
		public const string InvDateLabelText = "Inventory Date";
		public const string QuantitiesInvReconLabelText = "Total Variances";
		public const string QuantitiesManualLabelText = "Quantities";
		public const string ExpectedGrossLabelText = "Gross";
		public const string ExpectedNetLabelText = "Net";
		public const string ExpectedMassLabelText = "Mass";
		public const string ReasonCodeLabelText = "Reason Code";
		public const string NotesLabelText = "Notes";
		public const string DistributionsLabelText = "Distributions";

		//  The following are used in the grid.
		public const string OwnerColumnLabelText = "Owner";
		public const string GrossColumnLabelText = "Gross";
		public const string NetColumnLabelText = "Net";
		public const string MassColumnLabelText = "Mass";

		public const string ThruputColumnLabelText = "Throughput";
		public const string PercentColumnLabelText = "Percent";
		public const string QuantityColumnLabelText = "Quantity";

		public const string PostBackDateControlChangedArgument = "DateControlChanged";

		public const string UrlParamOperationType = "OpType";
		public const string UrlParamRuleGuid = "RuleGuid";
		public const string UrlParamManagerGuid = "ManagerGuid";
		public const string UrlParamProductGuid = "ProductGuid";
		public const string UrlParamGross = "Gross";
		public const string UrlParamNet = "Net";
		public const string UrlParamMass = "Mass";
		public const string UrlParamInventoryMonth = "InvMonth";

		// error messages
		public const string NoRuleMessage = "No enabled rule is available.";
		public const string InvalidUrlParameterMessage = "Invalid URL parameter(s) found.";
		public const string InvalidRuleMessage = "A valid rule is not selected.";
		public const string InvalidManagerMessage = "A valid manager is not selected.";
		public const string InvalidProductMessage = "A valid product is not selected.";
		public const string InvalidDatesMessage = "Please enter valid dates and Throughput End Date has to be later than Throughput Start Date.";
		public const string InvalidGrossMessage = "Please enter a valid gross quantity.";
		public const string InvalidNetMessage = "Please enter a valid net quantity.";
		public const string InvalidMassMessage = "Please enter a valid mass quantity.";
		public const string InvalidQuantityEnteredMessage = "Please enter a valid quantity.";
		public const string NoDefaultEomRuleMessage = "No enabled default EOM rule is found.";
		public const string MoreThanOneDefaultEomRuleMessage = "More than one enabled default EOM rule is found.";

		private const string SortAscending = "ASC";
		private const string SortDescending = "DESC";

		protected AutoDistributionOperationTypes operationType;
		private InventoryReconPageInfo invReconInfo;
		protected AutoDistributionOperationHelper myOperationHelper;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the my session distribution list.
		/// </summary>
		private DataTable MySessionDistributionList
		{
			get
			{
				return this.Session[PageSessionKeyConstants.AutoDistributionOperationDistributionList] as DataTable;
			}

			set
			{
				this.Session[PageSessionKeyConstants.AutoDistributionOperationDistributionList] = value;
			}
		}

		/// <summary>
		/// Gets or sets the my session rule list.
		/// </summary>
		private AutoDistributionRuleDOCollection MySessionRuleList
		{
			get
			{
				return this.Session[PageSessionKeyConstants.AutoDistributionOperationRuleList] as AutoDistributionRuleDOCollection;
			}

			set
			{
				this.Session[PageSessionKeyConstants.AutoDistributionOperationRuleList] = value;
			}
		}

		/// <summary>
		/// Gets or sets the my session current rule GUID.
		/// </summary>
		private Guid MySessionCurrentRuleGuid
		{
			get
			{
				if (this.Session[PageSessionKeyConstants.AutoDistributionOperationRuleGuid] == null)
				{
					return Guid.Empty;
				}

				return (Guid)this.Session[PageSessionKeyConstants.AutoDistributionOperationRuleGuid];
			}

			set
			{
				this.Session[PageSessionKeyConstants.AutoDistributionOperationRuleGuid] = value;
			}
		}

		/// <summary>
		/// Gets the my session current rule.
		/// </summary>
		private AutoDistributionRuleDO MySessionCurrentRule
		{
			get
			{
				AutoDistributionRuleDO currentRule = null;

				if (this.MySessionCurrentRuleGuid != Guid.Empty && this.MySessionRuleList != null)
				{
					currentRule = this.MySessionRuleList[this.MySessionCurrentRuleGuid];
				}

				return currentRule;
			}
		}

		/// <summary>
		/// Gets or sets the my session sort direction.
		/// </summary>
		private string MySessionSortDirection
		{
			get
			{
				return (string)this.Session[PageSessionKeyConstants.AutoDistributionOperationSortDirection];
			}

			set
			{
				this.Session[PageSessionKeyConstants.AutoDistributionOperationSortDirection] = value;
			}
		}

		/// <summary>
		/// Gets or sets the my session sort expression.
		/// Session SortExpression variable as string.
		/// </summary>
		private string MySessionSortExpression
		{
			get
			{
				return (string)this.Session[PageSessionKeyConstants.AutoDistributionOperationSortExpression];
			}

			set
			{
				this.Session[PageSessionKeyConstants.AutoDistributionOperationSortExpression] = value;
			}
		}
		#endregion Session Variables

		#region page events
		/// <summary>
		/// The on initialize.
		/// </summary>
		/// <param name="eventArgList">
		/// The event argument list.
		/// </param>
		protected override void OnInit(EventArgs eventArgList)
		{
			base.OnInit(eventArgList);
			this.Initialize();
		}

		/// <summary>
		/// The page initialization.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="eventArgList">
		/// The event argument list.
		/// </param>
		protected void Page_Init(object sender, EventArgs eventArgList)
		{
			try
			{
				this.GetSecurity();

				this.ReadUrlParameters();
				this.MaintainScrollPositionOnPostBack = true;

				if (this.IsPostBack == false)
				{
					this.SetControlStates();
					this.PopulateRules();
					this.ClearGrid();
					this.MySessionSortDirection = SortAscending;
					this.MySessionSortExpression = AutoDistributionOperationHelper.OwnerIDColumnName;
				}

				if (this.operationType == AutoDistributionOperationTypes.Manual)
				{
					this.thruputStartDateControl.Calendar.SelectionChanged += this.ThruputDateSelectionChanged;
					this.thruputEndDateControl.Calendar.SelectionChanged += this.ThruputDateSelectionChanged;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The page load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="eventArgList">
		/// The event argument list.
		/// </param>
		protected void Page_Load(object sender, EventArgs eventArgList)
		{
			try
			{
				this.InitializeVariablesAndControls();

				// Only display the Close Button if navigated from the Inventory Reconciliation page.
				this.CloseBtn.Visible = this.operationType == AutoDistributionOperationTypes.InventoryReconcilliation;

				if (this.Page.IsPostBack)
				{
					string argument = this.Request.GetQueryOrFormValue("__EVENTARGUMENT");

					if (string.IsNullOrWhiteSpace(argument) == false &&
						string.Compare(argument, PostBackDateControlChangedArgument, true) == 0)
					{
						this.ExpectedQuantityChanged(null, null);
					}
				}
				else if (this.operationType == AutoDistributionOperationTypes.InventoryReconcilliation)
				{
					this.UpdateView(false, true); // false = not JustSort, true = firstTimeCalculating
					this.CloseBtn.Visible = true;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The rule ID dropdown selected index changed.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="eventArgList">
		/// The event argument list.
		/// </param>
		protected void RuleIdDropDownSelectedIndexChanged(object source, EventArgs eventArgList)
		{
			try
			{
				this.HandleRuleSelectionChanged();
				this.CriticalInfoChanged();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The manager dropdown selected index changed.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="eventArgList">
		/// The event argument list.
		/// </param>
		protected void ManagerDropDownSelectedIndexChanged(object source, EventArgs eventArgList)
		{
			try
			{
				this.CriticalInfoChanged();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The product dropdown selected index changed.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="eventArgList">
		/// The event argument list.
		/// </param>
		protected void ProductDropDownSelectedIndexChanged(object source, EventArgs eventArgList)
		{
			try
			{
				this.CriticalInfoChanged();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The throughput date selection changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="eventArgList">
		/// The event argument list.
		/// </param>
		protected void ThruputDateSelectionChanged(object sender, EventArgs eventArgList)
		{
			try
			{
				this.CriticalInfoChanged();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The calculate button click.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="eventArgList">
		/// The event argument list.
		/// </param>
		protected void CalculateButtonClick(object source, EventArgs eventArgList)
		{
			try
			{
				this.UpdateView(false, true); // false = don't JustSort, true = firstTimeCalculating
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The create button click.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="eventArgList">
		/// The event argument list.
		/// </param>
		protected void CreateButtonClick(object source, EventArgs eventArgList)
		{
			var volumeBalanceFlag = this.Session[PageSessionKeyConstants.AutoDistributionOperationBalanceFlag] as bool?;

			// Since the Create button is always available, check for out of balance
			// volumes and display a warning message if there is one. 
			if (volumeBalanceFlag != null && volumeBalanceFlag.Value == false)
			{
				var outOfBalanceErrorMsg = this.Session[PageSessionKeyConstants.AutoDistributionOperationWarningMsg] as string;

				// Display out of balance error message.
				this.RenderErrorMessage(outOfBalanceErrorMsg);
				return;
			}

			bool hasError = false;

			try
			{
				this.CreateDistributions();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
				hasError = true;
			}

			if (hasError == false)
			{
				const string RedirectPage = "InventoryReconciliation.aspx";
				this.Redirect(RedirectPage);
			}
		}

		/// <summary>
		/// The close button click.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="eventArgList">
		/// The event argument list.
		/// </param>
		protected void CloseButtonClick(object source, EventArgs eventArgList)
		{
			const string RedirectPage = "InventoryReconciliation.aspx";
			this.Redirect(RedirectPage);
		}

		/// <summary>
		/// The grid row data bound.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="eventArgs">
		/// The event arguments.
		/// </param>
		protected void GridRowDataBound(object sender, GridViewRowEventArgs eventArgs)
		{
			const string CssBackgroundColor = "background-color";
			const string AlternateRowColor = "#EEEEEE";

			try
			{
				// set up alternate row colors
				if (eventArgs.Row.RowIndex % 2 == 0)
				{
					CssStyleCollection rowStyle = eventArgs.Row.Style;
					rowStyle.Remove(CssBackgroundColor);
					rowStyle.Add(CssBackgroundColor, AlternateRowColor);
				}
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The grid sorting.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="eventArg">
		/// The event argument.
		/// </param>
		protected void GridSorting(object sender, GridViewSortEventArgs eventArg)
		{
			try
			{
				if (eventArg.SortExpression != this.MySessionSortExpression ||
					this.MySessionSortDirection == SortDescending)
				{
					this.MySessionSortDirection = SortAscending;
				}
				else
				{
					this.MySessionSortDirection = SortDescending;
				}

				this.MySessionSortExpression = eventArg.SortExpression;

				this.UpdateView(true, false);		// true= JustSort
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The grid row created.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="eventArg">
		/// The event argument.
		/// </param>
		protected void GridRowCreated(object sender, GridViewRowEventArgs eventArg)
		{
			try
			{
				if (eventArg.Row.RowType == DataControlRowType.Header)
				{
					FMGridView.CreateSortIndicator(sender, eventArg, this.MySessionSortExpression, this.MySessionSortDirection);
				}
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The expected quantity changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="eventArgs">
		/// The event arguments.
		/// </param>
		protected void ExpectedQuantityChanged(object sender, EventArgs eventArgs)
		{
			try
			{
				this.CriticalInfoChanged();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The gross quantity column text changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="eventArgs">
		/// The event arguments.
		/// </param>
		protected void GrossQuantityColumnTextChanged(object sender, EventArgs eventArgs)
		{
			try
			{
				this.QuantityInGridChanged(sender, AutoDistributionQuantityTypes.Gross);
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The net quantity column text changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="eventArgs">
		/// The event arguments.
		/// </param>
		protected void NetQuantityColumnTextChanged(object sender, EventArgs eventArgs)
		{
			try
			{
				this.QuantityInGridChanged(sender, AutoDistributionQuantityTypes.Net);
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// The mass quantity column text changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="eventArgs">
		/// The event arguments.
		/// </param>
		protected void MassQuantityColumnTextChanged(object sender, EventArgs eventArgs)
		{
			try
			{
				this.QuantityInGridChanged(sender, AutoDistributionQuantityTypes.Mass);
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}
		#endregion page events


		#region public methods
		/// <summary>
		/// Used by the HTML to bind data column, also format the data using the given format.
		/// </summary>
		/// <param name="container">Data Grid container/row</param>
		/// <param name="columnName">Name of the column</param>
		/// <param name="numberFormat">Format used for display</param>
		/// <returns>The column value</returns>
		public static object BindColumn(object container, string columnName, NumberFormatInfo numberFormat)
		{
			object columnValue = DataBinder.Eval(container, "DataItem." + columnName);

			if (numberFormat != null)
			{
				columnValue = string.Format(numberFormat, "{0:N}", columnValue);
			}

			return columnValue;
		}

		/// <summary>
		/// Called from Inventory Reconciliation page to find the default EOM rule.
		/// </summary>
		/// <param name="mySecurity">
		/// The my security.
		/// </param>
		/// <param name="managerGuid">
		/// The manager GUID.
		/// </param>
		/// <param name="productGuid">
		/// The product GUID.
		/// </param>
		/// <param name="ruleGuid">
		/// The rule GUID.
		/// </param>
		/// <param name="errorMessage">
		/// Error message if there is no or there is more than 1 rule is found.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public static bool FindDefaultRule(SecurityClass mySecurity, Guid managerGuid, Guid productGuid, out Guid ruleGuid, out string errorMessage)
		{
            
            
            //Get the product's parent record if one exists
            ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(mySecurity, productGuid));
            Guid rootProductGuid = (product != null && 
                                    product.MasterRecordGuid != null && 
                                    !product.MasterRecordGuid.IsEmpty()) ? product.MasterRecordGuid : productGuid;
            //Get the manager company parent record guid if one exists
            CompanyClass company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(mySecurity, managerGuid));
            Guid rootCompanyGuid = (company != null &&
                                    company.MasterRecordGuid != null &&
                                    !company.MasterRecordGuid.IsEmpty()) ? company.MasterRecordGuid : managerGuid;

            AutoDistributionRuleDOCollection ruleList = 
				FMChannelHelper.MakeCall<IAutoDistributionRules, AutoDistributionRuleDOCollection>(
                                                    x => x.Enumerate(mySecurity, rootCompanyGuid, rootProductGuid, null));

			// filtered out disabled and non-default-eom rules.
			IEnumerable<AutoDistributionRuleDO> filteredList = ruleList.Where(rule => rule.Enabled && rule.DefaultEOM);

			int ruleCount = filteredList.Count();
			bool onlyOneRuleFound = ruleCount == 1;

			ruleGuid = Guid.Empty;
			errorMessage = string.Empty;

			if (onlyOneRuleFound)
			{
				ruleGuid = filteredList.First().IdentityGuid;
			}
			else
			{
				errorMessage = ruleCount == 0 ? NoDefaultEomRuleMessage : MoreThanOneDefaultEomRuleMessage;
				}

			return onlyOneRuleFound;
		}
		#endregion public methods

		#region private methods
		/// <summary>
		/// Select the item from the given dropdown list with the given GUID.
		/// </summary>
		/// <param name="targetDropDown">
		/// Dropdown control to be checked.
		/// </param>
		/// <param name="targetGuid">
		/// GUID to be checked.
		/// </param>
		/// <exception cref="ApplicationException">
		/// Invalid URL exception.
		/// </exception>
		private static void SelectEntityFromDropDown(DropDownList targetDropDown, Guid targetGuid)
		{
			bool found = false;

			for (int idx = 0; idx < targetDropDown.Items.Count; idx++)
			{
				Guid tempGuid;

				if (Guid.TryParse(targetDropDown.Items[idx].Value, out tempGuid) && tempGuid == targetGuid)
				{
					found = true;
					targetDropDown.SelectedIndex = idx;
					break;
				}
			}

			if (found == false)
			{
				throw new ApplicationException(InvalidUrlParameterMessage);
			}
		}


		/// <summary>
		/// Initialize the Operation Helper which helps calculate quantities and percentages.
		/// </summary>
		private void InitializeOperationHelper()
		{
			Guid productGuid = Guid.Empty;

			if (this.MySessionCurrentRule != null)
			{
				if (this.operationType == AutoDistributionOperationTypes.Manual)
				{

					if (this.productDropDown.SelectedIndex >= 0)
					{
						productGuid = new Guid(this.productDropDown.SelectedValue);
					}
				}
				else
				{
					productGuid = this.invReconInfo.ProductGuid;
				}

				if (productGuid != Guid.Empty)
				{
					this.myOperationHelper = FMChannelHelper.MakeCall<IAutoDistributionProcessor, AutoDistributionOperationHelper>(
						x => x.PrepareHelper(this.Security, this.Security.SiteGuid, this.MySessionCurrentRule.TransactionAliasGuid, productGuid));
				}
			}
		}

		/// <summary>
		/// Initialize page variables like number formats.
		/// Initialize control initial values like default dates and values 
		/// from Inventory Reconciliation page.
		/// </summary>
		private void InitializeVariablesAndControls()
		{
			SiteClass siteBo = this.GetSiteBo(this.Security.SiteGuid);

			// reset last focus control info.
			this.restoreFocusControlID.Value = string.Empty;

			this.InitializeOperationHelper();

			if (this.IsPostBack == false)
			{
				if (this.operationType == AutoDistributionOperationTypes.Manual)
				{
					DateTimeOffset yesterday = TimeConverter.Today(siteBo).AddDays(-1);

					this.thruputStartDateControl.CurrentValue = yesterday;
					this.thruputEndDateControl.CurrentValue = yesterday;

					this.inventoryDateControlManual.CurrentValue = yesterday;
				}
				else
				{
					SelectEntityFromDropDown(this.ruleIDDropDown, this.invReconInfo.RuleGuid);
					this.HandleRuleSelectionChanged();
					SelectEntityFromDropDown(this.managerDropDown, this.invReconInfo.ManagerGuid);
					SelectEntityFromDropDown(this.productDropDown, this.invReconInfo.ProductGuid);

					this.expectedGrossTextBox.Text = this.invReconInfo.Gross.ToString("N", this.myOperationHelper.VolumeTrxNumberFormat);
					this.expectedNetTextBox.Text = this.invReconInfo.Net.ToString("N", this.myOperationHelper.VolumeTrxNumberFormat);
					this.expectedMassTextBox.Text = this.invReconInfo.Mass.ToString("N", this.myOperationHelper.MassTrxNumberFormat);

					// Parse and set inventory dates
					this.inventoryMonthTextBox.Text = this.invReconInfo.InventoryMonth;
					DateTimeOffset startDate = DateEfficacy.convertYearMonthDayToDateTime(DateEfficacy.getFirstDayOfMonth(this.invReconInfo.InventoryMonth));

					this.thruputStartDateControl.CurrentValue = startDate;
					this.thruputEndDateControl.CurrentValue = this.thruputStartDateControl.CurrentValue.AddMonths(1).AddDays(-1);  // get the end of the month;
					this.inventoryDateControlInvRecon.CurrentValue = this.thruputEndDateControl.CurrentValue;
				}
			}
		}

		/// <summary>
		/// Initialize controls
		/// </summary>
		private void SetControlStates()
		{
			string quantitiesText;

			// Inventory reconciliation has Inventory Month, Manual mode has date range.
			// The following is the row to be hidden.
			HtmlTableRow dateRowToHide;

			if (this.operationType == AutoDistributionOperationTypes.Manual)
			{
				quantitiesText = QuantitiesManualLabelText;

				dateRowToHide = this.inventoryReconRow;
			}
			else
			{
				this.ruleIDDropDown.Enabled	 = false;
				this.managerDropDown.Enabled = false;
				this.productDropDown.Enabled = false;

				this.expectedGrossTextBox.Enabled	= false;
				this.expectedNetTextBox.Enabled		= false;
				this.expectedMassTextBox.Enabled	= false;

				quantitiesText = QuantitiesInvReconLabelText;

				dateRowToHide = this.manualRow;
			}

			quantitiesText			= this.GetTranslatedText(quantitiesText);
			this.quantityLabel.Text = quantitiesText + ":";
			this.totalLabel.Text	= quantitiesText;

			dateRowToHide.Visible = false;
			dateRowToHide.Style.Add("Display", "None");

			// Populate reason codes
			AutoDistributionReasonCodeCollectionClass reasonCodeList = AutoDistributionReasonCodesForm.GetReasonCodeList(this.Security);
			this.reasonCodeDropDown.Items.Clear();

			foreach (AutoDistributionReasonCodeClass currentEntity in reasonCodeList)
			{
				string itemText = string.Format("{0} - {1}", currentEntity.ID, currentEntity.Description);
				var item = new ListItem(itemText, currentEntity.IdentityGuid.ToString());
				this.reasonCodeDropDown.Items.Add(item);
			}
		}

		/// <summary>
		/// The populate rules.
		/// </summary>
		/// <exception cref="ApplicationException">
		/// No rule exception.
		/// </exception>
		private void PopulateRules()
		{
			AutoDistributionRuleDOCollection ruleList = FMChannelHelper.MakeCall<IAutoDistributionRules, AutoDistributionRuleDOCollection>(
				x => x.Enumerate(this.Security, Guid.Empty, Guid.Empty, null));

			this.MySessionRuleList = ruleList;

			foreach (AutoDistributionRuleDO theRule in ruleList)
			{
				if (theRule.Enabled)
				{
					var item = new ListItem(this.GetTranslatedText(theRule.ID), theRule.IdentityGuid.ToString());
					this.ruleIDDropDown.Items.Add(item);
				}
			}

			if (ruleList.Count == 0)
			{
				this.calculateButton.Enabled = false;
				throw new ApplicationException(NoRuleMessage);
			}

			this.HandleRuleSelectionChanged();
		}

		/// <summary>
		/// Reads the GUID for the URL parameter with the given key.
		/// </summary>
		/// <param name="parameterName">Name of the parameter</param>
		/// <returns>Value parsed from the parameter</returns>
		private Guid ReadUrlGuid(string parameterName)
		{
			Guid retValue;

			string quantityString = this.Request.GetQueryOrFormValue(parameterName);

			if (string.IsNullOrWhiteSpace(quantityString) || Guid.TryParse(quantityString, out retValue) == false)
			{
				throw new ArgumentException(InvalidUrlParameterMessage);
			}

			return retValue;
		}

		/// <summary>
		/// Reads the quantity(double) value from the url parameter with the given key.
		/// </summary>
		/// <param name="parameterName">Name of the parameter</param>
		/// <returns>Value parsed from the parameter</returns>
		private double ReadUrlDouble(string parameterName)
		{
			double retValue;

			string quantityString = this.Request.GetQueryOrFormValue(parameterName);

			if (string.IsNullOrWhiteSpace(quantityString) || double.TryParse(quantityString, out retValue) == false)
			{
				throw new ArgumentException(InvalidUrlParameterMessage);
			}

			return retValue;
		}

		/// <summary>
		/// The read URL parameters.
		/// </summary>
		/// <exception cref="ArgumentException">
		/// Invalid URL parameter.
		/// </exception>
		private void ReadUrlParameters()
		{
			string operationTypeString = this.Request.GetQueryOrFormValue(UrlParamOperationType);

			if (string.IsNullOrWhiteSpace(operationTypeString) ||
				Enum.TryParse<AutoDistributionOperationTypes>(operationTypeString, out this.operationType) == false)
			{
				throw new ArgumentException(InvalidUrlParameterMessage);
			}

			if (this.operationType == AutoDistributionOperationTypes.InventoryReconcilliation)
			{
				this.invReconInfo = new InventoryReconPageInfo();

				this.invReconInfo.RuleGuid = this.ReadUrlGuid(UrlParamRuleGuid);
				this.invReconInfo.ManagerGuid = this.ReadUrlGuid(UrlParamManagerGuid);
				this.invReconInfo.ProductGuid = this.ReadUrlGuid(UrlParamProductGuid);

				this.invReconInfo.Gross = this.ReadUrlDouble(UrlParamGross);
				this.invReconInfo.Net = this.ReadUrlDouble(UrlParamNet);
				this.invReconInfo.Mass = this.ReadUrlDouble(UrlParamMass);

				this.invReconInfo.InventoryMonth = this.Request.Params[UrlParamInventoryMonth];

				if (string.IsNullOrWhiteSpace(this.invReconInfo.InventoryMonth))
				{
					throw new ArgumentException(InvalidUrlParameterMessage);
				}
			}
		}

		/// <summary>
		/// The handle rule selection changed.
		/// </summary>
		private void HandleRuleSelectionChanged()
		{
			if (this.ruleIDDropDown.SelectedIndex != -1)
			{
				// get the rule
				var newRuleGuid						= new Guid(this.ruleIDDropDown.SelectedValue);
				this.MySessionCurrentRuleGuid		= newRuleGuid;
				AutoDistributionRuleDO currentRule	= this.MySessionCurrentRule;

				// set simple fields
				this.descriptionTextBox.Text			= currentRule.Description;
				this.transactionAliasTextBox.Text		= currentRule.TransactionAlias.ID;
				this.reasonCodeDropDown.SelectedValue	= currentRule.DefaultReasonCodeGuid.ToString();
				this.notesTextBox.Text					= currentRule.DefaultNotes;

				// Update dropdown list for managers and products
				var childList = new[]
					{ 
						new { ChildType = AutoDistributionRuleChildMapTypes.Manager, DropDownList = this.managerDropDown },
						new { ChildType = AutoDistributionRuleChildMapTypes.Product, DropDownList = this.productDropDown },
					};

				foreach (var child in childList)
				{
					child.DropDownList.Items.Clear();

					List<BaseMapAssignedInfoDO> entityList = 
						FMChannelHelper.MakeCall<IAutoDistributionRules, List<BaseMapAssignedInfoDO>>(
																x => x.EnumerateAssigned(this.Security, newRuleGuid, child.ChildType));

					foreach (BaseMapAssignedInfoDO currentEntity in entityList)
					{
						var item = new ListItem(this.GetTranslatedText(currentEntity.ID), currentEntity.AssignedGuid.ToString());
						child.DropDownList.Items.Add(item);
					}
				}
			}
		}

		/// <summary>
		/// The get selected GUID value from dropdown.
		/// </summary>
		/// <param name="theList">
		/// The list.
		/// </param>
		/// <param name="errorMsg">
		/// The error message.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <exception cref="ApplicationException">
		/// </exception>
		private static Guid GetSelectedGuidValueFromDropDown(FMDropDownList theList, string errorMsg)
		{
			var targetGuid = Guid.Empty;

			if (theList.SelectedIndex >= 0)
			{
				targetGuid = new Guid(theList.SelectedValue);
			}

			if (targetGuid.IsEmpty())
			{
				throw new ApplicationException(errorMsg);
			}

			return targetGuid;
		}

		/// <summary>
		/// The get double value from text box.
		/// </summary>
		/// <param name="theTextBox">
		/// The text box.
		/// </param>
		/// <param name="formatProvider">
		/// The format provider.
		/// </param>
		/// <param name="errorMsg">
		/// The error message.
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		/// <exception cref="ApplicationException">
		/// Invalidate value.
		/// </exception>
		private static double GetDoubleValueFromTextBox(FMTextBox theTextBox, IFormatProvider formatProvider, string errorMsg)
		{
			double retValue;

			if (string.IsNullOrWhiteSpace(theTextBox.Text))
			{
				retValue = 0;
			}
			else if (double.TryParse(theTextBox.Text, NumberStyles.Any, formatProvider, out retValue) == false)
			{
				throw new ApplicationException(errorMsg);
			}

			return retValue;
		}

		/// <summary>
		/// Retrieves values from the page.
		/// </summary>
		/// <param name="selectedRuleGuid">Selected Rule GUID</param>
		/// <param name="selectedManagerGuid">Selected Manager GUID</param>
		/// <param name="selectedProductGuid">Selected Product GUID</param>
		/// <param name="thruputStartDate">Throughput Start Date</param>
		/// <param name="thruputEndDate">Throughput End Date</param>
		/// <param name="totalGross">Total Gross</param>
		/// <param name="totalNet">Total Net</param>
		/// <param name="totalMass">Total Mass</param>
		private void CollectParameters(
									out Guid selectedRuleGuid, 
									out Guid selectedManagerGuid, 
									out Guid selectedProductGuid,
									out DateTimeOffset thruputStartDate, 
									out DateTimeOffset thruputEndDate,
									out double totalGross, 
									out double totalNet, 
									out double totalMass)
		{
			selectedRuleGuid = GetSelectedGuidValueFromDropDown(this.ruleIDDropDown, InvalidRuleMessage);
			selectedManagerGuid = GetSelectedGuidValueFromDropDown(this.managerDropDown, InvalidManagerMessage);
			selectedProductGuid = GetSelectedGuidValueFromDropDown(this.productDropDown, InvalidProductMessage);

			thruputStartDate = this.thruputStartDateControl.CurrentValue;
			thruputEndDate = this.thruputEndDateControl.CurrentValue;

			if (thruputEndDate < thruputStartDate)
			{
				throw new ApplicationException(InvalidDatesMessage);
			}

			totalGross = GetDoubleValueFromTextBox(this.expectedGrossTextBox, this.myOperationHelper.VolumeTrxNumberFormat, InvalidGrossMessage);
			totalNet   = GetDoubleValueFromTextBox(this.expectedNetTextBox, this.myOperationHelper.VolumeTrxNumberFormat, InvalidNetMessage);
			totalMass  = GetDoubleValueFromTextBox(this.expectedMassTextBox, this.myOperationHelper.MassTrxNumberFormat, InvalidMassMessage);

		}

		/// <summary>
		/// Show the footer of the table. 
		/// </summary>
		/// <param name="toShow">Show or hide the bottom Division</param>
		private void ShowBottomDiv(bool toShow)
		{
			this.bottomDiv.Visible = toShow;
		}

		/// <summary>
		/// The clear grid.
		/// </summary>
		private void ClearGrid()
		{
			this.ApplicationGrid.DataSource = null;
			this.ApplicationGrid.DataBind();
		}

		/// <summary>
		/// Due to rounding error, straight compare may not work
		/// e.g. 2318.8 + 568.8 = 2887.6000000000004 or 299.1 + 299.8 = 598.90000000000009.
		/// </summary>
		/// <param name="accumulatedValue">
		/// The accumulated value.
		/// </param>
		/// <param name="expectedValue">
		/// The expected value.
		/// </param>
		/// <param name="quantityType">
		/// The quantity type.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private bool AreTwoValuesTheSame(double accumulatedValue, double expectedValue, AutoDistributionQuantityTypes quantityType)
		{
			return this.myOperationHelper.FMRoundingByQuantityType(accumulatedValue, quantityType) == this.myOperationHelper.FMRoundingByQuantityType(expectedValue, quantityType);
		}

		/// <summary>
		/// The update view.
		/// </summary>
		/// <param name="justSort">
		/// The just sort.
		/// </param>
		/// <param name="isFirstTimeCalculating">
		/// The first time calculating.
		/// </param>
		private void UpdateView(bool justSort, bool isFirstTimeCalculating)
		{
			DataTable distributionData = null;

			if (justSort == false)
			{
				double totalMass;
				double totalNet;
				double totalGross;

				DateTimeOffset thruputEndDate;
				DateTimeOffset thruputStartDate;

				Guid selectedProductGuid;
				Guid selectedManagerGuid;
				Guid selectedRuleGuid;

				this.CollectParameters(
									out selectedRuleGuid,
									out selectedManagerGuid,
									out selectedProductGuid,
									out thruputStartDate,
									out thruputEndDate,
									out totalGross, 
									out totalNet, 
									out totalMass);

				double[] expectedTotalQuantities = new[] { totalGross, totalNet, totalMass };

				distributionData = this.EnumerateData(
												isFirstTimeCalculating, 
												selectedRuleGuid, 
												selectedManagerGuid, 
												selectedProductGuid,
												thruputStartDate, 
												thruputEndDate, 
												expectedTotalQuantities);

				// Update totals 
				bool totalMatchExpected = true;
				bool volumeBalanceFlag = true;
				string outOfBalanceErrorMsg = string.Empty;
				const double DefaultFraction = 1; // this is 100% when total quantities = 0

				this.Session.Remove(PageSessionKeyConstants.AutoDistributionOperationBalanceFlag);
				this.Session.Remove(PageSessionKeyConstants.AutoDistributionOperationWarningMsg);

				foreach (AutoDistributionQuantityTypes quantityType in Enum.GetValues(typeof(AutoDistributionQuantityTypes)))
				{
					double theQuantityPercent = AutoDistributionOperationHelper.CalculateFraction(
																				this.myOperationHelper.AccumulatedQuantities[(int)quantityType],
																				expectedTotalQuantities[(int)quantityType], 
																				DefaultFraction);
					theQuantityPercent = theQuantityPercent * 100;

					double accmulatedQuantity = this.myOperationHelper.AccumulatedQuantities[(int)quantityType];
					double expectedTotal	  = expectedTotalQuantities[(int)quantityType];
					totalMatchExpected		  = totalMatchExpected && this.AreTwoValuesTheSame(accmulatedQuantity, expectedTotal, quantityType);

					this.UpdateTotalLabel(quantityType, AutoDistributionColumnTypes.Thruput, this.myOperationHelper.TotalThruputs[(int)quantityType]);
					this.UpdateTotalLabel(quantityType, AutoDistributionColumnTypes.ThruputPercent, 100);
					this.UpdateTotalLabel(quantityType, AutoDistributionColumnTypes.Quantity, this.myOperationHelper.AccumulatedQuantities[(int)quantityType]);
					this.UpdateTotalLabel(quantityType, AutoDistributionColumnTypes.QuantityPercent, theQuantityPercent);

					// Build error message for the volume that is out of balance.
					if (totalMatchExpected == false && volumeBalanceFlag)
					{
						string volumeName = quantityType.ToString();
						outOfBalanceErrorMsg = string.Format(
															"{0} volume is out of balance. Expected = {1}; Accumulated = {2}.", 
															volumeName, 
															expectedTotal, 
															accmulatedQuantity);
						
						volumeBalanceFlag = false;
					}
				}

				// Show the table footer and create button
				if (isFirstTimeCalculating)
				{
					this.ShowBottomDiv(true);
				}

				this.Session.Add(PageSessionKeyConstants.AutoDistributionOperationBalanceFlag, volumeBalanceFlag);
				this.Session.Add(PageSessionKeyConstants.AutoDistributionOperationWarningMsg, outOfBalanceErrorMsg);
			}

			if (justSort || isFirstTimeCalculating)
			{
				// Create a new table with same structure
				DataTable originalTable = this.MySessionDistributionList;

				// Do the sorting
				string sortInfo = string.Format("{0} {1}", this.MySessionSortExpression, this.MySessionSortDirection);
				DataRow[] tempRows = originalTable.Select(string.Empty, sortInfo);

				// Copy to the original table
				distributionData = originalTable.Clone();
				distributionData.Rows.Clear();

				foreach (DataRow currentRow in tempRows)
				{
					distributionData.ImportRow(currentRow);
				}

				this.MySessionDistributionList = distributionData;
			}

			this.ApplicationGrid.DataSource = new DataView(distributionData);
			this.ApplicationGrid.DataBind();
		}

		/// <summary>
		/// The update total label.
		/// </summary>
		/// <param name="quantityType">
		/// The quantity type.
		/// </param>
		/// <param name="columnType">
		/// The column type.
		/// </param>
		/// <param name="newValue">
		/// The new value.
		/// </param>
		private void UpdateTotalLabel(AutoDistributionQuantityTypes quantityType, AutoDistributionColumnTypes columnType, double newValue)
		{
			Label targetLabel = null;

			switch (quantityType)
			{
				case AutoDistributionQuantityTypes.Gross:
					switch (columnType)
					{
						case AutoDistributionColumnTypes.Thruput:
							targetLabel = this.totalGrossThruputLabel;
							break;
						case AutoDistributionColumnTypes.ThruputPercent:
							targetLabel = this.totalGrossThruputPercentLabel;
							break;
						case AutoDistributionColumnTypes.Quantity:
							targetLabel = this.totalGrossLabel;
							break;
						case AutoDistributionColumnTypes.QuantityPercent:
							targetLabel = this.totalGrossPercentLabel;
							break;
					}

					break;

				case AutoDistributionQuantityTypes.Net:
					switch (columnType)
					{
						case AutoDistributionColumnTypes.Thruput:
							targetLabel = this.totalNetThruputLabel;
							break;
						case AutoDistributionColumnTypes.ThruputPercent:
							targetLabel = this.totalNetThruputPercentLabel;
							break;
						case AutoDistributionColumnTypes.Quantity:
							targetLabel = this.totalNetLabel;
							break;
						case AutoDistributionColumnTypes.QuantityPercent:
							targetLabel = this.totalNetPercentLabel;
							break;
					}

					break;

				case AutoDistributionQuantityTypes.Mass:
					switch (columnType)
					{
						case AutoDistributionColumnTypes.Thruput:
							targetLabel = this.totalMassThruputLabel;
							break;
						case AutoDistributionColumnTypes.ThruputPercent:
							targetLabel = this.totalMassThruputPercentLabel;
							break;
						case AutoDistributionColumnTypes.Quantity:
							targetLabel = this.totalMassLabel;
							break;
						case AutoDistributionColumnTypes.QuantityPercent:
							targetLabel = this.totalMassPercentLabel;
							break;
					}

					break;
			}

			NumberFormatInfo numberFormat = this.myOperationHelper.GetNumberFormatInfo(quantityType, columnType);

			if (targetLabel != null)
			{
				targetLabel.Text = newValue.ToString("N", numberFormat);
			}
		}

		/// <summary>
		/// The restore focus.
		/// </summary>
		private void RestoreFocus()
		{
			string lastControlId = this.lastFocusControlID.Value;
			if (string.IsNullOrWhiteSpace(lastControlId) == false)
			{
				this.restoreFocusControlID.Value = lastControlId;
			}
		}

		/// <summary>
		/// When manager, product or inventory dates got changed, the grid is wiped.
		/// </summary>
		private void CriticalInfoChanged()
		{
			if (this.bottomDiv.Visible)
			{
				this.ShowBottomDiv(false);
				this.ClearGrid();
			}

			this.RestoreFocus();
		}

		/// <summary>
		/// The quantity grid changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="quantityType">
		/// The quantity type.
		/// </param>
		/// <exception cref="ApplicationException">
		/// Invalid quantity exception.
		/// </exception>
		private void QuantityInGridChanged(object sender, AutoDistributionQuantityTypes quantityType)
		{
			var srcTextBox = sender as FMTextBox;
			Common.ValidateObject(srcTextBox);

			// Get the current row in grid
			if (srcTextBox != null)
			{
				var currentRow = srcTextBox.Parent.Parent as GridViewRow;
			Common.ValidateObject(currentRow);

			// get the new quantity
			double newQuantity;

				if (double.TryParse(srcTextBox.Text, NumberStyles.Any, this.myOperationHelper.GetNumberFormatInfo(quantityType, AutoDistributionColumnTypes.Quantity), out newQuantity) == false)
			{
				throw new ApplicationException(InvalidQuantityEnteredMessage);
			}

			// Save quantity to the data source
				DataTable mainDataTable = this.MySessionDistributionList;

				// If the user enters more decimal places than configured
			// sum of 3.4, 3.4 3.4 equals 10 but showing up as 3, 3, 3 and 10
				newQuantity = this.myOperationHelper.FMRoundingByQuantityType(newQuantity, quantityType);

                if (currentRow != null)
                {
                    AutoDistributionOperationHelper.SaveRowDataByType(mainDataTable.Rows[currentRow.RowIndex], quantityType, AutoDistributionColumnTypes.Quantity, newQuantity);
                }

				this.MySessionDistributionList = mainDataTable;
			}

			// Update calculations
			this.UpdateView(false, false);	// false = not JustSort, false = not firstTimeCalculating

			this.RestoreFocus();
		}

		/// <summary>
		/// The get throughput from server.
		/// </summary>
		/// <param name="selectedRuleGuid">
		/// The selected rule GUID.
		/// </param>
		/// <param name="selectedManagerGuid">
		/// The selected manager GUID.
		/// </param>
		/// <param name="selectedProductGuid">
		/// The selected product GUID.
		/// </param>
		/// <param name="thruputStartDate">
		/// The throughput start date.
		/// </param>
		/// <param name="thruputEndDate">
		/// The throughput end date.
		/// </param>
		/// <returns>
		/// The <see cref="DataTable"/>.
		/// </returns>
		private DataTable GetThruputFromServer(
											Guid selectedRuleGuid, 
											Guid selectedManagerGuid, 
											Guid selectedProductGuid,
											DateTimeOffset thruputStartDate, 
											DateTimeOffset thruputEndDate)
		{
			var requestData = new AutoDistributionThruputSR()
			{
				RuleGuid = selectedRuleGuid,
				ManagerGuid = selectedManagerGuid,
				ProductGuid = selectedProductGuid,
				StartDate = thruputStartDate,
				EndDate = thruputEndDate
			};

			return FMChannelHelper.MakeCall<IAutoDistributionProcessor,DataTable>(x => x.CalculateThruput(this.Security, requestData));
		}

		/// <summary>
		/// The enumerate data.
		/// </summary>
		/// <param name="isFirstTimeCalculating">
		/// The first time calculating.
		/// </param>
		/// <param name="selectedRuleGuid">
		/// The selected rule GUID.
		/// </param>
		/// <param name="selectedManagerGuid">
		/// The selected manager GUID.
		/// </param>
		/// <param name="selectedProductGuid">
		/// The selected product GUID.
		/// </param>
		/// <param name="thruputStartDate">
		/// The throughput start date.
		/// </param>
		/// <param name="thruputEndDate">
		/// The throughput end date.
		/// </param>
		/// <param name="expectedTotalQuantities">
		/// The expected total quantities.
		/// </param>
		/// <returns>
		/// The <see cref="DataTable"/>.
		/// </returns>
		private DataTable EnumerateData(
										bool isFirstTimeCalculating, 
										Guid selectedRuleGuid, 
										Guid selectedManagerGuid, 
										Guid selectedProductGuid,
										DateTimeOffset thruputStartDate, 
										DateTimeOffset thruputEndDate,
			double[] expectedTotalQuantities)
		{
			DataTable mainDataTable;

			if (isFirstTimeCalculating)
			{
				mainDataTable = this.GetThruputFromServer(
														selectedRuleGuid, 
														selectedManagerGuid, 
														selectedProductGuid,
														thruputStartDate, 
														thruputEndDate);
			}
			else
			{
				mainDataTable = this.MySessionDistributionList;
			}

			this.myOperationHelper.Calculate(mainDataTable, isFirstTimeCalculating, expectedTotalQuantities);

			this.MySessionDistributionList = mainDataTable;

			return mainDataTable;
		}

		/// <summary>
		/// The get transaction alias.
		/// </summary>
		/// <param name="transactionAliasGuid">
		/// The transaction alias GUID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasClass"/>.
		/// </returns>
		private TransactionAliasClass GetTransactionAliasBo(Guid transactionAliasGuid)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(x => x.Get(this.Security, transactionAliasGuid, false));
		}

		/// <summary>
		/// The get product.
		/// </summary>
		/// <param name="productGuid">
		/// The product GUID.
		/// </param>
		/// <returns>
		/// The <see cref="ProductClass"/>.
		/// </returns>
		private ProductClass GetProductBo(Guid productGuid)
		{
			return FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(this.Security, productGuid));
		}

		/// <summary>
		/// The get company.
		/// </summary>
		/// <param name="companyGuid">
		/// The company GUID.
		/// </param>
		/// <returns>
		/// The <see cref="CompanyClass"/>.
		/// </returns>
		private CompanyClass GetCompanyBo(Guid companyGuid)
		{
			return FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(this.Security, companyGuid));
		}

		/// <summary>
		/// The get site.
		/// </summary>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="SiteClass"/>.
		/// </returns>
		private SiteClass GetSiteBo(Guid siteGuid)
		{
			return FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, siteGuid, false, false, false));
		}

		/// <summary>
		/// The copy data to transaction.
		/// </summary>
		/// <param name="newDistribution">
		/// The new distribution.
		/// </param>
		/// <param name="siteBo">
		/// The site.
		/// </param>
		/// <param name="trxAliasBo">
		/// The trx alias.
		/// </param>
		/// <param name="managerBo">
		/// The manager.
		/// </param>
		/// <param name="ownerGuid">
		/// The owner GUID.
		/// </param>
		/// <param name="inventoryDate">
		/// The inventory date.
		/// </param>
		/// <param name="reasonCodeGuid">
		/// The reason code GUID.
		/// </param>
		/// <param name="transactionNotes">
		/// The transaction notes.
		/// </param>
		private void CopyDataToTransaction(
										TransactionDO newDistribution, 
										SiteClass siteBo, 
										TransactionAliasClass trxAliasBo, 
										CompanyClass managerBo, 
										Guid ownerGuid, 
										DateTime inventoryDate, 
										Guid reasonCodeGuid, 
										string transactionNotes)
		{
			DateTimeOffset siteTimeNow = TimeConverter.Now(siteBo);

			newDistribution.TransID = FuelsManagerId.NewId();

			newDistribution.Site = this.Security.SiteID;
			newDistribution.SiteGuid = this.Security.SiteGuid;

			newDistribution.Alias = trxAliasBo.ID;
			newDistribution.TransactionAliasGuid = trxAliasBo.MasterRecordGuid;

			newDistribution.TransTypeID = trxAliasBo.TransTypeID;
			newDistribution.Status = TransactionStatus.Completed;
			newDistribution.InventoryDate = inventoryDate;
			newDistribution.TransactionDateTime = siteTimeNow;

			newDistribution.ManagerCode = managerBo.Code;
			newDistribution.ManagerID = managerBo.ID;
			newDistribution.ManagerCompanyGuid = managerBo.MasterRecordGuid;

			CompanyClass ownerBo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(this.Security, ownerGuid));

			newDistribution.OwnerCode = ownerBo.Code;
			newDistribution.OwnerID = ownerBo.ID;
			newDistribution.OwnerCompanyGuid = ownerBo.MasterRecordGuid;

			newDistribution.ReasonCodeGuid = reasonCodeGuid;
			newDistribution.Notes = transactionNotes;
		}

		/// <summary>
		/// The copy data to transaction line.
		/// </summary>
		/// <param name="lineItem">
		/// The line item.
		/// </param>
		/// <param name="productBo">
		/// The product.
		/// </param>
		/// <param name="grossQuantity">
		/// The gross quantity.
		/// </param>
		/// <param name="netQuantity">
		/// The net quantity.
		/// </param>
		/// <param name="massQuantity">
		/// The mass quantity.
		/// </param>
		private static void CopyDataToTransactionLine(
													LineItemDO lineItem, 
													ProductClass productBo,
													double grossQuantity, 
													double netQuantity, 
													double massQuantity)
		{
			lineItem.Status		 = TransactionStatus.Completed;
			lineItem.Product	 = productBo.ID;
			lineItem.ProductCode = productBo.Code;
			lineItem.ProductGuid = productBo.MasterRecordGuid;
			lineItem.ProductType = ProductClass.ProductTypeID(productBo.ProductType);

			lineItem.Quantity = new QuantityDO { Gross = grossQuantity, Net = netQuantity, Mass = massQuantity };
		}

		/// <summary>
		/// The get inventory date.
		/// </summary>
		/// <returns>
		/// The <see cref="DateTimeOffset"/>.
		/// </returns>
		private DateTime GetInventoryDate()
		{
			DateTime inventoryDate;

			if (this.operationType == AutoDistributionOperationTypes.InventoryReconcilliation)
			{
				inventoryDate = this.inventoryDateControlInvRecon.CurrentValue.Date;
			}
			else
			{
				inventoryDate = this.inventoryDateControlManual.CurrentValue.Date;
			}

			return inventoryDate;
		}

		/// <summary>
		/// The create distributions.
		/// </summary>
		private void CreateDistributions()
		{
			// Prepare info shared by all transactions
			DataTable mainDataTable = this.MySessionDistributionList;

			SiteClass siteBo				 = this.GetSiteBo(this.Security.SiteGuid);
			TransactionAliasClass trxAliasBo = this.GetTransactionAliasBo(this.MySessionCurrentRule.TransactionAliasGuid);
			CompanyClass managerBo			 = this.GetCompanyBo(new Guid(this.managerDropDown.SelectedValue));
			ProductClass productBo			 = this.GetProductBo(new Guid(this.productDropDown.SelectedValue));

			Guid reasonCodeGuid			 = new Guid(this.reasonCodeDropDown.SelectedValue);
			string transactionNotes		 = this.notesTextBox.Text;
			DateTime inventoryDate = this.GetInventoryDate();

			var saveSr = new SaveTransactionsSR
				             {
					             Security = this.Security,
					             CurrentSiteGuid = this.Security.SiteGuid,
					             ConvertUnits = true
				             };

			// Prepare service/helper used beloew
			var unitsHelper = new UnitsHelperClass(this.Security, siteBo, trxAliasBo, productBo);

			foreach (DataRow currentRow in mainDataTable.Rows)
			{
				double grossQuantity = AutoDistributionOperationHelper.GetRowDataByType(currentRow, AutoDistributionQuantityTypes.Gross, AutoDistributionColumnTypes.Quantity);
				double netQuantity = AutoDistributionOperationHelper.GetRowDataByType(currentRow, AutoDistributionQuantityTypes.Net, AutoDistributionColumnTypes.Quantity);
				double massQuantity = AutoDistributionOperationHelper.GetRowDataByType(currentRow, AutoDistributionQuantityTypes.Mass, AutoDistributionColumnTypes.Quantity);

				// No need to create a transaction if there is no quantity for the current owner
				if (grossQuantity == 0 && netQuantity == 0 && massQuantity == 0)
				{
					continue;
				}

				// Main Transaction Record
				var newDistribution = new TransactionDO();
				var ownerGuid = (Guid)currentRow[AutoDistributionOperationHelper.OwnerGuidColumnName];
				this.CopyDataToTransaction(newDistribution, siteBo, trxAliasBo, managerBo, ownerGuid, inventoryDate, reasonCodeGuid, transactionNotes);
				unitsHelper.SetUnits(newDistribution, productBo.ProductType);

				// Transaction Line
				var lineItem = new LineItemDO();
				newDistribution.LineItems.Add(lineItem);

				CopyDataToTransactionLine(lineItem, productBo, grossQuantity, netQuantity, massQuantity);
				unitsHelper.SetUnits(lineItem, productBo.ProductType, productBo);

				saveSr.Transactions.Add(newDistribution);
			}

			SaveTransactionsResultDO results = 
				FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(x => x.SaveTransactions(saveSr));

			TransactionDetailBase.CheckForAndDisplayWarningMessages(results, this.Security.SiteGuid, this, null);
		}
		#endregion
	}
}