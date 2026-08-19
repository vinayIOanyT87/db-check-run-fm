// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaxRateGstSummaryForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TaxRateGstSummaryForm type.
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

    public partial class TaxRateGstSummaryForm : AccountingWebFormView
	{
		#region Constants and Fields

		private string dateFormat;

		private GoodsAndServicesTaxDOCollection gstCollection;

		#endregion

		#region Properties

		/// <summary>
		///    This property returns the data format.
		/// </summary>
		protected string DateFormat => this.dateFormat;

        #endregion

		#region Methods

		/// <summary>
		///    This method will handle the add button (bottom button) event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddButtonBottomClick(object sender, EventArgs e)
		{
			this.AddNewGst();
		}

		/// <summary>
		///    This method will handle the add button (top button) event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddButtonTopClick(object sender, EventArgs e)
		{
			this.AddNewGst();
		}

		/// <summary>
		///    This method handles the grid's item data binding. It will disable the edit and delete buttons in
		///    the grid if the user does not have premissions.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		protected void GstDataGridItemDataBound(object source, DataGridItemEventArgs eventArgs)
		{
			try
			{
				var editButton = (LinkButton)eventArgs.Item.FindControl("btnEdit");
				var deleteButton = (LinkButton)eventArgs.Item.FindControl("btnDelete");

				// Disable the edit and delete buttons if the user does not have modify rights
				if ((editButton != null) && (deleteButton != null))
				{
					if (this.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
					{
						editButton.Enabled = false;
						deleteButton.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
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
			if (this.GSTDataGrid.EditItemIndex > -1)
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
		}

		/// <summary>
		///    This is the main entry point for the GST Summary page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.gstCollection = FMChannelHelper.MakeCall<IGoodsAndServices, GoodsAndServicesTaxDOCollection>(
																	 x =>
																	 x.GetAll(this.security)
																);

			if (this.Page.IsPostBack == false)
			{
				if (this.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
				{
					this.EnableControls(false);
				}

				this.UpdateView();
			}
		}

		/// <summary>
		///    This method will create a new GST object, place it into session and redirect
		///    the adding to the GST detail page.
		/// </summary>
		private void AddNewGst()
		{
			var newGst = new GoodsAndServicesTaxDO
				             {
					             IdentityGuid	= Guid.Empty,
					             GstDate		= DateTimeOffset.Now,
					             SiteGuid		= this.security.SiteGuid,
					             SiteID			= this.security.SiteID
				             };

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT);
			}

			this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT, newGst);
			this.Redirect("TaxGstDetailForm.aspx?Mode=Add");
		}

		/// <summary>
		///    This method enables controls based on the input.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			this.AddButtonTop.Enabled = enable;
			this.AddButtonBottom.Enabled = enable;
		}

		/// <summary>
		///    This method will handle the deletion of an item in the grid.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void GstDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			// Find the GST to delete
			GoodsAndServicesTaxDO selectedGst = null;
			Guid selectedGuid = Guid.Parse(e.CommandArgument.ToString());

			foreach (GoodsAndServicesTaxDO gst in this.gstCollection)
			{
				if (gst.IdentityGuid == selectedGuid)
				{
					selectedGst = gst;
					break;
				}
			}

			try
			{
				FMChannelHelper.MakeCall<IGoodsAndServices>(
																	 x =>
																	 x.Remove(selectedGst, this.security)
																);

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			// Now remove the selected GST from the collection
			this.gstCollection.RemoveByIdentityGuid(selectedGst);

			this.UpdateView();
		}

		/// <summary>
		///    This method handles the edit event. It will redirect the item being edit to the
		///    GST detail page.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void GstDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			Guid gstItemGuid = Guid.Parse(e.CommandArgument.ToString());
			int selectedIndex = e.Item.ItemIndex;
			GoodsAndServicesTaxDO selectedGst = null;

			if ((selectedIndex >= 0) && (selectedIndex < this.gstCollection.Count))
			{
				// This is an existing GST so find it
				foreach (GoodsAndServicesTaxDO gst in this.gstCollection)
				{
					if (gst.IdentityGuid == gstItemGuid)
					{
						selectedGst = gst;
						break;
					}
				}
			}

			if (selectedGst == null)
			{
				string errMsg = "GST Selected object not found.";
				this.ErrorHandler(new Exception(errMsg));
			}
			else
			{
				if (this.Page.Session[PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT] != null)
				{
					this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT);
				}

				this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT, selectedGst);
				this.Redirect("TaxGstDetailForm.aspx?Mode=edit");
			}
		}

		/// <summary>
		///    This method will handle the page index change. It will update the view to the
		///    new page.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void GstDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.GSTDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.GSTDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		///    This method will initialize event handles.
		/// </summary>
		private void InitializeComponent()
		{
			this.GSTDataGrid.EditCommand += this.GstDataGridEditCommand;
			this.GSTDataGrid.DeleteCommand += this.GstDataGridDeleteCommand;
			this.GSTDataGrid.ItemDataBound += this.GstDataGridItemDataBound;
			this.GSTDataGrid.PageIndexChanged += this.GstDataGridPageIndexChanged;
		}

		/// <summary>
		///    This method updates the GST Summary grid with new data.
		/// </summary>
		private void UpdateView()
		{
			this.GridSizeDropDown.SetPageSize(this.GSTDataGrid, this.gstCollection.Count);
			this.GSTDataGrid.DataSource = this.gstCollection;

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.security,
																			this.security.SiteGuid,
																			getMemberSites: true,
																			getSchedulesAndProcessVariables: true,
																			bGetAssociatedAliases: true)
																	);

			DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();
			this.dateFormat = "{0:" + dateTimeFormatInfo.ShortDatePattern + "}"; // used in aspx 
			this.GSTDataGrid.DataBind();
		}

		#endregion
	}
}