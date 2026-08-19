// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaxRateExciseSummaryForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TaxRateExciseSummaryForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FinanceWebApp
{
	using System;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using Accounting;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

    public partial class TaxRateExciseSummaryForm : AccountingWebFormView
	{
		#region Constants and Fields

		private string dateFormat;

		private ExciseTaxDOCollection exciseCollection;

		#endregion

		#region Properties

		/// <summary>
		///    This property returns the data format.
		/// </summary>
		protected string DateFormat
		{
			get
			{
				return this.dateFormat;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will handle the add button (bottom button) event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddBottomButtonOnClick(object sender, EventArgs e)
		{
			this.AddNewExcise();
		}

		/// <summary>
		///    This method will handle the add button (top button) event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddTopButtonOnClick(object sender, EventArgs e)
		{
			this.AddNewExcise();
		}

		/// <summary>
		///    This method handles the grid's item data binding. It will disable the edit and delete buttons in
		///    the grid if the user does not have premissions.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		protected void ExciseDataGrid_ItemDataBound(object source, DataGridItemEventArgs eventArgs)
		{
			try
			{
				var editButton = (LinkButton)eventArgs.Item.FindControl("btnEdit");
				var deleteButton = (LinkButton)eventArgs.Item.FindControl("btnDelete");

				// Disable the edit and delete buttons if the user does not have modify rights
				if ((editButton != null) && (deleteButton != null))
				{
					if (base.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
					{
						editButton.Enabled = false;
						deleteButton.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
				base.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method handles the grid size dropdown change. It will update the
		///    grid size accordingly.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void GridSizeDropdownOnChange(object sender, EventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.ExciseDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.UpdateView();
		}

		/// <summary>
		///    This method will handle the On Init event for the page. It will initialize the base
		///    page OnInit and setup event handlers.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
			base.Initialize();
		}

		/// <summary>
		///    This is the main entry point for the Excise Summary page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				if (base.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
				{
					this.EnableControls(false);
				}
			}

			// Retrieve the grid data and update the view.
			this.RefreshButtonOnClick(null, null);
		}

		/// <summary>
		///    This method handles the refresh button event. It will retrieve excise
		///    from the database based on the filter settings and update the grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void RefreshButtonOnClick(object sender, EventArgs e)
		{
			string productID = null;
			bool hasDates = false;
			bool fromButtonClick = false;

			if (sender != null)
			{
				fromButtonClick = true;
			}

			this.RetrieveAndValidateFilters(fromButtonClick);

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_PRODUCT_ID] != null)
			{
				productID = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_PRODUCT_ID] as string;

				// If there is a { or }, then it means there is an "ALL". Set the product ID
				// to null in order to retrieve excise for all products.
				if (productID.Contains("{") || productID.Contains("}"))
				{
					productID = null;
				}
			}

			if ((this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_STARTDATE] != null)
			    && (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_ENDDATE] != null))
			{
				hasDates = true;
			}

			if (hasDates)
			{
				var startDate = (DateTimeOffset)this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_STARTDATE];
				var endDate = (DateTimeOffset)this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_ENDDATE];

				this.exciseCollection = FMChannelHelper.MakeCall<IExcises, ExciseTaxDOCollection>(
																	 x =>
																	 x.GetForProductAndDateRange(productID, startDate, endDate, base.security)
																);

			}
			else
			{
				this.exciseCollection = FMChannelHelper.MakeCall<IExcises, ExciseTaxDOCollection>(
																	 x =>
																	 x.GetForProduct(productID, base.security)
																);
			}

			// Update the view with the new excise data.
			this.UpdateView();
		}

		/// <summary>
		///    This method will create a new Excise object, place it into session and redirect
		///    the adding to the Excise detail page.
		/// </summary>
		private void AddNewExcise()
		{
			var newExcise = new ExciseTaxDO();
			newExcise.IdentityGuid = Guid.Empty;
			newExcise.ExciseDate = DateTimeOffset.Now;
			newExcise.ExciseRate = 0.0;
			newExcise.Product = "";
			newExcise.ProductGuid = Guid.Empty;

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT);
			}

			this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT, newExcise);
			this.Redirect("TaxExciseDetailForm.aspx?Mode=Add");
		}

		/// <summary>
		///    This method enables controls based on the input.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			this.AddTopButton.Enabled = enable;
			this.AddBottomButton.Enabled = enable;
			this.RefreshButton.Enabled = enable;
			this.ProductSelectControl.Enabled = enable;
			this.StartDateControl.Enabled = enable;
			this.EndDateControl.Enabled = enable;
		}

		/// <summary>
		///    This method will handle the deletion of an item in the grid.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void ExciseDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			// Find the Excise to delete
			ExciseTaxDO selectedExcise = null;
			Guid selectedGuid = Guid.Parse(e.CommandArgument.ToString());

			foreach (ExciseTaxDO exciseDO in this.exciseCollection)
			{
				if (exciseDO.IdentityGuid == selectedGuid)
				{
					selectedExcise = exciseDO;
					break;
				}
			}

			try
			{
				FMChannelHelper.MakeCall<IExcises>(
																	 x =>
																	 x.Remove(selectedExcise, base.security)
																);
			}
			catch (Exception except)
			{
				base.ErrorHandler(except);
			}

			// Now remove the selected Excise from the collection
			this.exciseCollection.RemoveByIdentityGuid(selectedExcise);

			this.UpdateView();
		}

		/// <summary>
		///    This method handles the edit event. It will redirect the item being edit to the
		///    Excise detail page.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void ExciseDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			Guid exciseItemGuid = Guid.Parse(e.CommandArgument.ToString());
			int selectedIndex = e.Item.ItemIndex;
			ExciseTaxDO selectedExcise = null;

			if ((selectedIndex >= 0) && (selectedIndex < this.exciseCollection.Count))
			{
				// This is an existing Excise so find it
				foreach (ExciseTaxDO ecciseDO in this.exciseCollection)
				{
					if (ecciseDO.IdentityGuid == exciseItemGuid)
					{
						selectedExcise = ecciseDO;
						break;
					}
				}
			}

			if (selectedExcise == null)
			{
				string errMsg = "Excise Selected object not found.";
				base.ErrorHandler(new Exception(errMsg));
			}
			else
			{
				if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT] != null)
				{
					this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT);
				}

				this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT, selectedExcise);
				this.Redirect("TaxExciseDetailForm.aspx?Mode=edit");
			}
		}

		/// <summary>
		///    This method will handle the page index change. It will update the view to the
		///    new page.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void ExciseDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.ExciseDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.ExciseDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.RefreshButtonOnClick(null, null);
		}

		/// <summary>
		///    This method will initialize event handles.
		/// </summary>
		private void InitializeComponent()
		{
			this.ExciseDataGrid.EditCommand += new DataGridCommandEventHandler(this.ExciseDataGrid_EditCommand);
			this.ExciseDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.ExciseDataGrid_DeleteCommand);
			this.ExciseDataGrid.ItemDataBound += new DataGridItemEventHandler(this.ExciseDataGrid_ItemDataBound);
			this.ExciseDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.ExciseDataGrid_PageIndexChanged);
		}

		/// <summary>
		///    This method will retrieve the filters from the page and validate the
		///    dates. It will throw an exception if the start date is greater than
		///    the end date.
		/// </summary>
		private void RetrieveAndValidateFilters(bool fromButtonClick)
		{
			string productID = null;
			bool hasStartDate = false;
			bool hasEndDate = false;
			DateTimeOffset startDate = DateTimeOffset.Now;
			DateTimeOffset endDate = DateTimeOffset.Now;

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_PRODUCT_ID] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_PRODUCT_ID);
			}

			if ((this.ProductSelectControl.Text != null) && (this.ProductSelectControl.Text.Length > 0))
			{
				productID = this.ProductSelectControl.Text;
				this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_PRODUCT_ID, productID);
			}
			else
			{
				string newValue = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "{All}")
																);

				this.ProductSelectControl.Text = newValue;
				this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_PRODUCT_ID, newValue);
			}

			if (string.IsNullOrEmpty(this.StartDateControl.Text))
			{
				if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_STARTDATE] != null)
				{
					this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_STARTDATE);
				}
			}
			else
			{
				hasStartDate = true;
				startDate = this.StartDateControl.CurrentValue;
			}

			if (string.IsNullOrEmpty(this.EndDateControl.Text))
			{
				if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_ENDDATE] != null)
				{
					this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_ENDDATE);
				}
			}
			else
			{
				hasEndDate = true;
				endDate = this.EndDateControl.CurrentValue;
			}

			// Make sure that the start date is before the end date.
			if (hasStartDate && hasEndDate)
			{
				int indication = startDate.CompareTo(endDate);

				// -1 = start date is less than end date
				// 0  = start date is equal to end date
				// 1  = start date is greater than end date
				if (indication > 0)
				{
					string errMsg = "Start Date must be before End Date.";
					base.ErrorHandler(new Exception(errMsg));
				}

				if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_STARTDATE] != null)
				{
					this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_STARTDATE);
				}

				this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_STARTDATE, startDate);

				if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_ENDDATE] != null)
				{
					this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_ENDDATE);
				}

				this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_ENDDATE, endDate);
			}
			else if (hasStartDate && (hasEndDate == false) && fromButtonClick)
			{
				string errMsg = "Must have end date";
				base.ErrorHandler(new Exception(errMsg));
			}
			else if (hasEndDate && (hasStartDate == false) && fromButtonClick)
			{
				string errMsg = "Must have start date";
				base.ErrorHandler(new Exception(errMsg));
			}
		}

		/// <summary>
		///    This method updates the Excise Summary grid with new data.
		/// </summary>
		private void UpdateView()
		{
			this.GridSizeDropDown.SetPageSize(this.ExciseDataGrid, this.exciseCollection.Count);
			this.ExciseDataGrid.DataSource = this.exciseCollection;

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																		base.security,
																		base.security.SiteGuid,
																		getMemberSites: true,
																		getSchedulesAndProcessVariables: true,
																		bGetAssociatedAliases: true)
																	);

			DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();
			this.dateFormat = "{0:" + dateTimeFormatInfo.ShortDatePattern + "}"; // used in aspx 
			this.ExciseDataGrid.DataBind();
		}

		#endregion
	}
}